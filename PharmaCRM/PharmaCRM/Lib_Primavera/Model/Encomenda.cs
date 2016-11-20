﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCRM.Lib_Primavera.Model
{
    public class Encomenda
    {
        public Encomenda() {
            NumeroDocumento = -1;
            Anulada = false;
        }

        public string Entidade
        {
            get;
            set;
        }

        public int NumeroDocumento
        {
            get;
            set;
        }

        public DateTime Data
        {
            get;
            set;
        }

        public double TotalMercadoria
        {
            get;
            set;
        }

        public string Serie
        {
            get;
            set;
        }

        public string idResponsavel
        {
            get;
            set;
        }

        public string Filial
        {
            get;
            set;
        }

        public List<Model.LinhaEncomenda> LinhasDocumento

        {
            get;
            set;
        }

        public Boolean Anulada
        {
            get;
            set;
        }

        public string idInterno
        {
            get;
            set;
        }
    }
}