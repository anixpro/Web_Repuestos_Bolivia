using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de AmbienteERQ
/// </summary>
public class AmbienteERQ
{
    //CHILE 
    //Variables para la consulta de repuestos
    //ERQ.DT_Web_Repuestos _localDtWebRepuestos = new ERQ.DT_Web_Repuestos();

    ERQ.ZEWS004 _localDtWebRepuestos = new ERQ.ZEWS004();
    public ERQ.ZEWS004 LocalDtWebRepuestos
    {
        get { return _localDtWebRepuestos; }
        set { _localDtWebRepuestos = value; }
    }

    //ERQ.DT_Web_RepuestosItem _localDtWebRepuestosItem = new ERQ.DT_Web_RepuestosItem();
    ERQ.ZEWS026 _localDtWebRepuestosItem = new ERQ.ZEWS026();
    public ERQ.ZEWS026 LocalDtWebRepuestosItem
    {
        get { return _localDtWebRepuestosItem; }
        set { _localDtWebRepuestosItem = value; }
    }

    //ERQ.ZB_CONSUL_REP _WsConsultaRepuesto = new ERQ.ZB_CONSUL_REP();
    ERQ.SI_ConsultaRepuesto_oaService _WsConsultaRepuesto = new ERQ.SI_ConsultaRepuesto_oaService();
    public ERQ.SI_ConsultaRepuesto_oaService WsConsultaRepuesto
    {
        get { return _WsConsultaRepuesto; }
        set { _WsConsultaRepuesto = value; }
    }

    

    
    //ERQ.DT_ERP_Repuestos _WsResponseConsultaRepuesto = new ERQ.DT_ERP_Repuestos();
    ERQ.ZEWS027 _WsResponseConsultaRepuesto = new ERQ.ZEWS027();
    public ERQ.ZEWS027 WsResponseConsultaRepuesto
    {
        get { return _WsResponseConsultaRepuesto; }
        set { _WsResponseConsultaRepuesto = value; }
    }
    //CHILE

    //Variables para la generación de cotizaciones
    //ERQ.ZB_COTIZACION _WsIngresoCotizacion = new ERQ.ZB_COTIZACION();
    /* ERQ.SI_Generacion_Cotizacion_Interna_OutService _WsIngresoCotizacion = new ERQ.SI_Generacion_Cotizacion_Interna_OutService();
     public ERQ.SI_Generacion_Cotizacion_Interna_OutService WsIngresoCotizacion
     {
         get { return _WsIngresoCotizacion; }
         set { _WsIngresoCotizacion = value; }
     }

     //ERQ.DT_Web_Ingreso_Cotizacion _DtWebIngresoCotizacion = new ERQ.DT_Web_Ingreso_Cotizacion();
     ERQ.DT_Generacion_Cotizacion_Interna_Response _DtWebIngresoCotizacion = new ERQ.DT_Generacion_Cotizacion_Interna_Response();
     public ERQ.DT_Generacion_Cotizacion_Interna_Response DtWebIngresoCotizacion
     {
         get { return _DtWebIngresoCotizacion; }
         set { _DtWebIngresoCotizacion = value; }
     }

    // ERQ.DT_Web_Ingreso_CotizacionItem _DtWebIngresoCotizacionItem = new ERQ.DT_Web_Ingreso_CotizacionItem();
     ERQ.DT_Generacion_Cotizacion_Interna_Request _DtWebIngresoCotizacionItem = new ERQ.DT_Generacion_Cotizacion_Interna_Request();
     public ERQ.DT_Generacion_Cotizacion_Interna_Request DtWebIngresoCotizacionItem
     {
         get { return _DtWebIngresoCotizacionItem; }
         set { _DtWebIngresoCotizacionItem = value; }
     }

     //ERQ.DT_ERP_Cotizacion _WsResponse = new ERQ.DT_ERP_Cotizacion();
     ERQ.DT_Generacion_Cotizacion_Interna_Response _WsResponse = new ERQ.DT_Generacion_Cotizacion_Interna_Response();
     public ERQ.DT_Generacion_Cotizacion_Interna_Response WsResponse
     {
         get { return _WsResponse; }
         set { _WsResponse = value; }
     }
     */
    //Variables para la confirmacion del pedido
    // ERQ.ZB_CREAR_PED _WsCreaPedido = new ERQ.ZB_CREAR_PED();
    ERQ.DT_Generacion_Pedido_Venta_RequestORDER_HEADER_IN _WsCreaPedido = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_HEADER_IN();
    public ERQ.DT_Generacion_Pedido_Venta_RequestORDER_HEADER_IN WsCreaPedido
    {
        get { return _WsCreaPedido; }
        set { _WsCreaPedido = value; }
    }

    ERQ.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN _WsCreaPedido_1 = new ERQ.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN();
    public ERQ.DT_Generacion_Pedido_Venta_RequestORDER_ITEMS_IN WsCreaPedido_1
    {
        get { return _WsCreaPedido_1; }
        set { _WsCreaPedido_1 = value; }
    }

