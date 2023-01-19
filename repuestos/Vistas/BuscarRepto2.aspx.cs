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
using System.Net;
using ExcelLibrary.BinaryFileFormat;

public partial class Vistas_buscarRepto : System.Web.UI.Page
{

    // Clase que interactua con la base de datos
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
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

    //envio correo 
    SendMail_helper _mail = new SendMail_helper();

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
    int vfcoReserva;
    int btnVfc = 5;
    int btnReserva = 6;
    int VFC = 1;
    int RESERVA = 2;

    string prefijoMarca2 = "";

    int _precioSolicitud = 0;
    string _solicitudDias = "";
    string _solicitudTipoTransporte = "";

    SqlConnection con;
    SqlCommand cmd, cmd1;

    int Consulta = 1;     //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
    int rptoExiste;   //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
    int AgregaCarro = 2; //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 


    protected void Page_Load(object sender, EventArgs e)
    {
        
        Page.Form.Attributes.Add("enctype", "multipart/form-data"); //para evitar que el fileupload quede vacio la primera ves que se sube el archivo

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

            string descrpcionDireccionSucursal = _sapApi.GetDireccionPorShipCod(_sapApi.GetCodigoShipCode(Session["rut"].ToString()), idsucursal, PerUsu);
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

        //VALIDAR DONDE CREAR COTIZACION AUTOMATICA
        // REQ - Cotización automatica Marzo 2022


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
            prefijoMarca2 = prefijoMarca;
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
            //CODIGO NO EXISTE EN SAP
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

            // REQ - Cotización automatica Marzo 2022
            //VALIDA SI LA MARCA ESTA HABILITADA PARA COTIZACION 2.0
            string _marcaHabilitada = "";

            DataSet dsValidaMarca = _controlBD.ObtenerDatosFiltrados(" SELECT cotizacionAutomatica FROM marca WHERE nombreMarca = '" + ComboMarcas.SelectedValue + "' ");
            foreach (DataRow drValidaMarca in dsValidaMarca.Tables[0].Rows)
            {
                _marcaHabilitada = drValidaMarca["cotizacionAutomatica"].ToString();
            }
            //FIN VALIDA SI LA MARCA ESTA HABILITADA PARA COTIZACION 2.0

            if (Convert.ToBoolean(_marcaHabilitada) == false)
            {
                msjesError.InnerText = "ATENCION: Marca no habilitada para cotizaciones automáticas.";
                msjesError.Visible = true;
                return;
            }
            else
            {
                string _codigoCotizado = "";
                DataSet dsCotizado = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM carga_fob_tmp WHERE campo0 = '" + prefijoMarca2 + "' AND campo1 = '" + txtCodigo.Text.ToUpper() + "' ");
                foreach (DataRow dr in dsCotizado.Tables[0].Rows)
                {
                    _codigoCotizado = dr["campo2"].ToString();
                }


                if (_codigoCotizado.Length > 0)
                {
                    btnCotizar2.Enabled = true;
                    int _nReg = 0;

                    DataSet dsValidaCarroNornal = _controlBD.ObtenerDatosFiltrados(" SELECT COUNT(*) AS n_reg FROM carro WHERE idSession = '" + Session["idSession"].ToString() + "' ");
                    foreach (DataRow drValidaCarroNormal in dsValidaCarroNornal.Tables[0].Rows)
                    {
                        _nReg = Convert.ToInt32(drValidaCarroNormal["n_reg"]);
                    }

                    if (_nReg > 0)
                    {
                        msjesError.InnerText = "ATENCION: Repuesto '" + txtCodigo.Text + "' no tiene stock. Favor realizar una nueva cotización para este producto (en un nuevo carro de compra).";
                        msjesError.Visible = true;

                        GridViewListaRep.Visible = false;
                        VFCbusqueda.Visible = false;
                        GridViewCarro.Visible = true;
                        btnCotizar2.Enabled = false;
                        return;
                    }

                    //***********************fin cotizacion automatica *******************************//

                    //VFCbusqueda.Visible = true;
                    tipoSolicitud.Visible = true;
                    btnCotizar2.Enabled = true;

                    lblCodigo2.Text = txtCodigo.Text.ToUpper();
                    lblCodigoCotizado.Text = _codigoCotizado.ToUpper();
                    lblTitulo.Text = "Repuesto no tiene stock.";

                    VFCbusqueda.Visible = false;
                    GridViewListaRep.Visible = false;
                    GridViewCarro.Visible = false;
                    GridViewReemplazos.Visible = false;
                    msjesError.Visible = false;

                    //Cotizacion 2.0
                    //calculaValores();
                }
                else
                {
                    GridViewListaRep.DataSource = null;
                    GridViewListaRep.DataBind();
                    GridViewListaRep.Visible = false;
                    GridViewTodosPreferidos.Visible = false;
                    GridViewReemplazos.DataSource = null;
                    GridViewListaRep.DataBind();
                    GridViewReemplazos.Visible = false;

                    //VFCbusqueda.Visible = true;
                    //msjesError.InnerText = _pedido.mensajeError;
                    //msjesError.Visible = true;

                }
                // REQ - Cotización automatica Marzo 2022
            }
        }
        else
        {
            //CODIGO EXISTE EN SAP

            int _nReg = 0;
            int _nReg2 = 0;

            if (!_pedido.disponibilidadRepuesto)
            {
                msjesError.InnerText = _pedido.mensajeError;
                msjesError.Visible = true;
            }

            //agregado por Anibal para Cotizacion Automatica
            if (_pedido.disponibilidadVFC) //si no tiene stock ni precio
            {
                if (ComboMarcas.SelectedItem.Text != "MOBIL-COPEC")
                {
                    string _codigoCotizado = "";
                    //VFCbusqueda.Visible = true;

                    //if (_stock == 0 && _nReg == 0 && _nReg2 == 0)
                    if (_nReg == 0 && _nReg2 == 0)
                    {
                        DataSet dsCotizado = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM carga_fob_tmp WHERE campo0 = '" + prefijoMarca2 + "' AND campo1 = '" + txtCodigo.Text.ToUpper() + "' ");
                        foreach (DataRow dr in dsCotizado.Tables[0].Rows)
                        {
                            _codigoCotizado = dr["campo2"].ToString();
                        }


                        if (_codigoCotizado.Length > 0)
                        {
                            string _marcaHabilitada = "";

                            DataSet dsValidaMarca = _controlBD.ObtenerDatosFiltrados(" SELECT cotizacionAutomatica FROM marca WHERE nombreMarca = '" + ComboMarcas.SelectedValue + "' ");
                            foreach (DataRow drValidaMarca in dsValidaMarca.Tables[0].Rows)
                            {
                                _marcaHabilitada = drValidaMarca["cotizacionAutomatica"].ToString();
                            }

                            if (Convert.ToBoolean(_marcaHabilitada) == false)
                            {
                                VFCbusqueda.Visible = true;
                            }
                            else
                            {
                                lblCodigo2.Text = txtCodigo.Text;
                                lblCodigoCotizado.Text = _codigoCotizado;
                                tipoSolicitud.Visible = true;
                                GridViewListaRep.Visible = false;
                                btnCotizar2.Enabled = true;
                                return;
                            }
                            /*
                            lblCodigo2.Text = txtCodigo.Text;
                            lblCodigoCotizado.Text = _codigoCotizado;
                            tipoSolicitud.Visible = true;
                            GridViewListaRep.Visible = false;
                            btnCotizar2.Enabled = true;
                            return;
                            */

                        }
                        else
                        {
                            VFCbusqueda.Visible = true;
                        }
                    }

                    if (_nReg > 0 || _nReg2 > 0)
                    {
                        if (_nReg2 > 0)
                        {

                            DataSet dsCotizado = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM carga_fob_tmp WHERE campo0 = '" + prefijoMarca2 + "' AND campo1 = '" + txtCodigo.Text.ToUpper() + "' ");
                            foreach (DataRow dr in dsCotizado.Tables[0].Rows)
                            {
                                _codigoCotizado = dr["campo2"].ToString();
                            }

                            if (_codigoCotizado.Length > 0)
                            {
                                VFCbusqueda.Visible = false;
                                GridViewListaRep.Visible = false;
                                valoresPorVia.Visible = true;
                                pnlSolicitud.Visible = true;
                                tipoSolicitud.Visible = true;
                                //dgvSolicitud.Visible = true;
                                GridViewListaRep.Visible = false;
                                GridViewCarro.Visible = false;
                                msjesError.Visible = false;

                                lblCodigo2.Text = txtCodigo.Text;
                                lblCodigoCotizado.Text = _codigoCotizado;
                                btnCotizar2.Enabled = true;
                            }
                            else
                            {
                                VFCbusqueda.Visible = false;
                                GridViewListaRep.Visible = false;

                                msjesError.InnerText = "ATENCION: Repuesto '" + txtCodigo.Text + "' no tiene stock. Favor realizar una nueva cotización para este producto (en un nuevo carro de compra).";
                                msjesError.Visible = true;

                                btnCotizar2.Enabled = false;
                                valoresPorVia.Visible = true;
                                pnlSolicitud.Visible = true;
                                tipoSolicitud.Visible = true;
                                //dgvSolicitud.Visible = true;
                                GridViewListaRep.Visible = false;
                                GridViewCarro.Visible = false;
                                btnCotizar2.Enabled = false;
                            }


                        }

                        if (_nReg > 0)
                        {
                            VFCbusqueda.Visible = false;
                            GridViewListaRep.Visible = false;

                            msjesError.InnerText = "ATENCION: Repuesto '" + txtCodigo.Text + "' no tiene stock. Favor realizar una nueva cotización para este producto (en un nuevo carro de compra).";
                            msjesError.Visible = true;


                            valoresPorVia.Visible = false;
                            pnlSolicitud.Visible = false;
                            tipoSolicitud.Visible = false;
                            //dgvSolicitud.Visible = false;
                            GridViewListaRep.Visible = false;
                            GridViewCarro.Visible = true;
                            btnCotizar2.Enabled = false;
                        }

                        return;
                    }
                }
            }
            //fin bloque cotizacion automatica

            //Consulto si mi repuesto encontro algo
            string busqueda = _sapApi.EncontreMaterial(Session["idSession"].ToString(), _consultaRep.CodRepuesto);
            if (busqueda == "0" && (_consultaRep.CodRepuesto != null))
            {
                VFCbusqueda.Visible = true;
            }

            // Si se encuentran resultados se muestran en una grilla
            // Obtener los datos de la tabla con los resultados de la busqueda
            SqlDataSource1.SelectCommand = "select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'";

            try
            {
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

        //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento       
        int porce;

        string ipPc;
        ipPc = GetIP4Address();
        //ipPc = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily.ToString().ToUpper().Equals("INTERNETWORK")).FirstOrDefault().ToString();
        string seg_idSesion = Session["idSession"].ToString();
        string seg_identificador = Session["rut"].ToString();
        string seg_marca = ComboMarcas.SelectedItem.Text;
        string seg_socSap = shipCode;
        string seg_cnalDistrib = _consultaRep.CanalDistribucion;
        if (seg_cnalDistrib == null)
        { seg_cnalDistrib = ""; }
        string seg_codMaterialSap = _consultaRep.CodRepuesto;  //txtCodigo.Text;
        if (_consultaRep.CodRepuesto == null)
        { seg_codMaterialSap = ""; }

        string seg_descrpMaterial = "";
        int seg_precioConce = 0;
        int seg_precioListaSugerido = 0;
        int seg_stockConsulta = 0;
        if (_pedido.ResultBusqueda.Codigo != null) //Aqui te tocamos 02/08 dti-66019
        {
            seg_descrpMaterial = _pedido.ResultBusqueda.Descripcion;
            rptoExiste = 1;
            seg_precioConce = QuitaDecimal(_pedido.ResultBusqueda.PrecioConce);
            seg_precioListaSugerido = QuitaDecimal(_pedido.ResultBusqueda.PrecioLista);
            seg_stockConsulta = Convert.ToInt32(_pedido.ResultBusqueda.Stock);
        }
        else
        {
            seg_descrpMaterial = "";
            rptoExiste = 0;
        }


        string seg_gpoMaterial = _consultaRep.GrupoMaterial;
        if (_consultaRep.GrupoMaterial == null)
        { seg_gpoMaterial = ""; }
        string seg_gpoMaterialFrec = "1"; //De donde extraigo este dato?
        int seg_cantidadCotizada = Convert.ToInt32(txtCantidad.Text);


        if (seg_precioConce != 0 || seg_precioListaSugerido != 0)
        {
            double resto = double.Parse(_pedido.ResultBusqueda.PrecioConce) / double.Parse(_pedido.ResultBusqueda.PrecioLista);
            double subtotal = resto * 100;
            double totaldesc = 100 - subtotal;
            porce = Convert.ToInt32(Math.Round(totaldesc));
        }
        else
        {
            porce = 0;
        }
        //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
        int seg_descuento = porce;
        int seg_agregaCarro = 0;
        string seg_nroCotizacion = "0";
        string seg_nroPedidoSap = "0";
        int seg_ultimoEvento = Consulta;
        int seg_rptoExiste = rptoExiste;
        int seg_vfcReserba = vfcoReserva;


        _controlBD.SeguimientoPedido(seg_idSesion,
                                      seg_identificador,
                                      seg_marca,
                                      seg_socSap,
                                      seg_cnalDistrib,
                                      seg_codMaterialSap,
                                      seg_descrpMaterial,
                                      seg_gpoMaterial,
                                      seg_gpoMaterialFrec,
                                      seg_cantidadCotizada,
                                      seg_precioListaSugerido,
                                      seg_descuento,
                                      seg_precioConce,
                                      seg_stockConsulta,
                                      seg_agregaCarro,
                                      seg_nroCotizacion,
                                      seg_nroPedidoSap,
                                      seg_ultimoEvento,
                                      seg_rptoExiste,
                                      seg_vfcReserba,
                                      ipPc,
                                      0);
        //Seguimiento --> REQ FEbrero 2022
    } //fianaliza proceso 


    //Obtener la IP ce consulta  //Seguimiento --> REQ FEbrero 2022
    public static string GetIP4Address()
    {
        string IP4Address = String.Empty;

        foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
        {
            if (IPA.AddressFamily.ToString() == "InterNetwork")
            {
                IP4Address = IPA.ToString();
                break;
            }
        }

        if (IP4Address != String.Empty)
        {
            return IP4Address;
        }

        foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (IPA.AddressFamily.ToString() == "InterNetwork")
            {
                IP4Address = IPA.ToString();
                break;
            }
        }

        return IP4Address;
    }
    //Incluido con //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
    public static String Test(string s)
    {
        if (String.IsNullOrEmpty(s))
            return "0";
        else
            return s;
    }
    public int QuitaDecimal(string valor)
    {
        valor = valor.Replace(" ", ""); //algunos valores llegan con espacion en blanco desde SAP. Con esta linea los quieto para que no problema en la busqueda.
        string valorDev = "";
        string valorRetorno = "";
        int decimalPosition;

        valorDev = Test(valor);

        if (valorDev != "0")
        {
            decimalPosition = valor.IndexOf(".");
            if (decimalPosition >= 0)
            {
                valorDev = valorDev.Remove(decimalPosition, 3).Trim();
                valorRetorno = valorDev;
            }

        }
        else
        {
            valorRetorno = "0";
        }

        return int.Parse(valorRetorno);

    }
    //Seguimiento --> REQ FEbrero 2022



