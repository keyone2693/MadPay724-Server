using MadPay724.Common.Enums;
using MadPay724.Common.Enums.Common;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Data.Models.MainDB;
using MadPay724.Data.Models.MainDB.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MadPay724.Common.Helpers.Utilities.Extensions
{
    public static class Expressions
    {
        public static Expression<Func<Blog, bool>> ToBlogExpression(this string Filter,
    bool isAdmin, string id = "")
        {
            if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
            {
                Expression<Func<Blog, bool>> exp;
                if (isAdmin)
                {
                    exp = null;
                }
                else
                {
                    exp = p => p.UserId == id;
                }
                return exp;
            }
            else
            {
                Expression<Func<Blog, bool>> exp;
                if (isAdmin)
                {
                    exp =
                                p => p.Id.Contains(Filter) ||
                                p.DateModified.ToString().Contains(Filter) ||
                                p.PicAddress.Contains(Filter) ||
                                p.SummerText.Contains(Filter) ||
                                p.Tags.Contains(Filter) ||
                                p.Text.Contains(Filter) ||
                                p.Title.Contains(Filter) ||
                                p.BlogGroup.Name.Contains(Filter);
                }
                else
                {
                    exp =

                               p => (p.Id.Contains(Filter) ||
                               p.DateModified.ToString().Contains(Filter) ||
                               p.PicAddress.Contains(Filter) ||
                               p.SummerText.Contains(Filter) ||
                               p.Tags.Contains(Filter) ||
                               p.Text.Contains(Filter) ||
                               p.Title.Contains(Filter) ||
                               p.BlogGroup.Name.Contains(Filter)) &&
                               p.UserId == id;
                }


                return exp;
            }

        }
        public static Expression<Func<User, bool>> ToUserExpression(this string Filter,
            bool isAdmin, string id = "")
        {
            if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
            {
                return null;
            }
            else
            {
                Expression<Func<User, bool>> exp =
                                 p => p.Id.Contains(Filter) ||
                                 p.UserName.Contains(Filter) ||
                                 p.Name.Contains(Filter) ||
                                 p.Id.Contains(Filter) ||
                                 p.Wallets.Any(s => s.Name.Contains(Filter)) ||
                                 p.Wallets.Any(s => s.Inventory.ToString().Contains(Filter)) ||
                                 p.Wallets.Any(s => s.InterMoney.ToString().Contains(Filter)) ||
                                 p.Wallets.Any(s => s.ExitMoney.ToString().Contains(Filter));

                return exp;
            }

        }
        public static Expression<Func<Wallet, bool>> ToWalletExpression(this string Filter,
           bool isAdmin, string id = "")
        {
            if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
            {
                return null;
            }
            else
            {
                Expression<Func<Wallet, bool>> exp =
                                p => p.Id.Contains(Filter) ||
                                p.Code.ToString().Contains(Filter) ||
                                p.Name.Contains(Filter) ||
                                p.Inventory.ToString().Contains(Filter) ||
                                p.InterMoney.ToString().Contains(Filter) ||
                                p.ExitMoney.ToString().Contains(Filter) ||
                                p.OnExitMoney.ToString().Contains(Filter) ||

                                p.User.Name.Contains(Filter) ||
                                p.User.UserName.Contains(Filter) ||
                                p.Gates.Any(s => s.WebsiteName.Contains(Filter)) ||
                                p.Gates.Any(s => s.WebsiteUrl.Contains(Filter));

                return exp;
            }

        }
        public static Expression<Func<BankCard, bool>> ToBankCardExpression(this string Filter,
         bool isAdmin, string id = "")
        {
            if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
            {
                return null;
            }
            else
            {
                Expression<Func<BankCard, bool>> exp =
                                p => p.Id.Contains(Filter) ||
                                p.OwnerName.Contains(Filter) ||
                                p.BankName.Contains(Filter) ||
                                p.Shaba.Contains(Filter) ||
                                p.CardNumber.Contains(Filter) ||
                                p.HesabNumber.Contains(Filter) ||
                                p.ExpireDateMonth.Contains(Filter) ||
                                p.ExpireDateYear.Contains(Filter) ||
                                p.User.Name.Contains(Filter) ||
                                p.User.UserName.Contains(Filter);

                return exp;
            }

        }

        public static Expression<Func<Entry, bool>> ToEntryExpression(this string Filter,
        EntryState state, SearchIdEnums searchIdType, string id = "")
        {
            switch (searchIdType)
            {
                case SearchIdEnums.None:
                    if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
                    {
                        switch (state)
                        {
                            case EntryState.All:
                                return null;
                            case EntryState.Approve:
                                return p => !p.IsApprove && !p.IsPardakht && !p.IsReject;
                            case EntryState.Pardakht:
                                return p => p.IsApprove && !p.IsPardakht && !p.IsReject;
                            case EntryState.Archive:
                                return p => p.IsReject || p.IsPardakht;
                            default:
                                return null;
                        }
                    }
                    else
                    {
                        switch (state)
                        {
                            case EntryState.All:
                                return p => p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter);
                            case EntryState.Approve:
                                {
                                    return p => (
                                    p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter)
                                        )
                                        &&
                                        (
                                        !p.IsApprove && !p.IsPardakht && !p.IsReject
                                        );
                                }
                            case EntryState.Pardakht:
                                {
                                    return p => (
                                    p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter)
                                        )
                                        &&
                                        (
                                        p.IsApprove && !p.IsPardakht && !p.IsReject
                                        );
                                }
                            case EntryState.Archive:
                                {
                                    return p => (
                                    p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter)
                                        )
                                        &&
                                        (
                                        p.IsReject || p.IsPardakht
                                        );
                                }
                            default:
                                return p => p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter);
                        }
                    }
                case SearchIdEnums.BankCard:
                    if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
                    {
                        switch (state)
                        {
                            case EntryState.All:
                                return p => p.BankCardId == id;
                            case EntryState.Approve:
                                return p => p.BankCardId == id && !p.IsApprove && !p.IsPardakht && !p.IsReject;
                            case EntryState.Pardakht:
                                return p => p.BankCardId == id && p.IsApprove && !p.IsPardakht && !p.IsReject;
                            case EntryState.Archive:
                                return p => p.BankCardId == id && p.IsReject || p.IsPardakht;
                            default:
                                return null;
                        }
                    }
                    else
                    {
                        switch (state)
                        {
                            case EntryState.All:
                                return p => p.BankCardId == id &&
                                        (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter));
                            case EntryState.Approve:
                                {
                                    return p => (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter))
                                        &&
                                        (p.BankCardId == id && !p.IsApprove && !p.IsPardakht && !p.IsReject);
                                }
                            case EntryState.Pardakht:
                                {
                                    return p => (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter))
                                        &&
                                        (p.BankCardId == id && p.IsApprove && !p.IsPardakht && !p.IsReject);
                                }
                            case EntryState.Archive:
                                {
                                    return p => (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter)) &&
                                        p.BankCardId == id &&
                                        (p.IsReject || p.IsPardakht);
                                }
                            default:
                                return p => p.BankCardId == id &&
                                        (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter));
                        }
                    }
                case SearchIdEnums.Wallet:
                    if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
                    {
                        switch (state)
                        {
                            case EntryState.All:
                                return p => p.WalletId == id;
                            case EntryState.Approve:
                                return p => p.WalletId == id && !p.IsApprove && !p.IsPardakht && !p.IsReject;
                            case EntryState.Pardakht:
                                return p => p.WalletId == id && p.IsApprove && !p.IsPardakht && !p.IsReject;
                            case EntryState.Archive:
                                return p => p.WalletId == id && p.IsReject || p.IsPardakht;
                            default:
                                return null;
                        }
                    }
                    else
                    {
                        switch (state)
                        {
                            case EntryState.All:
                                return p => p.WalletId == id &&
                                        (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter));
                            case EntryState.Approve:
                                {
                                    return p => (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter))
                                        &&
                                        (p.WalletId == id && !p.IsApprove && !p.IsPardakht && !p.IsReject);
                                }
                            case EntryState.Pardakht:
                                {
                                    return p => (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter))
                                        &&
                                        (p.WalletId == id && p.IsApprove && !p.IsPardakht && !p.IsReject);
                                }
                            case EntryState.Archive:
                                {
                                    return p => (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter)) &&
                                        p.WalletId == id &&
                                        (p.IsReject || p.IsPardakht);
                                }
                            default:
                                return p => p.WalletId == id &&
                                        (p.Id.Contains(Filter) ||
                                        p.TextForUser.Contains(Filter) ||
                                        p.BankName.Contains(Filter) ||
                                        p.OwnerName.Contains(Filter) ||
                                        p.Shaba.Contains(Filter) ||

                                        p.CardNumber.Contains(Filter) ||
                                        p.WalletName.Contains(Filter) ||
                                        p.UserId.Contains(Filter) ||
                                        p.BankCardId.Contains(Filter) ||
                                        p.WalletId.Contains(Filter));
                        }
                    }

                default:
                    return null;
            }

        }
        public static Expression<Func<Factor, bool>> ToFactorExpression(this FactorPaginationDto factorPaginationDto,
            SearchIdEnums searchIdType, string id = "")
        {
            Expression<Func<Factor, bool>> exp = null;
            switch (searchIdType)
            {
                case SearchIdEnums.None:
                    if (!string.IsNullOrEmpty(factorPaginationDto.Filter) && !string.IsNullOrWhiteSpace(factorPaginationDto.Filter))
                    {
                        Expression<Func<Factor, bool>> tempExp =
                                        p => p.Id.Contains(factorPaginationDto.Filter) ||
                                        p.UserName.Contains(factorPaginationDto.Filter) ||
                                        p.GiftCode.Contains(factorPaginationDto.Filter) ||
                                        p.Price.ToString().Contains(factorPaginationDto.Filter) ||
                                        p.EndPrice.ToString().Contains(factorPaginationDto.Filter) ||
                                        p.RefBank.Contains(factorPaginationDto.Filter) ||
                                        p.EnterMoneyWalletId.Contains(factorPaginationDto.Filter) ||
                                        p.UserId.Contains(factorPaginationDto.Filter) ||
                                        p.GateId.Contains(factorPaginationDto.Filter);

                        exp = CombineExpressions.CombiningExpressions<Factor>(exp, tempExp, ExpressionsTypeEnum.And);
                    }
                    //
                    if (factorPaginationDto.Status == 1)
                    {
                        Expression<Func<Factor, bool>> tempExp =
                                        p => p.Status;
                        exp = CombineExpressions.CombiningExpressions<Factor>(exp, tempExp, ExpressionsTypeEnum.And);
                    }
                    if (factorPaginationDto.Status == 2)
                    {
                        Expression<Func<Factor, bool>> tempExp =
                                        p => !p.Status;
                        exp = CombineExpressions.CombiningExpressions<Factor>(exp, tempExp, ExpressionsTypeEnum.And);
                    }
                    //
                    if (factorPaginationDto.FactorType > 0 && factorPaginationDto.FactorType < 5)
                    {
                        Expression<Func<Factor, bool>> tempExp =
                                        p => p.Kind == factorPaginationDto.FactorType;
                        exp = CombineExpressions.CombiningExpressions<Factor>(exp, tempExp, ExpressionsTypeEnum.And);
                    }
                    //
                    if (factorPaginationDto.Bank > 0 && factorPaginationDto.Bank < 10)
                    {
                        Expression<Func<Factor, bool>> tempExp =
                                        p => p.Bank == factorPaginationDto.Bank;
                        exp = CombineExpressions.CombiningExpressions<Factor>(exp, tempExp, ExpressionsTypeEnum.And);
                    }
                    //
                    Expression<Func<Factor, bool>> priceExp =
                                    p => p.Price >= factorPaginationDto.MinPrice && p.Price <= factorPaginationDto.MaxPrice;
                    exp = CombineExpressions.CombiningExpressions<Factor>(exp, priceExp, ExpressionsTypeEnum.And);
                    //
                    Expression<Func<Factor, bool>> datExp =
                                    p => p.Price >= factorPaginationDto.MinPrice && p.Price <= factorPaginationDto.MaxPrice;
                    exp = CombineExpressions.CombiningExpressions<Factor>(exp, datExp, ExpressionsTypeEnum.And);

                case SearchIdEnums.Wallet:
                    if (string.IsNullOrEmpty(factorPaginationDto.Filter) || string.IsNullOrWhiteSpace(factorPaginationDto.Filter))
                    {
                        Expression<Func<Factor, bool>> exp =
                                        p => p.EnterMoneyWalletId == id;
                        return exp;
                    }
                    else
                    {
                        Expression<Func<Factor, bool>> exp =
                                        p => p.EnterMoneyWalletId == id &&
                                        (p.Id.Contains(factorPaginationDto.Filter) ||
                                        p.UserName.Contains(factorPaginationDto.Filter) ||
                                        p.GiftCode.Contains(factorPaginationDto.Filter) ||
                                        p.Price.ToString().Contains(factorPaginationDto.Filter) ||
                                        p.EndPrice.ToString().Contains(factorPaginationDto.Filter) ||
                                        p.RefBank.Contains(factorPaginationDto.Filter) ||
                                        p.EnterMoneyWalletId.Contains(factorPaginationDto.Filter) ||
                                        p.UserId.Contains(factorPaginationDto.Filter) ||
                                        p.GateId.Contains(factorPaginationDto.Filter));
                        return exp;
                    }
                default:
                    return null;
            }


        }




    }

}
