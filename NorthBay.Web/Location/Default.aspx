﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Masterpage/Main.master" AutoEventWireup="true"  Debug="true" Inherits="NorthBay.Web.Location.Location_Default" Codebehind="Default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  
    <div  style="margin:5px;">
    <%--Display North Bay hospital location information with hospital pictures--%>
<h2 style="font-weight:bold; font-size: large;" >North Bay Hospital Locations</h2>
<br /><br />
    <asp:Listview ID="lv_locations" runat="server">
   
        <ItemTemplate>

        
         <table>
                <tr style="padding:10px;">
                    <td>
                        <asp:ImageButton ID="img_location"  ImageAlign="TextTop"  Width="300px" BorderWidth="3px" BorderColor="Gray" ImageUrl='<%#String.Format("{0}{1}", "../App_Themes/Main/Images/",Eval("ImageUrl")) %>'
                        PostBackUrl='<%#String.Format("Directions.aspx?toAddress={0}{1}{4}{2}{4}{3}",Eval("Address"),Eval("City"),Eval("Province"),"Canada",", ")%>'
                        runat="server" />
                       <br /><br /><br />
                    </td>
                    <td  style="padding-left:50px;padding-right:30px;" >
                   
                        <asp:Label ID="lblName" Font-Size="14px" Font-Bold="true" runat="server"  Text='<%# Eval("Name") %>' />                       
                        <br />                   
                        <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("Address") %>' />
                        <br />
                        <asp:Label ID="lblCity" runat="server" Text='<%# Eval("City") %>' />
                        <br />
                        <asp:Label ID="lblProvince" runat="server" Text='<%# Eval("Province") %>' />           
                        <br />
                        <asp:Label ID="lblPcode" runat="server" Text='<%# Eval("Pcode") %>' />
                        <br />
                        <asp:Label ID="lblTnumber" runat="server" Text='<%# Eval("Tnumber") %>' />
                        <br />
                        <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' />
                    <br /> <br /> <br />
                    </td>
                    <td>
                      
                        <asp:Button ID="btn_directions" runat="server" Text=" Get Directions " PostBackUrl='<%#String.Format("Directions.aspx?toAddress={0}{1}{4}{2}{4}{3}",Eval("Address"),Eval("City"),Eval("Province"),"Canada",", ")%>' />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
       <%-- Display a message if there are no records in the database --%>
        <EmptyDataTemplate>
            No Locations found ...!</EmptyDataTemplate>
    </asp:Listview>
    
</div>
  
</asp:Content>
