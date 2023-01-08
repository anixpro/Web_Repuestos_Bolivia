using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ModeloSeguimiento
/// </summary>
public class ModeloSeguimiento
{
    public string id { get; set; }

    public string idSolicitudVFC { get; set; }

    public string fechaCreacion { get; set; }

    public string creador { get; set; }

    public string fechaETA { get; set; }

    public string detalle { get; set; }

    public string nuevoEstado { get; set; }

    public string  idMarca { get; set; }

    public string codigoRepuesto { get; set; }
}