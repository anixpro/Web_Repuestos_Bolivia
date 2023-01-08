<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CasaMatriz.aspx.cs" Inherits="Vistas_CasaMatriz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Casa Matriz
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="contenedor">
        <div class="homeUp">
            <asp:Repeater ID="RepeaterCazaMatriz0" runat="server" onitemdatabound="limitar">
                <ItemTemplate>
                    <asp:Label ID="lblCasaMatriz1" runat="server" Text='<%# Eval("html") %>'></asp:Label><br />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="homeUpBotonAgregar">
            <asp:Button ID="ImageButton4" runat="server" PostBackUrl="~/Vistas/EditorContenido.aspx?tipo=9"
                Text = "Editar contenido" CssClass="button"/>
        </div>
        <div class="homeIzq"><!--homeIzq-->
            <asp:Repeater ID="RepeaterCazaMatriz" runat="server" onitemdatabound="limitar">
                <ItemTemplate>
                    <asp:Label ID="lblCasaMatriz1" runat="server" Text='<%# Eval("html") %>'></asp:Label><br />              
                </ItemTemplate>
            </asp:Repeater>
        </div><!--fin homeIzq-->
        <div class="homeDer"><!--homeDer-->
            <asp:Repeater ID="RepeaterCasaMatriz2" runat="server" onitemdatabound="limitar">
                <ItemTemplate>
                    <asp:Label ID="lblCasaMatriz2" runat="server" Text='<%# Eval("html") %>'></asp:Label><br />
                </ItemTemplate>
            </asp:Repeater>
        </div>  <!--Fin homeDer--> 
        <div class="homeIzqBotonAgregar">
            <asp:Button ID="ImageButton1" runat="server" 
                Text = "Editar contenido" CssClass="button"
                PostBackUrl="~/Vistas/EditorContenido.aspx?tipo=3"/>
        </div>
        <div class="homeDerBotonAgregar">
            <asp:Button ID="ImageButton2" runat="server" 
                Text = "Editar contenido" CssClass="button"
                PostBackUrl="~/Vistas/EditorContenido.aspx?tipo=4"/>
        </div>
    </div>
</asp:Content>
