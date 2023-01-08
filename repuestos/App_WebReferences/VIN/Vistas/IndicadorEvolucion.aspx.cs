using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Diagnostics;
using log4net;
using log4net.Config;

public partial class Vistas_indicadorEvolucion : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_indicadorEvolucion));
    private int _iAno;

    protected String[] _marcasSelecionadas;
    protected int _ano, _cantidadMeses, _iDesde, _cantidadMarcas, _cantidadTodasLasCompras,_triDesde,_cantidadTrimestres;


    protected String _marca, _concesionario;

    //MESES
    protected int[] _metaMeses, _compraMeses, _comprasTodosLosMeses;

    protected int[] _comprasEnero;
    protected int[] _comprasFebrero;
    protected int[] _comprasMarzo;
    protected int[] _comprasAbril;
    protected int[] _comprasMayo;
    protected int[] _comprasJunio;
    protected int[] _comprasJulio;
    protected int[] _comprasAgosto;
    protected int[] _comprasSeptiembre;
    protected int[] _comprasOctubre;
    protected int[] _comprasNoviembre;
    protected int[] _comprasDiciembre;


    private ControlAuto controlAuto;
    private ControlExcel ControlExcel;
    private ControlPersona controlPersona;

    private List<int> _compras;

    protected void Page_Init(object sender, EventArgs e)
    {
        controlAuto = new ControlAuto();
        foreach (String concesionario in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(concesionario);
        }

        int permisos = int.Parse(Session["permisos"].ToString());

        if (permisos == 2 ||
            permisos == 3 ||
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
        controlPersona = new ControlPersona();
        msjesError.Visible = false;

        if (int.Parse(Session["permisos"].ToString()) == 1 ||
            int.Parse(Session["permisos"].ToString()) == 5
            )
        {
            ddlConcesionario.Visible = true;
            lblConcesionario.Visible = false;
        }
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            _concesionario = controlPersona.miConcesionario(Session["rut"].ToString());
            lblConcesionario.Text = _concesionario;
            hlkConcesionario.Visible = false;
            ddlConcesionario.Visible = false;
            lblConcesionario.Visible = true;

        }

        HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
        body.Attributes.Add("onLoad", "muestra();");

        btnAceptar.OnClientClick = "javascript:return validaCheck();";


        if (rbtnMes.Checked)
        {
            body = Master.FindControl("bodyMaster") as HtmlGenericControl;
            body.Attributes.Add("onLoad", "drawChartMeses();muestra();");
        }
        else
        if (rbtnTrimestre.Checked)
        {
            body = Master.FindControl("bodyMaster") as HtmlGenericControl;
            body.Attributes.Add("onLoad", "drawChartTrimestre();muestra();");
        }

        try
        {
            _iAno = ddlAno.SelectedIndex;
            _ano = int.Parse(ddlAno.SelectedItem.ToString());
        }
        catch (NullReferenceException ex)
        {
            _iAno = 0;
            _ano = 0; Debug.Write(ex);
        }

        ddlAno.Items.Clear();

        for (int x = 2008; x < int.Parse(DateTime.Now.ToString("yyyy")) + 3; x++)
        {
            ddlAno.Items.Add(x.ToString());
        }


        ControlExcel = new ControlExcel();

        String[] marcas = controlAuto.obtenerMarcasPorRut(int.Parse(Session["rut"].ToString()));

        try
        {
            List<String> marcaARescatar = new List<String>();
            for (int i = 0; i < cbxlMarcas.Items.Count; i++)
            {

                if (cbxlMarcas.Items[i].Selected)
                {
                    marcaARescatar.Add(cbxlMarcas.Items[i].Text);                   
                }

            }
            _marcasSelecionadas = marcaARescatar.ToArray(); 
        }
        catch (NullReferenceException ex)
        {
            _marca = "";
            Debug.Write(ex);
        }

        cbxlMarcas.Items.Clear();
        foreach (String marca in marcas)
        {
            cbxlMarcas.Items.Add(marca);
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
        
      
        _compras = new List<int>();
        foreach (String marcaseleccionada in _marcasSelecionadas) 
        {
            _compraMeses = ControlExcel.compraMeses(marcaseleccionada, concesionario, _ano);
            foreach (int compraMarca in _compraMeses)
            {
                _compras.Add(compraMarca);
            }
                        
        }

        //TODAS LAS COMPRAS
                List<int> cEnero= new List<int>();
                List<int> cFebrero = new List<int>();
                List<int> cMarzo = new List<int>();
                List<int> cAbril = new List<int>();
                List<int> cMayo = new List<int>();
                List<int> cJunio = new List<int>();
                List<int> cJulio= new List<int>();
                List<int> cAgosto = new List<int>();
                List<int> cSeptiembre = new List<int>();
                List<int> cOctubre = new List<int>();
                List<int> cNoviembre = new List<int>();
                List<int> cDiciembre = new List<int>();
        foreach (String marcaseleccionada in _marcasSelecionadas)
        {
            int enero = 0;
            int febrero = 1;
            int marzo = 2;
            int abril = 3;
            int mayo = 4;
            int junio = 5;
            int julio = 6;
            int agosto = 7;
            int septiembre = 8;
            int octubre = 9;
            int noviembre = 10;
            int diciembre = 11;
            try
            {//[0] = 3156109[12] = 571323
                cEnero.Add(_compras[enero]);

                cFebrero.Add(_compras[febrero]);

                cMarzo.Add(_compras[marzo]);

                cAbril.Add(_compras[abril]);

                cMayo.Add(_compras[mayo]);

                cJunio.Add(_compras[junio]);

                cJulio.Add(_compras[julio]);

                cAgosto.Add(_compras[agosto]);

                cSeptiembre.Add(_compras[septiembre]);

                cOctubre.Add(_compras[octubre]);

                cNoviembre.Add(_compras[noviembre]);

                cDiciembre.Add(_compras[diciembre]);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Write(ex);
            }
            enero += 12;
            febrero += 12;
            marzo += 12;
            abril += 12;
            mayo += 12;
            junio += 12;
            julio += 12;
            agosto += 12;
            septiembre += 12;
            octubre += 12;
            noviembre += 12;
            diciembre += 12;
        }
        _comprasEnero = cEnero.ToArray<int>();
        _comprasFebrero = cFebrero.ToArray<int>();
        _comprasMarzo = cMarzo.ToArray<int>();
        _comprasAbril = cAbril.ToArray<int>();
        _comprasMayo = cMayo.ToArray<int>();
        _comprasJunio = cJunio.ToArray<int>();
        _comprasJulio = cJulio.ToArray<int>();
        _comprasAgosto = cAgosto.ToArray<int>();
        _comprasSeptiembre = cSeptiembre.ToArray<int>();
        _comprasOctubre = cOctubre.ToArray<int>();
        _comprasNoviembre = cNoviembre.ToArray<int>();
        _comprasDiciembre = cDiciembre.ToArray<int>();

        ddlAno.SelectedIndex = _iAno;

        _iDesde = ddlDesde.SelectedIndex;
        _cantidadMeses = ddlHasta.SelectedIndex - ddlDesde.SelectedIndex;

        _triDesde = ddlTriDesde.SelectedIndex;
        _cantidadTrimestres = ddlTriHasta.SelectedIndex - ddlTriDesde.SelectedIndex;

        _cantidadMarcas = _marcasSelecionadas.Length;
        _comprasTodosLosMeses = _compras.ToArray<int>();
       
        _cantidadTodasLasCompras = _comprasTodosLosMeses.Length;

    }//FIN DE PAGELOAD
}