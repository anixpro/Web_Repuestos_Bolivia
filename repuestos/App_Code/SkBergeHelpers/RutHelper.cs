using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de RutHelper
/// </summary>
public class RutHelper
{
	public RutHelper()
	{
		//
		// TODO: Agregar aquí la lógica del constructor
		//
	}

    //función valida rut
    public string GetRutConDigito(string rut_texto)
    {

       /* int Digito;
        int Contador;
        int Multiplo;
        int Acumulador;
        string rut;
        String dig;

        rut = (rut_texto);

        Contador = 2;
        Acumulador = 0;

        while (rut != "0")
        {
            Multiplo = (rut % 10) * Contador;
            Acumulador = Acumulador + Multiplo;
            rut = rut / 10;
            Contador = Contador + 1;
            if (Contador == 8)
            {
                Contador = 2;
            }

        }

        Digito = 11 - (Acumulador % 11);
        dig = Digito.ToString().Trim();
        if (Digito == 10)
        {
            dig = "K";
        }
        if (Digito == 11)
        {
            dig = "0";
        }
        */
        return rut_texto;
    }
}