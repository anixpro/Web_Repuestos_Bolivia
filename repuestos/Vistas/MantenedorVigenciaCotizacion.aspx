<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="MantenedorVigenciaCotizacion.aspx.cs" Inherits="Vistas_MantenedorVigenciaCotizacion" %>

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
        Mantenedor de Plazos de Vigencia de Cotización
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
    <center>
        <b>- Modifique los campos: -</b>
    </center>
    <div class="viasUpdate">
        <center>
            <asp:GridView ID="grParametro" runat="server" Width="50%" EmptyDataText="No se encontraron registros"
                AutoGenerateColumns="false" AllowPaging="True" PageSize="20">
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <%--     <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />--%>
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="Id" InsertVisible="False" ReadOnly="True" />
                    <asp:BoundField DataField="nombre" HeaderText="Tipo" InsertVisible="False" ReadOnly="True" />
                    <asp:TemplateField HeaderText="Días" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDias" runat="server" onkeypress="return isNumberKey(event)" MaxLength="5"
                                Width="80px" Text='<%# Eval("dias") %>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div style="text-align: center; width: 100%; margin-top: 10px;">
                <asp:Button ID="Button1" runat="server" CssClass="button" ClientIDMode="Static" Text="Actualizar"
                    OnClick="actualizaInfo_Click" />
            </div>
        </center>

    </div>
</asp:Content>
