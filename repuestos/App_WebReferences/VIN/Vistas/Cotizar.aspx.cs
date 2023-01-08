using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Configuration;
using log4net;
using log4net.Config;

public partial class Vistas_cotizar : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();
    CrearCotizacion _creaCotizacion = new CrearCotizacion();
    SapAPI _sapApi = new SapAPI();
    RealizarPedido _pedido = new RealizarPedido();
    DataSet dsRegistrosSinStock;
    PdfHelper _pdf;
    string tipoPed = "", canalDis = "", direccion = "", MtvoPed = "";

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_cotizar));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        // Tipo de pedido (normal/garantía)
        tipoPed = Request.QueryString["tipoPed"];

        // Canal de distribución (tierra/aire)
        canalDis = Request.QueryString["canalDis"];

        // Direccion
        direccion = Request.QueryString["direc"];

        //Motivo Pedido
        MtvoPed = Request.QueryString["MtvoPed"];

        // Se muestran los resumenes de productos sin stock
        //VerListasResumenes();

        if (GridViewResumen.Rows.Count < 1)
        {
            btnGenerarPDF.Visible = false;
        }

        // Se asigna la ruta del server al coinstructor de PdfHelper
        _pdf = new PdfHelper(Server.MapPath(""));

        DataSet dsCart = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '"+Session["idSession"].ToString()+"'");
        if (dsCart.Tables[0].Rows.Count != 0)
        {
            CotizarEnSap();
            //PanelConfirmaPedido.Visible = true;
        }
        else 
        {
            //PanelConfirmaPedido.Visible = false;
        }

        dsRegistrosSinStock = new DataSet();
        dsRegistrosSinStock = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + Session["idSession"].ToString() + "'");
        GridViewNoStock.DataSource ="";
        GridViewNoStock.DataBind();
        SinStock();
    }

    public void CotizarEnSap()
    {   
        //Insertar en la tabla Pedido
        DataSet dsM = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
        string marcaRep = "";
        string prefMarca = "";
        foreach (DataRow campos in dsM.Tables[0].Rows)
        {
            marcaRep = campos["marca"].ToString();
            prefMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(campos["grupoMat"].ToString(), campos["marca"].ToString()); //Se agrega Marca
            _creaCotizacion.Cantidad.Add(campos["cantidad"].ToString()); //(cantidades[x].ToString());
            _creaCotizacion.Codigo.Add(prefMarca.Trim() + campos["codigo"].ToString());

            _controlBD.InsertarDatos(@"insert into MATERIALES_PEDIDO(id_pedido,marca,codigo,descripcion,strGrupoMateriales,cantidad,stock,valor,total)
                values('" + Session["idSession"].ToString() + "','" + campos["marca"].ToString() + "','" + campos["codigo"].ToString() + "', " +
                "    '" + campos["descripcion"].ToString() + "', '" + campos["grupoMat"].ToString() + "' ," + campos["cantidad"].ToString() + ", " +
                "    " + campos["stock"].ToString() + "," + "convert(float,replace('" + campos["precioC"].ToString() + "',',','.'))" + "," + "convert(float,replace('" + campos["totalC"].ToString() + "',',','.'))" + ")");
        }

        //Obtener fecha de caducidad SAP
        dynamic fechaActual = System.DateTime.Now;
        int ValidezDeLaOferta = 0;
        // int decimalPosition = 0;
        DateTime thisDay = DateTime.Today;
        dynamic fechaFutura = thisDay.AddDays(ValidezDeLaOferta).ToString("yyyyMMdd");

        //Obtener fecha de caducidad SAP
        dynamic fechaActual1 = System.DateTime.Now;
        int ValidezDeLaOferta1 = 5;
        // int decimalPosition = 0;
        DateTime thisDay1 = DateTime.Today;
        dynamic fechaFutura1 = thisDay.AddDays(ValidezDeLaOferta1).ToString("yyyyMMdd");

        //Obtener fecha de caducidad del sistema
        dynamic fechaActual2 = System.DateTime.Now;
        int ValidezDeLaOferta2 = 15;
        // int decimalPosition = 0;
        DateTime thisDay2 = DateTime.Today;
        dynamic fechaFutura2 = thisDay.AddDays(ValidezDeLaOferta2).ToString("yyyyMMdd");

        //Obtener datos para cotizar
        _creaCotizacion.ClaseDocVentas = _sapApi.ClaseDocumentoDeVentas;


        // Para marcas foraneas, va una X en el campo i_foraneo
        ControlMarca _controlMarca = new ControlMarca();
        Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marcaRep);
        if (marcaVehiculo == null)
        {
            throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
        }
        if (_controlMarca.obtenerMarcaPorNombre(marcaRep).esForaneo)
        {
            _creaCotizacion.ClaseDocVentas = _sapApi.ClaseDocumentoDeVentasForaneo;
        }

        _creaCotizacion.FecValides = fechaFutura;
        _creaCotizacion.CodClienteSap = _sapApi.GetCodigoClienteSapByRut(Session["rut"].ToString());
        _creaCotizacion.CodClienteSap2 = direccion;
        _creaCotizacion.SpartRep = _sapApi.Sector;
        _creaCotizacion.DescripCotizacion = "";
        _creaCotizacion.OrgVentas = _sapApi.GetVkorgByGrupoMaterial(_sapApi.GetGrupoMaterialesByMarca(marcaRep),marcaRep);

        //Seleccionar pedido o garantia
        _creaCotizacion.CDistribucion = canalDis;
        _creaCotizacion.MtvoPedido = MtvoPed;

        string idSession = Session["idSession"].ToString();
        string rutUser = Session["rut"].ToString();

        //Envio los datos para generar la cotización
        if (_pedido.CrearCotizacion(_creaCotizacion, marcaRep))
        {
            if (_pedido.ResultCotiza.NumCotizacion != null)
            {
                //Se inserta en la tabla pedido
                
                _controlBD.InsertarDatos(@"insert into pedido(idSession,E_VBELN,I_AUART,I_BNDDT,I_KUNNR,I_KUNNR2,I_SPART,I_TEXTO,I_VKORG,
                                                      I_VTWEG,SOLICITADO_POR,FECHA_SOLICITUD,estado,FECHA_EXPIRACION,FECHA_EXPIRACION_SYS,SUCURSAL,
                                                      TOTAL_NETO,COMENTARIOS,E_VBELN_PEDIDO,I_AUART_PEDIDO,I_LPRIO_PEDIDO)
                                values('" + idSession + "' , '" + _pedido.ResultCotiza.NumCotizacion + "', '" + _creaCotizacion.ClaseDocVentas + "' , '" + _creaCotizacion.FecValides + "', '" + _creaCotizacion.CodClienteSap + "' , " +
                                    "        '" + _creaCotizacion.CodClienteSap2 + "' , '" + _creaCotizacion.SpartRep + "' , '" + _creaCotizacion.DescripCotizacion + "','" + _creaCotizacion.OrgVentas + "' , " +
                                    "        '" + _creaCotizacion.CDistribucion + "' , '" + rutUser + "' , GETDATE(), 'VIGENTE', '" + fechaFutura1 + "' , '" + fechaFutura2 + "' , " +
                                    "        '" + _sapApi.GetDestinatarioMercanciaByUserid(_sapApi.GetRutConcesionario(rutUser)) + "' , " +
                                    "        0, '" + _creaCotizacion.DescripCotizacion + "' , null , '"+tipoPed+"' , null )");

                //Codigo Antiguo con Variable "fechaFutura"
                /*_controlBD.InsertarDatos(@"insert into pedido(idSession,E_VBELN,I_AUART,I_BNDDT,I_KUNNR,I_KUNNR2,I_SPART,I_TEXTO,I_VKORG,
                                                      I_VTWEG,SOLICITADO_POR,FECHA_SOLICITUD,estado,FECHA_EXPIRACION,FECHA_EXPIRACION_SYS,SUCURSAL,
                                                      TOTAL_NETO,COMENTARIOS,E_VBELN_PEDIDO,I_AUART_PEDIDO,I_LPRIO_PEDIDO)
                                values('" + idSession + "' , '" + _pedido.ResultCotiza.NumCotizacion + "', '" + _creaCotizacion.ClaseDocVentas + "' , '" + _creaCotizacion.FecValides + "', '" + _creaCotizacion.CodClienteSap + "' , " +
                                  "        '" + _creaCotizacion.CodClienteSap2 + "' , '" + _creaCotizacion.SpartRep + "' , '" + _creaCotizacion.DescripCotizacion + "','" + _creaCotizacion.OrgVentas + "' , " +
                                  "        '" + _creaCotizacion.CDistribucion + "' , '" + rutUser + "' , '" + fechaFutura + "', 'VIGENTE', '" + fechaFutura1 + "' , '" + fechaFutura2 + "' , " +
                                  "        '" + _sapApi.GetDestinatarioMercanciaByUserid(_sapApi.GetRutConcesionario(rutUser)) + "' , " +
                                  "        0, '" + _creaCotizacion.DescripCotizacion + "' , null , '" + tipoPed + "' , null )");*/

                DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where E_VBELN = '" + _pedido.ResultCotiza.NumCotizacion + "' ");
                string idPedido = "";
                foreach (DataRow idP in ds.Tables[0].Rows)
                {
                    idPedido = idP["id_pedido"].ToString();
                }

                //Se inserta en tabla materiales_pedido
                _controlBD.InsertarDatos("update MATERIALES_PEDIDO set id_pedido = '" + idPedido + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
            }
            else {
                // Caso en que numero de cotización retorne vacío
                logger.Error("Error: " + _pedido.ResultCotiza.NumCotizacion + ". Favor reportar a SKBergé. Rogamos disculpar los inconvenientes");
                msjesError.InnerText = "Error: " + _pedido.ResultCotiza.NumCotizacion + ". Favor reportar a SKBergé. Rogamos disculpar los inconvenientes";
                msjesError.Visible = true;
                return;
            }

            string numCotizacion = _pedido.ResultCotiza.NumCotizacion;
            //txtNumCoti.Text = numCotizacion;

            MessageBox.Show("Su Pedido se ha generado exitosamente con el numero: " + numCotizacion);
            
            //Se calcula el valor total neto y se inserta en la tabla pedido
            double suma = 0;
            DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + Session["idSession"].ToString() + "'");
            foreach (DataRow row in dsT.Tables[0].Rows)
            {
                suma += double.Parse(row[8].ToString());
            }
            string sumaF = System.Convert.ToString(suma).ToString();

            _controlBD.InsertarDatos("update pedido set total_neto = " + "convert(decimal(18,2),replace('" + sumaF + "',',','.'))" + " where E_VBELN = '" + numCotizacion + "'");

            //Muestro el resumen de los repeustos cotizados
            DataSet cotizacion = _controlBD.ObtenerDatosFiltrados("select id_pedido, marca, codigo, descripcion, strGrupoMateriales,cantidad,stock,'$ ' + CAST(valor AS VARCHAR) AS 'Valor',total,cast(stock-cantidad as varchar) as diferencia from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN(numCotizacion) + "'");

            GridViewResumen.DataSource = cotizacion;
            GridViewResumen.DataBind();
            GridViewResumen.Visible = true;
        }
        else
        {
            msjesError.InnerText = "Error grave: " + _pedido.mensajeError + ". Porfavor reportar a SKBergé. Gracias";
            logger.Error("En [CotizarEnSap] error grave en [Cotizar.aspx]. " + _pedido.mensajeError);
            msjesError.Visible = true;
            //btnGenerarPedido.Enabled = false;
            btnGenerarPDF.Enabled = false;
            //btnDescartaCotizacion.Enabled = false;
            return;
        }

        //Se eliminan los datos de carro y de las listas
        _controlBD.InsertarDatos("delete from carro where idSession = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + Session["idSession"].ToString() + "'");
    }

    public void SinStock()
    {
        RutHelper _rutHelper = new RutHelper();
        ControlVfc _controlVFC = new ControlVfc();

        // Para el correo
        int cont = 1;
        String detallePedido = "";

        foreach (DataRow dr in dsRegistrosSinStock.Tables[0].Rows)
        {
            
            //Se declaran las variables de respuestas del WS
            List<String> identificadores = new List<String>();
            List<String> detalles = new List<String>();

            //Obtener los datos desde la grilla noStock
            //Obtener los datos seleccionados desde la grilla noStock
            string cantidad = dr["cantidad"].ToString();
            string codigo = dr["codigo"].ToString();
            string detalle = dr["descripcion"].ToString();
            string marca = _sapApi.GetCmpCod(dr["marca"].ToString());
            /*
             * Si se está en ambiente de desarrollo, los VFC se envían con RUT 22222222-2
             * como se acordó con Humano2
             * */
            string creador = "22222222-2";
            if(ConfigurationManager.AppSettings["ambiente"].ToString()=="p"){
                creador = _rutHelper.GetRutConDigito(Session["rut"].ToString());
            }
            string nombreUser = _sapApi.GetNombreUsuario(Session["rut"].ToString());
            string dealer = _sapApi.GetNombreDealer(Session["rut"].ToString());
            string tipoPedidoReserva = dr["tipoPedido"].ToString();
            string direccion = dr["destino"].ToString();
            string tipoPed = "RESERVA";
            string numBO = "";
            string numBO2 = "";
            string idPedido = Session["idSession"].ToString();
            string vin = dr["vfc"].ToString();
            string cc = _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString());
            string codsap = _sapApi.GetCodigoShipCode(Session["rut"].ToString());

            if (dr["descarte"].ToString() == "s")
            {
                _controlBD.InsertarDatos(@"insert into DESCARTADOS(id_pedido,rut_user,fechaDescarte,codigo_rep,marca,cantidad,detalle_rep)
                    values('" + idPedido + "','" + creador + "',GETDATE(),'" + codigo + "','" + marca + "'," + cantidad + ",'" + detalle + "')");
            }
            
            //Para backOrder///////////////////////////////////////////////////////////////////////////////////////
            else if (dr["reserva"].ToString() == "s")
            {
                //Si el repuesto fue checkeado para envio a BO se obtiene el detalle de dicho repuesto
                //y se envian los datos a la clase VFC para backoreder
                _controlVFC.hacerBo(cantidad, codigo, detalle, marca, creador, nombreUser, dealer, direccion, tipoPed, tipoPedidoReserva, "","",cc, codsap);

                identificadores = _controlVFC.getIdentificadores();

                numBO = identificadores[0];
                numBO2 = identificadores[1];

                //se insertan los datos en la tabla BACKORDER
                _controlBD.InsertarDatos(@"insert into BACKORDER(num_backOrder,id_pedido,rut_user,fecha_creacion,
                    codigo_rep,marca,cantidad,detalle_rep)
                    values('" + numBO.Substring(9, numBO.Length - 9) + "','" + idPedido + "','" + creador + "' , GETDATE(), " +
                    " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "')");

                // Para el correo
                detallePedido += cont + ". " + " | Reserva | " + numBO.Substring(9, numBO.Length - 9) + " | " + marca + " | " + codigo + " | " + cantidad + " | " + detalle + " | " + vin + " |\n";
                cont++;
            }
            
            //VFC
            else if (dr["vfc"].ToString() != "n")
            {
                //Si el repuesto fue checkeado para envio a VFC se obtiene el detalle de dicho repuesto
                //y se envian los datos a la clase VFC 
                tipoPed = "NORMAL";

                //Se llama al método que inserta el vfc
                _controlVFC.hacerVfc(cantidad, codigo, detalle, marca, vin, creador, nombreUser, dealer, direccion, tipoPed, tipoPedidoReserva,"","",cc, codsap);

                string numVFC = "";
                string numVFC2 = "";
                identificadores = _controlVFC.getIdentificadores();

                try
                {
                    numVFC = identificadores[0];
                    numVFC2 = identificadores[1];
                }
                catch (ArgumentOutOfRangeException)
                {
                    string error = "errorVfc";
                    Response.Redirect("Contacto.aspx?error="+error+"");
                }

                //se insertan los datos en la tabla VFC
                _controlBD.InsertarDatos(@"insert into VFC(num_VFC,id_pedido,rut_user,fecha_creacion,
                                                                codigo_rep,marca,cantidad,detalle_rep,cod_vin)
                                        values('" + numVFC.Substring(9, numVFC.Length - 9) + "','" + idPedido + "','" + creador + "' , GETDATE(), " +
                                        " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "','" + vin + "')");

                detallePedido += cont + ". " + " | VFC | " + numVFC.Substring(9, numVFC.Length - 9) + " | " + marca + " | " +  codigo + " | " + cantidad + " | " + detalle + " | " + vin + " |\n";
                cont++;
            }
        }
        //VerListasResumenes();

        // Se limpia tabla con repuestos sin stock para que no se vuelvan a mostrar
        _controlBD.InsertarDatos("delete from no_stock where idSession = '" + Session["idSession"].ToString() + "'");

        // Se envía correo con reservas 
        try
        {
            SendMail_helper _mail = new SendMail_helper();
            string para = _sapApi.GetCorreoUsuario(Session["rut"].ToString());
            string asunto = "SKBERGE: Solicitud de reserva confirmado [NO RESPONDER]";
            // Se envía un correo con los repuestos sin stock solicitados
            String textoCorreo = "";
            // Se traen solo VFCs y reservas solicitados en la sesión
            DataSet dsDetallePed = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + Session["idSession"].ToString() + "' and descarte='n'");
            if (cont > 1)
            {
                textoCorreo = "Usted ha realizado solicitud (VFCs) de repuestos a SKBergé. Le recordamos que las reservas se despacharán y facturarán una vez arribados los repuestos a nuestra bodega";
                textoCorreo += "\nConcesionario: " + _sapApi.GetNombreDealer(Session["rut"].ToString());
                textoCorreo += "\nDireccion sucursal: " + _sapApi.GetDireccionSucursalByShipCode(direccion);
                textoCorreo += "\n\nDetalle del pedido";
                textoCorreo += "\n" + detallePedido;
                _mail.EnviarCorreo(para, asunto, textoCorreo);
                _mail.EnviarCorreo("jcontreras@skbergeperu.com.pe", asunto, textoCorreo ); //Cambio Momentaneo
                _mail.EnviarCorreo("acumpa@skbergeperu.com.pe", asunto, textoCorreo); //Cambio Momentaneo
            }
        }
        catch (Exception ex)
        {
            logger.Error("En [SinStock] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            msjesError.InnerText = "Advertencia, hubo un problema en el envío de correo. Los pedidos se han procesado de todas maneras";
            msjesError.Visible = true;
        }

        // Se limpian las tablas de los sin stock (VFC, Reserva, descartados) para no vuelvan a molestar en otro pedido
        limpiaSinStocks();
    }

   /* public void VerListasResumenes()
    {
        DataSet dsBo = _controlBD.ObtenerDatosFiltrados("select * from BACKORDER where id_pedido = '" + Session["idSession"].ToString() + "'");
        DataSet dsVfc = _controlBD.ObtenerDatosFiltrados("select * from VFC where id_pedido = '" + Session["idSession"].ToString() + "'");
        DataSet dsDes = _controlBD.ObtenerDatosFiltrados("select * from DESCARTADOS where id_pedido = '" + Session["idSession"].ToString() + "'");

        if (dsBo.Tables[0].Rows.Count != 0)
        {
            GridViewReserva.DataSource = dsBo;
            GridViewReserva.DataBind();
            GridViewReserva.Visible = true;
        }
        else
        {
            GridViewReserva.DataSource = "";
            GridViewReserva.DataBind();
            GridViewReserva.Visible = true;
        }

        if (dsVfc.Tables[0].Rows.Count != 0)
        {
            GridViewVfc.DataSource = dsVfc;
            GridViewVfc.DataBind();
            GridViewVfc.Visible = true;
        }
        else
        {
            GridViewVfc.DataSource = "";
            GridViewVfc.DataBind();
            GridViewVfc.Visible = true;
        }

        if (dsDes.Tables[0].Rows.Count != 0)
        {
            GridViewDescartado.DataSource = dsDes;
            GridViewDescartado.DataBind();
            GridViewDescartado.Visible = true;
        }
        else
        {
            GridViewDescartado.DataSource = "";
            GridViewDescartado.DataBind();
            GridViewDescartado.Visible = true;
        }
    }*/

    public void limpiaSinStocks()
    {
        string numeroCotizacion = "0";
        //numeroCotizacion = txtNumCoti.Text;
        _controlBD.InsertarDatos("update vfc set id_pedido = '" + numeroCotizacion + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("update backorder set id_pedido = '" + numeroCotizacion + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("update descartados set id_pedido = '" + numeroCotizacion + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
    }

    public void PedidoSap()
    {
        string numCotiza = "";
        //string tipoPedido = "";//Normal o garantía
        string prioridad = "";

        //Extraer el codigo de cotización
        //numCotiza = txtNumCoti.Text;

        //Determinar prioridad de pedido: 2 es normal, 1 es alta
        //prioridad = RadioButtonListPrio.SelectedValue.ToString();

        // Se limpia las tablas VFC, BO y descartados, para que no vuelvan a aparecer los VFC,
        // reservas y descartados de un pedido anterior
        _controlBD.InsertarDatos("update vfc set id_pedido = '" + numCotiza + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("update backorder set id_pedido = '" + numCotiza + "' where id_pedido = '" + Session["idSession"].ToString() + "'");
        _controlBD.InsertarDatos("update descartados set id_pedido = '" + numCotiza + "' where id_pedido = '" + Session["idSession"].ToString() + "'");

        Response.Redirect("crearPedido.aspx?numeroCot="+numCotiza+"&tipoPedido="+tipoPed+"&prioridad="+prioridad+"");
    }

    #region Calcula el gran total de una columna
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

    protected void btnGenerarPedido_Click(object sender, EventArgs e)
    {
        PedidoSap();
    }

    protected void btnDescartaCotizacion_Click(object sender, EventArgs e)
    {
        string idSession = Session["idSession"].ToString();
        int stock = 0;
        string marca = "";

        // Obtengo la marca
        DataSet dsMar = _controlBD.ObtenerDatosFiltrados("select top 1 * from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN(""/*txtNumCoti.Text*/) + "'");
        foreach (DataRow drMarca in dsMar.Tables[0].Rows)
        {
            marca = drMarca["marca"].ToString();
        }


        // Rescatar desde la tabla MATERIALES_PEDIDO los codigos de los repuestos
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN(""/*txtNumCoti.Text*/) + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            // Se obtiene el grupo de material segun la marca seleccionada
            _consultaRep.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(marca);
            if (_consultaRep.GrupoMaterial == "")
            {
                MessageBox.Show("Error al obtener el grupo Materiales");
                return;
            }

            //Se obtiene el codigo del repuesto
            _consultaRep.CodRepuesto = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consultaRep.GrupoMaterial,marca) + dr["codigo"].ToString(); //se agrega marca

            _consultaRep.DestinaMercacia = direccion;

            if (_consultaRep.DestinaMercacia == "")
            {
                MessageBox.Show("ERROR, no se pudo obtener el prefijo de marca");
                return;
            }

            //Se obtiene el DocVentas y el canal de distribución
            _consultaRep.DocVentas = _sapApi.GetVkorgByGrupoMaterial(_consultaRep.GrupoMaterial,marca);
            _consultaRep.CanalDistribucion = _sapApi.CanalDeDistribucionPedido;
            _consultaRep.TextoRep = "";
            int cantidad = int.Parse(dr["cantidad"].ToString());

            if (!_pedido.BuscarRepuesto(_consultaRep, idSession, cantidad, "", marca))
            {
                MessageBox.Show("No hubo resultados en la búsqueda");
            }
            else
            {
                //Obtengo los datos desde la la tabla LISTA_BUSQUEDA_TMP
                DataSet dsR = _controlBD.ObtenerDatosFiltrados("select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + idSession + "'");
                foreach (DataRow drR in dsR.Tables[0].Rows)
                {
                    double totalC = double.Parse(drR["precion_conce"].ToString()) * int.Parse(drR["cantidad"].ToString());
                    double totalL = double.Parse(drR["precio_lista"].ToString()) * int.Parse(drR["cantidad"].ToString());

                    string totalCF = System.Convert.ToString(totalC);
                    string totalLF = System.Convert.ToString(totalL);

                    //Se insertan los datos en la tabla carro
                    stock = int.Parse(drR["stock"].ToString());
                    _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL)
                                   values('" + idSession + "','" + marca + "', '" + _sapApi.GetGrupoMaterialesByMarca(marca) + "','" + drR["codigo"].ToString() + "','" + drR["descrip"].ToString() + "', " +
                                            "  " + drR["cantidad"].ToString() + ", " + stock + " , " + "convert(float,replace('" + drR["precion_conce"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                            "  " + "convert(float,replace('" + drR["precio_lista"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ")");
                    string numCot = "";//txtNumCoti.Text;
                    string estado = "DESCARTADO";
                    _controlBD.InsertarDatos("update pedido set estado = '" + estado + "' where E_VBELN = '" + numCot.Trim() + "'");
                }
            }
        }
        limpiaSinStocks();
        Response.Redirect("buscarrepto2.aspx?visible=true");
    }

    protected void btnGenerarPDF_Click(object sender, EventArgs e)
    {
        string filePath = _pdf.CrearArchivoPdfCotizacion(""/*txtNumCoti.Text*/, Session["rut"].ToString());
        if (!File.Exists(filePath))
            throw new FileNotFoundException(string.Format("Final PDF file '{0}' was not found on disk.", filePath));
        var fi = new FileInfo(filePath);

        Response.Clear();
        Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", filePath));
        Response.AddHeader("Content-Length", fi.Length.ToString());
        Response.ContentType = "application/octet-stream";
        Response.WriteFile(fi.FullName);
        Response.End();
    }
	
	 protected void btnGenerarPdfCotizacion_Click(object sender, EventArgs e)
    {
        
		String namepdf = "Cotizacion_" + ""/*txtNumCoti.Text*/+".pdf";
		string filePath = _pdf.CrearArchivoPdfCotizacion(""/*txtNumCoti.Text*/, Session["rut"].ToString());
        if (!File.Exists(filePath))
            throw new FileNotFoundException(string.Format("Final PDF file '{0}' was not found on disk.", filePath));
        var fi = new FileInfo(filePath);

        Response.Clear();
        //Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", filePath));
		Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\""+ namepdf +"\"", filePath));
        Response.AddHeader("Content-Length", fi.Length.ToString()); 
        Response.ContentType = "application/octet-stream";
        Response.WriteFile(fi.FullName);
        Response.End();
	}	
}
