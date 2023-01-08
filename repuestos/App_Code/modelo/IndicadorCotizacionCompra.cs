using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for IndicadorCotizacionCompra
/// </summary>
public class IndicadorCotizacionCompra
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(IndicadorCotizacionCompra));

    String queryCountCotizaciones = "select COUNT(*) from PEDIDO where E_VBELN_PEDIDO is null";
    String queryCountPedidos = "select COUNT(*) from PEDIDO where E_VBELN_PEDIDO is not null";
    String queryPedidos = "select *,convert(varchar, FECHA_SOLICITUD, 1) as fechasol from PEDIDO";

    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

    public String Indentificador { get; set; }
    public String NumCotizacion { get; set; }
    public String CodClienteSAP { get; set; }
    public String CodSucursal { get; set; }
    public String ClaseDocumentoVentas { get; set; }
    public String RutSolicitante { get; set; }
    public String FechaSolicitud { get; set; }
    public String TotalNeto { get; set; }
    public String NumPedido { get; set; }
    public String TipoPedido { get; set; }
    public String PrioridadPedido { get; set; }

    public IndicadorCotizacionCompra()
	{
		//
		// TODO: Add constructor logic here
		//
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    public int[] obtenerCantidadCotizacionCompra(
            String anioInicio,
            String anioTermino,
            String mesInicio,
            String mesTermino,
            String concesionario,
            String sucursal
        )
    {
        if (concesionario != "")
        {
            this.queryCountCotizaciones = this.queryCountCotizaciones + " and I_KUNNR ='"+concesionario+"'";
            this.queryCountPedidos = this.queryCountPedidos + " and I_KUNNR ='" + concesionario + "'";
        }
        if (sucursal != "")
        {
            this.queryCountCotizaciones = this.queryCountCotizaciones + " and I_KUNNR2 ='" + sucursal + "'";
            this.queryCountPedidos = this.queryCountPedidos + " and I_KUNNR2 ='" + sucursal + "'";
        }
        int[] retorno = {0,0};

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandText = queryCountCotizaciones;
        cmd.CommandTimeout = 10;

        try
        {
            retorno[0] = (Int32)cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + queryCountCotizaciones + "]");
        }
        finally
        {
            
        }

        cmd.CommandText = queryCountPedidos;

        try
        {
            retorno[1] = (Int32)cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + queryCountPedidos + "]");
        }
        finally
        {
            con.Close();
        }

        return retorno;
    }

    public List<IndicadorCotizacionCompra> obtenerCotizacionCompra(
            String anioInicio,
            String anioTermino,
            String mesInicio,
            String mesTermino,
            String concesionario,
            String sucursal,
            Boolean soloCotizaciones
        )
    {
        int intMesFin = int.Parse(mesTermino);
        int intAnioTermino = int.Parse(anioTermino);
        intMesFin++;
        if (intMesFin == 13)
        {
            intMesFin = 1;
            intAnioTermino++;
        }

        this.queryPedidos = this.queryPedidos = this.queryPedidos + " where 1=1";

        if (concesionario != "")
        {
            this.queryPedidos = this.queryPedidos + " and I_KUNNR ='" + concesionario + "'";
        }
        if (sucursal != "")
        {
            this.queryPedidos = this.queryPedidos + " and I_KUNNR2 ='" + sucursal + "'";
        }

        if (soloCotizaciones)
        {
            this.queryPedidos = this.queryPedidos + " and E_VBELN_PEDIDO is null";
        }

        this.queryPedidos = this.queryPedidos + @"
            and
	            FECHA_SOLICITUD >= '" + anioInicio + "-" + mesInicio + @"-01'
                and FECHA_SOLICITUD<'" + intAnioTermino + "-" + intMesFin + @"-01'";

        return ejecutarQuery(this.queryPedidos);
    }

    private List<IndicadorCotizacionCompra> ejecutarQuery(String query)
    {
        List<IndicadorCotizacionCompra> indicadores = new List<IndicadorCotizacionCompra>();
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
                IndicadorCotizacionCompra indicador = new IndicadorCotizacionCompra();
                indicador.Indentificador = dr["ID_PEDIDO"].ToString();
                indicador.NumCotizacion = dr["E_VBELN"].ToString();
                indicador.CodClienteSAP = dr["I_KUNNR"].ToString();
                indicador.CodSucursal = dr["I_KUNNR2"].ToString();
                indicador.ClaseDocumentoVentas = dr["I_VKORG"].ToString();
                indicador.RutSolicitante = dr["SOLICITADO_POR"].ToString();
                indicador.FechaSolicitud = dr["fechasol"].ToString();
                indicador.TotalNeto = dr["TOTAL_NETO"].ToString();
                indicador.NumPedido = dr["E_VBELN_PEDIDO"].ToString();

                if (indicador.Indentificador == null) indicador.Indentificador = "";
                if (indicador.NumCotizacion == null) indicador.NumCotizacion = "";
                if (indicador.CodClienteSAP == null) indicador.CodClienteSAP = "";
                if (indicador.CodSucursal == null) indicador.CodSucursal = "";
                if (indicador.ClaseDocumentoVentas == null) indicador.ClaseDocumentoVentas = "";
                if (indicador.RutSolicitante == null) indicador.RutSolicitante = "";
                if (indicador.FechaSolicitud == null) indicador.FechaSolicitud = "";
                if (indicador.TotalNeto == null) indicador.TotalNeto = "";
                if (indicador.NumPedido == null) indicador.NumPedido = "";

                if (dr["I_AUART_PEDIDO"].ToString() == "ZBPE")
                {
                    indicador.TipoPedido = "NORMAL";
                }
                else if (dr["I_AUART_PEDIDO"].ToString() == "ZBPG")
                {
                    indicador.TipoPedido = "GARANTIA";
                }
                else
                {
                    indicador.TipoPedido = "";
                }

                if (dr["I_LPRIO_PEDIDO"].ToString() == "2")
                {
                    indicador.PrioridadPedido = "NORMAL";
                }
                else
                {
                    indicador.PrioridadPedido = "AEREO";
                }
                
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
}
