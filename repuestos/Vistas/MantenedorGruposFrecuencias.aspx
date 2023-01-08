<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorGruposFrecuencias.aspx.cs" Inherits="Vistas_MantenedorGruposFrecuencias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor de Grupos Técnicos y/o Frecuencias
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Opciones</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorGruposFrecuencias.aspx">Sku</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFrecuencias.aspx">Frecuencias</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink5" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorGrupoTecnico.aspx">Grupos técnicos</asp:HyperLink><br />    
            </li>
            <li>
                <asp:HyperLink ID="HyperLink2" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorMarcasGT.aspx">Marcas</asp:HyperLink><br />    
            </li>
            <li>
                <asp:HyperLink ID="HyperLink3" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/PrecioFijoPorMaterial.aspx">Carga masiva precios fijos</asp:HyperLink><br />    
            </li>
        </ul>
    </div>
    <div class="mantenedorConcesionariosDerecha" style="width:auto;">
        <b><u>Listar SKU, filtrados por código</u></b>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Código :
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtCodigo" runat="server" ClientIDMode="Static" MaxLength="15"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:GridView ID="dgvFrecuencias" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" OnRowEditing="dgvFrecuencias_RowEditing" OnPageIndexChanging="dgvFrecuencias_PageIndexChanging"
            OnRowUpdating="dgvFrecuencias_RowUpdating" OnRowCancelingEdit="dgvFrecuencias_RowCancelingEdit"
            EnableModelValidation="True" Style="text-align:center;"
            EmptyDataText="No hay registros en este estado" CellPadding="10" 
            ForeColor="#333333" GridLines="None" CssClass="tablaMarcas">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="CODIGO_SKU" HeaderText="Código" ReadOnly/>
                <%-- <asp:BoundField HeaderText="Descripción"/> --%>
                <%-- <asp:BoundField DataField="CLASE_FRECUENCIA_DEMANDA" HeaderText="Frecuencia"/> --%>
                <%-- <asp:BoundField DataField="GRUPO_TECNICO" HeaderText="Grupo Técnico"/> --%>

                <asp:TemplateField HeaderText="Frecuencia">
                    <ItemTemplate>
                        <asp:Label ID="lblFrecuencia" runat="server" Text='<%# Eval("CLASE_FRECUENCIA_DEMANDA") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtFrecuencia" runat="server" Text='<%# Eval("CLASE_FRECUENCIA_DEMANDA") %>' EnableViewState="true"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Grupo Técnico">
                    <ItemTemplate>
                        <asp:Label ID="lblGrupoTecnico" runat="server" Text='<%# Eval("GRUPO_TECNICO") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtGrupoTecnico" runat="server" Text='<%# Eval("GRUPO_TECNICO") %>' EnableViewState="true"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Tipo">
                    <ItemTemplate>
                        <asp:Label ID="lblTipo" runat="server" Text='<%# Eval("TIPO") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtTipo" runat="server" Text='<%# Eval("TIPO") %>' EnableViewState="true"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:CommandField ShowEditButton="False" />
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
</asp:Content>

