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
    [Route("api/ca/comment")]
    [ApiController]
    public class CACommentApiController : ControllerBase
    {
        private readonly ICorrectiveActionService _correctiveActionService;

        public CACommentApiController(ICorrectiveActionService caSvc)
        {
            _correctiveActionService = caSvc;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CACommentGet>> RetrieveAll(int id)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CACommentApiController][HttpGet][RetrieveAll] => ")
                                .ToString();
            
            Console.WriteLine(logSnippet + $"(id): '{id}'");

            List<CACommentGet> apiCommentList = new List<CACommentGet>();
            List<CorrectiveActionComment> svcCommentList = _correctiveActionService.RetrieveComments(id);

            Console.WriteLine(logSnippet + $"(svcCommentList == null): '{svcCommentList == null}'");
            if ( svcCommentList != null)
            {
                Console.WriteLine(logSnippet + $"(svcCommentList.Count)..: '{svcCommentList.Count}'");
            }

            foreach (CorrectiveActionComment svcComment in svcCommentList)
            {
                Console.WriteLine(logSnippet + $"(svcCommentsvcComment.Id)................: {svcComment.Id}");
                Console.WriteLine(logSnippet + $"(svcCommentsvcComment.CorrectiveActionId): {svcComment.CorrectiveActionId}");
                Console.WriteLine(logSnippet + $"(svcCommentsvcComment.AuthorId)..........: {svcComment.AuthorId}");
                Console.WriteLine(logSnippet + $"(svcComment.Author == null)..............: {svcComment.Author == null}");

                CACommentGet apiComment = new CACommentGet();
                apiComment.OrgLabel     = svcComment.Author.OrganizationName;
                apiComment.DisplayName  = svcComment.Author.DisplayName;
                apiComment.Message      = svcComment.Message;
                apiComment.DateCreated  = svcComment.CreatedAt.ToString("MM/dd/yyyy HH:mm:ss");
                apiCommentList.Add(apiComment);
            }

            return apiCommentList;
        }

        [HttpPost]
        public ActionResult<CACommentPost> Create(CACommentPost caComment)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CACommentApiController][HttpPost][Create] => ")
                                .ToString();

            UserViewModel qmsUserVM = HttpContext.Session.GetObject<UserViewModel>(MiscConstants.USER_SESSION_VM_KEY);

            Console.WriteLine(logSnippet + $"(caComment == null)...........: '{caComment == null}'");
            Console.WriteLine(logSnippet + $"(caComment.UserId)............: '{caComment.UserId}'");
            Console.WriteLine(logSnippet + $"(caComment.CorrectiveActionI).: '{caComment.CorrectiveActionId}'");
            Console.WriteLine(logSnippet + $"(caComment.Comment)...........: '{caComment.Comment}'");
            Console.WriteLine(logSnippet + $"(qmsUserVM): {qmsUserVM}");

            _correctiveActionService.SaveComment(caComment.Comment, Int32.Parse(caComment.CorrectiveActionId), Int32.Parse(caComment.UserId));

            return CreatedAtAction(nameof(RetrieveAll), new { @id = caComment.CorrectiveActionId} );
        }
    }
}