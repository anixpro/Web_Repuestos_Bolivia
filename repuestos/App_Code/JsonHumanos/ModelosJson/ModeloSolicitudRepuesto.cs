using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ModeloSolicitudRepuesto
/// </summary>
public class ModeloSolicitudRepuesto
{
    public string idSolicitud { get; set; }

    public string fechaCreacion { get; set; }

    public string cantidad { get; set; }

    public string producto { get; set; }

    public string marca { get; set; }

    public string creador { get; set; }

    public string fechaETA { get; set; }

    public string direccion { get; set; }
    public string estado { get; set; }
    public string concesionario { get; set; }
    public string local { get; set; }
    public string tipoVFC { get; set; }
    public string tipoPedido { get; set; }
    public string detalle { get; set; }
    public string critico { get; set; }
    public string obsVFC { get; set; }
}