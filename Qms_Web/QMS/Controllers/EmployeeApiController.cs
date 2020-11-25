using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QmsCore.UIModel;
using QmsCore.Services;

namespace QMS.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeApiController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeApiController(IEmployeeService emplSvc)
        {
            _employeeService = emplSvc;
        }

        [Produces("application/json")]
        [HttpGet("search")]
        public IActionResult Search()
        {
            string logSnippet = new StringBuilder("[")
                                .Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                .Append("][EmployeeApiController][HttpGet][Search] => ")
                                .ToString();
            try
            {
                var searchTerm      = HttpContext.Request.Query["term"].ToString();             
                string showInactive = HttpContext.Request.Query["showInactive"].ToString();
                bool retrieveAll    = bool.Parse(showInactive);
                    
                Console.WriteLine(" ");
                Console.WriteLine(logSnippet + "(searchTerm)...: '" + searchTerm + "'");
                Console.WriteLine(logSnippet + "(showInactive).: '" + showInactive + "'");
                Console.WriteLine(logSnippet + "(retrieveAll)..: '" + retrieveAll + "'");
                Console.WriteLine(" ");

                List<string> employees = null;

                if (retrieveAll)
                {
                    Console.WriteLine(logSnippet + "Retrieving all employees (active and inactive).");
                    employees = _employeeService
                    .RetrieveAll()
                    .Where(e => e.LastName.Contains(searchTerm))
                    .Select(e => e.SearchResultValue)
                    .ToList();
                }
                else
                {
                    Console.WriteLine(logSnippet + "Retrieving active employees only.");
                    employees = _employeeService
                    .RetrieveAllActive()
                    .Where(e => e.LastName.Contains(searchTerm))
                    .Select(e => e.SearchResultValue)
                    .ToList();
              }

                Console.WriteLine(logSnippet + "(employees == null): " + (employees == null));
                if (employees != null)
                {
                    Console.WriteLine(logSnippet + "(employees.Count()): " + (employees.Count()));
                }
                
                return Ok(employees);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}