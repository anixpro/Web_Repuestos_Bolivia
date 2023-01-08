using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IndicadorNivelCompra
/// </summary>
public class IndicadorNivelCompra
{
    public int IdIndicador { get; set; }
    public Concesionario Concesionario { get; set; }
    public Marca Marca { get; set; }
    public int Anio { get; set; }
    public int Mes { get; set; }
    public String MesExtenso { get; set; }
    public int Compras { get; set; }
    public int Metas { get; set; }

	public IndicadorNivelCompra()
	{

    }
}
