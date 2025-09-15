using System;
using System.Data;
using System.Data.Linq;
using System.Linq;
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
using System.Globalization;
using System.Threading;

/// <summary>
/// Clase ControlBD, esta clase controla todas las transacciones SQL, de los contenidos editables del sitio.
/// </summary>
[Obsolete("Debería estar deprecada, ya que cada clase de la capa Modelo debería tener sus consultas")]
public class ControlBD
{
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;

    Contenido _contenido;
    Marca _marca;
    Boletin _boletin;

    Canal _canal;
    MensajeSistema RespuestaBD = new MensajeSistema();


    //envio correo 
    SendMail_helper _mail = new SendMail_helper();

    public SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString);

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ControlBD));


    public ControlBD()
    {

    }

    #region Obtencion de Datos
    /// <summary>
    /// Método que devuelve datos de una consulta SQL, tiene como parametro entrante el tipo de contenido,
    /// este método retorna un DataSet con los datos traidos de la BD
    /// </summary>
    public DataSet ObtenerDatos(string query)
    {
        //La variable consulta es seteada según el parametro entrante
        string consulta = "";
        //if (query == "noticia") { consulta = "SELECT idContenido,cast (fecha as varchar) as fecha,html,titulo,tipo  FROM CONTENIDO WHERE TIPO = 1 order by idContenido desc"; }
        if (query == "casaMatriz0") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 9 order by idContenido desc"; }
        if (query == "casaMatriz1") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 3 order by idContenido desc"; }
        if (query == "casaMatriz2") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 4 order by idContenido desc"; }
        if (query == "casaMatriz3") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 5 order by idContenido desc"; }
        if (query == "promo1") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 6 order by idContenido desc"; }
        if (query == "promo2") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 7 order by idContenido desc"; }
        if (query == "promo3") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 8 order by idContenido desc"; }
        if (query == "promo4") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 10 order by idContenido desc"; }
        if (query == "infoGeneral") { consulta = "SELECT top 1 * FROM CONTENIDO WHERE TIPO = 11 order by idContenido desc"; }
        if (query == "accesorios") { consulta = "SELECT top 1 * FROM ACCESORIO order by idAccesorio desc"; }
        if (query == "linkInteres") { consulta = "SELECT * FROM linksInteres order by idLinks desc"; }
        if (query == "encuesta") { consulta = "SELECT top 1 * FROM encuesta order by idEncuesta desc"; }
        if (query == "lista") { consulta = "SELECT * FROM LISTA_PRODUCTOS_TEMP"; }
        if (query == "marca") { consulta = "SELECT * FROM MARCA"; }
        if (query == "boletines")
        {
            consulta = @"
            select idBoletin, titulo,descripcion,cast(fecha as varchar) as fecha,documento,('No') as leido
            from boletines;
        ";
        }
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

    /// <summary>
    /// Obtiene el nombre del concesionario segun el rut del usuario
    /// </summary>
    /// <param name="rutUser"></param>
    /// <returns></returns>
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
    //REQ - Cotizaciones Automaticas Marzo 2022
    public List<string> getGrupoTecnico()
    {
        string _grupo = "";
        List<string> listaGrupòTecnico = new List<string>();
        //DataSet _dSet = ObtenerDatosFiltrados("SELECT codigo_grupo_tecnico FROM PARAMETROS_GRUPO_TECNICO WHERE activo = '1' ");
        DataSet _dSet = ObtenerDatosFiltrados("SELECT codigo_grupo_tecnico FROM PARAMETROS_GRUPO_TECNICO ");
        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            _grupo = campos["codigo_grupo_tecnico"].ToString();
            listaGrupòTecnico.Add(_grupo);
        }
        return listaGrupòTecnico;
    }
    //REQ - Cotizaciones Automaticas Marzo 2022
    public string ObtenerSucursalID(string shipcode)
    {
        string sucursal = "";

        DataSet ds = ObtenerDatosFiltrados(@"select top 1 idSucursal
                                            from sucursal
                                            where sucursal.shipCode = '" + shipcode + "'");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            sucursal = campo["idSucursal"].ToString();
        }

        return sucursal;
    }

    /// <summary>
    /// Obtiene datos de la BD segun la query recibida como parametro
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public DataSet ObtenerDatosFiltrados(string query)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
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


    ///<summary>
    ///método que retorna una lista generica con todas las marcas existentes
    /// </summary>
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

    //Nueva consulta para cargar marcas 

    public  DataSet CargaMarca()
    {
        con = new SqlConnection();
        cmd = new SqlCommand();

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

        SqlDataAdapter dataAdapter = new SqlDataAdapter("sp_CargaMarcas", con);

        dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

        DataSet Datos = new DataSet();

        dataAdapter.Fill(Datos);

        if (Datos.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return Datos;
        }
    }

    //Nuevas funciones motivo pedido
    public List<string> TipoPedido(string marca)
    {
        string mpedido = "";
        List<string> listpedido = new List<string>();
        DataSet ds = ObtenerDatosFiltrados("SELECT ID_Motivo_pedido, MP_Descripcion FROM Motivo_Pedido WHERE MP_Flag = '1' AND MP_marca = '" + marca + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            mpedido = campo["MP_Descripcion"].ToString();
            listpedido.Add(mpedido);
        }
        return listpedido;
    }

    public string ObtieneGrupoMateriales(String MVGR)
    {
        string mpedido = "";
        //List<string> listpedido = new List<string>();
        DataSet ds = ObtenerDatosFiltrados("SELECT [Codigo_GM_Web] FROM [Grupo_Material_Web] WHERE [GM_Web] = '" + MVGR + "' AND [Flag_MG_web] = 'SI'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            mpedido = campo["Codigo_GM_Web"].ToString();
            //listpedido.Add(mpedido);
        }
        return mpedido;
    }


    public string TipoPedidoValue(string descripcion, string marca)
    {
        string mpedido = "";
        DataSet ds = ObtenerDatosFiltrados("SELECT MP_codigo FROM Motivo_Pedido WHERE MP_Descripcion = '" + descripcion + "' AND MP_marca = '" + marca + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            mpedido = campo["MP_Codigo"].ToString();
        }
        return mpedido;
    }



    public string TipoPedidoBloqueo(string codigoMP, string marca)
    {
        string mpedido = "";
        DataSet ds = ObtenerDatosFiltrados("SELECT MP_Bloqueo FROM Motivo_Pedido WHERE MP_Codigo = '" + codigoMP + "' AND MP_marca = '" + marca + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            mpedido = campo["MP_Bloqueo"].ToString();
        }
        return mpedido;
    }
    //fin nuevas funciones motivo pedido
    /// <summary>
    /// Este método recibe un parametro idContenido y en base a ese parametro muestra la noticia en una nueva página
    /// </summary>
    /// <param name="idCont"></param>
    /// <returns></returns>
    public DataSet MostrarContenido(string contenido)
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
                _comando.CommandText = "select * from contenido where idContenido = " + contenido + "";
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

    /// <summary>
    /// Obtiene las sucursales pertenecientes al dealer especificado
    /// </summary>
    /// <param name="nomConc"></param>
    /// <returns></returns>
    public List<string> ObtenerSucursalesPorConcesionario(string nomConc)
    {
        List<string> direccion = new List<string>();
        DataSet ds = ObtenerDatosFiltrados(@"select sucursal.direccionSucursal
                            from concesionario, sucursal
                            where sucursal.nombreConcesionario = concesionario.nombreConcesionario
                            and concesionario.nombreConcesionario = '" + nomConc + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            direccion.Add(dr["direccionSucursal"].ToString());
        }

        return direccion;

    }

    /// <summary>
    /// Obtiene el ShipCode según la direccion especificada
    /// </summary>
    /// <param name="direccion"></param>
    /// <returns></returns>
    public string GetShipCodPorDirec(string direccion)
    {
        string shipcod = "";
        DataSet ds = ObtenerDatosFiltrados("select shipCode from sucursal where direccionSucursal = '" + direccion + "' ");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            shipcod = dr["shipCode"].ToString();
        }

        return shipcod;
    }

    #endregion


    #region Métodos para insertar, actualizar y eliminar registros

    /// <summary>
    ///Método InsertarContenido, este método lo que hace es insertar registros en la tabla contenido,
    ///seteando la clase Contenido del modelo he insertando los objetos creados 
    /// </summary>
    /// <param name="marca"></param>
    /// <param name="html"></param>
    /// <param name="titulo"></param>
    /// <param name="tipo"></param>
    public void InsertarContenido(string html, string titulo, int tipo)
    {
        _contenido = new Contenido();
        _contenido.Html = html;
        _contenido.Titulo = titulo;
        _contenido.Tipo = tipo;


        string queryInsert = @"INSERT INTO CONTENIDO(fecha,html,titulo,tipo)
                                VALUES(GETDATE() , '" + _contenido.Html + "' , '" + _contenido.Titulo + "' , '" + _contenido.Tipo + "')";
        InsertarDatos(queryInsert);
    }

    /// <summary>
    /// Método InsertarContenidoImportante, este método lo que hace es insertar registros en la tabla contenido,
    /// que son importante
    /// </summary>
    /// <param name="marca"></param>
    /// <param name="html"></param>
    /// <param name="titulo"></param>
    /// <param name="tipo"></param>
    /// <param name="importancia"></param>
    public void InsertarContenidoImportante(string html, string titulo, int tipo)
    {
        _contenido = new Contenido();
        _contenido.Html = html;
        _contenido.Titulo = titulo;
        _contenido.Tipo = tipo;


        string queryInsert = @"INSERT INTO CONTENIDO(fecha,html,titulo,tipo,importante)
                                VALUES(GETDATE() , '" + _contenido.Html + "' , '" + _contenido.Titulo + "' , '" + _contenido.Tipo + "',1)";
        InsertarDatos(queryInsert);
    }

    /// <summary>
    /// Método Actualiza, actualiza una notica
    /// </summary>
    /// <param name="marca"></param>
    /// <param name="html"></param>
    /// <param name="titulo"></param>
    /// <param name="tipo"></param>
    public void ActualizaContenido(string html, string id)
    {
        _contenido = new Contenido();
        _contenido.Html = html;

        string queryInsert = @"UPDATE CONTENIDO set html='" + html + "' where idContenido='" + id + "'";
        InsertarDatos(queryInsert);
    }

    /// <summary>
    /// Método que inserta una cotización en la base de datos
    /// </summary>
    public void InsertarCotizacion(string nomConcesionario, string nomUsuario, string direccionDest, string emailUsusrio,
                                    string codigo, string marca, string descrip, float precio)
    {
        //Aquí todo el código
        string queryInsert = @"insert into cotizacion(fechaEmision,fechaExpiracion,concesionario,usuario,direccion,
                                email,codigoRep,marca,descripcionRep,precio)
                                values(GETDATE() , GETDATE() , '" + nomConcesionario + "' , '" + nomUsuario + "' ,'" + direccionDest + "' , '" + emailUsusrio + "' , '" + codigo + "' , '" + marca + "' , '" + descrip + "' , " + precio + ")";
    }

    /// <summary>
    ///Método insertar Boletin, este método lo que hace es insertar registros en la tabla boletines,
    ///seteando la clase Boletin del modelo he insertando los objetos creados
    /// </summary>
    /// <param name="tiutlo"></param>
    /// <param name="descripcion"></param>
    /// <param name="documento"></param>
    public void InsertarBoletin(string tiutlo, string descripcion, string documento)
    {
        _boletin = new Boletin();
        _boletin.Titulo = tiutlo;
        _boletin.Descripcion = descripcion;
        _boletin.Documento = documento;

        string queryInsert = @"INSERT INTO boletines(titulo,descripcion,fecha,documento)
                                VALUES('" + _boletin.Titulo + "' , '" + _boletin.Descripcion + "' , GETDATE() ,  '" + _boletin.Documento + "')";
        InsertarDatos(queryInsert);
    }

    /// <summary>
    /// Método para insertar Links de interes en la bases de datos
    /// </summary>
    /// <param name="url"></param>
    /// <param name="descripcion"></param>
    public void InsertarLinksDeInteres(string url, string descripcion)
    {
        string idlink = "";
        DataSet ds = ObtenerDatosFiltrados(@"select top 1 idLinks from linksInteres order by idLinks desc");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            idlink = campo["idLinks"].ToString();
        }
        int idfinal = int.Parse(idlink) + 1;


        string queryInsert = @"INSERT INTO linksInteres(idLinks,url,descripcion,fechaCreacion)
                                VALUES(" + idfinal + " ,'" + url + "' , '" + descripcion + "' , GETDATE())";
        InsertarDatos(queryInsert);
    }

    /// <summary>
    ///Método que inserta datos en la Base de Datos, este método inserta datos en la base de daatos recibiendo como
    ///parametro una consulta SQL de tipo INSERT. 
    /// </summary>
    /// <param name="query"></param>
    public Boolean InsertarDatos(string query)
    {
        Boolean returnValue = true;
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
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

    public void UpdateFechasExpiracion(string query_sp, string dato_sp)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString))
        {
            try
            {
                //Abrimos la conexión a la base de datos
                conn.Open();
                //Creamos el comando que contendrá la query a ejecutar en el servidor
                SqlCommand _query = conn.CreateCommand();

                _query.CommandType = CommandType.StoredProcedure;
                _query.CommandText = query_sp;
                _query.Parameters.Add("@NUM_PEDIDO", SqlDbType.NVarChar).Value = dato_sp;
                //Especifiamos la query a ejecutar
                _query.CommandText = string.Format(query_sp);

                int fil = _query.ExecuteNonQuery();

                if (fil > 0)
                {
                    //Todo ha ido bien
                }
                else
                {
                    //No se ha ejecutado ni una query
                    // _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en [UpdateFechasExpiracion]. Message: Procedimiento SP_CADUCAR_COTIZACION no ejecuto de forma correcta . Num Cotizacion: " + dato_sp);

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error en [UpdateFechasExpiracion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en [UpdateFechasExpiracion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + " . Num Cotizacion: " + dato_sp);


            }
            finally
            {
                //Nos aseguramos de cerrar la conexión
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }


    }

    /// <summary>
    /// Método que elimina un registro de la base de datos, recibe como parametros el nombre
    /// de la tabla y el id del registro.
    /// </summary>
    public void EliminarRegistro(string tabla, string id, string idDelete)
    {
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
                string queryInsert = "delete " + tabla + " where " + idDelete + " = " + id + " ";
                _query.CommandText = string.Format(queryInsert);

                //Como es un INSERT, la query no devuelve resultados, asi que ejecutamos un ExecuteNonQuery que nos
                //devuelve el número de filas afectadas
                int fil = _query.ExecuteNonQuery();

                if (fil > 0)
                {
                    //Todo ha ido bien
                }
                else
                {
                    //No se ha ejecutado correctamente
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error en [EliminarRegistro]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
            finally
            {
                //Nos aseguramos de cerrar la conexión
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
    }

    /// <summary>
    /// Método que agrega una nueva encuesta.
    /// </summary>
    /// <param name="pregunta"></param>
    /// <param name="texRes1"></param>
    /// <param name="texRes2"></param>
    /// <param name="texRes3"></param>
    /// <param name="texRes4"></param>
    /// <param name="votosR1"></param>
    /// <param name="votosR2"></param>
    /// <param name="votosR3"></param>
    /// <param name="votosR4"></param>
    /// <param name="NumOpciones"></param>
    /// <param name="fechaTermino"></param>
    public void PublicarEncuesta(string pregunta, string texRes1, string texRes2, string texRes3, string texRes4,
                                 int votosR1, int votosR2, int votosR3, int votosR4, int NumOpciones)//, string fechaTermino)
    {
        string idencuesta = "";
        DataSet ds = ObtenerDatosFiltrados(@"select top 1 idEncuesta from encuesta order by idEncuesta desc");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            idencuesta = campo["idEncuesta"].ToString();
        }
        int idfinal = int.Parse(idencuesta) + 1;

        string queryInsert = @"insert into encuesta(idEncuesta, pregunta,texRes1,texRes2,texRes3,texRes4,votosR1,votosR2,votosR3,votosR4, NumOpciones,inicio)
                             values (" + idfinal + ",'" + pregunta + "','" + texRes1 + "','" + texRes2 + "','" + texRes3 + "','" + texRes4 + "'," + votosR1 + " , " + votosR2 + " , " + votosR3 + "," + votosR4 + "," + NumOpciones + ", GETDATE())";

        InsertarDatos(queryInsert);
    }

    /// <summary>
    /// Método que permite votar en una encuesta, recibe como parametro la alternativa seleccionada del RadioButton
    /// </summary>
    /// <param name="alternativa"></param>
    public void VotarEncuesta(string idEnc, string alter, int voto, string user)
    {
        //inserto en la tabla encuesta la alternativa seleccionada
        string queryVotar = "update encuesta set " + alter + " = " + voto + "  where idEncuesta = " + idEnc + "";
        InsertarDatos(queryVotar);

        //creo un registro del usuario que ha votado en la encuesta
        string queryUserVoto = "insert into encuesta_user(idEncuesta,rutUser) values(" + idEnc + ",'" + user + "')";
        InsertarDatos(queryUserVoto);

    }

    /// <summary>
    /// Inserta En una lista los repuestos consultados por código al SAP
    /// </summary>
    /// <param name="EZ_BEZEI"></param>
    /// <param name="EZ_MFRPN"></param>
    /// <param name="EZ_MAKTX"></param>
    /// <param name="EZ_KONDM"></param>
    /// <param name="STOCK"></param>
    /// <param name="EZ_KBETR1"></param>
    /// <param name="EZ_KBETR2"></param>
    public void InsertarListaRepuestos(string idSession, string EZ_BEZEI, string EZ_MFRPN, string EZ_MAKTX, string EZ_KONDM, double STOCK, string EZ_KBETR1, string EZ_KBETR2, string cantidad, int total, int totalLista)
    {
        string queryLista = @"insert into LISTA_PRODUCTOS_TEMP(sessionId,marca,codigo,descripcion,grupoMat,stock,precio,precioConce,cantidad,total,totalLista)
                            values('" + idSession + "','" + EZ_BEZEI + "','" + EZ_MFRPN + "','" + EZ_MAKTX + "','" + EZ_KONDM + "','" + STOCK + "','" + EZ_KBETR1 + "','" + EZ_KBETR2 + "', " + cantidad + " , " + total + " , " + totalLista + ")";
        InsertarDatos(queryLista);
    }


    #endregion

    /// <summary>
    /// Método que valida si el usuario actual ha votado en la encuesta
    /// </summary>
    /// <param name="idEnc"></param>
    /// <param name="rutUser"></param>
    /// <returns></returns>
    public bool ValidarVotoEncuesta(string idEnc, string rutUser)
    {
        try
        {
            DataSet ds = ObtenerDatosFiltrados("select * from encuesta_user where idEncuesta = " + idEnc + " and rutUser = '" + rutUser + "'");

            if (ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        catch (NullReferenceException ex)
        {
            logger.Error("Error en la encuesta en [ValidarVotoEncuesta]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            return false;
        }
    }

    public int getContenidoIndice()
    {
        string idcontenido = "";
        DataSet ds = ObtenerDatosFiltrados(@"select top 1 idContenido from contenido order by idContenido desc");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            idcontenido = campo["idContenido"].ToString();
        }
        int idfinal = int.Parse(idcontenido) + 1;

        return idfinal;
    }

    public int getAccesorioUltimoIndice()
    {
        string idAccesorio = "";
        DataSet ds = ObtenerDatosFiltrados(@"select top 1 idAccesorio from accesorio order by idAccesorio desc");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            idAccesorio = campo["idAccesorio"].ToString();
        }
        int idfinal = int.Parse(idAccesorio);

        return idfinal;
    }

    public int getidModeloByModelo(string modelo)
    {
        string idModelo = "";
        DataSet ds = ObtenerDatosFiltrados(@"select top 1 idModelo from modelo where nombreModelo = '" + modelo + "' order by idModelo desc");

        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            idModelo = campo["idModelo"].ToString();
        }
        int idfinal = int.Parse(idModelo);

        return idfinal;
    }

    public DataSet obtienedealercotiza(int id, string cotizacion)
    {
        con = new SqlConnection();
        cmd = new SqlCommand();
        DataSet ds2;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        try
        {
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_datos_cotiza";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@i_cotizacion", SqlDbType.Int).Value = Convert.ToInt32(cotizacion);
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            ds2 = new DataSet();
            da.Fill(ds2);

        }
        catch (Exception ex)
        {
            con.Close();
            ds2 = null;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error obtienedealercotiza", "MENSAJE" + ex + " ID:" + id + ". cotización:" + cotizacion + "", "");
        }
        finally
        {
            con.Close();

        }

        return ds2;


    }

    public string SeguimientoPedido(string sesId,
                                    string identificador,
                                    string marca,
                                    string socSap,
                                    string canalDistribucion,
                                    string codMaterialSap,
                                    string descrpMaterial,
                                    string grupoMtrlSap,
                                    string grupoMtrlFrec,
                                    int cantCotizada,
                                    double precioListaSug,
                                    int descuento,
                                    double precioConcecionario,
                                    int stockConsulta,
                                    int agregaCarro,
                                    string nroCotizacion,
                                    string nroPedidoSap,
                                    int ultimoEvento,
                                    int rptoExiste,
                                    int vfcReserva,
                                    string ipPc,
                                    int cadenaReemplazo)
    {

        con = new SqlConnection();
        cmd = new SqlCommand();
        string errorSalida;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();


        try
        {

            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "LOG_SEGUIMIENTO_GRABAR";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_sess_id", SqlDbType.VarChar, 50).Value = sesId;
            cmd.Parameters.Add("@i_identificador", SqlDbType.VarChar, 20).Value = identificador;
            cmd.Parameters.Add("@i_marca", SqlDbType.VarChar, 50).Value = marca;
            cmd.Parameters.Add("@i_sociedad_sap", SqlDbType.VarChar, 20).Value = socSap;
            cmd.Parameters.Add("@i_canal_distribucion", SqlDbType.VarChar, 50).Value = canalDistribucion;
            cmd.Parameters.Add("@i_codigo_material_sap", SqlDbType.VarChar, 50).Value = codMaterialSap;
            cmd.Parameters.Add("@i_descripcion_material", SqlDbType.VarChar, 150).Value = descrpMaterial;
            cmd.Parameters.Add("@i_grupo_material_sap", SqlDbType.VarChar, 10).Value = grupoMtrlSap;
            cmd.Parameters.Add("@i_grupo_material_frec", SqlDbType.VarChar, 20).Value = grupoMtrlFrec;
            cmd.Parameters.Add("@i_cantidad_cotizada", SqlDbType.Int).Value = cantCotizada;
            cmd.Parameters.Add("@i_precio_lista_sugerido", SqlDbType.Int).Value = precioListaSug;
            cmd.Parameters.Add("@i_descuento", SqlDbType.Int).Value = descuento;
            cmd.Parameters.Add("@i_precio_concesionario", SqlDbType.Int).Value = precioConcecionario;
            cmd.Parameters.Add("@i_stock_consulta", SqlDbType.Int).Value = stockConsulta;
            cmd.Parameters.Add("@i_agrega_carro", SqlDbType.Bit).Value = agregaCarro;
            cmd.Parameters.Add("@i_numero_cotizacion", SqlDbType.VarChar, 20).Value = nroCotizacion;
            cmd.Parameters.Add("@i_nro_pedido_sap", SqlDbType.VarChar, 50).Value = nroPedidoSap;
            cmd.Parameters.Add("@i_ultimo_evento", SqlDbType.Int).Value = ultimoEvento;
            cmd.Parameters.Add("@i_rpto_existe", SqlDbType.Int).Value = rptoExiste;
            cmd.Parameters.Add("@i_vfc_reserva", SqlDbType.Int).Value = vfcReserva;
            cmd.Parameters.Add("@i_ip_pc", SqlDbType.VarChar, 50).Value = ipPc;
            cmd.Parameters.Add("@i_cadena_reemplazo", SqlDbType.Int).Value = cadenaReemplazo;
            cmd.Parameters.Add("@o_codigo_salida", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            errorSalida = "";
            int codigoError = 0;


        }
        catch (Exception ex)
        {
            con.Close();
            errorSalida = cmd.Parameters["@o_msg_error"].Value.ToString();
            _mail.EnviarCorreo("czurita@skberge.cl", "Error SeguimientoPedido", "ERROR: " + errorSalida + " MENSAJE: " + ex + "", "");
        }

        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            cmd = null;
            con = null;

        }

        //cmd.Parameters.Clear();
        //con.Close();

        return errorSalida;
    }

    public string obtstock(string session, string codrep)
    {
        string stock = "";
        DataSet ds = ObtenerDatosFiltrados("select stock from carro where idSession = '" + session + "' and codigo = '" + codrep + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            stock = campo["stock"].ToString();
           
        }
        return stock;
    }

    public DataSet ShipCodeSucursal(string nomConc)
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
                _comando.CommandText = "select sucursal.shipCode, sucursal.direccionSucursal from concesionario, sucursal where sucursal.nombreConcesionario = concesionario.nombreConcesionario and concesionario.nombreConcesionario = '" + nomConc + "'";
                _adapter.SelectCommand = _comando;
                _adapter.Fill(_dSet);
            }
        }
        catch
        {
            _dSet = null;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error ShipCodeSucursal", "select sucursal.shipCode, sucursal.direccionSucursal from concesionario, sucursal where sucursal.nombreConcesionario = concesionario.nombreConcesionario and concesionario.nombreConcesionario = '" + nomConc + "'", "");
            _conection.Close();

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

    public DataSet ObtenerListaEmail(string query)
    {
        DataSet _dset = new DataSet();
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adp = new SqlDataAdapter();

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        try
        {
            if (con.State == ConnectionState.Open)
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                adp.SelectCommand = cmd;
                adp.Fill(_dset);
            }
        }
        catch (Exception ex)
        {
            _dset = null;
            con.Close();
            //logger.Error("Error en [ObtenerDatosFiltrados]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            cmd = null;
            con = null;
            adp = null;
        }
        return _dset;
    }

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
                //logger.Error("Error en [InsertarDatos]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + query + "]");
                conn.Close();
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error EjecutaQuery", "Erroe. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + query + "]", "");
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

    public string GetParametroByKey(string llave)
    {
        string retorno = "";
        int? nro_error = null;
        string msg_error = null;

        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        var result = (from i in ctx.sp_get_parametro_sistema(llave, ref nro_error, ref msg_error)
                    select i).FirstOrDefault();

        if (result != null)
        {
            retorno = result.Valor; ;
        }

        return retorno;
    }

    public DataSet reportePedidos(string marca, string fechaIni, string fechaFin)
    {

        con = new SqlConnection();
        cmd = new SqlCommand();
        DataSet ds2 = new DataSet();

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        try
        {
            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_ia_reporte_pedidos";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@marca", SqlDbType.VarChar, 50).Value = marca;
            cmd.Parameters.Add("@desde", SqlDbType.VarChar, 10).Value = fechaIni;
            cmd.Parameters.Add("@hasta", SqlDbType.VarChar, 10).Value = fechaFin;
            cmd.Parameters.Add("@xls", SqlDbType.VarChar, 2).Value = "SI";
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds2);

        }
        catch (Exception ex)
        {
            con.Close();
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error reportePedidos", "MENSAJE" + ex, "");
            ds2 = null;

        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            cmd = null;
            con = null;
        }

        return ds2;
    }
    // REQ - PRECIO FIJOS DE REPUESTOS -  MARZO 2022
    public int activafrecuencia(string material, string marca)
    {


        if (material == null)
        {
            material = "";
        }
        con = new SqlConnection();
        cmd = new SqlCommand();
        int activa = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        try
        {
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "FRNC_OBTIENE_FRECUENCIA";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@i_material", SqlDbType.VarChar).Value = material;
            cmd.Parameters.Add("@i_marca", SqlDbType.VarChar).Value = marca;
            cmd.Parameters.Add("@o_activa", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();


            activa = Convert.ToInt32(cmd.Parameters["@o_activa"].Value.ToString());
            cmd.Parameters.Clear();


        }
        catch (Exception ex)
        {
            con.Close();
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "obtieneHomologo ", "MENSAJE: " + ex + " Material: " + material + "", "");

        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            cmd = null;
            con = null;
        }

        //con.Close();
        return activa;


    }
    public string obtieneHomologo(string material)
    {


        if (material == null)
        {
            material = "";
        }
        con = new SqlConnection();
        cmd = new SqlCommand();
        string material_b;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        try
        {
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_homologo";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@i_material", SqlDbType.VarChar).Value = material;
            cmd.Parameters.Add("@o_material", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();


            material_b = cmd.Parameters["@o_material"].Value.ToString();
            cmd.Parameters.Clear();


        }
        catch (Exception ex)
        {
            material_b = "";
            con.Close();
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "obtieneHomologo ", "MENSAJE: " + ex + " Material: " + material + "", "");

        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            cmd = null;
            con = null;
        }

        //con.Close();
        return material_b;


    }
    // REQ - PRECIO FIJOS DE REPUESTOS -  MARZO 2022


    //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 
    public DataSet SeguimientoPedidoBuscar(string marca, string fechaIni, string fechaFin, string evento)
    {

        con = new SqlConnection();
        cmd = new SqlCommand();
        DataSet ds2 = new DataSet();

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        try
        {
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "LOG_SEGUIMIENTO_EXPORTAR";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_marca", SqlDbType.VarChar, 100).Value = marca;
            cmd.Parameters.Add("@i_fecha_inicio", SqlDbType.VarChar, 20).Value = fechaIni;
            cmd.Parameters.Add("@i_fecha_fin", SqlDbType.VarChar, 20).Value = fechaFin;
            cmd.Parameters.Add("@i_evento", SqlDbType.VarChar, 30).Value = evento;
            cmd.Parameters.Add("@o_codigo_salida", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds2);

        }
        catch (Exception ex)
        {
            con.Close();
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error SeguimientoPedidoBuscar", "MENSAJE" + ex + ". EVENTO: " + evento + "", "");
            ds2 = null;

        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();
            cmd = null;
            con = null;
        }

        return ds2;
    }

    //Seguimiento --> REQ FEbrero 2022 --> Graba consulta Seguimiento 


     //se agrega consulta para el modulo cotizaciones automaticas 
    public List<string> CanalesPorUsuario(string rutUsuario, string idconcesionario)
    {
        _canal = new Canal();
        List<string> listaCanalesVenta = new List<string>();

        DataSet _dSet = ObtenerDatosFiltrados(" SELECT " +
                                              "     CV.descripcion" +
                                              " FROM " +
                                              "     dbo.canales_venta_usuario_canal CVUC, " +
                                              "     dbo.canales_venta_concesionario CVC, " +
                                              "     dbo.canales_venta CV " +
                                              " WHERE " +
                                              " CVUC.rut_usuario = '" + rutUsuario + "' " +
                                              " AND CVUC.id_concesionario = CVC.id_concesionario " +
                                              " AND CVUC.id_canal = CVC.id_canal " +
                                              " AND CVC.habilitado = 1 " +
                                              " AND CVUC.habilitado = 1 " +
                                              " AND CV.id_canal = CVUC.id_canal" +
                                              " AND CVUC.id_concesionario = '" + idconcesionario + "' ");

        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            _canal.descripcion = campos["descripcion"].ToString();
            listaCanalesVenta.Add(_canal.descripcion);
        }
        return listaCanalesVenta;
    }

    public List<string> SubclientesPorCanal(string idConcesionario, string canalVenta)
    {
        int _idConcesionario = 0;
        int _idCanal = 0;

        DataSet dsIdConcesionario = ObtenerDatosFiltrados(" SELECT idConcesionario FROM concesionario WHERE RTRIM(LTRIM(nombreConcesionario)) = '" + idConcesionario + "' ");
        foreach (DataRow drIdConcesionario in dsIdConcesionario.Tables[0].Rows)
        {
            _idConcesionario = Convert.ToInt32(drIdConcesionario["idConcesionario"]);
        }

        DataSet dsIdCanal = ObtenerDatosFiltrados(" SELECT id_canal FROM canales_venta WHERE descripcion = '" + canalVenta + "' ");
        foreach (DataRow drIdCanal in dsIdCanal.Tables[0].Rows)
        {
            _idCanal = Convert.ToInt32(drIdCanal["id_canal"]);
        }

        _canal = new Canal();
        List<string> listaCanalesVenta = new List<string>();
        DataSet _dSet = ObtenerDatosFiltrados(" SELECT " +
            "                                   (SCC.rut_subcliente + ' / ' + SC.razon_social) AS subcliente " +
            "                                   FROM " +
            "                                       dbo.canales_venta_subcliente_canal SCC, " +
            "                                       dbo.canales_venta_subclientes SC " +
            "                                   WHERE " +
            "                                       SCC.id_concesionario = '" + _idConcesionario + "' " +
            "                                   AND " +
            "                                       SCC.id_canal = '" + _idCanal + "' " +
            "                                   AND " +
            "                                       SCC.habilitado = 1 " +
            "                                   AND " +
            "                                       SC.habilitado = 1 " +
            "                                   " +
            "                                   AND " +
            "                                       SCC.rut_subcliente = SC.rut_subcliente ");
        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            _canal.descripcion = campos["subcliente"].ToString();
            listaCanalesVenta.Add(_canal.descripcion);
        }
        return listaCanalesVenta;
    }

    //Anibal Berrios Requerimiento Bolvia 19-08-2025
    public DataSet CargaComboClasePedido(int IdUsuario)
    {
        SqlDataAdapter dataAdapter = new SqlDataAdapter("SP_CargaClasePedido", conexion);
        dataAdapter.SelectCommand.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = IdUsuario;

        dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

        DataSet dataSet = new DataSet();

        dataAdapter.Fill(dataSet);

        if (dataSet.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return dataSet;
        }
    }

    public DataSet CargaSectorMarca(string marca)
    {
        SqlDataAdapter dataAdapter = new SqlDataAdapter("SP_CargaSectorMarca", conexion);

        dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
        dataAdapter.SelectCommand.Parameters.Add("@marca", SqlDbType.VarChar, 100).Value = marca;

        DataSet dataSet = new DataSet();

        dataAdapter.Fill(dataSet);

        if (dataSet.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return dataSet;
        }
    }

    //*************CODIGO MANTENEDOR CLASE PEDIDO**********///

    //Bloque insertar nueva clase 

    public MensajeSistema InsertaClasePedido(string NombreClasePedido, string CodigoClasePedido)
    {
        ControlBD cBD = new ControlBD();
        try
        {
            cBD.conexion.Open();

            SqlCommand comando = new SqlCommand("SP_InsertaClasePedido", cBD.conexion)
            {
                CommandType = CommandType.StoredProcedure
            };

            comando.Parameters.Add("@NombreClase", SqlDbType.Char, 50).Value = NombreClasePedido;
            comando.Parameters.Add("@CodigoClase", SqlDbType.Char, 50).Value = CodigoClasePedido;

            comando.Parameters.Add("@errcod", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
            comando.Parameters.Add("@errmsje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

            comando.ExecuteNonQuery();

            RespuestaBD.Codigo = comando.Parameters["@errcod"].Value.ToString();
            RespuestaBD.Mensaje = comando.Parameters["@errmsje"].Value.ToString().Replace("'", "");

            return RespuestaBD;

        }
        catch (Exception e)
        {
            RespuestaBD.Codigo = "EXCEPCION";
            RespuestaBD.Mensaje = e.Message.ToString().Replace("'", "");
            return RespuestaBD;
        }
        finally
        {
            cBD.conexion.Close();
        }
    }

    //Bloque actualizar clase
    //Carga ComboBox
    public DataSet CargaComboClasePedidoMantedor()
    {
        try
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SP_CargaClasePedidoMantenedor", conexion);

            dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dataSet;
            }
        }
        catch(Exception ex)
        {
            return null;
        }
    }


}

    