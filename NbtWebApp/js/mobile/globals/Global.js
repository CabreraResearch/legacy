
//#region Globals (yuck)
var homeUrl = 'Mobile.html';
//#endregion Globals (yuck)

//#region Session Expiration
var expiretime = '';
var expiretime_interval;
var expired_interval;

function getExpireTime() {
    "use strict"; 
    return expiretime;
}

function setExpireTime(value) {
    "use strict"; 
    expiretime = value;
    setExpireTimeInterval();
}

function setExpireTimeInterval() {
    "use strict"; 
    clearInterval(expiretime_interval);
    clearInterval(expired_interval);
    expiretime_interval = setInterval(function () { checkExpireTime(); }, 60000);
    expired_interval = setInterval(function () { checkExpired(); }, 60000);
}

function checkExpired() {
    "use strict"; 
    var now = new Date();
    if (Date.parse(expiretime) - Date.parse(now) < 0)
    {
        clearInterval(expired_interval);
        Logout();
    }
}

function checkExpireTime() {
    "use strict"; 
    var now = new Date();
    if (Date.parse(expiretime) - Date.parse(now) < 180000)     	// 3 minutes until timeout
    {
        clearInterval(expiretime_interval);
        $.CswDialog('ExpireDialog', {
            'onYes': function ()
            {
                CswAjaxJson({
                    'url': '/NbtWebApp/wsNBT.asmx/RenewSession',
                    'success': function () { }
                });
            }
        });
    }
}
//#endregion Session Expiration

//#region Ajax
var _ajaxCount = 0;
function ajaxInProgress() {
    "use strict"; 
    return (_ajaxCount > 0);
}

// Events for all Ajax requests
var onBeforeAjax = null;  // function (watchGlobal) {}
var onAfterAjax = null;   // function (succeeded) {}

function CswAjaxJson(options) { /// <param name="$" type="jQuery" />
    "use strict"; 
    /// <summary>
    ///   Executes Async webservice request for JSON
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.url: WebService URL
    ///     &#10;2 - options.data: {field1: value, field2: value}
    ///     &#10;3 - options.success: function() {}
    ///     &#10;4 - options.error: function() {}
    /// </param>
    var o = {
        url: '',
        data: {},
        onloginfail: function () { _finishLogout(); },
        success: null, //function () { },
        error: null, //function () { },
        overrideError: false,
        formobile: false,
        async: true,
        watchGlobal: true
    };
    if (options) $.extend(o, options);

    //var starttime = new Date();
    if(o.watchGlobal) {
        _ajaxCount+=1;
    }
    if (isFunction(onBeforeAjax)) onBeforeAjax(o.watchGlobal);

    $.ajax({
        type: 'POST',
        async: o.async,
        url: o.url,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(o.data),
        success: function (data, textStatus, xmlHttpRequest) {
            if(o.watchGlobal) {
                _ajaxCount-=1;
            }
            //var endtime = new Date();
            //$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");
            var result = $.parseJSON(data.d);

            if (result.error !== undefined) {
                if (false === o.overrideError) {
                    _handleAjaxError(xmlHttpRequest, {
                        'display': result.error.display,
                        'type': result.error.type,
                        'message': result.error.message,
                        'detail': result.error.detail
                    }, '');
                }
                if (isFunction(o.error)) {
                    o.error(result.error);
                }
            }
            else {

                var auth = tryParseString(result['AuthenticationStatus'], 'Unknown');
                if (!o.formobile) {
                    setExpireTime(tryParseString(result.timeout, ''));
                }

                delete result['AuthenticationStatus'];
                delete result['timeout'];

                _handleAuthenticationStatus({
                    status: auth,
                    success: function () { if (isFunction(o.success)) { o.success(result); } },
                    failure: o.onloginfail,
                    usernodeid: result.nodeid,
                    usernodekey: result.cswnbtnodekey,
                    passwordpropid: result.passwordpropid,
                    ForMobile: o.formobile
                });
            }
            if (isFunction(onAfterAjax)) onAfterAjax(true);
        }, // success{}
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if(o.watchGlobal) {
                _ajaxCount-=1;
            }
            //_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
            log("Webservice Request (" + o.url + ") Failed: " + textStatus);
            if (isFunction(o.error)) {
                o.error();
            }
            if (isFunction(onAfterAjax)) onAfterAjax(false);
        }
    });                  // $.ajax({
} // CswAjaxJson()

