using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using log4net;
using log4net.Config;

/// <summary>
/// Clase modelo Boletin, encargada de crear boletines.
/// </summary>
public class Boletin
{
    string _titulo;
    string _descripcion;
    string _fecha;
    string _documento;
    SqlConnection con;
    SqlCommand cmd, cmd1;
    SqlDataReader dr, dr1;

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Boletin));

	public Boletin()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
	}

    public string Titulo
    {
        get { return this._titulo; }
        set { _titulo = value; }
    }

    public string Descripcion
    {
        get { return this._descripcion; }
        set { _descripcion = value; }
    }

    public string Fecha
    {
        get { return this._fecha; }
        set { _fecha = value; }
    }


    public string Documento
    {
        get { return this._documento; }
        set { _documento = value; }
    }

    // retorna true en caso que se leyó la notica
    public bool leyoBoletin(String rut, string idBoletin)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        bool retorno = true;

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"
            select count(*) as cantidad from boletines_persona where idBoletin = " + idBoletin + @" 
            and rutUser = " + rut;

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            // Quiere decir que NO ha leido la noticia
            if (dr[0].ToString() == "0")
            {
                retorno = false;
            }
        }
        dr.Close();
        con.Close();

        return retorno;
    }

    public DataSet obtenerBoletinesLeidosPorRut(string rut)
    {
        string consulta = @"
            select idBoletin
            from boletines_persona where rutUser = " + rut;

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
                _comando.CommandText = consulta;
                _adapter.SelectCommand = _comando;
                _adapter.Fill(_dSet);
            }
        }
        catch
        {
            _dSet = null;
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

    public int obtenerCantidadDeBoletinesLeidosPorRut(string rut)
    {
        int retorno = 0;

        try
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();

            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = @"
                select (
	                (select COUNT(*) from boletines)
	                - (select COUNT(*) from boletines_persona where rutUser=" + rut + @")
                )
                as cantidad;
            ";

            cmd.CommandTimeout = 10;
            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                // Quiere decir que NO ha leido la noticia
                retorno = int.Parse(dr[0].ToString());
            }
        }
        catch (Exception)
        {
            
            
        }
        
        dr.Close();
        con.Close();

        return retorno;
    }

    public DataSet obtenerBoletinesPorFecha(String fecha)
    {
        String consulta = "";
        consulta = @"
        select idBoletin, titulo,descripcion,cast(fecha as varchar) as fecha,documento,('No') as leido
        from boletines where Convert(varchar(10),fecha,120) = '" + fecha + "'";
        DataSet _dSet = new DataSet();
        SqlConnection _conection = new SqlConnection();
        SqlCommand _comando = new SqlCommand();
        SqlDataAdapter _adapter = new SqlDataAdapter();
        try
        {
            _conection.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            _conection.Open();
        }
        catch (Exception ex)
        {
            logger.Error("Error en [UpdateFechasExpiracion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        try
        {
            if (_conection.State == ConnectionState.Open)
            {
                _comando.Connection = _conection;
                _comando.CommandType = CommandType.Text;
                _comando.CommandText = consulta;
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
}
