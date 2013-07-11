
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    var cswDispenseWizardStateName = 'cswDispenseWizardStateName';

    Csw.nbt.dispenseContainerWizard = Csw.nbt.dispenseContainerWizard ||
        Csw.nbt.register('dispenseContainerWizard', function (cswParent, options) {
            'use strict';

            var cswPublic = {};

            Csw.tryExec(function () {

                //#region Variable Declaration
                var cswPrivate = {
                    name: 'cswDispenseContainerWizard',
                    state: {
                        sourceContainerNodeId: '',
                        currentQuantity: '',
                        currentUnitName: '',
                        precision: 6,
                        quantityAfterDispense: '',
                        initialQuantity: '',
                        dispenseType: 'Dispense into a Child Container',
                        quantity: '',
                        unitId: '',
                        sizeId: '',
                        materialId: '',
                        containerNodeTypeId: '',
                        materialname: '',
                        barcode: '',
                        location: '',
                        requestItemId: '',
                        dispenseMode: '',
                        customBarcodes: false,
                        netQuantityEnforced: true
                    },
                    onCancel: null,
                    onFinish: null,
                    amountsGrid: null,
                    startingStep: 1,
                    wizard: '',
                    wizardSteps: {
                        1: 'Select a Dispense Type',
                        2: 'Select Amount(s)'
                    },
                    stepOneComplete: false,
                    stepTwoComplete: false,
                    buttons: {
                        next: 'next',
                        prev: 'previous',
                        finish: 'finish',
                        cancel: 'cancel'
                    },
                    dispenseTypes: {
                        Dispense: 'Dispense into a Child Container',
                        Use: 'Dispense for Use',
                        Add: 'Add Material to Container',
                        Waste: 'Waste Material'
                    },
                    divStep1: '',
                    divStep2: '',
                    quantityControl: null,
                    updateQuantityAfterDispense: null,
                    quantityAfterDispenseSpan: null,
                    netQuantityExceededSpan: null,
                    title: 'Dispense from Container',
                    dispenseModes: {
                        Direct: 'Direct',
                        RequestMaterial: 'RequestMaterial',
                        RequestContainer: 'RequestContainer'
                    },
                    printBarcodes: false,
                    formIsValid: false,
                    roundToPrecision: null,
                    getTotalQuantityToDispense: null
                };

                cswPrivate.setDispenseMode = function () {
                    cswPrivate.state.dispenseType = cswPrivate.dispenseTypes.Dispense;
                    if (false === Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                        if (Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId)) {
                            cswPrivate.state.dispenseMode = cswPrivate.dispenseModes.RequestMaterial;
                            cswPrivate.wizardSteps["1"] = 'Select a Container';
                        } else {
                            cswPrivate.state.dispenseMode = cswPrivate.dispenseModes.RequestContainer;
                        }
                    } else {
                        cswPrivate.state.dispenseMode = cswPrivate.dispenseModes.Direct;
                    }
                };

                cswPrivate.validateState = function () {
                    var state;
                    if (Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId) && Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                        state = cswPrivate.getState();
                        Csw.extend(cswPrivate.state, state);
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.state.dispenseMode)) {
                        cswPrivate.setDispenseMode();
                    } else if (cswPrivate.state.dispenseMode === cswPrivate.dispenseModes.RequestMaterial) {
                        cswPrivate.wizardSteps["1"] = 'Select a Container';
                    }
                    cswPrivate.setState();
                };

                cswPrivate.getState = function () {
                    var ret = Csw.clientDb.getItem(cswPrivate.name + '_' + cswDispenseWizardStateName);
                    if (false === Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                        ret.sourceContainerNodeId = null;
                        ret.barcode = null;
                        ret.location = null;
                    }
                    return ret;
                };

                cswPrivate.setState = function () {
                    Csw.clientDb.setItem(cswPrivate.name + '_' + cswDispenseWizardStateName, cswPrivate.state);
                };

                cswPrivate.clearState = function () {
                    Csw.clientDb.removeItem(cswPrivate.name + '_' + cswDispenseWizardStateName);
                };

                (function _pre() {
                    if (options) {
                        Csw.extend(cswPrivate, options);
                    }
                    cswPrivate.validateState();
                    cswPublic = cswParent.div();
                    cswPrivate.currentStepNo = cswPrivate.startingStep;
                } ());

                cswPrivate.toggleButton = function (button, isEnabled, doClick) {
                    var btn;
                    if (Csw.bool(isEnabled)) {
                        btn = cswPrivate.wizard[button].enable();
                        if (Csw.bool(doClick)) {
                            btn.click();
                        }
                    } else {
                        cswPrivate.wizard[button].disable();
                    }
                };

                //Step 1. Select a Dispense Type.
                cswPrivate.makeStepOne = (function () {
                    return function () {

                        var dispenseTypeTable;

                        var toggleNext = function () {
                            return cswPrivate.toggleButton(cswPrivate.buttons.next, false === Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId));
                        };
                        var resetStepTwo = function () {
                            cswPrivate.stepTwoComplete = false;
                            cswPrivate.state.quantity = [];
                            cswPrivate.setState();
                        };
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                        var initStepOne = Csw.method(function () {
                            var makeTypeSelect = function () {

                                if (cswPrivate.state.dispenseMode !== cswPrivate.dispenseModes.RequestMaterial) {
                                    cswPrivate.divStep1.br({ number: 2 });
                                    cswPrivate.divStep1.span({ text: 'Pick a type of dispense:' });
                                    cswPrivate.divStep1.br({ number: 1 });

                                    var dispenseTypesArray = [
                                        cswPrivate.dispenseTypes.Dispense,
                                        cswPrivate.dispenseTypes.Use,
                                        cswPrivate.dispenseTypes.Add,
                                        cswPrivate.dispenseTypes.Waste
                                    ];

                                    cswPrivate.state.dispenseType = cswPrivate.dispenseTypes.Dispense;

                                    var dispenseTypeRadioGroup = cswPrivate.divStep1.radiobutton({
                                        name: 'dispensetypes',
                                        names: dispenseTypesArray,
                                        onChange: function () {
                                            if (false === Csw.isNullOrEmpty(dispenseTypeRadioGroup.val())) {
                                                if (dispenseTypeRadioGroup.val() !== cswPrivate.state.dispenseType) {
                                                    resetStepTwo();
                                                }
                                                cswPrivate.state.dispenseType = dispenseTypeRadioGroup.val();
                                            }
                                        },
                                        checkedIndex: 0
                                    });
                                }
                            };

                            var makeContainerGrid = function () {
                                Csw.ajax.post({
                                    urlMethod: 'getDispenseContainerView',
                                    data: {
                                        RequestItemId: cswPrivate.state.requestItemId
                                    },
                                    success: function (data) {
                                        if (Csw.isNullOrEmpty(data.viewid)) {
                                            Csw.error.throwException(Csw.error.exception('Could not get a grid of containers for this request item.', '', 'csw.dispensecontainer.js', 141));
                                        }

                                        cswPrivate.containerGrid = Csw.wizard.nodeGrid(cswPrivate.divStep1, {
                                            hasMenu: false,
                                            viewid: data.viewid,
                                            ReadOnly: true,
                                            onSelect: function () {
                                                if (cswPrivate.state.sourceContainerNodeId !== cswPrivate.containerGrid.getSelectedNodeId()) {
                                                    resetStepTwo();
                                                }
                                                cswPrivate.state.sourceContainerNodeId = cswPrivate.containerGrid.getSelectedNodeId();
                                                toggleNext();
                                            },
                                            onSuccess: makeTypeSelect
                                        });
                                    }
                                });
                            };

                            cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                            cswPrivate.divStep1.empty();

                            var helpText = 'Confirm the container to use for this dispense';
                            if (cswPrivate.state.dispenseMode !== cswPrivate.dispenseModes.RequestMaterial) {
                                helpText += ', and select a type of dispense to perform';
                            }
                            helpText += '.';
                            cswPrivate.divStep1.span({ text: helpText });

                            cswPrivate.divStep1.br({ number: 2 });

                            dispenseTypeTable = cswPrivate.divStep1.table({
                                name: 'setDispenseTypeTable',
                                width: '45%',
                                cellpadding: '1px',
                                cellalign: 'left',
                                cellvalign: 'middle',
                                FirstCellRightAlign: true
                            });

                            if (cswPrivate.state.dispenseMode !== cswPrivate.dispenseModes.RequestMaterial) {
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.barcode)) {
                                    dispenseTypeTable.cell(1, 1).span().setLabelText('Barcode: ');
                                    dispenseTypeTable.cell(1, 2).span({ text: Csw.string(cswPrivate.state.barcode) });
                                    
                                }
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.materialname)) {
                                    dispenseTypeTable.cell(2, 1).span().setLabelText('Material: ');
                                    dispenseTypeTable.cell(2, 2).span({
                                        text: Csw.string(cswPrivate.state.materialname)
                                    });
                                }
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.location)) {
                                    dispenseTypeTable.cell(3, 1).span().setLabelText('Location: ');
                                    dispenseTypeTable.cell(3, 2).span({
                                        text: Csw.string(cswPrivate.state.location)
                                    });
                                }
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.currentQuantity)) {
                                    dispenseTypeTable.cell(4, 1).span().setLabelText('Current Quantity: ');
                                    dispenseTypeTable.cell(4, 2).span({
                                        text: cswPrivate.state.currentQuantity + ' ' + cswPrivate.state.currentUnitName
                                    });
                                }
                            }

                            if (cswPrivate.state.dispenseMode === cswPrivate.dispenseModes.RequestMaterial) {
                                makeContainerGrid();
                            } else if (Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId)) {
                                Csw.error.throwException(Csw.error.exception('Cannot dispense without a source container.', '', 'csw.dispensecontainer.js', 173));
                            } else {
                                makeTypeSelect();
                            }

                        });

                        if (false === cswPrivate.stepOneComplete) {
                            initStepOne();
                            toggleNext();
                            cswPrivate.stepOneComplete = true;
                        }
                    };
                } ());

                //Step 2. Select Amount
                //state.dispenseType != Dispense ? 
                //Select a state.quantity :
                //Select the number of destination containers and their quantities.
                cswPrivate.makeStepTwo = (function () {
                    return function () {
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                        if (false === cswPrivate.stepTwoComplete) {
                            var quantityTable,
                                qtyTableRow = 1,
                                blankText = '[Select One]';

                            cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                            cswPrivate.divStep2.empty();
                            cswPrivate.divStep2.span({ text: 'Confirm the source container and define the amounts to dispense:' });
                            cswPrivate.divStep2.br();

                            quantityTable = cswPrivate.divStep2.table({
                                width: '45%',
                                name: 'setQuantityTable',
                                cellpadding: '1px',
                                cellvalign: 'middle'
                            });

                            quantityTable.cell(qtyTableRow, 1).br();
                            qtyTableRow += 1;
                            var propTbl = quantityTable.cell(qtyTableRow, 1).table({
                                FirstCellRightAlign: true
                            });
                            qtyTableRow += 1;
                            var propRow = 1;
                            
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.barcode)) {
                                propTbl.cell(propRow, 1).span().setLabelText('Barcode: ');
                                propTbl.cell(propRow, 2).span({ text: Csw.string(cswPrivate.state.barcode) });
                                propRow += 1;
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.materialname)) {
                                propTbl.cell(propRow, 1).span().setLabelText('Material: ');
                                propTbl.cell(propRow, 2).span({ text: Csw.string(cswPrivate.state.materialname) });
                                propRow += 1;
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.location)) {
                                propTbl.cell(propRow, 1).span().setLabelText('Location: ');
                                propTbl.cell(propRow, 2).span({ text: Csw.string(cswPrivate.state.location) });
                                propRow += 1;
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.currentQuantity)) {
                                propTbl.cell(propRow, 1).span().setLabelText('Current Quantity: ');
                                propTbl.cell(propRow, 2).span({ text: cswPrivate.state.currentQuantity + ' ' + cswPrivate.state.currentUnitName });
                                cswPrivate.state.quantityAfterDispense = cswPrivate.state.currentQuantity;
                                propTbl.cell(propRow, 3).span().setLabelText('After Dispense: ');
                                cswPrivate.quantityAfterDispenseSpan = propTbl.cell(propRow, 4).span({ text: cswPrivate.state.quantityAfterDispense + ' ' + cswPrivate.state.currentUnitName });
                                cswPrivate.netQuantityExceededSpan = propTbl.cell(propRow, 2).span({ cssclass: 'CswErrorMessage_ValidatorError', text: ' Total quantity to dispense cannot exceed source container\'s net quantity.' });
                                cswPrivate.netQuantityExceededSpan.hide();
                                propRow += 1;
                            }

                            var makeContainerSelect = function () {
                                var containerTypeTable = quantityTable.cell(qtyTableRow, 1).table({
                                    name: 'setContainerTypeTable',
                                    cellpadding: '1px',
                                    cellvalign: 'middle'
                                }).hide();
                                qtyTableRow++;

                                containerTypeTable.cell(1, 1).span({ text: 'Select a Container Type' });

                                var containerTypeSelect = containerTypeTable.cell(2, 1).nodeTypeSelect({
                                    name: 'nodeTypeSelect',
                                    objectClassName: 'ContainerClass',
                                    blankOptionText: blankText,
                                    onSelect: function () {
                                        if (blankText !== containerTypeSelect.val()) {
                                            cswPrivate.state.containerNodeTypeId = containerTypeSelect.val();
                                        }
                                    },
                                    onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                        if (Csw.number(nodeTypeCount) > 1) {
                                            containerTypeTable.show();
                                            containerTypeTable.cell(3, 1).br();
                                        } else {
                                            cswPrivate.state.containerNodeTypeId = lastNodeTypeId;
                                        }
                                    }
                                });
                            };

                            var makeQuantityForm = function () {
                                cswPrivate.state.initialQuantity.quantity = cswPrivate.state.initialQuantity.value;
                                cswPrivate.state.initialQuantity.selectedNodeId = cswPrivate.state.initialQuantity.nodeid;
                                cswPrivate.state.initialQuantity.isRequired = true;
                                cswPrivate.amountsGrid = Csw.wizard.amountsGrid(quantityTable.cell(qtyTableRow, 1), {
                                    name: 'wizardAmountsThinGrid',
                                    onChange: function (quantities) {
                                        cswPrivate.formIsValid = cswPrivate.updateQuantityAfterDispense(quantities);
                                        cswPrivate.toggleButton(cswPrivate.buttons.finish, cswPrivate.formIsValid);
                                    },
                                    quantity: cswPrivate.state.initialQuantity,
                                    containerlimit: cswPrivate.containerlimit,
                                    containerMinimum: 0,
                                    action: 'Dispense',
                                    relatedNodeId: cswPrivate.state.sourceContainerNodeId,
                                    selectedSizeId: cswPrivate.state.sizeId,
                                    customBarcodes: cswPrivate.state.customBarcodes
                                });

                                //make container div, then populate with ext.splitbutton -- this is the Dispense to Container version
                                

                                qtyTableRow++;
                            };

                            var makePrintBarcodesCheckBox = function() {
                                var checkBoxTable = quantityTable.cell(qtyTableRow, 1).table({
                                    name: 'checkboxTable',
                                    cellpadding: '1px',
                                    cellvalign: 'middle'
                                });
                                qtyTableRow++;

                                cswPrivate.printBarcodesCheckBox = checkBoxTable.cell(1, 1).checkBox({
                                    onChange: Csw.method(function() {
                                        if (cswPrivate.printBarcodesCheckBox.checked()) {
                                            cswPrivate.printBarcodes = true;
                                        } else {
                                            cswPrivate.printBarcodes = false;
                                        }
                                    })
                                });
                                checkBoxTable.cell(1, 2).span({ text: 'Print barcode labels for new containers' });
                            };

                            var getQuantityAfterDispense = function() {
                                var deductingValue = Csw.bool(cswPrivate.state.dispenseType !== cswPrivate.dispenseTypes.Add);
                                cswPrivate.state.quantityAfterDispense = cswPrivate.state.currentQuantity;
                                var quantities = [];
                                quantities.push({
                                    quantity: cswPrivate.quantityControl.value() * (deductingValue ? 1 : -1),
                                    unitid: cswPrivate.quantityControl.selectedUnit(),
                                    containerNo: 1
                                });
                                cswPrivate.formIsValid = cswPrivate.updateQuantityAfterDispense(quantities);
                                cswPrivate.toggleButton(cswPrivate.buttons.finish, cswPrivate.formIsValid);
                            };

                            if (cswPrivate.state.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                                makeContainerSelect();
                                makeQuantityForm();
                                makePrintBarcodesCheckBox();
                            } else {
                                quantityTable.cell(qtyTableRow, 1).br();
                                quantityTable.cell(qtyTableRow, 1).span({ text: 'Set quantity for dispense:' });                                
                                qtyTableRow++;
                                cswPrivate.state.initialQuantity.onNumberChange = function() {
                                    getQuantityAfterDispense();
                                };
                                cswPrivate.state.initialQuantity.onQuantityChange = function () {
                                    getQuantityAfterDispense();
                                };
                                cswPrivate.state.initialQuantity.quantity = cswPrivate.state.initialQuantity.value;
                                cswPrivate.state.initialQuantity.selectedNodeId = cswPrivate.state.initialQuantity.nodeid;
                                cswPrivate.state.initialQuantity.isRequired = true;
                                quantityTable.cell(qtyTableRow, 1).br({ number: 2 });
                                cswPrivate.quantityControl = quantityTable.cell(qtyTableRow, 1).quantity(cswPrivate.state.initialQuantity);
                                
                                //make container div, then populate with ext.splitbutton -- this is the Dispense for Use version
                                

                                qtyTableRow++;
                                getQuantityAfterDispense();
                            }
                            cswPrivate.stepTwoComplete = true;
                        } else {
                            cswPrivate.toggleButton(cswPrivate.buttons.finish, cswPrivate.formIsValid);
                        }
                        window.setTimeout(function () {
                            cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                        }, 250);
                    };
                } ());

                cswPrivate.roundToPrecision = function (num) {
                    var precision = Csw.number(cswPrivate.state.precision, 6);
                    return Math.round(Csw.number(num) * Math.pow(10, precision)) / Math.pow(10, precision);
                };

                cswPrivate.getTotalQuantityToDispense = function(quantities) {
                    var totalQuantityToDispense = 0;
                    Csw.each(quantities, function(quantity) {
                        if (false === Csw.isNullOrEmpty(quantity)) {
                            var containerNo = quantity.containerNo;
                            if (Csw.number(containerNo) === 0) {
                                //Not all deducted quantity needs to go into a container, but the quantity remaining still needs to be updated
                                containerNo = 1;
                            }
                            if (quantity.unitid !== cswPrivate.state.unitId) {
                                Csw.ajax.post({
                                    urlMethod: 'convertUnit',
                                    async: false,
                                    data: {
                                        ValueToConvert: Csw.number(quantity.quantity, 0),
                                        OldUnitId: quantity.unitid,
                                        NewUnitId: cswPrivate.state.unitId,
                                        MaterialId: cswPrivate.state.materialId
                                    },
                                    success: function(data) {
                                        if (false === Csw.isNullOrEmpty(data)) {
                                            totalQuantityToDispense += cswPrivate.roundToPrecision(Csw.number(data.convertedvalue, 0) * Csw.number(containerNo, 0));
                                        }
                                    }
                                });
                            } else {
                                totalQuantityToDispense += cswPrivate.roundToPrecision(Csw.number(quantity.quantity, 0) * Csw.number(containerNo, 0));
                            }
                        }
                    });
                    return totalQuantityToDispense;
                };

                cswPrivate.updateQuantityAfterDispense = function(quantities) {
                    var enableFinishButton = true;
                    cswPrivate.state.quantityAfterDispense = cswPrivate.roundToPrecision(Csw.number(cswPrivate.state.currentQuantity - cswPrivate.getTotalQuantityToDispense(quantities)));

                    if (false === Csw.isNullOrEmpty(cswPrivate.amountsGrid)) {
                        enableFinishButton = Csw.bool(cswPrivate.amountsGrid.containerCount >= 0 &&
                            cswPrivate.amountsGrid.containerCount <= cswPrivate.amountsGrid.containerlimit);
                    }

                    if (Csw.bool(cswPrivate.state.netQuantityEnforced)) {
                        if (cswPrivate.state.quantityAfterDispense < 0) {
                            cswPrivate.netQuantityExceededSpan.show();
                            enableFinishButton = false;
                        } else {
                            cswPrivate.netQuantityExceededSpan.hide();
                        }
                    }
                    cswPrivate.quantityAfterDispenseSpan.text(cswPrivate.state.quantityAfterDispense + ' ' + cswPrivate.state.currentUnitName);

                    if (cswPrivate.state.quantityAfterDispense === cswPrivate.state.currentQuantity) {
                        enableFinishButton = false;
                    }
                    return enableFinishButton;
                };

                cswPrivate.handleNext = function (newStepNo) {
                    cswPrivate.currentStepNo = newStepNo;
                    cswPrivate.setState();
                    switch (newStepNo) {
                        case 2:
                            if (Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId)) {
                                Csw.error.throwException(Csw.error.exception('Cannot dispense without a source container.', '', 'csw.dispensecontainer.js', 283));
                            } else {
                                if (Csw.isNullOrEmpty(cswPrivate.state.barcode) ||
                                Csw.isNullOrEmpty(cswPrivate.state.materialname) ||
                                Csw.isNullOrEmpty(cswPrivate.state.location) ||
                                Csw.isNullOrEmpty(cswPrivate.state.containerNodeTypeId)) {

                                    Csw.ajax.post({
                                        urlMethod: 'getDispenseSourceContainerData',
                                        data: {
                                            ContainerId: cswPrivate.state.sourceContainerNodeId
                                        },
                                        async: false,
                                        success: function (data) {
                                            cswPrivate.state.barcode = data.barcode;
                                            cswPrivate.state.materialname = data.materialname;
                                            cswPrivate.state.location = data.location;
                                            cswPrivate.state.containerNodeTypeId = data.nodetypeid;
                                            cswPrivate.state.unitId = data.unitid;
                                            cswPrivate.state.currentQuantity = data.quantity;
                                            cswPrivate.state.currentUnitName = data.unit;
                                            cswPrivate.state.sizeId = data.sizeid;
                                            cswPrivate.state.materialId = data.materialid;
                                        }
                                    });
                                }
                                cswPrivate.makeStepTwo(true);
                            }
                            break;
                    }
                };

                cswPrivate.handlePrevious = function (newStepNo) {
                    cswPrivate.currentStepNo = newStepNo;
                    cswPrivate.setState();
                    switch (newStepNo) {
                        case 1:
                            cswPrivate.makeStepOne();
                            break;
                    }
                };

                cswPrivate.onConfirmFinish = function () {
                    var designGrid = '',
                        finalQuantity = '',
                        finalUnit = '',
                        enableFinish = true;
                    if (false === Csw.isNullOrEmpty(cswPrivate.amountsGrid)) {
                        finalQuantity = cswPrivate.state.quantity;
                        finalUnit = cswPrivate.state.unitId;
                        designGrid = Csw.serialize(cswPrivate.amountsGrid.quantities());
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.quantityControl) && cswPrivate.state.dispenseType !== cswPrivate.dispenseTypes.Dispense) {
                        finalQuantity = cswPrivate.quantityControl.value();
                        finalUnit = cswPrivate.quantityControl.selectedUnit();
                        designGrid = '';
                    }

                    if (false === enableFinish) {
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    } else {

                        cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                        cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                        var jsonData = {
                            SourceContainerNodeId: Csw.string(cswPrivate.state.sourceContainerNodeId),
                            DispenseType: Csw.string(cswPrivate.state.dispenseType),
                            Quantity: Csw.string(finalQuantity),
                            UnitId: Csw.string(finalUnit),
                            ContainerNodeTypeId: Csw.string(cswPrivate.state.containerNodeTypeId),
                            DesignGrid: Csw.string(designGrid),
                            RequestItemId: Csw.string(cswPrivate.state.requestItemId)
                        };

                        Csw.ajax.post({
                            urlMethod: 'finalizeDispenseContainer',
                            data: jsonData,
                            success: function (data) {
                                var viewId = data.viewId;
                                Csw.tryExec(cswPrivate.onFinish, viewId);
                                cswPrivate.clearState();
                                if (cswPrivate.printBarcodes) {
                                    if (false === Csw.isNullOrEmpty(data.barcodes) &&
                                        Object.keys(data.barcodes).length > 0) {

                                        $.CswDialog('PrintLabelDialog', {
                                            nodes: data.barcodes,
                                            nodetypeid: cswPrivate.state.containerNodeTypeId
                                        });
                                    } else {
                                        //handle warning
                                    }
                                }
                            },
                            error: function () {
                                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                                cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                            }
                        });
                    }
                };

                (function _post() {

                    cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                        sourceContainerNodeId: cswPrivate.state.sourceContainerNodeId,
                        currentQuantity: cswPrivate.state.currentQuantity,
                        currentUnitName: cswPrivate.state.currentUnitName,
                        initialQuantity: cswPrivate.state.initialQuantity,
                        Title: Csw.string(cswPrivate.title),
                        StepCount: 2,
                        Steps: cswPrivate.wizardSteps,
                        StartingStep: cswPrivate.startingStep,
                        FinishText: 'Finish',
                        onNext: cswPrivate.handleNext,
                        onPrevious: cswPrivate.handlePrevious,
                        onCancel: function () {
                            cswPrivate.clearState();
                            Csw.tryExec(cswPrivate.onCancel);
                        },
                        onFinish: cswPrivate.onConfirmFinish,
                        doNextOnInit: false
                    });

                    cswPrivate.makeStepOne();
                } ());
            });
            return cswPublic;
        });
} ());

