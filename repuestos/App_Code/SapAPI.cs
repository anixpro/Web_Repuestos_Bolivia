using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;


/// <summary>
/// Descripción breve de SapAPI
/// </summary>
public class SapAPI
{
    // Conexion a BD
    ControlBD _controlBD;

    // Objetos relacionados con el ambiente
    AmbienteERP _erp = new AmbienteERP();
    AmbienteERQ _erq = new AmbienteERQ();
    string ambiente = "";

    // Variables y Constantes comunes del sistema
    public string CanalDeDistribucion = "B7";
    public string ClaseDocumentoDeVentas = "ZBVE";
    public string ClaseDocumentoDeVentasForaneo = "ZB04";
    public string Sector = "BR";

    // Tipos de Clases de Documentos
    public string TipoClaseDocumentoVentasNormal = "ZBVE";
    public string TipoClaseDocumentoVentasGarantias = "ZBPG";
    public string CanalDeDistribucionPedido = "BA";
    public string CanalDeDistribucionGarantia = "B6";

    // Dias de Validez de la Oferta para la creacion del pedido
    const int ValidezDeLaOferta = 5;

    // Flags de Debug de la aplicacion (Salida en Output del IDE)
    const bool DebugActivated = true;
    const bool SqlDebugActivated = true;
    
    public SapAPI()
    {
        _controlBD = new ControlBD();

        //Se obtiene el ambiente de trabajo y las credenciales correspondientes
        ConfAmbiente.ConfCredenciales();
        ambiente = ConfAmbiente.ambiente;
    }

    /// <summary>
    /// Este método obtiene el código del Dealer, recibiendo como parametro el código del Id del usuario
    /// </summary>
    /// <param name="UserId"></param>
    /// <returns></returns>
    /// 
    public string GetDealerCodeByUserId(string UserId)
    {
        string SQL = "SELECT DEALER_CODE FROM DEALER_USERS WHERE USER_CODE='" + UserId + "'";
        string DealerId = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            DealerId = campos["DEALER_CODE"].ToString();
        }

