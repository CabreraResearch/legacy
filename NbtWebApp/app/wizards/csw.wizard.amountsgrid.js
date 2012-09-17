
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    Csw.wizard.amountsGrid = Csw.wizard.amountsGrid ||
        Csw.wizard.register('amountsGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates an amounts thin grid with an Add form.</summary>
            var cswPublic = {
                quantities: [],
                qtyControl: [],
                containerNoControl: [],
                barcodeControl: [],
                sizeControl: [],
                thinGrid: null,
                containerlimit: 25,
                containerCount: ''
            };

            Csw.tryExec(function () {

                var cswPrivate = {
                    ID: 'wizardAmountsThinGrid',
                    onAdd: null,
                    onDelete: null,
                    onChange: null,
                    quantity: {},
                    makeId: function (text) {
                        return text;
                    },
                    containerMinimum: 1,
                    action: 'Receive',
                    selectedSizeId: null,
                    relatedNodeId: null,
                    materialId: null,
                    rows: [],
                    config: {
                        numberName: 'No. Containers *',
                        sizeName: 'Size *',
                        quantityName: 'Net Quantity *',
                        barcodeName: 'Barcodes (Optional)'
                    },
                    customBarcodes: false
                };
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.header = [cswPrivate.config.numberName];
                if (false === Csw.isNullOrEmpty(cswPrivate.materialId) && cswPrivate.action === 'Receive') {
                    cswPrivate.header = cswPrivate.header.concat([cswPrivate.config.sizeName]);
                }
                cswPrivate.header = cswPrivate.header.concat([cswPrivate.config.quantityName]);
                if (cswPrivate.customBarcodes) {
                    cswPrivate.header = cswPrivate.header.concat([cswPrivate.config.barcodeName]);
                }

                if (cswPrivate.rows.length === 0) {
                    cswPrivate.rows.push(cswPrivate.header);
                } else {
                    var firstRow = cswPrivate.rows.splice(0, 1, cswPrivate.header);
                    cswPrivate.rows.push(firstRow);
                }

                cswPrivate.getQuantity = function () {
                    var ret = false;
                    if (Csw.isNullOrEmpty(cswPrivate.selectedSizeId) && false === Csw.isNullOrEmpty(cswPrivate.relatedNodeId)) {
                        Csw.ajax.post({
                            urlMethod: 'getSize',
                            async: false,
                            data: { RelatedNodeId: cswPrivate.relatedNodeId },
                            success: function (data) {
                                cswPrivate.selectedSizeId = data.sizeid;
                                ret = false === Csw.isNullOrEmpty(cswPrivate.selectedSizeId);
                            }
                        });
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.selectedSizeId)) {
                        Csw.ajax.post({
                            urlMethod: 'getQuantity',
                            async: false,
                            data: { SizeId: cswPrivate.selectedSizeId, Action: cswPrivate.action },
                            success: function (data) {
                                cswPrivate.quantity = data;
                                ret = false === Csw.isNullOrEmpty(cswPrivate.quantity);
                            }
                        });
                    }
                    if (false === ret) {
                        cswPrivate.quantity = {
                            qtyReadonly: true,
                            unitReadonly: true
                        }
                    }
                    return ret;
                };

                (function _pre() {
                    if (Csw.isNullOrEmpty(cswParent)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without a parent.', '', 'csw.wizard.amountsgrid.js', 22));
                    }
                    cswPublic.containerCount = 0;
                    cswParent.span({ text: '<b>Total number of containers: </b>' });
                    var containerNoSpan = cswParent.span({ text: cswPublic.containerCount });
                    var containerLimitExceededSpan = cswParent.span({ cssclass: 'CswErrorMessage_ValidatorError', text: ' The limit for containers created at receipt is [' + cswPublic.containerlimit + '].' });
                    containerLimitExceededSpan.hide();

                    cswParent.br({ number: 2 });

                    var getTotalContainerQuantity = function () {
                        var totalContainerQuantity = 0;
                        Csw.each(cswPublic.quantities, function (quantity) {
                            if (false === Csw.isNullOrEmpty(quantity)) {
                                totalContainerQuantity += Csw.number(quantity.containerNo, 0);
                            }
                        });
                        return totalContainerQuantity;
                    }

                    var updateTotalContainerCount = function () {
                        cswPublic.containerCount = getTotalContainerQuantity();
                        containerNoSpan.text(cswPublic.containerCount);
                        if (cswPublic.containerCount > cswPublic.containerlimit) {
                            containerLimitExceededSpan.show();
                        } else {
                            containerLimitExceededSpan.hide();
                        }
                    };

                    cswPublic.thinGrid = cswParent.thinGrid({
                        linkText: '',
                        hasHeader: true,
                        rows: cswPrivate.rows,
                        allowDelete: true,
                        allowAdd: true,
                        makeAddRow: function (cswCell, columnName, rowid) {
                            'use strict';
                            var updateSizeVals = function () {
                                cswPrivate.selectedSizeId = cswPublic.sizeControl[rowid].selectedNodeId();
                                cswPublic.quantities[rowid].sizeid = cswPublic.sizeControl[rowid].selectedNodeId();
                                cswPublic.quantities[rowid].sizename = cswPublic.sizeControl[rowid].selectedText();
                            };
                            var updateColumnVals = function (changeContainerNo) {
                                cswPublic.quantities[rowid].quantity = cswPublic.qtyControl[rowid].quantityValue;
                                cswPublic.quantities[rowid].unit = cswPublic.qtyControl[rowid].unitText;
                                cswPublic.quantities[rowid].unitid = cswPublic.qtyControl[rowid].unitVal;
                                if (changeContainerNo) {
                                    cswPublic.containerNoControl[rowid].val(Csw.number(cswPrivate.quantity.unitCount, 1));
                                    cswPublic.quantities[rowid].containerNo = Csw.number(cswPrivate.quantity.unitCount, 1);
                                }
                                Csw.tryExec(cswPrivate.onChange, cswPublic.quantities);
                            };
                            var updateBarcodes = function (value) {
                                var parseBarcodes = function (anArray) {
                                    if (anArray.length > cswPublic.quantities[rowid].containerNo) {
                                        anArray.splice(cswPublic.quantities[rowid].containerNo, anArray.length - cswPublic.quantities[rowid].containerNo);
                                    }
                                    value = anArray.join(',');
                                };
                                var barcodeToParse = Csw.delimitedString(Csw.string(value).trim()).array;
                                parseBarcodes(barcodeToParse);
                                cswPublic.quantities[rowid].barcodes = value;
                            };
                            switch (columnName) {
                                case cswPrivate.config.numberName:
                                    cswPublic.containerNoControl[rowid] = cswCell.numberTextBox({
                                        ID: Csw.tryExec(cswPrivate.makeId + rowid, 'containerCount'),
                                        name: Csw.tryExec(cswPrivate.makeId + rowid, 'containerCount'),
                                        value: 1,
                                        MinValue: cswPrivate.containerMinimum,
                                        MaxValue: cswPublic.containerlimit,
                                        width: (3 * 8) + 'px', //3 characters wide, 8 is the characters-to-pixels ratio
                                        Precision: 0,
                                        onChange: function (value) {
                                            cswPublic.quantities[rowid].containerNo = value;
                                            updateTotalContainerCount();
                                            if (false === Csw.isNullOrEmpty(cswPublic.barcodeControl[rowid])) {
                                                updateBarcodes(cswPublic.barcodeControl[rowid].val());
                                            }
                                            Csw.tryExec(cswPrivate.onChange, cswPublic.quantities);
                                        }
                                    });
                                    cswPublic.quantities[rowid].containerNo = cswPublic.containerNoControl[rowid].val();
                                    updateTotalContainerCount();
                                    break;
                                case cswPrivate.config.sizeName:
                                    cswPublic.sizeControl[rowid] = cswCell.nodeSelect({
                                        ID: Csw.tryExec(cswPrivate.makeId + rowid, 'sizes'),
                                        name: Csw.tryExec(cswPrivate.makeId + rowid, 'sizes'),
                                        async: false,
                                        objectClassName: 'SizeClass',
                                        addNodeDialogTitle: 'Size',
                                        relatedTo: {
                                            objectClassName: 'MaterialClass',
                                            nodeId: cswPrivate.materialId
                                        },
                                        onSelect: function () {
                                            updateSizeVals();
                                            cswPrivate.getQuantity();
                                            cswPublic.qtyControl[rowid].refresh(cswPrivate.quantity);
                                            updateColumnVals(true);
                                        },
                                        canAdd: true
                                    });
                                    updateSizeVals();
                                    break;
                                case cswPrivate.config.quantityName:
                                    cswPrivate.getQuantity();                                    
                                    cswPrivate.quantity.minvalue = 0;
                                    cswPrivate.quantity.isClosedSet = false;
                                    cswPrivate.quantity.onChange = function () {
                                        updateColumnVals(false);
                                    };
                                    cswPrivate.quantity.ID = Csw.tryExec(cswPrivate.makeId + rowid, 'containerQuantity');
                                    cswPrivate.quantity.qtyWidth = (7 * 8) + 'px'; //7 characters wide, 8 is the characters-to-pixels ratio
                                    cswPublic.qtyControl[rowid] = cswCell.quantity(cswPrivate.quantity);
                                    updateColumnVals(false);
                                    break;
                                case cswPrivate.config.barcodeName:
                                    cswPublic.barcodeControl[rowid] = cswCell.textArea({
                                        ID: Csw.tryExec(cswPrivate.makeId + rowid, 'containerBarcodes'),
                                        name: Csw.tryExec(cswPrivate.makeId + rowid, 'containerBarcodes'),
                                        rows: 1,
                                        cols: 14,
                                        onChange: function (value) {
                                            updateBarcodes(value);
                                        }
                                    });
                                    break;
                            }
                        },
                        onAdd: function (newRowid) {
                            var newAmount = {};
                            //This while loop serves as a buffer to remove the +1/-1 issues when comparing the data with the table cell rows in the thingrid.
                            //This puts the burden on the user of thingrid to ensure their data lines up with the table cells.
                            while (cswPublic.quantities.length < newRowid) {
                                cswPublic.quantities.push(newAmount);
                            }
                            newAmount = {
                                containerNo: '',
                                quantity: '',
                                sizeid: '',
                                sizename: '',
                                unit: '',
                                unitid: '',
                                barcodes: ''
                            };
                            var extractNewAmount = function (object) {
                                var ret = Csw.extend({}, object, true);
                                return ret;
                            };
                            cswPublic.quantities.push(extractNewAmount(newAmount));
                        },
                        onDelete: function (rowid) {
                            delete cswPublic.quantities[rowid];
                            cswPublic.quantities[rowid] = {};
                            delete cswPublic.qtyControl[rowid];
                            delete cswPublic.containerNoControl[rowid];
                            updateTotalContainerCount();
                            Csw.tryExec(cswPrivate.onChange, cswPublic.quantities);
                        }
                    });

                    cswParent.br();

                } ());

                (function _post() { } ());

            });

            return cswPublic;

        });
} ());

