﻿using PharmaCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PharmaCRM.Controllers
{
    public class ActivitiesController : ApiController
    {
        public IEnumerable<Actividade> GetActividade()
        {
            return PharmaCRM.Lib_Primavera.PriIntegration.GetActividade();
        }
    }
}
