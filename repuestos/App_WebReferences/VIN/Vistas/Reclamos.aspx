<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Reclamos.aspx.cs" Inherits="Vistas_Reclamos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="msjError" runat="server" clientidmode="Static">
    </div>
    <%--<div class="contenedor">--%>
    <%-- <div class="homeUp">--%>
    <table id="reclamos" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <img src="../img/Reclamos.png" width="100%" height="100%" align="middle" border="0"
                    alt="Reclamos CDR" />
            </td>
        </tr>
        <tr>
            <td>
               <%-- Descargar Formulario de Reclamos--%> <a href="../doc/contenidoProcedimientos/Reclamo.xls"
                    target="_blank"><strong>Descargar Formulario de Reclamos</strong> 
                   <%-- <img src="../img/reclamo.png" width="160px" height="100px" alt="slide 1" />--%>
                </a>
            </td>
        </tr>
    </table>
    <%--</div>--%>
    <%-- </div>--%>
</asp:Content>