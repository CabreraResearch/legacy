(function () {
    'use strict';

    Csw.composites.fileUpload = Csw.composites.fileUpload ||
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

                cswPrivate.uploadBtn = cswPrivate.uploadInp.$.fileupload({
                    dataType: cswPrivate.dataType,
                    url: cswPrivate.url,
                    paramName: 'fileupload',
                    forceIframeTransport: cswPrivate.forceIframeTransport,
                    send: function (e, data) {
                        cswPrivate.progressBar = cswPrivate.progressBar || window.Ext.create('Ext.ProgressBar', {
                            renderTo: cswPublic.p().getId(),
                            width: 300
                        });

                        cswPrivate.progressBar.wait();
                    },
                    done: function (e, jqXHR) {
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
                        $.each($(jqXHR.result).find('Status').children(), function(i, xmlnode) {
                           if (xmlnode.nodeName == "A:SUCCESS") {
                               succeeded = Csw.bool($(xmlnode).text());
                           }
                           if (xmlnode.nodeName == "A:ERRORS") {
                               var thiserr = {};
                               $.each($(xmlnode).children().first().children(), function(j, errchild) {
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
