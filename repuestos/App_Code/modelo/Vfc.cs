using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using com.h2center.skberge;
//using cl.humano2.skbergecl;
using System.Xml;
using System.Xml.Linq;
using log4net;
using log4net.Config;
using System.Data.SqlClient;
using System.Configuration;
using SaveVFCPRO = pe.com.skberge.crm;
using pe.humano2.skberge;
using SaveVFCQA = QAS.PERU;

/// <summary>
/// Summary description for Vfc
/// </summary>
public class Vfc
{
    private String _usuario,_contrasena,_vin,_idEntity,_chasis,_anio,_marca,_modelo,_version;
    private String _usuarioVFC, _contrasenaVFC;
   // private ConsultaVin consultaVin;
    private SaveVFCPRO.SaveVFC saveVfc;
    private SaveVFCQA.SaveVFC saveVfcQa;
    private pe.humano2.skberge.ConsultaVFC consultaVFC;
    private List<registroSolicitud> registroSolicitudes;
    private List<detalleSolicitud> detalleSolicitudes;
    private List<String> identificadores;
    private List<String> detalles;

    SaveVFCPRO.SaveVFC save = new SaveVFCPRO.SaveVFC();
    SaveVFCQA.SaveVFC saveQa = new SaveVFCQA.SaveVFC();
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Persona));
	
	// Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

	public Vfc()
	{
        _usuario = "H2WSINTELLICORE";
        _contrasena = "h2ws";
        _usuarioVFC = "userVFC";
        _contrasenaVFC = "us3rVF";

        // Propiedades de la solicitud de detalle de vehiculo
        _vin = "";
        _idEntity = "";
        _chasis = "";
        _anio = "";
        _marca = "";
        _modelo = "";
        _version = "";

        identificadores = new List<String>();
        detalles = new List<String>();

       // consultaVin = new ConsultaVin();
        saveVfc = new SaveVFCPRO.SaveVFC();
        saveVfcQa = new SaveVFCQA.SaveVFC();
        consultaVFC = new pe.humano2.skberge.ConsultaVFC();
        registroSolicitudes = new List<registroSolicitud>();
        detalleSolicitudes = new List<detalleSolicitud>();
		
		 con = new SqlConnection();
        cmd = new SqlCommand();
	}

    public void limpiarIdentificadores()
    {
        identificadores.Clear();
    }

    //SETTER
    public void setUsuario(String usuario)
    {
        _usuario = usuario;
    }
    public void setContrasena(String contrasena)
    {
        _contrasena = contrasena;
    }
    public void setVin(String vin)
    {
        _vin = vin;
    }
    //GETTER
    public String getIdEntity()
    {
        return _idEntity;
    }
    public String getChasis()
    {
        return _chasis;
    }
    public String getAnio()
    {
        return _anio;
    }
    public String getVin()
    {
        return _vin;
    }
    public String getMarca()
    {
        return _marca;
    }
    public String getModelo()
    {
        return _modelo;
    }
    public String getVersion()
    {
        return _version;
    }

    public List<String> getIdentificadores()
    {
        return identificadores;
    }
    public List<String> getDetalles()
    {
        return detalles;
    }

    public List<detalleSolicitud> DetalleSolicitudes
    {
        get { return detalleSolicitudes; }
        set { detalleSolicitudes = value; }
    }

    /*private String getValidaVin()
    {
        //return consultaVin.validaVin(_usuario, _contrasena, _vin);
    }
    private String getObtieneVehiculo()
    {
        //return consultaVin.ObtieneVehiculo(_usuario, _contrasena, _idEntity);
    }*/

    //METODO DEL WS, LE PASO 3 PARAMETROS Y DEVUELVE UN XML
    private String getHacerVin(String xmlDatosAEnviar, String userVFC, String us3rVF)
    {
        try
        {
            string ambiente = "";
            ambiente = ConfAmbiente.ambiente;
            if (ambiente == "ERP")
            {
                return save.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
            }
            else if (ambiente == "ERQ")
            {
                return saveQa.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
            }
            else
            {
                return "";
            }

            //return save.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
        }
        catch 
        {
            return "";
        }
    }

    // Metodo para saber estado del VFCs y Reservas
    private String getSolicitudesRepuestos(string xmlDatosEntrada)
    {
        return consultaVFC.getSolicitudesRepuestos(xmlDatosEntrada, _usuarioVFC, _contrasenaVFC);
    }

    // Metodo para saber estado de una solicitud
    private ModeloConsultaVFC getSeguimientoRepuestos(string xmlDatosEntrada)
    {
        SeguimientoPedido seguimiento = new SeguimientoPedido();
        ModeloConsultaVFC Consulta = new ModeloConsultaVFC();

        Consulta = seguimiento.SeguimientoPedidoVFC(xmlDatosEntrada);
        return Consulta;
    }

    /*public void vin()
    {
        
        String datosXml = getValidaVin().ToString();
        XmlDocument xDoc = new XmlDocument();

        //La ruta del documento XML permite rutas relativas
        //respecto del ejecutable!

        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("resultados");

        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("resultado");

        try
        {
            _idEntity = lista[0].ChildNodes[0].InnerText;
            _vin = lista[0].ChildNodes[1].InnerText;
            _chasis = lista[0].ChildNodes[2].InnerText;
            _anio = lista[0].ChildNodes[3].InnerText;

        }
        catch (NullReferenceException ex)
        {
            logger.Error("NullReferenceException en [vin] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }*/

   /* public void obtieneVehiculo()
    {
        String datosXml = getObtieneVehiculo().ToString();
        XmlDocument xDoc = new XmlDocument();

        //La ruta del documento XML permite rutas relativas
        //respecto del ejecutable!

        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("vehiculo");

        try
        {

            XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("resultado");

            _anio = lista[0].ChildNodes[0].InnerText;

            _vin = lista[0].ChildNodes[1].InnerText;

            _chasis = lista[0].ChildNodes[2].InnerText;

            _marca = lista[0].ChildNodes[3].InnerText;

            _modelo = lista[0].ChildNodes[4].InnerText;

            _version = lista[0].ChildNodes[5].InnerText;
        }
        catch (NullReferenceException ex)
        {

            logger.Error("NullReferenceException en [obtieneVehiculo] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
        
    }*/

    private String crearXml(String cantidad,String codigo,String detalle,String marca,String vin, 
                            String creador,String descripcionUsuario,String dealer,
                            String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String CC, String codsap="")
    {
        //if (vin == "")
        //    vin = "1";

        XDocument miXML = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        new XComment("Lista de Registros"),
        new XElement("registros",
                            new XElement("registro",
                                new XElement("cantidad", cantidad),
                                new XElement("codigo", codigo),
                                new XElement("detalle", detalle),
                                new XElement("marca", marca),
                                new XElement("vin", vin),
                                new XElement("creador", creador),
                                new XElement("mailcreador", "mbascunan@h2center.com"),
                                new XElement("descripcionUsuario", descripcionUsuario),
                                new XElement("dealer", dealer),
                                new XElement("direccion", direccion),
                                new XElement("tipoPedido", tipoPedido),
                                new XElement("file", ""),
                                new XElement("opcionvfc", opcionvfc),
                                new XElement("km", km),
                                new XElement("nsiniestro", nSiniestro),
                                new XElement("CC", CC),
                                new XElement("codsap", codsap)
                            )
                    )
               );
        return miXML.ToString();

    }//fin metodoescribeXml



    private String crearXmlSolicitudRepuestos(String usuario, String fechaDesde, String fechaHasta,
                                                String concesionario, String local, String estado,
                                                String opcionvfc, String tipoPedido)
    {
        XDocument miXML = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        new XComment("Lista de Registros"),
        new XElement("registros",
                            new XElement("registro",
                                new XElement("usuario", usuario),
                                new XElement("fechaDesde", fechaDesde),
                                new XElement("fechaHasta", fechaHasta),
                                new XElement("concesionario", concesionario),
                                new XElement("local", local),
                                new XElement("estado", estado),
                                new XElement("opcionvfc", opcionvfc),
                                new XElement("tipoPedido", tipoPedido),  
                                 new XElement("id_solicitud", "")//nuevo - faltaba este tag para la invocacion - Nov 2021 MaikolQ
                            )
                    )
               );
        return miXML.ToString();

    }//fin metodoescribeXml

    private String crearXmlDetalleSolicitud(String idSolicitud)
    {

        XDocument miXML = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
       // new XComment("Lista de Registros"),
        new XElement("registros",
                            new XElement("registro",
                                new XElement("id_solicitud", idSolicitud)
                            )
                    )
               );
        return miXML.ToString();

    }//fin metodoescribeXml

    //HACER VFC METODO DE WS ESTE ES PARA VFC YA QUE RECIBE EL PARAMETRO VIN
    public void hacerVfc(String cantidad, String codigo, String detalle, String marca, String vin,
                         String creador, String descripcionUsuario, String dealer,
                         String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String CC="",String codsap="")
    {
        //LLAMO A MI METODO CREADO ANTERIORMENTE EL CUAL LLAMA AL METODO DEL WS, Y TOMO EL RETORNO(XML) EN UN STRING
        //'datosXml' PARA POSTERIORMENTE PARSEARLO Y TIRARLO A UNA LISTA
        /*le entrego como primer parametro el xml que pide el ws el cual es creado en el metodo 'crearXml' con los datos recibidos y devuelto en un String no en un xml
         ya que asi lo pide el ws*/
        //Cambios antes llamaha gethacervin
        String datosXml = "";
        string ambiente = "";
        ambiente = ConfAmbiente.ambiente;
        if (ambiente == "ERP")
        {
            datosXml = save.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), marca, vin, Util.limpiaPalabras(creador), Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), km, nSiniestro, CC, codsap), "userVFC", "us3rVF").ToString();
        }
        else if (ambiente == "ERQ")
        {
            datosXml = saveQa.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), marca, vin, Util.limpiaPalabras(creador), Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), km, nSiniestro, CC, codsap), "userVFC", "us3rVF").ToString();
        }

        //datosXml = save.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), marca, vin, Util.limpiaPalabras(creador), Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), km, nSiniestro, CC, codsap), "userVFC", "us3rVF").ToString();
        XmlDocument xDoc = new XmlDocument();

        if (datosXml == "")
        {
            return;
        }
        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("sucesos");

        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("suceso");


        foreach (XmlElement nodo in lista)
        {
            //XmlNodeList identificador = nodo.GetElementsByTagName("identificador");

            //XmlNodeList detalle = nodo.GetElementsByTagName("detalle");
            identificadores.Add(nodo.ChildNodes[0].InnerText);
            detalles.Add(nodo.ChildNodes[1].InnerText);
        }
        
        
    }
    //HACER VFC METODO DE WS ESTE ES PARA BO YA QUE NO RECIBE EL PARAMETRO VIN
    public void hacerVfc(String cantidad, String codigo, String detalle, String marca,
                        String creador, String descripcionUsuario, String dealer,
                        String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String CC = "", String codsap = "")
    {
        //LLAMO A MI METODO CREADO ANTERIORMENTE EL CUAL LLAMA AL METODO DEL WS, Y TOMO EL RETORNO(XML) EN UN STRING
        //'datosXml' PARA POSTERIORMENTE PARSEARLO Y TIRARLO A UNA LISTA
        /*le entrego como primer parametro el xml que pide el ws el cual es creado en el metodo 'crearXml' con los datos recibidos y devuelto en un String no en un xml
         ya que asi lo pide el ws*/
        //Cambios antes llamaha gethacervin

        String datosXml = "";
        string ambiente = "";
        ambiente = ConfAmbiente.ambiente;
        if (ambiente == "ERP")
        {
            datosXml = save.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), Util.limpiaPalabras(marca), "1", Util.limpiaPalabras(creador), Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), Util.limpiaPalabras(km), Util.limpiaPalabras(nSiniestro), Util.limpiaPalabras(CC), Util.limpiaPalabras(codsap)), "userVFC", "us3rVF").ToString();
        }
        else if (ambiente == "ERQ")
        {
            datosXml = saveQa.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), Util.limpiaPalabras(marca), "", Util.limpiaPalabras(creador), Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), Util.limpiaPalabras(km), Util.limpiaPalabras(nSiniestro), Util.limpiaPalabras(CC), Util.limpiaPalabras(codsap)), "userVFC", "us3rVF").ToString();
        }

        //String datosXml = save.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), Util.limpiaPalabras(marca), "", Util.limpiaPalabras(creador), Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), Util.limpiaPalabras(km), Util.limpiaPalabras(nSiniestro), Util.limpiaPalabras(CC), Util.limpiaPalabras(codsap)), "userVFC", "us3rVF").ToString();
        XmlDocument xDoc = new XmlDocument();

        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("sucesos");
        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("suceso");

        foreach (XmlElement nodo in lista)
        {
            identificadores.Add(nodo.ChildNodes[0].InnerText);
            detalles.Add(nodo.ChildNodes[1].InnerText);
        }
    }

    

    public void hacerBo(String cantidad, String codigo, String detalle, String marca, 
                         String creador, String descripcionUsuario, String dealer,
                         String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String CC = "", String codsap = "")
    {
        //LLAMO A MI METODO CREADO ANTERIORMENTE EL CUAL LLAMA AL METODO DEL WS, Y TOMO EL RETORNO(XML) EN UN STRING
        //'datosXml' PARA POSTERIORMENTE PARSEARLO Y TIRARLO A UNA LISTA
        /*le entrego como primer parametro el xml que pide el ws el cual es creado en el metodo 'crearXml' con los datos recibidos y devuelto en un String no en un xml
         ya que asi lo pide el ws*/

        String datosXml = "";
        string ambiente = "";
        ambiente = ConfAmbiente.ambiente;
        if (ambiente == "ERP")
        {
            datosXml = save.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), marca, "", creador, Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), km, nSiniestro, Util.limpiaPalabras(CC), Util.limpiaPalabras(codsap)), "userVFC", "us3rVF").ToString();
        }
        else if (ambiente == "ERQ")
        {
            datosXml = saveQa.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), marca, "", creador, Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), km, nSiniestro, Util.limpiaPalabras(CC), Util.limpiaPalabras(codsap)), "userVFC", "us3rVF").ToString();
        }


        //Cambios antes llamaha gethacervin
        //String datosXml = save.setVFC(crearXml(cantidad, codigo, Util.limpiaPalabras(detalle), marca, "", creador, Util.limpiaPalabras(descripcionUsuario), Util.limpiaPalabras(dealer), Util.limpiaPalabras(direccion), Util.limpiaPalabras(tipoPedido), Util.limpiaPalabras(opcionvfc), km, nSiniestro, Util.limpiaPalabras(CC), Util.limpiaPalabras(codsap)), "userVFC", "us3rVF").ToString();
        XmlDocument xDoc = new XmlDocument();

        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("sucesos");

        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("suceso");

        foreach (XmlElement nodo in lista)
        {
            identificadores.Add(nodo.ChildNodes[0].InnerText);
            detalles.Add(nodo.ChildNodes[1].InnerText);
        }
    }

    public void obtenerSolicitudes(String usuario, String fechaDesde, String fechaHasta, String concesionario,
                                    String local, String estado, String opcionvfc, String tipoPedido)
    {
        String datosXml = getSolicitudesRepuestos(crearXmlSolicitudRepuestos(usuario, fechaDesde, fechaHasta, concesionario, local, estado, opcionvfc, tipoPedido)).ToString();
        XmlDocument xDoc = new XmlDocument();

        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("solicitudes");
        XmlNodeList lista = null;

        if (resultados.Count > 0)
        {
            lista = ((XmlElement)resultados[0]).GetElementsByTagName("solicitud");
            foreach (XmlElement nodo in lista)
            {
                registroSolicitud nuevoRegistro = new registroSolicitud();

                //<id_solicitud>3877597</id_solicitud>
                nuevoRegistro.Id_solicitud = nodo.ChildNodes[0].InnerText;

                //<fecha_creacion>27/09/2011</fecha_creacion>
                nuevoRegistro.Fecha_creacion = nodo.ChildNodes[1].InnerText;

                //<cantidad>1</cantidad>
                nuevoRegistro.Cantidad = nodo.ChildNodes[2].InnerText;

                //<producto>8321A097</producto>
                nuevoRegistro.Producto = nodo.ChildNodes[3].InnerText;

                //<vin>MMBJNKB408D067564</vin>
                nuevoRegistro.Vin = nodo.ChildNodes[4].InnerText;

                //<marca>MMCC</marca>
                nuevoRegistro.Marca = nodo.ChildNodes[5].InnerText;

                //<creador>7954759-K</creador>
                nuevoRegistro.Creador = nodo.ChildNodes[6].InnerText;

                //<fecha_eta>28/10/2011</fecha_eta>
                nuevoRegistro.Fecha_eta = nodo.ChildNodes[7].InnerText;

                //<desc_usuario>PORFIRIO MONTECINOS</desc_usuario>
                nuevoRegistro.Desc_usuario = nodo.ChildNodes[8].InnerText;

                //<dealer>SERVITAL ATOMOTRIZ S.A</dealer>
                nuevoRegistro.Dealer = nodo.ChildNodes[9].InnerText;

                //<direccion>SERVITAL</direccion>
                nuevoRegistro.Direccion = nodo.ChildNodes[10].InnerText;

                //<estado>EN PROCESO</estado>
                nuevoRegistro.Estado = nodo.ChildNodes[11].InnerText;

                //<concesionario>SERVITAL AUTOMOTRIZ S.A.</concesionario>
                nuevoRegistro.Concesionario = nodo.ChildNodes[12].InnerText;

                //<local>SERVITAL ATOMOTRIZ S.A</local>
                nuevoRegistro.Local = nodo.ChildNodes[13].InnerText;

                //<tipovfc>NORMAL</tipovfc>
                nuevoRegistro.Tipovfc = nodo.ChildNodes[14].InnerText;

                // Tipo de pedido (reserva o normal)
                nuevoRegistro.TipoPedido = "";// nodo.ChildNodes[15].InnerText;

                registroSolicitudes.Add(nuevoRegistro);
            }
        }

        


        
    }

    public void obtenerDetalleSolicitud(String idSolicitud)
    {
        ModeloSeguimiento Modelo = new ModeloSeguimiento();
        ModeloConsultaVFC datosXml = getSeguimientoRepuestos(crearXmlDetalleSolicitud(idSolicitud));
        XmlDocument xDoc = new XmlDocument();

        foreach (var nodo in datosXml.seguimientos)
        {
            detalleSolicitud nuevoDetalle = new detalleSolicitud();

            //<id_solicitud>3877561</id_solicitud>
            nuevoDetalle.Id_solicitud = nodo.id.ToString();

            //<id_seguimiento>3877562</id_seguimiento>
            nuevoDetalle.Id_seguimiento = nodo.idSolicitudVFC.ToString(); ;

            //<marca>BC02</marca>
            nuevoDetalle.Marca = nodo.idMarca.ToString();

            //<codigo_repuesto>CBN1L251AA</codigo_repuesto>
            nuevoDetalle.Codigo_repuesto = nodo.codigoRepuesto.ToString(); ;

            //<detalle>ESPACIADOR</detalle>
            nuevoDetalle.Detalle = nodo.fechaETA.ToString();

            //<fecha_movimiento>27/09/2011 18:39:59</fecha_movimiento>
            nuevoDetalle.Fecha_movimiento = nodo.fechaCreacion.ToString();

            //<estado>PENDIENTE</estado>
            nuevoDetalle.Estado = nodo.nuevoEstado.ToString();

            ////<fecha_eta>28/10/2011</fecha_eta>
            nuevoDetalle.Fecha_eta = nodo.fechaETA.ToString();

            ////<orden_compra></orden_compra>
            //nuevoDetalle.Orden_compra = nodo.ChildNodes[8].InnerText;

            //<orden_desarme></orden_desarme>
            //var algo = nodo.ChildNodes[10];
            //string untexto = "";
            //if (algo != null)
            //{
            //    untexto = nodo.ChildNodes[10].InnerText;
            //}
            //nuevoDetalle.Orden_desarme = nodo.ChildNodes[9].InnerText;

            ////observacion
            nuevoDetalle.Observacion = nodo.detalle.ToString();

            detalleSolicitudes.Add(nuevoDetalle);
        }
    }

    public List<registroSolicitud> RegistroSolicitudes
    {
        get { return registroSolicitudes; }
        set { registroSolicitudes = value; }
    }
	
	public String VinGrupoporMarca(String marca)
    {
        string grupo = "";
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select isnull(VKORG,0) from GRUPO_MATERIALES where MARCA = '" + marca + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        grupo = dr[0].ToString();
        dr.Close();
        con.Close();

        return grupo;
    }	
}