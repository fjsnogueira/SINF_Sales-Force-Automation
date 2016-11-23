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
        [Route("api/vendedores")]
        [HttpGet]
        public IEnumerable<Lib_Primavera.Model.Vendedor> Get()
        {
            return PharmaCRM.Lib_Primavera.PriIntegration.ListaVendedores();
        }

        [Route("api/vendedores/{id}")]
        [HttpGet]
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

        // http://localhost:49559/api/vendedores/1/atividades?dataInicio=2010-11-15&dataFim=2016-11-15
        [Route("api/vendedores/{id}/atividades")]
        [HttpGet]
        public IEnumerable<Lib_Primavera.Model.Atividade> GetVendedorAtividades(string id, [FromUri] string dataInicio = null, [FromUri] string dataFim = null)
        {
            return Lib_Primavera.PriIntegration.GetVendedorAtividades(id, dataInicio, dataFim);
        }

        [Route("api/vendedores/{id}/encomendas")]
        [HttpGet]
        public IEnumerable<Lib_Primavera.Model.Encomenda> GetVendedorEncomendas(string id)
        {
            return Lib_Primavera.PriIntegration.GetEncomendasVendedor(id);
        }

        [Route("api/vendedores/{id}/oportunidades")]
        [HttpGet]
        public IEnumerable<Lib_Primavera.Model.Oportunidade> GetVendedorOportunidades(string id)
        {
            return Lib_Primavera.PriIntegration.GetVendedorOportunidades(id);
        }

        [Route("api/vendedores/{id}/objetivo")]
        [HttpGet]
        public Lib_Primavera.Model.Objetivo GetVendedorObjetivo(string id)
        {
            Lib_Primavera.Model.Objetivo obj = Lib_Primavera.PriIntegration.GetObjetivoVendedor(id);

            if (obj == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return obj;
        }

        [Route("api/vendedores/{id}/kpi")]
        [HttpGet]
        public Lib_Primavera.Model.KPI GetVendedorKPIs(string id)
        {
            Lib_Primavera.Model.KPI kpis = Lib_Primavera.PriIntegration.getVendedorKPIs(id);

            if (kpis == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return kpis;
        }
    }
}
