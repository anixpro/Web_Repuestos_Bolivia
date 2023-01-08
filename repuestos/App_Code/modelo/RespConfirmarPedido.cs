using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de RespConfirmarPedido
/// </summary>
public class RespConfirmarPedido
{
    string _numPedido;
    
	public RespConfirmarPedido()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}


    public string NumPedido
    {
        get { return _numPedido; }
        set { _numPedido = value; }
    }
}