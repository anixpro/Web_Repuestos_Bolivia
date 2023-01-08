using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for ControlBoletin
/// </summary>
public class ControlBoletin
{
    Boletin modeloBoletin;

	public ControlBoletin()
	{
        modeloBoletin = new Boletin();
	}

    public DataSet obtenerBoletinesLeidosPorRut(String rut)
    {
        return modeloBoletin.obtenerBoletinesLeidosPorRut(rut);
    }

    public DataSet obtenerBoletinesPorFecha(String fecha)
    {
        return modeloBoletin.obtenerBoletinesPorFecha(fecha);
    }
}