using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for ControlIndicadores
/// </summary>
public class ControlIndicadores
{
    IndicadorEstadisticaDeCompra estadisticaCompra;
    IndicadorNivelCompra nivelCompra;
    IndicadorMetas metas;
    IndicadorCotizacionCompra cotizacionCompra;
    IndicadorEvolucionCompra evolucionCompra;
    indicadorFallidas fallidas;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

	public ControlIndicadores()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public void indicadorNivelDeCompra()
    {
        // holi
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompra(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String rutSolicitante,
            String marca,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompra(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            rutSolicitante,
            marca,
            codigo
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorMarcaYCodigo(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String marca,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorMarcaYCodigo(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            marca,
            codigo
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorConcesionario(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String concesionario,
            String marca,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorConcesionario(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            concesionario,
            marca,
            codigo
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaPorOperario(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String rutOperario
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompra(anioInicio,mesInicio,anioFin,mesFin,rutOperario);
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaTodos(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompra(anioInicio, mesInicio, anioFin, mesFin);
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaPorConcesionario(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String concesionario
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorConcesionario(
            anioInicio,mesInicio,anioFin,mesFin,concesionario
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaPorMarca(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String marca
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorMarca(
            anioInicio, mesInicio, anioFin, mesFin, marca
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaPorMarca(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String marca,
            String rut
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompra(
            anioInicio, mesInicio, anioFin, mesFin, rut, marca
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaPorCodigo(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorCodigo(
            anioInicio, mesInicio, anioFin, mesFin, codigo
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorRutYCodigo(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String codigo
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorRutYCodigo(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            rutSolicitante,
            codigo
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorRUTMarcaCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rut,
        String marca,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompra(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            rut,
            marca,
            codigo,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorMarcaCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraMarcaCodigoYSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            marca,
            codigo,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorMarcaYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraMarcaYSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            marca,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorRUTMarcaYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String marca,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraRUTMarcaYSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            RUT,
            marca,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraCodigoYSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            codigo,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorRUTCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraRUTCodigoYSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            RUT,
            codigo,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorRUTYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraRUTYSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            RUT,
            sucursal
        );
    }

    public List<IndicadorEstadisticaDeCompra> obtieneIndicadorEstadisticaCompraPorSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra();
        return estadisticaCompra.obtenerEstadisticasDeCompraPorSucursal(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String rutSolicitante,
            String marca,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            rutSolicitante,
            marca,
            codigo
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorMarcaYCodigoEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String marca,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorMarcaYCodigoEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            marca,
            codigo
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorConcesionarioEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String concesionario,
            String marca,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorConcesionarioEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            concesionario,
            marca,
            codigo
        );
    }

    public int obtieneIndicadorEstadisticaPorOperarioEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String rutOperario
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraEscalar(anioInicio, mesInicio, anioFin, mesFin, rutOperario);
    }

    public int obtieneIndicadorEstadisticaTodosEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraEscalar(anioInicio, mesInicio, anioFin, mesFin);
    }

    public int obtieneIndicadorEstadisticaPorConcesionarioEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String concesionario
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorConcesionarioEscalar(
            anioInicio, mesInicio, anioFin, mesFin, concesionario
        );
    }

    public int obtieneIndicadorEstadisticaPorMarcaEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String marca
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorMarcaEscalar(
            anioInicio, mesInicio, anioFin, mesFin, marca
        );
    }

    public int obtieneIndicadorEstadisticaPorMarcaEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String marca,
            String rut
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraEscalar(
            anioInicio, mesInicio, anioFin, mesFin, rut, marca
        );
    }

    public int obtieneIndicadorEstadisticaPorCodigoEscalar(
            String anioInicio,
            String mesInicio,
            String anioFin,
            String mesFin,
            String codigo
        )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorCodigoEscalar(
            anioInicio, mesInicio, anioFin, mesFin, codigo
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorRutYCodigoEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String codigo
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorRutYCodigoEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            rutSolicitante,
            codigo
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorRUTMarcaCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rut,
        String marca,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            rut,
            marca,
            codigo,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorMarcaCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraMarcaCodigoYSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            marca,
            codigo,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorMarcaYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraMarcaYSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            marca,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorRUTMarcaYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String marca,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraRUTMarcaYSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            RUT,
            marca,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraCodigoYSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            codigo,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorRUTCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String codigo,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraRUTCodigoYSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            RUT,
            codigo,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorRUTYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraRUTYSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            RUT,
            sucursal
        );
    }

    public int obtieneIndicadorEstadisticaCompraPorSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String sucursal
    )
    {
        estadisticaCompra = new IndicadorEstadisticaDeCompra(true);
        return estadisticaCompra.obtenerEstadisticasDeCompraPorSucursalEscalar(
            anioInicio,
            mesInicio,
            anioFin,
            mesFin,
            sucursal
        );
    }

    public List<IndicadorEvolucionCompra> obtenerEvolucionCompra(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String concesionario
    )
    {
        evolucionCompra = new IndicadorEvolucionCompra();
        return evolucionCompra.obtenerPorFechas(anioInicio, anioFin, mesInicio, mesFin,marca,concesionario);
    }
    public List<indicadorFallidas> obtenerFallidas(
            String anioInicio,
            String anioTermino,
            String mesInicio,
            String mesFin
        )
    {
        fallidas = new indicadorFallidas();
        return fallidas.obtenerFallidas(anioInicio, anioTermino, mesInicio, mesFin);
    }

    public List<indicadorFallidas> obtenerFallidas(
            String anioInicio,
            String anioTermino,
            String mesInicio,
            String mesFin,
            String codigo,
            String marca
        )
    {
        fallidas = new indicadorFallidas();
        return fallidas.obtenerFallidas(anioInicio, anioTermino, mesInicio, mesFin, codigo, marca);
    }

    public int[] obtenerCotiCompra(
        String anioInicio,
        String anioTermino,
        String mesInicio,
        String mesTermino,
        String concesionario,
        String sucursal
        )
    {
        cotizacionCompra = new IndicadorCotizacionCompra();
        return cotizacionCompra.obtenerCantidadCotizacionCompra(
            anioInicio,
            anioTermino,
            mesInicio,
            mesTermino,
            concesionario,
            sucursal
        );
    }

    public List<IndicadorCotizacionCompra> obtenerTodoCotiCompra(
            String anioInicio,
            String anioTermino,
            String mesInicio,
            String mesTermino,
            String concesionario,
            String sucursal,
            Boolean soloCotizaciones
        )
    {
        cotizacionCompra = new IndicadorCotizacionCompra();
        return cotizacionCompra.obtenerCotizacionCompra(
            anioInicio,
            anioTermino,
            mesInicio,
            mesTermino,
            concesionario,
            sucursal,
            soloCotizaciones
        );
    }
}
