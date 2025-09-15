using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Vistas_MantenedorClasePedido : System.Web.UI.Page
{
    MensajeSistema RespuestaBD;
    ControlBD BaseDatos = new ControlBD();
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantenedorClasePedido));
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                // Solo se ejecuta la primera vez que carga la página
                PanelCrearClase.Visible = false;
                PanelEditarClase.Visible = false;
                PanelDesactivarClase.Visible = false;
                PanelAsignarClasePedido.Visible = false;
                CargarComboClasePedido();
            }
            else
            {
                // En postback, mantén visible el panel si estaba visible antes
                // o si el postback proviene del botón de crear clase
            }
        }
        catch(Exception ex)
        {

        }
    }

    protected void BtnInsertarClase_Click(object sender, EventArgs e)
    {
        try
        {
            PanelCrearClase.Visible = true;
            PanelEditarClase.Visible = false;
            PanelDesactivarClase.Visible = false;
            PanelAsignarClasePedido.Visible = false;
        }
        catch(Exception ex)
        {

        }
    }

    protected void BtnEdiatClase_Click(object sender, EventArgs e)
    {
        PanelCrearClase.Visible = false;
        PanelEditarClase.Visible = true;
        PanelDesactivarClase.Visible = false;
        PanelAsignarClasePedido.Visible = false;
    }


    protected void BtnDesactivarClase_Click(object sender, EventArgs e)
    {
        PanelCrearClase.Visible = false;
        PanelEditarClase.Visible = false;
        PanelDesactivarClase.Visible = true;
        PanelAsignarClasePedido.Visible = false;
    }

    protected void BtnClasePedidoUsuario_Click(object sender, EventArgs e)
    {
        PanelCrearClase.Visible = false;
        PanelEditarClase.Visible = false;
        PanelDesactivarClase.Visible = false;
        PanelAsignarClasePedido.Visible = true;
    }

    protected void btnAgregarClase_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtNombreClase.Text == "")
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Debe ingresar nombre clase";
                return;
            }
            else if (TxtCodigoClase.Text == "")
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Debe ingresar código clase";
                return;
            }
            else
            {
                RespuestaBD = BaseDatos.InsertaClasePedido(txtNombreClase.Text, TxtCodigoClase.Text);

                if (RespuestaBD.Codigo == "1")
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Nombre de clase de pedido ya existe";
                    return;
                }
                else if (RespuestaBD.Codigo == "2")
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Codigo de clase de pedido ya existe";
                    return;
                }
                else if (RespuestaBD.Codigo == "Error")
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                    logger.Error("Error al insertar nueva clase: " + RespuestaBD.Mensaje);
                    return;
                }
                else if (RespuestaBD.Codigo == "OK")
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Clase creada exitosamente";
                    txtNombreClase.Text = "";
                    TxtCodigoClase.Text = "";
                    return;
                }
                else
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Ha ocurrido un error desconocido en el sistemas, favor contactar a administrador";
                    logger.Error("Error al insertar nueva clase: " + RespuestaBD.Mensaje);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error al insertar nueva clase: " + ex.Message);
            return;
        }
    }

    protected void ddlClasePedidoEditar_SelectedIndexChanged(object sender, EventArgs e)
    {
        TxtNombreClaseEditar.Text = ddlClasePedidoEditar.SelectedItem.ToString();
    }

    protected void BtnEditarClase_Click(object sender, EventArgs e)
    {

    }

    private void CargarComboClasePedido()
    {
        ddlClasePedidoEditar.AppendDataBoundItems = true;
        ddlClasePedidoEditar.Items.Add("Seleccione");
        ddlClasePedidoEditar.DataSource = BaseDatos.CargaComboClasePedidoMantedor();
        ddlClasePedidoEditar.DataMember = "Table";
        ddlClasePedidoEditar.DataValueField = "ID";
        ddlClasePedidoEditar.DataTextField = "Nombre";
        ddlClasePedidoEditar.DataBind();
    }
}