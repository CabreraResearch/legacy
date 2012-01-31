/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswAjax() {
    'use strict';
    
    var _ajaxCount = 0;
    var ajax = (function () {
        /// <summary> The Csw Ajax class for submitting aync webservice requests</summary>
        /// <returns> An instance of the class with get()/post()</returns>
        var clientSession = Csw.clientSession(),
            onBeforeAjax = null, 
            onAfterAjax = null; 

        var dataType = {
            json: 'json',
            xml: 'xml'
        };
       
        function _handleAjaxError(errorJson) {
            Csw.error.showError(errorJson);
        } /* _handleAjaxError() */
        
        function _jsonPost (options) {
            
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
                    clientSession.finishLogout();
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

            if (o.watchGlobal) {
                _ajaxCount += 1;
            }
            Csw.tryExec(onBeforeAjax, o.watchGlobal);

            $.ajax({
                type: 'POST',
                async: o.async,
                url: o.url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(o.data),
                success: function (data) {
                    if (o.watchGlobal) {
                        _ajaxCount -= 1;
                    }
                    var result = $.parseJSON(data.d);

                    if (result.error !== undefined) {
                        if (false === o.overrideError) {
                            _handleAjaxError({
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
                            clientSession.setExpireTime(Csw.string(result.timeout, ''));
                        }

                        delete result.AuthenticationStatus;
                        delete result.timeout;

                        clientSession.handleAuthenticationStatus({
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
                }, /* success{} */
                error: function (xmlHttpRequest, textStatus) {
                    if (o.watchGlobal) {
                        _ajaxCount -= 1;
                    }
                    Csw.log("Webservice Request (" + o.url + ") Failed: " + textStatus);
                    Csw.tryExec(o.error);
                    Csw.tryExec(onAfterAjax, false);
                }
            }); /* $.ajax({ */
        } /* _jsonPost */
        
        function _jsonGet (options) {
            
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
                    clientSession.finishLogout();
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

            if (o.watchGlobal) {
                _ajaxCount += 1;
            }
            Csw.tryExec(onBeforeAjax, o.watchGlobal);

            $.ajax({
                type: 'GET',
                async: o.async,
                url: o.url,
                dataType: 'json',
                data: JSON.stringify(o.data),
                success: function (result) {
                    if (o.watchGlobal) {
                        _ajaxCount -= 1;
                    }

                    if (false === Csw.isNullOrEmpty(result.error)) {
                        if (false === o.overrideError) {
                            _handleAjaxError({
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
                    Csw.tryExec(onAfterAjax, true);
                }, /* success{} */
                error: function (xmlHttpRequest, textStatus) {
                    if (o.watchGlobal) {
                        _ajaxCount -= 1;
                    }
                    Csw.log("Webservice Request (" + o.url + ") Failed: " + textStatus);
                    Csw.tryExec(o.error);
                    Csw.tryExec(onAfterAjax, false);
                }
            }); /* $.ajax({ */
        } /* _jsonGet() */

        function _xmlPost (options) {
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
                stringify: false, /* in case we need to conditionally apply $.param() instead of JSON.stringify() (or both) */
                onloginfail: function () {
                    clientSession.finishLogout();
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
                if (o.watchGlobal) {
                    _ajaxCount += 1;
                }
                $.ajax({
                    type: 'POST',
                    async: o.async,
                    url: o.url,
                    dataType: "text",
                    data: $.param(o.data), 
                    success: function (data, textStatus, xmlHttpRequest) {
                        if (o.watchGlobal) {
                            _ajaxCount -= 1;
                        }
                        
                        var $realxml;
                        if ($.browser.msie) {
                            /* We have to use third-party jquery.xml.js for Internet Explorer to handle non-DOM XML content */
                            $realxml = $.xml(data);
                        } else {
                            $realxml = $(xmlHttpRequest.responseXML).children().first();
                        }

                        if ($realxml.first().get(0).nodeName === "error") {
                            _handleAjaxError({
                                'display': $realxml.CswAttrNonDom('display'),
                                'type': $realxml.CswAttrNonDom('type'),
                                'message': $realxml.CswAttrNonDom('message'),
                                'detail': $realxml.CswAttrNonDom('detail')
                            }, '');
                            o.error();
                        } else {
                            var auth = Csw.string($realxml.CswAttrNonDom('authenticationstatus'), 'Unknown');
                            if (false === o.formobile) {
                                clientSession.setExpireTime($realxml.CswAttrNonDom('timeout'));
                            }

                            clientSession.handleAuthenticationStatus({
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
                        if (o.watchGlobal) {
                            _ajaxCount -= 1;
                        }
                        Csw.log("Webservice Request (" + o.url + ") Failed: " + textStatus);
                        Csw.tryExec(o.error);
                    }
                }); /* $.ajax({ */
            } /* if(o.url != '') */
        } /* _xmlPost() */
        
        function ajaxInProgress () {
            /// <summary> Evaluates whether a pending ajax request is still open. </summary>
            return (_ajaxCount > 0);
        }
        
        function get(options, type) {
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
            if(ajaxType.toLowerCase() !== dataType.xml) {
                ret = _jsonGet(options);
            } 
            return ret;
        }

        function post(options, type) {
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
            var ret,
                ajaxType = Csw.string(type);
            if(ajaxType.toLowerCase() === dataType.xml) {
                ret = _xmlPost(options);
            } else {
                ret = _jsonPost(options);
            }
            return ret;
        }

        return {
            get: get,
            post: post,
            dataType: dataType,
            ajaxInProgress: ajaxInProgress,
            onBeforeAjax: onBeforeAjax,
            onAfterAjax: onAfterAjax
        };

    }());    
    Csw.register('ajax', ajax);
    Csw.ajax = Csw.ajax || ajax;
}());