﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PharmaCRM.Controllers
{

    public class OportunidadesController : ApiController
    {
        [Route("api/oportunidades")]
        [HttpGet]
        public IEnumerable<Lib_Primavera.Model.Oportunidade> Get()
        {
            return Lib_Primavera.PriIntegration.getOportunidades();
        }

        [Route("api/oportunidades/{id}")]
        [HttpGet]
        public Lib_Primavera.Model.Oportunidade Get(string id)
        {
            return Lib_Primavera.PriIntegration.getOportunidade(id);
        }

        [Route("api/oportunidades/{id}/atividades")]
        [HttpGet]
        public IEnumerable<Lib_Primavera.Model.Atividade> GetAtividades(string id)
        {
            return Lib_Primavera.PriIntegration.getAtividadesDeOportunidade(id);
        }
        
        [Route("api/oportunidades")]
        [HttpPost]
        public HttpResponseMessage Post(Lib_Primavera.Model.Oportunidade Oportunidade)
        {
            Lib_Primavera.Model.RespostaErro respostaErro = new Lib_Primavera.Model.RespostaErro();
            respostaErro = Lib_Primavera.PriIntegration.createOportunidade(Oportunidade);
            if (respostaErro.Erro == 0)
            {
                return Request.CreateResponse(HttpStatusCode.Created, Oportunidade);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, respostaErro.Descricao);
            }
        }

        [Route("api/oportunidades/{id}")]
        [HttpPut]
        public HttpResponseMessage Put(Lib_Primavera.Model.Oportunidade oportunidade)
        {
            try
            {
                Lib_Primavera.Model.RespostaErro respostaErro = new Lib_Primavera.Model.RespostaErro();
                respostaErro = Lib_Primavera.PriIntegration.UpdOportunidade(oportunidade);
                if (respostaErro.Erro == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, respostaErro.Descricao);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, respostaErro.Descricao);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Route("api/oportunidades/{id}")]
        [HttpPut]
        public HttpResponseMessage Close(string idOportunidade, Boolean won)
        {
            try
            {
                Lib_Primavera.Model.RespostaErro respostaErro = new Lib_Primavera.Model.RespostaErro();
                respostaErro = Lib_Primavera.PriIntegration.CloseOportunidade(idOportunidade, won);
                if (respostaErro.Erro == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, respostaErro.Descricao);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, respostaErro.Descricao);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Route("api/oportunidades/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                Lib_Primavera.Model.RespostaErro respostaErro = new Lib_Primavera.Model.RespostaErro();
                respostaErro = Lib_Primavera.PriIntegration.deleteOportunidade(id);
                if (respostaErro.Erro == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, respostaErro.Descricao);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, respostaErro.Descricao);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}
