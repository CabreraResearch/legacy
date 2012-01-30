/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
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

    
}());