
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.riesgos
{
    public class riesgoEmitido
    {
        public Int16 numero { get; set; }
        public string estado { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public string monedaDescripcion { get; set; }
        public string monedaSimbolo { get; set; }
        public string companiaNombre { get; set; }
        public string companiaAbreviatura { get; set; }
        public string ramoDescripcion { get; set; }
        public string ramoAbreviatura { get; set; }
        public string asegurado { get; set; }
        public string nombreAsegurado { get; set; }
        public string suscriptor { get; set; }

        public decimal sumaAsegurada { get; set; }
        public decimal nuestraOrdenPorc { get; set; }
        public decimal sumaReasegurada { get; set; }
        public decimal prima { get; set; }
        public decimal primaBruta { get; set; }
        public decimal comMasImp { get; set; }
        public decimal primaNeta { get; set; }
        public decimal corretaje { get; set; }
        public decimal primaNetaFinal { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<riesgoEmitido> get_RiesgosEmitidos()
        {
            List<riesgoEmitido> list = new List<riesgoEmitido>();
            return list;
        }
    }
}