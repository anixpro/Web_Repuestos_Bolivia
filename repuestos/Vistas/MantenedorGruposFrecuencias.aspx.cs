using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


public partial class Vistas_MantenedorGruposFrecuencias : System.Web.UI.Page
{
    SqlConnection con;
    SqlCommand cmd;
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //LlenaFrecuencias(txtCodigo.Text);
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        ConsultaFrecuencia(txtCodigo.Text);
    }

    private void LlenaFrecuencias(string codigo)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_frecuencias";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar, 15).Value = txtCodigo.Text;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvFrecuencias.DataSource = dt;
            dgvFrecuencias.DataBind();

            con.Close();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    private void ConsultaFrecuencia(string codigo)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_sku_frecuencias";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar, 15).Value = txtCodigo.Text;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvFrecuencias.DataSource = dt;
            dgvFrecuencias.DataBind();

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
        dgvFrecuencias.EditIndex = e.NewEditIndex;
        if (txtCodigo.Text != "")
        {
            LlenaFrecuencias(txtCodigo.Text);
        }
        else
        {
            LlenaFrecuencias("");
        }
    }

    protected void dgvFrecuencias_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        
        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = dgvFrecuencias.Rows[indice];

        TextBox txtFrecuencia   = (TextBox)dgvFrecuencias.Rows[indice].FindControl("txtFrecuencia");
        TextBox txtGrupoTecnico = (TextBox)dgvFrecuencias.Rows[indice].FindControl("txtGrupoTecnico");

        string cod = row.Cells[0].Text;

        ActualizaDatos(cod, txtFrecuencia.Text, txtGrupoTecnico.Text);
        dgvFrecuencias.EditIndex = -1;
        LlenaFrecuencias("");
    }

    protected void dgvFrecuencias_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvFrecuencias.EditIndex = -1;
        if (txtCodigo.Text != "")
        {
            LlenaFrecuencias(txtCodigo.Text);
        }
        else
        {
            LlenaFrecuencias("");
        }
    }
    protected void dgvFrecuencias_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvFrecuencias.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        dgvFrecuencias.PageIndex = e.NewPageIndex;
        LlenaFrecuencias(txtCodigo.Text);
    }


    private void ActualizaDatos(string codigo, string frecuencia, string grupotecnico)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_actualiza_maestro_frecuencia";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar, 35).Value = codigo;
            cmd.Parameters.Add("@i_frecuencia", SqlDbType.VarChar, 1).Value = frecuencia;
            cmd.Parameters.Add("@i_grupotecnico", SqlDbType.VarChar, 5).Value = grupotecnico;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvFrecuencias.DataSource = dt;
            dgvFrecuencias.DataBind();

            con.Close();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }
}