<%@ Page Title="Mantenedor de Motivos DR" Language="C#" AutoEventWireup="true" CodeFile="MantMotivoDr.aspx.cs" 
Inherits="Vistas_MantMotivoDr" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="titulo" style="margin-bottom: 10px">
        Mantenedor de Motivos Dr
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
    <div>
        <table style="width:100%" border="0">
            <tr>
                <td style="width:120px;">
                <b>Filtro por Tipo:</b>
                </td>
                <td style="width:200px;">
                    <asp:DropDownList Width="150px" ID="combofiltro" runat="server" ClientIDMode="Static"
                        AutoPostBack="false" TabIndex="1">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button runat="server" CssClass="button" ID="btnFiltrar" ClientIDMode="Static" Text="Filtrar" OnClick="filtro_Click"  />
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-bottom: 10px; margin-top: 10px; width:100%" >
        <center>
            <table  style="width:100%" border="0">
                <tr>
                    <td>
                        <b>Nuevo:</b>
                    </td>
                    <td>
                        Tipo:
                    </td>
                    <td>
                        <asp:DropDownList Width="150px" ID="comboTipos" runat="server" ClientIDMode="Static"
                            AutoPostBack="false" TabIndex="1">
                        </asp:DropDownList>
                    </td>
                    <td>
                        Descripción:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtDescripcionNuevo" Width="120px" MaxLength="50"></asp:TextBox>
                    </td>
                    <td>
                        Paridad:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtParidad" Width="50px" MaxLength="3"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button runat="server" CssClass="button" ID="btnNuevoRegistro" ClientIDMode="Static" Text="Agregar" OnClick="nuevo_Click"  />
                    </td>
                </tr>
            </table>
        </center>
    </div>
    <div class="viasUpdate">
        <center>
            <asp:GridView ID="grMotivos" runat="server" Width="100%" EmptyDataText="No se encontraron registros"
                AutoGenerateColumns="false" AllowPaging="false" PageSize="20" 
                OnRowCommand="grTipos_RowCommand" Style="text-align: center;">
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                <Columns>
                    <asp:TemplateField HeaderText="Id">
                        <ItemTemplate>
                            <center>
                                <asp:Label ID="lblId" runat="server"><%# Eval("ID")%></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Descripción">
                        <ItemTemplate>
                            <center>
                                <asp:HiddenField ID="hdId" runat="server" Value='<%# Eval("ID") %>' />
                                <asp:Label ID="lblDescripcion" runat="server"><%# Eval("DESCRIPCION")%></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipo">
                        <ItemTemplate>
                            <center>
                                <asp:Label ID="lblTipo" runat="server"><%# Eval("TIPO_MOTIVO")%></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Paridad">
                        <ItemTemplate>
                            <center>
                                <asp:HiddenField ID="hdParidad" runat="server" Value='<%# Eval("PARIDAD") %>' />
                                <asp:TextBox ID="txtParidad" runat="server" onkeypress="return isNumberKey(event)" MaxLength="3"
                                    Width="80px" Text='<%# Eval("PARIDAD") %>'></asp:TextBox>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:ButtonField ButtonType="Image" ImageUrl="~/img/cross.png" CommandName="elimina" 
                        Text="Elimina" HeaderText="Eliminar" ItemStyle-Width="50px" ControlStyle-CssClass="btnElimina" />
                </Columns>
            </asp:GridView>
            <div style="text-align: center; width: 100%; margin-top: 10px; margin-bottom: 30px;">
                <asp:Button ID="btnActualizarTodo" runat="server" CssClass="button" ClientIDMode="Static"
                    Text="Actualizar" OnClick="actualizaInfo_Click" />
            </div>
        </center>
    </div>
</asp:Content>
