using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for PermisosPersona
/// </summary>
public class PermisosPersona
{
    public int Id { get; set; }
    public int Cargo { get; set; }

    // Atributos de conexion a BD
    SqlConnection con;
    SqlCommand cmd, cmd1;
    SqlDataReader dr, dr1;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Persona));

	public PermisosPersona()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    public List<PermisosPersona> obtienePermisosPersonaPorRUt(String RUT)
    {
        String query = @"
            select * from personaPermisos where personaPermisos.rut = '" + RUT + "'";
        return ejecutarQuery(query);
    }


    private List<PermisosPersona> ejecutarQuery(String query)
    {
        List<PermisosPersona> permisos = new List<PermisosPersona>();
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
                PermisosPersona permiso = new PermisosPersona();
                permiso.Cargo = int.Parse(dr["cargo"].ToString());
                permiso.Id = int.Parse(dr["idPermisos"].ToString());
                permisos.Add(permiso);
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
        return permisos;
    }
}
