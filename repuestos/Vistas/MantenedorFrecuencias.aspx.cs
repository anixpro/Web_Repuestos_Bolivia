using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

public partial class Vistas_MantenedorFrecuencias : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    SqlConnection con;
    SqlCommand cmd;
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LlenarComboMarcas();
            //LlenaFrecuencias();
        }
    }

    private void LlenarComboMarcas()
    {
        ddlMarca.AppendDataBoundItems = true;
        ddlMarca.Items.Add("Seleccione");
        ddlMarca.DataSource = _controlBD.CargaMarca();
        ddlMarca.DataMember = "Table";
        ddlMarca.DataValueField = "ID_MARCA";
        ddlMarca.DataTextField = "CLASE_MARCA";
        ddlMarca.DataBind();
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {

        if (ddlMarca.SelectedValue != "Seleccione")
        {
            LlenaFrecuencias(Convert.ToInt32(ddlMarca.SelectedValue));
        }  
    }

    private void LlenaFrecuencias(int marca)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_frecuencias_datos";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@id_Marca", SqlDbType.Int).Value = marca ;
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
        LlenaFrecuencias(Convert.ToInt32(ddlMarca.SelectedValue));
    }

    protected void dgvFrecuencias_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = dgvFrecuencias.Rows[indice];
        
        TextBox txtDescripcion = (TextBox)dgvFrecuencias.Rows[indice].FindControl("txtDescripcion");

        string cod = row.Cells[0].Text;

        HabilitaFrecuencia(cod, Convert.ToInt32(ddlMarca.SelectedValue),3, txtDescripcion.Text);

        dgvFrecuencias.EditIndex = -1;
        LlenaFrecuencias(Convert.ToInt32(ddlMarca.SelectedValue));
    }

    protected void dgvFrecuencias_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvFrecuencias.EditIndex = -1;
        LlenaFrecuencias(Convert.ToInt32(ddlMarca.SelectedValue));
    }

    protected void dgvFrecuencias_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "activa")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = dgvFrecuencias.Rows[indice];
            string cod = row.Cells[0].Text;
            string descripcion = row.Cells[1].Text;
            string activo = row.Cells[2].Text;

            if (activo == "NO")
            {
                HabilitaFrecuencia(cod, Convert.ToInt32(ddlMarca.SelectedValue), 1, descripcion);
            }
            else
            {
                HabilitaFrecuencia(cod, Convert.ToInt32(ddlMarca.SelectedValue),0, descripcion);
            }
        }
    }

    private void HabilitaFrecuencia(string codigo,int marca, int flag, string descripcion)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_frecuencia_actualiza";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar, 35).Value = codigo;
            cmd.Parameters.Add("@i_marca", SqlDbType.Int).Value = marca;
            cmd.Parameters.Add("@i_flag", SqlDbType.Int).Value = flag;
            cmd.Parameters.Add("@i_clase_frecuencia", SqlDbType.VarChar, 5).Value = descripcion;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();
            con.Close();

            LlenaFrecuencias(Convert.ToInt32(ddlMarca.SelectedValue));
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    protected void dgvFrecuencias_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvFrecuencias.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        dgvFrecuencias.PageIndex = e.NewPageIndex;
        LlenaFrecuencias(Convert.ToInt32(ddlMarca.SelectedValue));
    }
}