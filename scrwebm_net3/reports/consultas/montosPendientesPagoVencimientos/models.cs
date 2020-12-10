
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace scrwebm_net3.reports.consultas.montosPendientesPagoVencimientos
{
    public class montoPendientePago_item
    {
        public string id { get; set; }
        public string moneda { get; set; }
        public string monedaDescripcion { get; set; }
        public string monedaSimbolo { get; set; }
        public string compania { get; set; }
        public string companiaNombre { get; set; }
        public string companiaAbreviatura { get; set; }
        public string cedenteNombre { get; set; }
        public string cedenteAbreviatura { get; set; }
        public string suscriptor { get; set; }
        public string suscriptorAbreviatura { get; set; }
        public string aseguradoAbreviatura { get; set; }


        public string codigo { get; set; }
        public string referencia { get; set; }
        public string origen { get; set; }      // source.origen + source.numero 
        public short numero { get; set; }
        public short cantidad { get; set; }

        public DateTime? fechaEmision { get; set; }
        public DateTime? fecha { get; set; }
        public DateTime? fechaVencimiento { get; set; }

        public int diasPendientes { get; set; }
        public int diasVencidos { get; set; }
        public int cantidadPagos { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal montoCuota { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal montoYaPagado { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal resta { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal montoYaCobrado { get; set; }

        public string user { get; set; }
        public string cia { get; set; }
    }

    public class montoPendientePago_reportItem
    {
        public string monedaDescripcion { get; set; }
        public string monedaSimbolo { get; set; }
        public string companiaNombre { get; set; }
        public string companiaAbreviatura { get; set; }
        public string cedenteNombre { get; set; }
        public string cedenteAbreviatura { get; set; }
        public string suscriptorAbreviatura { get; set; }
        public string aseguradoAbreviatura { get; set; }


        public string codigo { get; set; }
        public string referencia { get; set; }
        public string origen { get; set; }      // source.origen + source.numero 
        public short numero { get; set; }
        public short cantidad { get; set; }

        public DateTime? fechaEmision { get; set; }
        public DateTime? fecha { get; set; }
        public DateTime? fechaVencimiento { get; set; }

        public int diasPendientes { get; set; }
        public int diasVencidos { get; set; }

        public decimal montoCuota { get; set; }
        public decimal montoYaPagado { get; set; }
        public decimal resta { get; set; }
        public decimal montoYaCobrado { get; set; }

        public string vencimiento { get; set; }
        public string vencimientoAbreviatura { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<montoPendientePago_reportItem> get_MontosPendientesPago()
        {
            List<montoPendientePago_reportItem> list = new List<montoPendientePago_reportItem>();
            return list;
        }
    }

    public class config_item
    {
        public string id { get; set; }
        public config_item_opcionesReporte opcionesReporte { get; set; }
        public config_item_compania compania { get; set; }
        public string user { get; set;  }
    }

    public class config_item_opcionesReporte
    {
        public string subTitulo { get; set; }
        public bool mostrarColores { get; set; }
        public DateTime fechaPendientesAl { get; set; }
        public DateTime fechaLeerHasta { get; set; }
        public bool resumen { get; set; }
        public bool formatoExcel { get; set; }
    }

    public class config_item_compania
    {
        public string nombre { get; set; }
        public string id { get; set; }
    }
}
