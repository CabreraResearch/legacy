/// <reference path="~/js/CswNbt-vsdoc.js" />
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
                state: {
                    materialId: null,
                    materialNodeTypeId: '',
                    containerNodeTypeId: '',
                    containerAddLayout: {},
                    tradeName: '',
                    quantities: [],
                    selectedSizeId: '',
                    customBarcodes: false
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
                    $.extend(cswPrivate.state, state);
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
                    $.extend(cswPrivate, options);
                }

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
                }
                cswPrivate.validateState();

                cswPrivate.wizardSteps = {
                    1: 'Create Containers',
                    2: 'Define Properties'
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

                cswPrivate.getQuantity = function (async) {
                    var ret = Csw.bool(async);                    
                    //We may need to block (async==false) if we're validating prior to changing steps.
                    Csw.ajax.post({
                        urlMethod: 'getQuantity',
                        async: Csw.bool(async),
                        data: { SizeId: cswPrivate.state.selectedSizeId, Action: 'Receive' },
                        success: function (data) {
                            cswPrivate.state.quantity = data;
                            ret = false === Csw.isNullOrEmpty(cswPrivate.state.quantity);
                        }
                    });
                    return ret;
                };

                cswPrivate.finalize = function () {
                    var container = {
                        materialid: cswPrivate.state.materialId,
                        containernodetypeid: cswPrivate.state.containerNodeTypeId,
                        quantities: cswPrivate.amountsGrid.quantities,
                        sizeid: cswPrivate.state.selectedSizeId,
                        props: cswPrivate.tabsAndProps.getPropJson()
                    };

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
                    StepCount: 2,
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
                        return cswPrivate.state.quantities && cswPrivate.state.quantities.length > 0 && false === Csw.isNullOrEmpty(cswPrivate.state.selectedSizeId);
                    };

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.span({ text: 'Select a Size of ' + cswPrivate.state.tradeName + ' to receive. Then define the container quantities to create.' });
                        cswPrivate.divStep1.br({ number: 2 });

                        //If multiple container nodetypes exist
                        cswPrivate.container = {};
                        var containerSelect = Csw.nbt.wizard.nodeTypeSelect(cswPrivate.divStep1, {
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
                                makeSizeSelect();
                            }
                        });

                        var makeSizeSelect = function () {

                            cswPrivate.sizeDiv = cswPrivate.sizeDiv || cswPrivate.divStep1.div();
                            cswPrivate.sizeDiv.empty();

                            cswPrivate.sizeDiv.span({ text: '<b>Pick a Size:</b>' });
                            cswPrivate.sizeDiv.br({ number: 2 });

                            cswPrivate.sizeSelect = cswPrivate.sizeDiv.nodeSelect({
                                ID: cswPrivate.wizard.makeStepId('sizes'),
                                objectClassName: 'SizeClass',
                                relatedTo: {
                                    objectClassName: 'MaterialClass',
                                    nodeId: cswPrivate.state.materialId
                                },
                                onSuccess: function () {
                                    makeAmountsGrid();
                                    makeBarcodeCheckBox();
                                },
                                onSelect: function () {
                                    makeAmountsGrid();
                                    makeBarcodeCheckBox();
                                }
                            });
                        };

                        var makeAmountsGrid = function () {
                            cswPrivate.state.selectedSizeId = cswPrivate.sizeSelect.selectedNodeId();
                            cswPrivate.getQuantity(false);

                            cswPrivate.amountsDiv = cswPrivate.amountsDiv || cswPrivate.divStep1.div();
                            cswPrivate.amountsDiv.empty();

                            cswPrivate.amountsGrid = Csw.nbt.wizard.amountsGrid(cswPrivate.amountsDiv, {
                                ID: cswPrivate.wizard.makeStepId('wizardAmountsThinGrid'),
                                onAdd: function () {
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                                },
                                onDelete: function (qtyCnt) {
                                    if (qtyCnt < 1) {
                                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    }
                                },
                                quantity: cswPrivate.state.quantity,
                                containerlimit: cswPrivate.state.containerlimit,
                                makeId: cswPrivate.wizard.makeStepId,
                                materialId: cswPrivate.state.materialId,
                                action: 'Receive',
                                customBarcodes: cswPrivate.state.customBarcodes
                            });
                        };

                        var makeBarcodeCheckBox = function () {
                            cswPrivate.barcodeCheckBoxDiv = cswPrivate.barcodeCheckBoxDiv || cswPrivate.divStep1.div();
                            cswPrivate.barcodeCheckBoxDiv.empty();

                            var checkBoxTable = cswPrivate.barcodeCheckBoxDiv.table({
                                cellvalign: 'middle'
                            });
                            var printBarcodesCheckBox = checkBoxTable.cell(1, 1).checkBox({
                                onChange: Csw.method(function () {
                                    var val;
                                    if (printBarcodesCheckBox.checked()) {
                                        cswPrivate.printBarcodes = true;
                                    } else {
                                        cswPrivate.printBarcodes = false;
                                    }
                                })
                            });
                            checkBoxTable.cell(1, 2).span({ text: 'Print barcode labels for new containers' });
                        }

                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: Add Layout
            cswPrivate.makeStep2 = (function () {

                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.span({ text: '<b>Configure the new Container(s):</b>' });
                        cswPrivate.divStep2.br({ number: 2 });

                        cswPrivate.tabsAndProps = Csw.nbt.wizard.addLayout(cswPrivate.divStep2, {
                            ID: cswPrivate.state.containerNodeTypeId + 'add_layout',
                            nodetypeid: cswPrivate.state.containerNodeTypeId,
                            propertyData: cswPrivate.state.containerAddLayout
                        });

                        cswPrivate.stepTwoComplete = true;
                    }
                    window.setTimeout(function () {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    }, 250);
                };
            } ());

            (function _post() {
                cswPrivate.makeStep1();
            } ());
            return cswPublic;
        });
} ());