/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { 
    "use strict";
    var authenticateUrl = '/NbtWebApp/wsNBT.asmx/authenticate';

    // Called with context
    $.fn.CswLogin = function (method) {
        var pluginName = 'CswLogin';

        var methods = {
            'init': function (options) {
                var o = {
                    onAuthenticate: null, //function (Username) {}
                    onFail: null // function (errormessage) {}
                };
                if (options) $.extend(o, options);

                var ThisSessionId = Csw.cookie.get(Csw.cookie.cookieNames.SessionId);
                if( !Csw.isNullOrEmpty(ThisSessionId) )
                {
                    o.onAuthenticate( Csw.cookie.get(Csw.cookie.cookieNames.Username) );
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
                                        '      <td><input type="text" name="login_accessid" id="login_accessid" /></td>' +
                                        '    </tr>' +
                                        '    <tr>' +
                                        '      <td align="right">User Name:</td>' +
                                        '      <td><input type="text" name="login_username" id="login_username" /></td>' +
                                        '    </tr>' +
                                        '    <tr>' +
                                        '      <td align="right">Password:</td>' +
                                        '      <td><input type="password" name="login_password" id="login_password" /></td>' +
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

                    var $loginbutton = $('#login_button_cell').CswButton({
                                                ID: 'login_button', 
                                                enabledText: 'Login', 
                                                disabledText: 'Logging in...', 
                                                onclick: function () {
                                                    $('#loginmsg').hide()
                                                            .children().remove();

                                                    _handleLogin({
                                                        AccessId: $('#login_accessid').val(), 
                                                        UserName: $('#login_username').val(), 
                                                        Password: $('#login_password').val(), 
                                                        ForMobile: false, 
                                                        onAuthenticate: function (UserName) {
                                                            $LoginDiv.remove();
                                                            if(Csw.isFunction(o.onAuthenticate)) {
                                                                o.onAuthenticate(UserName);
                                                            }
                                                        }, 
                                                        onFail: function (txt) {
                                                            $('#loginmsg').CswErrorMessage({'type': 'Warning', 'message': txt });
                                                            $('#login_password').val('');   // case 21303
                                                            $loginbutton.CswButton('enable');
                                                            if(Csw.isFunction(o.onFail)) {
                                                                o.onFail(txt);
                                                            }
                                                        }
                                                    });
                                                } // onclick
                                            }); // CswButton

                    $('#login_accessid').clickOnEnter($loginbutton);
                    $('#login_username').clickOnEnter($loginbutton);
                    $('#login_password').clickOnEnter($loginbutton);

                } // if-else(ThisSessionId !== null)
            } // init
        }; // methods

        // Method calling logic
        if ( methods[method] ) {
            return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    

    }; // function (options) {


    // Called without context
    $.CswLogin = function (options) {
        var o = {
            AccessId: '',
            UserName: '',
            Password: '',
            ForMobile: false,
            onAuthenticate: null, // function (UserName) {} 
            onFail: null, // function (errormessage) {} 
            LogoutPath: ''
        };
        if(options) $.extend(o, options);
        _handleLogin(o);				
    }; // login

    function _handleLogin(loginopts) {
        var l = {
            AccessId: '',
            UserName: '',
            Password: '',
            ForMobile: false,
            onAuthenticate: null, // function (UserName) {} 
            onFail: null, // function (errormessage) {} 
            LogoutPath: ''
        };
        if(loginopts) $.extend(l, loginopts);
        Csw.ajax.post({
                    url: authenticateUrl,
                    data: {
                            AccessId: l.AccessId, 
                            UserName: l.UserName, 
                            Password: l.Password,
                            ForMobile: l.ForMobile
                        },
                    success: function () {
                            Csw.cookie.set(Csw.cookie.cookieNames.Username, l.UserName);
                            Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, l.LogoutPath);
                            if(Csw.isFunction(l.onAuthenticate)) {
                                l.onAuthenticate(l.UserName);
                            }
                        },
                    onloginfail: function (txt) {
                            if(Csw.isFunction(l.onFail)) {
                                l.onFail(txt);
                            }
                        },
                    error: function () {
                            if(Csw.isFunction(l.onFail)) {
                                l.onFail('Webservice Error');
                            }
                        }
        }); // ajax
    } // _handleLogin()
    
})(jQuery);

