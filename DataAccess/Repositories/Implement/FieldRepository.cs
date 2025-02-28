using BusinessObject.Models;
using DataAccess.Model;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implement
{
    public class FieldRepository : IFieldRepository
    {
        private Db12353Context _dbcontext = new();
        public async Task<List<FieldHomeModel>> GetFieldHomeData()
        {
            var result = await (from f in _dbcontext.Fields
                                join stadium in _dbcontext.Stadiums on f.StadiumId equals stadium.StadiumId
                                join s in _dbcontext.Sports on f.SportId equals s.SportId
                                join i in _dbcontext.Images on f.FieldId equals i.FieldId
                                select new FieldHomeModel
                                {
                                    FieldId = f.FieldId,
                                    FieldName = stadium.StadiumName ?? string.Empty,
                                    SportName = s.SportName ?? string.Empty,
                                    ImagePath = i.Url ?? string.Empty
                                }).AsNoTracking().ToListAsync();
            return result;
        }
    }
}
