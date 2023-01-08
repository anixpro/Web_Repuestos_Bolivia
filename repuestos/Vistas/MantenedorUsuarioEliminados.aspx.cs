using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorUsuarioEliminados : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorUsuarioEliminados));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;

    protected void Page_Init(object sender, EventArgs e)
    {
        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        controlPersona = new ControlPersona();
        controlAuto = new ControlAuto();
        String evento = "";
        string rut = "0";
        btnBuscar.OnClientClick = "javascript:return validaBlancos();";

        try
        {
            evento = Request.QueryString["ev"];
            rut = Request.QueryString["rut"];
        }
        catch (ArgumentNullException)
        {
            evento = "";
            rut = "0";
        }
        catch (FormatException)
        {
            evento = "";
            rut = "0";
        }

        try
        {

            if (evento.Equals("rehacer"))
            {
                controlPersona.rehacerPersona(rut);
            }
        }
        catch (NullReferenceException)
        {

        }

        sqldPersona.SelectCommand = controlPersona.datosEliminadosPersona();
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorUsuarioUpdate.aspx?rut=" + txtRut.Text + "");
    }

}