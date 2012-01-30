/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';
    
    function makeErrorObj(errorType, friendlyMsg, esotericMsg) {
        /// <summary>Generates a Csw Error object suitable for displaying a client-side error.</summary>
        /// <param name="errorType" type="Enum"> Error type: Error or Warning </param>
        /// <param name="friendlyMsg" type="String"> Friendly message. </param>
        /// <param name="esotericMsg" type="String"> (Optional) Error message with Developer context. </param>
        /// <returns type="Object"> The error object. </returns>
        return {
            type: Csw.string(errorType, Csw.enums.ErrorType.warning.name),
            message: Csw.string(friendlyMsg),
            detail: Csw.string(esotericMsg)
        };
    }
    Csw.register('makeErrorObj', makeErrorObj);
    Csw.makeErrorObj = Csw.makeErrorObj || makeErrorObj;

    function error(errorJson) {
        /// <summary>Displays an error message.</summary>
        /// <param name="errorJson" type="Object"> An error object. Should contain type, message and detail properties.</param>
        /// <returns type="Boolean">True</returns>
        var e = {
            'type': '',
            'message': '',
            'detail': '',
            'display': true
        };
        if (errorJson) {
            $.extend(e, errorJson);
        }
        
        var $errorsdiv = $('#DialogErrorDiv');
        if ($errorsdiv.length <= 0) {
            $errorsdiv = $('#ErrorDiv');
        }

        if ($errorsdiv.length > 0 && Csw.bool(e.display)) {
            $errorsdiv.CswErrorMessage({'type': e.type, 'message': e.message, 'detail': e.detail});
        } else {
            log(e.message + '; ' + e.detail);
        }
        return true;
    } // CswError()
    Csw.register('error', error);
    Csw.error = Csw.error || error;

    function errorHandler(errorMsg, includeCallStack, includeLocalStorage, doAlert) {
        if (Csw.hasWebStorage() && includeLocalStorage) {
            log(localStorage);
        }
        if (doAlert) {
            $.CswDialog('ErrorDialog', errorMsg);
            //alert('Error: ' + error.message + ' (Code ' + error.code + ')');
        } else {
            log('Error: ' + errorMsg.message + ' (Code ' + errorMsg.code + ')', includeCallStack);
        }
    }
    Csw.register('errorHandler', errorHandler);
    Csw.errorHandler = Csw.errorHandler || errorHandler;

}());