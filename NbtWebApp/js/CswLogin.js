/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswLogin = function (method) {
		var PluginName = 'CswLogin';

		var methods = {
			'login': function (options) {
						var o = {
							AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/authenticate',
							onAuthenticate: function(Username) {}
						};
						if (options) $.extend(o, options);

						var ThisSessionId = $.CswCookie('get', CswCookieName.SessionId);
                        if(ThisSessionId !== undefined && ThisSessionId !== '' && ThisSessionId !== null)
						{

							o.onAuthenticate( $.CswCookie('get', CswCookieName.Username) );

						}
						else 
						{
							var $LoginDiv = $( '<div id="logindiv" align="center">' +
												'  <form>' +  
												'    <table>' +
												'    <tr>' +
                                                '      <td align="right"></td>' +
                                                '      <td id="loginmsg" style="display: none;"></td>' +
                                                '    </tr>' +
                                                '    <tr>' +
												'      <td align="right">Customer ID:</td>' +
												'      <td><input type="text" name="accessid" id="login_accessid" /></td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td align="right">User Name:</td>' +
												'      <td><input type="text" name="username" id="login_username" /></td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td align="right">Password:</td>' +
												'      <td><input type="password" name="password" id="login_password" /></td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td align="right"></td>' +
												'      <td id="login_button_cell"> '+ //<input type="submit" id="login_button" name="Login" value="Login" />' +
												'          </td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td></td>' +
												'      <td></td>' +
												'    </tr>' + 
                                                '  </table>' +
												' </form>' +
												' </div>' +
												' <br/><br/><br/>' +
												' <div id="assemblydiv" width="100%" align="right"></div>')
                                            .appendTo($(this));

							$('#assemblydiv').load('_Assembly.txt');
							
							$('#login_accessid').focus();

							var $loginbutton = $('#login_button_cell').CswButton({ID: 'login_button', 
														enabledText: 'Login', 
														disabledText: 'Logging in...', 
														onclick: function() {
                                                            $('#loginmsg').hide()
																	.children().remove();

															var AccessId = $('#login_accessid').val();
															var UserName = $('#login_username').val();
															var Password = $('#login_password').val();

															function _handleAuthenticated(UserName)
															{
																$.CswCookie('set', CswCookieName.Username, UserName);
																$LoginDiv.remove();
																o.onAuthenticate(UserName);
															}

															CswAjaxJSON({
																		url: o.AuthenticateUrl,
																		data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
																		success: function (data) {
																			auth = data.AuthenticationStatus;
                                                                            if(auth === 'Authenticated')
																			{
																				_handleAuthenticated(UserName);
																			}
																			else 
																			{
                                                                                _handleAuthenticationStatus(data, _handleAuthenticated, $loginbutton);
                                                                            }
                                                                        },  // success{}
																		error: function() {
																			$loginbutton.CswButton('enable');
																		}
															}); // ajax
											} // onclick
							}); // button
                        } // if-else(ThisSessionId !== null)
					},  // login

			'logout': function(options) { _Logout(options); }
		};


		function _Logout(options) {
			var o = {
				DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
				onDeauthenticate: function() {}
			};

			if (options) {
				$.extend(o, options);
			}
						
			CswAjaxJSON({
							url: o.DeauthenticateUrl,
							data: "",
							success: function (data) {
								$.CswCookie('clear', CswCookieName.Username);
								o.onDeauthenticate();
							} // success{}
						});                        
		} // logout

		function _handleAuthenticationStatus(data, onAuthenticated)
		{
			var txt = '';
			var enableButton = true;
			switch(data.AuthenticationStatus)
			{
				case 'Failed': txt = "Login Failed"; break;
				case 'Locked': txt = "Your account is locked.  Please see your account administrator."; break;
				case 'Deactivated': txt = "Your account is deactivated.  Please see your account administrator."; break;
				case 'TooManyUsers': txt = "Too many users are currently connected.  Try again later."; break;
				case 'NonExistentAccessId': txt = "Login Failed"; break;
				case 'NonExistentSession': txt = "Login Failed"; break;
				case 'Unknown': txt = "An Unknown Error Occurred"; break;
				case 'ExpiredPassword': 
					enableButton = false;
					$.CswDialog('EditNodeDialog', { 
						'nodeid': data.nodeid,
						'cswnbtnodekey': data.cswnbtnodekey,
						'filterToPropId': data.passwordpropid, 
						'title': 'Your password has expired.  Please change it now:',
						'onEditNode': function(nodeid, nodekey) { onAuthenticated(); } 
					}); 
					break;
				case 'ShowLicense': 
					enableButton = false;
					$.CswDialog('ShowLicenseDialog', {
						'onAccept': function() { onAuthenticated(); },
						'onDecline': function() { _Logout(); }
					}); 
					break;
			}
			$('#loginmsg').CswErrorMessage({'message': txt });

			$('#login_password').val('');   // case 21303

            if(enableButton)
			{
				$('#login_button').CswButton('enable');
			}

		} // _handleAuthenticationStatus()


		// Method calling logic
		if ( methods[method] ) {
			return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
			return methods.init.apply( this, arguments );
		} else {
			$.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    

	}; // function(options) {
})(jQuery);

