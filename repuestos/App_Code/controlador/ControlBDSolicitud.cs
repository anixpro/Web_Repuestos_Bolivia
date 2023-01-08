using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using log4net.Config;

/// <summary>
/// Descripción breve de ControlBDSolicitud
/// </summary>
public class ControlBDSolicitud
{
    Marca _marca;
    private string concesionario;
    private string local;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ControlBDSolicitud));
    public String rutaFinal { get; set; }
    SendMail_helper _mail = new SendMail_helper();

    public ControlBDSolicitud()
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }
    #region Eliminacion de Datos
    //public void EliminaTMP()
    //{
    //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString))
    //    {
    //        try
    //        {
    //            conn.Open();
    //            SqlCommand _query = conn.CreateCommand();
    //            _query.CommandType = CommandType.Text;
    //            string queryDelet = "TRUNCATE WebRepuesto..t_SolicitudCotizacionTMP";
    //            _query.CommandText = string.Format(queryDelet);
    //            int fil = _query.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error("Error en [EliminaTMP]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
    //        }
    //        finally
    //        {
    //            if (conn.State != ConnectionState.Closed)
    //                conn.Close();
    //        }
    //    }
    //}

    public Boolean EjecutaQuery(string query)
    {
        Boolean returnValue = true;
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString))
        {
            try
            {
                conn.Open();
                SqlCommand _query = conn.CreateCommand();
                _query.CommandType = CommandType.Text;
                _query.CommandText = string.Format(query);
                int fil = _query.ExecuteNonQuery();

                if (fil > 0)
                {
                    returnValue = Convert.ToBoolean(1);
                }
                else
                {
                    returnValue = Convert.ToBoolean(0);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error en [InsertarDatos]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + query + "]");
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
    #endregion

    public List<string> CrearMarcas(string rut)
    {
        _marca = new Marca();
        List<string> listaMarcas = new List<string>();
        DataSet _dSet = ObtenerDatosFiltrados(@"select marca.nombreMarca
                        from marca, concesionarioMarca, concesionario
                        where concesionarioMarca.nombreConcesionario = 
                        concesionario.nombreConcesionario and
	                    marca.nombreMarca = concesionarioMarca.nombreMarca and
	                    concesionario.nombreConcesionario = '" + ObtenerConcesionario(rut) + "'");
        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            _marca.NomMarca = campos["nombreMarca"].ToString();
            listaMarcas.Add(_marca.NomMarca);
        }
        return listaMarcas;
    }
    public DataSet ObtenerDatosFiltrados(string query)
    {
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
            logger.Error("Error en [ObtenerDatosFiltrados]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
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
    public string ObtenerConcesionario(string rutUser)
    {
        string concesionario = "";

        DataSet ds = ObtenerDatosFiltrados(@"select top 1 nombreConcesionario
                                            from sucursal, persona
                                            where sucursal.shipCode = persona.shipCode and
	                                        persona.rut = '" + rutUser + "'");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            concesionario = campo["nombreConcesionario"].ToString();
        }

        return concesionario;
    }

    public string Existencia(string query)
    {
        string numero = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numero = campo["numeroSolicitud"].ToString();
        }
        return numero;
    }
    public string ExistenciaRep(string query)
    {
        string numero = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numero = campo["codigo"].ToString();
        }
        return numero;
    }
    public string usuario(string query)
    {
        string numero = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numero = campo["nombre"].ToString();
        }
        return numero;
    }
    public string ConsultarExiRepto(string query)
    {
        string cantidad = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow dato in ds.Tables[0].Rows)
        {
            {
                cantidad = dato["cantidad"].ToString();
            }
        }
        return cantidad;
    }
    //REQ - Cotizaciones Automaticas Marzo 2022
    public List<string> obtieneMonedas()
    {
        _marca = new Marca();
        List<string> listaMonedas = new List<string>();
        DataSet _dSet = ObtenerDatosFiltrados(" SELECT * FROM monedas" +
                                              " WHERE habilitado = '1' ");
        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            _marca.NomMarca = campos["descripcion"].ToString();
            listaMonedas.Add(_marca.NomMarca);
        }
        return listaMonedas;
    }
    //REQ - Cotizaciones Automaticas Marzo 2022
    public string consultarCodigoMotivoMarca(string query)
    {
        string codigo = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow dato in ds.Tables[0].Rows)
        {
            {
                codigo = dato["MP_codigo"].ToString();
            }
        }
        return codigo;
    }

    public string Email(string query)
    {
        string email = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            email = campo["email"].ToString();
        }
        return email;
    }

    public Boolean InsertarDatos(string query)
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
                //Si vamos a ejecutar un procedimiento almacenado, especificamos StoredProcedure
                //query.CommandType= CommandType.StoredProcedure;
                //query.CommandText="nombreDelProcedimientoAlmacenado";

                //Especifiamos la query a ejecutar
                _query.CommandText = string.Format(query);

                //Como es un INSERT, la query no devuelve resultados, asi que ejecutamos un ExecuteNonQuery que nos
                //devuelve el número de filas afectadas
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

                //logger.Error("Error en [InsertarDatos]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + query + "]");
                //EnviarCorreo(string para, string asunto, string texto, string cc = "")
                conn.Close();
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + query + "]", "");
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

    public string Proveedor(string query)
    {
        string provee = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            provee = campo["nom_Centro"].ToString();
        }
        return provee;
    }

    public string Proveedornom(string query)
    {
        string provee = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            provee = campo["nom_Centro"].ToString();
        }
        return provee;
    }

    public string sesion(string query)
    {
        string sesion = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            sesion = campo["sesion"].ToString();
        }
        return sesion;
    }

    public string tipoSoli(string query)
    {
        string tiposoli = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            tiposoli = campo["tipo"].ToString();
        }
        return tiposoli;
    }

    public string motivoSoli(string query)
    {
        string motivosoli = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            motivosoli = campo["motivo"].ToString();
        }
        return motivosoli;
    }

    public string envio(string query)
    {
        string envio = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            envio = campo["envio"].ToString();
        }
        return envio;
    }

    public string User(string query)
    {
        string user = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            user = campo["usuario"].ToString();
        }
        return user;
    }

    public string Concecio(string query)
    {
        string concecio = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            concecio = campo["concesionario"].ToString();
        }
        return concecio;
    }

    public string Dias(string query, string nombre)
    {
        string dias = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        if (nombre == "aereo")
        {
            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                dias = campo["aereo"].ToString();
            }
        }
        if (nombre == "courier")
        {
            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                dias = campo["courier"].ToString();
            }
        }
        if (nombre == "maritimo")
        {
            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                dias = campo["maritimo"].ToString();
            }
        }
        return dias;
    }

    public string Dias(string query)
    {
        string dias = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            dias = campo["dias"].ToString();
        }
        return dias;
    }

    public string usuariocotizacion(string query)
    {
        string nombre = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            nombre = campo["usuario"].ToString();
        }
        return nombre;
    }

    public List<string> CrearLocales(string rut)
    {
        List<string> listaLocales = new List<string>();
        DataSet _set = ObtenerDatosFiltrados(@"SELECT  DISTINCT
                                                scs.direccionSucursal as local
                                                FROM t_Cotiza tc INNER JOIN t_SolicitudCotizacion tsc ON tc.id_Cotiza = tsc.numeroSolicitud 
                                                INNER JOIN persona ps ON ps.rut = tc.rut_cotiza
                                                INNER JOIN sucursal scs ON scs.shipcode = ps.shipcode 
                                                INNER JOIN Concesionario cns ON cns.numerofactura = scs.numerofactura 
                                                WHERE cns.nombreConcesionario = tsc.concesionario 
                                                AND scs.nombreConcesionario = cns.nombreConcesionario 
                                                AND scs.nombreConcesionario = (SELECT top 1 LTRIM(RTRIM(nombreConcesionario)) 
                                                from sucursal where shipcode = (SELECT shipCode  FROM persona WHERE rut = '" + rut + "'))");
        foreach (DataRow campo in _set.Tables[0].Rows)
        {
            local = campo["local"].ToString();
            listaLocales.Add(local);
        }
        return listaLocales;
    }

    public string CodigoSAPConce(string sucursal)
    {
        string nombre = "";
        DataSet ds = ObtenerDatosFiltrados("select TOP 1 a.numeroFactura from concesionario a inner join sucursal b on a.numeroFactura = b.numeroFactura where b.shipCode = '" + sucursal + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            nombre = campo["numeroFactura"].ToString();
        }
        return nombre;
    }

    public string MultiSucursal(String query)
    {
        string dat = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow dato in ds.Tables[0].Rows)
        {
            dat = dato["multipleSucursal"].ToString();
        }
        return dat;
    }

    public string EmailCriticidad(string query)
    {
        string email = "";
        String correo = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            email = campo["Correos"].ToString();
        }
        return email;
    }

    public string corrreoVFC(string query)
    {
        string marca = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            marca = campo["email"].ToString();
        }
        return marca;
    }

    public string marcaVFC(string query)
    {
        string marca = "";
        DataSet ds = ObtenerDatosFiltrados(query);
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            marca = campo["marca"].ToString();
        }
        return marca;
    }
}