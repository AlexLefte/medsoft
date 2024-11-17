using MedSoft.Models;

namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface IMedicamenteRepository: IRepository<Medicamente>
    {
        void Update(Medicamente medicament);
    }
}
