using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

public partial class Vistas_MantenedorFobValores : System.Web.UI.Page
{
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    SendMail_helper _mail = new SendMail_helper();
    SQL_DevRec _SQL = new SQL_DevRec();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void dgvMateriales_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvMateriales.EditIndex = e.NewEditIndex;
        cargarGrilla();
    }

    protected void dgvMateriales_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvMateriales.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        dgvMateriales.PageIndex = e.NewPageIndex;
        cargarGrilla();
    }

    protected void dgvMateriales_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = dgvMateriales.Rows[indice];

        TextBox txtDescripcion  = (TextBox)dgvMateriales.Rows[indice].FindControl("txtDescripcion");
        TextBox txtValor        = (TextBox)dgvMateriales.Rows[indice].FindControl("txtValor");
        TextBox txtVolumen      = (TextBox)dgvMateriales.Rows[indice].FindControl("txtVolumen");
        TextBox txtUnidadMedida = (TextBox)dgvMateriales.Rows[indice].FindControl("txtUnidadMedida");
        TextBox txtMoneda       = (TextBox)dgvMateriales.Rows[indice].FindControl("txtMoneda");
        TextBox txtGrupoTecnico = (TextBox)dgvMateriales.Rows[indice].FindControl("txtGrupoTecnico");

        txtValor.Text   = txtValor.Text.Replace(".", ",");
        txtVolumen.Text = txtVolumen.Text.Replace(".", ",");

        string prefijo          = row.Cells[0].Text + row.Cells[1].Text;
        string codigo           = row.Cells[1].Text;
        string descripcion      = txtDescripcion.Text;// row.Cells[1].Text;
        //decimal fob = Convert.ToDecimal(txtValor.Text);
        //decimal volumen = Convert.ToDecimal(txtVolumen.Text);
        string fob = txtValor.Text;
        string volumen = txtVolumen.Text;
        string unidadMedida     = txtUnidadMedida.Text;
        string moneda           = txtMoneda.Text;
        string grupoTecnico     = txtGrupoTecnico.Text;

        actualizaFob(prefijo, codigo, descripcion, fob, volumen, unidadMedida, moneda, grupoTecnico);

        dgvMateriales.EditIndex = -1;
        cargarGrilla();
    }

    protected void dgvMateriales_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvMateriales.EditIndex = -1;
        cargarGrilla();
    }

    protected void dgvMateriales_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        /*
        if (e.CommandName == "eliminar")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = dgvMateriales.Rows[indice];
            string marca = row.Cells[0].Text;
            string codigo = row.Cells[1].Text;

            eliminarFob(marca, codigo);
        }
        */
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        mjsError.Visible = false;

        try
        {
            mjsError.Visible = false;
            cargarGrilla();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + "");
        }

    }

    private void actualizaFob(string prefijo, string codigo, string descripcion, string fob, string volumen, string unidadMedida, string moneda, string grupoTecnico)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_fob_modifica_valores";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@prefijo", SqlDbType.VarChar).Value = prefijo;
            cmd.Parameters.Add("@codigo", SqlDbType.VarChar).Value = codigo;
            cmd.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = descripcion;
            cmd.Parameters.Add("@fob", SqlDbType.VarChar).Value = fob;
            cmd.Parameters.Add("@volumen", SqlDbType.VarChar).Value = volumen;
            cmd.Parameters.Add("@unidadMedida", SqlDbType.VarChar).Value = unidadMedida;
            cmd.Parameters.Add("@moneda", SqlDbType.VarChar).Value = moneda;
            cmd.Parameters.Add("@grupoTecnico", SqlDbType.VarChar).Value = grupoTecnico;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();

            con.Close();

            cargarGrilla();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    private void cargarGrilla()
    {
        int coderror = 0;
        string msgerror = "";

        dgvMateriales.DataSource = _SQL.obtieneSkuFob(txtCodigo.Text, out coderror, out msgerror);
        dgvMateriales.DataBind();
    }


    protected void dgvMateriales_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            int indice = Convert.ToInt32(e.RowIndex);
            GridViewRow row = dgvMateriales.Rows[indice];

            string marca = row.Cells[0].Text;
            string codigo = row.Cells[1].Text;

            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_fob_elimina_fob";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@marca", SqlDbType.VarChar).Value = marca;
            cmd.Parameters.Add("@codigo", SqlDbType.VarChar).Value = codigo;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();

            con.Close();

            cargarGrilla();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }
}