using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de RespCotizacionSap
/// </summary>
public class RespCotizacionSap
{
    string _numCotizacion; //E_VBELN

	public RespCotizacionSap()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public string NumCotizacion
    {
        get { return _numCotizacion; }
        set { _numCotizacion = value; }
    }
}