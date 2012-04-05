/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
        Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
            'use strict';

            var internal = {
                ID: 'cswCreateMaterialWizard',
                exitFunc: null, //function ($wizard) {},
                startingStep: 1,
                wizard: null,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '', divStep2: '', divStep3: '', divStep4: '', divStep5: '',
                materialTypeSelect: null,
                materialType: ''
            };

            var external = {};

            (function () {
                if (options) {
                    $.extend(internal, options);
                }

                internal.wizardSteps = {
                    1: 'Choose Type',
                    2: 'Identity',
                    3: 'Validate',
                    4: 'Properties',
                    5: 'Size(s)'
                };

                internal.currentStepNo = internal.startingStep;

                internal.handleStep = function (newStepNo) {
                    if (Csw.contains(internal, 'makeStep' + newStepNo)) {
                        internal.currentStepNo = newStepNo;
                        internal['makeStep' + newStepNo]();
                    }
                };

                internal.wizard = Csw.layouts.wizard(cswParent.div(), {
                    ID: Csw.makeId(internal.ID, 'wizard'),
                    Title: 'Create Material',
                    StepCount: 5,
                    Steps: internal.wizardSteps,
                    StartingStep: internal.startingStep,
                    FinishText: 'Finish',
                    onNext: internal.handleStep,
                    onPrevious: internal.handleStep,
                    onCancel: internal.onCancel,
                    onFinish: internal.onFinish,
                    doNextOnInit: false
                });

            } ());


            internal.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = internal.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    internal.wizard[button].disable();
                }
                return false;
            };

            internal.makeStep1 = (function () {
                var stepOneComplete = false;

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(internal.materialType);
                    };

                    internal.toggleButton(internal.buttons.prev, false);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === stepOneComplete) {
                        internal.divStep1 = internal.wizard.div(1);

                        internal.divStep1.br({ number: 2 });

                        internal.materialTypeSelect = internal.divStep1.nodeTypeSelect({
                            ID: internal.wizard.makeStepId('nodeTypeSelect'),
                            labelText: 'Select a Material Type: ',
                            objectClassName: 'MaterialClass',
                            onSelect: function () {
                                internal.materialType = { name: internal.materialTypeSelect.text(), val: internal.materialTypeSelect.val() };
                                internal.toggleButton(internal.buttons.next, true);
                                //                                    var selected = internal.inspectionTargetSelect.find(':selected');
                                //                                    internal.isNewTarget(selected.propNonDom('data-newNodeType'));
                                //                                    internal.selectedInspectionTarget = selected.text();
                                //                                    Csw.publish(internal.createInspectionEvents.targetNameChanged);
                            },
                            onSuccess: function (data) {
                                //                                    onNodeTypeSelectSuccess(data);
                                internal.materialType = { name: internal.materialTypeSelect.text(), val: internal.materialTypeSelect.val() };
                                internal.toggleButton(internal.buttons.next, true);
                                //                                    internal.selectedInspectionTarget = internal.inspectionTargetSelect.find(':selected').text();
                            }
                        });

                        stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: 
            internal.makeStep2 = (function () {
                var stepTwoComplete = false;

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(internal.tradename) && false === Csw.isNullOrEmpty(internal.supplier);
                    };

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === stepTwoComplete) {
                        internal.divStep2 = internal.wizard.div(2);

                        internal.divStep2.br({ number: 2 });

                        /* TRADENAME */
                        internal.tradeNameInput = internal.divStep2.input({
                            ID: internal.wizard.makeStepId('tradename'),
                            labelText: 'Tradename: ',
                            onChange: function () {
                                internal.tradename = internal.tradeNameInput.val();
                                internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                            }
                        });
                        internal.divStep2.br({ number: 1 });

                        /* SUPPLIER */
                        internal.supplierSelect = internal.divStep2.nodeSelect({
                            ID: internal.wizard.makeStepId('supplier'),
                            objectClassName: 'VendorClass',
                            labelText: 'Supplier: ',
                            onChange: function () {
                                internal.supplier = { name: internal.supplierSelect.text(), val: internal.supplierSelect.val() };
                                internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                            }
                        });
                        internal.divStep2.br({ number: 1 });

                        /* PARTNO */
                        internal.partNoInput = internal.divStep2.input({
                            ID: internal.wizard.makeStepId('partno'),
                            labelText: 'Part No: ',
                            onChange: function () {
                                internal.partno = internal.partNoInput.val();
                            }
                        });
                        
                        stepTwoComplete = true;
                    }
                };
            } ());

            internal.makeStep1();

            return external;
        });
} ());