using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using log4net;
using log4net.Config;

public partial class Vistas_Indicadores : System.Web.UI.Page
{
    protected String[] abreviados;
    protected int q;
    private ControlAuto controlAuto;
    private ControlExcel ControlExcel;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Indicadores));

    protected void Page_Init(object sender, EventArgs e)
    {
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            Response.Redirect("verIndicador.aspx");
        }
        else if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            Response.Redirect("../Index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        btnAceptar.OnClientClick = "javascript:return validaBlancos();";
        ControlExcel = new ControlExcel();
        controlAuto = new ControlAuto();
        abreviados = controlAuto.todosLosAbreviado();
        q = 0;
    }

    protected void btnAceptar_Click(object sender, EventArgs e)
    {
        String[] nombreArchivo = fldExcel.FileName.Split('.');
        String[] marca = nombreArchivo[0].Split(' ');

        ControlExcel.setearAno(fldExcel.FileName);

        String ruta = Server.MapPath("~/doc/archivosCsv/" + ControlExcel.getAno() + fldExcel.FileName);
        
        if (!File.Exists(ruta))
        {
            fldExcel.SaveAs(ruta);
            ControlExcel.guardaIndicador(fldExcel.FileName,ruta);
            msjesError.InnerHtml = "Archivo " + marca[0] + " insertado correctamente";
            msjesError.Visible = true;
        }
        else
        {
            fldExcel.SaveAs(ruta);
            ControlExcel.cambiaIndicador(fldExcel.FileName,ruta);
            msjesError.InnerHtml = "El archivo " + marca[0] + " se sobreescribio correctamente";
            msjesError.Visible = true;
        }
    }
}