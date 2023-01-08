using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class Vistas_mantenedorUsuarioDoble : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_mantenedorUsuarioDoble));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;
    private String _nombreConcesionario;
    private int _iConcesionario;
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
            rut = (Request.QueryString["rut"]);
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
            if (evento.Equals("doble"))
            {
                controlPersona.doblePermiso(rut);
            }
        }
        catch (NullReferenceException)
        {

        }

        try
        {
            _nombreConcesionario = ddlConcesionario.SelectedItem.ToString();
            _iConcesionario = ddlConcesionario.SelectedIndex;
        }
        catch (NullReferenceException ex)
        {
            _nombreConcesionario = "";
            _iConcesionario = 0;
            logger.Error("NullReferenceException Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }

        ddlConcesionario.Items.Clear();
        ddlConcesionario.Items.Add("Todos");
        foreach (String concesionario in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(concesionario);
        }

        ddlConcesionario.SelectedIndex = _iConcesionario;

        sqldPersona.SelectCommand = "";
        gridUsuarios.DataBind();


        if (_iConcesionario == 0)
        {
            sqldPersona.SelectCommand = controlPersona.datosPersonaDoble();
        }
        else
        {
            sqldPersona.SelectCommand = controlPersona.datosConcesionarioDoble(_nombreConcesionario);
        }
    }
    protected void ddlConcesionario_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorUsuarioUpdate.aspx?rut=" + txtRut.Text + "");
    }

}