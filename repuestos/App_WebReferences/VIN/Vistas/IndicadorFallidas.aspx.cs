using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class Vistas_indicadorFallidas : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_indicadorFallidas));

    private int _iMarca, _iAnoDesde, _iAnoHasta, _anoHasta, _anoDesde;

    protected String _nombreMarca, _concesionario;

    protected int _fallidas;

    private String _prefijoCodigo;

    private ControlAuto controlAuto;
    private ControlExcel ControlExcel;
    private ControlPersona controlPersona;
    private ControlPedido controlPedido;
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!(int.Parse(Session["permisos"].ToString()) == 1 ||
            int.Parse(Session["permisos"].ToString()) == 5))
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
        else 
        {
            hlkCotizacion.Visible = true;
            hlkFallidas.Visible = true;
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        controlPersona = new ControlPersona();
        controlPedido = new ControlPedido();

        int permisos = int.Parse(Session["permisos"].ToString());
        if (permisos != 1 || permisos != 5)
        {
            hlkConcesionario.Visible = false;
        }

        HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
        body.Attributes.Add("onLoad", "muestra();");

        //DESDE 
        try
        {
            _iAnoDesde = ddlAnoDesde.SelectedIndex;
            _anoDesde = int.Parse(ddlAnoDesde.SelectedItem.ToString());
        }
        catch (NullReferenceException)
        {
            _iAnoDesde = 0;
            _anoDesde = 0;
        }

        ddlAnoDesde.Items.Clear();

        for (int x = 2008; x < int.Parse(DateTime.Now.ToString("yyyy")) + 3; x++)
        {
            ddlAnoDesde.Items.Add(x.ToString());
        }
        //HASTA 
        try
        {
            _iAnoHasta = ddlAnoHasta.SelectedIndex;
            _anoHasta = int.Parse(ddlAnoHasta.SelectedItem.ToString());
        }
        catch (NullReferenceException)
        {
            _iAnoHasta = 0;
            _anoHasta = 0;
        }

        ddlAnoHasta.Items.Clear();

        for (int x = 2008; x < int.Parse(DateTime.Now.ToString("yyyy")) + 3; x++)
        {
            ddlAnoHasta.Items.Add(x.ToString());
        }

        controlAuto = new ControlAuto();
        ControlExcel = new ControlExcel();

        try
        {
            _iMarca = ddlMarca.SelectedIndex;
            _nombreMarca = ddlMarca.SelectedItem.ToString();
            _prefijoCodigo = _nombreMarca.Substring(0, 2).ToUpper()+"R";
        }
        catch (NullReferenceException)
        {
            _iMarca = 0;
            _nombreMarca = "";
            _prefijoCodigo = "";
        }
        ddlMarca.Items.Clear();
        foreach (String marca in controlAuto.obtenerMarcasPorRut(int.Parse(Session["rut"].ToString())))
        {
            ddlMarca.Items.Add(marca);
        }
        int desde = ddlMesDesde.SelectedIndex + 1;
        int hasta = ddlMesHasta.SelectedIndex + 1;
        String fechaDesde = _anoDesde + "-" + desde.ToString() + "-" + "01";
        String fechaHasta = _anoHasta + "-" + hasta.ToString() + "-" + "01";

        ddlAnoDesde.SelectedIndex = _iAnoDesde;
        ddlAnoHasta.SelectedIndex = _iAnoHasta;
        ddlMarca.SelectedIndex = _iMarca;


        if (rbtnCodigo.Checked)
        {
            try
            {
                _fallidas = controlPedido.detalleFallidaPorCodigo(_prefijoCodigo+txtCodigo.Text, _nombreMarca, fechaDesde, fechaHasta);
            }
            catch (Exception) { }
        }
        else if (rbtnOperario.Checked)
        {


            try
            {
                _fallidas = controlPedido.detalleFallidaPorCodigoOperario(int.Parse(txtRut.Text), _nombreMarca, fechaDesde, fechaHasta);
            }
            catch (Exception) { }
        }
        else if (rbtnTodos.Checked)
        {

            try
            {
                _fallidas = controlPedido.detalleFallidaPorTodos(_nombreMarca, fechaDesde, fechaHasta);
            }
            catch (Exception) { }
        }

    }//FIN DE PAGELOAD
}