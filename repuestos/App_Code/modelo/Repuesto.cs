using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for Repuesto
/// </summary>
public class Repuesto
{
    public String IdRepuesto { get; set; }
    public String Imagen { get; set; }
    public String Marca { get; set; }
    public String Modelo { get; set; }
    public String Concesionario { get; set; }
    public String Contacto { get; set; }
    public String Descripcion { get; set; }
    public String CodigoRepto { get; set; }
    public Boolean esNuevo { get; set; }
    public String Telefono { get; set; }
    public String Correo { get; set; }
    public String CodigoArea { get; set; }

    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Accesorio));

	public Repuesto()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
	}

    public List<Repuesto> obtenerTodos()
    {
        return ejecutarQuery("select * from Repuestos");
    }

    public List<Repuesto> obtenerRepuesto(String id)
    {
        return ejecutarQuery("select * from Repuestos where id="+id);
    }

    public List<Repuesto> obtenerRepuestoPorCodigo(String codigo)
    {
        return ejecutarQuery("select * from Repuestos where codigo='" + codigo + "'");
    }

    private List<Repuesto> ejecutarQuery(String query)
    {
        List<Repuesto> repuestos = new List<Repuesto>();
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
                Repuesto repuesto = new Repuesto();
                repuesto.IdRepuesto = dr["id"].ToString();
                repuesto.Imagen = dr["imagen"].ToString();
                repuesto.Marca = dr["marca"].ToString();
                repuesto.Modelo = dr["modelo"].ToString();
                repuesto.Concesionario = dr["concesionario"].ToString();
                repuesto.Descripcion = dr["descripcion"].ToString();
                repuesto.CodigoRepto = dr["codigo"].ToString();
                repuesto.esNuevo = dr["esNuevo"].ToString()=="True";
                repuesto.Telefono = dr["telefono"].ToString();
                repuesto.Correo = dr["correo"].ToString();
                repuesto.CodigoArea = dr["codigoArea"].ToString();
                repuesto.Contacto = dr["contacto"].ToString();
                repuestos.Add(repuesto);
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
        return repuestos;
    }

    public long grabarRepuesto(Repuesto nuevoRepuesto)
    {
        int esNuevo = nuevoRepuesto.esNuevo ? 1 : 0;
        String query = "insert into repuestos (imagen, marca, modelo, contacto, concesionario, descripcion, codigo, esNuevo, telefono, codigoArea, correo) values ('" + nuevoRepuesto.Imagen + "','" + nuevoRepuesto.Marca.ToUpper() + "','" + nuevoRepuesto.Modelo.ToUpper() + "','" + nuevoRepuesto.Contacto + "','" + nuevoRepuesto.Concesionario.ToUpper() + "','" + nuevoRepuesto.Descripcion.ToUpper() + "','" + nuevoRepuesto.CodigoRepto.ToUpper() + "'," + esNuevo.ToString() + ", '" + nuevoRepuesto.Telefono + "', '" + nuevoRepuesto.CodigoArea + "', '" + nuevoRepuesto.Correo.ToUpper()+ "')";
        return BD.insertaYRetornaId(query);
    }

    public Boolean actualizaImagen(String imagen, String idRepuesto)
    {
        String query = "update repuestos set imagen='" + imagen + "' where id = " + idRepuesto;
        return BD.InsertaOActualiza(query);
    }
    public Boolean eliminaRepto(String idRepuesto)
    {
        String query = "delete repuestos where id=" + idRepuesto;
        return BD.InsertaOActualiza(query);
    }

	public Boolean actualizaCantidadMin(String cantidad, String idRepuesto)
	{
		String query = "update repuesto_cantidadmin set cantidadMin='" + cantidad + "' where id = " + idRepuesto;
        return BD.InsertaOActualiza(query);
	}
	
	public String detalleRepuestoPorMarca(String marca)
    {
        return @"SELECT *
                FROM repuesto_cantidadmin
                WHERE marca ='"+marca+"'";
                
    }
	
	public String detalleRepuesto()
    {
        return @"SELECT *
                FROM repuesto_cantidadmin";
               
                
    }
	public String detalleRepuestoporCodigo(String codigo)
    {
        return @"SELECT *
                FROM repuesto_cantidadmin
                WHERE codigo ='"+codigo+"'";
               
                
    }
	
	public Boolean ExisteRepuesto (String marca, String codigo)
	{
		con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT * FROM repuesto_cantidadmin WHERE codigo ='"+codigo+"'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
			dr.Close();
			con.Close();	
			return true;  
        }else {
			dr.Close();
			con.Close();
			return false;
		}
	}
	public String detalleRepuestoPorMarcaCodigo(String marca, String codigo)
    {
	
		if (ExisteRepuesto( marca, codigo) == true){
         return @"SELECT *
                FROM repuesto_cantidadmin
                WHERE codigo ='"+codigo+"'";
        }else {        
	      return "none";
		}		
    }
	
	public Boolean repuestoPorCantidad( String cantidad,  String codigo){
	
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select cantidadMin from repuesto_cantidadmin where codigo= '"+ codigo + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
			 dr.Read();
			 string cant = dr[0].ToString();
			 int cantbd = int.Parse(cant);
			 int cant2 = int.Parse(cantidad);
			 if (cantbd == 0) {
                 cantbd = 1;
             } 
			 float resto = cant2%cantbd;
			 if (resto > 0){
				dr.Close();
				con.Close();
				return false;
			 }
			 if (cantbd > cant2)  {
				dr.Close();
				con.Close();
				return false;
			 }else{ 
				 dr.Close();
				 con.Close();	
				 return true;  
			}	 
        }else {
			dr.Close();
			con.Close();
			return true;
		}
       

	}

    public String getCantidadporCodigo(String codigo)
    {
        string cantidad = "";
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select isnull(cantidadMin,0) from repuesto_cantidadmin where codigo = '" + codigo + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        cantidad = dr[0].ToString();
        dr.Close();
        con.Close();

        return cantidad;

    }

}
