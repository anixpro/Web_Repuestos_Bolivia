<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorFobValores.aspx.cs" Inherits="Vistas_MantenedorFobValores" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<style>
        .ColumnaOculta {display:none;}
        .auto-style1 {
            height: 26px;
        }

        div.scrollmenu {
              overflow: auto;
            }
</style>
    <div class="titulo">
        Mantenedor valores FOB
    </div>

    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Opciones</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobCargaMasiva.aspx">Carga masiva</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobValores.aspx">Mantenedor SKU</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink2" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobParidad.aspx">Mantenedor Paridad</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink3" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobVolumenes.aspx">Mantenedor Volumenes</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink4" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobFactorUtilidad.aspx">Mantenedor factores de utilidad</asp:HyperLink>
            </li>
        </ul>
    </div>

    <div id="mjsError" runat="server" clientidmode="Static" style="background-color:coral">
    </div>

    <div class="mantenedorConcesionariosDerecha scrollmenu">
        <b><u>Listar SKU, filtrados por código</u></b>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Código :
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtCodigo" runat="server" ClientIDMode="Static" MaxLength="20"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:GridView ID="dgvMateriales" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" 
            OnRowEditing="dgvMateriales_RowEditing" 
            OnRowDeleting="dgvMateriales_RowDeleting"            
            OnPageIndexChanging="dgvMateriales_PageIndexChanging"
            OnRowUpdating="dgvMateriales_RowUpdating" 
            OnRowCancelingEdit="dgvMateriales_RowCancelingEdit" 
            OnRowCommand="dgvMateriales_RowCommand"
            EnableModelValidation="True" Style="text-align:center;"
            EmptyDataText="No hay registros en este estado" CellPadding="10" 
            ForeColor="#333333" GridLines="None" CssClass="tablaMarcas">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="campo0" HeaderText="Marca" ReadOnly/>
                <asp:BoundField DataField="campo1" HeaderText="Código" ReadOnly/>

                <asp:TemplateField HeaderText="Descripcion">
                    <ItemTemplate>
                        <asp:Label ID="lblDescripcion" runat="server" Text='<%# Eval("campo2") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtDescripcion" runat="server" Text='<%# Eval("campo2") %>' EnableViewState="true"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="FOB">
                    <ItemTemplate>
                        <asp:Label ID="lblValor" runat="server" Text='<%# Eval("campo3") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtValor" runat="server" Text='<%# Eval("campo3") %>' EnableViewState="true" size="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Volumen">
                    <ItemTemplate>
                        <asp:Label ID="lblVolumen" runat="server" Text='<%# Eval("campo4") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtVolumen" runat="server" Text='<%# Eval("campo4") %>' EnableViewState="true" size="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Unidad de medida">
                    <ItemTemplate>
                        <asp:Label ID="lblUnidadMedida" runat="server" Text='<%# Eval("campo5") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtUnidadMedida" runat="server" Text='<%# Eval("campo5") %>' EnableViewState="true" size="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Moneda">
                    <ItemTemplate>
                        <asp:Label ID="lblMoneda" runat="server" Text='<%# Eval("campo6") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtMoneda" runat="server" Text='<%# Eval("campo6") %>' EnableViewState="true" size="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Grupo">
                    <ItemTemplate>
                        <asp:Label ID="lblGrupoTecnico" runat="server" Text='<%# Eval("campo7") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                            <asp:TextBox ID="txtGrupoTecnico" runat="server" Text='<%# Eval("campo7") %>' EnableViewState="true" size="4"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

               <asp:CommandField ShowEditButton="True" buttontype="Image" editimageurl="~/img/edit.png" UpdateImageUrl="~/img/cambia2.jpg" CancelImageUrl="~/img/quitar.png" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" />
               <asp:CommandField ShowDeleteButton="True" buttontype="Image" DeleteImageUrl="~/img/quitar.png"/>
               <%--
               <asp:ButtonField ButtonType="Image" ImageUrl="~/img/quitar.png" CommandName="eliminar" HeaderText="&nbsp;" ControlStyle-CssClass="btnElimina" ItemStyle-HorizontalAlign="Center" />
               --%>

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