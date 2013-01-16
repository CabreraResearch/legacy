
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

                    cswPrivate.OperationData = {
                        Mode: '',
                        ModeStatusMsg: '',
                        ModeServerValidated: false,
                        Field1: {
                            Name: '',
                            Value: '',
                            StatusMsg: '',
                            ServerValidated: false
                        },
                        Field2: {
                            Name: '',
                            Value: '',
                            StatusMsg: '',
                            ServerValidated: false
                        }
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

                cswPrivate.renderAvailableModes = function () {
                    Csw.ajaxWcf.post({
                        urlMethod: 'KioskMode/GetAvailableModes',
                        success: function (data) {
                            var rowNum = 1;
                            Csw.each(data.AvailableModes, function (mode) {
                                var imgCell = cswPrivate.actionTbl.cell(rowNum, 2).css({ 'width': '10%', 'padding-bottom': '10px' });
                                imgCell.img({
                                    src: 'http://upload.wikimedia.org/wikipedia/commons/f/fe/UPC_A.svg', //temp pic of barcode until I get real barcodes
                                    height: '20%',
                                    weight: '20%'
                                });

                                var textCell = cswPrivate.actionTbl.cell(rowNum, 3).css({ 'vertical-align': 'middle', 'font-size': '155%' });
                                textCell.span({ text: mode.name });

                                rowNum = rowNum + 1;
                            });
                            cswPrivate.init();
                        }
                    });
                };

                cswPrivate.init = function () {
                    var barcodeCell = cswPrivate.actionTbl.cell(2, 1);
                    barcodeCell.span({ text: 'Scan a barcode: ' });

                    cswPrivate.scanArea = barcodeCell.input({
                        size: '30',
                        autofocus: true,
                        onChange: function (value) {
                            cswPrivate.scanArea.disable();
                            if (false === cswPrivate.OperationData.ModeServerValidated) {
                                cswPrivate.OperationData.Mode = value;
                            } else if (false === cswPrivate.OperationData.Field1.ServerValidated) {
                                cswPrivate.OperationData.Field1.Value = value;
                            } else if (false === cswPrivate.OperationData.Field2.ServerValidated) {
                                cswPrivate.OperationData.Field2.Value = value;
                            }
                            Csw.ajaxWcf.post({
                                urlMethod: 'KioskMode/HandleScan',
                                data: {
                                    OperationData: cswPrivate.OperationData
                                },
                                success: function (KioskModeData) {
                                    cswPrivate.scanArea.enable();
                                    cswPrivate.OperationData = KioskModeData.OperationData;

                                    cswPrivate.actionTbl.cell(3, 1).empty();
                                    cswPrivate.scanArea.val('');
                                    var propsTbl = cswPrivate.actionTbl.cell(3, 1).table({
                                        name: 'propstbl',
                                        cellpadding: 10
                                    });
                                    propsTbl.cell(1, 1).span({ text: 'Mode: ' });
                                    propsTbl.cell(1, 2).span({ text: cswPrivate.OperationData.Mode });
                                    propsTbl.cell(1, 3).span({ text: cswPrivate.OperationData.ModeStatusMsg }).css('color', 'Red');

                                    propsTbl.cell(2, 1).span({ text: cswPrivate.OperationData.Field1.Name });
                                    propsTbl.cell(2, 2).span({ text: cswPrivate.OperationData.Field1.Value });
                                    propsTbl.cell(2, 3).span({ text: cswPrivate.OperationData.Field1.StatusMsg }).css('color', 'Red');

                                    propsTbl.cell(3, 1).span({ text: cswPrivate.OperationData.Field2.Name });
                                    propsTbl.cell(3, 2).span({ text: cswPrivate.OperationData.Field2.Value });
                                    propsTbl.cell(3, 3).span({ text: cswPrivate.OperationData.Field2.StatusMsg }).css('color', 'Red');
                                }
                            });
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
                        .css({ 'text-align': 'left', 'font-size': '225%', 'width': '80%'})
                        .span({ text: 'CISPro Kiosk Mode' });

                    cswPrivate.renderAvailableModes();

                } ());

                return cswPublic;
            });
} ());