
namespace DataAccess.Services.Interfaces
{
    public interface IStadiumService
    {
        Task<int> DisableStadium(int id,string status);
    }
}
