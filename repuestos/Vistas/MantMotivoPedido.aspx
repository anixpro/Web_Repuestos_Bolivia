<%@ Page Title="Mantenedor de Motivos de Pedido" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master"
CodeFile="MantMotivoPedido.aspx.cs" Inherits="Vistas_MantMotivoPedido" %>

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
    <div class="titulo" style="margin-bottom: 10px">
        Mantenedor de Motivos de Pedido
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <div class="avisoInsert">
        <center>
            <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" BackColor="#FF8000" ForeColor="#B00000"></asp:Label>
        </center>
    </div>
    <div>
        <table style="width:100%" border="0">
            <tr>
                <td style="width:120px;">
                <b>Filtro por Marca:</b>
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
                        Marca:
                    </td>
                    <td>
                        <asp:DropDownList Width="150px" ID="combomarcas" runat="server" ClientIDMode="Static"
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
                        Código:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtCodigoNuevo" Width="50px" MaxLength="3"></asp:TextBox>
                    </td>
                    <td>
                        Bloqueo:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBloqueoNuevo" Width="50px" MaxLength="2"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button runat="server" CssClass="button" ID="btnNuevoRegistro" ClientIDMode="Static" Text="Agregar" OnClick="nuevo_Click"  />
                    </td>
                </tr>
            </table>
        </center>
    </div>

    <div style="text-align: left; width: 100%; margin-top: 10px; margin-bottom: 30px;">
        <asp:Button ID="btnActualizarTodo" runat="server" CssClass="button" ClientIDMode="Static" Text="Actualizar" OnClick="actualizaInfo_Click" />
    </div>
    
    <div class="viasUpdate">
        <center>
            <asp:GridView ID="grTipos" runat="server" Width="100%" EmptyDataText="No se encontraron registros"
                AutoGenerateColumns="false" AllowPaging="false" PageSize="20" 
                OnRowCommand="grTipos_RowCommand" Style="text-align: center;">
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                <Columns>
                    <asp:TemplateField HeaderText="Descripción">
                        <ItemTemplate>
                            <center>
                                <asp:HiddenField ID="hdId" runat="server" Value='<%# Eval("ID_Motivo_pedido") %>' />
                                <asp:Label ID="lblDescripcion" runat="server" Width="80px"><%# Eval("MP_Descripcion")%></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Marca">
                        <ItemTemplate>
                            <center>
                                <asp:Label ID="lblMarca" runat="server" Width="80px"><%# Eval("mp_Marca")%></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Código">
                        <ItemTemplate>
                            <center>
                                <asp:Label ID="lblCodigo" runat="server" Width="80px"><%# Eval("MP_codigo")%></asp:Label>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Bloqueo">
                        <ItemTemplate>
                            <center>
                                <asp:TextBox ID="txtBloqueo" runat="server" onkeypress="return isNumberKey(event)" MaxLength="2"
                                    Width="80px" Text='<%# Eval("MP_Bloqueo") %>'></asp:TextBox>
                            </center>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="true" HeaderText="Habilitado">
                        <ItemTemplate>
                            <center>
                                <asp:CheckBox ID="chkHabilitado" AutoPostBack="true" OnCheckedChanged="chkHabilitado_CheckedChanged"
                                    runat="server" Checked='<%#Convert.ToBoolean(Eval("MP_Flag")) %>' />
                            </center>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="55px" VerticalAlign="Middle" Wrap="true" />
                    </asp:TemplateField>


                    <asp:TemplateField Visible="true" HeaderText="Exigir Documentación">
                        <ItemTemplate>
                            <center>
                                <asp:CheckBox ID="chkDocumentacion" AutoPostBack="true" OnCheckedChanged="chkDocumentacion_CheckedChanged"
                                    runat="server" Checked='<%#Convert.ToBoolean(Eval("mp_documentacion")) %>' />
                            </center>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="55px" VerticalAlign="Middle" Wrap="true" />
                    </asp:TemplateField>

                    <asp:TemplateField Visible="true" HeaderText="Exigir Información adicional">
                        <ItemTemplate>
                            <center>
                                <asp:CheckBox ID="chkInformacioAdicional" AutoPostBack="true" OnCheckedChanged="chkInformacioAdicional_CheckedChanged"
                                    runat="server" Checked='<%#Convert.ToBoolean(Eval("mp_informacion")) %>' />
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
