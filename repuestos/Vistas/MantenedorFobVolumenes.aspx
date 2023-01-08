<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorFobVolumenes.aspx.cs" Inherits="Vistas_MantenedorFobVolumenes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor valores FOB
    </div>
    <div id="msjesError" runat="server" clientidmode="Static"></div>
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
    
    <div class="mantenedorConcesionariosDerecha" style="width:auto;">
        <b><u>Actualización de tramos y factores de volúmenes</u></b>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">

            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static">
                    Marca :
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlMarca" runat="server"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static">
                    Grupo técnico :
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlGrupoTecnico" runat="server"></asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Agregar tramo" OnClick="btnAgregar_Click"/>
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow>
                <asp:TableCell ColumnSpan="4">
                    <asp:GridView ID="dgvTramos" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
                        AutoGenerateColumns="False" 
                        OnRowEditing="dgvTramos_RowEditing"
                        OnPageIndexChanging="dgvTramos_PageIndexChanging"
                        OnRowUpdating="dgvTramos_RowUpdating"
                        OnRowCancelingEdit="dgvTramos_RowCancelingEdit"
                        OnRowCommand="dgvTramos_RowCommand" 
                        EnableModelValidation="True" Style="text-align:center;"
                        EmptyDataText="No hay registros." CellPadding="5" 
                        ForeColor="#333333" GridLines="None" CssClass="tablaMarcas">
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <Columns>

                            <asp:TemplateField HeaderText="Id">
                                <ItemTemplate>
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("id") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Desde">
                                <ItemTemplate>
                                    <asp:Label ID="lblDesde" runat="server" Text='<%# Eval("desde") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                        <asp:TextBox ID="txtDesde" runat="server" Text='<%# Eval("desde") %>' EnableViewState="true"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Hasta">
                                <ItemTemplate>
                                    <asp:Label ID="lblHasta" runat="server" Text='<%# Eval("hasta") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                        <asp:TextBox ID="txtHasta" runat="server" Text='<%# Eval("hasta") %>' EnableViewState="true" size="4"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Factor">
                                <ItemTemplate>
                                    <asp:Label ID="lblFactor" runat="server" Text='<%# Eval("factor") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                        <asp:TextBox ID="txtFactor" runat="server" Text='<%# Eval("factor") %>' EnableViewState="true" size="4"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:CommandField ShowEditButton="True" buttontype="Image" editimageurl="~/img/edit.png" ItemStyle-Width="5"/>
                            <%--
                            <asp:CommandField ShowDeleteButton="true" buttontype="Image" DeleteImageUrl="~/img/quitar.png" ItemStyle-Width="5"/>
                            --%>
                            <asp:ButtonField ButtonType="Image" ImageUrl="~/img/quitar.png" CommandName="elimina" ItemStyle-Width="5" ControlStyle-CssClass="btnElimina" />


                        </Columns>
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    </asp:GridView>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
</asp:Content>