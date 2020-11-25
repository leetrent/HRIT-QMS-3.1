using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.Services;
using QmsCore.UIModel;
using QMS.ApiModels;

namespace QMS.Controllers
{
    [Route("api/ua/users")]
    [ApiController]
    public class CAUserApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public CAUserApiController(IUserService usrSvc)
        {
            _userService = usrSvc;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UAUserGet>> RetrieveUsersByOrganizationId(int orgId)
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][CAUserApiController][HttpGet][RetrieveUsersByOrganizationId] => ")
                                .ToString();

            Console.WriteLine(logSnippet + $"(orgId): '{orgId}'");

            List<UAUserGet> apiUserList = new List<UAUserGet>();
            List<User> svcUserList = _userService.RetrieveUsersByOrganizationId(orgId);

            Console.WriteLine(logSnippet + $"(svcUserList == null): '{svcUserList == null}'");
            Console.WriteLine(logSnippet + $"(svcUserList.Count)..: '{svcUserList.Count}'");

            foreach (var svcUser in svcUserList)
            {
                apiUserList.Add
                (
                    new UAUserGet
                    {
                        UserId          = Convert.ToString(svcUser.UserId),
                        OrgId           = (svcUser.OrgId.HasValue) ? Convert.ToString(svcUser.OrgId.Value) : null,
                        EmailAddress    = svcUser.EmailAddress,
                        DisplayLabel    = svcUser.DisplayLabel
                    }
                );
            }

            foreach (var apiUser in apiUserList)
            {
                Console.WriteLine(logSnippet + apiUser);
            }

            return apiUserList;
        }
    }
}