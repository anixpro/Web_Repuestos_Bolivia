using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using log4net;
using log4net.Config;

public partial class Vistas_EditorAccesorio : System.Web.UI.Page
{
    ControlBD _controlBD;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_EditorAccesorio));
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _controlBD = new ControlBD();

        lblValidaCodigo.Visible = false;
        lblValidaDescrip.Visible = false;
		
		string query = "select nombreMarca + '-' +  nombreModelo as nombre from modelo where nombreModelo <> 'Todas' order by nombreMarca, nombreModelo ";

        DataSet _dSet = _controlBD.ObtenerDatosFiltrados(query);

        foreach (DataRow campo in _dSet.Tables[0].Rows)
            {
                ListItem ls = new ListItem();
                ls.Text = campo["nombre"].ToString();
                CheckBoxListModelos.Items.Add(ls);
            }
    }

    private void InsertarContenido(string rutaImg)
    {
		string models = "";
        string codigo = txtCodigo.Text;
        string descripcion = txtDescripcion.Text;
        string queryInsertar = @"INSERT INTO accesorio(fecha,codigo,descripcion,imagen)
                                VALUES(GETDATE() , '" + codigo + "' , '" + descripcion + "' , '" + rutaImg + "')";
        _controlBD.InsertarDatos(queryInsertar);
		int idAccesorio = _controlBD.getAccesorioUltimoIndice();
		
		
		for (int i = 0; i < CheckBoxListModelos.Items.Count; i++)
            {
                if (CheckBoxListModelos.Items[i].Selected)
                {
                    string _modelo = CheckBoxListModelos.Items[i].Text;
					string[] exploded = _modelo.Split('-');
					int _idmodelo = _controlBD.getidModeloByModelo(exploded[1]);
                    _controlBD.InsertarDatos("insert into accesorio_modelo values(" + idAccesorio + "," + _idmodelo + ")");
                }
        }
		
		txtCodigo.Text = "";
		txtDescripcion.Text = "";
        msjesError.InnerText = "Accesorio insertado exitosamente";
        msjesError.Visible = true;
    }

    protected void btnAgregaAccesorio_Click(object sender, EventArgs e)
    {		
        if (txtCodigo.Text == "")
        {
            lblValidaCodigo.Visible = true;
        }
        else if (txtDescripcion.Text == "")
        {
            lblValidaDescrip.Visible = true;
        }
        else
        {
            if (subirImagen.HasFile)
            {
                ControlAccesorio _controlAccesorio = new ControlAccesorio();
                /*if(_controlAccesorio.siExsite(txtCodigo.Text,cbxMarcas.SelectedItem.ToString()))
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Ya existe un código de accesorio para esa marca/modelo";
                    return;
                }*/

                string nombreArchivo = "";

                // Get the name of the file to upload.
                string fileName = Server.HtmlEncode(subirImagen.FileName);

                // Get the extension of the uploaded file.
                string extension = System.IO.Path.GetExtension(fileName);
                extension = extension.ToLower();

                // se restringen los formatos
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                {
                    nombreArchivo = txtCodigo.Text + extension;
                    if (File.Exists(Server.MapPath("~/doc/contenidoAccesorios/") + nombreArchivo))
                    {
                        msjesError.InnerText = "El Accesorio ya existe";
                        msjesError.Visible = true;
                        return;
                    }
                    try
                    {
                        subirImagen.SaveAs(Server.MapPath("~/doc/contenidoAccesorios/") + nombreArchivo);
                        InsertarContenido("../doc/contenidoAccesorios/" + nombreArchivo);
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
                msjesError.InnerText = "No hay imágen para cargar, o es inválida";
                msjesError.Visible = true;
                logger.Warn("En [btnAgregaAccesorio_Click] imagen inexistente o inválida");
            }
        }
    }

    protected void btnRestaurar_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditorAccesorio.aspx", false);
    }
}