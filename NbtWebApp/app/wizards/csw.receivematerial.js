/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
    var cswReceiveMaterialWizardState = 'cswReceiveMaterialWizardState';

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
                nodeName: '',
                selectedSizeId: '',
                customBarcodes: false,
                nodetypename: '',
                addSDSPermission: false,
                canAddSDS: true,
                canAddCofA: false,
                //sdsDocs: [{//Case 31855 - this is the object structure, but we shouldn't add it to the parent state
                //    revisiondate: '',
                //    displaytext: '',
                //    linktext: ''
                //}],
                sdsDocTypeId: '',
                sdsDocId: '',
                cofaDocTypeId: '',
                cofaDocId: '',
                receiptLotTypeId: '',
                receiptLotId: '',
                requestitem: {},
                printLabels: true
            },
            amountsGrid: null,
            saveError: false,
            manufacturerLotInfoChanged: false
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
            if (false === Csw.isNullOrEmpty(cswPrivate.onStepChange[cswPrivate.currentStepNo])) {
                cswPrivate.onStepChange[cswPrivate.currentStepNo](cswPrivate.currentStepNo);
            }
            cswPrivate.lastStepNo = cswPrivate.currentStepNo;
            cswPrivate.currentStepNo = newStepNo;
            if (false === Csw.isNullOrEmpty(cswPrivate.stepFunc[newStepNo])) {
                cswPrivate.stepFunc[newStepNo](newStepNo);
            }
        };

        cswPrivate.setWizardSteps = function () {
            var wizardSteps = {};
            cswPrivate.stepFunc = {};
            cswPrivate.onStepChange = {};
            cswPrivate.stepCount = 0;
            var setWizardStep = function (wizardStep) {
                cswPrivate.stepCount++;
                cswPrivate.stepFunc[cswPrivate.stepCount] = wizardStep.makeStep;
                cswPrivate.onStepChange[cswPrivate.stepCount] = wizardStep.onStepChange;
                wizardStep.stepNo = cswPrivate.stepCount;
                wizardSteps[cswPrivate.stepCount] = wizardStep.stepName;
            };
            //Add steps here:
            if (cswPrivate.state.canAddCofA) {
                setWizardStep(cswPrivate.wizardStepManufacturerLotInfo);
            }
            setWizardStep(cswPrivate.wizardStepCreateContainers);
            setWizardStep(cswPrivate.wizardStepContainerProps);
            if (cswPrivate.state.canAddSDS) {
                setWizardStep(cswPrivate.wizardStepAttachSDS);
            }
            setWizardStep(cswPrivate.wizardStepPrintLabels);
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
            if (false === Csw.isNullOrEmpty(cswPrivate.containerTabsAndProps)) {
                cswPrivate.containerTabsAndProps.tearDown();
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.sdsDocTabsAndProps)) {
                cswPrivate.sdsDocTabsAndProps.tearDown();
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.cofaDocTabsAndProps)) {
                cswPrivate.cofaDocTabsAndProps.tearDown();
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.receiptLotTabsAndProps)) {
                cswPrivate.receiptLotTabsAndProps.tearDown();
            }
        };
        //#endregion State Functions

        //#region ctor preInit
        (function _pre() {
            if (options) {
                Csw.extend(cswPrivate, options, true);
            }
            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
            }
           
            cswPrivate.validateState();
        }());
        //#endregion ctor preInit

        //#region Step: Manufacturer Lot Info
        cswPrivate.wizardStepManufacturerLotInfo = {
            stepName: 'Manufacturer Lot Info',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Define Manufacturer Lot information for this Receipt.');

                        var manufacturerLotInfoTable = cswPrivate['divStep' + StepNo].table();
                        cswPrivate.receiptLotTabsAndProps = Csw.layouts.tabsAndProps(manufacturerLotInfoTable.cell(1, 1), {
                            tabState: {
                                excludeOcProps: ['material', 'save'],
                                ShowAsReport: false,
                                nodetypeid: cswPrivate.state.receiptLotTypeId,
                                EditMode: Csw.enums.editMode.Add
                            },
                            ReloadTabOnSave: false,
                            onNodeIdSet: function (receiptLotId) {
                                cswPrivate.state.receiptLotId = receiptLotId;
                            },
                            onPropertyChange: function () {
                                cswPrivate.manufacturerLotInfoChanged = true;
                                cswPrivate.reinitSteps(cswPrivate.wizardStepContainerProps.stepNo);
                            }
                        });

                        var attachCofATable = cswPrivate['divStep' + StepNo].table();
                        attachCofATable.cell(1, 1).a({
                            text: 'Add a new C of A:',
                            onClick: function () {
                                attachCofATable.cell(2, 1).show();
                            }
                        });
                        attachCofATable.cell(2, 1).hide();

                        cswPrivate.cofaDocTabsAndProps = Csw.layouts.tabsAndProps(attachCofATable.cell(2, 1), {
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
            }()),
            onStepChange: function () {
                if (cswPrivate.manufacturerLotInfoChanged && false === Csw.isNullOrEmpty(cswPrivate.receiptLotTabsAndProps)) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Containers/updateExpirationDate',
                        data: {
                            ReceiptLotId: cswPrivate.state.receiptLotId,
                            ReceiptLotProps: Csw.serialize(cswPrivate.receiptLotTabsAndProps.getProps()),
                            ContainerId: cswPrivate.state.containerNodeId
                        },
                        success: function (data) {
                            cswPrivate.manufacturerLotInfoChanged = false;
                            if (false === Csw.isNullOrEmpty(data.ContainerProps)) {
                                cswPrivate.state.containerAddLayout = JSON.parse(data.ContainerProps);
                            }
                        }
                    });
                }
            }
        };
        //#endregion Step: Manufacturer Lot Info

        //#region Step: Create Containers
        cswPrivate.wizardStepCreateContainers = {
            stepName: 'Create Containers',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'The limit for containers created at receipt is [' + cswPrivate.state.containerlimit + ']');

                        //Container Select (if multiple container nodetypes exist)
                        var containerSelect = Csw.wizard.nodeTypeSelect(cswPrivate['divStep' + StepNo].div(), {
                            objectClassName: 'ContainerClass',
                            labelText: 'Select a Container: ',
                            data: cswPrivate.state.container,
                            onSelect: function () {
                                if (cswPrivate.state.containerNodeTypeId !== containerSelect.selectedNodeTypeId) {
                                    cswPrivate.reinitSteps(cswPrivate.wizardStepContainerProps.stepNo);
                                    cswPrivate.state.containerAddLayout = null;
                                    //TODO - instead of blanking out the existing temp container, we should make a new temp
                                    cswPrivate.state.containerNodeId = null;
                                }
                                cswPrivate.state.containerNodeTypeId = containerSelect.selectedNodeTypeId;
                            },
                            onSuccess: function () {
                                makeAmountsGrid();
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
                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }())
        };
        //#endregion Step: Create Containers

        //#region Step: Define Properties
        cswPrivate.wizardStepContainerProps = {
            stepName: 'Define Properties',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Configure the properties to apply to all containers upon receipt.');
                        cswPrivate['divStep' + StepNo].br({ number: 2 });

                        cswPrivate.containerTabsAndProps = Csw.wizard.addLayout(cswPrivate['divStep' + StepNo], {
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
            }())
        };
        //#endregion Step: Define Properties

        //#region Step: Attach SDS
        cswPrivate.wizardStepAttachSDS = {
            stepName: 'Attach SDS',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Define a Safety Data Sheet to attach to ' + cswPrivate.state.tradeName);

                        var SDSTable = cswPrivate['divStep' + StepNo].table(),
                            SDSAddCell = SDSTable.cell(1, 1).css({ width: '450px' }),
                            SDSGridCell = SDSTable.cell(1, 2).css({ width: '550px' }),
                            attachSDSTable = SDSAddCell.table();

                        if (cswPrivate.state.addSDSPermission) {
                            attachSDSTable.cell(1, 1).a({
                                text: 'Add a new SDS Document',
                                onClick: function() {
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
                                onNodeIdSet: function(sdsDocId) {
                                    cswPrivate.state.sdsDocId = sdsDocId;
                                }
                            });
                        } else {
                            SDSAddCell.span({
                                text: 'You do not have permission to create an SDS Document.'
                            }).css({ 'color': 'red' });
                        }

                        if (cswPrivate.state.sdsDocs.length > 0) {
                            SDSGridCell.span().setLabelText('Existing SDS Documents:').br({ number: 2 });
                            cswPrivate.sdsDocGrid = SDSGridCell.thinGrid({ linkText: '' });
                            var row = 2;
                            Csw.iterate(cswPrivate.state.sdsDocs, function (sdsDoc) {
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
            }())
        };
        //#endregion Step: Attach SDS

        //#region Step: Print Labels
        cswPrivate.wizardStepPrintLabels = {
            stepName: 'Print Labels',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Print barcode labels for new containers');

                        var printLabelsTbl = cswPrivate['divStep' + StepNo].table().css({
                            'padding-top': '20px'
                        });

                        //get the properties of the container from the "Define Properties" step. Find the label format, and store it to pass in to the print label composite
                        var selectedLabel;
                        var containerProps = cswPrivate.containerTabsAndProps.getProps();
                        Csw.iterate(containerProps, function(prop) {
                            if (prop.name == "Label Format") {
                                selectedLabel = prop.values.nodeid;
                            }
                        });


                        var printLabelCell = printLabelsTbl.cell(1, 1).css({ 'padding-bottom': '20px' });
                        printLabelCell.div({ text: 'I want to print labels: ' }).checkBox({
                            name: 'printLabelCheckBox',
                            checked: true,
                            onChange: function (newVal) {
                                if (newVal) {
                                    labelsDiv.show();
                                    cswPrivate.state.printLabels = true;
                                } else {
                                    labelsDiv.hide();
                                    cswPrivate.state.printLabels = false;
                                }
                            }
                        });

                        var labelsDiv = printLabelsTbl.cell(2, 1).div();
                        cswPrivate.printLabels = Csw.composites.printLabels(labelsDiv,
                            {
                                showButton: false,
                                selectedLabel: selectedLabel,
                            },
                            cswPrivate.state.containerNodeTypeId,
                            cswPrivate.state.containerNodeId);

                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }())
        };
        //#endregion Step: Print Labels

        //#region Finish
        cswPrivate.finalize = function () {
            cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

            var container = {
                containernodeid: cswPrivate.state.containerNodeId,
                materialid: cswPrivate.state.materialId,
                containernodetypeid: cswPrivate.state.containerNodeTypeId,
                sdsDocId: cswPrivate.state.sdsDocId,
                cofaDocId: cswPrivate.state.cofaDocId,
                receiptLotId: cswPrivate.state.receiptLotId,
                quantities: cswPrivate.amountsGrid.quantities(),
                sizeid: cswPrivate.state.selectedSizeId,
                props: Csw.serialize(cswPrivate.containerTabsAndProps.getProps()),
                requestitem: cswPrivate.state.requestitem
            };
            if (false === Csw.isNullOrEmpty(cswPrivate.sdsDocTabsAndProps)) {
                container.sdsDocProperties = Csw.serialize(cswPrivate.sdsDocTabsAndProps.getProps());
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.cofaDocTabsAndProps)) {
                container.cofaDocProperties = Csw.serialize(cswPrivate.cofaDocTabsAndProps.getProps());
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.receiptLotTabsAndProps)) {
                container.receiptLotProperties = Csw.serialize(cswPrivate.receiptLotTabsAndProps.getProps());
            }
            if (cswPrivate.state.printLabels) {
                var printData = cswPrivate.printLabels.getPrintData();
                container.LabelId = printData.LabelId;
                container.PrinterId = printData.PrinterId;
            }

            Csw.ajaxWcf.post({
                urlMethod: 'Containers/Receive',
                data: container,
                success: function (data) {
                    cswPrivate.onFinish(Csw.deserialize(data.ActionData));
                }
            });
        };
        //#endregion Finish

        //#region ctor _post
        (function _post() {
            var wizardSteps = cswPrivate.setWizardSteps();
            cswPrivate.state.containerlimit = Csw.number(cswPrivate.state.containerlimit, 250);
            cswPrivate.currentStepNo = cswPrivate.startingStep;

            cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                Title: 'Receive: ' + cswPrivate.state.nodeName,
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
        }());
        //#endregion ctor _post

        return cswPublic;
    });
}());