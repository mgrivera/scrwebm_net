
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Reporting.WebForms;

namespace scrwebm_net3.reports.primasEmitidas.facultativo.reaseguradores
{
    public partial class report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string errorMessage = "";

                if (Request.QueryString["user"] == null ||
                    Request.QueryString["report"] == null)
                {
                    ErrMessage_Cell.InnerHtml = "Aparentemente, los parámetros pasados a esta página no son correctos.<br /><br />" +
                                                "Los parámetros que reciba esta página deben ser correctos para su ejecución pueda ser posible.";
                    return;
                }

                string userID = Request.QueryString["user"].ToString();
                string report = Request.QueryString["report"].ToString();

                // ----------------------------------------------------------------------------------------------
                // establecemos una conexión a mongodb; específicamente, a la base de datos del programa contabM; 
                string scrwebm_mongodb_connection = System.Web.Configuration.WebConfigurationManager.AppSettings["scrwebm_mongodb_connectionString"];
                string scrwebm_mongodb_name = System.Web.Configuration.WebConfigurationManager.AppSettings["scrwebm_mongodb_name"];

                var client = new MongoClient(scrwebm_mongodb_connection);
                var database = client.GetDatabase(scrwebm_mongodb_name);

                try
                {
                    // --------------------------------------------------------------------------------------------------------------------------
                    // solo para que ocura una exception si mongo no está iniciado ... nótese que antes, cuando establecemos mongo, no ocurre un 
                    // exception si mongo no está iniciado ...  
                    var collection = database.GetCollection<BsonDocument>("empresasUsuarias");
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", "@@@xxxyyyzzzAAABBBCCC@@@");
                    var result = collection.DeleteMany(filter);
                }
                catch (Exception ex)
                {
                    errorMessage = "Error al intentar establecer una conexión a la base de datos (mongo) de 'scrwebm'; el mensaje de error es: " + ex.Message;
                    ErrMessage_Cell.InnerHtml = errorMessage;
                    return;
                }

                switch (report)
                {
                    case "primasEmitidasReaseguradores":
                        {
                            errorMessage = "";

                            if (!primasEmitidas_reaseguradores_report(database, userID, out errorMessage))
                            {
                                errorMessage = "Error: ha ocurrido un error al intentar obtener el reporte: " + errorMessage;
                                ErrMessage_Cell.InnerHtml = errorMessage;
                            }
                            break;
                        }
                    default:
                        {
                            errorMessage = "Error: el parámetro 'report' indicado a esta página no corresponde a alguno de los establecidos.";
                            ErrMessage_Cell.InnerHtml = errorMessage;

                            break;
                        }
                }
            }
        }








        private bool primasEmitidas_reaseguradores_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<riesgoEmitido>("temp_consulta_riesgosEmitidosReaseguradores");
            var collection_config = database.GetCollection<riesgoEmitido_config>("temp_consulta_riesgosEmitidosReaseguradores_config");

            var config = collection_config.AsQueryable().Where(c => c.user == userID).FirstOrDefault();

            if (config == null) 
            {
                errorMessage = "Error: se ha producido un error al intentar leer la 'configuración' para la ejecución del reporte, " +
                               "en la tabla específica que debe existir para ello. Por favor revise.<br />" +
                               "Aparentemente, no existe un registro de configuración para este reporte en la baee de datos."; 
                return false;
            }


            var primasEmitidas = collection.AsQueryable().Where(r => r.user == userID).ToList(); 

            if (primasEmitidas.Count() == 0)
            {
                errorMessage = "No existe información para mostrar el reporte " +
                    "que Ud. ha requerido. <br /><br /> Probablemente Ud. no ha aplicado un " +
                    "filtro y seleccionado información aún.";
                return false;
            }

            List<riesgoEmitido_report> report_list = new List<riesgoEmitido_report>(); 

            foreach (var item in primasEmitidas)
            {
                riesgoEmitido_report report_item = new riesgoEmitido_report()
                {
                    numero = item.numero,
                    moneda = item.moneda.descripcion,
                    monedaSimbolo = item.moneda.simbolo, 
                    cedente = item.cedente.abreviatura,
                    compania = item.compania.nombre,
                    companiaAbreviatura = item.compania.abreviatura, 
                    ramo = item.ramo.descripcion,
                    ramoAbreviatura = item.ramo.abreviatura, 
                    asegurado = item.asegurado.abreviatura,
                    estado = item.estado,
                    movimiento = item.movimiento,
                    fechaEmision = item.fechaEmision,
                    desde = item.desde,
                    hasta = item.hasta,

                    valorARiesgo = item.valorARiesgo,
                    sumaAsegurada = item.sumaAsegurada,
                    prima = item.prima,
                    ordenPorc = item.ordenPorc,
                    sumaReasegurada = item.sumaReasegurada,
                    primaBruta = item.primaBruta,
                    comision = item.comision,
                    impuesto = item.impuesto,
                    corretaje = item.corretaje,
                    impuestoSobrePN = item.impuestoSobrePN,
                    primaNeta = item.primaNeta
                };

                decimal costos = report_item.comision + report_item.impuestoSobrePN + report_item.impuesto + report_item.corretaje;
                report_item.totalCostos = costos; 

                report_list.Add(report_item); 
            }


            if (config.opcionesReporte.formatoExcel)
            {
                // la idea de este formato es que el usuario pueda convertir el report a Excel y los rows queden bastante adecuados ... 
                ReportViewer1.LocalReport.ReportPath = "reports/primasEmitidas/facultativo/reaseguradores/report-excel.rdlc";
            }
            else
            {
                ReportViewer1.LocalReport.ReportPath = "reports/primasEmitidas/facultativo/reaseguradores/report.rdlc";
            }
 
            ReportDataSource myReportDataSource = new ReportDataSource();

            myReportDataSource.Name = "DataSet1";
            myReportDataSource.Value = report_list;

            ReportViewer1.LocalReport.DataSources.Add(myReportDataSource);

            ReportParameter[] MyReportParameters = {
                                                       new ReportParameter("nombreEmpresaSeleccionada", config.compania.nombre),
                                                       new ReportParameter("subTituloReporte", config.opcionesReporte.subTitulo),
                                                       new ReportParameter("mostrarColores", config.opcionesReporte.mostrarColores.ToString())
                                                   };

            ReportViewer1.LocalReport.SetParameters(MyReportParameters);

            ReportViewer1.LocalReport.Refresh();

            return true;
        }
    }
}