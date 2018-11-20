using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.consultas.montosPendientesPagoVencimientos
{
    public class montoPendientePago_item
    {
        public string monedaDescripcion { get; set; }
        public string monedaSimbolo { get; set; }
        public string companiaNombre { get; set; }
        public string companiaAbreviatura { get; set; }
        public string aseguradoAbreviatura { get; set; }
        public string suscriptorAbreviatura { get; set; }

        public string origen { get; set; }      // source.origen + source.numero 
        public short numero { get; set; }
        public short cantidad { get; set; }

        public DateTime? fechaEmision { get; set; }
        public DateTime? fecha { get; set; }
        public DateTime? fechaVencimiento { get; set; }

        public int diasPendientes { get; set; }
        public int diasVencidos { get; set; }
        public string vencimiento { get; set; }
        public string vencimientoAbreviatura { get; set; }

        public decimal montoCuota { get; set; }
        public decimal montoYaPagado { get; set; }
        public decimal resta { get; set; }

        public decimal montoYaCobrado { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<montoPendientePago_item> get_MontosPendientesPago()
        {
            List<montoPendientePago_item> list = new List<montoPendientePago_item>();
            return list;
        }
    }
}