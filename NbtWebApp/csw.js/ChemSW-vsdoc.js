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
(function(e,g){var c=e.document,b=e.navigator,a=e.location;e.ChemSW=e.Csw=function(){var d={document:c,navigator:b,location:a,$:g,homeUrl:"Main.html"},i=["register"],f=["register","deregister","getGlobalProp","setGlobalProp"],h={},e=function(d,a,b){var c=!1;-1!==i.indexOf(d)&&(i.push(d),a[d]=!0,b&&-1!==f.indexOf(d)&&f.push(d),h[d]=a,c=!0);return c};h.register=e;var j=function(d){var a=!1;-1!==f.indexOf(d)&&(-1!==i.indexOf(d)&&i.splice(d,1),delete h[d],a=!0);return a};e("deregister",j);h.deregister=
h.deregister||j;j=function(a){a&&d.hasOwnProperty(a)?a=d[a]:(a={},g.extend(a,d));return a};e("getGlobalProp",j);h.getGlobalProp=h.getGlobalProp||j;j=function(a,b){var f=!1;a&&b&&d.hasOwnProperty(a)&&(d[a]=b,f=!0);return f};e("setGlobalProp",j);h.setGlobalProp=h.setGlobalProp||j;j=function(a,b){var f=!1;a&&b&&!1==d.hasOwnProperty(a)&&(d[a]=b,f=!0);return f};e("addGlobalProp",j);h.addGlobalProp=h.addGlobalProp||j;j=function(){return i.slice(0)};e("getCswMethods",j);h.getCswMethods=h.getCswMethods||
j;return h}()})(window,jQuery);(function(){var e,g;function c(){return 0<h}function b(d){var a={url:"",data:{},onloginfail:function(){Csw.finishLogout()},success:null,error:null,overrideError:!1,formobile:!1,async:!0,watchGlobal:!0};d&&$.extend(a,d);a.watchGlobal&&(h+=1);Csw.tryExec(k,a.watchGlobal);$.ajax({type:"POST",async:a.async,url:a.url,dataType:"json",contentType:"application/json; charset=utf-8",data:JSON.stringify(a.data),success:function(d,b,c){a.watchGlobal&&(h-=1);var i=$.parseJSON(d.d);void 0!==i.error?(!1===a.overrideError&&
f(c,{display:i.error.display,type:i.error.type,message:i.error.message,detail:i.error.detail},""),Csw.tryExec(a.error,i.error)):(d=Csw.string(i.AuthenticationStatus,"Unknown"),!1===a.formobile&&Csw.setExpireTime(Csw.string(i.timeout,"")),delete i.AuthenticationStatus,delete i.timeout,Csw.handleAuthenticationStatus({status:d,success:function(){Csw.tryExec(a.success,i)},failure:a.onloginfail,usernodeid:i.nodeid,usernodekey:i.cswnbtnodekey,passwordpropid:i.passwordpropid,ForMobile:a.formobile}));Csw.tryExec(j,
!0)},error:function(d,b){a.watchGlobal&&(h-=1);Csw.log("Webservice Request ("+a.url+") Failed: "+b);Csw.tryExec(a.error);Csw.tryExec(j,!1)}})}function a(d){var a={url:"",data:{},onloginfail:function(){Csw.finishLogout()},success:null,error:null,overrideError:!1,formobile:!1,async:!0,watchGlobal:!0};d&&$.extend(a,d);a.watchGlobal&&(h+=1);isFunction(k)&&k(a.watchGlobal);$.ajax({type:"GET",async:a.async,url:a.url,dataType:"json",data:JSON.stringify(a.data),success:function(d){a.watchGlobal&&(h-=1);!1===
Csw.isNullOrEmpty(d.error)?(!1===a.overrideError&&Csw.error({display:d.error.display,type:d.error.type,message:d.error.message,detail:d.error.detail}),Csw.isFunction(a.error)&&a.error(d.error)):Csw.isFunction(a.success)&&a.success(d);isFunction(j)&&j(!0)},error:function(d,b){a.watchGlobal&&(h-=1);Csw.log("Webservice Request ("+a.url+") Failed: "+b);isFunction(a.error)&&a.error();Csw.isFunction(j)&&j(!1)}})}function d(d){var a={url:"",data:{},stringify:!1,onloginfail:function(){Csw.finishLogout()},
success:function(){},error:function(){},formobile:!1,async:!0,watchGlobal:!0};d&&$.extend(a,d);!1===Csw.isNullOrEmpty(a.url)&&(a.watchGlobal&&(h+=1),$.ajax({type:"POST",async:a.async,url:a.url,dataType:"text",data:$.param(a.data),success:function(d,b,i){a.watchGlobal&&(h-=1);var c;c=$.browser.msie?$.xml(d):$(i.responseXML).children().first();"error"===c.first().get(0).nodeName?(f(i,{display:c.CswAttrNonDom("display"),type:c.CswAttrNonDom("type"),message:c.CswAttrNonDom("message"),detail:c.CswAttrNonDom("detail")},
""),a.error()):(d=Csw.string(c.CswAttrNonDom("authenticationstatus"),"Unknown"),a.formobile||Csw.setExpireTime(c.CswAttrNonDom("timeout")),Csw.handleAuthenticationStatus({status:d,success:function(){a.success(c)},failure:a.onloginfail,usernodeid:Csw.string(c.CswAttrNonDom("nodeid"),""),usernodekey:Csw.string(c.CswAttrNonDom("cswnbtnodekey"),""),passwordpropid:Csw.string(c.CswAttrNonDom("passwordpropid"),""),ForMobile:a.formobile}))},error:function(d,b){a.watchGlobal&&(h-=1);Csw.log("Webservice Request ("+
a.url+") Failed: "+b);a.error()}}))}function i(f,c,i){i=Csw.string(i);c=Csw.string(c);switch(i){case e:f=d(f);break;default:f=c.toUpperCase()===g?a(f):b(f)}return{ajax:f}}function f(a,d){Csw.error(d)}var h=0;Csw.register("ajaxInProgress",c);Csw.ajaxInProgress=Csw.ajaxInProgress||c;var k=null;Csw.register("onBeforeAjax",k);Csw.onBeforeAjax=Csw.onBeforeAjax||k;var j=null;Csw.register("onAfterAjax",j);Csw.onAfterAjax=Csw.onAfterAjax||j;g="GET";e="xml";Csw.register("ajax",i);Csw.ajax=Csw.ajax||i})();(function(){function e(a){return $.isArray(a)}function g(a,d){var b=-1,f=0,c=a.length;if(Csw.isFunction(a.indexOf))b=a.indexOf(d);else if(Csw.hasLength(a)&&0<c)for(;f<c;f++)if(a[f]===d){b=f;break}return b}function c(){var a=[];0<arguments.length&&(a=Array.prototype.slice.call(arguments,0));a.contains=a.contains||function(d){return-1!=a.cswIndexOf(d)};return a}function b(a,d){var b=c(),f=+a,d=+d;if(Csw.isNumber(a)&&Csw.isNumber(d))for(;f<=d;f+=1)b.push(f);return b}if(!Array.prototype.forEach)Array.prototype.forEach=
function(a,d){var b="",f;if(null==this)throw new TypeError(" this is null or not defined");var c=Object(this),e=c.length>>>0;if("[object Function]"!={}.toString.call(a))throw new TypeError(a+" is not a function");d&&(b=d);for(f=0;f<e;){var g;f in c&&(g=c[f],a.call(b,g,f,c));f++}};if(!Array.prototype.indexOf)Array.prototype.indexOf=function(a){if(void 0===this||null===this)throw new TypeError;var d=Object(this),b=d.length>>>0;if(0===b)return-1;var f=0;0<arguments.length&&(f=Number(arguments[1]),f!==
f?f=0:0!==f&&Infinity!==f&&-Infinity!==f&&(f=(0<f||-1)*Math.floor(Math.abs(f))));if(f>=b)return-1;for(f=0<=f?f:Math.max(b-Math.abs(f),0);f<b;f++)if(f in d&&d[f]===a)return f;return-1};Csw.register("isArray",e);Csw.isArray=Csw.isArray||e;Csw.register("cswIndexOf",g);Csw.cswIndexOf=Csw.cswIndexOf||g;Csw.register("array",c);Csw.array=Csw.array||c;Csw.register("makeSequentialArray",b);Csw.makeSequentialArray=Csw.makeSequentialArray||b})();(function(){function e(e,c){var b=Csw.string(e).toLowerCase().trim();return"true"===b||"1"===b?!0:"false"===b||"0"===b?!1:c&&Csw.isNullOrEmpty(e)?!0:!1}Csw.register("bool",e);Csw.bool=Csw.bool||e})();(function(){function e(){i&&(d=1)}function g(){i&&(d=0)}function c(){if(i&&1===d)return"If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button."}function b(){var a=!0;i&&1===d&&(a=confirm("Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page."))&&
(d=0);return a}function a(){window.onbeforeunload=!1===Csw.isNullOrEmpty(window.onbeforeunload)?function(){var a=window.onbeforeunload;return a()?c():!1}:function(){return c()}}var d=0,i=!0;Csw.register("setChanged",e);Csw.setChanged=Csw.setChanged||e;Csw.register("unsetChanged",g);Csw.unsetChanged=Csw.unsetChanged||g;Csw.register("checkChanges",c);Csw.checkChanges=Csw.checkChanges||c;Csw.register("manuallyCheckChanges",b);Csw.manuallyCheckChanges=Csw.manuallyCheckChanges||b;Csw.register("initCheckChanges",
a);Csw.initCheckChanges=Csw.initCheckChanges||a;$(window).load(a)})();(function(){function e(){return h}function g(a){h=a;c()}function c(){clearInterval(k);clearInterval(j);k=setInterval(function(){b()},6E4);j=setInterval(function(){var a=new Date;0>Date.parse(h)-Date.parse(a)&&(clearInterval(j),d())},6E4)}function b(){var a=new Date;18E4>Date.parse(h)-Date.parse(a)&&(clearInterval(k),$.CswDialog("ExpireDialog",{onYes:function(){Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/RenewSession",success:function(){}})}}))}function a(a){var d={status:"",success:function(){},failure:function(){},
usernodeid:"",usernodekey:"",passwordpropid:"",ForMobile:!1};a&&$.extend(d,a);var a="",b=!1;switch(d.status){case "Authenticated":d.success();break;case "Deauthenticated":d.success();break;case "Failed":a="Invalid login.";break;case "Locked":a="Your account is locked.  Please see your account administrator.";break;case "Deactivated":a="Your account is deactivated.  Please see your account administrator.";break;case "ModuleNotEnabled":a="This feature is not enabled.  Please see your account administrator.";
break;case "TooManyUsers":a="Too many users are currently connected.  Try again later.";break;case "NonExistentAccessId":a="Invalid login.";break;case "NonExistentSession":a="Your session has timed out.  Please login again.";break;case "Unknown":a="An Unknown Error Occurred";break;case "TimedOut":b=!0;a="Your session has timed out.  Please login again.";break;case "ExpiredPassword":b=!0;d.ForMobile||$.CswDialog("EditNodeDialog",{nodeids:[d.usernodeid],nodekeys:[d.usernodekey],filterToPropId:d.passwordpropid,
title:"Your password has expired.  Please change it now:",onEditNode:function(){d.success()}});break;case "ShowLicense":b=!0,d.ForMobile||$.CswDialog("ShowLicenseDialog",{onAccept:function(){d.success()},onDecline:function(){d.failure("You must accept the license agreement to use this application")}})}d.ForMobile&&"Authenticated"!==d.status&&b?d.success():!1===Csw.isNullOrEmpty(a)&&"Authenticated"!==d.status&&d.failure(a,d.status)}function d(a){var d={DeauthenticateUrl:"/NbtWebApp/wsNBT.asmx/deauthenticate",
onDeauthenticate:function(){}};a&&$.extend(d,a);Csw.ajax({url:d.DeauthenticateUrl,data:{},success:function(){i();d.onDeauthenticate()}})}function i(){var a=$.CswCookie("get",CswCookieName.LogoutPath);$.CswCookie("clearAll");window.location=!1===Csw.isNullOrEmpty(a)?a:Csw.getGlobalProp("homeUrl")}function f(a){var d={Yes:function(){},No:function(){}};a&&$.extend(d,a);Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/isAdministrator",success:function(a){"true"===a.Administrator?d.Yes():d.No()}})}var h="",k,j;Csw.register("getExpireTime",
e);Csw.getExpireTime=Csw.getExpireTime||e;Csw.register("setExpireTime",g);Csw.setExpireTime=Csw.setExpireTime||g;Csw.register("setExpireTimeInterval",c);Csw.setExpireTimeInterval=Csw.setExpireTimeInterval||c;Csw.register("setExpireTimeInterval",c);Csw.setExpireTimeInterval=Csw.setExpireTimeInterval||c;Csw.register("checkExpireTime",b);Csw.checkExpireTime=Csw.checkExpireTime||b;Csw.register("handleAuthenticationStatus",a);Csw.handleAuthenticationStatus=Csw.handleAuthenticationStatus||a;Csw.register("logout",
d,!0);Csw.logout=Csw.logout||d;Csw.register("finishLogout",i,!0);Csw.finishLogout=Csw.finishLogout||i;Csw.register("isAdministrator",f);Csw.isAdministrator=Csw.isAdministrator||f})();(function(){function e(){function e(){$.CswCookie("set",CswCookieName.LastViewId,$.CswCookie("get",CswCookieName.CurrentViewId));$.CswCookie("set",CswCookieName.LastViewMode,$.CswCookie("get",CswCookieName.CurrentViewMode));$.CswCookie("set",CswCookieName.LastActionName,$.CswCookie("get",CswCookieName.CurrentActionName));$.CswCookie("set",CswCookieName.LastActionUrl,$.CswCookie("get",CswCookieName.CurrentActionUrl));$.CswCookie("set",CswCookieName.LastReportId,$.CswCookie("get",CswCookieName.CurrentReportId));
$.CswCookie("clear",CswCookieName.CurrentViewId);$.CswCookie("clear",CswCookieName.CurrentViewMode);$.CswCookie("clear",CswCookieName.CurrentActionName);$.CswCookie("clear",CswCookieName.CurrentActionUrl);$.CswCookie("clear",CswCookieName.CurrentReportId);return!0}return{clearCurrent:e,setCurrentView:function(c,b){e();!1===Csw.isNullOrEmpty(c)&&!1===Csw.isNullOrEmpty(b)&&($.CswCookie("set",CswCookieName.CurrentViewId,c),$.CswCookie("set",CswCookieName.CurrentViewMode,b));return!0},setCurrentAction:function(c,
b){e();$.CswCookie("set",CswCookieName.CurrentActionName,c);$.CswCookie("set",CswCookieName.CurrentActionUrl,b);return!0},setCurrentReport:function(c){e();$.CswCookie("set",CswCookieName.CurrentReportId,c);return!0},getCurrent:function(){return{viewid:$.CswCookie("get",CswCookieName.CurrentViewId),viewmode:$.CswCookie("get",CswCookieName.CurrentViewMode),actionname:$.CswCookie("get",CswCookieName.CurrentActionName),actionurl:$.CswCookie("get",CswCookieName.CurrentActionUrl),reportid:$.CswCookie("get",
CswCookieName.CurrentReportId)}},getLast:function(){return{viewid:$.CswCookie("get",CswCookieName.LastViewId),viewmode:$.CswCookie("get",CswCookieName.LastViewMode),actionname:$.CswCookie("get",CswCookieName.LastActionName),actionurl:$.CswCookie("get",CswCookieName.LastActionUrl),reportid:$.CswCookie("get",CswCookieName.LastReportId)}},setCurrent:function(c){e();$.CswCookie("set",CswCookieName.CurrentViewId,c.viewid);$.CswCookie("set",CswCookieName.CurrentViewMode,c.viewmode);$.CswCookie("set",CswCookieName.CurrentActionName,
c.actionname);$.CswCookie("set",CswCookieName.CurrentActionUrl,c.actionurl);$.CswCookie("set",CswCookieName.CurrentReportId,c.reportid);return!0}}}Csw.register("clientState",e);Csw.clientState=Csw.clientState||e;Csw.clientState.clearCurrent=Csw.clientState.clearCurrent||e.clearCurrent;Csw.clientState.setCurrentView=Csw.clientState.setCurrentView||e.setCurrentView;Csw.clientState.setCurrentAction=Csw.clientState.setCurrentAction||e.setCurrentAction;Csw.clientState.setCurrentReport=Csw.clientState.setCurrentReport||
e.setCurrentReport;Csw.clientState.getCurrent=Csw.clientState.getCurrent||e.getCurrent;Csw.clientState.getLast=Csw.clientState.getLast||e.getLast;Csw.clientState.setCurrent=Csw.clientState.setCurrent||e.setCurrent})();(function(){function e(a){a=a.replace(/M/g,"m");a=a.replace(/mmm/g,"M");return a=a.replace(/yyyy/g,"yy")}function g(a){return a}function c(a){return a instanceof Date}function b(a){var b=!0,f=/^(\d?\d):(\d\d)\s?([APap][Mm])?$/g.exec(a);if(null===f)b=!1;else if(a=parseInt(f[1]),f=parseInt(f[2]),0>a||24<=a||0>f||60<=f)b=!1;return b}var a=new Date("1/1/0001 12:00:00 AM");Csw.register("dateTimeMinValue",a);Csw.dateTimeMinValue=Csw.dateTimeMinValue||a;Csw.register("serverDateFormatToJQuery",e);Csw.serverDateFormatToJQuery=
Csw.serverDateFormatToJQuery||e;Csw.register("serverTimeFormatToJQuery",g);Csw.serverTimeFormatToJQuery=Csw.serverTimeFormatToJQuery||g;Csw.register("isDate",c);Csw.isDate=Csw.isDate||c;Csw.register("validateTime",b);Csw.validateTime=Csw.validateTime||b})();(function(){function e(a,d){var b="";Csw.bool(d)&&(b=console.trace());try{!1===Csw.isNullOrEmpty(b)?console.log(a,b):console.log(a)}catch(c){alert(a),!1===Csw.isNullOrEmpty(b)&&alert(b)}}function g(a){var d="",b;for(b in a)d=d+b+"="+a[b]+"<br><br>";a=window.open("","popup");null!==a?a.document.write(d):console.log("iterate() error: No popup!")}function c(a){var d=void 0;if(Csw.hasWebStorage())1===arguments.length&&(localStorage.doLogging=Csw.bool(a)),d=Csw.bool(localStorage.doLogging);return d}function b(a){var d=
void 0;if(Csw.hasWebStorage())1===arguments.length&&(localStorage.debugOn=Csw.bool(a)),d=Csw.bool(localStorage.debugOn);return d}function a(){hasWebStorage()&&window.sessionStorage.clear()}Csw.register("log",e);Csw.log=Csw.log||e;Csw.register("iterate",g);Csw.iterate=Csw.iterate||g;Csw.register("doLogging",c);Csw.doLogging=Csw.doLogging||c;Csw.register("debugOn",b);Csw.debugOn=Csw.debugOn||b;var d=function(a){if(c()&&hasWebStorage()){void 0!==a.setEnded&&a.setEnded();var d=CswClientDb(),b=d.getItem("debuglog"),
b=b+a.toHtml();d.setItem("debuglog",b)}};Csw.register("cacheLogInfo",d);Csw.cacheLogInfo=Csw.cacheLogInfo||d;Csw.register("purgeLogInfo",a);Csw.purgeLogInfo=Csw.purgeLogInfo||a})();(function(){function e(b,a,d){return{type:Csw.string(b,Csw.enums.ErrorType.warning.name),message:Csw.string(a),detail:Csw.string(d)}}function g(b){var a={type:"",message:"",detail:"",display:!0};b&&$.extend(a,b);b=$("#DialogErrorDiv");0>=b.length&&(b=$("#ErrorDiv"));0<b.length&&Csw.bool(a.display)?b.CswErrorMessage({type:a.type,message:a.message,detail:a.detail}):log(a.message+"; "+a.detail);return!0}function c(b,a,d,c){Csw.hasWebStorage()&&d&&log(localStorage);c?$.CswDialog("ErrorDialog",b):log("Error: "+
b.message+" (Code "+b.code+")",a)}Csw.register("makeErrorObj",e);Csw.makeErrorObj=Csw.makeErrorObj||e;Csw.register("error",g);Csw.error=Csw.error||g;Csw.register("errorHandler",c);Csw.errorHandler=Csw.errorHandler||c})();(function(){function e(c,b){return function(){c(b)}}function g(c,b){return function(a){c(a,b)}}Csw.register("makeDelegate",e);Csw.makeDelegate=Csw.makeDelegate||e;Csw.register("makeEventDelegate",g);Csw.makeEventDelegate=Csw.makeEventDelegate||g})();(function(){function e(c){g(c)&&c.apply(this,Array.prototype.slice.call(arguments,1))}function g(c){return $.isFunction(c)}Csw.register("tryExec",e);Csw.tryExec=Csw.tryExec||e;Csw.register("isFunction",g);Csw.isFunction=Csw.isFunction||g})();(function(){function e(){clearCurrent();window.location=homeUrl}function g(c){var b={$ul:"",itemKey:"",itemJson:"",onLogout:null,onAlterNode:null,onSearch:{onViewSearch:null,onGenericSearch:null},onMultiEdit:null,onEditView:null,onSaveView:null,onQuotas:null,onSessions:null,Multi:!1,NodeCheckTreeId:""};c&&$.extend(b,c);var a,d=b.itemJson;a=Csw.string(d.href);var i=Csw.string(b.itemKey),f=Csw.string(d.popup),c=Csw.string(d.action);if(!1===Csw.isNullOrEmpty(a))a=$('<li><a href="'+a+'">'+i+"</a></li>").appendTo(b.$ul);
else if(!1==Csw.isNullOrEmpty(f))a=$('<li class="headermenu_dialog"><a href="'+f+'" target="_blank">'+i+"</a></li>").appendTo(b.$ul);else if(!1===Csw.isNullOrEmpty(c)){a=$('<li><a href="#">'+i+"</a></li>").appendTo(b.$ul);var f=a.children("a"),g=Csw.string(d.nodeid),k=Csw.string(d.nodename),j=Csw.string(d.viewid);switch(c){case "About":f.click(function(){$.CswDialog("AboutDialog");return!1});break;case "AddNode":f.click(function(){$.CswDialog("AddNodeDialog",{text:i,nodetypeid:Csw.string(d.nodetypeid),
relatednodeid:Csw.string(d.relatednodeid),relatednodetypeid:Csw.string(d.relatednodetypeid),onAddNode:b.onAlterNode});return!1});break;case "DeleteNode":f.click(function(){$.CswDialog("DeleteNodeDialog",{nodenames:[k],nodeids:[g],onDeleteNode:b.onAlterNode,NodeCheckTreeId:b.NodeCheckTreeId,Multi:b.Multi});return!1});break;case "editview":f.click(function(){b.onEditView(j);return!1});break;case "CopyNode":f.click(function(){$.CswDialog("CopyNodeDialog",{nodename:k,nodeid:g,onCopyNode:b.onAlterNode});
return!1});break;case "PrintView":f.click(b.onPrintView);break;case "PrintLabel":f.click(function(){$.CswDialog("PrintLabelDialog",{nodeid:g,propid:Csw.string(d.propid)});return!1});break;case "Logout":f.click(function(){b.onLogout();return!1});break;case "Home":f.click(function(){e();return!1});break;case "Profile":f.click(function(){$.CswDialog("EditNodeDialog",{nodeids:[d.userid],filterToPropId:"",title:"User Profile",onEditNode:null});return!1});break;case "ViewSearch":f.click(function(){Csw.tryExec(b.onSearch.onViewSearch)});
break;case "GenericSearch":f.click(function(){Csw.tryExec(b.onSearch.onGenericSearch)});break;case "multiedit":f.click(b.onMultiEdit);break;case "SaveViewAs":f.click(function(){$.CswDialog("AddViewDialog",{viewid:j,viewmode:Csw.string(d.viewmode),onAddView:b.onSaveView});return!1});break;case "Quotas":f.click(b.onQuotas);break;case "Sessions":f.click(b.onSessions)}}else a=$("<li>"+i+"</li>").appendTo(b.$ul);return a}Csw.register("goHome",e);Csw.goHome=Csw.goHome||e;Csw.register("handleMenuItem",g);
Csw.handleMenuItem=Csw.handleMenuItem||g})();(function(){function e(a,c){var f=0,e=+a;!1===isNaN(e)&&e!==b?f=e:(e=+c,!1===isNaN(e)&&e!==b&&(f=e));return f}function g(a){return"number"===typeof a}function c(a,b){var c,e="";if(0<a&&a<=(b||6)){e+=".";for(c=0;c<a;c+=1)e+="9"}return e}var b=-2147483648;Csw.register("int32MinVal",b);Csw.int32MinVal=Csw.int32MinVal||b;Csw.register("number",e);Csw.number=Csw.number||e;Csw.register("isNumber",g);Csw.isNumber=Csw.isNumber||g;var a=function(a){var b=!1;g(a)&&!1===Csw.isNullOrEmpty(a)&&!1===isNaN(+a)&&
(b=!0);return b};Csw.register("isNumeric",a);Csw.isNumeric=Csw.isNumeric||a;Csw.register("getMaxValueForPrecision",c);Csw.getMaxValueForPrecision=Csw.getMaxValueForPrecision||c})();(function(){function e(a){return $.isPlainObject(a)}function g(a){return a instanceof jQuery}function c(a){return Csw.isArray(a)||g(a)}function b(a){return!1===Csw.isFunction(a)&&!1===c(a)&&!1===e(a)}function a(a,d){var e=i(a);!1===e&&b(a)?e=""===Csw.trim(a)||Csw.isDate(a)&&a===Csw.dateTimeMinValue||Csw.isNumber(a)&&a===Csw.int32MinVal:d&&c(a)&&(e=0===a.length);return e}function d(a,d){return Csw.contains(a,d)&&Csw.bool(d[a])}function i(a){var d=!1;!1===Csw.isFunction(a)&&(d=null===a||void 0===a||
$.isPlainObject(a)&&$.isEmptyObject(a));return d}function f(a,d,b){var c="";!1===Csw.isNullOrEmpty(b)&&(c=b);Csw.contains(a,d)&&!1===Csw.isNullOrUndefined(a[d])&&(c=a[d]);return c}function h(a,d){var b=!1;!1===Csw.isNullOrUndefined(a)&&(Csw.isArray(a)&&(b=-1!==Csw.cswIndexOf(a,d)),!1===b&&a.hasOwnProperty(d)&&(b=!0));return b}function k(a,d,b){!1===Csw.isNullOrUndefined(a)&&h(a,d)&&(a[b]=a[d],delete a[d]);return a}function j(a,d,b){var c=!1;!1===Csw.isNullOrEmpty(a)&&h(a,d)&&a[d]===b&&(c=!0);return c}
function n(a){var d=null,b=a;this.find=function(c,e){var f=!1;j(a,c,e)&&(b=d=f=a);!1===f&&l(a,function(a,g,i){g=!1;j(a,c,e)&&(d=f=a,b=i,g=!0);return g},!0);return f};this.remove=function(b,c){return l(a,function(a,e,f){var g=!1;j(a,b,c)&&(g=!0,delete f[e],d=null);return g},!0)};this.obj=a;this.parentObj=b;this.currentKey=this.currentObj=d}function m(a,d){var b=!1;if(Csw.isFunction(d))if(Csw.isArray(a)||Csw.isPlainObject(a)&&!1===h(a,"length"))$.each(a,function(c,e){b=d(a[c],c,a,e);return!b});else if(Csw.isPlainObject(a))for(var c in a)if(h(a,
c)&&(b=d(a[c],c,a)))break;return b}function l(a,d,b){var c=!1;return c=m(a,function(a,e,f,g){!1===c&&(c=Csw.bool(d(a,e,f,g)));!1===c&&b&&(c=Csw.bool(l(a,d,b)));return c})}Csw.register("isPlainObject",e);Csw.isPlainObject=Csw.isPlainObject||e;Csw.register("isJQuery",g);Csw.isJQuery=Csw.isJQuery||g;Csw.register("hasLength",c);Csw.hasLength=Csw.hasLength||c;Csw.register("isGeneric",b);Csw.isGeneric=Csw.isGeneric||b;Csw.register("isNullOrEmpty",a);Csw.isNullOrEmpty=Csw.isNullOrEmpty||a;Csw.register("isInstanceOf",
d);Csw.isInstanceOf=Csw.isInstanceOf||d;Csw.register("isNullOrUndefined",i);Csw.isNullOrUndefined=Csw.isNullOrUndefined||i;Csw.register("tryParseObjByIdx",f);Csw.tryParseObjByIdx=Csw.tryParseObjByIdx||f;Csw.register("contains",h);Csw.contains=Csw.contains||h;Csw.register("renameProperty",k);Csw.renameProperty=Csw.renameProperty||k;Csw.register("foundMatch",j);Csw.foundMatch=Csw.foundMatch||j;Csw.register("ObjectHelper",n);Csw.ObjectHelper=Csw.ObjectHelper||n;Csw.register("each",m);Csw.each=Csw.each||
m;Csw.register("crawlObject",l);Csw.crawlObject=Csw.crawlObject||l})();(function(){function e(a,b){var c=function(){var c="";!1===Csw.isPlainObject(a)&&!1===Csw.isFunction(a)&&!1===Csw.isNullOrEmpty(a)?c=a.toString():!1===Csw.isPlainObject(b)&&!1===Csw.isFunction(b)&&!1===Csw.isNullOrEmpty(b)&&(c=b.toString());return c}();c.val=function(){return a};c.trim=function(){return a=$.trim(a)};c.contains=function(b){return-1!==a.indexOf(b)};return c}function g(a){return"string"===typeof a||Csw.isInstanceOf("string",a)}function c(a){return $.trim(a)}function b(a,b){return a.substr(0,
b.length)===b}function a(a,b){var c=!1;!1===Csw.isNullOrEmpty(b)&&"H:mm:ss"===b&&(c=!0);var e=a.getHours(),g=a.getMinutes(),j=a.getSeconds();10>g&&(g="0"+g);10>j&&(j="0"+j);c?c=e+":"+g+":"+j:(c=e%12+":"+g+":"+j+" ",c=11<e?c+"PM":c+"AM");return c}String.prototype.trim=String.prototype.trim||function(){return this.replace(/^\s+|\s+$/g,"")};String.prototype.toUpperCaseFirstChar=String.prototype.toUpperCaseFirstChar||function(){return this.substr(0,1).toUpperCase()+this.substr(1)};String.prototype.toLowerCaseFirstChar=
String.prototype.toLowerCaseFirstChar||function(){return this.substr(0,1).toLowerCase()+this.substr(1)};String.prototype.toUpperCaseEachWord=String.prototype.toUpperCaseEachWord||function(a){a=a||" ";return this.split(a).map(function(a){return a.toUpperCaseFirstChar()}).join(a)};String.prototype.toLowerCaseEachWord=String.prototype.toLowerCaseEachWord||function(a){a=a||" ";return this.split(a).map(function(a){return a.toLowerCaseFirstChar()}).join(a)};Csw.register("string",e);Csw.string=Csw.string||
e;Csw.register("isString",g);Csw.isString=Csw.isString||g;Csw.register("trim",c);Csw.trim=window.Csw.trim||c;Csw.register("startsWith",b);Csw.startsWith=Csw.startsWith||b;Csw.register("getTimeString",a);Csw.getTimeString=Csw.getTimeString||a})();(function(){function e(b){var a=b.CswAttrDom("id"),b=b.jstree("get_selected");return{iconurl:b.children("a").children("ins").css("background-image"),id:Csw.string(b.CswAttrDom("id")).substring(a.length),text:Csw.string(b.children("a").first().text()).trim(),$item:b}}function g(b,a,d){var c,e,g;Csw.isAdministrator({Yes:function(){b.CswTable("cell",a,1).append(d);var k=b.CswTable("cell",a,2),j=b.CswAttrDom("id");c=$('<select id="'+j+'_vissel" />').appendTo(k);c.append('<option value="User">User:</option>');
c.append('<option value="Role">Role:</option>');c.append('<option value="Global">Global</option>');e=k.CswNodeSelect("init",{ID:j+"_visrolesel",objectclass:"RoleClass"}).hide();g=k.CswNodeSelect("init",{ID:j+"_visusersel",objectclass:"UserClass"});c.change(function(){var a=c.val();"Role"===a?(e.show(),g.hide()):"User"===a?(e.hide(),g.show()):(e.hide(),g.hide())})}});return{getvisibilityselect:function(){return c},getvisroleselect:function(){return e},getvisuserselect:function(){return g}}}function c(b,
a,d){b=window.open(b,null,"height="+a+", width="+d+", status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes");b.focus();return b}Csw.register("jsTreeGetSelected",e);Csw.jsTreeGetSelected=Csw.jsTreeGetSelected||e;Csw.register("makeViewVisibilitySelect",g);Csw.makeViewVisibilitySelect=Csw.makeViewVisibilitySelect||g;Csw.register("openPopup",c);Csw.openPopup=Csw.openPopup||c})();(function(){function e(a,b,c,e){b={ID:"",prefix:Csw.string(b),suffix:Csw.string(c),Delimiter:Csw.string(e,"_")};Csw.isPlainObject(a)?$.extend(b,a):b.ID=Csw.string(a);a=b.ID;!1===Csw.isNullOrEmpty(b.prefix)&&!1===Csw.isNullOrEmpty(a)&&(a=b.prefix+b.Delimiter+a);!1===Csw.isNullOrEmpty(b.suffix)&&!1===Csw.isNullOrEmpty(a)&&(a+=b.Delimiter+b.suffix);return a}function g(a,b,c,e){b={ID:"",prefix:Csw.string(b),suffix:Csw.string(c),Delimiter:Csw.string(e,"_")};Csw.isPlainObject(a)?$.extend(b,a):b.ID=Csw.string(a);
a=b.ID;c=[/'/gi,/ /gi,/\//g];!1===Csw.isNullOrEmpty(b.prefix)&&!1===Csw.isNullOrEmpty(a)&&(a=b.prefix+b.Delimiter+a);!1===Csw.isNullOrEmpty(b.suffix)&&!1===Csw.isNullOrEmpty(a)&&(a+=b.Delimiter+b.suffix);for(b=0;b<c.length;b++)Csw.contains(c,b)&&!1===Csw.isNullOrEmpty(a)&&(a=a.replace(c[b],""));return a}function c(){return window.Modernizr.localstorage||window.Modernizr.sessionstorage}function b(a,b){var c=$(""),e=Csw.getGlobalProp("document");!1===Csw.isNullOrEmpty(a)&&(c=2===arguments.length&&!1===
Csw.isNullOrEmpty(b)?$("#"+a,b):$("#"+a),0===c.length&&(c=$(e.getElementById(a))),0===c.length&&(c=$(e.getElementsByName(a))));return c}Csw.register("makeId",e);Csw.makeId=Csw.makeId||e;Csw.register("makeSafeId",g);Csw.makeSafeId=Csw.makeSafeId||g;Csw.register("hasWebStorage",c);Csw.hasWebStorage=Csw.hasWebStorage||c;Csw.register("tryParseElement",b);Csw.tryParseElement=Csw.tryParseElement||b})();(function(){var e=function(){var e={},c=[],b=0;return{getItem:function(a){var b=null;a&&e.hasOwnProperty(a)&&(b=e[a]);return b},key:function(a){var b=null;c.hasOwnProperty(a)&&(b=c[a]);return b},setItem:function(a,d){a&&(!1===e.hasOwnProperty(a)&&(c.push(a),b+=1),e[a]=d);return null},length:b,removeItem:function(a){var d=!1;a&&e.hasOwnProperty(a)&&(c.splice(a,1),b-=1,delete e[a],d=!0);return d},clear:function(){e={};c=[];b=0;return!0},hasOwnProperty:function(a){return e.hasOwnProperty(a)}}}();if(!1===
window.Modernizr.localstorage)window.localStorage=e;if(!1===window.Modernizr.sessionstorage)window.sessionStorage=e})();(function(){var e={unknownEnum:"unknown"};Csw.register("constants",e);Csw.constants=Csw.constants||e;e={tryParse:function(e,c,b){var a=ChemSW.constants.unknownEnum;contains(e,c)?a=e[c]:!1===b&&each(e,function(b){contains(e,b)&&Csw.string(b).toLowerCase()===Csw.string(c).toLowerCase()&&(a=b)});return a},EditMode:{Edit:"Edit",AddInPopup:"AddInPopup",EditInPopup:"EditInPopup",Demo:"Demo",PrintReport:"PrintReport",DefaultValue:"DefaultValue",AuditHistoryInPopup:"AuditHistoryInPopup",Preview:"Preview"},
ErrorType:{warning:{name:"warning",cssclass:"CswErrorMessage_Warning"},error:{name:"error",cssclass:"CswErrorMessage_Error"}},Events:{CswNodeDelete:"CswNodeDelete"},CswInspectionDesign_WizardSteps:{step1:{step:1,description:"Select an Inspection Target"},step2:{step:2,description:"Select an Inspection Design"},step3:{step:3,description:"Upload Template"},step4:{step:4,description:"Review Inspection Design"},step5:{step:5,description:"Finish"},stepcount:5},CswScheduledRulesGrid_WizardSteps:{step1:{step:1,
description:"Select a Customer ID"},step2:{step:2,description:"Review the Scheduled Rules"},stepcount:2},CswDialogButtons:{1:"ok",2:"ok/cancel",3:"yes/no"},CswOnObjectClassClick:{reauthenticate:"reauthenticate",home:"home",refresh:"refresh",url:"url"}};Csw.register("enums",e);Csw.enums=Csw.enums||e})();
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
childrelationships:{name:"childrelationships"},properties:{name:"properties"},filters:{name:"filters"},propfilters:{name:"filters"},filtermodes:{name:"filtermodes"}},CswNodeTree_DefaultSelect={root:{name:"root"},firstchild:{name:"firstchild"},none:{name:"none"}};(function(){var e=function(e){var c={nodeid:"",nodekey:"",onSuccess:function(){},onError:function(){}};e&&$.extend(c,e);Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/CopyNode",data:{NodePk:c.nodeid},success:function(b){c.onSuccess(b.NewNodeId,"")},error:c.onError})};Csw.register("copyNode",e);Csw.copyNode=Csw.copyNode||e;e=function(e){var c={nodeids:[],nodekeys:[],onSuccess:function(){},onError:function(){}};e&&$.extend(c,e);if(!isArray(c.nodeids))c.nodeids=[c.nodeids],c.nodekeys=[c.nodekeys];Csw.ajax({url:"/NbtWebApp/wsNBT.asmx/DeleteNodes",
data:{NodePks:c.nodeids,NodeKeys:c.nodekeys},success:function(){c.nodeid=$.CswCookie("clear",CswCookieName.CurrentNodeId);c.cswnbtnodekey=$.CswCookie("clear",CswCookieName.CurrentNodeKey);c.onSuccess("","")},error:c.onError})};Csw.register("deleteNodes",e);Csw.deleteNodes=Csw.deleteNodes||e})();(function(){var e=void 0,g=function(c,b,a){e=$.CswNodePreview("open",{ID:b+"_preview",nodeid:b,cswnbtnodekey:a,eventArg:c})};Csw.register("nodeHoverIn",g);Csw.nodeHoverIn=Csw.nodeHoverIn||g;g=function(){void 0!==e&&(e.CswNodePreview("close"),e=void 0)};Csw.register("nodeHoverOut",g);Csw.nodeHoverOut=Csw.nodeHoverOut||g})();(function(){var e=function(c,b,a){var d=!1;if(!1===Csw.isNullOrEmpty(b))contains(b,"values")&&(d=g(c,b.values,a)),b.wasmodified=b.wasmodified||d};Csw.register("preparePropJsonForSave",e);Csw.preparePropJsonForSave=Csw.preparePropJsonForSave||e;var g=function(c,b,a){if(!1===Csw.isNullOrEmpty(b)){var d=!1;crawlObject(b,function(e,f){if(contains(a,f)){var h=a[f];if(isPlainObject(h))d=g(c,b[f],h)||d;else if(!1===c&&b[f]!==h||c&&!1===isNullOrUndefined(h)&&h!==CswMultiEditDefaultValue)d=!0,b[f]=h}},!1)}return d}})();(function(e){e.CswCookie=function(g){var c={get:function(b){b=e.cookie(b);void 0==b&&(b="");return b},set:function(b,a){e.cookie(b,a)},clear:function(b){e.cookie(b,"")},clearAll:function(){for(var b in CswCookieName)CswCookieName.hasOwnProperty(b)&&e.cookie(CswCookieName[b],null)}};if(c[g])return c[g].apply(this,Array.prototype.slice.call(arguments,1));if("object"===typeof g||!g)return c.init.apply(this,arguments);e.error("Method "+g+" does not exist on CswCookie");return!1}})(jQuery);
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

    function cswIndexOf(object, property) {
        /// <summary>
        ///   Because IE 8 doesn't implement indexOf on the Array prototype.
        /// </summary>
        var ret = -1,
            i = 0,
            len = object.length;
        if (Csw.isFunction(object.indexOf)) {
            ret = object.indexOf(property);
        } else if (Csw.hasLength(object) && len > 0) {
            for ( ; i < len; i++) {
                if (object[i] === property) {
                    ret = i;
                    break;
                }
            }
        }
        return ret;
    };
    Csw.register('cswIndexOf', cswIndexOf);
    Csw.cswIndexOf = Csw.cswIndexOf || cswIndexOf;
    
    function array() {
        var retArray = []; 
        if(arguments.length > 0) {
            retArray = Array.prototype.slice.call(arguments, 0);
        }
        
        retArray.contains = retArray.contains || function (value) {
            return retArray.cswIndexOf(value) != -1;
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
                ret = Csw.cswIndexOf(object, index) !== -1;
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




