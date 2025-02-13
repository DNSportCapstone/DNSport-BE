using BusinessObject.Models;
using DataAccess.Interface;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implement
{
    public class FieldRepository : IFieldRepository
    {
        private Db12353Context _dbcontext = new ();
        public async Task<List<FieldHomeModel>> GetFieldHomeData()
        {
            var result = await (from f in _dbcontext.Fields
                                join stadium in _dbcontext.Stadium on f.StadiumId equals stadium.StadiumId
                                join s in _dbcontext.Sports on f.SportId equals s.SportId
                                join i in _dbcontext.Images on f.FieldId equals i.FieldId
                                select new FieldHomeModel
                                {
                                    FieldId = f.FieldId,
                                    FieldName = stadium.StadiumName,
                                    SportName = s.SportName,
                                    ImagePath = i.Url
                                }).AsNoTracking().ToListAsync();
            return result;
        }
    }
}
