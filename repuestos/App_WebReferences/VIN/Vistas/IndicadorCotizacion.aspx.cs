using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class Vistas_indicadorCotizacion : System.Web.UI.Page
{
    protected int _iMarca, _iAnoDesde, _iAnoHasta, _cantidadMeses, _iDesde, _anoHasta, _anoDesde, _mesDesde, mesHasta,_ano,_iAno;
    protected int[] _cotizacionVsCompra;
    int permiso = 0;

    protected String _nombreMarca, _concesionario;
    private String _prefijoCodigo;

    private ControlAuto controlAuto;
    private ControlExcel ControlExcel;
    private ControlPersona controlPersona;
    private ControlPedido controlPedido;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_indicadorCotizacion));

    protected void Page_Init(object sender, EventArgs e)
    {
        permiso = int.Parse(Session["permisos"].ToString());

        if (permiso > 2 && permiso < 5)
        {
            Response.Redirect("../Index.aspx?evento=ev2");
        }
        else if (permiso == 1 && permiso == 5)
        {
            hlkCotizacion.Visible = true;
            hlkFallidas.Visible = true;
        }
        else
        {
            Response.Redirect("../Index.aspx?evento=ev2");
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        controlPersona = new ControlPersona();
        controlPedido = new ControlPedido();

        _cotizacionVsCompra = new int[2];
        _cotizacionVsCompra[0] = 0;
        _cotizacionVsCompra[1] = 0;

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            hlkConcesionario.Visible = false;
        }
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            _concesionario = controlPersona.miConcesionario(Session["rut"].ToString());
            lblConcesionario.Text = _concesionario;
        }

        HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
        body.Attributes.Add("onLoad", "muestra();");

        //DESDE 
        try
        {
            _iAno = ddlAno.SelectedIndex;
            _ano = int.Parse(ddlAno.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            _iAno = 0;
            _ano = 0;
            msjesError.Visible = true;
            msjesError.InnerHtml = "Error al calcular indicadores. Contacte al administrador";
            logger.Error("en Page_Load NullReference Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        ddlAno.Items.Clear();

        for (int x = 2008; x < int.Parse(DateTime.Now.ToString("yyyy")) + 3; x++)
        {
            ddlAno.Items.Add(x.ToString());
        }

        controlAuto = new ControlAuto();
        ControlExcel = new ControlExcel();

        ddlAno.SelectedIndex = _iAno;

        try
        {
            _iMarca = ddlMarca.SelectedIndex;
            _nombreMarca = ddlMarca.SelectedItem.ToString();
            _prefijoCodigo = controlPedido.obtienePrefijoMarca(ddlMarca.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            _iMarca = 0;
            _nombreMarca = "";
            _prefijoCodigo = "";

            msjesError.Visible = true;
            msjesError.InnerHtml = "Error al calcular indicadores. Contacte al administrador";
            logger.Error("en Page_Load NullReference Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        ddlMarca.Items.Clear();
        foreach (String marca in controlAuto.obtenerMarcasPorRut(int.Parse(Session["rut"].ToString())))
        {
            ddlMarca.Items.Add(marca);
        }
        int desde = ddlMesDesde.SelectedIndex + 1;
        int hasta = ddlMesHasta.SelectedIndex + 1;
        String fechaDesde = _ano + "-" + desde.ToString() + "-" + "01";
        String fechaHasta = _ano + "-" + hasta.ToString() + "-" + "01";

        ddlMarca.SelectedIndex = _iMarca;

     
        if (rbtnCodigo.Checked)
        {
            try
            {
                _cotizacionVsCompra = controlPedido.detalleCotizacionVsCompraPorCodigo(_prefijoCodigo + txtCodigo.Text,  _nombreMarca, fechaDesde, fechaHasta);
            }
            catch (Exception ex) {
                msjesError.Visible = true;
                msjesError.InnerHtml = "Error al determinar cotizacion y compras. Contacte al administrador";
                logger.Error("en [Page_Load] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
        }
        else if (rbtnOperario.Checked)
        {
            try
            {
                _cotizacionVsCompra = controlPedido.detalleCotizacionVsCompraPorCodigoOperario(int.Parse(txtRut.Text), _nombreMarca, fechaDesde, fechaHasta);
       
            }
            catch (Exception ex)
            {
                msjesError.Visible = true;
                msjesError.InnerHtml = "Error al determinar cotizacion y compras. Contacte al administrador";
                logger.Error("en [Page_Load] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
        }
        else if (rbtnCodigoDestinatario.Checked)
        {
            try
            {
                _cotizacionVsCompra = controlPedido.detalleCotizacionVsCompraPorCodigoSucursal(txtCodigoDespacho.Text, _nombreMarca, fechaDesde, fechaHasta);
            }
            catch (Exception ex)
            {
                msjesError.Visible = true;
                msjesError.InnerHtml = "Error al determinar cotizacion y compras. Contacte al administrador";
                logger.Error("en [Page_Load] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
        }
        else if (rbtnTodos.Checked)
        {
          
            try
            {
                _cotizacionVsCompra = controlPedido.detalleCotizacionVsCompraPorTodos(_nombreMarca, fechaDesde, fechaHasta);
       
            }
            catch (Exception ex)
            {
                msjesError.Visible = true;
                msjesError.InnerHtml = "Error al determinar cotizacion y compras. Contacte al administrador";
                logger.Error("en [Page_Load] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
        }
    }//FIN DE PAGELOAD
}
