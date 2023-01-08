using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for IndicadorEstadisticaDeCompra
/// </summary>
public class IndicadorEstadisticaDeCompra
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

    public String Cotizacion { get; set; }
    public String Cliente { get; set; }
    public String DestinatarioMercancia { get; set; }
    public String RutSolicitante { get; set; }
    public String TotalNeto { get; set; }
    public String Codigo { get; set; }
    public String Descipcion { get; set; }
    public String Marca { get; set; }
    public String Cantidad { get; set; }
    public String Valor { get; set; }
    public String Total { get; set; }

    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;
    private String query = @"
        select  
	        pedido.E_VBELN as cotizacion,
	        pedido.I_KUNNR as cliente,
	        pedido.I_KUNNR2 as shipcode,
	        pedido.SOLICITADO_POR as solicitadoPor,
	        pedido.TOTAL_NETO,
	        PEDIDO.FECHA_SOLICITUD,
	        MATERIALES_PEDIDO.codigo,
	        MATERIALES_PEDIDO.descripcion,
	        MATERIALES_PEDIDO.marca,
	        MATERIALES_PEDIDO.cantidad,
	        MATERIALES_PEDIDO.valor,
	        MATERIALES_PEDIDO.total
        from PEDIDO	
	        inner join MATERIALES_PEDIDO
		        on cast(PEDIDO.ID_PEDIDO as varchar) = MATERIALES_PEDIDO.id_pedido ";

    public IndicadorEstadisticaDeCompra()
    {
        inicio();
    }
	public IndicadorEstadisticaDeCompra(Boolean escalar)
	{
        inicio();
        if (escalar)
        {
            query = @"
                select  
	                COUNT(*)
                from PEDIDO	
	                inner join MATERIALES_PEDIDO
		                on cast(PEDIDO.ID_PEDIDO as varchar) = MATERIALES_PEDIDO.id_pedido ";
        }
	}

    private void inicio()
    {
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
    }

    /// <summary>
    /// Obtiene estadísticas de compra solo por fechas
    /// </summary>
    /// <param name="anioInicio">Año de inicio</param>
    /// <param name="mesInicio">Mes de inicio</param>
    /// <param name="anioFin">Año de Término</param>
    /// <param name="mesFin">Mes de Término</param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompra(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01'";
        return ejecutarQuery(this.query);
    }

    /// <summary>
    /// Obtiene estadísticas de compra por fechas, rut, marca y código
    /// </summary>
    /// <param name="anioInicio">Año de inicio</param>
    /// <param name="mesInicio">Mes de inicio</param>
    /// <param name="anioFin">Año de término</param>
    /// <param name="mesFin">Mes de término</param>
    /// <param name="rutSolicitante">Rut del operador solicitante</param>
    /// <param name="marca">Marca</param>
    /// <param name="codigo">Código de repuesto</param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompra(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + "'";
        return ejecutarQuery(this.query);
    }

    /// <summary>
    /// Obtiene estadísticas de compra por fecha, sucursal, marca y código
    /// </summary>
    /// <param name="anioInicio"></param>
    /// <param name="mesInicio"></param>
    /// <param name="anioFin"></param>
    /// <param name="mesFin"></param>
    /// <param name="sucursal"></param>
    /// <param name="marca"></param>
    /// <param name="codigo"></param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String sucursal,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR2 = '" + sucursal + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + "'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD <'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR2 = '" + sucursal + @"'";
        return ejecutarQuery(this.query);
    }

    /// <summary>
    /// Obtiene estadísticas de compra por concesionario por fecha, concesionario, marca y código
    /// </summary>
    /// <param name="anioInicio">Año de inicio</param>
    /// <param name="mesInicio">Mes de inicio</param>
    /// <param name="anioFin">Año de término</param>
    /// <param name="mesFin">Mes de término</param>
    /// <param name="concesionario">Nombre del concesionario</param>
    /// <param name="marca">Marca</param>
    /// <param name="codigo">Código del repuesto</param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorConcesionario(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String concesionario,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR = '" + concesionario + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + "'";
        return ejecutarQuery(this.query);
    }

    /// <summary>
    /// Obtiene estadísticas de compra por fecha, operario y marca
    /// </summary>
    /// <param name="anioInicio"></param>
    /// <param name="mesInicio"></param>
    /// <param name="anioFin"></param>
    /// <param name="mesFin"></param>
    /// <param name="rutSolicitante"></param>
    /// <param name="marca"></param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompra(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String marca
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"'";
        return ejecutarQuery(this.query);
    }

    /// <summary>
    /// Obtiene estadísticas de compra por operario
    /// </summary>
    /// <param name="anioInicio"></param>
    /// <param name="mesInicio"></param>
    /// <param name="anioFin"></param>
    /// <param name="mesFin"></param>
    /// <param name="rutSolicitante"></param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompra(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"'";
        return ejecutarQuery(this.query);
    }
    /// <summary>
    /// Obtiene estadística de compra solo por concesionario
    /// </summary>
    /// <param name="anioInicio"></param>
    /// <param name="mesInicio"></param>
    /// <param name="anioFin"></param>
    /// <param name="mesFin"></param>
    /// <param name="concesionario"></param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorConcesionario(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String concesionario
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD <'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR = '" + concesionario + @"'";
        return ejecutarQuery(this.query);
    }

    /// <summary>
    /// Obtiene estadística de compra según marca
    /// </summary>
    /// <param name="anioInicio">Año de inicio</param>
    /// <param name="mesInicio">Mes de inicio</param>
    /// <param name="anioFin">Año de término</param>
    /// <param name="mesFin">Mes de término</param>
    /// <param name="marca">Marca</param>
    /// <returns></returns>
    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorMarca(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorCodigo(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.codigo = '" + codigo + @"'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorRutYCodigo(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rut,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + rut + @"' and
	            MATERIALES_PEDIDO.codigo = '" + codigo + @"'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraPorMarcaYCodigo(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                MATERIALES_PEDIDO.marca = '" + marca + @"' and 
	            MATERIALES_PEDIDO.codigo = '" + codigo + @"'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompra(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String marca,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
                                              ;
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraMarcaCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraMarcaYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraRUTMarcaYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rut,
        String marca,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + rut + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraRUTCodigoYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + RUT + @"' and
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQuery(this.query);
    }

    public List<IndicadorEstadisticaDeCompra> obtenerEstadisticasDeCompraRUTYSucursal(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + RUT + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQuery(this.query);
    }

    private List<IndicadorEstadisticaDeCompra> ejecutarQuery(String query)
    {
        List<IndicadorEstadisticaDeCompra> indicadores = new List<IndicadorEstadisticaDeCompra>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandText = query;
        cmd.CommandTimeout = 10;
        try
        {
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                IndicadorEstadisticaDeCompra indicador = new IndicadorEstadisticaDeCompra();
                indicador.Cantidad = dr["cantidad"].ToString();
                indicador.Cliente = dr["cliente"].ToString();
                indicador.Codigo = dr["codigo"].ToString();
                indicador.Cotizacion = dr["cotizacion"].ToString();
                indicador.Descipcion = dr["descripcion"].ToString();
                indicador.DestinatarioMercancia = dr["shipcode"].ToString();
                indicador.Marca = dr["marca"].ToString();
                indicador.RutSolicitante = dr["solicitadoPor"].ToString();
                indicador.Total = dr["total"].ToString();
                indicador.TotalNeto = dr["total_Neto"].ToString();
                indicador.Valor = dr["valor"].ToString();
                indicadores.Add(indicador);
            }
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + query + "]");
        }
        finally
        {
            dr.Close();
            con.Close();
        }
        return indicadores;
    }

    public int obtenerEstadisticasDeCompraEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String sucursal,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR2 = '" + sucursal + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR2 = '" + sucursal + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorConcesionarioEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String concesionario,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR = '" + concesionario + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String marca
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorConcesionarioEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String concesionario
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            pedido.I_KUNNR = '" + concesionario + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorMarcaEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorCodigoEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.codigo = '" + codigo + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorRutYCodigoEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rut,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + rut + @"' and
	            MATERIALES_PEDIDO.codigo = '" + codigo + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraPorMarcaYCodigoEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String codigo
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                MATERIALES_PEDIDO.marca = '" + marca + @"' and 
	            MATERIALES_PEDIDO.codigo = '" + codigo + @"'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rutSolicitante,
        String marca,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            PEDIDO.SOLICITADO_POR = '" + rutSolicitante + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        ;
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraMarcaCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and 
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraMarcaYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String marca,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraRUTMarcaYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String rut,
        String marca,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + rut + @"' and
	            MATERIALES_PEDIDO.marca = '" + marca + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraRUTCodigoYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String codigo,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + RUT + @"' and
                MATERIALES_PEDIDO.codigo = '" + codigo + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQueryEscalar(this.query);
    }

    public int obtenerEstadisticasDeCompraRUTYSucursalEscalar(
        String anioInicio,
        String mesInicio,
        String anioFin,
        String mesFin,
        String RUT,
        String sucursal
    )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioFin);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.query = this.query + @"
            where
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD <'" + intAnioTermino + "-" + intMesFin + @"-01' and
                PEDIDO.SOLICITADO_POR = '" + RUT + @"' and
                I_KUNNR2 = '" + sucursal + "'";
        return ejecutarQueryEscalar(this.query);
    }

    private int ejecutarQueryEscalar(String query)
    {
        int resultado = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandText = query;
        cmd.CommandTimeout = 10;
        try
        {
            resultado = (Int32)cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + query + "]");
        }
        finally
        {
            con.Close();
        }
        return resultado;
    }
}
