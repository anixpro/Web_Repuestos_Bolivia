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

public partial class Vistas_MantMotivoDr : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantMotivoDr));
    SendMail_helper _mail = new SendMail_helper();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            cargarGrilla();
            LlenarComboFiltro();
            LlenarComboTipos();
        }
    }

    private void cargarGrilla()
    {
        try
        {
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            int? nro_error = null;
            string msg_error = null;
            var listaResultado = (from i in ctx.webr_obtiene_motivos_dr(ref nro_error, ref msg_error)
                                  select i).OrderBy(m => m.ID).ToList();

            if (combofiltro.SelectedValue != "" && combofiltro.SelectedValue != "-1")
            {
                listaResultado = listaResultado.Where(i => i.TIPO_MOTIVO == combofiltro.SelectedItem.Text).ToList();
            }

            if (listaResultado != null)
            {
                grMotivos.DataSource = listaResultado;
                grMotivos.DataBind();
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
                GridViewRow row = grMotivos.Rows[indice];
                HiddenField idHd = (HiddenField)row.FindControl("hdId");

                if (idHd != null)
                {
                    int? nro_error = null;
                    string msg_error = null;
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    var resultado = ctx.webr_elimina_motivo_dr(int.Parse(idHd.Value), ref nro_error, ref msg_error);
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

    protected void nuevo_Click(object sender, EventArgs e)
    {
        try
        {
            string descNuevo = txtDescripcionNuevo.Text;
            string paridadNuevo = txtParidad.Text;
            string tipoNuevo = comboTipos.SelectedItem.Text;

            if (descNuevo == "" || tipoNuevo == "" || comboTipos.SelectedValue == null || comboTipos.SelectedValue == "-1")
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
                var resultado = ctx.webr_agrega_motivo_dr(paridadNuevo, descNuevo, tipoNuevo, ref nro_error, ref msg_error);

                txtDescripcionNuevo.Text = "";
                txtParidad.Text = "";
                comboTipos.ClearSelection();
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
            foreach (GridViewRow row in grMotivos.Rows)
            {
                HiddenField idHd = (HiddenField)row.FindControl("hdId");
                int idHdVal = int.Parse(idHd.Value);
                HiddenField hdParidad = (HiddenField)row.FindControl("hdParidad");
                string hdParidadVal = hdParidad.Value;
                TextBox paridad = (TextBox)row.Cells[3].FindControl("txtParidad");

                if (paridad != null && (paridad.Text != hdParidadVal))
                {
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    int? nro_error = null;
                    string msg_error = null;
                    var ejecutaSp = ctx.webr_actualiza_motivo_dr(paridad.Text,idHdVal , ref nro_error, ref msg_error);
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

    private void LlenarComboTipos()
    {
        comboTipos.Items.Clear();
        comboTipos.Items.Add(new ListItem("Seleccionar", "-1"));

        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        int? nro_error = null;
        string msg_error = null;
        var listaTipos = from i in ctx.webr_obtiene_tipo_motivodr(ref nro_error, ref msg_error)
                         select i;

        //Llenar el combo box con las marcas
        foreach (var tipo in listaTipos)
        {           
            comboTipos.Items.Add(new ListItem(tipo.Nombre, tipo.Id.ToString()));
        }
    }

    private void LlenarComboFiltro()
    {
        combofiltro.Items.Clear();
        combofiltro.Items.Add(new ListItem("Seleccionar", "-1"));

        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        int? nro_error = null;
        string msg_error = null;
        var listaTipos = from i in ctx.webr_obtiene_tipo_motivodr(ref nro_error, ref msg_error)
                         select i;

        //Llenar el combo box con las marcas
        foreach (var tipo in listaTipos)
        {
            combofiltro.Items.Add(new ListItem(tipo.Nombre, tipo.Id.ToString()));
        }
    }

    protected void filtro_Click(object sender, EventArgs e)
    {
        cargarGrilla();
    }
}