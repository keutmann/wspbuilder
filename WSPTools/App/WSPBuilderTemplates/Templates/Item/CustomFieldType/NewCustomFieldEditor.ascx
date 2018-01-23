<%@ Control Language="C#" Inherits="$DefaultNamespace$.$rootname$FieldEditor, $AssemblyName$, Version=$AssemblyVersion$, Culture=neutral, PublicKeyToken=$PublicKeyToken$"    compilationMode="Always" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>

<wssuc:InputFormControl runat="server"
	LabelText="Sample Dropdown"
	>
	<Template_Control>
		<asp:Label id="LabelLookupFieldTargetListTitle" runat="server" Visible="True"/>
		<asp:DropDownList id="DdlLookupFieldTargetList" runat="server"
			AutoPostBack="True"
			Title = "Sample Dropdown"
			Visible="False"
			>
		</asp:DropDownList>
	</Template_Control>
</wssuc:InputFormControl>

