using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.consultas.montosPendientes
{
    public class montoPendiente_item
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
        public DateTime cuota_fecha { get; set; }
        public DateTime cuota_fechaVencimiento { get; set; }
        public decimal cuota_monto { get; set; }

        public short cantidadPagosParciales { get; set; }
        public decimal montoPendiente { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<montoPendiente_item> get_MontosPendientes()
        {
            List<montoPendiente_item> list = new List<montoPendiente_item>();
            return list;
        }
    }
}