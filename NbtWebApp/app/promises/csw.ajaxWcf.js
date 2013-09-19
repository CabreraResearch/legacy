/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var cswPrivate = {};

    cswPrivate.onJsonSuccess = Csw.method(function (o, data, url) {

        var response = {
            Data: '',
            Authentication: {
                AuthenticationStatus: 'Unknown',
                AuthenticationStatusText: '',
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
        Csw.extend(response, data, true);

        if (false === response.Status.Success ||
            response.Status.Errors.length > 0) {
            var lastErr = response.Status.Errors.length - 1;
            if (false === o.overrideError) {
                Csw.ajaxCore.handleError({
                    display: response.Status.Errors[lastErr].Display,
                    type: response.Status.Errors[lastErr].Type,
                    message: response.Status.Errors[lastErr].Message,
                    detail: response.Status.Errors[lastErr].Detail
                });

            }
            Csw.tryExec(o.error, response.Status.Errors[lastErr]);
        } else {

            var auth = Csw.string(response.Authentication.AuthenticationStatus, 'Unknown');
            var text = Csw.string(response.Authentication.AuthenticationStatusText);
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
                txt: text,
                success: function () {
                    Csw.ajaxCore.onSuccess(url, response.Data, o.useCache, o.success, o.cachedResponse);
                },
                failure: o.onloginfail,
                data: response.Authentication
            });
        }
    });

    cswPrivate.onJsonError = Csw.method(function (xmlHttpRequest, textStatus, param1, o) {
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
            complete: function () { },
            overrideError: false,
            watchGlobal: true,
            useCache: false,
            removeTimer: true
        };
        Csw.extend(cswInternal, options);

        cswInternal.urlMethod = 'Services/' + cswInternal.urlMethod;
        cswInternal.url = Csw.string(cswInternal.url, cswInternal.urlMethod);
        cswInternal.startTime = new Date();
        if (false === Csw.isNullOrEmpty(cswInternal.data)) {
            if (verb === 'GET') {
                cswInternal.data = Csw.params(cswInternal.data);
            }
            else {
                cswInternal.data = Csw.serialize(cswInternal.data);
            }
        }

        var getAjaxPromise = function (watchGlobal) {
            var ret = $.ajax({
                type: verb,
                url: cswInternal.urlMethod,
                xhrFields: {
                    withCredentials: true
                },
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                //processdata: false,
                data: cswInternal.data,
                watchGlobal: false !== watchGlobal
            });
            ret.done(function (data) {
                return cswPrivate.onJsonSuccess(cswInternal, data, cswInternal.urlMethod);
            }); /* success{} */
            ret.fail(function (jqXHR, textStatus, errorText) {
                return cswPrivate.onJsonError(jqXHR, textStatus, errorText, {
                    data: cswInternal.data,
                    watchGlobal: cswInternal.watchGlobal,
                    urlMethod: document.location + '/' + cswInternal.urlMethod
                });
            });
            ret.always(function (xmlHttpRequest, textStatus) {
                return Csw.tryExec(cswInternal.complete, xmlHttpRequest, textStatus);
            });
            return Csw.promises.ajax(ret);
        };

        var promise;
        if (true === cswInternal.useCache) {
            promise = Csw.getCachedWebServiceCall(cswInternal.urlMethod)
                .then(function (ret) {
                    cswInternal.cachedResponse = ret;
                    return Csw.ajaxCore.onSuccess(cswInternal.urlMethod, cswInternal.cachedResponse, false, cswInternal.success, cswInternal.cachedResponse);
                })
                .then(getAjaxPromise(false));
        } else {
            promise = getAjaxPromise(cswInternal.watchGlobal);
        }

        return promise;
    }); /* cswPrivate.jsonPost */


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



}());
