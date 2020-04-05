using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parbad;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers
{
    [AllowAnonymous]
    public class BankController : Controller
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<BankController> _logger;
        private readonly IOnlinePayment _onlinePayment;
        private readonly IWalletService _walletService;

        private ApiReturn<Factor> model;

        public BankController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IUnitOfWork<Financial_MadPayDbContext> dbFinancial,
            IMapper mapper, IWalletService walletService,
            ILogger<BankController> logger, IOnlinePayment onlinePayment)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            _onlinePayment = onlinePayment;
            _walletService = walletService;
            model = new ApiReturn<Factor>
            {
                Result = null
            };
        }
        [HttpGet, Route("bank/pay/{id}")]
        public async Task<IActionResult> Pay(string id)
        {
            var factorFromRepo = await _dbFinancial.FactorRepository.GetByIdAsync(id);

            if (factorFromRepo == null)
            {
                return RedirectToAction("Verify", new {error = "تراکنش مورد نظر وجود ندارد" });
            }
            //
            if (factorFromRepo.Status)
            {
                return RedirectToAction("Verify", new { token = factorFromRepo.Id, error = "پرداخت قبلا به صورت موفق انجام شده است" });
            }

            var callbackUrl = Url.Action("Verify", "Bank", null, Request.Scheme);
            var result = await _onlinePayment.RequestAsync(invoice =>
            {
                invoice
                .UseAutoIncrementTrackingNumber()
                .SetAmount(factorFromRepo.EndPrice)
                .SetCallbackUrl(callbackUrl)
                //.UseParbadVirtual();
                .UseZarinPal("پرداخت از سایت مادپی");
                });
            if (result.IsSucceed)
            {
                factorFromRepo.RefBank = result.TrackingNumber.ToString();
                factorFromRepo.DateModified = DateTime.Now;
                factorFromRepo.Bank = result.GatewayName.ToBank();
                _dbFinancial.FactorRepository.Update(factorFromRepo);

                if (await _dbFinancial.SaveAsync())
                {
                    await result.GatewayTransporter.TransportAsync();
                    return new EmptyResult();
                }
                else
                {
                    return RedirectToAction("Verify", new { token = factorFromRepo.Id, error = "خطا در ثبت " });
                }
            }
            else
            {
                return RedirectToAction("Verify", new { token = factorFromRepo.Id, error = result.Message });
            }
        }
        [ Route("bank/verify")]
        public async Task<IActionResult> Verify(string token = null, string error = null)
        {
            if (!string.IsNullOrEmpty(error))
            {
                if (!string.IsNullOrEmpty(token))
                {       
                    var factor = await _dbFinancial.FactorRepository.GetByIdAsync(token);
                    factor.IsAlreadyVerified = true;
                    factor.DateModified = DateTime.Now;
                    factor.Message = error;
                    _dbFinancial.FactorRepository.Update(factor);
                    await _dbFinancial.SaveAsync();

                    model.Status = false;
                    model.Message = error;
                    model.Result = factor;
                    return View(model);
                }
                else
                {
                    model.Status = false;
                    model.Message = error;
                    model.Result = null;
                    return View(model);
                }               
            }

            var invoice = await _onlinePayment.FetchAsync();

            var factorFromRepo = (await _dbFinancial.FactorRepository
                .GetManyAsync(p => p.RefBank == invoice.TrackingNumber.ToString(), null, "")).SingleOrDefault();

            if (invoice.Status == PaymentFetchResultStatus.AlreadyProcessed)
            {
                factorFromRepo.IsAlreadyVerified = true;
                factorFromRepo.DateModified = DateTime.Now;
                factorFromRepo.Message = "این تراکنش قبلا برررسی شده است";
                factorFromRepo.GatewayName = invoice.GatewayName;

                _dbFinancial.FactorRepository.Update(factorFromRepo);
                await _dbFinancial.SaveAsync();

                model.Status = false;
                model.Message = "این تراکنش قبلا برررسی شده است";
                model.Result = factorFromRepo;
                return View(model);
            }
            else
            {
                //Verify
                var trackingNumber = Convert.ToInt64(factorFromRepo.RefBank);
                var verifyResult = await _onlinePayment.VerifyAsync(trackingNumber);
                if (verifyResult.IsSucceed)
                {
                    factorFromRepo.Status = true;
                    factorFromRepo.IsAlreadyVerified = true;
                    factorFromRepo.DateModified = DateTime.Now;
                    factorFromRepo.Message = "تراکنش با موفقیت انجام شد";
                    _dbFinancial.FactorRepository.Update(factorFromRepo);
                    await _dbFinancial.SaveAsync();

                    await _walletService
                        .IncreaseInventoryAsync(factorFromRepo.EndPrice, factorFromRepo.EnterMoneyWalletId, false);

                    model.Status = true;
                    model.Message = "تراکنش با موفقیت انجام شد";
                    model.Result = factorFromRepo;
                    return View(model);

                }
                else
                {
                    factorFromRepo.IsAlreadyVerified = true;
                    factorFromRepo.DateModified = DateTime.Now;
                    factorFromRepo.Message = verifyResult.Message;
                    _dbFinancial.FactorRepository.Update(factorFromRepo);
                    await _dbFinancial.SaveAsync();

                    model.Status = false;
                    model.Message = verifyResult.Message;
                    model.Result = factorFromRepo;
                    return View(model);
                }
            }
        }
    }
}