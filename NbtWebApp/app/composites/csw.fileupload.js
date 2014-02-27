(function () {
    'use strict';

    Csw.composites.register('fileUpload', function (cswParent, cswPrivate) {

        //#region Variables

        var cswPublic = {};

        //#endregion Variables

        //#region Pre-ctor
        (function _pre() {
            cswPrivate.uploadUrl = cswPrivate.uploadUrl || '';
            cswPrivate.params = cswPrivate.params || {};
            cswPrivate.onSuccess = cswPrivate.onSuccess || function () { };
            cswPrivate.onError = cswPrivate.onError || function () { };

            //'forceIframeTransport' should always be false unless you want to upload a file and get the contents back via a WCF service - see StructureSearch dialog for more detail
            //if forceIframeTransport == true, the dataType must be 'iframe' or 'text' or there will not be a response
            cswPrivate.forceIframeTransport = cswPrivate.forceIframeTransport || false;
            cswPrivate.url = cswPrivate.url || cswPrivate.uploadUrl + '?' + Csw.params(cswPrivate.params);
            cswPrivate.dataType = cswPrivate.dataType || 'json';
            cswPrivate.paramName = cswPrivate.paramName || 'fileupload';
            cswParent.empty();
            cswPublic = cswParent.div();

        }());

        //#endregion Pre-ctor


        //#region Define Class Members


        //#endregion Define Class Members


        //#region Post-ctor

        (function _post() {

            cswPrivate.uploadInp = cswPublic.input({
                name: 'fileupload',
                type: Csw.enums.inputTypes.file
            });

            // Note: We send the sessionid in the querystring because IE9 doesn't support XHR
            cswPrivate.uploadBtn = cswPrivate.uploadInp.$.fileupload({
                dataType: cswPrivate.dataType,
                url: cswPrivate.url,
                paramName: cswPrivate.paramName,
                forceIframeTransport: cswPrivate.forceIframeTransport,
                beforeSend: function (jqXHR, settings) {
                    if (false === Csw.isNullOrEmpty(cswPrivate.params)) {
                        settings.url = cswPrivate.url +'&X-NBT-SessionId=' + Csw.cookie.get(Csw.cookie.cookieNames.SessionId);
                    }
                },
                send: function (e, data) {
                    cswPrivate.progressBar = cswPrivate.progressBar || window.Ext.create('Ext.ProgressBar', {
                        renderTo: cswPublic.p().getId(),
                        width: 300
                    });

                    cswPrivate.progressBar.wait();
                },
                headers: {
                    'X-NBT-SessionId': Csw.cookie.get(Csw.cookie.cookieNames.SessionId)
                },
                done: function (e, jqXHR) {
                    // We do this for any ajax response, so we need to do set it here too (Case 31888)
                    // Note: jqXHR has a 'headers' property
                    Csw.cookie.set(Csw.cookie.cookieNames.SessionId, jqXHR.headers['X-NBT-SessionId']);

                    cswPrivate.progressBar.reset();
                    cswPrivate.progressBar.updateText('Upload Complete');

                    // jqXHR.result is XML format, not JSON, because of iframe craziness.  So we can't use it like this.
                    var data = Csw.extend(jqXHR.result);
                    if (jqXHR.result && false === Csw.isNullOrEmpty(jqXHR.result.data)) {
                        Csw.extend(data, jqXHR.result.data);
                    }
                    if (Csw.isNullOrEmpty(data)) {
                        Csw.extend(data, jqXHR.data);
                    }

                    // COMMENCE KLUDGE to detect errors from return context
                    var succeeded;
                    var errors = [];
                    // Fatal server errors (like file too large)
                    var errortxt = $(jqXHR.result).find('span').children('h2').children('i').text();
                    if (errortxt === 'Maximum request length exceeded.' || errortxt === 'Runtime Error') {    // see case 30512
                        succeeded = false;
                        errors.push({
                            display: true,
                            type: 'Error',
                            message: 'Error: File exceeds maximum size',
                            detail: 'Server Error: Maximum request length exceeded.',
                        });
                    }

                    // Csw-handled errors
                    $.each($(jqXHR.result).find('Status').children(), function (i, xmlnode) {
                        if (xmlnode.nodeName == "A:SUCCESS") {
                            succeeded = Csw.bool($(xmlnode).text());
                        }
                        if (xmlnode.nodeName == "A:ERRORS") {
                            var thiserr = {};
                            $.each($(xmlnode).children().first().children(), function (j, errchild) {
                                if (errchild.nodeName == "A:SHOWERROR") thiserr.display = $(errchild).text();
                                if (errchild.nodeName == "A:TYPE") thiserr.type = $(errchild).text();
                                if (errchild.nodeName == "A:MESSAGE") thiserr.message = $(errchild).text();
                                if (errchild.nodeName == "A:DETAIL") thiserr.detail = $(errchild).text();
                            });
                            errors.push(thiserr);
                        }
                    });

                    if (false === succeeded) {
                        var lastErr = errors.length - 1;
                        Csw.error.showError(errors[lastErr], '');
                        Csw.tryExec(cswPrivate.onError, errors[lastErr]);
                    } else {
                        Csw.tryExec(cswPrivate.onSuccess, data); //use Csw.getPropFromIFrame to extract data (see ImageGallery for example)
                    }
                }
            });
        }());

        //#endregion Post-ctor

        return cswPublic;
    });


}());
