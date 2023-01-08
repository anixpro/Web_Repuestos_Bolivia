using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.Data.Sql;
using System.Data.SqlClient;

public partial class Vistas_Devolucion_CDR_Control : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        cargasolicitudesdevolucion();
    }

    private void cargasolicitudesdevolucion()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            grVistaControl.DataSource = _SQL.cargasolicitudesControl(1, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_VistaControlCDR] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grVistaControl.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_VistaControlCDR_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }

    protected void Buscar_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            //grSolicitudesDetalleDevolucion.DataSource = _SQL.cargasolicitudes(13, txtAprobacion.Text, 0, out coderror, out msgerror);
            grVistaControl.DataSource = _SQL.cargasolicitudesfiltradas(2, Session["rut"].ToString(), 0, txtAprobacion.Text, txtshipcode.Text, txtfecha.Text, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDevBuscar] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grVistaControl.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudesbuscar_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }
    }
    protected void btnRefrescar_Click(object sender, ImageClickEventArgs e)
    {
        txtAprobacion.Text = "";
        cargasolicitudesdevolucion();
    }

}