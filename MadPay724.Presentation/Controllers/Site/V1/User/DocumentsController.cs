using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Document;
using MadPay724.Data.Models;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<DocumentsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.Document.AddDocument)]
        public async Task<IActionResult> AddDocument(string userId, DocumentForCreateDto documentForCreateDto)
        {
            var documentFromRepoApprove = await _db.DocumentRepository.GetAsync(p => p.Approve ==1);

            if (documentFromRepoApprove == null)
            {
                var documentFromRepoChecking = await _db.DocumentRepository.GetAsync(p => p.Approve == 0);
                if (documentFromRepoApprove == null)
                {
                    var documentForCreate = new Document()
                    {
                        UserId = userId,
                        Approve = 0
                    };
                    var document = _mapper.Map(documentForCreateDto, documentForCreate);

                    await _db.DocumentRepository.InsertAsync(document);

                    if (await _db.SaveAsync())
                    {
                        var documentForReturn = _mapper.Map<DocumentForReturnDto>(document);

                        return CreatedAtRoute("GetDocument", new { id = document.Id, userId = userId }, documentForReturn);
                    }
                    else
                        return BadRequest("خطا در ثبت اطلاعات");
                }
                {
                    return BadRequest("مدارک ارسالی قبلیه شما در حال بررسی میباشد");
                }

                
            }
            {
                return BadRequest("شما مدرک شناسایی تایید شده دارید و نمیتوانید دوباره آنرا ارسال کنید");
            }


        }
        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Document.GetDocuments)]
        public async Task<IActionResult> GetDocuments(string userId)
        {
            var DocumentsFromRepo = await _db.DocumentRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.Approve), "");


            var Documents = _mapper.Map<List<DocumentForUserDetailedDto>>(DocumentsFromRepo);

            return Ok(Documents);
        }
    }
}