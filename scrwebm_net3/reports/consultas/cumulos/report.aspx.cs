using Microsoft.Reporting.WebForms;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3.reports.consultas.cumulos
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
                    case "cumulos":
                        {
                            errorMessage = "";

                            if (!cumulos_consulta_report(database, userID, out errorMessage))
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

        private bool cumulos_consulta_report(IMongoDatabase database, string userID, out string errorMessage)
        {
            errorMessage = "";

            var collection = database.GetCollection<BsonDocument>("consulta_cumulos");
            var collection_config = database.GetCollection<BsonDocument>("consulta_cumulos_config");

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

            List<cumulo_item> reportItems = new List<cumulo_item>();
            cumulo_item reportItem;

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                // todo lo que sigue es para protegernos de keys que pueden no venir en el documento ... 
                /// si el key en el documento (mongo) no existe, traemos cero ... 
                var valoresARiesgo = document.GetValue("valoresARiesgo", 0);
                var sumaAsegurada = document.GetValue("sumaAsegurada", 0);
                var prima = document.GetValue("prima", 0);
                var nuestraOrdenPorc = document.GetValue("nuestraOrdenPorc", 0);
                var sumaReasegurada = document.GetValue("sumaReasegurada", 0);
                var primaBruta = document.GetValue("primaBruta", 0);

                var indoleAbreviatura = document.GetValue("indoleAbreviatura", "");
                var tipoObjetoAseguradoAbreviatura = document.GetValue("tipoObjetoAseguradoAbreviatura", "");

                // los valores pueden venir en nulls (en realidad, BsonNull), porque existen pero en null; de ser así, los convertimos a cero ...  
                decimal valoresARiesgo2 = Convert.ToDecimal(valoresARiesgo.IsBsonNull ? 0 : valoresARiesgo);
                decimal sumaAsegurada2 = Convert.ToDecimal(sumaAsegurada.IsBsonNull ? 0 : sumaAsegurada);
                decimal prima2 = Convert.ToDecimal(prima.IsBsonNull ? 0 : prima);
                decimal nuestraOrdenPorc2 = Convert.ToDecimal(nuestraOrdenPorc.IsBsonNull ? 0 : nuestraOrdenPorc);
                decimal sumaReasegurada2 = Convert.ToDecimal(sumaReasegurada.IsBsonNull ? 0 : sumaReasegurada);
                decimal primaBruta2 = Convert.ToDecimal(primaBruta.IsBsonNull ? 0 : primaBruta);

                string indoleAbreviatura2 = Convert.ToString(indoleAbreviatura.IsBsonNull ? "" : indoleAbreviatura);
                string tipoObjetoAseguradoAbreviatura2 = Convert.ToString(tipoObjetoAseguradoAbreviatura.IsBsonNull ? "" : tipoObjetoAseguradoAbreviatura);
                
                reportItem = new cumulo_item
                {
                    monedaDescripcion = document["monedaDescripcion"].AsString,
                    monedaSimbolo = document["monedaSimbolo"].AsString,
                    tipoCumuloDescripcion = document["tipoCumuloDescripcion"].AsString,
                    tipoCumuloAbreviatura = document["tipoCumuloAbreviatura"].AsString,
                    zonaDescripcion = document["zonaDescripcion"].AsString,
                    zonaAbreviatura = document["zonaAbreviatura"].AsString,
                    cedenteAbreviatura = document["cedenteAbreviatura"].AsString,
                    ramoAbreviatura = document["ramoAbreviatura"].AsString,
                    indoleAbreviatura = indoleAbreviatura2,
                    tipoObjetoAseguradoAbreviatura = tipoObjetoAseguradoAbreviatura2,

                    origen = document["origen"].AsString,

                    desde = Convert.ToDateTime(document["desde"]),
                    hasta = Convert.ToDateTime(document["hasta"]),

                    valoresARiesgo = valoresARiesgo2,
                    sumaAsegurada = sumaAsegurada2, 
                    prima = prima2,
                    nuestraOrdenPorc = nuestraOrdenPorc2, 
                    sumaReasegurada = sumaReasegurada2,
                    primaBruta = primaBruta2
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
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/cumulos/report-excel.rdlc";
            } else
            {
                ReportViewer1.LocalReport.ReportPath = "reports/consultas/cumulos/report.rdlc";
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