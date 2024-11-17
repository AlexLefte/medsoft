using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;

namespace MedSoft.DataAccess.Repository
{
    public class PacientRepository: Repository<Pacient>, IPacientRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public PacientRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #endregion

        #region Methods

        public void Update(Pacient pacient)
        {
            var pacientFromDb = _db.Pacient.FirstOrDefault(p =>
                p.PacientID == pacient.PacientID);
            if (pacientFromDb != null)
            {
                pacientFromDb.NumePacient = pacient.NumePacient;
                pacientFromDb.PrenumePacient = pacient.PrenumePacient;
                pacientFromDb.Adresa = pacient.Adresa;
                pacientFromDb.Asigurare = pacient.Asigurare;
                pacientFromDb.CNP = pacient.CNP;
                _db.Pacient.Update(pacientFromDb);
            }
        }
        #endregion
    }
}
