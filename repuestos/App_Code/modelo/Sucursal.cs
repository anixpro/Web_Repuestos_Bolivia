using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using log4net;
using log4net.Config;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Sucursal
/// </summary>
public class Sucursal
{

    SqlConnection con;
    SqlCommand cmd, cmd1;
    SqlDataReader dr, dr1;

    public String ShipCode { get; set; }
    public String Direccion { get; set; }

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Sucursal));

	public Sucursal(string shipCode,string direccion)
	{
        this.ShipCode = shipCode;
        this.Direccion = direccion;
        inicio();
	}
    public Sucursal()
    {
        inicio();
    }
    private void inicio()
    {
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
    }

    public List<Sucursal> obtieneTodasSucursales()
    {
        String query = @"
            select * from sucursal";
        return ejecutarQuery(query);
    }

    public List<Sucursal> obtieneSucursalPorShipCode(String ShipCode)
    {
        String query = @"
            select * from sucursal where shipcode = '"+ShipCode+"'";
        return ejecutarQuery(query);
    }

    private List<Sucursal> ejecutarQuery(String query)
    {
        List<Sucursal> sucursales = new List<Sucursal>();
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
                Sucursal sucursal = new Sucursal();
                sucursal.Direccion = dr["direccionSucursal"].ToString();
                sucursal.ShipCode = dr["ShipCode"].ToString();
                sucursales.Add(sucursal);
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
        return sucursales;
    }
}


