using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using System.IO;

public partial class Vistas_Accesorio : System.Web.UI.Page
{
    ControlBD _controlBD;
    TreeNode lista;
    private PagedDataSource pdt;
    private DataTable rdt;
    private string query;

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Accesorio));

    protected void Page_Load(object sender, EventArgs e)
    {
        lblSesion.Text = Session["idSession"].ToString();
        msjesError.Visible = false;
        rdt = new DataTable();
        pdt = new PagedDataSource();

        query = "";
        divAccesorioFirstTime.Visible = false;
        divCatalogoRepuestos.Visible = false;
        divSinAccesorios.Visible = false;
        divConAccesorios.Visible = false;
        panelEdicionAccesorio.Visible = false;

        _controlBD = new ControlBD();

        if (int.Parse(Session["permisos"].ToString()) != 1 && int.Parse(Session["permisos"].ToString()) != 6)
        {
            btnAdminEquipados.Visible = false;
            btnEditarAccesorio.Visible = false;
        }

        // Si se entra por primera vez a la página, se muestra el carrusel
        // de imágenes
        if (!IsPostBack)
        {
            divAccesorioFirstTime.Visible = true;
            // Se muestran los ultimos vehiculos equipados
            DataTable auxDt = cargarEquipados();
            if (auxDt.Rows.Count > 0)
            {
                divConAccesorios.Visible = true;
            }
            else
            {
                divSinAccesorios.Visible = true;
            }
            try
            {
                rptrEquipados.DataSource = auxDt;
                rptrEquipados.DataBind();
            }
            catch (Exception ex)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Hubo un problema al enlazar datos. Disculpe las molestias";
                logger.Error("Problema al enlazar datos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            }
        }
        // Si no,(es decir, que no es la primera vez que se entra a esta
        // página) se muestra el catálogo
        else
        {
            divCatalogoRepuestos.Visible = true;
        }
        // Metodo que llena arbol de marcas
        LlenarTreeViewTrade();
        TreeViewAcc.SelectedNodeChanged += OnFiltrarResultadosPorMarca;
		
		
    }

    /// <summary>
    /// Método para llenar el TreeView con las marcas y los modelos correspondientes
    /// </summary>
    public void LlenarTreeViewTrade()
    {
        TreeViewAcc.Nodes.Clear();
        DataSet _dSet;
        ControlMarca ctrlMarca = new ControlMarca();

        _dSet = ctrlMarca.obtieneMarcasModelosPorNombreConcesionario(_controlBD.ObtenerConcesionario(Session["rut"].ToString()));
        if (_dSet != null)
        {
            //Se declara un TreeNode que contendrá las elementos de la lista
            TreeNode listaModel;
            String lastMarca = "";
            Boolean firstTime = true;
            lista = new TreeNode();

            foreach (DataRow campos in _dSet.Tables[0].Rows)
            {
                listaModel = new TreeNode();
                listaModel.Value = campos["nombreModelo"].ToString();
                if (firstTime)
                {
                    lista.Value = campos["nombreMarca"].ToString();
                    lastMarca = lista.Value;
                    firstTime = false;
                    lista.ChildNodes.Add(listaModel);
                    TreeViewAcc.Nodes.Add(lista);
                    continue;
                }
                if (!lastMarca.Equals(campos["nombreMarca"].ToString()))
                {
                    lista = new TreeNode();
                    lista.Value = campos["nombreMarca"].ToString();
                    lastMarca = lista.Value;
                    listaModel = new TreeNode();
                    listaModel.Value = campos["nombreModelo"].ToString();
                    lista.ChildNodes.Add(listaModel);
                    TreeViewAcc.Nodes.Add(lista);
                }
                else
                {
                    lista.ChildNodes.Add(listaModel);
                }
            }
        }
        else
        {
            msjesError.InnerText = "Problemas en la Base de Datos. Disculpe las molestias";
            msjesError.Visible = true;
            logger.Error("Problemas en la Base de Datos en [LlenarTreeViewTrade]");
        }
    }

    public void CargarAccesorios(string query, int paginaActual)
    {
        try
        {
            // RFC.Pagare es un objeto de tipo DataTable, que ya contiene los elementos a presentar
            pdt.DataSource = _controlBD.ObtenerDatosFiltrados(query).Tables[0].DefaultView;

            if (pdt.Count == 0)
            {
                msjesError.InnerText = "No hay accesorios para esa marca / modelo";
                msjesError.Visible = true;
            }

            pdt.PageSize = 6; //Número de elementos por página
            pdt.CurrentPageIndex += int.Parse(lblCurrentPage.Text) + paginaActual;
            lblCurrentPage.Text = pdt.CurrentPageIndex.ToString();
            pdt.AllowPaging = true;

            // Disable Prev or Next buttons if necessary
            cmdPrev.Enabled = !pdt.IsFirstPage;
            cmdNext.Enabled = !pdt.IsLastPage;
            lblPaginaActual.Text = (int.Parse(lblCurrentPage.Text) + 1).ToString();
            lblPaginaTotal.Text = pdt.PageCount.ToString();
            RepAcce.DataSource = pdt;
            RepAcce.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Hubo un problema al enlazar datos. Disculpe las molestias";
            logger.Error("Problema al enlazar datos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    public void OnFiltrarResultadosPorMarca(object sender, EventArgs e)
    {
        // Se almacena marca o modelo en input hidden para paginacion
        lblSelectedMarcaModelo.Text = TreeViewAcc.SelectedValue.ToString();
        lblSelectedNode.Text = TreeViewAcc.SelectedNode.ValuePath;
        lblCurrentPage.Text = "0";

        if(TreeViewAcc.SelectedNode.Depth==0)
        {
            lblSelectedMarca.Text = TreeViewAcc.SelectedValue.ToString();
        } else {
            lblSelectedMarca.Text = TreeViewAcc.SelectedNode.Parent.Value.ToString();
        }
        
        if (TreeViewAcc.SelectedNode.Depth == 0)
        {
            lblSelectedDepth.Text = "0";
            query = "SELECT * FROM ACCESORIO a " + 
					" WHERE a.idAccesorio in ( SELECT b.idAccesorio from accesorio_modelo b where b.idModelo " +
					" in (SELECT c.idModelo from modelo c WHERE c.nombreMarca = '" + TreeViewAcc.SelectedValue.ToString() + "' ))" +
					" order by a.idAccesorio desc"; 
        }
        else
        {
            lblSelectedDepth.Text = "1";
            query = "SELECT * FROM ACCESORIO a" +
			        " WHERE  a.idAccesorio in (Select b.idAccesorio from accesorio_modelo b where b.idModelo " +
                    " in ( SELECT c.idModelo from modelo c WHERE c.nombreModelo =  '" + TreeViewAcc.SelectedValue.ToString() + "')) " +					
         			" order by a.idAccesorio desc";
            TreeViewAcc.FindNode(TreeViewAcc.SelectedNode.ValuePath).Parent.Expand();
        }
        CargarAccesorios(query, pdt.CurrentPageIndex);
    }
    
    protected void EditarRegistro(object source, RepeaterCommandEventArgs e)
    {
        int idAccesorio = int.Parse(e.CommandArgument.ToString());

        if (e.CommandName == "eliminar")
        {
            try
            {
                ControlAccesorio _controlAccesorio = new ControlAccesorio();
                Accesorio accesorio = _controlAccesorio.obtenerAccesorio("" + idAccesorio);

                // Voy a buscar 
                if (_controlBD.InsertarDatos("delete from accesorio where idAccesorio = " + idAccesorio + ""))
                {
                    if (accesorio != null)
                    {
                        File.Delete(Server.MapPath("") + "\\" + accesorio.imagen);
                    }
					_controlBD.InsertarDatos("delete from accesorio_modelo where idAccesorio = " + idAccesorio + "");
                    msjesError.InnerText = "Accesorio eliminado";
                    msjesError.Visible = true;
                    e.Item.Visible = false;
                }
                else
                {
                    msjesError.InnerText = "Se produjo un error al eliminar. Disculpe las molestias";
                    msjesError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("En [EditarRegistro] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                msjesError.InnerText = "Se produjo un error al eliminar. Disculpe las molestias";
                msjesError.Visible = true;
            }
        }
        else if (e.CommandName == "editar")
        {
            // Se configuran los paneles
            divCatalogoRepuestos.Visible = false;
            divPaginacionAccesorios.Visible = false;
            panelEdicionAccesorio.Visible = true;

            // Se trae el accesorio que se está editando
            ControlAccesorio _controlAccesorio = new ControlAccesorio();
            Accesorio accesorioEditando = _controlAccesorio.obtenerAccesorio(idAccesorio.ToString());

			/*
			carga modelos
			*/
			
			string query_modelos = "select nombreMarca + '-' +  nombreModelo as nombre from modelo where nombreModelo <> 'Todas' order by nombreMarca, nombreModelo ";
			string query_modelosmarcado = "select nombreMarca + '-' +  nombreModelo as nombre from modelo where nombreModelo <> 'Todas' and idModelo in (select idModelo from accesorio_modelo where idAccesorio = "+ idAccesorio.ToString() +") order by nombreMarca, nombreModelo ";
			
			DataSet _dSet = _controlBD.ObtenerDatosFiltrados(query_modelos);
			DataSet _dSet2 = _controlBD.ObtenerDatosFiltrados(query_modelosmarcado);
			
			CheckBoxListModelos.Items.Clear();
			int indiceModelo = 0;
			int existe = 0;
			foreach (DataRow campo in _dSet.Tables[0].Rows)
				{
					existe = 0;
					ListItem ls = new ListItem();
					ls.Text = campo["nombre"].ToString();
					CheckBoxListModelos.Items.Add(ls);
					foreach (DataRow campo2 in _dSet2.Tables[0].Rows)
					{
						if (campo["nombre"].ToString() == campo2["nombre"].ToString())
						{
							existe = 1;
						}
					}
					if (existe == 1)
					{
						CheckBoxListModelos.Items[indiceModelo].Selected = true;
					}
					indiceModelo++; 
				}
				
            txtCodigoAccesorio.Text = accesorioEditando.codigo;
            txtDescripcionAccesorio.Text = accesorioEditando.descripcion;
            imgRepuesto.ImageUrl = accesorioEditando.imagen;
            lblMarcaAccesorio.Text = accesorioEditando.nombreMarca;
            lblModeloAccesorio.Text = accesorioEditando.nombreModelo;
            hddIdAccesorioEditando.Value = accesorioEditando.idAccesorio.ToString();
        }
        
    }

    protected void cmdPrev_Click(object sender, System.EventArgs e)
    {
        if (lblSelectedDepth.Text == "0")
        {
            //query = "SELECT * FROM ACCESORIO WHERE nombreMarca = '" + lblSelectedMarcaModelo.Text.ToString() + "' order by idAccesorio desc";
			 query = "SELECT * FROM ACCESORIO a " + 
					" WHERE a.idAccesorio in ( SELECT b.idAccesorio from accesorio_modelo b where b.idModelo " +
					" in (SELECT c.idModelo from modelo c WHERE c.nombreMarca = '" + lblSelectedMarcaModelo.Text.ToString() + "' ))" +
					" order by a.idAccesorio desc"; 
        }
        else
        {
            //query = "SELECT * FROM ACCESORIO WHERE nombreModelo = '" + lblSelectedMarcaModelo.Text.ToString() + "' order by idAccesorio desc";
			 query = "SELECT * FROM ACCESORIO a" +
			        " WHERE  a.idAccesorio in (Select b.idAccesorio from accesorio_modelo b where b.idModelo " +
                    " in ( SELECT c.idModelo from modelo c WHERE c.nombreModelo =  '" + lblSelectedMarcaModelo.Text.ToString() + "')) " +					
         			" order by a.idAccesorio desc";
			
            TreeViewAcc.FindNode(lblSelectedNode.Text.ToString()).Parent.Expand();
        }
        CargarAccesorios(query, -1);
    }

    protected void cmdNext_Click(object sender, System.EventArgs e)
    {
        if (lblSelectedDepth.Text == "0")
        {
           // query = "SELECT * FROM ACCESORIO WHERE nombreMarca = '" + lblSelectedMarcaModelo.Text.ToString() + "' order by idAccesorio desc";
		    query = "SELECT * FROM ACCESORIO a " + 
					" WHERE a.idAccesorio in ( SELECT b.idAccesorio from accesorio_modelo b where b.idModelo " +
					" in (SELECT c.idModelo from modelo c WHERE c.nombreMarca = '" + lblSelectedMarcaModelo.Text.ToString() + "' ))" +
					" order by a.idAccesorio desc"; 
        }
        else
        {
            //query = "SELECT * FROM ACCESORIO WHERE nombreModelo = '" + lblSelectedMarcaModelo.Text.ToString() + "' order by idAccesorio desc";
			 query = "SELECT * FROM ACCESORIO a" +
			        " WHERE  a.idAccesorio in (Select b.idAccesorio from accesorio_modelo b where b.idModelo " +
                    " in ( SELECT c.idModelo from modelo c WHERE c.nombreModelo =  '" + lblSelectedMarcaModelo.Text.ToString() + "')) " +					
         			" order by a.idAccesorio desc";
            TreeViewAcc.FindNode(lblSelectedNode.Text.ToString()).Parent.Expand();
        }
        CargarAccesorios(query,1);
    }

    private DataTable obtenerUltimasImagenes()
    {
        DataTable resultado;
        DataTableCollection resultadoColeccion =
            _controlBD.ObtenerDatosFiltrados(@"select top 5 * from contenido where tipo='199'order by idContenido desc").Tables;
        if (resultadoColeccion.Count > 0)
        {
            resultado = resultadoColeccion[0];
        }
        else
        {
            resultado = new DataTable();
        }
        return resultado;
    }

    protected void AgregarAccesorio_Click(object sender, ImageClickEventArgs e)
    {

    }

    private DataTable cargarEquipados()
    {
        DataTable resultado = null;
        try
        {
            DataTableCollection resultadoColeccion =
                _controlBD.ObtenerDatosFiltrados(@"select * from contenido where tipo='199' order by idContenido desc").Tables;
            resultado = resultadoColeccion[0];
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al enlazar los datos. Favor notificar al administrador. Disculpe las molestias";
            msjesError.Visible = true;
            logger.Error("En [cargarEquipados] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            resultado = new DataTable();
        }
        return resultado;
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        msjesError.InnerText = "Edición cancelada";
        msjesError.Visible = true;
        panelEdicionAccesorio.Visible = false;
        divCatalogoRepuestos.Visible = true;
    }

    protected void btnActualizarAccesorio_Click(object sender, EventArgs e)
    {
        String idAccesorio = hddIdAccesorioEditando.Value;
        String codigoAccesorio = txtCodigoAccesorio.Text;
        String descripcionAccesorio = txtDescripcionAccesorio.Text;
        String marcaAccesorio = lblMarcaAccesorio.Text;
        String modeloAccesorio = lblModeloAccesorio.Text;
        string modelo ="";

        if (codigoAccesorio == "")
        {
            lblValidaCodigoAcces.Visible = true;
        }
        else if (descripcionAccesorio == "")
        {
            lblValidaDescr.Visible = true;
        }
        else
        {
            if (fileImagenAccesorio.HasFile)
            {
                string nombreArchivo = "";

                // Get the name of the file to upload.
                string fileName = Server.HtmlEncode(fileImagenAccesorio.FileName);

                // Get the extension of the uploaded file.
                string extension = System.IO.Path.GetExtension(fileName);
                extension = extension.ToLower();

                // se restringen los formatos
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                {
                    nombreArchivo = marcaAccesorio.Trim() + modeloAccesorio.Trim() + codigoAccesorio + extension;
                    File.Delete(Server.MapPath("~/doc/contenidoAccesorios/") + nombreArchivo);

                    try
                    {
                        fileImagenAccesorio.SaveAs(Server.MapPath("~/doc/contenidoAccesorios/") + nombreArchivo);
                        string queryInsertar = @"UPDATE accesorio set nombreMarca='"+ marcaAccesorio + "' ,nombreModelo = '" +modeloAccesorio + "',codigo = '"+codigoAccesorio +"',descripcion = '" + descripcionAccesorio+ "',imagen = '../doc/contenidoAccesorios/"+ nombreArchivo + "' where idAccesorio = "+idAccesorio.ToString();
                        _controlBD.InsertarDatos(queryInsertar);
						_controlBD.InsertarDatos("delete from accesorio_modelo where idAccesorio = " + idAccesorio.ToString() + "");
						
						for (int i = 0; i < CheckBoxListModelos.Items.Count; i++)
						{
								if (CheckBoxListModelos.Items[i].Selected)
								{
									string _modelo = CheckBoxListModelos.Items[i].Text;
									string[] exploded = _modelo.Split('-');
									int _idmodelo = _controlBD.getidModeloByModelo(exploded[1]);
									_controlBD.InsertarDatos("insert into accesorio_modelo values(" + idAccesorio.ToString() + "," + _idmodelo + ")");
								}
						}
						
						
						
						
                        msjesError.InnerText = "El accesorio de codigo " + codigoAccesorio +  " fue actualizado correctamente";
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
                string queryInsertar = @"UPDATE accesorio set nombreMarca='" + marcaAccesorio + "' ,nombreModelo = '" + modeloAccesorio + "',codigo = '" + codigoAccesorio + "',descripcion = '" + descripcionAccesorio + "' where idAccesorio = " + idAccesorio.ToString();
                _controlBD.InsertarDatos(queryInsertar);
				
				_controlBD.InsertarDatos("delete from accesorio_modelo where idAccesorio = " + idAccesorio.ToString() + "");
				for (int i = 0; i < CheckBoxListModelos.Items.Count; i++)
						{
								if (CheckBoxListModelos.Items[i].Selected)
								{
									string _modelo = CheckBoxListModelos.Items[i].Text;
									string[] exploded = _modelo.Split('-');

                                    if (exploded.Length > 2)
                                    {
                                        modelo = "";

                                        for (int j = 1; j < exploded.Length; j++)
                                        {
                                            modelo = modelo + exploded[j] + "-"; 
                                        }

                                        modelo=modelo.Substring(0, modelo.Length - 1);

                                    }
                                    else {
                                        modelo = exploded[1];
                                    }

                                    int _idmodelo = _controlBD.getidModeloByModelo(modelo);
									_controlBD.InsertarDatos("insert into accesorio_modelo values(" + idAccesorio.ToString() + "," + _idmodelo + ")");
								}
						}
				
				
                msjesError.InnerText = "El accesorio de codigo " + codigoAccesorio+ " fue actualizado correctamente";
                msjesError.Visible = true;
            }

            panelEdicionAccesorio.Visible = false;
            divCatalogoRepuestos.Visible = true;
            divPaginacionAccesorios.Visible = true;
            
            String query = "SELECT * FROM ACCESORIO WHERE nombreModelo = '" + modeloAccesorio + "' order by idAccesorio desc";
            CargarAccesorios(query,0);
        }
    }
}