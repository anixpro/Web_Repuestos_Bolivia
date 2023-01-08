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
using System.Data.Sql;
using System.Data.SqlClient;
/// <summary>
/// Descripción breve de SQL_DevRec
/// </summary>
public class SQL_DevRec
{
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;

    public SQL_DevRec()
    {

    }

    public void InsertaDocumentos(int identificador,
                                string Session,
                                string usuario,
                                string nropedido,
                                string pospedido,
                                string nrodoc,
                                string posdoc,
                                int cantidad,
                                string metrica,
                                decimal neto,
                                decimal iva,
                                decimal subtotal,
                                string moneda,
                                string codrepuesto,
                                string descrepuesto,
                                string fechadoc,
                                string url,
                                string usuariosap,
                                string zona,
                                string prefijo,
                                out int coderror,
                                out string msgerror)
    {

        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_INSERTA_DATOS_DOCUEMENTOS";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_id_session", SqlDbType.VarChar).Value = Session;
        cmd.Parameters.Add("@i_id_usuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_nro_pedido", SqlDbType.VarChar).Value = nropedido;
        cmd.Parameters.Add("@i_pos_pedido", SqlDbType.VarChar).Value = posdoc;        
        cmd.Parameters.Add("@i_nro_doc", SqlDbType.VarChar).Value = nrodoc;
        cmd.Parameters.Add("@i_pos_doc", SqlDbType.VarChar).Value = posdoc;
        cmd.Parameters.Add("@i_cantidad_sol", SqlDbType.VarChar).Value = cantidad;
        cmd.Parameters.Add("@i_metrica", SqlDbType.VarChar).Value = metrica;
        cmd.Parameters.Add("@i_valor_neto", SqlDbType.Decimal).Value = neto;
        cmd.Parameters.Add("@i_valor_iva", SqlDbType.Decimal).Value = iva;
        cmd.Parameters.Add("@i_valor_subtotal", SqlDbType.Decimal).Value = subtotal;
        cmd.Parameters.Add("@i_moneda", SqlDbType.VarChar).Value = moneda;
        cmd.Parameters.Add("@i_codigo_repuesto", SqlDbType.VarChar).Value = codrepuesto;
        cmd.Parameters.Add("@i_descripcion_repuesto", SqlDbType.VarChar).Value = descrepuesto;
        cmd.Parameters.Add("@i_fecha_doc", SqlDbType.VarChar).Value = fechadoc;
        cmd.Parameters.Add("@i_url_doc", SqlDbType.VarChar).Value = url;
        cmd.Parameters.Add("@i_usuario", SqlDbType.VarChar).Value = usuariosap;
        cmd.Parameters.Add("@i_zona", SqlDbType.VarChar).Value = zona;
        cmd.Parameters.Add("@i_prefijo", SqlDbType.VarChar).Value = prefijo;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.ExecuteNonQuery();

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();

    }
    //REQ - Cotizaciones Automaticas Marzo 2022
    public DataSet obtieneTramosGrupoTecnico(string marca, string grupoTecnico, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "webr_fob_obtiene_tramos_grupos_tecnico";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@marca", SqlDbType.VarChar).Value = marca;
        cmd.Parameters.Add("@grupoTecnico", SqlDbType.VarChar).Value = grupoTecnico;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        da.Fill(ds);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds;
    }
    public DataSet obtieneValoresMonedas(out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "webr_obtiene_valores_monedas";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }
    public DataSet obtieneLogParidad(out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "webr_obtiene_log_paridad";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }
    public DataSet obtieneFactorUtilidad(string marca, string grupoTecnico, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "webr_fob_obtiene_factor_utilidad";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@marca", SqlDbType.VarChar).Value = marca;
        cmd.Parameters.Add("@grupoTecnico", SqlDbType.VarChar).Value = grupoTecnico;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        da.Fill(ds);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds;
    }
    public DataSet obtieneSkuFob(string codigo, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "webr_fob_obtiene_sku";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@codigo", SqlDbType.VarChar).Value = codigo;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }
    //REQ - Cotizaciones Automaticas Marzo 2022
    public DataSet llenaGrillaFactura(int identificador, string nro_factura, string id_session, string id_usuario, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_GENERA_LOGICA_MOSTRAR";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_factura", SqlDbType.VarChar).Value = nro_factura;
        cmd.Parameters.Add("@i_session", SqlDbType.VarChar).Value = id_session;
        cmd.Parameters.Add("@i_usuario", SqlDbType.VarChar).Value = id_usuario;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }

    public DataSet llenacombomotivo(int identificador, out int coderror, out string msgerror, out int flag)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_CARGA_PARAMETROS";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_flag", SqlDbType.Int).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        flag = System.Convert.ToInt32(cmd.Parameters["@o_flag"].Value);
        con.Close();
        return ds2;
    }

    public DataSet cargacombos(int identificador, string usuario, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_GET_PARAMETROS_LOCALES";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_usuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }

    public string rutconcesionario(int identificador, string usuario, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";
        string rut = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_GET_PARAMETROS_LOCALES";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_usuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }
        else
        {
            foreach (DataRow campo in ds2.Tables[0].Rows)
            {
                rut = campo["rutholding"].ToString();
            }
        }


        con.Close();
        return rut;   
    
    }

    public void borradatossession(int identificador, string usuario, string session, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_DEL_CONSULTAS";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_usuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_session", SqlDbType.VarChar).Value = session;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.ExecuteNonQuery();

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }
        con.Close();
    }

    public void creasolicitud(int identificador,int idsol, string usuario, string session, string nrodoc, string posdoc, int cantidad, decimal neto, string codrepuesto, string descrepuesto, string fechadoc, int cant_sol_dr, string RoD, DateTime fecha_sol, int motivodr, string evidencias, string nrotrans, string urlfac, string zona, string prefijo,string marca,  out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_INSERTA_SOLICITUD";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_marca", SqlDbType.VarChar).Value = marca;
        cmd.Parameters.Add("@i_id_sol", SqlDbType.BigInt).Value = idsol;
        cmd.Parameters.Add("@i_id_session", SqlDbType.VarChar).Value = session;
        cmd.Parameters.Add("@i_id_usuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_nro_doc", SqlDbType.VarChar).Value = nrodoc;
        cmd.Parameters.Add("@i_pos_doc", SqlDbType.VarChar).Value = posdoc;
        cmd.Parameters.Add("@i_valor_neto", SqlDbType.Decimal).Value = neto;
        cmd.Parameters.Add("@i_cantidad_sol", SqlDbType.Int).Value = cantidad;
        cmd.Parameters.Add("@i_codigo_repuesto", SqlDbType.VarChar).Value = codrepuesto;
        cmd.Parameters.Add("@i_descripcion_repuesto", SqlDbType.VarChar).Value = descrepuesto;
        cmd.Parameters.Add("@i_fecha_doc", SqlDbType.VarChar).Value = fechadoc;
        cmd.Parameters.Add("@i_cant_sol_DR", SqlDbType.Int).Value = cant_sol_dr;
        cmd.Parameters.Add("@i_reclamo_devolucion", SqlDbType.VarChar).Value = RoD;
        cmd.Parameters.Add("@i_fecha_solicitud", SqlDbType.DateTime).Value = fecha_sol;
        cmd.Parameters.Add("@i_motivo_solicitud", SqlDbType.Int).Value = motivodr;
        cmd.Parameters.Add("@i_evidencia", SqlDbType.VarChar).Value = evidencias;
        cmd.Parameters.Add("@i_nrotransporte", SqlDbType.VarChar).Value = nrotrans;
        cmd.Parameters.Add("@i_urlfac", SqlDbType.VarChar).Value = urlfac;
        cmd.Parameters.Add("@i_zona", SqlDbType.VarChar).Value = zona;
        cmd.Parameters.Add("@i_prefijo", SqlDbType.VarChar).Value = prefijo;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
         cmd.ExecuteNonQuery();

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }
        con.Close();
    }

    public DataSet cargasolicitudes(int identificador, string usuario, int idsolicitud, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_CARGA_SOLICITUDES";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_idusuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_idSolicitud", SqlDbType.BigInt).Value = idsolicitud;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }

    public DataSet cargaSolicitudesFiltradas(int identificador, string usuario, out int coderror, out string msgerror, string solId, string facId)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_CARGA_SOLICITUDES_FILTRADAS";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_idusuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_idSolicitud", SqlDbType.VarChar).Value = solId;
        cmd.Parameters.Add("@i_idFactura", SqlDbType.VarChar).Value = facId;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }

    public DataSet cargasolicitudesfiltradas(int identificador, string usuario, int idsolicitud, string nroaprobacion, string shipcode, string fecha, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_CARGA_SOLICITUDES_FILTRADO";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_idusuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_idSolicitud", SqlDbType.BigInt).Value = idsolicitud;
        cmd.Parameters.Add("@i_nroaprobacion", SqlDbType.VarChar).Value = nroaprobacion;
        cmd.Parameters.Add("@i_shipcode", SqlDbType.VarChar).Value = shipcode;
        cmd.Parameters.Add("@i_fecha", SqlDbType.VarChar).Value = fecha;
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }
    public void CambiaEstadosSolicitud(int identificador, 
                                        string usuario, 
                                        int solicitud, 
                                        string nrofac, 
                                        string posfac, 
                                        DateTime fecha, 
                                        string aprueba, 
                                        int vbcomercial, 
                                        int motivorechazo, 
                                        string pedidoNC, 
                                        string comentario, 
                                        string responsable, 
                                        string observacion,
                                        string observacion_comercial,
                                        out int coderror, 
                                        out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_UPD_ESTADO";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_idusuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_idSolicitud", SqlDbType.BigInt).Value = solicitud;
        cmd.Parameters.Add("@i_nrofactura", SqlDbType.VarChar).Value = nrofac;
        cmd.Parameters.Add("@i_posfactura", SqlDbType.VarChar).Value = posfac;
        cmd.Parameters.Add("@i_fecha", SqlDbType.DateTime).Value = fecha;
        cmd.Parameters.Add("@i_aprueba", SqlDbType.VarChar).Value = aprueba;
        cmd.Parameters.Add("@i_vbcomercial", SqlDbType.Bit).Value = vbcomercial;
        cmd.Parameters.Add("@i_motivorechazo", SqlDbType.Int).Value = motivorechazo;
        cmd.Parameters.Add("@i_pedidoNC", SqlDbType.VarChar).Value = pedidoNC;
        cmd.Parameters.Add("@i_comentario", SqlDbType.VarChar).Value = comentario;
        cmd.Parameters.Add("@i_responsable", SqlDbType.VarChar).Value = responsable;
        cmd.Parameters.Add("@i_observacion", SqlDbType.VarChar).Value = observacion;
        cmd.Parameters.Add("@i_observacion_comercial", SqlDbType.VarChar).Value = observacion_comercial;
       
        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.ExecuteNonQuery();

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }
        con.Close();
    }

    public void EstadosCDRreclamos(int identificador, string usuario, int solicitud, string nrofac, string posfac, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_UPD_ESTADO_CDR";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_idusuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_idSolicitud", SqlDbType.BigInt).Value = solicitud;
        cmd.Parameters.Add("@i_nrofactura", SqlDbType.VarChar).Value = nrofac;
        cmd.Parameters.Add("@i_posfactura", SqlDbType.VarChar).Value = posfac;

        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.ExecuteNonQuery();

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }
        con.Close();
    }

    public void EstadosConcesionario(int identificador, string usuario, string idsesion, string nrofac, string posfac, string path, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_DATOS_CONCESIONARIO";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@i_idusuario", SqlDbType.VarChar).Value = usuario;
        cmd.Parameters.Add("@i_idsesion", SqlDbType.VarChar).Value = idsesion;
        cmd.Parameters.Add("@i_nrofactura", SqlDbType.VarChar).Value = nrofac;
        cmd.Parameters.Add("@i_posfactura", SqlDbType.VarChar).Value = posfac;
        cmd.Parameters.Add("@i_path", SqlDbType.VarChar).Value = path;

        cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        cmd.ExecuteNonQuery();

        if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }
        con.Close();
    }

    public DataSet cargasolicitudesControl(int identificador, out int coderror, out string msgerror)
    {
        coderror = 0;
        msgerror = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "DEVR_SOLICITUDES_PENDIENTES";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@i_accion", SqlDbType.Int).Value = identificador;
        cmd.Parameters.Add("@o_cod_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
        cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds2 = new DataSet();
        da.Fill(ds2);

        if (System.Convert.ToInt32(cmd.Parameters["@o_cod_error"].Value) != 0)
        {
            coderror = Convert.ToInt32(cmd.Parameters["@o_cod_error"].Value);
            msgerror = cmd.Parameters["@o_msg_error"].Value.ToString();
        }

        con.Close();
        return ds2;
    }
}