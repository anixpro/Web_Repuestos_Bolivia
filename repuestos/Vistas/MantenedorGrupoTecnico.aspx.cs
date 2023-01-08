using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

public partial class Vistas_MantenedorGrupoTecnico : System.Web.UI.Page
{
    SqlConnection con;
    SqlCommand cmd;
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LlenaGruposTecnicos();
        }
    }

    private void LlenaGruposTecnicos()
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_grupo_tecnico_datos";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvGruposTecnicos.DataSource = dt;
            dgvGruposTecnicos.DataBind();

            con.Close();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }
    protected void dgvGruposTecnicos_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvGruposTecnicos.EditIndex = e.NewEditIndex;
        LlenaGruposTecnicos();
    }

    protected void dgvGruposTecnicos_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = dgvGruposTecnicos.Rows[indice];

        TextBox txtDescripcion = (TextBox)dgvGruposTecnicos.Rows[indice].FindControl("txtCodigoGrupoTecnico");

        string cod = row.Cells[0].Text;

        HabilitaGrupoTecnico(cod, 3, txtDescripcion.Text);

        dgvGruposTecnicos.EditIndex = -1;
        LlenaGruposTecnicos();
    }

    protected void dgvGruposTecnicos_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvGruposTecnicos.EditIndex = -1;
        LlenaGruposTecnicos();
    }

    protected void dgvGruposTecnicos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "activo")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = dgvGruposTecnicos.Rows[indice];
            string cod = row.Cells[0].Text;
            string grupo_tecnico = row.Cells[1].Text;
            string descripcion = row.Cells[2].Text;
            string activo = row.Cells[3].Text;

            if (activo == "NO")
            {
                HabilitaGrupoTecnico(cod, 1, descripcion);
            }
            else
            {
                HabilitaGrupoTecnico(cod, 0, descripcion);
            }
        }
    }

    private void HabilitaGrupoTecnico(string codigo, int flag, string descripcion)
    {
        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_grupo_tecnico_actualiza";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar, 35).Value = codigo;
            cmd.Parameters.Add("@i_flag", SqlDbType.Int).Value = flag;
            cmd.Parameters.Add("@i_codigo_grupo_tecnico", SqlDbType.VarChar, 5).Value = descripcion;
            cmd.Parameters.Add("@i_clase_grupo_tecnico", SqlDbType.VarChar, 50).Value = descripcion;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();
            con.Close();

            LlenaGruposTecnicos();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    protected void dgvGruposTecnicos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvGruposTecnicos.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        dgvGruposTecnicos.PageIndex = e.NewPageIndex;
        LlenaGruposTecnicos();
    }

}