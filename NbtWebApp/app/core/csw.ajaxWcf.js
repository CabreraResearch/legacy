/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var cswPrivate = {
        response: {
            Data: '',
            Authentication: {
                AuthenticationStatus: 'Unknown',
                TimeOut: 0
            },
            Performance: {
                ServerInit: 0,
                DbInit: 0,
                DbQuery: 0,
                DbCommit: 0,
                DbDeinit: 0,
                TreeLoaderSql: 0, 
                ServerTotal: 0 
            },
            Logging: {
                LogLevel: 'None',
                CustomerId: 'nbt_master',
                Server: 'localhost',
                LogglyInput: '75c30bba-4b60-496c-a348-7eb413c01037'
            },
            Status: {
                Success: true,
                Errors: [],
                DisplayErrors: false
            }
        }
    };

    cswPrivate.handleAjaxError = function (errorJson) {
        Csw.error.showError(errorJson);
    }; /* cswPrivate.handleAjaxError() */

    cswPrivate.onJsonSuccess = Csw.method(function (o, data, url) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);
        Csw.extend(cswPrivate.response, data, true);
        
        if (false === cswPrivate.response.Status.Success &&
            cswPrivate.response.Status.Errors.length > 0) {
            var lastErr = cswPrivate.response.Status.Errors.length - 1;
            if (false === o.overrideError) {
                cswPrivate.handleAjaxError({
                    display: cswPrivate.response.Status.Errors[lastErr].Display,
                    type: cswPrivate.response.Status.Errors[lastErr].Type,
                    message: cswPrivate.response.Status.Errors[lastErr].Message,
                    detail: cswPrivate.response.Status.Errors[lastErr].Detail
                    }, '');
                
            }
            Csw.tryExec(o.error, cswPrivate.response.Status.Errors[lastErr]);
        } else {

            var auth = Csw.string(cswPrivate.response.Authentication.AuthenticationStatus, 'Unknown');
            if (false === o.formobile) {
                Csw.clientSession.setExpireTime(Csw.string(cswPrivate.response.Authentication.TimeOut, ''));
            }

            if (false === Csw.isNullOrEmpty(cswPrivate.response.Performance)) {
                cswPrivate.response.Performance.url = url;
                
                var endTime = new Date();
                var etms = Csw.string(endTime.getMilliseconds());
                while (etms.length < 3) {
                    etms = '0' + etms;
                }
                
                cswPrivate.response.Performance.TimeStamp = endTime.toLocaleTimeString() + '.' + etms;
                cswPrivate.response.Performance.Client = (endTime - o.startTime);
                
                Csw.clientSession.setLogglyInput(cswPrivate.response.Logging.LogglyInput, cswPrivate.response.Logging.LogLevel, cswPrivate.response.Logging.Server);
                Csw.debug.perf(cswPrivate.response.Performance);
                
            }
            
            Csw.clientSession.handleAuthenticationStatus({
                status: auth,
                success: function () {
                    Csw.tryExec(o.success, cswPrivate.response.Authentication);
                },
                failure: o.onloginfail,
                data: cswPrivate.response.Authentication
            });
        }
    });

    cswPrivate.onJsonError = Csw.method(function (xmlHttpRequest, textStatus, o) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
        Csw.debug.error('Webservice Request (' + o.url + ') Failed: ' + textStatus);
        Csw.tryExec(o.error, textStatus);
    });

    cswPrivate.execRequest = Csw.method(function (verb, options) {
        /// <summary>Executes Async webservice request for JSON</summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.url: WebService URL
        ///     &#10;2 - options.data: {field1: value, field2: value}
        ///     &#10;3 - options.success: function () {}
        ///     &#10;4 - options.error: function () {}
        /// </param>
        verb = verb || 'POST';
        var cswInternal = {
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
        Csw.extend(cswInternal, options);
        
        var cswExternal = { };
        cswInternal.url = Csw.string(cswInternal.url, cswInternal.urlPrefix + cswInternal.urlMethod);
        cswInternal.startTime = new Date();

        Csw.publish(Csw.enums.events.ajax.ajaxStart, cswInternal.watchGlobal);
        cswExternal.ajax = $.ajax({
            type: verb,
            url: cswInternal.urlMethod,
            xhrFields: {
                withCredentials: true
            },
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            //processdata: false,
            data: Csw.serialize(cswInternal.data),
            success: function (data) {
                cswPrivate.onJsonSuccess(cswInternal, data, cswInternal.url);
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus) {
                cswPrivate.onJsonError(xmlHttpRequest, textStatus, cswInternal);
            }
        }); /* $.ajax({ */
        return cswExternal;
    }); /* cswPrivate.jsonPost */

    Csw.ajaxWcf.post = Csw.ajaxWcf.post ||
        Csw.ajaxWcf.register('post', function (options, type) {
            /// <summary> Executes Async webservice request using HTTP verb 'POST'. </summary>
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
            return cswPrivate.execRequest('POST', options);
        });

    Csw.ajaxWcf['get'] = Csw.ajaxWcf['get'] ||
        Csw.ajaxWcf.register('get', function (options, type) {
            /// <summary> Executes Async webservice request using HTTP verb 'GET'. </summary>
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
            return cswPrivate.execRequest('GET', options);
        });

    Csw.ajaxWcf['delete'] = Csw.ajaxWcf['delete'] ||
        Csw.ajaxWcf.register('delete', function (options, type) {
            /// <summary> Executes Async webservice request using HTTP verb 'DELETE'. </summary>
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
            return cswPrivate.execRequest('DELETE', options);
        });
    
    Csw.ajaxWcf.put = Csw.ajaxWcf.put ||
        Csw.ajaxWcf.register('put', function (options, type) {
            /// <summary> Executes Async webservice request using HTTP verb 'PUT'. </summary>
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
            return cswPrivate.execRequest('PUT', options);
        });



} ());
