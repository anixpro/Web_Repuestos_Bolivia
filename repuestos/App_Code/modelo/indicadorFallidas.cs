using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for indicadorFallidas
/// </summary>
public class indicadorFallidas
{
    public String Tipo { get; set; }
    public String Identificador { get; set; }
    public String RutSolicitante { get; set; }
    public String FechaSolicitud { get; set; }
    public String Marca { get; set; }
    public String Cantidad { get; set; }
    public String DetalleRepuesto { get; set; }
    public String VIN { get; set; }
    public String Repuesto { get; set; }
    
    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;
    private String queryVFC = "select * from vfc";
    private String queryBO = "select * from backorder";
    private String queryDescartados = "select * from descartados";

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

	public indicadorFallidas()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    public List<indicadorFallidas> obtenerFallidas(
            String anioInicio,
            String anioTermino,
            String mesInicio,
            String mesFin
        )
    {
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioTermino);
        intMesFin++;
        if(intMesFin==13){
            intMesFin=1;
            intAnioTermino++;
        }

        this.queryBO = this.queryBO + @"
            where not(
                fecha_creacion<'"+anioInicio+"-"+mesInicio+@"-01 00:00:00' or
                fecha_creacion>='" + intAnioTermino + "-" + intMesFin + "-01 00:00:00') order by fecha_creacion asc";
        this.queryDescartados = this.queryDescartados + @"
            where not(
                fechaDescarte<'" + anioInicio + "-" + mesInicio + @"-01 00:00:00' or
                fechaDescarte>='" + intAnioTermino + "-" + intMesFin + "-01 00:00:00') order by fechaDescarte asc";
        this.queryVFC = this.queryVFC + @"
            where not(
                fecha_creacion<'" + anioInicio + "-" + mesInicio + @"-01 00:00:00' or
                fecha_creacion>='" + intAnioTermino + "-" + intMesFin + "-01 00:00:00') order by fecha_creacion asc";
        return ejecutarQuery(queryBO, queryDescartados, queryVFC);
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
        int intMesFin = int.Parse(mesFin);
        int intAnioTermino = int.Parse(anioTermino);
        intMesFin++;
        if(intMesFin==13){
            intMesFin=1;
            intAnioTermino++;
        }

        this.queryBO = this.queryBO + @"
            where not(
                fecha_creacion<'"+anioInicio+"-"+mesInicio+@"-01 00:00:00' or
                fecha_creacion>='"+intAnioTermino+"-"+intMesFin+@"-01 00:00:00')";
        this.queryDescartados = this.queryDescartados + @"
            where not(
                fechaDescarte<'" + anioInicio + "-" + mesInicio + @"-01 00:00:00' or
                fechaDescarte>='" + intAnioTermino + "-" + intMesFin + @"-01 00:00:00')";
        this.queryVFC = this.queryVFC + @"
            where not(
                fecha_creacion<'" + anioInicio + "-" + mesInicio + @"-01 00:00:00' or
                fecha_creacion>='" + intAnioTermino + "-" + intMesFin + @"-01 00:00:00')";

        if(codigo!="")
        {
            this.queryBO = this.queryBO + " and codigo_rep ='" + codigo + "'";
            this.queryDescartados = this.queryDescartados + " and codigo_rep ='" + codigo + "'";
            this.queryVFC = this.queryVFC + " and codigo_rep ='" + codigo + "'";
        }
        if (marca != "")
        {
            this.queryBO = this.queryBO + " and marca ='" + marca + "'";
            this.queryDescartados = this.queryDescartados + " and marca ='" + marca + "'";
            this.queryVFC = this.queryVFC + " and marca ='" + marca + "'";
        }

        return ejecutarQuery(queryBO, queryDescartados, queryVFC);
    }
    
    private List<indicadorFallidas> ejecutarQuery(
        String queryBO,
        String queryDescartados,
        String queryVFC)
    {
        List<indicadorFallidas> indicadores = new List<indicadorFallidas>();

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;

        // VFCs
        cmd.CommandText = queryVFC;
        cmd.CommandTimeout = 10;
        try
        {
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                indicadorFallidas indicador = new indicadorFallidas();
                indicador.Cantidad = dr["cantidad"].ToString();
                indicador.Repuesto = dr["codigo_rep"].ToString();
                indicador.DetalleRepuesto = dr["detalle_rep"].ToString();
                indicador.FechaSolicitud = dr["fecha_creacion"].ToString();
                indicador.Identificador = dr["num_VFC"].ToString();
                indicador.Marca = dr["marca"].ToString();
                indicador.RutSolicitante = dr["rut_user"].ToString();
                indicador.Tipo = "VFC";
                indicador.VIN = dr["cod_vin"].ToString();
                indicadores.Add(indicador);
            }
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + queryBO + "]");
        }
        finally
        {
            dr.Close();
        }

        // BO
        cmd.CommandText = queryBO;
        cmd.CommandTimeout = 10;
        try
        {
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                indicadorFallidas indicador = new indicadorFallidas();
                indicador.Cantidad = dr["cantidad"].ToString();
                indicador.Repuesto = dr["codigo_rep"].ToString();
                indicador.DetalleRepuesto = dr["detalle_rep"].ToString();
                indicador.FechaSolicitud = dr["fecha_creacion"].ToString();
                indicador.Identificador = dr["num_backOrder"].ToString();
                indicador.Marca = dr["marca"].ToString();
                indicador.RutSolicitante = dr["rut_user"].ToString();
                indicador.Tipo = "Reserva";
                indicador.VIN = "";
                indicadores.Add(indicador);
            }
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + queryVFC + "]");
        }
        finally
        {
            dr.Close();
        }

        // Descartados
        cmd.CommandText = queryDescartados;
        cmd.CommandTimeout = 10;
        try
        {
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                indicadorFallidas indicador = new indicadorFallidas();
                indicador.Cantidad = dr["cantidad"].ToString();
                indicador.Repuesto = dr["codigo_rep"].ToString();
                indicador.DetalleRepuesto = dr["detalle_rep"].ToString();
                indicador.FechaSolicitud = dr["fechaDescarte"].ToString();
                indicador.Identificador = dr["idDescartado"].ToString();
                indicador.Marca = dr["marca"].ToString();
                indicador.RutSolicitante = dr["rut_user"].ToString();
                indicador.Tipo = "Descartado";
                indicador.VIN = "";
                indicadores.Add(indicador);
            }
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + queryDescartados + "]");
        }
        
        finally
        {
            dr.Close();
            con.Close();
        }
        return indicadores;
    }
}
