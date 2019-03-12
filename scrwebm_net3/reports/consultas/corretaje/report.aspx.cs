using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.consultas.corretaje
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
                    case "corretaje":
                        {
                            errorMessage = "";

                            if (!corretaje_consulta_report(database, userID, out errorMessage))
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

        private bool corretaje_consulta_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("consulta_corretaje");
            var collection_config = database.GetCollection<BsonDocument>("consulta_corretaje_config");

            var filter = Builders<BsonDocument>.Filter.Eq("user", userID);

            var config = collection_config.Find(filter).FirstOrDefault();

            string nombreEmpresaSeleccionada = "";
            string subTituloReporte = "";
            bool? mostrarColores = false;
            bool? formatoExcel = false;

            try
            {
                var opcionesReporte = config["opcionesReporte"].AsBsonDocument;
                nombreEmpresaSeleccionada = config["compania"]["nombre"].AsString;

                subTituloReporte = opcionesReporte.GetValue("subTitulo", "").ToString(); 

                mostrarColores = opcionesReporte.GetValue("mostrarColores", false).ToBoolean();
                formatoExcel = opcionesReporte.GetValue("formatoExcel", false).ToBoolean();
            }
            catch (Exception ex)
            {
                errorMessage = "Error: se ha producido un error al intentar leer la 'configuración' para la ejecución del reporte, " +
                               "en la tabla específica que debe existir para ello. Por favor revise.<br />" +
                               "El mensaje específico del error obtenido es: " + ex.Message;
                return false;
            }


            var cursor = collection.Find(filter).ToCursor();
            int cantidadRegistros = 0;

            List<corretaje_item> reportItems = new List<corretaje_item>();
            corretaje_item reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                var montoPorPagar = document.GetValue("montoPorPagar", 0);
                var montoCorretaje = document.GetValue("montoCorretaje", 0);
                var montoCobrado = document.GetValue("montoCobrado", 0);
                var montoPagado = document.GetValue("montoPagado", 0);

                // los valores pueden venir en nulls (en realidad, BsonNull), porque existen pero en null; de ser así, los convertimos a 0 
                decimal montoPorPagar2 = Convert.ToDecimal(montoPorPagar.IsBsonNull ? 0 : montoPorPagar);
                decimal montoCorretaje2 = Convert.ToDecimal(montoCorretaje.IsBsonNull ? 0 : montoCorretaje);
                decimal montoCobrado2 = Convert.ToDecimal(montoCobrado.IsBsonNull ? 0 : montoCobrado);
                decimal montoPagado2 = Convert.ToDecimal(montoPagado.IsBsonNull ? 0 : montoPagado);

                reportItem = new corretaje_item
                {
                    monedaDescripcion = document["monedaDescripcion"].AsString,
                    monedaSimbolo = document["monedaSimbolo"].AsString,
                    companiaNombre = document["companiaNombre"].AsString,
                    companiaAbreviatura = document["companiaAbreviatura"].AsString,

                    // estos valores pueden venir en null ... 
                    ramoDescripcion = document["ramoDescripcion"].IsBsonNull ? "Indef" : document.GetValue("ramoDescripcion", "Indef").ToString(),
                    ramoAbreviatura = document["ramoAbreviatura"].IsBsonNull ? "Indef" : document.GetValue("ramoAbreviatura", "Indef").ToString(),
                    aseguradoAbreviatura = document["aseguradoAbreviatura"].IsBsonNull ? "Indef" : document.GetValue("aseguradoAbreviatura", "Indef").ToString(),
                    suscriptorAbreviatura = document["suscriptorAbreviatura"].IsBsonNull ? "Indef" : document.GetValue("suscriptorAbreviatura", "Indef").ToString(),

                    origen = document["origen"].AsString,
                    cuota_numero = Convert.ToInt16(document["numero"]),
                    cuota_cantidad = Convert.ToInt16(document["cantidad"]),
                    cuota_fechaEmision = Convert.ToDateTime(document["fechaEmision"]),
                    cuota_fechaCuota = Convert.ToDateTime(document["fechaCuota"]),
                    cuota_fechaVencimiento = Convert.ToDateTime(document["fechaVencimiento"]),
                    cuota_monto = Convert.ToDecimal(document["montoCuota"]),

                    montoPorPagar = montoPorPagar2, 
                    montoCorretaje = montoCorretaje2, 
                    montoCobrado = montoCobrado2, 
                    montoPagado = montoPagado2
                };
 
                reportItems.Add(reportItem);
                cantidadRegistros++;
            }

            if (cantidadRegistros == 0)
            {
                errorMessage = "No existe información para mostrar el reporte " +
                    "que Ud. ha requerido. <br /><br /> Probablemente Ud. no ha aplicado un " +
                    "filtro y seleccionado información aún.";
                return false;
            }


            if (formatoExcel.HasValue && formatoExcel.Value)
            {
                // la idea de este formato es que el usuario pueda convertir el report a Excel y los rows queden bastante adecuados ... 
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/corretaje/report-excel.rdlc";
            } else
            {
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/corretaje/report.rdlc";
            }
            

            ReportDataSource myReportDataSource = new ReportDataSource();

            myReportDataSource.Name = "DataSet1";
            myReportDataSource.Value = reportItems;

            ReportViewer1.LocalReport.DataSources.Add(myReportDataSource);

            ReportParameter[] MyReportParameters = {
                                                       new ReportParameter("nombreEmpresaSeleccionada", nombreEmpresaSeleccionada),
                                                       new ReportParameter("subTituloReporte", subTituloReporte),
                                                       new ReportParameter("mostrarColores", mostrarColores.ToString())
                                                   };

            ReportViewer1.LocalReport.SetParameters(MyReportParameters);

            ReportViewer1.LocalReport.Refresh();

            return true;
        }
    }
}