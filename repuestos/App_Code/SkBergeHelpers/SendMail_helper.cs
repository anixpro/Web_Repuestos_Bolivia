using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

/// <summary>
/// Descripción breve de SendMail_helper
/// </summary>
public class SendMail_helper
{
    private MailMessage _correo = new MailMessage();
    private SmtpClient _smtp = new SmtpClient();

    private System.Net.NetworkCredential credenciales = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["user_correo"].ToString(), ConfigurationManager.AppSettings["pass_correo"].ToString());
    private string _msgEstado;

    public string EnviarCorreo(string para, string asunto, string texto, string cc="" )
    {
        _correo.From = new MailAddress(ConfigurationManager.AppSettings["correo_sistema"].ToString());
        _correo.To.Add(para);
        if (cc != "" && cc != ",")
        {

            List<string> Correos = new List<string>();

            string[] listcorreos = cc.Split(',');
            foreach (string uncorreo in listcorreos)
            {
                Correos.Add(uncorreo.ToString());
            }
            string MailAddres = string.Join(",", Correos.ToArray());
            _correo.Bcc.Add(MailAddres);
        }
        _correo.Subject = asunto;
        _correo.Body = texto;
        _correo.IsBodyHtml = false;
        _correo.Priority = MailPriority.Normal;

        //Asingnar datos del servidor de correo
        _smtp.Host = ConfigurationManager.AppSettings["host_correo"].ToString();
        _smtp.Credentials = credenciales;
        _smtp.Port = int.Parse(ConfigurationManager.AppSettings["puerto_correo"].ToString());
        _smtp.EnableSsl = false;

        try
        {
            _smtp.Send(_correo);
            return _msgEstado = "Correo enviado con exito";

        }
        catch (Exception ex)
        {
            return _msgEstado = "Error al enviar  correo " + ex.Message;
        }
    }

    public string EnviarCorreoAdjunto(string para, string asunto, string texto, string directorio)
    {
        string cc = "";
        _correo.From = new MailAddress(ConfigurationManager.AppSettings["correo_sistema"].ToString());
        _correo.To.Add(para);

        try
        {
            if (cc != "")
            {

                List<string> Correos = new List<string>();

                string[] listcorreos = cc.Split(',');
                foreach (string uncorreo in listcorreos)
                {
                    Correos.Add(uncorreo.ToString());
                }
                string MailAddres = string.Join(",", Correos.ToArray());
                _correo.Bcc.Add(MailAddres);
            }



            _correo.Subject = asunto;
            _correo.Body = texto;
            _correo.IsBodyHtml = false;

            _correo.Attachments.Add(new Attachment(GetStreamFile(directorio), Path.GetFileName(directorio)));
            //_correo.Attachments.Add(new Attachment(ms, new FileInfo(directorio).Name));

            //_correo.Attachments.Add(new System.Net.Mail.Attachment(directorio));
            _correo.Priority = MailPriority.Normal;

            //Asingnar datos del servidor de correo
            _smtp.Host = ConfigurationManager.AppSettings["host_correo"].ToString();
            _smtp.Credentials = credenciales;
            _smtp.Port = int.Parse(ConfigurationManager.AppSettings["puerto_correo"].ToString());
            _smtp.EnableSsl = false;


            _smtp.Send(_correo);
            //_smtp.Send("repeustos@skberge.cl", "porosteguig@skberge.cl; jpo@skberge.cl", "correcto EnviarCorreoAdjunto", "Correcto envio a correo " + para + "");
            //ms.Close();
            return _msgEstado = "Correo enviado con exito";

        }
        catch (Exception ex)
        {
            //envio correo 
            SendMail_helper _mail = new SendMail_helper();
            //_smtp.Send("repeustos@skberge.cl", "porosteguig@skberge.cl; jpo@skberge.cl", "error EnviarCorreoAdjunto", "Error " + ex.Message + " al enviar correo a " + para + "");
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [2_RealizaPedido_CrearCotizacion_SAP] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + "   ruta: " + directorio);
            return _msgEstado = "Error al enviar  correo " + ex.Message;
        }



    }

    public Stream GetStreamFile(string filePath)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            MemoryStream memStream = new MemoryStream();
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

            return memStream;
        }
    }

    public string EnviarCorreoHTML(string para, string asunto, string texto, string cc = "")
    {

         SmtpClient _smtp2 = new SmtpClient();
        MailMessage _correo2 = new MailMessage();

        _correo2.To.Clear();
        _correo2.CC.Clear();
        _correo2.Bcc.Clear();

        _correo2.From = new MailAddress(ConfigurationManager.AppSettings["correo_sistema"].ToString());
        _correo2.To.Add(para);
        if (cc != "" && cc != ",")
        {

            List<string> Correos = new List<string>();

            string[] listcorreos = cc.Split(',');
            foreach (string uncorreo in listcorreos)
            {
                Correos.Add(uncorreo.ToString());
            }
            string MailAddres = string.Join(",", Correos.ToArray());
            MailAddres = MailAddres.TrimEnd(',');
            _correo2.Bcc.Add(MailAddres);
        }
        _correo2.Subject = asunto;
        _correo2.Body = texto;
        _correo2.IsBodyHtml = true;
        _correo2.Priority = MailPriority.Normal;

        //Asingnar datos del servidor de correo
        _smtp2.Host = ConfigurationManager.AppSettings["host_correo"].ToString();
        _smtp2.Credentials = credenciales;
        _smtp2.Port = int.Parse(ConfigurationManager.AppSettings["puerto_correo"].ToString());
        _smtp2.EnableSsl = false;

        try
        {
            _smtp2.Send(_correo2);
            return _msgEstado = "Correo enviado con exito";
        }
        catch (Exception ex)
        {
            _smtp2.Send("Repuestos@skberge.cl", ConfigurationManager.AppSettings["correo_error"].ToString(), "error EnviarCorreo", "Error " + ex.Message + " al enviar correo a " + para + "");
            return _msgEstado = "Error al enviar  correo " + ex.Message;
        }
        finally
        {
            //smtp2.Dispose();     
        }
    }

    public string EnviarCorreo2VFc(string para, string asunto, string texto, string cc = "")
    {

        _correo.From = new MailAddress(ConfigurationManager.AppSettings["correo_sistema"].ToString());

        char[] delimit = new char[] { ';' };
        foreach (string enviar_a in para.Split(delimit))
        {
            _correo.To.Add(new MailAddress(enviar_a));
        }

        if (cc != "")
        {
            List<string> Correos = new List<string>();

            string[] listcorreos = cc.Split(',');
            foreach (string uncorreo in listcorreos)
            {
                Correos.Add(uncorreo.ToString());
            }
            string MailAddres = string.Join(",", Correos.ToArray());
            _correo.CC.Add(MailAddres);
        }
        _correo.Subject = asunto;
        _correo.Body = texto;
        _correo.IsBodyHtml = true;
        _correo.Attachments.Clear();
        _correo.Priority = MailPriority.Normal;

        //Asingnar datos del servidor de correo
        _smtp.Host = ConfigurationManager.AppSettings["host_correo"].ToString();
        _smtp.Credentials = credenciales;
        _smtp.Port = int.Parse(ConfigurationManager.AppSettings["puerto_correo"].ToString());
        _smtp.EnableSsl = false;

        try
        {
            _smtp.Send(_correo);
            //_smtp.Send("repeustos@skberge.cl", "porosteguig@skberge.cl", "correcto EnviarCorreo2VFc", "Correcto envio a correo " + para + "");

            return _msgEstado = "Correo enviado con exito";

        }
        catch (Exception ex)
        {
            _smtp.Send("Repuestos@skberge.cl", ConfigurationManager.AppSettings["correo_error"].ToString(), "error EnviarCorreo2VFc", "Error " + ex.Message + " al enviar correo a " + para + "");
            return _msgEstado = "Error al enviar  correo " + ex.Message;

        }
    }
}