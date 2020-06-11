using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace scrwebm_net3.reports.primasEmitidas.facultativo.reaseguradores
{
    public class riesgoEmitido
    {
        public string _id { get; set; }
        public string riesgoId { get; set; }
        public int numero { get; set; }
        public riesgoEmitido_moneda moneda { get; set; }
        public riesgoEmitido_cedente cedente { get; set; }
        public riesgoEmitido_compania compania { get; set; }
        public riesgoEmitido_ramo ramo { get; set; }
        public riesgoEmitido_asegurado asegurado { get; set; }
        public string estado { get; set; }
        public int movimiento { get; set; }
        public DateTime fechaEmision { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal valorARiesgo { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal sumaAsegurada { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal prima { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal ordenPorc { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal sumaReasegurada { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaBruta { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal comision { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal impuesto { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal corretaje { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal impuestoSobrePN { get; set; }
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaNeta { get; set; }
        public string cia { get; set; }
        public string user { get; set; }
    }

    public class riesgoEmitido_cedente
    {
        public string _id { get; set; }
        public string nombre { get; set; }
        public string abreviatura { get; set; }
    }

    public class riesgoEmitido_moneda
    {
        public string _id { get; set; }
        public string descripcion { get; set; }
        public string simbolo { get; set; }
    }

    public class riesgoEmitido_compania
    {
        public string _id { get; set; }
        public string nombre { get; set; }
        public string abreviatura { get; set; }
    }

    public class riesgoEmitido_ramo
    {
        public string _id { get; set; }
        public string descripcion { get; set; }
        public string abreviatura { get; set; }
    }

    public class riesgoEmitido_asegurado
    {
        public string _id { get; set; }
        public string nombre { get; set; }
        public string abreviatura { get; set; }
    }

    public class riesgoEmitido_config
    {
        public string _id { get; set; }
        public config_opcionesReporte opcionesReporte { get; set; }
        public config_compania compania { get; set; }
        public string user { get; set; }
    }

    public class config_opcionesReporte
    {
        public string subTitulo { get; set; }
        public bool mostrarColores { get; set; }
        public bool formatoExcel { get; set; }
    }

    public class config_compania
    {
        public string _id { get; set; }
        public string nombre { get; set; }
    }

    // ---------------------------------------------------------------------------------------------
    // usamos esta clase para pasar los registros al report; la idea es convertir los objetos como: 
    // compania: { nombre, alias } en compania-nombre; el objeto que usa el report no puede tener 
    // estos tipos complejos ... 
    public class riesgoEmitido_report
    {
        public int numero { get; set; }
        public string moneda { get; set; }
        public string monedaSimbolo { get; set; }
        public string cedente { get; set; }
        public string compania { get; set; }
        public string companiaAbreviatura { get; set; }
        public string ramo { get; set; }
        public string ramoAbreviatura { get; set; }
        public string asegurado { get; set; }
        public string estado { get; set; }
        public int movimiento { get; set; }
        public DateTime fechaEmision { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }

        public decimal valorARiesgo { get; set; }
        public decimal sumaAsegurada { get; set; }
        public decimal prima { get; set; }
        public decimal ordenPorc { get; set; }
        public decimal sumaReasegurada { get; set; }
        public decimal primaBruta { get; set; }
        public decimal comision { get; set; }
        public decimal impuesto { get; set; }
        public decimal corretaje { get; set; }
        public decimal impuestoSobrePN { get; set; }
        public decimal totalCostos { get; set; }
        public decimal primaNeta { get; set; }

        // aparentemente, un método como éste, en la clase, es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<riesgoEmitido_report> get_RiesgosEmitidos_report()
        {
            List<riesgoEmitido_report> list = new List<riesgoEmitido_report>();
            return list;
        }
    }
}