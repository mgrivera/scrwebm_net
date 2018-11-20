using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.consultas.corretaje
{
    public class corretaje_item
    {
        public string monedaDescripcion { get; set; }
        public string monedaSimbolo { get; set; }
        public string companiaNombre { get; set; }
        public string companiaAbreviatura { get; set; }
        public string ramoDescripcion { get; set; }
        public string ramoAbreviatura { get; set; }
        public string aseguradoAbreviatura { get; set; }
        public string suscriptorAbreviatura { get; set; }

        public string origen { get; set; }      // source.origen + source.numero 

        public short cuota_numero { get; set; }
        public short cuota_cantidad { get; set; }
        public DateTime cuota_fechaEmision { get; set; }
        public DateTime cuota_fechaCuota { get; set; }
        public DateTime cuota_fechaVencimiento { get; set; }
        public decimal cuota_monto { get; set; }

        public decimal montoPorPagar { get; set; }
        public decimal montoCorretaje { get; set; }
        public decimal montoCobrado { get; set; }
        public decimal montoPagado { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<corretaje_item> get_Corretaje()
        {
            List<corretaje_item> list = new List<corretaje_item>();
            return list;
        }
    }
}