<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Promociones.aspx.cs" Inherits="Vistas_Promociones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/galleria-1.2.5.min.js" type="text/javascript"></script>
    <script src="../css/myGalleriaTheme/galleria.classic.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Se configura el carrusel de promociones
            //Galleria.loadTheme('../css/myGalleriaTheme/galleria.classic.js');
            $('#promociones').galleria({
                width: 784,
                height: 652
            });

            // Se configura dialogo de ranking
            $("#divRanking").dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width:413,
                heigth:300,
                title:'Ranking'
            });

            // Se configura dialogo de gift card
            $("#divGiftCard").dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 300,
                heigth: 300,
                title: 'Ganadores de Gift Card'
            });

            // Configuracion de click en ranking
            $("#btnRanking").click(function () {
                $("#divRanking").dialog('open');
            });

            // Configuracion de click en gift card
            $("#btnGiftCard").click(function () {
                $("#divGiftCard").dialog('open');
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Promociones
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div id="divConPromociones" runat="server">
        <div class="promocionUp">
            <div id="promociones">
                <asp:Repeater ID="rptrPromos" runat="server">
                    <ItemTemplate>
                        <img src="<%# Eval("html") %>" alt="Promocion"/>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <div id="divSinPromociones" runat="server">
        <center>
            <p>
                <b>** No hay promociones para sus marcas **</b>
            </p>
        </center>
    </div>
    <div class="homeBajoBotonAgregar">
        <input class="button" id="btnRanking" type="button" value="Ver Ranking" />
        <input class="button" id="btnGiftCard" type="button" value="Ver Ganadores GiftCard" />
    </div>
    <div id="divRanking" class="ui-widget">
        <div class="ranking">
            <asp:Repeater ID="RepeaterPromociones3" runat="server" ClientIDMode="Static" onitemdatabound="limitar">
                <ItemTemplate>
                    <asp:Label ID="lblPromo3" runat="server" ClientIDMode="Static" Text='<%# Eval("html") %>'></asp:Label><br />              
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div id="divGiftCard" class="ui-widget">
        <div class="giftCard">
            <asp:Repeater ID="RepeaterPromociones2" runat="server" ClientIDMode="Static" onitemdatabound="limitar">
                <ItemTemplate>
                    <asp:Label ID="lblPromo2" runat="server" ClientIDMode="Static" Text='<%# Eval("html") %>'></asp:Label><br />              
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div class="rankingBotonAgregar">
        <asp:Button ID="ImageButton3" runat="server" ClientIDMode="Static" 
            Text = "Editar Rankings" CssClass="button"
            PostBackUrl="~/Vistas/EditorContenido.aspx?tipo=8" />
        <asp:Button ID="ImageButton2" runat="server" ClientIDMode="Static" 
            Text = "Editar Gift Cards" CssClass="button"
            PostBackUrl="~/Vistas/EditorContenido.aspx?tipo=7" />
        <asp:Button ID="btnAdministrarPromociones" runat="server" ClientIDMode="Static"
            Text = "Administrar Promociones" CssClass="button"
            PostBackUrl="~/Vistas/AdminPromos.aspx"/>
    </div>
</asp:Content>

