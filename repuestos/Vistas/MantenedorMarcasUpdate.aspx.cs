using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorMarcasUpdate : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorMarcasUpdate));
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
        String marca = "";

        btnBuscar.OnClientClick = "javascript:return validaBlancos();";

        try
        {
            evento = Request.QueryString["ev"];
            marca = Request.QueryString["marca"];
        }
        catch (ArgumentNullException)
        {
            evento = "";
            marca ="";
        }
        catch (FormatException)
        {
            evento = "";
            marca = "";
        }

        try
        {

            if (evento.Equals("elimina"))
            {
                controlAuto.eliminaMarca(marca);
            }
        }
        catch (NullReferenceException)
        {

        }

        sqldPersona.SelectCommand = controlAuto.detalleMarcaPorMarca(marca);

    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorMarcasUpdate.aspx?marca=" + txtMarca.Text + "");
    }

}