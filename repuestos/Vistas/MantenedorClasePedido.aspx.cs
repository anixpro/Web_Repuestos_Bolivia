using log4net;
using System;
using System.Collections.Generic;
using System.Data;
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
                CargarComboClasePedidoActivo();
                CargarCheckMarcas();
                CargarComboUsuarioAsignar();
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
            LimpiarValoresModuloInsertar();
            msjesErrorInsertar.Visible = false;
            msjesErrorAsignar.Visible = false;
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
        LimpiarValoresModuloEditar();
        msjesErrorEditar.Visible = false;
        msjesErrorAsignar.Visible = false;
    }


    protected void BtnDesactivarClase_Click(object sender, EventArgs e)
    {
        PanelCrearClase.Visible = false;
        PanelEditarClase.Visible = false;
        PanelDesactivarClase.Visible = true;
        PanelAsignarClasePedido.Visible = false;
        msjesErrorDesactivar.Visible = false;
        msjesErrorAsignar.Visible = false;
    }

    protected void BtnClasePedidoUsuario_Click(object sender, EventArgs e)
    {
        PanelCrearClase.Visible = false;
        PanelEditarClase.Visible = false;
        PanelDesactivarClase.Visible = false;
        PanelAsignarClasePedido.Visible = true;
        msjesErrorAsignar.Visible = false;
    }

    protected void btnAgregarClase_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtNombreClase.Text == "")
            {
                msjesErrorInsertar.Visible = true;
                msjesErrorInsertar.InnerText = "Debe ingresar nombre clase";
                return;
            }
            else if (TxtCodigoClase.Text == "")
            {
                msjesErrorInsertar.Visible = true;
                msjesErrorInsertar.InnerText = "Debe ingresar código clase";
                return;
            }
            else
            {
                msjesErrorInsertar.Visible = false;
                RespuestaBD = BaseDatos.InsertaClasePedido(txtNombreClase.Text, TxtCodigoClase.Text);

                if (RespuestaBD.Codigo == "1")
                {
                    msjesErrorInsertar.Visible = true;
                    msjesErrorInsertar.InnerText = "Nombre de clase de pedido ya existe";
                    return;
                }
                else if (RespuestaBD.Codigo == "2")
                {
                    msjesErrorInsertar.Visible = true;
                    msjesErrorInsertar.InnerText = "Codigo de clase de pedido ya existe";
                    return;
                }
                else if (RespuestaBD.Codigo == "Error")
                {
                    msjesErrorInsertar.Visible = true;
                    msjesErrorInsertar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                    logger.Error("Error al insertar nueva clase: " + RespuestaBD.Mensaje);
                    return;
                }
                else if (RespuestaBD.Codigo == "OK")
                {
                    msjesErrorInsertar.Visible = true;
                    msjesErrorInsertar.InnerText = "Clase creada exitosamente";
                    LimpiarValoresModuloInsertar();
                    return;
                }
                else
                {
                    msjesErrorInsertar.Visible = true;
                    msjesErrorInsertar.InnerText = "Ha ocurrido un error desconocido en el sistemas, favor contactar a administrador";
                    logger.Error("Error al insertar nueva clase: " + RespuestaBD.Mensaje);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            msjesErrorInsertar.Visible = true;
            msjesErrorInsertar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error al insertar nueva clase: " + ex.Message);
            return;
        }
    }

    protected void ddlClasePedidoEditar_SelectedIndexChanged(object sender, EventArgs e)
    {
        TxtNombreClaseEditar.Text = ddlClasePedidoEditar.SelectedItem.ToString();
    }
    protected void ddlClasePedidoActivar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            msjesErrorDesactivar.Visible = false;
            if (int.Parse(ddlClasePedidoActivar.SelectedValue) != 0)
            {
                string estado = "0";
                DataSet dt = BaseDatos.CargaEstadoClasePedidoMantedor(int.Parse(ddlClasePedidoActivar.SelectedValue));

                if (dt.Tables.Count > 0)
                {
                    foreach (DataTable Tabla in dt.Tables)
                    {
                        foreach (DataRow Fila in Tabla.Rows)
                        {
                            estado = Fila["Habilitado"].ToString();
                        }
                    }

                    rblEstadoClase.SelectedValue = estado;
                }
            }
        }
        catch(Exception ex)
        {
            msjesErrorDesactivar.Visible = true;
            msjesErrorDesactivar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error en drop desactivar clase: " + ex.Message);
            return;
        }

        
    }

    protected void ddlUsuarioAsignar_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            msjesErrorAsignar.Visible = false;

            DataSet ds = BaseDatos.CargaClaseUsuario(int.Parse(ddlUsuarioAsignar.SelectedValue));

            DataTable TablaClaseUsuario = (ds != null && ds.Tables.Count > 0) ? ds.Tables["Table"] : null;


            if(TablaClaseUsuario != null)
            {
                foreach (DataRow row in TablaClaseUsuario.Rows)
                {
                    string idClase = row["ID"].ToString(); 
                    ListItem item = cbxClasePedido.Items.FindByValue(idClase);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
            }
            else
            {
                foreach (ListItem item in cbxClasePedido.Items)
                {
                    item.Selected = false;
                }
            }
        }
        catch(Exception ex)
        {
            msjesErrorAsignar.Visible = true;
            msjesErrorAsignar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error al ddlAsiganrUsuario nueva clase: " + ex.Message);
            return;
        }
    }


    protected void BtnEditarClase_Click(object sender, EventArgs e)
    {
        try
        {
            if(ddlClasePedidoEditar.Text == "0")
            {
                msjesErrorEditar.Visible = true;
                msjesErrorEditar.InnerText = "Debe seleccionar clase de clase";
                return;
            }
            else if (TxtNombreClaseEditar.Text == "")
            {
                msjesErrorEditar.Visible = true;
                msjesErrorEditar.InnerText = "Debe ingresar nombre de clase";
                return;
            }
            else
            {
                msjesErrorEditar.Visible = false;
                RespuestaBD = BaseDatos.ActualizaClasePedido(TxtNombreClaseEditar.Text, Int32.Parse(ddlClasePedidoEditar.SelectedValue));

                if(RespuestaBD.Codigo == "OK")
                {
                    msjesErrorEditar.Visible = true;
                    msjesErrorEditar.InnerText = "Clase actualizada correctamente";
                    LimpiarValoresModuloEditar();
                    return;
                }
                else if(RespuestaBD.Codigo == "ERROR")
                {
                    msjesErrorEditar.Visible = true;
                    msjesErrorEditar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                    LimpiarValoresModuloEditar();
                    return;
                }
                else
                {
                    msjesErrorEditar.Visible = true;
                    msjesErrorEditar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                    LimpiarValoresModuloEditar();
                    return;
                }

            }
        }
        catch(Exception ex)
        {
            msjesErrorEditar.Visible = true;
            msjesErrorEditar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error al insertar nueva clase: " + ex.Message);
            return;
        }
    }

    protected void btnDesactivarClasePedido_Click(object sender, EventArgs e)
    {
        try
        { 
            RespuestaBD = BaseDatos.DesactivarClasePedido(int.Parse(ddlClasePedidoActivar.SelectedValue), int.Parse(rblEstadoClase.SelectedValue));

            if(RespuestaBD.Codigo == "OK")
            {
                msjesErrorDesactivar.Visible = true;
                msjesErrorDesactivar.InnerText = "Estado actualizado correctamente";
                LimpiarValoresModuloDesactivar();
                return;
            }
            else if(RespuestaBD.Codigo == "ERROR")
            {
                msjesErrorDesactivar.Visible = true;
                msjesErrorDesactivar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                return;
            }
            else
            {
                msjesErrorDesactivar.Visible = true;
                msjesErrorDesactivar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                return;
            }


        }
        catch(Exception ex)
        {
            msjesErrorDesactivar.Visible = true;
            msjesErrorDesactivar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error al Desactivar clase: " + ex.Message);
            return;
        }
    }

    protected void btnAignarClasePedido_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlUsuarioAsignar.SelectedValue == "0")
            {
                msjesErrorAsignar.Visible = true;
                msjesErrorAsignar.InnerText = "Debe seleccionar Usuario";
                return;
            }
            else
            {
                RespuestaBD = BaseDatos.EliminarClasePedidoUsuario(int.Parse(ddlUsuarioAsignar.SelectedValue));

                if (RespuestaBD.Codigo == "OK")
                {
                    List<int> ClasesSeleccionadas = new List<int>();

                    foreach (ListItem item in cbxClasePedido.Items)
                    {
                        if (item.Selected)
                        {
                            ClasesSeleccionadas.Add(int.Parse(item.Value));
                        }
                    }

                    if (ClasesSeleccionadas.Count > 0)
                    {
                        foreach (int claseid in ClasesSeleccionadas)
                        {
                            RespuestaBD = BaseDatos.AsignarClasePedidoUsuario(int.Parse(ddlUsuarioAsignar.SelectedValue), claseid);       
                        }
                        if (RespuestaBD.Codigo == "OK")
                        {
                            msjesErrorAsignar.Visible = true;
                            msjesErrorAsignar.InnerText = "Clase asignada correctamente";
                            return;
                        }
                        else if (RespuestaBD.Codigo == "ERROR")
                        {
                            msjesErrorAsignar.Visible = true;
                            msjesErrorAsignar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                        }
                    }
                }
                else if(RespuestaBD.Codigo == "ERROR")
                {
                    msjesErrorAsignar.Visible = true;
                    msjesErrorAsignar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
                }
            }
        }
        catch(Exception ex)
        {
            msjesErrorAsignar.Visible = true;
            msjesErrorAsignar.InnerText = "Ha ocurrido un error en el sistemas, favor contactar a administrador";
            logger.Error("Error al asignar clase: " + ex.Message);
        }
    }

    private void CargarComboClasePedido()
    {
        ddlClasePedidoEditar.AppendDataBoundItems = true;
        ddlClasePedidoEditar.Items.Add(new ListItem("Seleccione", "0"));
        ddlClasePedidoEditar.DataSource = BaseDatos.CargaComboClasePedidoMantedor();
        ddlClasePedidoEditar.DataMember = "Table";
        ddlClasePedidoEditar.DataValueField = "ID";
        ddlClasePedidoEditar.DataTextField = "Nombre";
        ddlClasePedidoEditar.DataBind();
    }

    private void CargarComboClasePedidoActivo()
    {
        ddlClasePedidoActivar.AppendDataBoundItems = true;
        ddlClasePedidoActivar.Items.Add(new ListItem("Seleccione", "0"));
        ddlClasePedidoActivar.DataSource = BaseDatos.CargaComboClasePedidoMantedor();
        ddlClasePedidoActivar.DataMember = "Table";
        ddlClasePedidoActivar.DataValueField = "ID";
        ddlClasePedidoActivar.DataTextField = "Nombre";
        ddlClasePedidoActivar.DataBind();
    }

    private void CargarComboUsuarioAsignar()
    {
        ddlUsuarioAsignar.AppendDataBoundItems = true;
        ddlUsuarioAsignar.Items.Add(new ListItem("Seleccione", "0"));
        ddlUsuarioAsignar.DataSource = BaseDatos.CargaUsuarioAsignar();
        ddlUsuarioAsignar.DataMember = "Table";
        ddlUsuarioAsignar.DataValueField = "idPersona";
        ddlUsuarioAsignar.DataTextField = "NOMBRE";
        ddlUsuarioAsignar.DataBind();
    }

    private void CargarCheckMarcas()
    {
        try
        {
            DataTable TablaClase = BaseDatos.CargaClasePeidiosAsignar().Tables["Table"];

            string[] ArrayNombre = new string[TablaClase.Rows.Count];

            for (int i = 0; i < TablaClase.Rows.Count; i++)
            {
                ArrayNombre[i] = TablaClase.Rows[i]["Nombre"].ToString();
            }

            cbxClasePedido.DataSource = TablaClase;
            cbxClasePedido.DataValueField = "id";       // Esto será el Value
            cbxClasePedido.DataTextField = "nombre";    // Esto será el Text visible
            cbxClasePedido.DataBind();

        }
        catch (Exception ex)
        {

        }
    }

    private void LimpiarValoresModuloInsertar()
    {
        txtNombreClase.Text = "";
        TxtCodigoClase.Text = "";
    }

    private void  LimpiarValoresModuloEditar()
    {
        ddlClasePedidoEditar.Items.Clear();
        CargarComboClasePedido();
        TxtNombreClaseEditar.Text = "";
    }

    private void LimpiarValoresModuloDesactivar()
    {
        ddlClasePedidoActivar.Items.Clear();
        CargarComboClasePedidoActivo();
    }
}