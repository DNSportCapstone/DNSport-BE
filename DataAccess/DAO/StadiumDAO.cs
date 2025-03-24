using BusinessObject.Models;

namespace DataAccess.DAO
{
    public class StadiumDAO
    {
        public async Task<int> DisableStadium(int id, string status)
        {
            using var context = new Db12353Context();
            
            var stadium = await context.Stadiums.FindAsync(id);
            if (stadium == null)
            {
                return 0;
            }
            stadium.Status = status;
            await context.SaveChangesAsync();
            return stadium.StadiumId;
        }
    }
}
