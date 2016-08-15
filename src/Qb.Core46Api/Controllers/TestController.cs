using System;
using Microsoft.AspNetCore.Mvc;
using Qb.Core46Api.Helpers;

namespace Qb.Core46Api.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Res.PlainUtf8("Salutations.");
        }

        [Route("[action]")]
        public IActionResult Exception()
        {
            throw new Exception("Test exception.");
        }
    }
}