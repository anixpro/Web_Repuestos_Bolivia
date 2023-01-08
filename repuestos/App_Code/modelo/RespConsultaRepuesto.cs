using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de RespConsultaRepuesto
/// </summary>
public class RespConsultaRepuesto
{
    string _marca; //EZ_BEZEI;
    string _cantidad; //EZ_COM_QTY;
    string _precioLista; //EZ_KBETR1;
    string _precioConce; //EZ_KBETR2;
    string _grupoMat; //EZ_KONDM;
    string _descripcion; //EZ_MAKTX;
    string _codigo; //EZ_MFRPN;
    int _stock;

	public RespConsultaRepuesto()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public string Marca
    {
        get { return _marca; }
        set { _marca = value; }
    }

    public string Cantidad
    {
        get { return _cantidad; }
        set { _cantidad = value; }
    }

    public string PrecioLista
    {
        get { return _precioLista; }
        set { _precioLista = value; }
    }

    public string PrecioConce
    {
        get { return _precioConce; }
        set { _precioConce = value; }
    }

    public string GrupoMat
    {
        get { return _grupoMat; }
        set { _grupoMat = value; }
    }

    public string Descripcion
    {
        get { return _descripcion; }
        set { _descripcion = value; }
    }

    public string Codigo
    {
        get { return _codigo; }
        set { _codigo = value; }
    }


    public int Stock
    {
        get { return _stock; }
        set { _stock = value; }
    }
}