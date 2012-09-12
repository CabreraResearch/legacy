
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
                    ID: 'cswDispenseContainerWizard',
                    state: {
                        sourceContainerNodeId: '',
                        currentQuantity: '',
                        currentUnitName: '',
                        precision: 6,
                        quantityAfterDispense: '',
                        capacity: '',
                        dispenseType: 'Dispense into a Child Container',
                        quantity: '',
                        unitId: '',
                        sizeId: '',
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
                    printBarcodes: false
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
                    var ret = Csw.clientDb.getItem(cswPrivate.ID + '_' + cswDispenseWizardStateName);
                    if (false === Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                        ret.sourceContainerNodeId = null;
                        ret.barcode = null;
                        ret.location = null;
                    }
                    return ret;
                };

                cswPrivate.setState = function () {
                    Csw.clientDb.setItem(cswPrivate.ID + '_' + cswDispenseWizardStateName, cswPrivate.state);
                };

                cswPrivate.clearState = function () {
                    Csw.clientDb.removeItem(cswPrivate.ID + '_' + cswDispenseWizardStateName);
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

                cswPrivate.makeStepId = function (suffix, stepNo) {
                    var step = stepNo || cswPrivate.currentStepNo;
                    return Csw.makeId({ prefix: 'step_' + step, ID: cswPrivate.ID, suffix: suffix });
                };

                cswPrivate.validationFailed = function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
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
                            var dispenseTypeSelect;

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
                                        ID: 'dispensetypes',
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
                                ID: cswPrivate.makeStepId('setDispenseTypeTable'),
                                cellpadding: '1px',
                                cellalign: 'left',
                                cellvalign: 'middle',
                                FirstCellRightAlign: true
                            });

                            if (cswPrivate.state.dispenseMode !== cswPrivate.dispenseModes.RequestMaterial) {
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.barcode)) {
                                    dispenseTypeTable.cell(1, 1).span({ text: 'Barcode: ' }).addClass('propertylabel');
                                    dispenseTypeTable.cell(1, 2).span({ text: Csw.string(cswPrivate.state.barcode) });
                                }
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.materialname)) {
                                    dispenseTypeTable.cell(2, 1).span({ text: 'Material: ' }).addClass('propertylabel');
                                    dispenseTypeTable.cell(2, 2).span({ text: Csw.string(cswPrivate.state.materialname) });
                                }
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.location)) {
                                    dispenseTypeTable.cell(3, 1).span({ text: 'Location: ' }).addClass('propertylabel');
                                    dispenseTypeTable.cell(3, 2).span({ text: Csw.string(cswPrivate.state.location) });
                                }
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.currentQuantity)) {
                                    dispenseTypeTable.cell(4, 1).span({ text: 'Current Quantity: ' }).addClass('propertylabel');
                                    dispenseTypeTable.cell(4, 2).span({ text: cswPrivate.state.currentQuantity + ' ' + cswPrivate.state.currentUnitName });
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
                                blankText = '[Select One]';

                            cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                            cswPrivate.divStep2.empty();
                            cswPrivate.divStep2.span({ text: 'Confirm the source container and define the amounts to dispense:' });
                            cswPrivate.divStep2.br();

                            quantityTable = cswPrivate.divStep2.table({
                                ID: cswPrivate.makeStepId('setQuantityTable'),
                                cellpadding: '1px',
                                cellvalign: 'middle'
                            });

                            quantityTable.cell(1, 1).br();
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.barcode)) {
                                quantityTable.cell(2, 1).span({ labelText: 'Barcode: ', text: Csw.string(cswPrivate.state.barcode) });
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.materialname)) {
                                quantityTable.cell(3, 1).span({ labelText: 'Material: ', text: Csw.string(cswPrivate.state.materialname) });
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.location)) {
                                quantityTable.cell(4, 1).span({ labelText: 'Location: ', text: Csw.string(cswPrivate.state.location) });
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.state.currentQuantity)) {
                                quantityTable.cell(5, 1).span({ labelText: 'Current Quantity: ', text: cswPrivate.state.currentQuantity + ' ' + cswPrivate.state.currentUnitName });
                                cswPrivate.state.quantityAfterDispense = cswPrivate.state.currentQuantity;
                                cswPrivate.quantityAfterDispenseSpan = quantityTable.cell(6, 1).span({ labelText: 'Quantity after Dispense: ', text: cswPrivate.state.quantityAfterDispense + ' ' + cswPrivate.state.currentUnitName });
                                cswPrivate.netQuantityExceededSpan = quantityTable.cell(6, 1).span({ cssclass: 'CswErrorMessage_ValidatorError', text: ' Total quantity to dispense cannot exceed source container\'s net quantity.' });
                                cswPrivate.netQuantityExceededSpan.hide();
                            }
                            quantityTable.cell(6, 1).br({ number: 1 });

                            var makeContainerSelect = function () {
                                var containerTypeTable = quantityTable.cell(7, 1).table({
                                    ID: cswPrivate.makeStepId('setContainerTypeTable'),
                                    cellpadding: '1px',
                                    cellvalign: 'middle'
                                }).hide();

                                containerTypeTable.cell(1, 1).span({ text: 'Select a Container Type' });

                                var containerTypeSelect = containerTypeTable.cell(2, 1).nodeTypeSelect({
                                    ID: Csw.makeSafeId('nodeTypeSelect'),
                                    objectClassName: 'ContainerClass',
                                    blankOptionText: blankText,
                                    onSelect: function (data, nodeTypeCount) {
                                        if (blankText !== containerTypeSelect.val()) {
                                            cswPrivate.state.containerNodeTypeId = containerTypeSelect.val();
                                        }
                                    },
                                    onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                        if (Csw.number(nodeTypeCount) > 1) {
                                            containerTypeTable.show();
                                        } else {
                                            cswPrivate.state.containerNodeTypeId = lastNodeTypeId;
                                        }
                                    }
                                });
                                containerTypeTable.cell(3, 1).br();
                            };

                            var makeQuantityForm = function () {

                                cswPrivate.amountsGrid = Csw.wizard.amountsGrid(quantityTable.cell(8, 1), {
                                    ID: cswPrivate.wizard.makeStepId('wizardAmountsThinGrid'),
                                    onAdd: function (hasQuantity, quantityToDeduct, unitToDeduct) {
                                        var enableFinishButton = cswPrivate.updateQuantityAfterDispense(hasQuantity, quantityToDeduct, unitToDeduct, true);
                                        cswPrivate.toggleButton(cswPrivate.buttons.finish, enableFinishButton);
                                    },
                                    onDelete: function (hasQuantity, quantityToAdd, unitToAdd) {
                                        var enableFinishButton = cswPrivate.updateQuantityAfterDispense(hasQuantity, quantityToAdd, unitToAdd, false);
                                        cswPrivate.toggleButton(cswPrivate.buttons.finish, enableFinishButton);
                                    },
                                    quantity: cswPrivate.state.capacity,
                                    containerlimit: cswPrivate.containerlimit,
                                    makeId: cswPrivate.wizard.makeStepId,
                                    containerMinimum: 0,
                                    action: 'Dispense',
                                    relatedNodeId: cswPrivate.state.sourceContainerNodeId,
                                    selectedSizeId: cswPrivate.state.sizeId,
                                    customBarcodes: cswPrivate.state.customBarcodes
                                });
                            };

                            var makePrintBarcodesCheckBox = function () {
                                var checkBoxTable = quantityTable.cell(9, 1).table({
                                    ID: cswPrivate.makeStepId('checkboxTable'),
                                    cellpadding: '1px',
                                    cellvalign: 'middle'
                                });

                                cswPrivate.printBarcodesCheckBox = checkBoxTable.cell(1, 1).checkBox({
                                    onChange: Csw.method(function () {
                                        var val;
                                        if (cswPrivate.printBarcodesCheckBox.checked()) {
                                            cswPrivate.printBarcodes = true;
                                        } else {
                                            cswPrivate.printBarcodes = false;
                                        }
                                    })
                                });
                                checkBoxTable.cell(1, 2).span({ text: 'Print barcode labels for new containers' });
                            }

                            var getQuantityAfterDispense = function () {
                                var deductingValue = Csw.bool(cswPrivate.state.dispenseType !== cswPrivate.dispenseTypes.Add);
                                cswPrivate.state.quantityAfterDispense = cswPrivate.state.currentQuantity;
                                var enableFinish = cswPrivate.updateQuantityAfterDispense(true, cswPrivate.quantityControl.quantityValue, cswPrivate.quantityControl.unitVal, deductingValue);
                                cswPrivate.toggleButton(cswPrivate.buttons.finish, enableFinish);
                            }

                            if (cswPrivate.state.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                                makeContainerSelect();
                                makeQuantityForm();
                                makePrintBarcodesCheckBox();
                            } else {
                                quantityTable.cell(8, 1).span({ text: 'Set quantity for dispense:' });
                                cswPrivate.state.capacity.onChange = function () {
                                    getQuantityAfterDispense();
                                }
                                cswPrivate.quantityControl = quantityTable.cell(9, 1).quantity(cswPrivate.state.capacity);
                                getQuantityAfterDispense();
                            }
                            cswPrivate.stepTwoComplete = true;
                        }
                        window.setTimeout(function () {
                            cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                        }, 250);
                    };
                } ());

                var roundToPrecision = function (num) {
                    var precision = Csw.number(cswPrivate.state.precision);
                    return Math.round(Csw.number(num) * Math.pow(10, precision)) / Math.pow(10, precision);
                };

                cswPrivate.updateQuantityAfterDispense = function (enableFinishButton, quantityDeltaValue, quantityDeltaUnitId, deductingValue) {
                    if (quantityDeltaUnitId !== cswPrivate.state.unitId) {
                        Csw.ajax.post({
                            urlMethod: 'convertUnit',
                            async: false,
                            data: {
                                ValueToConvert: quantityDeltaValue,
                                OldUnitId: quantityDeltaUnitId,
                                NewUnitId: cswPrivate.state.unitId
                            },
                            success: function (data) {
                                if (Csw.isNullOrEmpty(data)) {
                                    quantityDeltaValue = 0;
                                } else {
                                    var precision = Csw.number(cswPrivate.state.precision);
                                    quantityDeltaValue = roundToPrecision(Csw.number(data.convertedvalue));
                                }
                            },
                            error: function () {
                                quantityDeltaValue = 0;
                            }
                        });
                    }
                    if (deductingValue) {
                        cswPrivate.state.quantityAfterDispense = roundToPrecision(Csw.number(cswPrivate.state.quantityAfterDispense) - Csw.number(quantityDeltaValue));
                    } else {
                        cswPrivate.state.quantityAfterDispense = roundToPrecision(Csw.number(cswPrivate.state.quantityAfterDispense) + Csw.number(quantityDeltaValue));
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
                    return enableFinishButton;
                }

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
                        designGrid = Csw.serialize(cswPrivate.amountsGrid.quantities);
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.quantityControl) && cswPrivate.state.dispenseType !== cswPrivate.dispenseTypes.Dispense) {
                        finalQuantity = cswPrivate.quantityControl.quantityValue;
                        finalUnit = cswPrivate.quantityControl.unitVal;
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
                                if (false === Csw.isNullOrEmpty(data.barcodeId)) {
                                    if (cswPrivate.printBarcodes) {
                                        $.CswDialog('PrintLabelDialog', { 'nodeid': cswPrivate.state.sourceContainerNodeId, 'propid': data.barcodeId });
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
                        ID: Csw.makeId({ ID: cswPrivate.ID, suffix: 'wizard' }),
                        sourceContainerNodeId: cswPrivate.state.sourceContainerNodeId,
                        currentQuantity: cswPrivate.state.currentQuantity,
                        currentUnitName: cswPrivate.state.currentUnitName,
                        capacity: cswPrivate.state.capacity,
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

