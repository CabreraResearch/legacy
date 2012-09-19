
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    Csw.wizard.amountsGrid = Csw.wizard.amountsGrid ||
        Csw.wizard.register('amountsGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates an amounts thin grid with an Add form.</summary>
            var cswPublic = {
                rows: {
                    rowid: {
                        containerNoControl: {},
                        sizeControl: {},
                        qtyControl: {},
                        barcodeControl: {},
                        quantityValues: {}
                    }
                },
                quantities: function() {
                    var qtyVals = [];
                    Csw.each(cswPublic.rows, function (row) {
                        qtyVals.push(row.quantityValues);
                    });
                    return qtyVals;
                },
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
                        Csw.each(cswPublic.rows, function (row) {
                            if (false === Csw.isNullOrEmpty(row.quantityValues)) {
                                totalContainerQuantity += Csw.number(row.quantityValues.containerNo, 0);
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
                                cswPrivate.selectedSizeId = cswPublic.rows[rowid].sizeControl.selectedNodeId();
                                cswPublic.rows[rowid].quantityValues.sizeid = cswPublic.rows[rowid].sizeControl.selectedNodeId();
                                cswPublic.rows[rowid].quantityValues.sizename = cswPublic.rows[rowid].sizeControl.selectedText();
                            };
                            var updateColumnVals = function (changeContainerNo) {
                                if (false === Csw.isNullOrEmpty(cswPublic.rows[rowid].qtyControl)) {
                                    cswPublic.rows[rowid].quantityValues.quantity = cswPublic.rows[rowid].qtyControl.quantityValue;
                                    cswPublic.rows[rowid].quantityValues.unit = cswPublic.rows[rowid].qtyControl.unitText;
                                    cswPublic.rows[rowid].quantityValues.unitid = cswPublic.rows[rowid].qtyControl.unitVal;
                                }
                                if (changeContainerNo) {
                                    cswPublic.rows[rowid].containerNoControl.val(Csw.number(cswPrivate.quantity.unitCount, 1));
                                    cswPublic.rows[rowid].quantityValues.containerNo = Csw.number(cswPrivate.quantity.unitCount, 1);
                                    updateTotalContainerCount();
                                }
                                Csw.tryExec(cswPrivate.onChange, cswPublic.quantities());
                            };
                            var updateBarcodes = function (value) {
                                var parseBarcodes = function (anArray) {
                                    if (anArray.length > cswPublic.rows[rowid].quantityValues.containerNo) {
                                        anArray.splice(cswPublic.rows[rowid].quantityValues.containerNo, anArray.length - cswPublic.rows[rowid].quantityValues.containerNo);
                                    }
                                    value = anArray.join(',');
                                };
                                var barcodeToParse = Csw.delimitedString(Csw.string(value).trim()).array;
                                parseBarcodes(barcodeToParse);
                                cswPublic.rows[rowid].quantityValues.barcodes = value;
                            };
                            switch (columnName) {
                                case cswPrivate.config.numberName:
                                    cswPublic.rows[rowid].containerNoControl = cswCell.numberTextBox({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'containerCount' + rowid),
                                        name: Csw.tryExec(cswPrivate.makeId, 'containerCount' + rowid),
                                        value: 1,
                                        MinValue: cswPrivate.containerMinimum,
                                        MaxValue: cswPublic.containerlimit,
                                        width: (3 * 8) + 'px', //3 characters wide, 8 is the characters-to-pixels ratio
                                        Precision: 0,
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].quantityValues.containerNo = value;
                                            updateTotalContainerCount();
                                            if (false === Csw.isNullOrEmpty(cswPublic.rows[rowid].barcodeControl)) {
                                                updateBarcodes(cswPublic.rows[rowid].barcodeControl.val());
                                            }
                                            Csw.tryExec(cswPrivate.onChange, cswPublic.quantities());
                                        }
                                    });
                                    cswPublic.rows[rowid].quantityValues.containerNo = cswPublic.rows[rowid].containerNoControl.val();
                                    break;
                                case cswPrivate.config.sizeName:
                                    cswPublic.rows[rowid].sizeControl = cswCell.nodeSelect({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'sizes' + rowid),
                                        name: Csw.tryExec(cswPrivate.makeId, 'sizes' + rowid),
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
                                            cswPublic.rows[rowid].qtyControl.refresh(cswPrivate.quantity);
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
                                    cswPrivate.quantity.ID = Csw.tryExec(cswPrivate.makeId, 'containerQuantity' + rowid);
                                    cswPrivate.quantity.qtyWidth = (7 * 8) + 'px'; //7 characters wide, 8 is the characters-to-pixels ratio
                                    cswPublic.rows[rowid].qtyControl = cswCell.quantity(cswPrivate.quantity);
                                    updateColumnVals(true);
                                    break;
                                case cswPrivate.config.barcodeName:
                                    cswPublic.rows[rowid].barcodeControl = cswCell.textArea({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'containerBarcodes' + rowid),
                                        name: Csw.tryExec(cswPrivate.makeId, 'containerBarcodes' + rowid),
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
                            //Also, undefined quantity values break the serverside foreach loop, so an empty one is inserted in each element (including deleted elements).
                            while (cswPublic.rows.length < newRowid) {
                                cswPublic.rows[newRowid] = { quantityValues: {} };
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
                            cswPublic.rows[newRowid] = { quantityValues: extractNewAmount(newAmount) };
                        },
                        onDelete: function (rowid) {
                            delete cswPublic.rows[rowid];
                            cswPublic.rows[rowid] = { quantityValues: {} };
                            updateTotalContainerCount();
                            Csw.tryExec(cswPrivate.onChange, cswPublic.quantities());
                        }
                    });

                    cswParent.br();

                } ());

                (function _post() { } ());

            });

            return cswPublic;

        });
} ());

