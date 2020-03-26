using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.Document;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Panel.Admin
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Admin")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<DocumentsController> logger, IUploadService uploadService,
            IWebHostEnvironment env)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminDocument.GetDocuments)]
        public async Task<IActionResult> GetDocuments([FromQuery]PaginationDto paginationDto)
        {
            var documentsFromRepo = await _db.DocumentRepository.GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToDocumentExpression(),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "");

            Response.AddPagination(documentsFromRepo.CurrentPage, documentsFromRepo.PageSize,
                documentsFromRepo.TotalCount, documentsFromRepo.TotalPage);

            var documents = new List<DocumentForReturnDto>();

            foreach (var item in documentsFromRepo)
            {
                documents.Add(_mapper.Map<DocumentForReturnDto>(item));
            }

            return Ok(documents);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminDocument.GetUserDocuments)]
        public async Task<IActionResult> GetUserDocuments(string userId)
        {
            var documentsFromRepo = await _db.DocumentRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.Approve), "");

            var documents = _mapper.Map<List<DocumentForReturnDto>>(documentsFromRepo);

            return Ok(documents);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminDocument.GetDocument)]
        public async Task<IActionResult> GetDocument(string documentId)
        {
            var documentFromRepo = 
                await _db.DocumentRepository.GetManyAsync(p=>p.Id == documentId,null,"User");
            if (documentFromRepo.Any())
            {
                var document = _mapper.Map<DocumentForReturnDto>(documentFromRepo.Single());

                return Ok(document);
            }
            else
            {
                return BadRequest("مدرکی وجود ندارد");
            }

        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut(ApiV1Routes.AdminDocument.UpdateDocument)]
        public async Task<IActionResult> UpdateDocument(string documentId,DocumentForUpdateDto documentForUpdateDto)
        {
            var documentFromRepo = await _db.DocumentRepository.GetByIdAsync(documentId);
            if (documentFromRepo != null)
            {
                documentFromRepo.Message = documentForUpdateDto.Message;
                documentFromRepo.Approve = documentForUpdateDto.Approve;

                _db.DocumentRepository.Update(documentFromRepo);

                if(await _db.SaveAsync())
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("خطا رد اپدیت");
                }
            }
            else
            {
                return BadRequest("مدرکی وجود ندارد");
            }

        }
    }
}