using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml;

public partial class prueba : System.Web.UI.Page
{
    com.h2center.skberge.AdmUserVFC h2center = new com.h2center.skberge.AdmUserVFC();
    String user = "userVFC";
    String pass = "us3rVF";

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnAcepta_Click(object sender, EventArgs e)
    {
        RutHelper rh = new RutHelper();
        XDocument miXML = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XComment("Lista de Registros"),
            new XElement("registros",
                new XElement("registro",
                    new XElement("idusuario", idUsuario.Text),
                    new XElement("usuario", rh.GetRutConDigito(usuario.Text)),
                    new XElement("password", password.Text),
                    new XElement("userdesc", userdesc.Text),
                    new XElement("concesionariodesc", concesionariodesc.Text),
                    new XElement("destinatariosap", destinatariosap.Text),
                    new XElement("email", email.Text)
                    )
                )
            );
        String resultado = h2center.setUsuario(miXML.ToString(), user, pass);
    }
    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        XDocument miXML = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XComment("Lista de Registros"),
            new XElement("registros",
                new XElement("registro",
                    new XElement("idusuario", txtConsulta.Text)
                    )
                )
            );
        String resultado = h2center.getUsuario(miXML.ToString(), user, pass);
    }


    protected void Unnamed1_Click1(object sender, EventArgs e)
    {
        String datosXml = h2center.getUsuariosTodos(user,pass);
        datosXml = datosXml.Replace("&AMP;", "Y");
        XmlDocument xDoc = new XmlDocument();

        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);

        XmlNodeList resultados = xDoc.GetElementsByTagName("usuarios");

        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("usuario");


        foreach (XmlElement nodo in lista)
        {
            Persona p = new Persona();
            //<idusuario>3525485</idusuario>
            p.IdHumano2 = nodo.ChildNodes[0].InnerText;
            //<nombreusuario>ADMINISTRADOR_H2</nombreusuario>
            p.RUT = nodo.ChildNodes[1].InnerText;
            //<password>12345</password>
            p.Contraseña = nodo.ChildNodes[2].InnerText;
            //<userdesc>ADMINISTRADOR</userdesc>

            //<concesionario>TESTING HUMANO2</concesionario>

            //<nombreconcesionario>MMC CHILE S.A.</nombreconcesionario>
            p.NombreConcesionario = nodo.ChildNodes[5].InnerText;
            //<destinatariochsap>3100001049</destinatariochsap>
            p.DestinatarioMercancia = nodo.ChildNodes[6].InnerText;
            //<email></email>
            p.Correo = nodo.ChildNodes[7].InnerText;
            //<estado></estado>

            // Se ingresa idHumano2 en BD
            p.actualizaIDHUMANO2PorRut(p);
        }
    }

    protected void btnGrabar_Click(object sender, EventArgs e)
    {
        RutHelper rh = new RutHelper();
        Persona persona = new Persona();
        List<Persona> personas = persona.obtieneTodas();
        foreach (Persona p in personas)
        {
            Response.Write("id:" + p.IdPersona + "<br/>");
            Response.Write("pass:" + p.Contraseña + "<br/>");
            Response.Write("correo:" + p.Correo + "<br/>");
            Response.Write("destMercancia:" + p.DestinatarioMercancia + "<br/>");
            Response.Write("habilitador:" + p.EsHabilitado + "<br/>");
            Response.Write("multi:" + p.EsMultiSucursal + "<br/>");
            Response.Write("idH2:" + p.IdHumano2 + "<br/>");
            Response.Write("concesionario:" + p.NombreConcesionario + "<br/>");
            Response.Write("nombre:" + p.NombreReal + "<br/>");
            Response.Write("rut:" + p.RUT + "<br/>");
            Response.Write("telefono:" + p.Telefono + "<br/>");
            Response.Write("usuario:" + p.Usuario + "<br/>");
            Response.Write("<br/><br/><br/>");
            if (p.IdHumano2 != "")
            {
                XDocument miXML = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Lista de Registros"),
                new XElement("registros",
                    new XElement("registro",
                        new XElement("idusuario", p.IdHumano2),
                        new XElement("usuario", rh.GetRutConDigito(p.RUT)),
                        new XElement("password", Util.limpiaPalabras(p.Contraseña)),
                        new XElement("userdesc", Util.limpiaPalabras(p.NombreReal)),
                        new XElement("concesionariodesc", Util.limpiaPalabras(p.NombreConcesionario)),
                        new XElement("destinatariosap", p.DestinatarioMercancia),
                        new XElement("email", p.Correo)
                        )
                    )
                );
                String resultado = h2center.setUsuario(miXML.ToString(), user, pass);
            }
        }
    }
}

