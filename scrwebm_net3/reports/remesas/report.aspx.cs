using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.remesas
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
                    case "list":
                        {
                            errorMessage = "";

                            if (!remesas_list_report(database, userID, out errorMessage))
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

        private bool remesas_list_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("temp_consulta_list_remesas");
            var collection_config = database.GetCollection<BsonDocument>("temp_consulta_list_remesas_config");

            var filter = Builders<BsonDocument>.Filter.Eq("user", userID);

            var config = collection_config.Find(filter).FirstOrDefault();

            string nombreEmpresaSeleccionada = "";
            string subTituloReporte = "";
            bool? mostrarColores = false;

            try
            {
                var opcionesReporte = config["opcionesReporte"].AsBsonDocument;
                nombreEmpresaSeleccionada = config["compania"]["nombre"].AsString;

                subTituloReporte = opcionesReporte["subTituloReporte"].AsString;
                mostrarColores = opcionesReporte.GetValue("mostrarColores", false).ToBoolean();
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

            List<remesaList> reportItems = new List<remesaList>();
            remesaList reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                var monto = document.GetValue("monto", 0);
                decimal monto2 = Convert.ToDecimal(monto.IsBsonNull ? 0 : monto);

                DateTime? fechaCerrada = null;

                if (!document["fechaCerrada"].IsBsonNull)
                {
                    fechaCerrada = document["fechaCerrada"].ToUniversalTime();
                }

                reportItem = new remesaList
                {
                    remesaID = Convert.ToInt16(document["numero"]),
                    compania = document["compania"].AsString,
                    miSu = document["miSu"].AsString,
                    moneda = document["moneda"].AsString,
                    monto = monto2,
                    fecha = document["fecha"].ToUniversalTime(),
                    fechaCerrada = fechaCerrada, 
                    observaciones = !document["observaciones"].IsBsonNull ? document["observaciones"].AsString : "",

                    // estos datos vienen en un 'sub' object, pero los 'aplanamos' para poder pasarlos al reporte (???) 
                    operacion_cantPagos = Convert.ToInt16(document["infoOperaciones"]["cantPagos"]),
                    operacion_sumatoriaMontoPagos = Convert.ToDecimal(document["infoOperaciones"]["sumatoriaMontoPagos"]),
                    operacion_pagosMismaMonedaDeLaRemesa = Convert.ToBoolean(document["infoOperaciones"]["pagosMismaMonedaDeLaRemesa"]),
                    operacion_pagosMultiMoneda = Convert.ToBoolean(document["infoOperaciones"]["pagosMultiMoneda"]),
                    operacion_diferenciaMontoRemesaPagos = Convert.ToDecimal(document["infoOperaciones"]["diferenciaMontoRemesaPagos"]),
                    operacion_completo = Convert.ToBoolean(document["infoOperaciones"]["completo"]),
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


            ReportViewer1.LocalReport.ReportPath = "reports/remesas/report.rdlc";

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