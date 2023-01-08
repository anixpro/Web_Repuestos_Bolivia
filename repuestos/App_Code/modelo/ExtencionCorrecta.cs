using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace manejoTXT.Clases
{
    static class ExtencionCorrecta
    {

        public static void cambiaExtencionPdf(String rutaMasArchivo,String rutaSinArchivo, String archivoTxt)
        {
            if (File.Exists(rutaMasArchivo))
            {

                String[] pedidospdf = Directory.GetFiles(rutaSinArchivo, archivoTxt + ".copia*.pdf");
                if (pedidospdf.Length == 0)//SI NO EXISTEN COPIAS (.*COPIA1.ERROR)...ETC LE DAMOS SU NUMERO 1 COMO PRIMERA COPIA
                {
                    File.Move(rutaMasArchivo, rutaMasArchivo + ".copia1.pdf");

                }
                else
                {
                    //CODIGO PARA OBTENER NUMERO MAYOR 
                    ArrayList numero = new ArrayList();
                    foreach (String slash in pedidospdf)
                    {

                        String[] spliteado = slash.Split('\\');
                        String[] archivoErroneoCopiado = spliteado[4].Split('.');
                        String numeroBuscado = archivoErroneoCopiado[2].Substring(5, 1);

                        numero.Add(numeroBuscado);


                    }
                    int numeroMayor = 0;
                    numeroMayor = int.Parse(numero[0].ToString());

                    foreach (String Numero in numero)
                    {

                        if (numeroMayor < int.Parse(Numero))
                        {
                            numeroMayor = int.Parse(Numero);
                        }

                    }

                    //FIN DE OBTENER NUMERO MAYOR
                    String numeroNuevaCopia = (numeroMayor + 1).ToString();
                    File.Move(rutaMasArchivo, rutaMasArchivo + ".copia" + numeroNuevaCopia + ".pdf");
                }// FIN ELSE SI ES QUE EXISTEN COPIAS
            }//FIN SI YA EXISTE UN ARCHIVO DE ERROR IGUAL
            else
            {
                File.Move(rutaMasArchivo, rutaMasArchivo + ".pdf");
            }
        }
    }
}
