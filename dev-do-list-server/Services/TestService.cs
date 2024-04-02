using DevDoListServer.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Security;

namespace DevDoListServer.Services
{
    public class TestService
    {
        public TestService() { }

        public ActionResult<Hello> TestServiceFunc(int id)
        {
            if (id == 0)
                return new OkObjectResult("Good");

            if (id == 1)
                return new NotFoundObjectResult("Not data exists");

            return new ConflictObjectResult("Not good");
        }
    }
}
