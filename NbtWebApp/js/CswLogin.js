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

                        var ThisSessionId = GetSessionId();
                        if(ThisSessionId != null)
                        {

                            o.onAuthenticate( GetUsername() );

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
                                                '      <td><input type="button" id="login_button" name="Login" value="Login" /></td>' +
                                                '    </tr>' +
                                                '  </table>')
                                                .appendTo($(this));

                            $('#login_accessid').focus();

                            $('#login_button').click( function() {
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
                                                        SetUsername(UserName);
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
                                            o.onDeauthenticate();
                                        } // success{}
                                    });                        
                    } // logout
        };

        function _handleAuthenticationStatus(status)
        {
            alert(status);
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

