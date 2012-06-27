/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    var _internal = {
        lastReceivingMaterialId: 'lastReceivingMaterialId',
        lastReceivingViewId: 'lastReceivingViewId'
    };

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
                sizesViewId: '',
                selectedSizeId: '',
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

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
                }
                if (Csw.isNullOrEmpty(cswPrivate.materialId)) {
                    cswPrivate.materialId = Csw.clientDb.getItem(_internal.lastReceivingMaterialId);
                    if (Csw.isNullOrEmpty(cswPrivate.materialId)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a Material ID.', '', 'csw.receivematerial.js', 60));
                    }
                }
                Csw.clientDb.setItem(_internal.lastReceivingMaterialId, cswPrivate.materialId);

                if (Csw.isNullOrEmpty(cswPrivate.sizesViewId)) {
                    cswPrivate.sizesViewId = Csw.clientDb.getItem(_internal.lastReceivingViewId);
                    if (Csw.isNullOrEmpty(cswPrivate.sizesViewId)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a Sizes View.', '', 'csw.receivematerial.js', 68));
                    }
                }
                Csw.clientDb.setItem(_internal.lastReceivingViewId, cswPrivate.sizesViewId);

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

            } ()); //_preCtor

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

            //SIZES
            cswPrivate.makeStep1 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(cswPrivate.selectedSizeId);
                    };

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();
                        cswPrivate.divStep1.span({ text: 'Select a Size to Receive' });
                        cswPrivate.divStep1.br({ number: 2 });

                        var sizeGrid = Csw.nbt.wizardNodeGrid(cswPrivate.divStep1, {
                            nodeid: cswPrivate.materialId,
                            viewid: cswPrivate.sizesViewId,
                            onSelect: function () {
                                cswPrivate.selectedSizeId = sizeGrid.getSelectedNodeId();
                                cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                            }
                        });

                        //var containerDiv = cswPrivate.divStep1.div().hide();
                        //var containerSelect = containerDiv.nodeTypeSelect();

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