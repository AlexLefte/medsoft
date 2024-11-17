using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;

namespace MedSoft.DataAccess.Repository
{
    public class MedicamenteRepository: Repository<Medicamente>, IMedicamenteRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public MedicamenteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #endregion

        #region Methods

        public void Update(Medicamente medicament)
        {
            var medicamentFromDb = _db.Medicamente.FirstOrDefault(m =>
                m.MedicamentID == medicament.MedicamentID);
            if (medicamentFromDb != null)
            {
                medicamentFromDb.Denumire = medicament.Denumire;
                medicamentFromDb.ImageUrl = medicament.ImageUrl;
                _db.Medicamente.Update(medicamentFromDb);
            }
        }
        #endregion
    }
}
