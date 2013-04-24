<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage/Main.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NorthBay.Web.Publication.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="width:1024px">
<div style="margin:0 auto"><h2>Publication and Mail Page</h2></div>
<div style="float:left; margin:5px;">
        <br />
        <asp:GridView ID="gridView" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" 
        DataKeyNames="PublicationId" CellPadding="4" ForeColor="#333333" 
        GridLines="None">
            <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:TemplateField HeaderText="Publication Title" SortExpression="Title">
                <ItemTemplate>
                    <asp:LinkButton ID="linkButtonPub" runat="server" onclick="linkButtonPub_Click" CommandArgument='<%#Eval("PublicationId")%>' > <%#Eval("Title") %></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>
    <br />
    <br />
    </div>
    <div style="float:left; margin:15px 5px 5px 20px; width:250px">
    <asp:Label ID="lblPublicationDetail" runat="server" Text="Details" Font-Bold="true" />
    <br />
    <asp:Label ID="lblPublicationTitle" runat="server" text='<%#Eval("PublicationTitle")%>'/>
    <br />
    <br />
    <asp:Label ID="lblPublicationDescription" runat="server" />
    <br />
    <br />
    </div>
    <div style="clear:both; margin:15px 5px 5px 20px;">
    <asp:Label ID="lblMailFriend" runat="server" Text="Mail to a friend" />
    <br />
   
    <asp:TextBox ID="txtMailTo" runat="server" />
    <asp:RegularExpressionValidator ID="revMail" runat="server" Text="Invalid email" ControlToValidate="txtMailTo" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="Please enter a valid email address"/>
    <br />
    <asp:Label ID="lblMsg" runat="server" Text="Recommend Publications to a friend" />
    <br />
    <asp:TextBox ID="txtMsg" runat="server" TextMode="MultiLine" Rows="5" Columns="15" />
    <br />
   
    <asp:Button ID="Button1" runat="server" Text="Send Email" OnClick="Button1_Click" />
    </div>
    </div>
</asp:Content>
