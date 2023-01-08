using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Descripción breve de Marca
/// </summary>
public class Marca
{
    int _idMarca;
    string _nomMarca;
    string _abreviado;
    string _descripcion;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Marca));

    public String Modelo { get; set; }
    public String Nombre { get; set; }
    public String OrgVentas { get; set; }
    public String Descripcion { get; set; }
    public String Abreviado { get; set; }
    public String NomMarca { get; set; }
    public int IdMarca { get; set; }
    public bool esForaneo { get; set; }
    public String GrupoMaterial { get; set; }
    public String CodigoCompania { get; set; }
    public String Prefijo { get; set; }

    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

	public Marca()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
        inicio();
	}

    public Marca(String nombre)
    {
        this.NomMarca = nombre;
        inicio();
    }

    private void inicio()
    {
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
    }

    public DataSet obtieneMarcasModelosPorRutConcesionario(String nombreConcesionario)
    {
        String query = @"
            select
	            marca.nombreMarca, modelo.nombreModelo 
            from
	            marca, modelo, concesionarioMarca
            where
	            marca.nombreMarca = modelo.nombreMarca and 
	            marca.nombreMarca = concesionarioMarca.nombreMarca and
				modelo.nombreModelo <> 'Todas' and
	            concesionarioMarca.nombreConcesionario = '" + nombreConcesionario +
	            @"' order by marca.nombreMarca asc
        ";
        DataSet _dSet = new DataSet();
        SqlConnection _conection = new SqlConnection();
        SqlCommand _comando = new SqlCommand();
        SqlDataAdapter _adapter = new SqlDataAdapter();

        _conection.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        _conection.Open();

        try
        {
            if (_conection.State == ConnectionState.Open)
            {
                _comando.Connection = _conection;
                _comando.CommandType = CommandType.Text;
                _comando.CommandText = query;
                _adapter.SelectCommand = _comando;
                _adapter.Fill(_dSet);
            }
        }
        catch (Exception ex)
        {
            _dSet = null;
            logger.Error("Error en [UpdateFechasExpiracion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        finally
        {
            if (_conection.State == ConnectionState.Open)
                _conection.Close();
            _comando = null;
            _conection = null;
            _adapter = null;
        }
        return _dSet;
    }

    public DataSet obtieneMarcasPorRutConcesionario(String nombreConcesionario)
    {
        String query = @"
            select
                marca.nombreMarca
            from
                marca, concesionarioMarca
            where
                marca.nombreMarca = concesionarioMarca.nombreMarca and
                concesionarioMarca.nombreConcesionario = '" + nombreConcesionario + 
                @"' order by marca.nombreMarca asc
        ";
        DataSet _dSet = new DataSet();
        SqlConnection _conection = new SqlConnection();
        SqlCommand _comando = new SqlCommand();
        SqlDataAdapter _adapter = new SqlDataAdapter();

        _conection.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        _conection.Open();

        try
        {
            if (_conection.State == ConnectionState.Open)
            {
                _comando.Connection = _conection;
                _comando.CommandType = CommandType.Text;
                _comando.CommandText = query;
                _adapter.SelectCommand = _comando;
                _adapter.Fill(_dSet);
            }
        }
        catch (Exception ex)
        {
            _dSet = null;
            logger.Error("Error en [UpdateFechasExpiracion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        finally
        {
            if (_conection.State == ConnectionState.Open)
                _conection.Close();
            _comando = null;
            _conection = null;
            _adapter = null;
        }
        return _dSet;
    }

    public List<Marca> obtenerTodos()
    {
        return ejecutarQuery("select * from marca");
    }

    public List<Marca> obtenerPorId(string Id)
    {
        return ejecutarQuery("select * from marca where idMarca = " + Id);
    }

    public List<Marca> obtenerPorNombre(string nombre)
    {
        if (nombre == "LUBRICANTES")
        {
            nombre = "SKBP";
        }        
        return ejecutarQuery("select * from marca where nombreMarca = '" + nombre + "'");
    }

    private List<Marca> ejecutarQuery(String query)
    {
        List<Marca> marcas = new List<Marca>();
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
                Marca marca = new Marca();
                marca.Abreviado = dr["abreviado"].ToString();
                marca.Descripcion = dr["descripcion"].ToString();
                marca.IdMarca = int.Parse(dr["idMarca"].ToString());
                marca.NomMarca = dr["nombreMarca"].ToString();
                marca.OrgVentas = dr["orgVentas"].ToString();
                marca.esForaneo = int.Parse(dr["esForaneo"].ToString()) == 1;
                marcas.Add(marca);
            }
            dr.Close();
        }
        catch (Exception ex)
        {
            logger.Error("en [ejecutarQuery] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + query + "]");
        }
        finally
        {
            con.Close();
        }
        return marcas;
    }
}