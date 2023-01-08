<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="EdicionSolicitud.aspx.cs" Inherits="Vistas_EdicionSolicitud" %>
    
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../css/jqueryUItheme/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function format(input) {
            var num = input.value.replace(/\,/g, '');
            num = num.replace(/\,/g, '.');
            if (!isNaN(num)) {
                //num = num.replace(/\./g, ',');}
                var valorTotal;
                num = num.toString().split('').reverse().join('').replace(/(?=\d*\,?)(\d{3})/g, '$1,');
                num = num.split('').reverse().join('').replace(/^[\,]/, '');
                var text = num.split('.');
                if (text.length > 1) {
                    var t2 = text[1];
                    t2 = t2.replace(',', '').replace(',', '').replace(',', '');
                    valorTotal = text[0] + '.' + t2;
                } else {
                valorTotal = num;
                }
                input.value = valorTotal;
            }

            else {
                //alert('Solo se permiten numeros');
                var splittext = input.value.split('.');
                var splitlargo = splittext.length;
                var text = splittext[0] + '.' + splittext[1];
                input.value = text;
            }
        }

//        function format(input) {
//            var num = input.value.replace(/\./g, '');
//            num = num.replace(/\,/g, '.');
//            if (!isNaN(num)) {
//                num = num.replace(/\./g, ',');
//                num = num.toString().split('').reverse().join('').replace(/(?=\d*\.?)(\d{3})/g, '$1.');
//                num = num.split('').reverse().join('').replace(/^[\.]/, '');
//                input.value = num;
//            }

