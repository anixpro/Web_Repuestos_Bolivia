using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlRepuestos
/// </summary>
public class ControlRepuestos
{
    Repuesto _repuesto = new Repuesto();
	public ControlRepuestos()
	{
		
	}

    public List<Repuesto> obtenerTodos()
    {
        return _repuesto.obtenerTodos();
    }

    public Repuesto obtenerRepuesto(String id)
    {
        List<Repuesto> reps= _repuesto.obtenerRepuesto(id);
        if (reps.Count > 0)
        {
            return reps[0];
        }
        else
        {
            return null;
        }
    }

    public Repuesto obtenerRepuestoPorCodigo(String codigo)
    {
        List<Repuesto> reps = _repuesto.obtenerRepuestoPorCodigo(codigo);
        if (reps.Count > 0)
        {
            return reps[0];
        }
        else
        {
            return null;
        }
    }

    public long grabarRepuesto(
        String imagen,
        String marca,
        String modelo,
        String concesionario,
        String contacto,
        String descripcion,
        String codigo,
        Boolean esNuevo,
        String telefono,
        String codigoArea,
        String correo
    )
    {
        Repuesto nuevoRepto = new Repuesto();
        nuevoRepto.esNuevo = esNuevo;
        nuevoRepto.CodigoRepto = codigo;
        nuevoRepto.Concesionario = concesionario;
        nuevoRepto.Contacto = contacto;
        nuevoRepto.Descripcion = descripcion;
        nuevoRepto.Imagen = imagen;
        nuevoRepto.Marca = marca;
        nuevoRepto.Modelo = modelo;
        nuevoRepto.Telefono = telefono;
        nuevoRepto.CodigoArea = codigoArea;
        nuevoRepto.Correo = correo;
        return _repuesto.grabarRepuesto(nuevoRepto);
    }

    public Boolean actualizaImagen(String imagen, String id)
    {
        return _repuesto.actualizaImagen(imagen,id);
    }

	public Boolean actualizaCantidadMin(String cantidad, String id){
		 return _repuesto.actualizaCantidadMin(cantidad,id);
	}
	
	public String detalleRepuesto(){
		 return _repuesto.detalleRepuesto();
	}
	public String detalleRepuestoporCodigo( String codigo){
		 return _repuesto.detalleRepuestoporCodigo(codigo);
	}
	public String detalleRepuestoPorMarcaCodigo(String marca , String codigo){
		return _repuesto.detalleRepuestoPorMarcaCodigo(marca ,codigo);
			
	}
	
	public String detalleRepuestoPorMarca(String marca){
		 return _repuesto.detalleRepuestoPorMarca(marca);
	}

    public Boolean eliminaRepto(String idRepto)
    {
        Repuesto repto = new Repuesto();
        return repto.eliminaRepto(idRepto);
    }
	
	public Boolean repuestoPorCantidad (String cantidad, String id){
		return _repuesto.repuestoPorCantidad(cantidad, id);
	}

    public String getCantidadporCodigo(String codigo)
    {
        return _repuesto.getCantidadporCodigo(codigo);
    }

}
