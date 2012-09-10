/// <reference path="~/app/CswApp-vsdoc.js" />


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
                state: {
                    materialId: null,
                    materialNodeTypeId: '',
                    containerNodeTypeId: '',
                    containerAddLayout: {},
                    tradeName: '',
                    selectedSizeId: '',
                    customBarcodes: false,
                    documentTypeId: '',
                    documentId: ''
                },
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                printBarcodes: false,
                amountsGrid: null
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

            cswPrivate.validateState = function () {
                var state;
                if (Csw.isNullOrEmpty(cswPrivate.state.materialId)) {
                    state = cswPrivate.getState();
                    Csw.extend(cswPrivate.state, state);
                    if (Csw.isNullOrEmpty(cswPrivate.state.materialId)) {
                        Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a Material ID.', '', 'csw.receivematerial.js', 60));
                    }
                }
                cswPrivate.setState();
            };

            cswPrivate.getState = function () {
                var ret = Csw.clientDb.getItem(cswPrivate.ID + '_' + cswReceiveMaterialWizardState);
                return ret;
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.ID + '_' + cswReceiveMaterialWizardState, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.ID + '_' + cswReceiveMaterialWizardState);
            };

            //ctor preInit
            (function _pre() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
                }
                cswPrivate.validateState();

                cswPrivate.wizardSteps = {
                    1: 'Create Containers',
                    2: 'Define Properties',
                    3: 'Attach MSDS'
                };
                cswPrivate.state.containerlimit = Csw.number(cswPrivate.state.containerlimit, 25);
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    cswPrivate.setState();
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();
                        //Csw.clientDb.setItem(cswReceiveMaterialWizardState, cswPrivate);
                    }
                };

                cswPrivate.finalize = function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    var container = {
                        materialid: cswPrivate.state.materialId,
                        containernodetypeid: cswPrivate.state.containerNodeTypeId,
                        quantities: cswPrivate.amountsGrid.quantities,
                        sizeid: cswPrivate.state.selectedSizeId,
                        props: cswPrivate.tabsAndProps.getPropJson(),
                        documentid: cswPrivate.state.documentId
                    };
                    if(false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                        container.documentProperties = cswPrivate.documentTabsAndProps.getPropJson();
                    }
                    Csw.ajax.post({
                        urlMethod: 'receiveMaterial',
                        data: { ReceiptDefinition: Csw.serialize(container) },
                        success: function (data) {
                            if (Csw.number(data.containerscreated) < 1) {
                                Csw.error.throwException(Csw.error.exception('Failed to create any containers.'));
                            } else {
                                Csw.tryExec(cswPrivate.onFinish, data.viewid);
                                if (false === Csw.isNullOrEmpty(data.barcodeId)) {
                                    if (cswPrivate.printBarcodes) {
                                        $.CswDialog('PrintLabelDialog', { 'nodeid': data.containerId, 'propid': data.barcodeId });
                                    }
                                }
                            }
                        }
                    });
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    ID: Csw.makeId(cswPrivate.ID, 'wizard'),
                    Title: 'Receive: ' + cswPrivate.state.tradeName,
                    StepCount: 3,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onBeforeNext: function () {
                        var ret = cswPrivate.amountsGrid.amountsGridOnAdd();
                        return ret;
                    },
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
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.label({
                            text: 'Select the number of containers and their quantities to receive.',
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 4 });

                        //If multiple container nodetypes exist
                        cswPrivate.container = {};
                        var containerSelect = Csw.wizard.nodeTypeSelect(cswPrivate.divStep1, {
                            labelText: 'Select a Container: ',
                            objectClassName: 'ContainerClass',
                            data: cswPrivate.state.container,
                            onSelect: function () {
                                if (cswPrivate.state.containerNodeTypeId !== containerSelect.selectedNodeTypeId) {
                                    cswPrivate.reinitSteps(2);
                                    cswPrivate.state.containerAddLayout = null;
                                }
                                cswPrivate.state.containerNodeTypeId = containerSelect.selectedNodeTypeId;
                            },
                            onSuccess: function (types, nodetypecount) {
                                makeAmountsGrid();
                                makeBarcodeCheckBox();
                            }
                        });

                        var makeAmountsGrid = function () {
                            cswPrivate.amountsDiv = cswPrivate.amountsDiv || cswPrivate.divStep1.div();
                            cswPrivate.amountsDiv.empty();

                            cswPrivate.amountsGrid = Csw.wizard.amountsGrid(cswPrivate.amountsDiv, {
                                ID: cswPrivate.wizard.makeStepId('wizardAmountsThinGrid'),
                                quantity: cswPrivate.state.quantity,
                                containerlimit: cswPrivate.state.containerlimit,
                                makeId: cswPrivate.wizard.makeStepId,
                                materialId: cswPrivate.state.materialId,
                                action: 'Receive',
                                customBarcodes: cswPrivate.state.customBarcodes
                            });
                        };

                        var makeBarcodeCheckBox = function() {
                            cswPrivate.barcodeCheckBoxDiv = cswPrivate.barcodeCheckBoxDiv || cswPrivate.divStep1.div();
                            cswPrivate.barcodeCheckBoxDiv.empty();

                            var checkBoxTable = cswPrivate.barcodeCheckBoxDiv.table({
                                cellvalign: 'middle'
                            });
                            var printBarcodesCheckBox = checkBoxTable.cell(1, 1).checkBox({
                                onChange: Csw.method(function() {
                                    var val;
                                    if (printBarcodesCheckBox.checked()) {
                                        cswPrivate.printBarcodes = true;
                                    } else {
                                        cswPrivate.printBarcodes = false;
                                    }
                                })
                            });
                            checkBoxTable.cell(1, 2).span({ text: 'Print barcode labels for new containers' });
                        };

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
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.label({
                            text: 'Configure the properties to apply to all containers upon receipt.',
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 4 });

                        cswPrivate.tabsAndProps = Csw.wizard.addLayout(cswPrivate.divStep2, {
                            ID: cswPrivate.state.containerNodeTypeId + 'add_layout',
                            nodetypeid: cswPrivate.state.containerNodeTypeId,
                            propertyData: cswPrivate.state.containerAddLayout
                        });

                        cswPrivate.stepTwoComplete = true;
                    }
                };
            } ());

            //Step 3: MSDS upload
            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;

                return function () {
                    var div;

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();

                        if (Csw.isNullOrEmpty(cswPrivate.state.documentTypeId)) {
                            cswPrivate.divStep3.label({
                                text: 'No Material Documents have been defined. Click Finish to complete the wizard.',
                                cssclass: 'wizardHelpDesc'
                            });
                        } else {

                            cswPrivate.divStep3.label({
                                text: 'Define a Material Safety Data Sheet to attach to ' + cswPrivate.state.tradeName,
                                cssclass: 'wizardHelpDesc'
                            });
                            cswPrivate.divStep3.br({ number: 4 });

                            div = cswPrivate.divStep3.div();

                            cswPrivate.documentTabsAndProps = Csw.layouts.tabsAndProps(div, {
                                nodetypeid: cswPrivate.state.documentTypeId,
                                showSaveButton: false,
                                EditMode: Csw.enums.editMode.Add,
                                ReloadTabOnSave: false,
                                ShowAsReport: false,
                                excludeOcProps: ['owner'],
                                onNodeIdSet: function (documentId) {
                                    cswPrivate.state.documentId = documentId;
                                }
                            });

                        }

                        cswPrivate.stepThreeComplete = true;
                    }
                };

            }());


            (function _post() {
                cswPrivate.makeStep1();
            } ());
            return cswPublic;
        });
} ());