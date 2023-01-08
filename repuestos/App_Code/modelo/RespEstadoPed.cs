using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de RespEstadoPed
/// </summary>
public class RespEstadoPed
{
    string _E_VKORG;
    string _E_VTEXT;
    string _E_VBELN;
    string _E_AUDAT;
    string _E_NETWR;
    string _E_KZWI5;
    string _E_TOTAL;

    

	public RespEstadoPed()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    public string E_VKORG
    {
        get { return _E_VKORG; }
        set { _E_VKORG = value; }
    }

    public string E_VTEXT
    {
        get { return _E_VTEXT; }
        set { _E_VTEXT = value; }
    }
    public string E_VBELN
    {
        get { return _E_VBELN; }
        set { _E_VBELN = value; }
    }

    public string E_AUDAT
    {
        get { return _E_AUDAT; }
        set { _E_AUDAT = value; }
    }

    public string E_NETWR
    {
        get { return _E_NETWR; }
        set { _E_NETWR = value; }
    }

    public string E_KZWI5
    {
        get { return _E_KZWI5; }
        set { _E_KZWI5 = value; }
    }
    public string E_TOTAL
    {
        get { return _E_TOTAL; }
        set { _E_TOTAL = value; }
    }


}