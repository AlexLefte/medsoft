using MedSoft.Models;


namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface IMedicRepository : IRepository<Medic>
    {
        void Update(Medic medic);
    }
}
