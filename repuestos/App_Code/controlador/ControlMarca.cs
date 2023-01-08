using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ControlMarca
/// </summary>
public class ControlMarca
{
    Marca _marca;

	public ControlMarca()
	{
        _marca = new Marca();
	}

    public DataSet obtieneMarcasModelosPorNombreConcesionario(String nombreConcesionario)
    {
        return _marca.obtieneMarcasModelosPorRutConcesionario(nombreConcesionario);
    }

    public DataSet obtieneMarcasPorNombreConcesionario(String nombreConcesionario)
    {
        return _marca.obtieneMarcasPorRutConcesionario(nombreConcesionario);
    }

    public List<Marca> obtenerMarcas()
    {
        return _marca.obtenerTodos();
    }

    public Marca obtenerMarca(string id)
    {
        // Si encuentra lo retorna
        if (_marca.obtenerPorId(id).Count > 0)
        {
            return _marca.obtenerPorId(id)[0];
        }
        return null;
    }

    public Marca obtenerMarcaPorNombre(String nombre)
    {
        // Si encuentra lo retorna
        if (_marca.obtenerPorNombre(nombre).Count > 0)
        {
            return _marca.obtenerPorNombre(nombre)[0];
        }
        return null;
    }
}