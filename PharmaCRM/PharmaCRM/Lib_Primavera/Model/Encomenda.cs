﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCRM.Lib_Primavera.Model
{
    public class Encomenda
    {
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

        public string Entidade
        {
            get;
            set;
        }

        public double TotalMercadoria
        {
            get;
            set;
        }

        public double TotalIva
        {
            get;
            set;
        }

        public double TotalDesconto
        {
            get;
            set;
        }

        public DateTime DataVencimento
        {
            get;
            set;
        }
    }
}