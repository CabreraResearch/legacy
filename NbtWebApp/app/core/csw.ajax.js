/// <reference path="~/app/CswApp-vsdoc.js" />


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
                    display: result.error.display,
                    type: result.error.type,
                    message: result.error.message,
                    detail: result.error.detail
                }, '');
            }
            Csw.tryExec(o.error, result.error);
        } else {

            var auth = Csw.string(result.AuthenticationStatus, 'Unknown');
                Csw.clientSession.setExpireTime(Csw.string(result.timeout, ''));

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
                passwordpropid: result.passwordpropid
            });
        }
    });

    cswPrivate.onJsonError = Csw.method(function (xmlHttpRequest, textStatus, o) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
        if (textStatus !== 'abort' && xmlHttpRequest.status !== 0 && xmlHttpRequest.readyState !== 0) {
            Csw.debug.error('Webservice Request (' + o.url + ') Failed: ' + textStatus);
            Csw.tryExec(o.error, textStatus);
        }
    });

    cswPrivate.jsonPost = Csw.method(function (options) {
        /// <summary>Executes Async webservice request for JSON</summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        /// </param>
        var cswInternal = {
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
            async: true,
            watchGlobal: true,
            removeTimer: true
        };
        Csw.extend(cswInternal, options);

        var cswExternal = { };
        cswInternal.url = Csw.string(cswInternal.url, cswInternal.urlPrefix + cswInternal.urlMethod);
        cswInternal.startTime = new Date();

        Csw.publish(Csw.enums.events.ajax.ajaxStart, cswInternal.watchGlobal);
        cswExternal.ajax = $.ajax({
            type: 'POST',
            async: cswInternal.async,
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            url: cswInternal.url,
            xhrFields: {
                withCredentials: true
            },
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(cswInternal.data),
            success: function (data) {
                cswPrivate.onJsonSuccess(cswInternal, data, cswInternal.url);
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                cswPrivate.onJsonError(xmlHttpRequest, textStatus, cswInternal);
            }
        }); /* $.ajax({ */
        return cswExternal;
    }); /* cswPrivate.jsonPost */

    cswPrivate.jsonGet = Csw.method(function (options) {

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
        var cswInternal = {
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
            async: true,
            watchGlobal: true
        };
        Csw.extend(cswInternal, options);
        var cswExternal = { };
        cswInternal.url = Csw.string(cswInternal.url, cswInternal.urlPrefix + cswInternal.urlMethod);
        //Csw.debug.time(url);
        Csw.publish(Csw.enums.events.ajax.ajaxStart, cswInternal.watchGlobal);

        cswExternal.ajax = $.ajax({
            type: 'GET',
            async: cswInternal.async,
            url: cswInternal.url,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: $.param(cswInternal.data),
            success: function (data) {
                cswPrivate.onJsonSuccess(cswInternal, data, cswInternal.url);
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                cswPrivate.onJsonError(xmlHttpRequest, textStatus, cswInternal);
            }
        }); /* $.ajax({ */
        return cswExternal;
    });

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
