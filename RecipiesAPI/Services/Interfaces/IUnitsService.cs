using RecipiesAPI.Models.DTO.Responce;
using RecipiesAPI.Models.DTO.Response;

namespace RecipiesAPI.Services.Interfaces
{
    public interface IUnitsService
    {
        Task<List<UnitsResponse>> GetAllUnitsAsync();
    }
}
