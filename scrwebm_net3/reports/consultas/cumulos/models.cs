using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.consultas.cumulos
{
    public class cumulo_item
    {
        public ObjectId _id { get; set; }
        public DateTime fechaEmision { get; set; }
        public string cia { get; set; }
        public string compania { get; set; }
        public string ramo { get; set; }
        public string entityId { get; set; }
        public string subEntityId { get; set; }
        public string numero { get; set; }
        public string moneda { get; set; }

        public item_moneda monedas { get; set; }
        public item_compania companias { get; set; }
        public item_ramo ramos { get; set; }
        public item_cumulo cumulos { get; set; }
        public item_tipoCumulo tiposCumulo { get; set; }

        public string user { get; set; }      // source.origen + source.numero 

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<cumulo_item> get_Cumulos()
        {
            List<cumulo_item> list = new List<cumulo_item>();
            return list;
        }
    }

    public class item_moneda
    {
        public string descripcion { get; set; }
        public string simbolo { get; set; }
    }

    public class item_compania
    {
        public string nombre { get; set; }
        public string abreviatura { get; set; }
    }

    public class item_ramo
    {
        public string descripcion { get; set; }
        public string abreviatura { get; set; }
    }

    public class item_cumulo
    {
        public string origen { get; set; }
        public string tipoCumulo { get; set; }
        public string zona { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal valorARiesgo { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal sumaAsegurada { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaSeguro { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal montoAceptado { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaAceptada { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal cesionCuotaParte { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaCesionCuotaParte { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal cesionExcedente { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaCesionExcedente { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal cesionFacultativo { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaCesionFacultativo { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal cumulo { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal primaCumulo { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public decimal montoCedido { get; set; }
    }

    public class item_tipoCumulo
    {
        public string descripcion { get; set; }
        public string abreviatura { get; set; }
        public item_zona zonas { get; set; }
    }

    public class item_zona
    {
        public string descripcion { get; set; }
        public string abreviatura { get; set; }
    }

    public class cumulos_config
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
    // estos tipos complejos ... además, no todos los campos que se graban al collection en mongo 
    // son usados por el report. Por este motivo, aprovechamos para transformar el contenido en mongo 
    // a un contenido adecuado para el report 
    public class cumulo_report
    {
        public string tipoCumulo { get; set; }
        public string tipoCumuloAbreviatura { get; set; }
        public string moneda { get; set; }
        public string monedaSimbolo { get; set; }
        public string compania { get; set; }
        public string companiaAbreviatura { get; set; }
        public string ramo { get; set; }
        public string ramoAbreviatura { get; set; }
        public string zona { get; set; }
        public string zonaAbreviatura { get; set; }
        public string origen { get; set; }
        public string numero { get; set; }

        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }

        public decimal valorARiesgo { get; set; }
        public decimal sumaAsegurada { get; set; }
        public decimal primaSeguro { get; set; }
        public decimal montoAceptado { get; set; }
        public decimal primaAceptada { get; set; }
        public decimal cesionCuotaParte { get; set; }
        public decimal primaCesionCuotaParte { get; set; }
        public decimal cesionExcedente { get; set; }
        public decimal primaCesionExcedente { get; set; }
        public decimal cesionFacultativo { get; set; }
        public decimal primaCesionFacultativo { get; set; }
        public decimal cumulo { get; set; }
        public decimal primaCumulo { get; set; }
        public decimal montoCedido { get; set; }

        // aparentemente, un método como éste, en la clase, es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<cumulo_report> get_cumulos_report()
        {
            List<cumulo_report> list = new List<cumulo_report>();
            return list;
        }
    }
}