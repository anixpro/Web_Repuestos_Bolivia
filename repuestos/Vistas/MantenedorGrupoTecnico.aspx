<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorGrupoTecnico.aspx.cs" Inherits="Vistas_MantenedorGrupoTecnico" %>

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
        <b><u>Activar / Desactivar grupos técnicos</u></b>
        <asp:GridView ID="dgvGruposTecnicos" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" OnRowEditing="dgvGruposTecnicos_RowEditing" OnRowCommand="dgvGruposTecnicos_RowCommand"
            OnRowUpdating="dgvGruposTecnicos_RowUpdating" OnRowCancelingEdit="dgvGruposTecnicos_RowCancelingEdit" OnPageIndexChanging="dgvGruposTecnicos_PageIndexChanging"
            EnableModelValidation="True" Style="text-align:center;"
            EmptyDataText="No hay registros en este estado" CellPadding="10" 
            ForeColor="#333333" GridLines="None" CssClass="tablaMarcas">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <%-- <asp:BoundField DataField="CLASE_FRECUENCIA" HeaderText="Descripción"/> --%>
                <asp:BoundField DataField="id_grupo_tecnico" HeaderText="Id" ReadOnly="True"/>
                <asp:TemplateField HeaderText="Código">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("codigo_grupo_tecnico") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtCodigoGrupoTecnico" runat="server" Text='<%# Eval("codigo_grupo_tecnico") %>' EnableViewState="true"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="clase_grupo_tecnico" HeaderText="Descripción" ReadOnly="True" ItemStyle-HorizontalAlign="Left"/>
                <asp:BoundField DataField="ACTIVO" HeaderText="¿Activo?" ReadOnly="True"/>
                <asp:ButtonField ButtonType="Image" ImageUrl="~/img/cambia2.jpg" CommandName="activo" HeaderText="Act/Desc" ControlStyle-CssClass="btnElimina" ItemStyle-HorizontalAlign="Center" />
                <%-- <asp:CommandField ShowEditButton="True" /> --%>
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

