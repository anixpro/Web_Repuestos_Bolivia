<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master"  CodeFile="TrackingExportExcel.aspx.cs" Inherits="Vistas_TrackingExportExcel" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>


     <!-- Fin inputs ocultos -->
            <div class="titulo" id="titCotizar">
                Tracking
            </div>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static">
            </div>
    <asp:Panel ID="PanelSeccionBusqueda" runat="server">
                <table>
                    <tr>
                        <td class="tdBuscaRepuesto" runat="server" clientidmode="Static" id="tdBuscaRepusto">
                            <table>
                                <tr>
                                    <td>
                                        Marca:
                                    </td>
                                    <td style="font-family: Arial, Helvetica, sans-serif" class="style1">
                                        <asp:DropDownList Width="120px" ID="ComboMarcas" runat="server" ClientIDMode="Static"
                                            AutoPostBack="false">
                                          
                                        </asp:DropDownList>                                       
                                    </td>
                                </tr>
                                 <tr>
                                        <td>
                                            Fecha Desde
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtdesde" runat="server" Width="80px" ></asp:TextBox>
                                            <asp:ImageButton runat="Server" ID="btnDesde" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                                            <asp:CalendarExtender ID="fecDesde" runat="server" TargetControlID="txtdesde" PopupButtonID="btnDesde"  Format="dd-MM-yyyy" />
                                        </td>
                                        <td>
                                            Fecha Hasta
                                            <asp:TextBox ID="txtHasta" runat="server" Width="80px" Style="text-transform: uppercase" 
                                               ></asp:TextBox>
                                            <asp:ImageButton runat="server" ID="btnHasta" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                                            <asp:CalendarExtender ID="fechasta" runat="server" TargetControlID="txtHasta" PopupButtonID="btnHasta"
                                                Format="dd-MM-yyyy" />
                                        </td>
                                    </tr>
                                 <tr>
                                    <td>
                                        Ultimo Evento:
                                    </td>
                                    <td style="font-family: Arial, Helvetica, sans-serif" class="style1">
                                        <asp:DropDownList Width="145px" ID="ddlLasEvent" runat="server" 
                                            ClientIDMode="Static" AutoPostBack="false" >
                                            <asp:ListItem Text="Seleccion Evento" Value=""></asp:ListItem>  
                                            <asp:ListItem Text="CONSULTA" Value="1"></asp:ListItem>  
                                            <asp:ListItem Text="AGREGA CARRO" Value ="2"></asp:ListItem>
                                            <asp:ListItem Text="COTIZA" Value="3"></asp:ListItem> 
                                            <asp:ListItem Text="PEDIDO" Value="4"></asp:ListItem> 
                                            <asp:ListItem Text="VFC" Value ="5"></asp:ListItem>
                                            <asp:ListItem Text="RESERVA" Value ="6"></asp:ListItem>

                                        </asp:DropDownList>
                                    </td>
                                </tr>                             
                                <tr>
                                    <td colspan="2" align="right">
                                        <asp:ImageButton ID="btnBuscarCodigo" runat="server" ImageUrl="~/img/buscarBtn.png"
                                            ClientIDMode="Static" TabIndex="5"  Height="25px" 
                                            onclick="btnBuscarCodigo_Click"/>
                                    </td>
                                </tr>                                                     
                            </table>
                        </td>
                        <td class="tdResultadoBusqueda" id="tdResultadoBusqueda" runat="server">
                            <!-- Div que despliega mensaje que ofrece transformar a VFC busqueda de repuesto -->                           
                            
                            <!--Fin reemplazos-->
                        </td>
                    </tr>
                </table>
            </asp:Panel>


</asp:Content>

