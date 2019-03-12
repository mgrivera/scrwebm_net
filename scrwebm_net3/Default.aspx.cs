using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace scrwebm_net3
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.errorMessageDiv.InnerHtml = "";
            this.errorMessageDiv.Visible = false;

            this.infoMessageDiv.InnerHtml = "";
            this.infoMessageDiv.Visible = false;

            if (!Page.IsPostBack)
            {
                string message = ""; 
                // ----------------------------------------------------------------------------------------------
                // establecemos una conexión a mongodb; específicamente, a la base de datos del programa contabM; 
                string scrwebm_mongodb_connection = System.Web.Configuration.WebConfigurationManager.AppSettings["scrwebm_mongodb_connectionString"];
                string scrwebm_mongodb_name = System.Web.Configuration.WebConfigurationManager.AppSettings["scrwebm_mongodb_name"];

                var client = new MongoClient(scrwebm_mongodb_connection);
                var database = client.GetDatabase(scrwebm_mongodb_name);

                bool mongoError = false; 

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
                    message = $"<br />Error al intentar establecer una conexión a la base de datos (mongo) {scrwebm_mongodb_name}; el mensaje de error es: <br />" + ex.Message;

                    this.errorMessageDiv.InnerHtml = message;
                    this.errorMessageDiv.Visible = true;

                    mongoError = true;
                }
                finally
                {
                    if (!mongoError)
                    {
                        message = $"<br />Ok, se ha establecido una conexión exitosa a la base de datos (mongo): <b><em>{scrwebm_mongodb_name}</b></em>, " + 
                                  $"usando el connection string: <br /> <b><em>{scrwebm_mongodb_connection}</b></em>";

                        this.infoMessageDiv.InnerHtml = message;
                        this.infoMessageDiv.Visible = true;
                    }
                }
            }
        }
    }
}