using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Wallet.Interface;

namespace MadPay724.Services.Site.Admin.Wallet.Service
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUtilities _utilities;

        public WalletService(IUnitOfWork<Main_MadPayDbContext> dbContext, IUtilities utilities)
        {
            _db = dbContext;
            _utilities = utilities;
        }

        public async Task<bool> CheckInventoryAsync(int cost, string walletId)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                if (wallet.Inventory >= cost)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public async Task<ReturnMessage> IncreaseInventoryAsync(int cost, string walletId, bool isFactor = false)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Inventory += cost;
                if (isFactor)
                {
                    wallet.InterMoney += cost;
                }
                _db.WalletRepository.Update(wallet);
                if (await _db.SaveAsync())
                {
                    return new ReturnMessage()
                    {
                        status = true
                    };
                }
                else
                {
                    return new ReturnMessage()
                    {
                        status = false,
                        message = "خطا در افزایش موجودی"
                    };
                }
            }
            else
            {
                return new ReturnMessage()
                {
                    status = false,
                    message = "کیف پولی وجود ندارد"
                };
            }
        }

        public async Task<ReturnMessage> DecreaseInventoryAsync(int cost, string walletId, bool isFactor = false)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                if (wallet.Inventory >= cost)
                {
                    wallet.Inventory -= cost;
                    if (isFactor)
                    {
                        wallet.InterMoney -= cost;
                    }
                    _db.WalletRepository.Update(wallet);
                    if (await _db.SaveAsync())
                    {
                        return new ReturnMessage()
                        {
                            status = true
                        };
                    }
                    else
                    {
                        return new ReturnMessage()
                        {
                            status = false,
                            message = "خطا در کاهش موجودی"
                        };
                    }
                }
                else
                {
                    return new ReturnMessage()
                    {
                        status = false,
                        message = "کیف پول انتخابی موجودی کافی ندارد"
                    };
                }
            }
            else
            {
                return new ReturnMessage()
                {
                    status = false,
                    message = "کیف پولی وجود ندارد"
                };
            }
        }

        public async Task<ReturnMessage> EntryIncreaseInventoryAsync(int cost, string walletId)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Inventory += cost;
                wallet.OnExitMoney += cost;
                wallet.ExitMoney -= cost;

                _db.WalletRepository.Update(wallet);
                if (await _db.SaveAsync())
                {
                    return new ReturnMessage()
                    {
                        status = true
                    };
                }
                else
                {
                    return new ReturnMessage()
                    {
                        status = false
                    };
                }
            }
            else
            {
                return new ReturnMessage()
                {
                    status = false
                };
            }
        }

        public async Task<ReturnMessage> EntryDecreaseInventoryAsync(int cost, string walletId)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                if (wallet.Inventory >= cost)
                {
                    wallet.Inventory -= cost;
                    wallet.OnExitMoney -= cost;
                    wallet.ExitMoney += cost;

                    _db.WalletRepository.Update(wallet);
                    if (await _db.SaveAsync())
                    {
                        return new ReturnMessage()
                        {
                            status = true
                        };
                    }
                    else
                    {
                        return new ReturnMessage()
                        {
                            status = false
                        };
                    }
                }
                else
                {
                    return new ReturnMessage()
                    {
                        status = false
                    };
                }
            }
            else
            {
                return new ReturnMessage()
                {
                    status = false
                };
            }
        }

        public async Task<ReturnMessage> EntryIncreaseOnExitMoneyAsync(int cost, string walletId)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.OnExitMoney += cost;

                _db.WalletRepository.Update(wallet);
                if (await _db.SaveAsync())
                {
                    return new ReturnMessage()
                    {
                        status = true
                    };
                }
                else
                {
                    return new ReturnMessage()
                    {
                        status = false
                    };
                }
            }
            else
            {
                return new ReturnMessage()
                {
                    status = false
                };
            }
        }

        public async Task<ReturnMessage> EntryDecreaseOnExitMoneyAsync(int cost, string walletId)
        {
            var wallet = await _db.WalletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.OnExitMoney -= cost;

                _db.WalletRepository.Update(wallet);
                if (await _db.SaveAsync())
                {
                    return new ReturnMessage()
                    {
                        status = true
                    };
                }
                else
                {
                    return new ReturnMessage()
                    {
                        status = false
                    };
                }
            }
            else
            {
                return new ReturnMessage()
                {
                    status = false
                };
            }
        }
    }
}
