
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
                        Log: [],
                        Field1: {
                            Name: '',
                            Value: '',
                            StatusMsg: '',
                            ServerValidated: false,
                            SecondValue: ''
                        },
                        Field2: {
                            Name: '',
                            Value: '',
                            StatusMsg: '',
                            ServerValidated: false,
                            SecondValue: ''
                        }
                    };

                    cswParent.empty();
                } ());

                cswPrivate.onSubmitClick = function () {
                    cswPrivate.commitOperation();
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

                                var textCell = cswPrivate.barcodesTbl.cell(rowNum, 2).css({ 'vertical-align': 'middle', 'font-size': '135%', 'padding-right': '5px' });
                                textCell.span({ text: mode.name });

                                var imgCell = cswPrivate.barcodesTbl.cell(rowNum, 3).css({ 'padding-bottom': '10px' });
                                imgCell.img({
                                    src: mode.imgUrl
                                });

                                rowNum = rowNum + 1;
                            });

                            cswPrivate.init();
                        }
                    });
                };

                cswPrivate.init = function () {
                    var barcodeCell = cswPrivate.operationTbl.cell(2, 1).css('padding-bottom', '20px');
                    barcodeCell.span({ text: 'Scan a barcode: ' });

                    cswPrivate.scanArea = barcodeCell.input({
                        size: '30',
                        autofocus: true,
                        onChange: function (value) {
                            if (value === 'Reset') {
                                cswPrivate.clearOpData();
                                cswPrivate.renderUI();
                            } else {
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
                                        cswPrivate.OperationData = KioskModeData.OperationData;
                                        cswPrivate.renderUI();
                                        if (cswPrivate.readyToCommit()) {
                                            cswPrivate.commitOperation();
                                        }
                                    }
                                });
                            }
                        }
                    });
                    cswPrivate.renderUI();
                };

                cswPrivate.commitOperation = function () {
                    cswPrivate.scanArea.disable();
                    Csw.ajaxWcf.post({
                        urlMethod: 'KioskMode/CommitOperation',
                        data: {
                            OperationData: cswPrivate.OperationData
                        },
                        success: function (KioskModeData) {
                            cswPrivate.scanArea.enable();
                            cswPrivate.OperationData = KioskModeData.OperationData;
                            cswPrivate.renderUI();
                        }
                    });
                };

                cswPrivate.clearOpData = function () {
                    var Log = cswPrivate.OperationData.Log;
                    cswPrivate.OperationData = {
                        Mode: '',
                        ModeStatusMsg: '',
                        ModeServerValidated: false,
                        Log: Log,
                        Field1: {
                            Name: '',
                            Value: '',
                            StatusMsg: '',
                            ServerValidated: false,
                            SecondValue: ''
                        },
                        Field2: {
                            Name: '',
                            Value: '',
                            StatusMsg: '',
                            ServerValidated: false,
                            SecondValue: ''
                        }
                    };
                };

                cswPrivate.renderUI = function () {
                    cswPrivate.scanArea.enable();

                    cswPrivate.operationTbl.cell(3, 1).empty();
                    cswPrivate.scanArea.val('');
                    var propsTbl = cswPrivate.operationTbl.cell(3, 1).table({
                        name: 'propstbl',
                        cellpadding: 10
                    });

                    var modeCell = propsTbl.cell(1, 1).css('height', '25px');
                    modeCell.span({ text: 'Mode: ' });
                    propsTbl.cell(1, 2).span({ text: cswPrivate.OperationData.Mode }).css({ 'font-size': '160%', 'font-weight': 'bold' });
                    propsTbl.cell(1, 3).span({ text: cswPrivate.OperationData.ModeStatusMsg }).css('color', 'Red');

                    var field1Cell = propsTbl.cell(2, 1).css('height', '25px');
                    field1Cell.span({ text: cswPrivate.OperationData.Field1.Name });
                    propsTbl.cell(2, 2).span({ text: cswPrivate.OperationData.Field1.Value });
                    propsTbl.cell(2, 3).span({ text: cswPrivate.OperationData.Field1.SecondValue });
                    propsTbl.cell(2, 4).span({ text: cswPrivate.OperationData.Field1.StatusMsg }).css('color', 'Red');

                    var field2Cell = propsTbl.cell(3, 1).css('height', '25px');
                    field2Cell.span({ text: cswPrivate.OperationData.Field2.Name });
                    propsTbl.cell(3, 2).span({ text: cswPrivate.OperationData.Field2.Value });
                    propsTbl.cell(3, 3).span({ text: cswPrivate.OperationData.Field2.SecondValue });
                    propsTbl.cell(3, 4).span({ text: cswPrivate.OperationData.Field2.StatusMsg }).css('color', 'Red');

                    var logStr = '';
                    Csw.each(cswPrivate.OperationData.Log, function (item) {
                        logStr = item + '\n\n' + logStr;
                    });

                    cswPrivate.operationTbl.cell(4, 1).empty();
                    cswPrivate.operationTbl.cell(4, 1).textArea({
                        rows: 10,
                        cols: 80,
                        readonly: true,
                        text: logStr
                    });
                    cswPrivate.scanArea.$.focus();
                };

                cswPrivate.readyToCommit = function () {
                    if (cswPrivate.OperationData.Field1.ServerValidated && cswPrivate.OperationData.Field2.ServerValidated && cswPrivate.OperationData.ModeServerValidated) {
                        return true;
                    } else {
                        return false;
                    }
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

                    cswPrivate.operationCell = cswPrivate.actionTbl.cell(1, 1).css({ 'width': '65%' });
                    cswPrivate.operationTbl = cswPrivate.operationCell.table({
                        name: cswPrivate.name + '_operation_tbl'
                    });
                    cswPrivate.barcodesCell = cswPrivate.actionTbl.cell(1, 2);
                    cswPrivate.barcodesTbl = cswPrivate.barcodesCell.table({
                        name: cswPrivate.name + '_barcodes_tbl'
                    });

                    cswPrivate.operationTbl.cell(1, 1)
                        .css({ 'text-align': 'left', 'font-size': '225%', 'width': '55%', 'padding-bottom': '75px' })
                        .span({ text: 'CISPro Kiosk Mode' });

                    cswPrivate.renderAvailableModes();
                } ());

                return cswPublic;
            });
} ());