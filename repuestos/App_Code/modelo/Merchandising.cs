using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Merchandising
/// </summary>
public class Merchandising
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Merchandising));

    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

	public Merchandising()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    // Propiedades
    public int idMerchandising { get; set; }
    public String nombreMarca { get; set; }
    public String codigo { get; set; }
    public String fecha { get; set; }
    public String descripcion { get; set; }
    public String imagen { get; set; }

    public List<Merchandising> obtenerTodos()
    {
        return ejecutarQuery("select * from merchandising");
    }

    public List<Merchandising> obtenerPorId(string Id)
    {
        return ejecutarQuery("select * from merchandising where idMerchandising = " + Id);
    }

    private List<Merchandising> ejecutarQuery(String query)
    {
        List<Merchandising> merchandisings = new List<Merchandising>();
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
                Merchandising merchandising = new Merchandising();
                merchandising.idMerchandising = int.Parse(dr["idMerchandising"].ToString());
                merchandising.nombreMarca = dr["nombreMarca"].ToString();
                merchandising.codigo = dr["codigo"].ToString();
                merchandising.fecha = dr["fecha"].ToString();
                merchandising.descripcion = dr["descripcion"].ToString();
                merchandising.imagen = dr["imagen"].ToString();
                merchandisings.Add(merchandising);
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
        return merchandisings;
    }
}