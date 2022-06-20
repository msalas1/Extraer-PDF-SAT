<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PDFExtraer._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div><asp:Label ID="Label1" runat="server" Text="Selecciona datos a extraer: "></asp:Label></div>
        <asp:CheckBox ID="chName" Text=" Nombre " runat="server" Value="NAME"/><br/> 
        <asp:CheckBox ID="chRFC" Text=" RFC " runat="server" Value="RFC" /><br/> 
        <asp:CheckBox ID="chCURP" Text=" CURP " runat="server" Value="CURP" /><br/> 
        <asp:CheckBox ID="chCP" Text=" Código Postal " runat="server" Value="CP"/><br/> 
        <asp:CheckBox ID="chSTREET" Text=" Nombre de Vialidad " runat="server" Value="STREET"/>
    </div>
    <div class="row">
        <asp:FileUpload ID="fuPDFS" runat="server" AllowMultiple="true" accept =".pdf"/>
        <br/> 
        <asp:Label ID="lblResult" runat="server" /> 
    </div>
    <br/> 
    <div class="row">
       <div style="height:50px; width:500px;">
           <asp:Button ID="btnGenerar" Text="Extraer Datos" ToolTip="Dar Clic para generar datos" runat="server" OnClick="btnGenerar_Click" accept="application/pdf" />
           <asp:Button ID="btnRefresh" Text="Limpiar" ToolTip="Dar Clic para limpiar datos" runat="server" OnClick="btnRefresh_Click" />
          
       </div>
        
        <div>
            <asp:GridView ID="GridView1" HeaderStyle-BackColor="#3AC0F2" HeaderStyle-ForeColor="White"
                runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="NAME" HeaderText="Nombre(s)" ItemStyle-Width="250" readOnly="true"/>
                    <asp:BoundField DataField="LASTNAME" HeaderText="Primer Apellido" ItemStyle-Width="150" readOnly="true" />
                    <asp:BoundField DataField="SECONDLASTNAME" HeaderText="Segundo Apellido" ItemStyle-Width="150" readOnly="true"/>
                    <asp:BoundField DataField="RFC" HeaderText="RFC" ItemStyle-Width="150" readOnly="true"/>
                    <asp:BoundField DataField="CURP" HeaderText="CURP" ItemStyle-Width="200" readOnly="true"/>
                    <asp:BoundField DataField="CP" HeaderText="Código Postal" ItemStyle-Width="150" readOnly="true"/>
                    <asp:BoundField DataField="STREET" HeaderText="Nombre de Vialidad" ItemStyle-Width="300" readOnly="true"/>
                    <asp:BoundField DataField="EXTNUMBER" HeaderText="No. Exterior" ItemStyle-Width="100" readOnly="true"/>
                </Columns>
            </asp:GridView>
            
        </div>
        <div>
            <asp:Label ID="lblShow" runat="server" ></asp:Label>
        </div>

    </div>

</asp:Content>
