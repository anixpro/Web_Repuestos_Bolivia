using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlConcesionarios
/// </summary>
public class ControlConcesionarios
{
    Concesionario _concesionario;
	public ControlConcesionarios()
	{
		_concesionario = new Concesionario();
	}

    public List<Concesionario> obtieneTodos()
    {
        return _concesionario.obtieneConcesionarios();
    }

    public Concesionario ObtienePorID(String ID)
    {
        List<Concesionario> resultado = _concesionario.obtieneConcesionarioPorID(ID);
        if (resultado.Count > 0)
        {
            return resultado[0];
        }
        return null;
    }
}