function CswAjaxJsonGet(options) { /// <param name="$" type="jQuery" />
    "use strict"; 
    /// <summary>
    ///   Executes Async webservice request for JSON
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.url: WebService URL
    ///     &#10;2 - options.data: {field1: value, field2: value}
    ///     &#10;3 - options.success: function() {}
    ///     &#10;4 - options.error: function() {}
    /// </param>
    var o = {
        url: '',
        data: {},
        onloginfail: function () { _finishLogout(); },
        success: null, //function () { },
        error: null, //function () { },
        overrideError: false,
        formobile: false,
        async: true,
        watchGlobal: true
    };
    if (options) $.extend(o, options);

    if(o.watchGlobal) {
        _ajaxCount+=1;
    }
    if (isFunction(onBeforeAjax)) onBeforeAjax(o.watchGlobal);

    $.ajax({
        type: 'GET',
        async: o.async,
        url: o.url,
        dataType: 'json',
        data: JSON.stringify(o.data),
        success: function (result, textStatus, xmlHttpRequest) {
            if(o.watchGlobal) {
                _ajaxCount-=1;
            }
            
            if (false === isNullOrEmpty(result.error)) {
                if (false === o.overrideError) {
                    _handleAjaxError(xmlHttpRequest, {
                        'display': result.error.display,
                        'type': result.error.type,
                        'message': result.error.message,
                        'detail': result.error.detail
                    }, '');
                }
                if (isFunction(o.error)) {
                    o.error(result.error);
                }
            } else {
                if (isFunction(o.success)) {
                     o.success(result);
                }
            }
            if (isFunction(onAfterAjax)) onAfterAjax(true);
        }, // success{}
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            if(o.watchGlobal) {
                _ajaxCount-=1;
            }
            //_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
            log("Webservice Request (" + o.url + ") Failed: " + textStatus);
            if (isFunction(o.error)) {
                o.error();
            }
            if (isFunction(onAfterAjax)) onAfterAjax(false);
        }
    }); // $.ajax({
} // CswAjaxJsonGet()

function CswAjaxXml(options) {
    "use strict"; 
    /// <summary>
    ///   Executes Async webservice request for XML
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.url: WebService URL
    ///     &#10;2 - options.data: {field1: value, field2: value}
    ///     &#10;3 - options.success: function() {}
    ///     &#10;4 - options.error: function() {}
    ///     &#10;5 - options.formobile: false
    /// </param>

    var o = {
        url: '',
        data: {},
        stringify: false, //in case we need to conditionally apply $.param() instead of JSON.stringify() (or both)
        onloginfail: function () { _finishLogout(); },
        success: function () { },
        error: function () { },
        formobile: false,
        async: true,
        watchGlobal: true
    };

    if (options) $.extend(o, options);
    
    if (!isNullOrEmpty(o.url)) {
        if(o.watchGlobal) {
            _ajaxCount+=1;
        }
        $.ajax({
            type: 'POST',
            async: o.async,
            url: o.url,
            dataType: "text",
            //contentType: 'application/json; charset=utf-8',
            data: $.param(o.data),     // should be 'field1=value&field2=value'
            success: function (data, textStatus, xmlHttpRequest)
            {
                if(o.watchGlobal) {
                    _ajaxCount-=1;
                }
                //var endtime = new Date();
                //$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

                var $realxml;
                if ($.browser.msie)
                {
                    // We have to use third-party jquery.xml.js for Internet Explorer to handle non-DOM XML content
                    $realxml = $.xml(data);
                }
                else
                {
                    $realxml = $(xmlHttpRequest.responseXML).children().first();
                }

                if ($realxml.first().get(0).nodeName === "error")
                {
                    _handleAjaxError(xmlHttpRequest, {
                        'display': $realxml.CswAttrNonDom('display'),
                        'type': $realxml.CswAttrNonDom('type'),
                        'message': $realxml.CswAttrNonDom('message'),
                        'detail': $realxml.CswAttrNonDom('detail')
                    }, '');
                    o.error();
                }
                else
                {
                    var auth = tryParseString($realxml.CswAttrNonDom('authenticationstatus'), 'Unknown');
                    if (!o.formobile) {
                        setExpireTime($realxml.CswAttrNonDom('timeout'));
                    }
                    
                    _handleAuthenticationStatus({
                        status: auth,
                        success: function () { o.success($realxml); },
                        failure: o.onloginfail,
                        usernodeid: tryParseString($realxml.CswAttrNonDom('nodeid'), ''),
                        usernodekey: tryParseString($realxml.CswAttrNonDom('cswnbtnodekey'), ''),
                        passwordpropid: tryParseString($realxml.CswAttrNonDom('passwordpropid'), ''),
                        ForMobile: o.formobile
                    });
                }

            }, // success{}
            error: function (xmlHttpRequest, textStatus, errorThrown) {
                if(o.watchGlobal) {
                    _ajaxCount-=1;
                }
                //_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
                log("Webservice Request (" + o.url + ") Failed: " + textStatus);
                o.error();
            }
        });                               // $.ajax({
    } // if(o.url != '')
} // CswAjaxXml()

