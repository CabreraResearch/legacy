/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

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
                        printBarcodes: false
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
                    title: 'Dispense from Container',
                    dispenseMode: 'Direct',
                    dispenseModes: {
                        Direct: 'Direct',
                        Request: 'Request'
                    }
                };

                cswPrivate.setDispenseMode = function () {
                    if (false === Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                        cswPrivate.dispenseMode = cswPrivate.dispenseModes.Request;
                        cswPrivate.wizardSteps["1"] = 'Select a Container';
                        cswPrivate.state.dispenseType = cswPrivate.dispenseTypes.Dispense;
                    } else {
                        cswPrivate.dispenseMode = cswPrivate.dispenseModes.Direct;
                    }
                };

                cswPrivate.validateState = function () {
                    var state;
                    if (Csw.isNullOrEmpty(cswPrivate.state.sourceContainerNodeId) && Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
                        state = cswPrivate.getState();
                        $.extend(cswPrivate.state, state);
                    }
                    cswPrivate.setState();
                    cswPrivate.setDispenseMode();
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
                        $.extend(cswPrivate, options);
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

                                if (cswPrivate.dispenseMode !== cswPrivate.dispenseModes.Request) {
                                    cswPrivate.divStep1.br({ number: 2 });
                                    cswPrivate.divStep1.span({ text: 'Pick a type of dispense:' });
                                    cswPrivate.divStep1.br({ number: 1 });

                                    var dispenseTypesArray = [
                                        cswPrivate.dispenseTypes.Dispense,
                                        cswPrivate.dispenseTypes.Use,
                                        cswPrivate.dispenseTypes.Add,
                                        cswPrivate.dispenseTypes.Waste
                                    ];

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
                                        }
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

                                        cswPrivate.containerGrid = Csw.nbt.wizard.nodeGrid(cswPrivate.divStep1, {
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
                            if (cswPrivate.dispenseMode !== cswPrivate.dispenseModes.Request) {
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

                            if (cswPrivate.dispenseMode !== cswPrivate.dispenseModes.Request) {
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

                            if (false === Csw.isNullOrEmpty(cswPrivate.state.requestItemId)) {
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

                                cswPrivate.amountsGrid = Csw.nbt.wizard.amountsGrid(quantityTable.cell(8, 1), {
                                    ID: cswPrivate.wizard.makeStepId('wizardAmountsThinGrid'),
                                    onAdd: function (hasQuantity) {
                                        cswPrivate.toggleButton(cswPrivate.buttons.finish, hasQuantity);
                                    },
                                    onDelete: function (hasQuantity) {
                                        cswPrivate.toggleButton(cswPrivate.buttons.finish, hasQuantity);
                                    },
                                    quantity: cswPrivate.state.capacity,
                                    containerlimit: cswPrivate.containerlimit,
                                    makeId: cswPrivate.wizard.makeStepId,
                                    containerMinimum: 0,
                                    action: 'Dispense',
                                    relatedNodeId: cswPrivate.state.sourceContainerNodeId,
                                    selectedSizeId: cswPrivate.state.sizeId
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
                                            cswPrivate.state.printBarcodes = true;
                                        } else {
                                            cswPrivate.state.printBarcodes = false;
                                        }
                                    })
                                });
                                checkBoxTable.cell(1, 2).span({ text: 'Print barcode labels for new containers' });
                            }

                            if (cswPrivate.state.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                                makeContainerSelect();
                                makeQuantityForm();
                                makePrintBarcodesCheckBox();
                            } else {
                                quantityTable.cell(8, 1).span({ text: 'Set quantity for dispense:' });
                                cswPrivate.quantityControl = quantityTable.cell(9, 1).quantity(cswPrivate.state.capacity);

                                cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                            }
                            cswPrivate.stepTwoComplete = true;
                        }
                        window.setTimeout(function () {
                            cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                        }, 250);
                    };
                } ());

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
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    var designGrid = 'Unknown';
                    if (false === Csw.isNullOrEmpty(cswPrivate.amountsGrid)) {
                        designGrid = Csw.serialize(cswPrivate.amountsGrid.quantities);
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.quantityControl)) {
                        cswPrivate.state.quantity = cswPrivate.quantityControl.quantityValue;
                        cswPrivate.state.unitId = cswPrivate.quantityControl.unitVal;
                    }

                    var jsonData = {
                        SourceContainerNodeId: Csw.string(cswPrivate.state.sourceContainerNodeId),
                        DispenseType: Csw.string(cswPrivate.state.dispenseType),
                        Quantity: Csw.string(cswPrivate.state.quantity),
                        UnitId: Csw.string(cswPrivate.state.unitId),
                        ContainerNodeTypeId: Csw.string(cswPrivate.state.containerNodeTypeId),
                        DesignGrid: designGrid,
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
                                if (cswPrivate.state.printBarcodes) {
                                    $.CswDialog('PrintLabelDialog', { 'nodeid': cswPrivate.state.sourceContainerNodeId, 'propid': data.barcodeId });
                                }
                            }
                        },
                        error: function () {
                            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                        }
                    });
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

