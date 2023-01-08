using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for registroSolicitud
/// </summary>
public class registroSolicitud
{
    private String id_solicitud = "";
    private String fecha_creacion = "";
    private String cantidad = "";
    private String producto = "";
    private String vin = "";
    private String marca = "";
    private String creador = "";
    private String fecha_eta = "";
    private String dealer = "";
    private String direccion = "";
    private String estado = "";
    private String concesionario = "";
    private String local = "";
    private String tipovfc = "";
    private String tipoPedido = "";
    private String observacion = "";

    public String TipoPedido
    {
        get { return tipoPedido; }
        set { tipoPedido = value; }
    }

    public String Observacion
    {
        get { return observacion; }
        set { observacion = value; }
    }

    public String Id_solicitud
    {
        get { return id_solicitud; }
        set { id_solicitud = value; }
    }
    
    public String Fecha_creacion
    {
        get { return fecha_creacion; }
        set { fecha_creacion = value; }
    }
    
    public String Cantidad
    {
        get { return cantidad; }
        set { cantidad = value; }
    }
    
    public String Producto
    {
        get { return producto; }
        set { producto = value; }
    }
    
    public String Vin
    {
        get { return vin; }
        set { vin = value; }
    }
    
    public String Marca
    {
        get { return marca; }
        set { marca = value; }
    }
    
    public String Creador
    {
        get { return creador; }
        set { creador = value; }
    }
    
    public String Fecha_eta
    {
        get { return fecha_eta; }
        set { fecha_eta = value; }
    }
    private String desc_usuario = "";

    public String Desc_usuario
    {
        get { return desc_usuario; }
        set { desc_usuario = value; }
    }
    
    public String Dealer
    {
        get { return dealer; }
        set { dealer = value; }
    }
    
    public String Direccion
    {
        get { return direccion; }
        set { direccion = value; }
    }
    
    public String Estado
    {
        get { return estado; }
        set { estado = value; }
    }

    public String Concesionario
    {
        get { return concesionario; }
        set { concesionario = value; }
    }
    
    public String Local
    {
        get { return local; }
        set { local = value; }
    }

    public String Tipovfc
    {
        get { return tipovfc; }
        set { tipovfc = value; }
    }

	public registroSolicitud()
	{

	}
}