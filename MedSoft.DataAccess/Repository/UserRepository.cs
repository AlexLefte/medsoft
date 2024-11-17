using MedSoft.DataAccess.Data;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        #region Fields
        ApplicationDbContext _db;
        #endregion

        #region Constructor
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #endregion
    }
}
