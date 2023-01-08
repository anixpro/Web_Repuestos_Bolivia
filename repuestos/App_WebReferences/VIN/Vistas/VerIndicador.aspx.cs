using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using log4net;
using log4net.Config;

public partial class Vistas_verIndicador : System.Web.UI.Page
{
    // Variables de indicadores
    private String _ruta;
    private int _iMarca,_iAno,_iMarcaComparacion1,_iMarcaComparacion2,_iAnoComparacion1,_iAnoComparacion2;
    protected String _nombreMarca,_nombreMarcaComparacion;
    protected int _ano, _cantidadMeses,_iDesde,_triDesde,_cantidadTrimestres,_iComparacion1,_iComparacion2,_anoComparacion1,_anoComparacion2,_compraMes1,_compraMes2,_metaMes1,_metaMes2;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_verIndicador));

    // Nombre de concesionario
    protected String _concesionario;

    //TRIMESTRES compras
    protected int _primerTrimestreCompras;
    protected int _segundoTrimestreCompras;
    protected int _tercerTrimestreCompras;
    protected int _cuartoTrimestreCompras;

    //TRIMESTRES metas
    protected int _primerTrimestreMetas;
    protected int _segundoTrimestreMetas;
    protected int _tercerTrimestreMetas;
    protected int _cuartoTrimestreMetas;

    // MESES
    protected int[] _metaMeses;
    protected int[] _compraMeses;

    // MESES meta
    protected int _metaEnero;
    protected int _metaFebrero;
    protected int _metaMarzo;
    protected int _metaAbril;
    protected int _metaMayo;
    protected int _metaJunio;
    protected int _metaJulio;
    protected int _metaAgosto;
    protected int _metaSeptiembre;
    protected int _metaOctubre;
    protected int _metaNoviembre;
    protected int _metaDiciembre;

    // MESES compra
    protected int _compraEnero;
    protected int _compraFebrero;
    protected int _compraMarzo;
    protected int _compraAbril;
    protected int _compraMayo;
    protected int _compraJunio;
    protected int _compraJulio;
    protected int _compraAgosto;
    protected int _compraSeptiembre;
    protected int _compraOctubre;
    protected int _compraNoviembre;
    protected int _compraDiciembre;

    // Instanciacion de objetos
    private ControlAuto controlAuto;
    private ControlExcel ControlExcel;
    private ControlPersona controlPersona;

    int permisos = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        liCotizacion.Visible = false; 
        liFallidas.Visible = false; 
        hlkCotizacion.Visible = true;
        hlkFallidas.Visible = true;

        permisos = int.Parse(Session["permisos"].ToString());

        if (permisos == 3 ||
            permisos == 4)
        {
            Response.Redirect("../Index.aspx?evento=ev2");
        }

        else if (
            permisos == 1 ||
            permisos == 5)
        {
            liCotizacion.Visible = true;
            liFallidas.Visible = true;
        }

        controlAuto = new ControlAuto();

        foreach (String concesionario in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(concesionario);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        controlPersona= new ControlPersona();

        if (permisos == 1)
        {           
            ddlConcesionario.Visible = true;
        }

        if (permisos == 2)
        {
            _concesionario = controlPersona.miConcesionario(Session["rut"].ToString());
            if (_concesionario == "")
            {
                msjesError.InnerText = "Este usuario no tiene concesionario asociado";
                msjesError.Visible = true;
                btnAceptar.Enabled = false;
                btnAceptarTrimestre.Enabled = false;
                return;
            }
            lblConcesionario.Text = _concesionario;
            hlkConcesionario.Visible = false;
            ddlConcesionario.Visible = false;
        }

        HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
        if (rbtnUnoAUno.Checked)
        {
            body = Master.FindControl("bodyMaster") as HtmlGenericControl;
            body.Attributes.Add("onLoad", "drawChartComparacion();");
        }
        else if (rbtnNormal.Checked)
        {
            if (rbtnMes.Checked)
            {
                body = Master.FindControl("bodyMaster") as HtmlGenericControl;
                body.Attributes.Add("onLoad", "drawChartMeses();");
            }
            else if (rbtnTrimestre.Checked)
            {
                body = Master.FindControl("bodyMaster") as HtmlGenericControl;
                body.Attributes.Add("onLoad", "drawChartTrimestre();");
            }
        }

        _ruta = "";
        try
        {
            _iAno = ddlAno.SelectedIndex;
            _ano = int.Parse(ddlAno.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            _iAno = 0;
            _ano = 0;
            Debug.Write(ex);
        }

        ddlAno.Items.Clear();

        for(int x=2008; x<int.Parse(DateTime.Now.ToString("yyyy"))+3; x++)
        {
            ddlAno.Items.Add(x.ToString());
        }

        ControlExcel= new ControlExcel();

        try
        {
            _iMarca = ddlMarca.SelectedIndex;
            _nombreMarca = ddlMarca.SelectedItem.ToString();
        }
        catch (NullReferenceException ex)
        {
            _iMarca = 0;
            _nombreMarca = "";
            Debug.Write(ex);
        }

        try
        {
            _iMarcaComparacion1 = ddlMarcaComparacion.SelectedIndex;
            _nombreMarcaComparacion = ddlMarcaComparacion.SelectedItem.ToString();
        }
        catch (NullReferenceException ex)
        {
            _iMarcaComparacion1 = 0;
            _nombreMarcaComparacion = "";
            Debug.Write(ex);
        }

        try
        {
            _iAnoComparacion1 = ddlAñoComparacion1.SelectedIndex;
            _anoComparacion1 = int.Parse(ddlAñoComparacion1.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            _iAnoComparacion1 = 0;
            _anoComparacion1 = 0;
            Debug.Write(ex);
        }

        try
        {
            _iAnoComparacion2 = ddlAñoComparacion2.SelectedIndex;
            _anoComparacion2 = int.Parse(ddlAñoComparacion2.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            _iAnoComparacion2 = 0;
            _anoComparacion2 = 0;
            Debug.Write(ex);
        }


        ddlAñoComparacion1.Items.Clear();
        for (int x = 2008; x < int.Parse(DateTime.Now.ToString("yyyy")) + 3; x++)
        {
            ddlAñoComparacion1.Items.Add(x.ToString());
        }

        ddlAñoComparacion2.Items.Clear();
        for (int x = 2008; x < int.Parse(DateTime.Now.ToString("yyyy")) + 3; x++)
        {
            ddlAñoComparacion2.Items.Add(x.ToString());
        }

        String[] todas = controlAuto.obtenerMarcasPorRut(int.Parse(Session["rut"].ToString()));

        ddlMarca.Items.Clear();
        foreach (String marca in todas)
        {
            ddlMarca.Items.Add(marca);
        }

        ddlMarcaComparacion.Items.Clear();
        foreach (String marca in todas)
        {
            ddlMarcaComparacion.Items.Add(marca);
        }

        /*CONCESIONARIO SEGUN MI LOGUIN(A TRAVES DEL RUT SACO LA SUCURSAL Y POSTERIORMENTE EL CONCESIONARIO QUE ME CORRESPONDE)
          EN CASO DE SER ADMINISTRADOR LO SELECCIONO SEGUN EL DROPDOWNLIST
        */
       
        String concesionario = "";
        if (int.Parse(Session["permisos"].ToString()) == 1)
        {
            concesionario = ddlConcesionario.SelectedItem.Text;
        }
        else if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            concesionario = controlAuto.obtenerconcesionarioPorRut(int.Parse(Session["rut"].ToString()));
        }

        //GUARDO LOS DATOS DE LA BASE DE DATOS EN INT GLOBALES DECLARADOS AL PRINCIPIO DE COMPRAS Y METAS
        
        // compras
        _primerTrimestreCompras = ControlExcel.primerTrimestre(_nombreMarca, concesionario,_ano);
        _segundoTrimestreCompras = ControlExcel.segundoTrimestre(_nombreMarca, concesionario, _ano);
        _tercerTrimestreCompras = ControlExcel.tercerTrimestre(_nombreMarca, concesionario, _ano);
        _cuartoTrimestreCompras = ControlExcel.cuartoTrimestre(_nombreMarca, concesionario, _ano);

        // metas
        _primerTrimestreMetas = ControlExcel.primerTrimestreMetas(_nombreMarca, concesionario, _ano);
        _segundoTrimestreMetas = ControlExcel.segundoTrimestreMetas(_nombreMarca, concesionario, _ano);
        _tercerTrimestreMetas = ControlExcel.tercerTrimestreMetas(_nombreMarca, concesionario, _ano);
        _cuartoTrimestreMetas = ControlExcel.cuartoTrimestreMetas(_nombreMarca, concesionario, _ano);

        //GUARDO LOS MESES DE METAS Y COMPRAS
        _metaMeses = ControlExcel.metaMeses(_nombreMarca, concesionario, _ano);
        if (_metaMeses.Length != 0)
        {
            _metaEnero = _metaMeses[0];
            _metaFebrero = _metaMeses[1];
            _metaMarzo = _metaMeses[2];
            _metaAbril = _metaMeses[3];
            _metaMayo = _metaMeses[4];
            _metaJunio = _metaMeses[5];
            _metaJulio = _metaMeses[6];
            _metaAgosto = _metaMeses[7];
            _metaSeptiembre = _metaMeses[8];
            _metaOctubre = _metaMeses[9];
            _metaNoviembre = _metaMeses[10];
            _metaDiciembre = _metaMeses[11];
        }
        _compraMeses = ControlExcel.compraMeses(_nombreMarca, concesionario, _ano);
        if (_compraMeses.Length != 0)
        {
            _compraEnero = _compraMeses[0];
            _compraFebrero = _compraMeses[1];
            _compraMarzo = _compraMeses[2];
            _compraAbril = _compraMeses[3];
            _compraMayo = _compraMeses[4];
            _compraJunio = _compraMeses[5];
            _compraJulio = _compraMeses[6];
            _compraAgosto = _compraMeses[7];
            _compraSeptiembre = _compraMeses[8];
            _compraOctubre = _compraMeses[9];
            _compraNoviembre = _compraMeses[10];
            _compraDiciembre = _compraMeses[11];
        }

        ddlAno.SelectedIndex = _iAno;
        ddlMarca.SelectedIndex = _iMarca;

        _iDesde = ddlDesde.SelectedIndex;
        _cantidadMeses = ddlHasta.SelectedIndex-ddlDesde.SelectedIndex;

        _triDesde = ddlTriDesde.SelectedIndex;
        _cantidadTrimestres = ddlTriHasta.SelectedIndex - ddlTriDesde.SelectedIndex;

        _iComparacion1 = ddlComparacionMes1.SelectedIndex+1;
        _iComparacion2 = ddlComparacionMes2.SelectedIndex+1;

        _compraMes1 = ControlExcel.compraDeUnMes(_nombreMarca, concesionario, _anoComparacion1,_iComparacion1);
        _compraMes2 = ControlExcel.compraDeUnMes(_nombreMarca, concesionario, _anoComparacion2, _iComparacion2);

        _metaMes1=ControlExcel.metaDeUnMes(_nombreMarca, concesionario, _anoComparacion1,_iComparacion1);
        _metaMes2 = ControlExcel.metaDeUnMes(_nombreMarca, concesionario, _anoComparacion2, _iComparacion2);

        try
        {
            _ruta = "~/doc/archivosCsv/" + _ano.ToString() + "/" + controlAuto.obtenerAbreviado(_nombreMarca) + " " + _ano.ToString() + ".csv";
        }
        catch (Exception ex)
        {
            logger.Warn("Excepcion en page load de verIndicador. Falla asignar _ruta. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        hlkExcel.NavigateUrl = _ruta;
    }//FIN DE PAGELOAD
}