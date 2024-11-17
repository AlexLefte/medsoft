using MedSoft.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ApplicationDbContext DbContext { get; }
        IAdministratorRepository Administrator { get; }
        IMedicRepository Medic { get; }
        IPacientRepository Pacient { get; }
        ISpecializareRepository Specializare { get; }
        IMedicamenteRepository Medicamente { get; }
        IConsultatieRepository Consultatie { get; }
        IUserRepository User { get; }
        void Save();
    }
}