function _handleAjaxError(xmlHttpRequest, errorJson, errorThrown) {
    "use strict"; 
    CswError(errorJson);
} // _handleAjaxError()

function CswError(errorJson) {
    "use strict"; 
    var e = {
        'type': '',
        'message': '',
        'detail': '',
        'display': true
    };
    if (errorJson) $.extend(e, errorJson);

    var $errorsdiv = $('#DialogErrorDiv');
    if ($errorsdiv.length <= 0) 
        $errorsdiv = $('#ErrorDiv');

    if ($errorsdiv.length > 0 && isTrue(e.display))
    {
        $errorsdiv.CswErrorMessage({ 'type': e.type, 'message': e.message, 'detail': e.detail });
    } 
    else
    {
        log(e.message + '; ' + e.detail);
    }
} // CswError()

function _handleAuthenticationStatus(options) {
    "use strict"; 
    var o = {
        status: '',
        success: function () { },
        failure: function () { },
        usernodeid: '',
        usernodekey: '',
        passwordpropid: '',
        ForMobile: false
    };
    if(options) $.extend(o, options);

    var txt = '';

    switch (o.status)
    {
        case 'Authenticated': o.success(); break;
        //case 'Deauthenticated': o.success(); break;  // yes, o.success() is intentional here.

        case 'Locked': txt = "Your account is locked.  Please see your account administrator."; break;
        case 'Deactivated': txt = "Your account is deactivated.  Please see your account administrator."; break;
        case 'ModuleNotEnabled': txt = "This feature is not enabled.  Please see your account administrator."; break;
        case 'TooManyUsers': txt = "Too many users are currently connected.  Try again later."; break;
        case 'NonExistentSession': txt = "Your session has timed out.  Please login again."; break;
        case 'Unknown': txt = "An Unknown Error Occurred"; break;
        case 'TimedOut': 
            o.success();
            break;
        case 'ExpiredPassword':
            o.success();
            break;
        case 'ShowLicense':
            o.success();
            break;
        default:
            txt = "Invalid login."; 
            break;
    }
    
    if (!isNullOrEmpty(txt) && o.status !== 'Authenticated' ) {
        o.failure(txt,o.status);
    }
} // _handleAuthenticationStatus()

//#endregion Ajax

//#region Dates

function ServerDateFormatToJQuery(serverDateFormat) {
    "use strict"; 
    var ret = serverDateFormat;
    ret = ret.replace(/M/g, 'm');
    ret = ret.replace(/mmm/g, 'M');
    ret = ret.replace(/yyyy/g, 'yy');
    return ret;
}
function ServerTimeFormatToJQuery(serverTimeFormat) {
    "use strict"; 
    var ret = serverTimeFormat;
    return ret;
}

//#endregion Dates


//#region Debug
function iterate(obj) {
    "use strict"; 
    var str = '';
    for (var x in obj)
    {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    if (popup !== null)
        popup.document.write(str);
    else
        console.log("iterate() error: No popup!");
}

function errorHandler(error, includeCallStack, includeLocalStorage, doAlert) {
    "use strict"; 
    if (hasWebStorage() && includeLocalStorage) log(localStorage);
    if( doAlert ) {
        $.CswDialog('ErrorDialog', error);
        //alert('Error: ' + error.message + ' (Code ' + error.code + ')');
    }
    else {
        log('Error: ' + error.message + ' (Code ' + error.code + ')', includeCallStack);
    }
}
//#endregion Debug

//#region Persistent Logging

function doLogging(value) {
    "use strict"; 
    var ret = undefined;
    if (hasWebStorage()) {
        if (arguments.length === 1) {
            localStorage['doLogging'] = isTrue(value);
        }
        ret = isTrue(localStorage['doLogging']);
    }
    return ret;
}

function debugOn(value) { 
    "use strict"; 
    var ret = undefined;
    if (hasWebStorage()) {
        if (arguments.length === 1) {
            localStorage['debugOn'] = isTrue(value);
        }
        ret = isTrue(localStorage['debugOn']);
    }
    return ret;
}

function cacheLogInfo(logger) {
    "use strict"; 
    if (doLogging()) {
        if (hasWebStorage()) {
            if (undefined !== logger.setEnded) logger.setEnded();
            var logStorage = CswClientDb();
            var log = logStorage.getItem('debuglog');
            log += logger.toHtml();

            logStorage.setItem('debuglog', log);
        }
    }
}

function purgeLogInfo() {
    "use strict"; 
    if (hasWebStorage()) {
        window.sessionStorage.clear();
    }
}

//#endregion Persistent Logging

//#region Browser Compatibility

function hasWebStorage() {
    "use strict"; 
    var ret = (Modernizr.localstorage || Modernizr.sessionstorage); 
    return ret;
}

//#endregion Browser Compatibility