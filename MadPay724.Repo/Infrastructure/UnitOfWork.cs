using MadPay724.Repo.Repositories.FinancialDB.Interface;
using MadPay724.Repo.Repositories.FinancialDB.Repo;
using MadPay724.Repo.Repositories.MainDB.Interface;
using MadPay724.Repo.Repositories.MainDB.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using MadPay724.Common.Helpers.Utilities.Extensions;

namespace MadPay724.Repo.Infrastructure
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        #region ctor
        protected readonly DbContext _db;
        public UnitOfWork(TContext tContext)
        {
            _db = tContext;
        }
        #endregion
        #region privateMainrepository
        private IUserRepository userRepository;
        public IUserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(_db);
                }
                return userRepository;
            }
        }


        private IPhotoRepository photoRepository;
        public IPhotoRepository PhotoRepository
        {
            get
            {
                if (photoRepository == null)
                {
                    photoRepository = new PhotoRepository(_db);
                }
                return photoRepository;
            }
        }

        private ISettingRepository settingRepository;
        public ISettingRepository SettingRepository
        {
            get
            {
                if (settingRepository == null)
                {
                    settingRepository = new SettingRepository(_db);
                }
                return settingRepository;
            }
        }


        private IRoleRepository roleRepository;
        public IRoleRepository RoleRepository
        {
            get
            {
                if (roleRepository == null)
                {
                    roleRepository = new RoleRepository(_db);
                }
                return roleRepository;
            }
        }

        private ITokenRepository tokenRepository;
        public ITokenRepository TokenRepository
        {
            get
            {
                if (tokenRepository == null)
                {
                    tokenRepository = new TokenRepository(_db);
                }
                return tokenRepository;
            }
        }

        private INotificationRepository notificationRepository;
        public INotificationRepository NotificationRepository
        {
            get
            {
                if (notificationRepository == null)
                {
                    notificationRepository = new NotificationRepository(_db);
                }
                return notificationRepository;
            }
        }

        private IBankCardRepository bankCardRepository;
        public IBankCardRepository BankCardRepository
        {
            get
            {
                if (bankCardRepository == null)
                {
                    bankCardRepository = new BankCardRepository(_db);
                }
                return bankCardRepository;
            }
        }

        private IDocumentRepository documentRepository;
        public IDocumentRepository DocumentRepository
        {
            get
            {
                if (documentRepository == null)
                {
                    documentRepository = new DocumentRepository(_db);
                }
                return documentRepository;
            }
        }

        private IWalletRepository walletRepository;
        public IWalletRepository WalletRepository
        {
            get
            {
                if (walletRepository == null)
                {
                    walletRepository = new WalletRepository(_db);
                }
                return walletRepository;
            }
        }

        private ITicketRepository ticketRepository;
        public ITicketRepository TicketRepository
        {
            get
            {
                if (ticketRepository == null)
                {
                    ticketRepository = new TicketRepository(_db);
                }
                return ticketRepository;
            }
        }

        private ITicketContentRepository ticketContentRepository;
        public ITicketContentRepository TicketContentRepository
        {
            get
            {
                if (ticketContentRepository == null)
                {
                    ticketContentRepository = new TicketContentRepository(_db);
                }
                return ticketContentRepository;
            }
        }

        private IGateRepository gateRepository;
        public IGateRepository GateRepository
        {
            get
            {
                if (gateRepository == null)
                {
                    gateRepository = new GateRepository(_db);
                }
                return gateRepository;
            }
        }

        private IEasyPayRepository easyPayRepository;
        public IEasyPayRepository EasyPayRepository
        {
            get
            {
                if (easyPayRepository == null)
                {
                    easyPayRepository = new EasyPayRepository(_db);
                }
                return easyPayRepository;
            }
        }

        private IBlogRepository blogRepository;
        public IBlogRepository BlogRepository
        {
            get
            {
                if (blogRepository == null)
                {
                    blogRepository = new BlogRepository(_db);
                }
                return blogRepository;
            }
        }

        private IBlogGroupRepository blogGroupRepository;
        public IBlogGroupRepository BlogGroupRepository
        {
            get
            {
                if (blogGroupRepository == null)
                {
                    blogGroupRepository = new BlogGroupRepository(_db);
                }
                return blogGroupRepository;
            }
        }
        private IVerificationCodeRepository verificationCodeRepository;
        public IVerificationCodeRepository VerificationCodeRepository
        {
            get
            {
                if (verificationCodeRepository == null)
                {
                    verificationCodeRepository = new VerificationCodeRepository(_db);
                }
                return verificationCodeRepository;
            }
        }
        #endregion

        #region privateFinancialrepository
        private IEntryRepository entryRepository;
        public IEntryRepository EntryRepository
        {
            get
            {
                if (entryRepository == null)
                {
                    entryRepository = new EntryRepository(_db);
                }
                return entryRepository;
            }
        }
        private IFactorRepository factorRepository;
        public IFactorRepository FactorRepository
        {
            get
            {
                if (factorRepository == null)
                {
                    factorRepository = new FactorRepository(_db);
                }
                return factorRepository;
            }
        }
        #endregion
        #region save
        public bool Save()
        {
            _cleanStrings();
            if (_db.SaveChanges() > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> SaveAsync()
        {
            _cleanStrings();
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
        private void _cleanStrings()
        {
            var changedEntities = _db.ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Added || p.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.CleanString();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
            {

            }
        }
        #endregion


        #region dispose
        private bool disposed = false;




        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
        #endregion
    }
}
