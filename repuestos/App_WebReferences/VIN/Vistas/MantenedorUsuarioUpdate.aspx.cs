using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using log4net;
using log4net.Config;

public partial class Vistas_mantenedorUsuarioUpdate : System.Web.UI.Page
{
    private ControlPersona controlPersona;
    private ControlAuto controlAuto;
    private String[] datosPersona;
    private String _nombreConcesionario, _shipCode;
    ControlBD _controlBD = new ControlBD();

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_mantenedorUsuarioUpdate));

    protected void Page_Init(object sender, EventArgs e)
    {
        // Si los permisos no son de administrador se manda usuario al index
        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
        if (Request.QueryString["rut"] != null)
        {
            txtRut.Text = Request.QueryString["rut"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        controlPersona = new ControlPersona();
        controlAuto = new ControlAuto();
        String evento = "";
        //int rut = 0;
        string rut = "";

        btnBuscar.OnClientClick = "javascript:return validaBlancos();";

        //Parche
        if (ddlSucursal.SelectedValue != null)
        {
            _shipCode = ddlSucursal.SelectedValue;
        }
        // fin parche

        // La primera vez se leen los datos del usuario
        if (!IsPostBack)
        {
            evento = Request.QueryString["ev"];
            rut = Request.QueryString["rut"];

            // Busca persona y llena la informacion
            buscarPersona(rut);
        }
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        buscarPersona(txtRut.Text);
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (ddlSucursal.SelectedValue == null)
        {
            msjesError.InnerText = "Debe seleccionar sucursal y concesionario";
            msjesError.Visible = true;
            return;
        }

        if(ddlTipoUsuario.SelectedValue == "1"){
            _shipCode = "X";
        }

        if (ddlTipoUsuario.SelectedValue == "4" && _shipCode == "")
        {
            _shipCode = "Y";
        }

        String Sucursal = System.Convert.ToString(ddlSucursal.SelectedItem);
        string IdSucursal = _controlBD.ObtenerSucursalID(_shipCode);


        String rut = txtRut.Text;
        String nombre = txtNombre.Text;
        String mail = txtEmail.Text;
        String telefono = txtCodigoArea.Text + "-" + txtTelefono.Text;
        String IdHumano2 = hddIdHumano2.Value.ToString();
        String contrasena = txtContrasena.Text;
        String concesionario = ddlConcesionario.SelectedValue;
        Boolean habilitado = chkHabilitado.Checked;
        String Idllave = txtIdllave.Text;
        int multiSucursal = 0;
        if (cbxMultipleSucursal.Checked)
        {
            multiSucursal = 1;
        }
        int cargo = int.Parse(ddlTipoUsuario.SelectedValue.ToString());

        if (controlPersona.modificaUsuario(rut, _shipCode, nombre, mail, telefono, cargo, multiSucursal, IdHumano2, contrasena, concesionario, habilitado, Idllave.Trim(),System.Convert.ToInt32(IdSucursal)))
        {
            //lblAviso.Text = "Usuario "+ txtRut.Text +" modificado exitosamente";
            msjesError.InnerText = "Usuario " + txtRut.Text + " modificado exitosamente";
            msjesError.Visible = true;
        }
        else
        {
            //lblAviso.Text = "No se pudo modificar al usuario";
            msjesError.InnerText = "No se pudo modificar al usuario. Contacte a soporte";
            msjesError.Visible = true;
        }
//PARCHE
        ddlSucursal.SelectedIndex =
                    ddlSucursal.Items.IndexOf(
                        ddlSucursal.Items.FindByValue(_shipCode));
// FIN PARCHE
    }//fin btnAgregar

    protected void ddlConcesionario_SelectedIndexChanged(object sender, EventArgs e)
    {
        _nombreConcesionario = ddlConcesionario.SelectedItem.Text;
        llena_sucursales();
    }

    protected void llena_sucursales()
    {
        ddlSucursal.Items.Clear();
        foreach (Sucursal sucursal in controlAuto.obtenerDireccionSucursal(_nombreConcesionario))
        {
            ddlSucursal.Items.Add(new ListItem(sucursal.Direccion,sucursal.ShipCode));
        }
    }

    protected void btnVolver_Click(object sender, EventArgs e)
    {
        Response.Redirect("modificaUsuario.aspx");
    }

    protected void llena_concesionarios()
    {
        ddlConcesionario.Items.Clear();
        ddlConcesionario.Items.Add("Seleccionar Concesionario");
        foreach (String concesionario in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(concesionario);
        }
    }

    protected void buscarPersona(string rut)
    {
        try
        {
            llena_concesionarios();

            // primero se comprueba si existe usuario tiene asociada una sucursal
            if (controlPersona.obtenerNombreConcesionarioPorRut(rut) == "")
            {
                msjesError.InnerText = "EL usuario no tiene una sucursal asociada, regularizar lo antes posible";
                msjesError.Visible = true;
            }
            // Se rescata persona
            Persona p = controlPersona.obtenerDatosPorRut(rut.ToString());

            // Si es que no encuentra la persona
            if (p == null)
            {
                msjesError.InnerText = "El usuario no existe";
                msjesError.Visible = true;
                btnAgregar.Enabled = false;
                return;
            }

            // Nombre
            txtNombre.Text = p.NombreReal;

            // Mail
            txtEmail.Text = p.Correo;

            chkHabilitado.Checked = p.EsHabilitado;

            // Id Humano 2
            hddIdHumano2.Value = p.IdHumano2;

            // Teléfono
            String[] prefijoMasNumeroTelefono = p.Telefono.Split('-');
            if (prefijoMasNumeroTelefono.Length == 2)
            {
                txtCodigoArea.Text = prefijoMasNumeroTelefono[0];
                txtTelefono.Text = prefijoMasNumeroTelefono[1];
            }
            else
            {
                txtCodigoArea.Text = "";
                txtTelefono.Text = "";
            }

            // Permisos
            if (p.Permisos[1] != null)
            {
                ListItem item = new ListItem("Doble Permiso", "0");
                ddlTipoUsuario.Items.Add(item);
                ddlTipoUsuario.SelectedIndex = ddlTipoUsuario.Items.IndexOf(item);
            }
            else
            {
                ddlTipoUsuario.SelectedIndex = p.Permisos[0].Cargo - 1;
            }

            // Contrseña
            txtContrasena.Text = p.Contraseña;


            //idllave

            txtIdllave.Text = p.Idllave; 

            // Se completa informacion de concesionario
            ddlConcesionario.SelectedIndex =
                ddlConcesionario.Items.IndexOf(
                    ddlConcesionario.Items.FindByText(p.NombreConcesionario)
                    );

            // Se llena la sucursal
            _nombreConcesionario = p.NombreConcesionario;
            llena_sucursales();

            ControlSucursal _controlSucursal = new ControlSucursal();
                
            ddlSucursal.SelectedIndex =
                ddlSucursal.Items.IndexOf(
                    ddlSucursal.Items.FindByText(_controlSucursal.obtenerSucursalPorShipCode(p.DestinatarioMercancia).Direccion));
            
            cbxMultipleSucursal.Checked = p.EsMultiSucursal;
        }
        catch (ArgumentNullException)
        {
            rut = "0";
        }
        catch (FormatException)
        {
            rut = "0";
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Hubo problemas al cargar el usuario, contacte a soporte";
            logger.Error("En [buscarPersona] M:" + ex.Message + ", S: " + ex.StackTrace + ", I: " + ex.InnerException);
            btnAgregar.Enabled = false;
        }
    }
}
