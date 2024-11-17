using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.Repository
{
    public class ConsultatieRepository: Repository<Consultatie>, IConsultatieRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public ConsultatieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #endregion

        #region Methods

        public void Update(Consultatie consultatie)
        {
            _db.Consultatie.Update(consultatie);
        }
        #endregion
    }
}
