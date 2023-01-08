using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlSucursal
/// </summary>
public class ControlSucursal
{
    Sucursal _sucursal;
	public ControlSucursal()
	{
        _sucursal = new Sucursal();
	}
    public List<Sucursal> obtenerTodasSucursales()
    {
        return _sucursal.obtieneTodasSucursales();
    }
    public Sucursal obtenerSucursalPorShipCode(String shipCode)
    {
        List<Sucursal> sucursales = _sucursal.obtieneSucursalPorShipCode(shipCode);
        if (sucursales.Count > 0)
        {
            return sucursales[0];
        }
        return null;
    }
}