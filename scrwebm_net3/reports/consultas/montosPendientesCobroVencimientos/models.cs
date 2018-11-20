using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.consultas.montosPendientesCobroVencimientos
{
    public class montoPendienteCobro_item
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
        public decimal montoPorPagar { get; set; }
        public decimal saldo1 { get; set; }

        public decimal montoCobrado { get; set; }
        public decimal montoPagado { get; set; }
        public decimal saldo2 { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<montoPendienteCobro_item> get_MontosPendientesCobro()
        {
            List<montoPendienteCobro_item> list = new List<montoPendienteCobro_item>();
            return list;
        }
    }
}