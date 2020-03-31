using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Data.Dtos.Api.Pay;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parbad;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Payment.Controllers
{

    public class BankController : Controller
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<BankController> _logger;
        private readonly IOnlinePayment _onlinePayment;
        private GateApiReturn<BankPayDto> model;

        public BankController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IUnitOfWork<Financial_MadPayDbContext> dbFinancial,
            IMapper mapper,
            ILogger<BankController> logger, IOnlinePayment onlinePayment)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            _onlinePayment = onlinePayment;
            model = new GateApiReturn<BankPayDto>
            {
                Result = new BankPayDto()
            };
        }
        [HttpGet]
        public async Task<IActionResult> Pay(string token)
        {

            var factorFromRepo = await _dbFinancial.FactorRepository.GetByIdAsync(token);

            if (factorFromRepo == null)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "token-error" };
                model.Result = null;
                return View(model);
            }
            //
            model.Result.Factor = factorFromRepo;
            model.Result.Gate = await _db.GateRepository.GetByIdAsync(factorFromRepo.GateId);
            //
            if (factorFromRepo.DateCreated.AddMinutes(10) < DateTime.Now)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "زمان تکمیل عملیات پرداخت تمام شده است" };
                return View(model);
            }
            if (factorFromRepo.Status)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "پرداخت قبلا به صورت موفق انجام شده است" };
                return View(model);
            }

            if (model.Result.Gate.IsDirect)
            {
                var callbackUrl = Url.Action("Verify", "Bank", null, Request.Scheme);
                var result = await _onlinePayment.RequestAsync(invoice =>
                {
                    invoice
                    .UseAutoIncrementTrackingNumber()
                    .SetAmount(factorFromRepo.EndPrice)
                    .SetCallbackUrl(callbackUrl)
                    .UseParbadVirtual();
                    //.UseZarinPal("پرداخت از سایت مادپی");
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
                        model.Status = false;
                        model.Messages.Clear();
                        model.Messages = new string[] { "خطا در ثبت " };
                        return View(model);
                    }
                }
                else
                {
                    model.Status = false;
                    model.Messages.Clear();
                    model.Messages = new string[] { result.Message };

                    return View(model);
                }
            }
            else
            {
                model.Status = true;
                model.Messages.Clear();
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> MadPay(string token)
        {

            var factorFromRepo = await _dbFinancial.FactorRepository.GetByIdAsync(token);

            if (factorFromRepo == null)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "token-error" };
                model.Result = null;
                return View("Pay", model);
            }
            //
            model.Result.Factor = factorFromRepo;
            model.Result.Gate = await _db.GateRepository.GetByIdAsync(factorFromRepo.GateId);
            //
            if (factorFromRepo.DateCreated.AddMinutes(10) < DateTime.Now)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "زمان تکمیل عملیات پرداخت تمام شده است" };
                return View("Pay", model);
            }
            if (factorFromRepo.Status)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "پرداخت قبلا به صورت موفق انجام شده است" };
                return View("Pay", model);
            }


            if (!model.Result.Gate.IsDirect)
            {
                model.Status = true;
                model.Messages.Clear();
                return View("Pay", model);
            }

            var callbackUrl = Url.Action("Verify", "Bank", null, Request.Scheme);
            var result = await _onlinePayment.RequestAsync(invoice =>
            {
                invoice
                .UseAutoIncrementTrackingNumber()
                .SetAmount(factorFromRepo.EndPrice)
                .SetCallbackUrl(callbackUrl)
                .UseParbadVirtual();
                //.UseZarinPal("پرداخت از سایت مادپی");
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
                    model.Status = false;
                    model.Messages.Clear();
                    model.Messages = new string[] { "خطا در ثبت " };
                    return View("Pay", model);
                }
            }
            else
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { result.Message };
                return View("Pay", model);
            }

        }
        public async Task<IActionResult> Verify(string token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var factor = await _dbFinancial.FactorRepository.GetByIdAsync(token);
                factor.IsAlreadyVerified = false;
                factor.DateModified = DateTime.Now;
                factor.Message = "کاربر پرداخت را کنسل کرده است";

                _dbFinancial.FactorRepository.Update(factor);
                await _dbFinancial.SaveAsync();
                return Redirect(factor.RedirectUrl + "?token=" + factor.Id);
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

            }
            else
            {
                factorFromRepo.IsAlreadyVerified = invoice.IsAlreadyVerified;
                factorFromRepo.DateModified = DateTime.Now;
                factorFromRepo.Message = "پرداخت با موفقیت انجام شد اما توسط کاربر تایید نشده است";
                factorFromRepo.GatewayName = invoice.GatewayName;

                _dbFinancial.FactorRepository.Update(factorFromRepo);
                await _dbFinancial.SaveAsync();
            }

            return Redirect(factorFromRepo.RedirectUrl + "?token=" + factorFromRepo.Id);
        }
    }
}