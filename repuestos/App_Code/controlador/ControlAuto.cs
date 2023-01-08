using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// Summary description for ControlAuto
/// </summary>
public class ControlAuto
{
    private Auto auto;
	public ControlAuto()
	{
        auto = new Auto();
	}

    public String[] obtenerConcesionario()
    {
        return (auto.concesionario());
    }
	
	public String[] obtenerMarca()
    {
        return (auto.marcas());
    }

    public List<Concesionario> obtenerConcesionariosIDs()
    {
        return auto.concesionariosIds();
    }

    public List<Sucursal> obtenerDireccionSucursal(String concesionario)
    {
        return auto.direccionSucursalPorConcesionario(concesionario);
    }
    public String obtenerShipCodeSucursal(String sucursal,String concesionario)
    {
        return auto.shipCode(sucursal, concesionario);
    }
    public String[] obtenerMarcasPorRut(int rut)
    {
        return (auto.marcaRut(rut));
    }
    public String obtenerconcesionarioPorRut(int rut)
    {
        return (auto.concesionarioRut(rut));
    }
    public int insertaConcesionario(String[] marcas, String zona, String nombre, String rutaImagen, String rutHolding, String numeroFactura, String rutSupervisor, String rutAdministrador, String correosVFC)
    {
        return auto.insertaConcesionario(marcas,zona, nombre, rutaImagen,rutHolding,numeroFactura,  rutSupervisor, rutAdministrador, correosVFC);
    }

    public int modificaConcesionario(String nombre, String rutaImagen)
    {
        return auto.modificaConcesionario(nombre, rutaImagen);
    }

    public int insertaSucursal(String nombreConcesionario, String comunaSucursal, String nombreSucursal, String shipCode, String direccionSucursal)
    {
        return auto.isertaSucursal(nombreConcesionario, comunaSucursal, nombreSucursal, shipCode, direccionSucursal);
    }
    public int insertaZona(String sector)
    {
        return auto.isertaZona(sector);
    }
    public String[] obtenerZona()
    {
        return (auto.obtenerZona());
    }
    public String[] marcas()
    {
        return (auto.todasLasMarcas());
    }
    public int insertaConcesionarioMarca(String[] marca, String concesionario)
    {
        return auto.insertaConcesionarioMarcas(marca, concesionario);
    }
    public String[] obtenerMarcasQueNoTengo(String marca)
    {
        return (auto.todasLasMarcasQueNoTengo(marca));
    }
    public int insertaNuevasMarcasConcesionario(String[] marcas, String concesionario)
    {
        return auto.insertaConcesionarioMarcas(marcas, concesionario);
    }
    public String[] marcasPorConcesionario(String concesionario)
    {
        return (auto.marcaConcesionario(concesionario));
    }
    public int eliminaMarcasConcesionario(String[] marcas, String concesionario)
    {
        return auto.eliminaConcesionarioMarcas(marcas, concesionario);
    }
    public String[] regiones()
    {
        return (auto.regiones());
    }
    public String[] provincias(String region)
    {
        return (auto.provincias(region));
    }
    public String[] comunas(String provincia)
    {
        return (auto.comunas(provincia));
    }
    public String obtenerAbreviado(String marca)
    {
        return auto.abreviado(marca);
    }
    public String detalleMarca()
    {
        return auto.detalleMarcas();
    }
    public String detalleMarcaPorConcesionario(String concesionario)
    {
        return auto.detalleMarcas(concesionario);
    }
    public String detalleMarcaPorMarca(String marca)
    {
        return auto.detalleMarcaPorMarca(marca);
    }
    public int eliminaMarca(String marca)
    {
        return auto.eliminaMarca(marca);
    }
    public int insertaMarca(
        String marca,
        String abreviado,
        String descripcion,
        String orgVenta,
        String grupoMaterial,
        String prefijoMarca,
        String codigoCompania,
        Boolean esForaneo
        )
    {
        return auto.insertaMarca(marca, abreviado, descripcion, orgVenta, grupoMaterial, prefijoMarca, codigoCompania, esForaneo);
    }
    //MODELO
    public String detalleModelo()
    {
        return auto.detalleModelo();
    }
    public String detalleModeloPorMarca(String marca)
    {
        return auto.detalleModeloPorMarca(marca);
    }
    public String detalleModeloPorModelo(String modelo)
    {
        return auto.detalleModeloPorModelo(modelo);
    }
    public int eliminaModelo(String modelo)
    {
        return auto.eliminaModelo(modelo);
    }
    public int insertaModelo(String marca, String nombreModelo, String descripcion)
    {
        return auto.insertaModelo(marca, nombreModelo, descripcion);
    }
    public string[] marcaContenido(string idContenido)
    {
        return auto.marcaContenido(idContenido);
    }
    public String[] todosLosAbreviado()
    {
        return auto.todosLosAbreviado();
    }
    // REQ - Cotizaciones Automaticas Marzo 2022
    public int habilitaCotizacionAutomatica(String abreviado, String marca)
    {
        return auto.habilitaCotizacionAutomatica(abreviado, marca);
    }
}
