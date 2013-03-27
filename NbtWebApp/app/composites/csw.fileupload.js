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

                cswPrivate.forceIframeTransport = cswPrivate.forceIframeTransport || false;
                cswPrivate.url = cswPrivate.url || Csw.enums.ajaxUrlPrefix + cswPrivate.uploadUrl + '?' + Csw.params(cswPrivate.params);
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
                        var data = Csw.extend(jqXHR.result);
                        if (jqXHR.result && false === Csw.isNullOrEmpty(jqXHR.result.data)) {
                            Csw.extend(data, jqXHR.result.data);
                        }
                        if (Csw.isNullOrEmpty(data)) {
                            Csw.extend(data, jqXHR.data);
                        }

                        Csw.tryExec(cswPrivate.onSuccess, data);
                    }
                });

                //cswPrivate.uploadBtn = cswPrivate.uploadInp.$.fileupload({
                //    dataType: cswPrivate.dataType,
                //    url: cswPrivate.url,
                //    paramName: 'fileupload',
                //    forceIframeTransport: cswPrivate.forceIframeTransport,
                //    send: function () {
                //        cswPrivate.progressBar = cswPrivate.progressBar || window.Ext.create('Ext.ProgressBar', {
                //            renderTo: cswPublic.p().getId(),
                //            width: 300
                //        });
                //
                //        cswPrivate.progressBar.wait();
                //    },
                //    done: function (e, jqXHR) {
                //        cswPrivate.progressBar.reset();
                //        cswPrivate.progressBar.updateText('Upload Complete');
                //        var data = Csw.extend(jqXHR.result);
                //        if (jqXHR.result && false === Csw.isNullOrEmpty(jqXHR.result.data)) {
                //            Csw.extend(data, jqXHR.result.data);
                //        }
                //        if (Csw.isNullOrEmpty(data)) {
                //            Csw.extend(data, jqXHR.data);
                //        }
                //
                //        Csw.tryExec(cswPrivate.onSuccess, data);
                //    }
                //});
            }());

            //#endregion Post-ctor

            return cswPublic;
        });


}());
