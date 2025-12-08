using Microsoft.EntityFrameworkCore;
using RecipiesAPI.Data;

namespace RecipiesAPI.Services
{
    /// <summary>
    /// Background service that periodically cleans up expired and revoked refresh tokens
    /// </summary>
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24); // Run once per day
        private readonly TimeSpan _tokenRetentionPeriod = TimeSpan.FromDays(30); // Keep tokens for 30 days after expiry

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Cleanup Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredTokens(stoppingToken);
                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    _logger.LogInformation("Token Cleanup Service is stopping");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired tokens");
                    // Wait a bit before retrying to avoid hammering the database if there's an issue
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task CleanupExpiredTokens(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoffDate = DateTime.UtcNow.Subtract(_tokenRetentionPeriod);

            _logger.LogInformation("Starting token cleanup. Removing tokens expired or revoked before {CutoffDate}", cutoffDate);

            // Delete tokens that are either:
            // 1. Expired more than retention period ago, OR
            // 2. Revoked more than retention period ago
            var deletedCount = await context.Token
                .Where(t => t.ExpiryDate < cutoffDate || (t.RevokedAt != null && t.RevokedAt < cutoffDate))
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedCount > 0)
            {
                _logger.LogInformation("Successfully cleaned up {Count} expired/revoked refresh tokens", deletedCount);
            }
            else
            {
                _logger.LogDebug("No expired tokens found for cleanup");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Cleanup Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
