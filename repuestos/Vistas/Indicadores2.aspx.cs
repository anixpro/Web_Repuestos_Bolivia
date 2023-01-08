using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using log4net;
using log4net.Config;
using System.Web.UI.DataVisualization.Charting;

public partial class Vistas_Indicadores2 : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Indicadores2));
    private List<IndicadorMetas> indicadoresMetas = new List<IndicadorMetas>();
    String nombreDealer = "";
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (panelEstadisticaDeCompra.Visible)
        {
            if (ddlEstadisticaCompraConcesionario.SelectedValue != "")
            {
                ControlConcesionarios _controlConcesionarios = new ControlConcesionarios();
                Concesionario concesionarioActual = _controlConcesionarios.ObtienePorID(ddlEstadisticaCompraConcesionario.SelectedValue);
                if (concesionarioActual == null)
                {
                    msjesError.InnerText = "Falta información de concesionario. Contacte a soporte";
                    msjesError.Visible = true;
                }
                else
                {
                    lblNombreConcesionario.Text = concesionarioActual.Nombre;
                }
            }
        }
        msjesError.InnerText = "";
        msjesError.Visible = false;

        if (int.Parse(Session["permisos"].ToString()) != 1 &&
            int.Parse(Session["permisos"].ToString()) != 2 &&
            int.Parse(Session["permisos"].ToString()) != 5
            )
        {
            Response.Redirect("../Index.aspx?evento=ev2",false);
            return;
        }
        /*
         * Si usuario es gerente, hay que permitir:
         * 
         * Links:
         *  - Nivel de compra, bloqueando concesionario
         *  - Ev de compra, bloqueando concesionario
         *  - Metas, bloqueando concesionario
         * No permitir:
         * - Ventas fallidas
         * - Coti v/s compras
         * - Estadística de compra 
        */
        // Si usuario es gerente...
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            nombreDealer = Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());
            if (nombreDealer == "")
            {
                msjesError.InnerText = "Su usuario no tiene asociado un concesionario, contacte al administrador";
                msjesError.Visible = true;
                return;
            }

            // Se desactivan paneles que no deben ver usuarios gerentes
            lnkBtnCotizacionCompras.Visible = false;
            lnkBtnVentasFallidas.Visible = false;

            // Se bloquea concesionario de metas
            ddlMetasConcesionarios.DataBound += new EventHandler(ddlMetasConcesionarios_DataBound);
            ddlNivelCompraConcesionarios.DataBound+=new EventHandler(ddlNivelCompraConcesionarios_DataBound);
            ddlEvolucionConcesionario.DataBound+=new EventHandler(ddlEvolucionConcesionario_DataBound);

            // Se bloquean la listas de concesionarios
            ddlNivelCompraConcesionarios.Enabled = false;
            ddlMetasConcesionarios.Enabled = false;
            ddlEvolucionConcesionario.Enabled = false;
            liCotiCompra.Visible = false;
            liVtasFallidas.Visible = false;
            liEstadistica.Visible = false;
        }

        if (!IsPostBack)
        {
            esconderPaneles();
            panelBienvenida.Visible = true;
        }
    }

    void ddlNivelCompraConcesionarios_DataBound(object sender, EventArgs e)
    {
        // Si usuario es gerente...
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            // Se bloquea eleccion de concesionario en panel Nivel de compra
            ddlNivelCompraConcesionarios.SelectedIndex =
                ddlNivelCompraConcesionarios.Items.IndexOf(
                    ddlNivelCompraConcesionarios.Items.FindByText(
                        nombreDealer
                    )
                );
        }
    }

    void ddlMetasConcesionarios_DataBound(object sender, EventArgs e)
    {
        // Si usuario es gerente...
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            ddlMetasConcesionarios.SelectedIndex =
                ddlMetasConcesionarios.Items.IndexOf(
                    ddlMetasConcesionarios.Items.FindByText(
                        nombreDealer
                    )
                );
        }
    }

    #region Links menu izquierda
    protected void lnkBtnNivelDe_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelNivelDeCompra.Visible = true;
    }
    protected void lnkBtnEstadisticaDeCompra_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelEstadisticaDeCompra.Visible = true;
    }
    protected void lnkBtnEvolucionCompra_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelEvolucionCompra.Visible = true;
    }
    protected void lnkBtnVentasFallidas_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelVentasFallidas.Visible = true;
    }
    protected void lnkBtnMetas_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelMetas.Visible = true;
    }
    protected void lnkBtnCotizacionCompras_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelCotizacionesCompras.Visible = true;
    }
    #endregion

    private void esconderPaneles()
    {
        // se ocultan paneles
        panelNivelDeCompra.Visible = false;
        panelEstadisticaDeCompra.Visible = false;
        panelBienvenida.Visible = false;
        panelCotizacionesCompras.Visible = false;
        panelEvolucionCompra.Visible = false;
        panelMetas.Visible = false;
        panelVentasFallidas.Visible = false;

        // se ocultan graficos
        ChartMetas.Visible = false;
        ChartNivelDeCompra.Visible = false;
    }

    #region METAS
    protected void btnGenerarMetas_Click(object sender, EventArgs e)
    {
        ChartMetas.Visible = true;
        panelMetas.Visible = true;
        SqlDSnivelDeCompra.SelectCommand = @"
            select 
	            indicador.idIndicador,
	            concesionarioMarca.nombreConcesionario,
	            concesionarioMarca.nombreMarca,
	            indicador.ano,
	            indicador.mes,
	            indicador.compras,
	            indicador.metas,
	            mes1 =
	                CASE indicador.mes
		                WHEN 1 then 'ENERO'
		                WHEN 2 then 'FEBRERO'
		                WHEN 3 then 'MARZO'
		                WHEN 4 then 'ABRIL'
		                WHEN 5 then 'MAYO'
		                WHEN 6 then 'JUNIO'
		                WHEN 7 then 'JULIO'
		                WHEN 8 then 'AGOSTO'
		                WHEN 9 then 'SEPTIEMBE'
		                WHEN 10 then 'OCTUBRE'
		                WHEN 11 then 'NOVIEMBRE'
		                WHEN 12 then 'DICIEMBRE'
	                END
                from indicador
	                inner join concesionarioMarca on
		                indicador.idConcesionarioMarca = concesionarioMarca.idConcesionarioMarca
                where 
                    concesionarioMarca.nombreConcesionario = '" + ddlMetasConcesionarios.SelectedItem.Text + @"' and
                    concesionarioMarca.nombreMarca = '" + ddlMetasMarca.SelectedItem.Text + @"' and
                    not(indicador.ano<=" + ddlMetasAnioInicio.SelectedValue + @" and indicador.mes<" + ddlMetasMesInicio.SelectedValue + @") and
                    not(indicador.ano>=" + ddlMetasAnioFin.SelectedValue + @" and indicador.mes>" + ddlMetasMesFin.SelectedValue + ")";
        DataSourceSelectArguments arg = new DataSourceSelectArguments();
        System.Data.DataView dv = (System.Data.DataView)SqlDSnivelDeCompra.Select(arg);
        if (dv.Count == 0)
        {
            msjesError.InnerText = "No hay información para el filtro indicado";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        else
        {
            btnMetasDescarcagaExcel.Visible = true;
        }
        SqlDSnivelDeCompra.DataBind();
        ChartMetas.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        ChartMetas.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        ChartMetas.DataSourceID = "SqlDSnivelDeCompra";
        ChartMetas.DataBind();
    }

    protected void btnMetasDescarcagaExcel_Click(object sender, EventArgs e)
    {
        panelNivelDeCompra.Visible = true;
        SqlDSnivelDeCompra.SelectCommand = @"
            select 
	            indicador.idIndicador,
	            concesionarioMarca.nombreConcesionario,
	            concesionarioMarca.nombreMarca,
	            indicador.ano,
	            indicador.mes,
	            indicador.compras,
	            indicador.metas,
	            mes1 =
	                CASE indicador.mes
		                WHEN 1 then 'ENERO'
		                WHEN 2 then 'FEBRERO'
		                WHEN 3 then 'MARZO'
		                WHEN 4 then 'ABRIL'
		                WHEN 5 then 'MAYO'
		                WHEN 6 then 'JUNIO'
		                WHEN 7 then 'JULIO'
		                WHEN 8 then 'AGOSTO'
		                WHEN 9 then 'SEPTIEMBE'
		                WHEN 10 then 'OCTUBRE'
		                WHEN 11 then 'NOVIEMBRE'
		                WHEN 12 then 'DICIEMBRE'
	                END
                from indicador
	                inner join concesionarioMarca on
		                indicador.idConcesionarioMarca = concesionarioMarca.idConcesionarioMarca
                where 
                    concesionarioMarca.nombreConcesionario = '" + ddlMetasConcesionarios.SelectedItem.Text + @"' and
                    concesionarioMarca.nombreMarca = '" + ddlMetasMarca.SelectedItem.Text + @"' and
                    not(indicador.ano<=" + ddlMetasAnioInicio.SelectedValue + @" and indicador.mes<" + ddlMetasMesInicio.SelectedValue + @") and
                    not(indicador.ano>=" + ddlMetasAnioFin.SelectedValue + @" and indicador.mes>" + ddlMetasMesFin.SelectedValue + ")";
        DataSourceSelectArguments arg = new DataSourceSelectArguments();
        System.Data.DataView dv = (System.Data.DataView)SqlDSnivelDeCompra.Select(arg);
        if (dv.Count == 0)
        {
            msjesError.InnerText = "No hay información para el filtro indicado";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        else
        {
            indicadoresMetas.Clear();
            foreach (System.Data.DataRowView drv in dv)
            {
                IndicadorMetas indicador = new IndicadorMetas();
                indicador.Anio = int.Parse(drv["ano"].ToString());
                indicador.Compras = int.Parse(drv["compras"].ToString());
                indicador.Concesionario = new Concesionario(drv["nombreConcesionario"].ToString());
                indicador.IdIndicador = int.Parse(drv["idIndicador"].ToString());
                indicador.Marca = new Marca(drv["nombreMarca"].ToString());
                indicador.Mes = int.Parse(drv["mes"].ToString());
                indicador.MesExtenso = drv["mes1"].ToString();
                indicador.Metas = int.Parse(drv["metas"].ToString());
                indicadoresMetas.Add(indicador);
            }
        }
        SqlDSnivelDeCompra.DataBind();

        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(indicadoresMetas, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Metas.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            
            // Para que el scroll vuelva arriba
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }
    #endregion

    #region ESTADISTICA DE COMPRAS
    protected void btnEstadisticaGeneraData_Click(object sender, EventArgs e)
    {
        esconderPaneles();
        panelEstadisticaDeCompra.Visible = true;

        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        String anioInicio = "1900";
        String mesInicio = "1";
        String anioFin = "2999";
        String mesFin = "12";
        String rutSolicitante = "";
        Boolean sucursal = false;
        String numeroClienteSAP = "";

        // Caso en que no haya concesionario seleccionado
        if (ddlEstadisticaCompraConcesionario.SelectedValue != "")
        {
            ControlConcesionarios _controlConcesionarios = new ControlConcesionarios();
            Concesionario concesionarioActual = _controlConcesionarios.ObtienePorID(ddlEstadisticaCompraConcesionario.SelectedValue);
            if (concesionarioActual == null)
            {
                msjesError.InnerText ="Falta información de concesionario. Contacte a soporte";
                msjesError.Visible = true;
                return;
            }
            else
            {
                numeroClienteSAP = concesionarioActual.CodigoClienteSAP;
            }
        }

        // En caso que esté activo filtro por fecha
        if (chkEstadisticaFechas.Checked)
        {
            anioInicio = ddlEstadisticaCompraAnioInicio.SelectedValue;
            mesInicio = ddlEstadisticaCompraMesInicio.SelectedValue;
            anioFin = ddlEstadisticaCompraAnioFin.SelectedValue;
            mesFin = ddlEstadisticaCompraMesFin.SelectedValue;
        }

        // En caso que este seleccionada busqueda por concesionario/sucursal
        if (rbEstadisticaCompraBuscaPorConcesSucursal.Checked)
        {
            if (numeroClienteSAP == "" ||
                ddlEstadisticaCompraSucursal.SelectedValue=="" ||
                ddlEstadisticaCompraSucursal.SelectedValue == "-1")
            {
                msjesError.InnerText = "Seleccione concesionario/sucursal";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            rutSolicitante = ddlEstadisticaCompraOperario.SelectedValue;
            sucursal = ddlEstadisticaCompraSucursal.SelectedValue != "";
        }

        // En caso que esté seleccionado busqueda por rut
        else if (rbEstadisticaCompraBuscaPorRut.Checked)
        {
            rutSolicitante = txtEstadisticaRut.Text;
            if (rutSolicitante == "")
            {
                msjesError.InnerText = "Ingrese un RUT";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            int resultado;
            if (!int.TryParse(rutSolicitante, out resultado))
            {
                msjesError.InnerText = "Ingrese un RUT válido";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }

        // Caso que no haya seleccionadio ninguno de los radio buttons (imposible)
        else
        {
            // No se escogio un usuario
        }

        // Aca vienen toda la combinacion de busquedas

        // Busca por MARCA, CODIGO Y SUCURSAL + con o sin rut
        if (chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && !sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String codigo = txtEstadisticaCodigo.Text;

            // Sin rut
            if (rutSolicitante == "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaYCodigo(anioInicio, mesInicio, anioFin, mesFin, marca, codigo);
            }

            // Con rut
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompra(anioInicio, mesInicio, anioFin, mesFin, rutSolicitante, marca, codigo);
            }

            GridView1.DataBind();
        }

        // Busca por MARCA + con o sin rut
        else if (chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && !sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaPorMarca(anioInicio, mesInicio, anioFin, mesFin, marca);
            }

            // con rut
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaPorMarca(anioInicio, mesInicio, anioFin, mesFin, marca, rutSolicitante);
            }
            GridView1.DataBind();
        }

        // Busca por Codigo + con o sin rut
        else if (!chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && !sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaPorCodigo(anioInicio, mesInicio, anioFin, mesFin, codigo);
            }

            // con rut
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRutYCodigo(anioInicio, mesInicio, anioFin, mesFin, rutSolicitante, codigo);
            }
            GridView1.DataBind();
        }

        // Busca por rut y sin rut (solo fecha)
        else if (!chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && !sucursal)
        {
            if (rutSolicitante != "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaPorOperario(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante);
            }
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaTodos(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin);
            }
            GridView1.DataBind();
        }

        // Busca por MARCA, CODIGO Y SUCURSAL + con o sin rut y sucursal
        else if (chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    marca,
                    codigo,
                    shipCode
                );
            }

            // con rut
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTMarcaCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    marca,
                    codigo,
                    shipCode
                );
            }
            GridView1.DataBind();
        }

        // Busca por MARCA + con o sin rut y sucursal
        else if (chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    marca,
                    shipCode
                );
            }

            // con rut
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTMarcaYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    marca,
                    shipCode
                );
            }
            GridView1.DataBind();
        }

        // Busca por Codigo + con o sin rut y sucursal
        else if (!chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    codigo,
                    shipCode
                );
            }

            // con rut
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    codigo,
                    shipCode
                );
            }
            GridView1.DataBind();
        }

        // Busca por rut y sin rut (solo fecha) y sucursal
        else if (!chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            if (rutSolicitante != "")
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    shipCode
                );
            }
            else
            {
                GridView1.DataSource = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    shipCode
                );
            }
            GridView1.DataBind();
        }
        
        else
        {
            msjesError.InnerText = "Combinación no soportada en los filtros";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }

    /// <summary>
    /// Permite mostrar cantidad de resultados de según lo configurado
    /// </summary>
    private void contadorRegistrosEstadisticaCompra()
    {
        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        String anioInicio = "1900";
        String mesInicio = "1";
        String anioFin = "2999";
        String mesFin = "12";
        String rutSolicitante = "";
        Boolean sucursal = false;
        int resultadoFiltro = -1;

        String numeroClienteSAP = "";
        if (ddlEstadisticaCompraConcesionario.SelectedValue != "")
        {
            ControlConcesionarios _controlConcesionarios = new ControlConcesionarios();
            Concesionario concesionarioActual = _controlConcesionarios.ObtienePorID(ddlEstadisticaCompraConcesionario.SelectedValue);
            if (concesionarioActual == null)
            {
                msjesError.InnerText = "Falta información de concesionario. Contacte a soporte";
                msjesError.Visible = true;
                return;
            }
            else
            {
                numeroClienteSAP = concesionarioActual.CodigoClienteSAP;
            }
        }

        if (chkEstadisticaFechas.Checked)
        {
            anioInicio = ddlEstadisticaCompraAnioInicio.SelectedValue;
            mesInicio = ddlEstadisticaCompraMesInicio.SelectedValue;
            anioFin = ddlEstadisticaCompraAnioFin.SelectedValue;
            mesFin = ddlEstadisticaCompraMesFin.SelectedValue;
        }

        if (rbEstadisticaCompraBuscaPorConcesSucursal.Checked)
        {
            if (numeroClienteSAP == "" ||
                ddlEstadisticaCompraSucursal.SelectedValue=="" ||
                ddlEstadisticaCompraSucursal.SelectedValue == "-1"
                )
            {
                msjesError.InnerText="Escoja concesionario/sucursal";
                msjesError.Visible = true;
                return;
            }

            rutSolicitante = ddlEstadisticaCompraOperario.SelectedValue;
            sucursal = ddlEstadisticaCompraSucursal.SelectedValue != "";
        }
        else if (rbEstadisticaCompraBuscaPorRut.Checked)
        {
            rutSolicitante = txtEstadisticaRut.Text;
            if (rutSolicitante == "")
            {
                return;
            }
            int resultado;
            if (!int.TryParse(rutSolicitante, out resultado))
            {
                return;
            }
        }
        else
        {
            // No se escogio un usuario
        }

        // Aca vienen toda la combinacion de busquedas

        // Busca por MARCA Y CODIGO (SIN SUCURSAL) + con o sin rut
        if (chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && !sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String codigo = txtEstadisticaCodigo.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaYCodigoEscalar(anioInicio, mesInicio, anioFin, mesFin, marca, codigo);
            }

            // con rut
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraEscalar(anioInicio, mesInicio, anioFin, mesFin, rutSolicitante, marca, codigo);
            }
        }

        // Busca por MARCA + con o sin rut
        else if (chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && !sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaPorMarcaEscalar(anioInicio, mesInicio, anioFin, mesFin, marca);
            }

            // con rut
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaPorMarcaEscalar(anioInicio, mesInicio, anioFin, mesFin, marca, rutSolicitante);
            }
        }

        // Busca por Codigo + con o sin rut
        else if (!chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && !sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaPorCodigoEscalar(anioInicio, mesInicio, anioFin, mesFin, codigo);
            }

            // con rut
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRutYCodigoEscalar(anioInicio, mesInicio, anioFin, mesFin, rutSolicitante, codigo);
            }
        }

        // Busca por rut y sin rut (solo fecha)
        else if (!chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && !sucursal)
        {
            if (rutSolicitante != "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaPorOperarioEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante);
            }
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaTodosEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin);
            }
        }

        // Busca por MARCA, CODIGO Y SUCURSAL + con o sin rut y sucursal
        else if (chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaCodigoYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    marca,
                    codigo,
                    shipCode
                );
            }

            // con rut
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTMarcaCodigoYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    marca,
                    codigo,
                    shipCode
                );
            }
        }

        // Busca por MARCA Y SUCURSAL + con o sin rut
        else if (chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    marca,
                    shipCode
                );
            }

            // con rut
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTMarcaYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    marca,
                    shipCode
                );
            }
        }

        // Busca por CODIGO Y SUCURSAL + con o sin rut
        else if (!chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorCodigoYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    codigo,
                    shipCode
                );
            }

            // con rut
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTCodigoYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    codigo,
                    shipCode
                );
            }
        }

        // Busca por SUCURSAL rut y sin rut (solo fecha)
        else if (!chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            if (rutSolicitante != "")
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTYSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    shipCode
                );
            }
            else
            {
                resultadoFiltro = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorSucursalEscalar(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    shipCode
                );
            }
        }
        else
        {
            //
        }

        if (resultadoFiltro > -1)
        {
            if (resultadoFiltro == 1)
            {
                msjesError.InnerText = "Su filtro actual arroja " + resultadoFiltro.ToString() + " resultado";
            }
            else
            {
                msjesError.InnerText = "Su filtro actual arroja " + resultadoFiltro.ToString() + " resultados";
            }
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }

    protected void rbEstadisticaCompraBuscaPorConcesSucursal_CheckedChanged(object sender, EventArgs e)
    {
        ddlEstadisticaCompraConcesionario.Enabled = true;
        ddlEstadisticaCompraSucursal.Enabled = true;
        ddlEstadisticaCompraOperario.Enabled = true;
        txtEstadisticaRut.Enabled = false;
    }
    protected void rbEstadisticaCompraBuscaPorRut_CheckedChanged(object sender, EventArgs e)
    {
        ddlEstadisticaCompraConcesionario.Enabled = false;
        ddlEstadisticaCompraSucursal.Enabled = false;
        ddlEstadisticaCompraOperario.Enabled = false;
        txtEstadisticaRut.Enabled = true;
    }
    protected void chkEstadisticaMarca_CheckedChanged(object sender, EventArgs e)
    {
        ddlEstadisticaMarca.Enabled = chkEstadisticaMarca.Checked;
    }
    protected void chkEstadisticaCodigo_CheckedChanged(object sender, EventArgs e)
    {
        txtEstadisticaCodigo.Enabled = chkEstadisticaCodigo.Checked;
    }
    protected void chkEstadisticaFechas_CheckedChanged(object sender, EventArgs e)
    {
        ddlEstadisticaCompraAnioFin.Enabled = chkEstadisticaFechas.Checked;
        ddlEstadisticaCompraAnioInicio.Enabled = chkEstadisticaFechas.Checked;
        ddlEstadisticaCompraMesFin.Enabled = chkEstadisticaFechas.Checked;
        ddlEstadisticaCompraMesInicio.Enabled = chkEstadisticaFechas.Checked;
    }
    protected void btnEstadisticaCompraDescargarDatos_Click(object sender, EventArgs e)
    {
        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        List<IndicadorEstadisticaDeCompra> indicadorEstadisticasCompra = new List<IndicadorEstadisticaDeCompra>();
        String anioInicio = "1900";
        String mesInicio = "1";
        String anioFin = "2999";
        String mesFin = "12";
        String rutSolicitante = "";
        Boolean sucursal = false;

        String numeroClienteSAP = "";
        if (ddlEstadisticaCompraConcesionario.SelectedValue != "")
        {
            ControlConcesionarios _controlConcesionarios = new ControlConcesionarios();
            Concesionario concesionarioActual = _controlConcesionarios.ObtienePorID(ddlEstadisticaCompraConcesionario.SelectedValue);
            if (concesionarioActual == null)
            {
                msjesError.InnerText = "Falta información de concesionario. Contacte a soporte";
                msjesError.Visible = true;
                return;
            }
            else
            {
                numeroClienteSAP = concesionarioActual.CodigoClienteSAP;
            }
        }

        if (chkEstadisticaFechas.Checked)
        {
            anioInicio = ddlEstadisticaCompraAnioInicio.SelectedValue;
            mesInicio = ddlEstadisticaCompraMesInicio.SelectedValue;
            anioFin = ddlEstadisticaCompraAnioFin.SelectedValue;
            mesFin = ddlEstadisticaCompraMesFin.SelectedValue;
        }

        if (rbEstadisticaCompraBuscaPorConcesSucursal.Checked)
        {
            rutSolicitante = ddlEstadisticaCompraOperario.SelectedValue;

            if (numeroClienteSAP == "" ||
                ddlEstadisticaCompraSucursal.SelectedValue == "" ||
                ddlEstadisticaCompraSucursal.SelectedValue == "-1")
            {
                msjesError.InnerText = "Seleccione concesionario/sucursal";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            sucursal = ddlEstadisticaCompraSucursal.SelectedValue != "";
        }
        else if (rbEstadisticaCompraBuscaPorRut.Checked)
        {
            rutSolicitante = txtEstadisticaRut.Text;
            if (rutSolicitante == "")
            {
                msjesError.InnerText = "Ingrese un RUT";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            int resultado;
            if (!int.TryParse(rutSolicitante, out resultado))
            {
                msjesError.InnerText = "Ingrese un RUT válido";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }
        else
        {
            // No se escogio un usuario
        }

        // Aca vienen toda la combinacion de busquedas

        // Busca por MARCA, CODIGO Y SUCURSAL + con o sin rut
        if (chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && !sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String codigo = txtEstadisticaCodigo.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaYCodigo(anioInicio, mesInicio, anioFin, mesFin, marca, codigo);
            }

            // con rut
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompra(anioInicio, mesInicio, anioFin, mesFin, rutSolicitante, marca, codigo);
            }
        }

        // Busca por MARCA + con o sin rut
        else if (chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && !sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaPorMarca(anioInicio, mesInicio, anioFin, mesFin, marca);
            }

            // con rut
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaPorMarca(anioInicio, mesInicio, anioFin, mesFin, marca, rutSolicitante);
            }
        }

        // Busca por Codigo + con o sin rut
        else if (!chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && !sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;

            // sin rut
            if (rutSolicitante == "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaPorCodigo(anioInicio, mesInicio, anioFin, mesFin, codigo);
            }

            // con rut
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRutYCodigo(anioInicio, mesInicio, anioFin, mesFin, rutSolicitante, codigo);
            }
        }

        // Busca por rut y sin rut (solo fecha)
        else if (!chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && !sucursal)
        {
            if (rutSolicitante != "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaPorOperario(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante);
            }
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaTodos(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin);
            }
        }

        // Busca por MARCA, CODIGO Y SUCURSAL + con o sin rut y sucursal
        else if (chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    marca,
                    codigo,
                    shipCode
                );
            }

            // con rut
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTMarcaCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    marca,
                    codigo,
                    shipCode
                );
            }
        }

        // Busca por MARCA + con o sin rut y sucursal
        else if (chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && sucursal)
        {
            String marca = ddlEstadisticaMarca.SelectedItem.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorMarcaYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    marca,
                    shipCode
                );
            }

            // con rut
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTMarcaYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    marca,
                    shipCode
                );
            }
        }

        // Busca por Codigo + con o sin rut y sucursal
        else if (!chkEstadisticaMarca.Checked && chkEstadisticaCodigo.Checked && sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            // sin rut
            if (rutSolicitante == "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    codigo,
                    shipCode
                );
            }

            // con rut
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTCodigoYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    codigo,
                    shipCode
                );
            }
        }

        // Busca por rut y sin rut (solo fecha) y sucursal
        else if (!chkEstadisticaMarca.Checked && !chkEstadisticaCodigo.Checked && sucursal)
        {
            String codigo = txtEstadisticaCodigo.Text;
            String shipCode = ddlEstadisticaCompraSucursal.SelectedValue;

            if (rutSolicitante != "")
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorRUTYSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    rutSolicitante,
                    shipCode
                );
            }
            else
            {
                indicadorEstadisticasCompra = _controlIndicadores.obtieneIndicadorEstadisticaCompraPorSucursal(
                    anioInicio,
                    mesInicio,
                    anioFin,
                    mesFin,
                    shipCode
                );
            }
        }
        else
        {
            msjesError.InnerText = "Combinación no soportada en los filtros";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        if (indicadorEstadisticasCompra.Count == 0)
        {
            msjesError.InnerText = "No hubo resultados para generar documento Excel";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(indicadorEstadisticasCompra, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=EstadisticaCompra.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }
    #endregion

    #region NIVEL DE COMPRAS
    protected void btnNivelCompraGeneraGraf_Click(object sender, EventArgs e)
    {
        ChartNivelDeCompra.Visible = true;
        SqlDSnivelDeCompra.SelectCommand = @"
            select 
	            indicador.idIndicador,
	            concesionarioMarca.nombreConcesionario,
	            concesionarioMarca.nombreMarca,
	            indicador.ano,
	            indicador.mes,
	            indicador.compras,
	            indicador.metas,
	            mes1 =
	                CASE indicador.mes
		                WHEN 1 then 'ENERO'
		                WHEN 2 then 'FEBRERO'
		                WHEN 3 then 'MARZO'
		                WHEN 4 then 'ABRIL'
		                WHEN 5 then 'MAYO'
		                WHEN 6 then 'JUNIO'
		                WHEN 7 then 'JULIO'
		                WHEN 8 then 'AGOSTO'
		                WHEN 9 then 'SEPTIEMBE'
		                WHEN 10 then 'OCTUBRE'
		                WHEN 11 then 'NOVIEMBRE'
		                WHEN 12 then 'DICIEMBRE'
	                END
                from indicador
	                inner join concesionarioMarca on
		                indicador.idConcesionarioMarca = concesionarioMarca.idConcesionarioMarca
                where 
                    concesionarioMarca.nombreConcesionario = '" + ddlNivelCompraConcesionarios.SelectedItem.Text + @"' and
                    concesionarioMarca.nombreMarca = '" + ddlNivelCompraNombreMarca.SelectedItem.Text + @"' and
                    not(indicador.ano<=" + ddlNivelComprasAnioIncio.SelectedValue + @" and indicador.mes<" + ddlNivelComprasMesInicio.SelectedValue + @") and
                    not(indicador.ano>=" + ddlNivelComprasAnioTermino.SelectedValue + @" and indicador.mes>" + ddlNivelComprasMesTermino.SelectedValue + ")";
        DataSourceSelectArguments arg = new DataSourceSelectArguments();
        System.Data.DataView dv = (System.Data.DataView)SqlDSnivelDeCompra.Select(arg);
        if (dv.Count == 0)
        {
            msjesError.InnerText = "No hay información para el filtro indicado";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        else
        {

        }
        ChartNivelDeCompra.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        ChartNivelDeCompra.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        SqlDSnivelDeCompra.DataBind();
        ChartNivelDeCompra.DataSourceID = "SqlDSnivelDeCompra";
        ChartNivelDeCompra.DataBind();
    }
    protected void btnNivelCompraGeneraExcel_Click(object sender, EventArgs e)
    {
        panelNivelDeCompra.Visible = true;
        SqlDSnivelDeCompra.SelectCommand = @"
            select 
	            indicador.idIndicador,
	            concesionarioMarca.nombreConcesionario,
	            concesionarioMarca.nombreMarca,
	            indicador.ano,
	            indicador.mes,
	            indicador.compras,
	            indicador.metas,
	            mes1 =
	                CASE indicador.mes
		                WHEN 1 then 'ENERO'
		                WHEN 2 then 'FEBRERO'
		                WHEN 3 then 'MARZO'
		                WHEN 4 then 'ABRIL'
		                WHEN 5 then 'MAYO'
		                WHEN 6 then 'JUNIO'
		                WHEN 7 then 'JULIO'
		                WHEN 8 then 'AGOSTO'
		                WHEN 9 then 'SEPTIEMBE'
		                WHEN 10 then 'OCTUBRE'
		                WHEN 11 then 'NOVIEMBRE'
		                WHEN 12 then 'DICIEMBRE'
	                END
                from indicador
	                inner join concesionarioMarca on
		                indicador.idConcesionarioMarca = concesionarioMarca.idConcesionarioMarca
                where 
                    concesionarioMarca.nombreConcesionario = '" + ddlNivelCompraConcesionarios.SelectedItem.Text + @"' and
                    concesionarioMarca.nombreMarca = '" + ddlNivelCompraNombreMarca.SelectedItem.Text + @"' and
                    not(indicador.ano<=" + ddlNivelComprasAnioIncio.SelectedValue + @" and indicador.mes<" + ddlNivelComprasMesInicio.SelectedValue + @") and
                    not(indicador.ano>=" + ddlNivelComprasAnioTermino.SelectedValue + @" and indicador.mes>" + ddlNivelComprasMesTermino.SelectedValue + ")";
        DataSourceSelectArguments arg = new DataSourceSelectArguments();
        System.Data.DataView dv = (System.Data.DataView)SqlDSnivelDeCompra.Select(arg);
        if (dv.Count == 0)
        {
            msjesError.InnerText = "No hay información para el filtro indicado";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        else
        {
            indicadoresMetas.Clear();
            foreach (System.Data.DataRowView drv in dv)
            {
                IndicadorMetas indicador = new IndicadorMetas();
                indicador.Anio = int.Parse(drv["ano"].ToString());
                indicador.Compras = int.Parse(drv["compras"].ToString());
                indicador.Concesionario = new Concesionario(drv["nombreConcesionario"].ToString());
                indicador.IdIndicador = int.Parse(drv["idIndicador"].ToString());
                indicador.Marca = new Marca(drv["nombreMarca"].ToString());
                indicador.Mes = int.Parse(drv["mes"].ToString());
                indicador.MesExtenso = drv["mes1"].ToString();
                indicador.Metas = int.Parse(drv["metas"].ToString());
                indicadoresMetas.Add(indicador);
            }
        }
        SqlDSnivelDeCompra.DataBind();

        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(indicadoresMetas, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=NivelesCompra.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }
    #endregion
    protected void ddlEstadisticaMarca_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    protected void ddlEstadisticaCompraOperario_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    protected void ddlEstadisticaCompraSucursal_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }

    #region EVOLUCION DE COMPRA
    private void enlazarCheckBoxMarcasEvolucion()
    {
        ControlAuto controlAuto = new ControlAuto();
        String[] misMarcas = controlAuto.marcasPorConcesionario(ddlEvolucionConcesionario.SelectedItem.Text);
        chkbxlstMarcas.Items.Clear();
        int indiceMarca = 0;
        foreach (String marca in misMarcas)
        {
            chkbxlstMarcas.Items.Add(marca);
            chkbxlstMarcas.Items[indiceMarca].Selected = false;
            indiceMarca++;
        }
    }
    protected void ddlEvolucionConcesionario_DataBound(object sender, EventArgs e)
    {
        // Si usuario es gerente...
        if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            // Se bloquea eleccion de concesionario en panel Nivel de compra
            ddlEvolucionConcesionario.SelectedIndex =
                ddlEvolucionConcesionario.Items.IndexOf(
                    ddlEvolucionConcesionario.Items.FindByText(
                        nombreDealer
                    )
                );
        }
        enlazarCheckBoxMarcasEvolucion();
    }
    protected void ddlEvolucionConcesionario_SelectedIndexChanged(object sender, EventArgs e)
    {
        enlazarCheckBoxMarcasEvolucion();
    }
    protected void btnEvolucionGeneraGrafica_Click(object sender, EventArgs e)
    {
        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        String anioInicio = ddlEvolucionAnioIncio.SelectedItem.Text;
        String mesInicio = ddlEvolucionMesInicio.SelectedValue;
        String anioFin = ddlEvolucionAnioTermino.SelectedItem.Text;
        String mesFin = ddlEvolucionMesTermino.SelectedValue;
        String concesionario = ddlEvolucionConcesionario.SelectedItem.Text;
        Boolean tieneData = false;

        foreach (ListItem li in chkbxlstMarcas.Items)
        {
            if (li.Selected)
            {
                tieneData = true;
                String marca = li.Text;
                List<IndicadorEvolucionCompra> indicadores = _controlIndicadores.obtenerEvolucionCompra(anioInicio, mesInicio, anioFin, mesFin, marca, concesionario);
                int[] compras = new int[indicadores.Count];
                int x = 0;
                foreach (IndicadorEvolucionCompra indicador in indicadores)
                {
                    compras[x] = indicador.Compras;
                    //ChartEvolucion.ChartAreas[0].AxisX.CustomLabels.Add(new CustomLabel());
                    //ChartEvolucion.ChartAreas[0].AxisX.CustomLabels[x].Text = indicador.Mes;
                    x++;
                }

                Series mainSeries = new Series(marca);
                mainSeries.ChartType = SeriesChartType.Line;
                mainSeries.Points.DataBindY(compras);

                ChartEvolucion.Series.Add(mainSeries);
            }
        }

        if (!tieneData)
        {
            msjesError.InnerText = "No hay datos para graficar";
            msjesError.Visible = true;
            ChartEvolucion.Visible = false;
            return;
        }

        ChartEvolucion.ChartAreas[0].AxisX.Title = "Meses";
        ChartEvolucion.ChartAreas[0].AxisY.Title = "Compras";
        ChartMetas.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        ChartMetas.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
    }
    #endregion
    protected void btnEvolucionGeneraExcel_Click(object sender, EventArgs e)
    {
        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        String anioInicio = ddlEvolucionAnioIncio.SelectedItem.Text;
        String mesInicio = ddlEvolucionMesInicio.SelectedValue;
        String anioFin = ddlEvolucionAnioTermino.SelectedItem.Text;
        String mesFin = ddlEvolucionMesTermino.SelectedValue;
        String concesionario = ddlEvolucionConcesionario.SelectedItem.Text;
        Boolean tieneData = false;
        List<IndicadorEvolucionCompra> indicadores = new List<IndicadorEvolucionCompra>();
        foreach (ListItem li in chkbxlstMarcas.Items)
        {
            if (li.Selected)
            {
                tieneData = true;
                String marca = li.Text;
                indicadores.AddRange(_controlIndicadores.obtenerEvolucionCompra(anioInicio, mesInicio, anioFin, mesFin, marca, concesionario));
            }
        }

        if (!tieneData)
        {
            msjesError.InnerText = "No hay datos para generar Excel";
            msjesError.Visible = true;
            return;
        }

        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(indicadores, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=EvolucionCompra.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }
    protected void btnFallidasGenerarExcel_Click(object sender, EventArgs e)
    {
        String anioInicio = ddlFallidasAnioInicio.SelectedItem.Text;
        String anioFin = ddlFallidasAnioTermino.SelectedItem.Text;
        String mesInicio = ddlFallidasMesInicio.SelectedValue;
        String mesFin = ddlFallidasMesTermino.SelectedValue;
        String codigo = txtFallidasCodigo.Text;
        String marca = ddlFallidasMarcas.SelectedValue;
        
        List<indicadorFallidas> indicadores = new List<indicadorFallidas>();
        ControlIndicadores _controlIndicadores = new ControlIndicadores();

        if (codigo == "" && marca == "")
        {
            indicadores = _controlIndicadores.obtenerFallidas(anioInicio, anioFin, mesInicio, mesFin);
        }
        else
        {
            indicadores = _controlIndicadores.obtenerFallidas(anioInicio, anioFin, mesInicio, mesFin, codigo, marca);
        }

        if (indicadores.Count==0)
        {
            msjesError.InnerText = "No hay registros para el filtro indicado";
            msjesError.Visible = true;
            return;
        }

        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(indicadores, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Fallidas.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }
    protected void btnCotiCompraGenerarGrafica_Click(object sender, EventArgs e)
    {
        String anioInicio = ddlCotiCompraAnioInicio.SelectedItem.Text;
        String anioTermino = ddlCotiCompraAnioFin.SelectedItem.Text;
        String mesInicio = ddlCotiCompraMesInicio.SelectedValue;
        String mesTermino = ddlCotiCompraMesFin.SelectedValue;
        String concesionario = ddlCotiCompraConcesionario.SelectedValue;
        String sucursal = ddlCotiCompraSucursal.SelectedValue;
        
        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        int[] indicadores = _controlIndicadores.obtenerCotiCompra(
            anioInicio,
            anioTermino,
            mesInicio,
            mesTermino,
            concesionario,
            sucursal
        );

        if (indicadores[0] == 0 && indicadores[1] == 0)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "No hubo resultados para el filtro indicado";
            ChartCotiCompra.Visible = false;
            return;
        }

        string[] xValues = {"Pedidos", "Cotizaciones"};
        Series mainSerie1 = new Series("Cotizacion");
        mainSerie1.ChartType = SeriesChartType.Pie;
        mainSerie1.Points.DataBindXY(xValues,indicadores);
        ChartCotiCompra.Series.Add(mainSerie1);
        ChartCotiCompra.ChartAreas[0].AxisX.Title = "Meses";
        ChartCotiCompra.ChartAreas[0].AxisY.Title = "Compras";
        ChartCotiCompra.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        ChartCotiCompra.Visible = true;
    }
    protected void btnCotiCompraGeneraExcel_Click(object sender, EventArgs e)
    {
        String anioInicio = ddlCotiCompraAnioInicio.SelectedItem.Text;
        String anioTermino = ddlCotiCompraAnioFin.SelectedItem.Text;
        String mesInicio = ddlCotiCompraMesInicio.SelectedValue;
        String mesTermino = ddlCotiCompraMesFin.SelectedValue;
        String concesionario = ddlCotiCompraConcesionario.SelectedValue;
        String sucursal = ddlCotiCompraSucursal.SelectedValue;
        Boolean soloCotizaciones = chkBxCotiCompraSoloCotizaciones.Checked;

        ControlIndicadores _controlIndicadores = new ControlIndicadores();
        List<IndicadorCotizacionCompra> indicadores= _controlIndicadores.obtenerTodoCotiCompra(
            anioInicio,
            anioTermino,
            mesInicio,
            mesTermino,
            concesionario,
            sucursal,
            soloCotizaciones
        );

        if (indicadores.Count == 0)
        {
            msjesError.InnerText = "No hubo resultados para el filtro indicado";
            msjesError.Visible = true;
            return;
        }

        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(indicadores, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=CotizacionCompra.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            msjesError.Visible = true;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }
    protected void ddlEstadisticaCompraConcesionario_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlEstadisticaCompraOperario.Items.Clear();
        ddlEstadisticaCompraOperario.Items.Add(new ListItem("Seleccione Sucursal",""));
        /*
        lblNombreConcesionario.Text = ddlEstadisticaCompraSucursal.SelectedItem.Text;
        SqlDataSourceEstasticaSucursales.DataBind();
        ddlEstadisticaCompraSucursal.DataBind();*/
    }
    protected void ddlEstadisticaCompraSucursal_DataBound(object sender, EventArgs e)
    {
        if (ddlEstadisticaCompraSucursal.Items.Count == 0)
        {
            ddlEstadisticaCompraSucursal.Items.Add(new ListItem("No hay Sucursales",""));
        }

        else
        {
            ddlEstadisticaCompraSucursal.Items.Add(new ListItem("Seleccione Sucursal", ""));
            ddlEstadisticaCompraSucursal.SelectedIndex =
                ddlEstadisticaCompraSucursal.Items.IndexOf(
                    ddlEstadisticaCompraSucursal.Items.FindByText("Seleccione Sucursal")
                );
        }
    }
    protected void ddlEstadisticaCompraOperario_DataBound(object sender, EventArgs e)
    {
        if (ddlEstadisticaCompraOperario.Items.Count == 0)
        {
            ddlEstadisticaCompraOperario.Items.Add(new ListItem("No hay Operadores", ""));
        }
        else
        {
            ddlEstadisticaCompraOperario.Items.Add(new ListItem("Seleccione Operador", ""));
            ddlEstadisticaCompraOperario.SelectedIndex =
                ddlEstadisticaCompraOperario.Items.IndexOf(
                    ddlEstadisticaCompraOperario.Items.FindByText("Seleccione Operador")
                );
        }
    }

    //contadorRegistrosEstadisticaCompra();
    protected void btnEstadisticaContar_Click(object sender, EventArgs e)
    {
        contadorRegistrosEstadisticaCompra();
    }
}
