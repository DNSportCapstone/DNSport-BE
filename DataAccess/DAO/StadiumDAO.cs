using BusinessObject.Models;

namespace DataAccess.DAO
{
    public class StadiumDAO
    {
        private readonly Db12353Context _dbcontext;
        public StadiumDAO(Db12353Context dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<int> DisableStadium(int id, string status)
        {
            var stadium = await _dbcontext.Stadiums.FindAsync(id);
            if (stadium == null)
            {
                return 0;
            }
            stadium.Status = status;
            await _dbcontext.SaveChangesAsync();
            return stadium.StadiumId;
        }
    }
}
