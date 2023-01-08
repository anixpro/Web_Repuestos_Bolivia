using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlMerchandising
/// </summary>
public class ControlMerchandising
{
    Merchandising _merchandising;

	public ControlMerchandising()
	{
        _merchandising = new Merchandising();
	}

    public List<Merchandising> obtenerMerchandising()
    {
        return _merchandising.obtenerTodos();
    }

    public Merchandising obtenerMerchandising(String idMerchandising)
    {
        List<Merchandising> retorno = _merchandising.obtenerPorId(idMerchandising);
        if (retorno.Count > 0)
        {
            return _merchandising.obtenerPorId(idMerchandising)[0];
        }
        else
        {
            return null;
        }
    }
}