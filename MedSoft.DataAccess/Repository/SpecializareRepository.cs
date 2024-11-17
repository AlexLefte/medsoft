using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MedSoft.DataAccess.Repository
{
    public class SpecializareRepository : Repository<Specializare>, ISpecializareRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public SpecializareRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        #endregion
        #region Methods
        
        public int CountMedici(long id)
        {
            var entityCount = _db.MedicCount
                .FromSqlRaw("EXEC NumaraMediciDupaSpecializareID @p0", id).AsEnumerable()
                .FirstOrDefault()?.NumarMedici ?? 0;
            return entityCount;
        }

        public void Update(Specializare specializare)
        {
            _db.Specializare.Update(specializare);
        }
        #endregion
    }
}
