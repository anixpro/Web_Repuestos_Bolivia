using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Diagnostics;
using log4net;
using log4net.Config;

public partial class Vistas_Mantenedor : System.Web.UI.Page
{
    private ControlPersona controlPersona;
    private ControlAuto controlAuto;
    ControlBD _controlBD = new ControlBD();

    protected String _nombreConcesionario, _shipCode, _sucursal;
    protected int _iConcesionario, _iSucursal;
    protected String rescate = "";

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Mantenedor));

    protected void Page_Init(object sender, EventArgs e)
    {
        // Comprobación de permisos
        if (int.Parse(Session["permisos"].ToString()) ==2)
        {
            Response.Redirect("mantenedorConcesionario.aspx");
        }

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }

        controlAuto = new ControlAuto();

        // Si se ingresa por primera vez
        if (!IsPostBack)
        {
            foreach (String concesionario in controlAuto.obtenerConcesionario())
            {
                ddlConcesionario.Items.Add(concesionario);
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        controlPersona = new ControlPersona();
        msjesError.Visible = false;
        DropDownList drop = new DropDownList();
        
        if (IsPostBack){
            _sucursal = hdnSucursal.Value;
            _iSucursal = Convert.ToInt32(hdnISucursal.Value);
            _shipCode = hdnValSucursal.Value;
        }
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        try
        {

            string IdSucursal = _controlBD.ObtenerSucursalID(_shipCode);
            
            
            if (ddlTipoUsuario.SelectedValue != "1" &&
                ddlTipoUsuario.SelectedValue != "5" &&
                (_shipCode == "" || _shipCode == "0"))
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Porfavor, seleccione una sucursal";
                return;
            }

            else if (ddlTipoUsuario.SelectedValue == "1" || ddlTipoUsuario.SelectedValue == "5")
            {
                _shipCode = "X";
            }

            int multiSucursal = 0;

            if (cbxMultipleSucursal.Checked && ddlTipoUsuario.SelectedValue == "3")
            {
                multiSucursal = 1;
            }
			if (controlPersona.obtenerDatosBasicosPorRut(txtRut.Text) != null)
					{
						throw new ArgumentException("Error, el usuario existe");
					}
            if (!controlPersona.insertaUsuario(txtRut.Text, _shipCode, txtNombre.Text, contrasena.Text, txtEmail.Text, txtCodigoArea.Text + "-" + txtTelefono.Text, ddlTipoUsuario.SelectedValue, multiSucursal, ddlConcesionario.SelectedItem.Text, txtIdllave.Text.Trim(), System.Convert.ToInt32(IdSucursal)))
				{
					if (controlPersona.obtenerDatosBasicosPorRut(txtRut.Text) != null)
					{
						throw new ArgumentException("Error, el usuario existe");
					}
					else
					{
						throw new Exception("No se pudo insertar usuario, contacte al administrador");
					}
				}
            String mensaje = "Estimado " + txtNombre.Text + ", ha sido registrado en la web de repuestos de Astara con los siguientes datos:\n";
            controlPersona.mail(txtNombre.Text, txtRut.Text, contrasena.Text, txtEmail.Text, mensaje);
            msjesError.Visible = true;
            msjesError.InnerText = "El usuario: " + txtNombre.Text + ", ha sido creado con éxito";
            txtNombre.Text = "";
            txtRut.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtCodigoArea.Text = "";
            //txtDvr.Text = "";
            contrasena.Text = "";
            txtIdllave.Text = "";
            ddlConcesionario.SelectedIndex = 0;
            ddlSucursal.SelectedIndex = 0;
            ddlTipoUsuario.SelectedIndex = 0;
        }

        catch (SqlException ex)
        {
            lblRut.ForeColor = System.Drawing.Color.Red;
            msjesError.Visible = true;
            msjesError.InnerText = "Usuario existe, hubo problema consistencia de datos o la BD no está bien configurada";
            txtNombre.Text = "";
            txtRut.Text = "";
            txtCodigoArea.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            //txtDvr.Text = "";
            contrasena.Text = "";
            txtIdllave.Text = ""; 
            logger.Error("En [btnAgregar_Click] Message:" + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        catch (FormatException)
        {
            //en caso de que el correo sea erroneo no hago nada simplemente la aplicacion sigue normalmente
            msjesError.Visible = true;
            msjesError.InnerText = "Usuario: " + txtNombre.Text + " agregado exitosamente";
            txtNombre.Text = "";
            txtRut.Text = "";
            txtCodigoArea.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            //txtDvr.Text = "";
            txtIdllave.Text = "";
            ddlConcesionario.SelectedIndex = 0;
            ddlSucursal.SelectedIndex = 0;
            ddlTipoUsuario.SelectedIndex = 0;
        }

        catch (ArgumentException ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Error. El rut ya existe en el sistema!";
            logger.Info("En [btnAgregar_Click] Message:" + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Error al registrar usuario, contacte a soporte";
            logger.Error("En [btnAgregar_Click] Message:" + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }
}
