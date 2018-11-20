using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.cierre
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
                string scrwebm_mongodb_name = System.Web.Configuration.WebConfigurationManager.AppSettings["scrwebm_mongodb_name"];

                var client = new MongoClient("mongodb://localhost:27017");
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
                    case "cuentaCorriente":
                        {
                            errorMessage = "";

                            if (!cuentasCorrientes_report(database, userID, out errorMessage))
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

        private bool cuentasCorrientes_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("temp_consulta_cierreRegistro");
            var collection_config = database.GetCollection<BsonDocument>("temp_consulta_cierreRegistro_config");

            var filter = Builders<BsonDocument>.Filter.Eq("user", userID);

            var config = collection_config.Find(filter).FirstOrDefault();

            string periodoConsulta = "";
            string nombreEmpresaSeleccionada = "";
            string subTituloReporte = "";
            bool? mostrarColores = false;
            bool? cuentasCorrientes = false;

            try
            {
                var fechaInicialConsulta = config["fechaInicialPeriodo"].ToNullableUniversalTime();
                var fechaFinalConsulta = config["fechaFinalPeriodo"].ToNullableUniversalTime();
                var opcionesReporte = config["opcionesReporte"].AsBsonDocument;

                nombreEmpresaSeleccionada = config["compania"]["nombre"].AsString;

                // con GetValue se regresa un default, si el field no existe en el registro ... 
                subTituloReporte = opcionesReporte.GetValue("subTitulo", "").ToString();
                mostrarColores = opcionesReporte.GetValue("mostrarColores", false).ToBoolean();
                cuentasCorrientes = opcionesReporte.GetValue("cuentasCorrientes", false).ToBoolean();

                if (fechaInicialConsulta != null && fechaFinalConsulta != null)
                {
                    periodoConsulta = fechaInicialConsulta.Value.ToString("dd-MMM-yyyy") + " al " + fechaFinalConsulta.Value.ToString("dd-MMM-yyyy");
                }
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

            List<movimientoCuentaCorriente> reportItems = new List<movimientoCuentaCorriente>();
            movimientoCuentaCorriente reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                reportItem = new movimientoCuentaCorriente
                {
                    orden = Convert.ToInt16(document["orden"]),
                    moneda = document["moneda"]["descripcion"].AsString,
                    compania = document["compania"]["nombre"].AsString,
                    fecha = document["fecha"].ToUniversalTime(),
                    origen = document["origen"].AsString,
                    descripcion = document["descripcion"].AsString,
                    referencia = document["referencia"].AsString,
                    serie = document["serie"].AsNullableInt32,
                    monto = document["monto"].ToDecimal()
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

            if (cuentasCorrientes.HasValue && cuentasCorrientes.Value) 
                ReportViewer1.LocalReport.ReportPath = "reports/cierre/report_cuentasCorrientes.rdlc";
            else
                ReportViewer1.LocalReport.ReportPath = "reports/cierre/report.rdlc";

            ReportDataSource myReportDataSource = new ReportDataSource();

            myReportDataSource.Name = "DataSet1";
            myReportDataSource.Value = reportItems;

            ReportViewer1.LocalReport.DataSources.Add(myReportDataSource);

            ReportParameter[] MyReportParameters = {
                                                       new ReportParameter("periodoConsulta", periodoConsulta),
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