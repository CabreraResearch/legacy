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
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                };
            } ());

            //Step 2. Select a Destination Container NodeType .
            //if( only one NodeType exists || Dispense Type != Dispense ) skip this step            
            cswPrivate.makeStepTwo = (function () {
                var stepTwoComplete = false;
                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                };
            } ());

            //Step 3. Select Amount
            //DispenseType != Dispense ? 
            //Select a Quantity :
            //Select the number of destination containers and their quantities.
            cswPrivate.makeStepThree = (function () {
                var stepThreeAComplete = false;
                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                };
            } ());

            //TODO - determine if we really need both handleNext and handlePrevious functions
            cswPrivate.handleNext = function (newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_DispenseContainer.step2.step:
                        cswPrivate.makeStepTwo();
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
                        cswPrivate.makeStepTwo();
                        break;
                }
            };

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

            //TODO - parse, refacotr, and rebuild makeGrid for dispensedContainers
            
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

