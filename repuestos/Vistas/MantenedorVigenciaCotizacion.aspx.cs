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

public partial class Vistas_MantenedorVigenciaCotizacion : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantenedorVigenciaCotizacion));
    private SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            cargarGrilla();
        }
    }

    private void cargarGrilla()
    {
        try
        {
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            int? nro_error = null;
            string msg_error = null;
            var listaParametros = (from i in ctx.webr_obtiene_validez_cotizacion(ref nro_error, ref msg_error)
                                   select i).ToList();

            if (listaParametros != null)
            {
                grParametro.DataSource = listaParametros;
                grParametro.DataBind();
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }

    protected void actualizaInfo_Click(object sender, EventArgs e)
    {
        ActualizaDatos();
    }

    private void ActualizaDatos()
    {
        try
        {
            string miRut = Session["rut"].ToString();
            foreach (GridViewRow row in grParametro.Rows)
            {
                int idReg = int.Parse(row.Cells[0].Text);
                string nombre = row.Cells[1].Text;
                TextBox dias = (TextBox)row.Cells[2].FindControl("txtDias");


                if (dias.Text != "")
                {
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    int? nro_error = null;
                    string msg_error = null;
                    var listaParametros = ctx.webr_actualiza_validez_cotizacion(idReg,int.Parse(dias.Text),ref nro_error, ref msg_error);
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
}