        return DealerId;
    }

    /// <summary>
    /// Este método devuelve el código del grupo de materiales, recibiendo como parametro la marca
    /// </summary>
    /// <param name="strMarca"></param>
    /// <returns></returns>
    public string GetGrupoMaterialesByMarca(string strMarca)
    {

        string SQL = "SELECT GRUPO_MATERIAL FROM GRUPO_MATERIALES WHERE MARCA = '" + strMarca + "'";
        string GrupoMaterial = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            GrupoMaterial = campos["GRUPO_MATERIAL"].ToString();
        }

        return GrupoMaterial;
    }

    /// <summary>
    /// Este método devuelve el código Vkorg, recibiendo como parametro el código del Grupo de material
    /// </summary>
    /// <param name="strGrupoMaterial"></param>
    /// <returns></returns>
    public string GetVkorgByGrupoMaterial(string strGrupoMaterial, string strMarca)
    {

        string SQL = "SELECT VKORG FROM GRUPO_MATERIALES WHERE GRUPO_MATERIAL = '" + strGrupoMaterial + "' AND marca = '" + strMarca + "'";
        string Vkorg = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            Vkorg = campos["VKORG"].ToString();
        }
        
        return Vkorg;
    }

    public string GetVkorgByGrupoMaterial(string strGrupoMaterial) //05-2018
    {
        string SQL = "SELECT VKORG FROM GRUPO_MATERIALES WHERE GRUPO_MATERIAL = '" + strGrupoMaterial + "'";
        string Vkorg = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            Vkorg = campos["VKORG"].ToString();
        }

        return Vkorg;
    }
    /// <summary>
    /// Este método devuelve la marca, recibiendo como parametro el código del Grupo de material
    /// </summary>
    /// <param name="GrupoMaterial"></param>
    /// <returns></returns>
    public string GetMarcaByGrupoMateriales(string GrupoMaterial)
    {
        string SQL = "SELECT MARCA FROM GRUPO_MATERIALES WHERE GRUPO_MATERIAL= '" + GrupoMaterial + "'";
        string Marca = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            Marca = campos["MARCA"].ToString();
        }

        return Marca;
    }

    /// <summary>
    /// Este método devuelve la NCODE, recibiendo como parametro el rut del concesionario
    /// </summary>
    /// <param name="RutConcesionario"></param>
    /// <returns></returns>
    public string GetNcodeByRutConcesionario(string RutConcesionario)
    {
        string SQL = "SELECT TOP 1 NCODE FROM CONVERTION_CODES WHERE TYPE_CODE = '02' AND OCODE = '" + RutConcesionario + "'";
        string SapId = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            SapId = campos["NCODE"].ToString();
        }
       
        return SapId;
    }

    /// <summary>
    /// Este método devuelve rut del concesionario, recibiendo como parametro el codigo del concesionario
    /// </summary>
    /// <param name="DealerCode"></param>
    /// <returns></returns>
    public string GetRutConcesionarioByDealerCode(string DealerCode)
    {
        string SQL = "SELECT CUST_CODE FROM DEALERS WHERE DEALER_CODE='" + DealerCode + "'";
        string RutConcesionario = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            RutConcesionario = campos["CUST_CODE"].ToString();
        }

        return RutConcesionario;
    }

    /// <summary>
    /// Método que devuelve el código SPART, recibiendo como parametro el NCODE
    /// </summary>
    /// <param name="Ncode"></param>
    /// <returns></returns>
    public string GetSpartByNcode(string Ncode)
    {
        string SQL = "SELECT SPART FROM DESTINATARIOS_MERCANCIA WHERE CODIGO_SAP = '" + Ncode + "'";
        string Spart = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            Spart = campos["SPART"].ToString();
        }

        return Spart;
    }

    /// <summary>
    /// Método que devuelve el prefijo de la marca, recibiendo como parametro el código del grupo de material
    /// </summary>
    /// <param name="GrupoMaterial"></param>
    /// <returns></returns>
    public string GetPrefijoMarcaByGrupoMateriales(string GrupoMaterial, string Marca)
    {
        string SQL = "SELECT PREFIJO FROM GRUPO_MATERIALES WHERE GRUPO_MATERIAL = '" + GrupoMaterial + "' AND MARCA = '" + Marca + "'";
        string PrefijoMarca = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            PrefijoMarca = campos["PREFIJO"].ToString();
        }

        return PrefijoMarca;
    }

    public string GetPrefijoMarcaByGrupoMateriales(string GrupoMaterial)
    {
        string SQL = "SELECT PREFIJO FROM GRUPO_MATERIALES WHERE GRUPO_MATERIAL = '" + GrupoMaterial + "'";
        string PrefijoMarca = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            PrefijoMarca = campos["PREFIJO"].ToString();
        }

        return PrefijoMarca;
    }

    /// <summary>
    /// Método que devuelve el primer prefijo de la marca, recibiendo como parametro el código Vkorg
    /// </summary>
    /// <param name="Vkorg"></param>
    /// <returns></returns>
    public string GetPrefijoMarcaByOrganizacionDeVentas(string Vkorg)
    {

        string SQL = "SELECT TOP 1 PREFIJO FROM GRUPO_MATERIALES WHERE VKORG = '" + Vkorg + "'";
        string PrefijoMarca = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            PrefijoMarca = campos["PREFIJO"].ToString();
        }

        return PrefijoMarca;

    }

    /// <summary>
    /// Método que devuelve el primer SHIP_CODE, recibiendo como parametro el id del usuario (Userid)
    /// </summary>
    /// <param name="Userid"></param>
    /// <returns></returns>
    public string GetDestinatarioMercanciaByUserid(string nomDealer)
    {

        string SQL = @"select * from sucursal where nombreConcesionario = '"+nomDealer+"'";
        string shipCode = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            shipCode = campos["shipCode"].ToString();
        }


        return shipCode;

    }

    /// <summary>
    /// Método que obtiene el rut del concesionario a traves del rut del usuario
    /// </summary>
    /// <param name="rutUsuario"></param>
    /// <returns></returns>
    public string GetRutConcesionario(string rutUsuario)
    {
        string rutConcesionario = "";
        string SQL = @"SELECT concesionario.rutHolding 
                        FROM concesionario
                        INNER JOIN (sucursal INNER JOIN persona ON sucursal.shipCode=persona.shipCode AND persona.rut='"+rutUsuario+"') "
                       +"ON concesionario.nombreConcesionario=sucursal.nombreConcesionario";

        DataSet _ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach(DataRow campo in _ds.Tables[0].Rows)
        {
            rutConcesionario = campo["rutHolding"].ToString();
        }

        return rutConcesionario;
    }

    public string GetTextoMarcaFromSap(string idPedido)
    {
        if (ambiente == "ERP")
        {
           /* #region Ambiente ERP

            // Se asigna el ID del pedido
            _erp.WsDataConsultaPedido.I_VBELN = idPedido;
            _erp.WsDataConsulta.Pedido = _erp.WsDataConsultaPedido;

            try
            {
                if (SapAPI.DebugActivated)
                {
                    Debug.WriteLine("Ejecutando Consulta Pedido con ID " + _erp.WsDataConsulta.Pedido.I_VBELN);
                }

                // Se ejecuta el pedido
                _erp.WsConsultaPedido.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                _erp.WsConsultaPedido.PreAuthenticate = true;
                _erp.WsResponse3 = _erp.WsConsultaPedido.MI_WebS_Consul_Ped_Repues_Synch(_erp.WsDataConsulta);

                if (_erp.WsResponse3.Pedido.E_VBELN == "00000000")
                {

                    if (SapAPI.DebugActivated)
                    {
                        Debug.WriteLine("Pedido/Cotizacion " + idPedido + "no existe");
                    }

                    return null;

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en GetTextoMarcaFromSap: " + ex.Message);
            }

            return _erp.WsResponse3.Pedido.E_VTEXT;

            #endregion
        }
        else if (ambiente == "ERQ")
        {
            #region Ambiente ERQ

            // Se asigna el ID del pedido
            _erq.WsDataConsultaPedido.I_VBELN = idPedido;
            _erq.WsDataConsulta.Pedido = _erq.WsDataConsultaPedido;

            try
            {
                if (SapAPI.DebugActivated)
                {
                    Debug.WriteLine("Ejecutando Consulta Pedido con ID " + _erq.WsDataConsulta.Pedido.I_VBELN);
                }

                // Se ejecuta el pedido
                _erq.WsConsultaPedido.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                _erq.WsConsultaPedido.PreAuthenticate = true;
                _erq.WsResponse3 = _erq.WsConsultaPedido.MI_WebS_Consul_Ped_Repues_Synch(_erq.WsDataConsulta);

                if (_erq.WsResponse3.Pedido.E_VBELN == "00000000")
                {

                    if (SapAPI.DebugActivated)
                    {
                        Debug.WriteLine("Pedido/Cotizacion " + idPedido + "no existe");
                    }

                    return null;

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en GetTextoMarcaFromSap: " + ex.Message);
            }

            return _erq.WsResponse3.Pedido.E_VTEXT;

            #endregion*/
            return "";// _erq.WsResponse3.E_ESTADOS.ToString();
        }
        else { return ""; }
    }

    public int GetCurrentStockByProduct(string codigoRep, string GrupoMateriales, string destinatarioMerc, string texto, string Marca) {

        if (ambiente == "ERP")
        {

            #region Ambiente ERP
            int stockActual = 0;

           /* _erq.LocalDtWebRepuestosItem.I_KUNNR = destinatarioMerc;
            _erq.LocalDtWebRepuestosItem.I_KWMENG = "1"; // Cantidad fijada en 1 ya que es solo consulta
            _erq.LocalDtWebRepuestosItem.I_MAKTX = texto;
            _erq.LocalDtWebRepuestosItem.I_MFRPN = codigoRep;
            _erq.LocalDtWebRepuestosItem.I_MVGR4 = GrupoMateriales;
            _erq.LocalDtWebRepuestosItem.I_VKORG = GetVkorgByGrupoMaterial(GrupoMateriales);
            _erq.LocalDtWebRepuestosItem.I_VTWEG = CanalDeDistribucionPedido;

            _erq.LocalDtWebRepuestos.Item = _erq.LocalDtWebRepuestosItem;*/

            _erp.WsConsultaRepuesto.BeginSI_ConsultaRepuesto_oa("",
                                                                    destinatarioMerc,
                                                                    "1",
                                                                    texto,
                                                                    codigoRep,
                                                                    GrupoMateriales,
                                                                    "",
                                                                    GetVkorgByGrupoMaterial(GrupoMateriales, Marca),
                                                                    CanalDeDistribucionPedido,
                                                                    null,
                                                                    null);

                String dato1;
                String dato2;
                String dato3;
                String dato4;
                String dato5;
                String dato6;
                
                var MyArray = new ERP.ZEWS004[1];
                var MyArray1 = new ERP.ZEWS026[1];
                var MyArray2 = new ERP.ZEWS027[1];

              
			
            try
            {
                _erp.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential("INT_WTY_SKBP", "5k82017PoPpe");
                _erp.WsConsultaRepuesto.PreAuthenticate = true;
                _erp.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("", destinatarioMerc,
                                                                  "1",
                                                                  texto,
                                                                  codigoRep,
                                                                  GrupoMateriales,
                                                                  "",
                                                                  GetVkorgByGrupoMaterial(GrupoMateriales, Marca),
                                                                  CanalDeDistribucionPedido,
                                                                  out dato1,
                                                                  out dato2,
                                                                  out dato3,
                                                                  out dato4,
                                                                  out dato5,
                                                                  out dato6,
                                                                  out MyArray, out MyArray1, out MyArray2);
               // _erq.WsResponseConsultaRepuesto = _erq.WsConsultaRepuesto.MI_WebS_Repuestos_Synch(_erq.LocalDtWebRepuestos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en GetCurrentStockByProduct al ejecutar MI_WebS_Repuestos_Synch(). Err:" + ex.Message);
                return stockActual;
            }

            try
            {
                if (MyArray1.Length == 0)
                {
                    return stockActual;
                }
            }
            catch (NullReferenceException ex)
            {
                //MessageBox.Show("ERROR, conflicto codigo marca");
                Debug.Write(ex.Message);
            }

            try
            {
                string stock = "";
                //foreach (ERQ.DT_ERP_RepuestosT_ZESD002 wsRepuestoItem in _erq.WsResponseConsultaRepuesto.T_ZESD002) ERQ.ZEWS026 WsDatos in MyArray1
                foreach (ERP.ZEWS026 wsRepuestoItem in MyArray1)
                {
                    stock = wsRepuestoItem.EZ_COM_QTY.Trim().Substring(0, wsRepuestoItem.EZ_COM_QTY.Trim().Length - 4);
                    stockActual = int.Parse(stock.Trim());

                    // Si el stock es < a 10, entonces se devuelve el valor original de stock.
                    /*if (stockActual < 10)
                    {
                        double temp = Convert.ToDouble(stockActual) * 0.8;
                        stockActual = Convert.ToInt32(temp);
                    }*/
                }
            }
            catch (NullReferenceException ex) { Debug.Write(ex.Message); stockActual = -100; }

            return stockActual;
            #endregion

        }
        else if (ambiente == "ERQ")
        {

            #region Ambiente ERQ
            int stockActual = 0;

           /* _erq.LocalDtWebRepuestosItem.I_KUNNR = destinatarioMerc;
            _erq.LocalDtWebRepuestosItem.I_KWMENG = "1"; // Cantidad fijada en 1 ya que es solo consulta
            _erq.LocalDtWebRepuestosItem.I_MAKTX = texto;
            _erq.LocalDtWebRepuestosItem.I_MFRPN = codigoRep;
            _erq.LocalDtWebRepuestosItem.I_MVGR4 = GrupoMateriales;
            _erq.LocalDtWebRepuestosItem.I_VKORG = GetVkorgByGrupoMaterial(GrupoMateriales);
            _erq.LocalDtWebRepuestosItem.I_VTWEG = CanalDeDistribucionPedido;

            _erq.LocalDtWebRepuestos.Item = _erq.LocalDtWebRepuestosItem;*/

            _erq.WsConsultaRepuesto.BeginSI_ConsultaRepuesto_oa("",
                                                                    destinatarioMerc,
                                                                    "1",
                                                                    texto,
                                                                    codigoRep,
                                                                    GrupoMateriales,
                                                                    "",
                                                                    GetVkorgByGrupoMaterial(GrupoMateriales, Marca),
                                                                    CanalDeDistribucionPedido,
                                                                    null,
                                                                    null);

                String dato1;
                String dato2;
                String dato3;
                String dato4;
                String dato5;
                String dato6;
                
                var MyArray = new ERQ.ZEWS004[1];
                var MyArray1 = new ERQ.ZEWS026[1];
                var MyArray2 = new ERQ.ZEWS027[1];

              

            try
            {
                _erq.WsConsultaRepuesto.Credentials = new System.Net.NetworkCredential(ConfAmbiente.user, ConfAmbiente.pass);
                _erq.WsConsultaRepuesto.PreAuthenticate = true;
                _erq.WsConsultaRepuesto.SI_ConsultaRepuesto_oa("", destinatarioMerc,
                                                                   "1",
                                                                   texto,
                                                                   codigoRep,
                                                                   GrupoMateriales,
                                                                   "",
                                                                   GetVkorgByGrupoMaterial(GrupoMateriales, Marca),
                                                                   CanalDeDistribucionPedido,
                                                                   out dato1,
                                                                   out dato2,
                                                                   out dato3,
                                                                   out dato4,
                                                                   out dato5,
                                                                   out dato6,
                                                                   out MyArray, out MyArray1, out MyArray2);
               // _erq.WsResponseConsultaRepuesto = _erq.WsConsultaRepuesto.MI_WebS_Repuestos_Synch(_erq.LocalDtWebRepuestos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en GetCurrentStockByProduct al ejecutar MI_WebS_Repuestos_Synch(). Err:" + ex.Message);
                return stockActual;
            }

            try
            {
                if (MyArray1.Length == 0)
                {
                    return stockActual;
                }
            }
            catch (NullReferenceException ex)
            {
                //MessageBox.Show("ERROR, conflicto codigo marca");
                Debug.Write(ex.Message);
            }

            try
            {
                string stock = "";
                //foreach (ERQ.DT_ERP_RepuestosT_ZESD002 wsRepuestoItem in _erq.WsResponseConsultaRepuesto.T_ZESD002) ERQ.ZEWS026 WsDatos in MyArray1
                foreach (ERQ.ZEWS026 wsRepuestoItem in MyArray1)
                {
                    stock = wsRepuestoItem.EZ_COM_QTY.Trim().Substring(0, wsRepuestoItem.EZ_COM_QTY.Trim().Length - 4);
                    stockActual = int.Parse(stock.Trim());

                    // Si el stock es < a 10, entonces se devuelve el valor original de stock.
                    /*if (stockActual < 10)
                    {
                        double temp = Convert.ToDouble(stockActual) * 0.8;
                        stockActual = Convert.ToInt32(temp);
                    }*/
                }
            }
            catch (NullReferenceException ex) { Debug.Write(ex.Message); stockActual = -100; }

            return stockActual;
            #endregion

        }
        else
        {
            return 0;
        }
        return 1;
    }
    
    public string GetCodigoClienteSapByRut(string rutUser)
    {
        string SQL = @"select sucursal.numeroFactura
                        from sucursal,persona
                        where persona.shipcode=sucursal.shipcode
                        and persona.rut='" + rutUser + "'";

        string CodigoClienteSap = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            CodigoClienteSap = campos["numeroFactura"].ToString();
        }

        return CodigoClienteSap;

    }

    public string GetCodigoShipCode(string rutUser)
    {
        string SQL = @"select sucursal.shipCode
                        from sucursal,persona
                        where persona.shipcode=sucursal.shipcode
                        and persona.rut='" + rutUser + "'";

        string CodigoClienteSap = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(SQL);

        foreach (DataRow campos in ds.Tables[0].Rows)
        {
            CodigoClienteSap = campos["shipCode"].ToString();
        }

        return CodigoClienteSap;

    }

   

    public string GetNombreUsuario(string rut)
    {
        //Consulto la tabla persona para obtener el nombre
        string nombre = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from persona where rut = '"+rut+"'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        { 
            nombre = campo["nombre"].ToString();   
        }

        return nombre;
    }

    [Obsolete("Porfavor usar métodos implementados en clase estática Util")]
    public string GetNombreDealer(string rut)
    {
        string dealer = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select concesionario.nombreConcesionario 
                                                        from persona,sucursal,concesionario
                                                        where persona.rut='"+rut+"' "+
                                                        "AND persona.shipCode=sucursal.shipCode "+
                                                        "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            dealer = campo["nombreConcesionario"].ToString();
        }

        return dealer;
    }

    //obtener dirección sucursal por ShipCode
    public string GetDireccionSucursalByShipCode(string shipCode)
    {
        string direccionSuc = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from sucursal
                                                        where shipCode='" + shipCode + "' ");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            direccionSuc = campo["direccionSucursal"].ToString();
        }

        return direccionSuc;
    }
    //obtener dirección sucursal por rut de usuario
    public string GetDireccionSucursal(string rut)
    {
        string direccionSuc = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select sucursal.direccionSucursal
                                                        from persona,sucursal
                                                        where persona.rut='"+rut+"' "+
                                                        "AND persona.shipCode=sucursal.shipCode");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            direccionSuc = campo["direccionSucursal"].ToString();
        }

        return direccionSuc;
 
    }

    public string GetAbreviadoMarca(string marca)
    {
        string avMarca = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from marca where nombreMarca = '"+marca+"'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            avMarca = campo["abreviado"].ToString();
        }

        return avMarca;
    }
    
    public string GetCorreoUsuario(string rut)
    {
        string correo = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from persona where rut = '" + rut + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            correo = campo["email"].ToString();
        }

        return correo;

 
    }




    public string GetCorreosVFCUsuario(string rut)
    {
        string correosVFC = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select persona.email 
                                                        from persona,sucursal,concesionario
                                                        where persona.rut='" + rut + "' " +
                                                        "AND persona.shipCode=sucursal.shipCode " +
                                                        "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            correosVFC = campo["email"].ToString();
        }

        return correosVFC;
    }

    public string GetCorreosVFCConcesionario(string rut)
    {
        string correosVFC = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select concesionario.correosVFC
                                                        from persona,sucursal,concesionario
                                                        where persona.rut='" + rut + "' " +
                                                        "AND persona.shipCode=sucursal.shipCode " +
                                                        "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            correosVFC = campo["correosVFC"].ToString();
        }

        return correosVFC;
    }




    public string GetTotalNeto(string numPedido)
    {
        string totalNeto = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where E_VBELN = '" + numPedido + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            totalNeto = campo["TOTAL_NETO"].ToString();
        }

        return totalNeto;
    }

    public string GetUltimoNumDescarte(string numPedido)
    {
        string numDescarte = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select top 1 idDescartado from DESCARTADOS order by idDescartado desc ");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numDescarte = campo["idDescartado"].ToString();
        }

        return numDescarte;
    }

    /// <summary>
    /// Método que devuelve un bool, si es fasle el pedido no existe, si es true el pedido xiste. Recibe 
    /// como parametro el idPedido
    /// </summary>
    /// <param name="idPedido"></param>
    /// <returns></returns>
    public bool PedidoConfirmado(string idPedido)
    {
        string E_VBELN_PEDIDO = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where id_Pedido = '" + idPedido + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            E_VBELN_PEDIDO = campo["E_VBELN_PEDIDO"].ToString();
        }
        if (E_VBELN_PEDIDO == "")
        {
            return false;
        }

        else {
            return true;
        }

        
    }

    /// <summary>
    /// Método que devuelve un bool, si es fasle la cotización no existe, si es true la cotización existe.
    /// Recibe como parametro el isPedido.
    /// </summary>
    /// <param name="idPedido"></param>
    /// <returns></returns>
    public bool CotizacionExiste(string idPedido)
    {
        string E_VBELN = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where id_Pedido = '" + idPedido + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            E_VBELN = campo["E_VBELN"].ToString();
        }
        if (E_VBELN == "")
        {
            return false;
        }

        else
        {
            return true;
        }


    }

    public DataSet GetSugeridoPorUser(string rut)
    {
        DataSet _dSet = _controlBD.ObtenerDatosFiltrados(@"select top 10 MATERIALES_PEDIDO.marca, 
                                   MATERIALES_PEDIDO.codigo as codigo , MATERIALES_PEDIDO.descripcion, 
                                   sum(MATERIALES_PEDIDO.cantidad) as cantidadSum
                                    from PEDIDO, MATERIALES_PEDIDO
                                    where FECHA_SOLICITUD between (select GETDATE()-30) and (select GETDATE())
                                    AND PEDIDO.ID_PEDIDO= MATERIALES_PEDIDO.id_pedido
                                    AND PEDIDO.SOLICITADO_POR = '" + rut+"' "+
                                   " group by MATERIALES_PEDIDO.marca,MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion " +
                                   " order by cantidadSum desc");
        return _dSet;
    }

    public string GetNumCotizacionPorNpedido(string numPedido)
    {
        string numCotizacion = "";

        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from pedido where E_VBELN_PEDIDO = '"+numPedido+"'");
        foreach(DataRow campos in ds.Tables[0].Rows)
        {
            numCotizacion = campos["E_VBELN"].ToString();
        }

        return numCotizacion;
    }

    public string getIdPedidoPorE_VBELN(string E_VBELN)
    {
        string idPedido = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from pedido where E_VBELN = '"+E_VBELN+"'");
        foreach (DataRow idPed in ds.Tables[0].Rows)
        {
            idPedido = idPed["id_pedido"].ToString();
        }

        return idPedido;
    }

    public string getIdPedidoPorE_VBELN_PEDIDO(string E_VBELN_PEDIDO)
    {
        string idPedido = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from pedido where E_VBELN_PEDIDO = '" + E_VBELN_PEDIDO + "'");
        foreach (DataRow idPed in ds.Tables[0].Rows)
        {
            idPedido = idPed["id_pedido"].ToString();
        }

        return idPedido;
    }

    public string getE_VBELN_PEDIDO_porIDpedido(string idPedido)
    {
        string E_VBELN_PEDIDO = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from pedido where ID_PEDIDO = '" + idPedido + "'");
        foreach (DataRow idPed in ds.Tables[0].Rows)
        {
            E_VBELN_PEDIDO = idPed["E_VBELN_PEDIDO"].ToString();
        }

        return E_VBELN_PEDIDO;
    }

    /// <summary>
    /// Este método determina si el carro esta vacio en una sesión, para poder recuperarlo 
    /// </summary>
    /// <param name="idSession"></param>
    /// <returns></returns>
    public bool OnCarroVacio(string idSession)
    {
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '"+idSession+"'");
        if (ds.Tables[0].Rows.Count == 0)
        {
            return false;
        }
        else 
        {
            return true;
        }
        
        
    }

    public bool OnCarroNoStockVacio(string idSession)
    {
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '" + idSession + "'");
        if (ds.Tables[0].Rows.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }


    }

    public bool DeterminarMultiSucursal(string rut)
    {

        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from persona where rut = '" + rut + "' and multipleSucursal = '1' ");

        if (ds.Tables[0].Rows.Count == 0)
        {
            return false;
        }
        else {
            return true; 
        }
        
    }

    public string GetDireccionPorShipCod(string shipcod, string idsucursal, int PerUsu)
    {
        string direccion = "";

        if (PerUsu == 1)
        {
            DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from sucursal where shipCode = '" + shipcod + "'");   
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                direccion = dr["direccionSucursal"].ToString();
            }
        }
        else
        {
            DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from sucursal where shipCode = '" + shipcod + "' and idsucursal =" + idsucursal);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                direccion = dr["direccionSucursal"].ToString();
            }
        }

        
       

        return direccion;
    }

    public string GetIdSucursalRut(string rutusr)
    {
        string IdSucursal = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select idSucursal from persona where usuario = '" + rutusr + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            IdSucursal = dr["idSucursal"].ToString();
        }

        return IdSucursal;
    }


    public string EncontreMaterial(string idsession, string codrep)
    {
        string total = "";
        string codrepf = "";
        if (codrep == null)
        {
            codrep = "";
        }
        else
        {
            codrepf = codrep.Substring(3, codrep.Length - 3);
        }
        

        DataSet ds = _controlBD.ObtenerDatosFiltrados("select count(*) as total from LISTA_BUSQUEDA_TMP where id_session = '" + idsession + "' and codigo = '" + codrepf + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            total = dr["total"].ToString();
        }

        return total;
    }




    public string GetCmpCod(string marca)
    {
        string cmpCod = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from GRUPO_MATERIALES where marca = '" + marca + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            cmpCod = dr["cmpy_code"].ToString();
        }

        return cmpCod;
    }

    public string GetShipCodeOfPedido(string numPed)
    {
        string shipCode = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from pedido where E_VBELN_PEDIDO = '" + numPed + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            shipCode = dr["I_KUNNR2"].ToString();
        }

        return shipCode;
    }

    public bool ValidarDealerExiste(string codCliSap)
    {
        bool ccs = false;
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from concesionario where numeroFactura = '"+codCliSap+"'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            if (codCliSap == dr["numeroFactura"].ToString())
            {
                ccs = true;
            }
            else {
                ccs = false;
            }
        }
        return ccs;
    }


    public string GetMarcaPorPrefijo(string prefijo)
    {
        string grupoMaterial = "";
        DataSet ds = _controlBD.ObtenerDatosFiltrados(" SELECT TOP 1 nombreMarca, GRUPO_MATERIAL " +
                                                        "FROM " +
                                                        "dbo.marca, " +
                                                        "dbo.GRUPO_MATERIALES " +
                                                        "WHERE " +
                                                        "(abreviado LIKE '" + prefijo + "%') " +
                                                        "AND " +
                                                        "nombreMarca = MARCA " +
                                                        "AND " +
                                                        "GRUPO_MATERIAL<> '' ");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            grupoMaterial = dr["nombreMarca"].ToString();
        }

        return grupoMaterial;
    }
}
