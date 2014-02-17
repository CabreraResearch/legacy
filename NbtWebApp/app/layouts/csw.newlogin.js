/// <reference path="~/app/CswApp-vsdoc.js" />
(function() {


    //NOTE: Login.aspx always uses the minified JS, so if you want to debug this or make changes, 
    //expect to require a rebuild of that file and a lot of refreshing to experience the update
    
    Csw.layouts.register('login', function(cswParent, cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate = cswPrivate || {};
            cswPrivate.username = cswPrivate.username || '';
            cswPrivate.password = cswPrivate.password || '';
            cswPrivate.accessid = cswPrivate.accessid || '';
            cswPrivate.logoutpath = cswPrivate.logoutpath || '';
            cswPrivate.onFail = cswPrivate.onFail || function () { };
            cswPrivate.onSuccess = function () { };
            
            cswPrivate.isAuthenticated = false;
            cswPrivate.isLoaded = false;
        }());

        //define two main content divs, one for eye candy and one for login controls
           cswPrivate.splashDiv = Csw.literals.div({ $parent: cswParent, name: 'splashDiv', align: 'center'});
           cswPrivate.loginDiv = Csw.literals.div({ $parent: cswParent, name: 'loginDiv', align: 'center' });
        
        //put a big logo on the front page
           cswPrivate.splashDiv.css('margin-top', '150px').css('margin-bottom', '50px');
           cswPrivate.splashDiv.img({ src: 'Images/cisprocloud_highrez.png', width: 600 });

        cswPrivate.splashDiv.br();

        //create a loading bar with some text under it to be manipulated by the app cache events at the very bottom of this page
           cswPrivate.progressbar = Ext.create('Ext.ProgressBar', {
               id: 'cacheprogress',
               width: 600,
               height: 10,
               //cls: 'left-align',
               renderTo: cswPrivate.splashDiv[0],
               style: { marginTop: '15px' },
               animate: true,
           });
           cswPrivate.splashDiv.br();
           cswPrivate.progresstext = cswPrivate.splashDiv.span({ text: "Loading assets..." });



        //called when either appcache complete or authentication occur
        //if both the authentication and appcache loading events have fired, then redirect
           cswPrivate.checkForReload = function () {
               var reloading = false;
               if (cswPrivate.isAuthenticated && cswPrivate.isLoaded) {
                   window.location = document.URL.contains("dev") ? "Dev.html" : "Main.html";
                   reloading = true;
               }
              
               return reloading;
           };


        //use the passed in accessid, username, and password to make an authentication attempt via clientSession.login
        cswPrivate.authenticate = function (accessid, username, password) {
            Csw.clientSession.login({
                AccessId: accessid,
                UserName: username,
                Password: password,
                ForMobile: false,
                onAuthenticate: cswPrivate.onSuccess,
                onFail: cswPrivate.onFail,
                logoutpath: cswPrivate.logoutpath
            });
        };


        //if POST data has supplied login credentials, submit them immediately
        if (false == Csw.isNullOrEmpty(cswPrivate.username) 
         && false == Csw.isNullOrEmpty(cswPrivate.accessid) 
         && false == Csw.isNullOrEmpty(cswPrivate.password) ) {
            //on failure send the message back to the logoutpath
            cswPrivate.onFail = function (error) { window.location = cswPrivate.logoutpath + "?error=" + error; };

            //if authentication was a success, re-submit a GET to the page so caching can begin (AppCache 'feature' stops it from working on POSTed pages)
            cswPrivate.onSuccess = function () {
                window.location = "Login.aspx";
            };

            cswPrivate.authenticate(cswPrivate.accessid, cswPrivate.username, cswPrivate.password);
        }
        //if we've already authenticated from the if branch above, skip the login controls and check for a redirect immediately
        else if (document.cookie.contains("CswSessionId")) {
            cswPrivate.isAuthenticated = true;
            cswPrivate.checkForReload();
        //otherwise, define all of the login controls
        } else {
            var loginForm = cswPrivate.loginDiv.form();
            var loginTable = loginForm.table({});
            
            var loginMsg = loginTable.cell(1, 1).hide().propDom('colspan', 2);
            loginTable.cell(2, 1).text('Customer ID: ').align('right');
            var accessidBox = loginTable.cell(2, 2).input({ cssclass: 'required' });
            loginTable.cell(3, 1).text('Username: ').align('right');
            var usernameBox = loginTable.cell(3, 2).input({ cssclass: 'required' });
            loginTable.cell(4, 1).text('Password: ').align('right');
            var passwordBox = loginTable.cell(4, 2).input({
                cssclass: 'required',
                type: Csw.enums.inputTypes.password,
                onKeyEnter: function() {
                    loginButton.click();
                }
            });
            
            
            var onLoginButnClick = function() {
                if (loginForm.isFormValid()) {
                    loginMsg.hide().empty();
                    //Csw.cookie.set(Csw.cookie.cookieNames.CustomerId, inpAccessId.val());
                    cswPrivate.authenticate(accessidBox.val(), usernameBox.val(), passwordBox.val());
                } else {
                    loginButton.enable();
                }
            };
            
            var loginButton = loginTable.cell(5, 2).buttonExt({
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.right),
                name: 'login_button',
                enabledText: 'Login',
                disabledText: 'Logging in...',
                width: '100px',
                bindOnEnter: true,
                onClick: onLoginButnClick
            });


            //if authentication was valid, see if we're ready for a full page refresh 
               cswPrivate.onSuccess = function() {
                   cswPrivate.isAuthenticated = true;
                   //hide the login form if authenticatiion was valid but caching isn't done, so people don't try to authenticate again
                   if (false == cswPrivate.checkForReload()) {
                       loginTable.$.fadeOut();
                   }
               };

            //when an authentication attempt fails for on-page logins, post the mustard to the window
               cswPrivate.onFail = function(error) {
                   loginMsg.$.CswErrorMessage({ 'type': 'Warning', 'message': error });
                   passwordBox.val(''); // case 21303
                   loginButton.enable();
               };

        }//else (show login controls)


        //assembly information for Daily
           cswPrivate.loginDiv.br({ number: 2 });
           var assemblyDiv = cswPrivate.loginDiv.div({
               name: 'assemblydiv',
               width: '100%',
               align: 'right'
           });
           assemblyDiv.css('margin-right', '80px');
           assemblyDiv.$.load('_Assembly.txt');





        ///APPCACHE EVENTS///
        
        //when app cache is complete, fill the bar completely, and set the text under it to Done/fade it out
           var updateComplete = function() {
               cswPrivate.isLoaded = true;
               
               if (false == cswPrivate.checkForReload()) {
                   cswPrivate.progressbar.updateProgress(1);
                   cswPrivate.progresstext[0].innerHTML = 'Done!'; 
                   cswPrivate.progresstext.$.fadeTo(1000, 0);
               }
           };

        if (window.applicationCache) {
            window.applicationCache.addEventListener('noupdate', updateComplete);
            window.applicationCache.addEventListener('updateready', updateComplete);
            window.applicationCache.addEventListener('cached', updateComplete);
            //these two aren't technically "completions", but when the appCache fails we want to just continue on to the app and assume its going to fail repeatedly
               window.applicationCache.addEventListener('error', updateComplete);
               window.applicationCache.addEventListener('obsolete', updateComplete);
        } else {
            //well don't we feel stupid for doing all the work making this pretty bar for IE9
            updateComplete();
        }


        //for each downloaded file, fill the bar to the appropriate fraction
           var inProgress = function(p) {
               cswPrivate.progressbar.updateProgress((p.loaded / p.total));
           };
           if (window.applicationCache) {
               window.applicationCache.addEventListener('progress', inProgress);
           }        


        return cswPublic;

    });
}());