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

    cswPrivate.consoleTable = null;
    cswPrivate.consoleRows = 1;
    cswPrivate.initConsoleTable = function () {
        cswPrivate.consoleTable = cswPrivate.consoleTable || Csw.literals.div({ ID: 'Csw_output_log' }).table();
        Csw.debug.info(cswPrivate.consoleTable.$[0]);
    };

    cswPrivate.onJsonSuccess = Csw.method(function (o, data, url) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);
        cswPrivate.initConsoleTable();
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
                $.extend(true, timer, result.timer);
                if (Csw.bool(o.removeTimer)) {
                    delete result.timer;
                }
                if (Csw.isNullOrEmpty(cswPrivate.perflogheaders)) {
                    cswPrivate.perflogheaders = true;
                    cswPrivate.consoleTable.cell(1, 1).span({ text: 'timestamp' });
                    cswPrivate.consoleTable.cell(1, 2).span({ text: 'client' });
                    cswPrivate.consoleTable.cell(1, 3).span({ text: 'serverinit' });
                    cswPrivate.consoleTable.cell(1, 4).span({ text: 'servertotal' });
                    cswPrivate.consoleTable.cell(1, 5).span({ text: 'dbinit' });
                    cswPrivate.consoleTable.cell(1, 6).span({ text: 'dbquery' });
                    cswPrivate.consoleTable.cell(1, 7).span({ text: 'dbcommit' });
                    cswPrivate.consoleTable.cell(1, 8).span({ text: 'dbdeinit' });
                    cswPrivate.consoleTable.cell(1, 9).span({ text: 'treeloadersql' });
                    cswPrivate.consoleTable.cell(1, 10).span({ text: 'url' });
                    Csw.debug.info("timestamp\t" +
                                 "client\t" +
                                 "serverinit\t" +
                                 "servertotal\t" +
                                 "dbinit\t" +
                                 "dbquery\t" +
                                 "dbcommit\t" +
                                 "dbdeinit\t" +
                                 "treeloadersql\t" +
                                 "url\t");
                }
                var endTime = new Date();
                var etms = Csw.string(endTime.getMilliseconds());
                while (etms.length < 3) {
                    etms = "0" + etms;
                }
                cswPrivate.consoleRows += 1;
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 1).span({ text: endTime.toLocaleTimeString() + "." + etms });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 2).span({ text: (endTime - o.startTime) });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 3).span({ text: timer.serverinit });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 4).span({ text: timer.servertotal });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 5).span({ text: timer.dbinit });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 6).span({ text: timer.dbquery });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 7).span({ text: timer.dbcommit });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 8).span({ text: timer.dbdeinit });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 9).span({ text: timer.treeloadersql });
                cswPrivate.consoleTable.cell(cswPrivate.consoleRows, 10).span({ text: url });

                Csw.debug.info(endTime.toLocaleTimeString() + "." + etms + "\t" +
                        (endTime - o.startTime) + "\t" +
                        timer.serverinit + "\t" +
                        timer.servertotal + "\t" +
                        timer.dbinit + "\t" +
                        timer.dbquery + "\t" +
                        timer.dbcommit + "\t" +
                        timer.dbdeinit + "\t" +
                        timer.treeloadersql + "\t" +
                        url);
            }

            delete result.AuthenticationStatus;
            delete result.timeout;

            Csw.clientSession.handleAuthenticationStatus({
                status: auth,
                success: function () {
                    Csw.tryExec(o.success, result);
                    if (true === Csw.displayAllExceptions) {
                        Csw.debug.profileEnd(url);
                        Csw.debug.timeEnd('onSuccess called for url: ' + url);
                    }
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
        Csw.debug.timeEnd(url);
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
            $.extend(o, options);
        }
        var url = Csw.string(o.url, o.urlPrefix + o.urlMethod);
        o.startTime = new Date();
        if (true === Csw.displayAllExceptions) {
            Csw.debug.profile(url);
            Csw.debug.time(url);
        }

        Csw.publish(Csw.enums.events.ajax.ajaxStart, o.watchGlobal);
        $.ajax({
            type: 'POST',
            async: o.async,
            urlPrefix: Csw.enums.ajaxUrlPrefix,
            url: url,
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
            $.extend(o, options);
        }
        var url = Csw.string(o.url, o.urlPrefix + o.urlMethod);
        Csw.debug.time('onSuccess called for url: ' + url);
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
            $.extend(o, options);
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
