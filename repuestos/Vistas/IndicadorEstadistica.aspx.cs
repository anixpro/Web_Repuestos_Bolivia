using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
// Namespaces del logger
using log4net;
using log4net.Config;

public partial class Vistas_indicadorEstadistica : System.Web.UI.Page
{
    private int _iMarca, _iAnoDesde, _iAnoHasta, _anoHasta, _anoDesde;
    protected String _nombreMarca,_concesionario;
    private String _prefijoCodigo;
    private ControlAuto controlAuto;
    private ControlExcel ControlExcel;
    private ControlPersona controlPersona;
    private ControlPedido controlPedido;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_indicadorEstadistica));

    int permisos = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        /*
         * Permisos del sitio:
         * Se permite:
         *  - Administradores
         *  - Supervisores
         *  - Gerentes
         * No se permite:
         *  - Operadores
         *  - Cotizadores
         */
        permisos = int.Parse(Session["permisos"].ToString());
        if (permisos == 3 ||
            permisos == 4)
        {
            Response.Redirect("../Index.aspx?evento=ev2");
        }
        else if (permisos == 1 ||
            permisos == 5)
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

        if (permisos == 1)
        {
            lblConcesionario.Visible = false;
        }
        else if (permisos == 2)
        {
            lblConcesionario.Visible = true;
            _concesionario = controlPersona.miConcesionario((Session["rut"].ToString()));
            lblConcesionario.Text = _concesionario;
            hlkConcesionario.Visible = false;
        }

        HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
        body.Attributes.Add("onLoad", "muestra();");

        if (Request.QueryString["rut"] != null)
        {
            try
            {
                sqldDetallePedidoPorRut.SelectCommand = controlPedido.detallePedidoPorRut(int.Parse(Request.QueryString["rut"].ToString()), int.Parse(Session["rut"].ToString()));
                gridDetallePedidoPorRut.DataBind();

                gridDetallePedidoPorRutResumen.DataSource = controlPedido.detallePedidoPorRutResumenTotal(int.Parse(Request.QueryString["rut"].ToString()), int.Parse(Session["rut"].ToString()));
                gridDetallePedidoPorRutResumen.DataBind();

                gridDetallePedidoPorCodigoResumen.DataSource = "";
                gridDetallePedidoPorCodigoResumen.DataBind();
            }
            catch (Exception ex) {
                logger.Error("en Page_Load Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            }
        }
        else if (Request.QueryString["codigo"] != null)
        {
            try
            {
                sqldDetallePedidoPorCodigo.SelectCommand = controlPedido.detallePedidoPorCodigo(Request.QueryString["codigo"].ToString(), int.Parse(Session["rut"].ToString()));
                gridDetallePedidoPorCodigo.DataBind();

                gridDetallePedidoPorCodigoResumen.DataSource = controlPedido.detallePedidoPorCodigoResumenTotal(Request.QueryString["codigo"].ToString(), int.Parse(Session["rut"].ToString()));
                gridDetallePedidoPorCodigoResumen.DataBind();

                gridDetallePedidoPorRutResumen.DataSource = "";
                gridDetallePedidoPorRutResumen.DataBind();
            }
            catch (Exception ex) {
                logger.Error("en Page_Load Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            }
        }

        //DESDE 
        try
        {
            _iAnoDesde = ddlAnoDesde.SelectedIndex;
            _anoDesde = int.Parse(ddlAnoDesde.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            logger.Error("en Page_Load NullReferenceException Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
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
        catch (NullReferenceException ex)
        {
            logger.Error("en Page_Load NullReferenceException Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
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
            _prefijoCodigo = _nombreMarca.Substring(0, 3).ToUpper();
        }
        catch (NullReferenceException ex)
        {
            logger.Error("en Page_Load NullReferenceException Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            _iMarca = 0;
            _nombreMarca = "";
            _prefijoCodigo="";
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
                if (int.Parse(Session["permisos"].ToString()) == 1)
                {
                    sqldDetallePedidoPorCodigo.SelectCommand = controlPedido.detallePedidoPorCodigo(_prefijoCodigo + txtCodigo.Text, int.Parse(Session["rut"].ToString()), _nombreMarca, fechaDesde, fechaHasta);
               
                }
                else if (int.Parse(Session["permisos"].ToString()) == 2)
                {
                    sqldDetallePedidoPorCodigo.SelectCommand = controlPedido.detallePedidoPorCodigo(_prefijoCodigo + txtCodigo.Text, int.Parse(Session["rut"].ToString()), _nombreMarca, fechaDesde, fechaHasta);
               
                }
                gridDetallePedidoPorCodigo.DataBind();

                //si no entra el catch hago tambien el grid de resumen

                gridDetallePedidoPorCodigoResumen.DataSource = controlPedido.detallePedidoPorCodigoResumen(_prefijoCodigo + txtCodigo.Text, int.Parse(Session["rut"].ToString()), _nombreMarca, fechaDesde, fechaHasta);
                gridDetallePedidoPorCodigoResumen.DataBind();


                //limpio los grids
                sqldDetallePedidoPorRut.SelectCommand = ""; gridDetallePedidoPorRut.DataBind();
                sqldDetallePedidoPorTodos.SelectCommand = ""; gridDetallePedidoPorTodos.DataBind();

                gridDetallePedidoPorRutResumen.DataSource = "";
                gridDetallePedidoPorRutResumen.DataBind();
            }
            catch (Exception ex) {
                logger.Error("en Page_Load Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            }
        }
        else if (rbtnOperario.Checked)
        {
            try
            {
                sqldDetallePedidoPorRut.SelectCommand = controlPedido.detallePedidoPorRut(int.Parse(txtRut.Text), int.Parse(Session["rut"].ToString()), _nombreMarca, fechaDesde, fechaHasta);
                gridDetallePedidoPorRut.DataBind();

                //si no entra el catch hago tambien el grid de resumen
                gridDetallePedidoPorRutResumen.DataSource = controlPedido.detallePedidoPorRutResumen(int.Parse(txtRut.Text), int.Parse(Session["rut"].ToString()), _nombreMarca, fechaDesde, fechaHasta);
                gridDetallePedidoPorRutResumen.DataBind();

                //limpio los grids
                sqldDetallePedidoPorCodigo.SelectCommand = ""; gridDetallePedidoPorCodigo.DataBind();
                sqldDetallePedidoPorTodos.SelectCommand = ""; gridDetallePedidoPorTodos.DataBind();

                gridDetallePedidoPorCodigoResumen.DataSource = "";
                gridDetallePedidoPorCodigoResumen.DataBind();
            }
            catch (Exception ex) {
                logger.Error("en Page_Load Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            }
        }
        else if (rbtnTodos.Checked)
        {
            try
            {
                gridDetallePedidoPorCodigoResumen.DataSource = "";
                gridDetallePedidoPorCodigoResumen.DataBind();

                gridDetallePedidoPorRutResumen.DataSource = "";
                gridDetallePedidoPorRutResumen.DataBind();

                sqldDetallePedidoPorTodos.SelectCommand = controlPedido.detallePedidoPorTodos(int.Parse(Session["rut"].ToString()),_nombreMarca, fechaDesde, fechaHasta);
                gridDetallePedidoPorTodos.DataBind();

                //limpio los grids
                sqldDetallePedidoPorRut.SelectCommand = ""; gridDetallePedidoPorRut.DataBind();
                sqldDetallePedidoPorCodigo.SelectCommand = ""; gridDetallePedidoPorCodigo.DataBind();
            }
            catch (Exception ex) {
                logger.Error("En Page_Load Inner:" + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            }
        }

    }//FIN DE PAGELOAD
}