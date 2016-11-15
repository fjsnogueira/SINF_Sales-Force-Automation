﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PharmaCRM.Controllers
{
    public class VendedoresController : ApiController
    {
        public IEnumerable<Lib_Primavera.Model.Vendedor> Get()
        {
            return PharmaCRM.Lib_Primavera.PriIntegration.ListaVendedores();
        }

        public Lib_Primavera.Model.Vendedor Get(String id)
        {
            Lib_Primavera.Model.Vendedor vendedor = PharmaCRM.Lib_Primavera.PriIntegration.GetVendedor(id);
            if (vendedor == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return vendedor;
            }
        }

        public IEnumerable<Lib_Primavera.Model.Atividade> GetVendedorAtividades(string id, string dataInicio, string dataFim)
        {
            return Lib_Primavera.PriIntegration.GetVendedorAtividades(id, dataInicio, dataFim);
        }
    }
}
