using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;

namespace MadPay724.Repo.Repositories.MainDB.Interface
{
   public  interface IDocumentRepository : IRepository<Document>
    {
        Task<int> DocumentCountAsync(string userId);
    }
}
