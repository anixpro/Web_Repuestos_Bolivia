using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for Persona
/// </summary>
public class Persona
{
    SqlConnection con;
    SqlCommand cmd,cmd1;
    SqlDataReader dr,dr1;

    private int _permisos;
    private String _nombre;
    private string _rut;
    private int _IdPersona;

    private MailMessage _correo = new MailMessage();
    private SmtpClient _smtp = new SmtpClient();
    SendMail_helper _mail = new SendMail_helper();
	private System.Net.NetworkCredential credenciales = new System.Net.NetworkCredential("web.skberge", "**w32i78");

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Persona));

    public String MsjeError { get; set; }

    // Implementación de propiedades
    public String IdPersona { get; set; }
    public String DestinatarioMercancia { get; set; }
    public String RUT { get; set; }
    public String NombreReal { get; set; }
    public String Usuario { get; set; }
    public String Contraseña { get; set; }
    public String Correo { get; set; }
    public String Telefono { get; set; }
    public Boolean EsMultiSucursal { get; set; }
    public Boolean EsHabilitado { get; set; }
    public String IdHumano2 { get; set; }
    public PermisosPersona[] Permisos = new PermisosPersona[2];
    public String NombreConcesionario { get; set; }
    public String Idllave { get; set; }
    public Boolean EsAprobVFC { get; set; }
    public Boolean Criticidad { get; set; }
    public Boolean devrec { get; set; }
    public Boolean reemplazo { get; set; }
    public int IdSucursal { get; set; }
	public Persona()
	{
        _permisos = 0;
        _rut = "0";
        _nombre = "";
		con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
	}

    public int getPermisos()
    {
        return _permisos;
    }
    public string getRut()
    {
        return _rut;
    }
    public String getNombre()
    {
        return _nombre;
    }
    public int getIdPersona()
    {
        return _IdPersona;
    }

    public void setPermisos(int permisos)
    {
        _permisos = permisos;
    }
    public void setRut(string rut)
    {
        _rut = rut;
    }
    public void setNombre(String nombre)
    {
        _nombre = nombre;
    }



    public int login(String usuario, String contrasena)
    {
        try
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
        }
        catch (SqlException ex)
        {
            MsjeError = "ERROR, conexión no establecida con la base de datos";
            logger.Error("Error al loggearse. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            return 0;
        }

        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @"SELECT personaPermisos.cargo
                            FROM persona, personaPermisos
                            WHERE persona.usuario ='"+usuario+"' " 
                            +"AND (SELECT CAST(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) AS VARCHAR(50)) "
		                    +"FROM persona  WHERE usuario='"+usuario+"')='"+contrasena+"' AND persona.rut = personaPermisos.rut AND habilitado=1;";
        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();

        int cantidadDePermisos = 0;
        while (dr1.Read())
        {
            cantidadDePermisos++;
        }
        dr1.Close();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"DECLARE @largo  int;
                            SET @largo =LEN('"+usuario+"')-3 "
                            +"SELECT persona.nombre,personaPermisos.cargo,"
                            + "CAST(DECRYPTBYPASSPHRASE('ENCRIPTADO',persona.contrasena) AS VARCHAR(50)) AS contrasena, SUBSTRING(nombre,0,5)+SUBSTRING(CAST(persona.rut as VARCHAR(12)),@largo,4),persona.rut,persona.idPersona "
                            + "FROM persona, personaPermisos "
                            +"WHERE persona.usuario ='"+usuario+"' "
                            +"AND (select CAST(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) AS VARCHAR(50)) "
			                        +"from persona "
			                        +"where usuario='"+usuario+"' "
		                        +")='"+contrasena+"' "
                            +"AND persona.rut = personaPermisos.rut "
                            +"AND habilitado=1"; 
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@usuario", SqlDbType.NVarChar).Value = usuario;
        cmd.Parameters.Add("@contrasena", SqlDbType.NVarChar).Value = contrasena;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            if (dr[2].ToString().Equals(dr[3].ToString()))
            {
                _rut = (dr[4].ToString());
                dr.Close();
                con.Close();
                return 1;
            }
            else
            {
                _nombre = dr[0].ToString();
                _permisos = int.Parse(dr[1].ToString());
                _rut = (dr[4].ToString());
                _IdPersona = int.Parse(dr[5].ToString());
                dr.Close();
                try
                {
                    con.Close();
                }
                catch (Exception exc) { }
                
                if (cantidadDePermisos == 1)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }//FIN HASROWS
        else
        {
            dr.Close();
            con.Close();
            return 4;
        }
    }
  
    public void insertaPersona(String empresa,String codigoSucursal ,int rut ,String nombre ,String email ,String telefono ,int cargo,Boolean multiSucursal)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_insertaPersona";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@empresa", SqlDbType.NVarChar).Value = empresa;
        cmd.Parameters.Add("@shipCode", SqlDbType.NVarChar).Value = codigoSucursal;
        cmd.Parameters.Add("@rut", SqlDbType.Int).Value = rut;
        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = nombre;
        cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
        cmd.Parameters.Add("@telefono", SqlDbType.NVarChar).Value = telefono;
        cmd.Parameters.Add("@cargo", SqlDbType.Int).Value = cargo;
        cmd.Parameters.Add("@multipleSucursal", SqlDbType.Bit).Value = multiSucursal;
        cmd.ExecuteNonQuery();
        con.Close();
    }

    public int modificaPersona(String empresa,String codigoSucursal ,int rut ,String nombre ,String email ,String telefono ,int cargo,Boolean multiSucursal)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_modificaPersona";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@empresa", SqlDbType.NVarChar).Value = empresa;
        cmd.Parameters.Add("@shipCode", SqlDbType.NVarChar).Value = codigoSucursal;
        cmd.Parameters.Add("@rut", SqlDbType.Int).Value = rut;
        cmd.Parameters.Add("@nombre", SqlDbType.NVarChar).Value = nombre;
        cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
        cmd.Parameters.Add("@telefono", SqlDbType.NVarChar).Value = telefono;
        cmd.Parameters.Add("@cargo", SqlDbType.Int).Value = cargo;
        cmd.Parameters.Add("@multipleSucursal", SqlDbType.Bit).Value = multiSucursal;
        int query=cmd.ExecuteNonQuery();
        con.Close();
        return query;
    }

    public int modificaContrasena(string rut, String nContrasena)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_modificaContrasena";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@rut", SqlDbType.NVarChar).Value = rut;
        cmd.Parameters.Add("@nuevaContrasena", SqlDbType.NVarChar).Value = nContrasena;
        int query= cmd.ExecuteNonQuery();
        con.Close();
        return query;
    }

    public int resetaContrasena(string rut)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_reseteaContrasena";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@rut", SqlDbType.NVarChar).Value = rut;
        int query=cmd.ExecuteNonQuery();
        con.Close();
        return query;
    }

    public Boolean comparaContrasena(String usuario,String contrasena)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_comparaContrasena";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@user", SqlDbType.NVarChar).Value = usuario;
        cmd.Parameters.Add("@pass", SqlDbType.NVarChar).Value = contrasena;
        dr = cmd.ExecuteReader();
        dr.Read();

            if (dr[0].ToString().Equals(dr[1].ToString()))
            {
                dr.Close();
                con.Close();
                return false;
            }
            else
            {
                dr.Close();
                con.Close();
                return true;
            }
        
    }

    public String saberMiMail(String usuario, String contrasena)
    {
        String mail = null;

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText =  @"SELECT email FROM persona
		                    WHERE usuario='" + usuario + "' and CAST(DECRYPTBYPASSPHRASE('ENCRIPTADO',persona.contrasena) AS VARCHAR(50)) = '" + contrasena + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            mail = dr[0].ToString();
            dr.Close();
            con.Close();
            return mail;
        }
        else
        {
            dr.Close();
            con.Close();

            return "";
        }
    }

    public String saberMiNombre(String usuario,String contrasena)
    {
        String nombre=null;

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombre FROM persona
		                    WHERE usuario='" + usuario + "' and contrasena = '" + contrasena + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            nombre = dr[0].ToString();
            dr.Close();
            con.Close();
            return nombre;
        }
        else
        {
            dr.Close();
            con.Close();

            return "";
        }
    }

    public String saberMiConcesionario(string rut)
    {
        String nombre = null;

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT concesionario.nombreConcesionario
                            FROM concesionario, persona,sucursal
                            WHERE persona.shipCode=sucursal.shipCode 
                            AND sucursal.nombreConcesionario = concesionario.nombreConcesionario
                            AND persona.rut= '" + rut + "'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        dr.Read();
        if (dr.HasRows)
        {
            nombre = dr[0].ToString();
            dr.Close();
            con.Close();
            return nombre;
        }
        else
        {
            dr.Close();
            con.Close();

            return "";
        }
    }

    public String datos()
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador' WHEN 5 THEN 'Supervisor' END as cargo
                FROM persona,personaPermisos
                WHERE persona.rut= personaPermisos.rut AND persona.habilitado=1";     
    }

    public String datosDoble()
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono
                FROM persona,personaPermisos
                WHERE persona.rut= personaPermisos.rut AND persona.habilitado=1
                AND personaPermisos.cargo!=1
                AND personaPermisos.cargo!=4
                GROUP BY persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono
				HAVING COUNT(personaPermisos.cargo) =1";
    }

    public String datosEliminados()
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador'END as cargo
                FROM persona,personaPermisos
                WHERE persona.rut= personaPermisos.rut AND persona.habilitado=0";
    }

    public String datosPorRut(string rut)
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador'END as cargo
                FROM persona,personaPermisos
                WHERE persona.rut='" + rut + "' AND persona.rut= personaPermisos.rut AND persona.habilitado=1";     
    }

    public String datosEliminadosPorRut(string rut)
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador'END as cargo
                FROM persona,personaPermisos
                WHERE persona.rut='" + rut + "' AND persona.rut= personaPermisos.rut AND persona.habilitado=0";     
    }

    public int eliminaPersona(string rut)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_eliminaPersona";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@rut", SqlDbType.NVarChar).Value = rut;
        int sql = cmd.ExecuteNonQuery();
        con.Close();
        return sql;
    }

    public int rehacerPersona(string rut)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_rehacerPersona";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@rut", SqlDbType.NVarChar).Value = rut;
        int sql = cmd.ExecuteNonQuery();
        con.Close();
        return sql;
    }

    public String datosPorConcesionario(String concesionario)
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador'END as cargo
                FROM persona,personaPermisos,sucursal,concesionario
                WHERE concesionario.nombreConcesionario='"+concesionario+"' AND concesionario.nombreConcesionario=sucursal.nombreConcesionario AND sucursal.shipCode=persona.shipCode AND persona.habilitado=1 AND persona.rut = personaPermisos.rut;";
    }

    public String datosPorConcesionarioDoble(String concesionario)
    {
        return @"	SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono
				FROM persona,personaPermisos,sucursal,concesionario
                WHERE concesionario.nombreConcesionario='" + concesionario + "' AND concesionario.nombreConcesionario=sucursal.nombreConcesionario AND sucursal.shipCode=persona.shipCode AND persona.habilitado=1 AND persona.rut = personaPermisos.rut GROUP BY persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono HAVING  COUNT(personaPermisos.cargo) =1";
    }

    public int insertaDoblePermiso(string rut)
    {
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "sp_doblePermiso";
        cmd.CommandTimeout = 10;
        cmd.Parameters.Add("@rut", SqlDbType.NVarChar).Value = rut;
        int sql = cmd.ExecuteNonQuery();
        con.Close();
        return sql;
    }

    public String enviaMail(String nombre, String usuario, String contrasena, String mail, String mensaje)
    {
        _correo.From = new MailAddress("repuestos@skberge.cl");
        _correo.To.Add(mail);
        _correo.Subject = "Usuario y contrasena para Skberge";
        _correo.Body = mensaje + "Usuario: " + usuario + "\nContrasena: " + contrasena + "\nURl Sistema de repuestos: http://repuestosnk.skberge.com.pe/"; 
        _correo.IsBodyHtml = false;
        _correo.Priority = MailPriority.Normal;


        //Asingnar datos del servidor de correo
        _smtp.Host = "postfix.skberge.com";
        _smtp.Credentials = credenciales;
        _smtp.Port = 25;
        _smtp.EnableSsl = false;
        // #Mejorar este codigo
        try
        {
            _smtp.Send(_correo);
            return "Correo enviado con exito";

        }
        catch
        {
            return "Error al enviar  correo";
        }
    }

    public String[] buscaPersonaPorRut(string rut)
    {
        String[] row = null;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombre,rut,CAST(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) AS VARCHAR(50)),email 
                            FROM persona
                            WHERE rut='"+rut+"'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            row = new String[] {dr[0].ToString(),dr[1].ToString(),dr[2].ToString(),dr[3].ToString() };
        }
        dr.Close();
        con.Close();

        return row;
    }

    public String usuariosPorZona(String zona)
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador'END as cargo
                FROM persona,personaPermisos,sucursal,concesionario,zona
                WHERE zona.sector='"+zona+"' "
                +"AND concesionario.nombreConcesionario=sucursal.nombreConcesionario  "
                +"AND sucursal.shipCode=persona.shipCode "
                +"AND persona.habilitado=1 "
                +"AND persona.rut = personaPermisos.rut "
                +"AND zona.sector=concesionario.sector;";     
    }

    public String usuariosPorZona()
    {
        return @"SELECT persona.shipCode,persona.rut,persona.nombre,persona.email,persona.telefono,CASE personaPermisos.cargo WHEN 1 THEN 'Administrador' WHEN 2 THEN 'Gerente' WHEN 3 THEN 'Operario' WHEN 4 THEN 'Cotizador'END as cargo
                FROM persona,personaPermisos,sucursal,concesionario,zona "
                + "WHERE concesionario.nombreConcesionario=sucursal.nombreConcesionario " 
                + "AND sucursal.shipCode=persona.shipCode "
                + "AND persona.habilitado=1 "
                + "AND persona.rut = personaPermisos.rut "
                + "AND zona.sector=concesionario.sector;";
    }

    public String[] obtenerDatosPorRut(string rut)
    {
        String[] row = null;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT sucursal.direccionSucursal,persona.nombre,persona.email,persona.telefono,persona.multipleSucursal,personaPermisos.cargo,concesionario.nombreConcesionario
                            FROM concesionario
                            INNER JOIN (sucursal INNER JOIN(persona INNER JOIN personaPermisos ON persona.Rut=personaPermisos.rut)
                            ON sucursal.shipcode=persona.shipcode)
                            ON concesionario.nombreConcesionario=sucursal.nombreConcesionario
                            AND persona.rut='"+rut+"'";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            row = new String[] { dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString() };
        }
        dr.Close();
        con.Close();

        return row;
    }

    [Obsolete("Utilizar la versión que funciona con RUT como String")]
    public String[] obtenerDatosBasicosPorRut(int rut)
    {
        String[] row = null;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombre,email,telefono,personaPermisos.cargo FROM persona
                            INNER JOIN personaPermisos on persona.Rut=personaPermisos.rut
                            where persona.rut=" + rut + "";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            // nombre,email,telefono,cargo
            row = new String[] { dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString()};
        }
        dr.Close();
        con.Close();

        return row;
    }

    public int calculaDv(int rut)
    {
 
        int largo = rut.ToString().Length;
        //var i = 0;
        //var dv = $("#txtDvr").val();
        int mult = 2;
        int suma = 0;
        largo--;
        while (largo >= 0) {
            suma = suma + (rut.ToString()[largo] * mult);
            if (mult > 6)
                mult = 2;
            else
                mult++;
            largo--;
        }

        var resto = suma % 11;
        String digito = (11 - resto).ToString();
        if (digito == "10") 
        {
            digito = "k";
        }
        else if (digito == "11") 
        {
            digito = "0";
        }
        /*if (digito != dv) {
            mje_rut = 'Rut inválido\n';
            $("#lblRut").css("color", "red");
            $("#txtRut").val("");
        }*/

        return Convert.ToInt32(digito);
    }

    /// <summary>
    /// Método que determina si un usuario tiene doble permiso
    /// </summary>
    /// <param name="rut"></param>
    /// <returns></returns>
    public bool GetDoblePermiso(string rut)
    {
        ControlBD _ctrlBD = new ControlBD();
        int count = _ctrlBD.ObtenerDatosFiltrados("select * from personaPermisos where rut = '"+rut+"'").Tables[0].Rows.Count;
        if (count == 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Método que elimina un permiso de los usuarios con doble permiso
    /// </summary>
    /// <param name="rut"></param>
    /// <param name="cargo"></param>
    /// <returns></returns>
    public bool DropPermiso(string rut, string cargo)
    {
        ControlBD _ctrlBD = new ControlBD();
        if (_ctrlBD.InsertarDatos("delete from personaPermisos where rut = " + rut + " and cargo = " + cargo + ""))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public String[] obtenerDatosBasicosPorRut(String rut)
    {
        String[] row = null;
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT nombre,email,telefono,personaPermisos.cargo FROM persona
                            INNER JOIN personaPermisos on persona.Rut=personaPermisos.rut
                            where persona.rut=" + rut + "";
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            // nombre,email,telefono,cargo
            row = new String[] { dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString() };
        }
        dr.Close();
        con.Close();

        return row;
    }

    public Boolean actualizaPersonaPorRut(Persona p, String rut)
    {
        int habilitado = p.EsHabilitado?1:0;
        int multisucursal = p.EsMultiSucursal?1:0;
        String query = "update persona set idPersona='" + p.IdPersona + "', shipCode='" + p.DestinatarioMercancia + "', rut='" + p.RUT + "', nombre='" + p.NombreReal + "', usuario='" + p.Usuario + "', contrasena='" + p.Contraseña + "', email='" + p.Correo + "', telefono='" + p.Telefono + "', multipleSucursal='" + multisucursal + "', habilitado='" + habilitado + "', idHumano2='" + p.IdHumano2 + "' where rut=" + rut;
        return BD.InsertaOActualiza(query);
    }

    public Boolean actualizaIDHUMANO2PorRut(Persona p)
    {
        char[] delimitador = {'-'};
        String rutMantisa = p.RUT.Split(delimitador)[0].Trim();
        String query = "update persona set idHumano2='" + p.IdHumano2 + "' where rut=" + rutMantisa;
        return BD.InsertaOActualiza(query);
    }

    public List<Persona> obtieneTodas()
    {
        String query = @"
            select  persona.idPersona, persona.shipCode, persona.usuario, cast(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) as varchar) as contrasena, concesionario.nombreConcesionario, persona.rut,nombre, email, telefono,multipleSucursal,habilitado, idHumano2
	            from dbo.persona
		            inner join sucursal
			            inner join concesionario on concesionario.nombreConcesionario = sucursal.nombreConcesionario
		            on sucursal.shipCode = persona.shipCode
        ";
        return ejecutarQuery(query);
    }

    public List<Persona> obtienePersonasPorCargo(int cargo)
    {
        String query = @"
        select  persona.idPersona, persona.shipCode, persona.usuario, cast(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) as varchar) as contrasena, concesionario.nombreConcesionario, persona.rut,nombre, email, telefono,multipleSucursal,habilitado, idHumano2, personaPermisos.cargo
	            from dbo.persona
		            inner join sucursal
			            inner join concesionario on concesionario.nombreConcesionario = sucursal.nombreConcesionario
		            on sucursal.shipCode = persona.shipCode
		            inner join personaPermisos
						on persona.rut = personaPermisos.rut
        where personaPermisos.cargo = " + cargo.ToString();
        return ejecutarQuery(query);
    }


    //DataSet 

    public DataSet obtienePersonasPorPerfil(int cargo)
    {
        String query = @"
        select  persona.idPersona, persona.shipCode, persona.usuario, cast(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) as varchar) as contrasena, concesionario.nombreConcesionario, persona.rut,nombre, email, telefono,multipleSucursal,habilitado, idHumano2, personaPermisos.cargo
	            from dbo.persona
		            inner join sucursal
			            inner join concesionario on concesionario.nombreConcesionario = sucursal.nombreConcesionario
		            on sucursal.shipCode = persona.shipCode
		            inner join personaPermisos
						on persona.rut = personaPermisos.rut
        where persona.habilitado= 1 and personaPermisos.cargo = " + cargo.ToString();

        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        con.Open();
        cmd.Connection = con;
        cmd.CommandText = query;
        cmd.CommandTimeout = 10;

        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        sda.Fill(ds);

        return ds;
    }


    public List<Persona> obtienePersonaPorRUT(String RUT)
    {
        String query = @"select persona.idLlave, persona.idPersona, persona.shipCode, persona.usuario, cast(DECRYPTBYPASSPHRASE('ENCRIPTADO',contrasena) as varchar) as contrasena, concesionario.nombreConcesionario, persona.rut,nombre, email, telefono,multipleSucursal,habilitado, idHumano2, personaPermisos.cargo, DevRec, aprobVFC, reemplazo, criticidad from dbo.persona inner join sucursal inner join concesionario on concesionario.nombreConcesionario = sucursal.nombreConcesionario on sucursal.shipCode = persona.shipCode inner join personaPermisos on persona.rut = personaPermisos.rut where persona.rut = '" + RUT + "'";
        return ejecutarQuery(query);
    }

    private List<Persona> ejecutarQuery(String query)
    {
        List<Persona> personas = new List<Persona>();
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
                Persona persona = new Persona();
                persona.IdPersona = dr["idPersona"].ToString();
                persona.IdHumano2 = dr["idHumano2"].ToString();
                persona.DestinatarioMercancia = dr["shipCode"].ToString();
                persona.RUT = dr["rut"].ToString();
                persona.NombreReal = dr["nombre"].ToString();
                persona.Usuario = dr["usuario"].ToString();
                persona.Contraseña = dr["contrasena"].ToString();
                persona.Correo = dr["email"].ToString();
                persona.Telefono = dr["telefono"].ToString();
                persona.EsMultiSucursal = dr["multipleSucursal"].ToString()=="True";
                persona.EsHabilitado = dr["habilitado"].ToString()=="True";
                persona.NombreConcesionario = dr["nombreConcesionario"].ToString();
                persona.Idllave = dr["idLlave"].ToString();

                string devrectxt = dr["DevRec"].ToString();
                bool devrecbool = true;
                if (devrectxt == "")
                    devrecbool = false;
                else
                    devrecbool = bool.Parse(devrectxt);
                persona.devrec = devrecbool;

                string EsAprobVFCt = dr["aprobVFC"].ToString();
                bool boolEsAprobVFCt = true;
                if (EsAprobVFCt == "")
                    boolEsAprobVFCt = false;
                else
                    boolEsAprobVFCt = bool.Parse(EsAprobVFCt);
                persona.EsAprobVFC = boolEsAprobVFCt;
               
                string crit = dr["criticidad"].ToString();
                bool boolCrit = true;
                if (crit == "")
                    boolCrit = false;
                else
                    boolCrit = bool.Parse(crit);
                persona.Criticidad = boolCrit;

                //
                var miReemplazo = dr["reemplazo"].ToString();
                Boolean esreemplazo = false;
                if (miReemplazo != null && miReemplazo == "1")
                { esreemplazo = true; }
                persona.reemplazo = esreemplazo;

                // Se completan los permisos
                PermisosPersona permisosActual = new PermisosPersona();
                List<PermisosPersona> pps = permisosActual.obtienePermisosPersonaPorRUt(persona.RUT);
                int counter = 0;
                foreach (PermisosPersona pp in pps)
                {
                    persona.Permisos[counter] = new PermisosPersona();
                    persona.Permisos[counter].Cargo = pp.Cargo;
                    persona.Permisos[counter].Id = pp.Id;
                    counter++;
                }
                personas.Add(persona);
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
        return personas;
    }

    public String saberMiConcesionario(int rut)
    {
        String nombre = null;

        try
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();

            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = @"SELECT concesionario.nombreConcesionario
                            FROM concesionario, persona,sucursal
                            WHERE persona.shipCode=sucursal.shipCode 
                            AND sucursal.nombreConcesionario = concesionario.nombreConcesionario
                            AND persona.rut= " + rut + "";
            cmd.CommandTimeout = 10;
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                nombre = dr[0].ToString();
                dr.Close();
                con.Close();
                return nombre;
            }
            else
            {
                dr.Close();
                con.Close();

                return "";
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [saberMiConcesionario] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            con.Close();
            return "";
        }


    }

}//FIN CLASE