using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;
using System.Configuration;

public partial class Vistas_SolicitudLlave : System.Web.UI.Page
{
    public string cantidad = "";
    public string codigo = "";
    public string detalle = "";
    public string marca = "";
    public string marcaAbreviada = "";
    public string creador = "";
    public string nombreUser = "";
    public string dealer = "";
    public string direccion = "";
    public string tipoPed = "";
    public string prioridad = "";
    public string numBO = "";
    public string numBO2 = "";
    public string idPedido = "";
    SapAPI _sapApi;
    RutHelper _rutHelper;
    ControlVfc _controlVFC;
    ControlBD _controlBD;

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_SolicitudLlave));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        _sapApi = new SapAPI();
        _rutHelper = new RutHelper();
        ddlSucursales.Enabled = false;
        ddlSucursales.Visible = false;

        _controlBD = new ControlBD();

        if (!IsPostBack)
        {
            marca = "";
            codigo = "";
            cantidad = "";
            txtCreador.Text = _rutHelper.GetRutConDigito(Session["rut"].ToString());
            txtCreador.Enabled = false;
            txtDealer.Text = _sapApi.GetNombreDealer(Session["rut"].ToString());
            txtDealer.Enabled = false;
            txtDirección.Text = _sapApi.GetDireccionSucursal(Session["rut"].ToString());
            txtDirección.Enabled = false;
            txtNombreUsuario.Text = _sapApi.GetNombreUsuario(Session["rut"].ToString());
            txtNombreUsuario.Enabled = false;

            marcaAbreviada = _sapApi.GetAbreviadoMarca(marca);

            // Si usuario es multiSucursal, se muestra direcciones multiples
            if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
            {
                ddlSucursales.Enabled = true;
                ddlSucursales.Visible = true;
                txtDirección.Enabled = false;
                txtDirección.Visible = false;

                if (ddlSucursales.Items.Count == 0)
                {
                    ddlSucursales.Items.Clear();
                    ddlSucursales.Items.Add(new ListItem("Seleccionar", "0"));
                    //obtengo las sucursales y lleno la lista con las direcciones de despacho
                    //Elección de ship code
                    foreach (string lista in _controlBD.ObtenerSucursalesPorConcesionario(_sapApi.GetNombreDealer(Session["rut"].ToString())))
                    {
                        ddlSucursales.Items.Add(lista);
                    }
                }
            }
        }
        else
        {

        }
    }

    protected void btnAceptar_Click(object sender, EventArgs e)
    {
        _controlVFC = new ControlVfc();
        _controlBD = new ControlBD();

        if (ddlSucursales.Enabled)
        {
            if (ddlSucursales.SelectedValue == "0")
            {
                msjesError.InnerText = "Debe seleccionar una destinatario de mercancía (dirección)";
                msjesError.Visible = true;
                return;
            }
        }

        string cantidad = txtCantidad.Text;
        string codigo = txtCodigo.Text;
        string detalle = txtDetalle.Text;
        string marca = ddlMarcas.SelectedValue;
        marca = _sapApi.GetCmpCod(marca);
        string creador = txtCreador.Text;
        string nombreUser = txtNombreUsuario.Text;
        string dealer = txtDealer.Text;
        string direccion = txtDirección.Text;
        string tipoPed = "NORMAL";
        string prioridad = "NORMAL";
        string vin = txtVin.Text;
        string numResp = "";
        string numResp2 = "";
        string idPedido = "000000";
        string cc = _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString());
        string codsap = _sapApi.GetCodigoShipCode(Session["rut"].ToString());

        if (ConfigurationManager.AppSettings["ambiente"].ToString() != "p")
        {
            creador = "22222222-2";
        }

        if (vin == "")
        {
            msjesError.InnerText = "Debe ingresar un numero VIN";
            msjesError.Visible = true;
            return;
        }

        if (codigo == "")
        {
            msjesError.InnerText = "Debe ingresar el código de repuesto";
            msjesError.Visible = true;
            return;
        }

        if (cantidad == "")
        {
            msjesError.InnerText = "Debe ingresar una cantidad";
            msjesError.Visible = true;
            return;
        }

        //Se declaran las variables de respuestas del WS
        List<String> identificadores = new List<String>();
        List<String> detalles = new List<String>();

        try
        {
            //Se llama al método que inserta el vfc
            _controlVFC.hacerVfc(cantidad, codigo, detalle, marca, vin, creador, nombreUser, dealer, direccion, tipoPed, prioridad, "", "", cc, codsap);

            identificadores = _controlVFC.getIdentificadores();

            numResp = identificadores[0];
            numResp2 = identificadores[1];

            //se insertan los datos en la tabla BACKORDER
            _controlBD.InsertarDatos(@"insert into VFC(num_VFC,id_pedido,rut_user,fecha_creacion,
            codigo_rep,marca,cantidad,detalle_rep,cod_vin)
            values('" + numResp.Substring(9, numResp.Length - 9) + "','" + idPedido + "','" + creador + "' , GETDATE(), " +
                " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "','" + vin + "')");
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Error: " + ex.Message;
            return;
        }
        
        msjesError.Visible = true;
        msjesError.InnerText = "Transacción satisfactoria. El número de la solicitud es: " + numResp.Substring(9, numResp.Length - 9).ToString();
        PanelBotonera.Visible = false;
    }
}
