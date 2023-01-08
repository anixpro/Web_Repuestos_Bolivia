using System;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Collections;


      /// <summary>
      /// Descripción de resumen para la clase MessageBox.
      /// </summary>
      public class MessageBox
      {
            private static Hashtable mpaginasEnEjecucion = new Hashtable();
 
            /// <summary>
            /// Constructor vacío.
            /// </summary>
            private MessageBox(){}
 
            public static void Show( string Mensaje )
            {
                  // Si es la primera vez que una página llama a este método.
                if (!mpaginasEnEjecucion.Contains(HttpContext.Current.Handler))
                  {
                        // Trata de castear el HttpHandler como Page.
                      Page executingPage = HttpContext.Current.Handler as Page;
 
                        if( executingPage != null )
                        {
                              // Se crea una cola para almacenar uno o más mensajes.
                              Queue colaMensajes = new Queue();
 
                            // Se agrega nuestro mensaje a la cola.
                              colaMensajes.Enqueue( Mensaje );
       
                              // Se agrega nuestra cola de mensajes a la tabla hash. Se utiliza nuestra referencia page
                              // (IHttpHandler) como llave para la tabla hash.
                              mpaginasEnEjecucion.Add(HttpContext.Current.Handler, colaMensajes);
 
                              // Se asocia el evento Unload para poder inyectar el código JavaScript para las alertas.

                              executingPage.Unload += new EventHandler( PaginaEnEjecucion_Unload );
                        }  
                  }
                  else
                  {
                        // Si el método ha sido llamado antes por la página.
                        // ya tenemos nuestra cola de mensajes y su referencia en nuestra tabla hash.
                      Queue cola = (Queue)mpaginasEnEjecucion[HttpContext.Current.Handler];
       
                        // Se agrega nuestro mensaje a la cola.
                        cola.Enqueue( Mensaje );
                  }
            }
 
 
            // Cuando nuestra página ha terminado de renderizar el código se agrega el código JavaScript para producir las alertas.
            private static void PaginaEnEjecucion_Unload(object sender, EventArgs e)
            {
                  // Se obtiene nuestra cola de mensajes desde la tabla hash.
                Queue cola = (Queue)mpaginasEnEjecucion[HttpContext.Current.Handler];
       
                  if( cola != null )
                  {
                        StringBuilder sb = new StringBuilder();
 
                        // Cantidad de mensajes registrados en la col.
                        int iMsgCount = cola.Count;
 
                        // Se utiliza al StringBuilder para crear el código JavaScript del cliente..
                        sb.Append( "<script language='javascript'>" );
 
                        // Se itera en los mensajes de la cola.
                        string sMsg;
                        while( iMsgCount-- > 0 )
                        {
                              sMsg = (string) cola.Dequeue();
                              sMsg = sMsg.Replace( "\n", "\\n" );
                             sMsg = sMsg.Replace( "\"", "'" );
                              sb.Append( @"alert( """ + sMsg + @""" );" );
                        }
 
                        // Se cierra la sección de código JavaScript.
                        sb.Append( @"</script>" );
 
                        // Una vez agregado el código JS se quita la asociación del evento Unload a este método
                        mpaginasEnEjecucion.Remove(HttpContext.Current.Handler);
 
                        // Se escribe el código JavaScript al final del stream de respuesta.
                        HttpContext.Current.Response.Write( sb.ToString() );
                  }
            }
      }