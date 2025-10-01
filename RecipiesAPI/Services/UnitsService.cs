using AutoMapper;
using RecipiesAPI.Data;
using RecipiesAPI.Models.DTO.Response;
using RecipiesAPI.Services.Interfaces;

namespace RecipiesAPI.Services
{
    public class UnitsService : IUnitsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UnitsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<List<UnitsResponse>> GetAllUnitsAsync()
        {
            var units = _context.Units.ToList();
            var response = _mapper.Map<List<UnitsResponse>>(units);
            return Task.FromResult(response);
        }
    }
}
