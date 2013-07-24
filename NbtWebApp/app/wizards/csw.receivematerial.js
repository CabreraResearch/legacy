/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    var cswReceiveMaterialWizardState = 'cswReceiveMaterialWizardState';

    Csw.nbt.receiveMaterialWizard = Csw.nbt.receiveMaterialWizard ||
        Csw.nbt.register('receiveMaterialWizard', function (cswParent, options) {
            'use strict';
            
            //#region Properties
            var cswPrivate = {
                name: 'cswReceiveMaterialWizard',
                startingStep: 1,
                stepCount: 0,
                wizard: null,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
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
                    canAddCofA: false,
                    sdsDocs: [{
                        revisiondate: '',
                        displaytext: '',
                        linktext: ''
                    }],
                    sdsDocTypeId: '',
                    sdsDocId: '',
                    cofaDocTypeId: '',
                    cofaDocId: '',
                    requestitem: {}
                },
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
            
            cswPrivate.toggleStepButtons = function (StepNo) {
                cswPrivate.toggleButton(cswPrivate.buttons.prev, StepNo > 1);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, StepNo === cswPrivate.stepCount);
                cswPrivate.toggleButton(cswPrivate.buttons.next, StepNo !== cswPrivate.stepCount);
            };

            cswPrivate.reinitSteps = function (startWithStep) {               
                for (var i = startWithStep; i <= cswPrivate.stepCount; i++) {
                    cswPrivate['step' + i + 'Complete'] = false;
                }
            };
            
            cswPrivate.handleStep = function (newStepNo) {
                cswPrivate.setState();
                //Not sure if we need last and current step...
                cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                cswPrivate.currentStepNo = newStepNo;
                if (false === Csw.isNullOrEmpty(cswPrivate.stepFunc[newStepNo])) {
                    cswPrivate.stepFunc[newStepNo](newStepNo);
                }
            };
            
            cswPrivate.setWizardSteps = function () {
                var wizardSteps = {};
                cswPrivate.stepFunc = {};
                cswPrivate.stepCount = 1;
                if (cswPrivate.state.canAddCofA) {
                    wizardSteps[cswPrivate.stepCount] = 'Manufacturer Lot Info';
                    cswPrivate.stepFunc[cswPrivate.stepCount] = cswPrivate.makeStepManufacturerLotInfo;
                    cswPrivate.stepCount++;
                }
                wizardSteps[cswPrivate.stepCount] = 'Create Containers';
                cswPrivate.stepFunc[cswPrivate.stepCount] = cswPrivate.makeStepCreateContainers;
                cswPrivate.stepCount++;
                wizardSteps[cswPrivate.stepCount] = 'Define Properties';
                cswPrivate.stepFunc[cswPrivate.stepCount] = cswPrivate.makeStepContainerProps;
                if (cswPrivate.state.canAddSDS) {
                    cswPrivate.stepCount++;
                    wizardSteps[cswPrivate.stepCount] = 'Attach SDS';
                    cswPrivate.stepFunc[cswPrivate.stepCount] = cswPrivate.makeStepAttachSDS;
                }
                cswPrivate.reinitSteps(1);
                return wizardSteps;
            };            

            cswPrivate.setStepHeader = function (StepNo, Header) {
                cswPrivate['divStep' + StepNo] = cswPrivate['divStep' + StepNo] || cswPrivate.wizard.div(StepNo);
                cswPrivate['divStep' + StepNo].empty();
                cswPrivate['divStep' + StepNo].span({
                    text: Header,
                    cssclass: 'wizardHelpDesc'
                });
                cswPrivate['divStep' + StepNo].br({ number: 2 });
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
                if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps)) {
                    cswPrivate.tabsAndProps.tearDown();
                }
                if (false === Csw.isNullOrEmpty(cswPrivate.sdsDocTabsAndProps)) {
                    cswPrivate.sdsDocTabsAndProps.tearDown();
                }
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
            }());
            //#endregion ctor preInit
            
            //#region Step 1: Manufacturer Lot Info
            cswPrivate.makeStepManufacturerLotInfo = (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Define Manufacturer Lot information for this Receipt.');

                        //TODO - Case 29700: Add ReceiptLot addlayout

                        var attachCofATable = cswPrivate['divStep' + StepNo].table();
                        attachCofATable.cell(1, 1).a({
                            text: 'Add a new C of A',
                            onClick: function () {
                                attachCofATable.cell(1, 1).hide();
                                attachCofATable.cell(1, 2).show();
                            }
                        });
                        attachCofATable.cell(1, 2).hide();

                        cswPrivate.cofaDocTabsAndProps = Csw.layouts.tabsAndProps(attachCofATable.cell(1, 2), {
                            tabState: {
                                excludeOcProps: ['owner', 'save'],
                                ShowAsReport: false,
                                nodetypeid: cswPrivate.state.cofaDocTypeId,
                                EditMode: Csw.enums.editMode.Add
                            },
                            ReloadTabOnSave: false,
                            onNodeIdSet: function (cofaDocId) {
                                cswPrivate.state.cofaDocId = cofaDocId;
                            }
                        });

                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }());
            //#endregion Step 1: Manufacturer Lot Info

            //#region Step 2: Create Containers
            cswPrivate.makeStepCreateContainers = (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);
                    
                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Select the number of containers and their quantities to receive.');

                        //Container Select (if multiple container nodetypes exist)
                        var containerSelect = Csw.wizard.nodeTypeSelect(cswPrivate['divStep' + StepNo].div(), {
                            objectClassName: 'ContainerClass',
                            labelText: 'Select a Container: ',
                            data: cswPrivate.state.container,
                            onSelect: function () {
                                if (cswPrivate.state.containerNodeTypeId !== containerSelect.selectedNodeTypeId) {
                                    cswPrivate.reinitSteps(StepNo+1);
                                    cswPrivate.state.containerAddLayout = null;
                                    //TODO - instead of blanking out the existing temp container, we should make a new temp
                                    cswPrivate.state.containerNodeId = null;
                                }
                                cswPrivate.state.containerNodeTypeId = containerSelect.selectedNodeTypeId;
                            },
                            onSuccess: function () {
                                makeAmountsGrid();
                                makeBarcodeCheckBox();
                            }
                        });
                        //Container Sizes and Amounts
                        var makeAmountsGrid = function () {
                            cswPrivate.amountsDiv = cswPrivate.amountsDiv || cswPrivate['divStep' + StepNo].div();
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
                        //Print Barcodes Checkbox
                        var makeBarcodeCheckBox = function () {
                            cswPrivate.barcodeCheckBoxDiv = cswPrivate.barcodeCheckBoxDiv || cswPrivate['divStep' + StepNo].div();
                            cswPrivate.barcodeCheckBoxDiv.empty();

                            var checkBoxTable = cswPrivate.barcodeCheckBoxDiv.table({
                                cellvalign: 'middle'
                            });
                            var printBarcodesCheckBox = checkBoxTable.cell(1, 1).checkBox({
                                checked: true,
                                onChange: Csw.method(function () {
                                    if (printBarcodesCheckBox.checked()) {
                                        cswPrivate.printBarcodes = true;
                                    } else {
                                        cswPrivate.printBarcodes = false;
                                    }
                                })
                            });
                            checkBoxTable.cell(1, 2).span({ text: 'Print barcode labels for new containers' });
                        };

                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }());
            //#endregion Step 2: Create Containers

            //#region Step 3: Define Properties
            cswPrivate.makeStepContainerProps = (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Configure the properties to apply to all containers upon receipt.');
                        cswPrivate['divStep' + StepNo].br({ number: 2 });

                        cswPrivate.tabsAndProps = Csw.wizard.addLayout(cswPrivate['divStep' + StepNo], {
                            name: cswPrivate.state.containerNodeTypeId + 'add_layout',
                            excludeOcProps: ['save'],
                            tabState: {
                                propertyData: cswPrivate.state.containerAddLayout,
                                removeTempStatus: false,
                                nodetypeid: cswPrivate.state.containerNodeTypeId,
                                nodeid: cswPrivate.state.containerNodeId
                            },
                            onSaveError: function (errorData) {
                                console.log(errorData);
                                cswPrivate.saveError = true;
                            }

                        });

                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }());
            //#endregion Step 3: Define Properties

            //#region Step 4: Attach SDS
            cswPrivate.makeStepAttachSDS = (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Define a Safety Data Sheet to attach to ' + cswPrivate.state.tradeName);

                        var SDSTable = cswPrivate['divStep' + StepNo].table(),
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

                        cswPrivate.sdsDocTabsAndProps = Csw.layouts.tabsAndProps(attachSDSTable.cell(1, 2), {
                            tabState: {
                                excludeOcProps: ['owner', 'save'],
                                ShowAsReport: false,
                                nodetypeid: cswPrivate.state.sdsDocTypeId,
                                EditMode: Csw.enums.editMode.Add
                            },
                            ReloadTabOnSave: false,
                            onNodeIdSet: function (sdsDocId) {
                                cswPrivate.state.sdsDocId = sdsDocId;
                            }
                        });

                        if (cswPrivate.state.sdsDocs.length > 0) {
                            SDSGridCell.span().setLabelText('Existing SDS Documents:').br({number: 2});
                            cswPrivate.sdsDocGrid = SDSGridCell.thinGrid({ linkText: '' });
                            var row = 2;
                            Csw.iterate(cswPrivate.state.sdsDocs, function(sdsDoc) {
                                cswPrivate.sdsDocGrid.addCell(sdsDoc.revisiondate, row, 1);
                                var linkCell = cswPrivate.sdsDocGrid.addCell('', row, 2);
                                linkCell.a({
                                    href: sdsDoc.linktext,
                                    text: sdsDoc.displaytext
                                });
                                row++;
                            });
                        }

                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }());
            //#endregion Step 4: Attach SDS
            
            //#region Finish
            cswPrivate.finalize = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                var container = {
                    containernodeid: cswPrivate.state.containerNodeId,
                    materialid: cswPrivate.state.materialId,
                    containernodetypeid: cswPrivate.state.containerNodeTypeId,
                    sdsDocId: cswPrivate.state.sdsDocId,
                    cofaDocId: cswPrivate.state.cofaDocId,
                    quantities: cswPrivate.amountsGrid.quantities(),
                    sizeid: cswPrivate.state.selectedSizeId,
                    props: cswPrivate.state.properties,
                    requestitem: cswPrivate.state.requestitem
                };
                if (false === Csw.isNullOrEmpty(cswPrivate.sdsDocTabsAndProps)) {
                    container.sdsDocProperties = cswPrivate.sdsDocTabsAndProps.getProps();
                }
                if (false === Csw.isNullOrEmpty(cswPrivate.cofaDocTabsAndProps)) {
                    container.cofaDocProperties = cswPrivate.cofaDocTabsAndProps.getProps();
                }
                Csw.ajax.post({
                    urlMethod: 'receiveMaterial',
                    data: { ReceiptDefinition: Csw.serialize(container) },
                    success: function (data) {
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
            };
            //#endregion Finish

            (function _post() {
                var wizardSteps = cswPrivate.setWizardSteps();
                cswPrivate.state.containerlimit = Csw.number(cswPrivate.state.containerlimit, 25);
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'Receive: ' + cswPrivate.state.tradeName,
                    StepCount: cswPrivate.stepCount,
                    Steps: wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onBeforeNext: function () {
                        var ret = true;
                        if (false === Csw.isNullOrEmpty(cswPrivate.amountsGrid)) {
                            ret = Csw.bool(cswPrivate.amountsGrid.containerCount > 0 &&
                                cswPrivate.amountsGrid.containerCount <= cswPrivate.amountsGrid.containerlimit);
                        }
                        return ret;
                    },
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: cswPrivate.onCancel,
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });
                cswPrivate.stepFunc[1](1);
            } ());

            return cswPublic;
        });
} ());