using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for detalleSolicitud
/// </summary>
public class detalleSolicitud
{
    private String id_solicitud = "";

    public String Id_solicitud
    {
        get { return id_solicitud; }
        set { id_solicitud = value; }
    }
    private String id_seguimiento = "";

    public String Id_seguimiento
    {
        get { return id_seguimiento; }
        set { id_seguimiento = value; }
    }
    private String marca = "";

    public String Marca
    {
        get { return marca; }
        set { marca = value; }
    }
    private String codigo_repuesto = "";

    public String Codigo_repuesto
    {
        get { return codigo_repuesto; }
        set { codigo_repuesto = value; }
    }
    private String detalle = "";

    public String Detalle
    {
        get { return detalle; }
        set { detalle = value; }
    }
    private String fecha_movimiento = "";

    public String Fecha_movimiento
    {
        get { return fecha_movimiento; }
        set { fecha_movimiento = value; }
    }
    private String estado = "";

    public String Estado
    {
        get { return estado; }
        set { estado = value; }
    }
    private String fecha_eta = "";

    public String Fecha_eta
    {
        get { return fecha_eta; }
        set { fecha_eta = value; }
    }
    private String orden_compra = "";

    public String Orden_compra
    {
        get { return orden_compra; }
        set { orden_compra = value; }
    }
    private String orden_desarme = "";

    public String Orden_desarme
    {
        get { return orden_desarme; }
        set { orden_desarme = value; }
    }

    private String observacion = "";

    public String Observacion
    {
        get { return observacion; }
        set { observacion = value; }
    }


	public detalleSolicitud()
	{
        
	}
}