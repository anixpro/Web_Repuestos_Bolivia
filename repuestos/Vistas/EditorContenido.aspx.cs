using System;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using log4net;
using log4net.Config;

public partial class Vistas_EditorContenido : System.Web.UI.Page
{
    ControlBD _controlBD;

    private String tipo;
    private String id;
    string _marca;
    string idActual;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_EditorContenido));

    protected void Page_Init(object sender, EventArgs e)
    {
        if (tipo != "8" || tipo != "7" || tipo != "3" || tipo != "4" || tipo != "9" || tipo != "11")
        {
            _controlBD = new ControlBD();
            DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from marca");
            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                ListItem ls = new ListItem();
                ls.Text = campo["nombreMarca"].ToString();
                CheckBoxListMarcas.Items.Add(ls);
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager1.RegisterPostBackControl(btnCopy);
        ScriptManager1.RegisterPostBackControl(btnAgregar);
        /*
        if (Request.Browser.Browser != "IE")
        {
            ScriptManager1.RegisterPostBackControl(btnAgregar);
        }
        */
        panelVistaPrevia.Visible = false;
        msjesError.Visible = false;
        _controlBD = new ControlBD();
        string contenidoActual = "";
        string fechaActual = "";
        string tituloActual = "";
        string importanciaActual = "";
        idActual = "";
        trActualizandoNoticia.Visible = false;

        //Labels para validar campos en blanco
        lblValidaTitulo.Visible = false;

        //trae la variable tipo en noticia
        try
        {
            tipo = "";
            // Si es que viene un ID, significa que se está editando un contenido
            if (Request.QueryString["id"] != "" && Request.QueryString["id"] != null)
            {
                divListaMarcas.Visible = false;
                tblTitulo.Visible = false;
                trNoticiaImportante.Visible = false;
                // Se rescata el contenido actual de la noticia que se desea editar
                id = Request.QueryString["id"];

                if (!IsPostBack)
                {
                    DataSet ds_2 = _controlBD.ObtenerDatosFiltrados("select html,cast(fecha as varchar) fecha,titulo,importante from contenido where idContenido = " + id);
                    foreach (DataRow campo in ds_2.Tables[0].Rows)
                    {
                        contenidoActual = campo["html"].ToString();
                        tituloActual = campo["titulo"].ToString();
                        fechaActual = campo["fecha"].ToString();
                        importanciaActual = campo["importante"].ToString();
                    }
                    txtContenido.Text = contenidoActual;
                    // Se muestran las marcas, el titulo y la importancia de la noticia actual
                    trActualizandoNoticia.Visible = true;
                    infoNoticia.Text = "Esta noticia titulada \"<b>" + tituloActual + "</b>\" se creó el \"<b>" + fechaActual + "</b>\" y es de caracter ";
                    if (importanciaActual == "1")
                    {
                        infoNoticia.Text = infoNoticia.Text + "<b>importante</b>";
                    }
                    else
                    {
                        infoNoticia.Text = infoNoticia.Text + "<b>NO importante</b>";
                    }
                }
            }
            // Si es que no viene un ID de noticia, debería venir un tipo, es decir, que se esta
            // editando un tipo estático
            else
            {
                tipo = Request.QueryString["tipo"];
                if (!IsPostBack)
                {
                    if (tipo != "1")
                    {
                        DataSet ds_ = _controlBD.ObtenerDatosFiltrados("select top 1 idContenido,html from contenido where tipo = " + tipo + " order by idContenido desc");
                        foreach (DataRow campo in ds_.Tables[0].Rows)
                        {
                            contenidoActual = campo["html"].ToString();
                            idActual = campo["idContenido"].ToString();
                        }
                        txtContenido.Text = contenidoActual;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("En [Page_Load] Message: " + ex.Message + ", Stack: " + ex.StackTrace + ", Inner: " + ex.InnerException);
            msjesError.InnerText = "Error al cargar el contenido, contacte a soporte";
            msjesError.Visible = true;
        }

        // Para contenido de Casa Matriz no se muestran las marcas ni la solicitud de titulo
        // ni la solcicitud de noticia importante
        if (tipo == "3" || tipo == "4" || tipo == "9" || tipo=="11")
        {
            txtTitulo.Text = DateTime.Today.ToString() + " Contenido home";
            txtTitulo.Visible = false;
            lblTitulo.Visible = false;
            divListaMarcas.Visible = false;
            trNoticiaImportante.Visible = false;
            btnFin.Text = "Volver a Casa Matriz";
            btnFin.PostBackUrl = "CasaMatriz.aspx";
            btnCancelar.PostBackUrl = "CasaMatriz.aspx";
        }
        else if(tipo=="8" || tipo =="7")
        {
            txtTitulo.Text = DateTime.Today.ToString() + " Ranking Promociones";
            txtTitulo.Visible = false;
            lblTitulo.Visible = false;
            divListaMarcas.Visible = false;
            trNoticiaImportante.Visible = false;
            btnFin.Text = "Volver a Promociones";
            btnFin.PostBackUrl = "Promociones.aspx";
            btnCancelar.PostBackUrl = "Promociones.aspx";
        }
        else if (tipo == "1")
        {
            btnFin.Text = "Volver a Noticias";
            btnFin.PostBackUrl = "PaginaNoticia.aspx";
            btnCancelar.PostBackUrl = "PaginaNoticia.aspx";
        }
        else
        {
            logger.Warn("Contenido de tipo incierto");
            btnFin.Text = "Volver a Casa Matriz";
            btnFin.PostBackUrl = "CasaMatriz.aspx";
        }
    }

    protected void btnAgregar_Click1(object sender, EventArgs e)
    {
        divVistaPrevia.InnerHtml = txtContenido.Text;
        panelEditor.Visible = false;
        panelVistaPrevia.Visible = true;
    }

    protected void Click_CopiarRuta(object sender, EventArgs e)
    {
        
        Thread cbThread = new Thread(new ThreadStart(CopyToClipboard));
        cbThread.SetApartmentState(ApartmentState.STA);
        cbThread.Start();
        cbThread.Join();
    }

    [STAThread]
    protected void CopyToClipboard()
    {
        if (cargadorImagenes.HasFile)
            try
            {
                // Se comprueba el tamaño del archivo
                if(cargadorImagenes.PostedFile.ContentLength>10000000)
                {
                    throw new Exception("El archivo es muy pesado, solo 3MB aprox máximo");
                }
                // Se comprueba la extensión del archivo
                // Obtiene el nombre del archivo a subir
                string fileName = Server.HtmlEncode(cargadorImagenes.FileName);
                // Se obtiene la extensión del archivo a subir
                string extension = System.IO.Path.GetExtension(fileName);
                // Caso de querer subir una imágen
                if ((extension.Equals(".jpg",StringComparison.OrdinalIgnoreCase)) || (extension.Equals(".jpeg",StringComparison.OrdinalIgnoreCase)) ||
                    (extension.Equals(".png",StringComparison.OrdinalIgnoreCase)) || (extension.Equals(".gif",StringComparison.OrdinalIgnoreCase)) ||
                    (extension.Equals(".bmp",StringComparison.OrdinalIgnoreCase)))
                {
                    // Caso de ser una imagen
                    cargadorImagenes.SaveAs(Server.MapPath("~/doc/contenidosImgVideo/") + cargadorImagenes.FileName);
                    string texto = "../doc/contenidosImgVideo/" + cargadorImagenes.FileName.ToString();
                    txtContenido.Text += "<img width=\"300\" \"../tinymce/jscripts/tiny_mce/themes/advanced/img/trans.gif\" src=\"" + texto + "\">";
                }
                // Caso de ser un video
                else if ((extension.Equals(".mpg",StringComparison.OrdinalIgnoreCase)) ||
                    (extension.Equals(".mpeg",StringComparison.OrdinalIgnoreCase)) ||
                    (extension.Equals(".avi",StringComparison.OrdinalIgnoreCase)) ||
                    (extension.Equals(".mp3",StringComparison.OrdinalIgnoreCase)))
                {
                    // Caso de ser una imagen
                    cargadorImagenes.SaveAs(Server.MapPath("~/doc/contenidosImgVideo/") + cargadorImagenes.FileName);
                    string texto = "../doc/contenidosImgVideo/" + cargadorImagenes.FileName.ToString();
                    txtContenido.Text += "<p><img class=\"mceItemMedia mceItemWindowsMedia\" width=\"320\" height=\"240\" data-mce-src=\"../tinymce/jscripts/tiny_mce/themes/advanced/img/trans.gif\" src=\"../tinymce/jscripts/tiny_mce/themes/advanced/img/trans.gif\" data-mce-json=\"{'type':'windowsmedia','video':{'sources':[]},'params':{'src':'"+texto+"'},'width':'320','height':'240'}\"><br data-mce-bogus=\"1\"></p>";
                }
                // Caso de ser una animación flash
                else if ((extension.Equals(".swf",StringComparison.OrdinalIgnoreCase)))
                {
                    // Caso de ser una imagen
                    cargadorImagenes.SaveAs(Server.MapPath("~/doc/contenidosImgVideo/") + cargadorImagenes.FileName);
                    string texto = "../doc/contenidosImgVideo/" + cargadorImagenes.FileName.ToString();
                    txtContenido.Text += "<p><img class=\"mceItemMedia mceItemFlash\" src=\"../tinymce/jscripts/tiny_mce/themes/advanced/img/trans.gif\" data-mce-json=\"{'video':{},'params':{'src':'" + texto + "','sound':'true','progress':'true','autostart':'true','swstretchstyle':'none','swstretchhalign':'none','swstretchvalign':'none'}}\" height=\"240\" width=\"320\"></p>";
                }
                else
                {
                    throw new Exception("La extensión del archivo es inválida");
                }
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Hubo un problema: " + ex.Message;
                msjesError.Visible = true;
                logger.Error("En [CopyToClipboard] " + ex.Message);
            }
        else
        {
            msjesError.InnerText = "Error al subir archivo";
            msjesError.Visible = true;
            logger.Error("En [CopyToClipboard] Error al subir archivo");
        }
    }
    protected void btnPreviaAcepta_Click(object sender, EventArgs e)
    {
        string IdCont = "";

        if (tipo != null && tipo != "")
        {
            // Se valida si no se insertó título alguno
            if (txtTitulo.Text == "")
            {
                msjesError.InnerText = "Debe ingresar un título de noticia. Gracias";
                msjesError.Visible = true;
                panelVistaPrevia.Visible = false;
                panelEditor.Visible = true;
                return;
            }

            _marca = "";
            try
            {
                // Si el contenido es importante
                if (!chkNoticiaImportante.Checked)
                {
                    _controlBD.InsertarContenido(divVistaPrevia.InnerHtml, txtTitulo.Text, int.Parse(tipo));
                }
                // Si contenido no es importante
                else
                {
                    _controlBD.InsertarContenidoImportante(divVistaPrevia.InnerHtml, txtTitulo.Text, int.Parse(tipo));
                }
                /*
                 * El código que está a continuación esta super mal hecho. Si queda tiempo lo reparo
                 * */
                // Parche para anuncios de casa matriz
				if ( tipo == "3" || tipo == "4"  || tipo == "5" || tipo == "9"  ) 
				{
					
					DataSet rs = _controlBD.ObtenerDatosFiltrados("select top 1 idContenido from contenido order by idContenido desc");			
					foreach (DataRow campo in rs.Tables[0].Rows)
					{
						IdCont = campo["idContenido"].ToString();
					}
					int idfinal = int.Parse(IdCont) + 1;
					string query = "insert into contenido values('" + idfinal + "', GETDATE(),'" + divVistaPrevia.InnerHtml + "','" + tipo + " Contenido Home','" + tipo + "', 0)";
					if (_controlBD.InsertarDatos(query))
					{
						    msjesError.InnerText = "Contenido publicado" ;
							msjesError.Visible = true;
							return;
					}else {
							msjesError.InnerText = "Error al insertar contenido de home" ;
							msjesError.Visible = true;
							return;
					}
				}	
					DataSet ds = _controlBD.ObtenerDatosFiltrados("select top 1 idContenido from contenido order by idContenido desc");
					foreach (DataRow campo in ds.Tables[0].Rows)
					{
						IdCont = campo["idContenido"].ToString();
					}
					for (int i = 0; i < CheckBoxListMarcas.Items.Count; i++)
					{
						if (CheckBoxListMarcas.Items[i].Selected)
						{
							_marca = CheckBoxListMarcas.Items[i].Text;
							_controlBD.InsertarDatos("insert into contenido_marca values(" + IdCont + ",'" + _marca + "')");
						}
					}
					CheckBoxListMarcas.Items.Clear();

                msjesError.InnerText = "Contenido publicado "+ tipo;
                msjesError.Visible = true;

            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Error al publicar contenido. Contacte a soporte";
                msjesError.Visible = true;
                logger.Error("En [btnPreviaAcepta_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }
            
            // Se va a la vista previa
            panelFin.Visible = true;
         }
        
        else
        {
            if (id != null)
            {
                // Se actualiza contenido
                _controlBD.ActualizaContenido(txtContenido.Text, id);
                // Se envía a vista previa
                Response.Redirect("verMas.aspx?valor="+id);
            }
            else
            {
                msjesError.InnerText = "Error de navegación. Vuelva atrás";
                msjesError.Visible = true;
            }
        }
    }
    protected void btnPreviaCancela_Click(object sender, EventArgs e)
    {
        panelVistaPrevia.Visible = false;
        panelEditor.Visible = true;
    }
}