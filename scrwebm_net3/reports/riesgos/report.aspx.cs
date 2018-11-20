using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.riesgos
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
                    case "riesgosEmitidos":
                        {
                            errorMessage = "";

                            if (!riesgosEmitidos_report(database, userID, out errorMessage))
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

        private bool riesgosEmitidos_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("temp_consulta_riesgosEmitidos");
            var collection_config = database.GetCollection<BsonDocument>("temp_consulta_riesgosEmitidos_config");

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

                subTituloReporte = opcionesReporte["subTitulo"].AsString;

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

            List<riesgoEmitido> reportItems = new List<riesgoEmitido>();
            riesgoEmitido reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                var sumaAsegurada = document.GetValue("sumaAsegurada", 0); 
                var nuestraOrdenPorc = document.GetValue("nuestraOrdenPorc", 0);
                var sumaReasegurada = document.GetValue("sumaReasegurada", 0);
                var prima = document.GetValue("prima", 0);
                var primaBruta = document.GetValue("primaBruta", 0);
                var comMasImp = document.GetValue("comMasImp", 0);
                var primaNeta = document.GetValue("primaNeta", 0);
                var corretaje = document.GetValue("corretaje", 0);

                // los valores pueden venir en nulls (en realidad, BsonNull), porque existen pero en null; de ser así, los convertimos a 0 
                decimal sumaAsegurada2 = Convert.ToDecimal(sumaAsegurada.IsBsonNull ? 0 : sumaAsegurada);
                decimal nuestraOrdenPorc2 = Convert.ToDecimal(nuestraOrdenPorc.IsBsonNull ? 0 : nuestraOrdenPorc);
                decimal sumaReasegurada2 = Convert.ToDecimal(sumaReasegurada.IsBsonNull ? 0 : sumaReasegurada);
                decimal prima2 = Convert.ToDecimal(prima.IsBsonNull ? 0 : prima);
                decimal primaBruta2 = Convert.ToDecimal(primaBruta.IsBsonNull ? 0 : primaBruta);
                decimal comMasImp2 = Convert.ToDecimal(comMasImp.IsBsonNull ? 0 : comMasImp);
                decimal primaNeta2 = Convert.ToDecimal((primaNeta.IsBsonNull ? 0 : primaNeta));
                decimal corretaje2 = Convert.ToDecimal(corretaje.IsBsonNull ? 0 : corretaje);


                reportItem = new riesgoEmitido
                {
                    numero = Convert.ToInt16(document["numero"]),
                    estado = document["estado"].AsString,
                    desde = document["desde"].ToUniversalTime(),
                    hasta = document["hasta"].ToUniversalTime(),
                    monedaDescripcion = document["monedaDescripcion"].AsString,
                    monedaSimbolo = document["monedaSimbolo"].AsString,
                    companiaNombre = document["companiaNombre"].AsString,
                    companiaAbreviatura = document["companiaAbreviatura"].AsString,

                    ramoDescripcion = document["ramoDescripcion"].AsString,
                    ramoAbreviatura = document["ramoAbreviatura"].AsString,
                    asegurado = document["asegurado"].AsString,
                    nombreAsegurado = document["nombreAsegurado"].AsString, 
                    suscriptor = document["suscriptor"].AsString,

                    sumaAsegurada = sumaAsegurada2, 
                    nuestraOrdenPorc = nuestraOrdenPorc2,
                    sumaReasegurada = sumaReasegurada2,
                    prima = prima2,
                    primaBruta = primaBruta2,
                    comMasImp = comMasImp2,
                    primaNeta = primaNeta2,
                    corretaje = corretaje2
                };

                // si los valores son diferentes a null, los convertimos a 

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
                ReportViewer1.LocalReport.ReportPath = "reports/riesgos/report-excel.rdlc";
            } else
            {
                ReportViewer1.LocalReport.ReportPath = "reports/riesgos/report.rdlc";
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