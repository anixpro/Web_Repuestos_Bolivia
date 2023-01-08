using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Clase que permite generar una cotización
/// </summary>
public class CrearCotizacion
{

    //datos cotización
    string _claseDocVentas; // I_AUART;
    string _fecValides; // I_BNDDT;
    string _codClienteSap; // I_KUNNR;
    string _codClienteSap2; // I_KUNNR2;
    string _spartRep; // I_SPART;
    string _descripCotizacion; // I_TEXTO;
    string _orgVentas; // I_VKORG;
    string _cDistribucion; // I_VTWEG;
    string _MtvoPedido;

    //Lista de repuestos
    List<string> _cantidad;
    List<string> _codigo;


	public CrearCotizacion()
	{
        _cantidad = new List<string>();
        _codigo = new List<string>();
	}


    public string ClaseDocVentas
    {
        get { return _claseDocVentas; }
        set { _claseDocVentas = value; }
    }

    public string FecValides
    {
        get { return _fecValides; }
        set { _fecValides = value; }
    }

    public string CodClienteSap
    {
        get { return _codClienteSap; }
        set { _codClienteSap = value; }
    }

    public string CodClienteSap2
    {
        get { return _codClienteSap2; }
        set { _codClienteSap2 = value; }
    }

    public string SpartRep
    {
        get { return _spartRep; }
        set { _spartRep = value; }
    }

    public string DescripCotizacion
    {
        get { return _descripCotizacion; }
        set { _descripCotizacion = value; }
    }

    public string OrgVentas
    {
        get { return _orgVentas; }
        set { _orgVentas = value; }
    }

    public string CDistribucion
    {
        get { return _cDistribucion; }
        set { _cDistribucion = value; }
    }

    public string MtvoPedido
    {
        get { return _MtvoPedido; }
        set { _MtvoPedido = value; }
    }

    public List<string> Cantidad
    {
        get { return _cantidad; }
        set { _cantidad = value; }
    }

    public List<string> Codigo
    {
        get { return _codigo; }
        set { _codigo = value; }
    }
}