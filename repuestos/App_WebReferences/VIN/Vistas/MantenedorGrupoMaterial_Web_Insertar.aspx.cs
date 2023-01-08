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

public partial class Vistas_MantenedorGrupoMaterial_Web_Insertar : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ControlBD));
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        String CodGMW = TxtGMWeb.Text;
        String CodGMDesc = TxtGMDesc.Text;
        String GrupoMat = TxtGrupoMaterial.Text;
        String Activo = DropDownList1.SelectedValue;

        if (CodGMW == "")
        {
            Label1.Text = "Debe ingresar dato, Grupo Material Web";
            Label1.Visible = true;
        }
        else
        {
            if (CodGMDesc == "")
            {
                Label1.Text = "Debe ingresar dato, Descripcion";
                Label1.Visible = true;
            }
            else
            {
                if (GrupoMat == "")
                {
                    Label1.Text = "Debe ingresar dato, Grupo Materiales SAP";
                    Label1.Visible = true;
                }
                else
                {
                     
                    if (Activo == "-1")
                    {
                        Label1.Text = "Debe seleccionar si esta activo o inactivo";
                        Label1.Visible = true;
                    }else
	                    {
                            string queryInsert = "INSERT Grupo_Material_Web ([Codigo_GM_Web],[Desc_GM_Web],[GM_Web],[Flag_MG_web])VALUES('" + CodGMW + "','" + CodGMDesc + "','" + GrupoMat + "','" + Activo +"')";
                            InsertarDatos(queryInsert);

                            TxtGMWeb.Text = "";
                            TxtGMDesc.Text = "";
                            TxtGrupoMaterial.Text = "";
                            DropDownList1.SelectedValue = "-1";

                            Label1.Text = "Sus Datos Fueron Ingresados Satisfactoriamente";
                            Label1.Visible = true;
                        }
                }
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