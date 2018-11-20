using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.consultas.cumulos
{
    public class cumulo_item
    {
        public string monedaDescripcion { get; set; }
        public string monedaSimbolo { get; set; }
        public string tipoCumuloDescripcion { get; set; }
        public string tipoCumuloAbreviatura { get; set; }
        public string zonaDescripcion { get; set; }
        public string zonaAbreviatura { get; set; }
        public string cedenteAbreviatura { get; set; }
        public string ramoAbreviatura { get; set; }
        public string indoleAbreviatura { get; set; }
        public string tipoObjetoAseguradoAbreviatura { get; set; }

        public string origen { get; set; }      // source.origen + source.numero 

        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }

        public decimal valoresARiesgo { get; set; }
        public decimal sumaAsegurada { get; set; }
        public decimal prima { get; set; }
        public decimal nuestraOrdenPorc { get; set; }
        public decimal sumaReasegurada { get; set; }
        public decimal primaBruta { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<cumulo_item> get_Cumulos()
        {
            List<cumulo_item> list = new List<cumulo_item>();
            return list;
        }
    }
}