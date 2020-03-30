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
using System;
using System.Threading.Tasks;

namespace MadPay724.Payment.Controllers
{

    public class BankController : Controller
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<BankController> _logger;
        private GateApiReturn<BankPayDto> model;

        public BankController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IUnitOfWork<Financial_MadPayDbContext> dbFinancial,
            IMapper mapper,
            ILogger<BankController> logger)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
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
            if (factorFromRepo.DateCreated.AddMinutes(10) < DateTime.Now)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "زمان تکمیل عملیات پرداخت تمام شده است" };
                model.Result.Factor = factorFromRepo;
                model.Result.Gate = await _db.GateRepository.GetByIdAsync(factorFromRepo.GateId);
                return View(model);
            }
            if (factorFromRepo.Status)
            {
                model.Status = false;
                model.Messages.Clear();
                model.Messages = new string[] { "پرداخت قبلا به صورت موفق انجام شده است" };
                model.Result.Factor = factorFromRepo;
                model.Result.Gate = await _db.GateRepository.GetByIdAsync(factorFromRepo.GateId);
                return View(model);
            }

            model.Result.Gate = await _db.GateRepository.GetByIdAsync(factorFromRepo.GateId);

            if (model.Result.Gate.IsDirect)
            {
                return Redirect("");
            }
            else
            {
                model.Status = true;
                model.Messages.Clear();
                model.Result.Factor = factorFromRepo;
                return View(model);
            }

        }
    }
}