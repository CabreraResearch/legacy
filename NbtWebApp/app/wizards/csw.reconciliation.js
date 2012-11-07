/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    var cswReconciliationWizardStateName = 'cswReconciliationWizardStateName';

    Csw.nbt.ReconciliationWizard = Csw.nbt.ReconciliationWizard ||
        Csw.nbt.register('ReconciliationWizard', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                name: 'cswReconciliationWizard',
                exitFunc: null,
                numOfSteps: 3,
                startingStep: 1,
                wizard: null,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '',
                divStep2: '',
                divStep3: '',
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                state: {
                    locationId: '',
                    includeChildLocations: false,
                    startDate: '',
                    endDate: ''
                }
            };

            var cswPublic = {};

            cswPrivate.reinitSteps = function (startWithStep) {
                cswPrivate.stepThreeComplete = false;
                if (startWithStep <= 2) {
                    cswPrivate.stepTwoComplete = false;
                }
            };

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
                return false;
            };

            cswPrivate.toggleStepButtons = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, Csw.bool(cswPrivate.currentStepNo > 1));
                cswPrivate.toggleButton(cswPrivate.buttons.finish, Csw.bool(cswPrivate.currentStepNo === cswPrivate.numOfSteps));
                cswPrivate.toggleButton(cswPrivate.buttons.next, Csw.bool(cswPrivate.currentStepNo < cswPrivate.numOfSteps));
            };

            //State Functions
            cswPrivate.validateState = function () {
                var state;
                if (Csw.isNullOrEmpty(cswPrivate.state.locationId)) {
                    state = cswPrivate.getState();
                    Csw.extend(cswPrivate.state, state);
                }
                cswPrivate.setState();
            };

            cswPrivate.getState = function () {
                return Csw.clientDb.getItem(cswPrivate.name + '_' + cswReconciliationWizardStateName);
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_' + cswReconciliationWizardStateName, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_' + cswReconciliationWizardStateName);
            };

            //Step 1: Locations and Dates
            cswPrivate.makeStep1 = (function () {
                cswPrivate.stepOneComplete = false;
                return function () {
                    cswPrivate.toggleStepButtons();

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.label({
                            text: "Choose the location of the Containers you wish to examine, and select a timeline with which to filter Reconciliation Scans.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 4 });
                        //TODO - create controls for location (with checkbox to include child locations), start date (blank), and end date (Today)
                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: Statistics
            cswPrivate.makeStep2 = (function () {
                cswPrivate.stepTwoComplete = false;
                return function () {
                    cswPrivate.toggleStepButtons();

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();
                        cswPrivate.divStep2.label({
                            text: "Container statistics in (location) from (start date) to (end date)",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 4 });
                        //TODO - take values from step 1 control and create grid of statuses in selected location(s) within selected timeframe
                        cswPrivate.stepTwoComplete = true;
                    }
                };
            } ());

            //Step 3: Containers
            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;
                return function () {
                    cswPrivate.toggleStepButtons();

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();
                        cswPrivate.divStep3.label({
                            text: "Containers and their statuses in (location) from (start date) to (end date)",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep3.br({ number: 4 });
                        //TODO - take values from step 1 control and create grid of containers with their most current containerlocation status
                        //TODO - if end time = Today, include an ActionSelect control in each row, along with a save button outside the grid to update action
                        cswPrivate.stepThreeComplete = true;
                    }
                };
            } ());

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.validateState();
                cswPrivate.wizardSteps = {
                    1: 'Choose Location and Dates',
                    2: 'Statistics',
                    3: 'Containers'
                };
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    cswPrivate.setState();
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();
                    }
                };

                cswPrivate.finalize = function () {
                    //TODO - Save any unsaved container statuses left over from step 3
                    Csw.tryExec(cswPrivate.onFinish);
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'Reconciliation',
                    StepCount: cswPrivate.numOfSteps,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: function () {
                        cswPrivate.clearState();
                        Csw.tryExec(cswPrivate.onCancel);
                    },
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

            } ());

            cswPrivate.makeStep1();

            return cswPublic;
        });
} ());