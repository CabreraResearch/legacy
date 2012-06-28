﻿/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    var cswReceiveMaterialWizardState = 'cswReceiveMaterialWizardState';

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
                materialNodeTypeId: '',
                containerNodeTypeId: '',
                containerAddLayout: {},
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
                if (Csw.isNullOrEmpty(cswPrivate.materialId) || Csw.isNullOrEmpty(cswPrivate.sizesViewId)) {
                    var _internal = Csw.clientDb.getItem(cswReceiveMaterialWizardState);
                    $.extend(true, cswPrivate, _internal);
                    if (Csw.isNullOrEmpty(cswPrivate.materialId)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a Material ID.', '', 'csw.receivematerial.js', 60));
                    }
                }
                Csw.clientDb.setItem(cswReceiveMaterialWizardState, cswPrivate);

                cswPrivate.wizardSteps = {
                    1: 'Select a Size',
                    2: 'Define Properties',
                    3: 'Set Amounts'
                };

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();
                        //Csw.clientDb.setItem(cswReceiveMaterialWizardState, cswPrivate);
                    }
                };

                cswPrivate.getQuantity = function (async) {
                    var ret = Csw.bool(async);
                    Csw.ajax.post({
                        urlMethod: 'getQuantity',
                        async: Csw.bool(async),
                        data: { SizeId: cswPrivate.selectedSizeId },
                        success: function (data) {
                            cswPrivate.quantity = data.quantity;
                            ret = false === Csw.isNullOrEmpty(cswPrivate.quantity);
                        }
                    });
                    return ret;
                };

                cswPrivate.onBeforeNext = function (currentStepNo) {
                    var isNotStepTwoOrIsValid = currentStepNo !== 2 || cswPrivate.tabsAndProps.isValid();
                    var isStepTwoAndHasQuantity = false;
                    if (currentStepNo === 2) {
                        if (false === Csw.isNullOrEmpty(cswPrivate.quantity)) {
                            isStepTwoAndHasQuantity = true;
                        } else {
                            isStepTwoAndHasQuantity = cswPrivate.getQuantity(false);
                        }
                    }
                    return isNotStepTwoOrIsValid || isStepTwoAndHasQuantity;
                };

                cswPrivate.finalize = function () {
                    var container = {
                        materialid: cswPrivate.materialId,
                        sizeid: cswPrivate.selectedSizeId,
                        props: cswPrivate.tabsAndProps.getPropJson(),
                        amounts: cswPrivate.amounts
                    };
                    //AJAX post
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
                    onBeforeNext: cswPrivate.onBeforeNext,
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
                        cswPrivate.divStep1.span({ text: 'Select a Size to Receive.' });
                        cswPrivate.divStep1.br({ number: 2 });

                        var sizeGrid = Csw.nbt.wizard.nodeGrid(cswPrivate.divStep1, {
                            nodeid: cswPrivate.materialId,
                            viewid: cswPrivate.sizesViewId,
                            onSelect: function () {
                                if (cswPrivate.selectedSizeId !== sizeGrid.getSelectedNodeId()) {
                                    cswPrivate.reinitSteps(2);
                                }
                                cswPrivate.selectedSizeId = sizeGrid.getSelectedNodeId();
                                cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                                //We are NOT fetching quantity here, because the user could be clicking back and forth between grid rows.
                            }
                        });

                        cswPrivate.container = {};
                        var containerSelect = Csw.nbt.wizard.nodeTypeSelect(cswPrivate.divStep1, {
                            labelText: 'Select a Container: ',
                            objectClassName: 'ContainerClass',
                            data: cswPrivate.container,
                            onSelect: function () {
                                if (cswPrivate.containerNodeTypeId !== containerSelect.selectedNodeTypeId) {
                                    cswPrivate.reinitSteps(2);
                                    cswPrivate.containerAddLayout = null;
                                }
                                cswPrivate.containerNodeTypeId = containerSelect.selectedNodeTypeId;
                            }
                        });

                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: Add Layout
            cswPrivate.makeStep2 = (function () {

                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepTwoComplete) {
                        //We're fetching quantity here so that it will be ready (hopefully) by the time we click next. 
                        //This does create a race condition, so we need to validate this on onBeforeClickNext
                        cswPrivate.getQuantity(true);
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();
                        cswPrivate.divStep2.span({ text: 'Configure the new Container: ' });
                        cswPrivate.divStep2.br({ number: 2 });

                        cswPrivate.tabsAndProps = Csw.nbt.wizard.addLayout(cswPrivate.divStep2, {
                            ID: cswPrivate.containerNodeTypeId + 'add_layout',
                            nodetypeid: cswPrivate.containerNodeTypeId,
                            propertyData: cswPrivate.containerAddLayout
                        });

                        cswPrivate.stepTwoComplete = true;
                    }
                };
            } ());

            //Step 3: Amounts
            cswPrivate.makeStep3 = (function () {

                return function () {
                    if (cswPrivate.tabsAndProps.isValid()) {
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                        if (false === cswPrivate.stepThreeComplete) {
                            if (Csw.isNullOrEmpty(cswPrivate.quantity)) {
                                Csw.error.throwException(Csw.error.exception('Cannot create a Container quantities without the Capacity if a Size.', '', 'csw.receivematerial.js', 244));
                            }
                            cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                            cswPrivate.divStep3.empty();
                            cswPrivate.divStep3.span({ text: 'Enter the Amounts to Receive.' });
                            cswPrivate.divStep3.br({ number: 2 });

                            cswPublic.thinGrid = cswPrivate.divStep3.thinGrid({ linkText: '', hasHeader: true, rows: [['#', 'Quantity', 'Barcode(s)']] });
                            cswPrivate.divStep3.br();
                            cswPrivate.amountsTable = cswPrivate.divStep3.table();
                            
                            //# of containers
                            
                            //Quantity

                            //Barcodes

                            cswPrivate.stepThreeComplete = true;
                        }
                    } else {
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                    }
                };

            } ());

            (function _post() {
                cswPrivate.makeStep1();
            } ());
            return cswPublic;
        });
} ());