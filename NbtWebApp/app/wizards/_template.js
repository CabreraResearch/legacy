
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    if (false) { //remove this when you're ready to use the template

        Csw.nbt.template = Csw.nbt.template ||
            Csw.nbt.register('template', function(cswParent, cswPrivate) {
                'use strict';

                //#region Variable Declaration

                var cswPublic = cswParent.div();

                //#endregion Variable Declaration

                //#region Pre-ctor

                (function _preCtor() {
                    //set default values on cswPrivate if none are supplied
                    cswPrivate.name = cswPrivate.name || 'Wizard Template';
                    cswPrivate.onFinish = cswPrivate.onFinish || function _onFinish() {
                    };
                    cswPrivate.onCancel = cswPrivate.onCancel || function _onCancel() {
                    };
                    cswPrivate.startingStep = cswPrivate.startingStep || 1;
                    cswPrivate.wizardSteps = cswPrivate.wizardSteps || { 1: 'Step 1', 2: 'Step 2' };
                    cswPrivate.stepCount = cswPrivate.stepCount || 2;
                    cswPrivate.buttons = cswPrivate.buttons || {
                        next: 'next',
                        prev: 'previous',
                        finish: 'finish',
                        cancel: 'cancel'
                    };
                    cswPrivate.divStep1 = cswPrivate.divStep1 || {};
                    cswPrivate.divStep2 = cswPrivate.divStep2 || {};
                    cswPrivate.wizard = cswPrivate.wizard || {};
                }());

                //#endregion Pre-ctor

                //#region Define Class Members

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.toggleButton = function(button, isEnabled, doClick) {
                    var btn;
                    if (Csw.bool(isEnabled)) {
                        btn = cswPrivate.wizard[button].enable();
                        if (Csw.bool(doClick)) {
                            btn.click();
                        }
                    } else {
                        cswPrivate.wizard[button].disable();
                    }
                    if (button !== cswPrivate.buttons.finish) {
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, (cswPrivate.currentStepNo === 5));
                    }

                    return false;
                };

                cswPrivate.handleNext = function(newStepNo) {
                    cswPrivate.currentStepNo = newStepNo;
                    switch (newStepNo) {
                    case cswPrivate.wizardSteps.step2:
                        cswPrivate.makeStepTwo();
                        break;
                    } // switch(newstepno)
                }; // handleNext()

                cswPrivate.handlePrevious = function(newStepNo) {
                    cswPrivate.currentStepNo = newStepNo;
                    switch (newStepNo) {
                    case cswPrivate.wizardSteps.step1:
                        cswPrivate.makeStepOne();
                        break;
                    }
                };

                cswPrivate.onFinishClick = function() {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    Csw.ajax.post({
                        urlMethod: '',
                        data: {},
                        success: function(data) {

                        },
                        error: function() {
                            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                        }
                    });
                };

                cswPrivate.onCancelClick = function() {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    Csw.ajax.post({
                        urlMethod: '',
                        data: {},
                        success: function(data) {

                        },
                        error: function() {
                            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                        }
                    });
                };

                //#endregion Define Class Members

                //#region Define Wizard Steps

                //Step 1. 
                cswPrivate.makeStepOne = (function() {
                    var stepOneComplete = false;
                    return function() {

                        if (false === stepOneComplete) {
                            cswPrivate.divStep1 = cswPrivate.wizard.div(cswPrivate.wizardSteps.step1);
                            cswPrivate.divStep1.br();


                            stepOneComplete = true;
                        } // if (false === stepOneComplete)

                        cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                    };
                }());

                //Step 2. 
                cswPrivate.makeStepTwo = (function() {
                    var stepTwoComplete = false;

                    return function() {

                        if (false === stepTwoComplete) {
                            cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(cswPrivate.wizardSteps.step1);
                            cswPrivate.divStep2.empty();
                            cswPrivate.divStep2.br();
                        }
                        stepTwoComplete = true;
                    };
                }());

                //#endregion Define Wizard Steps

                //#region Post-ctor

                (function() {

                    cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                        name: 'wizard',
                        Title: 'Wizard Template',
                        StepCount: cswPrivate.StepCount,
                        Steps: cswPrivate.wizardSteps,
                        StartingStep: cswPrivate.startingStep,
                        FinishText: 'Finish',
                        onNext: cswPrivate.handleNext,
                        onPrevious: cswPrivate.handlePrevious,
                        onCancel: cswPrivate.onCancelClick,
                        onFinish: cswPrivate.onFinishClick,
                        doNextOnInit: false
                    });

                    cswPrivate.makeStepOne();
                }());

                //#endregion Post-ctor

                return cswPublic;
            });
    }
} ());

