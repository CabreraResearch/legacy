/// <reference path="CswEnums.js" />
/// <reference path="CswGlobalTools.js" />
/// <reference path="CswPrototypeExtensions.js" />
/// <reference path="../main/tools/CswArray.js" />
/// <reference path="../main/tools/CswTools.js" />
/// <reference path="../main/tools/CswAttr.js" />
/// <reference path="../main/tools/CswClientDb.js" />
/// <reference path="../main/tools/CswCookie.js" />
/// <reference path="../main/tools/CswProfileMethod.js" />
/// <reference path="../main/tools/CswQueryString.js" />
/// <reference path="../main/tools/CswString.js" />
/// <reference path="../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../Scripts/jquery.cookie.js" />

//#region Globals (yuck)
var homeUrl = 'Main.html';
//#endregion Globals (yuck)

//#region Session Expiration
var expiretime = '';
var expiretime_interval;
var expired_interval;

function getExpireTime()
{
    return expiretime;
}
function setExpireTime(value)
{
    expiretime = value;
    setExpireTimeInterval();
}
function setExpireTimeInterval()
{
    clearInterval(expiretime_interval);
    clearInterval(expired_interval);
    expiretime_interval = setInterval(function () { checkExpireTime(); }, 60000);
    expired_interval = setInterval(function () { checkExpired(); }, 60000);
}
function checkExpired()
{
    var now = new Date();
    if (Date.parse(expiretime) - Date.parse(now) < 0)
    {
        clearInterval(expired_interval);
        Logout();
    }
}
function checkExpireTime()
{
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

//#region Current State
function setCurrentView(viewid, viewmode)
{
    clearCurrent();
    if (false === isNullOrEmpty(viewid) && false === isNullOrEmpty(viewmode)) {
        $.CswCookie('set', CswCookieName.CurrentViewId, viewid);
        $.CswCookie('set', CswCookieName.CurrentViewMode, viewmode);
    }
}

function setCurrentAction(actionname, actionurl)
{
    clearCurrent();
    $.CswCookie('set', CswCookieName.CurrentActionName, actionname);
    $.CswCookie('set', CswCookieName.CurrentActionUrl, actionurl);
}

function setCurrentReport(reportid)
{
    clearCurrent();
    $.CswCookie('set', CswCookieName.CurrentReportId, reportid);
}

function clearCurrent()
{
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
}

function getCurrent() {
    return {
        'viewid': $.CswCookie('get', CswCookieName.CurrentViewId),
        'viewmode': $.CswCookie('get', CswCookieName.CurrentViewMode),
        'actionname': $.CswCookie('get', CswCookieName.CurrentActionName),
        'actionurl': $.CswCookie('get', CswCookieName.CurrentActionUrl),
        'reportid': $.CswCookie('get', CswCookieName.CurrentReportId)
    };
}
function getLast() {
    return {
        'viewid': $.CswCookie('get', CswCookieName.LastViewId),
        'viewmode': $.CswCookie('get', CswCookieName.LastViewMode),
        'actionname': $.CswCookie('get', CswCookieName.LastActionName),
        'actionurl': $.CswCookie('get', CswCookieName.LastActionUrl),
        'reportid': $.CswCookie('get', CswCookieName.LastReportId)
    };
}
function setCurrent(json) {
    clearCurrent();
    $.CswCookie('set', CswCookieName.CurrentViewId, json.viewid);
    $.CswCookie('set', CswCookieName.CurrentViewMode, json.viewmode);
    $.CswCookie('set', CswCookieName.CurrentActionName, json.actionname);
    $.CswCookie('set', CswCookieName.CurrentActionUrl, json.actionurl);
    $.CswCookie('set', CswCookieName.CurrentReportId, json.reportid);
}

//#endregion Current State

//#region Ajax
var _ajaxCount = 0;
function ajaxInProgress()
{
    return (_ajaxCount > 0);
}

function CswAjaxJson(options)
{ /// <param name="$" type="jQuery" />
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
        formobile: false,
        async: true
    };

    if (options) $.extend(o, options);
    //var starttime = new Date();
    _ajaxCount++;
    $.ajax({
        type: 'POST',
        async: o.async,
        url: o.url,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(o.data),
        success: function (data, textStatus, XMLHttpRequest)
        {
            _ajaxCount--;
            //var endtime = new Date();
            //$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");
            var result = $.parseJSON(data.d);

            if (result.error !== undefined)
            {
                _handleAjaxError(XMLHttpRequest, {
                    'display': result.error.display,
                    'type': result.error.type,
                    'message': result.error.message,
                    'detail': result.error.detail
                }, '');
                if (isFunction(o.error)) {
                    o.error();
                }
            }
            else
            {

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
        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown)
        {
            _ajaxCount--;
            //_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
            log("Webservice Request (" + o.url + ") Failed: " + textStatus);
            if (isFunction(o.error)) {
                o.error();
            }
        }
    });                 // $.ajax({
} // CswAjaxJson()

function CswAjaxXml(options)
{
    /// <summary>
    ///   Executes Async webservice request for XML
    /// </summary>
//	/// <param name="options" type="Object">
//	///     A JSON Object
//	///     &#10;1 - options.url: WebService URL
//	///     &#10;2 - options.data: {field1: value, field2: value}
//	///     &#10;3 - options.success: function() {}
//	///     &#10;4 - options.error: function() {}
//	///     &#10;5 - options.formobile: false
//	/// </param>

    var o = {
        url: '',
        data: {},
        stringify: false, //in case we need to conditionally apply $.param() instead of JSON.stringify() (or both)
        onloginfail: function () { _finishLogout(); },
        success: function () { },
        error: function () { },
        formobile: false,
        async: true
    };

    if (options) $.extend(o, options);
    
    if (!isNullOrEmpty(o.url))
    {
        _ajaxCount++;
        $.ajax({
            type: 'POST',
            async: o.async,
            url: o.url,
            dataType: "text",
            //contentType: 'application/json; charset=utf-8',
            data: $.param(o.data),     // should be 'field1=value&field2=value'
            success: function (data, textStatus, XMLHttpRequest)
            {
                _ajaxCount--;
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
                    $realxml = $(XMLHttpRequest.responseXML).children().first();
                }

                if ($realxml.first().get(0).nodeName === "error")
                {
                    _handleAjaxError(XMLHttpRequest, {
                        'display': $realxml.CswAttrXml('display'),
                        'type': $realxml.CswAttrXml('type'),
                        'message': $realxml.CswAttrXml('message'),
                        'detail': $realxml.CswAttrXml('detail')
                    }, '');
                    o.error();
                }
                else
                {
                    var auth = tryParseString($realxml.CswAttrXml('authenticationstatus'), 'Unknown');
                    if (!o.formobile) {
                        setExpireTime($realxml.CswAttrXml('timeout'));
                    }
                    
                    _handleAuthenticationStatus({
                        status: auth,
                        success: function () { o.success($realxml) },
                        failure: o.onloginfail,
                        usernodeid: tryParseString($realxml.CswAttrXml('nodeid'), ''),
                        usernodekey: tryParseString($realxml.CswAttrXml('cswnbtnodekey'), ''),
                        passwordpropid: tryParseString($realxml.CswAttrXml('passwordpropid'), ''),
                        ForMobile: o.formobile
                    });
                }

            }, // success{}
            error: function (XMLHttpRequest, textStatus, errorThrown)
            {
                _ajaxCount--;
                //_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
                log("Webservice Request (" + o.url + ") Failed: " + textStatus);
                o.error();
            }
        });                               // $.ajax({
    } // if(o.url != '')
} // CswAjaxXml()

