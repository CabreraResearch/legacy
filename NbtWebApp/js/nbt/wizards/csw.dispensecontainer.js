/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.dispenseContainerWizard = Csw.nbt.dispenseContainerWizard ||
        Csw.nbt.register('dispenseContainerWizard', function (cswParent, options) {
            'use strict';

            Csw.enums.wizardSteps_DispenseContainer = Csw.enums.wizardSteps_DispenseContainer ||
                Csw.enums.register('wizardSteps_DispenseContainer', {
                    step1: { step: 1, description: 'Select a Dispense Type' },
                    step2: { step: 2, description: 'Select a Destination Container NodeType' },
                    step3: { step: 3, description: 'Select Amount' },
                    stepcount: 3
                });

            //#region Variable Declaration
            var cswPrivate = {
                ID: 'cswDispenseContainerWizard',
                nodeId: 'Unknown',
                currentQuantity: '',
                currentUnitName: '',
                onCancel: null,
                onFinish: null,
                startingStep: 1,
                wizard: '',
                wizardSteps: {
                    1: Csw.enums.wizardSteps_DispenseContainer.step1.description,
                    2: Csw.enums.wizardSteps_DispenseContainer.step2.description,
                    3: Csw.enums.wizardSteps_DispenseContainer.step3.description
                },
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '', divStep2: '', divstep3: '',
                //TODO - fill with necessary variables
                gridIsPopulated: false,
                dispensedContainerGrid: '',
                gridOptions: {},
                dispenseType: 'Unknown',
                quantity: 'Unknown',
                unitId: 'Unknown',
                containerNodeTypeId: 'Unknown'
            };
            if (options) $.extend(cswPrivate, options);

            var cswPublic = cswParent.div();
            cswPrivate.currentStepNo = cswPrivate.startingStep;


            //TODO - refactor this
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
                    var dispenseTypeTable = '',
                        dispenseTypes = {
                            0: '',
                            1: 'Add',
                            2: 'Waste',
                            3: 'Dispense'
                        };

                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    if (false === stepOneComplete) {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                        cswPrivate.divStep1 = cswPrivate.wizard.div(Csw.enums.wizardSteps_DispenseContainer.step1.step);
                        cswPrivate.divStep1.br();

                        dispenseTypeTable = cswPrivate.divStep1.table({
                            ID: cswPrivate.makeStepId('setDispenseTypeTable')
                        });

                        dispenseTypeTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'What kind of dispense would you like to do?' });

                        var dispenseTypeDiv = dispenseTypeTable.cell(1, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .div();

                        var dispenseTypeSelect = dispenseTypeDiv.select({
                            ID: cswPrivate.makeStepId('setDispenseTypePicklist'),
                            cssclass: 'selectinput',
                            values: dispenseTypes,
                            onChange: function () {
                                if (false === Csw.isNullOrEmpty(dispenseTypeSelect.val())) {
                                    cswPrivate.dispenseType = dispenseTypeSelect.val();
                                    cswPrivate.wizard.next.enable();
                                }
                                else {
                                    cswPrivate.wizard.next.disable();
                                }
                            },
                            selected: dispenseTypes[0]
                        });

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

                        cswPrivate.divStep2 = cswPrivate.wizard.div(Csw.enums.wizardSteps_DispenseContainer.step2.step);
                        cswPrivate.divStep2.br();

                        containerTypeTable = cswPrivate.divStep2.table({
                            ID: cswPrivate.makeStepId('setContainerTypeTable')
                        });

                        var containerTypeText = containerTypeTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'What kind of container would you like to use?' })
                            .hide();

                        var containerTypeDiv = containerTypeTable.cell(1, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .div().hide();

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
                                    containerTypeText.show();
                                    containerTypeDiv.show();
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
                var stepThreeComplete = false;
                return function () {
                    if (false === stepThreeComplete) {
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                        if (cswPrivate.dispenseType === 'Dispense') {
                            //TODO - step 3B - container/quantity grid
                            var temp = ''; //delete me
                        }
                        else {
                            var quantityTable = '',
                            blankText = '[Select One]';

                            cswPrivate.divStep3 = cswPrivate.wizard.div(Csw.enums.wizardSteps_DispenseContainer.step3.step);
                            cswPrivate.divStep3.br();

                            quantityTable = cswPrivate.divStep3.table({
                                ID: cswPrivate.makeStepId('setQuantityTable')
                            });

                            quantityTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'Current Quantity:    ' + cswPrivate.currentQuantity + ' ' + cswPrivate.currentUnitName }).br();

                            quantityTable.cell(2, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'Select the quantity you wish to dispense:' });

                            var quantityNumDiv = quantityTable.cell(2, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .div();

                            var quantityNumBox = quantityNumDiv.numberTextBox({
                                ID: cswPrivate.makeStepId('setQuantityNumBox'),
                                value: '',
                                MinValue: 0,
                                MaxValue: 999999999,
                                Precision: 6,
                                Required: true,
                                onChange: function () {
                                    cswPrivate.quantity = quantityNumBox.val();
                                    cswPrivate.unitId = quantityUnitSelect.val();
                                    if (false === Csw.isNullOrEmpty(quantityNumBox.val())
                                    && false === Csw.isNullOrEmpty(quantityUnitSelect.val())) {
                                        cswPrivate.wizard.finish.enable();
                                    }
                                    else {
                                        cswPrivate.wizard.finish.disable();
                                    }
                                }
                            });

                            var quantityUnitDiv = quantityTable.cell(2, 3)
                            .css({ 'padding': '1px' })
                            .div();

                            var quantityUnitSelect = quantityUnitDiv.nodeSelect({
                                ID: Csw.makeSafeId('nodeSelect'),
                                objectClassName: 'UnitOfMeasureClass',
                                onSelect: function (data, nodeTypeCount) {
                                    cswPrivate.unitId = quantityUnitSelect.val();
                                    if (false === Csw.isNullOrEmpty(quantityNumBox.val())
                                    && false === Csw.isNullOrEmpty(quantityUnitSelect.val())) {
                                        cswPrivate.wizard.finish.enable();
                                    }
                                    else {
                                        cswPrivate.wizard.finish.disable();
                                    }
                                }
                            });

                        }
                        stepThreeComplete = true;
                    }
                };
            } ());

            cswPrivate.handleNext = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_DispenseContainer.step2.step:
                        if ('Dispense' === cswPrivate.dispenseType) {
                            cswPrivate.makeStepTwo(true);
                        }
                        else {
                            cswPrivate.wizard.next.click();
                        }
                        break;
                    case Csw.enums.wizardSteps_DispenseContainer.step3.step:
                        cswPrivate.makeStepThree();
                        break;
                }
            };

            cswPrivate.handlePrevious = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_DispenseContainer.step1.step:
                        cswPrivate.makeStepOne();
                        break;
                    case Csw.enums.wizardSteps_DispenseContainer.step2.step:
                        if ('Dispense' === cswPrivate.dispenseType) {
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

                var designGrid = 'unknown';

                if (false === Csw.isNullOrEmpty(cswPrivate.dispensedContainerGrid)) {
                    designGrid = JSON.stringify(cswPrivate.dispensedContainerGrid.getAllGridRows());
                }

                //TODO - fill with new Container quantity/grid data, etc.
                var jsonData = {
                    SourceContainerNodeId: cswPrivate.nodeId,
                    DispenseType: cswPrivate.dispenseType,
                    Quantity: cswPrivate.quantity,
                    UnitId: cswPrivate.unitId,
                    ContainerNodeTypeId: cswPrivate.containerNodeTypeId,
                    DesignGrid: designGrid
                };

                Csw.ajax.post({
                    url: '/NbtWebApp/wsNBT.asmx/finalizeDispenseContainer',
                    data: jsonData,
                    success: function (data) {
                        var viewId = data.viewId;
                        Csw.tryExec(cswPrivate.onFinish, viewId);
                        //TODO - make a printLabel dialog to show after confirmFinish

                    },
                    error: function () {
                        cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    }
                }); //Csw.ajax                
            };

            //TODO - parse, refactor, and rebuild makeGrid for dispensedContainers

            (function () {

                cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                    ID: Csw.makeId({ ID: cswPrivate.ID, suffix: 'wizard' }),
                    nodeId: cswPrivate.nodeId,
                    currentQuantity: cswPrivate.currentQuantity,
                    currentUnitName: cswPrivate.currentUnitName,
                    Title: 'Create New Inspection',
                    StepCount: Csw.enums.wizardSteps_DispenseContainer.stepcount,
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

