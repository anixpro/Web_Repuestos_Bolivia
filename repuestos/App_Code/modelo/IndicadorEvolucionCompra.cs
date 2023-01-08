using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for IndicadorEvolucionCompra
/// </summary>
public class IndicadorEvolucionCompra
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

    public String Marca { get; set; }
    public String Concesionario { get; set; }
    public int Compras { get; set; }
    public String Mes { get; set; }
    public String Anio { get; set; }
    public int NumMes { get; set; }

    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;
    private String query = @"
        select 
	            indicador.idIndicador,
	            concesionarioMarca.nombreConcesionario,
	            concesionarioMarca.nombreMarca,
	            indicador.ano,
	            indicador.mes,
	            indicador.compras,
	            indicador.metas,
	            mes1 =
	                CASE indicador.mes
		                WHEN 1 then 'ENERO'
		                WHEN 2 then 'FEBRERO'
		                WHEN 3 then 'MARZO'
		                WHEN 4 then 'ABRIL'
		                WHEN 5 then 'MAYO'
		                WHEN 6 then 'JUNIO'
		                WHEN 7 then 'JULIO'
		                WHEN 8 then 'AGOSTO'
		                WHEN 9 then 'SEPTIEMBE'
		                WHEN 10 then 'OCTUBRE'
		                WHEN 11 then 'NOVIEMBRE'
		                WHEN 12 then 'DICIEMBRE'
	                END
                from indicador
	                inner join concesionarioMarca on
		                indicador.idConcesionarioMarca = concesionarioMarca.idConcesionarioMarca ";

	public IndicadorEvolucionCompra()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    public List<IndicadorEvolucionCompra> obtenerPorFechas(
        String anioInicio,
        String anioFin,
        String mesInicio,
        String mesFin,
        String marca,
        String concesionario)
    {
        this.query = this.query + @"
            where 
                not(indicador.ano<=" + anioInicio + @" and indicador.mes<" + mesInicio + @") and
                not(indicador.ano>=" + anioFin + @" and indicador.mes>" + mesFin + @") and
                nombreMarca='"+marca+"' and nombreConcesionario='"+concesionario+"'";
        return ejecutaQuery(this.query);
    }

    public List<IndicadorEvolucionCompra> obtenerPorFechaYConcesionario()
    {
        return ejecutaQuery(this.query);
    }

    public List<IndicadorEvolucionCompra> obtenerPorFechaYMarca()
    {
        return ejecutaQuery(this.query);
    }

    public List<IndicadorEvolucionCompra> obtenerPorFechaMarcaYConcesionario()
    {
        return ejecutaQuery(this.query);
    }

    private List<IndicadorEvolucionCompra> ejecutaQuery(String queryFiltrada)
    {
        List<IndicadorEvolucionCompra> indicadores = new List<IndicadorEvolucionCompra>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandText = queryFiltrada;
        cmd.CommandTimeout = 10;
        try
        {
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                IndicadorEvolucionCompra indicador = new IndicadorEvolucionCompra();
                indicador.Compras = int.Parse(dr["compras"].ToString());
                indicador.Concesionario = dr["nombreConcesionario"].ToString();
                indicador.Marca = dr["nombreMarca"].ToString();
                indicador.Mes = dr["mes1"].ToString();
                indicador.Anio = dr["ano"].ToString();
                indicador.NumMes = int.Parse(dr["mes"].ToString());
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