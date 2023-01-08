using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;
using System.Data;
using System.IO;

public partial class Vistas_BancoRepuestos : System.Web.UI.Page
{
    ControlBD _controlBD;
    DataTable rdt;
    PagedDataSource pdt;
    DataTable dtMisReptos;
    PagedDataSource pdtMisReptos;
    ControlRepuestos _controlRepuestos = new ControlRepuestos();

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_BancoRepuestos));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        panelBienvenida.Visible = false;
        lblNombreConcesionario.Text = Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());

        // Inicializacion de objetos
        _controlBD = new ControlBD();
        rdt = new DataTable();
        pdt = new PagedDataSource();
        pdtMisReptos = new PagedDataSource();
        dtMisReptos = new DataTable();

        // Event handlers
        TreeViewAcc.SelectedNodeChanged += OnFiltrarResultadosPorMarca;

        // Se llena el arbol por primera
        LlenarTreeViewTrade();

        // Se ocultan los botones en la página de bienvenida
        cmdPrev.Visible = false;
        cmdNext.Visible = false;
        lblPaginaActual.Visible = false;
        lblPaginaTotal.Visible = false;
        lblSlash.Visible = false;
        panelFiltraCodigo.Visible = false;

        if (!IsPostBack)
        {
            panelBienvenida.Visible = true;
            PanelAgregarRepuesto.Visible = false;
            panelEditaMerchandising.Visible = false;
            ddlMarcas.Items.Add(new ListItem("Seleccionar","-1"));
            ddlModelo.Items.Add(new ListItem("Seleccionar","-1"));
            ddlMisMarcas.Items.Add(new ListItem("Todas", ""));

            ControlMarca _controlMarca = new ControlMarca();

            List<Marca> marcas = _controlMarca.obtenerMarcas();
            foreach (Marca marca in marcas)
            {
                ddlMarcas.Items.Add(new ListItem(marca.NomMarca, marca.NomMarca));
                ddlMisMarcas.Items.Add(new ListItem(marca.NomMarca, marca.NomMarca));
            }
            /*
            ControlAuto controlAuto = new ControlAuto();
            String[] misMarcas = controlAuto.marcasPorConcesionario(ddlEvolucionConcesionario.SelectedItem.Text);
             * */
        }
    }

    public void CargarRepuestos(int paginaActual)
    {
        cmdPrev.Visible = true;
        cmdNext.Visible = true;
        lblPaginaActual.Visible = true;
        lblPaginaTotal.Visible = true;
        lblSlash.Visible = true;
        panelBienvenida.Visible = false;
        panelFiltraCodigo.Visible = true;

        String marca = lblSelectedMarcaModelo.Text.ToString();
        String query = "SELECT * FROM REPUESTOS WHERE marca = '" + marca + "' and concesionario != '" + Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString()) + "' order by id desc";
        try
        {
            // RFC.Pagare es un objeto de tipo DataTable, que ya contiene los elementos a presentar
            pdt.DataSource = _controlBD.ObtenerDatosFiltrados(query).Tables[0].DefaultView;

            if (pdt.Count == 0)
            {
                msjesError.InnerText = "Aun no hay repuestos para " + marca;
                msjesError.Visible = true;
            }

            //Número de elementos por página
            pdt.PageSize = 6;
            pdt.CurrentPageIndex += int.Parse(lblCurrentPage.Text) + paginaActual;
            lblCurrentPage.Text = pdt.CurrentPageIndex.ToString();
            pdt.AllowPaging = true;

            // Disable Prev or Next buttons if necessary
            cmdPrev.Enabled = !pdt.IsFirstPage;
            cmdNext.Enabled = !pdt.IsLastPage;
            lblPaginaActual.Text = (int.Parse(lblCurrentPage.Text) + 1).ToString();
            lblPaginaTotal.Text = pdt.PageCount.ToString();
            lblSlash.Visible = true;
            RepMerch.DataSource = pdt;
            RepMerch.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Hubo un problema al enlazar datos. Disculpe las molestias";
            logger.Error("Problema al enlazar datos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". La query que falló: " + query);
        }
    }

    // Se llena el arbol por primera vez
    public void LlenarTreeViewTrade()
    {
        TreeViewAcc.Nodes.Clear();
        DataSet _dSet;
        ControlMarca ctrlMarca = new ControlMarca();

        _dSet = ctrlMarca.obtieneMarcasPorNombreConcesionario(_controlBD.ObtenerConcesionario(Session["rut"].ToString()));
        if (_dSet != null)
        {
            //Se declara un TreeNode que contendrá las elementos de la lista
            TreeNode lista;
            lista = new TreeNode();
            foreach (DataRow campos in _dSet.Tables[0].Rows)
            {
                lista = new TreeNode();
                lista.Value = campos["nombreMarca"].ToString();
                TreeViewAcc.Nodes.Add(lista);
            }
        }
        else
        {
            msjesError.InnerText = "Problemas en la Base de Datos. Disculpe las molestias";
            msjesError.Visible = true;
            logger.Error("Problemas en la Base de Datos en [LlenarTreeViewTrade]");
        }
    }

    protected void cmdPrev_Click(object sender, System.EventArgs e)
    {
        CargarRepuestos(-1);
    }

    protected void cmdNext_Click(object sender, System.EventArgs e)
    {
        CargarRepuestos(1);
    }

    public void OnFiltrarResultadosPorMarca(object sender, EventArgs e)
    {
        panelEditaMerchandising.Visible = false;
        PanelAgregarRepuesto.Visible = false;
        divCatalogoRepuestos.Visible = true;
        panelMisRepuestos.Visible = false;
        panelFiltraCodigo.Visible = true;

        // Se almacena marca o modelo en input hidden para paginacion
        lblSelectedMarcaModelo.Text = TreeViewAcc.SelectedValue.ToString();
        lblCurrentPage.Text = "0";
        CargarRepuestos(pdt.CurrentPageIndex);
    }

    protected void EditarRegistro(object source, RepeaterCommandEventArgs e)
    {
        int idRepuesto = int.Parse(e.CommandArgument.ToString());
        if (e.CommandName == "eliminar")
        {
            ControlRepuestos _controlRepuestos = new ControlRepuestos();
            if (_controlRepuestos.eliminaRepto(idRepuesto.ToString()))
            {
                msjesError.InnerText = "Repuesto eliminado";
                msjesError.Visible = true;
                e.Item.Visible = false;
            }
            else
            {
                msjesError.InnerText = "Error al eliminar repuesto, contacte a soporte";
                msjesError.Visible = true;
            }
        }
        else if (e.CommandName == "editar")
        {
            ControlRepuestos _controlRepuestos = new ControlRepuestos();
            Repuesto repuestoActual = _controlRepuestos.obtenerRepuesto(idRepuesto.ToString());

            if (repuestoActual == null)
            {
                msjesError.InnerText = "Hubo problemas al rescatar los repuestos, Disculpe las molestias. Favor, contacte al administrador";
                msjesError.Visible = true;
                logger.Error("Error al rescatar repuesto...revisa el log más arriba!");
                return;
            }
            panelEditaMerchandising.Visible = true;
            divCatalogoRepuestos.Visible = false;
            panelMisRepuestos.Visible = false;
            hddIdRepto.Value = repuestoActual.IdRepuesto.ToString();
            lblMarcaModelo.Text = "Editando repuesto de " + repuestoActual.Marca + " " + repuestoActual.Modelo; 
            imgImagen.ImageUrl = repuestoActual.Imagen;
            txtEditaCodigo.Text = repuestoActual.CodigoRepto;
            txtEditaDescripcion.Text = repuestoActual.Descripcion;
            txtEditaContacto.Text = repuestoActual.Contacto;
            txtEditaCorreo.Text = repuestoActual.Correo;
            txtEditaTelefono.Text = repuestoActual.Telefono;
            chkEditaReptoEsNuevo.Checked = repuestoActual.esNuevo;
            txtEditaCodigoArea.Text = repuestoActual.CodigoArea;
        }
    }

    protected void btnEditarMerchandisingo_Click(object sender, EventArgs e)
    {
        String idRepto = hddIdRepto.Value;
        String codigoRepto = txtEditaCodigo.Text;
        String descripcionRepto = txtEditaDescripcion.Text;
        String contacto = txtEditaContacto.Text;
        String telefono = txtEditaTelefono.Text;
        String codigoArea = txtEditaCodigoArea.Text;
        String correo = txtEditaCorreo.Text;
        int esNuevo = chkEditaReptoEsNuevo.Checked ? 1 : 0;

        // Se valida que se hayan ingresado datos
        // Validación codigo area telefono
        int x;
        if (!int.TryParse(txtEditaCodigoArea.Text, out x) && txtEditaCodigoArea.Text != "")
        {
            msjesError.InnerText = "Ingrese un código de área válido";
            msjesError.Visible = true;
            return;
        }

        // Validacion telefono
        if (!int.TryParse(txtTelefono.Text, out x) && txtTelefono.Text != "")
        {
            msjesError.InnerText = "Ingrese un teléfono válido";
            msjesError.Visible = true;
            return;
        }

        // Validación contacto
        if (txtEditaContacto.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar un detalle de cómo contactarle en el campo <i>Contacto</i>";
            msjesError.Visible = true;
            return;
        }

        // Validación del código
        if (txtEditaCodigo.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar un código";
            msjesError.Visible = true;
            return;
        }

        // Validación de la descripcion
        if (txtEditaDescripcion.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar una descripción del repuesto a ofrecer";
            msjesError.Visible = true;
            return;
        }

        // Validación correo
        if (txtEditaCorreo.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar un correo para contactarle";
            msjesError.Visible = true;
            return;
        }
        
        if (subeArchivo.HasFile)
        {
            string nombreArchivo = "";

            // Get the name of the file to upload.
            string fileName = Server.HtmlEncode(subeArchivo.FileName);

            // Get the extension of the uploaded file.
            string extension = System.IO.Path.GetExtension(fileName);
            extension = extension.ToLower();

            // se restringen los formatos
            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
            {
                nombreArchivo = idRepto + extension;
                File.Delete(Server.MapPath("~/doc/contenidoRepuestos/") + nombreArchivo);

                try
                {
                    subeArchivo.SaveAs(Server.MapPath("~/doc/contenidoRepuestos/") + nombreArchivo);
                    string queryInsertar = @"UPDATE repuestos set codigo = '" + codigoRepto + "',descripcion = '" + descripcionRepto + "',imagen = '../doc/contenidoRepuestos/" + nombreArchivo + "',contacto='" + contacto + "', correo='"+correo+"', telefono='"+telefono+"', codigoArea='"+codigoArea+"', esNuevo=" + esNuevo + " where id = " + idRepto.ToString();
                    _controlBD.InsertarDatos(queryInsertar);
                    msjesError.InnerText = "El Repuesto de codigo " + codigoRepto + " fue actualizado correctamente";
                    msjesError.Visible = true;
                }
                catch (Exception ex)
                {
                    msjesError.InnerText = "Error al subir la imágen, porfavor contacte al administrador del sitio";
                    msjesError.Visible = true;
                    logger.Error("En [btnAgregaAccesorio_Click] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                }
            }
            else
            {
                msjesError.InnerText = "La imágen debe ser de extensión jpg, jpeg, png, gif o bmp";
                msjesError.Visible = true;
            }
        }
        else
        {
            string queryInsertar = @"UPDATE Repuestos set codigo = '" + codigoRepto + "',descripcion = '" + descripcionRepto + "', contacto='"+contacto+"', correo='"+correo+"', telefono='"+telefono+"', codigoArea='"+codigoArea+"', esNuevo=" + esNuevo + " where id = " + idRepto.ToString();
            _controlBD.InsertarDatos(queryInsertar);
            msjesError.InnerText = "El repuesto de codigo " + codigoRepto + " fue actualizado correctamente";
            msjesError.Visible = true;
        }

        panelEditaMerchandising.Visible = false;
        panelMisRepuestos.Visible = true;
        CargarMisRepuestos();
    }

    protected void btnCancelarEditarMerchandisingo_Click(object sender, EventArgs e)
    {
        msjesError.InnerText = "Edición cancelada";
        msjesError.Visible = true;
        panelEditaMerchandising.Visible = false;
        divCatalogoRepuestos.Visible = true;
    }

    protected String determinaNuevo(String nuevo)
    {
        if (nuevo == "True")
        {
            return ("El repuesto es nuevo");
        }
        else if (nuevo == "False")
        {
            return ("El repuesto es usado");
        }
        else
        {
            return ("No se sabe");
        }
    }

    protected void btnAceptarAgregar_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        
        // Validaciones
        if (txtContacto.Text == "" || ddlMarcas.SelectedValue == "-1")
        {
            msjesError.InnerText = "Debe llenar los campos obligatorios (con *)";
            msjesError.Visible = true;
            return;
        }

        int x;
        if (!int.TryParse(txtTelefono.Text, out x) && txtTelefono.Text!="")
        {
            msjesError.InnerText = "Ingrese teléfono válido";
            msjesError.Visible = true;
            return;
        }
        if (!int.TryParse(txtCodigoArea.Text, out x) && txtCodigoArea.Text!="")
        {
            msjesError.InnerText = "Ingrese un código de área válido";
            msjesError.Visible = true;
            return;
        }

        if (txtCodigoRepto.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar un código";
            msjesError.Visible = true;
            return;
        }

        if (txtDescripcion.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar una descripción del repuesto a ofrecer";
            msjesError.Visible = true;
            return;
        }

        if (txtCorreo.Text == "")
        {
            msjesError.InnerHtml = "Porfavor, ingresar un correo para contactarle";
            msjesError.Visible = true;
            return;
        }

        String nombreArchivo = "";
        String marca = ddlMarcas.SelectedValue;
        String modelo = ddlModelo.SelectedValue;
        String concesionario = Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());
        String contacto = txtContacto.Text;
        String descripcion = txtDescripcion.Text;
        String codigo = txtCodigoRepto.Text;
        String codigoArea = txtCodigoArea.Text;
        String telefono = txtTelefono.Text;
        String correo = txtCorreo.Text;
        Boolean esNuevo = chkbxEsNuevo.Checked;
        ControlRepuestos _controlRepuestos = new ControlRepuestos();
        long idResultado = 0;

        // La imagen se guarda despues de insertar el registro
        try
        {
            idResultado = _controlRepuestos.grabarRepuesto(nombreArchivo, marca, modelo, concesionario, contacto, descripcion, codigo, esNuevo, telefono,codigoArea,correo);
            msjesError.InnerText = "Repuesto ingresado";
            msjesError.Visible = true;
        }
        catch (Exception ex)
        {
            logger.Error("En [btnAceptarAgregar_Click] " + ex.Message + ", " + ex.StackTrace + ", " + ex.Source);
        }

        if (idResultado == 0)
        {
            logger.Error("El ingreso retornó 0");
        }

        // Get the name of the file to upload.
        string fileName = Server.HtmlEncode(fileImagenRepto.FileName);

        // Get the extension of the uploaded file.
        string extension = System.IO.Path.GetExtension(fileName);
        extension = extension.ToLower();

        if (fileImagenRepto.HasFile)
        {
            // se restringen los formatos
            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
            {
                nombreArchivo = idResultado + extension;

                try
                {
                    fileImagenRepto.SaveAs(Server.MapPath("~/doc/contenidoRepuestos/") + nombreArchivo);
                    if (!_controlRepuestos.actualizaImagen("../doc/contenidoRepuestos/" + nombreArchivo, idResultado.ToString()))
                    {
                        logger.Error("En [btnAgregaAccesorio_Click] no pudo subir imagen, imagen : ../doc/contenidoRepuestos/" + nombreArchivo + ", id : " + idResultado.ToString());
                        msjesError.InnerText = msjesError.InnerText + "<br/>Advertencia: Hubo problemas con la imágen. Intente con otra o avise al administrador, gracias";
                        msjesError.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    msjesError.InnerText = "Error al subir la imágen, porfavor contacte al administrador del sitio";
                    msjesError.Visible = true;
                    logger.Error("En [btnAgregaAccesorio_Click] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                }
            }
            else
            {
                msjesError.InnerText = "La imágen debe ser de extensión jpg, jpeg, png, gif o bmp";
                msjesError.Visible = true;
            }
        }
        else
        {
            // Nada que hacer
        }
    }

    protected void btnCancelarCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect("BancoRepuestos.aspx",true);
    }

    protected void ddlMarcas_SelectedIndexChanged(object sender, EventArgs e)
    {
        llenarModelos();
    }

    protected void llenarModelos()
    {
        ddlModelo.Enabled = true;
        ddlModelo.Items.Clear();
        string query = "select nombreModelo from modelo where nombreMarca = '" + ddlMarcas.SelectedValue+"'";

        DataSet _dSet = _controlBD.ObtenerDatosFiltrados(query);

        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            ddlModelo.Items.Add(new ListItem(campos["nombreModelo"].ToString(),campos["nombreModelo"].ToString()));
        }
        ddlModelo.Items.Add(new ListItem("Otro","Otro"));
    }

    protected void agregarRepuesto_Click(object sender, EventArgs e)
    {
        panelEditaMerchandising.Visible = false;
        PanelAgregarRepuesto.Visible = true;
        divCatalogoRepuestos.Visible = false;
        panelMisRepuestos.Visible = false;
    }

    protected void verMisRepuestos_Click(object sender, EventArgs e)
    {
        CargarMisRepuestos();
    }

    protected void CargarMisRepuestos()
    {
        lblMensajeMisReptos.Text = "Estos son los repuestos del concesionario: " + Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());
        PanelAgregarRepuesto.Visible = false;
        panelEditaMerchandising.Visible = false;
        panelMisRepuestos.Visible = true;
        divCatalogoRepuestos.Visible = false;

        String query = "SELECT * FROM REPUESTOS WHERE concesionario = '" + Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString()) +"'";

        if (ddlMisMarcas.SelectedValue != "" && ddlMisMarcas.SelectedValue != "Todas")
        {
            query = query + " and marca = '"+ddlMisMarcas.SelectedValue+"'";
        }
        if (ddlMisModelos.SelectedValue != "" && ddlMisModelos.SelectedValue != "Todas")
        {
            query = query + " and modelo = '" + ddlMisModelos.SelectedValue + "'";
        }

        query = query + " order by id desc";

        try
        {
            // RFC.Pagare es un objeto de tipo DataTable, que ya contiene los elementos a presentar
            pdtMisReptos.DataSource = _controlBD.ObtenerDatosFiltrados(query).Tables[0].DefaultView;

            if (pdtMisReptos.Count == 0)
            {
                msjesError.InnerText = "Aun no hay repuestos subidos por  " + Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());
                msjesError.Visible = true;
                return;
            }

            //Número de elementos por página
            //pdtMisReptos.PageSize = 10;
            //pdt.CurrentPageIndex += int.Parse(lblCurrentPage.Text) + paginaActual;
            //lblCurrentPage.Text = pdt.CurrentPageIndex.ToString();
            pdtMisReptos.AllowPaging = false;

            // Disable Prev or Next buttons if necessary
            /*cmdPrev.Enabled = !pdt.IsFirstPage;
            cmdNext.Enabled = !pdt.IsLastPage;
            lblPaginaActual.Text = (int.Parse(lblCurrentPage.Text) + 1).ToString();
            lblPaginaTotal.Text = pdt.PageCount.ToString();
            lblSlash.Visible = true;*/
            RepeaterMisReptos.DataSource = pdtMisReptos;
            RepeaterMisReptos.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Hubo un problema al enlazar datos. Disculpe las molestias";
            logger.Error("Problema al enlazar datos de 'mis repuestos'. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". La query que falló: " + query);
        }
    }

    protected void ddlMisMarcas_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlMisModelos.Enabled = true;
        string query = "select nombreModelo from modelo where nombreMarca = '" + ddlMisMarcas.SelectedValue + "'";
        DataSet _dSet = _controlBD.ObtenerDatosFiltrados(query);

        ddlMisModelos.Items.Clear();
        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            ddlMisModelos.Items.Add(new ListItem(campos["nombreModelo"].ToString(), campos["nombreModelo"].ToString()));
        }
        ddlMisModelos.Items.Add(new ListItem("Otra", ""));
    }

    protected void btnFiltrarCodRepuesto_Click(object sender, EventArgs e)
    {
        if(txtCodRepuesto.Text=="")
        {
            CargarRepuestos(pdt.CurrentPageIndex);
        }
        else{
            CargarRepuestosPorCodRepuesto(pdt.CurrentPageIndex, txtCodRepuesto.Text);
        }
    }

    public void CargarRepuestosPorCodRepuesto(int paginaActual, string codRepuesto)
    {
        cmdPrev.Visible = true;
        cmdNext.Visible = true;
        lblPaginaActual.Visible = true;
        lblPaginaTotal.Visible = true;
        lblSlash.Visible = true;
        panelBienvenida.Visible = false;
        panelFiltraCodigo.Visible = true;

        String marca = lblSelectedMarcaModelo.Text.ToString();
        String query = "SELECT * FROM REPUESTOS WHERE marca = '" + marca + "' and concesionario != '" + Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString()) + "' and codigo = '" + codRepuesto + "' order by id desc";
        try
        {
            // RFC.Pagare es un objeto de tipo DataTable, que ya contiene los elementos a presentar
            pdt.DataSource = _controlBD.ObtenerDatosFiltrados(query).Tables[0].DefaultView;

            if (pdt.Count == 0)
            {
                msjesError.InnerText = "Aun no hay repuestos para " + marca;
                msjesError.Visible = true;
            }

            //Número de elementos por página
            pdt.PageSize = 6;
            pdt.CurrentPageIndex += int.Parse(lblCurrentPage.Text) + paginaActual;
            lblCurrentPage.Text = pdt.CurrentPageIndex.ToString();
            pdt.AllowPaging = true;

            // Disable Prev or Next buttons if necessary
            cmdPrev.Enabled = !pdt.IsFirstPage;
            cmdNext.Enabled = !pdt.IsLastPage;
            lblPaginaActual.Text = (int.Parse(lblCurrentPage.Text) + 1).ToString();
            lblPaginaTotal.Text = pdt.PageCount.ToString();
            lblSlash.Visible = true;
            RepMerch.DataSource = pdt;
            RepMerch.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Hubo un problema al enlazar datos. Disculpe las molestias";
            logger.Error("Problema al enlazar datos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". La query que falló: " + query);
        }
    }
}
