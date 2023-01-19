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



public partial class Vistas_buscarRepto : System.Web.UI.Page
{

    // Clase que interactua con la base de datos
    ControlBD _controlBD = new ControlBD();

    // Clase que interactua con elementos de SAP
    SapAPI _sapApi = new SapAPI();

    // Objeto que se recibe parametros que se envian a SAP
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();

    // Objeto que trae resultados de SAP
    RespConsultaRepuesto _resConsulRepuesto = new RespConsultaRepuesto();

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_buscarRepto));

    // Clase que permite buscar si el repuesto esta en Banco
    ControlRepuestos _ctrlRepto = new ControlRepuestos();

    // Inicio de variables
    PdfHelper _pdf;
    RealizarPedido _pedido = new RealizarPedido();
    string rut, _marca;
    int _cantidad = 1;
    string _numVin, direccion = "", direccionExpandida = "";

    string precioCF = "";
    string totalCF = "";
    string precioLF = "";
    string totalLF = "";


    protected void Page_Load(object sender, EventArgs e)
    {

        ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
        scriptManager.RegisterPostBackControl(this.GridViewCarro);
        scriptManager.RegisterPostBackControl(this.GridViewListaRep);

        VFCbusqueda.Visible = false;
        msjesError.Visible = false;
        msjesError.InnerText = "";

        //se asigna la ruta del server al coinstructor de PdfHelper
        _pdf = new PdfHelper(Server.MapPath("~\\doc"));

        /* Se asignan event handlers */
        btnBuscarCodigo.Click += BuscarRepuesto;
        GridViewListaRep.SelectedIndexChanged += AddToCart;
        GridViewReemplazos.SelectedIndexChanged += AddToCartR;
        GridViewCarro.SelectedIndexChanged += OnEliminarDelCArro;
        GridViewTodosPreferidos.SelectedIndexChanged += ClickVerPreferido;
        GridViewCarro.DataBound += new EventHandler(GridViewCarro_DataBound);
        /* Fin asignacion event handlers */

        //Ocultar algunas funciones exclusivas del operario
        if (int.Parse(Session["permisos"].ToString()) != 3)
        {
            PanelAnteriores.Visible = false;
            PanelVerPreferido.Visible = false;
        }

        if (!IsPostBack)
        {
            // Se llenan los combos
            LlenarComboMarcas();

            // Se recupera el carro si ya se realizo una busqueda
            RecuperarCarro();

            // Se limpia la tabla no_stock, para que no vuelvan a aparecer los VFC, reservas y descartados al confirmar carro
            // de un pedido anterior
            try
            {
                _controlBD.InsertarDatos("update vfc set id_pedido = '" + Session["rut"].ToString() + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
                _controlBD.InsertarDatos("update backorder set id_pedido = '" + Session["rut"].ToString() + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
                _controlBD.InsertarDatos("update descartados set id_pedido = '" + Session["rut"].ToString() + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                msjesError.Visible = true;
                logger.Error("Error Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }


        // Indice de tabulación
        ComboMarcas.TabIndex = 0;
        txtCodigo.TabIndex = 1;
        txtCantidad.TabIndex = 2;
        btnBuscarCodigo.TabIndex = 3;

        //El rut del usuario autenticado
        rut = Session["rut"].ToString();

        try
        {
            _marca = ComboMarcas.SelectedItem.Text;
            _cantidad = int.Parse(lblCantidad.Text.Trim());
        }
        catch (NullReferenceException NullEx)
        {
            logger.Error("NullReferenceException en Page Load al asignar marca y cantidad. Inner: " + NullEx.InnerException + ". Stack: " + NullEx.StackTrace);
        }
        catch (Exception ex)
        {
            logger.Error("Exception en Page Load al asignar marca y cantidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        /* Se llenan los sucursales para usuarios multi sucursal */
        //Se determina si el usuario es multisucursal
        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            // Para lista con stock
            if (ListaSucursales.Items.Count == 0)
            {
                // Se llena la lista de sucursales
                ListaSucursales.Items.Clear();
                ListaSucursales.Items.Add("");
                foreach (string lista in _controlBD.ObtenerSucursalesPorConcesionario(_sapApi.GetNombreDealer(Session["rut"].ToString())))
                {
                    ListaSucursales.Items.Add(lista);
                }
            }
            ListaSucursales.Visible = true;

            // Para lista sin stock
            if (ddlSucursalesNoStock.Items.Count == 0)
            {
                // Se llena la lista de sucursales
                ddlSucursalesNoStock.Items.Clear();
                ddlSucursalesNoStock.Items.Add("");
                foreach (string lista in _controlBD.ObtenerSucursalesPorConcesionario(_sapApi.GetNombreDealer(Session["rut"].ToString())))
                {
                    ddlSucursalesNoStock.Items.Add(lista);
                }
            }
        }
        /* FIN: Se llenan los sucursales para usuarios multi sucursal */
        else
        {

            string idsucursal = _sapApi.GetIdSucursalRut(Session["rut"].ToString());
            int PerUsu = int.Parse(Session["permisos"].ToString());

            string descrpcionDireccionSucursal = _sapApi.GetDireccionPorShipCod(_sapApi.GetCodigoShipCode(Session["rut"].ToString()),idsucursal, PerUsu);
            if (descrpcionDireccionSucursal == "")
            {
                msjesError.InnerText = "Usted no tiene un destinatario asociado, favor comunicar al administrador del sitio a la brevedad";
                msjesError.Visible = true;
                logger.Error("Usuario no tiene destinatario asociado! (En page load)");
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            lblDireccion.Text = descrpcionDireccionSucursal;
            lblDestinatarioNoStock.Text = descrpcionDireccionSucursal;
            lblDireccion.Visible = true;
        }
    }

    void GridViewCarro_DataBound(object sender, EventArgs e)
    {
        if (int.Parse(Session["permisos"].ToString()) == 3 && GridViewCarro.Rows.Count > 0)
        {
            ocultaColumnaStock();
        }
    }

    /* Se oculta columna de stock para usuarios operarios... ¡No deben saber el stock! */
    private void ocultaColumnaStock()
    {
        try
        {
            GridViewCarro.HeaderRow.Cells[10].Visible = false;
            GridViewCarro.FooterRow.Cells[10].Visible = false;
            foreach (GridViewRow gvr in GridViewCarro.Rows)
            {
                gvr.Cells[10].Visible = false;
            }
        }
        catch (Exception ex)
        {
            logger.Error("Exception en ocultaColumnaStock, al esconder el stock de los usuarios operarios. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Message: " + ex.Message);
        }
    }

    private void BuscarRepuesto(object o, EventArgs e)
    {
        GridViewListaRep.DataSource = null;
        GridViewListaRep.DataBind();
        GridViewTodosPreferidos.Visible = false;

        //Validar que se escoja una marca de la lista
        if (ComboMarcas.SelectedValue == "-1")
        {
            msjesError.InnerText = "Debes seleccionar una marca de la lista, por favor";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        // Se obtiene la marca
        string marca = ComboMarcas.SelectedItem.Text;

        //Se bloquea la lista de marcas para cotizar solo una marca
        ComboMarcas.Enabled = false;
        btnNewSearch.Visible = true;

        // Si se trata de marca foránea, no se permite cotizar por garantía
        // Para marcas foraneas, va una X en el campo i_foraneo
        ControlMarca _controlMarca = new ControlMarca();
        Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
        if (marcaVehiculo == null)
        {
            throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
        }
        if (_controlMarca.obtenerMarcaPorNombre(marca).esForaneo)
        {
            ddlTipoDePedido.Items.Remove(ddlTipoDePedido.Items.FindByValue("garantia"));
            ddlTipoPedidoReserva.Items.Remove(ddlTipoDePedido.Items.FindByValue("garantia"));
        }


        // Se limpia las busquedas temporales
        _controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        logger.Debug("Se limpian las tablas temporales LISTA_BUSQUEDA_TMP y LISTA_REEMPLAZO_TMP!");

        //Se habilitan los GridViews correspondientes
        GridViewReemplazos.Visible = true;
        GridViewListaRep.Visible = true;

        //Validar la búsqueda por nombre; si el campo nombre es vacio se buscará por codigo        
        if (txtNombre.Text.Length == 0)
        {
            // Si no hay nombre ni codigo, se trata de un error
            if (txtCodigo.Text.Equals(""))
            {
                msjesError.InnerText = "Debe ingresar un código de repuesto o un nombre, por favor";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }

        // Si el usuario no ingresa cantidad, se solicita que lo haga
        if (txtCantidad.Text.Equals(""))
        {
            msjesError.InnerText = "Debe ingresar una cantidad de repuesto a solicitar";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Se obtiene el grupo de material segun la marca seleccionada
        _consultaRep.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(_marca);
        if (_consultaRep.GrupoMaterial == "")
        {
            msjesError.InnerText = "Error al obtener grupo de material. Favor informar al administrador del sitio";
            msjesError.Visible = true;
            logger.Error("Error en buscar repuesto. Usuario " + Session["rut"].ToString() + " buscando la marca " + _marca + " no tiene grupo de materiales");
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Se concatena el código ingresado por el usuario con el prefijo de la marca
        if (txtCodigo.Text.Trim().Length > 0)
        {
            //Se busca el prefijo marca de mercancia para este usuario por grupo de material //Se Agrega Marca a la Consulta
            string prefijoMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consultaRep.GrupoMaterial, _marca);
            if (prefijoMarca == "")
            {
                msjesError.InnerText = "Error al obtener grupo el prefijo de la marca. Favor informar al administrador del sitio";
                msjesError.Visible = true;
                logger.Error("Error en buscar repuesto. No hay prefijo de marca. Usuario " + Session["rut"].ToString() + " utilizando el grupo de material " + _consultaRep.GrupoMaterial);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            //Se concatena el prefijo de marca con el I_MFRPN
            _consultaRep.CodRepuesto = prefijoMarca.Trim().ToUpper() + txtCodigo.Text.ToUpper();
        }

        //Se busca el codigo de cliente SAP para este usuario
        string shipCode = _sapApi.GetCodigoClienteSapByRut(Session["rut"].ToString());

        // Si el destinatario de mercancía es "X", quiere decir que se trata de admin de SKBERGE
        if (shipCode == "X")
        {
            shipCode = "IBP02";
        }

        _consultaRep.DestinaMercacia = shipCode;
        if (_consultaRep.DestinaMercacia == "")
        {
            msjesError.InnerText = "Error al obtener destinatario de marcancía. Favor informar al administrador del sitio";
            msjesError.Visible = true;
            logger.Error("Error en buscar repuesto. No hay destinatario de mercancía. Usuario " + Session["rut"].ToString());
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Se obtiene el DocVentas y el canal de distribución
        _consultaRep.DocVentas = _sapApi.GetVkorgByGrupoMaterial(_consultaRep.GrupoMaterial, marca);
        _consultaRep.CanalDistribucion = _sapApi.CanalDeDistribucionPedido;
        _consultaRep.TextoRep = txtNombre.Text.ToUpper();
        _consultaRep.CantidadRep = int.Parse(txtCantidad.Text);

        // Se envía los datos al método BuscarRepuestos del controlador
        // Se verifica si el método que busca repuestos encontro algo
        // sino encuentra nada despliega un msj de error
        if (!_pedido.BuscarRepuesto(_consultaRep, Session["idSession"].ToString(), int.Parse(txtCantidad.Text.Trim()), txtNombre.Text, marca))
        {
            VFCbusqueda.Visible = true;

            // Datos en caso que usuario solicite VFC.
            // Por seguridad en caso que usuario modifique datos de entrada
            ultimaCantidad.Value = _consultaRep.CantidadRep.ToString();
            ultimaMarca.Value = marca;
            ultimoCodigo.Value = txtCodigo.Text.ToUpper();

            // Se busca repuesto en banco de repuesto en caso que exista
            if (_ctrlRepto.obtenerRepuestoPorCodigo(ultimoCodigo.Value) != null)
            {
                msjesError.Visible = true;
                msjesError.InnerHtml += "<br />ATENCION: El repuesto que usted busca se encuentra en el BANCO DE REPUESTOS. Si desea consultarlo, hága clic en <a target='_blank' href='BancoRepuestos.aspx'>Banco de Repuestos</a>";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            }
            // En el caso en que el servicio no este disponible
            if (!_pedido.disponibilidadServicio)
            {
                VFCbusqueda.Visible = false;
                msjesError.InnerText = _pedido.mensajeError;
                msjesError.Visible = true;
                msjesError.Style["background-color"] = "red";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);


            }
            if (!_pedido.disponibilidadRepuestoSap) //repuesto no existe en sap
            {
                GridViewListaRep.DataSource = null;
                GridViewListaRep.DataBind();
                GridViewListaRep.Visible = false;
                GridViewTodosPreferidos.Visible = false;
                GridViewReemplazos.DataSource = null;
                GridViewListaRep.DataBind();
                GridViewReemplazos.Visible = false;

                msjesError.InnerText = _pedido.mensajeError;
                msjesError.Visible = true;

                //Repuesto no Existe en SAP Realiza VFC
                VFCbusqueda.Visible = true;
            }
        }
        else
        {
            if (!_pedido.disponibilidadRepuesto)
            {
                msjesError.InnerText = _pedido.mensajeError;
                msjesError.Visible = true;
            }
           if (_pedido.disponibilidadVFC)
           {
                 VFCbusqueda.Visible = true;
           }

           //Consulto si mi repuesto encontro algo
           string busqueda = _sapApi.EncontreMaterial(Session["idSession"].ToString(), _consultaRep.CodRepuesto);
           if (busqueda == "0" && (_consultaRep.CodRepuesto != null))
             {
               VFCbusqueda.Visible = true;
             }


            /*int precioLista = 0;
            int stock = 0;
            DataSet _dSet = new DataSet();
            SqlConnection _conection = new SqlConnection();
            SqlCommand _comando = new SqlCommand();
            SqlDataAdapter _adapter = new SqlDataAdapter();

            try
            {
                _conection.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
                _conection.Open();
            }
            catch (Exception ex)
            {
                logger.Error("Error en [UpdateFechasExpiracion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }

            try
            {
                if (_conection.State == ConnectionState.Open)
                {
                    _comando.Connection = _conection;
                    _comando.CommandType = CommandType.Text;
                    _comando.CommandText = "select stock from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";
                    _adapter.SelectCommand = _comando;
                    _adapter.Fill(_dSet);
                }

                foreach (DataRow campo in _dSet.Tables[0].Rows)
                {
                    ListItem ls = new ListItem();
                    precioLista = int.Parse(campo["PRECIO_LISTA"].ToString());
                    stock = int.Parse(campo["stock"].ToString());
                }
            }
            catch(Exception ex)
            {
                _dSet = null;
                logger.Error("Error en [obtener stock]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
            finally
            {
                if (_conection.State == ConnectionState.Open)
                    _conection.Close();
                _comando = null;
                _conection = null;
                _adapter = null;
            }*/

            // Si se encuentran resultados se muestran en una grilla
            // Obtener los datos de la tabla con los resultados de la busqueda
            SqlDataSource1.SelectCommand = "select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";

            try
            {
                /*if (stock > 0 && precioLista > 0)
                {
                    VFCbusqueda.Visible = false;
                }
                else {
                    VFCbusqueda.Visible = true;

                    ultimaCantidad.Value = _consultaRep.CantidadRep.ToString();
                    ultimaMarca.Value = marca;
                    ultimoCodigo.Value = txtCodigo.Text.ToUpper();
                }*/

                GridViewListaRep.PagerSettings.Mode = PagerButtons.Numeric;
                GridViewListaRep.DataBind();
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpar las molestias";
                msjesError.Visible = true;
                logger.Error("Error Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            //Si existen cadenas de reemplazo se muestran en la grilla
            SqlDataSource2.SelectCommand = "select * from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";
            try
            {
                GridViewReemplazos.PagerSettings.Mode = PagerButtons.Numeric;
                GridViewReemplazos.DataBind();
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpar las molestias";
                msjesError.Visible = true;
                logger.Error("Error Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            if (GridViewReemplazos.Rows.Count == 0)
            {
                GridViewReemplazos.Visible = false;
            }

            if (GridViewListaRep.Rows.Count > 1)
            {
                VFCbusqueda.Visible = false;
                //msjesError.Visible = false;
                lblCantidad.Text = txtCantidad.Text;
                txtCodigo.Text = "";
                txtCantidad.Text = "";
                txtNombre.Text = "";
                ultimaDescripcion.Value = "";
            }
            else
            {
                lblCantidad.Text = txtCantidad.Text;
                ultimaCantidad.Value = _consultaRep.CantidadRep.ToString();
                ultimaMarca.Value = marca;
                ultimoCodigo.Value = txtCodigo.Text.ToUpper();
            }
        }

    }

    // Agregar al carro
    public void AddToCart(object o, EventArgs e)
    {
        if (GridViewCarro.Rows.Count >= 18)
        {
            msjesError.InnerText = "Ha alcanzado el límite de repuestos a solicitar en el mismo pedido";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        Double diferencia = 0.0;
        int cantCarro = 0;
        if (ComboMarcas.SelectedValue == "-1" || _cantidad == 0)
        {
            msjesError.InnerText = "Debe ingresar una marca y una cantidad";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        string marca = "";

        // Se obtienen los datos necesarios para agregar al carro
        string idSession = Session["idSession"].ToString();

        marca = ComboMarcas.SelectedItem.Text;

        string codigo = GridViewListaRep.SelectedRow.Cells[1].Text;
        string descripcion = GridViewListaRep.SelectedRow.Cells[2].Text;
        int cantidad = _cantidad;
        //Se cambio INT por FLOAT WEB Repuestos PERU
        double precioC = 0;
        double precioL = 0;
        double totalC = 0;
        double totalL = 0;
        Double porcentaje = 0;
        string descuen;

        if (double.Parse(GridViewListaRep.SelectedRow.Cells[6].Text) == 0)
        {
            precioC = 0;
            precioL = 0;
        }
        else
        {
        
            precioC = double.Parse(GridViewListaRep.SelectedRow.Cells[5].Text);

            precioL = double.Parse(GridViewListaRep.SelectedRow.Cells[4].Text, System.Globalization.CultureInfo.InvariantCulture);

        }

        totalC = precioC * cantidad;
        totalL = precioL * cantidad;
        descuen = GridViewListaRep.SelectedRow.Cells[8].Text;
        //porcentaje = totalL - totalC;
        //porcentaje = porcentaje / precioL;
        //if (porcentaje.ToString().Length > 4)
        //{
        //    descuen = Convert.ToDouble(porcentaje.ToString().Remove(4, 13)) * 100;
        //}
        //else
        //{
        //    descuen = porcentaje * 100;
        //}

        string descuento = "'" + Convert.ToString(descuen) + "'";

        // Se ve si la cantidad solicitada es igual o superior a la cantidad minima 

        //VFCbusqueda.Visible = true;
        string cantidadmin = cantidad.ToString();

        if (_ctrlRepto.repuestoPorCantidad(cantidadmin, codigo) == false)
        {
            msjesError.InnerText = "La Cantidad solicitada esta por debajo del minimo permitido";
            msjesError.Visible = true;
            string cantmin = _ctrlRepto.getCantidadporCodigo(codigo);
            msjesError.InnerHtml += "<br />ATENCION: La Permitida a solicitar es " + cantmin + " o multiplos de esta";
            return;
        }

        if (_ctrlRepto.obtenerRepuestoPorCodigo(codigo) != null)
        {
            msjesError.Visible = true;
            msjesError.InnerHtml += "<br />ATENCION: El repuesto que usted busca se encuentra en el BANCO DE REPUESTOS. Si desea consultarlo, hága clic en <a target='_blank' href='BancoRepuestos.aspx'>Banco de Repuestos</a>";
        }

        // En el caso en que venga sin precio
        if (precioC == 0 || precioL == 0)
        {
            msjesError.InnerText = "No se puede agregar al carro";
            msjesError.Visible = true;
            VFCbusqueda.Visible = false;
            txtCodigo.Text = codigo;
            txtCantidad.Text = cantidad.ToString();

            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);

            //Si stock es menor a 1
            if ((int.Parse(GridViewListaRep.SelectedRow.Cells[6].Text)) > 0 && precioL > 0)
            {
                VFCbusqueda.Visible = false;
            }
            // no muestra mensaje vfc si stock>0 y precio=0
            else if ((int.Parse(GridViewListaRep.SelectedRow.Cells[6].Text)) > 0 && precioL == 0)
            {
                VFCbusqueda.Visible = false;
            }
            else
            {
                VFCbusqueda.Visible = true;
                ultimaCantidad.Value = cantidad.ToString();
                ultimaMarca.Value = marca;
                ultimoCodigo.Value = txtCodigo.Text.ToUpper();
                ultimaDescripcion.Value = descripcion;
            }

            return;
        }

        // Obtener stock con la regla del 80%
        string grupoMat = _sapApi.GetGrupoMaterialesByMarca(marca);
        string prefMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(grupoMat, marca); //Se Agrega Marca
        string codSap = prefMarca.Trim() + codigo.Trim();
        string stock = GridViewListaRep.SelectedRow.Cells[6].Text;
        string A = GridViewListaRep.SelectedRow.Cells[4].Text;
        string B = GridViewListaRep.SelectedRow.Cells[5].Text;
        Double c = 0.0;
        Double d = 0.0;

        // Para usuarios comunes se agrega todo al carro
        if (int.Parse(Session["permisos"].ToString()) != 3)
        {

            // Se ve si la cantidad solicitada es igual o superior a la cantidad minima 

            //VFCbusqueda.Visible = true;

            if (_ctrlRepto.repuestoPorCantidad(cantidadmin, codigo) == false)
            {
                msjesError.InnerText = "La Cantidad solicitada esta por debajo del minimo permitido";
                msjesError.Visible = true;
                string cantmin = _ctrlRepto.getCantidadporCodigo(codigo);
                msjesError.InnerHtml += "<br />ATENCION: La Permitida a solicitar es " + cantmin + " o multiplos de esta";
                return;
            }


            precioCF = System.Convert.ToString(precioC);
            totalCF = System.Convert.ToString(totalC);
            precioLF = System.Convert.ToString(precioL);
            totalLF = System.Convert.ToString(totalL);

            //Repuesto se agrega al carro
            _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                precioC,totalC,precioL,totalL,descuento)
                    values('" + idSession + "','" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                    "  " + cantidad + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                    "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ", " + descuento + ")");

            //Se llena GridViewCarro con los datos de la tabla carro
            try
            {
                GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + idSession + "'");
                GridViewCarro.DataBind();
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                msjesError.Visible = true;
                logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            PanelCarroDeCompras.Visible = true;
            PanelCotizarComun.Visible = true;
        }

        //En esta parte se pregunta si el stock es suficiente para agragar al carro
        //si el stock no es suficiente se agrega a la grilla "noStock" para ser procesado 
        //como VFC, Reserva o descarte.
        else
        {
            if (cantidad <= int.Parse(stock))
            {
                cantCarro = cantidad;
            }
            else
            {
                c = Convert.ToDouble(A);
                d = Convert.ToDouble(B);
                diferencia = cantidad - int.Parse(stock);
                cantCarro = int.Parse(stock);

                totalC = precioC * diferencia;
                totalL = precioL * diferencia;

            }

            if (diferencia > 0)
            {

                precioCF = System.Convert.ToString(precioC);
                totalCF = System.Convert.ToString(totalC);
                precioLF = System.Convert.ToString(precioL);
                totalLF = System.Convert.ToString(totalL);

                //Repuesto se va a GridViewNoStock
                //Repuesto se agrega a la tabla no stock
                _controlBD.InsertarDatos(@"insert into no_stock(idSession,vfc,reserva,descarte,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL)
                                   values('" + idSession + "', 'n' , 'n' , 'n' , '" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                                        "  " + diferencia + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                        "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ")");

                //Se llena GridViewCarro con los datos de la tabla carro
                try
                {
                    GridViewNoStock.DataSource = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + idSession + "'");
                    GridViewNoStock.DataBind();
                }
                catch (Exception ex)
                {
                    msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                    msjesError.Visible = true;
                    logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }

                string plista3 = "";
                for (int i = 0; i < GridViewNoStock.Rows.Count; i++)
                {
                    GridViewRow row3 = GridViewNoStock.Rows[i];
                    plista3 = row3.Cells[7].Text;
                    if (plista3 == "0")
                    {
                        row3.Cells[7].Text = "N/A";
                    }
                }

                if (GridViewCarro.Rows.Count > 0)
                {
                    PanelSoloHNoStock.Visible = false;
                }

                PanelNoStock.Visible = true;

                if (GridViewCarro.Rows.Count == 0)
                {
                    PanelSoloHNoStock.Visible = true;
                }
                else
                {
                    PanelSoloHNoStock.Visible = false;
                }
            }

            if (cantCarro > 0)
            {
                //NUEVO
                totalC = precioC * cantCarro;
                totalL = precioL * cantCarro;

                precioCF = System.Convert.ToString(precioC);
                totalCF = System.Convert.ToString(totalC);
                precioLF = System.Convert.ToString(precioL);
                totalLF = System.Convert.ToString(totalL);

                //Repuesto se agrega al carro ---PO BORRAR
                _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL,descuento)
                                   values('" + idSession + "','" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                                        "  " + cantCarro + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                        "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + "," + descuento + ")");
                //Se llena GridViewCarro con los datos de la tabla carro
                GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + idSession + "'");
                GridViewCarro.DataBind();

                string plista = "";
                for (int i = 0; i < GridViewCarro.Rows.Count; i++)
                {
                    GridViewRow row = GridViewCarro.Rows[i];
                    plista = row.Cells[4].Text;
                    if (plista == "$ 0,00")
                    {
                        row.Cells[4].Text = "--";
                    }
                }

                PanelCarroDeCompras.Visible = true;
                PanelCotizarSap.Visible = true;
                PanelSoloHNoStock.Visible = false;
                //Carga Motivo Pedido
                CargarMotivoPedido();

                if (GridViewCarro.Rows.Count > 0)
                {
                    PanelSoloHNoStock.Visible = false;
                }

            }


        }
        //Fin proceso de no stock

        _controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");

        // Si se encuentran resultados se muestran en una grilla
        // Obtener los datos de la tabla con los resultados de la busqueda
        SqlDataSource1.SelectCommand = "select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";

        try
        {
            GridViewListaRep.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            GridViewListaRep.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
            msjesError.Visible = true;
            logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        // Si existen cadenas de reemplazo se muestran en la grilla
        SqlDataSource2.SelectCommand = "select * from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";

        try
        {
            GridViewReemplazos.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            GridViewReemplazos.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
            msjesError.Visible = true;
            logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        // Se determina si el usuario es multisucursal
        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            ListaSucursales.Visible = true;
            ddlSucursalesNoStock.Visible = true;
        }
        else
        {
            lblDireccion.Visible = true;
            lblDestinatarioNoStock.Visible = true;
        }
    }

    //Agregar al carro un reemplazo, este metodo agrega al carro la cadena de reemplazo de un repuesto X
    //actua igual que el metodo AddToCart
    public void AddToCartR(object o, EventArgs e)
    {
        if (GridViewCarro.Rows.Count >= 18)
        {
            msjesError.InnerText = "Ha alcanzado el máximo de elementos en un pedido";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        int diferencia = 0;
        int cantCarro = 0;
        if (ComboMarcas.SelectedValue == "-1" || _cantidad == 0)
        {
            msjesError.InnerText = "Debe seleccionar una marca y cantidad";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        string marca = "";

        string idSession = Session["idSession"].ToString();
        marca = ComboMarcas.SelectedItem.Text;
        string codigo = GridViewReemplazos.SelectedRow.Cells[1].Text;
        string descripcion = GridViewReemplazos.SelectedRow.Cells[2].Text;
        int cantidad = _cantidad;
        double precioC = double.Parse(GridViewReemplazos.SelectedRow.Cells[4].Text);
        double precioL = 0;
        double totalC = precioC * cantidad;
        double totalL = precioL * cantidad;
        Double porcentaje = 0;
        Double descuen = 0;
        //porcentaje = totalL - totalC;
        //porcentaje = porcentaje / precioL;
        if (porcentaje.ToString().Length > 4)
        {
            descuen = Convert.ToDouble(porcentaje.ToString().Remove(4, 13)) * 100;
        }
        else
        {
            descuen = porcentaje * 100;
        }

        string descuento = Convert.ToString(0) + "%";

        /* Cantidad minima */

        string cantidadmin = cantidad.ToString();

        if (_ctrlRepto.repuestoPorCantidad(cantidadmin, codigo) == false)
        {
            msjesError.InnerText = "La Cantidad solicitada esta por debajo del minimo permitido";
            msjesError.Visible = true;
            string cantmin = _ctrlRepto.getCantidadporCodigo(codigo);
            msjesError.InnerHtml += "<br />ATENCION: La Permitida a solicitar es " + cantmin + " o multiplos de esta";
            return;
        }


        /* ----------------- */
        /* ----------------- */

        //Obtener stock con la regla del 80%
        string grupoMat = _sapApi.GetGrupoMaterialesByMarca(marca);
        string destMerc = _sapApi.GetDestinatarioMercanciaByUserid(_sapApi.GetRutConcesionario(Session["rut"].ToString()));
        string prefMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(grupoMat, marca); //se agrega marca
        string codSap = prefMarca.Trim() + codigo.Trim();
        string stock = GridViewReemplazos.SelectedRow.Cells[6].Text;

        //Para usuarios comunes se agrega todo al carro
        if (int.Parse(Session["permisos"].ToString()) != 3)
        {
            try
            {
                precioCF = System.Convert.ToString(precioC);
                totalCF = System.Convert.ToString(totalC);
                precioLF = System.Convert.ToString(precioL);
                totalLF = System.Convert.ToString(totalL);

                //Repuesto se agrega al carro
                _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL,descuento)
                                   values('" + idSession + "','" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                                        "  " + cantidad + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                        "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + " , " + descuento + ")");
                //Se llena GridViewCarro con los datos de la tabla carro
                GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + idSession + "'");
                GridViewCarro.DataBind();
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                msjesError.Visible = true;
                logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            PanelCarroDeCompras.Visible = true;
            PanelCotizarComun.Visible = true;
        }

        //En esta parte se pregunta si el stock es suficiente para agragar al carro
        //si el stock no es suficiente se agrega a la grilla "noStock" para ser procesado 
        //como VFC, Reserva o descarte.
        else
        {
            if (cantidad <= int.Parse(stock))
            {
                cantCarro = cantidad;
            }
            else
            {
                diferencia = int.Parse(GridViewReemplazos.SelectedRow.Cells[7].Text) - int.Parse(GridViewReemplazos.SelectedRow.Cells[6].Text);
                cantCarro = cantidad - diferencia;
            }

            if (diferencia > 0)
            {

                precioCF = System.Convert.ToString(precioC);
                totalCF = System.Convert.ToString(totalC);
                precioLF = System.Convert.ToString(precioL);
                totalLF = System.Convert.ToString(totalL);

                //Repuesto se va a GridViewNoStock
                //Repuesto se agrega a la tabla no stock
                _controlBD.InsertarDatos(@"insert into no_stock(idSession,vfc,reserva,descarte,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL)
                                   values('" + idSession + "', 'n' , 'n' , 'n' , '" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                                        "  " + diferencia + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                        "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ")");

                //Se llena GridViewCarro con los datos de la tabla carro
                GridViewNoStock.DataSource = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + idSession + "'");
                GridViewNoStock.DataBind();
                string plista2 = "";
                for (int i = 0; i < GridViewNoStock.Rows.Count; i++)
                {
                    GridViewRow row2 = GridViewNoStock.Rows[i];
                    plista2 = row2.Cells[7].Text;
                    if (plista2 == "0")
                    {
                        row2.Cells[7].Text = "N/A";
                    }
                }

                if (GridViewCarro.Rows.Count > 0)
                {
                    PanelSoloHNoStock.Visible = false;
                }
                PanelNoStock.Visible = true;
            }

            if (cantCarro > 0)
            {
                precioCF = System.Convert.ToString(precioC);
                totalCF = System.Convert.ToString(totalC);
                precioLF = System.Convert.ToString(precioL);
                totalLF = System.Convert.ToString(totalL);




                //Repuesto se agrega al carro
                _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL,descuento)
                                   values('" + idSession + "','" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                                        "  " + cantCarro + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                        "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ",'" + descuento + "')");
                //Se llena GridViewCarro con los datos de la tabla carro
                GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + idSession + "'");
                GridViewCarro.DataBind();

                string plista = "";
                for (int i = 0; i < GridViewCarro.Rows.Count; i++)
                {
                    GridViewRow row = GridViewCarro.Rows[i];
                    plista = row.Cells[4].Text;
                    if (plista == "0")
                    {
                        row.Cells[4].Text = "N/A";
                    }
                }

                PanelCarroDeCompras.Visible = true;
                PanelCotizarSap.Visible = true;

                if (GridViewCarro.Rows.Count > 0)
                {
                    PanelSoloHNoStock.Visible = false;
                }
            }

            CargarMotivoPedido();
        }
        //Fin proceso de no stock

        _controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");

        // Si se encuentran resultados se muestran en una grilla
        // Obtener los datos de la tabla con los resultados de la busqueda
        SqlDataSource1.SelectCommand = "select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";
        try
        {
            GridViewListaRep.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            GridViewListaRep.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
            msjesError.Visible = true;
            logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Si existen cadenas de reemplazo se muestran en la grilla
        SqlDataSource2.SelectCommand = "select * from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";
        try
        {
            GridViewReemplazos.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            GridViewReemplazos.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
            msjesError.Visible = true;
            logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Se determina si el usuario es multisucursal
        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            ListaSucursales.Visible = true;
        }
        else
        {
            lblDireccion.Visible = true;
        }
    }

    protected void OnEliminarDelCArro(object o, EventArgs e)
    {
        //obtengo el codigo del repuesto a eliminar del carro
        string id = GridViewCarro.SelectedRow.Cells[1].Text.ToString();
        _controlBD.InsertarDatos("delete from carro where idSession = '" + Session["idSession"].ToString() + "' and codigo = '" + id + "'");

        GridViewCarro.DataSource = "";
        GridViewCarro.DataBind();
        GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
        GridViewCarro.DataBind();

        if (GridViewCarro.Rows.Count == 0)
        {
            PanelCotizarComun.Visible = false;
            PanelCarroDeCompras.Visible = false;
            PanelCotizarSap.Visible = false;
            if (GridViewNoStock.Rows.Count > 0)
            {
                PanelSoloHNoStock.Visible = true;
            }
        }

    }

    public void RecuperarCarro()
    {
        //Si el carro viene desde la pagina de consulta verifico si el usuarioe s multisursal
        if (Request.QueryString["visible"] == "true")
        {
            PanelPreferido.Visible = true;
            //Se determina si el usuario es multisucursal
            if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
            {
                ListaSucursales.Visible = true;
            }
            else
            {
                lblDireccion.Visible = true;
            }
        }

        //verifico si el carro esta vacio
        if (_sapApi.OnCarroVacio(Session["idSession"].ToString()))
        {
            PanelCarroDeCompras.Visible = true;

            //Se llena la grilla "CARRO" con los datos de la tabloa carro
            DataSet ds;
            try
            {
                GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
                GridViewCarro.DataBind();
                ds = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Problemas con la base de datos. Favor informar a los administradores. Gracias y disculpe las molestias";
                msjesError.Visible = true;
                logger.Error("Error al llenar grilla 'carro' en [RecuperarCarro]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ComboMarcas.SelectedIndex =
                    ComboMarcas.Items.IndexOf(
                        ComboMarcas.Items.FindByText(dr["marca"].ToString()));
            }

            ComboMarcas.Enabled = false;

            if (int.Parse(Session["permisos"].ToString()) != 3)
            {
                PanelCotizarComun.Visible = true;
            }
            else
            {
                PanelCotizarSap.Visible = true;
            }
            btnNewSearch.Visible = true;
        }

        //verifico si el carro no stock esta vacio
        if (_sapApi.OnCarroNoStockVacio(Session["idSession"].ToString()))
        {
            PanelNoStock.Visible = true;
            DataSet ds;

            //Se llena la grilla "NoStock" con los datos de la tabloa no_stock
            try
            {
                GridViewNoStock.DataSource = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + Session["idSession"].ToString() + "'");
                GridViewNoStock.DataBind();
                ds = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + Session["idSession"].ToString() + "'");
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Problemas con la base de datos. Favor informar a los administradores. Gracias y disculpe las molestias";
                msjesError.Visible = true;
                logger.Error("Error al llenar grilla 'NoStock' en [RecuperarCarro]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ComboMarcas.SelectedIndex =
                    ComboMarcas.Items.IndexOf(
                        ComboMarcas.Items.FindByText(dr["marca"].ToString()));
            }

            ComboMarcas.Enabled = false;

            if (int.Parse(Session["permisos"].ToString()) != 3)
            {
                PanelCotizarComun.Visible = true;
            }
            else
            {

            }
            btnNewSearch.Visible = true;

            if (GridViewNoStock.Rows.Count > 0 && GridViewCarro.Rows.Count == 0)
            {
                PanelSoloHNoStock.Visible = true;
            }
        }

        if (GridViewCarro.Rows.Count > 0)
        {
            ocultaColumnaStock();
        }
    }

    private void LlenarComboMarcas()
    {
        ComboMarcas.Items.Clear();
        ComboMarcas.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _controlBD.CrearMarcas(Session["rut"].ToString()))
        {
            ComboMarcas.Items.Add(marca);
        }
    }


    //Llena Combo con Motivo Pedido
    private void CargarMotivoPedido()
    {

        DropDownList ddlmotivodepedido = (DropDownList)PanelCotizarSap.FindControl("ddlmotivodepedido");

        ddlmotivodepedido.Items.Clear();
        ddlmotivodepedido.Items.Add(new ListItem("Seleccionar", "-1"));
        foreach (string pedido in _controlBD.TipoPedido(_marca))
        {
            ddlmotivodepedido.Items.Add(pedido);
        }
    }



    #region Calcula el gran total de una columna sin nada
    public double TotalUnitPrice;

    public double GetUnitPrice(double Price)
    {
        TotalUnitPrice += Price;
        return Price;
    }

    public double GetTotal()
    {
        return TotalUnitPrice;
    }
    #endregion


    #region Calcula el gran total de una columna
    public double TotalUnitPrice2;

    public double GetUnitPrice2(double Price)
    {
        TotalUnitPrice2 += Price;
        return Price;
    }

    public double GetTotal2()
    {
        return TotalUnitPrice2;
    }
    #endregion

    public void OnLimpiarCarro()
    {
        _controlBD.InsertarDatos("delete from carro where idSession = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        logger.Debug("Se limpia el carro, LISTA_BUSQUEDA_TMP y LISTA_REEMPLAZO_TMP");
        GridViewCarro.DataSource = "";
        GridViewCarro.DataBind();

        if (GridViewCarro.DataSource.Equals(""))
        {
            PanelCarroDeCompras.Visible = false;
            PanelCotizarSap.Visible = false;
        }
        btnNewSearch.Visible = true;
    }

    public void OnLimpiarNoStock()
    {
        _controlBD.InsertarDatos("delete from no_stock where idSession = '" + Session["idSession"].ToString() + "'");
        GridViewNoStock.DataSource = "";
        GridViewNoStock.DataBind();
        if (GridViewNoStock.DataSource.Equals(""))
        {
            PanelNoStock.Visible = false;
        }

        btnNewSearch.Visible = true;
    }

    protected void btnLimpiarCarro_Click(object sender, EventArgs e)
    {
        OnLimpiarCarro();
    }

    protected void btnLimpiaNoStock_Click(object sender, EventArgs e)
    {
        OnLimpiarNoStock();
    }

    protected void btnGenerarPdf_Click(object sender, EventArgs e)
    {
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
        if (ds.Tables[0].Rows.Count == 0)
        {
            Response.Redirect("buscarrepto2.aspx");
        }

        //Si el usuario modifica las cantidades del carro se actualiza la tabla en la BD
        int[] cantidades;
        cantidades = new int[GridViewCarro.Rows.Count];
        double totalC = 0;
        double totalL = 0;
        double precioLis = 0;
        double precioConc = 0;
        string cod = "";
        for (int i = 0; i < GridViewCarro.Rows.Count; i++)
        {
            GridViewRow row = GridViewCarro.Rows[i];
            TextBox txt = row.Cells[3].FindControl("txtCantidad") as TextBox;
            cod = row.Cells[1].Text;
            precioLis = double.Parse(row.Cells[7].Text);
            precioConc = double.Parse(row.Cells[8].Text);
            string rescate = txt.Text;
            totalL = double.Parse(rescate) * precioLis;
            totalC = double.Parse(rescate) * precioConc;

            totalCF = System.Convert.ToString(totalC);
            totalLF = System.Convert.ToString(totalL);

            //Realizo el calculo de los totales con la nueva cantidad
            _controlBD.InsertarDatos("update carro set cantidad = " + rescate + ", totalC = " + "convert(float,replace('" + totalCF + "',',','.'))" + ", totalL = " + "convert(float,replace('" + totalLF + "',',','.'))" + " where codigo = '" + cod + "' and idSession = '" + Session["idSession"].ToString() + "'");
        }

        GridViewCarro.DataSource = "";
        GridViewCarro.DataBind();

        string filePath = _pdf.CrearArchivoPdfComun(Session["rut"].ToString(), Session["idSession"].ToString());
        Boolean siGeneraArchivo = false;
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("Disculpa, el archivo '{0}' fue eliminado del servidor!", filePath));
            }
            siGeneraArchivo = true;
        }
        catch (FileNotFoundException ex)
        {
            logger.Error("En generar PDF FileNotFoundException. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            msjesError.Visible = true;
            msjesError.InnerText = "Error al generar documento PDF, favor contacte al administrador";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        catch (Exception ex)
        {
            logger.Error("En generar PDF Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            msjesError.Visible = true;
            msjesError.InnerText = "Error al generar documento PDF, favor contacte al administrador";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }

        var fi = new FileInfo(filePath);
        Response.Clear();
        Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", "CotizacionRespuestos.pdf"));
        Response.AddHeader("Content-Length", fi.Length.ToString());
        Response.ContentType = "application/octet-stream";
        Response.WriteFile(fi.FullName);
        Response.End();
    }

    protected void btnLimpiarCarroC_Click(object sender, EventArgs e)
    {
        OnLimpiarCarro();
        OnLimpiarNoStock();
    }

    protected void btnGuardarPref_Click(object sender, EventArgs e)
    {
        //Aquí todo el código para generar un pedido preferido
        try
        {
            //Variables para insertar pedido preferido
            string idSession = Session["idSession"].ToString();
            string rut = Session["rut"].ToString();
            string descrip = txtRefPrefe.Text.ToUpper();
            string idPreferido = "";

            //inserto en la tabla preferido
            _controlBD.InsertarDatos(@"insert into preferido(idSession,rutCreador,fechaCreacion,textoDescrip)
                                  values('" + idSession + "','" + rut + "',GETDATE(),'" + descrip + "')");

            //obtengo el idPreferido
            DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from preferido where idSession = '" + idSession + "' order by idPreferido desc");
            foreach (DataRow id in dsId.Tables[0].Rows)
            {
                idPreferido = id["idPreferido"].ToString();
            }

            string marca = "", codigo = "", descripcion = "", gMat = "", cantidad = "", stock = "", precioC = "", totalC = "", precioL = "", totalL = "";

            DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
            foreach (DataRow drow in ds.Tables[0].Rows)
            {
                marca = drow["marca"].ToString();
                codigo = drow["codigo"].ToString();
                descripcion = drow["descripcion"].ToString();
                gMat = drow["grupoMat"].ToString();
                cantidad = drow["cantidad"].ToString();
                stock = drow["stock"].ToString();
                precioC = drow["precioC"].ToString();
                precioL = drow["precioL"].ToString();
                totalC = drow["totalC"].ToString();
                totalL = drow["totalL"].ToString();

                _controlBD.InsertarDatos(@"insert into PREFERIDO_REPUESTO(idPreferido,marca,grupoMat,codigo,descripcion,cantidad,stock,precioC,totalC,precioL,totalL)
                                      values('" + idPreferido + "','" + marca + "','" + gMat + "','" + codigo + "','" + descripcion + "'," + cantidad + ", " +
                                        stock + " , " + "convert(float,replace('" + precioC + "',',','.'))" + " , " + "convert(float,replace('" + totalC + "',',','.'))" + " , " + "convert(float,replace('" + precioL + "',',','.'))" + "," + "convert(float,replace('" + totalL + "',',','.'))" + ")");
            }

            txtRefPrefe.Text = "";

            msjesError.InnerText = "Se ha guardado su carro como preferido. N°: " + idPreferido;
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Ha surgido un problema al guardar el pedido preferido. Favor notificar a los administradores";
            msjesError.Visible = true;
            logger.Error("Error al guardar pedido preferido. Inner: " + ex.InnerException + ". Message: " + ex.Message + ". Stack: " + ex.StackTrace);
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
    }

    //Metodo que busca un pedido preferido
    public void OnBuscarPreferido(string criterio)
    {
        GridViewTodosPreferidos.DataSource = "";
        GridViewTodosPreferidos.DataBind();

        if (ComboMarcas.SelectedIndex == 0)
        {
            msjesError.InnerText = "Debe seleccionar una marca, por favor";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select PREFERIDO_REPUESTO.marca,PREFERIDO_REPUESTO.codigo,PREFERIDO_REPUESTO.grupoMat,
            PREFERIDO_REPUESTO.descripcion,PREFERIDO_REPUESTO.cantidad,PREFERIDO_REPUESTO.stock,PREFERIDO_REPUESTO.precioC,
            PREFERIDO_REPUESTO.totalC,PREFERIDO_REPUESTO.precioL,PREFERIDO_REPUESTO.totalL,'' descuento
            from PREFERIDO_REPUESTO, PREFERIDO 
            where PREFERIDO.idPreferido = " + criterio + " and PREFERIDO_REPUESTO.idPreferido = '" + criterio + "' " +
            " and  PREFERIDO_REPUESTO.marca = '" + ComboMarcas.SelectedItem.Text + "'  ");
        DataSet dsN = _controlBD.ObtenerDatosFiltrados(@"select PREFERIDO_REPUESTO.marca,PREFERIDO_REPUESTO.codigo,PREFERIDO_REPUESTO.grupoMat,
            PREFERIDO_REPUESTO.descripcion,PREFERIDO_REPUESTO.cantidad,PREFERIDO_REPUESTO.stock,PREFERIDO_REPUESTO.precioC,
            PREFERIDO_REPUESTO.totalC,PREFERIDO_REPUESTO.precioL,PREFERIDO_REPUESTO.totalL,'' descuento
            from PREFERIDO_REPUESTO, PREFERIDO 
            where PREFERIDO.idPreferido = PREFERIDO_REPUESTO.idPreferido
            and PREFERIDO.textoDescrip = '" + criterio + "' " +
            " and  PREFERIDO_REPUESTO.marca = '" + ComboMarcas.SelectedItem.Text + "' ");

        if (criterio.Length == 0)
        {
            GridViewTodosPreferidos.DataSource = _controlBD.ObtenerDatosFiltrados(@"select idPreferido, textoDescrip, fechaCreacion from PREFERIDO where rutCreador = '" + Session["rut"].ToString() + "'  ");
            GridViewTodosPreferidos.DataBind();
            GridViewTodosPreferidos.Visible = true;
            return;
        }

        if (ds != null)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                // Se pisa el carro actual
                _controlBD.InsertarDatos("delete from carro where idSession = '" + Session["idSession"].ToString() + "'");

                foreach (DataRow campo in ds.Tables[0].Rows)
                {
                    _controlBD.InsertarDatos(@"insert into carro values('" + Session["idSession"].ToString() + "','" + campo["marca"].ToString() + "' , " +
                    " '" + campo["grupoMat"].ToString() + "' , '" + campo["codigo"].ToString() + "' , '" + campo["descripcion"].ToString() + "' , " + campo["cantidad"].ToString() + " , " + campo["stock"].ToString() + " , " + "convert(float,replace('" + campo["precioC"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + campo["totalC"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + campo["precioL"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + campo["totalL"].ToString() + "',',','.'))" + " , " + "NULL )");
                }
                GridViewCarro.DataSource = ds;
                GridViewCarro.DataBind();
                PanelCarroDeCompras.Visible = true;
                PanelCotizarSap.Visible = true;
                PanelPreferido.Visible = false;
                return;
            }
            else
            {
                msjesError.InnerText = "Su búsqueda no encontró resultados";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }
        if (dsN != null)
        {
            if (dsN.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow campo in dsN.Tables[0].Rows)
                {
                    _controlBD.InsertarDatos(@"insert into carro values('" + Session["idSession"].ToString() + "','" + campo["marca"].ToString() + "' , " +
                    " '" + campo["grupoMat"].ToString() + "' , '" + campo["codigo"].ToString() + "' , '" + campo["descripcion"].ToString() + "' , " + campo["cantidad"].ToString() + " , " + campo["stock"].ToString() + " , " + campo["precioC"].ToString() + " , " + campo["totalC"].ToString() + " , " + campo["precioL"].ToString() + " , " + campo["totalL"].ToString() + " )");
                }
                GridViewCarro.DataSource = dsN;
                GridViewCarro.DataBind();
                PanelCarroDeCompras.Visible = true;
                PanelCotizarSap.Visible = true;
                PanelPreferido.Visible = false;
                return;
            }
            else
            {
                msjesError.InnerText = "La búsqueda no encontró resultados";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        if (ComboMarcas.SelectedIndex == 0)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Debe escoger una marca";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        else
        {
            CargarMotivoPedido();
            OnBuscarPreferido(txtNPref.Text);
            txtNPref.Text = "";
        }
    }

    //Evento Click del commandfield de GridViewTodosPreferidos
    private void ClickVerPreferido(object o, EventArgs e)
    {
        string idPref = GridViewTodosPreferidos.SelectedRow.Cells[0].Text;
        OnBuscarPreferido(idPref);
        GridViewTodosPreferidos.Visible = false;
    }

    public void UserMultiSucursal()
    {
        //Se determina si el usuario es multisucursal
        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            ListaSucursales.Visible = true;
        }
        else
        {
            lblDireccion.Visible = true;
        }
    }

    public void CotizarSap()
    {
        int[] cantidades;

        //Modificar las cantidades del carro y los totales segun las nuevas cantidades
        cantidades = new int[GridViewCarro.Rows.Count];
        double totalC = 0;
        double totalL = 0;
        double precioLis = 0;
        double precioConc = 0;
        string cod = "";
        for (int i = 0; i < GridViewCarro.Rows.Count; i++)
        {
            GridViewRow row = GridViewCarro.Rows[i];
            TextBox txt = row.Cells[3].FindControl("txtCantidad") as TextBox;
            cod = row.Cells[1].Text;
            precioLis = double.Parse(row.Cells[7].Text);
            precioConc = double.Parse(row.Cells[8].Text);
            string rescate = txt.Text;

            // Se realiza el calculo de los totales con la nueva cantidad
            totalL = double.Parse(rescate) * precioLis;
            totalC = double.Parse(rescate) * precioConc;

            totalCF = System.Convert.ToString(totalC);
            totalLF = System.Convert.ToString(totalL);

            _controlBD.InsertarDatos("update carro set cantidad = " + rescate + ", totalC = " + "convert(float,replace('" + totalCF + "',',','.'))" + ", totalL = " + "convert(float,replace('" + totalLF + "',',','.'))" + " where codigo = '" + cod + "' and idSession = '" + Session["idSession"].ToString() + "'");
        }
    }

    public void EnviarNoStock(string DestinatarioMercancia, string tipoPedidoReserva)
    {
        RutHelper _rutHelper = new RutHelper();
        for (int i = 0; i < GridViewNoStock.Rows.Count; i++)
        {
            //Para backOrder///////////////////////////////////////////////////////////////////////////////////////
            GridViewRow row = GridViewNoStock.Rows[i];
            RadioButton rbt = row.Cells[0].FindControl("rbSeleccionarBO") as RadioButton;
            bool isChecked = ((RadioButton)row.FindControl("rbSeleccionarBO")).Checked;
            if (isChecked)
            {
                _controlBD.InsertarDatos("update no_stock set reserva = 's',destino='" + DestinatarioMercancia + "',tipoPedido='" + tipoPedidoReserva + "' where codigo = '" + row.Cells[4].Text + "' and idSession = '" + Session["idSession"].ToString() + "'");
            }

            //Para VFC//////////////////////////////////////////////////////////////////////////////////////////////
            GridViewRow row2 = GridViewNoStock.Rows[i];
            RadioButton rbt2 = row2.Cells[1].FindControl("rbSeleccionarVFC") as RadioButton;
            bool isChecked2 = ((RadioButton)row2.FindControl("rbSeleccionarVFC")).Checked;
            if (isChecked2)
            {
                for (int x = 0; x < GridViewNoStock.Rows.Count; x++)
                {
                    GridViewRow rowv = GridViewNoStock.Rows[x];
                    TextBox txt = rowv.Cells[1].FindControl("txtVin") as TextBox;

                    string rescate = txt.Text;
                    _numVin = rescate;
                }
                _controlBD.InsertarDatos("update no_stock set vfc = '" + _numVin + "',destino='" + DestinatarioMercancia + "',tipoPedido='" + tipoPedidoReserva + "' where codigo = '" + row.Cells[4].Text + "' and idSession = '" + Session["idSession"].ToString() + "'");

            }

            //Para Descarte ///////////////////////////////////////////////////////////////////////////////////////////
            GridViewRow row3 = GridViewNoStock.Rows[i];
            RadioButton rbt3 = row3.Cells[2].FindControl("rbSeleccionarDes") as RadioButton;
            bool isChecked3 = ((RadioButton)row3.FindControl("rbSeleccionarDes")).Checked;

            if (isChecked3)
            {
                _controlBD.InsertarDatos("update no_stock set descarte = 's' where codigo = '" + row.Cells[4].Text + "' and idSession = '" + Session["idSession"].ToString() + "'");
            }
        }
    }

    protected void btnNewSearch_Click(object sender, EventArgs e)
    {
        try
        {
            _controlBD.InsertarDatos("delete from carro where idSession = '" + Session["idSession"].ToString() + "'");
            _controlBD.InsertarDatos("delete from no_stock where idSession = '" + Session["idSession"].ToString() + "'");
            _controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
            _controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        }
        catch (Exception ex)
        {
            logger.Error("Error en [btnNewSearch_Click] al hacer nueva busqueda. Message: " + ex.Message + ". Inner: " + ex.Message + ". Stack: " + ex.StackTrace);
            return;
        }
        Response.Redirect("buscarrepto2.aspx");
    }

    protected void btnLimpiarSoloNoStock_Click(object sender, EventArgs e)
    {
        try
        {
            _controlBD.InsertarDatos("delete from no_stock where idSession = '" + Session["idSession"].ToString() + "'");
            GridViewNoStock.DataSource = "";
            GridViewNoStock.DataBind();
            PanelSoloHNoStock.Visible = false;
        }
        catch (Exception ex)
        {
            logger.Error("Error en [btnLimpiarSoloNoStock_Click] al limpiar no stock. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack:" + ex.StackTrace);
        }
    }

    protected void ResultadoBusqueda_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        SqlDataSource1.SelectCommand = "select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";
        GridViewListaRep.PagerSettings.Mode = PagerButtons.NumericFirstLast;
        try
        {
            GridViewListaRep.PageIndex = e.NewPageIndex;
            GridViewListaRep.DataBind();
        }
        catch (Exception ex)
        {
            logger.Error("Error en [ResultadoBusqueda_PageIndexChanging] al paginar reesultado busqueda. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    protected String determinaStock(string stock, string cantidad)
    {
        try
        {
            int _stock = int.Parse(stock);
            int _cantidad = int.Parse(cantidad);

            if (_stock >= _cantidad)
            {
                return "Sí";
            }
            else
            {
                return "No";
            }
        }
        catch (Exception)
        {
            return "Indef.";
        }
    }

    protected void btnVFCsi_Click(object sender, EventArgs e)
    {
        String marca = ultimaMarca.Value;
        String codigo = ultimoCodigo.Value;
        String cantidad = ultimaCantidad.Value;
        String detalle = ultimaDescripcion.Value;
        if (codigo.Trim() == "")
        {
            msjesError.InnerText = "Debe seleccionar un código para solicitar VFC o reserva";
            msjesError.Visible = true;
        }
        else
        {
            Response.Redirect("VFCEspecial.aspx?marca=" + marca + "&codigo=" + codigo + "&cantidad=" + cantidad + "&detalle=" + detalle);
        }
    }

    protected void btnVFCno_Click(object sender, EventArgs e)
    {
        VFCbusqueda.Visible = false;
    }

    protected void btnCotizar_Click(object sender, EventArgs e)
    {

        DropDownList ddlmotivodepedido = (DropDownList)PanelCotizarSap.FindControl("ddlmotivodepedido");

        string tipoPed = "";
        string I_BTWEG = "";
        string tipoPedido = ddlTipoDePedido.SelectedValue;
        string mitodopedido = "";
        string TPcodigo = "";


        //Validar que se escoja un Motivo de pedido
        if (ddlmotivodepedido.SelectedValue == "-1")
        {
            msjesError.InnerText = "Debes seleccionar un Motivo de Pedido de la lista, por favor";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        else
        {
            mitodopedido = ddlmotivodepedido.SelectedValue.ToString();
            TPcodigo = _controlBD.TipoPedidoValue(mitodopedido, _marca);
        }

        // Determinar garantía o normal para solo VFCs
        if (ddlTipoPedidoReserva.SelectedIndex == 0 && PanelSoloHNoStock.Visible)
        {
            msjesError.InnerText = "Debe seleccionar un tipo de pedido, garantía o normal";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Determinar garantia o normal
        if (ddlTipoDePedido.SelectedIndex == 0)
        {
            msjesError.InnerText = "Debe seleccionar un tipo de pedido, garantía o normal";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        if (ddlTipoDePedido.SelectedItem.Text == "Normal")
        {
            I_BTWEG = _sapApi.CanalDeDistribucionPedido;
            tipoPed = _sapApi.TipoClaseDocumentoVentasNormal;
        }

        if (ddlTipoDePedido.SelectedItem.Text == "Garantía")
        {
            I_BTWEG = _sapApi.CanalDeDistribucionGarantia;
            tipoPed = _sapApi.TipoClaseDocumentoVentasGarantias;
        }

        // Si el usuario no es multiple sucursal se envia el shipCode que
        // viene por defecto del usuario
        if (!_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            // Para panel con stock
            string idsucursal = _sapApi.GetIdSucursalRut(Session["rut"].ToString());
            int PerUsu = int.Parse(Session["permisos"].ToString());

            direccion = _sapApi.GetCodigoShipCode(Session["rut"].ToString());
            lblDireccion.Text = _sapApi.GetDireccionPorShipCod(_sapApi.GetCodigoShipCode(Session["rut"].ToString()), idsucursal, PerUsu);
            lblDireccion.Visible = true;

            // Para panel sin stock
            lblDestinatarioNoStock.Text = _sapApi.GetDireccionPorShipCod(_sapApi.GetCodigoShipCode(Session["rut"].ToString()), idsucursal, PerUsu);
            direccionExpandida = lblDestinatarioNoStock.Text;
        }

        //Si el usuario es multiple sucursal el sistema mostrará un comboBox con la opción de sucursales
        else
        {
            if (PanelSoloHNoStock.Visible)
            {
                tipoPedido = ddlTipoPedidoReserva.SelectedValue;

                if (ddlSucursalesNoStock.SelectedIndex == 0)
                {
                    msjesError.InnerText = "Debe seleccionar dirección de despacho";
                    msjesError.Visible = true;
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
                else
                {
                    direccion = _controlBD.GetShipCodPorDirec(ddlSucursalesNoStock.SelectedItem.ToString());
                    direccionExpandida = ddlSucursalesNoStock.SelectedItem.ToString();
                }
            }
            else
            {
                if (ListaSucursales.SelectedIndex == 0)
                {
                    msjesError.InnerText = "Debe seleccionar dirección de despacho";
                    msjesError.Visible = true;
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
                else
                {
                    direccion = _controlBD.GetShipCodPorDirec(ListaSucursales.SelectedItem.ToString());
                    direccionExpandida = ListaSucursales.SelectedItem.ToString();
                }
            }
        }

        CotizarSap();
        EnviarNoStock(direccion, tipoPedido);
        Response.Redirect("cotizar.aspx?direc=" + direccion + "&&tipoPed=" + tipoPed + "&&canalDis=" + I_BTWEG + "&&MtvoPed=" + TPcodigo);
    }

    protected void btnEnviarNoStock_Click(object sender, EventArgs e)
    {
        string tipoPedidoReserva = "";

        if (ddlTipoPedidoReserva.SelectedIndex == 0)
        {
            msjesError.InnerText = "Debe seleccionar un tipo de reserva";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }



        tipoPedidoReserva = ddlTipoPedidoReserva.SelectedValue;

        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            if (ddlSucursalesNoStock.SelectedIndex == 0)
            {
                msjesError.InnerText = "Debe seleccionar una dirección para el despacho, por favor";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            else
            {
                direccion = _controlBD.GetShipCodPorDirec(ddlSucursalesNoStock.SelectedItem.ToString());
            }
        }
        else
        {

            string idsucursal = _sapApi.GetIdSucursalRut(Session["rut"].ToString());
            int PerUsu = int.Parse(Session["permisos"].ToString());

            direccion = _sapApi.GetDireccionPorShipCod(_sapApi.GetCodigoShipCode(Session["rut"].ToString()), idsucursal,PerUsu);

            if (direccion == "")
            {
                msjesError.InnerText = "No tenemos registrado una dirección de despacho para usted. Por favor contacte al administrador del sitio";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }

        /*if (direccion != "")
            {
                msjesError.InnerText = direccion;
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }*/

        EnviarNoStock(direccion, tipoPedidoReserva);
        Response.Redirect("cotizar.aspx?direc=" + direccion + "&&tipoPed=" + tipoPedidoReserva + "");
        //Response.Redirect("cotizar.aspx");
    }

    protected void btnBuscarCodigo_Click(object sender, ImageClickEventArgs e)
    {

    }

    protected void GridViewListaRep_RowDataBound(object sender, EventArgs e)
    {
        //e.ToString
        double stock = 0;
        double precio = 0;

        if (((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row.RowType == DataControlRowType.DataRow)
        {
            if (((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells.Count > 0)
            {
                precio = Convert.ToDouble(((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[4].Text);
                stock = Convert.ToDouble(((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[6].Text);

                if (stock == 0 || precio == 0)
                {
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[9].Text = "No";
                }
                else
                {
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[9].Text = "Si";
                }
            }
        }
    }

    protected void GridViewReemplazos_RowDataBound(object sender, EventArgs e)
    {
        //e.ToString
        double stock = 0;
        double precio = 0;

        if (((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row.RowType == DataControlRowType.DataRow)
        {
            if (((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells.Count > 0)
            {
                stock = Convert.ToDouble(((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[7].Text);
                precio = Convert.ToDouble(((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[4].Text);

                if (stock == 0 || precio == 0)
                {
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[8].Text = "No";
                    //Nuevo
                    ((System.Web.UI.WebControls.TableRow) (((System.Web.UI.WebControls.GridViewRowEventArgs) (e)).Row)).Cells[4].Visible = false;
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[10].Visible = false;
                  
                }
                else
                {
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[8].Text = "Si";
                    //Nuevo
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[4].Visible = false;
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[10].Visible = false;
                   
                }
            }
        }
    }

    protected void btnUdpCarro_Click(object sender, EventArgs e)
    {
        String session = Session["idSession"].ToString();
        string cantiadad = "";
        string CodRep  = "";

        string stock = "";


        for (int i = 0; i < GridViewCarro.Rows.Count; i++)
        {
            GridViewRow row = GridViewCarro.Rows[i];
            CodRep = row.Cells[1].Text;
            stock = _controlBD.obtstock(session, CodRep);
            TextBox txtRespuesta = (TextBox)row.FindControl("txtCantidad");
            cantiadad = txtRespuesta.Text;

            if (int.Parse(cantiadad) > int.Parse(stock))
            {
                //mesage 
                string script = @"<script type='text/javascript'>alert('Error: Cantidad solicitada supera a stock');</script>"; 
                //llamar mensage 
                ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false); 
            }
            else
            {

                double precioLis = double.Parse(row.Cells[4].Text);
                double precioConc = double.Parse(row.Cells[5].Text);
                double totalL = double.Parse(cantiadad) * precioLis;
                double totalC = double.Parse(cantiadad) * precioConc;

                totalCF = System.Convert.ToString(totalC);
                totalLF = System.Convert.ToString(totalL);

                _controlBD.InsertarDatos("update carro set cantidad = " + cantiadad + ", totalC = " + "convert(float,replace('" + totalCF + "',',','.'))" + ", totalL = " + "convert(float,replace('" + totalLF + "',',','.'))" + " where codigo = '" + CodRep + "' and idSession = '" + session + "'");

            }

            
        }
        GridViewCarro.DataSource = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + session + "'");
        GridViewCarro.DataBind();    
    }

}
