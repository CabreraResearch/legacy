/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.receiveMaterialWizard = Csw.nbt.receiveMaterialWizard ||
        Csw.nbt.register('receiveMaterialWizard', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                ID: 'cswReceiveMaterialWizard',
                exitFunc: null, //function ($wizard) {},
                startingStep: 1,
                wizard: null,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '', divStep2: '', divStep3: '',
                materialId: null,
                tradeName: '',
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false
            };

            var cswPublic = {};

            cswPrivate.reinitSteps = function (startWithStep) {
                cswPrivate.stepThreeComplete = false;

                if (startWithStep <= 2) {
                    cswPrivate.stepTwoComplete = false;

                    if (startWithStep <= 1) {
                        /* This is mostly for debugging, you probably never need to reset step 1 in practice */
                        cswPrivate.stepOneComplete = false;
                    }
                }

            };

            //ctor preInit
            (function _pre() {
                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswPrivate.wizardSteps = {
                    1: 'Size',
                    2: 'Property',
                    3: 'Amout'
                };

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();

                        //                        if (cswPrivate.currentStepNo === 4 &&
                        //                            cswPrivate.useExistingMaterial) {
                        //                            if (cswPrivate.currentStepNo > cswPrivate.lastStepNo) {
                        //                                cswPrivate.toggleButton(cswPrivate.buttons.next, true, true);
                        //                            }
                        //                            else if (cswPrivate.currentStepNo < cswPrivate.lastStepNo) {
                        //                                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                        //                            }
                        //                        }
                    }
                };

                cswPrivate.finalize = function () {

                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    ID: Csw.makeId(cswPrivate.ID, 'wizard'),
                    Title: 'Receive: ' + cswPrivate.tradeName,
                    StepCount: 3,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: cswPrivate.onCancel,
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

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
                return false;
            };

            cswPrivate.makeStep1 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        //return false === Csw.isNullOrEmpty(cswPrivate.materialType);
                    };

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.br({ number: 2 });

                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: 
            cswPrivate.makeStep2 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        //return false === Csw.isNullOrEmpty(cswPrivate.tradeName) && false === Csw.isNullOrEmpty(cswPrivate.supplier.val);
                    };

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.br({ number: 2 });

                        cswPrivate.stepTwoComplete = true;
                    }
                };
            } ());

            cswPrivate.makeStep3 = (function () {

                return function () {

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();

                        cswPrivate.divStep3.br({ number: 2 });

                        cswPrivate.stepThreeComplete = true;
                    }
                };

            } ());

            (function _post() {
                cswPrivate.makeStep1();
            } ());
            return cswPublic;
        });
} ());