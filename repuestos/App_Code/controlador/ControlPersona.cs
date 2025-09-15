using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using System.Data;
/// <summary>
/// Summary description for ControlPersona
/// </summary>
public class ControlPersona
{
    ControlBD _controlBD;
    private Persona persona;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ControlPersona));

	public ControlPersona()
	{
        persona = new Persona();
		//
		// TODO: Add constructor logic here
		//
	}

    public int loguearse(String user, String pass)
    {
        return persona.login(user, pass);
    }


    public Boolean insertaUsuario(
        String rut,
        String shipcode,
        String nombre,
        String contrasena,
        String mail,
        String telefono,
        String cargo,
        int multiSucursal,
        string nombreConcesionario,
        String idllave,
        int IdSucursal

    )
    {
        persona.RUT = rut;
        persona.DestinatarioMercancia = shipcode;
        persona.NombreReal = nombre;
        persona.Contraseña = contrasena;
        persona.Correo = mail;
        persona.Telefono = telefono;
        persona.Permisos[0] = new PermisosPersona();
        persona.Permisos[0].Cargo = int.Parse(cargo);
        persona.EsMultiSucursal = multiSucursal == 1;
        persona.NombreConcesionario = nombreConcesionario;
        persona.Idllave = idllave;
        persona.IdSucursal = IdSucursal;
        

        _controlBD = new ControlBD();

        if (rut == "")
        {
            return false;
        }

        // Se inserta la persona
        if (!_controlBD.InsertarDatos(
            @"insert into persona
	    (idLlave,rut,shipCode,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado,IdSucursal)
        values(
	       " + idllave + ", " + rut + ",'" + shipcode + "','" + nombre + "','" + rut + "',ENCRYPTBYPASSPHRASE('ENCRIPTADO','" + contrasena + "'),'" + mail + "','" + telefono + "'," + multiSucursal.ToString() + @",1," + IdSucursal + ")"
        ))
        {
            return false;
        }

        // Se registran los permisos
        _controlBD.InsertarDatos(
            @"insert into personaPermisos (rut,cargo,descripcion) values ('" + rut + "'," + cargo + ",'Usuario')"
        );

        // Se ingresa usuario a Humano2
        if (!Util.ingresaPersonaHumano2(persona))
        {
            logger.Error("Advertencia: Se agregó usuario al sitio, pero hubo problemas al conectar con Humano2");
        }

        return true;
    }

    public Boolean insertaUsuario(
        String rut,
        String shipcode,
        String nombre,
        String contrasena,
        String mail,
        String telefono,
        String cargo,
        int multiSucursal,
        string nombreConcesionario,
        String idllave,
        int reemplazo,
        int aprobVFC,
        bool criticidad,
        int devreclamos

    )
    {
        persona.RUT = rut;
        persona.DestinatarioMercancia = shipcode;
        persona.NombreReal = nombre;
        persona.Contraseña = contrasena;
        persona.Correo = mail;
        persona.Telefono = telefono;
        persona.Permisos[0] = new PermisosPersona();
        persona.Permisos[0].Cargo = int.Parse(cargo);
        persona.EsMultiSucursal = multiSucursal == 1;
        persona.NombreConcesionario = nombreConcesionario;
        persona.Idllave = idllave;
        persona.Criticidad = criticidad;
        persona.devrec = Convert.ToBoolean(devreclamos);

        int criticidad1 = 0;

        if (persona.Criticidad == true)
        {

            criticidad1 = 1;

        }
        else
        {

            criticidad1 = 0;
        }


        _controlBD = new ControlBD();

        if (rut == "")
        {
            return false;
        }

        // Se inserta la persona
        if (!_controlBD.InsertarDatos(
            @"insert into persona
	    (idLlave,rut,shipCode,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado,reemplazo, aprobVFC, criticidad, devrec)
        values(
	       " + idllave + ", " + rut + ",'" + shipcode + "','" + nombre + "','" + rut + "',ENCRYPTBYPASSPHRASE('ENCRIPTADO','" + contrasena + "'),'" + mail + "','" + telefono + "'," + multiSucursal.ToString() + @",1," + reemplazo + ", " + aprobVFC + ", " + criticidad1 + ", " + devreclamos + ")"
        ))
        {
            return false;
        }

        // Se registran los permisos
        _controlBD.InsertarDatos(
            @"insert into personaPermisos (rut,cargo,descripcion) values (" + rut + "," + cargo + ",'Usuario')"
        );

        // Se ingresa usuario a Humano2
        if (!Util.ingresaPersonaHumano2(persona))
        {
            logger.Error("Advertencia: Se agregó usuario al sitio, pero hubo problemas al conectar con Humano2");
        }

        return true;
    }

    public Boolean modificaUsuario(
        String rut,
        String shipcode,
        String nombre,
        String mail,
        String telefono,
        int cargo,
        int multiSucursal,
        String IdHumano2,
        String contrasena,
        String concesionario,
        Boolean habilitado,
        String Idllave,        
        int IdSucursal,
        int reemplazo,
        int aprobVFC,
        bool criticidad,
        int devrec

    ) {
        persona.RUT = rut;
        persona.DestinatarioMercancia = shipcode;
        persona.NombreReal = nombre;
        persona.Contraseña = contrasena;
        persona.Correo = mail;
        persona.Telefono = telefono;
        persona.Permisos[0] = new PermisosPersona();
        //persona.Permisos[0].Cargo = int.Parse(cargo);
        persona.EsMultiSucursal = multiSucursal == 1;
        persona.IdHumano2 = IdHumano2;
        persona.NombreConcesionario = concesionario;
        persona.EsHabilitado = habilitado;
        persona.Idllave = Idllave;
        persona.IdSucursal = IdSucursal;
        persona.Criticidad = criticidad;
        persona.devrec = Convert.ToBoolean(devrec);
        persona.EsAprobVFC = Convert.ToBoolean(aprobVFC);

        int criticidad1 = 0;

        if (persona.Criticidad == true)
        {

            criticidad1 = 1;

        }
        else
        {

            criticidad1 = 0;
        } 

        _controlBD = new ControlBD();

        if (rut == "")
        {
            return false;
        }

        _controlBD.InsertarDatos(
            @"update persona set Idllave = " + Idllave + ", contrasena = ENCRYPTBYPASSPHRASE('ENCRIPTADO','" + contrasena + "') , shipCode = '" + shipcode + "',nombre='" + nombre + "',email='" + mail + "',telefono='" + telefono + "', multipleSucursal='" + multiSucursal + "', habilitado = " + (habilitado ? 1 : 0) + ", idSucursal = " + IdSucursal + ",reemplazo = " + reemplazo + ",aprobVFC = " + aprobVFC + ", criticidad = " + criticidad1 + ", devrec = " + devrec + " where rut = '" + rut + "'"
        );

        _controlBD.InsertarDatos(
            @"update personaPermisos set cargo = " + cargo + " where rut = '" + rut + "'"
        );


        if (System.Configuration.ConfigurationManager.AppSettings["ambiente"].ToString() == "p"){
            // Se ingresa usuario a Humano2
            if (!Util.actualizaPersonaHumano2(persona))
            {
                logger.Error("Error al ingresar persona " + persona.RUT + " en Humano2");
            }
        }

        return true;
    }

    public int modificaContrasena(string rut, String nContrasena)
    {
        return persona.modificaContrasena(rut, nContrasena);
    }

    public String nombrePersona()
    {
        return persona.getNombre();
    }

    public int permisosPersona()
    {
        return persona.getPermisos();
    }

    public string rutPersona()
    {
        return persona.getRut();
    }

    public int IdPersona()
    {
        return persona.getIdPersona();
    }

    public String mailPersona(String usuario, String contrasena)
    {
        return (persona.saberMiMail(usuario, contrasena));
    }

    public String nombrePersona(String usuario, String contrasena)
    {
        return (persona.saberMiNombre(usuario, contrasena));
    }

    public void mail(String nombre, String rut, String contrasena, String mail, String mensaje)
    {
        persona.enviaMail(nombre, rut, contrasena, mail, mensaje);
    }

    public String miConcesionario(string rut)
    {
        return persona.saberMiConcesionario(rut);
    }
    public String datosPersona()
    {
        return persona.datos();
    }

    public String datosPersonaDoble()
    {
        return persona.datosDoble();
    }

    public String datosEliminadosPersona()
    {
        return persona.datosEliminados();
    }

    public String datosRut(string rut)
    {
        return persona.datosPorRut(rut);
    }

    public int eliminaPersona(string rut)
    {
        return persona.eliminaPersona(rut);
    }

    public int rehacerPersona(string rut)
    {
        return persona.rehacerPersona(rut);
    }

    public String datosEliminadosRut(string rut)
    {
        return persona.datosEliminadosPorRut(rut);
    }

    public String datosConcesionario(String concesionario)
    {
        return persona.datosPorConcesionario(concesionario);
    }

    public String datosConcesionarioDoble(String concesionario)
    {
        return persona.datosPorConcesionarioDoble(concesionario);
    }

    public int doblePermiso(string rut)
    {
        return persona.insertaDoblePermiso(rut);
    }

    public int resetaContrasena(string rut)
    {
        return persona.resetaContrasena(rut);
    }

    public String[] buscaPersonaPorRut(string rut)
    {
        return persona.buscaPersonaPorRut(rut);
    }

    public String usuariosPorZona(String zona)
    {
        return persona.usuariosPorZona(zona);
    }

    public String usuariosPorZona()
    {
        return persona.usuariosPorZona();
    }
    [Obsolete("Utilizar método que retorna un objeto persona")]
    /*public String[] obtenerDatosPorRut(string rut)
    {
        return persona.obtenerDatosPorRut(rut);
    }*/
    
    public Persona obtenerDatosPorRut(String rut)
    {
        //var datosresultado = persona.obtienePersonaPorRUT(rut);

        List<Persona>personas = persona.obtienePersonaPorRUT(rut);
        if (personas.Count > 0)
        {
            return personas[0];
        }
        return null;
    }

    public List<Persona> obtenerTodasPersonas()
    {
        return persona.obtieneTodas();
    }

    public String obtenerNombreConcesionarioPorRut(string rut)
    {
        return persona.saberMiConcesionario(rut);
    }

    public List<Persona> obtienePersonasPorCargo(int cargo)
    {
        return persona.obtienePersonasPorCargo(cargo);
    }

    public DataSet obtienePersonasPorPerfil(int cargo)
    {
        return persona.obtienePersonasPorPerfil(cargo);
    }


    

    [Obsolete("Utilizar la sobrecarga del metodo que funciona con RUT como String como parámetro")]
    public String[] obtenerDatosBasicosPorRut(string rut)
    {
        return persona.obtenerDatosBasicosPorRut(rut);
    }

   /* public String[] obtenerDatosBasicosPorRut(String rut)
    {
        return persona.obtenerDatosBasicosPorRut(rut);
    }*/



}