/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var cswPrivate = {};

    cswPrivate.handleAjaxError = function (errorJson) {
        Csw.error.showError(errorJson);
    }; /* cswPrivate.handleAjaxError() */

    cswPrivate.onJsonSuccess = Csw.method(function (o, data, url) {

        var response = {
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
                Errors: []
            }
        };

        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal);
        Csw.extend(response, data, true);
        
        if (false === response.Status.Success ||
            response.Status.Errors.length > 0) {
            var lastErr = response.Status.Errors.length - 1;
            if (false === o.overrideError) {
                cswPrivate.handleAjaxError({
                    display: response.Status.Errors[lastErr].Display,
                    type: response.Status.Errors[lastErr].Type,
                    message: response.Status.Errors[lastErr].Message,
                    detail: response.Status.Errors[lastErr].Detail
                    }, '');
                
            }
            Csw.tryExec(o.error, response.Status.Errors[lastErr]);
        } else {

            var auth = Csw.string(response.Authentication.AuthenticationStatus, 'Unknown');
                Csw.clientSession.setExpireTime(Csw.string(response.Authentication.TimeOut, ''));

            if (false === Csw.isNullOrEmpty(response.Performance)) {
                response.Performance.url = url;
                
                var endTime = new Date();
                var etms = Csw.string(endTime.getMilliseconds());
                while (etms.length < 3) {
                    etms = '0' + etms;
                }
                
                response.Performance.TimeStamp = endTime.toLocaleTimeString() + '.' + etms;
                response.Performance.Client = (endTime - o.startTime);
                
                Csw.clientSession.setLogglyInput(response.Logging.LogglyInput, response.Logging.LogLevel, response.Logging.Server);
                Csw.debug.perf(response.Performance);
                
            }
            
            Csw.clientSession.handleAuthenticationStatus({
                status: auth,
                success: function () {
                    Csw.tryExec(o.success, response.Data);
                },
                failure: o.onloginfail,
                data: response.Authentication
            });
        }
    });

    cswPrivate.onJsonError = Csw.method(function (xmlHttpRequest, textStatus, param1, o) {
        Csw.publish(Csw.enums.events.ajax.ajaxStop, o.watchGlobal, xmlHttpRequest, textStatus);
        if (textStatus !== 'abort') {
            Csw.debug.error({
                'Webservice Request': o.urlMethod,
                data: o.data,
                Failed: textStatus,
                state: xmlHttpRequest.state(),
                status: xmlHttpRequest.status,
                statusText: xmlHttpRequest.statusText,
                readyState: xmlHttpRequest.readyState,
                responseText: xmlHttpRequest.responseText
            });
            Csw.tryExec(o.error, textStatus);
        }
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
            data: '',
            onloginfail: function () {
                Csw.clientSession.finishLogout();
            },
            success: function () { },
            error: function () { },
            complete: function () {},
            overrideError: false,
            async: true,
            watchGlobal: true,
            removeTimer: true
        };
        Csw.extend(cswInternal, options);
        
        var cswExternal = { };
        cswInternal.urlMethod = 'Services/' + cswInternal.urlMethod;
        cswInternal.url = Csw.string(cswInternal.url, cswInternal.urlMethod);
        cswInternal.startTime = new Date();
        if(false === Csw.isNullOrEmpty(cswInternal.data)) {
            if (verb === 'GET') {
                cswInternal.data = Csw.params(cswInternal.data);
            }
            else {
                cswInternal.data = Csw.serialize(cswInternal.data);
            }
        }

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
            data: cswInternal.data,
            success: function (data) {
                cswPrivate.onJsonSuccess(cswInternal, data, document.location + '/' + cswInternal.urlMethod);
            }, /* success{} */
            error: function (xmlHttpRequest, textStatus, param1) {
                cswPrivate.onJsonError(xmlHttpRequest, textStatus, param1, { 
                    data: cswInternal.data, 
                    watchGlobal: cswInternal.watchGlobal, 
                    urlMethod: document.location + '/' + cswInternal.urlMethod 
                    }
                 );
            },
            complete: function(xmlHttpRequest, textStatus) {
                Csw.tryExec(cswInternal.complete, xmlHttpRequest, textStatus);
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
            /// </param>
            /// <param name="type" type="String">XML or JSON (default)</param>
            /// <return type="Object">Returns the results of the $.ajax() request in an object wrapper.</return>
            return cswPrivate.execRequest('PUT', options);
        });



} ());
