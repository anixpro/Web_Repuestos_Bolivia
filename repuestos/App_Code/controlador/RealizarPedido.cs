using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;

/// <summary>
/// Descripción breve de Pedido
/// </summary>
///                 
/// 

public class RealizarPedido
{



    //Variables para BD
    ControlBD _controlBd = new ControlBD();
    SapAPI _sapApi = new SapAPI();

    private Persona persona;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(RealizarPedido));

    //Variables de respuesta
    RespConsultaRepuesto _resultBusqueda;
    RespCadenaReemplazo _resultReemplazos;
    RespCotizacionSap _resultCotiza;
    RespConfirmarPedido _resultPedido;
    RespEstadoPed _estadoPed;
    public Boolean disponibilidadServicio { get; set; }
    public Boolean disponibilidadRepuesto { get; set; }
    public Boolean disponibilidadRepuestoSap { get; set; }
    public Boolean disponibilidadVFC { get; set; }

    public int stockSap_Origen { get; set; }

    // Datos de autenticacion al WS
    private List<string> _cadenaReemplazo;
    private List<string> _errores;

    // propiedad para desplegar error
    public String mensajeError { get; set; }

    //Variables de ambiente
    AmbienteERP _erp = new AmbienteERP();
    AmbienteERQ _erq = new AmbienteERQ();
    string ambiente = "";
    string Centro;
    string PdocV;
    string PdocVF;

    //envio correo 
    SendMail_helper _mail = new SendMail_helper();


    public RealizarPedido()
    {
        //Obtengo el valor para el ambiente de trabajo
        ConfAmbiente.ConfCredenciales();
        ambiente = ConfAmbiente.ambiente;
        disponibilidadServicio = true;
    }



    /// <summary>
    /// Método que busca un repuesto desde SAP
    /// </summary>
    /// <param name="_consultaRep"></param>
    /// <returns></returns>
    public bool BuscarRepuesto(ConsultaRepuesto _consultaRep, string idSession, int cantidad, string nombre, string marca, out int activofrec)
    {


        _controlBd.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + idSession + "'");
        _controlBd.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + idSession + "'");
        activofrec = 0;

        if (ambiente == "ERP")
        {
            #region Ambiente ERP
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();

                ///////////////////Declaración de variables de acceso a Sap (AMBIENTE DE PRODUCCION)///////////
                //Se asignan los datos basicos a cada item



                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }
                /*if (_controlMarca.obtenerMarcaPorNombre(marca).esForaneo)
                {
                    _erq.LocalDtWebRepuestosItem.I_FORANEO = "X";
                }*/

                //Se asignan los item a la estructura de consulta
                //_erq.LocalDtWebRepuestos = _erq.LocalDtWebRepuestosItem;
                _erp.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential("INT_WTY_SKBP", "5k82017PoPpe");
                _erp.WsConsultaRepuesto.PreAuthenticate = true;

                //Envio todos los datosa para la consulta al WS
                //_erq.WsResponseConsultaRepuesto = _erq.WsConsultaRepuesto.MI_WebS_Repuestos_Synch(_erq.LocalDtWebRepuestos);

                string precio1 = "", precio2 = "", precio1R = "", precio2R = "";
                double totalLista = 0, totalConce = 0, totalListaR = 0, totalConceR = 0;
                int decimalPosition = 0, decimalPositionR = 0;

                String dato1; //E_KWMENG
                String dato2;
                String dato3; //E_MFRPN
                String dato4; //E_MVGR4
                String dato5; //E_VKORG
                String dato6; //E_VTWEG

                //Variable utilizada para agregar * al texto de busqueda
                String TxtRptoP = "";



                if (_consultaRep.TextoRep != "")
                {
                    TxtRptoP = "*" + _consultaRep.TextoRep + "*";
                }



                //Declaracion de arreglos para recepcion de datos
                var MyArray = new ERP.ZEWS004[1];
                var MyArray1 = new ERP.ZEWS026[1];
                var MyArray2 = new ERP.ZEWS027[1];
                String MVGR1 = "";



                //Consulta WS Consulta Repuesto 
                _erp.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("",
                                                                _consultaRep.DestinaMercacia,
                                                                "1",
                                                                TxtRptoP,
                                                                _consultaRep.CodRepuesto,
                                                                _consultaRep.GrupoMaterial,
                                                                "",
                                                                _consultaRep.DocVentas,
                                                                _consultaRep.CanalDistribucion,
                                                                out dato1, //E_KWMENG
                                                                out dato2,
                                                                out dato3, //E_MFRPN
                                                                out dato4, //E_MVGR4
                                                                out dato5, //E_VKORG
                                                                out dato6, //E_VTWEG
                                                                out MyArray,
                                                                out MyArray1,
                                                                out MyArray2);

                /* _erq.WsConsultaRepuesto.EndSI_ConsultaRepuesto_oa(ar,
                                                                      out dato1,
                                                                      out dato2,
                                                                      out dato3,
                                                                      out dato4,
                                                                      out dato5,
                                                                      out dato6,
                                                                      out MyArray, out MyArray1, out MyArray2);*/




                foreach (ERP.ZEWS026 WsDatos in MyArray1)
                {

                    //String a = WsDatos.EZ_MVGR1;
                    _resultBusqueda.Marca = "CHERY";

                    _resultBusqueda.Cantidad = "1";
                    _resultBusqueda.PrecioLista = "1000";
                    _resultBusqueda.PrecioConce = "1000";
                    _resultBusqueda.GrupoMat = "Z2";
                    _resultBusqueda.Descripcion = "ALTERNADOR MOTOR";
                    _resultBusqueda.Codigo = "x22333";
                    _resultBusqueda.Stock =  _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                    stockSap_Origen = _resultBusqueda.Stock;
                    MVGR1 = "1"; //REPERESENTACION DE LA MARCA;

                    //decimalPosition = ResultBusqueda.PrecioLista.IndexOf(".");
                    //decimalPosition = ResultBusqueda.PrecioConce.IndexOf(".");

                    //if(_resultBusqueda.Stock > 0){
                    //if (decimalPosition >= 0)
                    //{
                    //precio1 = ResultBusqueda.PrecioLista.Remove(decimalPosition, 0).Trim();
                    // precio2 = ResultBusqueda.PrecioConce.Remove(decimalPosition, 0).Trim();

                    precio1 = ResultBusqueda.PrecioLista;
                    precio2 = ResultBusqueda.PrecioConce;
                    // }
                    totalConce = double.Parse(precio2) * int.Parse(_resultBusqueda.Cantidad);
                    totalLista = double.Parse(precio1) * int.Parse(_resultBusqueda.Cantidad);//descomentar
                    //Nueva Funcionalidad Porcentaje
                    /*string porce;
                    string[] deml;
                    string aux;
                    int dat = 0;
                    double descuento = 0;
                    double de = 0;
                    de = totalLista - totalConce;
                    if (de == 0)
                    {
                        porce = "0";
                    }
                    else
                    {
                        de = de / totalLista;
                        if (de == 0)
                        {
                            porce = "0";
                        }
                        if (de.ToString().Length < 4)
                        {
                            descuento = de * 100;
                        }
                        if (de.ToString().Length > 4)
                        {
                            //descuento = Convert.ToDouble(de.ToString().Remove(4, 16)) * 100;
                            string dede = Convert.ToString(de);
                            deml = dede.Split(',');
                            aux = deml[1];
                            if (deml[1].Length > 2)
                            {
                                aux = aux.Substring(0, 2);
                                int daux = Convert.ToInt32(aux.Substring(1, 1));
                                if (daux <= 9 && daux > 5)
                                {
                                    dat = Convert.ToInt32(aux);
                                    dat = dat + 1;
                                    descuento = Convert.ToDouble(dat);
                                }
                                if (daux < 5 && daux > 0)
                                {
                                    dat = Convert.ToInt32(aux);
                                    descuento = Convert.ToDouble(dat);
                                }
                                if (daux == 0)
                                {
                                    descuento = Convert.ToDouble(aux);
                                }
                            }
                        }
                        else
                        {
                            descuento = de * 100;
                        }
                    }
                    string final;
                    int des = 0;
                    int valor = Convert.ToInt32(descuento);
                    if (valor < 10)
                    {
                        porce = Convert.ToString(valor) + "0";
                    }
                    else
                    {
                        porce = Convert.ToString(descuento);
                        if (porce.ToString().Contains(','))
                        {
                            string[] split = porce.Split(',');
                            des = Convert.ToInt32(split[1]);
                            if (des < 10 || des > 0)
                            {
                                des = Convert.ToInt32(split[0]);
                                des = des + 1;
                            }
                        }
                        int num = Convert.ToInt32(porce);
                        if (num > 5 && num < 10)
                        {
                            num = 10;
                            porce = Convert.ToString(num);
                        }
                        if (num <= 5 && num >= 1)
                        {
                            num = 5;
                            porce = Convert.ToString(num);
                        }
                        if (num == 0)
                        {
                            porce = "0";
                        }
                        else
                        {
                            porce = Convert.ToString(num);
                        }
                    }*/
                    int porce = 0;
                    String final = Convert.ToString(porce) + "%";


                    double totalLista_F = (double.Parse(precio1) * porce) / 100;
                    double totalConce_F = double.Parse(precio1) - totalLista_F;
                    //END FUNCIONALIDAD PORCENTAJE

                    String MVGR1_F = _controlBd.ObtieneGrupoMateriales(MVGR1);

                    if (MVGR1_F == "" || MVGR1_F == "000")
                    {
                        MVGR1_F = "";
                        MVGR1_F = "S/G";
                    }


                    _controlBd.InsertarDatos(@"insert into LISTA_BUSQUEDA_TMP(ID_SESSION,MARCA,CANTIDAD,PRECIO_LISTA,PRECION_CONCE,GRUPO_MAT,DESCRIP,CODIGO,STOCK,TOTAL_C,TOTAL_L, DESCUENTO,GRUPO)
                                  values ('" + idSession + "', '" + _resultBusqueda.Marca + "' , '" + cantidad + "' , '" + precio1 + "' , '" + Math.Round(totalConce_F, 2) + "' , '" + _resultBusqueda.GrupoMat + "' , '" + _resultBusqueda.Descripcion + "' , '" + _resultBusqueda.Codigo.Substring(3) + "','" + ResultBusqueda.Stock + "' , '" + totalConce + "', '" + totalLista + "','" + final + "'," + "'" + MVGR1_F + "')");
                }

                //Se crea un DataTable para mostrar los datos de la consulta del repuesto
                if (_erp.WsResponseConsultaRepuesto != null)
                {
                    foreach (ERP.ZEWS027 WsDatos2 in MyArray2)
                    {
                        _resultReemplazos.Marca = marca.ToUpper();
                        _resultReemplazos.T_INTTYPE = WsDatos2.T_INTTYPE;
                        _resultReemplazos.T_KBETR1 = WsDatos2.T_KBETR1;
                        _resultReemplazos.Cantidad = _consultaRep.CantidadRep.ToString();
                        //_resultReemplazos.T_KBETR2 = WsDatos2.T_KBETR2;
                        _resultReemplazos.T_KONDM = WsDatos2.T_KONDM;
                        _resultReemplazos.T_MAKTX = WsDatos2.T_MAKTX;
                        _resultReemplazos.T_MFRPN = WsDatos2.T_MFRPN;
                        _resultReemplazos.Stock = _sapApi.GetCurrentStockByProduct(_resultReemplazos.T_MFRPN, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                        String T_MVGR1 = WsDatos2.T_MVGR1;


                        //decimalPositionR = ResultReemplazos.T_KBETR2.IndexOf(".");
                        //decimalPositionR = ResultReemplazos.T_KBETR1.IndexOf(".");
                        //if (decimalPositionR >= 0)
                        // {
                        precio1R = ResultReemplazos.T_KBETR1;
                        // }

                        //totalConceR = int.Parse(precio2R) * cantidad;
                        totalListaR = double.Parse(precio1R, System.Globalization.CultureInfo.InvariantCulture) * cantidad;

                        int porce = 30;
                        double precio1R_F = (double.Parse(precio1R, System.Globalization.CultureInfo.InvariantCulture) * porce) / 100;
                        double precio1R_FF = double.Parse(precio1R, System.Globalization.CultureInfo.InvariantCulture) - precio1R_F;

                        String T_MVGR1_F = _controlBd.ObtieneGrupoMateriales(T_MVGR1);

                        if (T_MVGR1_F == "" || T_MVGR1_F == "000")
                        {
                            T_MVGR1_F = "";
                            T_MVGR1_F = "S/G";
                        }

                        _controlBd.InsertarDatos(@"insert into LISTA_REEMPLAZO_TMP(ID_SESSION,MARCA,CANTIDAD,T_INTTYPE,T_KBETR1,T_KBETR2,T_KONDM,T_MAKTX,T_MFRPN,TOTAL_C,TOTAL_L,STOCK,GRUPO)
                                values( '" + idSession + "' , '" + _resultReemplazos.Marca + "' , '" + _resultReemplazos.Cantidad + "' , '" + _resultReemplazos.T_INTTYPE + "' , '" + precio2R + "' , '" + Math.Round(precio1R_FF, 2) + "' , '" + _resultReemplazos.T_KONDM + "' , '" + _resultReemplazos.T_MAKTX + "' , '" + _resultReemplazos.T_MFRPN.Substring(3) + "' , '" + totalConceR + "', '" + totalListaR + "' , '" + _resultReemplazos.Stock + "','" + T_MVGR1_F + "')");
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            disponibilidadVFC = false;
            disponibilidadRepuesto = true;
            if ((_erq.LocalDtWebRepuestosItem == null) && (_erq.WsResponseConsultaRepuesto == null))
            {
                logger.Error("Error: Repuesto no existe en SAP");
                mensajeError = "ERROR Repuesto no existe en SAP";
                //disponibilidadServicio = false;
                disponibilidadRepuestoSap = false;
                return false;
            }//stock = 0 y precio=0 || _resultBusqueda.PrecioConce.Trim() == "0.00" || _resultBusqueda.PrecioLista.Trim() == "0.00"
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock == 0))
            {
                disponibilidadVFC = true; //MUESTRA MENSAJE vfc
                return true;
            }
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock > 0) && (_resultBusqueda.PrecioConce.Trim() == "0.00") && (_resultBusqueda.PrecioLista.Trim() == "0.00"))
            {
                logger.Error("Repuesto sin precio. Comuníquese con el supervisor");
                mensajeError = "Repuesto sin precio. Comuníquese con el supervisor";
                disponibilidadRepuesto = false; //muestra msj
                return true;
            }
            else
            {
                return true;
            }
            #endregion
        }

        else if (ambiente == "ERQ")
        {

            #region Ambiente ERQ
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();

                // REQ - PRECIO FIJOS DE REPUESTOS -  MARZO 2022

                //Consultamos Material
                string tiene_stock_a = "";
                string tiene_stock_b = "";
                string tiene_cadena_r = "";

                //VALIDA ACTIVA FECUENCIA Material      
                int activo_frecuencia = _controlBd.activafrecuencia(_consultaRep.CodRepuesto, marca);

                //VALIDA ACTIVA FECUENCIA Material  
                // REQ - PRECIO FIJOS DE REPUESTOS -  MARZO 2022

                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }

                //Se asignan los item a la estructura de consulta
                _erq.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential("INT_WTY_SKB", "skb2017poq");
                _erq.WsConsultaRepuesto.PreAuthenticate = true;

                //Envio todos los datosa para la consulta al WS
                //_erq.WsResponseConsultaRepuesto = _erq.WsConsultaRepuesto.MI_WebS_Repuestos_Synch(_erq.LocalDtWebRepuestos);

                string precio1 = "", precio2 = "", precio1R = "", precio2R = "";
                double totalLista = 0, totalConce = 0, totalListaR = 0, totalConceR = 0;
                int decimalPosition = 0, decimalPositionR = 0;

                String dato1; //E_KWMENG
                String dato2;
                String dato3; //E_MFRPN
                String dato4; //E_MVGR4
                String dato5; //E_VKORG
                String dato6; //E_VTWEG

                //Variable utilizada para agregar * al texto de busqueda
                String TxtRptoP = "";



                if (_consultaRep.TextoRep != "")
                {
                    TxtRptoP = "*" + _consultaRep.TextoRep + "*";
                }



                //Declaracion de arreglos para recepcion de datos
                var MyArray = new ERQ.ZEWS004[1];
                var MyArray1 = new ERQ.ZEWS026[1];
                var MyArray2 = new ERQ.ZEWS027[1];
                String MVGR1 = "";



                //Consulta WS Consulta Repuesto 
                _erq.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("",
                                                                _consultaRep.DestinaMercacia,
                                                                "1",
                                                                TxtRptoP,
                                                                _consultaRep.CodRepuesto,
                                                                _consultaRep.GrupoMaterial,
                                                                "",
                                                                _consultaRep.DocVentas,
                                                                _consultaRep.CanalDistribucion,
                                                                out dato1, //E_KWMENG
                                                                out dato2,
                                                                out dato3, //E_MFRPN
                                                                out dato4, //E_MVGR4
                                                                out dato5, //E_VKORG
                                                                out dato6, //E_VTWEG
                                                                out MyArray,
                                                                out MyArray1,
                                                                out MyArray2);
                

                if (MyArray1 != null)
                {
                    foreach (ERQ.ZEWS026 WsDatos in MyArray1)
                    {
                        _resultBusqueda.Marca = marca.ToUpper();

                        _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString();
                        _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                        _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                        _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                        _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                        _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                        _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                        stockSap_Origen = _resultBusqueda.Stock;

                        MVGR1 = WsDatos.EZ_MVGR1;

                        // REQ - PRECIO FIJOS DE REPUESTOS -  MARZO 2022
                        //Inicio Frecuencia A y B
                        if (_resultBusqueda.Stock == 0 && activo_frecuencia == 1)
                        {
                            activofrec = 1;
                            _resultBusqueda.Stock = 1;
                        }
                        //Fin Frecuencia A y B
                        if (_resultBusqueda.Stock == 0 && _consultaRep.CodRepuesto != "")
                        {
                            tiene_stock_a = "NO";
                        }

                        decimalPosition = ResultBusqueda.PrecioLista.IndexOf(".");

                        if (decimalPosition >= 0)
                        {
                            precio1 = ResultBusqueda.PrecioLista.Remove(decimalPosition, 3).Trim();
                        }

                        decimalPosition = ResultBusqueda.PrecioConce.IndexOf(".");

                        if (decimalPosition >= 0)
                        {
                            precio2 = ResultBusqueda.PrecioConce.Remove(decimalPosition, 3).Trim();
                        }

                        totalConce = int.Parse(precio2) * int.Parse(_resultBusqueda.Cantidad);
                        totalLista = int.Parse(precio1) * int.Parse(_resultBusqueda.Cantidad);
                        // REQ - PRECIO FIJOS DE REPUESTOS -  MARZO 2022

                        precio1 = ResultBusqueda.PrecioLista;
                        precio2 = ResultBusqueda.PrecioConce;
                        // }
                        totalConce = double.Parse(precio2, System.Globalization.CultureInfo.InvariantCulture) * int.Parse(_resultBusqueda.Cantidad);
                        totalLista = double.Parse(precio1, System.Globalization.CultureInfo.InvariantCulture) * int.Parse(_resultBusqueda.Cantidad);//descomentar

                        int porce = 0;
                        String final = Convert.ToString(porce) + "%";


                        double totalLista_F = (double.Parse(precio1, System.Globalization.CultureInfo.InvariantCulture) * porce) / 100;
                        double totalConce_F = double.Parse(precio1, System.Globalization.CultureInfo.InvariantCulture) - totalLista_F;
                        //END FUNCIONALIDAD PORCENTAJE

                        String MVGR1_F = _controlBd.ObtieneGrupoMateriales(MVGR1);

                        if (MVGR1_F == "" || MVGR1_F == "000")
                        {
                            MVGR1_F = "";
                            MVGR1_F = "S/G";
                        }

                        _controlBd.InsertarDatos(@"insert into LISTA_BUSQUEDA_TMP(ID_SESSION,MARCA,CANTIDAD,PRECIO_LISTA,PRECION_CONCE,GRUPO_MAT,DESCRIP,CODIGO,STOCK,TOTAL_C,TOTAL_L, DESCUENTO,GRUPO)
                                  values ('" + idSession + "', '" + _resultBusqueda.Marca + "' , '" + cantidad + "' , '" + precio1 + "' , '" + totalConce_F + "' , '" + _resultBusqueda.GrupoMat + "' , '" + _resultBusqueda.Descripcion + "' , '" + _resultBusqueda.Codigo.Substring(3) + "','" + ResultBusqueda.Stock + "' , '" + totalConce + "', '" + totalLista + "','" + final + "'," + "'" + MVGR1_F + "')");
                    }
                }


                //Se crea un DataTable para mostrar los datos de la consulta del repuesto
                if (_erq.WsResponseConsultaRepuesto != null)
                {

                    if (MyArray2 != null)
                    {
                        foreach (ERQ.ZEWS027 WsDatos2 in MyArray2)
                        {
                        _resultReemplazos.Marca = marca.ToUpper();
                        _resultReemplazos.T_INTTYPE = WsDatos2.T_INTTYPE;
                        _resultReemplazos.T_KBETR1 = WsDatos2.T_KBETR1;
                        _resultReemplazos.Cantidad = _consultaRep.CantidadRep.ToString();
                        _resultReemplazos.T_KONDM = WsDatos2.T_KONDM;
                        _resultReemplazos.T_MAKTX = WsDatos2.T_MAKTX;
                        _resultReemplazos.T_MFRPN = WsDatos2.T_MFRPN;
                        _resultReemplazos.Stock = _sapApi.GetCurrentStockByProduct(_resultReemplazos.T_MFRPN, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                        String T_MVGR1 = WsDatos2.T_MVGR1;

                        precio1R = ResultReemplazos.T_KBETR1;

                        totalListaR = double.Parse(precio1R, System.Globalization.CultureInfo.InvariantCulture) * cantidad;

                        int porce = 30;
                        double precio1R_F = (double.Parse(precio1R, System.Globalization.CultureInfo.InvariantCulture) * porce) / 100;
                        double precio1R_FF = double.Parse(precio1R, System.Globalization.CultureInfo.InvariantCulture) - precio1R_F;

                        String T_MVGR1_F = _controlBd.ObtieneGrupoMateriales(T_MVGR1);

                        if (T_MVGR1_F == "" || T_MVGR1_F == "000")
                        {
                            T_MVGR1_F = "";
                            T_MVGR1_F = "S/G";
                        }

                        _controlBd.InsertarDatos(@"insert into LISTA_REEMPLAZO_TMP(ID_SESSION,MARCA,CANTIDAD,T_INTTYPE,T_KBETR1,T_KBETR2,T_KONDM,T_MAKTX,T_MFRPN,TOTAL_C,TOTAL_L,STOCK,GRUPO)
                                values( '" + idSession + "' , '" + _resultReemplazos.Marca + "' , '" + _resultReemplazos.Cantidad + "' , '" + _resultReemplazos.T_INTTYPE + "' , '" + precio2R + "' , '" + Math.Round(precio1R_FF, 2) + "' , '" + _resultReemplazos.T_KONDM + "' , '" + _resultReemplazos.T_MAKTX + "' , '" + _resultReemplazos.T_MFRPN.Substring(3) + "' , '" + totalConceR + "', '" + totalListaR + "' , '" + _resultReemplazos.Stock + "','" + T_MVGR1_F + "')");
                        }
                    }                     
                }
            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }

            disponibilidadVFC = false;
            disponibilidadRepuesto = true;

            //validar para que sirve
            if ((_erq.LocalDtWebRepuestosItem == null) && (_erq.WsResponseConsultaRepuesto == null))
            {
                logger.Error("Error: Repuesto no existe en SAP");
                mensajeError = "ERROR Repuesto no existe en SAP";
                disponibilidadRepuestoSap = false;
                return false;
            }
            else if (_resultReemplazos.Stock==0 && (_resultBusqueda.Stock == 0))
            {
                disponibilidadVFC = true; //MUESTRA MENSAJE vfc
                return true;
            }
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock > 0) && (_resultBusqueda.PrecioConce.Trim() == "0.00") && (_resultBusqueda.PrecioLista.Trim() == "0.00"))
            {
                logger.Error("Repuesto sin precio. Comuníquese con el supervisor");
                mensajeError = "Repuesto sin precio. Comuníquese con el supervisor";
                disponibilidadRepuesto = false; //muestra msj
                return true;
            }
            else
            {
                return true;
            }
            #endregion
        }
        //retorno region ERQ
        else
        {
            return false;
        }

    }


    /// <summary>
    /// Método que inserta una cotizacion en sap
    /// </summary>
    /// <param name="_crearCotizacion"></param>
    /// <returns></returns>
    public bool CrearCotizacion(CrearCotizacion _crearCotizacion, string marca)
    {
        if (ambiente == "ERP")
        {

            #region Ambiente ERP

            string MtvoPedido = "";
            string MtvoPedidoBloq = "";
            DateTime dateTime = DateTime.UtcNow.Date;
            //TipoPedidoBloqueo

            MtvoPedido = _crearCotizacion.MtvoPedido;
            MtvoPedidoBloq = _controlBd.TipoPedidoBloqueo(_crearCotizacion.MtvoPedido, marca);

            //Nueva llamada Crea Pedido por Cotizacion
            _resultCotiza = new RespCotizacionSap();




            _erp.WsCreaPedido.DOC_TYPE = _crearCotizacion.ClaseDocVentas;//"ZBVE"; //Ejemplpo
            _erp.WsCreaPedido.SALES_ORG = _crearCotizacion.OrgVentas; // "BP02";
            _erp.WsCreaPedido.DISTR_CHAN = _crearCotizacion.CDistribucion; // "BA";
            _erp.WsCreaPedido.DIVISION = _crearCotizacion.SpartRep; //"BR";
            _erp.WsCreaPedido.SALES_GRP = "";
            _erp.WsCreaPedido.SALES_OFF = _crearCotizacion.OrgVentas;//Grupo Vendedores ejemplo B02 "BP02";
            _erp.WsCreaPedido.PURCH_NO_C = "";//Numero del pedido - No obligatorio
            _erp.WsCreaPedido.ORD_REASON = MtvoPedido; //Descomentar Cuando Corresponda Porostegui
            _erp.WsCreaPedido.DLV_BLOCK = MtvoPedidoBloq; //Descomentar Cuando Corresponda Porostegui
            _erp.WsCreaPedido.PURCH_DATE = dateTime.ToString("yyyyMMdd");

            _erp.WsCreaPedidoItem.ORDER_HEADER_IN = _erp.WsCreaPedido;

            //Metodo Antiguo crea cotizacion
            //_erq.DtWebIngresoCotizacionItem.I_AUART = _crearCotizacion.ClaseDocVentas;
            //_erq.DtWebIngresoCotizacionItem.I_BNDDT = _crearCotizacion.FecValides;
            //_erq.DtWebIngresoCotizacionItem.I_KUNNR = _crearCotizacion.CodClienteSap;
            //_erq.DtWebIngresoCotizacionItem.I_KUNNR2 = _crearCotizacion.CodClienteSap2;
            //_erq.DtWebIngresoCotizacionItem.I_SPART = _crearCotizacion.SpartRep;
            //_erq.DtWebIngresoCotizacionItem.I_TEXTO = _crearCotizacion.DescripCotizacion;
            //_erq.DtWebIngresoCotizacionItem.I_VKORG = _crearCotizacion.OrgVentas;
            //_erq.DtWebIngresoCotizacionItem.I_VTWEG = _crearCotizacion.CDistribucion;

            #region ObtieneCentro
            //Obtiene centros deacuerdo a marca
            if (_crearCotizacion.OrgVentas == "BP02")
            {
                Centro = "BR02";
            }
            if (_crearCotizacion.OrgVentas == "BP09")
            {
                Centro = "BH02";
            }
            if (_crearCotizacion.OrgVentas == "BP04")
            {
                Centro = "BG02";
            }
            if (_crearCotizacion.OrgVentas == "BP03")
            {
                Centro = "BY02";
            }
            if (_crearCotizacion.OrgVentas == "BP01")
            {
                Centro = "BO02";
            }
            if (_crearCotizacion.OrgVentas == "BP06")
            {
                Centro = "BG02";
            }
            if (_crearCotizacion.OrgVentas == "BP07")
            {
                Centro = "BX02";
            }
            if (_crearCotizacion.OrgVentas == "BP14")
            {
                Centro = "BP21";
            }
            #endregion

            //ERQ.DT_Web_Ingreso_CotizacionItemI_MATERIALES[] DtWebIngresoCotizacionItemMateriales = new ERQ.DT_Web_Ingreso_CotizacionItemI_MATERIALES[_crearCotizacion.Codigo.Count];
            ERP.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN[] DtWebIngresoCotizacionItemMateriales = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN[_crearCotizacion.Codigo.Count];
            ERP.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[] DtWebIngresoCotizacionItemMateriales_1 = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[2];
            ERP.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[] DtWebIngresoCotizacionItemMateriales_2 = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[_crearCotizacion.Codigo.Count];
            ERP.DT_Generacion_Pedido_Venta_RequestORDER_SCHEDULES_IN[] DtWebIngresoCotizacionItemMateriales_3 = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_SCHEDULES_IN[_crearCotizacion.Codigo.Count];

            //_mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [1_RealizaPedido_CrearCotizacion] Message: " + _crearCotizacion.CodClienteSap2 + " Inner: " + _crearCotizacion.CodClienteSap + " Stack: " + "");

            int i = 0;
            int j = 1;
            foreach (string codigo in _crearCotizacion.Codigo)
            {
                //Orders Item
                DtWebIngresoCotizacionItemMateriales[i] = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN();
                DtWebIngresoCotizacionItemMateriales[i].MATERIAL = codigo;// "KPR29110A4200";
                DtWebIngresoCotizacionItemMateriales[i].TARGET_QTY = _crearCotizacion.Cantidad[i];
                DtWebIngresoCotizacionItemMateriales[i].PLANT = Centro; //Centro "BR02"
                DtWebIngresoCotizacionItemMateriales[i].STORE_LOC = "3200";//almacen -----> desmarcar
                DtWebIngresoCotizacionItemMateriales[i].SALES_UNIT = "ST"; //Unidad de Medida para la cantidad Prevista
                //DtWebIngresoCotizacionItemMateriales[i].PROFIT_CTR = "";//Centro Beneficio (BP09, necesita incorporarlo al momento de crear los pedidos, para el resto no es necesario ingrasarñp)
                //_erq.WsCreaPedido_1.MATERIAL = DtWebIngresoCotizacionItemMateriales;
                _erp.WsCreaPedidoItem.ORDER_ITEMS_IN = DtWebIngresoCotizacionItemMateriales;

                //Orders Partners
                //Orders Partners
                DtWebIngresoCotizacionItemMateriales_1[0] = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS();
                DtWebIngresoCotizacionItemMateriales_1[0].PARTN_ROLE = "WE"; //Funcion de interlocutor
                DtWebIngresoCotizacionItemMateriales_1[0].PARTN_NUMB = _crearCotizacion.CodClienteSap2; //Destinatario Mercancia
                DtWebIngresoCotizacionItemMateriales_1[0].ITM_NUMBER = "";
                //_erq.WsCreaPedidoItem.ORDER_PARTNERS = DtWebIngresoCotizacionItemMateriales_1;
                DtWebIngresoCotizacionItemMateriales_1[1] = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS();
                DtWebIngresoCotizacionItemMateriales_1[1].PARTN_ROLE = "AG"; //Funcion de interlocutor
                DtWebIngresoCotizacionItemMateriales_1[1].PARTN_NUMB = _crearCotizacion.CodClienteSap;//_crearCotizacion.CodClienteSap; //Destinatario Mercancia
                DtWebIngresoCotizacionItemMateriales_1[1].ITM_NUMBER = "";
                _erp.WsCreaPedidoItem.ORDER_PARTNERS = DtWebIngresoCotizacionItemMateriales_1;

                PdocV = j + "0";
                PdocVF = PdocV.PadLeft(5, '0');

                //ORDER_SCHEDULES_IN
                DtWebIngresoCotizacionItemMateriales_3[i] = new ERP.DT_Generacion_Pedido_Venta_RequestORDER_SCHEDULES_IN();
                DtWebIngresoCotizacionItemMateriales_3[i].ITM_NUMBER = PdocVF; //Poscicion Documento Ventas "00010";
                DtWebIngresoCotizacionItemMateriales_3[i].REQ_QTY = _crearCotizacion.Cantidad[i];
                _erp.WsCreaPedidoItem.ORDER_SCHEDULES_IN = DtWebIngresoCotizacionItemMateriales_3;
                i += 1;
                j += 1;

            }


            //Se agrega el detalle de la cotización 


            try
            {
                //_erq.DtWebIngresoCotizacion.E_VBELN = _erq.DtWebIngresoCotizacionItem.ToString();
                _erp.WsCreaPedido = _erp.WsCreaPedido;
            }
            catch (Exception ex)
            {
                this.mensajeError = "Error: " + ex.Message + ". Detalle: " + ex.StackTrace;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [1_RealizaPedido_CrearCotizacion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }

            //Se asignan los item a la estructura de consulta
            _erp.WsCreaPedidoVenta.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
            _erp.WsCreaPedidoVenta.PreAuthenticate = true;

            try
            {
                //_erq.WsResponse = _erq.WsIngresoCotizacion.MI_WebS_Cotizacion_Synch(_erq.DtWebIngresoCotizacion);
                _erp.WsCreaPedidoResponse = _erp.WsCreaPedidoVenta.SI_Generacion_Pedido_Venta_Out(_erp.WsCreaPedidoItem);
                //_erq.WsResponse = _erq.WsIngresoCotizacion.SI_Generacion_Cotizacion_Interna_OutAsync(_erq.DtWebIngresoCotizacionItem);

            }
            catch (System.Net.WebException ex)
            {
                this.mensajeError = "Error: " + ex.Message + ". Detalle: " + ex.StackTrace;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [2_RealizaPedido_CrearCotizacion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);

            }

            try
            {

                //REspuesta del WS
                //_resultCotiza.NumCotizacion = _erq.WsResponse.Item.E_VBELN;
                if (_erp.WsCreaPedidoResponse.E_VBELN != null)  //blouqe para verificar referencia obtenida
                {
                    _resultCotiza.NumCotizacion = _erp.WsCreaPedidoResponse.E_VBELN;
                }

                //Errores del WS
                //if (_erq.WsResponse.Item.E_VBELN == null)
                if (_erp.WsCreaPedidoResponse.E_VBELN == null)
                {

                    foreach (ERP.DT_Errores1 wsError in _erp.WsCreaPedidoResponse.E_ERROR) //cambio POROSTEGUI REVISAR
                    {
                        MessageBox.Show(wsError.E_MESSAGE);
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [RealizaPedido_CrearCotizacion_Errores SAP] Message: " + wsError.E_MESSAGE);
                    }
                }


                //Devolución
                //if (_erq.WsResponse.Item.E_VBELN == null)
                if (_erp.WsCreaPedidoResponse.E_VBELN == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (NullReferenceException ex)
            {
                this.mensajeError = "Error: " + ex.Message + ". Detalle: " + ex.StackTrace;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                return false;
            }


            #endregion
        }

        else if (ambiente == "ERQ")
        {
            #region Ambiente ERQ

            string MtvoPedido = "";
            string MtvoPedidoBloq = "";
            DateTime dateTime = DateTime.UtcNow.Date;
            //TipoPedidoBloqueo

            MtvoPedido = _crearCotizacion.MtvoPedido;
            MtvoPedidoBloq = _controlBd.TipoPedidoBloqueo(_crearCotizacion.MtvoPedido, marca);

            //Nueva llamada Crea Pedido por Cotizacion
            _resultCotiza = new RespCotizacionSap();




            _erq.WsCreaPedido.DOC_TYPE = _crearCotizacion.ClaseDocVentas;//"ZBVE"; //Ejemplpo
            _erq.WsCreaPedido.SALES_ORG = _crearCotizacion.OrgVentas; // "BP02";
            _erq.WsCreaPedido.DISTR_CHAN = _crearCotizacion.CDistribucion; // "BA";
            _erq.WsCreaPedido.DIVISION = _crearCotizacion.SpartRep; //"BR";
            _erq.WsCreaPedido.SALES_GRP = "";
            _erq.WsCreaPedido.SALES_OFF = _crearCotizacion.OrgVentas;//Grupo Vendedores ejemplo B02 "BP02";
            _erq.WsCreaPedido.PURCH_NO_C = "";//Numero del pedido - No obligatorio
            _erq.WsCreaPedido.ORD_REASON = MtvoPedido;
            _erq.WsCreaPedido.DLV_BLOCK = MtvoPedidoBloq;
            _erq.WsCreaPedido.PURCH_DATE = dateTime.ToString("yyyyMMdd");

            _erq.WsCreaPedidoItem.ORDER_HEADER_IN = _erq.WsCreaPedido;

            //Metodo Antiguo crea cotizacion
            //_erq.DtWebIngresoCotizacionItem.I_AUART = _crearCotizacion.ClaseDocVentas;
            //_erq.DtWebIngresoCotizacionItem.I_BNDDT = _crearCotizacion.FecValides;
            //_erq.DtWebIngresoCotizacionItem.I_KUNNR = _crearCotizacion.CodClienteSap;
            //_erq.DtWebIngresoCotizacionItem.I_KUNNR2 = _crearCotizacion.CodClienteSap2;
            //_erq.DtWebIngresoCotizacionItem.I_SPART = _crearCotizacion.SpartRep;
            //_erq.DtWebIngresoCotizacionItem.I_TEXTO = _crearCotizacion.DescripCotizacion;
            //_erq.DtWebIngresoCotizacionItem.I_VKORG = _crearCotizacion.OrgVentas;
            //_erq.DtWebIngresoCotizacionItem.I_VTWEG = _crearCotizacion.CDistribucion;

            #region ObtieneCentro
            //Obtiene centros deacuerdo a marca
            if (_crearCotizacion.OrgVentas == "BP02")
            {
                Centro = "BR02";
            }
            if (_crearCotizacion.OrgVentas == "BP09")
            {
                Centro = "BH02";
            }
            if (_crearCotizacion.OrgVentas == "BP04")
            {
                Centro = "BG02";
            }
            if (_crearCotizacion.OrgVentas == "BP03")
            {
                Centro = "BY02";
            }
            if (_crearCotizacion.OrgVentas == "BP01")
            {
                Centro = "BO02";
            }
            if (_crearCotizacion.OrgVentas == "BP06")
            {
                Centro = "BG02";
            }
            if (_crearCotizacion.OrgVentas == "BP07")
            {
                Centro = "BX02";
            }
            if (_crearCotizacion.OrgVentas == "BP14")
            {
                Centro = "BP21";
            }
            #endregion

            //ERQ.DT_Web_Ingreso_CotizacionItemI_MATERIALES[] DtWebIngresoCotizacionItemMateriales = new ERQ.DT_Web_Ingreso_CotizacionItemI_MATERIALES[_crearCotizacion.Codigo.Count];
            ERQ.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN[] DtWebIngresoCotizacionItemMateriales = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN[_crearCotizacion.Codigo.Count];
            ERQ.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[] DtWebIngresoCotizacionItemMateriales_1 = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[2];
            ERQ.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[] DtWebIngresoCotizacionItemMateriales_2 = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS[_crearCotizacion.Codigo.Count];
            ERQ.DT_Generacion_Pedido_Venta_RequestORDER_SCHEDULES_IN[] DtWebIngresoCotizacionItemMateriales_3 = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_SCHEDULES_IN[_crearCotizacion.Codigo.Count];

            //_mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [1_RealizaPedido_CrearCotizacion] Message: " + _crearCotizacion.CodClienteSap2 + " Inner: " + _crearCotizacion.CodClienteSap + " Stack: " + "");


            int i = 0;
            int j = 1;
            foreach (string codigo in _crearCotizacion.Codigo)
            {
                //Orders Item
                DtWebIngresoCotizacionItemMateriales[i] = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN();
                DtWebIngresoCotizacionItemMateriales[i].MATERIAL = codigo;// "KPR29110A4200";
                DtWebIngresoCotizacionItemMateriales[i].TARGET_QTY = _crearCotizacion.Cantidad[i];
                DtWebIngresoCotizacionItemMateriales[i].PLANT = Centro; //Centro "BR02"
                DtWebIngresoCotizacionItemMateriales[i].STORE_LOC = "3200";//almacen
                DtWebIngresoCotizacionItemMateriales[i].SALES_UNIT = "ST"; //Unidad de Medida para la cantidad Prevista
                //DtWebIngresoCotizacionItemMateriales[i].PROFIT_CTR = "";//Centro Beneficio (BP09, necesita incorporarlo al momento de crear los pedidos, para el resto no es necesario ingrasarñp)
                //_erq.WsCreaPedido_1.MATERIAL = DtWebIngresoCotizacionItemMateriales;
                _erq.WsCreaPedidoItem.ORDER_ITEMS_IN = DtWebIngresoCotizacionItemMateriales;

                //Orders Partners
                DtWebIngresoCotizacionItemMateriales_1[0] = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS();
                DtWebIngresoCotizacionItemMateriales_1[0].PARTN_ROLE = "WE"; //Funcion de interlocutor
                DtWebIngresoCotizacionItemMateriales_1[0].PARTN_NUMB = _crearCotizacion.CodClienteSap2; //Destinatario Mercancia
                DtWebIngresoCotizacionItemMateriales_1[0].ITM_NUMBER = "";
                //_erq.WsCreaPedidoItem.ORDER_PARTNERS = DtWebIngresoCotizacionItemMateriales_1;
                DtWebIngresoCotizacionItemMateriales_1[1] = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_PARTNERS();
                DtWebIngresoCotizacionItemMateriales_1[1].PARTN_ROLE = "AG"; //Funcion de interlocutor
                DtWebIngresoCotizacionItemMateriales_1[1].PARTN_NUMB = _crearCotizacion.CodClienteSap;//_crearCotizacion.CodClienteSap; //Destinatario Mercancia
                DtWebIngresoCotizacionItemMateriales_1[1].ITM_NUMBER = "";
                _erq.WsCreaPedidoItem.ORDER_PARTNERS = DtWebIngresoCotizacionItemMateriales_1;

                PdocV = j + "0";
                PdocVF = PdocV.PadLeft(5, '0');

                //ORDER_SCHEDULES_IN
                DtWebIngresoCotizacionItemMateriales_3[i] = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_SCHEDULES_IN();
                DtWebIngresoCotizacionItemMateriales_3[i].ITM_NUMBER = PdocVF; //Poscicion Documento Ventas "00010";
                DtWebIngresoCotizacionItemMateriales_3[i].REQ_QTY = _crearCotizacion.Cantidad[i];
                _erq.WsCreaPedidoItem.ORDER_SCHEDULES_IN = DtWebIngresoCotizacionItemMateriales_3;
                i += 1;
                j += 1;

            }


            //Se agrega el detalle de la cotización 


            try
            {
                //_erq.DtWebIngresoCotizacion.E_VBELN = _erq.DtWebIngresoCotizacionItem.ToString();
                _erq.WsCreaPedido = _erq.WsCreaPedido;
            }
            catch (Exception ex)
            {
                this.mensajeError = "Error: " + ex.Message + ". Detalle: " + ex.StackTrace;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [1_RealizaPedido_CrearCotizacion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }

            //Se asignan los item a la estructura de consulta
            //_erq.WsCreaPedidoVenta.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
            //_erq.WsCreaPedidoVenta.Credentials = new System.Net.NetworkCredential("INT_REP_SKBP", "5k82017PoPpe");  //PROD
            _erq.WsCreaPedidoVenta.Credentials = new System.Net.NetworkCredential("INT_RPTOS_SKB", "skb2017poq");     // QA
            _erq.WsCreaPedidoVenta.PreAuthenticate = true;

            try
            {
                //_erq.WsResponse = _erq.WsIngresoCotizacion.MI_WebS_Cotizacion_Synch(_erq.DtWebIngresoCotizacion);
                _erq.WsCreaPedidoResponse = _erq.WsCreaPedidoVenta.SI_Generacion_Pedido_Venta_Out(_erq.WsCreaPedidoItem);
                //_erq.WsResponse = _erq.WsIngresoCotizacion.SI_Generacion_Cotizacion_Interna_OutAsync(_erq.DtWebIngresoCotizacionItem);

            }
            catch (System.Net.WebException ex)
            {
                this.mensajeError = "Error: " + ex.Message + ". Detalle: " + ex.StackTrace;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [2_RealizaPedido_CrearCotizacion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);

            }

            try
            {

                //REspuesta del WS
                //_resultCotiza.NumCotizacion = _erq.WsResponse.Item.E_VBELN;
                if (_erq.WsCreaPedidoResponse.E_VBELN != null)  //blouqe para verificar referencia obtenida
                {
                    _resultCotiza.NumCotizacion = _erq.WsCreaPedidoResponse.E_VBELN;

                }

                //Errores del WS
                //if (_erq.WsResponse.Item.E_VBELN == null)
                if (_erq.WsCreaPedidoResponse.E_VBELN == null)
                {

                    foreach (ERQ.DT_Errores1 wsError in _erq.WsCreaPedidoResponse.E_ERROR) //cambio POROSTEGUI REVISAR
                    {
                        MessageBox.Show(wsError.E_MESSAGE);
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [RealizaPedido_CrearCotizacion_Errores SAP] Message: " + wsError.E_MESSAGE);
                    }
                }


                //Devolución
                //if (_erq.WsResponse.Item.E_VBELN == null)
                if (_erq.WsCreaPedidoResponse.E_VBELN == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (NullReferenceException ex)
            {
                this.mensajeError = "Error: " + ex.Message + ". Detalle: " + ex.StackTrace;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                return false;
            }


            #endregion
        }
        else { return false; }
    }

    /// <summary>
    /// Método que confrma la cotización y la convierte en un pedido SAP
    /// </summary>
    /// <param name="_confirPedido"></param>
    /// <returns></returns>
    public bool ConfirmarPedido(ConfirmarPedido _confirPedido, out string error)
    {
        if (ambiente == "ERP")
        {
            /*#region Ambiente ERP

            //Autentificación al WS Crear Pedido
            _erp.WsCreaPedido.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
            _erp.WsCreaPedido.PreAuthenticate = true;

            _erp.DtWebPdoVtasPedido.I_VBELN = _confirPedido.NumCotizacion;
            _erp.DtWebPdoVtasPedido.I_AUART = _confirPedido.TipoPedido;
            _erp.DtWebPdoVtasPedido.I_LPRIO = _confirPedido.Prioridad;
            

            _resultPedido = new RespConfirmarPedido();

            //Se asignan los valores al WS
            _erp.DtWebPdoVtas.Pedido = _erp.DtWebPdoVtasPedido;

            try
            {
                _erp.WsResponse1 = _erp.WsCreaPedido.MI_WebS_Crear_Ped_Repues_Synch(_erp.DtWebPdoVtas);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("Error de acceso, favor volver a intentar " );
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "1- En [RealizaPedido_ConfirmarPedido_SAP_InvalidOperationException] Error de acceso a sap .Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);

            }
            catch (System.NullReferenceException ex)
            {
                MessageBox.Show("Error de acceso, favor volver a intentar ");
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "2- En [RealizaPedido_ConfirmarPedido_SAP_NullReferenceException] Error de acceso a sap .Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error de acceso, favor volver a intentar ");
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "3- En [RealizaPedido_ConfirmarPedido_SAP_Exception] Error de acceso a sap .Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);

            }

            try
            {

                if (_erp.WsResponse1.Pedido.E_VBELN == null)
                {
                    string err = "";
                    foreach (ERP.DT_ERP_PdoVtasPedidoErrores e in _erp.WsResponse1.Pedido.Errores)
                    {
                        //D
                        err += "Detalles del Error: Transacción no devuelve número de pedido, favor consulte estado de la cotización";
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "1-En [RealizaPedido_ConfirmarPedido_SAP_Errores_WS_SAP] Message: Error SAP: " + e.E_MESSAGE + "\n" + e.E_MESSAGE_V1 + "\n" + e.E_MESSAGE_V2 + "\n" + e.E_MESSAGE_V3 + "\n" + e.E_MESSAGE_V4  +" .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
                    }
                    error = err;
                    return false;
                }
                else if (_erp.WsResponse1.Pedido.E_VBELN == "")
                {
                    string err = "";
                    foreach (ERP.DT_ERP_PdoVtasPedidoErrores e in _erp.WsResponse1.Pedido.Errores)
                    {
                        err += "Detalles del Error: Transacción no devuelve número de pedido, favor consulte estado de la cotización";
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "2-En [RealizaPedido_ConfirmarPedido_Errores_WS_SAP] Message: Error SAP: " + e.E_MESSAGE + "\n" + e.E_MESSAGE_V1 + "\n" + e.E_MESSAGE_V2 + "\n" + e.E_MESSAGE_V3 + "\n" + e.E_MESSAGE_V4 + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
                    }
                    error = err;
                    return false;
                }
                else
                {
                    _resultPedido.NumPedido = _erp.WsResponse1.Pedido.E_VBELN;
                    error = "";
                    return true;
                }

            }
            catch (Exception ex)
            {
                //Error de conexion sap
                error = "Error de acceso, favor volver a intentar";
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [RealizaPedido_Error_Conexion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
                return false;
            }


             * 
            #endregion*/
            error = "";
            return true;
        }
        else if (ambiente == "ERQ")
        {
            /*#region Ambiente ERQ

            //Autentificación al WS Crear Pedido
            _erq.WsCreaPedido.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
            _erq.WsCreaPedido.PreAuthenticate = true;

                     

            _erq.DtWebPdoVtasPedido.I_VBELN = _confirPedido.NumCotizacion;
            _erq.DtWebPdoVtasPedido.I_AUART = _confirPedido.TipoPedido;
            _erq.DtWebPdoVtasPedido.I_LPRIO = _confirPedido.Prioridad;

            _resultPedido = new RespConfirmarPedido();

            //Se asignan los valores al WS
            _erq.DtWebPdoVtas.E_VBELN = _erq.DtWebPdoVtasPedido.ToString();
                       

            try
            {
                //_erq.WsResponse1 = _erq.WsCreaPedido.MI_WebS_Crear_Ped_Repues_Synch(_erq.DtWebPdoVtas);
                _erq.WsResponse1 = _erq.WsCreaPedido.SI_Generacion_Pedido_Venta_Out(_erq.DtWebPdoVtasPedido);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("Error de acceso, favor volver a intentar ");
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "1- En [RealizaPedido_ConfirmarPedido_SAP_InvalidOperationException] Error de acceso a sap .Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
            }
            catch (System.NullReferenceException ex)
            {
                MessageBox.Show("Error de acceso, favor volver a intentar ");
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "2- En [RealizaPedido_ConfirmarPedido_SAP_NullReferenceException]  Error de acceso a sap .Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error de acceso, favor volver a intentar ");
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "3- En [RealizaPedido_ConfirmarPedido_SAP_Exception]  Error de acceso a sap .Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
            }

            try
            {
                
                if (_erq.WsResponse1.E_VBELN == null)
                {
                    string err = "";
                    foreach (ERQ.DT_Errores1 e in _erq.WsResponse1.E_ERROR)
                    {
                        err += "Detalles del Error: Transacción no devuelve número de pedido, favor consulte estado de la cotización";
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "1-En [RealizaPedido_ConfirmarPedido_SAP_Errores_WS_SAP] Message: Error SAP: " + e.E_MESSAGE + "\n" + e.E_MESSAGE_V1 + "\n" + e.E_MESSAGE_V2 + "\n" + e.E_MESSAGE_V3 + "\n" + e.E_MESSAGE_V4 + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
                    }
                    error = err;
                    return false;
                }
                else if (_erq.WsResponse1.E_VBELN == "")
                {
                    string err = "";
                    foreach (ERQ.DT_Errores1 e in _erq.WsResponse1.E_ERROR)
                    {
                        err += "Detalles del Error: Transacción no devuelve número de pedido, favor consulte estado de la cotización";
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "2-En [RealizaPedido_ConfirmarPedido_Errores_WS_SAP] Message: Error SAP: " + e.E_MESSAGE + "\n" + e.E_MESSAGE_V1 + "\n" + e.E_MESSAGE_V2 + "\n" + e.E_MESSAGE_V3 + "\n" + e.E_MESSAGE_V4 + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
                    }
                    error = err;
                    return false;
                }
                else
                {
                    _resultPedido.NumPedido = _erq.WsResponse1.E_VBELN;
                    error = "";
                    return true;
                }

            }
            catch (Exception ex)
            {
                error = "Error de acceso, favor volver a intentar";
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [RealizaPedido_Error_Conexion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .num cotiz: " + _confirPedido.NumCotizacion + " .tipo pedido: " + _confirPedido.TipoPedido + " .Prioridad: " + _confirPedido.Prioridad);
                return false;
            }

            #endregion*/
        }
        else
        {
            error = "ERROR, ambiente no definido";
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [RealizaPedido_ConfirmarPedido_SAP] Message: ERROR, ambiente no definido (ERQ,ERP)");
            return false;
        }
        //Para salir del paso POG
        error = "";
        return true;
    }

    public RespConsultaRepuesto ResultBusqueda
    {
        get { return _resultBusqueda; }
        set { _resultBusqueda = value; }
    }

    public RespCotizacionSap ResultCotiza
    {
        get { return _resultCotiza; }
        set { _resultCotiza = value; }
    }

    public RespConfirmarPedido ResultPedido
    {
        get { return _resultPedido; }
        set { _resultPedido = value; }
    }

    public RespCadenaReemplazo ResultReemplazos
    {
        get { return _resultReemplazos; }
        set { _resultReemplazos = value; }
    }


    public RespEstadoPed EstadoPed
    {
        get { return _estadoPed; }
        set { _estadoPed = value; }
    }


    public bool BuscarRepuestoCant(ConsultaRepuesto _consultaRep, string idSession, int cantidad, string nombre, string marca)
    {
        if (ambiente == "ERP")
        {
            /* #region ambiente ERP
             try
             {

                 _resultBusqueda = new RespConsultaRepuesto();
                 _resultReemplazos = new RespCadenaReemplazo();

                 //Declaración de variables de acceso a Sap (AMBIENTE DE PRODUCCION)

                 //Se asignan los datos basicos a cada item
                 _erp.LocalDtWebRepuestosItem.I_VKORG = _consultaRep.DocVentas;
                 _erp.LocalDtWebRepuestosItem.I_MFRPN = _consultaRep.CodRepuesto;
                 _erp.LocalDtWebRepuestosItem.I_KWMENG = "1";// _consultaRep.CantidadRep.ToString();
                 _erp.LocalDtWebRepuestosItem.I_MAKTX = _consultaRep.TextoRep;
                 _erp.LocalDtWebRepuestosItem.I_MVGR4 = _consultaRep.GrupoMaterial;
                 _erp.LocalDtWebRepuestosItem.I_VTWEG = _consultaRep.CanalDistribucion;
                 _erp.LocalDtWebRepuestosItem.I_KUNNR = _consultaRep.DestinaMercacia;
                 _erp.LocalDtWebRepuestosItem.I_FORANEO = "";

                 // Para marcas foraneas, va una X en el campo i_foraneo
                 ControlMarca _controlMarca = new ControlMarca();
                 Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                 if (marcaVehiculo == null)
                 {
                     throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                 }
                 if (_controlMarca.obtenerMarcaPorNombre(marca).esForaneo)
                 {
                     _erp.LocalDtWebRepuestosItem.I_FORANEO = "X";
                 }

                 //Se asignan los item a la estructura de consulta
                 _erp.LocalDtWebRepuestos.Item = _erp.LocalDtWebRepuestosItem;
                 _erp.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                 _erp.WsConsultaRepuesto.PreAuthenticate = true;

                 //Envio todos los datosa para la consulta al WS
                 _erp.WsResponseConsultaRepuesto = _erp.WsConsultaRepuesto.MI_WebS_Repuestos_Synch(_erp.LocalDtWebRepuestos);


                 //Se obtiene el stock

                 foreach (ERP.DT_ERP_RepuestosT_ZESD002 WsDatos in _erp.WsResponseConsultaRepuesto.T_ZESD002)
                 {
                     _resultBusqueda.Marca = marca.ToUpper();
                     _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString(); //cantidad.ToString();
                     _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                     _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                     _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                     _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                     _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                     _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre);

                     _controlBd.InsertarDatos(@"insert into repuesto_cantidadmin (marca,codigo,cantidadMin,modelo) values ('" + _resultBusqueda.Marca + "', '" + _resultBusqueda.Codigo.Substring(3) + "',0 , 'modelo') ");
                 }



             }
             catch (System.Net.WebException ex)
             {
                 this.mensajeError = this.mensajeError +
                                     ". data: " + ex.Data +
                                     ". innerException: " + ex.InnerException +
                                     ". Response: " + ex.Response +
                                     ". Status: " + ex.Status +
                                     ". data: " + ex.TargetSite +
                                     "mensaje original: " + this.mensajeError + ". Error: " + ex.Message + ". Detalle traza: " + ex.StackTrace;
                 logger.Error("Error: " + ex.Message + ". Detalle traza: " + ex.StackTrace);
             }
             catch (Exception ex)
             {
                 this.mensajeError = this.mensajeError +
                                     ". data: " + ex.Data +
                                     ". innerException: " + ex.InnerException +
                                     ". data: " + ex.TargetSite +
                                     "mensaje original: " + this.mensajeError + ". Error: " + ex.Message + ". Detalle traza: " + ex.StackTrace;
                 logger.Error("Error: " + ex.Message + ". Detalle traza: " + ex.StackTrace);
             }

             if (_erp.WsResponseConsultaRepuesto.T_ZESD002 == null)
             {
                 return false;
             }
             else
             {
                 return true;
             }
             #endregion*/
            return true;
        }

        /*else if (ambiente == "ERQ")
        {

            #region Ambiente ERQ
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();

                ///////////////////Declaración de variables de acceso a Sap (AMBIENTE DE PRODUCCION)///////////
                //Se asignan los datos basicos a cada item
                _erq.LocalDtWebRepuestosItem.I_VKORG = _consultaRep.DocVentas;
                _erq.LocalDtWebRepuestosItem.I_MFRPN = _consultaRep.CodRepuesto;
                _erq.LocalDtWebRepuestosItem.I_KWMENG = "1";// _consultaRep.CantidadRep.ToString();
                _erq.LocalDtWebRepuestosItem.I_MAKTX = _consultaRep.TextoRep;
                _erq.LocalDtWebRepuestosItem.I_MVGR4 = _consultaRep.GrupoMaterial;
                _erq.LocalDtWebRepuestosItem.I_VTWEG = _consultaRep.CanalDistribucion;
                _erq.LocalDtWebRepuestosItem.I_FORANEO = "";
                _erq.LocalDtWebRepuestosItem.I_KUNNR = _consultaRep.DestinaMercacia;

                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }
                if (_controlMarca.obtenerMarcaPorNombre(marca).esForaneo)
                {
                    _erq.LocalDtWebRepuestosItem.I_FORANEO = "X";
                }

                //Se asignan los item a la estructura de consulta
                _erq.LocalDtWebRepuestos.Item = _erq.LocalDtWebRepuestosItem;
                _erq.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                _erq.WsConsultaRepuesto.PreAuthenticate = true;

                //Envio todos los datosa para la consulta al WS
                _erq.WsResponseConsultaRepuesto = _erq.WsConsultaRepuesto.MI_WebS_Repuestos_Synch(_erq.LocalDtWebRepuestos);

                foreach (ERQ.DT_ERP_RepuestosT_ZESD002 WsDatos in _erq.WsResponseConsultaRepuesto.T_ZESD002)
                {
                    _resultBusqueda.Marca = marca.ToUpper();

                    _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString();
                    _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                    _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                    _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                    _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                    _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                    _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre);

                    _controlBd.InsertarDatos(@"insert into repuesto_cantidadmin (marca,codigo,cantidadMin,modelo) values ('" + _resultBusqueda.Marca + "', '" + _resultBusqueda.Codigo.Substring(3) + "',0 , 'modelo') ");
                }


            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }

            if (_erq.WsResponseConsultaRepuesto.T_ZESD002 == null)
            {
                return false;
            }
            else
            {
                return true;
            }
            #endregion
        }*/
        else
        {
            return false;
        }

    }

    public string BuscarRepuestoDescripcion(ConsultaRepuesto _consultaRep, string idSession, int cantidad, string nombre, string marca)
    {
        string desc = "";
        if (ambiente == "ERP")
        {
            #region Ambiente ERP
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();
               
                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }

                //Se asignan los item a la estructura de consulta
                _erp.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential("INT_WTY_SKBP", "5k82017PoPpe");
                _erp.WsConsultaRepuesto.PreAuthenticate = true;

                String dato1; //E_KWMENG
                String dato2;
                String dato3; //E_MFRPN
                String dato4; //E_MVGR4
                String dato5; //E_VKORG
                String dato6; //E_VTWEG

                //Variable utilizada para agregar * al texto de busqueda
                String TxtRptoP = "";

                if (_consultaRep.TextoRep != "")
                {
                    TxtRptoP = "*" + _consultaRep.TextoRep + "*";
                }

                //Declaracion de arreglos para recepcion de datos
                var MyArray = new ERP.ZEWS004[1];
                var MyArray1 = new ERP.ZEWS026[1];
                var MyArray2 = new ERP.ZEWS027[1];
                String MVGR1 = "";

                //Consulta WS Consulta Repuesto 
                _erp.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("",
                                                                _consultaRep.DestinaMercacia,
                                                                "1",
                                                                TxtRptoP,
                                                                _consultaRep.CodRepuesto,
                                                                _consultaRep.GrupoMaterial,
                                                                "",
                                                                _consultaRep.DocVentas,
                                                                _consultaRep.CanalDistribucion,
                                                                out dato1, //E_KWMENG
                                                                out dato2,
                                                                out dato3, //E_MFRPN
                                                                out dato4, //E_MVGR4
                                                                out dato5, //E_VKORG
                                                                out dato6, //E_VTWEG
                                                                out MyArray,
                                                                out MyArray1,
                                                                out MyArray2);



                foreach (ERP.ZEWS026 WsDatos in MyArray1)
                {

                    //String a = WsDatos.EZ_MVGR1;
                    _resultBusqueda.Marca = marca.ToUpper();

                    _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString();
                    _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                    _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                    _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                    _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                    _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                    _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                    MVGR1 = WsDatos.EZ_MVGR1;

                    String MVGR1_F = _controlBd.ObtieneGrupoMateriales(MVGR1);

                }
               
            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            disponibilidadVFC = false;
            disponibilidadRepuesto = true;

            if (ResultBusqueda.Descripcion != null && ResultBusqueda.Descripcion != "")
            {
                desc = ResultBusqueda.Descripcion.ToString();
            }
            else
            {
                desc = "";
            }

            #endregion
        }

        else if (ambiente == "ERQ")
        {
            #region Ambiente ERQ
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();

                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }

                //Se asignan los item a la estructura de consulta
                _erq.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                _erq.WsConsultaRepuesto.PreAuthenticate = true;

                String dato1; //E_KWMENG
                String dato2;
                String dato3; //E_MFRPN
                String dato4; //E_MVGR4
                String dato5; //E_VKORG
                String dato6; //E_VTWEG

                //Variable utilizada para agregar * al texto de busqueda
                String TxtRptoP = "";

                if (_consultaRep.TextoRep != "")
                {
                    TxtRptoP = "*" + _consultaRep.TextoRep + "*";
                }

                //Declaracion de arreglos para recepcion de datos
                var MyArray = new ERQ.ZEWS004[1];
                var MyArray1 = new ERQ.ZEWS026[1];
                var MyArray2 = new ERQ.ZEWS027[1];
                String MVGR1 = "";

                //Consulta WS Consulta Repuesto 
                _erq.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("",
                                                                _consultaRep.DestinaMercacia,
                                                                "1",
                                                                TxtRptoP,
                                                                _consultaRep.CodRepuesto,
                                                                _consultaRep.GrupoMaterial,
                                                                "",
                                                                _consultaRep.DocVentas,
                                                                _consultaRep.CanalDistribucion,
                                                                out dato1, //E_KWMENG
                                                                out dato2,
                                                                out dato3, //E_MFRPN
                                                                out dato4, //E_MVGR4
                                                                out dato5, //E_VKORG
                                                                out dato6, //E_VTWEG
                                                                out MyArray,
                                                                out MyArray1,
                                                                out MyArray2);

                if (MyArray1 != null)
                {
                    foreach (ERQ.ZEWS026 WsDatos in MyArray1)
                    {

                        //String a = WsDatos.EZ_MVGR1;
                        _resultBusqueda.Marca = marca.ToUpper();

                        _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString();
                        _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                        _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                        _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                        _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                        _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                        _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                        MVGR1 = WsDatos.EZ_MVGR1;

                        String MVGR1_F = _controlBd.ObtieneGrupoMateriales(MVGR1);

                    }
                }



                if (ResultBusqueda.Descripcion != null && ResultBusqueda.Descripcion != "")
                {
                    desc = ResultBusqueda.Descripcion.ToString();
                }
                else
                {
                    desc = "";
                }
            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            disponibilidadVFC = false;
            disponibilidadRepuesto = true;

            #endregion
        }
        return desc;
    }

    //Buscador para las Solicitudes de Cotizacion.-
    public bool BuscarRepuestoSolicitud(ConsultaRepuesto _consultaRep, string idSession, int cantidad, string nombre, string marca)
    {
        if (ambiente == "ERP")
        {
            #region Ambiente ERP
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();

                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }

                //Se asignan los item a la estructura de consulta
                _erp.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential("INT_WTY_SKBP", "5k82017PoPpe");
                _erp.WsConsultaRepuesto.PreAuthenticate = true;

                String dato1; //E_KWMENG
                String dato2;
                String dato3; //E_MFRPN
                String dato4; //E_MVGR4
                String dato5; //E_VKORG
                String dato6; //E_VTWEG

                //Variable utilizada para agregar * al texto de busqueda
                String TxtRptoP = "";

                if (_consultaRep.TextoRep != "")
                {
                    TxtRptoP = "*" + _consultaRep.TextoRep + "*";
                }

                //Declaracion de arreglos para recepcion de datos
                var MyArray = new ERP.ZEWS004[1];
                var MyArray1 = new ERP.ZEWS026[1];
                var MyArray2 = new ERP.ZEWS027[1];
                String MVGR1 = "";

                //Consulta WS Consulta Repuesto 
                _erp.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("",
                                                                _consultaRep.DestinaMercacia,
                                                                "1",
                                                                TxtRptoP,
                                                                _consultaRep.CodRepuesto,
                                                                _consultaRep.GrupoMaterial,
                                                                "",
                                                                _consultaRep.DocVentas,
                                                                _consultaRep.CanalDistribucion,
                                                                out dato1, //E_KWMENG
                                                                out dato2,
                                                                out dato3, //E_MFRPN
                                                                out dato4, //E_MVGR4
                                                                out dato5, //E_VKORG
                                                                out dato6, //E_VTWEG
                                                                out MyArray,
                                                                out MyArray1,
                                                                out MyArray2);

                if (MyArray1 == null)
                {
                    return false;
                }
                else
                {
                    foreach (ERP.ZEWS026 WsDatos in MyArray1)
                    {

                        //String a = WsDatos.EZ_MVGR1;
                        _resultBusqueda.Marca = marca.ToUpper();

                        _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString();
                        _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                        _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                        _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                        _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                        _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                        _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                        MVGR1 = WsDatos.EZ_MVGR1;

                        if (_resultBusqueda.Stock < 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                

            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            disponibilidadVFC = false;
            disponibilidadRepuesto = true;
            if ((_erq.LocalDtWebRepuestosItem == null) && (_erq.WsResponseConsultaRepuesto == null))
            {
                logger.Error("Error: Repuesto no existe en SAP");
                mensajeError = "ERROR Repuesto no existe en SAP";
                //disponibilidadServicio = false;
                disponibilidadRepuestoSap = false;
                return false;
            }//stock = 0 y precio=0 || _resultBusqueda.PrecioConce.Trim() == "0.00" || _resultBusqueda.PrecioLista.Trim() == "0.00"
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock == 0))
            {
                disponibilidadVFC = true; //MUESTRA MENSAJE vfc
                return true;
            }
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock > 0) && (_resultBusqueda.PrecioConce.Trim() == "0.00") && (_resultBusqueda.PrecioLista.Trim() == "0.00"))
            {
                logger.Error("Repuesto sin precio. Comuníquese con el supervisor");
                mensajeError = "Repuesto sin precio. Comuníquese con el supervisor";
                disponibilidadRepuesto = false; //muestra msj
                return true;
            }
            else
            {
                return true;
            }
            #endregion
        }

        else if (ambiente == "ERQ")
        {

            #region Ambiente ERQ
            try
            {
                _resultBusqueda = new RespConsultaRepuesto();
                _resultReemplazos = new RespCadenaReemplazo();

                // Para marcas foraneas, va una X en el campo i_foraneo
                ControlMarca _controlMarca = new ControlMarca();
                Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
                if (marcaVehiculo == null)
                {
                    throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
                }


                //Se asignan los item a la estructura de consulta
                _erq.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                _erq.WsConsultaRepuesto.PreAuthenticate = true;

                //Envio todos los datosa para la consulta al WS

                String dato1; //E_KWMENG
                String dato2;
                String dato3; //E_MFRPN
                String dato4; //E_MVGR4
                String dato5; //E_VKORG
                String dato6; //E_VTWEG

                //Variable utilizada para agregar * al texto de busqueda
                String TxtRptoP = "";

                if (_consultaRep.TextoRep != "")
                {
                    TxtRptoP = "*" + _consultaRep.TextoRep + "*";
                }

                //Declaracion de arreglos para recepcion de datos
                var MyArray = new ERQ.ZEWS004[1];
                var MyArray1 = new ERQ.ZEWS026[1];
                var MyArray2 = new ERQ.ZEWS027[1];
                String MVGR1 = "";

                //Consulta WS Consulta Repuesto 
                _erq.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("",
                                                                _consultaRep.DestinaMercacia,
                                                                "1",
                                                                TxtRptoP,
                                                                _consultaRep.CodRepuesto,
                                                                _consultaRep.GrupoMaterial,
                                                                "",
                                                                _consultaRep.DocVentas,
                                                                _consultaRep.CanalDistribucion,
                                                                out dato1, //E_KWMENG
                                                                out dato2,
                                                                out dato3, //E_MFRPN
                                                                out dato4, //E_MVGR4
                                                                out dato5, //E_VKORG
                                                                out dato6, //E_VTWEG
                                                                out MyArray,
                                                                out MyArray1,
                                                                out MyArray2);

                if (MyArray1 != null)
                {
                    foreach (ERQ.ZEWS026 WsDatos in MyArray1)
                    {

                        //String a = WsDatos.EZ_MVGR1;
                        _resultBusqueda.Marca = marca.ToUpper();

                        _resultBusqueda.Cantidad = _consultaRep.CantidadRep.ToString();
                        _resultBusqueda.PrecioLista = WsDatos.EZ_KBETR1;
                        _resultBusqueda.PrecioConce = WsDatos.EZ_KBETR2;
                        _resultBusqueda.GrupoMat = WsDatos.EZ_KONDM;
                        _resultBusqueda.Descripcion = WsDatos.EZ_MAKTX;
                        _resultBusqueda.Codigo = WsDatos.EZ_MFRPN;
                        _resultBusqueda.Stock = _sapApi.GetCurrentStockByProduct(_resultBusqueda.Codigo, _consultaRep.GrupoMaterial, _consultaRep.DestinaMercacia, nombre, marca);
                        MVGR1 = WsDatos.EZ_MVGR1;

                        if (_resultBusqueda.Stock < 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
                

            }
            catch (System.Net.WebException ex)
            {
                logger.Error("WebException en [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                disponibilidadServicio = false;
                mensajeError = "ERROR FATAL. El servicio Web para conectarse a SAP no está operativo. Disculpe las molestias";
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                logger.Error("En [BuscarRepuesto] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [3_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            }
            disponibilidadVFC = false;
            disponibilidadRepuesto = true;
            if ((_erq.LocalDtWebRepuestosItem == null) && (_erq.WsResponseConsultaRepuesto == null))
            {
                logger.Error("Error: Repuesto no existe en SAP");
                mensajeError = "ERROR Repuesto no existe en SAP";
                //disponibilidadServicio = false;
                disponibilidadRepuestoSap = false;
                return false;
            }//stock = 0 y precio=0 || _resultBusqueda.PrecioConce.Trim() == "0.00" || _resultBusqueda.PrecioLista.Trim() == "0.00"
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock == 0))
            {
                disponibilidadVFC = true; //MUESTRA MENSAJE vfc
                return true;
            }
            else if (_erq.WsResponseConsultaRepuesto == null && (_resultBusqueda.Stock > 0) && (_resultBusqueda.PrecioConce.Trim() == "0.00") && (_resultBusqueda.PrecioLista.Trim() == "0.00"))
            {
                logger.Error("Repuesto sin precio. Comuníquese con el supervisor");
                mensajeError = "Repuesto sin precio. Comuníquese con el supervisor";
                disponibilidadRepuesto = false; //muestra msj
                return true;
            }
            else
            {
                return true;
            }
            #endregion
        }
        else
        {
            return false;
        }

    }

}