using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_sucursalInsert : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_sucursalInsert));

    private String _nombreConcesionario,_nombreRegion,_nombreProvincia,_nombreComuna;
    private int _iConcesionario,_iRegion,_iProvincia,_iComuna;

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
        foreach (String concesionario in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(concesionario);
        }

        ddlConcesionario.SelectedIndex = _iConcesionario;

        //region
        try
        {
            _nombreRegion = ddlRegion.SelectedItem.ToString();
            _iRegion = ddlRegion.SelectedIndex;
        }
        catch (NullReferenceException)
        {
            _nombreRegion = "";
            _iRegion = 0;
        }
        ddlRegion.Items.Clear();
        foreach (String region in controlAuto.regiones())
        {
            ddlRegion.Items.Add(region);
        }

        ddlRegion.SelectedIndex = _iRegion;

        //provincia
        try
        {
            _nombreProvincia = ddlProvincia.SelectedItem.ToString();
            _iProvincia = ddlProvincia.SelectedIndex;
        }
        catch (NullReferenceException)
        {
            _nombreProvincia = "";
            _iProvincia = 0;
        }
        ddlProvincia.Items.Clear();
        foreach (String provincia in controlAuto.provincias(ddlRegion.SelectedItem.Text))
        {
            ddlProvincia.Items.Add(provincia);
        }

        ddlProvincia.SelectedIndex = _iProvincia;

        //comuna

        try
        {
            _nombreComuna = ddlComuna.SelectedItem.ToString();
            _iComuna = ddlComuna.SelectedIndex;
        }
        catch (NullReferenceException)
        {
            _nombreComuna = "";
            _iComuna = 0;
        }
        ddlComuna.Items.Clear();
        foreach (String comuna in controlAuto.comunas(ddlProvincia.SelectedItem.Text))
        {
            ddlComuna.Items.Add(comuna);
        }

        ddlComuna.SelectedIndex = _iComuna;
    }
    protected void btnAceptar_Click(object sender, EventArgs e)
    {
       if (controlAuto.insertaSucursal(_nombreConcesionario, ddlComuna.SelectedItem.Text, txtNombre.Text, txtShipCode.Text, txtDireccion.Text) > 0)
        {
            lblAviso.Text = "Se inserto sucursal: " + txtNombre.Text + " correctamente";
            txtNombre.Text = "";
            txtShipCode.Text = "";
            txtDireccion.Text = "";
        }
        else
        {
            lblAviso.Text = "Se produjo un error al insertar sucursal. Compruebe que Destinatario de mercancía no esté siendo utilizado por otra sucursal";
            txtNombre.Text = "";
            txtDireccion.Text = "";
        }
    }
}