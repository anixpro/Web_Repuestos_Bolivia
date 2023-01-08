using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ConfirmarPedido
/// </summary>
public class ConfirmarPedido
{
    string _tipoPedido; //I_AUART
    string _prioridad; //I_LPRIO
    string _numCotizacion; //E_VBELN


	public ConfirmarPedido()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}


    public string TipoPedido
    {
        get { return _tipoPedido; }
        set { _tipoPedido = value; }
    }

    public string Prioridad
    {
        get { return _prioridad; }
        set { _prioridad = value; }
    }

    public string NumCotizacion
    {
        get { return _numCotizacion; }
        set { _numCotizacion = value; }
    }
}