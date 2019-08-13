using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Common.ErrorAndMessage;

namespace MadPay724.Services.Site.Admin.Wallet.Interface
{
    public interface IWalletService
    {
        Task<bool> CheckInventoryAsync(int cost, string walletId);
        Task<ReturnMessage> DecreaseInventoryAsync(int cost, string walletId);
        Task<ReturnMessage> IncreaseInventoryAsync(int cost, string walletId);
    }
}
