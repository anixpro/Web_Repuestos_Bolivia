using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlAccesorio
/// </summary>
public class ControlAccesorio
{
    Accesorio _accesorio;

	public ControlAccesorio()
	{
        _accesorio = new Accesorio();
	}

    public List<Accesorio> obtenerAccesorios()
    {
        return _accesorio.obtenerTodos();
    }

    public Accesorio obtenerAccesorio(string id)
    {
        // Si encuentra lo retorna
        if (_accesorio.obtenerPorId(id).Count > 0)
        {
            return _accesorio.obtenerPorId(id)[0];
        }
        return null;
    }

    public Boolean siExsite(String codigoAccesorio, String marca)
    {
        return _accesorio.obtenerPorCodigoyMarca(codigoAccesorio, marca).Count > 0;
    }
}