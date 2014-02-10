/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('fileUpload', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Upload';
            cswPrivate.height = cswPrivate.height || 300;
            cswPrivate.width = cswPrivate.width || 400;
            cswPrivate.urlMethod = cswPrivate.urlMethod || 'fileForProp';
            cswPrivate.url = cswPrivate.url || '';
            cswPrivate.dataType = cswPrivate.dataType || '';
            cswPrivate.forceIframeTransport = cswPrivate.forceIframeTransport || '';
            cswPrivate.params = cswPrivate.params || '';
            cswPrivate.onOpen = cswPrivate.onOpen || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
            cswPrivate.onSuccess = cswPrivate.onSuccess || function () { };
        }());

        return (function () {
            'use strict';
            var fileUploadDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: cswPrivate.width,
                height: cswPrivate.height,
                onOpen: cswPrivate.onOpen,
                onClose: cswPrivate.onClose
            });

            fileUploadDialog.div.fileUpload({
                uploadUrl: cswPrivate.urlMethod,
                url: cswPrivate.url,
                dataType: cswPrivate.dataType,
                forceIframeTransport: cswPrivate.forceIframeTransport,
                params: cswPrivate.params,
                onSuccess: function (data) {
                    fileUploadDialog.close();
                    Csw.tryExec(cswPrivate.onSuccess, data);
                }
            });

            fileUploadDialog.div.button({
                name: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    fileUploadDialog.close();
                }
            });

            return fileUploadDialog;
        }());
    });
}());