    // Agregar al carro
    public void AddToCart(object o, EventArgs e)
    {
        con = new SqlConnection();
        cmd = new SqlCommand();

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

        string descuento = Convert.ToString(descuen); //Modificacion, Revisar PO

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
                    "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ", '" + descuento + "')");

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


                //SP nueva Modalidad Carro
                con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "webr_agrega_carro";
                cmd.CommandTimeout = 10;
                cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@i_sesion", SqlDbType.VarChar).Value = idSession;
                cmd.Parameters.Add("@i_marca", SqlDbType.VarChar).Value = marca;
                cmd.Parameters.Add("@i_grupomat", SqlDbType.VarChar).Value = grupoMat;
                cmd.Parameters.Add("@i_codigo", SqlDbType.VarChar).Value = codigo;
                cmd.Parameters.Add("@i_descripcion", SqlDbType.VarChar).Value = descripcion;
                cmd.Parameters.Add("@i_cantidadcarro", SqlDbType.Int).Value = cantCarro;
                cmd.Parameters.Add("@i_stock", SqlDbType.Int).Value = stock;
                cmd.Parameters.Add("@i_precioConcesionario", SqlDbType.Float).Value = precioCF;
                cmd.Parameters.Add("@i_totalconcesionario", SqlDbType.Float).Value = totalCF;
                cmd.Parameters.Add("@i_precioLista", SqlDbType.Float).Value = precioLF;
                cmd.Parameters.Add("@i_totallista", SqlDbType.Float).Value = totalLF;
                cmd.Parameters.Add("@i_descuento", SqlDbType.VarChar).Value = descuento;
                //Salida
                cmd.Parameters.Add("@o_cod_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.ExecuteReader();
                //Repuesto se agrega al carro ---PO BORRAR
                /*
                _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL,descuento)
                                   values('" + idSession + "','" + marca + "', '" + grupoMat + "','" + codigo + "','" + descripcion + "', " +
                                        "  " + cantCarro + ", " + stock + " , " + "convert(float,replace('" + precioCF + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                        "  " + "convert(float,replace('" + precioLF + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + "," + descuento + ")");
                 */
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
        //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 

        //Traking, Agrega al carro
        //Actualiza consulta
        //string changemarca = "";
        Session["Tracking"] = "NO";
        string ipPc = "";
        string seg_idSesion = Session["idSession"].ToString();
        string seg_identificador = "";
        string seg_marca = "";
        string seg_socSap = "";
        string seg_cnalDistrib = "";
        string seg_codMaterialSap = "";  //txtCodigo.Text;
        string seg_descrpMaterial = "";
        string seg_gpoMaterial = "";
        string seg_gpoMaterialFrec = ""; //De donde extraigo este dato?
        int seg_cantidadCotizada = 0;
        int seg_precioConce = 0;
        int seg_precioListaSugerido = 0;
        int porce = 0;
        int seg_descuento = porce;
        int seg_stockConsulta = 0;
        int seg_agregaCarro = 1;
        string seg_nroCotizacion = "0";
        string seg_nroPedidoSap = "0";
        int seg_ultimoEvento = AgregaCarro;
        int seg_rptoExiste = 0;
        int seg_vfcReserba = 0;



        _controlBD.SeguimientoPedido(seg_idSesion,
                                  seg_identificador,
                                  seg_marca,
                                  seg_socSap,
                                  seg_cnalDistrib,
                                  seg_codMaterialSap,
                                  seg_descrpMaterial,
                                  seg_gpoMaterial,
                                  seg_gpoMaterialFrec,
                                  seg_cantidadCotizada,
                                  seg_precioListaSugerido,
                                  seg_descuento,
                                  seg_precioConce,
                                  seg_stockConsulta,
                                  seg_agregaCarro,
                                  seg_nroCotizacion,
                                  seg_nroPedidoSap,
                                  seg_ultimoEvento,
                                  seg_rptoExiste,
                                  seg_vfcReserba,
                                  ipPc,
                                  0);

        if (stock != "0")
        {
            Session["Tracking"] = "SI";
        }

        if (Session["Tracking"].ToString() == "SI")
        {
            Session["RepOriginal"] = codSap;
            _controlBD.InsertarDatos(@"INSERT INTO track_repuestos_fullback (repuesto_origen, repuesto_destino, cantidad_solicitada, precio_unidad, id_sesion, usuario, fecha_carro)
                                            VALUES('" + "ITR" + Session["RepOriginal"].ToString() + "','" + "MMR" + codigo + "','" + cantidad + "','" + precioC + "','" + idSession + "','" + Session["rut"].ToString() + "',GETDATE())");
        }
        //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 


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



                //Seguimiento --> REQ FEbrero 2022
                //Traking, Agrega al carro, cadena reemplazo
                //Actualiza agrega carro
                string ipPc = "";
                string seg_idSesion = Session["idSession"].ToString();
                string seg_identificador = "";
                string seg_marca = "";
                string seg_socSap = "";
                string seg_cnalDistrib = "";
                string seg_codMaterialSap = codigo;  //txtCodigo.Text;
                string seg_descrpMaterial = "";
                string seg_gpoMaterial = "";
                string seg_gpoMaterialFrec = ""; //De donde extraigo este dato?
                int seg_cantidadCotizada = 0;
                int seg_precioConce = 0;
                int seg_precioListaSugerido = 0;
                int porce = 0;
                int seg_descuento = porce;
                int seg_stockConsulta = 0;
                int seg_agregaCarro = 1;
                string seg_nroCotizacion = "0";
                string seg_nroPedidoSap = "0";
                int seg_ultimoEvento = AgregaCarro;
                int seg_rptoExiste = 0;
                int seg_vfcReserba = 0;

                _controlBD.SeguimientoPedido(seg_idSesion,
                                          seg_identificador,
                                          seg_marca,
                                          seg_socSap,
                                          seg_cnalDistrib,
                                          seg_codMaterialSap,
                                          seg_descrpMaterial,
                                          seg_gpoMaterial,
                                          seg_gpoMaterialFrec,
                                          seg_cantidadCotizada,
                                          seg_precioListaSugerido,
                                          seg_descuento,
                                          seg_precioConce,
                                          seg_stockConsulta,
                                          seg_agregaCarro,
                                          seg_nroCotizacion,
                                          seg_nroPedidoSap,
                                          seg_ultimoEvento,
                                          seg_rptoExiste,
                                          seg_vfcReserba,
                                          ipPc,
                                          1);

                // Traking, Agrega al carro 
                //Seguimiento --> REQ FEbrero 2022

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

    protected String determinaStock2()
    {
        try
        {
            /*int _stock2 = _sapApi.GetCurrentStockByProduct(_pedido.ResultBusqueda.Codigo, 
                _consultaRep.GrupoMaterial, 
                _consultaRep.DestinaMercacia, 
                txtNombre.Text, 
                ComboMarcas.SelectedItem.Text);*/
            int _stock2 = _pedido.stockSap_Origen;

            if (_stock2 > 0)
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
            vfcoReserva = VFC;
            //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
            //Actualiza consulta
            string ipPc = "";
            string seg_idSesion = Session["idSession"].ToString();
            string seg_identificador = "";
            string seg_marca = "";
            string seg_socSap = "";
            string seg_cnalDistrib = "";
            string seg_codMaterialSap = "";  //txtCodigo.Text;
            string seg_descrpMaterial = "";
            string seg_gpoMaterial = "";
            string seg_gpoMaterialFrec = ""; //De donde extraigo este dato?
            int seg_cantidadCotizada = 0;
            int seg_precioConce = 0;
            int seg_precioListaSugerido = 0;
            int porce = 0;
            int seg_descuento = porce;
            int seg_stockConsulta = 0;
            int seg_agregaCarro = 0;
            string seg_nroCotizacion = "0";
            string seg_nroPedidoSap = "0";
            int seg_ultimoEvento = btnReserva;
            int seg_rptoExiste = 0;
            int seg_vfcReserba = RESERVA;

            _controlBD.SeguimientoPedido(seg_idSesion,
                                      seg_identificador,
                                      seg_marca,
                                      seg_socSap,
                                      seg_cnalDistrib,
                                      seg_codMaterialSap,
                                      seg_descrpMaterial,
                                      seg_gpoMaterial,
                                      seg_gpoMaterialFrec,
                                      seg_cantidadCotizada,
                                      seg_precioListaSugerido,
                                      seg_descuento,
                                      seg_precioConce,
                                      seg_stockConsulta,
                                      seg_agregaCarro,
                                      seg_nroCotizacion,
                                      seg_nroPedidoSap,
                                      seg_ultimoEvento,
                                      seg_rptoExiste,
                                      seg_vfcReserba,
                                      ipPc,
                                      0);
            //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 


            Response.Redirect("VFCEspecial.aspx?marca=" + marca + "&codigo=" + codigo + "&cantidad=" + cantidad + "&detalle=" + detalle);
        }
    }

    protected void btnVFCno_Click(object sender, EventArgs e)
    {
        //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
        //VFCbusqueda.Visible = false;
        //Actualiza consulta
        string ipPc = "";
        string seg_idSesion = Session["idSession"].ToString();
        string seg_identificador = "";
        string seg_marca = "";
        string seg_socSap = "";
        string seg_cnalDistrib = "";
        string seg_codMaterialSap = "";  //txtCodigo.Text;
        string seg_descrpMaterial = "";
        string seg_gpoMaterial = "";
        string seg_gpoMaterialFrec = ""; //De donde extraigo este dato?
        int seg_cantidadCotizada = 0;
        int seg_precioConce = 0;
        int seg_precioListaSugerido = 0;
        int porce = 0;
        int seg_descuento = porce;
        int seg_stockConsulta = 0;
        int seg_agregaCarro = 0;
        string seg_nroCotizacion = "0";
        string seg_nroPedidoSap = "0";
        int seg_ultimoEvento = btnVfc;
        int seg_rptoExiste = 0;
        int seg_vfcReserba = VFC;

        _controlBD.SeguimientoPedido(seg_idSesion,
                                  seg_identificador,
                                  seg_marca,
                                  seg_socSap,
                                  seg_cnalDistrib,
                                  seg_codMaterialSap,
                                  seg_descrpMaterial,
                                  seg_gpoMaterial,
                                  seg_gpoMaterialFrec,
                                  seg_cantidadCotizada,
                                  seg_precioListaSugerido,
                                  seg_descuento,
                                  seg_precioConce,
                                  seg_stockConsulta,
                                  seg_agregaCarro,
                                  seg_nroCotizacion,
                                  seg_nroPedidoSap,
                                  seg_ultimoEvento,
                                  seg_rptoExiste,
                                  seg_vfcReserba,
                                  ipPc,
                                  0);
        //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
        VFCbusqueda.Visible = false;
        Response.Redirect("SolicitudCotizacion.aspx");
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

        //INSERTA INFORMACION ADICIONAL
        if (uplFile.Visible)
        {
            string pathDocu = Server.MapPath("~/doc/infoAdicionalPedido/");
            string extension = System.IO.Path.GetExtension(uplFile.FileName);
            extension = extension.ToLower();
            if (extension != ".pdf")
            {
                msjesError.InnerText = "Solo se permiten archivos de tipo PDF";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                //btnCotizar.Enabled = true;
                return;
            }

            if (System.IO.File.Exists(pathDocu + uplFile.FileName))
            {
                msjesError.InnerText = "El archivo que intenta subir, ya existe. Intente con otro nombre";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                //btnCotizar.Enabled = true;
                return;
            }

            uplFile.PostedFile.SaveAs(pathDocu + uplFile.FileName);

            _controlBD.InsertarDatos("  INSERT INTO info_ad_datos_pedido " +
                                     "  (id_pedido, compania, tipo_sustento, chasis, siniestro, ot, taller, nro_sustento, ruc, archivo, marca, motivo)" +
                                     "  VALUES " +
                                     "  ('" + Session["idSession"].ToString() + "', '" + ddlCompania.SelectedItem.ToString() + "', '" + ddlTipoSustento.SelectedItem.ToString() + "', '" + txtNroChasis.Text + "', '" + txtNroSiniestro.Text + "', '" + txtNroOt.Text + "', '" + txtTaller.Text + "', '" + txtNroSustento.Text + "', '" + txtRuc.Text + "', '" + uplFile.FileName.ToString() + "', '" + ComboMarcas.SelectedItem.ToString() + "', '" + ddlmotivodepedido.SelectedItem.ToString() + "') ");
        }
        //FIN INSERTA INFORMACION ADICIONAL

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

            direccion = _sapApi.GetDireccionPorShipCod(_sapApi.GetCodigoShipCode(Session["rut"].ToString()), idsucursal, PerUsu);

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
                    //Bloque se descomenta el 21Dic2021
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[4].Text = "--";
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[5].Text = "--";
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[8].Text = "--";
                    //Bloque se descomenta el 21Dic2021
                }
                //else
                //{
                //    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[9].Text = "Si";
                //}
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
                    ((System.Web.UI.WebControls.TableRow)(((System.Web.UI.WebControls.GridViewRowEventArgs)(e)).Row)).Cells[4].Visible = false;
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
        string CodRep = "";

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
    public Boolean validaArchivoAdicional()
    {
        int _valor = 0;

        DataSet ds = _controlBD.ObtenerDatosFiltrados(" SELECT mp_documentacion FROM Motivo_Pedido WHERE mp_Marca = '" + ComboMarcas.SelectedItem.ToString() + "' AND RTRIM(LTRIM(MP_Descripcion)) = '" + ddlmotivodepedido.SelectedItem.ToString() + "' ");
        if (ds != null)
        { //bloque que verifica referencia estalecida 
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                _valor = Convert.ToInt32(dr["mp_documentacion"]);
            }

        }

        if (_valor == 1)
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    public Boolean validaDatosAdicionales()
    {
        uplFile.Visible = false;
        int _valor = 0;

        DataSet ds = _controlBD.ObtenerDatosFiltrados(" SELECT mp_informacion FROM Motivo_Pedido WHERE mp_Marca = '" + ComboMarcas.SelectedItem.ToString() + "' AND RTRIM(LTRIM(MP_Descripcion)) = '" + ddlmotivodepedido.SelectedItem.ToString() + "' ");

        if (ds != null)
        { //bloque que verifica referencia estalecida 
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                _valor = Convert.ToInt32(dr["mp_informacion"]);
            }

        }


        if (_valor == 1)
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    protected void ddlmotivodepedido_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlmotivodepedido.SelectedValue.ToString() != "-1")
        {
            bool _archivoAdicional = validaArchivoAdicional();
            bool _datosAdicionales = validaDatosAdicionales();

            if (_archivoAdicional)
            {
                uplFile.Visible = true;
            }

            if (_datosAdicionales)
            {
                ddlCompania.SelectedValue = "-1";
                ddlTipoSustento.SelectedValue = "-1";
                txtNroChasis.Text = "";
                txtNroSiniestro.Text = "";
                txtNroOt.Text = "";
                txtTaller.Text = "";
                txtNroSustento.Text = "";
                txtRuc.Text = "";


                ddlCompania.Enabled = false;
                ddlTipoSustento.Enabled = false;
                txtNroChasis.Enabled = false;
                txtNroSiniestro.Enabled = false;
                txtNroOt.Enabled = false;
                txtTaller.Enabled = false;
                txtNroSustento.Enabled = false;
                txtRuc.Enabled = false;

                int _idMotivo = 0;
                string _query = " SELECT ID_Motivo_pedido FROM Motivo_Pedido WHERE mp_Marca = '" + ComboMarcas.SelectedItem.Text + "' AND LTRIM(RTRIM((MP_Descripcion))) = '" + ddlmotivodepedido.SelectedItem.Text.Trim() + "' ";

                DataSet ds = _controlBD.ObtenerDatosFiltrados(_query);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    _idMotivo = Convert.ToInt32(dr["ID_Motivo_pedido"]);
                }


                int _id = 0;
                bool _habilitado = false;

                DataSet dsMotivos = _controlBD.ObtenerDatosFiltrados(" SELECT " +
                                                                        "   IAD.id, IAD.descripcion, IADD.habilitado " +
                                                                        " FROM " +
                                                                        "    dbo.info_ad_datos_detalle IADD, " +
                                                                        "    dbo.info_ad_datos IAD " +
                                                                        " WHERE " +
                                                                        "    IAD.id = IADD.id_info_ad " +
                                                                        " AND " +
                                                                        "    IADD.id_motivo = '" + _idMotivo + "' " +
                                                                        " AND " +
                                                                        "    IADD.marca = '" + ComboMarcas.SelectedItem.Text + "' ");
                if (dsMotivos.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow drMotivos in dsMotivos.Tables[0].Rows)
                    {
                        _id = Convert.ToInt32(drMotivos["id"]);
                        _habilitado = Convert.ToBoolean(drMotivos["habilitado"]);

                        switch (_id)
                        {
                            case 1:
                                if (_habilitado) ddlCompania.Enabled = true;
                                break;
                            case 2:
                                if (_habilitado) ddlTipoSustento.Enabled = true;
                                break;
                            case 3:
                                if (_habilitado) txtNroChasis.Enabled = true;
                                break;
                            case 4:
                                if (_habilitado) txtNroSiniestro.Enabled = true;
                                break;
                            case 5:
                                if (_habilitado) txtNroOt.Enabled = true;
                                break;
                            case 6:
                                if (_habilitado) txtTaller.Enabled = true;
                                break;
                            case 7:
                                if (_habilitado) txtNroSustento.Enabled = true;
                                break;
                            case 8:
                                if (_habilitado) txtRuc.Enabled = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    ddlCompania.Enabled = false;
                    ddlTipoSustento.Enabled = false;
                    txtNroChasis.Enabled = false;
                    txtNroSiniestro.Enabled = false;
                    txtNroOt.Enabled = false;
                    txtTaller.Enabled = false;
                    txtNroSustento.Enabled = false;
                }

            }
            else
            {
                ddlCompania.Enabled = false;
                ddlTipoSustento.Enabled = false;
                txtNroChasis.Enabled = false;
                txtNroSiniestro.Enabled = false;
                txtNroOt.Enabled = false;
                txtTaller.Enabled = false;
                txtNroSustento.Enabled = false;
            }
        }
        else
        {
            uplFile.Visible = false;
        }
    }

