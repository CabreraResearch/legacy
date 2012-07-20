/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.dispenseContainerWizard = Csw.nbt.dispenseContainerWizard ||
        Csw.nbt.register('dispenseContainerWizard', function (cswParent, options) {
            'use strict';

            //#region Variable Declaration
            var cswPrivate = {
                ID: 'cswDispenseContainerWizard',
                sourceContainerNodeId: '',
                currentQuantity: '',
                currentUnitName: '',
                capacity: '',
                onCancel: null,
                onFinish: null,
                startingStep: 1,
                wizard: '',
                wizardSteps: {
                    1: 'Select a Dispense Type',
                    2: 'Select a Destination Container Type',
                    3: 'Select Amount'
                },
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                dispenseTypes: {
                    Unknown: '',
                    Dispense: 'Dispense into a Child Container',
                    Use: 'Dispense for Use',
                    Waste: 'Waste Material',
                    Add: 'Add Material to Container'
                },
                divStep1: '', divStep2: '', divstep3: '',
                dispenseType: 'Unknown',
                quantity: 'Unknown',
                unitId: 'Unknown',
                sizeId: '',
                containerNodeTypeId: 'Unknown',
                containerObjectClassId: '',
                quantityControl: null,
                requestItemId: '',
                title: 'Dispense from Container',
                materialname: '',
                barcode: '',
                location: ''
            };
            if (options) $.extend(cswPrivate, options);

            var cswPublic = cswParent.div();
            cswPrivate.currentStepNo = cswPrivate.startingStep;

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
                cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
            };

            //Step 1. Select a Dispense Type.
            cswPrivate.makeStepOne = (function () {
                var stepOneComplete = false;
                return function () {
                    
                    var dispenseTypeTable;

                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    var initStepOne = Csw.method(function() {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();
                        
                        cswPrivate.divStep1.br();
                        if (false === Csw.isNullOrEmpty(cswPrivate.barcode)) {
                            cswPrivate.divStep1.p({ text: 'You have selected container barcode: [' + Csw.string(cswPrivate.barcode) + ']' });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.materialname)) {
                            cswPrivate.divStep1.p({ text: 'On Material: ' + Csw.string(cswPrivate.materialname) });
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.location)) {
                            cswPrivate.divStep1.p({ text: 'At Location: ' + Csw.string(cswPrivate.location) });
                        }
                        cswPrivate.divStep1.br();

                        dispenseTypeTable = cswPrivate.divStep1.table({
                            ID: cswPrivate.makeStepId('setDispenseTypeTable'),
                            cellpadding: '1px',
                            cellvalign: 'middle'
                        });

                        dispenseTypeTable.cell(1, 1).span({ text: 'What kind of dispense would you like to do?' });

                        var dispenseTypeDiv = dispenseTypeTable.cell(1, 2).div();

                        var dispenseTypeSelect = dispenseTypeDiv.select({
                            ID: cswPrivate.makeStepId('setDispenseTypePicklist'),
                            cssclass: 'selectinput',
                            values: cswPrivate.dispenseTypes,
                            onChange: function() {
                                if (false === Csw.isNullOrEmpty(dispenseTypeSelect.val())) {
                                    cswPrivate.dispenseType = dispenseTypeSelect.val();
                                    cswPrivate.wizard.next.enable();
                                } else {
                                    cswPrivate.wizard.next.disable();
                                }
                            },
                            selected: cswPrivate.dispenseTypes.Unknown
                        });
                    });
                    
                    if (false === stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.wizard.div(1);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                        if (Csw.isNullOrEmpty(cswPrivate.sourceContainerNodeId)) {
                            cswPrivate.divStep1.p({ text: 'No container is selected. Find a container to dispense from.' });
                            cswPrivate.divStep1.br();
                            Csw.debug.assert(false === Csw.isNullOrEmpty(cswPrivate.containerNodeTypeId), 'Cannot find a container without a container nodetype.');
                            cswPrivate.divStep1.universalSearch({
                                ID: cswPrivate.makeStepId('containerSearch'),
                                nodetypeid: cswPrivate.containerNodeTypeId,
                                objectclassid: cswPrivate.containerObjectClassId,
                                showSaveAsView: false,
                                allowEdit: false,
                                allowDelete: false,
                                extraAction: 'Select',
                                extraActionIcon: Csw.enums.iconType.check,
                                onExtraAction: function (nodeObj) {
                                    Csw.debug.assert(false === Csw.isNullOrEmpty(nodeObj), 'Selected a container which did not yield a nodeObj');
                                    Csw.debug.assert(false === Csw.isNullOrEmpty(nodeObj.nodeid), 'Selected a container which did not yield a nodeid');
                                    if (false === Csw.isNullOrEmpty(nodeObj.nodeid)) {
                                        cswPrivate.sourceContainerNodeId = nodeObj.nodeid;
                                        initStepOne();
                                    }
                                }

                            });

                        } else {
                            initStepOne();
                        }
                        stepOneComplete = true;
                    }
                };
            } ());

            //Step 2. Select a Destination Container NodeType .
            //if( only one NodeType exists || Dispense Type != Dispense ) skip this step
            cswPrivate.makeStepTwo = (function () {
                var stepTwoComplete = false,
                    skipThisStep = false;
                return function (movingForward) {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    if (false === stepTwoComplete) {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                        var containerTypeTable = '',
                            blankText = '[Select One]';

                        cswPrivate.divStep2 = cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.br();

                        containerTypeTable = cswPrivate.divStep2.table({
                            ID: cswPrivate.makeStepId('setContainerTypeTable'),
                            cellpadding: '1px',
                            cellvalign: 'middle'
                        }).hide();

                        var containerTypeText = containerTypeTable.cell(1, 1).span({ text: 'What kind of container would you like to use?' });

                        var containerTypeDiv = containerTypeTable.cell(1, 2).div();

                        var containerTypeSelect = containerTypeDiv.nodeTypeSelect({
                            ID: Csw.makeSafeId('nodeTypeSelect'),
                            objectClassName: 'ContainerClass',
                            blankOptionText: blankText,
                            onSelect: function (data, nodeTypeCount) {
                                if (blankText !== containerTypeSelect.val()) {
                                    cswPrivate.containerNodeTypeId = containerTypeSelect.val();
                                    cswPrivate.wizard.next.enable();
                                }
                                else {
                                    cswPrivate.wizard.next.disable();
                                }
                            },
                            onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                if (Csw.number(nodeTypeCount) > 1) {
                                    containerTypeTable.show();
                                }
                                else {
                                    cswPrivate.containerNodeTypeId = lastNodeTypeId;
                                    skipThisStep = true;
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                                    cswPrivate.wizard.next.click();
                                }
                            }
                        });

                        stepTwoComplete = true;
                    }
                    if (skipThisStep) {
                        movingForward ? cswPrivate.wizard.next.click() : cswPrivate.wizard.previous.click();
                    }
                };
            } ());

            //Step 3. Select Amount
            //DispenseType != Dispense ? 
            //Select a Quantity :
            //Select the number of destination containers and their quantities.
            cswPrivate.makeStepThree = (function () {
                var stepThreeDispenseComplete = false;
                var stepThreeAddWasteUseComplete = false;
                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    if (cswPrivate.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                        if (false === stepThreeDispenseComplete) {
                            if (stepThreeAddWasteUseComplete) {
                                cswPrivate.divStep3.empty();
                                stepThreeAddWasteUseComplete = false;
                            }

                            cswPrivate.divStep3 = cswPrivate.wizard.div(3);
                            cswPrivate.divStep3.br();

                            quantityTable = cswPrivate.divStep3.table({
                                ID: cswPrivate.makeStepId('setQuantityTable'),
                                cellpadding: '1px',
                                cellvalign: 'middle'
                            });

                            quantityTable.cell(1, 1).span({ text: 'Current Quantity:    ' + cswPrivate.currentQuantity + ' ' + cswPrivate.currentUnitName }).br({ number: 2 });

                            cswPrivate.amountsGrid = Csw.nbt.wizard.amountsGrid(quantityTable.cell(2, 1), {
                                ID: cswPrivate.wizard.makeStepId('wizardAmountsThinGrid'),
                                onAdd: function () {
                                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                                },
                                quantity: cswPrivate.capacity,
                                containerlimit: cswPrivate.containerlimit,
                                makeId: cswPrivate.wizard.makeStepId,
                                containerMinimum: 0,
                                action: 'Dispense',
                                relatedNodeId: cswPrivate.sourceContainerNodeId,
                                selectedSizeId: cswPrivate.sizeId
                            });

                            stepThreeDispenseComplete = true;
                        }
                    }
                    else {
                        if (false === stepThreeAddWasteUseComplete) {
                            if (stepThreeDispenseComplete) {
                                cswPrivate.divStep3.empty();
                                stepThreeDispenseComplete = false;
                            }
                            var quantityTable = '',
                            blankText = '[Select One]';

                            cswPrivate.divStep3 = cswPrivate.wizard.div(3);
                            cswPrivate.divStep3.br();                            

                            quantityTable = cswPrivate.divStep3.table({
                                ID: cswPrivate.makeStepId('setQuantityTable'),
                                cellpadding: '1px',
                                cellvalign: 'middle'
                            });

                            quantityTable.cell(1, 1).span({ text: 'Current Quantity:    ' + cswPrivate.currentQuantity + ' ' + cswPrivate.currentUnitName }).br();
                            quantityTable.cell(2, 1).span({ text: 'Select the quantity you wish to dispense:' });
                            cswPrivate.quantityControl = quantityTable.cell(2, 2).quantity(cswPrivate.capacity);

                            cswPrivate.toggleButton(cswPrivate.buttons.finish, true);

                            stepThreeAddWasteUseComplete = true;
                        }
                    }
                };
            } ());

            cswPrivate.handleNext = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case 2:
                        if (cswPrivate.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                            cswPrivate.makeStepTwo(true);
                        }
                        else {
                            cswPrivate.wizard.next.click();
                        }
                        break;
                    case 3:
                        cswPrivate.makeStepThree();
                        break;
                }
            };

            cswPrivate.handlePrevious = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case 1:
                        cswPrivate.makeStepOne();
                        break;
                    case 2:
                        if (cswPrivate.dispenseType === cswPrivate.dispenseTypes.Dispense) {
                            cswPrivate.makeStepTwo(false);
                        }
                        else {
                            cswPrivate.wizard.previous.click();
                        }
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
                    cswPrivate.quantity = cswPrivate.quantityControl.quantityValue;
                    cswPrivate.unitId = cswPrivate.quantityControl.unitVal;
                }

                var jsonData = {
                    SourceContainerNodeId: cswPrivate.sourceContainerNodeId,
                    DispenseType: cswPrivate.dispenseType,
                    Quantity: cswPrivate.quantity,
                    UnitId: cswPrivate.unitId,
                    ContainerNodeTypeId: cswPrivate.containerNodeTypeId,
                    DesignGrid: designGrid,
                    RequestItemId: Csw.string(cswPrivate.requestItemId)
                };

                Csw.ajax.post({
                    urlMethod: 'finalizeDispenseContainer',
                    data: jsonData,
                    success: function (data) {
                        var viewId = data.viewId;
                        Csw.tryExec(cswPrivate.onFinish, viewId);
                        if (false === Csw.isNullOrEmpty(data.barcodeId)) {
                            $.CswDialog('GenericDialog', {
                                'div': Csw.literals.div().span({ text: 'Would you like to print Labels for the new Containers?' }),
                                'title': 'Print Labels?',
                                'onOk': function () {
                                    $.CswDialog('PrintLabelDialog', { 'nodeid': cswPrivate.sourceContainerNodeId, 'propid': data.barcodeId });
                                },
                                'okText': 'Yes',
                                'cancelText': 'No'
                            });

                        }
                    },
                    error: function () {
                        cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    }
                });
            };

            (function () {

                cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                    ID: Csw.makeId({ ID: cswPrivate.ID, suffix: 'wizard' }),
                    sourceContainerNodeId: cswPrivate.sourceContainerNodeId,
                    currentQuantity: cswPrivate.currentQuantity,
                    currentUnitName: cswPrivate.currentUnitName,
                    capacity: cswPrivate.capacity,
                    Title: Csw.string(cswPrivate.title),
                    StepCount: 3,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleNext,
                    onPrevious: cswPrivate.handlePrevious,
                    onCancel: cswPrivate.onCancel,
                    onFinish: cswPrivate.onConfirmFinish,
                    doNextOnInit: false
                });

                cswPrivate.makeStepOne();
            } ());

            return cswPublic;
        });
} ());

