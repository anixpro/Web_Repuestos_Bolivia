<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PaginaNoticia.aspx.cs" Inherits="Vistas_PaginaNoticia" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Noticias
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <h3 style="font-weight:bold">Historial Noticias</h3>
    <div class="costadoIzquierdoNoticias" style="min-height:389px;">
       
		 <asp:GridView ID="gridNoticias" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" DataSourceID="sqldNoticias"
            EnableModelValidation="True" Style="text-align:center;" EmptyDataText="No se encontraron Noticias" CellPadding="10" 
            ForeColor="#333333" GridLines="None" CssClass="tablaNoticias" PagerSettings-Mode="NextPreviousFirstLast">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                  <asp:TemplateField FooterText="Eliminar" HeaderText="Anteriores">
                    <ItemTemplate>
							 <a target="_self" style="cursor:pointer;" href="VerMas.aspx?valor='<%# Eval("idContenido") %>'"><%# Eval("titulo") %><br/><%# Eval("solfecha") %></a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
	
    <div class="costadoDerechoNoticias" style="min-height:389px;max-height:651px">
        <p><b>Última noticia:</b></p>
        <asp:Repeater ID="Repeater1" runat="server" ClientIDMode="Static" 
            onitemcommand="Repeater1_ItemCommand" onitemdatabound="limitar">
            <ItemTemplate>
                <div class="ContMedio">
                    <asp:Label runat="server" ClientIDMode="Static" ID="lbl" Text='<%# Eval("html") %>'></asp:Label>
                </div>
                <div class="ContMedio" style="text-align:right;">
                    <table style="font-family: Arial, Helvetica, sans-serif; font-size: 12px; color: #333333" width="100%">
                        <tr>
                            <td><asp:LinkButton ID="LinkButton2"  CommandArgument='<%# Eval("idContenido") %>' runat="server" ClientIDMode="Static"><img alt="ver más" src="../img/b_verMas.gif" /></asp:LinkButton></td>
                        </tr>
                    </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div id="divAgregar" class="ContMedio" style="text-align:center;">
            <asp:Button ID="AgregarNoticia" runat="server" ClientIDMode="Static" 
                CssClass="button" Text="Agregar nueva noticia"
                PostBackUrl="~/Vistas/EditorContenido.aspx?tipo=1" />
        </div>
	 <asp:SqlDataSource ID="sqldNoticias" runat="server" ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>
</asp:Content>

