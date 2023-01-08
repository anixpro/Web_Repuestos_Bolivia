using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

/// <summary>
/// Summary description for Auto
/// </summary>
public class Auto
{
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

	public Auto()
	{
		con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}
    public String[] concesionario()
    {
        List<String> concesionarios = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombreConcesionario 
	                        FROM concesionario order by nombreConcesionario ASC ";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            concesionarios.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return concesionarios.ToArray<String>();
    }
	
	public String[] marcas()
    {
        List<String> marcas = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombreMarca
	                        FROM marca";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            marcas.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return marcas.ToArray<String>();
    }

    public List<Concesionario> concesionariosIds()
    {
        List<Concesionario> concesionarios = new List<Concesionario>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombreConcesionario, idConcesionario
	                        FROM concesionario
	                        WHERE nombreConcesionario!='cotizadores'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            concesionarios.Add(new Concesionario(dr[0].ToString(), dr[1].ToString()));
        }
        dr.Close();
        con.Close();

        return concesionarios;
    }

    public List<Sucursal> direccionSucursalPorConcesionario(String concesionario)
    {
        List<Sucursal> sucursales = new List<Sucursal>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT direccionSucursal,shipCode 
                            FROM sucursal 
                            WHERE nombreConcesionario ='"+concesionario+"'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            sucursales.Add(new Sucursal(dr[1].ToString(),dr[0].ToString()));
        }
        dr.Close();
        con.Close();

        return sucursales;
    }

    public String[] marcaRut(int rut)
    {
        List<String> marca = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_marcaRut";
        cmd.Parameters.Add("@rut", SqlDbType.Int).Value = rut;
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            marca.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return marca.ToArray<String>();
    }

    public String concesionarioRut(int rut)
    {
        String concesionario = null;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"select concesionario.nombreConcesionario
	                        from sucursal,persona,concesionario
	                        where persona.rut=@rut
	                        and persona.shipCode=sucursal.shipCode
	                        and sucursal.nombreConcesionario=concesionario.nombreConcesionario";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        if(dr.HasRows)
        {
            concesionario=dr[0].ToString();
        }
        dr.Close();
        con.Close();

        return concesionario;
    }

    public int insertaConcesionario(String[] marcas, String zona, String concesionario, String imagen, String rutHolding, String numeroFactura, String rutSupervisor, String rutAdministrador, String correosVFC)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        int modifica = 0;
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura, rutSupervisor, rutAdministrador,correosVFC)VALUES('" + zona + "','" + concesionario + "','consecionario','" + imagen + "','" + rutHolding + "','" + numeroFactura + "','" + rutSupervisor + "','" + rutAdministrador + "','" + correosVFC + "')";    
        modifica += cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();

        con.Open();
        String insert = "";
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        foreach (String marca in marcas)
        {
            insert += "INSERT INTO concesionarioMarca(nombreMarca,nombreConcesionario)VALUES('" + marca + "','" + concesionario + "')\n";
        }
        cmd.CommandText = insert;
        modifica += cmd.ExecuteNonQuery();
        con.Close();
        return modifica;
    }

    public int modificaConcesionario(String concesionario, String imagen)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_modificaConcesionario";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = concesionario;
        cmd.Parameters.Add("@imagen", SqlDbType.NVarChar).Value = imagen;
        int modifica = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return modifica;
    }

    public int isertaSucursal(String nombreConcesionario, String comunaSucursal,String nombreSucursal,String shipCode,String direccionSucursal)
    {
        String factura = "";
        String rutHolding = "";
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @"SELECT rutHolding, numeroFactura
                            FROM concesionario
                            WHERE nombreConcesionario='"+nombreConcesionario+"'";
        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            rutHolding = dr1[0].ToString(); factura = dr1[1].ToString();
        }
        dr1.Close();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_insertaSucursal";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@nombreConcesionario", SqlDbType.NVarChar).Value = nombreConcesionario;
        cmd.Parameters.Add("@nombreComuna", SqlDbType.NVarChar).Value = comunaSucursal;
        cmd.Parameters.Add("@numeroFactura", SqlDbType.NVarChar).Value = factura;
        cmd.Parameters.Add("@shipCode", SqlDbType.NVarChar).Value = shipCode;
        cmd.Parameters.Add("@direccionSucursal", SqlDbType.NVarChar).Value = direccionSucursal;
        int rows = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return rows;
    }

    public String shipCode(String direccion,String concesionario)
    {
        String shipCode="";

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "SELECT shipcode FROM sucursal where direccionSucursal ='" + direccion + "' and nombreConcesionario='"+concesionario+"'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        if(dr.HasRows)
        {
            shipCode = dr[0].ToString();
        }
        //dr.Close();
        con.Close();
        return shipCode;
        
    }

    public String[] obtenerZona()
    {
        List<String> sector = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "SELECT sector FROM zona ORDER BY idZona";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            sector.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return sector.ToArray<String>();
    }

    public int isertaZona(String sector)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_insertaZona";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@sector", SqlDbType.NVarChar).Value = sector;
        int rows = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return rows;
    }

    public String[] todasLasMarcas()
    {
        List<String> marca = new List<String>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "SELECT nombreMarca FROM MARCA";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            marca.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return marca.ToArray<String>();
    }

    public int insertaConcesionarioMarcas(String []marcas,String concesionario)
    {
        int indice = maxConcesionarioMarca();
		con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        String insert= "";
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        foreach (String marca in marcas)
        {
           // insert += "INSERT INTO concesionarioMarca(idConcesionarioMarca, nombreMarca,nombreConcesionario)VALUES(" + indice + ",'" + marca + "','"+concesionario+"')\n";
			insert += "INSERT INTO concesionarioMarca(nombreMarca,nombreConcesionario)VALUES('" + marca + "','" + concesionario + "')\n";
		}
        cmd.CommandText = insert;
        cmd.CommandTimeout = 10;
        int modifica = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return modifica;
    }

    public String[] todasLasMarcasQueNoTengo(String marcaABuscar)
    {
       
        List<String> marcaQueNoTengo = new List<String>();
        List<String> marcaQueSiTengo = new List<String>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;

        cmd.CommandText = @"select marca.nombreMarca
                            FROM concesionario,marca,concesionarioMarca
                            WHERE concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
                            AND concesionarioMarca.nombreMarca =marca.nombreMarca
                            AND concesionario.nombreConcesionario ='"+marcaABuscar+"'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            marcaQueSiTengo.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        //SI ES SOLO UNA MARCA NO DEBO USAR LA SENTENCIA SQL AND EN CAMBIO, LA USO
        switch(marcaQueSiTengo.Count)
        {
            case 0:
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT nombreMarca FROM MARCA";
                cmd.CommandTimeout = 10;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    marcaQueNoTengo.Add(dr[0].ToString());
                }
                dr.Close();
                con.Close();
                break;

            case 1:
            
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT nombreMarca FROM MARCA WHERE nombreMarca!='" + marcaQueSiTengo[0] + "'";
                cmd.CommandTimeout = 10;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    marcaQueNoTengo.Add(dr[0].ToString());
                }
                dr.Close();
                con.Close();

            
                    break;
            default:
                String query = "SELECT nombreMarca FROM MARCA WHERE nombreMarca!='" + marcaQueSiTengo[0] + "'";
                con.Open();
                cmd.Connection = con;
                foreach (String marca in marcaQueSiTengo)
                {
                    query += " AND nombreMarca!='" + marca + "'\n";
                }
                cmd.CommandText = query;
                cmd.CommandTimeout = 10;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    marcaQueNoTengo.Add(dr[0].ToString());
                }
                dr.Close();
                con.Close();
                break;
        }//fin swtich

        return marcaQueNoTengo.ToArray<String>();
    }

    public String[] marcaConcesionario(String concesionario)
    {
        List<String> marca = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        /*ME TIRA UN ERROR DE TOO_MANY_ARGUMENTS EL CUAL NO CORRESPONDE POR LO TANTO LO HAGO SIN SP, ES EL MISMO CODIGO 
        DE INSERTAR NUEVA MARCA Y NO PERDERÉ TIEMPO BUSCANDO UN ERROR RREBUSCADO
        */
        //cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //cmd.CommandText = "sp_marcaConcesionario";
        //cmd.Parameters.Add("@concesionario", SqlDbType.NVarChar).Value = concesionario;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText=@"SELECT marca.nombreMarca
                        FROM concesionario,concesionarioMarca,marca
                        WHERE concesionario.nombreConcesionario='"+concesionario+"' AND concesionario.nombreConcesionario = concesionarioMarca.nombreConcesionario AND concesionarioMarca.nombreMarca = marca.nombreMarca";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            marca.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return marca.ToArray<String>();
    }

    public int eliminaConcesionarioMarcas(String[] marcas, String concesionario)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        String delete = "";
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        foreach (String marca in marcas)
        {
            delete += "DELETE FROM concesionarioMarca WHERE nombreConcesionario='"+concesionario+"' and nombreMarca='"+marca+"'\n";
        }
        cmd.CommandText = delete;
        cmd.CommandTimeout = 10;
        int modifica = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return modifica;
    }

    public String[] regiones()
    {
        List<String> region = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT nombreRegion FROM region";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            region.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return region.ToArray<String>();
    }

    public String[] provincias(String region)
    {
        List<String> provincia = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT provincia.nombreProvincia 
                            FROM provincia,region
                            WHERE region.nombreRegion='"+region+"' AND region.nombreRegion= provincia.nombreRegion";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            provincia.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return provincia.ToArray<String>();
    }

    public String[] comunas(String region)
    {
        List<String> comuna = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT comuna.nombreComuna 
                            FROM provincia,comuna
                            WHERE provincia.nombreProvincia='"+region+"' AND provincia.nombreProvincia= comuna.nombreProvincia ";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            comuna.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return comuna.ToArray<String>();
    }

    public String abreviado(String nombreMarca)
    {
        String marca = "";
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT abreviado FROM marca WHERE nombreMarca='" + nombreMarca + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        marca=dr[0].ToString();
        dr.Close();
        con.Close();
        return marca;
    }

    public String[] todosLosAbreviado()
    {
        List<String> abreviados = new List<String>(); 
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"SELECT abreviado FROM marca";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            abreviados.Add(dr[0].ToString());           
        }
        dr.Close();
        con.Close();
        return abreviados.ToArray<String>();
    }

    public String detalleMarcas()
    {
        return @"SELECT nombreMarca as marca,abreviado,descripcion,orgVentas FROM marca";
    }

    public String detalleMarcas(String concesionario)
    {
        return @"SELECT marca.nombreMarca as marca,marca.abreviado,marca.descripcion,marca.orgVentas 
                FROM marca,concesionario,concesionarioMarca
                WHERE concesionario.nombreConcesionario='"+concesionario+"' "+
                "AND concesionario.nombreConcesionario = concesionarioMarca.nombreConcesionario "+
                "AND concesionarioMarca.nombreMarca= marca.nombreMarca";
    }

    public String detalleMarcaPorMarca(String marca)
    {
        return @"SELECT nombreMarca as marca,abreviado,descripcion,orgVentas 
                FROM marca
                WHERE nombreMarca='" + marca + "'";
    }

    public int eliminaMarca(String marca)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_eliminaMarca";
        cmd.Parameters.Add("@nombreMarca", SqlDbType.NVarChar).Value = marca;
        cmd.CommandTimeout = 10;
        int modifica = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return modifica;
    }

    public int insertaMarca(
        String marca,
        String abreviado,
        String descripcion,
        String orgVenta,
        String grupoMaterial,
        String prefijoMarca,
        String codigoCompania,
        Boolean esForaneo
        )
    {
        int intEsForaneo = esForaneo ? 1 : 0;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "sp_insertaMarca";
        cmd.Parameters.Add("@nombreMarca",SqlDbType.NVarChar).Value = marca;
        cmd.Parameters.Add("@abreviado", SqlDbType.NVarChar).Value = abreviado;
        cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;
        cmd.Parameters.Add("@orgVentas", SqlDbType.NVarChar).Value = orgVenta;
        cmd.Parameters.Add("@esForaneo", SqlDbType.Int).Value = intEsForaneo;
        cmd.CommandTimeout = 10;
        int modifica = cmd.ExecuteNonQuery();
        con.Close();
        
        // Se ingresa informacion grupo material
        BD.InsertaOActualiza(@"insert into grupo_materiales (GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)
            values ('"+grupoMaterial+"','"+orgVenta+"','"+marca+"','"+codigoCompania+"','"+prefijoMarca+"')");
        return modifica;
    }

    //MODELO
    public String detalleModelo()
    {
        return @"SELECT nombreMarca as marca,nombreModelo as modelo,descripcion FROM Modelo";
    }

    public String detalleModeloPorMarca(String marca)
    {
        return @"SELECT modelo.nombreMarca AS marca,modelo.nombreModelo AS modelo,modelo.descripcion 
                FROM modelo,marca
                WHERE marca.nombreMarca='"+marca+"'"
                +"AND marca.nombreMarca=modelo.nombreMarca";
    }

    public String detalleModeloPorModelo(String modelo)
    {
        return @"SELECT nombreMarca as marca,nombreModelo as modelo,descripcion 
                FROM Modelo 
                WHERE modelo.nombreModelo='"+modelo+"'";
    }

    public int eliminaModelo(String modelo)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_eliminaModelo";
        cmd.Parameters.Add("@nombreModelo", SqlDbType.NVarChar).Value = modelo;
        cmd.CommandTimeout = 10;
        int modifica = cmd.ExecuteNonQuery();
        //dr = cmd.ExecuteReader();
        //dr.Close();
        con.Close();
        return modifica;
    }

    public int insertaModelo(String marca, String nombreModelo, String descripcion)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "sp_insertaModelo";
        cmd.Parameters.Add("@nombreMarca", SqlDbType.NVarChar).Value = marca;
        cmd.Parameters.Add("@nombreModelo", SqlDbType.NVarChar).Value = nombreModelo;
        cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = descripcion;
        cmd.CommandTimeout = 10;
        int modifica = cmd.ExecuteNonQuery();
        con.Close();
        return modifica;
    }

    public String[] marcaContenido(String idContenido)
    {
        List<String> marca = new List<string>();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = @"
            select * from contenido_marca where idContenido = " + idContenido + @"
        ";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            marca.Add(dr[0].ToString());
        }
        dr.Close();
        con.Close();

        return marca.ToArray<String>();
    }
	
	public int 	maxConcesionario()
	{
		string indice = "";
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select top 1 isnull(idConcesionario,1) from concesionario order by idConcesionario desc";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        indice = dr[0].ToString();
        dr.Close();
        con.Close();
		 int retorno = int.Parse(indice)+1;
		
		
        return retorno;
	}
	
	public int 	maxConcesionarioMarca()
	{
		string indice = "";
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select top 1 isnull(idConcesionarioMarca,1) from concesionarioMarca order by idConcesionarioMarca desc";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        indice = dr[0].ToString();
        dr.Close();
        con.Close();
		 int retorno = int.Parse(indice)+1;
		
		
        return retorno;
	}

    // REQ - Cotizaciones Automaticas Marzo 2022
    public int habilitaCotizacionAutomatica(String abreviado, String marca)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        int modifica = 0;
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_habilitaCotizacionAutomatica";
        cmd.Parameters.Add("@abreviado", SqlDbType.NVarChar).Value = abreviado;
        cmd.Parameters.Add("@marca", SqlDbType.NVarChar).Value = marca;
        cmd.CommandTimeout = 10;

        try
        {
            modifica = cmd.ExecuteNonQuery();
        }
        catch
        {
            con.Close();
        }
        finally
        {
            con.Close();
        }

        return modifica;
    }




}