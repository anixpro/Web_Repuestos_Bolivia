using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorMarcas : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorMarcas));

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
            marca = "";
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
            else if (evento.Equals("busca"))
            {
 
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
        catch (NullReferenceException)
        {
            _nombreConcesionario = "";
            _iConcesionario = 0;
        }

        ddlConcesionario.Items.Clear();
        ddlConcesionario.Items.Add("Todos");
        foreach (String concesionario in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(concesionario);
        }

        ddlConcesionario.SelectedIndex = _iConcesionario;

        sqldPersona.SelectCommand = "";
        gridMarcas.DataBind();


        if (_iConcesionario == 0)
        {
            sqldPersona.SelectCommand = controlAuto.detalleMarca();
        }
        else
        {
            sqldPersona.SelectCommand = controlAuto.detalleMarcaPorConcesionario(_nombreConcesionario);
        }
    }
    protected void ddlConcesionario_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorMarcasUpdate.aspx?marca=" + txtMarca.Text + "");
    }
}