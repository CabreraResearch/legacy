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
                materialType: { name: '', val: '' },
                tradeName: '',
                supplier: { name: '', val: '' },
                partNo: '',
                useExistingMaterial: false
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
                        internal.lastStepNo = internal.currentStepNo;
                        internal.currentStepNo = newStepNo;
                        internal['makeStep' + newStepNo]();

                        if (internal.currentStepNo === 4 && 
                            internal.useExistingMaterial) {
                            if (internal.currentStepNo > internal.lastStepNo) {
                                internal.toggleButton(internal.buttons.next, true, true);
                            }
                            else if (internal.currentStepNo < internal.lastStepNo) {
                                internal.toggleButton(internal.buttons.prev, true, true);
                            }
                        }
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
                    function typeSelect() {
                        internal.materialType = { name: internal.materialTypeSelect.find(':selected').text(), val: internal.materialTypeSelect.val() };
                        internal.toggleButton(internal.buttons.next, true);
                    }
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
                            onSelect: typeSelect,
                            onSuccess: typeSelect
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
                        return false === Csw.isNullOrEmpty(internal.tradeName) && false === Csw.isNullOrEmpty(internal.supplier);
                    };
                    function supplierSelect() {
                        internal.supplier = { name: internal.supplierSelect.find(':selected').text(), val: internal.supplierSelect.val() };
                        internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                    }

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
                                internal.tradeName = internal.tradeNameInput.val();
                                internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                            }
                        });
                        internal.divStep2.br({ number: 1 });

                        /* SUPPLIER */
                        internal.supplierSelect = internal.divStep2.nodeSelect({
                            ID: internal.wizard.makeStepId('supplier'),
                            objectClassName: 'VendorClass',
                            labelText: 'Supplier: ',
                            onChange: supplierSelect,
                            onSuccess: supplierSelect
                        });
                        internal.divStep2.br({ number: 1 });

                        /* PARTNO */
                        internal.partNoInput = internal.divStep2.input({
                            ID: internal.wizard.makeStepId('partno'),
                            labelText: 'Part No: ',
                            onChange: function () {
                                internal.partNo = internal.partNoInput.val();
                            }
                        });

                        stepTwoComplete = true;
                    }
                };
            } ());

            internal.makeStep3 = (function () {
                var stepThreeComplete = false;

                return function () {
                    var div;
                    function makeConfirmation() {
                        div.p({ labelText: 'Tradename:', text: internal.tradeName });
                        div.p({ labelText: 'Supplier: ', text: internal.supplier.name });
                        if (false === Csw.isNullOrEmpty(internal.partNo)) {
                            div.p({ labelText: 'Part No: ', text: internal.partNo });
                        }
                    }

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, true);

                    if (false === stepThreeComplete) {
                        internal.divStep3 = internal.wizard.div(3);

                        internal.divStep3.br({ number: 2 });

                        div = internal.divStep3.div();


                        Csw.ajax.post({
                            urlMethod: 'getMaterial',
                            data: {
                                NodeTypeId: internal.materialType.val,
                                Tradename: internal.tradeName,
                                Supplier: internal.supplier.name,
                                PartNo: internal.partNo
                            },
                            success: function (data) {
                                var topText = '', bottomText = '';
                                if (false === Csw.isNullOrEmpty(data.tradename)) {
                                    internal.useExistingMaterial = true;
                                    topText = 'A material named ' + data.tradename + ' already exists as: ';
                                    bottomText = 'Click next to use this existing material.';
                                    internal.tradeName = data.tradename;
                                    internal.supplier.name = data.supplier;
                                    internal.partNo = data.partno;
                                } else {
                                    topText = 'Creating a new ' + internal.tradeName + ' material: ';
                                }
                                div.p({ text: topText });
                                makeConfirmation();
                                div.p({ text: bottomText });
                            }
                        });

                        stepThreeComplete = true;
                    }
                };

            } ());

            internal.makeStep4 = (function () {
                var stepFourComplete = false;

                return function () {
                    var div;

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, true);

                    if (false === stepFourComplete && 
                        false === internal.useExistingMaterial) {
                        internal.divStep4 = internal.wizard.div(4);

                        internal.divStep4.br({ number: 2 });

                        div = internal.divStep4.div();
                        internal.tabsAndProps = Csw.layouts.tabsAndProps(div, {
                            nodetypeid: internal.materialType.val,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['tradename', 'supplier', 'partno']
                        });

                        stepFourComplete = true;
                    }
                };

            } ());

            internal.makeStep1();

            return external;
        });
} ());