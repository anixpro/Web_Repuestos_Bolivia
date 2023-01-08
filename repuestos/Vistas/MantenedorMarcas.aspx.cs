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
    ControlBD _controlBD = new ControlBD();    // REQ - Cotizaciones Automaticas Marzo 2022
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
        String abreviado = "";   // REQ - Cotizaciones Automaticas Marzo 2022
        btnBuscar.OnClientClick = "javascript:return validaBlancos();";
        try
        {
            evento = Request.QueryString["ev"];
            marca = Request.QueryString["marca"];
            abreviado =  Request.QueryString["abreviado"];// REQ - Cotizaciones Automaticas Marzo 2022
        }
        catch (ArgumentNullException)
        {
            evento = "";
            marca = "";
            abreviado = "";// REQ - Cotizaciones Automaticas Marzo 2022
        }
        catch (FormatException)
        {
            evento = "";
            marca = "";
            abreviado = "";// REQ - Cotizaciones Automaticas Marzo 2022
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
            else if (evento.Equals("cotiAuto"))// REQ - Cotizaciones Automaticas Marzo 2022
            {
                controlAuto.habilitaCotizacionAutomatica(abreviado, marca);// REQ - Cotizaciones Automaticas Marzo 2022
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

        //sqldPersona.SelectCommand = "";// REQ - Cotizaciones Automaticas Marzo 2022
        gridMarcas.DataSource = _controlBD.ObtenerDatosFiltrados(" SELECT  Nombremarca as marca, abreviado, orgVentas, esForaneo, Descripcion," +
                                                                 " CASE " +
                                                                 "   WHEN cotizacionAutomatica = 1 THEN 'SI' " +
                                                                 "   WHEN cotizacionAutomatica = 0 THEN 'NO' " +
                                                                 " END AS cotizacionAutomatica " +
                                                                 " FROM marca ");


        gridMarcas.DataBind();


        if (_iConcesionario == 0)
        {
            gridMarcas.DataSource = controlAuto.detalleMarca();
            // sqldPersona.SelectCommand = controlAuto.detalleMarca(); // REQ - Cotizaciones Automaticas Marzo 2022
        }
        else
        {
            gridMarcas.DataSource = controlAuto.detalleMarcaPorConcesionario(_nombreConcesionario);
            // sqldPersona.SelectCommand = controlAuto.detalleMarcaPorConcesionario(_nombreConcesionario); // REQ - Cotizaciones Automaticas Marzo 2022
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