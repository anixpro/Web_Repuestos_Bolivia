using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlVfc
/// </summary>
public class ControlVfc
{
    private Vfc vfc;
    private string vin;

    public string Vin
    {
        get { return vin; }
        set { vin = value; }
    }

	public ControlVfc()
	{
	    vfc= new Vfc();	
	}
    
    public String getAnio()
    {
        return vfc.getAnio();
    }
    
    public String getVin()
    {
        return vfc.getVin();
    }
    
    public String getChasis()
    {
        return vfc.getChasis();
    }
    
    public String getMarca()
    {
        return vfc.getMarca();
    }
    
    public String getModelo()
    {
        return vfc.getModelo();
    }
    
    public String getVersion()
    {
        return vfc.getVersion();
    }
    
    //metodos vfc
    public void ObtieneDatosVfc()
    {
        vfc.setVin(this.Vin);
        //vfc.vin();
        //vfc.obtieneVehiculo();
    }

    public void ObtieneDatosVfc(string vinInput)
    {
        vfc.setVin(vinInput);
       // vfc.obtieneVehiculo();
    }

    public void hacerVfc(String cantidad, String codigo, String detalle, String marca, String vin,
                        String creador, String descripcionUsuario, String dealer,
                        String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String CC = "", String codsap="")
    {
        // se limpian los identificadores que corresponde al id de seguimiento
        vfc.limpiarIdentificadores();

        //LLAMO AL METODO QUE DEUVLVE UN XML, EL PRIMER STRING DEBE IR EN FORMA DE XML
        vfc.hacerVfc(cantidad, codigo, detalle, marca, vin,
                         creador, descripcionUsuario, dealer,
                         direccion, tipoPedido, opcionvfc, km, nSiniestro, CC, codsap);
    }

    public void hacerBo(String cantidad, String codigo, String detalle, String marca,
                    String creador, String descripcionUsuario, String dealer,
                    String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String CC="", String codsap="")
    {
        // se limpian los identificadores que corresponde al id de seguimiento
        vfc.limpiarIdentificadores();

        //LLAMO AL METODO QUE DEUVLVE UN XML, EL PRIMER STRING DEBE IR EN FORMA DE XML
        vfc.hacerVfc(cantidad, codigo, detalle, marca,
                         creador, descripcionUsuario, dealer,
                         direccion, tipoPedido, opcionvfc, km, nSiniestro, CC, codsap);
    }

    public List<String> getIdentificadores()
    {
        return vfc.getIdentificadores();
    }

    public List<String> getDetalles()
    {
        return vfc.getDetalles();
    }

    public List<registroSolicitud> getSolicitudesRepuestos(String usuario, String fechaDesde, String fechaHasta, String concesionario,
            String local, String estado, String opcionvfc, String tipoPedido)
    {
        vfc.obtenerSolicitudes(usuario, fechaDesde, fechaHasta, concesionario, local, estado, opcionvfc, tipoPedido);
        return vfc.RegistroSolicitudes;
    }

    public List<detalleSolicitud> getDetalleSolicitud(String idSolicitud)
    {
        vfc.obtenerDetalleSolicitud(idSolicitud);
        return vfc.DetalleSolicitudes;
    }
	
	public String VinGrupoporMarca(String marca){
		return  vfc.VinGrupoporMarca(marca);
	}	
}