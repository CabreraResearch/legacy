; (function ($) {
    $.fn.CswLogin = function (method) {
        var PluginName = 'CswLogin';

        var methods = {
            'login': function (options) {
                        var o = {
                            AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/authenticate',
                            onAuthenticate: function(Username) {}
                        };

                        if (options) {
                            $.extend(o, options);
                        }

                        var ThisSessionId = $.CswCookie('get', CswCookieName.SessionId);
                        if(ThisSessionId != undefined && ThisSessionId != '' && ThisSessionId != null)
                        {

                            o.onAuthenticate( $.CswCookie('get', CswCookieName.Username) );

                        } else {
                            var $LoginDiv = $( '<div id="logindiv" align="center" />' +
                                                '  <table>' +
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
                                                '      <td><input type="button" id="login_button" name="Login" value="Login" />' +
												'          <span id="loginmsg" class="errorcontent" display="none"></span></td>' +
                                                '    </tr>' +
                                                '    <tr>' +
                                                '      <td></td>' +
												'      <td></td>' +
                                                '    </tr>' + 
                                                '  </table></div><br/><br/><br/><div id="assemblydiv" width="100%" align="right"></div>')
                                                .appendTo($(this));

                            $('#assemblydiv').load('_Assembly.txt');
							
							$('#login_accessid').focus();

                            $('#login_button').click( function() {
                                $('#loginmsg').text('');
								$(this).val('Logging in...')
                                       .attr('disabled', 'true');

                                var AccessId = $('#login_accessid').val();
                                var UserName = $('#login_username').val();
                                var Password = $('#login_password').val();

                                CswAjaxJSON({
                                                url: o.AuthenticateUrl,
                                                data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
                                                success: function (data) {
                                                    auth = data.AuthenticationStatus;
                                                    if(auth == 'Authenticated')
                                                    {
                                                        $.CswCookie('set', CswCookieName.Username, UserName);
                                                        $LoginDiv.remove();
                                                        o.onAuthenticate(UserName);
                                                    }
                                                    else 
                                                    {
                                                        _handleAuthenticationStatus(auth);
                                                    }
                                                } // success{}
                                           });
                            }); // login_button click()

                        } // if-else(ThisSessionId != null)
                    },  // login
            'logout': function(options) {
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
        };

        function _handleAuthenticationStatus(status)
        {
			var txt = '';
            switch(status)
			{
				case 'Failed': txt = "Login Failed"; break;
				case 'Locked': txt = "Your account is locked.  Please see your account administrator."; break;
				case 'Deactivated': txt = "Your account is deactivated.  Please see your account administrator."; break;
				case 'TooManyUsers': txt = "Too many users are currently connected.  Try again later."; break;
				case 'NonExistentAccessId': txt = "Login Failed"; break;
				case 'NonExistentSession': txt = "Login Failed"; break;
				case 'Unknown': txt = "An Unknown Error Occurred"; break;
			}
			$('#loginmsg').text(txt);
			$('#login_password').val('');   // case 21303

            $('#login_button').val('Login')
                              .attr('disabled', '');
            //Logout();

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

