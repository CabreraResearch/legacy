/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    var internal = {
        ajaxCount: 0,
        enums: {
            dataType: {
                json: 'json',
                xml: 'xml'
            }
        },
        ajaxBindingsHaveRun: false
    };

    internal.handleAjaxError = function (errorJson) {
        Csw.error.showError(errorJson);
    }; /* internal.handleAjaxError() */

    internal.jsonPost = function (options) {
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
            $.extend(o, options);
        }

        Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);

        $.ajax({
            type: 'POST',
            async: o.async,
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            url: o.url,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(o.data),
            success: function (data) {
                Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);
                var result = $.parseJSON(data.d);

                if (result.error !== undefined) {
                    if (false === o.overrideError) {
                        internal.handleAjaxError({
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

                    delete result.AuthenticationStatus;
                    delete result.timeout;

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
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
                Csw.log('Webservice Request (' + o.url + ') Failed: ' + textStatus);
                Csw.tryExec(o.error, textStatus);
            }
        }); /* $.ajax({ */
    }; /* internal.jsonPost */

    internal.jsonGet = function (options) {

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
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            url: '',
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
            $.extend(o, options);
        }

        Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);

        $.ajax({
            type: 'GET',
            async: o.async,
            url: o.url,
            dataType: 'json',
            data: JSON.stringify(o.data),
            success: function (result) {
                Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);

                if (false === Csw.isNullOrEmpty(result.error)) {
                    if (false === o.overrideError) {
                        internal.handleAjaxError({
                            'display': result.error.display,
                            'type': result.error.type,
                            'message': result.error.message,
                            'detail': result.error.detail
                        }, '');
                    }
                    Csw.tryExec(o.error, result.error);
                } else {
                    Csw.tryExec(o.success, result);
                }
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
                Csw.log('Webservice Request (' + o.url + ') Failed: ' + textStatus);
                Csw.tryExec(o.error);
            }
        }); /* $.ajax({ */
    };

    internal.xmlPost = function (options) {
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
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            url: '',
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
            $.extend(o, options);
        }

        if (false === Csw.isNullOrEmpty(o.url)) {
            Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);
            $.ajax({
                type: 'POST',
                async: o.async,
                url: o.url,
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
                        internal.handleAjaxError({
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
                    Csw.log('Webservice Request (' + o.url + ') Failed: ' + textStatus);
                    Csw.tryExec(o.error);
                }
            }); /* $.ajax({ */
        } /* if(o.url != '') */
    }; /* internal.xmlPost() */

    internal.bindAjaxEvents = (function () {
        if (false === internal.ajaxBindingsHaveRun) {
            return function () {
                function ajaxStart(eventObj, watchGlobal) {
                    if (watchGlobal) {
                        internal.ajaxCount += 1;
                        if (internal.ajaxCount === 1) {
                            Csw.publish(Csw.enums.events.ajax.globalAjaxStart);
                        }
                    }
                }

                Csw.subscribe(Csw.enums.events.ajax.ajaxStart, ajaxStart);

                function ajaxStop(eventObj, watchGlobal) {
                    if (watchGlobal) {
                        internal.ajaxCount -= 1;
                        if (internal.ajaxCount < 0) {
                            internal.ajaxCount = 0;
                        }
                    }
                    if (internal.ajaxCount <= 0) {
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
        Csw.register('ajax', Csw.makeNameSpace(null, internal));


    Csw.ajax.ajaxInProgress = Csw.ajax.ajaxInProgress ||
        Csw.ajax.register('ajaxInProgress', function () {
            /// <summary> Evaluates whether a pending ajax request is still open. </summary>
            return (internal.ajaxCount > 0);
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
            if (false === internal.ajaxBindingsHaveRun) {
                Csw.tryExec(internal.bindAjaxEvents);
            }
            if (ajaxType.toLowerCase() !== internal.enums.dataType.xml) {
                ret = internal.jsonGet(options);
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
            if (false === internal.ajaxBindingsHaveRun) {
                Csw.tryExec(internal.bindAjaxEvents);
            }
            if (ajaxType.toLowerCase() === internal.enums.dataType.xml) {
                ret = internal.xmlPost(options);
            } else {
                ret = internal.jsonPost(options);
            }
            return ret;
        });

    Csw.ajax.dataType = Csw.ajax.dataType ||
        Csw.ajax.register('dataType', internal.enums.dataType);

} ());
