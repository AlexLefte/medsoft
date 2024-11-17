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
    public class AdministratorRepository: Repository<Administrator>, IAdministratorRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public AdministratorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #endregion

        #region Methods

        public void Update(Administrator administrator)
        {
            var administratorFromDb = _db.Administrator.FirstOrDefault(admin =>
                admin.AdministratorID == administrator.AdministratorID);
            if (administratorFromDb != null)
            {
                administratorFromDb.Nume = administrator.Nume;
                administratorFromDb.Prenume = administrator.Prenume;
                administratorFromDb.CNP = administrator.CNP;
                administratorFromDb.Adresa = administrator.Adresa;
                _db.Administrator.Update(administratorFromDb);
            }
        }
        #endregion
    }
}
