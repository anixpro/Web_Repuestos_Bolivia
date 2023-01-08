<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devoluciones.aspx.cs" Inherits="Vistas_Devoluciones" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function format(input,imput2) {
            var num = input.value.replace(/\./g, '');
            if (!isNaN(num)) {
                input.value = num;

                if (imput > imput2) {
                    alert('Supera Maximo');
                }
            }
            else {
                alert('Solo se permiten numeros');
                input.value = input.value.replace(/[^\d\.]*/g, '');
            }
        }

        function settext(val1) {

            var gridviewcontrol = document.getElementById("<%=grDetalle.ClientID%>");
            var gridviewrow = $(gridviewcontrol.rows[parseInt(index)])
            gridviewrow.find("span[id*='lblok']").text("OK");
        }

     
    
    </script>
    <style type="text/css">
        div.upload
        {
            position: relative;
            width: 40px;
            height: 12px;
            overflow: hidden;
            background: transparent url(../img/examinar.png) center center no-repeat;
            clip: rect(0px, 50px, 24px, 0px );
        }
        
        div.upload input
        {
            position: absolute;
            right: 0px;
            top: 0px;
            margin: 0;
            padding: 0;
            filter: Alpha(Opacity=0);
            -moz-opacity: 0;
            opacity: 0;
            left: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="titulo" id="titCotizar">
        Busqueda de Factura
    </div>
    <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
    </div>
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <table>
        <tr>
            <td>
                Marca
            </td>
            <td>
                <asp:DropDownList ID="drMarca" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                Numero Factura Interno
            </td>
            <td>
                <asp:TextBox ID="txtFactura" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnBuscarF" runat="server" Text="Buscar" OnClick="btnBuscarF_Click" />
            </td>
            <td>
                <asp:HyperLink ID="HyperLink1" NavigateUrl="~/DocDR/Documentos/Solicitudes.pdf" runat="server" Target="_blank" ImageUrl="~/img/ayuda.png"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <!--Cabecera-->
    <asp:Panel ID="Panel1" runat="server" Visible="false">
        <table>
            <tr>
                <td>
                    <b>Nombre:</b>
                </td>
                <td>
                    <asp:Label ID="lblNombre" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <b>Sociedad:</b>
                </td>
                <td>
                    <asp:Label ID="lblSociedad" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Rut:</b>
                </td>
                <td>
                    <asp:Label ID="lblRut" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <b>Pedido:</b>
                </td>
                <td>
                    <asp:Label ID="lblPedido" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Factura:</b>
                </td>
                <td>
                    <asp:HyperLink ID="hypFactura" runat="server" Target="_blank">Ver</asp:HyperLink>
                </td>
                <td>
                    <b>Fecha Factura:</b>
                </td>
                <td>
                    <asp:Label ID="lblFecFac" runat="server" Text="Label"></asp:Label>
                </td>
                <td>
                    <b>Numero Factura Interno:</b>
                </td>
                <td>
                    <asp:Label ID="lblNumeroFactura" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="HdnZona" runat="server" />
        <asp:HiddenField ID="HdnPrefijo" runat="server" />
        <asp:HiddenField ID="HdnUrl" runat="server" />
        <div>
            Para que su solicitud sea válida, favor indicar cantidad que quiere reclamar o devolver,
            junto con seleccionar motivo y adjuntar evidencias en caso de ser necesario.</div>
              <asp:UpdatePanel ID="UpdatePanel1" runat="server"  >
                    <ContentTemplate>
        <div style="overflow-x: auto; width: 100%">
            <div>
         
                        <asp:GridView ID="grDetalle" runat="server" AutoGenerateColumns="False" OnRowCommand="grdetalle_RowCommand" DataKeyNames="pos_factura">
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                                Font-Size="XX-Small" />
                            <Columns>
                                <asp:BoundField DataField="pos_factura" HeaderText="Posición" ReadOnly="True" />
                                <asp:BoundField DataField="cantidad_sol" HeaderText="Cantidad" ReadOnly="True" />
                                <asp:BoundField DataField="valor_neto" HeaderText="Neto" ReadOnly="True" />
                                <asp:BoundField DataField="codigo_repuesto" HeaderText="Cod. Repuesto" ReadOnly="True" />
                                <asp:BoundField DataField="descripcion_repuesto" HeaderText="Descripción" ReadOnly="True" />
                                <%--<asp:BoundField DataField="tiene_nc" HeaderText="Nota Credito" ReadOnly="True" />--%>
                                <%--<asp:BoundField DataField="nro_nc" HeaderText="Numero NC" ReadOnly="True" />--%>
                                <asp:TemplateField Visible="true" HeaderText="Documento NC">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hypDocNC" runat="server" Target="_blank" NavigateUrl='<%# Eval("doc_nc") %>'>Ver</asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--<asp:BoundField DataField="tiene_nd" HeaderText="Nota Debito" ReadOnly="True" />--%>
                                <%-- <asp:BoundField DataField="nro_nd" HeaderText="Numero ND" ReadOnly="True" />--%>
                                <asp:TemplateField Visible="true" HeaderText="Documento ND">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hypDocND" runat="server" Target="_blank" NavigateUrl='<%# Eval("doc_nd") %>'>Ver</asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Solicitud Devolución">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtCantSol" runat="server" Width="20px" onkeyup='format(this,<%# Eval("cantidad_sol") %>);'></asp:TextBox>
                                        <asp:RangeValidator ID="rgValida" runat="server" ControlToValidate="txtCantSol" ErrorMessage="Supera Maximo"
                                            MaximumValue='<%#  ((int)Eval("cantidad_sol") < 1) ? "1" : Eval("cantidad_sol") %>'
                                            MinimumValue="1" Type="Integer" ForeColor="Red"></asp:RangeValidator>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Reclamo">
                                    <ItemTemplate>
                                        <asp:RadioButton ID="rdReclamo" runat="server" GroupName="DV" AutoPostBack="true" ClientIDMode="Static"
                                            OnCheckedChanged="rdReclamo_CheckedChanged" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Devolución">
                                    <ItemTemplate>
                                        <asp:RadioButton ID="rdDevolucion" runat="server" GroupName="DV" AutoPostBack="true" ClientIDMode="Static"
                                            OnCheckedChanged="rdDevolucion_CheckedChanged" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Motivo">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="drMotivo" runat="server" Width="60px" >
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="N° Guía Remisión">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtPdq" runat="server" Width="80px"></asp:TextBox>
                                       
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Evidencias">
                                    <ItemTemplate>
                                        <%--<div class="upload input">--%>
                                        <asp:FileUpload ID="flEvidencias" runat="server" Width="200px" />
                                        <%-- </div>--%>
                                        <asp:Label ID="lblok" runat="server" Text=""></asp:Label>
                                        <%-- <asp:ImageButton ID="imgSubir" runat="server" ImageUrl="~/img/subirDR.png" 
                                            Width="16px" Height="16px" onclick="imgSubir_Click"/>--%>
                                        <%--   <asp:Button ID="saveBtn" runat="server" CommandArgument="<%# Container.DataItemIndex%>"
                                            CommandName="save" Text="OK" />--%>
                                        <asp:Image ID="Image1" ImageUrl="~/img/ayuda2.png" runat="server" ToolTip="Para subir multiples archivos se debe realizar un archivo comprimido .ZIP o .RAR" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Observación">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtObservacion" runat="server" TextMode="MultiLine" Width="90px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                              
                                <asp:ButtonField ButtonType="Image" ImageUrl="~/img/Limpiar.PNG" Text="Limpiar" CommandName="limpiar" />
                            </Columns>
                        </asp:GridView>
                 
          
            </div>
        </div>
           </ContentTemplate>

                </asp:UpdatePanel>
        <asp:Button ID="btnSolicitar" runat="server" OnClick="btnSolicitar_Click" Text="Solicitar" />
    </asp:Panel>
</asp:Content>
