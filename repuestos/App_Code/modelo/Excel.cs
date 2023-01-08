using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using log4net;
using log4net.Config;

public class Excel
{
    private String _rutaLeer;
    private String _hojaCalculo;
    private String _rutaDestino;

    SqlConnection con;
    SqlCommand cmd;
    SqlDataReader dr;

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Excel));

    public Excel()
    {
        _rutaLeer = "";
        _hojaCalculo = "";
        _rutaDestino = "";

        con = new SqlConnection();
        cmd = new SqlCommand();
    }


    //ACCESADORES
    public String getRutaLeer()
    {
        return _rutaLeer;
    }
    public String getHojaCalculo()
    {
        return _hojaCalculo;
    }
    public String getRutaDestino()
    {
        return _rutaDestino;
    }
    //MUTADORES
    public void setRutaLeer(String rutaLeer)
    {
        _rutaLeer = rutaLeer;
    }

    public void setHojaCalculo(String hojaCalculo)
    {
        _hojaCalculo = hojaCalculo;
    }

    public void setRutaDestino(String rutaDestino)
    {
        _rutaDestino = rutaDestino;
    }


    public ArrayList sacaDatos()
    {
        ArrayList insertar = new ArrayList();
        ArrayList lineas = new ArrayList();
        StreamReader sr = null;
        try
        {
            sr = new StreamReader(_rutaLeer);
            String linea = sr.ReadLine();
            do
            {
                lineas.Add(linea);
                linea = sr.ReadLine();

            } while (linea != null);
        }
        catch (Exception ex)
        {
            logger.Error("En [sacaDatos] Msj: " + ex.Message + ". Inn: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        finally
        {
            sr.Close();
        }

        return lineas;
    }



    private String insertaIndicadorMetas(int idConcesionarioMarca, int ano, int mes, String compras, String metas)
    {
        return "insert into indicador(idConcesionarioMarca,ano,mes,compras,metas)values('" + idConcesionarioMarca + "'," + ano + ",'" + mes + "',replace('" + compras + "','.',''),replace('" + metas + "','.',''))";
    }

    private String modificaIndicador(int idConcesionarioMarca, int ano, int mes, String compras, String metas)
    {
        return "UPDATE indicador SET ano=" + ano + ",mes=" + mes + ",compras=replace('" + compras + "','.',''),metas=replace('" + metas + "','.','') WHERE idConcesionarioMarca=" + idConcesionarioMarca + " AND mes =" + mes + " AND ano = " + ano;
    }

    private String buscaId(String concesionario, String marca)
    {
        return @"select idConcesionarioMarca 
                   from concesionarioMarca
                   where nombreConcesionario=LTRIM('" + concesionario + "') and nombreMarca=LTRIM('" + marca + "')";
    }

    public void insertaIndicador(String archivo, String marcaAuto, int ano)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

        setRutaLeer(archivo);
        foreach (String datos in sacaDatos())
        {
            String[] dato = datos.Split(';');
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = buscaId(dato[0], marcaAuto.Split('.')[0]);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                int id = int.Parse(dr[0].ToString());
                dr.Close();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 1, dato[5], dato[1]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 2, dato[8], dato[2]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 3, dato[11], dato[3]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 4, dato[21], dato[17]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 5, dato[24], dato[18]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 6, dato[27], dato[19]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 7, dato[37], dato[33]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 8, dato[40], dato[34]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 9, dato[43], dato[35]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 10, dato[53], dato[49]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 11, dato[56], dato[50]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = insertaIndicadorMetas(id, ano, 12, dato[59], dato[51]);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }//FIN METODO


    public void modificaIndicador(String archivo, String marcaAuto, int ano)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

        setRutaLeer(archivo);
        foreach (String datos in sacaDatos())
        {
            String[] dato = datos.Split(';');
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = buscaId(dato[0], marcaAuto.Split('.')[0]);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                int id = int.Parse(dr[0].ToString());
                dr.Close();

                cmd.CommandText = modificaIndicador(id, ano, 1, dato[5], dato[1]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 2, dato[8], dato[2]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 3, dato[11], dato[3]);
                cmd.ExecuteNonQuery();


                cmd.CommandText = modificaIndicador(id, ano, 4, dato[21], dato[17]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 5, dato[24], dato[18]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 6, dato[27], dato[19]);
                cmd.ExecuteNonQuery();


                cmd.CommandText = modificaIndicador(id, ano, 7, dato[37], dato[33]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 8, dato[40], dato[34]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 9, dato[43], dato[35]);
                cmd.ExecuteNonQuery();


                cmd.CommandText = modificaIndicador(id, ano, 10, dato[53], dato[49]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 11, dato[56], dato[50]);
                cmd.ExecuteNonQuery();

                cmd.CommandText = modificaIndicador(id, ano, 12, dato[59], dato[51]);
                cmd.ExecuteNonQuery();


            }

            con.Close();
        }
    }//FIN METODO
    public int insertaConcesionario(String zona, String concesionario, String imagen)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_insertaIndicador";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@zona", SqlDbType.NVarChar).Value = zona;
        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = concesionario;
        cmd.Parameters.Add("@imagen", SqlDbType.NVarChar).Value = imagen;

        int modifica = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return modifica;
    }

    public int primerTrimestre(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.compras)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "')AND ano=2011 AND indicador.mes<4 ";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int segundoTrimestre(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.compras)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes between 4 and 6";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int tercerTrimestre(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.compras)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes between 7 and 9";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int cuartoTrimestre(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.compras)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes between 10 and 12";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int primerTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.metas)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes<4";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int segundoTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.metas)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes between 4 and 6";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int tercerTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.metas)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes between 7 and 9";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int cuartoTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        int trimestre = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT SUM(indicador.metas)
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND indicador.mes between 10 and 12";
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            try
            {
                trimestre = int.Parse(dr[0].ToString());
            }
            catch (FormatException ex)
            {

            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return trimestre;

    }

    public int[] metaMeses(String nombreMarca, String nombreConcesionario, int ano)
    {
        List<int> meses = null;
        meses = new List<int>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT indicador.metas
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "')";
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            if (dr[0] != null)
                meses.Add(int.Parse(dr[0].ToString()));
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return meses.ToArray();

    }

    public int[] compraMeses(String nombreMarca, String nombreConcesionario, int ano)
    {
        List<int> meses = null;
        meses = new List<int>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT indicador.compras
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "')";
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            if (dr[0] != null)
                meses.Add(int.Parse(dr[0].ToString()));
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return meses.ToArray();

    }


    public int compraDeUnMes(String nombreMarca, String nombreConcesionario, int ano, int mes)
    {
        int compraMes = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT indicador.compras
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND mes='"+mes+"'";
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            if (dr[0] != null)
                compraMes=(int.Parse(dr[0].ToString()));
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return compraMes;

    }


    public int metaDeUnMes(String nombreMarca, String nombreConcesionario, int ano, int mes)
    {
        int metaMes = 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT indicador.metas
	                        FROM  concesionarioMarca,indicador 
	                        WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	                        AND concesionarioMarca.nombreMarca=LTRIM('" + nombreMarca + "') AND ano=" + ano + " AND concesionarioMarca.nombreConcesionario=LTRIM('" + nombreConcesionario + "') AND mes='"+mes+"'";
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            if (dr[0] != null)
                metaMes=(int.Parse(dr[0].ToString()));
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
        return metaMes;

    }
}
