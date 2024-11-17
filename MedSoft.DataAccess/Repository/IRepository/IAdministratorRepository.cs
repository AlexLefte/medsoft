using MedSoft.Models;

namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface IAdministratorRepository : IRepository<Administrator>
    {
        void Update(Administrator administrator);
    }
}
