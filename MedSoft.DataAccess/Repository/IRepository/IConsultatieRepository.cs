using MedSoft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.Repository.IRepository
{
    public interface IConsultatieRepository : IRepository<Consultatie>
    {
        void Update(Consultatie consultatie);
    }
}
