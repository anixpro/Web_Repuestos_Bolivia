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

public partial class Vistas_MantPlazosImportacion : System.Web.UI.Page
{
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantPlazosImportacion));
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LlenarComboMarcas();
            cargarGrilla();
        }
    }

    protected void actualizaInfo_Click(object sender, EventArgs e)
    {
        try
        {
            string miRut = Session["rut"].ToString();
            foreach (GridViewRow row in grEnvios.Rows)
            {
                //int idReg = int.Parse(row.Cells[0].Text);
                HiddenField idHd = (HiddenField)row.FindControl("hdId");
                int idHdVal = int.Parse(idHd.Value);
                TextBox dias = (TextBox)row.Cells[2].FindControl("txtDias");
                TextBox fci = (TextBox)row.Cells[3].FindControl("txtFci"); // REQ - Cotizaciones Automaticas Marzo 2022
                if (dias.Text != "")
                {
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    int? nro_error = null;
                    string msg_error = null;
                    //var ejecutaSp = ctx.webr_modifica_plazos_importacion_v2(idHdVal, dias.Text, ref nro_error, ref msg_error); // REQ - Cotizaciones Automaticas Marzo 2022
                    var ejecutaSp = ctx.webr_modifica_plazos_importacion_v2(idHdVal, dias.Text, fci.Text, ref nro_error, ref msg_error); // REQ - Cotizaciones Automaticas Marzo 2022
                }
            }

            lblAviso.Visible = true;
            lblAviso.Text = "Datos Actualizados";
            cargarGrilla();
        }
        catch(Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        
        }
    }

    private void cargarGrilla()
    {
        string marca = "";
        marca = combomarcas.SelectedValue;
        lblAviso.Visible = false;// REQ - Cotizaciones Automaticas Marzo 2022
        lblAviso.Text = "";// REQ - Cotizaciones Automaticas Marzo 2022
        try
        {
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            int? nro_error = null;
            string msg_error = null;
            var listaParametros = (from i in ctx.webr_obtiene_plazos_importacion_v2(marca,ref nro_error, ref msg_error)
                                   select i).ToList();

            if (listaParametros != null)
            {
                grEnvios.DataSource = listaParametros;
                grEnvios.DataBind();
            }
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

    protected void filtro_Click(object sender, EventArgs e)
    {
        cargarGrilla();
    }
}