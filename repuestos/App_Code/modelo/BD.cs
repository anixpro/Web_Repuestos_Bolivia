using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for BD
/// </summary>
static public class BD
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(BD));

    // <summary>
    ///Método que inserta datos en la Base de Datos, este método inserta datos en la base de daatos recibiendo como
    ///parametro una consulta SQL de tipo INSERT. 
    /// </summary>
    /// <param name="query"></param>
    static public Boolean InsertaOActualiza(string query)
    {
        Boolean returnValue = true;
        //Utilizando la cláusula using te aseguras de que liberarás los recursos una vez hayas terminado
        //de utilizar la conexión a la base de datos
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString))
        {
            try
            {
                //Abrimos la conexión a la base de datos
                conn.Open();
                //Creamos el comando que contendrá la query a ejecutar en el servidor
                SqlCommand _query = conn.CreateCommand();

                //Si vamos a ejecutar una consulta, especificamos como CommandType Text
                _query.CommandType = CommandType.Text;

                //Especifiamos la query a ejecutar
                _query.CommandText = string.Format(query);

                // Devuelve el número de filas afectadas
                int fil = _query.ExecuteNonQuery();

                if (fil > 0)
                {
                    // Todo OK
                }
                else
                {
                    returnValue = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error en [Insertar]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                returnValue = false;
            }
            finally
            {
                //Nos aseguramos de cerrar la conexión
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
        return returnValue;
    }

    static public long insertaYRetornaId(String query)
    {
        long returnValue = 0;
        query = query + ";SELECT @@IDENTITY as 'Identity'";
        //Utilizando la cláusula using te aseguras de que liberarás los recursos una vez hayas terminado
        //de utilizar la conexión a la base de datos
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString))
        {
            try
            {
                //Abrimos la conexión a la base de datos
                conn.Open();
                //Creamos el comando que contendrá la query a ejecutar en el servidor
                SqlCommand _query = conn.CreateCommand();

                //Si vamos a ejecutar una consulta, especificamos como CommandType Text
                _query.CommandType = CommandType.Text;

                //Especifiamos la query a ejecutar
                _query.CommandText = string.Format(query);

                Object resultado = _query.ExecuteScalar();

                returnValue = long.Parse(resultado.ToString());

                return returnValue;
            }
            catch (Exception ex)
            {
                logger.Error("Error en [insertaYRetornaId]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                return 0;
            }
            finally
            {
                //Nos aseguramos de cerrar la conexión
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
    }

    static public String queryRetornaValorExacto(String query)
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString);
        SqlCommand cmd = null;
        SqlDataReader dr = null;
        cmd = new SqlCommand();

        con.Open();
        cmd.Connection = con;
        cmd.CommandText = query;
        cmd.CommandTimeout = 10;
        
        try
        {
            dr = cmd.ExecuteReader();
            dr.Read();
            return dr[0].ToString();
        }
        catch (Exception ex)
        {
            logger.Error("en [queryRetornaValorExacto] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Murio con la siguiente query: [" + query + "]");
            return "";
        }
        finally
        {
            dr.Close();
            con.Close();
        }
    }
}
