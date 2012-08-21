/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    var cswPrivate = {
        ajaxCount: 0,
        enums: {
            dataType: {
                json: 'json',
                xml: 'xml'
            }
        },
        ajaxBindingsHaveRun: false
    };

    cswPrivate.handleAjaxError = function (errorJson) {
        Csw.error.showError(errorJson);
    }; /* cswPrivate.handleAjaxError() */

    cswPrivate.onJsonSuccess = Csw.method(function (o, data, url) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);
        var result = data;
        if (data.d) {
            result = $.parseJSON(data.d);
        }
        if (result.error !== undefined) {
            if (false === o.overrideError) {
                cswPrivate.handleAjaxError({
                    'display': result.error.display,
                    'type': result.error.type,
                    'message': result.error.message,
                    'detail': result.error.detail
                }, '');
            }
            Csw.tryExec(o.error, result.error);
        } else {

            var auth = Csw.string(result.AuthenticationStatus, 'Unknown');
            if (false === o.formobile) {
                Csw.clientSession.setExpireTime(Csw.string(result.timeout, ''));
            }

            if (false === Csw.isNullOrEmpty(result.timer)) {
                var timer = {};
                timer[url] = result.timer;
                
                var endTime = new Date();
                var etms = Csw.string(endTime.getMilliseconds());
                while (etms.length < 3) {
                    etms = '0' + etms;
                }
                
                result.timer.timestap = endTime.toLocaleTimeString() + '.' + etms;
                result.timer.client = (endTime - o.startTime);
                result.timer.url = url;
                Csw.clientSession.setLogglyInput(result.LogglyInput, result.LogLevel, result.server);
                Csw.debug.perf(result.timer);
                
            }
            delete result.server;
            delete result.LogglyInput;
            delete result.LogLevel;
            delete result.AuthenticationStatus;
            delete result.timeout;
            delete result.timer;
            
            Csw.clientSession.handleAuthenticationStatus({
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
    });

    cswPrivate.onJsonError = Csw.method(function (xmlHttpRequest, textStatus, o) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
        Csw.debug.error('Webservice Request (' + o.url + ') Failed: ' + textStatus);
        Csw.tryExec(o.error, textStatus);
    });

    cswPrivate.jsonPost = function (options) {
        /// <summary>Executes Async webservice request for JSON</summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        /// </param>
        var o = {
            url: '',
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            urlMethod: '',
            data: {},
            onloginfail: function () {
                Csw.clientSession.finishLogout();
            },
            success: null,
            error: null,
            overrideError: false,
            formobile: false,
            async: true,
            watchGlobal: true,
            removeTimer: true
        };
        if (options) {
            Csw.extend(o, options);
        }
        var url = Csw.string(o.url, o.urlPrefix + o.urlMethod);
        o.startTime = new Date();

        Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);
        $.ajax({
            type: 'POST',
            async: o.async,
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            url: url,
            xhrFields: {
                withCredentials: true
            },
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(o.data),
            success: function (data) {
                cswPrivate.onJsonSuccess(o, data, url);
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                cswPrivate.onJsonError(xmlHttpRequest, textStatus, o);
            }
        }); /* $.ajax({ */
    }; /* cswPrivate.jsonPost */

    cswPrivate.jsonGet = function (options) {

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
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            urlMethod: '',
            data: {},
            onloginfail: function () {
                Csw.clientSession.finishLogout();
            },
            success: null,
            error: null,
            overrideError: false,
            formobile: false,
            async: true,
            watchGlobal: true
        };
        if (options) {
            Csw.extend(o, options);
        }
        var url = Csw.string(o.url, o.urlPrefix + o.urlMethod);
        //Csw.debug.time(url);
        Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);

        $.ajax({
            type: 'GET',
            async: o.async,
            url: url,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: $.param(o.data),
            success: function (data) {
                cswPrivate.onJsonSuccess(o, data, url);
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                cswPrivate.onJsonError(xmlHttpRequest, textStatus, o);
            }
        }); /* $.ajax({ */
    };

    cswPrivate.xmlPost = function (options) {
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
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            urlMethod: '',
            data: {},
            stringify: false, /* in case we need to conditionally apply $.param() instead of JSON.stringify() (or both) */
            onloginfail: function () {
                Csw.clientSession.finishLogout();
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
            Csw.extend(o, options);
        }

        var url = Csw.string(o.url, o.urlPrefix + o.urlMethod);
        if (false === Csw.isNullOrEmpty(o.url)) {
            Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);
            $.ajax({
                type: 'POST',
                async: o.async,
                url: url,
                dataType: 'text',
                data: $.param(o.data),
                success: function (data, textStatus, xmlHttpRequest) {
                    Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);

                    var $realxml;
                    if ($.browser.msie) {
                        /* We have to use third-party jquery.xml.js for Internet Explorer to handle non-DOM XML content */
                        $realxml = $.xml(data);
                    } else {
                        $realxml = $(xmlHttpRequest.responseXML).children().first();
                    }

                    if ($realxml.first().get(0).nodeName === 'error') {
                        cswPrivate.handleAjaxError({
                            'display': $realxml.CswAttrNonDom('display'),
                            'type': $realxml.CswAttrNonDom('type'),
                            'message': $realxml.CswAttrNonDom('message'),
                            'detail': $realxml.CswAttrNonDom('detail')
                        }, '');
                        o.error();
                    } else {
                        var auth = Csw.string($realxml.CswAttrNonDom('authenticationstatus'), 'Unknown');
                        if (false === o.formobile) {
                            Csw.clientSession.setExpireTime($realxml.CswAttrNonDom('timeout'));
                        }

                        Csw.clientSession.handleAuthenticationStatus({
                            status: auth,
                            success: function () {
                                Csw.tryExec(o.success, $realxml);
                            },
                            failure: o.onloginfail,
                            usernodeid: Csw.string($realxml.CswAttrNonDom('nodeid'), ''),
                            usernodekey: Csw.string($realxml.CswAttrNonDom('cswnbtnodekey'), ''),
                            passwordpropid: Csw.string($realxml.CswAttrNonDom('passwordpropid'), ''),
                            ForMobile: o.formobile
                        });
                    }

                }, /* success{} */
                error: function (xmlHttpRequest, textStatus) {
                    Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
                    Csw.debug.log('Webservice Request (' + o.url + ') Failed: ' + textStatus);
                    Csw.tryExec(o.error);
                }
            }); /* $.ajax({ */
        } /* if(o.url != '') */
    }; /* cswPrivate.xmlPost() */

    cswPrivate.bindAjaxEvents = (function () {
        if (false === cswPrivate.ajaxBindingsHaveRun) {
            return function () {
                function ajaxStart(eventObj, watchGlobal) {
                    if (watchGlobal) {
                        cswPrivate.ajaxCount += 1;
                        if (cswPrivate.ajaxCount === 1) {
                            Csw.publish(Csw.enums.events.ajax.globalAjaxStart);
                        }
                    }
                }

                Csw.subscribe(Csw.enums.events.ajax.ajaxStart, ajaxStart);

                function ajaxStop(eventObj, watchGlobal) {
                    if (watchGlobal) {
                        cswPrivate.ajaxCount -= 1;
                        if (cswPrivate.ajaxCount < 0) {
                            cswPrivate.ajaxCount = 0;
                        }
                    }
                    if (cswPrivate.ajaxCount <= 0) {
                        Csw.publish(Csw.enums.events.ajax.globalAjaxStop);
                    }
                }

                Csw.subscribe(Csw.enums.events.ajax.ajaxStop, ajaxStop);
            };
        } else {
            return null;
        }
    } ());

    Csw.ajax = Csw.ajax ||
        Csw.register('ajax', Csw.makeNameSpace(null, cswPrivate));


    Csw.ajax.ajaxInProgress = Csw.ajax.ajaxInProgress ||
        Csw.ajax.register('ajaxInProgress', function () {
            /// <summary> Evaluates whether a pending ajax request is still open. </summary>
            return (cswPrivate.ajaxCount > 0);
        });

    Csw.ajax.get = Csw.ajax.get ||
        Csw.ajax.register('get', function (options, type) {
            /// <summary>
            ///   Executes Async webservice request for XML
            /// </summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.url: WebService URL</para>
            /// <para>options.data: {field1: value, field2: value}</para>
            /// <para>options.success: function () {}</para>
            /// <para>options.error: function () {}</para>
            /// <para>options.formobile: false</para>
            /// </param>
            /// <param name="type" type="String">XML or JSON (default)</param>
            /// <return type="Object">Returns the results of the $.ajax() request in an object wrapper.</return>
            var ret = {},
                ajaxType = Csw.string(type);
            if (false === cswPrivate.ajaxBindingsHaveRun) {
                Csw.tryExec(cswPrivate.bindAjaxEvents);
            }
            if (ajaxType.toLowerCase() !== cswPrivate.enums.dataType.xml) {
                ret = cswPrivate.jsonGet(options);
            }
            return ret;
        });

    Csw.ajax.post = Csw.ajax.post ||
        Csw.ajax.register('post', function (options, type) {
            /// <summary> Executes Async webservice request. </summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.url: WebService URL</para>
            /// <para>options.data: {field1: value, field2: value}</para>
            /// <para>options.success: function () {}</para>
            /// <para>options.error: function () {}</para>
            /// <para>options.formobile: false</para>
            /// </param>
            /// <param name="type" type="String">XML or JSON (default)</param>
            /// <return type="Object">Returns the results of the $.ajax() request in an object wrapper.</return>
            var ret,
                ajaxType = Csw.string(type);
            if (false === cswPrivate.ajaxBindingsHaveRun) {
                Csw.tryExec(cswPrivate.bindAjaxEvents);
            }
            if (ajaxType.toLowerCase() === cswPrivate.enums.dataType.xml) {
                ret = cswPrivate.xmlPost(options);
            } else {
                ret = cswPrivate.jsonPost(options);
            }
            return ret;
        });

    Csw.ajax.dataType = Csw.ajax.dataType ||
        Csw.ajax.register('dataType', cswPrivate.enums.dataType);

} ());
