using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;
using System.IO;

public partial class Vistas_MantenedorFobVolumenes : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    SendMail_helper _mail = new SendMail_helper();
    SQL_DevRec _SQL = new SQL_DevRec();
    SapAPI _sapApi = new SapAPI();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();
    SqlConnection con;
    SqlCommand cmd;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LlenarComboMarcas();
            LlenarComboGruposTecnicos();
        }
    }

    private void LlenarComboMarcas()
    {
        ddlMarca.Items.Clear();
        ddlMarca.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _controlBD.CrearMarcas(Session["rut"].ToString()))
        {
            ddlMarca.Items.Add(marca);
        }
    }
    private void LlenarComboGruposTecnicos()
    {
        ddlGrupoTecnico.Items.Clear();
        ddlGrupoTecnico.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string grupoTecnico in _controlBD.getGrupoTecnico())
        {
            ddlGrupoTecnico.Items.Add(grupoTecnico);
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        try
        {
            msjesError.Visible = false;
            cargarGrilla();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + "");
        }
    }

    protected void dgvTramos_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvTramos.EditIndex = e.NewEditIndex;
        cargarGrilla();
    }

    protected void dgvTramos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvTramos.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        dgvTramos.PageIndex = e.NewPageIndex;
        cargarGrilla();
    }

    protected void dgvTramos_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            string path = Server.MapPath("~/Documentacion/");
            string _sql;
            int indice = Convert.ToInt32(e.RowIndex);
            GridViewRow row = dgvTramos.Rows[indice];

            Label lblId = (Label)dgvTramos.Rows[indice].FindControl("lblId");
            TextBox txtDesde = (TextBox)dgvTramos.Rows[indice].FindControl("txtDesde");
            TextBox txtHasta = (TextBox)dgvTramos.Rows[indice].FindControl("txtHasta");
            TextBox txtFactor = (TextBox)dgvTramos.Rows[indice].FindControl("txtFactor");

            txtDesde.Text = txtDesde.Text.Replace(".", ",");
            txtHasta.Text = txtHasta.Text.Replace(".", ",");
            txtFactor.Text = txtFactor.Text.Replace(".", ",");

            int id = Convert.ToInt32(lblId.Text);// row.Cells[1].Text;
            decimal desde = Convert.ToDecimal(txtDesde.Text);
            decimal hasta = Convert.ToDecimal(txtHasta.Text);
            decimal factor = Convert.ToDecimal(txtFactor.Text);

            //Grabo log cambio de valores
            string _marca = "";
            string _grupo_tecnico = "";
            string _desde = "";
            string _hasta = "";
            string _factor = "";

            _sql = "SELECT * FROM carga_fob_tramos_volumen WHERE id = '"+ id + "' ";

            DataSet ds = _controlBD.ObtenerDatosFiltrados(_sql);

            foreach (DataRow campos in ds.Tables[0].Rows)
            {
                _marca           = Convert.ToString(campos["marca"]);
                _grupo_tecnico   = Convert.ToString(campos["grupo_tecnico"]);
                _desde           = Convert.ToString(campos["desde"]);
                _hasta           = Convert.ToString(campos["hasta"]);
                _factor          = Convert.ToString(campos["factor"]);

            }
            StreamWriter arch = new StreamWriter(path + "/log_factor_volumenes.txt", true);
            arch.WriteLine("Id " + id + " - " + _marca + " - " + _grupo_tecnico + " Desde " + _desde + " Hasta " + _hasta + " Factor " + _factor);
            arch.Close();
            //Fin grabo log

            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_fob_actualiza_tramo";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@desde", SqlDbType.Decimal).Value = desde;
            cmd.Parameters.Add("@hasta", SqlDbType.Decimal).Value = hasta;
            cmd.Parameters.Add("@factor", SqlDbType.Decimal).Value = factor;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();

            con.Close();

            dgvTramos.EditIndex = -1;
            cargarGrilla();

            StreamWriter arch2 = new StreamWriter(path + "/log_factor_volumenes.txt", true);
            arch2.WriteLine("Id " + id + " Desde " + desde + " Hasta " + hasta + " Factor " + factor + " Usuario " + Session["rut"].ToString() + " Fecha/Hora " + DateTime.Now);
            arch2.Close();
        }
        catch (Exception ex)
        {

        }
    }

    protected void dgvTramos_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgvTramos.EditIndex = -1;
        cargarGrilla();
    }

    protected void dgvTramos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "elimina")
        {
            try
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = dgvTramos.Rows[indice];
                Label lblId = (Label)row.FindControl("lblId");

                if (lblId != null)
                {
                    con = new SqlConnection();
                    cmd = new SqlCommand();

                    con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "webr_fob_elimina_tramo";
                    cmd.CommandTimeout = 10;
                    cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = lblId.Text;
                    cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.ExecuteReader();

                    con.Close();

                    cargarGrilla();
                }
            }
            catch (Exception ex)
            {
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }
        }
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (ddlMarca.SelectedValue == "-1")
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Selecciones una marca.";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        if (ddlGrupoTecnico.SelectedValue == "-1")
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Selecciones un grupo técnico.";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_fob_agrega_tramo";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@marca", SqlDbType.VarChar).Value = ddlMarca.SelectedValue;
            cmd.Parameters.Add("@grupoTecnico", SqlDbType.VarChar).Value = ddlGrupoTecnico.SelectedValue;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();

            con.Close();

            cargarGrilla();
        }
        catch (Exception ex)
        {

        }
    }
    private void cargarGrilla()
    {
        int coderror = 0;
        string msgerror = "";

        dgvTramos.DataSource = _SQL.obtieneTramosGrupoTecnico(ddlMarca.SelectedValue, ddlGrupoTecnico.SelectedValue, out coderror, out msgerror);
        dgvTramos.DataBind();
    }
}