using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de RespConsultaRepuesto
/// </summary>
public class RespCadenaReemplazo
{
    string _marca;
    string _cantidad;
    string _T_INTTYPE;
    string _T_KBETR1;
    string _T_KBETR2;
    string _T_KONDM;//grupoMat
    string _T_MAKTX;
    string _T_MFRPN;
    int _stock;

    public string Marca
    {
        get { return _marca; }
        set { _marca = value; }
    }
    
    public string T_INTTYPE
    {
        get { return _T_INTTYPE; }
        set { _T_INTTYPE = value; }
    }
    
    public string T_KBETR1
    {
        get { return _T_KBETR1; }
        set { _T_KBETR1 = value; }
    }
    
    public string T_KBETR2
    {
        get { return _T_KBETR2; }
        set { _T_KBETR2 = value; }
    }
    
    public string T_KONDM
    {
        get { return _T_KONDM; }
        set { _T_KONDM = value; }
    }
    
    public string T_MAKTX
    {
        get { return _T_MAKTX; }
        set { _T_MAKTX = value; }
    }
    
    public string T_MFRPN
    {
        get { return _T_MFRPN; }
        set { _T_MFRPN = value; }
    }
    
    public int Stock
    {
        get { return _stock; }
        set { _stock = value; }
    }

    public string Cantidad
    {
        get { return _cantidad; }
        set { _cantidad = value; }
    }


    public RespCadenaReemplazo()
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }

   
}