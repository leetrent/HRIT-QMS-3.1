using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;
using QmsCore.Services;
using QMS.ApiModels;
using QMS.ViewModels;
using QMS.Extensions;
using QMS.Constants;

namespace QMS.Controllers
{
    [Route("api/de/comment")]
    [ApiController]
    public class DECommentApiController : ControllerBase
    {
        private readonly IDataErrorService _dataErrorService;

        public DECommentApiController(IDataErrorService deSvc)
        {
            _dataErrorService = deSvc;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DECommentGet>> RetrieveAll(int id)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][DECommentApiController][HttpGet][RetrieveAll] => ")
                                .ToString();
            
            Console.WriteLine(logSnippet + $"(id): '{id}'");

            List<DECommentGet> apiCommentList = new List<DECommentGet>();
            List<DataErrorComment> svcCommentList = _dataErrorService.RetrieveComments(id);

            Console.WriteLine(logSnippet + $"(svcCommentList == null): '{svcCommentList == null}'");
            if ( svcCommentList != null)
            {
                Console.WriteLine(logSnippet + $"(svcCommentList.Count)..: '{svcCommentList.Count}'");
            }

            foreach (DataErrorComment svcComment in svcCommentList)
            {
                Console.WriteLine(logSnippet + $"(svcCommentsvcComment.Id)................: {svcComment.Id}");
                Console.WriteLine(logSnippet + $"(svcCommentsvcComment.CorrectiveActionId): {svcComment.CorrectiveActionId}");
                Console.WriteLine(logSnippet + $"(svcCommentsvcComment.AuthorId)..........: {svcComment.AuthorId}");
                Console.WriteLine(logSnippet + $"(svcComment.Author == null)..............: {svcComment.Author == null}");

                DECommentGet apiComment = new DECommentGet();
                apiComment.OrgLabel     = svcComment.Author.OrganizationName;
                apiComment.DisplayName  = svcComment.Author.DisplayName;
                apiComment.Message      = svcComment.Message;
                apiComment.DateCreated  = svcComment.CreatedAt.ToString("MM/dd/yyyy HH:mm:ss");
                apiCommentList.Add(apiComment);
            }

            return apiCommentList;
        }

        [HttpPost]
        public ActionResult<DECommentPost> Create(DECommentPost deComment)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][DECommentApiController][HttpPost][Create] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            Console.WriteLine(logSnippet + $"(deComment == null)...........: '{deComment == null}'");
            Console.WriteLine(logSnippet + $"(deComment.UserId)............: '{deComment.UserId}'");
            Console.WriteLine(logSnippet + $"(deComment.CorrectiveActionI).: '{deComment.CorrectiveActionId}'");
            Console.WriteLine(logSnippet + $"(deComment.Comment)...........: '{deComment.Comment}'");
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            _dataErrorService.SaveComment(deComment.Comment, Int32.Parse(deComment.CorrectiveActionId), Int32.Parse(deComment.UserId));

            return CreatedAtAction(nameof(RetrieveAll), new { @id = deComment.CorrectiveActionId} );
        }
    }
}