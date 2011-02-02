; (function ($) {
    $.fn.CswLogin = function (options) {

        var o = {
            AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/Authenticate',
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

                authenticate($('#login_accessid').val(),
                             $('#login_username').val(),
                             $('#login_password').val(),
                             o.onAuthenticate);
            });

        }

        function authenticate(AccessId, UserName, Password, onsuccess) {
            CswAjaxJSON({
                url: o.AuthenticateUrl,
                data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
                success: function (data) {
                    auth = data.AuthenticationStatus;
                    if(auth == 'Authenticated')
                    {
                        SetUsername(UserName);
                        $LoginDiv.remove();
                        onsuccess(UserName);
                    }
                    else 
                    {
                        _handleAuthenticationStatus(auth);
                    }
                } // success{}
            });
        } // authenticate()
        
        function _handleAuthenticationStatus(status)
        {
            alert(status);
            $('#login_button').val('Login')
                              .attr('disabled', '');
            //Logout();

        } // _handleAuthenticationStatus()


        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

