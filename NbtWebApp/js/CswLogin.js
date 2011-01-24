; (function ($) {
    $.fn.CswLogin = function (options) {

        var o = {
            AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/Authenticate',
        };

        if (options) {
            $.extend(o, options);
        }

        var $LoginDiv = $(this);


        var LoginDivHtml = //'<div align="center">' +
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
                           '  </table>';
                           //'</div>';

        $LoginDiv.attr('align', 'center');
        $LoginDiv.append( LoginDivHtml );

        $('#login_button').click( function() {
            $(this).attr('disabled', 'true');

            authenticate($('#login_accessid').val(),
                         $('#login_username').val(),
                         $('#login_password').val(),
                         function(s) {
                            SessionId = s;
                            $('#header_dashboard').CswDashboard({ 'SessionId': SessionId });
                            $('#header_menu').CswHeaderMenu({ 'SessionId': SessionId });
                            $('#header_username').text('admin');

                            $LoginDiv.remove();
                            $('#treediv').CswNbt({ 'SessionId': SessionId });
                         });
        });

        function authenticate(AccessId, UserName, Password, onsuccess) {
            $.ajax({
                type: 'POST',
                url: o.AuthenticateUrl,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
                success: function (data, textStatus, XMLHttpRequest) {
                    var $xml = $(data.d);
                    if ($xml.get(0).nodeName == "ERROR") {
                        _handleAjaxError(XMLHttpRequest, $xml.text(), '');
                    } else {
                        SessionId = $xml.find('SessionId').text();
                        if (SessionId != "") {

                            onsuccess(SessionId);

                        } // if (SessionId != "")
                        else {
                            _handleAuthenticationStatus($xml.find('AuthenticationStatus').text());
                        }
                    }
                }, // success{}
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //_handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                }
            });  // $.ajax({
        } // authenticate()
        
        function _handleAuthenticationStatus(status)
        {
            alert(status);
            //Logout();
        } // _handleAuthenticationStatus()


        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);





