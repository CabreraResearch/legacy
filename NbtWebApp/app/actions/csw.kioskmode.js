
(function () {


    Csw.actions.register('kioskmode', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.kioskmode', 'kioskmode.js', 10);
        }
        (function _preCtor() {
            //set default values on cswPrivate if none are supplied
            cswPrivate.name = cswPrivate.name || 'Kiosk Mode';
            cswPrivate.onCancel = cswPrivate.onCancel || function _onCancel() { };
            cswPrivate.onInit = cswPrivate.onInit || function _onInit() { };

            cswPrivate.showValField2 = true;

            cswPrivate.availableModes = [];
            cswPrivate.log = [];
            cswPrivate.OperationData = {
                ScanTextLabel: 'Scan a barcode:',
                Mode: '',
                ModeStatusMsg: '',
                ModeServerValidated: false,
                Log: [],
                Field1: {
                    Name: '',
                    Value: '',
                    StatusMsg: '',
                    ServerValidated: false,
                    SecondValue: '',
                    FoundObjClass: '',
                    Active: false
                },
                Field2: {
                    Name: '',
                    Value: '',
                    StatusMsg: '',
                    ServerValidated: false,
                    SecondValue: '',
                    FoundObjClass: '',
                    Active: false
                },
                LastItemScanned: ''
            };

            cswParent.empty();
        }());

        cswPrivate.renderAvailableModes = function () {
            Csw.ajaxWcf.post({
                urlMethod: 'KioskMode/GetAvailableModes',
                success: function (data) {
                    var rowNum = 1;
                    cswPrivate.availableModes = data.AvailableModes;
                    Csw.each(data.AvailableModes, function (mode) {

                        var textCell = cswPrivate.barcodesTbl.cell(rowNum, 2).css({ 'vertical-align': 'middle', 'font-size': '135%', 'padding-right': '5px' });
                        //textCell.span({ text: mode.name });

                        var subtbl = textCell.table();
                        subtbl.cell(1, 1).css({ 'vertical-align': 'middle', 'font-size': '135%', 'padding-right': '5px' }).span({ text: mode.name });
                        subtbl.cell(2, 1).css({ 'vertical-align': 'middle', 'font-size': '70%', 'padding-right': '5px', 'color': '#C0C0C0' }).span({ text: mode.applies_to_types });


                        var imgCell = cswPrivate.barcodesTbl.cell(rowNum, 3).css({ 'padding-bottom': '10px' });
                        imgCell.img({
                            title: mode.name,
                            src: mode.imgUrl,
                            onClick: function (clickData) {
                                var clickedMode = $(this).context.attributes.title.nodeValue;
                                cswPrivate.handleItem(clickedMode);
                            }
                        });

                        rowNum = rowNum + 1;
                    });

                    cswPrivate.renderUI();
                }
            });
        };

        cswPrivate.handleItem = function (value) {
            cswPrivate.OperationData.LastItemScanned = value;
            if (value.toUpperCase() === 'RESET') {
                cswPrivate.clearOpData();
                cswPrivate.renderUI();
            } else {
                if (value.toUpperCase() === 'DISPOSE' && cswPrivate.isModeScan(value)) {
                    cswPrivate.showValField2 = false;
                } else if (cswPrivate.isModeScan(value)) {
                    cswPrivate.showValField2 = true;
                }

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
                        AvailableModes: cswPrivate.availableModes,
                        OperationData: cswPrivate.OperationData
                    },
                    success: function (KioskModeData) {
                        cswPrivate.addToLog(KioskModeData.OperationData.Log);
                        KioskModeData.OperationData.Log = [];
                        cswPrivate.OperationData = KioskModeData.OperationData;
                        cswPrivate.renderUI();
                        if (cswPrivate.readyToCommit()) {
                            cswPrivate.commitOperation();
                        }
                    }
                });
            }
        };

        cswPrivate.commitOperation = function () {
            Csw.ajaxWcf.post({
                urlMethod: 'KioskMode/CommitOperation',
                data: {
                    OperationData: cswPrivate.OperationData
                },
                success: function (KioskModeData) {
                    cswPrivate.addToLog(KioskModeData.OperationData.Log);
                    KioskModeData.OperationData.Log = [];
                    cswPrivate.OperationData = KioskModeData.OperationData;
                    cswPrivate.renderUI();
                }
            });
        };

        cswPrivate.clearOpData = function () {
            var Log = cswPrivate.OperationData.Log;
            cswPrivate.OperationData = {
                ScanTextLabel: 'Scan a barcode:',
                Mode: '',
                ModeStatusMsg: '',
                ModeServerValidated: false,
                Log: Log,
                Field1: {
                    Name: '',
                    Value: '',
                    StatusMsg: '',
                    ServerValidated: false,
                    SecondValue: '',
                    FoundObjClass: '',
                    Active: false
                },
                Field2: {
                    Name: '',
                    Value: '',
                    StatusMsg: '',
                    ServerValidated: false,
                    SecondValue: '',
                    FoundObjClass: '',
                    Active: false
                }
            };
        };

        cswPrivate.renderUI = function () {

            cswPrivate.operationTbl.cell(3, 1).empty();
            var propsTbl = cswPrivate.operationTbl.cell(3, 1).table({
                name: 'propstbl',
                cellpadding: 10
            });

            var modeCell = propsTbl.cell(1, 2).css('height', '25px');
            modeCell.span({ text: 'Mode: ' });
            var modeValCell = propsTbl.cell(1, 3).css({ 'width': '155px' });
            modeValCell.span({ text: cswPrivate.OperationData.Mode }).css({ 'font-size': '160%', 'font-weight': 'bold' });
            propsTbl.cell(1, 4).span({ text: cswPrivate.OperationData.ModeStatusMsg }).css('color', 'Red');

            var iconCell1 = propsTbl.cell(1, 1).css({ 'width': '10px' });
            var iconCell2 = propsTbl.cell(2, 1).css({ 'width': '10px' });
            var iconCell3 = propsTbl.cell(3, 1).css({ 'width': '10px' });

            var field1Cell = propsTbl.cell(2, 2).css({ 'height': '25px', 'width': '85px' });
            field1Cell.span({ text: cswPrivate.OperationData.Field1.Name });
            var field1Value1Cell = propsTbl.cell(2, 3).css({ 'width': '300px' }); //was 215px
            if (cswPrivate.showValField2) {
                field1Value1Cell.span({ text: cswPrivate.OperationData.Field1.Value + ' ' + cswPrivate.OperationData.Field1.SecondValue });
                propsTbl.cell(2, 4).span({ text: cswPrivate.OperationData.Field1.StatusMsg }).css('color', 'Red');
            }

            var field2Cell = propsTbl.cell(3, 2).css({ 'height': '25px', 'width': '85px' });
            field2Cell.span({ text: cswPrivate.OperationData.Field2.Name });
            var field2Value1Cell = propsTbl.cell(3, 3).css({ 'width': '155px' });

            if (false === Csw.isNullOrEmpty(cswPrivate.OperationData.Field1.Name) && (cswPrivate.OperationData.Field1.Active || false === Csw.isNullOrEmpty(cswPrivate.OperationData.Field1.StatusMsg))) {
                field1Value1Cell.css({ 'background-color': 'yellow' });
                field1Cell.css({ 'background-color': 'yellow' });
                iconCell2.icon({
                    iconType: Csw.enums.iconType.right,
                    isButton: false
                });
                iconCell2.css({ 'background-color': 'yellow' });
            } else if (false === Csw.isNullOrEmpty(cswPrivate.OperationData.Field2.Name) && (cswPrivate.OperationData.Field2.Active || false === Csw.isNullOrEmpty(cswPrivate.OperationData.Field2.StatusMsg))) {
                field2Value1Cell.css({ 'background-color': 'yellow' });
                field2Cell.css({ 'background-color': 'yellow' });
                iconCell3.icon({
                    iconType: Csw.enums.iconType.right,
                    isButton: false
                });
                iconCell3.css({ 'background-color': 'yellow' });
            } else if (Csw.isNullOrEmpty(cswPrivate.OperationData.Mode) || false === cswPrivate.OperationData.ModeServerValidated) {
                modeValCell.css({ 'background-color': 'yellow' });
                modeCell.css({ 'background-color': 'yellow' });
                iconCell1.icon({
                    iconType: Csw.enums.iconType.right,
                    isButton: false
                });
                iconCell1.css({ 'background-color': 'yellow' });
            }
            cswPrivate.scanArea.$.focus();
        };

        cswPrivate.readyToCommit = function () {
            if (cswPrivate.OperationData.Field1.ServerValidated && cswPrivate.OperationData.Field2.ServerValidated && cswPrivate.OperationData.ModeServerValidated) {
                return true;
            } else {
                return false;
            }
        };

        cswPrivate.isModeScan = function (scannedItem) {
            var Ret = false;
            if (false === Csw.isNullOrEmpty(cswPrivate.availableModes)) {
                Csw.each(cswPrivate.availableModes, function (mode) {
                    if (mode.name.toLowerCase() === scannedItem.toLowerCase()) {
                        Ret = true;
                    }
                });
            }
            return Ret;
        };

        cswPrivate.addToLog = function (LogData) {
            if (LogData.length > 0) {
                cswPrivate.log.push(LogData[LogData.length - 1]);
            }

            var logStr = '';
            Csw.each(cswPrivate.log, function (item) {
                logStr = item + '\n\n' + logStr;
            });

            cswPrivate.operationTbl.cell(4, 1).empty();
            cswPrivate.operationTbl.cell(4, 1).textArea({
                rows: 7,
                cols: 55,
                readonly: true,
                text: logStr
            }).css('margin-top', '10px');
        };

        cswPrivate.invalidateField = function (field) {
            field.Value = '';
            field.ServerValidated = false;
            field.FoundObjClass = '';
            field.StatusMsg = '';
            field.SecondValue = '';
            cswPrivate.scanArea.val('');
        };

        (function _postCtor() {

            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'Kiosk Mode',
                cancelText: 'Exit Kiosk Mode',
                onFinish: cswPrivate.onSubmitClick,
                onCancel: cswPrivate.onCancel
            });

            cswPrivate.action.finish.hide();

            cswPrivate.onInit();

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
                .css({ 'text-align': 'left', 'font-size': '255%', 'width': '55%', 'padding-bottom': '75px' })
                .span({ text: 'Kiosk Mode' });

            cswPrivate.renderAvailableModes();

            var barcodeCell = cswPrivate.operationTbl.cell(2, 1).css('padding-bottom', '20px');
            barcodeCell.span({ text: cswPrivate.OperationData.ScanTextLabel });

            cswPrivate.scanArea = barcodeCell.input({
                size: '30',
                autofocus: true,
                onKeyEnter: function () {
                    var scanned = cswPrivate.scanArea.val();
                    cswPrivate.scanArea.val('');
                    if (false === Csw.isNullOrEmpty(scanned)) {
                        if (cswPrivate.OperationData.Field1.ServerValidated && cswPrivate.showValField2) {
                            cswPrivate.invalidateField(cswPrivate.OperationData.Field2);
                        } else if (cswPrivate.OperationData.ModeServerValidated && false === cswPrivate.showValField2) {
                            cswPrivate.invalidateField(cswPrivate.OperationData.Field1);
                        }
                        cswPrivate.handleItem(scanned);
                    }
                }
            });

            cswPrivate.scanArea.$.blur(function () { //keep focus on scan area
                setTimeout(function () {
                    if (Csw.dialogsCount() == 0) {
                        cswPrivate.scanArea.$.focus();
                    }
                }, 5);
            });

        }());

        return cswPublic;
    });
}());