    protected void btnCotizar2_Click(object sender, EventArgs e)
    {
        ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;
        btnCrearSolicitud.Visible = false;
        btnAgregar.Enabled = true;

        string query = "delete from carro where idSession = '" + Session["idSession"].ToString() + "'";
        _ControlBD.EjecutaQuery(query);

        if (ddlTipoSolicitud.SelectedValue == "0")
        {
            msjesError.InnerText = "Debes seleccionar un tipo de solicitud.";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        if (lblCodigo2.Text.Length == 0)
        {
            msjesError.InnerText = "Debe Ingresar un codigo para cotizar";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //valida vin
        if (txtVin2.Text.Length == 0 || txtVin2.Text.Length > 17)
        {
            msjesError.InnerText = "Debe Ingresar un Vin Valido";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        if (txtVin2.Text.Length < 17)
        {
            msjesError.InnerText = "El numero de VIN, debe tener 17 caracteres";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        try
        {
            String vinval = null;
            String modelo_vin = null;

            ControlVfc _controlVFC = new ControlVfc();
            _controlVFC.Vin = txtVin2.Text;
            _controlVFC.ObtieneDatosVfc();
            vinval = _controlVFC.getVin();
            modelo_vin = _controlVFC.getModelo();

            if (modelo_vin == null) modelo_vin = "";

            lblEtiModelo.Visible = true;
            lblModelo.Visible = true;
            lblEtiModelo.Text = "Modelo :";
            lblModelo.Text = modelo_vin;

            if (lblModelo.Text.Length > 0)
            {
                if (vinval.ToUpper() == txtVin2.Text.ToUpper())
                {
                    //hfVIn.Value = "1";
                    modelo_vin = "";
                }
                else
                {
                    modelo_vin = "";
                }
            }
            else
            {
                lblEtiModelo.Visible = false;
                lblModelo.Visible = false;
            }
        }
        catch (Exception)
        {
            txtVin.Text = "";
            msjesError.InnerText = "Uno de los datos obtenidos esta en blanco.";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        //fin valida vin


        calculaValores();
        tipoSolicitud.Visible = true;
        valoresPorVia.Visible = true;
        pnlSolicitud.Visible = true;
    }


    private void calculaValores()
    {
        try
        {
            //Cotizacion 2.0
            //si el material NO tiene stoick y NO es frecuncia, se activa la cotizacion 2.0
            string _codigoCotizado = "";
            decimal _fob = 0;
            decimal _tipoCambio = 0;
            decimal _volumen = 0;
            decimal _volumenFob = 0;
            decimal _volumenNeog = 0;
            decimal _factorVolumen = 0;
            decimal _precioLista = 0;
            decimal _factorGrupoTecnico = 0;
            string _viaTransporte = "";
            string _plazoTransporte = "";
            string _moneda = "";
            string _grupoTecnico = "";
            decimal _fci = 0;
            int _costoAereo = 0;
            int _costoTerrestre = 0;
            int _costoMaritimo = 0;


            rbOpcionAereo.Checked = false;
            rbOpcionMaritimo.Checked = false;
            rbOpcionTerrestre.Checked = false;

            rbOpcionAereo.Enabled = true;
            rbOpcionMaritimo.Enabled = true;
            rbOpcionTerrestre.Enabled = true;

            lblTitulo.Text = "Valores por via de importación.";

            _consultaRep.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(_marca);
            string prefijoMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consultaRep.GrupoMaterial);

            DataSet dsCotizado = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM carga_fob_tmp WHERE campo0 = '" + prefijoMarca + "' AND campo1 = '" + txtCodigo.Text.ToUpper() + "' ");
            foreach (DataRow dr in dsCotizado.Tables[0].Rows)
            {
                _codigoCotizado = dr["campo2"].ToString();
                _fob = Convert.ToDecimal(dr["campo3"].ToString());
                _volumenFob = Convert.ToDecimal(dr["campo4"].ToString());
                _moneda = dr["campo6"].ToString();
                _grupoTecnico = dr["campo7"].ToString();
            }

            DataSet dsVolumenNeog = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM NEOGISTICA_PE WHERE CODIGO_SKU = '" + prefijoMarca + txtCodigo.Text.ToUpper() + "' ");
            foreach (DataRow dr in dsVolumenNeog.Tables[0].Rows)
            {
                _volumenNeog = Convert.ToDecimal(dr["VOLUMEN"].ToString());
            }

            if (_volumenFob >= _volumenNeog) _volumen = _volumenFob;
            if (_volumenNeog >= _volumenFob) _volumen = _volumenNeog;

            //_factorVolumen = _volumen;

            decimal _desde = 0;
            decimal _hasta = 0;
            DataSet dsFactorVolumen = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM carga_fob_tramos_volumen WHERE marca = '" + ComboMarcas.SelectedValue + "' AND grupo_tecnico = '" + _grupoTecnico + "' ");
            foreach (DataRow dr in dsFactorVolumen.Tables[0].Rows)
            {
                _desde = Convert.ToDecimal(dr["desde"]);
                _hasta = Convert.ToDecimal(dr["hasta"]);
                if (_volumen >= _desde && _volumen <= _hasta)
                {
                    _factorVolumen = Convert.ToDecimal(dr["factor"]);
                }
            }

            DataSet dsTipoCambio = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM monedas WHERE sigla = '" + _moneda + "' AND habilitado = 1 ");
            foreach (DataRow dr in dsTipoCambio.Tables[0].Rows)
            {
                _tipoCambio = Convert.ToDecimal(dr["valor"]);
            }

            if (_codigoCotizado.Length > 0)
            {
                //lblCodigoCotizado.Text = _codigoCotizado;
                valoresCotizacion.Visible = true;

                DataSet dsPlazoTransporte = _controlBD.ObtenerDatosFiltrados(" SELECT TT.Nombre, PMT.Dias, PMT.fci FROM PlazoMarcaTransporte PMT, TipoTransporte TT WHERE PMT.Marca = '" + ComboMarcas.SelectedItem + "' AND PMT.TipoTransporteId = tt.Id AND tt.Habilitado = '1' ");
                foreach (DataRow dr in dsPlazoTransporte.Tables[0].Rows)
                {
                    _viaTransporte = dr["Nombre"].ToString();
                    _plazoTransporte = dr["Dias"].ToString();
                    _fci = Convert.ToDecimal(dr["fci"]);

                    if (_factorVolumen == 0) _factorVolumen = 1;

                    // se camabia lbl precios por el FOB
                    switch (_viaTransporte.ToUpper())
                    {
                        case "AEREO NORMAL":
                            lblTranposrteAereo.Text = _plazoTransporte;

                            lblPrecioAereo.Text = Math.Round((_fob * _fci * _tipoCambio * _factorVolumen)).ToString();
                            _costoAereo = Convert.ToInt32(lblPrecioAereo.Text);

                            _desde = 0;
                            _hasta = 0;
                            DataSet dsFactorGrupoTecnico = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM fob_factores_utilidad WHERE marca = '" + ComboMarcas.SelectedItem + "' AND grupo_tecnico = '" + _grupoTecnico + "' ");
                            foreach (DataRow dr2 in dsFactorGrupoTecnico.Tables[0].Rows)
                            {
                                _desde = Convert.ToInt32(dr2["desde"]);
                                _hasta = Convert.ToInt32(dr2["hasta"]);
                                if (Convert.ToInt32(_fob) >= _desde && Convert.ToInt32(_fob) <= _hasta)
                                {
                                    _factorGrupoTecnico = Convert.ToDecimal(dr2["factor"]);
                                }
                            }

                            _precioLista = Convert.ToInt32(lblPrecioAereo.Text) * _factorGrupoTecnico;
                            //lblPrecioAereo.Text = _costoAereo + "-" + _factorGrupoTecnico + "-" + _precioLista.ToString("N0");
                            lblPrecioAereo.Text = _precioLista.ToString("N0");

                            if (lblPrecioAereo.Text == "0") rbOpcionAereo.Enabled = false;

                            break;
                        case "MARITIMO":
                            lblTransporteMaritimo.Text = _plazoTransporte;

                            lblPrecioMaritimo.Text = Math.Round((_fob * _fci * _tipoCambio * _factorVolumen)).ToString();
                            _costoMaritimo = Convert.ToInt32(lblPrecioMaritimo.Text);

                            _desde = 0;
                            _hasta = 0;
                            DataSet dsFactorGrupoTecnico2 = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM fob_factores_utilidad WHERE marca = '" + ComboMarcas.SelectedItem + "' AND grupo_tecnico = '" + _grupoTecnico + "' ");
                            foreach (DataRow dr2 in dsFactorGrupoTecnico2.Tables[0].Rows)
                            {
                                _desde = Convert.ToInt32(dr2["desde"]);
                                _hasta = Convert.ToInt32(dr2["hasta"]);
                                if (Convert.ToInt32(_fob) >= _desde && Convert.ToInt32(_fob) <= _hasta)
                                {
                                    _factorGrupoTecnico = Convert.ToDecimal(dr2["factor"]);
                                }
                            }

                            _precioLista = Convert.ToInt32(lblPrecioMaritimo.Text) * _factorGrupoTecnico;
                            //lblPrecioMaritimo.Text = _costoMaritimo + "-" + _factorGrupoTecnico + "-" + _precioLista.ToString("N0");
                            lblPrecioMaritimo.Text = _precioLista.ToString("N0");

                            if (lblPrecioMaritimo.Text == "0") rbOpcionMaritimo.Enabled = false;

                            break;
                        case "COURIER":
                            lblTransporteTerrestre.Text = _plazoTransporte;

                            lblPrecioTerrestre.Text = Math.Round((_fob * _fci * _tipoCambio * _factorVolumen)).ToString();
                            _costoTerrestre = Convert.ToInt32(lblPrecioTerrestre.Text);

                            _desde = 0;
                            _hasta = 0;
                            DataSet dsFactorGrupoTecnico3 = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM fob_factores_utilidad WHERE marca = '" + ComboMarcas.SelectedItem + "' AND grupo_tecnico = '" + _grupoTecnico + "' ");
                            foreach (DataRow dr2 in dsFactorGrupoTecnico3.Tables[0].Rows)
                            {
                                _desde = Convert.ToInt32(dr2["desde"]);
                                _hasta = Convert.ToInt32(dr2["hasta"]);
                                if (Convert.ToInt32(_fob) >= _desde && Convert.ToInt32(_fob) <= _hasta)
                                {
                                    _factorGrupoTecnico = Convert.ToDecimal(dr2["factor"]);
                                }
                            }

                            _precioLista = Convert.ToInt32(lblPrecioTerrestre.Text) * _factorGrupoTecnico;
                            //lblPrecioTerrestre.Text = _costoTerrestre + "-" + _factorGrupoTecnico + "-" + _precioLista.ToString("N0");
                            lblPrecioTerrestre.Text = _precioLista.ToString("N0");

                            if (lblPrecioTerrestre.Text == "0") rbOpcionTerrestre.Enabled = false;

                            break;
                    }
                }
            }

            //FIN si el material NO tiene stoick y NO es frecuncia, se activa la cotizacion 2.0
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Se produjo un error en la generacion de la solicitud. Favor re intentar ";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            //_mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [BuscaRepto_calculaValores] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            return;
        }
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        ddlTipoSolicitud.Enabled = false;
        txtVin2.Enabled = false;
        msjesError.Visible = false;
        dgvSolicitud.Visible = true;

        if (dgvSolicitud.Rows.Count >= 18)
        {
            msjesError.InnerText = "Ha alcanzado el límite de repuestos a solicitar en el mismo pedido";
            msjesError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }



        string query = "";
        string sesion = Session["idSession"].ToString();

        lblPrecioAereo.Text = lblPrecioAereo.Text.Replace(".", "");
        lblPrecioMaritimo.Text = lblPrecioMaritimo.Text.Replace(".", "");
        lblPrecioTerrestre.Text = lblPrecioTerrestre.Text.Replace(".", "");

        if (rbOpcionAereo.Checked) _precioSolicitud = Convert.ToInt32(lblPrecioAereo.Text);
        if (rbOpcionAereo.Checked) _solicitudDias = lblTranposrteAereo.Text;
        if (rbOpcionAereo.Checked) _solicitudTipoTransporte = "Aereo Normal";

        if (rbOpcionMaritimo.Checked) _precioSolicitud = Convert.ToInt32(lblPrecioMaritimo.Text);
        if (rbOpcionMaritimo.Checked) _solicitudDias = lblTransporteMaritimo.Text;
        if (rbOpcionMaritimo.Checked) _solicitudTipoTransporte = "Maritimo";

        if (rbOpcionTerrestre.Checked) _precioSolicitud = Convert.ToInt32(lblPrecioTerrestre.Text);
        if (rbOpcionTerrestre.Checked) _solicitudDias = lblTransporteTerrestre.Text;
        if (rbOpcionTerrestre.Checked) _solicitudTipoTransporte = "Courier";

        try
        {
            if (ddlTipoSolicitud.SelectedValue == "0")
            {
                msjesError.InnerText = "Debes seleccionar un tipo de solicitud.";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            if (!rbOpcionAereo.Checked && !rbOpcionMaritimo.Checked && !rbOpcionTerrestre.Checked)
            {
                msjesError.InnerText = "Debe seleccionar alguna via de importación.";
                msjesError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }


            btnCrearSolicitud.Visible = true;


            if (ComboMarcas.Enabled == true)
            {
                query = "insert into t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin, detalle, tipo_solicitud) Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + txtVin2.Text + "'" + "," + "''" + "," + "'FO'" + ")";
                _ControlBD.EjecutaQuery(query);

                query = "exec sp_CreaSolicitudCotizacion_TMP_SA " + "'" + rut + "'" + "," + "'" + ComboMarcas.SelectedValue + "'" + "," + "'" + txtCodigo.Text + "'" + "," + "'" + lblCodigoCotizado.Text + "'" + "," + "'" + txtCantidad.Text + "'" + "," + "'" + ddlTipoSolicitud.SelectedValue + "'" + "," + "'" + sesion + "'" + "," + "'" + txtVin2.Text + "'" + "," + "'" + _solicitudTipoTransporte + "'" + "," + "'" + _solicitudDias + "'" + "," + "'" + _precioSolicitud + "' ";
                _ControlBD.EjecutaQuery(query);

                try
                {
                    //aviso.Visible = true;
                    dgvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias FROM t_SolicitudCotizacion_TMP tmp FULL JOIN t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'");
                    dgvSolicitud.DataBind();
                }
                catch (Exception ex)
                {
                    msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                    msjesError.Visible = true;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Problemas con los reemplazos. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
            }
            else
            {
                query = "SELECT numeroSolicitud FROM t_SolicitudCotizacion_TMP WHERE sesion = '" + sesion + "'"; //Cambio 1
                string resol = Convert.ToString(_ControlBD.Existencia(query));
                if (resol == "")
                {
                    query = "insert into t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin, detalle, tipo_solicitud) Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + txtVin2.Text + "'" + "," + "''" + "," + "'FO'" + ")";
                    _ControlBD.EjecutaQuery(query);

                    query = "exec sp_CreaSolicitudCotizacion_TMP_SA " + "'" + rut + "'" + "," + "'" + ComboMarcas.SelectedValue + "'" + "," + "'" + txtCodigo.Text + "'" + "," + "'" + lblCodigoCotizado.Text + "'" + "," + "'" + txtCantidad.Text + "'" + "," + "'" + ddlTipoSolicitud.SelectedValue + "'" + "," + "'" + sesion + "'" + "," + "'" + txtVin2.Text + "'" + "," + "'" + _solicitudTipoTransporte + "'" + "," + "'" + _solicitudDias + "'" + "," + "'" + _precioSolicitud + "' ";
                    _ControlBD.EjecutaQuery(query);

                    try
                    {
                        //aviso.Visible = true;
                        dgvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias FROM t_SolicitudCotizacion_TMP tmp FULL JOIN t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'");
                        dgvSolicitud.DataBind();
                    }
                    catch (Exception ex)
                    {
                        msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                        msjesError.Visible = true;
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Problemas con los reemplazos. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                        //logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                        ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                        return;
                    }
                    //PanelSolicitud.Visible = true;
                    //gvSolicitud.Visible = true;
                    //descrepto.Visible = true;
                    //txtdescripcion.Visible = true;
                    //Combotipotrans.Enabled = false;
                    //aviso.Visible = true;
                }
                else
                {
                    query = "exec sp_CreaSolicitudCotizacion_TMP_SA " + "'" + rut + "'" + "," + "'" + ComboMarcas.SelectedValue + "'" + "," + "'" + txtCodigo.Text + "'" + "," + "'" + lblCodigoCotizado.Text + "'" + "," + "'" + txtCantidad.Text + "'" + "," + "'" + ddlTipoSolicitud.SelectedValue + "'" + "," + "'" + sesion + "'" + "," + "'" + txtVin2.Text + "'" + "," + "'" + _solicitudTipoTransporte + "'" + "," + "'" + _solicitudDias + "'" + "," + "'" + _precioSolicitud + "' ";
                    _ControlBD.EjecutaQuery(query);

                    try
                    {
                        //aviso.Visible = true;
                        //cambio 3
                        string queryConsulta;
                        queryConsulta = "SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias FROM t_SolicitudCotizacion_TMP tmp FULL JOIN t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'";
                        dgvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(queryConsulta);
                        dgvSolicitud.DataBind();
                    }
                    catch (Exception ex)
                    {
                        msjesError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                        msjesError.Visible = true;
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Problemas con los reemplazos. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                        //logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                        ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                        return;
                    }
                    //PanelSolicitud.Visible = true;
                    //descrepto.Visible = true;
                    //txtdescripcion.Visible = true;
                    //Combotipotrans.Enabled = false;
                    //aviso.Visible = true;
                }
            }

        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Se produjo un error en la generacion de la solicitud. Favor re intentar ";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [BuscaRepto_btnAgregar_Click] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + "###  " + Session["idSession"].ToString());
            return;
        }
    }

    protected void btnCrearSolicitud_Click(object sender, EventArgs e)
    {
        int nro_solicitud = 0;

        try
        {
            string query = "";
            string sesion = Session["idSession"].ToString();
            string resol = "";
            string correo = "";
            string envio = "";

            query = "SELECT numeroSolicitud FROM t_SolicitudCotizacion_TMP WHERE sesion = '" + sesion + "'"; //Cambio 1
            resol = Convert.ToString(_ControlBD.Existencia(query));
            correo = "";

            if (resol == "")
            {
                msjesError.Visible = true;
                msjesError.InnerText = "No se puede generar una solicitud vacia, Ingrese al menos un material. ";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            else
            {
                DataSet ds;
                ds = _controlBD.ObtenerDatosFiltrados("SELECT envio FROM t_SolicitudCotizacion_TMP WHERE sesion = '" + sesion + "' GROUP BY envio ORDER BY envio ");

                int sw = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    envio = dr["envio"].ToString();

                    if (sw == 1)
                    {
                        DataSet ds1;
                        ds1 = _controlBD.ObtenerDatosFiltrados("SELECT MAX(numeroSolicitud) AS nro_solicitud FROM t_SolicitudCotizacion_TMP WHERE sesion = '" + sesion + "'");

                        foreach (DataRow dr1 in ds1.Tables[0].Rows)
                        {
                            nro_solicitud = Convert.ToInt32(dr1["nro_solicitud"]);
                            sw = sw + 1;
                        }

                        query = "UPDATE t_SolicitudCotizacion_TMP SET " +
                                "numeroSolicitud = '' " +
                                "WHERE sesion = '" + sesion + "' " +
                                "AND envio <> '" + envio + "' ";
                        _ControlBD.EjecutaQuery(query);

                    }
                    else
                    {
                        query = "insert into t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin, detalle, tipo_solicitud) Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + txtVin2.Text + "'" + "," + "''" + "," + "'FO'" + ")";
                        _ControlBD.EjecutaQuery(query);

                        DataSet ds2;
                        ds2 = _controlBD.ObtenerDatosFiltrados("SELECT MAX(id_Cotiza) AS nro_solicitud FROM t_Cotiza WHERE sesion = '" + sesion + "'");

                        foreach (DataRow dr2 in ds2.Tables[0].Rows)
                        {
                            nro_solicitud = Convert.ToInt32(dr2["nro_solicitud"]);
                        }

                        query = "UPDATE t_SolicitudCotizacion_TMP SET " +
                                "numeroSolicitud = '" + nro_solicitud + "' " +
                                "WHERE envio = '" + envio + "'" +
                                "AND sesion = '" + sesion + "' ";
                        _ControlBD.EjecutaQuery(query);
                    }

                    query = "exec sp_CreaSolicitudFinal_SA " + "'" + nro_solicitud + "'";
                    _ControlBD.EjecutaQuery(query);

                    query = "SELECT email FROM persona WHERE rut = '" + rut + "'";
                    correo = _ControlBD.Email(query);

                    string text = "Se creó nueva solicitud de cotización número " + nro_solicitud + "";

                    string filePath = _pdf.CrearSolicitudPdfComunFinal(Session["idSession"].ToString(), nro_solicitud.ToString(), rut);

                    _mail.EnviarCorreoAdjunto(correo, "Solicitud de Cotización Realizada: " + nro_solicitud, text, filePath);

                }

                query = "DELETE t_SolicitudCotizacion_TMP WHERE sesion = '" + Session["idSession"].ToString() + "'";
                _ControlBD.EjecutaQuery(query);

                Response.Redirect("SolConsultaCotizacion.aspx?txtdesde=" + DateTime.Today.ToString("dd/MM/yyyy") + "&txthasta=" + DateTime.Today.ToString("dd/MM/yyyy") + "&txtVin=" + txtVin2.Text + "&txtEstado=C", false);
            }
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Se produjo un error en la generacion de la solicitud. Favor re intentar ";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [BuscaRepto_btnCrearSolicitud_Click] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + "###  " + Session["idSession"].ToString() + " @@@ " + nro_solicitud + "###" + rut);
            return;
        }
    }

    protected void btnFrecuencia_Click(object sender, EventArgs e)
    {

    }

    protected void eliminaDeSolicitud(object sender, GridViewCommandEventArgs e)
    {

    }
}

