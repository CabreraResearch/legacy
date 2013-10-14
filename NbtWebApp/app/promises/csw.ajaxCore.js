/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    // Handle messages that don't interfere with the success of a method
    var handleAjaxMessage = function (messageJson) {
        Csw.message.showMessage(messageJson);
    };
    Csw.ajaxCore.register('handleMessage', handleAjaxMessage);
    
    // Errors
    var handleAjaxError = function (errorJson) {
        Csw.error.showError(errorJson);
    };
    Csw.ajaxCore.register('handleError', handleAjaxError);
    
    var onSuccess = function (url, data, saveToCache, successCallback, cachedResponse) {

        //
        if (data === cachedResponse || (null === data && saveToCache && cachedResponse)) {
            return Csw.tryExec(successCallback, cachedResponse);
        } else {
            if (saveToCache) {
                if (!cachedResponse || !Csw.compare(data, cachedResponse)) {
                    Csw.setCachedWebServiceCall(url, data);
                    return Csw.tryExec(successCallback, data);
                }
            } else {
                return Csw.tryExec(successCallback, data);
            }
        }
        return true;
    };
    Csw.ajaxCore.register('onSuccess', onSuccess);
   
} ());