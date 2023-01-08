using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ControlExcel
/// </summary>
public class ControlExcel
{
    private Excel exl;
    private String _ano;

    String[] marca = null;
	public ControlExcel()
	{
        exl = new Excel();
        _ano = "";
	}

    public void setearAno(String archivo)
    {
        String[] nombreArchivo = archivo.Split('.');
        marca = nombreArchivo[0].Split(' ');

        _ano = marca[1] + @"\";
    }

    public String getAno()
    {
        return _ano;
    }

    public void guardaIndicador(String archivo,String rutaCsv)
    {
        String nombreMarca = "";
        String[] nombreArchivo = archivo.Split('.');
        String[] marca = nombreArchivo[0].Split(' ');

        switch (marca[0])
        {
            case "SSY":
                nombreMarca = "SSANGYONG";
                break;
            case "MMC":
                nombreMarca = "MITSUBISHI";
                break;
            case "IT":
                nombreMarca = "FIAT";
                break;
            case "CH":
                nombreMarca = "CHRYSLER";
                break;
            case "NW":
                nombreMarca = "CHERY";
                break;
            case "MG":
                nombreMarca = "MG";
                break;
            case "TATA":
                nombreMarca = "TATA";
                break;
            default: break;
        }
        exl.insertaIndicador(rutaCsv, nombreMarca, int.Parse(marca[1]));
    }

    public void cambiaIndicador(String archivo, String rutaCsv)
    {
        String nombreMarca = "";
        String[] nombreArchivo = archivo.Split('.');
        String[] marca = nombreArchivo[0].Split(' ');

        switch (marca[0])
        {
            case "SSY":
                nombreMarca = "SSANGYONG";
                break;
            case "MMC":
                nombreMarca = "MITSUBISHI";
                break;
            case "IT":
                nombreMarca = "FIAT";
                break;
            case "CH":
                nombreMarca = "CHRYSLER";
                break;
            case "NW":
                nombreMarca = "CHERY";
                break;
            case "MG":
                nombreMarca = "MG";
                break;
            case "TATA":
                nombreMarca = "TATA";
                break;
            default: break;
        }
        exl.modificaIndicador(rutaCsv, nombreMarca, int.Parse(marca[1]));
    }
    //COMPRAS
    public int primerTrimestre(String nombreMarca, String nombreConcesionario,int ano)
    {
        return exl.primerTrimestre(nombreMarca, nombreConcesionario,ano);
    }
    public int segundoTrimestre(String nombreMarca, String nombreConcesionario,int ano)
    {
        return exl.segundoTrimestre(nombreMarca, nombreConcesionario,ano);
    }
    public int tercerTrimestre(String nombreMarca, String nombreConcesionario,int ano)
    {
        return exl.tercerTrimestre(nombreMarca, nombreConcesionario,ano);
    }
    public int cuartoTrimestre(String nombreMarca, String nombreConcesionario, int ano)
    {
        return exl.cuartoTrimestre(nombreMarca, nombreConcesionario,ano);
    }


    //METAS
    public int primerTrimestreMetas(String nombreMarca, String nombreConcesionario,int ano)
    {
        return exl.primerTrimestreMetas(nombreMarca, nombreConcesionario, ano);
    }
    public int segundoTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        return exl.segundoTrimestreMetas(nombreMarca, nombreConcesionario, ano);
    }
    public int tercerTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        return exl.tercerTrimestreMetas(nombreMarca, nombreConcesionario, ano);
    }
    public int cuartoTrimestreMetas(String nombreMarca, String nombreConcesionario, int ano)
    {
        return exl.cuartoTrimestreMetas(nombreMarca, nombreConcesionario, ano);
    }

    public int[] metaMeses(String nombreMarca, String nombreConcesionario, int ano)
    {
        return exl.metaMeses(nombreMarca, nombreConcesionario, ano);
    }

    public int[] compraMeses(String nombreMarca, String nombreConcesionario, int ano)
    {
        return exl.compraMeses(nombreMarca, nombreConcesionario, ano);
    }

    public int metaDeUnMes(String nombreMarca, String nombreConcesionario, int ano, int mes)
    {
        return exl.metaDeUnMes(nombreMarca, nombreConcesionario, ano, mes);
    }

    public int compraDeUnMes(String nombreMarca, String nombreConcesionario, int ano, int mes)
    {
        return exl.compraDeUnMes(nombreMarca, nombreConcesionario, ano, mes);
    }
}