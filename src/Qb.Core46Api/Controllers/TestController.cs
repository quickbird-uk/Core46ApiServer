using System;
using Microsoft.AspNetCore.Mvc;
using Qb.Core46Api.Helpers;

namespace Qb.Core46Api.Controllers
{
    /// <summary>Test conteoller used to make sure the server si fuctions as it should be.</summary>
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        /// <summary>Simple plain text hello message.</summary>
        /// <returns>`text/plain` UTF-8 hello message.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Res.PlainUtf8("Salutations.");
        }

        /// <summary>Throws an test exeception on the server.</summary>
        /// <returns>Client should be given an error page.</returns>
        [HttpGet("[action]")]
        public IActionResult Exception()
        {
            throw new Exception("Test exception.");
        }
    }
}