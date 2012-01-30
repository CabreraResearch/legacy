/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

window.abandonHope = false;
(function (window, $, undefined) {
    'use strict';
    var document = window.document,
        navigator = window.navigator,
        location = window.location;
   
    window.ChemSW = window.Csw = (function () {

        var cswUniverse = {
            document: document,
            navigator: navigator,
            location: location,
            $: $,
            homeUrl: 'Main.html'
        };

        var methods = ['register'],
            protectedMethods = ['register', 'deregister', 'getGlobalProp', 'setGlobalProp'],
            ret = { };
       
        var register = function (name, obj, isProtected) {
            /// <summary>
            ///   Register an Object in the ChemSW namespace
            /// </summary>
            /// <param name="name" type="String"> Name of the object </param>
            /// <param name="obj" type="Object"> Object to pass </param>
            /// <param name="isProtected" type="Boolean"> If true, the object cannot be removed from the namespace </param>
            /// <returns type="Boolean">True if the object name did not already exist in the namespace.</returns>
            var succeeded = false;
            if (methods.indexOf(name) !== -1) {
                methods.push(name);
                obj[name] = true; //for shimming our own instanceof 
                if (isProtected && protectedMethods.indexOf(name) !== -1) {
                    protectedMethods.push(name);
                }
                ret[name] = obj;
                succeeded = true;
            }
            return succeeded;
        };
        ret.register = register;

        var deregister = function (name) {
            /// <summary>
            ///   Deregister an Object from the ChemSW namespace
            /// </summary>
            /// <param name="name" type="String"> Name of the object.</param>
            /// <returns type="Boolean">True if the object was removed.</returns>
            var succeeded = false;
            if (protectedMethods.indexOf (name) !== -1) {
                if (methods.indexOf (name) !== -1) {
                    methods.splice (name, 1);
                }
                delete ret[name];
                succeeded = true;
            }
            return succeeded;
        };
        register('deregister', deregister);
        ret.deregister = ret.deregister || deregister;

        var getGlobalProp = function (propName) {
            /// <summary>
            ///   Fetch a dereferenced copy of a property from the private universe collection
            /// </summary>
            /// <param name="propName" type="String"> Name of the property </param>
            /// <returns type="Object">A clone of the property.</returns>
            var retVal;
            if (propName && cswUniverse.hasOwnProperty (propName)) {
                retVal = cswUniverse[propName];
            } else {
                retVal = {};
                $.extend (retVal, cswUniverse);
            }
            return retVal;
        };
        register('getGlobalProp', getGlobalProp);
        ret.getGlobalProp = ret.getGlobalProp || getGlobalProp;

        var setGlobalProp = function (prop, val) {
            /// <summary>
            ///   Change the value of a property in the private universe collection
            /// </summary>
            /// <param name="name" type="String"> Name of the object </param>
            /// <returns type="Boolean">True if the property was updated.</returns>
            var success = false;
            if (prop && val && cswUniverse.hasOwnProperty (prop)) {
                cswUniverse[prop] = val;
                success = true;
            }
            return success;
        };
        register('setGlobalProp', setGlobalProp);
        ret.setGlobalProp = ret.setGlobalProp || setGlobalProp;
        
        var addGlobalProp = function (propName, val) {
            /// <summary>
            ///   Add a property to the private universe collection
            /// </summary>
            /// <param name="propName" type="String"> Name of the object </param>
            /// <param name="val" type="Object"> Value of the object </param>
            /// <returns type="Boolean">True if the property was added.</returns>
            var success = false;
            if (propName && val && false == cswUniverse.hasOwnProperty (propName)) {
                cswUniverse[propName] = val;
                success = true;
            }
            return success;
        };
        register('addGlobalProp', addGlobalProp);
        ret.addGlobalProp = ret.addGlobalProp || addGlobalProp;

        var getCswMethods = function () {
            /// <summary>
            ///   Fetch a dereferenced copy of the currently registered properties on the ChemSW namespace
            /// </summary>
            /// <returns type="Array">An array of property names.</returns>
            var retMethods = methods.slice (0);
            return retMethods;
        };
        register('getCswMethods', getCswMethods);
        ret.getCswMethods = ret.getCswMethods || getCswMethods;

        return ret;
                       
    }());                  
}(window, jQuery));window.abandonHope=!1;
(function(d,f){var b=d.document,a=d.navigator,e=d.location;d.ChemSW=d.Csw=function(){var c={document:b,navigator:a,location:e,$:f,homeUrl:"Main.html"},g=["register"],j=["register","deregister","getGlobalProp","setGlobalProp"],h={},d=function(c,a,e){var b=!1;-1!==g.indexOf(c)&&(g.push(c),a[c]=!0,e&&-1!==j.indexOf(c)&&j.push(c),h[c]=a,b=!0);return b};h.register=d;var i=function(c){var a=!1;-1!==j.indexOf(c)&&(-1!==g.indexOf(c)&&g.splice(c,1),delete h[c],a=!0);return a};d("deregister",i);h.deregister=
h.deregister||i;i=function(a){a&&c.hasOwnProperty(a)?a=c[a]:(a={},f.extend(a,c));return a};d("getGlobalProp",i);h.getGlobalProp=h.getGlobalProp||i;i=function(a,e){var g=!1;a&&e&&c.hasOwnProperty(a)&&(c[a]=e,g=!0);return g};d("setGlobalProp",i);h.setGlobalProp=h.setGlobalProp||i;i=function(a,e){var g=!1;a&&e&&!1==c.hasOwnProperty(a)&&(c[a]=e,g=!0);return g};d("addGlobalProp",i);h.addGlobalProp=h.addGlobalProp||i;i=function(){return g.slice(0)};d("getCswMethods",i);h.getCswMethods=h.getCswMethods||
i;return h}()})(window,jQuery);(function(){var d,f;function b(){return 0<h}function a(c){var a={url:"",data:{},onloginfail:function(){Csw.finishLogout()},success:null,error:null,overrideError:!1,formobile:!1,async:!0,watchGlobal:!0};c&&$.extend(a,c);a.watchGlobal&&(h+=1);Csw.tryExec(k,a.watchGlobal);$.ajax({type:"POST",async:a.async,url:a.url,dataType:"json",contentType:"application/json; charset=utf-8",data:JSON.stringify(a.data),success:function(c,e,g){a.watchGlobal&&(h-=1);var b=$.parseJSON(c.d);void 0!==b.error?(!1===a.overrideError&&
j(g,{display:b.error.display,type:b.error.type,message:b.error.message,detail:b.error.detail},""),Csw.tryExec(a.error,b.error)):(c=Csw.string(b.AuthenticationStatus,"Unknown"),!1===a.formobile&&Csw.setExpireTime(Csw.string(b.timeout,"")),delete b.AuthenticationStatus,delete b.timeout,Csw.handleAuthenticationStatus({status:c,success:function(){Csw.tryExec(a.success,b)},failure:a.onloginfail,usernodeid:b.nodeid,usernodekey:b.cswnbtnodekey,passwordpropid:b.passwordpropid,ForMobile:a.formobile}));Csw.tryExec(i,
!0)},error:function(c,e){a.watchGlobal&&(h-=1);Csw.log("Webservice Request ("+a.url+") Failed: "+e);Csw.tryExec(a.error);Csw.tryExec(i,!1)}})}function e(a){var c={url:"",data:{},onloginfail:function(){Csw.finishLogout()},success:null,error:null,overrideError:!1,formobile:!1,async:!0,watchGlobal:!0};a&&$.extend(c,a);c.watchGlobal&&(h+=1);isFunction(k)&&k(c.watchGlobal);$.ajax({type:"GET",async:c.async,url:c.url,dataType:"json",data:JSON.stringify(c.data),success:function(a){c.watchGlobal&&(h-=1);!1===
Csw.isNullOrEmpty(a.error)?(!1===c.overrideError&&Csw.error({display:a.error.display,type:a.error.type,message:a.error.message,detail:a.error.detail}),Csw.isFunction(c.error)&&c.error(a.error)):Csw.isFunction(c.success)&&c.success(a);isFunction(i)&&i(!0)},error:function(a,e){c.watchGlobal&&(h-=1);Csw.log("Webservice Request ("+c.url+") Failed: "+e);isFunction(c.error)&&c.error();Csw.isFunction(i)&&i(!1)}})}function c(c){var a={url:"",data:{},stringify:!1,onloginfail:function(){Csw.finishLogout()},
success:function(){},error:function(){},formobile:!1,async:!0,watchGlobal:!0};c&&$.extend(a,c);!1===Csw.isNullOrEmpty(a.url)&&(a.watchGlobal&&(h+=1),$.ajax({type:"POST",async:a.async,url:a.url,dataType:"text",data:$.param(a.data),success:function(c,e,g){a.watchGlobal&&(h-=1);var b;b=$.browser.msie?$.xml(c):$(g.responseXML).children().first();"error"===b.first().get(0).nodeName?(j(g,{display:b.CswAttrNonDom("display"),type:b.CswAttrNonDom("type"),message:b.CswAttrNonDom("message"),detail:b.CswAttrNonDom("detail")},
""),a.error()):(c=Csw.string(b.CswAttrNonDom("authenticationstatus"),"Unknown"),a.formobile||Csw.setExpireTime(b.CswAttrNonDom("timeout")),Csw.handleAuthenticationStatus({status:c,success:function(){a.success(b)},failure:a.onloginfail,usernodeid:Csw.string(b.CswAttrNonDom("nodeid"),""),usernodekey:Csw.string(b.CswAttrNonDom("cswnbtnodekey"),""),passwordpropid:Csw.string(b.CswAttrNonDom("passwordpropid"),""),ForMobile:a.formobile}))},error:function(c,e){a.watchGlobal&&(h-=1);Csw.log("Webservice Request ("+
a.url+") Failed: "+e);a.error()}}))}function g(b,g,j){j=Csw.string(j);g=Csw.string(g);switch(j){case d:b=c(b);break;default:b=g.toUpperCase()===f?e(b):a(b)}return{ajax:b}}function j(a,c){Csw.error(c)}var h=0;Csw.register("ajaxInProgress",b);Csw.ajaxInProgress=Csw.ajaxInProgress||b;var k=null;Csw.register("onBeforeAjax",k);Csw.onBeforeAjax=Csw.onBeforeAjax||k;var i=null;Csw.register("onAfterAjax",i);Csw.onAfterAjax=Csw.onAfterAjax||i;f="GET";d="xml";Csw.register("ajax",g);Csw.ajax=Csw.ajax||g})();(function(){function d(a){return $.isArray(a)}function f(){var a=[];0<arguments.length&&(a=Array.prototype.slice.call(arguments,0));a.contains=a.contains||function(b){return-1!=a.indexOf(b)};return a}function b(a,b){var c=f(),g=+a,b=+b;if(Csw.isNumber(a)&&Csw.isNumber(b))for(;g<=b;g+=1)c.push(g);return c}if(!Array.prototype.forEach)Array.prototype.forEach=function(a,b){var c="",g;if(null==this)throw new TypeError(" this is null or not defined");var j=Object(this),h=j.length>>>0;if("[object Function]"!=
{}.toString.call(a))throw new TypeError(a+" is not a function");b&&(c=b);for(g=0;g<h;){var d;g in j&&(d=j[g],a.call(c,d,g,j));g++}};if(!Array.prototype.indexOf)Array.prototype.indexOf=function(a){if(void 0===this||null===this)throw new TypeError;var b=Object(this),c=b.length>>>0;if(0===c)return-1;var g=0;0<arguments.length&&(g=Number(arguments[1]),g!==g?g=0:0!==g&&Infinity!==g&&-Infinity!==g&&(g=(0<g||-1)*Math.floor(Math.abs(g))));if(g>=c)return-1;for(g=0<=g?g:Math.max(c-Math.abs(g),0);g<c;g++)if(g in
b&&b[g]===a)return g;return-1};Csw.register("isArray",d);Csw.isArray=Csw.isArray||d;Csw.register("array",f);Csw.array=Csw.array||f;Csw.register("makeSequentialArray",b);Csw.makeSequentialArray=Csw.makeSequentialArray||b})();(function(){function d(d,b){var a=Csw.string(d).toLowerCase().trim();return"true"===a||"1"===a?!0:"false"===a||"0"===a?!1:b&&Csw.isNullOrEmpty(d)?!0:!1}Csw.register("bool",d);Csw.bool=Csw.bool||d})();(function(){function d(){g&&(c=1)}function f(){g&&(c=0)}function b(){if(g&&1===c)return"If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button."}function a(){var a=!0;g&&1===c&&(a=confirm("Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page."))&&
(c=0);return a}function e(){window.onbeforeunload=!1===Csw.isNullOrEmpty(window.onbeforeunload)?function(){var a=window.onbeforeunload;return a()?b():!1}:function(){return b()}}var c=0,g=!0;Csw.register("setChanged",d);Csw.setChanged=Csw.setChanged||d;Csw.register("unsetChanged",f);Csw.unsetChanged=Csw.unsetChanged||f;Csw.register("checkChanges",b);Csw.checkChanges=Csw.checkChanges||b;Csw.register("manuallyCheckChanges",a);Csw.manuallyCheckChanges=Csw.manuallyCheckChanges||a;Csw.register("initCheckChanges",
e);Csw.initCheckChanges=Csw.initCheckChanges||e;$(window).load(e)})();(function(){function d(){return h}function f(a){h=a;b()}function b(){clearInterval(k);clearInterval(i);k=setInterval(function(){a()},6E4);i=setInterval(function(){var a=new Date;0>Date.parse(h)-Date.parse(a)&&(clearInterval(i),c())},6E4)}function a(){var a=new Date;18E4>Date.parse(h)-Date.parse(a)&&(clearInterval(k),$.CswDialog("ExpireDialog",{onYes:function(){Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/RenewSession",success:function(){}})}}))}function e(a){var c={status:"",success:function(){},failure:function(){},
usernodeid:"",usernodekey:"",passwordpropid:"",ForMobile:!1};a&&$.extend(c,a);var a="",b=!1;switch(c.status){case "Authenticated":c.success();break;case "Deauthenticated":c.success();break;case "Failed":a="Invalid login.";break;case "Locked":a="Your account is locked.  Please see your account administrator.";break;case "Deactivated":a="Your account is deactivated.  Please see your account administrator.";break;case "ModuleNotEnabled":a="This feature is not enabled.  Please see your account administrator.";
break;case "TooManyUsers":a="Too many users are currently connected.  Try again later.";break;case "NonExistentAccessId":a="Invalid login.";break;case "NonExistentSession":a="Your session has timed out.  Please login again.";break;case "Unknown":a="An Unknown Error Occurred";break;case "TimedOut":b=!0;a="Your session has timed out.  Please login again.";break;case "ExpiredPassword":b=!0;c.ForMobile||$.CswDialog("EditNodeDialog",{nodeids:[c.usernodeid],nodekeys:[c.usernodekey],filterToPropId:c.passwordpropid,
title:"Your password has expired.  Please change it now:",onEditNode:function(){c.success()}});break;case "ShowLicense":b=!0,c.ForMobile||$.CswDialog("ShowLicenseDialog",{onAccept:function(){c.success()},onDecline:function(){c.failure("You must accept the license agreement to use this application")}})}c.ForMobile&&"Authenticated"!==c.status&&b?c.success():!1===Csw.isNullOrEmpty(a)&&"Authenticated"!==c.status&&c.failure(a,c.status)}function c(a){var c={DeauthenticateUrl:"/NbtWebApp/wsNBT.asmx/deauthenticate",
onDeauthenticate:function(){}};a&&$.extend(c,a);Csw.ajax({url:c.DeauthenticateUrl,data:{},success:function(){g();c.onDeauthenticate()}})}function g(){var a=$.CswCookie("get",CswCookieName.LogoutPath);$.CswCookie("clearAll");window.location=!1===Csw.isNullOrEmpty(a)?a:Csw.getGlobalProp("homeUrl")}function j(a){var c={Yes:function(){},No:function(){}};a&&$.extend(c,a);Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/isAdministrator",success:function(a){"true"===a.Administrator?c.Yes():c.No()}})}var h="",k,i;Csw.register("getExpireTime",
d);Csw.getExpireTime=Csw.getExpireTime||d;Csw.register("setExpireTime",f);Csw.setExpireTime=Csw.setExpireTime||f;Csw.register("setExpireTimeInterval",b);Csw.setExpireTimeInterval=Csw.setExpireTimeInterval||b;Csw.register("setExpireTimeInterval",b);Csw.setExpireTimeInterval=Csw.setExpireTimeInterval||b;Csw.register("checkExpireTime",a);Csw.checkExpireTime=Csw.checkExpireTime||a;Csw.register("handleAuthenticationStatus",e);Csw.handleAuthenticationStatus=Csw.handleAuthenticationStatus||e;Csw.register("logout",
c,!0);Csw.logout=Csw.logout||c;Csw.register("finishLogout",g,!0);Csw.finishLogout=Csw.finishLogout||g;Csw.register("isAdministrator",j);Csw.isAdministrator=Csw.isAdministrator||j})();(function(){function d(){function d(){$.CswCookie("set",CswCookieName.LastViewId,$.CswCookie("get",CswCookieName.CurrentViewId));$.CswCookie("set",CswCookieName.LastViewMode,$.CswCookie("get",CswCookieName.CurrentViewMode));$.CswCookie("set",CswCookieName.LastActionName,$.CswCookie("get",CswCookieName.CurrentActionName));$.CswCookie("set",CswCookieName.LastActionUrl,$.CswCookie("get",CswCookieName.CurrentActionUrl));$.CswCookie("set",CswCookieName.LastReportId,$.CswCookie("get",CswCookieName.CurrentReportId));
$.CswCookie("clear",CswCookieName.CurrentViewId);$.CswCookie("clear",CswCookieName.CurrentViewMode);$.CswCookie("clear",CswCookieName.CurrentActionName);$.CswCookie("clear",CswCookieName.CurrentActionUrl);$.CswCookie("clear",CswCookieName.CurrentReportId);return!0}return{clearCurrent:d,setCurrentView:function(b,a){d();!1===Csw.isNullOrEmpty(b)&&!1===Csw.isNullOrEmpty(a)&&($.CswCookie("set",CswCookieName.CurrentViewId,b),$.CswCookie("set",CswCookieName.CurrentViewMode,a));return!0},setCurrentAction:function(b,
a){d();$.CswCookie("set",CswCookieName.CurrentActionName,b);$.CswCookie("set",CswCookieName.CurrentActionUrl,a);return!0},setCurrentReport:function(b){d();$.CswCookie("set",CswCookieName.CurrentReportId,b);return!0},getCurrent:function(){return{viewid:$.CswCookie("get",CswCookieName.CurrentViewId),viewmode:$.CswCookie("get",CswCookieName.CurrentViewMode),actionname:$.CswCookie("get",CswCookieName.CurrentActionName),actionurl:$.CswCookie("get",CswCookieName.CurrentActionUrl),reportid:$.CswCookie("get",
CswCookieName.CurrentReportId)}},getLast:function(){return{viewid:$.CswCookie("get",CswCookieName.LastViewId),viewmode:$.CswCookie("get",CswCookieName.LastViewMode),actionname:$.CswCookie("get",CswCookieName.LastActionName),actionurl:$.CswCookie("get",CswCookieName.LastActionUrl),reportid:$.CswCookie("get",CswCookieName.LastReportId)}},setCurrent:function(b){d();$.CswCookie("set",CswCookieName.CurrentViewId,b.viewid);$.CswCookie("set",CswCookieName.CurrentViewMode,b.viewmode);$.CswCookie("set",CswCookieName.CurrentActionName,
b.actionname);$.CswCookie("set",CswCookieName.CurrentActionUrl,b.actionurl);$.CswCookie("set",CswCookieName.CurrentReportId,b.reportid);return!0}}}Csw.register("clientState",d);Csw.clientState=Csw.clientState||d;Csw.clientState.clearCurrent=Csw.clientState.clearCurrent||d.clearCurrent;Csw.clientState.setCurrentView=Csw.clientState.setCurrentView||d.setCurrentView;Csw.clientState.setCurrentAction=Csw.clientState.setCurrentAction||d.setCurrentAction;Csw.clientState.setCurrentReport=Csw.clientState.setCurrentReport||
d.setCurrentReport;Csw.clientState.getCurrent=Csw.clientState.getCurrent||d.getCurrent;Csw.clientState.getLast=Csw.clientState.getLast||d.getLast;Csw.clientState.setCurrent=Csw.clientState.setCurrent||d.setCurrent})();(function(){function d(a){a=a.replace(/M/g,"m");a=a.replace(/mmm/g,"M");return a=a.replace(/yyyy/g,"yy")}function f(a){return a}function b(a){return a instanceof Date}function a(a){var b=!0,e=/^(\d?\d):(\d\d)\s?([APap][Mm])?$/g.exec(a);if(null===e)b=!1;else if(a=parseInt(e[1]),e=parseInt(e[2]),0>a||24<=a||0>e||60<=e)b=!1;return b}var e=new Date("1/1/0001 12:00:00 AM");Csw.register("dateTimeMinValue",e);Csw.dateTimeMinValue=Csw.dateTimeMinValue||e;Csw.register("serverDateFormatToJQuery",d);Csw.serverDateFormatToJQuery=
Csw.serverDateFormatToJQuery||d;Csw.register("serverTimeFormatToJQuery",f);Csw.serverTimeFormatToJQuery=Csw.serverTimeFormatToJQuery||f;Csw.register("isDate",b);Csw.isDate=Csw.isDate||b;Csw.register("validateTime",a);Csw.validateTime=Csw.validateTime||a})();(function(){function d(a,c){var b="";Csw.bool(c)&&(b=console.trace());try{!1===Csw.isNullOrEmpty(b)?console.log(a,b):console.log(a)}catch(e){alert(a),!1===Csw.isNullOrEmpty(b)&&alert(b)}}function f(a){var c="",b;for(b in a)c=c+b+"="+a[b]+"<br><br>";a=window.open("","popup");null!==a?a.document.write(c):console.log("iterate() error: No popup!")}function b(a){var c=void 0;if(Csw.hasWebStorage())1===arguments.length&&(localStorage.doLogging=Csw.bool(a)),c=Csw.bool(localStorage.doLogging);return c}function a(a){var c=
void 0;if(Csw.hasWebStorage())1===arguments.length&&(localStorage.debugOn=Csw.bool(a)),c=Csw.bool(localStorage.debugOn);return c}function e(){hasWebStorage()&&window.sessionStorage.clear()}Csw.register("log",d);Csw.log=Csw.log||d;Csw.register("iterate",f);Csw.iterate=Csw.iterate||f;Csw.register("doLogging",b);Csw.doLogging=Csw.doLogging||b;Csw.register("debugOn",a);Csw.debugOn=Csw.debugOn||a;var c=function(a){if(b()&&hasWebStorage()){void 0!==a.setEnded&&a.setEnded();var c=CswClientDb(),e=c.getItem("debuglog"),
e=e+a.toHtml();c.setItem("debuglog",e)}};Csw.register("cacheLogInfo",c);Csw.cacheLogInfo=Csw.cacheLogInfo||c;Csw.register("purgeLogInfo",e);Csw.purgeLogInfo=Csw.purgeLogInfo||e})();(function(){function d(a,b,c){return{type:Csw.string(a,Csw.enums.ErrorType.warning.name),message:Csw.string(b),detail:Csw.string(c)}}function f(a){var b={type:"",message:"",detail:"",display:!0};a&&$.extend(b,a);a=$("#DialogErrorDiv");0>=a.length&&(a=$("#ErrorDiv"));0<a.length&&Csw.bool(b.display)?a.CswErrorMessage({type:b.type,message:b.message,detail:b.detail}):log(b.message+"; "+b.detail);return!0}function b(a,b,c,g){Csw.hasWebStorage()&&c&&log(localStorage);g?$.CswDialog("ErrorDialog",a):log("Error: "+
a.message+" (Code "+a.code+")",b)}Csw.register("makeErrorObj",d);Csw.makeErrorObj=Csw.makeErrorObj||d;Csw.register("error",f);Csw.error=Csw.error||f;Csw.register("errorHandler",b);Csw.errorHandler=Csw.errorHandler||b})();(function(){function d(b,a){return function(){b(a)}}function f(b,a){return function(e){b(e,a)}}Csw.register("makeDelegate",d);Csw.makeDelegate=Csw.makeDelegate||d;Csw.register("makeEventDelegate",f);Csw.makeEventDelegate=Csw.makeEventDelegate||f})();(function(){function d(b){f(b)&&b.apply(this,Array.prototype.slice.call(arguments,1))}function f(b){return $.isFunction(b)}Csw.register("tryExec",d);Csw.tryExec=Csw.tryExec||d;Csw.register("isFunction",f);Csw.isFunction=Csw.isFunction||f})();(function(){function d(){clearCurrent();window.location=homeUrl}function f(b){var a={$ul:"",itemKey:"",itemJson:"",onLogout:null,onAlterNode:null,onSearch:{onViewSearch:null,onGenericSearch:null},onMultiEdit:null,onEditView:null,onSaveView:null,onQuotas:null,onSessions:null,Multi:!1,NodeCheckTreeId:""};b&&$.extend(a,b);var e,c=a.itemJson;e=Csw.string(c.href);var g=Csw.string(a.itemKey),f=Csw.string(c.popup),b=Csw.string(c.action);if(!1===Csw.isNullOrEmpty(e))e=$('<li><a href="'+e+'">'+g+"</a></li>").appendTo(a.$ul);
else if(!1==Csw.isNullOrEmpty(f))e=$('<li class="headermenu_dialog"><a href="'+f+'" target="_blank">'+g+"</a></li>").appendTo(a.$ul);else if(!1===Csw.isNullOrEmpty(b)){e=$('<li><a href="#">'+g+"</a></li>").appendTo(a.$ul);var f=e.children("a"),h=Csw.string(c.nodeid),k=Csw.string(c.nodename),i=Csw.string(c.viewid);switch(b){case "About":f.click(function(){$.CswDialog("AboutDialog");return!1});break;case "AddNode":f.click(function(){$.CswDialog("AddNodeDialog",{text:g,nodetypeid:Csw.string(c.nodetypeid),
relatednodeid:Csw.string(c.relatednodeid),relatednodetypeid:Csw.string(c.relatednodetypeid),onAddNode:a.onAlterNode});return!1});break;case "DeleteNode":f.click(function(){$.CswDialog("DeleteNodeDialog",{nodenames:[k],nodeids:[h],onDeleteNode:a.onAlterNode,NodeCheckTreeId:a.NodeCheckTreeId,Multi:a.Multi});return!1});break;case "editview":f.click(function(){a.onEditView(i);return!1});break;case "CopyNode":f.click(function(){$.CswDialog("CopyNodeDialog",{nodename:k,nodeid:h,onCopyNode:a.onAlterNode});
return!1});break;case "PrintView":f.click(a.onPrintView);break;case "PrintLabel":f.click(function(){$.CswDialog("PrintLabelDialog",{nodeid:h,propid:Csw.string(c.propid)});return!1});break;case "Logout":f.click(function(){a.onLogout();return!1});break;case "Home":f.click(function(){d();return!1});break;case "Profile":f.click(function(){$.CswDialog("EditNodeDialog",{nodeids:[c.userid],filterToPropId:"",title:"User Profile",onEditNode:null});return!1});break;case "ViewSearch":f.click(function(){Csw.tryExec(a.onSearch.onViewSearch)});
break;case "GenericSearch":f.click(function(){Csw.tryExec(a.onSearch.onGenericSearch)});break;case "multiedit":f.click(a.onMultiEdit);break;case "SaveViewAs":f.click(function(){$.CswDialog("AddViewDialog",{viewid:i,viewmode:Csw.string(c.viewmode),onAddView:a.onSaveView});return!1});break;case "Quotas":f.click(a.onQuotas);break;case "Sessions":f.click(a.onSessions)}}else e=$("<li>"+g+"</li>").appendTo(a.$ul);return e}Csw.register("goHome",d);Csw.goHome=Csw.goHome||d;Csw.register("handleMenuItem",f);
Csw.handleMenuItem=Csw.handleMenuItem||f})();(function(){function d(c,b){var e=0,d=+c;!1===isNaN(d)&&d!==a?e=d:(d=+b,!1===isNaN(d)&&d!==a&&(e=d));return e}function f(a){return"number"===typeof a}function b(a,b){var e,d="";if(0<a&&a<=(b||6)){d+=".";for(e=0;e<a;e+=1)d+="9"}return d}var a=-2147483648;Csw.register("int32MinVal",a);Csw.int32MinVal=Csw.int32MinVal||a;Csw.register("number",d);Csw.number=Csw.number||d;Csw.register("isNumber",f);Csw.isNumber=Csw.isNumber||f;var e=function(a){var b=!1;f(a)&&!1===Csw.isNullOrEmpty(a)&&!1===isNaN(+a)&&
(b=!0);return b};Csw.register("isNumeric",e);Csw.isNumeric=Csw.isNumeric||e;Csw.register("getMaxValueForPrecision",b);Csw.getMaxValueForPrecision=Csw.getMaxValueForPrecision||b})();(function(){function d(a){return $.isPlainObject(a)}function f(a){return a instanceof jQuery}function b(a){return Csw.isArray(a)||f(a)}function a(a){return!1===Csw.isFunction(a)&&!1===b(a)&&!1===d(a)}function e(c,e){var d=g(c);!1===d&&a(c)?d=""===Csw.trim(c)||Csw.isDate(c)&&c===Csw.dateTimeMinValue||Csw.isNumber(c)&&c===Csw.int32MinVal:e&&b(c)&&(d=0===c.length);return d}function c(a,c){return Csw.contains(a,c)&&Csw.bool(c[a])}function g(a){var c=!1;!1===Csw.isFunction(a)&&(c=null===a||void 0===a||
$.isPlainObject(a)&&$.isEmptyObject(a));return c}function j(a,c,b){var e="";!1===Csw.isNullOrEmpty(b)&&(e=b);Csw.contains(a,c)&&!1===Csw.isNullOrUndefined(a[c])&&(e=a[c]);return e}function h(a,c){var b=!1;!1===Csw.isNullOrUndefined(a)&&(Csw.isArray(a)&&(b=-1!==a.indexOf(c)),!1===b&&a.hasOwnProperty(c)&&(b=!0));return b}function k(a,c,b){!1===Csw.isNullOrUndefined(a)&&h(a,c)&&(a[b]=a[c],delete a[c]);return a}function i(a,c,b){var e=!1;!1===Csw.isNullOrEmpty(a)&&h(a,c)&&a[c]===b&&(e=!0);return e}function n(a){var c=
null,b=a;this.find=function(e,d){var g=!1;i(a,e,d)&&(b=c=g=a);!1===g&&l(a,function(a,f,h){f=!1;i(a,e,d)&&(c=g=a,b=h,f=!0);return f},!0);return g};this.remove=function(b,e){return l(a,function(a,d,g){var f=!1;i(a,b,e)&&(f=!0,delete g[d],c=null);return f},!0)};this.obj=a;this.parentObj=b;this.currentKey=this.currentObj=c}function m(a,c){var b=!1;if(Csw.isFunction(c))if(Csw.isArray(a)||Csw.isPlainObject(a)&&!1===h(a,"length"))$.each(a,function(e,d){b=c(a[e],e,a,d);return!b});else if(Csw.isPlainObject(a))for(var e in a)if(h(a,
e)&&(b=c(a[e],e,a)))break;return b}function l(a,c,b){var e=!1;return e=m(a,function(a,d,g,f){!1===e&&(e=Csw.bool(c(a,d,g,f)));!1===e&&b&&(e=Csw.bool(l(a,c,b)));return e})}Csw.register("isPlainObject",d);Csw.isPlainObject=Csw.isPlainObject||d;Csw.register("isJQuery",f);Csw.isJQuery=Csw.isJQuery||f;Csw.register("hasLength",b);Csw.hasLength=Csw.hasLength||b;Csw.register("isGeneric",a);Csw.isGeneric=Csw.isGeneric||a;Csw.register("isNullOrEmpty",e);Csw.isNullOrEmpty=Csw.isNullOrEmpty||e;Csw.register("isInstanceOf",
c);Csw.isInstanceOf=Csw.isInstanceOf||c;Csw.register("isNullOrUndefined",g);Csw.isNullOrUndefined=Csw.isNullOrUndefined||g;Csw.register("tryParseObjByIdx",j);Csw.tryParseObjByIdx=Csw.tryParseObjByIdx||j;Csw.register("contains",h);Csw.contains=Csw.contains||h;Csw.register("renameProperty",k);Csw.renameProperty=Csw.renameProperty||k;Csw.register("foundMatch",i);Csw.foundMatch=Csw.foundMatch||i;Csw.register("ObjectHelper",n);Csw.ObjectHelper=Csw.ObjectHelper||n;Csw.register("each",m);Csw.each=Csw.each||
m;Csw.register("crawlObject",l);Csw.crawlObject=Csw.crawlObject||l})();(function(){function d(a,b){var e=function(){var e="";!1===Csw.isPlainObject(a)&&!1===Csw.isFunction(a)&&!1===Csw.isNullOrEmpty(a)?e=a.toString():!1===Csw.isPlainObject(b)&&!1===Csw.isFunction(b)&&!1===Csw.isNullOrEmpty(b)&&(e=b.toString());return e}();e.val=function(){return a};e.trim=function(){return a=$.trim(a)};e.contains=function(b){return-1!==a.indexOf(b)};return e}function f(a){return"string"===typeof a||Csw.isInstanceOf("string",a)}function b(a){return $.trim(a)}function a(a,b){return a.substr(0,
b.length)===b}function e(a,b){var e=!1;!1===Csw.isNullOrEmpty(b)&&"H:mm:ss"===b&&(e=!0);var d=a.getHours(),f=a.getMinutes(),i=a.getSeconds();10>f&&(f="0"+f);10>i&&(i="0"+i);e?e=d+":"+f+":"+i:(e=d%12+":"+f+":"+i+" ",e=11<d?e+"PM":e+"AM");return e}String.prototype.trim=String.prototype.trim||function(){return this.replace(/^\s+|\s+$/g,"")};String.prototype.toUpperCaseFirstChar=String.prototype.toUpperCaseFirstChar||function(){return this.substr(0,1).toUpperCase()+this.substr(1)};String.prototype.toLowerCaseFirstChar=
String.prototype.toLowerCaseFirstChar||function(){return this.substr(0,1).toLowerCase()+this.substr(1)};String.prototype.toUpperCaseEachWord=String.prototype.toUpperCaseEachWord||function(a){a=a||" ";return this.split(a).map(function(a){return a.toUpperCaseFirstChar()}).join(a)};String.prototype.toLowerCaseEachWord=String.prototype.toLowerCaseEachWord||function(a){a=a||" ";return this.split(a).map(function(a){return a.toLowerCaseFirstChar()}).join(a)};Csw.register("string",d);Csw.string=Csw.string||
d;Csw.register("isString",f);Csw.isString=Csw.isString||f;Csw.register("trim",b);Csw.trim=window.Csw.trim||b;Csw.register("startsWith",a);Csw.startsWith=Csw.startsWith||a;Csw.register("getTimeString",e);Csw.getTimeString=Csw.getTimeString||e})();(function(){function d(a){var b=a.CswAttrDom("id"),a=a.jstree("get_selected");return{iconurl:a.children("a").children("ins").css("background-image"),id:Csw.string(a.CswAttrDom("id")).substring(b.length),text:Csw.string(a.children("a").first().text()).trim(),$item:a}}function f(a,b,c){var d,f,h;Csw.isAdministrator({Yes:function(){a.CswTable("cell",b,1).append(c);var k=a.CswTable("cell",b,2),i=a.CswAttrDom("id");d=$('<select id="'+i+'_vissel" />').appendTo(k);d.append('<option value="User">User:</option>');
d.append('<option value="Role">Role:</option>');d.append('<option value="Global">Global</option>');f=k.CswNodeSelect("init",{ID:i+"_visrolesel",objectclass:"RoleClass"}).hide();h=k.CswNodeSelect("init",{ID:i+"_visusersel",objectclass:"UserClass"});d.change(function(){var a=d.val();"Role"===a?(f.show(),h.hide()):"User"===a?(f.hide(),h.show()):(f.hide(),h.hide())})}});return{getvisibilityselect:function(){return d},getvisroleselect:function(){return f},getvisuserselect:function(){return h}}}function b(a,
b,c){a=window.open(a,null,"height="+b+", width="+c+", status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes");a.focus();return a}Csw.register("jsTreeGetSelected",d);Csw.jsTreeGetSelected=Csw.jsTreeGetSelected||d;Csw.register("makeViewVisibilitySelect",f);Csw.makeViewVisibilitySelect=Csw.makeViewVisibilitySelect||f;Csw.register("openPopup",b);Csw.openPopup=Csw.openPopup||b})();(function(){function d(a,b,d,f){b={ID:"",prefix:Csw.string(b),suffix:Csw.string(d),Delimiter:Csw.string(f,"_")};Csw.isPlainObject(a)?$.extend(b,a):b.ID=Csw.string(a);a=b.ID;!1===Csw.isNullOrEmpty(b.prefix)&&!1===Csw.isNullOrEmpty(a)&&(a=b.prefix+b.Delimiter+a);!1===Csw.isNullOrEmpty(b.suffix)&&!1===Csw.isNullOrEmpty(a)&&(a+=b.Delimiter+b.suffix);return a}function f(a,b,d,f){b={ID:"",prefix:Csw.string(b),suffix:Csw.string(d),Delimiter:Csw.string(f,"_")};Csw.isPlainObject(a)?$.extend(b,a):b.ID=Csw.string(a);
a=b.ID;d=[/'/gi,/ /gi,/\//g];!1===Csw.isNullOrEmpty(b.prefix)&&!1===Csw.isNullOrEmpty(a)&&(a=b.prefix+b.Delimiter+a);!1===Csw.isNullOrEmpty(b.suffix)&&!1===Csw.isNullOrEmpty(a)&&(a+=b.Delimiter+b.suffix);for(b=0;b<d.length;b++)Csw.contains(d,b)&&!1===Csw.isNullOrEmpty(a)&&(a=a.replace(d[b],""));return a}function b(){return window.Modernizr.localstorage||window.Modernizr.sessionstorage}function a(a,b){var d=$(""),f=Csw.getGlobalProp("document");!1===Csw.isNullOrEmpty(a)&&(d=2===arguments.length&&!1===
Csw.isNullOrEmpty(b)?$("#"+a,b):$("#"+a),0===d.length&&(d=$(f.getElementById(a))),0===d.length&&(d=$(f.getElementsByName(a))));return d}Csw.register("makeId",d);Csw.makeId=Csw.makeId||d;Csw.register("makeSafeId",f);Csw.makeSafeId=Csw.makeSafeId||f;Csw.register("hasWebStorage",b);Csw.hasWebStorage=Csw.hasWebStorage||b;Csw.register("tryParseElement",a);Csw.tryParseElement=Csw.tryParseElement||a})();(function(){var d=function(){var d={},b=[],a=0;return{getItem:function(a){var b=null;a&&d.hasOwnProperty(a)&&(b=d[a]);return b},key:function(a){var c=null;b.hasOwnProperty(a)&&(c=b[a]);return c},setItem:function(e,c){e&&(!1===d.hasOwnProperty(e)&&(b.push(e),a+=1),d[e]=c);return null},length:a,removeItem:function(e){var c=!1;e&&d.hasOwnProperty(e)&&(b.splice(e,1),a-=1,delete d[e],c=!0);return c},clear:function(){d={};b=[];a=0;return!0},hasOwnProperty:function(a){return d.hasOwnProperty(a)}}}();if(!1===
window.Modernizr.localstorage)window.localStorage=d;if(!1===window.Modernizr.sessionstorage)window.sessionStorage=d})();(function(){var d={unknownEnum:"unknown"};Csw.register("constants",d);Csw.constants=Csw.constants||d;d={tryParse:function(d,b,a){var e=ChemSW.constants.unknownEnum;contains(d,b)?e=d[b]:!1===a&&each(d,function(a){contains(d,a)&&Csw.string(a).toLowerCase()===Csw.string(b).toLowerCase()&&(e=a)});return e},EditMode:{Edit:"Edit",AddInPopup:"AddInPopup",EditInPopup:"EditInPopup",Demo:"Demo",PrintReport:"PrintReport",DefaultValue:"DefaultValue",AuditHistoryInPopup:"AuditHistoryInPopup",Preview:"Preview"},
ErrorType:{warning:{name:"warning",cssclass:"CswErrorMessage_Warning"},error:{name:"error",cssclass:"CswErrorMessage_Error"}},Events:{CswNodeDelete:"CswNodeDelete"},CswInspectionDesign_WizardSteps:{step1:{step:1,description:"Select an Inspection Target"},step2:{step:2,description:"Select an Inspection Design"},step3:{step:3,description:"Upload Template"},step4:{step:4,description:"Review Inspection Design"},step5:{step:5,description:"Finish"},stepcount:5},CswScheduledRulesGrid_WizardSteps:{step1:{step:1,
description:"Select a Customer ID"},step2:{step:2,description:"Review the Scheduled Rules"},stepcount:2},CswDialogButtons:{1:"ok",2:"ok/cancel",3:"yes/no"},CswOnObjectClassClick:{reauthenticate:"reauthenticate",home:"home",refresh:"refresh",url:"url"}};Csw.register("enums",d);Csw.enums=Csw.enums||d})();
var EditMode={Edit:{name:"Edit"},AddInPopup:{name:"AddInPopup"},EditInPopup:{name:"EditInPopup"},Demo:{name:"Demo"},PrintReport:{name:"PrintReport"},DefaultValue:{name:"DefaultValue"},AuditHistoryInPopup:{name:"AuditHistoryInPopup"},Preview:{name:"Preview"}},CswInput_Types={button:{id:0,name:"button",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""},checkbox:{id:1,name:"checkbox",placeholder:!1,autocomplete:!1,value:{required:!0,allowed:!0},defaultwidth:""},color:{id:2,
name:"color",placeholder:!1,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:""},date:{id:3,name:"date",placeholder:!1,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:"200px"},datetime:{id:4,name:"datetime",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:"200px"},"datetime-local":{id:5,name:"datetime-local",placeholder:!1,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:"200px"},email:{id:6,name:"email",placeholder:!0,autocomplete:!0,value:{required:!1,
allowed:!0},defaultwidth:"200px"},file:{id:7,name:"file",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!1},defaultwidth:""},hidden:{id:8,name:"hidden",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""},image:{id:9,name:"image",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""},month:{id:10,name:"month",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""},number:{id:11,name:"number",placeholder:!1,autocomplete:!1,
value:{required:!1,allowed:!0},defaultwidth:"200px"},password:{id:12,name:"password",placeholder:!0,value:{required:!1,allowed:!0},defaultwidth:"200px"},radio:{id:13,name:"radio",placeholder:!1,autocomplete:!1,value:{required:!0,allowed:!0},defaultwidth:""},range:{id:14,name:"range",placeholder:!1,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:""},reset:{id:15,name:"reset",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""},search:{id:16,name:"search",placeholder:!0,
autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:""},submit:{id:17,name:"submit",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""},tel:{id:18,name:"button",placeholder:!0,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:""},text:{id:19,name:"text",placeholder:!0,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:"200px"},time:{id:20,name:"time",placeholder:!1,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:"200px"},url:{id:21,
name:"url",placeholder:!0,autocomplete:!0,value:{required:!1,allowed:!0},defaultwidth:"200px"},week:{id:22,name:"week",placeholder:!1,autocomplete:!1,value:{required:!1,allowed:!0},defaultwidth:""}},CswViewMode={grid:{name:"Grid"},tree:{name:"Tree"},list:{name:"List"},table:{name:"Table"}},CswRateIntervalTypes={WeeklyByDay:"WeeklyByDay",MonthlyByDate:"MonthlyByDate",MonthlyByWeekAndDay:"MonthlyByWeekAndDay",YearlyByDate:"YearlyByDate"},CswMultiEditDefaultValue="[Unchanged]",CswCookieName={SessionId:"CswSessionId",
Username:"csw_username",LogoutPath:"csw_logoutpath",CurrentNodeId:"csw_currentnodeid",CurrentNodeKey:"csw_currentnodekey",CurrentTabId:"csw_currenttabid",CurrentActionName:"csw_currentactionname",CurrentActionUrl:"csw_currentactionurl",CurrentViewId:"csw_currentviewid",CurrentViewMode:"csw_currentviewmode",CurrentReportId:"csw_currentreportid",LastActionName:"csw_lastactionname",LastActionUrl:"csw_lastactionurl",LastViewId:"csw_lastviewid",LastViewMode:"csw_lastviewmode",LastReportId:"csw_lastreportid"},
CswAppMode={mode:"full"},CswImageButton_ButtonType={None:-1,Add:27,ArrowNorth:28,ArrowEast:29,ArrowSouth:30,ArrowWest:31,Calendar:6,CheckboxFalse:18,CheckboxNull:19,CheckboxTrue:20,Clear:4,Clock:10,ClockGrey:11,Configure:26,Delete:4,Edit:3,Fire:5,PageFirst:23,PagePrevious:24,PageNext:25,PageLast:22,PinActive:17,PinInactive:15,Print:2,Refresh:9,SaveStatus:13,Select:32,ToggleActive:1,ToggleInactive:0,View:8},CswSearch_CssClasses={nodetype_select:{name:"csw_search_nodetype_select"},property_select:{name:"csw_search_property_select"}},
CswViewEditor_WizardSteps={viewselect:{step:1,description:"Choose a View",divId:"step1_viewselect"},attributes:{step:2,description:"Edit View Attributes",divId:"step2_attributes"},relationships:{step:3,description:"Add Relationships",divId:"step3_relationships"},properties:{step:4,description:"Select Properties",divId:"step4_properties"},filters:{step:5,description:"Set Filters",divId:"step5_filters"},tuning:{step:6,description:"Fine Tuning",divId:"step6_tuning"}},ViewBuilder_CssClasses={subfield_select:{name:"csw_viewbuilder_subfield_select"},
filter_select:{name:"csw_viewbuilder_filter_select"},default_filter:{name:"csw_viewbuilder_default_filter"},filter_value:{name:"csw_viewbuilder_filter_value"},metadatatype_static:{name:"csw_viewbuilder_metadatatype_static"}},CswDomElementEvent={click:{name:"click"},change:{name:"change"},vclick:{name:"vclick"},tap:{name:"tap"}},CswObjectClasses={GenericClass:{name:"GenericClass"},InspectionDesignClass:{name:"InspectionDesignClass"}},CswNodeSpecies={Plain:{name:"Plain"},More:{name:"More"}},CswSubField_Names=
{Unknown:{name:"unknown"},AllowedAnswers:{name:"allowedanswers"},Answer:{name:"answer"},Barcode:{name:"barcode"},Blob:{name:"blob"},Checked:{name:"checked"},Column:{name:"column"},Comments:{name:"comments"},CompliantAnswers:{name:"compliantanswers"},ContentType:{name:"contenttype"},CorrectiveAction:{name:"correctiveaction"},DateAnswered:{name:"dateanswered"},DateCorrected:{name:"datecorrected"},Href:{name:"href"},Image:{name:"image"},Interval:{name:"interval"},IsCompliant:{name:"iscompliant"},Mol:{name:"mol"},
Name:{name:"name"},NodeID:{name:"nodeid"},NodeType:{name:"nodetype"},Number:{name:"number"},Password:{name:"password"},Path:{name:"path"},Required:{name:"required"},Row:{name:"row"},Sequence:{name:"sequence"},StartDateTime:{name:"startdatetime"},Text:{name:"text"},Units:{name:"units"},Value:{name:"value"},ViewID:{name:"viewid"},ChangedDate:{name:"changeddate"},Base:{name:"base"},Exponent:{name:"exponent"}},CswSubFields_Map={AuditHistoryGrid:{name:"AuditHistoryGrid",subfields:{}},Barcode:{name:"Barcode",
subfields:{Barcode:CswSubField_Names.Barcode,Sequence:CswSubField_Names.Number}},Button:{name:"Button",subfields:{Text:CswSubField_Names.Text}},Composite:{name:"Composite",subfields:{}},DateTime:{name:"DateTime",subfields:{Value:{Date:{name:"date"},Time:{name:"time"},DateFormat:{name:"dateformat"},TimeFormat:{name:"timeformat"}},DisplayMode:{Date:{name:"Date"},Time:{name:"Time"},DateTime:{name:"DateTime"}}}},File:{name:"File",subfields:{}},Grid:{name:"Grid",subfields:{}},Image:{name:"Image",subfields:{}},
Link:{name:"Link",subfields:{Text:CswSubField_Names.Text,Href:CswSubField_Names.Href}},List:{name:"List",subfields:{Value:CswSubField_Names.Value}},Location:{name:"Location",subfields:{}},LocationContents:{name:"LocationContents",subfields:{}},Logical:{name:"Logical",subfields:{Checked:CswSubField_Names.Checked}},LogicalSet:{name:"LogicalSet",subfields:{}},Memo:{name:"Memo",subfields:{Text:CswSubField_Names.Text}},MTBF:{name:"MTBF",subfields:{}},MultiList:{name:"MultiList",subfields:{}},NodeTypeSelect:{name:"NodeTypeSelect",
subfields:{}},Number:{name:"Number",subfields:{Value:CswSubField_Names.Value}},Password:{name:"Password",subfields:{Password:CswSubField_Names.Password,ChangedDate:CswSubField_Names.ChangedDate}},PropertyReference:{name:"PropertyReference",subfields:{}},Quantity:{name:"Quantity",subfields:{Value:CswSubField_Names.Value,Units:CswSubField_Names.Number}},Question:{name:"Question",subfields:{Answer:CswSubField_Names.Answer,CorrectiveAction:CswSubField_Names.CorrectiveAction,IsCompliant:CswSubField_Names.IsCompliant,
Comments:CswSubField_Names.Comments,DateAnswered:CswSubField_Names.DateAnswered,DateCorrected:CswSubField_Names.DateCorrected}},Relationship:{name:"Relationship",subfields:{}},Scientific:{name:"Scientific",subfields:{}},Sequence:{name:"Sequence",subfields:{}},Static:{name:"Static",subfields:{Text:CswSubField_Names.Text}},Text:{name:"Text",subfields:{Text:CswSubField_Names.Text}},TimeInterval:{name:"TimeInterval",subfields:{}},UserSelect:{name:"UserSelect",subfields:{}},ViewPickList:{name:"ViewPickList",
subfields:{}},ViewReference:{name:"ViewReference",subfields:{}}},viewEditClasses={vieweditor_viewrootlink:{name:"vieweditor_viewrootlink"},vieweditor_viewrellink:{name:"vieweditor_viewrellink"},vieweditor_viewproplink:{name:"vieweditor_viewproplink"},vieweditor_viewfilterlink:{name:"vieweditor_viewfilterlink"},vieweditor_addfilter:{name:"vieweditor_addfilter"},vieweditor_deletespan:{name:"vieweditor_deletespan"},vieweditor_childselect:{name:"vieweditor_childselect"}},childPropNames={root:{name:"root"},
childrelationships:{name:"childrelationships"},properties:{name:"properties"},filters:{name:"filters"},propfilters:{name:"filters"},filtermodes:{name:"filtermodes"}},CswNodeTree_DefaultSelect={root:{name:"root"},firstchild:{name:"firstchild"},none:{name:"none"}};(function(){var d=function(d){var b={nodeid:"",nodekey:"",onSuccess:function(){},onError:function(){}};d&&$.extend(b,d);Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/CopyNode",data:{NodePk:b.nodeid},success:function(a){b.onSuccess(a.NewNodeId,"")},error:b.onError})};Csw.register("copyNode",d);Csw.copyNode=Csw.copyNode||d;d=function(d){var b={nodeids:[],nodekeys:[],onSuccess:function(){},onError:function(){}};d&&$.extend(b,d);if(!isArray(b.nodeids))b.nodeids=[b.nodeids],b.nodekeys=[b.nodekeys];Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/DeleteNodes",
data:{NodePks:b.nodeids,NodeKeys:b.nodekeys},success:function(){b.nodeid=$.CswCookie("clear",CswCookieName.CurrentNodeId);b.cswnbtnodekey=$.CswCookie("clear",CswCookieName.CurrentNodeKey);b.onSuccess("","")},error:b.onError})};Csw.register("deleteNodes",d);Csw.deleteNodes=Csw.deleteNodes||d})();(function(){var d=void 0,f=function(b,a,e){d=$.CswNodePreview("open",{ID:a+"_preview",nodeid:a,cswnbtnodekey:e,eventArg:b})};Csw.register("nodeHoverIn",f);Csw.nodeHoverIn=Csw.nodeHoverIn||f;f=function(){void 0!==d&&(d.CswNodePreview("close"),d=void 0)};Csw.register("nodeHoverOut",f);Csw.nodeHoverOut=Csw.nodeHoverOut||f})();(function(){var d=function(b,a,d){var c=!1;if(!1===Csw.isNullOrEmpty(a))contains(a,"values")&&(c=f(b,a.values,d)),a.wasmodified=a.wasmodified||c};Csw.register("preparePropJsonForSave",d);Csw.preparePropJsonForSave=Csw.preparePropJsonForSave||d;var f=function(b,a,d){if(!1===Csw.isNullOrEmpty(a)){var c=!1;crawlObject(a,function(g,j){if(contains(d,j)){var h=d[j];if(isPlainObject(h))c=f(b,a[j],h)||c;else if(!1===b&&a[j]!==h||b&&!1===isNullOrUndefined(h)&&h!==CswMultiEditDefaultValue)c=!0,a[j]=h}},!1)}return c}})();(function(d){d.CswCookie=function(f){var b={get:function(a){a=d.cookie(a);void 0==a&&(a="");return a},set:function(a,b){d.cookie(a,b)},clear:function(a){d.cookie(a,"")},clearAll:function(){for(var a in CswCookieName)CswCookieName.hasOwnProperty(a)&&d.cookie(CswCookieName[a],null)}};if(b[f])return b[f].apply(this,Array.prototype.slice.call(arguments,1));if("object"===typeof f||!f)return b.init.apply(this,arguments);d.error("Method "+f+" does not exist on CswCookie");return!1}})(jQuery);
﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswAjax() {
    'use strict';
    
    var _ajaxCount = 0;
    function ajaxInProgress() {
        return (_ajaxCount > 0);
    }
    Csw.register('ajaxInProgress', ajaxInProgress);
    Csw.ajaxInProgress = Csw.ajaxInProgress || ajaxInProgress;

    // Events for all Ajax requests
    var onBeforeAjax = null;  // function (watchGlobal) {}
    Csw.register('onBeforeAjax', onBeforeAjax);
    Csw.onBeforeAjax = Csw.onBeforeAjax || onBeforeAjax;
    
    var onAfterAjax = null;   // function (succeeded) {}
    Csw.register('onAfterAjax', onAfterAjax);
    Csw.onAfterAjax = Csw.onAfterAjax || onAfterAjax;
    
    var type = {
        POST: 'POST',
        GET: 'GET'
    };

    var dataType = {
        json: 'json',
        xml: 'xml'
    };

    function jsonPost(options) {
        /// <param name="$" type="jQuery" />
        /// <summary>
        ///   Executes Async webservice request for JSON
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        /// </param>
        var o = {
            url: '',
            data: {},
            onloginfail: function () {
                Csw.finishLogout ();
            },
            success: null, //function () { },
            error: null, //function () { },
            overrideError: false,
            formobile: false,
            async: true,
            watchGlobal: true
        };
        if (options) {
            $.extend (o, options);
        }

        //var starttime = new Date();
        if (o.watchGlobal) {
            _ajaxCount += 1;
        }
        Csw.tryExec(onBeforeAjax, o.watchGlobal);

        $.ajax ({
            type: 'POST',
            async: o.async,
            url: o.url,
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify (o.data),
            success: function (data, textStatus, xmlHttpRequest) {
                if (o.watchGlobal) {
                    _ajaxCount -= 1;
                }
                //var endtime = new Date();
                //$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");
                var result = $.parseJSON (data.d);

                if (result.error !== undefined) {
                    if (false === o.overrideError) {
                        handleAjaxError (xmlHttpRequest, {
                            'display': result.error.display,
                            'type': result.error.type,
                            'message': result.error.message,
                            'detail': result.error.detail
                        }, '');
                    }
                    Csw.tryExec(o.error, result.error);
                } else {

                    var auth = Csw.string (result['AuthenticationStatus'], 'Unknown');
                    if (false === o.formobile) {
                        Csw.setExpireTime(Csw.string (result.timeout, ''));
                    }

                    delete result['AuthenticationStatus'];
                    delete result['timeout'];

                    Csw.handleAuthenticationStatus ({
                        status: auth,
                        success: function () {
                            Csw.tryExec(o.success, result);
                        },
                        failure: o.onloginfail,
                        usernodeid: result.nodeid,
                        usernodekey: result.cswnbtnodekey,
                        passwordpropid: result.passwordpropid,
                        ForMobile: o.formobile
                    });
                }
               Csw.tryExec(onAfterAjax, true);
            }, // success{}
            error: function (xmlHttpRequest, textStatus) {
                if (o.watchGlobal) {
                    _ajaxCount -= 1;
                }
                Csw.log ("Webservice Request (" + o.url + ") Failed: " + textStatus);
                Csw.tryExec(o.error);
                Csw.tryExec(onAfterAjax, false);
            }
        }); // $.ajax({
    } // Csw.ajax()

    function jsonGet(options) {
        /// <param name="$" type="jQuery" />
        /// <summary>
        ///   Executes Async webservice request for JSON
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        /// </param>
        var o = {
            url: '',
            data: {},
            onloginfail: function () {
                Csw.finishLogout ();
            },
            success: null, //function () { },
            error: null, //function () { },
            overrideError: false,
            formobile: false,
            async: true,
            watchGlobal: true
        };
        if (options) {
            $.extend (o, options);
        }

        if (o.watchGlobal) {
            _ajaxCount += 1;
        }
        if (isFunction (onBeforeAjax)) {
            onBeforeAjax (o.watchGlobal);
        }

        $.ajax ({
            type: 'GET',
            async: o.async,
            url: o.url,
            dataType: 'json',
            data: JSON.stringify (o.data),
            success: function (result, textStatus, xmlHttpRequest) {
                if (o.watchGlobal) {
                    _ajaxCount -= 1;
                }

                if (false === Csw.isNullOrEmpty (result.error)) {
                    if (false === o.overrideError) {
                        handleAjaxError (xmlHttpRequest, {
                            'display': result.error.display,
                            'type': result.error.type,
                            'message': result.error.message,
                            'detail': result.error.detail
                        }, '');
                    }
                    if (Csw.isFunction (o.error)) {
                        o.error (result.error);
                    }
                } else {
                    if (Csw.isFunction (o.success)) {
                        o.success (result);
                    }
                }
                if (isFunction (onAfterAjax)) {
                    onAfterAjax (true);
                }
            }, // success{}
            error: function (xmlHttpRequest, textStatus) {
                if (o.watchGlobal) {
                    _ajaxCount -= 1;
                }
                Csw.log ("Webservice Request (" + o.url + ") Failed: " + textStatus);
                if (isFunction (o.error)) {
                    o.error ();
                }
                if (Csw.isFunction (onAfterAjax)) {
                    onAfterAjax (false);
                }
            }
        }); // $.ajax({
    } // Csw.ajaxGet()

    function xmlPost(options) {
        /// <summary>
        ///   Executes Async webservice request for XML
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        ///     &#10;5 - options.formobile: false
        /// </param>

        var o = {
            url: '',
            data: {},
            stringify: false, //in case we need to conditionally apply $.param() instead of JSON.stringify() (or both)
            onloginfail: function () {
                Csw.finishLogout ();
            },
            success: function () {
            },
            error: function () {
            },
            formobile: false,
            async: true,
            watchGlobal: true
        };

        if (options) {
            $.extend (o, options);
        }

        if (false === Csw.isNullOrEmpty (o.url)) {
            if (o.watchGlobal) {
                _ajaxCount += 1;
            }
            $.ajax ({
                type: 'POST',
                async: o.async,
                url: o.url,
                dataType: "text",
                //contentType: 'application/json; charset=utf-8',
                data: $.param (o.data),     // should be 'field1=value&field2=value'
                success: function (data, textStatus, xmlHttpRequest) {
                    if (o.watchGlobal) {
                        _ajaxCount -= 1;
                    }
                    //var endtime = new Date();
                    //$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

                    var $realxml;
                    if ($.browser.msie) {
                        // We have to use third-party jquery.xml.js for Internet Explorer to handle non-DOM XML content
                        $realxml = $.xml (data);
                    } else {
                        $realxml = $ (xmlHttpRequest.responseXML).children ().first ();
                    }

                    if ($realxml.first ().get (0).nodeName === "error") {
                        handleAjaxError (xmlHttpRequest, {
                            'display': $realxml.CswAttrNonDom ('display'),
                            'type': $realxml.CswAttrNonDom ('type'),
                            'message': $realxml.CswAttrNonDom ('message'),
                            'detail': $realxml.CswAttrNonDom ('detail')
                        }, '');
                        o.error ();
                    } else {
                        var auth = Csw.string($realxml.CswAttrNonDom ('authenticationstatus'), 'Unknown');
                        if (!o.formobile) {
                            Csw.setExpireTime ($realxml.CswAttrNonDom ('timeout'));
                        }

                        Csw.handleAuthenticationStatus ({
                            status: auth,
                            success: function () {
                                o.success ($realxml);
                            },
                            failure: o.onloginfail,
                            usernodeid: Csw.string ($realxml.CswAttrNonDom ('nodeid'), ''),
                            usernodekey: Csw.string ($realxml.CswAttrNonDom ('cswnbtnodekey'), ''),
                            passwordpropid: Csw.string ($realxml.CswAttrNonDom ('passwordpropid'), ''),
                            ForMobile: o.formobile
                        });
                    }

                }, // success{}
                error: function (xmlHttpRequest, textStatus) {
                    if (o.watchGlobal) {
                        _ajaxCount -= 1;
                    }
                    Csw.log ("Webservice Request (" + o.url + ") Failed: " + textStatus);
                    o.error ();
                }
            }); // $.ajax({
        } // if(o.url != '')
    } // CswAjaxXml()

    function ajax(options, requestDataType, requestType) {
        /// <summary>
        ///   Executes Async webservice request
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        /// </param>
        /// <param name="requestDataType" type="String">(Optional) Data type of request: json (default) or xml. </param>
        /// <param name="requestType" type="String">(Optional) type of request: POST (default) or GET. </param>
        /// <returns>Returns the result of of $.ajax()</returns>
        var ret,
            rType = Csw.string(requestType),
            rdType = Csw.string(requestDataType);
        
        switch(rType) {
            case dataType.xml:
                ret = xmlPost(options);
                break;
            default:
                if(rdType.toUpperCase() === type.GET) {
                    ret = jsonGet(options);
                } else {
                    ret = jsonPost(options);                    
                }
                break;
        }
        return {
            ajax: ret
            
        };
    }
    Csw.register('ajax', ajax);
    Csw.ajax = Csw.ajax || ajax;

    function handleAjaxError(xmlHttpRequest, errorJson) {
        Csw.error(errorJson);
    } // _handleAjaxError()

    
}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    //#region Prototype Extension

        // Production steps of ECMA-262, Edition 5, 15.4.4.18
        // Reference: http://es5.github.com/#x15.4.4.18
        if (!Array.prototype.forEach) {
            Array.prototype.forEach = function (callback, thisArg) {
                var t = '', k;
                if (this == null) {
                    throw new TypeError(" this is null or not defined");
                }
                // 1. Let O be the result of calling ToObject passing the |this| value as the argument.
                var o = Object(this);
                // 2. Let lenValue be the result of calling the Get internal method of O with the argument "length".
                // 3. Let len be ToUint32(lenValue).
                var len = o.length >>> 0; // Hack to convert O.length to a UInt32
                // 4. If IsCallable(callback) is false, throw a TypeError exception.
                // See: http://es5.github.com/#x9.11
                if ({ }.toString.call(callback) != "[object Function]") {
                    throw new TypeError(callback + " is not a function");
                }
                // 5. If thisArg was supplied, let T be thisArg; else let T be undefined.
                if (thisArg) {
                    t = thisArg;
                }
                // 6. Let k be 0
                k = 0;
                // 7. Repeat, while k < len
                while (k < len) {
                    var kValue;
                    // a. Let Pk be ToString(k).
                    //   This is implicit for LHS operands of the in operator
                    // b. Let kPresent be the result of calling the HasProperty internal method of O with argument Pk.
                    //   This step can be combined with c
                    // c. If kPresent is true, then
                    if (k in o) {
                        // i. Let kValue be the result of calling the Get internal method of O with argument Pk.
                        kValue = o[k];
                        // ii. Call the Call internal method of callback with T as the this value and
                        // argument list containing kValue, k, and O.
                        callback.call(t, kValue, k, o);
                    }
                    // d. Increase k by 1.
                    k++;
                }
                // 8. return undefined
            };
        }

        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
                "use strict";
                if (this === void 0 || this === null) {
                    throw new TypeError();
                }
                var t = Object(this);
                var len = t.length >>> 0;
                if (len === 0) {
                    return -1;
                }
                var n = 0;
                if (arguments.length > 0) {
                    n = Number(arguments[1]);
                    if (n !== n) { // shortcut for verifying if it's NaN
                        n = 0;
                    } else if (n !== 0 && n !== Infinity && n !== -Infinity) {
                        n = (n > 0 || -1) * Math.floor(Math.abs(n));
                    }
                }
                if (n >= len) {
                    return -1;
                }
                var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
                for (; k < len; k++) {
                    if (k in t && t[k] === searchElement) {
                        return k;
                    }
                }
                return -1;
            };
        }

    //#endregion Prototype Extension

    function isArray(obj) {
        /// <summary> Returns true if the object is an array</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isArray(obj));
        return ret;
    };
    Csw.register('isArray', isArray);
    Csw.isArray = Csw.isArray || isArray;

    function array() {
        var retArray = []; 
        if(arguments.length > 0) {
            retArray = Array.prototype.slice.call(arguments, 0);
        }
        
        retArray.contains = retArray.contains || function (value) {
            return retArray.indexOf(value) != -1;
        };
        
        return retArray;
    };
    Csw.register('array', array);
    Csw.array = Csw.array || array;
    
    function makeSequentialArray(start, end) {
        var ret = array(),
            i = +start;
        end = +end;
        if (Csw.isNumber(start) &&
            Csw.isNumber(end)) {
            for ( ; i <= end; i += 1) {
                ret.push(i);
            }
        }
        return ret;
    };    
    Csw.register('makeSequentialArray', makeSequentialArray);
    Csw.makeSequentialArray = Csw.makeSequentialArray || makeSequentialArray;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswBoolean() {
    'use strict';

    function bool(str, isTrueIfNull) {

        var retBool;
        function isTrue () {
            /// <summary>
            ///   Returns true if the input is true, 'true', '1' or 1.
            ///   &#10;1 Returns false if the input is false, 'false', '0' or 0.
            ///   &#10;2 Otherwise returns false and (if debug) writes an error to the log.
            /// </summary>
            /// <param name="str" type="Object">
            ///     String or object to test
            /// </param>
            /// <returns type="Bool" />
            var ret;
            var truthy = Csw.string(str).toLowerCase().trim();
            if (truthy === 'true' || truthy === '1') {
                ret = true;
            } else if (truthy === 'false' || truthy === '0') {
                ret = false;
            } else if (isTrueIfNull && Csw.isNullOrEmpty(str)) {
                ret = true;
            } else {
                ret = false;
            }
            return ret;
        }

        retBool = isTrue();
        
        return retBool;
    }

    Csw.register('bool', bool);
    Csw.bool = Csw.bool || bool;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientChanges() {
    'use strict';
    
    var _changed = 0;
    var _checkChangesEnabled = true;

    function setChanged() {
        if (_checkChangesEnabled) {
            _changed = 1;
        }
    }
    Csw.register('setChanged',setChanged);
    Csw.setChanged = Csw.setChanged || setChanged;

    function unsetChanged() {
        if (_checkChangesEnabled) {
            _changed = 0;
        }
    }
    Csw.register('unsetChanged',unsetChanged);
    Csw.unsetChanged = Csw.unsetChanged || unsetChanged;
    
    function checkChanges() {
        if (_checkChangesEnabled && _changed === 1) {
            return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
        }
    }
    Csw.register('checkChanges',checkChanges);
    Csw.checkChanges = Csw.checkChanges || checkChanges;
    
    function manuallyCheckChanges() {
        var ret = true;
        if (_checkChangesEnabled && _changed === 1) {
            /* remember: confirm is globally blocking call */
            ret = confirm('Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page.');

            // this serves several purposes:
            // 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
            // 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
            if (ret) {
                _changed = 0;
            }
        }
        return ret;
    }
    Csw.register('manuallyCheckChanges',manuallyCheckChanges);
    Csw.manuallyCheckChanges = Csw.manuallyCheckChanges || manuallyCheckChanges;
    
    function initCheckChanges() {
        // Assign the checkchanges event to happen onbeforeunload
        if (false === Csw.isNullOrEmpty(window.onbeforeunload)) {
            window.onbeforeunload = function () {
                var f = window.onbeforeunload;
                var ret = f();
                if (ret) {
                    return checkChanges();
                } else {
                    return false;
                }
            };
        } else {
            window.onbeforeunload = function () {
                return checkChanges();
            };
        }
    }
    Csw.register('initCheckChanges',initCheckChanges);
    Csw.initCheckChanges = Csw.initCheckChanges || initCheckChanges;

    $(window).load(initCheckChanges);
    
}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientSession() {
    'use strict';
    
    var expiretime = '';
    var expiretimeInterval;
    var expiredInterval;

    function getExpireTime() {
        return expiretime;
    }
    Csw.register('getExpireTime', getExpireTime);
    Csw.getExpireTime = Csw.getExpireTime || getExpireTime;
    
    function setExpireTime(value) {
        expiretime = value;
        setExpireTimeInterval ();
    }
    Csw.register('setExpireTime', setExpireTime);
    Csw.setExpireTime = Csw.setExpireTime || setExpireTime;

    function setExpireTimeInterval() {
        clearInterval (expiretimeInterval);
        clearInterval (expiredInterval);
        expiretimeInterval = setInterval (function () {
            checkExpireTime ();
        }, 60000);
        expiredInterval = setInterval (function () {
            checkExpired ();
        }, 60000);
    }
    Csw.register('setExpireTimeInterval', setExpireTimeInterval);
    Csw.setExpireTimeInterval = Csw.setExpireTimeInterval || setExpireTimeInterval;

    function checkExpired() {
        var now = new Date ();
        if (Date.parse (expiretime) - Date.parse (now) < 0) {
            clearInterval (expiredInterval);
            logout ();
        }
    }
    Csw.register('setExpireTimeInterval', setExpireTimeInterval);
    Csw.setExpireTimeInterval = Csw.setExpireTimeInterval || setExpireTimeInterval;

    function checkExpireTime() {
        var now = new Date ();
        if (Date.parse (expiretime) - Date.parse (now) < 180000)     	// 3 minutes until timeout
        {
            clearInterval (expiretimeInterval);
            $.CswDialog ('ExpireDialog', {
                'onYes': function () {
                    Csw.ajax ({
                        'url': '/NbtWebApp/wsNBT.asmx/RenewSession',
                        'success': function () {
                        }
                    });
                }
            });
        }
    }
    Csw.register('checkExpireTime', checkExpireTime);
    Csw.checkExpireTime = Csw.checkExpireTime || checkExpireTime;

    function handleAuthenticationStatus(options) {
        var o = {
            status: '',
            success: function () {
            },
            failure: function () {
            },
            usernodeid: '',
            usernodekey: '',
            passwordpropid: '',
            ForMobile: false
        };
        if (options) {
            $.extend(o, options);
        }

        var txt = '';
        var goodEnoughForMobile = false; //Ignore password expirery and license accept for Mobile for now
        switch (o.status) {
            case 'Authenticated':
                o.success();
                break;
            case 'Deauthenticated':
                o.success(); // yes, o.success() is intentional here.
                break;
            case 'Failed':
                txt = "Invalid login.";
                break;
            case 'Locked':
                txt = "Your account is locked.  Please see your account administrator.";
                break;
            case 'Deactivated':
                txt = "Your account is deactivated.  Please see your account administrator.";
                break;
            case 'ModuleNotEnabled':
                txt = "This feature is not enabled.  Please see your account administrator.";
                break;
            case 'TooManyUsers':
                txt = "Too many users are currently connected.  Try again later.";
                break;
            case 'NonExistentAccessId':
                txt = "Invalid login.";
                break;
            case 'NonExistentSession':
                txt = "Your session has timed out.  Please login again.";
                break;
            case 'Unknown':
                txt = "An Unknown Error Occurred";
                break;
            case 'TimedOut':
                goodEnoughForMobile = true;
                txt = "Your session has timed out.  Please login again.";
                break;
            case 'ExpiredPassword':
                goodEnoughForMobile = true;
                if (!o.ForMobile) {
                    $.CswDialog('EditNodeDialog', {
                        nodeids: [o.usernodeid],
                        nodekeys: [o.usernodekey],
                        filterToPropId: o.passwordpropid,
                        title: 'Your password has expired.  Please change it now:',
                        onEditNode: function () {
                            o.success();
                        }
                    });
                }
                break;
            case 'ShowLicense':
                goodEnoughForMobile = true;
                if (!o.ForMobile) {
                    $.CswDialog('ShowLicenseDialog', {
                        'onAccept': function () {
                            o.success();
                        },
                        'onDecline': function () {
                            o.failure('You must accept the license agreement to use this application');
                        }
                    });
                }
                break;
        }

        if (o.ForMobile &&
            (o.status !== 'Authenticated' && goodEnoughForMobile)) {
            o.success();
        } else if (false === Csw.isNullOrEmpty(txt) && o.status !== 'Authenticated') {
            o.failure(txt, o.status);
        }
    } // _handleAuthenticationStatus()
    Csw.register('handleAuthenticationStatus', handleAuthenticationStatus);
    Csw.handleAuthenticationStatus = Csw.handleAuthenticationStatus || handleAuthenticationStatus;
    
    function logout(options) {
        var o = {
            DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
            onDeauthenticate: function () {
            }
        };

        if (options) {
            $.extend(o, options);
        }

        Csw.ajax({
            url: o.DeauthenticateUrl,
            data: {},
            success: function () { //data
                finishLogout();
                o.onDeauthenticate();
            } // success{}
        });
    } // logout
    Csw.register('logout', logout, true);
    Csw.logout = Csw.logout || logout;

    function finishLogout() {
        var logoutpath = $.CswCookie('get', CswCookieName.LogoutPath);
        $.CswCookie('clearAll');
        if (false === Csw.isNullOrEmpty(logoutpath)) {
            window.location = logoutpath;
        } else {
            window.location = Csw.getGlobalProp('homeUrl');
        }
    }
    Csw.register('finishLogout', finishLogout, true);
    Csw.finishLogout = Csw.finishLogout || finishLogout;

    function isAdministrator(options) {
        var o = {
            'Yes': function () {
            },
            'No': function () {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        Csw.ajax({
            url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
            success: function (data) {
                if (data.Administrator === "true") {
                    o.Yes();
                } else {
                    o.No();
                }
            }
        });
    } // IsAdministrator()
    Csw.register('isAdministrator', isAdministrator);
    Csw.isAdministrator = Csw.isAdministrator || isAdministrator;
    
}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientState() {
    'use strict';

    function clientState() {

        function clearCurrent () {
            /// <summary> Clear all current state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            $.CswCookie('set', CswCookieName.LastViewId, $.CswCookie('get', CswCookieName.CurrentViewId));
            $.CswCookie('set', CswCookieName.LastViewMode, $.CswCookie('get', CswCookieName.CurrentViewMode));
            $.CswCookie('set', CswCookieName.LastActionName, $.CswCookie('get', CswCookieName.CurrentActionName));
            $.CswCookie('set', CswCookieName.LastActionUrl, $.CswCookie('get', CswCookieName.CurrentActionUrl));
            $.CswCookie('set', CswCookieName.LastReportId, $.CswCookie('get', CswCookieName.CurrentReportId));

            $.CswCookie('clear', CswCookieName.CurrentViewId);
            $.CswCookie('clear', CswCookieName.CurrentViewMode);
            $.CswCookie('clear', CswCookieName.CurrentActionName);
            $.CswCookie('clear', CswCookieName.CurrentActionUrl);
            $.CswCookie('clear', CswCookieName.CurrentReportId);
            return true;
        }
        
        function setCurrentView (viewid, viewmode) {
            /// <summary> Store the current view in a cookie.</summary>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            if (false === Csw.isNullOrEmpty(viewid) && false === Csw.isNullOrEmpty(viewmode)) {
                $.CswCookie('set', CswCookieName.CurrentViewId, viewid);
                $.CswCookie('set', CswCookieName.CurrentViewMode, viewmode);
            }
            return true;
        }

        function setCurrentAction (actionname, actionurl) {
            /// <summary> Store the current action in a cookie.</summary>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            $.CswCookie('set', CswCookieName.CurrentActionName, actionname);
            $.CswCookie('set', CswCookieName.CurrentActionUrl, actionurl);
            return true;
        }

        function setCurrentReport (reportid) {
            /// <summary> Store the current report in a cookie.</summary>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            $.CswCookie('set', CswCookieName.CurrentReportId, reportid);
            return true;
        }

        function getCurrent () {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: $.CswCookie('get', CswCookieName.CurrentViewId),
                viewmode: $.CswCookie('get', CswCookieName.CurrentViewMode),
                actionname: $.CswCookie('get', CswCookieName.CurrentActionName),
                actionurl: $.CswCookie('get', CswCookieName.CurrentActionUrl),
                reportid: $.CswCookie('get', CswCookieName.CurrentReportId)
            };
        }

        function getLast () {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: $.CswCookie('get', CswCookieName.LastViewId),
                viewmode: $.CswCookie('get', CswCookieName.LastViewMode),
                actionname: $.CswCookie('get', CswCookieName.LastActionName),
                actionurl: $.CswCookie('get', CswCookieName.LastActionUrl),
                reportid: $.CswCookie('get', CswCookieName.LastReportId)
            };
        }

        function setCurrent (json) {
            
            clearCurrent();
            $.CswCookie('set', CswCookieName.CurrentViewId, json.viewid);
            $.CswCookie('set', CswCookieName.CurrentViewMode, json.viewmode);
            $.CswCookie('set', CswCookieName.CurrentActionName, json.actionname);
            $.CswCookie('set', CswCookieName.CurrentActionUrl, json.actionurl);
            $.CswCookie('set', CswCookieName.CurrentReportId, json.reportid);
            return true;
        }

        return {
            clearCurrent: clearCurrent,
            setCurrentView: setCurrentView,
            setCurrentAction: setCurrentAction,
            setCurrentReport: setCurrentReport,
            getCurrent: getCurrent,
            getLast: getLast,
            setCurrent: setCurrent
        };

    }
    Csw.register('clientState', clientState);
    Csw.clientState = Csw.clientState || clientState;    
    Csw.clientState.clearCurrent = Csw.clientState.clearCurrent || clientState.clearCurrent;
    Csw.clientState.setCurrentView = Csw.clientState.setCurrentView || clientState.setCurrentView;
    Csw.clientState.setCurrentAction = Csw.clientState.setCurrentAction || clientState.setCurrentAction;
    Csw.clientState.setCurrentReport = Csw.clientState.setCurrentReport || clientState.setCurrentReport;
    Csw.clientState.getCurrent = Csw.clientState.getCurrent || clientState.getCurrent;
    Csw.clientState.getLast = Csw.clientState.getLast || clientState.getLast;
    Csw.clientState.setCurrent = Csw.clientState.setCurrent || clientState.setCurrent;
    
}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var dateTimeMinValue = new Date('1/1/0001 12:00:00 AM');
    Csw.register('dateTimeMinValue', dateTimeMinValue);
    Csw.dateTimeMinValue = Csw.dateTimeMinValue || dateTimeMinValue;
    
    function serverDateFormatToJQuery(serverDateFormat) {
        var ret = serverDateFormat;
        ret = ret.replace( /M/g , 'm');
        ret = ret.replace( /mmm/g , 'M');
        ret = ret.replace( /yyyy/g , 'yy');
        return ret;
    }
    Csw.register('serverDateFormatToJQuery', serverDateFormatToJQuery);
    Csw.serverDateFormatToJQuery = Csw.serverDateFormatToJQuery || serverDateFormatToJQuery;
    
    function serverTimeFormatToJQuery(serverTimeFormat) {
        "use strict";
        var ret = serverTimeFormat;
        return ret;
    }
    Csw.register('serverTimeFormatToJQuery', serverTimeFormatToJQuery);
    Csw.serverTimeFormatToJQuery = Csw.serverTimeFormatToJQuery || serverTimeFormatToJQuery;

    function isDate(obj) {
        /// <summary> Returns true if the object is a Date</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (obj instanceof Date);
        return ret;
    }
    Csw.register('isDate', isDate);
    Csw.isDate = Csw.isDate || isDate;

    function validateTime(value) {
        var isValid = true;
        var regex = /^(\d?\d):(\d\d)\s?([APap][Mm])?$/g ;
        var match = regex.exec(value);
        if (match === null) {
            isValid = false;
        } else {
            var hour = parseInt(match[1]);
            var minute = parseInt(match[2]);
            if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60) {
                isValid = false;
            }
        }
        return isValid;
    } // validateTime()
    Csw.register('validateTime', validateTime);
    Csw.validateTime = Csw.validateTime || validateTime;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';
    // because IE 8 doesn't support console.log unless the console is open (*duh*)
    function log(s, includeCallStack) {
        /// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
        /// <param name="s" type="String"> String to output </param>
        /// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
        var extendedLog = '';
        if (Csw.bool(includeCallStack)) {
            extendedLog = console.trace();
        }
        try {
            if (false === Csw.isNullOrEmpty(extendedLog)) {
                console.log(s, extendedLog);
            } else {
                console.log(s);
            }
        } catch (e) {
            alert(s);
            if (false === Csw.isNullOrEmpty(extendedLog)) {
                alert(extendedLog);
            }
        }
    }
    Csw.register('log', log);
    Csw.log = Csw.log || log;

    function iterate(obj) {
        var str = '';
        for (var x in obj) {
            str = str + x + "=" + obj[x] + "<br><br>";
        }
        var popup = window.open("", "popup");
        if (popup !== null) {
            popup.document.write(str);
        } else {
            console.log("iterate() error: No popup!");
        }
    };
    Csw.register('iterate', iterate);
    Csw.iterate = Csw.iterate || iterate;

    function doLogging(value) {
        var ret = undefined;
        if (Csw.hasWebStorage()) {
            if (arguments.length === 1) {
                localStorage['doLogging'] = Csw.bool(value);
            }
            ret = Csw.bool(localStorage['doLogging']);
        }
        return ret;
    }
    Csw.register('doLogging', doLogging);
    Csw.doLogging = Csw.doLogging || doLogging;
    
    function debugOn(value) {
        var ret = undefined;
        if (Csw.hasWebStorage()) {
            if (arguments.length === 1) {
                localStorage['debugOn'] = Csw.bool(value);
            }
            ret = Csw.bool(localStorage['debugOn']);
        }
        return ret;
    };
    Csw.register('debugOn', debugOn);
    Csw.debugOn = Csw.debugOn || debugOn;
    
    var cacheLogInfo = function (logger) {
        if (doLogging()) {
            if (hasWebStorage()) {
                if (undefined !== logger.setEnded) {
                    logger.setEnded();
                }
                var logStorage = CswClientDb();
                var log = logStorage.getItem('debuglog');
                log += logger.toHtml();

                logStorage.setItem('debuglog', log);
            }
        }
    }
    Csw.register('cacheLogInfo', cacheLogInfo);
    Csw.cacheLogInfo = Csw.cacheLogInfo || cacheLogInfo;

    function purgeLogInfo() {
        if (hasWebStorage()) {
            window.sessionStorage.clear();
        }
    }
    Csw.register('purgeLogInfo', purgeLogInfo);
    Csw.purgeLogInfo = Csw.purgeLogInfo || purgeLogInfo;
    
}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';
    
    function makeErrorObj(errorType, friendlyMsg, esotericMsg) {
        /// <summary>Generates a Csw Error object suitable for displaying a client-side error.</summary>
        /// <param name="errorType" type="Enum"> Error type: Error or Warning </param>
        /// <param name="friendlyMsg" type="String"> Friendly message. </param>
        /// <param name="esotericMsg" type="String"> (Optional) Error message with Developer context. </param>
        /// <returns type="Object"> The error object. </returns>
        return {
            type: Csw.string(errorType, Csw.enums.ErrorType.warning.name),
            message: Csw.string(friendlyMsg),
            detail: Csw.string(esotericMsg)
        };
    }
    Csw.register('makeErrorObj', makeErrorObj);
    Csw.makeErrorObj = Csw.makeErrorObj || makeErrorObj;

    function error(errorJson) {
        /// <summary>Displays an error message.</summary>
        /// <param name="errorJson" type="Object"> An error object. Should contain type, message and detail properties.</param>
        /// <returns type="Boolean">True</returns>
        var e = {
            'type': '',
            'message': '',
            'detail': '',
            'display': true
        };
        if (errorJson) {
            $.extend(e, errorJson);
        }
        
        var $errorsdiv = $('#DialogErrorDiv');
        if ($errorsdiv.length <= 0) {
            $errorsdiv = $('#ErrorDiv');
        }

        if ($errorsdiv.length > 0 && Csw.bool(e.display)) {
            $errorsdiv.CswErrorMessage({'type': e.type, 'message': e.message, 'detail': e.detail});
        } else {
            log(e.message + '; ' + e.detail);
        }
        return true;
    } // CswError()
    Csw.register('error', error);
    Csw.error = Csw.error || error;

    function errorHandler(errorMsg, includeCallStack, includeLocalStorage, doAlert) {
        if (Csw.hasWebStorage() && includeLocalStorage) {
            log(localStorage);
        }
        if (doAlert) {
            $.CswDialog('ErrorDialog', errorMsg);
            //alert('Error: ' + error.message + ' (Code ' + error.code + ')');
        } else {
            log('Error: ' + errorMsg.message + ' (Code ' + errorMsg.code + ')', includeCallStack);
        }
    }
    Csw.register('errorHandler', errorHandler);
    Csw.errorHandler = Csw.errorHandler || errorHandler;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    function makeDelegate(method, options) {
        /// <summary>
        /// Returns a function with the argument parameter of the value of the current instance of the object.
        /// </summary>
        /// <param name="method" type="Function"> A function to delegate. </param>
        /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
        /// <returns type="Function">A delegate function: function (options)</returns>
        return function () { method(options); };
    }
    Csw.register('makeDelegate', makeDelegate);
    Csw.makeDelegate = Csw.makeDelegate || makeDelegate;
   
    function makeEventDelegate(method, options) {
        /// <summary>
        /// Returns a function with the event object as the first parameter, and the current value of options as the second parameter.
        /// </summary>
        /// <param name="method" type="Function"> A function to delegate. </param>
        /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
        /// <returns type="Function">A delegate function: function (eventObj, options)</returns>
        return function (eventObj) { method(eventObj, options); };
    }
    Csw.register('makeEventDelegate', makeEventDelegate);
    Csw.makeEventDelegate = Csw.makeEventDelegate || makeEventDelegate;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswFunction() {
    'use strict';

     function tryExec(func) {
        /// <summary> If the supplied argument is a function, execute it. </summary>
        /// <param name="func" type="Function"> Function to evaluate </param>
        /// <returns type="undefined" />
        if (isFunction(func)) {
            func.apply(this, Array.prototype.slice.call(arguments, 1));
        }
    };
    Csw.register('tryExec', tryExec);
    Csw.tryExec = Csw.tryExec || tryExec;

    function isFunction(obj) {
        /// <summary> Returns true if the object is a function</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isFunction(obj));
        return ret;
    };
    Csw.register('isFunction', isFunction);
    Csw.isFunction = Csw.isFunction || isFunction;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswMenu() {
    'use strict';

    function goHome() {
        clearCurrent();
        window.location = homeUrl;
    }
    Csw.register('goHome',goHome);
    Csw.goHome = Csw.goHome || goHome;

    function handleMenuItem(options) {
        /// <param name="$" type="jQuery" />
        var o = {
            $ul: '',
            itemKey: '',
            itemJson: '',
            onLogout: null, // function () { },
            onAlterNode: null, // function (nodeid, nodekey) { },
            onSearch: {
                onViewSearch: null, // function () { }, 
                onGenericSearch: null // function () { }
            },
            onMultiEdit: null, //function () { },
            onEditView: null, //function (viewid) { },
            onSaveView: null, //function (newviewid) { },
            onQuotas: null, // function () { },
            onSessions: null, // function () { },
            Multi: false,
            NodeCheckTreeId: ''
        };
        if (options) {
            $.extend(o, options);
        }
        var $li;
        var json = o.itemJson;
        var href = Csw.string(json.href);
        var text = Csw.string(o.itemKey);
        var popup = Csw.string(json.popup);
        var action = Csw.string(json.action);

        if (false === Csw.isNullOrEmpty(href)) {
            $li = $('<li><a href="' + href + '">' + text + '</a></li>')
                .appendTo(o.$ul);
        } else if (false == Csw.isNullOrEmpty(popup)) {
            $li = $('<li class="headermenu_dialog"><a href="' + popup + '" target="_blank">' + text + '</a></li>')
                .appendTo(o.$ul);
        } else if (false === Csw.isNullOrEmpty(action)) {
            $li = $('<li><a href="#">' + text + '</a></li>')
                .appendTo(o.$ul);
            var $a = $li.children('a');
            var nodeid = Csw.string(json.nodeid);
            var nodename = Csw.string(json.nodename);
            var viewid = Csw.string(json.viewid);

            switch (action) {
                case 'About':
                    $a.click(function () {
                        $.CswDialog('AboutDialog');
                        return false;
                    });
                    break;
                case 'AddNode':
                    $a.click(function () {
                        $.CswDialog('AddNodeDialog', {
                            text: text,
                            nodetypeid: Csw.string(json.nodetypeid),
                            relatednodeid: Csw.string(json.relatednodeid), //for Grid Props
                            relatednodetypeid: Csw.string(json.relatednodetypeid), //for NodeTypeSelect
                            onAddNode: o.onAlterNode
                        });
                        return false;
                    });
                    break;
                case 'DeleteNode':
                    $a.click(function () {
                        $.CswDialog('DeleteNodeDialog', {
                            nodenames: [nodename],
                            nodeids: [nodeid],
                            onDeleteNode: o.onAlterNode,
                            NodeCheckTreeId: o.NodeCheckTreeId,
                            Multi: o.Multi
                        });

                        return false;
                    });
                    break;
                case 'editview':
                    $a.click(function () {
                        o.onEditView(viewid);
                        return false;
                    });
                    break;
                case 'CopyNode':
                    $a.click(function () {
                        $.CswDialog('CopyNodeDialog', {
                            nodename: nodename,
                            nodeid: nodeid,
                            onCopyNode: o.onAlterNode
                        });
                        return false;
                    });
                    break;            
                case 'PrintView':
                    $a.click(o.onPrintView);
                    break;                
                case 'PrintLabel':
                    $a.click(function () {
                        $.CswDialog('PrintLabelDialog', {
                            'nodeid': nodeid,
                            'propid': Csw.string(json.propid)
                        });
                        return false;
                    });
                    break;
                case 'Logout':
                    $a.click(function () {
                        o.onLogout();
                        return false;
                    });
                    break;                
                case 'Home':
                    $a.click(function () {
                        goHome();
                        return false;
                    });
                    break;
                case 'Profile':
                    $a.click(function () {
                        $.CswDialog('EditNodeDialog', {
                            nodeids: [json.userid],
                            filterToPropId: '',
                            title: 'User Profile',
                            onEditNode: null // function (nodeid, nodekey) { }
                        });
                        return false;
                    });
                    break;
                case 'ViewSearch':
                    $a.click(function () {
                        Csw.tryExec(o.onSearch.onViewSearch);
                    });
                    break;
                case 'GenericSearch':
                    $a.click(function () {
                        Csw.tryExec(o.onSearch.onGenericSearch);
                    });
                    break;
                case 'multiedit':
                    $a.click(o.onMultiEdit);
                    break;            
                case 'SaveViewAs':
                    $a.click(function () {
                        $.CswDialog('AddViewDialog', {
                            viewid: viewid,
                            viewmode: Csw.string(json.viewmode),
                            onAddView: o.onSaveView
                        });
                        return false;
                    });
                    break;
                case 'Quotas':
                    $a.click(o.onQuotas);
                    break;
                case 'Sessions':
                    $a.click(o.onSessions);
                    break;
            }
        } else {
            $li = $('<li>' + text + '</li>')
                .appendTo(o.$ul);
        }
        return $li;
    }
    Csw.register('handleMenuItem',handleMenuItem);
    Csw.handleMenuItem = Csw.handleMenuItem || handleMenuItem;
}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswNumber() {
    'use strict';

    var int32MinVal = -2147483648;
    Csw.register('int32MinVal', int32MinVal);
    Csw.int32MinVal = Csw.int32MinVal || int32MinVal;

    function number(inputNum, defaultNum) {
        function tryParseNumber() {
            /// <summary>
            ///   Returns the inputNum if !NaN, else returns the defaultNum
            /// </summary>
            /// <param name="inputNum" type="Number"> String to parse to number </param>
            /// <param name="defaultNum" type="Number"> Default value if not a number </param>
            /// <returns type="Number" />
            var ret = 0;
            var tryRet = +inputNum;

            if (false === isNaN(tryRet) && tryRet !== int32MinVal) {
                ret = tryRet;
            } else {
                tryRet = +defaultNum;
                if (false === isNaN(tryRet) && tryRet !== int32MinVal) {
                    ret = tryRet;
                }
            }
            return ret;
        }

        var retVal = tryParseNumber();

        return retVal;
    };
    Csw.register('number', number);
    Csw.number = Csw.number || number;
    
    function isNumber(obj) {
        /// <summary> Returns true if the object is typeof number</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (typeof obj === 'number');
        return ret;
    };
    Csw.register('isNumber', isNumber);
    Csw.isNumber = Csw.isNumber || isNumber;

    var isNumeric = function (obj) {
        /// <summary> Returns true if the input can be parsed as a Number </summary>
        /// <param name="str" type="Object"> String or object to test </param>
        /// <returns type="Boolean" />
        var ret = false;
        if (isNumber(obj) && false === Csw.isNullOrEmpty(obj)) {
            var num = +obj;
            if (false === isNaN(num)) {
                ret = true;
            }
        }
        return ret;
    };
    Csw.register('isNumeric', isNumeric);
    Csw.isNumeric = Csw.isNumeric || isNumeric;

    function validateFloatMinValue(value, minvalue) {
        var nValue = parseFloat(value);
        var nMinValue = parseFloat(minvalue);
        var isValid = true;

        if (nMinValue !== undefined) {
            if (nValue === undefined || nValue < nMinValue) {
                isValid = false;
            }
        }
        return isValid;
    } // validateFloatMinValue()

    function validateFloatMaxValue(value, maxvalue) {
        var nValue = parseFloat(value);
        var nMaxValue = parseFloat(maxvalue);
        var isValid = true;

        if (nMaxValue !== undefined) {
            if (nValue === undefined || nValue > nMaxValue) {
                isValid = false;
            }
        }
        return isValid;
    } // validateFloatMaxValue()

    function validateFloatPrecision(value, precision) {
        var isValid = true;

        var regex;
        if (precision > 0) {
            // Allow any valid number -- we'll round later
            regex = /^\-?\d*\.?\d*$/g ;
        } else {
            // Integers Only
            regex = /^\-?\d*$/g ;
        }
        if (isValid && !regex.test(value)) {
            isValid = false;
        }

        return isValid;
    } // validateFloatPrecision()

    function validateInteger(value) {
        // Integers Only
        var regex = /^\-?\d*$/g ;
        return (regex.test(value));
    } // validateInteger()
    

    function getMaxValueForPrecision(precision, maxPrecision) {
        var i,
            ret = '',
            precisionMax = maxPrecision || 6;
        if (precision > 0 &&
            precision <= precisionMax) {

            ret += '.';
            for (i = 0; i < precision; i += 1) {
                ret += '9';
            }
        }
        return ret;
    }
    Csw.register('getMaxValueForPrecision', getMaxValueForPrecision);
    Csw.getMaxValueForPrecision = Csw.getMaxValueForPrecision || getMaxValueForPrecision;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswObject() {
    'use strict';
    
     function isPlainObject(obj) {
        /// <summary> 
        ///    Returns true if the object is a JavaScript object.
        ///     &#10; isPlainObject(CswInput_Types) === true 
        ///     &#10; isPlainObject('CswInput_Types') === false
        /// </summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isPlainObject(obj));
        return ret;
    };
    Csw.register('isPlainObject', isPlainObject);
    Csw.isPlainObject = Csw.isPlainObject || isPlainObject;

    function isJQuery(obj) {
        /// <summary> Returns true if the object jQuery</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (obj instanceof jQuery);
        return ret;
    };
    Csw.register('isJQuery', isJQuery);
    Csw.isJQuery = Csw.isJQuery || isJQuery;
    
    function hasLength(obj) {
        /// <summary> Returns true if the object is an Array or jQuery</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (Csw.isArray(obj) || isJQuery(obj));
        return ret;
    };
    Csw.register('hasLength', hasLength);
    Csw.hasLength = Csw.hasLength || hasLength;
    
    function isGeneric(obj) {
        /// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (false === Csw.isFunction(obj) && false === hasLength(obj) && false === isPlainObject(obj));
        return ret;
    }
    Csw.register('isGeneric', isGeneric);
    Csw.isGeneric = Csw.isGeneric || isGeneric;

    function isNullOrEmpty(obj, checkLength) {
        /// <summary> Returns true if the input is null, undefined, or ''</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = isNullOrUndefined(obj);
        if (false === ret && isGeneric(obj)) {
            ret = ((Csw.trim(obj) === '') || (Csw.isDate(obj) && obj === Csw.dateTimeMinValue) || (Csw.isNumber(obj) && obj === Csw.int32MinVal));
        } else if (checkLength && hasLength(obj)) {
            ret = (obj.length === 0);
        }
        return ret;
    }
    Csw.register('isNullOrEmpty', isNullOrEmpty);
    Csw.isNullOrEmpty = Csw.isNullOrEmpty || isNullOrEmpty;

    function isInstanceOf(name, obj) {
        return (Csw.contains(name, obj) && Csw.bool(obj[name]));
    }
    Csw.register('isInstanceOf', isInstanceOf);
    Csw.isInstanceOf = Csw.isInstanceOf || isInstanceOf;

    function isNullOrUndefined(obj) {
        /// <summary> Returns true if the input is null or undefined</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = false;
        if (false === Csw.isFunction(obj)) {
            ret = obj === null || obj === undefined || ($.isPlainObject(obj) && $.isEmptyObject(obj));
        }
        return ret;
    }
    Csw.register('isNullOrUndefined', isNullOrUndefined);
    Csw.isNullOrUndefined = Csw.isNullOrUndefined || isNullOrUndefined;

    function tryParseObjByIdx(object, index, defaultStr) {
        /// <summary> Attempts to fetch the value at an array index. Null-safe.</summary>
        /// <param name="object" type="Object"> Object or array to parse </param>
        /// <param name="index" type="String"> Index or property to find </param>
        /// <param name="defaultStr" type="String"> Optional. String to use instead of '' if index does not exist. </param>
        /// <returns type="String">Parsed string</returns>
        var ret = '';
        if (false === Csw.isNullOrEmpty(defaultStr)) {
            ret = defaultStr;
        }
        if (Csw.contains(object, index)) {
            if (false === Csw.isNullOrUndefined(object[index])) {
                ret = object[index];
            }
        }
        return ret;
    }
    Csw.register('tryParseObjByIdx', tryParseObjByIdx);
    Csw.tryParseObjByIdx = Csw.tryParseObjByIdx || tryParseObjByIdx;

    function contains(object, index) {
        /// <summary>Determines whether an object or an array contains a property or index. Null-safe.</summary>
        /// <param name="object" type="Object"> An object to evaluate </param>
        /// <param name="index" type="String"> An index or property to find </param>
        /// <returns type="Boolean" />
        var ret = false;
        if (false === Csw.isNullOrUndefined(object)) {
            if (Csw.isArray(object)) {
                ret = object.indexOf(index) !== -1;
            }
            if (false === ret && object.hasOwnProperty(index)) {
                ret = true;
            }
        }
        return ret;
    }
    Csw.register('contains', contains);
    Csw.contains = Csw.contains || contains;
    
    function renameProperty(obj, oldName, newName) {
        /// <summary>Renames a property on a Object literal</summary>
        /// <param name="obj" type="Object"> Object containing property </param>
        /// <param name="oldName" type="String"> Current property name </param>
        /// <param name="newName" type="String"> New property name </param>
        /// <returns type="Object"> The modified object </returns>
        if (false === Csw.isNullOrUndefined(obj) && contains(obj, oldName)) {
            obj[newName] = obj[oldName];
            delete obj[oldName];
        }
        return obj;
    }
    Csw.register('renameProperty', renameProperty);
    Csw.renameProperty = Csw.renameProperty || renameProperty;

    function foundMatch(anObj, prop, value) {
        /// <summary>Determines whether a prop/value match can be found on a property</summary>
        /// <param name="anObj" type="Object"> Object containing property </param>
        /// <param name="prop" type="String"> Property name </param>
        /// <param name="value" type="Object"> Value to match </param>
        /// <returns type="Boolean"> True if succeeded </returns>
        var ret = false;
        if (false === Csw.isNullOrEmpty(anObj) && contains(anObj, prop) && anObj[prop] === value) {
            ret = true;
        }
        return ret;
    }
    Csw.register('foundMatch', foundMatch);
    Csw.foundMatch = Csw.foundMatch || foundMatch;

    function objectHelper(obj) {
        /// <summary>Find an object in a JSON object.</summary>
        /// <param name="obj" type="Object"> Object to search </param>
        /// <returns type="ObjectHelper"></returns>
        var thisObj = obj;
        var currentObj = null;
        var parentObj = thisObj;
        var currentKey;
        //var parentKey = null;

        function find(key, value) {
            /// <summary>Find a property's parent</summary>
            /// <param name="key" type="String"> Property name to match. </param>
            /// <param name="value" type="Object"> Property value to match </param>
            /// <returns type="Object"> Returns the value of the 'property' property which contains a matching key/value prop. </returns>
            var ret = false;
            if (foundMatch(thisObj, key, value)) {
                ret = thisObj;
                currentObj = ret;
                parentObj = ret;
                currentKey = key;
            }
            if (false === ret) {
                var onSuccess = function (childObj, childKey, parObj) {
                    var found = false;
                    if (foundMatch(childObj, key, value)) {
                        ret = childObj;
                        currentObj = ret;
                        parentObj = parObj;
                        currentKey = childKey;
                        found = true;
                    }
                    return found;
                };
                crawlObject(thisObj, onSuccess, true);
            }
            return ret;
        }

        function remove(key, value) {
            var onSuccess = function (childObj, childKey, parObj) {
                var deleted = false;
                if (foundMatch(childObj, key, value)) {
                    deleted = true;
                    delete parObj[childKey];
                    currentKey = null;
                    currentObj = null;
                    parentObj = parentObj;
                }
                return deleted;
            };
            return crawlObject(thisObj, onSuccess, true);
        }

        this.find = find;
        this.remove = remove;
        this.obj = thisObj;
        this.parentObj = parentObj;
        this.currentObj = currentObj;
        this.currentKey = currentObj;
    }
    Csw.register('ObjectHelper', objectHelper);
    Csw.ObjectHelper = Csw.ObjectHelper || objectHelper;

    function each(thisObj, onSuccess) {
        /// <summary>Iterates an Object or an Array and handles length property</summary>
        /// <param name="thisObj" type="Object"> An object to crawl </param>
        /// <param name="onSuccess" type="Function"> A function to execute on finding a property, which should return true to stop.</param>
        /// <returns type="Object">Returns the return of onSuccess</returns>
        //http://stackoverflow.com/questions/7356835/jquery-each-fumbles-if-non-array-object-has-length-property
        var ret = false;
        if (Csw.isFunction(onSuccess)) {
            if (Csw.isArray(thisObj) || (Csw.isPlainObject(thisObj) && false === contains(thisObj, 'length'))) {
                $.each(thisObj, function (key, value) {
                    var obj = thisObj[key];
                    ret = onSuccess(obj, key, thisObj, value);
                    return !ret; //false signals break
                });
            } else if (Csw.isPlainObject(thisObj)) {
                for (var childKey in thisObj) {
                    if (contains(thisObj, childKey)) {
                        var childObj = thisObj[childKey];
                        ret = onSuccess(childObj, childKey, thisObj);
                        if (ret) {
                            break;
                        }
                    }
                }
            }
        }
        return ret;
    }; // each()
    Csw.register('each', each);
    Csw.each = Csw.each || each;

    function crawlObject(thisObj, onSuccess, doRecursion) {
        /// <summary>Iterates (optionally recursively) an object and executes a function on each of its properties.</summary>
        /// <param name="thisObj" type="Object"> An object to crawl </param>
        /// <param name="onSuccess" type="Function"> A function to execute on finding a property. To force iteration to stop, onSuccess should return false. </param>
        /// <param name="doRecursion" type="Boolean"> If true, recurse on all properties. Recursion will stop if onSuccess returns false. </param>
        /// <returns type="Object">Returns the return of onSuccess</returns>
        //borrowed from http://code.google.com/p/shadejs
        var stopCrawling = false;
        var onEach = function (childObj, childKey, parentObj, value) {
            if (false === stopCrawling) {
                stopCrawling = Csw.bool(onSuccess(childObj, childKey, parentObj, value));
            }
            if (false === stopCrawling && doRecursion) {
                stopCrawling = Csw.bool(crawlObject(childObj, onSuccess, doRecursion));
            }
            return stopCrawling;
        };
        stopCrawling = each(thisObj, onEach);
        return stopCrawling;
    };
    Csw.register('crawlObject', crawlObject);
    Csw.crawlObject = Csw.crawlObject || crawlObject;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    //#region Prototype Extension

    // for IE 8
    String.prototype.trim = String.prototype.trim || function () {
        return this.replace( /^\s+|\s+$/g , '');
    };

    String.prototype.toUpperCaseFirstChar = String.prototype.toUpperCaseFirstChar || function () {
        return this.substr(0, 1).toUpperCase() + this.substr(1);
    };

    String.prototype.toLowerCaseFirstChar = String.prototype.toLowerCaseFirstChar || function () {
        return this.substr(0, 1).toLowerCase() + this.substr(1);
    };

    String.prototype.toUpperCaseEachWord = String.prototype.toUpperCaseEachWord || function (delim) {
        delim = delim || ' ';
        return this.split(delim).map(function (v) { return v.toUpperCaseFirstChar(); }).join(delim);
    };

    String.prototype.toLowerCaseEachWord = String.prototype.toLowerCaseEachWord || function (delim) {
        delim = delim || ' ';
        return this.split(delim).map(function (v) { return v.toLowerCaseFirstChar(); }).join(delim);
    };

    //#endregion Prototype Extension

    function string(inputStr, defaultStr) {

        function tryParseString() {
            /// <summary>
            ///   Returns the inputStr if !isNullOrEmpty, else returns the defaultStr
            /// </summary>
            /// <param name="inputStr" type="String"> String to parse </param>
            /// <param name="defaultStr" type="String"> Default value if null or empty </param>
            /// <returns type="String" />
            var ret = '';
            if (false === Csw.isPlainObject(inputStr) &&
                false === Csw.isFunction(inputStr) &&
                    false === Csw.isNullOrEmpty(inputStr)) {
                ret = inputStr.toString();
            } else if (false === Csw.isPlainObject(defaultStr) &&
                false === Csw.isFunction(defaultStr) &&
                    false === Csw.isNullOrEmpty(defaultStr)) {
                ret = defaultStr.toString();
            }
            return ret;
        }

        var retObj = tryParseString();

        retObj.val = function () {
            return inputStr;
        };

        retObj.trim = function () {
            inputStr = $.trim(inputStr);
            return inputStr;
        };

        retObj.contains = function (valTest) {
            return inputStr.indexOf(valTest) !== -1;
        };
        
        return retObj;

    }
    Csw.register('string', string);
    Csw.string = Csw.string || string;
    
    function isString(obj) {
        /// <summary> Returns true if the object is a String object. </summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = typeof obj === 'string' || Csw.isInstanceOf('string', obj);
        return ret;
    }
    Csw.register('isString', isString);
    Csw.isString = Csw.isString || isString;
    
    function trim(str) {
        /// <summary>Returns a string without left and right whitespace</summary>
        /// <param name="str" type="String"> String to parse </param>
        /// <returns type="String">Parsed string</returns>
        return $.trim(str);
    }
    Csw.register('trim', trim);
    Csw.trim = window.Csw.trim || trim;
    
    function startsWith(source, search) {
        return (source.substr(0, search.length) === search);
    }
    Csw.register('startsWith', startsWith);
    Csw.startsWith = Csw.startsWith || startsWith;
    
    function getTimeString(date, timeformat) {
        var militaryTime = false;
        if (false === Csw.isNullOrEmpty(timeformat) && timeformat === "H:mm:ss") {
            militaryTime = true;
        }

        var ret;
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();

        if (minutes < 10) {
            minutes = "0" + minutes;
        }
        if (seconds < 10) {
            seconds = "0" + seconds;
        }

        if (militaryTime) {
            ret = hours + ":" + minutes + ":" + seconds;
        } else {
            ret = (hours % 12) + ":" + minutes + ":" + seconds + " ";
            if (hours > 11) {
                ret += "PM";
            } else {
                ret += "AM";
            }
        }
        return ret;
    };
    Csw.register('getTimeString', getTimeString);
    Csw.getTimeString = Csw.getTimeString || getTimeString;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswTBD() {
    'use strict';

    function jsTreeGetSelected($treediv) {
        var idPrefix = $treediv.CswAttrDom('id');
        var $SelectedItem = $treediv.jstree('get_selected');
        var ret = {
            'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
            'id': Csw.string($SelectedItem.CswAttrDom('id')).substring(idPrefix.length),
            'text': Csw.string($SelectedItem.children('a').first().text()).trim(),
            '$item': $SelectedItem
        };
        return ret;
    };
    Csw.register('jsTreeGetSelected', jsTreeGetSelected);
    Csw.jsTreeGetSelected = Csw.jsTreeGetSelected || jsTreeGetSelected;

    function makeViewVisibilitySelect($table, rownum, label) {
        // Used by CswDialog and CswViewEditor
        var $visibilityselect;
        var $visroleselect;
        var $visuserselect;

        Csw.isAdministrator({
            'Yes': function () {

                $table.CswTable('cell', rownum, 1).append(label);
                var $parent = $table.CswTable('cell', rownum, 2);
                var id = $table.CswAttrDom('id');

                $visibilityselect = $('<select id="' + id + '_vissel" />')
                    .appendTo($parent);
                $visibilityselect.append('<option value="User">User:</option>');
                $visibilityselect.append('<option value="Role">Role:</option>');
                $visibilityselect.append('<option value="Global">Global</option>');

                $visroleselect = $parent.CswNodeSelect('init', {
                    'ID': id + '_visrolesel',
                    'objectclass': 'RoleClass'
                }).hide();
                $visuserselect = $parent.CswNodeSelect('init', {
                    'ID': id + '_visusersel',
                    'objectclass': 'UserClass'
                });

                $visibilityselect.change(function () {
                    var val = $visibilityselect.val();
                    if (val === 'Role') {
                        $visroleselect.show();
                        $visuserselect.hide();
                    } else if (val === 'User') {
                        $visroleselect.hide();
                        $visuserselect.show();
                    } else {
                        $visroleselect.hide();
                        $visuserselect.hide();
                    }
                }); // change
            } // yes
        }); // IsAdministrator

        return {
            'getvisibilityselect': function () {
                return $visibilityselect;
            },
            'getvisroleselect': function () {
                return $visroleselect;
            },
            'getvisuserselect': function () {
                return $visuserselect;
            }
        };

    }; // makeViewVisibilitySelect()
    Csw.register('makeViewVisibilitySelect', makeViewVisibilitySelect);
    Csw.makeViewVisibilitySelect = Csw.makeViewVisibilitySelect || makeViewVisibilitySelect;

    function openPopup(url, height, width) {
        var popup = window.open(url, null, 'height=' + height + ', width=' + width + ', status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes');
        popup.focus();
        return popup;
    };
    Csw.register('openPopup', openPopup);
    Csw.openPopup = Csw.openPopup || openPopup;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswTools() {
    'use strict';

//    var loadResource = function (filename, filetype, useJquery) {
//        var fileref, type = filetype || 'js';
//        switch (type) {
//            case 'js':
//                if (jQuery && (($.browser.msie && $.browser.version <= 8) || useJquery)) {
//                    $.ajax({
//                        url: '/NbtWebApp/' + filename,
//                        dataType: 'script'
//                    });
//                } else {
//                    fileref = document.createElement('script');
//                    fileref.setAttribute("type", "text/javascript");
//                    fileref.setAttribute("src", filename);
//                }
//                break;
//            case 'css':
//                fileref = document.createElement("link");
//                fileref.setAttribute("rel", "stylesheet");
//                fileref.setAttribute("type", "text/css");
//                fileref.setAttribute("href", filename);
//                break;
//        }
//        if (fileref) {
//            document.getElementsByTagName("head")[0].appendChild(fileref);
//        }
//    };
//    Csw.register('loadResource', loadResource);
//    Csw.loadResource = Csw.loadResource || loadResource;

    function makeId(options, prefix, suffix, delimiter) {
        /// <summary>
        ///   Generates an ID for DOM assignment
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.ID: Base ID string
        ///     &#10;2 - options.prefix: String prefix to prepend
        ///     &#10;3 - options.suffix: String suffix to append
        ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
        /// </param>
        /// <returns type="String">A concatenated string of provided values</returns>
        var o = {
            ID: '',
            prefix: Csw.string(prefix),
            suffix: Csw.string(suffix),
            Delimiter: Csw.string(delimiter, '_')
        };
        if (Csw.isPlainObject(options)) {
            $.extend(o, options);
        } else {
            o.ID = Csw.string(options);
        }
        
        var elementId = o.ID;
        if (false === Csw.isNullOrEmpty(o.prefix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId = o.prefix + o.Delimiter + elementId;
        }
        if (false === Csw.isNullOrEmpty(o.suffix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId += o.Delimiter + o.suffix;
        }
        return elementId;
    }
    Csw.register('makeId', makeId);
    Csw.makeId = Csw.makeId || makeId;
    
    function makeSafeId(options, prefix, suffix, delimiter) {
        /// <summary>   Generates a "safe" ID for DOM assignment </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.ID: Base ID string
        ///     &#10;2 - options.prefix: String prefix to prepend
        ///     &#10;3 - options.suffix: String suffix to append
        ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
        /// </param>
        /// <returns type="String">A concatenated string of provided values</returns>
        var o = {
            ID: '',
            prefix: Csw.string(prefix),
            suffix: Csw.string(suffix),
            Delimiter: Csw.string(delimiter, '_')
        };
        if (Csw.isPlainObject(options)) {
            $.extend(o, options);
        } else {
            o.ID = Csw.string(options);
        }
        
        var elementId = o.ID;
        var toReplace = [ /'/gi , / /gi , /\//g ];
        if (false === Csw.isNullOrEmpty(o.prefix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId = o.prefix + o.Delimiter + elementId;
        }
        if (false === Csw.isNullOrEmpty(o.suffix) && false === Csw.isNullOrEmpty(elementId)) {
            elementId += o.Delimiter + o.suffix;
        }
        for (var i = 0; i < toReplace.length; i++) {
            if (Csw.contains(toReplace, i)) {
                if (false === Csw.isNullOrEmpty(elementId)) {
                    elementId = elementId.replace(toReplace[i], '');
                }
            }
        }
        return elementId;
    }
    Csw.register('makeSafeId', makeSafeId);
    Csw.makeSafeId = Csw.makeSafeId || makeSafeId;

    function hasWebStorage() {
        var ret = (window.Modernizr.localstorage || window.Modernizr.sessionstorage);
        return ret;
    }
    Csw.register('hasWebStorage', hasWebStorage);
    Csw.hasWebStorage = Csw.hasWebStorage || hasWebStorage;
    
    function tryParseElement(elementId, $context) {
        /// <summary>Attempts to fetch an element from the DOM first through jQuery, then through JavaScript.</summary>
        /// <param name="elementId" type="String"> ElementId to find </param>
        /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
        /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
        var $ret = $('');
        var document = Csw.getGlobalProp('document');
        if (false === Csw.isNullOrEmpty(elementId)) {
            if (arguments.length === 2 && false === Csw.isNullOrEmpty($context)) {
                $ret = $('#' + elementId, $context);
            } else {
                $ret = $('#' + elementId);
            }
            if ($ret.length === 0) {
                $ret = $(document.getElementById(elementId));
            }
            if ($ret.length === 0) {
                $ret = $(document.getElementsByName(elementId));
            }
        }
        return $ret;
    }
    Csw.register('tryParseElement', tryParseElement);
    Csw.tryParseElement = Csw.tryParseElement || tryParseElement;
    
}());﻿(function CswWindow() {

    //#region Window

    //Case 24114: IE7 doesn't support web storage
    var newLocalStorage = (function storageClosure() {
            var storage = { };
            var keys = [];
            var length = 0;
            return {
                getItem: function (sKey) {
                    var ret = null;
                    if (sKey && storage.hasOwnProperty(sKey)) {
                        ret = storage[sKey];
                    }
                    return ret;
                },
                key: function (nKeyId) {
                    var ret = null;
                    if (keys.hasOwnProperty(nKeyId)) {
                        ret = keys[nKeyId];
                    }
                    return ret;
                },
                setItem: function (sKey, sValue) {
                    var ret = null;
                    if (sKey) {
                        if (false === storage.hasOwnProperty(sKey)) {
                            keys.push(sKey);
                            length += 1;
                        }
                        storage[sKey] = sValue;
                    }
                    return ret;
                },
                length: length,
                removeItem: function (sKey) {
                    var ret = false;
                    if (sKey && storage.hasOwnProperty(sKey)) {
                        keys.splice(sKey, 1);
                        length -= 1;
                        delete storage[sKey];
                        ret = true;
                    }
                    return ret;
                },
                clear: function () {
                    storage = { };
                    keys = [];
                    length = 0;
                    return true;
                },
                hasOwnProperty: function (sKey) {
                    return storage.hasOwnProperty(sKey);
                }
            };
        }());
        
    if (false === window.Modernizr.localstorage) {
        window.localStorage = newLocalStorage;
    }
    if (false === window.Modernizr.sessionstorage) {
        window.sessionStorage = newLocalStorage;
    }

    //#endregion Window


 }());

﻿(function () {
    'use strict';

    var constants = {
        unknownEnum: 'unknown'
    };
    Csw.register ('constants', constants);
    Csw.constants = Csw.constants || constants;

    var enums = {
        tryParse: function (cswEnum, enumMember, caseSensitive) {
            var ret = ChemSW.constants.unknownEnum;
            if (contains (cswEnum, enumMember)) {
                ret = cswEnum[enumMember];
            } else if (false === caseSensitive) {
                each (cswEnum, function (member) {
                    if (contains (cswEnum, member) &&
                        Csw.string (member).toLowerCase () === Csw.string (enumMember).toLowerCase ()) {
                        ret = member;
                    }
                });
            }
            return ret;
        },
        EditMode: {
            Edit: 'Edit',
            AddInPopup: 'AddInPopup',
            EditInPopup: 'EditInPopup',
            Demo: 'Demo',
            PrintReport: 'PrintReport',
            DefaultValue: 'DefaultValue',
            AuditHistoryInPopup: 'AuditHistoryInPopup',
            Preview: 'Preview'
        },
        ErrorType: {
            warning: {
                name: 'warning',
                cssclass: 'CswErrorMessage_Warning'
            },
            error: {
                name: 'error',
                cssclass: 'CswErrorMessage_Error'
            }
        },
        Events: {
            CswNodeDelete: 'CswNodeDelete'
        },
        CswInspectionDesign_WizardSteps: {
            step1: {step: 1, description: 'Select an Inspection Target'},
            step2: {step: 2, description: 'Select an Inspection Design'},
            step3: {step: 3, description: 'Upload Template'},
            step4: {step: 4, description: 'Review Inspection Design'},
            step5: {step: 5, description: 'Finish'},
            stepcount: 5
        },
        CswScheduledRulesGrid_WizardSteps: {
            step1: {step: 1, description: 'Select a Customer ID'},
            step2: {step: 2, description: 'Review the Scheduled Rules'},
            stepcount: 2
        },
        CswDialogButtons: {
            1: 'ok',
            2: 'ok/cancel',
            3: 'yes/no'
        },
        CswOnObjectClassClick: {
            reauthenticate: 'reauthenticate',
            home: 'home',
            refresh: 'refresh',
            url: 'url'
        }
    };
    Csw.register ('enums', enums);
    Csw.enums = Csw.enums || enums;
    
} ());

var EditMode = {
    Edit: { name: 'Edit' },
    AddInPopup: { name: 'AddInPopup' },
    EditInPopup: { name: 'EditInPopup' },
    Demo: { name: 'Demo' },
    PrintReport: { name: 'PrintReport' },
    DefaultValue: { name: 'DefaultValue' },
    AuditHistoryInPopup: { name: 'AuditHistoryInPopup' },
    Preview: { name: 'Preview' }
};

// for CswInput
var CswInput_Types = {
    button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
    color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
    'datetime-local': { id: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false }, defaultwidth: '' },
    hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
    password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
    range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' }
};

var CswViewMode = {
    grid: { name: 'Grid' },
    tree: { name: 'Tree' },
    list: { name: 'List' },
    table: { name: 'Table' }
};

var CswRateIntervalTypes = {
        WeeklyByDay: 'WeeklyByDay',
        MonthlyByDate: 'MonthlyByDate',
        MonthlyByWeekAndDay: 'MonthlyByWeekAndDay',
        YearlyByDate: 'YearlyByDate'
    };

var CswMultiEditDefaultValue = '[Unchanged]';

// for CswCookie
var CswCookieName = {
    SessionId: 'CswSessionId',
    Username: 'csw_username',
    LogoutPath: 'csw_logoutpath',
    CurrentNodeId: 'csw_currentnodeid',
    CurrentNodeKey: 'csw_currentnodekey',
    CurrentTabId: 'csw_currenttabid',
    CurrentActionName: 'csw_currentactionname',
    CurrentActionUrl: 'csw_currentactionurl',
    CurrentViewId: 'csw_currentviewid',
    CurrentViewMode: 'csw_currentviewmode',
    CurrentReportId: 'csw_currentreportid',
    LastActionName: 'csw_lastactionname',
    LastActionUrl: 'csw_lastactionurl',
    LastViewId: 'csw_lastviewid',
    LastViewMode: 'csw_lastviewmode',
    LastReportId: 'csw_lastreportid'
};

var CswAppMode = {
    mode: 'full'     
};

// For CswImageButton
var CswImageButton_ButtonType = {
    None: -1,
    Add: 27,
    ArrowNorth: 28,
    ArrowEast: 29,
    ArrowSouth: 30,
    ArrowWest: 31,
    Calendar: 6,
    CheckboxFalse: 18,
    CheckboxNull: 19,
    CheckboxTrue: 20,
    Clear: 4,
    Clock: 10,
    ClockGrey: 11,
    Configure: 26,
    Delete: 4,
    Edit: 3,
    Fire: 5,
    PageFirst: 23,
    PagePrevious: 24,
    PageNext: 25,
    PageLast: 22,
    PinActive: 17,
    PinInactive: 15,
    Print: 2,
    Refresh: 9,
    SaveStatus: 13,
    Select: 32,
    ToggleActive: 1,
    ToggleInactive: 0,
    View: 8
};

// for CswSearch
var CswSearch_CssClasses = {
    nodetype_select: { name: 'csw_search_nodetype_select' },
    property_select: { name: 'csw_search_property_select' }
};

// for CswViewEditor
var CswViewEditor_WizardSteps = {
    viewselect: { step: 1, description: 'Choose a View', divId: 'step1_viewselect' },
    attributes: { step: 2, description: 'Edit View Attributes', divId: 'step2_attributes' },
    relationships: { step: 3, description: 'Add Relationships', divId: 'step3_relationships' },
    properties: { step: 4, description: 'Select Properties', divId: 'step4_properties' },
    filters: { step: 5, description: 'Set Filters', divId: 'step5_filters' },
    tuning: { step: 6, description: 'Fine Tuning', divId: 'step6_tuning' }
};

// for CswViewPropFilter
var ViewBuilder_CssClasses = {
    subfield_select: { name: 'csw_viewbuilder_subfield_select' },
    filter_select: { name: 'csw_viewbuilder_filter_select' },
    default_filter: { name: 'csw_viewbuilder_default_filter' },
    filter_value: { name: 'csw_viewbuilder_filter_value' },
    metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
};

var CswDomElementEvent = {
    click: {name: 'click'},
    change: {name: 'change'},
    vclick: {name: 'vclick'},
    tap: {name: 'tap'}
};

var CswObjectClasses = {
    GenericClass: { name: 'GenericClass' },
    InspectionDesignClass: { name: 'InspectionDesignClass' }
};

var CswNodeSpecies = {
    Plain: { name: 'Plain' },
    More: { name: 'More' }
};

var CswSubField_Names = {
    Unknown: { name: 'unknown' }, 
    AllowedAnswers: { name: 'allowedanswers' },
    Answer: { name: 'answer' },
    Barcode: { name: 'barcode' },
    Blob: { name: 'blob' },
    Checked: { name: 'checked' },
    Column: { name: 'column' },
    Comments: { name: 'comments' },
    CompliantAnswers: { name: 'compliantanswers' },
    ContentType: { name: 'contenttype' },
    CorrectiveAction: { name: 'correctiveaction' },
    DateAnswered: { name: 'dateanswered' },
    DateCorrected: { name: 'datecorrected' },
    Href: { name: 'href' },
    Image: { name: 'image' },
    Interval: { name: 'interval' },
    IsCompliant: { name: 'iscompliant' },
    Mol: { name: 'mol' },
    Name: { name: 'name' },
    NodeID: { name: 'nodeid' },
    NodeType: { name: 'nodetype' },
    Number: { name: 'number' },
    Password: { name: 'password' },
    Path: { name: 'path' },
    Required: { name: 'required' },
    Row: { name: 'row' },
    Sequence: { name: 'sequence' },
    StartDateTime: { name: 'startdatetime' },
    Text: { name: 'text' },
    Units: { name: 'units' },
    Value: { name: 'value' },
    ViewID: { name: 'viewid' },
    ChangedDate: { name: 'changeddate' },
    Base: { name: 'base' },
    Exponent: { name: 'exponent' }
};

var CswSubFields_Map = {
    AuditHistoryGrid: { name: 'AuditHistoryGrid', subfields: { } },
    Barcode: { 
        name: 'Barcode', 
        subfields: {
            Barcode: CswSubField_Names.Barcode,
            Sequence: CswSubField_Names.Number
        } 
    },
    Button: {
        name: 'Button',
        subfields: {
            Text: CswSubField_Names.Text
        }
    },
    Composite: { name: 'Composite', subfields: { } },
    DateTime: {
         name: 'DateTime', 
         subfields: {
            Value: {
                Date: { name: 'date' },
                Time: { name: 'time' },
                DateFormat: { name: 'dateformat' },
                TimeFormat: { name: 'timeformat' }
            },
            DisplayMode: {
                Date: { name: 'Date' },
                Time: { name: 'Time' },
                DateTime: { name: 'DateTime' }
            } 
        } 
    },
    File: { name: 'File', subfields: { } },
    Grid: { name: 'Grid', subfields: { } },
    Image: { name: 'Image', subfields: { } },
    Link: {
         name: 'Link', 
         subfields: {
            Text: CswSubField_Names.Text,
            Href: CswSubField_Names.Href
        } 
    },
    List: {
         name: 'List', 
         subfields: {
            Value: CswSubField_Names.Value
        } 
    },
    Location: { name: 'Location', subfields: { } },
    LocationContents: { name: 'LocationContents', subfields: { } },
    Logical: {
         name: 'Logical', 
         subfields: {
            Checked: CswSubField_Names.Checked
        } 
    },
    LogicalSet: { name: 'LogicalSet', subfields: { } },
    Memo: {
         name: 'Memo', 
         subfields: {
            Text: CswSubField_Names.Text
        } 
    },
    MTBF: { name: 'MTBF', subfields: { } },
    MultiList: { name: 'MultiList', subfields: { } },
    NodeTypeSelect: { name: 'NodeTypeSelect', subfields: { } },
    Number: {
         name: 'Number', 
         subfields: {
            Value: CswSubField_Names.Value
        } 
    },
    Password: {
         name: 'Password', 
         subfields: {
            Password: CswSubField_Names.Password,
            ChangedDate: CswSubField_Names.ChangedDate
        } 
    },
    PropertyReference: { name: 'PropertyReference', subfields: { } },
    Quantity: {
         name: 'Quantity', 
         subfields: {
            Value: CswSubField_Names.Value,
            Units: CswSubField_Names.Number
        } 
    },
    Question: {
         name: 'Question', 
         subfields: {
            Answer: CswSubField_Names.Answer,
            CorrectiveAction: CswSubField_Names.CorrectiveAction,
            IsCompliant: CswSubField_Names.IsCompliant,
            Comments: CswSubField_Names.Comments,
            DateAnswered: CswSubField_Names.DateAnswered,
            DateCorrected: CswSubField_Names.DateCorrected
        } 
    },
    Relationship: { name: 'Relationship', subfields: { } },
    Scientific: { name: 'Scientific', subfields: { } },
    Sequence: { name: 'Sequence', subfields: { } },
    Static: {
         name: 'Static', 
         subfields: {
            Text: CswSubField_Names.Text
        } 
    },
    Text: {
         name: 'Text', 
         subfields: {
            Text: CswSubField_Names.Text
        } 
    },
    TimeInterval: { name: 'TimeInterval', subfields: { } },
    UserSelect: { name: 'UserSelect', subfields: { } },
    ViewPickList: { name: 'ViewPickList', subfields: { } },
    ViewReference: { name: 'ViewReference', subfields: { } }
};

// For CswViewEditor and CswViewContentTree
var viewEditClasses = {
    vieweditor_viewrootlink: { name: 'vieweditor_viewrootlink' },
    vieweditor_viewrellink: { name: 'vieweditor_viewrellink' },
    vieweditor_viewproplink: { name: 'vieweditor_viewproplink' },
    vieweditor_viewfilterlink: { name: 'vieweditor_viewfilterlink' },
    vieweditor_addfilter: { name: 'vieweditor_addfilter' },
    vieweditor_deletespan: { name: 'vieweditor_deletespan' },
    vieweditor_childselect: { name: 'vieweditor_childselect' }
};

var childPropNames = {
    root: { name: 'root' },
    childrelationships: { name: 'childrelationships' },
    properties: { name: 'properties' },
    filters: { name: 'filters' },
    propfilters: { name: 'filters' },
    filtermodes: { name: 'filtermodes' }
};


var CswNodeTree_DefaultSelect = {
    root: { name: 'root' },
    firstchild: { name: 'firstchild' },
    none: { name: 'none' }
};﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var copyNode = function (options) {
        var o = {
            'nodeid': '',
            'nodekey': '',
            'onSuccess': function () {
            },
            'onError': function () {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        var dataJson = {
            NodePk: o.nodeid
        };

        Csw.ajax({
            url: '/NbtWebApp/wsNBT.asmx/CopyNode',
            data: dataJson,
            success: function (result) {
                o.onSuccess(result.NewNodeId, '');
            },
            error: o.onError
        });
    };
    Csw.register('copyNode', copyNode);
    Csw.copyNode = Csw.copyNode || copyNode;

    var deleteNodes = function (options) {
        var o = {
            'nodeids': [],
            'nodekeys': [],
            'onSuccess': function () {
            },
            'onError': function () {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        if (!isArray(o.nodeids))  // case 22722
        {
            o.nodeids = [o.nodeids];
            o.nodekeys = [o.nodekeys];
        }

        var jData = {
            NodePks: o.nodeids,
            NodeKeys: o.nodekeys
        };

        Csw.ajax({
            url: '/NbtWebApp/wsNBT.asmx/DeleteNodes',
            data: jData,
            success: function () {
                // clear selected node cookies
                o.nodeid = $.CswCookie('clear', CswCookieName.CurrentNodeId);
                o.cswnbtnodekey = $.CswCookie('clear', CswCookieName.CurrentNodeKey);
                // returning '' will reselect the first node in the tree
                o.onSuccess('', '');
            },
            error: o.onError
        });
    };
    Csw.register('deleteNodes', deleteNodes);
    Csw.deleteNodes = Csw.deleteNodes || deleteNodes;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var $nodepreview = undefined;
    var nodeHoverIn = function (event, nodeid, cswnbtnodekey) {
        $nodepreview = $.CswNodePreview('open', {
            ID: nodeid + "_preview",
            nodeid: nodeid,
            cswnbtnodekey: cswnbtnodekey,
            eventArg: event
        });
    };
    Csw.register('nodeHoverIn', nodeHoverIn);
    Csw.nodeHoverIn = Csw.nodeHoverIn || nodeHoverIn;
    
    var nodeHoverOut = function () {
        if ($nodepreview !== undefined) {
            $nodepreview.CswNodePreview('close');
            $nodepreview = undefined;
        }
    };
    Csw.register('nodeHoverOut', nodeHoverOut);
    Csw.nodeHoverOut = Csw.nodeHoverOut || nodeHoverOut;

}());﻿/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var preparePropJsonForSave = function (isMulti, propData, attributes) {
        ///<summary>Takes property JSON from the form and modifies it in order to send back to the server.</summary>
        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
        var wasModified = false;
        if (false === Csw.isNullOrEmpty(propData)) {
            if (contains(propData, 'values')) {
                var propVals = propData.values;
                wasModified = preparePropJsonForSaveRecursive(isMulti, propVals, attributes);
            }
            propData.wasmodified = propData.wasmodified || wasModified;
        }
    };
    Csw.register('preparePropJsonForSave', preparePropJsonForSave);
    Csw.preparePropJsonForSave = Csw.preparePropJsonForSave || preparePropJsonForSave;

    var preparePropJsonForSaveRecursive = function (isMulti, propVals, attributes) {
        ///<summary>Recurses over the subfields and sub-subfields of a property to update its JSON.</summary>
        ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
        ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
        ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
        ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
        if (false === Csw.isNullOrEmpty(propVals)) {
            var wasModified = false;
            crawlObject(propVals, function (prop, key) {
                if (contains(attributes, key)) {
                    var attr = attributes[key];
                    //don't bother sending this to server unless it's changed
                    if (isPlainObject(attr)) {
                        wasModified = preparePropJsonForSaveRecursive(isMulti, propVals[key], attr) || wasModified;
                    } else if ((false === isMulti && propVals[key] !== attr) ||
                        (isMulti && false === isNullOrUndefined(attr) && attr !== CswMultiEditDefaultValue)) {
                        wasModified = true;
                        propVals[key] = attr;
                    }
                }
            }, false);
        } else {
            //debug
        }
        return wasModified;
    };

}());﻿/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = 'CswCookie';

    $.CswCookie = function (method) {
        var methods = {
            'get': function (cookiename) 
                {
                    var ret = $.cookie(cookiename);
                    if(ret == undefined)
                        ret = '';
                    return ret;
                },
            'set': function (cookiename, value) 
                {
                    $.cookie(cookiename, value);
                },
            'clear': function (cookiename)
                {
                    $.cookie(cookiename, '');
                },
            'clearAll': function () 
                {
                    for(var CookieName in CswCookieName) 
                    {
                        if(CswCookieName.hasOwnProperty(CookieName))
                        {
                            $.cookie(CswCookieName[CookieName], null);
                        }
                    }
                } // clearAll
            };

        // Method calling logic
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    

    };

})(jQuery);




