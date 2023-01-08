using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

/// <summary>
/// Descripción breve de Contenido
/// </summary>
public class Contenido
{
    
    SqlConnection con;
    SqlCommand cmd, cmd1;
    SqlDataReader dr, dr1;
  
    string _fecha;
    string _html;
    string _titulo;
    int _tipo;

	public Contenido()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
	}

    public String mensajeError { get; set; }

    public string Fecha
    {
        get { return this._fecha; }
        set { _fecha = value; }
    }

    public string Html
    {
        get { return this._html; }
        set { _html = value; }
    }

    public string Titulo
    {
        get { return this._titulo; }
        set { _titulo = value; }
    }

    public int Tipo
    {
        get { return this._tipo; }
        set { _tipo = value; }
    }

    // retorna true en caso que se leyó la notica
    public bool leyoNoticia (String rut)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        bool retorno = false;

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"
            select COUNT(*) as cantidad 
            from contenido_persona where rut= '"+ rut + @"' and idContenido = (select top 1 idContenido  from contenido
            where importante = 1
            order by idContenido desc)
        ";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            // Quiere decir que NO ha leido la noticia
            if (dr[0].ToString() == "0")
            {
                retorno = true;
            }
        }
        dr.Close();
        con.Close();

        return retorno;
    }

    // Retorna html ultima noticia importante
    public string obtieneUltimaNoticia()
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        string retorno = "";

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"
            select top 1 html  from contenido
	        where importante = 1
	        order by idContenido desc
        ";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            retorno = dr[0].ToString();
        }
        dr.Close();
        con.Close();

        return retorno;
    }

    // Retorna html ultima noticia importante
    public int leeUltimaNoticia(String rut)
    {
        int x = 0;
        int idNoticiaImportante = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

        // Primero, se rescata el valor de la ultima noticia importante leida
        using (SqlConnection con2 = new SqlConnection(con.ConnectionString))
        {
            SqlCommand cmd = new SqlCommand(@"
                select top 1 idContenido from contenido
                where importante = 1
                order by idContenido desc
            ", con2);
            try
            {
                con2.Open();
                idNoticiaImportante = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                mensajeError = "Error: " + ex.Message + ". Seguimiento: " + ex.StackTrace;
            }
            finally
            {
                con2.Close();
            }
         }

        try
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();

            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = @"
                insert into contenido_persona (rut,idContenido) values(
	        " + rut + @"," + idNoticiaImportante + ")";
            cmd.CommandTimeout = 10;
            x = cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            mensajeError = "Error: " + ex.Message + ". Seguimiento: " + ex.StackTrace;
            x = -1;
        }
        finally
        {
            con.Close();
        }
        return x;
    }

    // Retorna html ultima noticia importante
    public int eliminaContenido(String id)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"
            delete from contenido where idContenido = '" + id + @"'
        ";
        cmd.CommandTimeout = 10;
        int x = cmd.ExecuteNonQuery();
        con.Close();

        return x;
    }
}
