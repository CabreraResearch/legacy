﻿; (function ($) {
    $.fn.CswLogin = function (options) {

        var o = {
            AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/Authenticate',
            onAuthenticate: function(SessionId, Username) {}
        };

        if (options) {
            $.extend(o, options);
        }

        var $LoginDiv = $('<div id="logindiv" />')
                        .appendTo($(this));


        var ThisSessionId = GetSessionId();
        if(ThisSessionId != null)
        {
            
            o.onAuthenticate(ThisSessionId, GetUsername());

        } else {
            var LoginDivHtml = '  <table>' +
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
                               '  </table>';

            $LoginDiv.attr('align', 'center');
            $LoginDiv.append( LoginDivHtml );
            $('#login_accessid').focus();

            $('#login_button').click( function() {
                $(this).val('Logging in...')
                       .attr('disabled', 'true');

                authenticate($('#login_accessid').val(),
                             $('#login_username').val(),
                             $('#login_password').val(),
                             function(s, u) {
                                $LoginDiv.remove();
                                o.onAuthenticate(s, u);
                             });
            });

        }

        function authenticate(AccessId, UserName, Password, onsuccess) {
            CswAjax({
                url: o.AuthenticateUrl,
                data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
                success: function ($xml) {
                    var ThisSessionId = $xml.find('SessionId').text();
                    if (ThisSessionId != "") {
                            
                        SetSessionId(ThisSessionId);
                            
                        onsuccess(ThisSessionId, UserName);

                    } // if (SessionId != "")
                    else {
                        _handleAuthenticationStatus($xml.find('AuthenticationStatus').text());
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

