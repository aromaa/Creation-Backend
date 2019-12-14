using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Creation.Web.Controllers.Debug
{
    [Route("crossdomain.xml")]
    public class DefaultCrossdomainController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return @"<?xml version=""1.0""?><cross-domain-policy><allow-access-from domain=""*""/></cross-domain-policy>";
        }
    }
}
