using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using log4net;
using log4net.Config;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Concesionario
/// </summary>
public class Concesionario
{
    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

    public String Nombre { get; set; }
    public String ID { get; set; }
    public String CodigoClienteSAP { get; set; }

    private String query = "select * from concesionario";

	public Concesionario(String nombre, String id)
	{
        this.Nombre = nombre;
        this.ID = id;
        this.inicio();
	}

    public Concesionario()
    {
        this.inicio();
    }

    private void inicio()
    {
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
    }
    public Concesionario(String nombre)
    {
        this.Nombre = nombre;
    }

    public List<Concesionario> obtieneConcesionarios()
    {
        return this.ejecutarQuery(this.query);
    }

    public List<Concesionario> obtieneConcesionarioPorID(String idConcesionario)
    {
        this.query = this.query + " where IdConcesionario = " + idConcesionario;
        return this.ejecutarQuery(this.query);
    }

    private List<Concesionario> ejecutarQuery(String query)
    {
        List<Concesionario> concesionarios = new List<Concesionario>();
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
                Concesionario concesionario = new Concesionario();
                concesionario.ID = dr["idConcesionario"].ToString();
                concesionario.CodigoClienteSAP = dr["numeroFactura"].ToString();
                concesionario.Nombre = dr["nombreConcesionario"].ToString();
                concesionarios.Add(concesionario);
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
        return concesionarios;
    }
}