<%@ Page Title="Mantenedor de Tipos de Transporte" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="MantTipoTransporte.aspx.cs" Inherits="Vistas_MantTipoTransporte" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="titulo" style="margin-bottom: 10px">
        Mantenedor de Tipos de Transporte
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <div class="avisoInsert">
        <center>
            <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" BackColor="#FF8000"
                ForeColor="#B00000"></asp:Label>
        </center>
    </div>
    <div style="margin-bottom: 10px; margin: 10px">
        <center>
            <table>
                <tr>
                    <td>
                        Nombre:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtNuevoNombre" Width="120px" MaxLength="50"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button runat="server" CssClass="button" ID="btnNuevoTipo" ClientIDMode="Static" Text="Agregar Tipo" OnClick="nuevo_Click"  />
                    </td>
                </tr>
            </table>
        </center>
    </div>
    <div class="viasUpdate">
        <center>
            <asp:GridView ID="grTipos" runat="server" Width="70%" EmptyDataText="No se encontraron registros"
                AutoGenerateColumns="false" AllowPaging="false" PageSize="20" OnRowDataBound="grTipos_RowDataBound"
                OnRowCommand="grTipos_RowCommand" Style="text-align: center;">
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                <Columns>
                    <asp:TemplateField HeaderText="Nombre">
                        <ItemTemplate>
                            <center>
                                <asp:HiddenField ID="hdId" runat="server" Value='<%# Eval("Id") %>' />
                                <asp:Label ID="lblNombre" runat="server" Width="80px"><%# Eval("Nombre") %></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true" HeaderText="Habilitado">
                        <ItemTemplate>
                            <center>
                                <asp:CheckBox ID="chkHabilitado" AutoPostBack="true" OnCheckedChanged="chkHabilitado_CheckedChanged"
                                    runat="server" Checked='<%#Convert.ToBoolean(Eval("Habilitado")) %>' />
                            </center>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="55px" VerticalAlign="Middle" Wrap="true" />
                    </asp:TemplateField>
                    <asp:ButtonField ButtonType="Image" ImageUrl="~/img/cross.png" CommandName="elimina" 
                        Text="Elimina" HeaderText="Eliminar" ItemStyle-Width="50px" ControlStyle-CssClass="btnElimina" />
                </Columns>
            </asp:GridView>
        </center>
    </div>
</asp:Content>
