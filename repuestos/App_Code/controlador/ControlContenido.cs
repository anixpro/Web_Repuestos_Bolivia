using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlContenido
/// </summary>
public static class ControlContenido
{
	public static int eliminaContenido(string id)
    {
        Contenido _modeloContenido = new Contenido();
        return _modeloContenido.eliminaContenido(id);
    }
}
