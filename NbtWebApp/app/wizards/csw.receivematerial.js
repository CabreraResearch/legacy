/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    var cswReceiveMaterialWizardState = 'cswReceiveMaterialWizardState';

    Csw.nbt.receiveMaterialWizard = Csw.nbt.receiveMaterialWizard ||
        Csw.nbt.register('receiveMaterialWizard', function (cswParent, options) {
            'use strict';
            
            //#region Properties
            var cswPrivate = {
                name: 'cswReceiveMaterialWizard',
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
                    containerNodeId: '',
                    containerNodeTypeId: '',
                    containerAddLayout: {},
                    tradeName: '',
                    selectedSizeId: '',
                    customBarcodes: false,
                    nodetypename: '',
                    canAddSDS: true,
                    sdsDocs: [{
                        revisiondate: '',
                        displaytext: '',
                        linktext: ''
                    }],
                    documentTypeId: '',
                    documentId: ''
                },
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                printBarcodes: true,
                amountsGrid: null,
                saveError: false
            };

            var cswPublic = {};           
            //#endregion Properties
            
            //#region Wizard Functions
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
            //#endregion Wizard Functions
            
            //#region State Functions
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
                var ret = Csw.clientDb.getItem(cswPrivate.name + '_' + cswReceiveMaterialWizardState);
                return ret;
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_' + cswReceiveMaterialWizardState, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_' + cswReceiveMaterialWizardState);
            };           
            //#endregion State Functions

            //#region ctor preInit
            (function _pre() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
                }
                cswPrivate.validateState();

                var StepCount = 2;

                cswPrivate.wizardSteps = {
                    1: 'Create Containers',
                    2: 'Define Properties'
                };
                if(cswPrivate.state.canAddSDS) {
                    cswPrivate.wizardSteps[3] = 'Attach SDS';
                    StepCount = 3;
                }
                cswPrivate.state.containerlimit = Csw.number(cswPrivate.state.containerlimit, 25);
                cswPrivate.currentStepNo = cswPrivate.startingStep;
                


                cswPrivate.handleStep = function (newStepNo) {
                    cswPrivate.setState();
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();

                        if (cswPrivate.currentStepNo === 3) {
                            if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps) && cswPrivate.currentStepNo > 2) {
                                cswPrivate.state.properties = cswPrivate.tabsAndProps.getProps();
                                if (cswPrivate.lastStepNo === 2) {
                                    //TODO: remove this
                                    cswPrivate.tabsAndProps.callDeprecatedSaveMethod({}, '', null, false, false);
                                    if (cswPrivate.saveError === true) {
                                        cswPrivate.saveError = false;
                                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                                        cswPrivate.reinitSteps(2);
                                    }
                                }
                            }
                        }
                    }
                }; //cswPrivate.handleStep

                cswPrivate.finalize = function() {

                    var canReceiveMaterial = true;
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    if (false === cswPrivate.state.canAddSDS) {
                        if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps)) {
                            cswPrivate.state.properties = cswPrivate.tabsAndProps.getProps();
                            if (cswPrivate.lastStepNo === 1) {
                                //TODO: Remove this
                                cswPrivate.tabsAndProps.callDeprecatedSaveMethod({}, '', null, false, false);
                                if (cswPrivate.saveError === true) {
                                    cswPrivate.saveError = false;
                                    canReceiveMaterial = false;
                                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true, false);
                                    cswPrivate.reinitSteps(2);
                                }
                            }
                        }
                    }

                    if (canReceiveMaterial) {
                        var container = {
                            containernodeid: cswPrivate.state.containerNodeId,
                            materialid: cswPrivate.state.materialId,
                            containernodetypeid: cswPrivate.state.containerNodeTypeId,
                            documentid: cswPrivate.state.documentId,
                            quantities: cswPrivate.amountsGrid.quantities(),
                            sizeid: cswPrivate.state.selectedSizeId,
                            props: cswPrivate.state.properties
                        };
                        if (false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                            container.documentProperties = cswPrivate.documentTabsAndProps.getProps();
                        }
                        Csw.ajax.post({
                            urlMethod: 'receiveMaterial',
                            data: { ReceiptDefinition: Csw.serialize(container) },
                            success: function(data) {
                                if (Csw.number(data.containerscreated) < 1) {
                                    Csw.error.throwException(Csw.error.exception('Failed to create any containers.'));
                                } else {
                                    Csw.tryExec(cswPrivate.onFinish, data.viewid);
                                    if (cswPrivate.printBarcodes) {
                                        if (false === Csw.isNullOrEmpty(data.barcodes) &&
                                            Object.keys(data.barcodes).length > 0) {

                                            $.CswDialog('PrintLabelDialog', {
                                                nodes: data.barcodes,
                                                nodetypeid: cswPrivate.state.containerNodeTypeId
                                            });
                                        } else {
                                            //handle warning
                                        }
                                    }
                                }
                            }
                        });
                    }

                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'Receive: ' + cswPrivate.state.tradeName,
                    StepCount: StepCount,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onBeforeNext: function () {
                        var ret = Csw.bool(cswPrivate.amountsGrid.containerCount > 0 &&
                            cswPrivate.amountsGrid.containerCount <= cswPrivate.amountsGrid.containerlimit);
                        return ret;
                    },
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: cswPrivate.onCancel,
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

            }());
            //#endregion ctor preInit

            //#region Step 1: Create Containers
            cswPrivate.makeStep1 = (function () {

                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.span({
                            text: 'Select the number of containers and their quantities to receive.',
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 2 });

                        var tbl = cswPrivate.divStep1.table({
                            FirstCellRightAlign: true
                        });

                        //If multiple container nodetypes exist
                        cswPrivate.container = {};

                        var label = tbl.cell(1, 1).span().hide();
                        var containerSelect = Csw.wizard.nodeTypeSelect(tbl.cell(1, 2), {
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
                                if (nodetypecount > 1) {
                                    label.show();
                                    label.setLabelText('Select a Container: ', true);
                                }
                            }
                        });

                        var makeAmountsGrid = function () {
                            cswPrivate.amountsDiv = cswPrivate.amountsDiv || cswPrivate.divStep1.div();
                            cswPrivate.amountsDiv.empty();

                            cswPrivate.amountsGrid = Csw.wizard.amountsGrid(cswPrivate.amountsDiv, {
                                name: 'wizardAmountsThinGrid',
                                quantity: cswPrivate.state.quantity,
                                containerlimit: cswPrivate.state.containerlimit,
                                materialId: cswPrivate.state.materialId,
                                action: 'Receive',
                                customBarcodes: cswPrivate.state.customBarcodes,
                                nodeTypeName: cswPrivate.state.nodetypename
                            });
                        };

                        var makeBarcodeCheckBox = function () {
                            cswPrivate.barcodeCheckBoxDiv = cswPrivate.barcodeCheckBoxDiv || cswPrivate.divStep1.div();
                            cswPrivate.barcodeCheckBoxDiv.empty();

                            var checkBoxTable = cswPrivate.barcodeCheckBoxDiv.table({
                                cellvalign: 'middle'
                            });
                            var printBarcodesCheckBox = checkBoxTable.cell(1, 1).checkBox({
                                checked: true,
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
                        };

                        cswPrivate.stepOneComplete = true;
                    }
                };
            }());
            //#endregion Step 1: Create Containers

            //#region Step 2: Define Properties
            cswPrivate.makeStep2 = (function () {

                return function () {
                    var isLastStep = Csw.bool(false === cswPrivate.state.canAddSDS);
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.label({
                            text: 'Configure the properties to apply to all containers upon receipt.',
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 4 });

                        cswPrivate.tabsAndProps = Csw.wizard.addLayout(cswPrivate.divStep2, {
                            name: cswPrivate.state.containerNodeTypeId + 'add_layout',
                            excludeOcProps: ['save'],
                            tabState: {
                                propertyData: cswPrivate.state.containerAddLayout,
                                removeTempStatus: false,
                                nodetypeid: cswPrivate.state.containerNodeTypeId,
                                nodeid: cswPrivate.state.containerNodeId
                            },
                            //TODO: client-side property specific change events are evil. Remove this demon.
                            onOwnerPropChange: function (propObj, data, tabContentDiv) {
                                //TODO: Remove this
                                cswPrivate.tabsAndProps.callDeprecatedSaveMethod(tabContentDiv, data.tabid, null, false, true);
                            },
                            onSaveError: function (errorData) {
                                console.log(errorData);
                                cswPrivate.saveError = true;
                            }

                        });

                        cswPrivate.stepTwoComplete = true;
                    }
                };
            }());
            //#endregion Step 2: Define Properties

            //#region Step 3: Attach SDS
            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;

                return function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();

                        cswPrivate.divStep3.span({
                            text: 'Define a Safety Data Sheet to attach to ' + cswPrivate.state.tradeName,
                            cssclass: 'wizardHelpDesc'
                        });
                        cswPrivate.divStep3.br({ number: 2 });

                        var SDSTable = cswPrivate.divStep3.table(),
                            SDSAddCell = SDSTable.cell(1, 1).css({ width: '450px' }),
                            SDSGridCell = SDSTable.cell(1, 2).css({ width: '550px' }),
                            attachSDSTable = SDSAddCell.table();
                        
                        attachSDSTable.cell(1, 1).a({
                            text: 'Add a new SDS Document',
                            onClick: function () {
                                attachSDSTable.cell(1, 1).hide();
                                attachSDSTable.cell(1, 2).show();
                            }
                        });
                        attachSDSTable.cell(1, 2).hide();

                        cswPrivate.documentTabsAndProps = Csw.layouts.tabsAndProps(attachSDSTable.cell(1, 2), {
                            tabState: {
                                excludeOcProps: ['owner', 'save'],
                                ShowAsReport: false,
                                nodetypeid: cswPrivate.state.documentTypeId,
                                EditMode: Csw.enums.editMode.Add
                            },
                            ReloadTabOnSave: false,
                            onNodeIdSet: function (documentId) {
                                cswPrivate.state.documentId = documentId;
                            }
                        });

                        if (cswPrivate.state.sdsDocs.length > 0) {
                            SDSGridCell.span().setLabelText('Existing SDS Documents:').br({number: 2});
                            cswPrivate.documentGrid = SDSGridCell.thinGrid({ linkText: '' });
                            var row = 2;
                            Csw.iterate(cswPrivate.state.sdsDocs, function(sdsDoc) {
                                cswPrivate.documentGrid.addCell(sdsDoc.revisiondate, row, 1);
                                var linkCell = cswPrivate.documentGrid.addCell('', row, 2);
                                linkCell.a({
                                    href: sdsDoc.linktext,
                                    text: sdsDoc.displaytext
                                });
                                row++;
                            });
                        }

                        cswPrivate.stepThreeComplete = true;
                    }
                };

            }());
            //#endregion Step 3: Attach SDS

            (function _post() {

                cswPrivate.makeStep1();

            } ());

            return cswPublic;
        });
} ());