<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorGrupoMaterial_Web.aspx.cs" Inherits="Vistas_MantenedorGrupoMaterial_Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor de Grupo Material Web
    </div>
<div class="mantenedorUsuariosEditarIzquierda">
        <center><b><u>Mantención Grupo Material</u></b></center>
        <ul>
             <li>
                <asp:HyperLink ID="ActualizaGMW" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/MantenedorGrupoMaterial_Web.aspx">Habilitar, Deshabilitar
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="InsertaGMW" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/MantenedorGrupoMaterial_Web_Insertar.aspx">Insertar
                </asp:HyperLink>
            </li>
           
        </ul>
    </div>

 <div class="mantenedorConcesionariosDerecha">


     <asp:GridView ID="GridViewGMWeb" runat="server" 
         AllowPaging="True" AutoGenerateColumns="False" 
         DataKeyNames="ID_GM_Web,Codigo_GM_Web,GM_Web" DataSourceID="sqlDGrupMaterial" OnRowCommand="GridViewGMWeb_RowCommand">
         <RowStyle BackColor="#F7F6F3" ForeColor="#333333"  />
         <Columns>
             <asp:BoundField DataField="ID_GM_Web" HeaderText="ID_GM_Web" 
                 ReadOnly="True" SortExpression="ID_GM_Web" InsertVisible="False" 
                 Visible="False" />
             <asp:BoundField DataField="Codigo_GM_Web" HeaderText="Codigo Material Web" ReadOnly="True"
                 SortExpression="Codigo_GM_Web" />
             <asp:BoundField DataField="Desc_GM_Web" HeaderText="Descripción" 
                 SortExpression="Desc_GM_Web" />
             <asp:BoundField DataField="GM_Web" HeaderText="Grupo Material SAP" ReadOnly="True" 
                 SortExpression="GM_Web" />
             <asp:BoundField DataField="Flag_MG_web" HeaderText="Habilitado" SortExpression="Flag_MG_web">
               </asp:BoundField>
              <asp:ButtonField ButtonType="Image" ImageUrl="~/img/cambia2.jpg" CommandName="activa" Text="" HeaderText="Activa/Desactiva" ControlStyle-CssClass="btnElimina" ItemStyle-HorizontalAlign="Center" />                    
                 </Columns>
         <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
     </asp:GridView>
     <asp:SqlDataSource ID="sqlDGrupMaterial" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
         ProviderName="System.Data.SqlClient" 
          >
         
    </asp:SqlDataSource>
 </div>
</asp:Content>

