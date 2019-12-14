using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Creation.Web.Controllers
{
    [ControllerAttribute]
    [Route("guest-login")]
    public class GuestLoginController
    {
        [HttpGet]
        public Data Get()
        {
            return new Data();
        }

        public class Data
        {

        }
    }
}
