using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.remesas
{
    public class remesaList
    {
        public Int16 remesaID { get; set; }
        public string compania { get; set; }
        public string miSu { get; set; }
        public string moneda { get; set; }
        public decimal monto { get; set; }
        public DateTime fecha { get; set; }
        public DateTime? fechaCerrada { get; set; }
        public string observaciones { get; set; }

        public Int16 operacion_cantPagos { get; set; }
        public decimal operacion_sumatoriaMontoPagos { get; set; }
        public bool operacion_pagosMismaMonedaDeLaRemesa { get; set; }
        public bool operacion_pagosMultiMoneda { get; set; }
        public decimal operacion_diferenciaMontoRemesaPagos { get; set; }
        public bool operacion_completo { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<remesaList> get_RemesasList()
        {
            List<remesaList> list = new List<remesaList>();
            return list;
        }
    }
}