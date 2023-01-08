using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data;

public partial class Vistas_MantMotivoPedido : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantMotivoPedido));
    SendMail_helper _mail = new SendMail_helper();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            cargarGrilla();
            LlenarComboFiltro();
            LlenarComboMarcas();
        }
    }

    private void cargarGrilla()
    {
        try
        {
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            int? nro_error = null;
            string msg_error = null;
            var listaResultado = (from i in ctx.webr_obtiene_motivo_pedidos(1, ref nro_error, ref msg_error)
                                  select i).OrderBy(m=> m.mp_Marca).ToList();

            if (combofiltro.SelectedValue != "")
            {
                listaResultado = listaResultado.Where(i => i.mp_Marca == combofiltro.SelectedItem.Text).ToList();
            }

            if (listaResultado != null)
            {
                grTipos.DataSource = listaResultado;
                grTipos.DataBind();
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }

    }

    protected void grTipos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "elimina")
        {
            try
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = grTipos.Rows[indice];
                HiddenField idHd = (HiddenField)row.FindControl("hdId");

                if (idHd != null)
                {
                    int? nro_error = null;
                    string msg_error = null;
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    var resultado = ctx.webr_elimina_motivo_pedido(int.Parse(idHd.Value), ref nro_error, ref msg_error);
                    lblAviso.Visible = false;
                    msjesError.Visible = false;
                    cargarGrilla();
                }
            }
            catch (Exception ex)
            {
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }
        }
    }

    protected void chkHabilitado_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            CheckBox cb1 = (CheckBox)grTipos.Rows[index].FindControl("chkHabilitado");
            HiddenField idHd = (HiddenField)row.FindControl("hdId");
            bool checkValue = cb1.Checked;

            int? nro_error = null;
            string msg_error = null;
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var resultado = ctx.webr_actualiza_motivo_pedido_habilitado(checkValue, idHd.Value, ref nro_error, ref msg_error);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }

    protected void chkDocumentacion_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            CheckBox cb1 = (CheckBox)grTipos.Rows[index].FindControl("chkDocumentacion");
            HiddenField idHd = (HiddenField)row.FindControl("hdId");
            bool checkValue = cb1.Checked;

            int? nro_error = null;
            string msg_error = null;
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var resultado = ctx.webr_actualiza_motivo_pedido_documentacion(checkValue, idHd.Value, ref nro_error, ref msg_error);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }

    protected void chkInformacioAdicional_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            CheckBox cb1 = (CheckBox)grTipos.Rows[index].FindControl("chkInformacioAdicional");
            HiddenField idHd= (HiddenField)row.FindControl("hdId");
            bool checkValue = cb1.Checked;

            int? nro_error = null;
            string msg_error = null;
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var resultado = ctx.webr_actualiza_motivo_pedido_informacioAdicional(checkValue, idHd.Value, ref nro_error, ref msg_error);

            if (cb1.Checked)
            {
                string url = "MantMotivoPedidoInfoAd.aspx?idMotivo=" + idHd.Value + " ";
                string s = "window.open('" + url + "', 'popup_window', 'width=600,height=350,left=100,top=100,resizable=yes');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }

    protected void nuevo_Click(object sender, EventArgs e)
    {
        try
        {
            string descNuevo = txtDescripcionNuevo.Text;
            string bloqueoNuevo = txtBloqueoNuevo.Text;
            string codigoNuevo = txtCodigoNuevo.Text;
            string marcaNuevo = combomarcas.SelectedItem.Text;

            if (descNuevo == "" || bloqueoNuevo == "" || codigoNuevo == "" || combomarcas.SelectedValue == null || combomarcas.SelectedValue == "-1")
            {
                lblAviso.Visible = false;
                msjesError.InnerText = "Debe completar el formulario para agregar un nuevo elemento.";
                msjesError.Visible = true;
                return;
            }
            else
            {
                int? nro_error = null;
                string msg_error = null;
                RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                var resultado = ctx.webr_inserta_motivo_pedido(descNuevo,codigoNuevo,marcaNuevo,bloqueoNuevo, ref nro_error, ref msg_error);

                txtDescripcionNuevo.Text = "";
                txtBloqueoNuevo.Text = "";
                txtCodigoNuevo.Text = "";
                msjesError.Visible = false;
                lblAviso.Visible = true;
                lblAviso.Text = "Datos Actualizados";
                cargarGrilla();
            }

        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

        }
    }

    protected void actualizaInfo_Click(object sender, EventArgs e)
    {
        try
        {
            string miRut = Session["rut"].ToString();
            foreach (GridViewRow row in grTipos.Rows)
            {
                HiddenField idHd = (HiddenField)row.FindControl("hdId");
                int idHdVal = int.Parse(idHd.Value);
                TextBox bloqueo = (TextBox)row.Cells[3].FindControl("txtBloqueo");

                if (bloqueo.Text != "")
                {
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    int? nro_error = null;
                    string msg_error = null;
                    var ejecutaSp = ctx.webr_modifica_bloqueo_motivo_pedido(idHdVal, bloqueo.Text, ref nro_error, ref msg_error);
                }
            }

            lblAviso.Visible = true;
            lblAviso.Text = "Datos Actualizados";
            cargarGrilla();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

        }
    }

    private void LlenarComboMarcas()
    {
        combomarcas.Items.Clear();
        combomarcas.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _ControlBD.CrearMarcas(Session["rut"].ToString()))
        {
            combomarcas.Items.Add(marca);
        }
    }

    private void LlenarComboFiltro()
    {
        combofiltro.Items.Clear();
        combofiltro.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _ControlBD.CrearMarcas(Session["rut"].ToString()))
        {
            combofiltro.Items.Add(marca);
        }
    }

    protected void filtro_Click(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        lblAviso.Visible = false;

        cargarGrilla();
    }
}