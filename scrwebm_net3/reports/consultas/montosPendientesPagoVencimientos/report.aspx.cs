using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.consultas.montosPendientesPagoVencimientos
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
                    errorMessage = "Error al intentar establecer una conexión a la base de datos (mongo), de nombre: '" + 
                                   database.DatabaseNamespace.DatabaseName + "'.<br /> El mensaje de error es: " + ex.Message;
                    ErrMessage_Cell.InnerHtml = errorMessage;
                    return;
                }

                switch (report)
                {
                    case "montosPendientesPagoVencimientos":
                        {
                            errorMessage = "";

                            if (!montosPendientesPagoVencimientos_consulta_report(database, userID, out errorMessage))
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

        private bool montosPendientesPagoVencimientos_consulta_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("consulta_MontosPendientesPago_Vencimientos");
            var collection_config = database.GetCollection<BsonDocument>("consulta_MontosPendientesPago_Vencimientos_config");

            var filter = Builders<BsonDocument>.Filter.Eq("user", userID);

            var config = collection_config.Find(filter).FirstOrDefault();

            string nombreEmpresaSeleccionada = "";
            string subTituloReporte = "";
            bool? mostrarColores = false;
            bool? formatoExcel = false;

            DateTime fechaPendientesAl;                 // en base a esta fecha se calculan los vencimientos 
            DateTime fechaLeerHasta;                    // los montos (ie: cuotas) se leen hasta esa fecha 

            try
            {
                var opcionesReporte = config["opcionesReporte"].AsBsonDocument;
                nombreEmpresaSeleccionada = config["compania"]["nombre"].AsString;

                subTituloReporte = opcionesReporte.GetValue("subTitulo", "").ToString();

                mostrarColores = opcionesReporte.GetValue("mostrarColores", false).ToBoolean();
                formatoExcel = opcionesReporte.GetValue("formatoExcel", false).ToBoolean();

                fechaPendientesAl = Convert.ToDateTime(opcionesReporte.GetValue("fechaPendientesAl", new DateTime()));   
                fechaLeerHasta = Convert.ToDateTime(opcionesReporte.GetValue("fechaLeerHasta", new DateTime()));
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

            List<montoPendientePago_item> reportItems = new List<montoPendientePago_item>();
            montoPendientePago_item reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                // las fechas pueden venir en nulls ... revisamos antes de intentar convertirlas ... 
                DateTime? fecha = null;
                DateTime? fechaEmision = null;
                DateTime? fechaVencimiento = null;

                if (!document["fecha"].IsBsonNull) { fecha = Convert.ToDateTime(document["fecha"]); }; 
                if (!document["fechaEmision"].IsBsonNull) { fechaEmision = Convert.ToDateTime(document["fechaEmision"]); };
                if (!document["fechaVencimiento"].IsBsonNull) { fechaVencimiento = Convert.ToDateTime(document["fechaVencimiento"]); };

                reportItem = new montoPendientePago_item
                {
                    monedaDescripcion = document["monedaDescripcion"].AsString,
                    monedaSimbolo = document["monedaSimbolo"].AsString,
                    companiaNombre = document["companiaNombre"].AsString,
                    companiaAbreviatura = document["companiaAbreviatura"].AsString,
                    aseguradoAbreviatura = document["aseguradoAbreviatura"].AsString,
                    suscriptorAbreviatura = document["suscriptorAbreviatura"].AsString,

                    origen = document["origen"].AsString,
                    numero = Convert.ToInt16(document["numero"]),
                    cantidad = Convert.ToInt16(document["cantidad"]),

                    diasPendientes = Convert.ToInt32(document["diasPendientes"]),
                    diasVencidos = Convert.ToInt32(document["diasVencidos"]),

                    fecha = fecha,
                    fechaEmision = fechaEmision,
                    fechaVencimiento = fechaVencimiento,

                    montoCuota = Convert.ToDecimal(document["montoCuota"]),
                    montoYaPagado = Convert.ToDecimal(document["montoYaPagado"]),
                    resta = Convert.ToDecimal(document["resta"]),

                    montoYaCobrado = Convert.ToDecimal(document["montoYaCobrado"]),
                };

                // calculamos el vencimiento para separar (agrupar) en el reporte de acuerdo a la cantidad de días de 
                // vencimiento: 1 a 30, 31 a 60, ... 
                if (reportItem.diasVencidos < 0)
                {
                    reportItem.vencimiento = "5: vencido";
                    reportItem.vencimientoAbreviatura = "5: vencido";
                } else
                {
                    if (reportItem.diasPendientes <= 30)
                    {
                        reportItem.vencimiento = "1: 0 a 30 días por vencer";
                        reportItem.vencimientoAbreviatura = "1: 0 a 30";
                    }
                    else if (reportItem.diasPendientes >= 31 && reportItem.diasPendientes <= 60)
                    {
                        reportItem.vencimiento = "2: 31 a 60 días por vencer";
                        reportItem.vencimientoAbreviatura = "2: 31 a 60";
                    }
                    else if (reportItem.diasPendientes >= 61 && reportItem.diasPendientes <= 90)
                    {
                        reportItem.vencimiento = "3: 61 a 90 días por vencer";
                        reportItem.vencimientoAbreviatura = "3: 61 a 90";
                    } 
                    else
                    {
                        reportItem.vencimiento = "4: más de 90 días por vencer";
                        reportItem.vencimientoAbreviatura = "4: más de 90";
                    }
                        
                }

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
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/montosPendientesPagoVencimientos/report-excel.rdlc";
            }
            else
            {
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/montosPendientesPagoVencimientos/report.rdlc";
            }


            ReportDataSource myReportDataSource = new ReportDataSource();

            myReportDataSource.Name = "DataSet1";
            myReportDataSource.Value = reportItems;

            ReportViewer1.LocalReport.DataSources.Add(myReportDataSource);

            ReportParameter[] MyReportParameters = {
                                                       new ReportParameter("nombreEmpresaSeleccionada", nombreEmpresaSeleccionada),
                                                       new ReportParameter("subTituloReporte", subTituloReporte),
                                                       new ReportParameter("mostrarColores", mostrarColores.ToString()),
                                                       new ReportParameter("fechaPendientesAl", fechaPendientesAl.ToString("dd-MMM-yy")),
                                                       new ReportParameter("fechaLeerHasta", fechaLeerHasta.ToString("dd-MMM-yy"))
                                                   };

            ReportViewer1.LocalReport.SetParameters(MyReportParameters);

            ReportViewer1.LocalReport.Refresh();

            return true;
        }
    }
}