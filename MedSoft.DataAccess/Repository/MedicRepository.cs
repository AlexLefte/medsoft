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
    public class MedicRepository: Repository<Medic>, IMedicRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public MedicRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #endregion

        #region Methods
        public void Update(Medic medic)
        {
            var medicFromDb = _db.Medic.FirstOrDefault(med =>
                med.MedicID == medic.MedicID);
            if (medicFromDb != null)
            {
                medicFromDb.NumeMedic = medic.NumeMedic;
                medicFromDb.PrenumeMedic = medic.PrenumeMedic;
                medicFromDb.PretConsultatie = medic.PretConsultatie;
                medicFromDb.SpecializareID = medic.SpecializareID;
                // Update image URL only if it is not null
                if (medicFromDb.ImageUrl != null)
                {
                    medicFromDb.ImageUrl = medic.ImageUrl;
                }
                _db.Medic.Update(medicFromDb);
            }
        }
        #endregion
    }
}
