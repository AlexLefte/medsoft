using MedSoft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface ISpecializareRepository : IRepository<Specializare>
    {
        int CountMedici(long id);
        void Update(Specializare specializare);
    }
}
