<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="HorarioCDR.aspx.cs" Inherits="Vistas_HorarioCDR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="msjError" runat="server" clientidmode="Static">
    </div>
    <%--<div class="contenedor">--%>
       <%-- <div class="homeUp">--%>
            <table id="horario" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td>
                    <img src="../img/horarioCDR.png" width="100%" height="100%" align="middle" border="0" alt="horario CDR" />
                    </td>
                </tr>
            </table>
        <%--</div>--%>
    <%-- </div>--%>
</asp:Content>