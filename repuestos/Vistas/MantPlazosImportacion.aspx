<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="MantPlazosImportacion.aspx.cs" Inherits="Vistas_MantPlazosImportacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="titulo">
        Mantenedor de Plazos de vías de Importación
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
                        Marca:
                    </td>
                    <td>
                        <asp:DropDownList Width="150px" ID="combomarcas" runat="server" ClientIDMode="Static"
                            AutoPostBack="false" TabIndex="1">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button runat="server" CssClass="button" ID="btnFiltrar" ClientIDMode="Static" Text="Filtrar" OnClick="filtro_Click"  />
                    </td>
                </tr>
            </table>
        </center>
    </div>
    <div class="viasUpdate">
        <center>
            <asp:GridView ID="grEnvios" runat="server" Width="70%" EmptyDataText="No se encontraron registros"
                AutoGenerateColumns="false" AllowPaging="false" PageSize="20">
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                <Columns>
                    <asp:BoundField DataField="Marca" HeaderText="Marca" InsertVisible="False" ReadOnly="True" />
                    <asp:BoundField DataField="Nombre" HeaderText="Tipo Transporte" InsertVisible="False"
                        ReadOnly="True" />
                    <asp:TemplateField HeaderText="Días">
                        <ItemTemplate>
                            <center>
                                <asp:HiddenField ID="hdId" runat="server" Value='<%# Eval("Id") %>' />
                                <asp:TextBox ID="txtDias" runat="server" onkeypress="return isNumberKey(event)" MaxLength="5"
                                    Width="80px" Text='<%# Eval("dias") %>'></asp:TextBox>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>

                      <asp:TemplateField HeaderText="FCI">
                        <ItemTemplate>
                            <center>
                                <asp:TextBox ID="txtFci" runat="server" MaxLength="5" Width="80px" Text='<%# Eval("fci") %>'></asp:TextBox>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
            <div style="text-align: center; width: 100%; margin-top: 10px; margin-bottom: 30px;">
                <asp:Button ID="btnActualizarVias" runat="server" CssClass="button" ClientIDMode="Static"
                    Text="Actualizar" OnClick="actualizaInfo_Click" />
            </div>
        </center>
    </div>
</asp:Content>
