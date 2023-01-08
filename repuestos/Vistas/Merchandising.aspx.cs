using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using log4net;
using log4net.Config;
using System.IO;

public partial class Vistas_Merchandising : System.Web.UI.Page
{
    ControlBD _controlBD;
    DataTable rdt;
    PagedDataSource pdt;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Merchandising));

    protected void Page_Load(object sender, EventArgs e)
    {
        panelEditaMerchandising.Visible = false;
        msjesError.Visible = false;

        // Inicializacion de objetos
        _controlBD = new ControlBD();
        rdt = new DataTable();
        pdt = new PagedDataSource();

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
    }

    public void CargarMerchandising(int paginaActual)
    {
        cmdPrev.Visible = true;
        cmdNext.Visible = true;
        lblPaginaActual.Visible = true;
        lblPaginaTotal.Visible = true;
        lblSlash.Visible = true;
        panelBienvenida.Visible = false;
        String marca = lblSelectedMarcaModelo.Text.ToString();
        String query = "SELECT * FROM MERCHANDISING WHERE nombreMarca = '" + marca + "' order by idMerchandising desc";
        try
        {
            // RFC.Pagare es un objeto de tipo DataTable, que ya contiene los elementos a presentar
            pdt.DataSource = _controlBD.ObtenerDatosFiltrados(query).Tables[0].DefaultView;

            if (pdt.Count == 0)
            {
                msjesError.InnerText = "Aun no hay merchandising para " + marca;
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
        CargarMerchandising(-1);
    }

    protected void cmdNext_Click(object sender, System.EventArgs e)
    {
        CargarMerchandising(1);
    }

    public void OnFiltrarResultadosPorMarca(object sender, EventArgs e)
    {
        // Se almacena marca o modelo en input hidden para paginacion
        lblSelectedMarcaModelo.Text = TreeViewAcc.SelectedValue.ToString();
        lblCurrentPage.Text = "0";
        CargarMerchandising(pdt.CurrentPageIndex);
    }

    protected void EditarRegistro(object source, RepeaterCommandEventArgs e)
    {
        int idMerchandising = int.Parse(e.CommandArgument.ToString());
        if (e.CommandName == "eliminar")
        {
            if (_controlBD.InsertarDatos("delete from merchandising where idMerchandising = " + idMerchandising + ""))
            {
                msjesError.InnerText = "Merchandising eliminado";
                msjesError.Visible = true;
                e.Item.Visible = false;
            }
            else
            {
                msjesError.InnerText = "Se produjo un error al eliminar. Disculpe las molestias";
                msjesError.Visible = true;
            }
        }
        else if (e.CommandName == "editar")
        {
            ControlMerchandising _controlMerchandising = new ControlMerchandising();
            Merchandising merchandisingActual = _controlMerchandising.obtenerMerchandising(idMerchandising.ToString());
            if (merchandisingActual == null)
            {
                msjesError.InnerText = "Hubo problemas al rescatar el merchandising, contacte al administrador";
                msjesError.Visible = true;
                logger.Error("Error al rescatar merchandising...revisa el stack");
                return;
            }
            panelEditaMerchandising.Visible = true;
            divCatalogoRepuestos.Visible = false;
            hddIdMerchandising.Value = merchandisingActual.idMerchandising.ToString();
            hddMarca.Value = merchandisingActual.nombreMarca;
            imgImagen.ImageUrl = merchandisingActual.imagen;
            txtEditaCodigo.Text = merchandisingActual.codigo;
            txtEditaDescripcion.Text = merchandisingActual.descripcion;
        }else   {
            if (_controlBD.InsertarDatos("delete from merchandising where idMerchandising = " + idMerchandising + ""))
            {
                msjesError.InnerText = "Merchandising eliminado";
                msjesError.Visible = true;
                e.Item.Visible = false;
            }
            else
            {
                msjesError.InnerText = "Se produjo un error al eliminar. Disculpe las molestias";
                msjesError.Visible = true;
            }
        }
    }

    protected void btnEditarMerchandisingo_Click(object sender, EventArgs e)
    {
        String idMerchandising = hddIdMerchandising.Value;
        String codigoMerchandising = txtEditaCodigo.Text;
        String descripcionMerchandising = txtEditaDescripcion.Text;
        String marcaMerchandising = hddMarca.Value;
        String modeloMerchandising = hddModelo.Value;

        if (codigoMerchandising == "")
        {
            lblCodigo.Visible = true;
        }

        else if (descripcionMerchandising == "")
        {
            lblDescripcion.Visible = true;
        }

        else
        {
            if (fileImagenMerchandising.HasFile)
            {
                string nombreArchivo = "";

                // Get the name of the file to upload.
                string fileName = Server.HtmlEncode(fileImagenMerchandising.FileName);

                // Get the extension of the uploaded file.
                string extension = System.IO.Path.GetExtension(fileName);
                extension = extension.ToLower();

                // se restringen los formatos
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                {
                    nombreArchivo = marcaMerchandising.Trim() + codigoMerchandising + extension;
                    File.Delete(Server.MapPath("~/doc/contenidoMerchandising/") + nombreArchivo);

                    try
                    {
                        fileImagenMerchandising.SaveAs(Server.MapPath("~/doc/contenidoMerchandising/") + nombreArchivo);
                        string queryInsertar = @"UPDATE merchandising set nombreMarca='" + marcaMerchandising + "' , codigo = '" + codigoMerchandising + "',descripcion = '" + descripcionMerchandising + "',imagen = '../doc/contenidoMerchandising/" + nombreArchivo + "' where idMerchandising = " + idMerchandising.ToString();
                        _controlBD.InsertarDatos(queryInsertar);
                        msjesError.InnerText = "El merchandising de codigo " + codigoMerchandising + " de la marca " + marcaMerchandising + " fue actualizado correctamente";
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
                string queryInsertar = @"UPDATE merchandising set nombreMarca='" + marcaMerchandising + "' ,codigo = '" + codigoMerchandising + "',descripcion = '" + descripcionMerchandising + "' where idMerchandising = " + idMerchandising.ToString();
                _controlBD.InsertarDatos(queryInsertar);
                msjesError.InnerText = "El merchandising de codigo " + codigoMerchandising+ " de la marca " + marcaMerchandising + " fue actualizado correctamente";
                msjesError.Visible = true;
            }

            panelEditaMerchandising.Visible = false;
            divCatalogoRepuestos.Visible = true;
            CargarMerchandising(0);
        }
    }

    protected void btnCancelarEditarMerchandisingo_Click(object sender, EventArgs e)
    {
        msjesError.InnerText = "Edición cancelada";
        msjesError.Visible = true;
        panelEditaMerchandising.Visible = false;
        divCatalogoRepuestos.Visible = true;
    }
}
