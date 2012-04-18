/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswAjax() {
    'use strict';

    var ajax = (function () {
        /// <summary> The Csw Ajax class for submitting aync webservice requests</summary>
        /// <returns> An instance of the class with get()/post()</returns>
        var external = {
            onBeforeAjax: null,
            onAfterAjax: null
        };

        var internal = {
            ajaxCount: 0,
            enums: {
                dataType: {
                    json: 'json',
                    xml: 'xml'
                }
            }
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
            
            var startTime = new Date();
            $.ajax({
                type: 'POST',
                async: o.async,
                urlPrefix: Csw.enums.ajaxUrlPrefix,
                url: o.url,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(o.data),
                success: function (data) {
                    var endTime = new Date();
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
		            

if(Csw.isNullOrEmpty(internal.perflogheaders)) {
    internal.perflogheaders = true;
    Csw.log( "timestamp\t" + 
             "url\t" + 
             "client\t" + 
             "serverinit\t" + 
             "servertotal\t" + 
             "dbinit\t" + 
             "dbquery\t" + 
             "dbcommit\t" + 
             "dbdeinit" );
}
var ms = Csw.string(endTime.getMilliseconds());
while (ms.length < 3) { 
    ms = "0" + ms;
}
Csw.log( endTime.toLocaleTimeString() + "." + ms + "\t" + 
         o.url + "\t" + 
         (endTime - startTime) + "\t" + 
         result.timer.serverinit + "\t" + 
         result.timer.servertotal + "\t" + 
         result.timer.dbinit + "\t" + 
         result.timer.dbquery + "\t" + 
         result.timer.dbcommit + "\t" + 
         result.timer.dbdeinit );

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
                    Csw.tryExec(external.onAfterAjax, true);
                }, /* success{} */
                error: function (xmlHttpRequest, textStatus) {
                    Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
                    Csw.log('Webservice Request (' + o.url + ') Failed: ' + textStatus);
                    Csw.tryExec(o.error,textStatus);
                    Csw.tryExec(external.onAfterAjax, false);
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
                    Csw.tryExec(external.onAfterAjax, true);
                }, /* success{} */
                error: function (xmlHttpRequest, textStatus) {
                    Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
                    Csw.log('Webservice Request (' + o.url + ') Failed: ' + textStatus);
                    Csw.tryExec(o.error);
                    Csw.tryExec(external.onAfterAjax, false);
                }
            }); /* $.ajax({ */
        }; /* internal.jsonGet() */

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

        external.ajaxInProgress = function () {
            /// <summary> Evaluates whether a pending ajax request is still open. </summary>
            return (internal.ajaxCount > 0);
        };

        external.get = function (options, type) {
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
            if (ajaxType.toLowerCase() !== internal.enums.dataType.xml) {
                ret = internal.jsonGet(options);
            }
            return ret;
        };

        external.post = function (options, type) {
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
            if (ajaxType.toLowerCase() === internal.enums.dataType.xml) {
                ret = internal.xmlPost(options);
            } else {
                ret = internal.jsonPost(options);
            }
            return ret;
        };

        external.dataType = internal.enums.dataType;

        (function () {

            function ajaxStart(eventObj, watchGlobal) {
                Csw.tryExec(external.onBeforeAjax, watchGlobal);
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

        } ());

        return external;

    } ());
    Csw.register('ajax', ajax);
    Csw.ajax = Csw.ajax || ajax;
} ());
