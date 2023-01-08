using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

public partial class Vistas_MantenedorMarcasGT : System.Web.UI.Page
{
    SqlConnection con;
    SqlCommand cmd;
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LlenaFrecuencias();
        }
    }

    private void LlenaFrecuencias()
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_marcas_datos";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvMarcas.DataSource = dt;
            dgvMarcas.DataBind();

            con.Close();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }
    protected void dgvFrecuencias_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvMarcas.EditIndex = e.NewEditIndex;
        LlenaFrecuencias();
    }

    protected void dgvFrecuencias_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = dgvMarcas.Rows[indice];
        
        TextBox txtDescripcion = (TextBox)dgvMarcas.Rows[indice].FindControl("txtDescripcion");

        string cod = row.Cells[0].Text;

        HabilitaFrecuencia(cod, 3, txtDescripcion.Text);

        dgvMarcas.EditIndex = -1;
        LlenaFrecuencias();
    }

    protected void dgvFrecuencias_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvMarcas.EditIndex = -1;
        LlenaFrecuencias();
    }

    protected void dgvFrecuencias_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "activa")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = dgvMarcas.Rows[indice];
            string cod = row.Cells[0].Text;
            string descripcion = row.Cells[1].Text;
            string activo = row.Cells[2].Text;

            if (activo == "NO")
            {
                HabilitaFrecuencia(cod, 1, descripcion);
            }
            else
            {
                HabilitaFrecuencia(cod, 0, descripcion);
            }
        }
    }

    private void HabilitaFrecuencia(string codigo, int flag, string descripcion)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_marca_actualiza";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar, 35).Value = codigo;
            cmd.Parameters.Add("@i_flag", SqlDbType.Int).Value = flag;
            cmd.Parameters.Add("@i_clase_marca", SqlDbType.VarChar, 5).Value = descripcion;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();
            con.Close();

            LlenaFrecuencias();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    protected void dgvFrecuencias_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvMarcas.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        dgvMarcas.PageIndex = e.NewPageIndex;
        LlenaFrecuencias();
    }
}