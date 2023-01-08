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
using ClosedXML.Excel;
using System.Linq;
using System.Drawing;
using System.IO;

public partial class Vistas_Devolucion_CDR_Reclamos_Realizadas : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        cargasolicitudes();
    }

    private void cargasolicitudes()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            grSolicitudesDetalle.DataSource = _SQL.cargasolicitudes(14, Session["rut"].ToString(), 0, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudes] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grSolicitudesDetalle.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudes_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }

    protected void btnExportar_Click(object sender, EventArgs e)
    {
        int coderror = 0;
        string msgerror = "";
        DataSet dt = new DataSet();
        dt = _SQL.cargasolicitudes(14, Session["rut"].ToString(), 0, out coderror, out msgerror);
        if (dt.Tables.Count != 0)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                //wb.Worksheets.Add(dt, "Cotizaciones");
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Reclamos.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}