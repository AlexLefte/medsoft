using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Properties
        public ApplicationDbContext DbContext { get => _db; }
        public IMedicRepository Medic {  get; private set; }
        public IPacientRepository Pacient { get; private set; }
        public IAdministratorRepository Administrator { get; private set; }
        public ISpecializareRepository Specializare { get; private set; }
        public IMedicamenteRepository Medicamente { get; private set; }
        public IConsultatieRepository Consultatie { get; private set; }
        public IUserRepository User { get; private set; }
        #endregion

        #region Constructor
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Medic = new MedicRepository(_db);
            Pacient = new PacientRepository(_db);
            Administrator = new AdministratorRepository(_db);
            Specializare = new SpecializareRepository(_db);
            Medicamente = new MedicamenteRepository(_db);
            Consultatie = new ConsultatieRepository(_db);
            User = new UserRepository(_db);
        }
        #endregion

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
