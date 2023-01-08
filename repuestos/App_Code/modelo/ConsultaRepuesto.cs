using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ConsultaRepuesto
/// </summary>
public class ConsultaRepuesto
{

    private string _docVentas;// I_VKORG;
    private string _codRepeusto; // I_MFRPN;
    private int _cantidadRep; // I_KWMENG;
    private string _textoRep; // I_MAKTX;
    private string _grupoMaterial; // I_MVGR4;
    private string _canalDistribucion; // I_VTWEG;
    private string _destinaMercacia; // I_KUNNR;
    
	public ConsultaRepuesto()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}


    //Accesorios
    public string DocVentas
    {
        get { return this._docVentas; }
        set { _docVentas = value; }
    }

    public string CodRepuesto
    {
        get { return this._codRepeusto; }
        set { _codRepeusto = value; }
    }

    public int CantidadRep
    {
        get { return this._cantidadRep; }
        set { _cantidadRep = value; }
    }

    public string TextoRep
    {
        get { return this._textoRep; }
        set { _textoRep = value; }
    }

    public string GrupoMaterial
    {
        get { return this._grupoMaterial; }
        set { _grupoMaterial = value; }
    }

    public string CanalDistribucion
    {
        get { return _canalDistribucion; }
        set { _canalDistribucion = value; }
    }

    public string DestinaMercacia
    {
        get { return _destinaMercacia; }
        set { _destinaMercacia = value; }
    }



}