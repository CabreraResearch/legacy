/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    //var authenticateUrl = 'authenticate';

    // Called with context
    $.fn.CswLogin = function (method) {
        var pluginName = 'CswLogin';

        var methods = {
            'init': function (options) {
                var o = {
                    onAuthenticate: null, //function (Username) {}
                    onFail: null // function (errormessage) {}
                };
                if (options) Csw.extend(o, options);

                var thisSessionId = Csw.cookie.get(Csw.cookie.cookieNames.SessionId);
                var $parent = $(this);
                var parent = Csw.literals.factory($parent, {});
                var loginDiv, loginTable, loginBtn, inpAccessId, inpUserName, inpPassword, loginMsg;

                if (false === Csw.isNullOrEmpty(thisSessionId)) {
                    Csw.tryExec(o.onAuthenticate, Csw.cookie.get(Csw.cookie.cookieNames.Username));
                }
                else {
                    loginDiv = parent.div({
                        name: 'logindiv',
                        align: 'center'
                    });
                    loginTable = loginDiv.form().table({ cellalign: 'center', cellvalign: 'center' });
                    loginMsg = loginTable.cell(1, 2, 'loginmsg').hide();
                    loginTable.cell(2, 1).text('Customer ID: ').align('right');
                    inpAccessId = loginTable.cell(2, 2).align('left').input({ name: 'login_accessid', width: '120px' });
                    loginTable.cell(3, 1).text('User Name: ').align('right');
                    inpUserName = loginTable.cell(3, 2).align('left').input({
                        name: 'login_username',
                        width: '120px',
                        onChange: function () {//Case 26866/27114
                            var regex = /[^a-zA-Z0-9_]+/g;
                            var validUserName = inpUserName.val();
                            validUserName = validUserName.replace(regex, "");
                            inpUserName.val(validUserName);
                        }
                    });
                    loginTable.cell(4, 1).text('Password: ').align('right');
                    inpPassword = loginTable.cell(4, 2).align('left').input({ name: 'login_password', type: Csw.enums.inputTypes.password, width: '120px', cssclass: 'mousetrap' });
                    loginBtn = loginTable.cell(5, 2, 'login_button_cell')
                                        .align('left')
                                        .buttonExt({
                                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.right),
                                            name: 'login_button',
                                            enabledText: 'Login',
                                            disabledText: 'Logging in...',
                                            width: '100px',
                                            bindOnEnter: true,
                                            onClick: function () {
                                                loginMsg.hide().empty();
                                                Csw.cookie.set(Csw.cookie.cookieNames.CustomerId, inpAccessId.val());
                                                Csw.clientSession.login({
                                                    AccessId: inpAccessId.val(),
                                                    UserName: inpUserName.val(),
                                                    Password: inpPassword.val(),
                                                    ForMobile: false,
                                                    onAuthenticate: function (userName) {
                                                        parent.empty();
                                                        Csw.tryExec(o.onAuthenticate, userName);
                                                    },
                                                    onFail: function (txt) {
                                                        loginMsg.$.CswErrorMessage({ 'type': 'Warning', 'message': txt });
                                                        inpPassword.val('');   // case 21303
                                                        loginBtn.enable();
                                                        Csw.tryExec(o.onFail, txt);
                                                    }
                                                });
                                            } // onClick
                                        });
                    loginTable.cell(6, 2);
                    parent.br({ number: 3 });
                    parent.div({
                        name: 'assemblydiv',
                        width: '100%',
                        align: 'right'
                    });

                    $('#assemblydiv').load('_Assembly.txt');

                    $('#login_accessid').focus();
                    
                } // if-else(ThisSessionId !== null)
            } // init
        }; // methods

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
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
            logoutpath: ''
        };
        if (options) Csw.extend(o, options);
        Csw.clientSession.login(o);
    }; // login



} (jQuery));

