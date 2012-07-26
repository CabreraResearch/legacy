/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {
    Csw.nbt.wizard.amountsGrid = Csw.nbt.wizard.amountsGrid ||
        Csw.nbt.wizard.register('amountsGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates an amounts thin grid with an Add form.</summary>
            var cswPublic = {
                quantities: [],
                countControl: null,
                qtyControl: null,
                barcodeControl: null,
                amountForm: null,
                thinGrid: null,
                selectedSizeId: null
            };

            Csw.tryExec(function () {

                var cswPrivate = {
                    ID: 'wizardAmountsThinGrid',
                    onAdd: null,
                    onDelete: null, //function(quantities.length)
                    quantity: {},
                    containerlimit: 25,
                    makeId: function (text) {
                        return text;
                    },
                    containerMinimum: 1,
                    action: 'Receive',
                    selectedSizeId: null,
                    relatedNodeId: null,
                    rows: [],
                    config: {
                        barcodeName: 'Barcodes (Optional)',
                        quantityName: 'Quantity *',
                        numberName: 'No. *'
                    }
                };
                if (options) {
                    $.extend(cswPrivate, options);
                }
                cswPrivate.header = [cswPrivate.config.numberName, cswPrivate.config.quantityName, cswPrivate.config.barcodeName];
                if(cswPrivate.rows.length === 0) {
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
                            data: { SizeId: cswPrivate.selectedSizeId },
                            success: function(data) {
                                cswPrivate.quantity = data;
                                ret = false === Csw.isNullOrEmpty(cswPrivate.quantity);
                            }
                        });
                    }
                    if(false === ret) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without the Initial Quantity of a Size.', '', 'csw.wizard.amountsgrid.js', 68));
                    }
                    return ret;
                };

                (function _pre() {
                    if (Csw.isNullOrEmpty(cswParent)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without a parent.', '', 'csw.wizard.amountsgrid.js', 22));
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.quantity)) {
                        cswPrivate.getQuantity();
                    }

                    cswPrivate.count = 0;

                    cswParent.span({ text: 'Enter the Amounts to ' + cswPrivate.action + ':' });
                    cswParent.br({ number: 1 });

                    //This object will be mutated on each call to makeAddRow below, but the reference to the object is constant--so we can pass it to onAdd.
                    //There is a fragility here in that if you were to pass newAmount to an external function, the values of the properties of the object would be unknowable
                    //This is a traditional MVC problem which would easily by templating libraries (Spine, Ember, Backbone, Knockout, etc);
                    //however, as these two modules are so tightly coupled, the fragility should be an acceptable risk for now.
                    var newAmount = {
                        rowid: 1,
                        containerNo: 1,
                        quantity: '',
                        unit: '',
                        unitid: '',
                        barcodes: ''
                    };

                    var extendNewAmount = function (object) {
                        //To mitigate the risk of unknowingly passing the outer scope thisAmount, we're explicitly mapping the values down
                        $.extend(newAmount, object);
                    };

                    var extractNewAmount = function(object) {
                        var ret = $.extend(true, {}, object);
                        return ret;
                    };

                    cswPublic.amountForm = cswParent.form();
                    cswPublic.thinGrid = cswPublic.amountForm.thinGrid({
                        linkText: '',
                        hasHeader: true, 
                        rows: cswPrivate.rows,
                        allowDelete: true,
                        allowAdd: true,
                        makeAddRow: function (cswCell, columnName, rowid) {
                            'use strict';
                            var thisAmount = {
                                rowid: rowid,
                                containerNo: 1,
                                quantity: '',
                                unit: '',
                                unitid: '',
                                barcodes: ''
                            };
                            
                            switch (columnName) {
                                case cswPrivate.config.numberName:
                                    cswPublic.countControl = cswCell.numberTextBox({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'containerCount'),
                                        value: thisAmount.containerNo,
                                        MinValue: cswPrivate.containerMinimum,
                                        MaxValue: cswPrivate.containerlimit,
                                        ceilingVal: cswPrivate.containerlimit,
                                        Precision: 0,
                                        Required: true,
                                        onChange: function (value) {
                                            thisAmount.containerNo = value;
                                            extendNewAmount(thisAmount);
                                        }
                                    });
                                    break;
                                case cswPrivate.config.quantityName:
                                    cswPrivate.quantity.ID = Csw.tryExec(cswPrivate.makeId, 'containerQuantity');
                                    cswPrivate.quantity.qtyWidth = '40px';
                                    cswPublic.qtyControl = cswCell.quantity(cswPrivate.quantity);
                                    break;
                                case cswPrivate.config.barcodeName:
                                    cswPublic.barcodeControl = cswCell.input({
                                        ID: Csw.tryExec(cswPrivate.makeId, 'containerBarcodes'),
                                        onChange: function (value) {
                                            thisAmount.barcodes = value;
                                            extendNewAmount(thisAmount);
                                        }
                                    });
                                    break;
                            }
                            extendNewAmount(thisAmount);
                        },
                        onAdd: function () {
                            var newCount = cswPrivate.count + Csw.number(newAmount.containerNo);
                            if (newCount <= cswPrivate.containerlimit) {
                                cswPrivate.count = newCount;

                                var parseBarcodes = function(anArray) {
                                    if (anArray.length > newAmount.containerNo) {
                                        anArray.splice(0, anArray.length - newAmount.containerNo);
                                    }
                                    newAmount.barcodes = barcodeToParse.join(',');
                                };
                                var barcodeToParse = Csw.delimitedString(newAmount.barcodes).array;
                                parseBarcodes(barcodeToParse);

                                if (cswPublic.amountForm.isFormValid()) {
                                    newAmount.quantity = cswPublic.qtyControl.quantityValue;
                                    newAmount.unit = cswPublic.qtyControl.unitText;
                                    newAmount.unitid = cswPublic.qtyControl.unitVal;
                                    newAmount.rowid = cswPublic.thinGrid.addRows([newAmount.containerNo, newAmount.quantity + ' ' + newAmount.unit, newAmount.barcodes]);
                                    cswPublic.quantities.push(extractNewAmount(newAmount));
                                }
                            } else {
                                $.CswDialog('AlertDialog', 'The limit for containers created at receipt is [' + cswPrivate.containerlimit + ']. You have already added [' + cswPrivate.count + '] containers.', 'Cannot add [' + newCount + '] containers.');
                            }
                            Csw.tryExec(cswPrivate.onAdd, (cswPrivate.count > 0));
                        },
                        onDelete: function (rowid) {
                            Csw.debug.assert(false === Csw.isNullOrEmpty(rowid), 'Rowid is null.');
                            var quantityToRemove = cswPublic.quantities.filter(function (quantity, index, array) { return quantity.rowid === rowid; });
                            if (false === Csw.isNullOrEmpty(quantityToRemove, true)) {
                                cswPrivate.count -= Csw.number(quantityToRemove[0].containerNo,0);
                                var reducedQuantities = cswPublic.quantities.filter(function(quantity, index, array) { return quantity.rowid !== rowid; });
                                Csw.debug.assert(reducedQuantities !== cswPublic.quantities, 'Rowid is null.');
                                cswPublic.quantities = reducedQuantities;
                                Csw.tryExec(cswPrivate.onDelete, (cswPrivate.count > 0));
                            }
                        }
                        });
                    } ());
                
                    (function _post() {
                
                    } ());

                });

                return cswPublic;

            });
        } ());

