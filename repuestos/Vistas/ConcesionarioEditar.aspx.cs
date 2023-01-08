using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using log4net;
using log4net.Config;
using System.Text.RegularExpressions;

public partial class Vistas_ConcesionarioEditar : System.Web.UI.Page
{
    ControlBD _controlBd;
    protected void Page_Init(object sender, EventArgs e)
    {

        if (int.Parse(Session["permisos"].ToString()) > 2)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        if (Request.QueryString["id"] != null)
        {
            string idConcesionario = Request.QueryString["id"];
            hdIdConcesionario.Value = idConcesionario;

            if (!IsPostBack)
            {
                 cargaddlAdministrador();
                 cargaddlSucursal();
     
                _controlBd = new ControlBD();
                DataSet ds = _controlBd.ObtenerDatosFiltrados("Select * from concesionario where idConcesionario=" + idConcesionario);

                if (ds.Tables.Count < 1)
                {
                    if (ds.Tables[0].Rows.Count < 1)
                    {
                        Response.Redirect("mantenedorConcesionario.aspx");
                    }
                }

                String _nombre = ds.Tables[0].Rows[0]["nombreConcesionario"].ToString();
                String _RUT = ds.Tables[0].Rows[0]["rutHolding"].ToString();
                String _CodCliente = ds.Tables[0].Rows[0]["numeroFactura"].ToString();
                String _RutSupervisor = ds.Tables[0].Rows[0]["RutSupervisor"].ToString();
                String _RutAdministrador = ds.Tables[0].Rows[0]["RutAdministrador"].ToString();
                String _CorreosVFC = ds.Tables[0].Rows[0]["CorreosVFC"].ToString();
                nombre.Text = _nombre;
                hdAntiguoNombreConcesionario.Value = _nombre;
                RUT.Text = _RUT;
                txtNumFactura.Text = _CodCliente;
                ddlSupervisor.SelectedValue = _RutSupervisor;
                ddlAdministrador.SelectedValue = _RutAdministrador;
                txtCorreoVFC.Text = _CorreosVFC; 
            }
        }
        else
        {
            Response.Redirect("mantenedorConcesionario.aspx");
        }
    }
    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        if (txtNumFactura.Text == "")
        {
            msjesError.InnerText = "Debe ingresar un codigo de cliente SAP";
            msjesError.Visible = true;
            return;
        }

        /*if (ddlAdministrador.SelectedValue.ToString() == "0") 
        {
            msjesError.InnerText = "Debe ingresar un Administrador";
            msjesError.Visible = true;
            return;
        
        }*/

        /*if (ddlSupervisor.SelectedValue.ToString() == "0")
        {
            msjesError.InnerText = "Debe ingresar un Supervisor";
            msjesError.Visible = true;
            return;

        }*/

        if (txtCorreoVFC.Text != "")
        {
            string correos = txtCorreoVFC.Text; 
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (correos.Substring((correos.Length - 1), 1) == ",") {
                msjesError.InnerText = "Error: Las direcciones de correo deben ir sin , al final ";
                msjesError.Visible = true;
                return;
            }

         

            string[] strArr = null;

            char[] splitchar = { ',' };
            strArr = correos.Split(splitchar);
            int count = 0;
            for (count = 0; count <= strArr.Length - 1; count++)
            {
                if (strArr[count].ToString() != "")
                {
                    if (!(Regex.IsMatch(strArr[count], expresion)))
                    {
                        msjesError.InnerText = "Error: La dirección de correo " + strArr[count].ToString() + " es incorrecta. ";
                        msjesError.Visible = true;
                        return;
                    }
                }
            }




            


            //var mySplitResult = cajaTexto.split(";");

            //        for (i = 0; i < mySplitResult.length; i++) {

            //            email = mySplitResult[i];
            //            if (email != "") {
            //                if (!expr.test(email)) {
            //                    alert("Error: La dirección de correo " + email + " es incorrecta.");
            //                    mje_corrvfc = 'La dirección de correo ' + email + ' es incorrecta.\n';
            //                    document.getElementById('lblCorreoVFC').style.color = "red";
            //                }
            //            }
            //        }

        }


        _controlBd = new ControlBD();
        try
        {
            _controlBd.InsertarDatos("update concesionario set nombreConcesionario='" + nombre.Text + "', rutHolding='" + RUT.Text + "', numeroFactura='" + txtNumFactura.Text + "' , rutSupervisor= '" + ddlSupervisor.SelectedValue.ToString() + "', rutAdministrador='" + ddlAdministrador.SelectedValue.ToString() + "',correosVFC='" + txtCorreoVFC.Text + "' where idConcesionario = " + hdIdConcesionario.Value);
            _controlBd.InsertarDatos("update sucursal set numeroFactura='" + txtNumFactura.Text + "' where nombreConcesionario = '" + nombre.Text + "'");
            msjesError.InnerText = "Concesionario " + nombre.Text + ", " + RUT.Text + " actualizado";
            msjesError.Visible = true;
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Hubo error al actualizar. Contacte al administrador. Detalle técnico: " + ex.Message;
            msjesError.Visible = true;
        }
    }
    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorConcesionario.aspx");
    }

    public void cargaddlAdministrador()
    {
        ControlPersona contper = new ControlPersona();
        ddlAdministrador.DataSource = contper.obtienePersonasPorPerfil(5);
        ddlAdministrador.DataValueField = "rut";
        ddlAdministrador.DataTextField = "nombre";
        ddlAdministrador.DataBind();
        ddlAdministrador.Items.Insert(0, new ListItem("Elija una Opcion..", "0"));

    }

    public void cargaddlSucursal()
    {
        ControlPersona contper = new ControlPersona();
        ddlSupervisor.DataSource = contper.obtienePersonasPorPerfil(5);
        ddlSupervisor.DataValueField = "rut";
        ddlSupervisor.DataTextField = "nombre";
        ddlSupervisor.DataBind();
        ddlSupervisor.Items.Insert(0, new ListItem("Elija una Opcion..", "0"));

    }
}