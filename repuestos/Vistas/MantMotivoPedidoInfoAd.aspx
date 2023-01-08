<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MantMotivoPedidoInfoAd.aspx.cs" Inherits="Vistas_MantMotivoPedidoInfoAd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width:100%" border="0">
                <tr>
                    <td>
                        <asp:Label ID="lblMarca" runat="server" Text="Marca"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtMarca" runat="server"></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblMotivo" runat="server" Text="Motivo"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtMotivo" runat="server"></asp:TextBox>
                    </td>
                </tr>

                <tr><td colspan="2">&nbsp;</td></tr>

                <tr>
                    <td colspan="2">
                        <asp:GridView ID="grwInfoAdicional" runat="server" Width="100%" EmptyDataText="No se encontraron registros"
                            AutoGenerateColumns="false" PageSize="20" 
                            OnRowCommand="grwInfoAdicional_RowCommand">
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" />
                            <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Verdana" Font-Size="Small" />
                            <RowStyle Font-Names="Verdana" Font-Size="Small" />

                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                            <asp:Label ID="lblId" runat="server" Text='<%# Eval("id") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Descripción">
                                    <ItemTemplate>
                                            <asp:Label ID="lblDescripcion" runat="server" ><%# Eval("descripcion")%></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="center">
                                    <ItemTemplate>
                                            <asp:CheckBox ID="chkHabilitado" OnCheckedChanged="chkHabilitado_CheckedChanged" runat="server" Checked='<%#Convert.ToBoolean(Eval("habilitado")) %>' />
                                   </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>

                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
                    </td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
