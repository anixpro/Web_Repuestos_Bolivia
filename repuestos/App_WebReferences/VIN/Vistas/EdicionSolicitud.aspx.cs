using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;
using System.Data.SqlClient;

public partial class Vistas_EdicionSolicitud : System.Web.UI.Page
{
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    PdfHelper _pdf;
    public static int numero = 0;
    public static string cod = string.Empty;
    public static int cantidad = 0;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_EdicionSolicitud));

    string rut, marca;
    int idsilo = 0;
    int _cantidad = 0, inicio = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        _pdf = new PdfHelper(Server.MapPath("~\\doc"));
        string usuario = "";
        usuario = Session["rut"].ToString();
        string query = "SELECT nombre FROM persona WHERE rut = '" + usuario + "'";
        var dato = _ControlBD.usuario(query);
        txtusuario.Text = dato;
        mjsError.Visible = false;
        mjsError.InnerText = "";
        PanelListado.Visible = false;

        gvListSolicitud.SelectedIndexChanged += IngresaPrecio;
        btnBuscarSoli.Click += BuscaSolicitud;

        if (!IsPostBack)
        {
            LlenarComboMarcas();
            //gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT numeroSolicitud, fecha, marca, codRepto, cantidad, usuario, concesionario, precio_Solicitud, tipo, vin, envio FROM t_SolicitudCotizacion");
            //gvListSolicitud.DataBind();
        }
        try
        {
            //_marca = ComboMarcas.SelectedItem.Text;
            //_cantidad = int.Parse(txtCantidad.Text.Trim());
        }
        catch (NullReferenceException NullEx)
        {
            logger.Error("NullReferenceException en Page Load al asignar marca y cantidad. Inner: " + NullEx.InnerException + ". Stack: " + NullEx.StackTrace);
        }
        catch (Exception ex)
        {
            logger.Error("Exception en Page Load al asignar marca y cantidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    protected void IngresaPrecio(object sender, EventArgs e)
    {
        GridViewRow row = gvListSolicitud.SelectedRow;
        Session["numeroSolicitud"] = row.Cells[1].Text;
        numero = Convert.ToInt32(Session["numero"]);

        Session["codRepto"] = row.Cells[6].Text;
        cod = Convert.ToString(Session["codigo"]);

        Session["cantidad"] = row.Cells[8].Text;
        cantidad = Convert.ToInt32(Session["cantidad"]);
        //Response.Redirect("precio.aspx",true);
        

        string precio = Request.QueryString["txtPrecio"];
        string query = "";
        query = "UPDATE t_SolicitudCotizacion SET precio = '" + precio + "' where numeroSolictud = '" + numero + "' AND codRepto = '" + cod + "' AND cantidad = '" + cantidad + "'";
        _ControlBD.EjecutaQuery(query);
        string query2 = "SELECT * FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + numero + "'";
        gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query2);
        gvListSolicitud.DataBind();
        return;
        //Response.Write("<script language='javascript'> window.open('precio.aspx','window','HEIGHT=150,WIDTH=150,top=50,left=50,toolbar=yes,scrollbars=yes,resizable=no');</script>");
    }

    private string MessageBox(string p, int p_2)
    {
        throw new NotImplementedException();
    }
    private void LlenarComboMarcas()
    {
        combomarcas.Items.Clear();
        combomarcas.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _ControlBD.CrearMarcas(Session["rut"].ToString()))
        {
            combomarcas.Items.Add(marca);
        }
    }
    protected void BuscaSolicitud(object sender, EventArgs e)
    {
        string query = "";
        string marca;
        string numsoli, codre, user, fecdesde, fechasta, vin;
        marca = combomarcas.Text;
        numsoli = txtnumsoli.Text;
        fecdesde = txtdesde.Text;
        fechasta = txtHasta.Text;
        codre = txtCodRep.Text;
        user = txtusuario.Text;
        vin = txtVin.Text;

        if (marca != "-1")
        {
            if (numsoli == "")
            {
                numsoli = "0";
                Convert.ToInt32(numsoli);
            }
            if (fecdesde == "")
            {
                fecdesde = "2015/01/01";
            }
            if (fechasta == "")
            {
                fechasta = Convert.ToString(DateTime.Today);
            }
            if (codre == "")
            {
                codre = "0A";
            }
            if (vin == "")
            {
                vin = "allVin";
            }
            if (numsoli != "all")
            {
                PanelListado.Visible = true;
                query = "SELECT * FROM t_SolicitudCotizacion WHERE marca = '" + marca + "' AND numeroSolicitud = '" + numsoli + "' AND codRepto <> '" + codre + "' AND fecha BETWEEN '" + fecdesde + "' AND '" + fechasta + "' AND vin <> '" + vin + "' AND usuario= '" + user + "'";
                gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query);
                gvListSolicitud.DataBind();
                return;
            }
            if (codre != "0A")
            {
                PanelListado.Visible = true;
                query = "SELECT * FROM t_SolicitudCotizacion WHERE marca = '" + marca + "' AND numeroSolicitud <> '" + numsoli + "' AND codRepto = '" + codre + "' AND fecha BETWEEN '" + fecdesde + "' AND '" + fechasta + "' AND vin <> '" + vin + "' AND usuario= '" + user + "'";
                gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query);
                gvListSolicitud.DataBind();
                return;
            }
            if (vin != "allVin")
            {
                PanelListado.Visible = true;
                query = "SELECT * FROM t_SolicitudCotizacion WHERE marca = '" + marca + "' AND numeroSolicitud <> '" + numsoli + "' AND codRepto <> '" + codre + "' AND fecha BETWEEN '" + fecdesde + "' AND '" + fechasta + "' AND vin = '" + vin + "' AND usuario= '" + user + "'";
                gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query);
                gvListSolicitud.DataBind();
                return;
            }
            else
            {
                PanelListado.Visible = true;
                query = "SELECT * FROM t_SolicitudCotizacion WHERE marca = '" + marca + "' AND numeroSolicitud = '" + numsoli + "' AND codRepto = '" + codre + "' AND fecha BETWEEN '" + fecdesde + "' AND '" + fechasta + "' AND vin = '" + vin + "' AND usuario = '" + user + "'";
                gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query);
                gvListSolicitud.DataBind();
            }
        }
        else
        {
            mjsError.InnerText = "Debe Ingresar un Vin Valido";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
    }
}