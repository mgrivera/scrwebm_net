using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scrwebm_net3.reports.cierre
{
    public class movimientoCuentaCorriente
    {
        public Int16 orden { get; set; }
        public string moneda { get; set; }
        public string compania { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public string origen { get; set; }
        public string referencia { get; set; }
        public int? serie { get; set; }
        public decimal monto { get; set; }

        // aparentemente, un método como éste es necesario para que el report pueda 'ver' la clase como un datasource ... 
        public List<movimientoCuentaCorriente> get_movimientoCuentaCorriente()
        {
            List<movimientoCuentaCorriente> list = new List<movimientoCuentaCorriente>();
            return list;
        }
    }
}