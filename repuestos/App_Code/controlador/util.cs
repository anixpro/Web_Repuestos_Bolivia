using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Data;
using log4net;
using log4net.Config;
using System.Xml.Linq;
using System.Xml;
using System.Configuration;
//using com.h2center.skberge;
//using cl.humano2.skbergecl;

/// <summary>
/// Summary description for util
/// </summary>
public static class Util
{
    // Atributos relacionados con boletin
    static int boletinesPorLeer = 0;
    static Boletin modeloBoletin;

    // Atributos para ser usados con WS de Humano2
    //static AdmUserVFC h2center = new AdmUserVFC();
    static String user = "userVFC";
    static String pass = "us3rVF";

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ControlPersona));

    public static int BoletinesPorLeer
    {
        get { return boletinesPorLeer; }
        set { boletinesPorLeer = value; }
    }

    /// <summary>
    /// method for validating a url with regular expressions
    /// </summary>
    /// <param name="url">url we're validating</param>
    /// <returns>true if valid, otherwise false</returns>
    public static bool isValidUrl(ref string url)
    {
        string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
        Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return reg.IsMatch(url);
    }

    public static string convertToMoneyPeru(decimal valor){
        string numero = string.Format("{0:N2}", valor); //se revierte el formato de . y ,
        numero = numero.Replace(",", "|");
        numero = numero.Replace(".", ",");
        numero = numero.Replace("|", ".");
        return numero; 
    }

    public static bool noLeyoNoticiaImportante(String rut)
    {
        Contenido contenido= new Contenido();
        return contenido.leyoNoticia(rut);
    }

    public static string obtenerUltimaNoticiaImportante()
    {
        Contenido contenido = new Contenido();

        // Se obtiene ultima noticia importante
        return contenido.obtieneUltimaNoticia();
    }

    public static int confirmaLectura(String rut)
    {
        Contenido contenido = new Contenido();

        // Se obtiene ultima noticia importante
        return contenido.leeUltimaNoticia(rut);
    }

    public static void BoletinLeido(string rut, string idBoletin)
    {
        ControlBD _controlBd = new ControlBD();
        //Insertar en la tabla boletin_persona
       
        _controlBd.InsertarDatos("insert into boletines_persona values('"+idBoletin+"','"+rut+"')");
        
    }

    public static int obtieneCantidadDeBoletinesSinLeerPorRut(string rut)
    {
        modeloBoletin = new Boletin();
        BoletinesPorLeer = modeloBoletin.obtenerCantidadDeBoletinesLeidosPorRut(rut);
        return BoletinesPorLeer;
    }

    public static string limpiaPalabras(String palabra)
    {
        try
        {
            palabra = palabra.Replace('ñ', 'n');
            palabra = palabra.Replace('Ñ', 'N');
            palabra = palabra.Replace('á', 'a');
            palabra = palabra.Replace('é', 'e');
            palabra = palabra.Replace('í', 'i');
            palabra = palabra.Replace('ó', 'o');
            palabra = palabra.Replace('ú', 'u');
            palabra = palabra.Replace('Á', 'A');
            palabra = palabra.Replace('É', 'E');
            palabra = palabra.Replace('Í', 'I');
            palabra = palabra.Replace('Ó', 'O');
            palabra = palabra.Replace('Ú', 'U');
            palabra = palabra.Replace('&', 'Y');
        }
        catch (Exception)
        {
            
        }
        
        return palabra;
    }

    static public String obtieneNombreConcesionarioPorRUTCualquierUsuario(String rut)
    {
        String query=@"
            select concesionario.nombreConcesionario 
            from persona,sucursal,concesionario
            where persona.rut='"+rut+"'"+
            @"AND persona.shipCode=sucursal.shipCode "+
            @"AND sucursal.nombreConcesionario=concesionario.nombreConcesionario";
        return BD.queryRetornaValorExacto(query);
    }

    static public Boolean ingresaPersonaHumano2(Persona persona)
    {
       /* RutHelper rh = new RutHelper();
        XDocument miXML = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XComment("Lista de Registros"),
            new XElement("registros",
                new XElement("registro",
                    new XElement("idusuario", ""),
                    new XElement("usuario", rh.GetRutConDigito(persona.RUT)),
                    new XElement("password", Util.limpiaPalabras(persona.Contraseña)),
                    new XElement("userdesc", Util.limpiaPalabras(persona.NombreReal)),
                    new XElement("concesionariodesc", Util.limpiaPalabras(persona.NombreConcesionario)),
                    new XElement("destinatariosap", persona.DestinatarioMercancia),
                    new XElement("email", Util.limpiaPalabras(persona.Correo))
                    )
                )
            );

        // Si se trata de ambiente de desarrollo, no molestamos a nuestros colegas de H2Center
        if (ConfigurationManager.AppSettings["ambiente"].ToString() == "d")
        {
            return true;
        }

        //String resultado = h2center.setUsuario(miXML.ToString(), user, pass);

        //String datosXml = resultado;
        XmlDocument xDoc = new XmlDocument();

        //leo el xml en formato String con loadXml
       // xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("sucesos");

        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("suceso");

        Boolean returnValue = false;

        foreach (XmlElement nodo in lista)
        {
            char[] separador = { ':' };
            if (nodo.ChildNodes[0].InnerText.Split(separador).Length > 1)
            {
                String a = nodo.ChildNodes[0].InnerText.Split(separador)[1].Trim();
                persona.IdHumano2 = a;
                returnValue = persona.actualizaIDHUMANO2PorRut(persona);
            }
        }
       */
        return true;
    }

    static public Boolean actualizaPersonaHumano2(Persona persona)
    {
      /*  RutHelper rh = new RutHelper();
        XDocument miXML = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XComment("Lista de Registros"),
            new XElement("registros",
                new XElement("registro",
                    new XElement("idusuario", persona.IdHumano2),
                    new XElement("usuario", rh.GetRutConDigito(persona.RUT)),
                    new XElement("password", Util.limpiaPalabras(persona.Contraseña)),
                    new XElement("userdesc", Util.limpiaPalabras(persona.NombreReal)),
                    new XElement("concesionariodesc", Util.limpiaPalabras(persona.NombreConcesionario)),
                    new XElement("destinatariosap", persona.DestinatarioMercancia),
                    new XElement("email", Util.limpiaPalabras(persona.Correo))
                    )
                )
            );
        String resultado = h2center.setUsuario(miXML.ToString(), user, pass);

        String datosXml = resultado;
        XmlDocument xDoc = new XmlDocument();

        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);
        XmlNodeList resultados = xDoc.GetElementsByTagName("sucesos");
        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("suceso");
        Boolean returnValue = false;

        foreach (XmlElement nodo in lista)
        {
            char[] separador = { ':' };
            if (nodo.ChildNodes[0].InnerText.Split(separador).Length > 1)
            {
                returnValue = true;
            }
        }*/
        return true;
    }
}