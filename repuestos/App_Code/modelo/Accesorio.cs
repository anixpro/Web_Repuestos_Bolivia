using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for Accesorio
/// </summary>
public class Accesorio
{
    // Propiedades
    public int idAccesorio { get; set; }
    public String nombreMarca { get; set; }
    public String nombreModelo { get; set; }
    public String codigo { get; set; }
    public String fecha { get; set; }
    public String descripcion { get; set; }
    public String imagen { get; set; }

    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

	public Accesorio()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    public List<Accesorio> obtenerPorCodigoyMarca(String codigo, String marca)
    {
        return ejecutarQuery("select * from accesorio where nombreMarca='" + marca + "' and codigo = '" + codigo+"'");
    }

    public List<Accesorio> obtenerTodos()
    {
        return ejecutarQuery("select * from accesorio");
    }

    public List<Accesorio> obtenerPorId(string Id)
    {
        return ejecutarQuery("select * from accesorio where idAccesorio = " + Id);
    }

    private List<Accesorio> ejecutarQuery(String query)
    {
        List<Accesorio> accesorios = new List<Accesorio>();
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
                Accesorio accesorio = new Accesorio();
                accesorio.idAccesorio = int.Parse(dr["idAccesorio"].ToString());
                accesorio.nombreMarca = dr["nombreMarca"].ToString();
                accesorio.nombreModelo = dr["nombreModelo"].ToString();
                accesorio.codigo = dr["codigo"].ToString();
                accesorio.fecha = dr["fecha"].ToString();
                accesorio.descripcion = dr["descripcion"].ToString();
                accesorio.imagen = dr["imagen"].ToString();
                accesorios.Add(accesorio);
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
        return accesorios;
    }
}