using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorModelo : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorModelo));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto = new ControlAuto();
    private String _nombreMarca;
    private int iMarca;
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
        String evento = "";
        String modelo = "";

        btnBuscar.OnClientClick = "javascript:return validaBlancos();";
        try
        {
            evento = Request.QueryString["ev"];
            modelo = Request.QueryString["modelo"];
            _nombreMarca = Request.QueryString["marca"];
        }
        catch (ArgumentNullException)
        {
            evento = "";
            modelo = "";
            _nombreMarca = "";
        }
        catch (FormatException)
        {
            evento = "";
            modelo = "";
            _nombreMarca = "";
        }

        try
        {

            if (evento.Equals("elimina"))
            {
                controlAuto.eliminaModelo(modelo);
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
            _nombreMarca = ddlMarca.SelectedItem.ToString();
            iMarca = ddlMarca.SelectedIndex;
        }
        catch (NullReferenceException)
        {
            _nombreMarca = Request.QueryString["marca"];
            if (_nombreMarca == "")
            {
                iMarca = 0;
            }
            else
            {
                iMarca = 1;
                try
                {
                    ddlMarca.SelectedItem.Text = _nombreMarca;
                    return;                    
                }
                catch (NullReferenceException)
                { }
                
            }
        }

        ddlMarca.Items.Clear();
        ddlMarca.Items.Add("Todas");
        foreach (String marcas in controlAuto.marcas())
        {
            ddlMarca.Items.Add(marcas);
        }

        sqldPersona.SelectCommand = "";
        gridMarcas.DataBind();


        if (iMarca == 0)
        {
            sqldPersona.SelectCommand = controlAuto.detalleModelo();
        }
        else
        {
            sqldPersona.SelectCommand = controlAuto.detalleModeloPorMarca(_nombreMarca);
        }


        //gridMarcas.DataSource = controlPersona.datosPersona();
        //gridMarcas.DataBind();

    }
    protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorModeloUpdate.aspx?modelo=" + txtModelo.Text + "");
    }

}