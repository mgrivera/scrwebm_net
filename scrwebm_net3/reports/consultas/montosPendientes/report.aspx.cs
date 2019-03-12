using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.consultas.montosPendientes
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
                // establecemos una conexión a mongodb ... 
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
                    errorMessage = "Error al intentar establecer una conexión a la base de datos (mongo), de nombre: '" + 
                                   database.DatabaseNamespace.DatabaseName + "'.<br /> El mensaje de error es: " + ex.Message;
                    ErrMessage_Cell.InnerHtml = errorMessage;
                    return;
                }

                switch (report)
                {
                    case "montosPendientes":
                        {
                            errorMessage = "";

                            if (!montosPendientes_consulta_report(database, userID, out errorMessage))
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

        private bool montosPendientes_consulta_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("consulta_montosPendientes");
            var collection_config = database.GetCollection<BsonDocument>("consulta_montosPendientes_config");

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
                               "en la tabla específica que debe existir para ello, en la base de datos (mongo) '" + 
                               database.DatabaseNamespace.DatabaseName + "'.<br /> " + 
                               "Por favor revise.<br />" +
                               "El mensaje específico del error obtenido es: " + ex.Message;
                return false;
            }


            var cursor = collection.Find(filter).ToCursor();
            int cantidadRegistros = 0;

            List<montoPendiente_item> reportItems = new List<montoPendiente_item>();
            montoPendiente_item reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                var cantidadPagosParciales = document.GetValue("cantidadPagosParciales", 0);
                var montoPendiente = document.GetValue("montoPendiente", 0);
       
                // los valores pueden venir en nulls (en realidad, BsonNull), porque existen pero en null; de ser así, los convertimos a 0 
                short cantidadPagosParciales2 = Convert.ToInt16(cantidadPagosParciales.IsBsonNull ? 0 : cantidadPagosParciales);
                decimal montoPendiente2 = Convert.ToDecimal(montoPendiente.IsBsonNull ? 0 : montoPendiente);

                reportItem = new montoPendiente_item
                {
                    monedaDescripcion = document["monedaDescripcion"].AsString,
                    monedaSimbolo = document["monedaSimbolo"].AsString,
                    companiaNombre = document["companiaNombre"].AsString,
                    companiaAbreviatura = document["companiaAbreviatura"].AsString,
                    ramoDescripcion = document["ramoDescripcion"].AsString,
                    ramoAbreviatura = document["ramoAbreviatura"].AsString,
                    aseguradoAbreviatura = document["aseguradoAbreviatura"].AsString,
                    suscriptorAbreviatura = document["suscriptorAbreviatura"].AsString,

                    origen = document["source"]["origen"].AsString + "-" + document["source"]["numero"].AsString,
                    cuota_numero = Convert.ToInt16(document["cuota"]["numero"]),
                    cuota_cantidad = Convert.ToInt16(document["cuota"]["cantidad"]),
                    cuota_fecha = Convert.ToDateTime(document["cuota"]["fecha"]),
                    cuota_fechaVencimiento = Convert.ToDateTime(document["cuota"]["fechaVencimiento"]),
                    cuota_monto = Convert.ToDecimal(document["cuota"]["monto"]),

                    cantidadPagosParciales = cantidadPagosParciales2,
                    montoPendiente = montoPendiente2
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
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/montosPendientes/report-excel.rdlc";
            } else
            {
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/montosPendientes/report.rdlc";
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