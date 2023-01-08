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

public partial class Vistas_Devolucion_Comercial_Control : System.Web.UI.Page
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

            grVistaControl.DataSource = _SQL.cargasolicitudesControl(2, out coderror, out msgerror);

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
}