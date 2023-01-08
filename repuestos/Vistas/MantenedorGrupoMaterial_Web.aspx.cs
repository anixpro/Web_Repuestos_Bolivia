using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using log4net.Config;


public partial class Vistas_MantenedorGrupoMaterial_Web : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ControlBD));
    protected void Page_Load(object sender, EventArgs e)
    {
        completaGrillaUsuarios();
            
    }


    private void completaGrillaUsuarios()
    {
       
        sqlDGrupMaterial.SelectCommand = "select * from dbo.Grupo_Material_Web";
        GridViewGMWeb.DataBind();

        
     }

   
    protected void GridViewGMWeb_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "activa")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GridViewGMWeb.Rows[indice];
            string cod = row.Cells[1].Text;
            string gm = row.Cells[3].Text;
            string activa = row.Cells[4].Text;

            if (activa == "NO")
            {

                string queryInsert = "update Grupo_Material_Web set flag_mg_web = 'SI' where codigo_gm_web = '" + cod + "'AND gm_web = '" + gm + "'";
                InsertarDatos(queryInsert);

                //sqlDGrupMaterial.SelectCommand = "updat Grupo_Material_Web set flag_mg_web = 'SI' where codigo_gm_web = '" + cod + "', gm_web = '" + gm + "'";
                GridViewGMWeb.DataBind();
            }
            else
            {

                string queryInsert = "update Grupo_Material_Web set flag_mg_web = 'NO' where codigo_gm_web = '" + cod + "' AND gm_web = '" + gm + "'";
                InsertarDatos(queryInsert);
                //sqlDGrupMaterial.SelectCommand = "updat Grupo_Material_Web set flag_mg_web = 'NO' where codigo_gm_web = '" + cod + "', gm_web = '" + gm + "'";
                GridViewGMWeb.DataBind();
            }      

        }
    }


    public Boolean InsertarDatos(string query)
    {
        Boolean returnValue = true;
        //Utilizando la cláusula using te aseguras de que liberarás los recursos una vez hayas terminado
        //de utilizar la conexión a la base de datos
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString))
        {
            try
            {
                //Abrimos la conexión a la base de datos
                conn.Open();
                //Creamos el comando que contendrá la query a ejecutar en el servidor
                SqlCommand _query = conn.CreateCommand();

                //Si vamos a ejecutar una consulta, especificamos como CommandType Text
                _query.CommandType = CommandType.Text;
                //Si vamos a ejecutar un procedimiento almacenado, especificamos StoredProcedure
                //query.CommandType= CommandType.StoredProcedure;
                //query.CommandText="nombreDelProcedimientoAlmacenado";

                //Especifiamos la query a ejecutar
                _query.CommandText = string.Format(query);

                //Como es un INSERT, la query no devuelve resultados, asi que ejecutamos un ExecuteNonQuery que nos
                //devuelve el número de filas afectadas
                int fil = _query.ExecuteNonQuery();

                if (fil > 0)
                {
                    // Todo OK
                }
                else
                {
                    returnValue = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error en [InsertarDatos]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + query + "]");
                returnValue = false;
            }
            finally
            {
                //Nos aseguramos de cerrar la conexión
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        return returnValue;
    }

}