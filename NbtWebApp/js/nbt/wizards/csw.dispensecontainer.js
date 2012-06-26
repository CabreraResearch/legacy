/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.dispenseContainerWizard = Csw.nbt.dispenseContainerWizard ||
        Csw.nbt.register('dispenseContainerWizard', function (cswParent, options) {
            'use strict';

            //#region Variable Declaration
            var cswPrivate = {
                ID: 'cswDispenseContainerWizard',
                nodeId: 'Unknown',
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
                var stepTwoComplete = false;
                return function () {
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

                        containerTypeTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'What kind of container would you like to use?' });

                        var containerTypeDiv = containerTypeTable.cell(1, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .div();

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
                                    ;
                                }
                            }
                        });

                        stepTwoComplete = true;
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
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    if (false === stepThreeComplete) {
                        if (cswPrivate.dispenseType === 'Dispense') {
                            stepThreeComplete = true; //do step 3B
                        }
                        else {
                            stepThreeComplete = true; //do step 3A
                        }
                        stepThreeComplete = true;
                    }
                };
            } ());

            cswPrivate.handleNext = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_DispenseContainer.step2.step:
                        if (stepTwoNeeded()) {
                            cswPrivate.makeStepTwo();
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
                        if (stepTwoNeeded()) {
                            cswPrivate.makeStepTwo();
                        }
                        else {
                            cswPrivate.wizard.previous.click();
                        }
                        break;
                }
            };

            var stepTwoNeeded = function () {
                var doStepTwo = true;
                if (cswPrivate.dispenseType !== 'Dispense') {
                    doStepTwo = false;
                }
                else {
                    var numOfContianerNodeTypes = 1;
                    //if (Csw.number(nodeTypeCount) <= 1) {
                    //    doStepTwo = false;
                    //}
                    //TODO - figure out how to call nodetypeSelect here so that we can determine correct nodeTypeCount 
                }
                return doStepTwo;
            }

            cswPrivate.onConfirmFinish = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                var designGrid = 'undefined';

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
                        //TODO - set up the right view
                        var views = data.views,
                            values = [];
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

