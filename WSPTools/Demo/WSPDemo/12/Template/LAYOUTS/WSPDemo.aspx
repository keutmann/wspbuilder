<%@ Page Language="C#" MasterPageFile="~/_layouts/simple.master" %>
<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>

<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server" >
    WSPDemo
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server" >
    <h1>Hello World!</h1>
</asp:Content>

