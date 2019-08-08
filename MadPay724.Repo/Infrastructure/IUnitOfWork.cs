using MadPay724.Repo.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MadPay724.Repo.Infrastructure
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext:DbContext
    {
        IUserRepository UserRepository { get;}
        IPhotoRepository PhotoRepository { get;}
        ISettingRepository SettingRepository { get; }
        IRoleRepository RoleRepository { get; }
        ITokenRepository TokenRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IBankCardRepository BankCardRepository { get; }
        IDocumentRepository DocumentRepository { get; }
        bool Save();
        Task<bool> SaveAsync();

    }
}