//            else {
//                alert('Solo se permiten numeros');
//                input.value = input.value.replace(/[^\d\.]*/g, '');
//            }
//        }

        function allowOnlyNumber(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="False">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ConnectionStrings:skbergeConnectionString%>">
            </asp:SqlDataSource>
            <div class="Titulo" id="titEdiSoli">
                Listado de Solicitudes de Cotizacion
            </div>
            <div id="mjsError" runat="server" clientidmode="Static" style="color:Red; font-weight: bold">
                <strong></strong>
            </div>
            <div>
                <asp:Panel ID="panelBuscador" runat="server">
                    <table>
                        <tr>
                            <td class="tdBuscaRepuesto" runat="server" clientidmode="Static" id="tdbuscasoli">
                                <table>
                                    <tr>
                                        <td>
                                            Seleccione Marca
                                        </td>
                                        <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                            <asp:DropDownList Width="150px" ID="combomarcas" runat="server" ClientIDMode="Static"
                                                AutoPostBack="false" TabIndex="1">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            Estado Solicitud
                                        </td>
                                        <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                            <asp:DropDownList Width="150px" ID="ddlEstadoSol" runat="server" ClientIDMode="Static"
                                                AutoPostBack="false" TabIndex="1">
                                                <asp:ListItem Value="">...</asp:ListItem>
                                                <asp:ListItem Value="C">Cerrada</asp:ListItem>
                                                <asp:ListItem Value="A">Abierta</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Numero de Solicitud
                                        </td>
                                        <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                            <asp:TextBox ID="txtnumsoli" Width="150px" runat="server" MaxLength="25" ClientIDMode="Static"
                                                AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Fecha Desde
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtdesde" runat="server" Width="80px" 
                                                Style="text-transform: uppercase" 
                                                ></asp:TextBox>
                                            <asp:ImageButton runat="Server" ID="btnDesde" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                                            <asp:CalendarExtender ID="fecDesde" runat="server" TargetControlID="txtdesde" PopupButtonID="btnDesde"
                                                Format="yyyy-MM-dd" />
                                        </td>
                                        <td>
                                            Fecha Hasta
                                            <asp:TextBox ID="txtHasta" runat="server" Width="80px" Style="text-transform: uppercase" 
                                               ></asp:TextBox>
                                            <asp:ImageButton runat="server" ID="btnHasta" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                                            <asp:CalendarExtender ID="fechasta" runat="server" TargetControlID="txtHasta" PopupButtonID="btnHasta"
                                                Format="yyyy-MM-dd" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Codigo Repuesto
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodRep" runat="server" MaxLength="25" ClientIDMode="Static" Width="150px"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                        <td align="center">
                                            VIN
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtVin" runat="server" MaxLength="17" ClientIDMode="Static" Width="150px"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Usuario Creador
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtusuario" runat="server" MaxLength="15" Width="150px" ClientIDMode="Static"
                                                AutoCompleteType="Disabled" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td colspan="2" align="center">
                                            <asp:Button ID="btnBuscarSoli" runat="server" Text="Buscar" class="button" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="PanelEdicion" runat="server">
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td colspan="6">
                                            <strong>Datos Generales Solicitud</strong><br />
                                            <hr />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Numero de Solicitud
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtnumero" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                                Enabled="false" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Marca
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtmarca" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                                Enabled="false"></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Fecha Solicitud
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtfecsol" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                                Enabled="false"></asp:TextBox>
                                        </td>
                                        <td>
                                            Usuario
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtusser" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                                Enabled="false"></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Tipo de Solicitud
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txttipnew" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                                Enabled="false"></asp:TextBox>
                                            <asp:DropDownList Width="120px" ID="CombotipoSolici" runat="server" Visible="false">
                                                <asp:ListItem Value="0">Seleccione</asp:ListItem>
                                                <asp:ListItem Value="normal">Normal</asp:ListItem>
                                                <asp:ListItem Value="garantia">Garantía</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            Tipo de Envio
                                        </td>
                                        <td style="font-family: Arial, Helvatica, Sans-Serif" class="style1">
                                            <asp:TextBox ID="txttransnew" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                                Enabled="false"></asp:TextBox>
                                            <asp:DropDownList Width="120px" ID="Combotipotrans" runat="server" Visible="false">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnedittip" runat="server" ClientIDMode="Static" Height="30px" Text="Editar Tipo"
                                                CssClass="button" />
                                            <%--                                            <asp:Button ID="btnCancelar" runat="server" ClientIDMode="Static" Height="30px" Text="Cancelar"
                                                CssClass="button" />--%>
                                        </td>
                                    </tr>
                                    <tr id="nuevo">
                                        <td colspan="5">
                                            <strong>Agregar Repuesto</strong><br />
                                            <hr />
                                        </td>
                                    </tr>
                                    <tr id="nuevo2">
                                        <div id="mjserroradd" runat="server" clientidmode="Static">
                                        </div>
                                        <td>
                                            <asp:Label ID="lblcodigo" Text="Codigo Repuesto" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox MaxLength="25" ID="txtCodigo" runat="server" Width="120px" ClientIDMode="Static"
                                                AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblDescripcion" Text="Descripcion" Visible="false" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox MAs="100" ID="txtdescrip" runat="server" Visible="false" Width="120px"
                                                ClientIDMode="Static" AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                        <td align="center">
                                            <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Height="30px" Text="Nuevo Rep."
                                                CssClass="button" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCantidad" Text="Cantidad" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox CssClass="txtCantidad" onkeypress="return allowOnlyNumber(event);" ID="txtCantidad" runat="server" MaxLength="3"
                                                Width="50px" ClientIDMode="Static" TabIndex="3"></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnAddNew" runat="server" ClientIDMode="Static" Height="30px" Text="Agregar"
                                                CssClass="button" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblVin" Text="Vin" runat="server"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox CssClass="txtVin" ID="txtVinAdd" runat="server" MaxLength="17" Width="120px"
                                                ClientIDMode="Static" TabIndex="3">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            </asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">
                                            <hr />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <p style="text-align: center; font-size: medium; font-weight: bold">
                        Detalle de Solicitud
                    </p>
                    <hr />
                    <div id="divgrid" style="overflow: auto; width: 770px; height: 300px">
                        <asp:GridView ID="gvDetalleSolicitud" runat="server" AutoGenerateColumns="false" 
                            OnRowDataBound="gvDetalleSolicitud_RowDataBound"
                            BackColor="White" GridLines="Vertical" Font-Size="12px" Width="100%" CellPadding="3"
                            BorderColor="GreenYellow" Caption="" OnRowCommand="editLine" ShowFooter="true">
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="Brown" Font-Bold="true" ForeColor="White" Font-Names="Tahoma"
                                Font-Size="X-Small" />
                            <AlternatingRowStyle BackColor="Gainsboro" />
                            <Columns>
                                <asp:TemplateField Visible="true" HeaderText="Numero">
                                    <ItemTemplate>
                                        <asp:Label ID="numsoli" runat="server" Text='<%# Eval("numeroSolicitud") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="20px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Codigo">
                                    <ItemTemplate>
                                        <asp:Label ID="codrepto" runat="server" Text='<%# Eval("codRepto") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="20px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Descripcion">
                                    <ItemTemplate>
                                        <asp:Label ID="desc" runat="server" Text='<%# Eval("descripcion") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="25px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cantidad" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtcantgrid" onkeypress="return allowOnlyNumber(event);" ReadOnly="false" runat="server" Text='<%# Eval("cantidad") %>'
                                            Width="20px"></asp:TextBox>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Precio P/U" FooterStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtvalorgrid" ReadOnly="false" runat="server"
                                            Width="70px" onkeyup="format(this)"></asp:TextBox>
                                    </ItemTemplate>
                                    <FooterStyle Font-Bold="True" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="VIN">
                                    <ItemTemplate>
                                        <asp:Label ID="vin" runat="server" Text='<%# Eval("vin") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="25px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Envio">
                                    <ItemTemplate>
                                        <asp:Label ID="envio" runat="server" Text='<%# Eval("envio") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="85px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Dias Envio">
                                    <ItemTemplate>
                                        <asp:Label ID="dias" runat="server" Text='<%# Eval("dias") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="55px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="true" HeaderText="Tipo Solicitud">
                                    <ItemTemplate>
                                        <asp:Label ID="tipo" runat="server" Text='<%# Eval("tipo") %>'></asp:Label>
                                    </ItemTemplate>
                                     <FooterTemplate>
                                         <b>Total :</b>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="80px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>

                                 <asp:TemplateField Visible="true" HeaderText="Sub Total">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSubTotal" runat="server" ></asp:Label>
                                    </ItemTemplate>
                                    <%--<ItemTemplate>
                                        <asp:Label ID="subtotal" runat="server" Text='<%# GetUnitPriceDecimal(decimal.Parse(Eval("precio_Solicitud").ToString())).ToString("N2") %>'></asp:Label>
                                    </ItemTemplate>--%>
                                    <FooterTemplate>
                                        <b><%# GetTotal() %></b>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="80px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>

                                
                                <asp:ButtonField ButtonType="Image" HeaderText="Quitar" ImageUrl="~/img/quitar.png"
                                    ItemStyle-HorizontalAlign="Center" CommandName="sacar" Visible="true" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:ButtonField>

                                <asp:TemplateField HeaderText="Comentario" FooterStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtvalorgridCome" ReadOnly="false" runat="server" Text='<%# Eval("Comentario") %>'
                                            Width="70px"></asp:TextBox>
                                    </ItemTemplate>
                                    
                                    <FooterStyle Font-Bold="True" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Rechazado">
                                    <ItemTemplate>
                                        <asp:CheckBox id="anulada" runat="server" Checked='<%#Convert.ToBoolean(Eval("anulada")) %>' />
                                        
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="55px" VerticalAlign="Middle" Wrap="true" />
                                </asp:TemplateField>

                                 


                            </Columns>
                        </asp:GridView>
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="../img/boton-cambios.jpg" onclick="ImageButton1_Click" />
                        <br />
                        <table align="right">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnTerminar" runat="server" ClientIDMode="Static" Height="30px" Text="Terminar"
                                        CssClass="button" />
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                                <td align="center">
                                    <asp:Button ID="btnCancelar" runat="server" ClientIDMode="Static" Height="30px" Text="Cancelar"
                                        CssClass="button" OnClientClick="return confirm('Desea Cancelar Edicion');" OnClick="CancelarEdicion" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <asp:Panel ID="PanelListado" Visible="true" runat="server" style="overflow-x: auto; overflow-y: auto;">
                    <p style="text-align: center; font-size: large; font-weight: bold">
                        Listado de Solicitudes de Cotizacion
                    </p>
                    <hr />
                    <div id="divGrd" style="width: 1000px; height: 550px">
                        <asp:GridView ID="gvListSolicitud" runat="server" AutoGenerateColumns="false" BackColor="White"
                            GridLines="Vertical" Font-Size="12px" widht="900px" CellPadding="3" BorderColor="#999999"
                            Caption="" OnRowCommand="editLinea" Width="770px">
                            <FooterStyle BackColor="SkyBlue" ForeColor="Black" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#000084" Font-Bold="true" ForeColor="White" Font-Names="Tahoma"
                                Font-Size="Small" />
                            <AlternatingRowStyle BackColor="Gainsboro" />
                            <Columns>
                                <asp:TemplateField Visible="true" HeaderText="Numero">
                                    <ItemTemplate>
                                        <asp:Label ID="numeroSolicitud" runat="server" Text='<%#Eval("numeroSolicitud") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="30px" Height="20px" VerticalAlign="Middle"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Fecha">
                                    <ItemTemplate>
                                        <asp:Label ID="fecha" runat="server" Text='<%#Eval("fecha") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="60px" Height="10px" VerticalAlign="Middle"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Marca">
                                    <ItemTemplate>
                                        <asp:Label ID="marca" runat="server" Text='<%# Eval("marca") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="40px" Height="10px" VerticalAlign="Middle"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="false" HeaderText="Codigo">
                                    <ItemTemplate>
                                        <asp:Label ID="codRepto" runat="server" Text='<%# Eval("codRepto") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="25px" Height="10px" VerticalAlign="Middle"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="false" HeaderText="Detalle">
                                    <ItemTemplate>
                                        <asp:Label ID="descripcion" runat="server" Text='<%# Eval("descripcion") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="20px" Height="10px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="false" HeaderText="Cantidad">
                                    <ItemTemplate>
                                        <asp:Label ID="cantidad" runat="server" Text='<%# Eval("cantidad") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="20px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Concesionario">
                                    <ItemTemplate>
                                        <asp:Label ID="concesionario" runat="server" Text='<%# Eval("concesionario") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="50px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Tipo">
                                    <ItemTemplate>
                                        <asp:Label ID="tipo" runat="server" Text='<%# Eval("tipo") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="40px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="VIN">
                                    <ItemTemplate>
                                        <asp:Label ID="vin" runat="server" Text='<%# Eval("vin") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="50px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Transporte">
                                    <ItemTemplate>
                                        <asp:Label ID="envio" runat="server" Text='<%# Eval("envio") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="20px" Width="40px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="true" HeaderText="Dias">
                                    <ItemTemplate>
                                        <asp:Label ID="dias" runat="server" Text='<%# Eval("dias") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="20px"
                                        Wrap="true" />
                                </asp:TemplateField>

                                <asp:TemplateField Visible="false" HeaderText="Precio">
                                    <ItemTemplate>
                                        <asp:Label ID="precio" runat="server" Text='<%# Eval("precio_Solicitud") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="20px"
                                        Wrap="true" />
                                </asp:TemplateField>
                                <asp:TemplateField Visible="True" HeaderText="Estado">
                                    <ItemTemplate>
                                        <asp:Label ID="Estado" runat="server" Text='<%# Eval("Estado") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="15px" Width="20px"
                                        Wrap="True" />
                                </asp:TemplateField>

                                <asp:ButtonField ButtonType="Link" HeaderText="Editar" ItemStyle-HorizontalAlign="Center"
                                    Text="Editar" CommandName="editar" Visible="true" />
                                <asp:ButtonField ButtonType="Link" HeaderText="Copiar" ItemStyle-HorizontalAlign="Center"
                                    Text="Copiar" CommandName="copiar" Visible="true" />
                                <asp:ButtonField ButtonType="Image" ImageUrl="~/img/quitar.png" HeaderText="Eliminar"
                                    ItemStyle-HorizontalAlign="Center" CommandName="sacar" Visible="true" />

                            </Columns>
                        </asp:GridView>
                        <br />
                        
                    </div>

                    
                    <br />
                </asp:Panel>
                <table align="left">
                    <tr>
                        <td align="left">
                            &nbsp;
                            <asp:Button runat="server" ID="btnExportExell" Text="Excel" class="button" OnClick="btnExportExell_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="PanelPDF" runat="server" Visible="true">
                    <table>
                        <tr>
                            <td>
                                <asp:Button runat="server" ID="btnCreaPdf" Text="PDF" class="button" OnClick="btnCreaPdf_Click" />&nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnCreaPdf" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
