<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalLogin.aspx.cs" Inherits="ChemSW.Nbt.WebPages.ExternalLogin" %>

<!doctype html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<meta http-equiv="X-UA-Compatible" content="IE=9">
		<title>Login</title>
		<!--#include file="MainIncludes.html" -->
	</head>
	<body>
		<form id="form1" runat="server">
		<div>
			Please Wait...

			<script language="javascript">
				<asp:PlaceHolder ID="JSPlaceHolder" runat="server"></asp:PlaceHolder>
			</script>

			<!--
			<form action="" method="post">
			AccessId: <input type="text" name="accessid" id="accessid" /><br/>
			Username: <input type="text" name="username" id="username" /><br/>
			Password: <input type="text" name="password" id="password" /><br/>
			<input type="submit" />
			</form>
			-->

		</div>
		</form>
	</body>
</html>