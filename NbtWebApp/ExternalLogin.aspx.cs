using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChemSW.Nbt.WebPages
{
	public partial class ExternalLogin : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			string username = Request.Form["username"];
			string password = Request.Form["password"];
			string accessid = Request.Form["accessid"];

			if( false == String.IsNullOrEmpty( username ) &&
				false == String.IsNullOrEmpty( password ) &&
				false == String.IsNullOrEmpty( accessid ) )
			{
				Literal JSLiteral = new Literal();
				JSLiteral.Text = @"
							$(document).ready(function () {
								var dataJson = {
									AccessId: '" + accessid + @"', 
									UserName: '" + username + @"', 
									Password: '" + password + @"',
									ForMobile: false
								};

								CswAjaxJson({
											url: '/NbtWebApp/wsNBT.asmx/authenticate',
											data: dataJson,
											success: function (data) 
												{
													$.CswCookie('set', CswCookieName.Username, dataJson.UserName);
													window.location = 'Main.html';
												},
											onloginfail: function(txt) 
												{
													window.location = 'Main.html';
												},
											error: function() 
												{
													window.location = 'Main.html';
												}
								});
							});";
				JSPlaceHolder.Controls.Add( JSLiteral );
			}
			else
			{
				Response.Redirect( "Main.html" );
			}
		} // Page_Load()
	}
}