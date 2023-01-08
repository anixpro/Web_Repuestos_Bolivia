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


public partial class Vistas_Devolucion_Concesionario_Realizadas : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        Button1.Style["visibility"] = "hidden";
        cargasolicitudes();
    }

    private void cargasolicitudes()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            string numSolicitud = "";
            string numFactura = "";

            if (txtNumFactura.Text != "")
            {
                numFactura = txtNumFactura.Text;
            }

            if (txtSolId.Text != "")
            {
                numSolicitud = txtSolId.Text;
            }

            grSolicitudes.DataSource = _SQL.cargaSolicitudesFiltradas(3, Session["rut"].ToString(), out coderror, out msgerror, numSolicitud, numFactura);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudes] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grSolicitudes.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudes_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }

    protected void grSolicitudes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grSolicitudes.PageIndex = e.NewPageIndex;
        cargasolicitudes();
    }


    protected void grSolicitudes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            if (e.CommandName == "detalle")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = grSolicitudes.Rows[index];

                int id_solicitud = Convert.ToInt32(row.Cells[0].Text);

                grSolicitudesDetalle.DataSource = _SQL.cargasolicitudes(4, Session["rut"].ToString(), id_solicitud, out coderror, out msgerror);

                if (coderror != 0)
                {
                    msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                    msjesError.Visible = true;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDET] Message: " + msgerror + " Inner: " + coderror);
                }
                else
                {
                    grSolicitudesDetalle.DataBind();
                    this.ModalPopupExtender1.Show();
                }

            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDET_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }

    protected void btnFiltrar_Click(object sender, EventArgs e)
    {
        cargasolicitudes();
    }
}