using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
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
        private GateApiReturn<Factor> model;

        public BankController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IUnitOfWork<Financial_MadPayDbContext> dbFinancial,
            IMapper mapper,
            ILogger<BankController> logger)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            model = new GateApiReturn<Factor>
            {
                Result = null
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
                model.Messages = new string[] { "توکن ارسالی معتبر نمیباشد" };
                return View(model);
            }
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

            model.Status = true;
            model.Messages.Clear();
            model.Result = factorFromRepo;
            return View(model);
        }
    }
}