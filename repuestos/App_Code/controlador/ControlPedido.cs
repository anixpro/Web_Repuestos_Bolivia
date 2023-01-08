using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;

/// <summary>
/// Summary description for ControlPedido
/// </summary>
public class ControlPedido
{
    private Pedido pedido;
	public ControlPedido()
	{
        pedido = new Pedido();
	}

    public String detallePedidoPorCodigo(String codigo,int rutEnSesion,String marca,String desde,String hasta)
    {
        return pedido.detallePedidoPorCodigo(codigo, rutEnSesion,marca,desde,hasta);
    }
    //eligiendo concesionario en el caso de ser adminstrador
    public String detallePedidoPorCodigo(String codigo, String concesionario, String marca, String desde, String hasta)
    {
        return pedido.detallePedidoPorCodigo(codigo, concesionario, marca, desde, hasta);
    }
    //resumen por codigo
    public DataTable detallePedidoPorCodigoResumen(String codigo, int rutEnSesion, String marca, String desde, String hasta)
    {
        return pedido.detallePedidoPorCodigoResumen(codigo,rutEnSesion,marca,desde,hasta);
    }
    //resumen por codigo total
    public DataTable detallePedidoPorCodigoResumenTotal(String codigo, int rutEnSesion)
    {
        return pedido.detallePedidoPorCodigoResumenTotal(codigo, rutEnSesion);
    }
    //sin fecha ni marca 
    public String detallePedidoPorCodigo(String codigo, int rutEnSesion)
    {
        return pedido.detallePedidoPorCodigo(codigo, rutEnSesion);
    }
    public String detallePedidoPorRut(int rut, int rutEnSesion, String marca, String desde, String hasta)
    {
        return pedido.detallePedidoPorRut(rut, rutEnSesion, marca, desde, hasta);
    }
    //resumen por rut
    public DataTable detallePedidoPorRutResumen(int rut, int rutEnSesion, String marca, String desde, String hasta)
    {
        return pedido.detallePedidoPorRutResumen(rut, rutEnSesion, marca, desde, hasta);
    }
    //resumen por rut total
    public DataTable detallePedidoPorRutResumenTotal(int rut, int rutEnSesion)
    {
        return pedido.detallePedidoPorRutResumenTotal(rut, rutEnSesion);
    }
    //sin fecha ni marca
    public String detallePedidoPorRut(int rut, int rutEnSesion)
    {
        return pedido.detallePedidoPorRut(rut, rutEnSesion);
    }
    public String detallePedidoPorTodos(int rutEnSesion, String marca, String desde, String hasta)
    {
        return pedido.detallePedidoPorTodos(rutEnSesion,marca, desde, hasta);
    }
    public int[] detalleCotizacionVsCompraPorCodigo(String codigo, String marca, String desde, String hasta)
    {
        return pedido.detalleCotizacionVsCompraPorCodigo(codigo, marca, desde, hasta);
    }
    public int[] detalleCotizacionVsCompraPorCodigoSucursal(String codigoSucursal, String marca, String desde, String hasta)
    {
        return pedido.detalleCotizacionVsCompraPorCodigoSucursal(codigoSucursal, marca, desde, hasta);
    }
    public int[] detalleCotizacionVsCompraPorCodigoOperario(int rut, String marca, String desde, String hasta)
    {
        return pedido.detalleCotizacionVsCompraPorCodigoOperario(rut, marca, desde, hasta);
    }
    public int[] detalleCotizacionVsCompraPorTodos(String marca, String desde, String hasta)
    {
        return pedido.detalleCotizacionVsCompraPorTodos(marca, desde, hasta);
    }
    public int detalleFallidaPorCodigo(String codigo, String marca, String desde, String hasta)
    {
        return pedido.detalleFallidaPorCodigo(codigo,marca,desde, hasta);
    }
    public int detalleFallidaPorCodigoOperario(int codigoOperario, String marca, String desde, String hasta)
    {
        return pedido.detalleFallidaPorCodigoOperario(codigoOperario, marca, desde, hasta);
    }
    public int detalleFallidaPorTodos(String marca, String desde, String hasta)
    {
        return pedido.detalleFallidaPorTodos(marca, desde, hasta);
    }
    public String obtienePrefijoMarca(String marcaRecibida)
    {
        return pedido.obtienePrefijoMarca(marcaRecibida);
    }
    public Boolean obtieneSiCotizacionFueConfirmada(String numeroCotizacion, String idSession)
    {
        return pedido.siConfirmoPedido(numeroCotizacion, idSession);
    }

    public Boolean siEsForaneo(String numeroCotizacion)
    {
        return pedido.siEsForaneo(numeroCotizacion);
    }
}