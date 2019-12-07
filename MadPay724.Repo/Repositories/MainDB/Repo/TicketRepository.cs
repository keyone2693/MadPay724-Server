using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.MainDB.Interface;
using Microsoft.EntityFrameworkCore;

namespace MadPay724.Repo.Repositories.MainDB.Repo
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly DbContext _db;
        public TicketRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (Main_MadpayDbContext)_db;
        }
    }
}
