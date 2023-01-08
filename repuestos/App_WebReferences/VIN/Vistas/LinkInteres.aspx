<%@ Page Title="SKBergé: Links de Interés" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="LinkInteres.aspx.cs" Inherits="Vistas_LinkInteres" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Link de Interés
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="costadoIzquierdo">
        Estos son enlaces a otros sitios que puedan ser de su utilidad
    </div>
    <div class="costadoDerecho">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Repeater ID="RepeaterLink" OnItemCommand="RepeaterLink_ItemCommand" runat="server">
                    <ItemTemplate>
                        <div style="font-weight:bolder">
                            <asp:Label ID="lblDescrip" runat="server" Text='<%# Eval("descripcion") %>'>
                            </asp:Label>
                        </div>
                        <asp:HyperLink ID="hlkUrl" ForeColor="#2E2E2E" runat="server" NavigateUrl='<%# Eval("url") %>' Text='<%# Eval("url") %>' Target="_blank">
                        </asp:HyperLink>
                        <%
                        if (int.Parse(Session["permisos"].ToString()) == 1)
                        {
                        %>
                            <asp:LinkButton OnClientClick="javascript:return(confirm('¿Seguro que desea eliminar?'))" ID="LinkButton1" CommandArgument='<%# Eval("idLinks") %>' runat="server">
                                <img alt="(X)" src="../img/gtk-no.png" width="15px" height="15px" />
                            </asp:LinkButton>
                        <%
                        }
                        %>
                        <hr />
                    </ItemTemplate>
                    </asp:Repeater>
                    <div style="text-align:center;">
                        <asp:Button ID="agregarLink" runat="server" Text="Agregar nuevo enlace" 
                            CssClass="button" ClientIDMode="Static" onclick="agregarLink_Click1"/>
                    </div>
                    <asp:Panel ID="panelAgregaLink" runat="server">
                        <table>
                            <tr>
                                <td>Descripción: </td>
                                <td>
                                    <asp:TextBox ID="txtNomLinkInteres" runat="server" MaxLength="70" ClientIDMode="Static"></asp:TextBox>
                                </td>
                                <td>
                                    <img alt="Ejemplo: Conesionario A" title="Ingrese la descripción de enlace de interés del sitio" src="../img/help.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>URL del sitio: </td>
                                <td>
                                    <asp:TextBox ID="txtLinkInteres" runat="server" Text="http://" MaxLength="100" 
                                        ClientIDMode="Static"></asp:TextBox></td>
                                <td>
                                    <img alt="Ejemplo: http://www.ferrari.cl" title="Ingrese un enlace de interés del sitio. Ejemplo: 'http://www.mg.com'" src="../img/help.png" />
                                   <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Debe completar todos los campos"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:Button CssClass="button" ID="btnAgregarLink" ClientIDMode="Static" 
                                        runat="server" Text="Agregar" onclick="btnAgregarLink_Click"/>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UP1" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                <script type="text/javascript">
                    document.write("<div class='UpdateProgressBackground'></div>");
                </script>
                <center>
                    <div class="UpdateProgressContent">Cargando...</div>
                </center>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
</asp:Content>

