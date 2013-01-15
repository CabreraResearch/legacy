
(function () {

    Csw.actions.kioskmode = Csw.actions.kioskmode ||
            Csw.actions.register('kioskmode', function (cswParent, cswPrivate) {
                'use strict';
                var cswPublic = {};

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.kioskmode', 'kioskmode.js', 10);
                }
                (function _preCtor() {
                    //set default values on cswPrivate if none are supplied
                    cswPrivate.name = cswPrivate.name || 'Kiosk Mode';
                    cswPrivate.onSubmit = cswPrivate.onSubmit || function _onSubmit() {
                    };
                    cswPrivate.onCancel = cswPrivate.onCancel || function _onCancel() {
                    };

                    cswParent.empty();
                } ());

                cswPrivate.onSubmitClick = function () {
                    Csw.ajax.post({
                        urlMethod: '',
                        data: {},
                        success: function (json) {
                            if (json.succeeded) {
                                Csw.tryExec(cswPrivate.onSubmit);
                            }
                        }
                    });
                };

                cswPrivate.onCancelClick = function () {
                    Csw.ajax.post({
                        urlMethod: '',
                        data: {},
                        success: function (json) {
                            if (json.succeeded) {
                                Csw.tryExec(cswPrivate.onCancel);
                            }
                        }
                    });
                };

                (function _postCtor() {

                    cswPrivate.action = Csw.layouts.action(cswParent, {
                        title: 'CISPro Kiosk Mode',
                        finishText: 'Done',
                        onFinish: cswPrivate.onSubmitClick,
                        onCancel: cswPrivate.onCancelClick
                    });

                    cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                        name: cswPrivate.name + '_tbl',
                        align: 'center'
                    }).css('width', '95%');

                    cswPrivate.actionTbl.cell(1, 1)
                        .css({ 'text-align': 'left', 'font-size': '255%' })
                        .span({ text: 'CISPro Kiosk Mode' });

                } ());

                return cswPublic;
            });
} ());