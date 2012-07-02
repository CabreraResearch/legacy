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
                thinGrid: null
            };

            Csw.tryExec(function () {

                var cswPrivate = {
                    ID: 'wizardAmountsThinGrid',
                    onAdd: null,
                    quantity: {},
                    containerlimit: 25,
                    makeId: function (text) {
                        return text;
                    }
                };
                if (options) $.extend(cswPrivate, options);


                (function _pre() {
                    if (Csw.isNullOrEmpty(cswParent)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without a parent.', '', 'csw.wizard.amountsgrid.js', 22));
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.quantity)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Wizard amounts grid without the Capacity of a Size.', '', 'csw.wizard.amountsgrid.js', 34));
                    }
                    cswParent.span({ text: 'Enter the Amounts to Receive.' });
                    cswParent.br({ number: 2 });

                    cswParent.br();
                    cswPublic.amountForm = cswParent.form();
                    cswPrivate.amountsTable = cswPublic.amountForm.table();
                    cswPrivate.count = 0;
                    cswPublic.amountForm.br({ number: 2 });
                    cswPublic.thinGrid = cswPublic.amountForm.thinGrid({ linkText: '', hasHeader: true, rows: [['#', 'Quantity', 'Unit', 'Barcode(s)']] });
                    cswPublic.thinGrid.hide();
                } ());

                cswPrivate.makeAddAmount = function () {
                    'use strict';
                    cswPrivate.amountsTable.empty();

                    var thisAmount = {
                        containerNo: 1,
                        quantity: '',
                        unit: '',
                        unitid: '',
                        barcodes: ''
                    };
                    //# of containers
                    cswPublic.countControl = cswPrivate.amountsTable.cell(1, 1).numberTextBox({
                        ID: Csw.tryExec(cswPrivate.makeId, 'containerCount'),
                        labelText: 'Number: ',
                        value: thisAmount.containerNo,
                        MinValue: 1,
                        MaxValue: cswPrivate.containerlimit,
                        ceilingVal: cswPrivate.containerlimit,
                        Precision: 6,
                        Required: true,
                        onChange: function (value) {
                            thisAmount.containerNo = value;
                        }
                    });

                    //Quantity
                    cswPrivate.quantity.labelText = 'Quantity: ';
                    cswPrivate.quantity.ID = Csw.tryExec(cswPrivate.makeId, 'containerQuantity');
                    cswPublic.qtyControl = cswPrivate.amountsTable.cell(2, 1).quantity(cswPrivate.quantity);

                    //Barcodes
                    cswPublic.barcodeControl = cswPrivate.amountsTable.cell(3, 1).textArea({
                        ID: Csw.tryExec(cswPrivate.makeId, 'containerBarcodes'),
                        labelText: 'Barcodes: ',
                        onChange: function (value) {
                            thisAmount.barcodes = value;
                        }
                    });

                    //Add
                    cswPrivate.amountsTable.cell(4, 1).button({
                        ID: Csw.tryExec(cswPrivate.makeId, 'addBtn'),
                        enabledText: 'Add',
                        disableOnClick: false,
                        onClick: function () {

                            var newCount = cswPrivate.count + Csw.number(thisAmount.containerNo);
                            if (newCount <= cswPrivate.containerlimit) {
                                cswPrivate.count = newCount;
                                if (cswPrivate.count > 0) {
                                    cswPublic.thinGrid.show();
                                }
                                var parseBarcodes = function (anArray) {
                                    if (anArray.length > thisAmount.containerNo) {
                                        anArray.splice(0, anArray.length - thisAmount.containerNo);
                                    }
                                    thisAmount.barcodes = barcodeToParse.join(',');
                                };
                                var barcodeToParse = Csw.delimitedString(thisAmount.barcodes).array;
                                parseBarcodes(barcodeToParse);

                                if (cswPublic.amountForm.isFormValid()) {
                                    thisAmount.quantity = cswPublic.qtyControl.quantityValue;
                                    thisAmount.unit = cswPublic.qtyControl.unitText;
                                    thisAmount.unitid = cswPublic.qtyControl.unitVal;
                                    cswPublic.thinGrid.addRows([thisAmount.containerNo, thisAmount.quantity, thisAmount.unit, thisAmount.barcodes]);
                                    cswPublic.quantities.push(thisAmount);
                                    Csw.tryExec(cswPrivate.onAdd);
                                    cswPrivate.makeAddAmount();
                                }
                            } else {
                                $.CswDialog('AlertDialog', 'The limit for containers created at receipt is [' + cswPrivate.containerlimit + ']. You have already added [' + cswPrivate.count + '] containers.', 'Cannot add [' + newCount + '] containers.');
                            }
                        }
                    });
                };

                (function _post() {
                    cswPrivate.makeAddAmount();
                } ());

            });

            return cswPublic;

        });
} ());