function _handleAjaxError(XMLHttpRequest, errorJson, errorThrown)
{
    CswError(errorJson);
} // _handleAjaxError()

function CswError(errorJson)
{
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

function _handleAuthenticationStatus(options)
{
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
    var GoodEnoughForMobile = false; //Ignore password expirery and license accept for Mobile for now
    switch (o.status)
    {
        case 'Authenticated': o.success(); break;
        case 'Deauthenticated': o.success(); break;  // yes, o.success() is intentional here.
        case 'Failed': txt = "Invalid login."; break;
        case 'Locked': txt = "Your account is locked.  Please see your account administrator."; break;
        case 'Deactivated': txt = "Your account is deactivated.  Please see your account administrator."; break;
        case 'ModuleNotEnabled': txt = "This feature is not enabled.  Please see your account administrator."; break;
        case 'TooManyUsers': txt = "Too many users are currently connected.  Try again later."; break;
        case 'NonExistentAccessId': txt = "Invalid login."; break;
        case 'NonExistentSession': txt = "Your session has timed out.  Please login again."; break;
        case 'Unknown': txt = "An Unknown Error Occurred"; break;
        case 'TimedOut': 
            GoodEnoughForMobile = true;
            txt = "Your session has timed out.  Please login again."; 
            break;
        case 'ExpiredPassword':
            GoodEnoughForMobile = true;
            if( !o.ForMobile ) {
                $.CswDialog('EditNodeDialog', {
                    nodeids: [ o.usernodeid ],
                    nodekeys: [ o.usernodekey ],
                    filterToPropId: o.passwordpropid,
                    title: 'Your password has expired.  Please change it now:',
                    onEditNode: function () { o.success(); }
                });
            }
            break;
        case 'ShowLicense':
            GoodEnoughForMobile = true;
            if( !o.ForMobile ) {
                $.CswDialog('ShowLicenseDialog', {
                    'onAccept': function () { o.success(); },
                    'onDecline': function () { o.failure('You must accept the license agreement to use this application'); }
                });
            }
            break;
    }

    if( o.ForMobile &&   
        ( o.status !== 'Authenticated' && GoodEnoughForMobile ) ) {
        o.success();
    }
    else if (!isNullOrEmpty(txt) && o.status !== 'Authenticated' )
    {
        o.failure(txt,o.status);
    }
} // _handleAuthenticationStatus()

//#endregion Ajax

function Logout(options)
{
    var o = {
        DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
        onDeauthenticate: function () { }
    };

    if (options)
    {
        $.extend(o, options);
    }

    CswAjaxJson({
        url: o.DeauthenticateUrl,
        data: {},
        success: function (data)
        {
            _finishLogout();
            o.onDeauthenticate();
        } // success{}
    });
} // logout

function _finishLogout()
{
    $.CswCookie('clearAll');
    window.location = homeUrl;
}


function jsonToString(j)
{
    return JSON.stringify(j);
} // jsonToString

//#endregion Ajax

//#region Check Changes
var changed = new Number(0);
var checkChangesEnabled = true;

function setChanged()
{
    if (checkChangesEnabled)
    {
        changed = 1;
        //        var statusimage = getMainStatusImage();
        //var savebutton = $('#SaveTab');
        //        if (statusimage != null) {
        //            statusimage.style.backgroundPosition = "0px -210px";
        //            statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -210px"; }
        //            statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -210px"; }
        //            statusimage.title = "There are unsaved changes";
        //        } 
        //		if (savebutton != null)
        //		{
        //			savebutton.value = "Save Changes";
        //			savebutton.disabled = false;
        //		}
    }
}

function unsetChanged()
{
    if (checkChangesEnabled)
    {
        //        var statusimage = getMainStatusImage();
        //        if(statusimage != null)
        //            statusimage.style.backgroundPosition = "0px -195px";
        //        statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -195px"; }
        //        statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -195px"; }
        //        statusimage.title = "There are no changes";
        //		var savebutton = $('#SaveTab');
        //		if (savebutton != null)
        //		{
        //			if (changed != 0)
        //				savebutton.value = "Changes Saved";
        //			savebutton.disabled = true;
        //		}
        changed = 0;
    }
}

function checkChanges()
{
    if (checkChangesEnabled && changed === 1)
    {
        return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
    }
}

function manuallyCheckChanges()
{
    var ret = true;
    if (checkChangesEnabled && changed === 1)
    {
        ret = confirm('Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page.');

        // this serves several purposes:
        // 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
        // 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
        if (ret)
        {
            changed = 0;
        }
    }
    return ret;
}

function initCheckChanges()
{
    // Assign the checkchanges event to happen onbeforeunload
    if (!isNullOrEmpty(window.onbeforeunload))
    {
        window.onbeforeunload = function ()
        {
            var f = window.onbeforeunload;
            var ret = f();
            if (ret)
            {
                return checkChanges();
            } else
            {
                return false;
            }
        };
    } else
    {
        window.onbeforeunload = function ()
        {
            return checkChanges();
        };
    }

    //	// IE6 has this annoying habit of throwing unspecified errors if we prevent
    //	// the navigation with onbeforeunload after clicking a button.
    //	// So we're going to trap this error and prevent it from being shown.
    //	window.onerror = function (strError, uri, line)
    //	{
    //		if (strError.toLowerCase().indexOf('unspecified error') >= 0)
    //		{
    //			window.event.returnValue = true;
    //		} else
    //		{
    //			window.event.returnValue = false;
    //		}
    //	}
}

if (CswAppMode.mode === 'full')
{
    if (!isNullOrEmpty(window.onload))
    {
        window.onload = new Function('initCheckChanges(); var f=' + window.onload + '; return f();');
    } else
    {
        window.onload = function() { initCheckChanges(); };
    }
}
//#endregion Check Changes

//#region User permissions
function IsAdministrator(options)
{
    var o = {
        'Yes': function () { },
        'No': function () { }
    };
    if (options) $.extend(o, options);

    CswAjaxJson({
        url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
        success: function (data)
            {
                if (data.Administrator === "true")
                {
                    o.Yes();
                } else
                {
                    o.No();
                }
            }
    });
} // IsAdministrator()
//#endregion User permissions

//#region Node interactions
function copyNode(options)
{
    var o = {
        'nodeid': '',
        'nodekey': '',
        'onSuccess': function (nodeid, nodekey) { },
        'onError': function () { }
    };
    if (options)
    {
        $.extend(o, options);
    }

    var dataJson = {
        NodePk: o.nodeid
    };

    CswAjaxJson({
        url: '/NbtWebApp/wsNBT.asmx/CopyNode',
        data: dataJson,
        success: function (result)
        {
            o.onSuccess(result.NewNodeId, '');
        },
        error: o.onError
    });
}

function deleteNodes(options)
{ /// <param name="$" type="jQuery" />
    var o = {
        'nodeids': [],
        'nodekeys': [],
        'onSuccess': function (nodeid, nodekey) { },
        'onError': function () { }
    };
    if (options) $.extend(o, options);

    if (!isArray(o.nodeids))  // case 22722
    {
        o.nodeids = [o.nodeids];
        o.nodekeys = [o.nodekeys];
    }

    var jData = {
        NodePks: o.nodeids,
        NodeKeys: o.nodekeys
    };

    CswAjaxJson({
        url: '/NbtWebApp/wsNBT.asmx/DeleteNodes',
        data: jData,
        success: function (result)
        {
            // clear selected node cookies
            o.nodeid = $.CswCookie('clear', CswCookieName.CurrentNodeId);
            o.cswnbtnodekey = $.CswCookie('clear', CswCookieName.CurrentNodeKey);
            // returning '' will reselect the first node in the tree
            o.onSuccess('', '');
        },
        error: o.onError
    });
}

//#endregion Node interactions

//#region Node Preview

var $nodepreview = undefined;
function nodeHoverIn(event, nodeid, cswnbtnodekey) {
    $nodepreview = $.CswNodePreview('open', {
        ID: nodeid + "_preview",
        nodeid: nodeid,
        cswnbtnodekey: cswnbtnodekey,
        eventArg: event
    });
}

function nodeHoverOut() {
    if ($nodepreview !== undefined)
    {
        $nodepreview.CswNodePreview('close');
        $nodepreview = undefined;
    }
}

//#endregion Node Preview

//#region Node Props

function preparePropJsonForSave(isMulti, propData, attributes) {
    ///<summary>Takes property JSON from the form and modifies it in order to send back to the server.</summary>
    ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
    ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
    ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
    ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
    var wasModified = false;
    if(false === isNullOrEmpty(propData)) {
        if (contains(propData, 'values')) {
            var propVals = propData.values;
            wasModified = preparePropJsonForSaveRecursive(isMulti, propVals, attributes);
        }
        propData.wasmodified = wasModified;
    }
}

function preparePropJsonForSaveRecursive(isMulti, propVals, attributes) {
    ///<summary>Recurses over the subfields and sub-subfields of a property to update its JSON.</summary>
    ///<param name="isMulti" type="Boolean">True if this is Multi-Edit</param>
    ///<param name="propVals" type="Object">Likely an o.propData.values object. This contains the cached prop JSON.</param>
    ///<param name="attributes" type="Object">An object which mirrors the structure of propVals. This contains the new prop JSON derived from the form.</param>
    ///<returns type="Void">No return, but the JSON is updated. propVals.wasmodified is set according to whether the subfield values changed.</returns>
    if (false === isNullOrEmpty(propVals)) {
        var wasModified = false;
        crawlObject(propVals, function(prop, key, par) {
            if (contains(attributes, key)) {
                var attr = attributes[key];
                //don't bother sending this to server unless it's changed
                if (isPlainObject(attr)) {
                    wasModified = preparePropJsonForSaveRecursive(isMulti, propVals[key], attr);
                }
                else if ((false === isMulti && propVals[key] !== attr) ||
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
}

//#endregion Node Props

//#region jsTree
function jsTreeGetSelected($treediv)
{ /// <param name="$" type="jQuery" />
    var IdPrefix = $treediv.CswAttrDom('id');
    var $SelectedItem = $treediv.jstree('get_selected');
    var ret = {
        'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
        'id': $SelectedItem.CswAttrDom('id').substring(IdPrefix.length),
        'text': $SelectedItem.children('a').first().text().trim(),
        '$item': $SelectedItem
    };
    return ret;
}
//#endregion jsTree

//#region Menu
function GoHome()
{ /// <param name="$" type="jQuery" />
    clearCurrent();
    window.location = homeUrl;
}

function HandleMenuItem(options)
{ /// <param name="$" type="jQuery" />
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
        Multi: false,
        NodeCheckTreeId: ''
    };
    if (options)
    {
        $.extend(o, options);
    }
    var $li;
    var json = o.itemJson;
    var href = tryParseString(json.href);
    var text = tryParseString(o.itemKey);
    var popup = tryParseString(json.popup);
    var action = tryParseString(json.action);
    
    if (!isNullOrEmpty(href))
    {
        $li = $('<li><a href="' + href + '">' + text + '</a></li>')
            .appendTo(o.$ul);
    }
    else if (!isNullOrEmpty(popup))
    {
        $li = $('<li class="headermenu_dialog"><a href="' + popup + '" target="_blank">' + text + '</a></li>')
                        .appendTo(o.$ul);
    }
    else if (!isNullOrEmpty(action))
    {
        $li = $('<li><a href="#">' + text + '</a></li>')
                        .appendTo(o.$ul);
        var $a = $li.children('a');
        var nodeid = tryParseString(json.nodeid);
        var nodename = tryParseString(json.nodename);
        var viewid = tryParseString(json.viewid);

        switch (action)
        {
            case 'About':
                $a.click(function () { $.CswDialog('AboutDialog'); return false; });
                break;

            case 'AddNode':
                $a.click(function ()
                {
                    $.CswDialog('AddNodeDialog', {
                        nodetypeid: tryParseString(json.nodetypeid),
                        relatednodeid: tryParseString(json.relatednodeid), //for Grid Props
                        onAddNode: o.onAlterNode
                    });
                    return false;
                });
                break;

            case 'DeleteNode':
                $a.click(function ()
                {
                    $.CswDialog('DeleteNodeDialog', {
                        nodenames: [ nodename ],
                        nodeids: [ nodeid ],
                        onDeleteNode: o.onAlterNode,
                        NodeCheckTreeId: o.NodeCheckTreeId,
                        Multi: o.Multi
                    });
                    return false;
                });
                break;

            case 'editview':
                $a.click(function () { o.onEditView(viewid); return false; });
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

            case 'PrintLabel':
                $a.click(function ()
                {
                    $.CswDialog('PrintLabelDialog', {
                        'nodeid': nodeid,
                        'propid': tryParseString(json.propid)
                    });
                    return false;
                });
                break;

            case 'Home':
                $a.click(function () { GoHome(); return false; });
                break;

            case 'Logout':
                $a.click(function () { o.onLogout(); return false; });
                break;

            case 'Profile':
                $a.click(function ()
                {
                    $.CswDialog('EditNodeDialog', {
                        nodeids: [ json.userid ],
                        filterToPropId: '',
                        title: 'User Profile',
                        onEditNode: null // function (nodeid, nodekey) { }
                    });
                    return false;
                });
                break;

            case 'ViewSearch':
                $a.click(function ()
                {
                    o.onSearch.onViewSearch();
                });
                break;

            case 'GenericSearch':
                $a.click(function ()
                {
                    o.onSearch.onGenericSearch();
                });
                break;

            case 'multiedit':
                $a.click(o.onMultiEdit);
                break;
            
            case 'SaveViewAs':
                $a.click(function ()
                {
                    $.CswDialog('AddViewDialog', {
                        viewid: viewid,
                        viewmode: tryParseString(json.viewmode),
                        onAddView: o.onSaveView
                    });
                    return false;
                });
                break;

        }
    }
    else
    {
        $li = $('<li>' + text + '</li>')
            .appendTo(o.$ul);
    }
    return $li;
}

// Used by CswDialog and CswViewEditor
function makeViewVisibilitySelect($table, rownum, label)
{
    var $visibilityselect;
    var $visroleselect;
    var $visuserselect;

    IsAdministrator({
        'Yes': function ()
        {

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

            $visibilityselect.change(function ()
            {
                var val = $visibilityselect.val();
                if (val === 'Role')
                {
                    $visroleselect.show();
                    $visuserselect.hide();
                }
                else if (val === 'User')
                {
                    $visroleselect.hide();
                    $visuserselect.show();
                }
                else
                {
                    $visroleselect.hide();
                    $visuserselect.hide();
                }
            }); // change
        } // yes
    }); // IsAdministrator

    return {
        'getvisibilityselect': function () { return $visibilityselect; },
        'getvisroleselect': function () { return $visroleselect; },
        'getvisuserselect': function () { return $visuserselect; }
    };

} // makeViewVisibilitySelect()
//#endregion Menu


//#region Popups
function openPopup(url, height, width)
{
    var popup = window.open(url, null, 'height=' + height + ', width=' + width + ', status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes');
    popup.focus();
    return popup;
}
//#endregion Popups

//#region Validation
function validateTime(value)
{
    var isValid = true;
    var regex = /^(\d?\d):(\d\d)\s?([APap][Mm])?$/g;
    var match = regex.exec(value);
    if (match === null)
    {
        isValid = false;
    }
    else
    {
        var hour = parseInt(match[1]);
        var minute = parseInt(match[2]);
        if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
        {
            isValid = false;
        }
    }
    return isValid;
} // validateTime()

function validateFloatMinValue(value, minvalue)
{
    var nValue = parseFloat(value);
    var nMinValue = parseFloat(minvalue);
    var isValid = true;

    if (nMinValue !== undefined)
    {
        if (nValue === undefined || nValue < nMinValue)
        {
            isValid = false;
        }
    }
    return isValid;
} // validateFloatMinValue()

function validateFloatMaxValue(value, maxvalue)
{
    var nValue = parseFloat(value);
    var nMaxValue = parseFloat(maxvalue);
    var isValid = true;

    if (nMaxValue !== undefined)
    {
        if (nValue === undefined || nValue > nMaxValue)
        {
            isValid = false;
        }
    }
    return isValid;
} // validateFloatMaxValue()

function validateFloatPrecision(value, precision)
{
    var isValid = true;

    var regex;
    if (precision > 0)
    {
        // Allow any valid number -- we'll round later
        regex = /^\-?\d*\.?\d*$/g;
    }
    else
    {
        // Integers Only
        regex = /^\-?\d*$/g;
    }
    if (isValid && !regex.test(value))
    {
        isValid = false;
    }

    return isValid;
} // validateFloatPrecision()

function validateInteger(value)
{
    // Integers Only
    var regex = /^\-?\d*$/g;
    return (regex.test(value));
} // validateInteger()
//#endregion Validation

//#region Dates

function ServerDateFormatToJQuery(ServerDateFormat)
{
    var ret = ServerDateFormat;
    ret = ret.replace(/M/g, 'm');
    ret = ret.replace(/mmm/g, 'M');
    ret = ret.replace(/yyyy/g, 'yy');
    return ret;
}
function ServerTimeFormatToJQuery(ServerTimeFormat)
{
    var ret = ServerTimeFormat;
    return ret;
}

//#endregion Dates

//#region Strings
function startsWith(source, search)
{
    return (source.substr(0, search.length) === search);
}

function getTimeString(date, timeformat)
{
    var MilitaryTime = false;
    if (!isNullOrEmpty(timeformat) && timeformat === "H:mm:ss")
    {
        MilitaryTime = true;
    }

    var ret = '';
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();

    if (minutes < 10) minutes = "0" + minutes;
    if (seconds < 10) seconds = "0" + seconds;

    if (MilitaryTime)
    {
        ret = hours + ":" + minutes + ":" + seconds;
    } 
    else
    {
        ret = (hours % 12) + ":" + minutes + ":" + seconds + " ";
        if (hours > 11)
        {
            ret += "PM";
        } else
        {
            ret += "AM";
        }
    }
    return ret;
}
//#endregion Strings

//#region Debug
function iterate(obj)
{
    var str;
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

function errorHandler(error, includeCallStack, includeLocalStorage, doAlert)
{
    if (hasWebStorage() && includeLocalStorage) log(localStorage);
    if( doAlert ) {
        alert('Error: ' + error.message + ' (Code ' + error.code + ')');
    }
    else {
        log('Error: ' + error.message + ' (Code ' + error.code + ')', includeCallStack);
    }
}
//#endregion Debug

//#region Persistent Logging

function doLogging(value)
{
    var ret = undefined;
    if (hasWebStorage())
    {
        if (arguments.length === 1)
        {
            localStorage['doLogging'] = isTrue(value);
        }
        ret = isTrue(localStorage['doLogging']);
    }
    return ret;
}

function debugOn(value)
{
    var ret = undefined;
    if (hasWebStorage())
    {
        if (arguments.length === 1)
        {
            localStorage['debugOn'] = isTrue(value);
        }
        ret = isTrue(localStorage['debugOn']);
    }
    return ret;
}

function cacheLogInfo(logger, includeCallStack)
{
    if ( doLogging() )
    {
        if (hasWebStorage())
        {
            if (undefined !== logger.setEnded) logger.setEnded();
            var logStorage = new CswClientDb();
            var log = logStorage.getItem('debuglog');
            log += logger.toHtml();

            var extendedLog = '';
            if (isTrue(includeCallStack)) {
                extendedLog = getCallStack();
            }
            if (!isNullOrEmpty(extendedLog)) {
                log += ',' + extendedLog;
            }
            logStorage.setItem('debuglog', log);
        }
    }
}

function purgeLogInfo()
{
    if (hasWebStorage())
    {
        window.sessionStorage.clear();
    }
}

//#endregion Persistent Logging

//#region Browser Compatibility

function hasWebStorage()
{
    var ret = (Modernizr.localstorage || Modernizr.sessionstorage); 
    return ret;
}

// This caused case 22968!
// for Mobile Safari
//function fixGeometry()
//{
//    //thanks to: http://www.semicomplete.com/blog/geekery/jquery-mobile-full-height-content.html
//    /* Some orientation changes leave the scroll position at something
//    * that isn't 0,0. This is annoying for user experience. */
//    scroll(0, 0);

//    /* Calculate the geometry that our content area should take */
//    var $header = $('div:jqmData(role="header"):visible');
//    var $footer = $('div:jqmData(role="footer"):visible');
//    var $content = $('div:jqmData(role="content"):visible');

//    var viewport_height = $(window).height();
//    var content_height = viewport_height - $header.outerHeight() - $footer.outerHeight();

//    //if ((content.outerHeight() - header.outerHeight() - footer.outerHeight()) <= viewport_height)
//    //{
//    content_height -= ($content.outerHeight() - $content.height());
//    $content.height(content_height);
//    //} /* Trim margin/border/padding height */
//};

//#endregion Browser Compatibility