    ERQ.DT_Generacion_Pedido_Venta_Request _WsCreaPedidoItem = new ERQ.DT_Generacion_Pedido_Venta_Request();
    public ERQ.DT_Generacion_Pedido_Venta_Request WsCreaPedidoItem
    {
        get { return _WsCreaPedidoItem; }
        set { _WsCreaPedidoItem = value; }
    }

    ERQ.SI_Generacion_Pedido_Venta_OutService _WsCreaPedidoVenta = new ERQ.SI_Generacion_Pedido_Venta_OutService();
    public ERQ.SI_Generacion_Pedido_Venta_OutService WsCreaPedidoVenta
    {
        get { return _WsCreaPedidoVenta; }
        set { _WsCreaPedidoVenta = value; }
    }

    ERQ.DT_Generacion_Pedido_Venta_Response _WsCreaPedidoResponse = new ERQ.DT_Generacion_Pedido_Venta_Response();
    public ERQ.DT_Generacion_Pedido_Venta_Response WsCreaPedidoResponse
    {
        get { return _WsCreaPedidoResponse; }
        set { _WsCreaPedidoResponse = value; }
    }

   
    ///FIN MOD



    //ERQ.DT_Web_PdoVtas _dtWebPdoVtas = new ERQ.DT_Web_PdoVtas();
    ERQ.DT_Generacion_Pedido_Venta_Response _dtWebPdoVtas = new ERQ.DT_Generacion_Pedido_Venta_Response();
    public ERQ.DT_Generacion_Pedido_Venta_Response DtWebPdoVtas
    {
        get { return _dtWebPdoVtas; }
        set { _dtWebPdoVtas = value; }
    }

    //ERQ.DT_Web_PdoVtasPedido _dtWebPdoVtasPedido = new ERQ.DT_Web_PdoVtasPedido();
    //ERQ.DT_Generacion_Pedido_Venta_Request _dtWebPdoVtasPedido = new ERQ.DT_Generacion_Pedido_Venta_Request();
    ERQ.DT_Generacion_Pedido_Venta_Request _dtWebPdoVtasPedido = new ERQ.DT_Generacion_Pedido_Venta_Request();
    public ERQ.DT_Generacion_Pedido_Venta_Request DtWebPdoVtasPedido
    {
        get { return _dtWebPdoVtasPedido; }
        set { _dtWebPdoVtasPedido = value; }
    }

    //ERQ.DT_ERP_PdoVtas _wsResponse = new ERQ.DT_ERP_PdoVtas();
    ERQ.DT_Generacion_Pedido_Venta_Response _wsResponse = new ERQ.DT_Generacion_Pedido_Venta_Response();
    public ERQ.DT_Generacion_Pedido_Venta_Response WsResponse1
    {
        get { return _wsResponse; }
        set { _wsResponse = value; }
    }



    // consultar estado
    ERQ.SI_Consulta_Estado_Pedido_Venta_OutService _wsConsultaPedido = new ERQ.SI_Consulta_Estado_Pedido_Venta_OutService();
    public ERQ.SI_Consulta_Estado_Pedido_Venta_OutService WsConsultaPedido
    {
        get { return _wsConsultaPedido; }
        set { _wsConsultaPedido = value; }
    }

    
    /*

    ERQ.DT_Web_PdoVtas_Consul _wsDataConsulta = new ERQ.DT_Web_PdoVtas_Consul();
    public ERQ.DT_Web_PdoVtas_Consul WsDataConsulta
    {
        get { return _wsDataConsulta; }
        set { _wsDataConsulta = value; }
    }

    ERQ.DT_Web_PdoVtas_ConsulPedido _wsDataConsultaPedido = new ERQ.DT_Web_PdoVtas_ConsulPedido();
    public ERQ.DT_Web_PdoVtas_ConsulPedido WsDataConsultaPedido
    {
        get { return _wsDataConsultaPedido; }
        set { _wsDataConsultaPedido = value; }
    }

    // Variables de la respuesta del pedido
    ERQ.DT_ERP_PdoVtas_Consul _wsResponse3 = new ERQ.DT_ERP_PdoVtas_Consul();
    public ERQ.DT_ERP_PdoVtas_Consul WsResponse3
    {
        get { return _wsResponse3; }
        set { _wsResponse3 = value; }
    }

    ERQ.DT_ERP_PdoVtas_ConsulPedido _wsResponsePedido = new ERQ.DT_ERP_PdoVtas_ConsulPedido();
    public ERQ.DT_ERP_PdoVtas_ConsulPedido WsResponsePedido
    {
        get { return _wsResponsePedido; }
        set { _wsResponsePedido = value; }
    }

    ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores _wsResponseErrores = new ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores();
    public ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores WsResponseErrores
    {
        get { return _wsResponseErrores; }
        set { _wsResponseErrores = value; }
    }*/
    /*  //ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores _wsResponseErrores = new ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores();
      ERQ.DT_Errores2 _wsResponseErrores = new ERQ.DT_Errores2();
      public ERQ.DT_Errores2 WsResponseErrores
      {
          get { return _wsResponseErrores; }
          set { _wsResponseErrores = value; }
      }*/
}