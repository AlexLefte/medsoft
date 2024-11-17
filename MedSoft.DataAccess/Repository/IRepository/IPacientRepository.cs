using MedSoft.Models;

namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface IPacientRepository: IRepository<Pacient>
    {
        void Update(Pacient pacient);
    }
}
