/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.error.makeErrorObj = Csw.error.makeErrorObj ||
        Csw.error.register('makeErrorObj', function (errorType, friendlyMsg, esotericMsg) {
            'use strict';
            /// <summary>Generates a Csw Error object suitable for displaying a client-side error.</summary>
            /// <param name="errorType" type="Enum"> Error type: Error or Warning </param>
            /// <param name="friendlyMsg" type="String"> Friendly message. </param>
            /// <param name="esotericMsg" type="String"> (Optional) Error message with Developer context. </param>
            /// <returns type="Object"> The error object. </returns>
            return {
                type: Csw.string(errorType, Csw.enums.errorType.warning.name),
                message: Csw.string(friendlyMsg),
                detail: Csw.string(esotericMsg)
            };
        });

    Csw.error.showError = Csw.error.showError ||
        Csw.error.register('showError', function (errorJson, friendlyMsg, esotericMsg) {
            'use strict';
            /// <summary>Displays an error message.</summary>
            /// <param name="errorJson" type="Object/String"> An error object or a String for errorType. If object, should contain type, message and detail properties.</param>
            /// <param name="friendlyMsg" type="String"> A friendly message to display.</param>
            /// <param name="esotericMsg" type="String"> A verbose message for developers.</param>
            ///<returns type="Boolean">True</returns>
            var e = {
                'type': '',
                'message': friendlyMsg,
                'detail': esotericMsg,
                'display': true
            };
            if (Csw.isPlainObject(errorJson)) {
                Csw.extend(e, errorJson);
            } else {
                e.type = errorJson;
            }

            var $errorsdiv = $('#DialogErrorDiv');
            if ($errorsdiv.length <= 0) {
                $errorsdiv = $('#ErrorDiv');
            }

            if ($errorsdiv.length > 0 && Csw.bool(e.display)) {
                $errorsdiv.CswErrorMessage({ 'type': e.type, 'message': e.message, 'detail': e.detail });
            } 
            Csw.debug.error(e.message + '; ' + e.detail);
            return true;
        });

    Csw.error.errorHandler = Csw.error.errorHandler ||
        Csw.error.register('errorHandler', function (errorMsg, includeCallStack, includeLocalStorage, doAlert) {
            'use strict';
            if (Csw.hasWebStorage() && includeLocalStorage) {
                Csw.debug.log(window.localStorage);
            }
            if (doAlert) {
                $.CswDialog('ErrorDialog', errorMsg);
            } else {
                Csw.debug.error('Error: ' + errorMsg.message + ' (Code ' + errorMsg.code + ')', includeCallStack);
            }
        });

    Csw.error.exception = Csw.error.exception ||
        Csw.error.register('exception', function (message, name, fileName, lineNumber) {
            var ret = {
                message: message,
                name: name,
                fileName: fileName,
                lineNumber: lineNumber
            };
            return ret;
        });

    Csw.error.throwException = Csw.error.throwException ||
        Csw.error.register('throwException', function (exception) {
            Csw.debug.error(exception);
            throw exception;
        });

    Csw.error.catchException = Csw.error.catchException ||
        Csw.error.register('catchException', function (exception) {
            var e = {
                type: 'js',
                message: exception.message,
                detail: 'JS Error type: ' + exception.type + '<br/>\n' + 'Stack: ' + exception.stack,
                display: Csw.debug.showExceptions() === true
            };
            
            var _errs = ["4ff2f798551a4ccb08002430"]; 
            
                window.onerror = function () { _errs.push(arguments); };
                var d = function () {
                    var e = document.createElement("script"), c = document.getElementsByTagName("script")[0];
                    e.src = "//d15qhc0lu1ghnk.cloudfront.net/beacon.js"; e.async = !0;
                    c.parentNode.insertBefore(e, c);
                };
            window.addEventListener ? window.addEventListener("load", d, !1) :
            window.attachEvent("onload", d);
    
            
            
            Csw.error.showError(e);
        });


} ());
