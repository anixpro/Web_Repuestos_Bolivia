<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="MantenedorCorreosVFC_Criticidad.aspx.cs" Inherits="Vistas_MantenedorCorreosVFC_Criticidad"
    EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="titulo">
        Mantenedor de Correos VFC - Criticidad<br />
        <br />
    </div>
    <div id="mjsError" runat="server" clientidmode="Static">
    </div>
    <div id="mjsAlert" runat="server" clientidmode="Static">
    </div>
    <div id="mjs" runat="server" clientidmode="Static">

    <table>
    <tr>
    <td>Nombre</td>
    <td>
        <asp:TextBox ID="txtnombre" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
    <td>Correo</td>
    <td>
        <asp:TextBox ID="txtcorreo" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
    <td>Marca</td>
    <td>
        <asp:DropDownList ID="drMarcas" runat="server">       
        </asp:DropDownList>
    </tr>
     <tr>
    <td></td>
    <td>
        <asp:Button ID="btnAgregar" runat="server" Text="Agregar" 
            onclick="btnAgregar_Click" />
    </tr>
    </table>

    <asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:GridView ID="gvEmails" runat="server" AllowPaging="True" AutoGenerateColumns="False"
            CellPadding="10" CssClass="floatLeft" EmptyDataText="No se encontraron Datos"
            ForeColor="#333333" GridLines="None" 
            OnRowCommand="GridViewGMWeb_RowCommand" OnRowEditing="gvEmails_RowEditing"
            OnRowCancelingEdit="gvEmails_RowCancelingEdit" OnRowUpdating="gvEmails_RowUpdating"
            Style="text-align: center;" 
            OnPageIndexChanging="gvEmails_PageIndexChanged" 
            OnRowDeleting="gvEmails_RowDeleting" 
             >
            <Columns>
                <asp:TemplateField Visible="False">
                    <ItemTemplate>
                        <asp:Label ID="idEmail" runat="server" Text='<%#Eval("idPersona_vfc") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Organizacion">
                    <ItemTemplate>
                        <asp:Label ID="lblorg" runat="server" Text='<%#Eval("orgPersona_VFC") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <asp:Label ID="lblemail" runat="server" Text='<%#Eval("emaiilPersona_vfc") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmailGrid" runat="server" EnableViewState="True" Text='<%#Eval("emaiilPersona_vfc") %>'
                            Width="150px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Analista">
                    <ItemTemplate>
                        <asp:Label ID="lblnom" runat="server" Text='<%#Eval("nomPersona_vfc") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtAnalisGrid" runat="server" EnableViewState="True" Text='<%#Eval("nomPersona_vfc") %>'
                            Width="110"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="110px" />
                </asp:TemplateField>
                
               

                <asp:CommandField ShowEditButton="True" />
                <asp:CommandField ShowDeleteButton ="True" />
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>

</ContentTemplate>
</asp:UpdatePanel>
    </div>
</asp:Content>
