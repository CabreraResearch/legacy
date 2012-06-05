/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.submitRequest = Csw.actions.submitRequest ||
        Csw.actions.register('submitRequest', function (cswParent, options) {
            'use strict';
            var cswPublic = {};
            var cswPrivate = {};
            try {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 14);
                }
                cswPrivate.urlMethod = '';
                cswPrivate.ID = 'CswSubmitRequest';

                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswParent.empty();
                cswParent.div({ text: 'Submit Request' });

                Csw.ajax.post({
                    urlMethod: cswPrivate.urlMethod,
                    data: {},
                    success: function (data) {

                        

                    }, // success
                    error: function () {
                    }
                });
            }
            catch (exception) {
                Csw.catchException(exception);
            }
            return cswPublic;
        });
} ());

