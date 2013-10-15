/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
    var cswCreateMaterialWizardStateName = 'cswCreateMaterialWizardStateName';
    Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
        'use strict';

        //#region Properties
        var cswPrivate = {
            name: 'cswCreateMaterialWizard',
            exitFunc: null,
            startingStep: 1,
            wizard: null,
            buttons: {
                next: 'next',
                prev: 'previous',
                finish: 'finish',
                cancel: 'cancel'
            },
            materialTypeSelect: null,
            newSupplierName: 'New Supplier Name >>',
            tabsAndProps: null,
            documentTabsAndProps: null,
            showQuantityEditable: false,
            showDispensable: false,
            state: {
                request: {},
                sizeNodeTypeId: '',
                relatedNodeId: null,
                materialId: '',
                documentTypeId: '',
                sdsDocId: '',
                materialType: {
                    name: '',
                    val: '',
                    objclassid: '',
                    readOnly: false
                },
                tradeName: '',
                supplier: {
                    name: '',
                    val: '',
                    nodelink: '',
                    corporate: false
                },
                c3SupplierName: '',
                addNewC3Supplier: false,
                partNo: '',
                properties: {},
                useExistingTempNode: false,
                physicalState: 'liquid',
                sizes: [],
                canAddSDS: true,
                showOriginalUoM: false,
                chemicalObjClassId: '',
                constituentNtIds: '',
                containerlimit: 25
            },
            physicalStateModified: false,
            containersModuleEnabled: true,
            SDSModuleEnabled: true,
            AllowSupplierAdd: true,
            sizesGrid: null
        };

        var cswPublic = {};
        //#endregion Properties

        //#region State Functions

        cswPrivate.validateState = function () {
            var state;
            if (Csw.isNullOrEmpty(cswPrivate.state.tradeName)) {
                state = cswPrivate.getState();
                Csw.extend(cswPrivate.state, state);
            }
            cswPrivate.setState();
        };

        cswPrivate.getState = function () {
            var ret = Csw.clientDb.getItem(cswPrivate.name + '_' + cswCreateMaterialWizardStateName);
            return ret;
        };

        cswPrivate.setState = function () {
            Csw.clientDb.setItem(cswPrivate.name + '_' + cswCreateMaterialWizardStateName, cswPrivate.state);
        };

        cswPrivate.clearState = function () {
            Csw.clientDb.removeItem(cswPrivate.name + '_' + cswCreateMaterialWizardStateName);
            if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps)) {
                cswPrivate.tabsAndProps.tearDown();
            }
            if (false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                cswPrivate.documentTabsAndProps.tearDown();
            }
        };

        //#endregion State Functions

        cswPrivate.isConstituent = function () {
            var ret = false;
            if (cswPrivate.state.materialType) {
                ret = cswPrivate.state.constituentNtIds.contains(cswPrivate.state.materialType.val);
            }
            if (cswPrivate.materialTypeSelect) {
                ret = ret || cswPrivate.state.constituentNtIds.contains(cswPrivate.materialTypeSelect.selectedVal());
            }
            return ret;
        }; // isConstituent()

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

        cswPrivate.setWizardSteps = function () {
            var wizardSteps = {};
            cswPrivate.stepFunc = {};
            cswPrivate.onStepChange = {};
            cswPrivate.onStepCleanup = {};
            cswPrivate.stepCount = 0;
            var setWizardStep = function (wizardStep) {
                cswPrivate.stepCount++;
                cswPrivate.stepFunc[cswPrivate.stepCount] = wizardStep.makeStep;
                cswPrivate.onStepChange[cswPrivate.stepCount] = wizardStep.onStepChange;
                cswPrivate.onStepCleanup[cswPrivate.stepCount] = wizardStep.onStepCleanup;
                wizardStep.stepNo = cswPrivate.stepCount;
                wizardSteps[cswPrivate.stepCount] = wizardStep.stepName;
            };
            //Add steps here:
            setWizardStep(cswPrivate.makeIdentityStep);
            setWizardStep(cswPrivate.makeAdditionalPropsStep);
            if (cswPrivate.containersModuleEnabled) {
                setWizardStep(cswPrivate.makeSizesStep);
            }
            if (cswPrivate.SDSModuleEnabled) {
                setWizardStep(cswPrivate.makeAttachSDSStep);
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

        cswPrivate.handleStep = function (newStepNo) {
            var onStepChangeSuccess = function () {
                cswPrivate.stepFunc[cswPrivate.currentStepNo](cswPrivate.currentStepNo);
            };
            cswPrivate.toggleButton(cswPrivate.buttons.next, false); //let each step activate this button if needed
            cswPrivate.setState();
            if (Csw.contains(cswPrivate.stepFunc, newStepNo)) {
                cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                cswPrivate.currentStepNo = newStepNo;
                cswPrivate.onStepChange[cswPrivate.lastStepNo](cswPrivate.lastStepNo, onStepChangeSuccess);
            }
        };

        cswPrivate.isLastStep = function () {
            return ((false === cswPrivate.state.canAddSDS || false === cswPrivate.SDSModuleEnabled) &&
                        (false === cswPrivate.containersModuleEnabled || cswPrivate.isConstituent()));
        };
        //#endregion Wizard Functions

        //#region Step: Choose Type and Identity
        cswPrivate.makeIdentityStep = {
            stepName: 'Choose Type and Identity',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    function changeMaterial() {
                        if (cswPrivate.materialTypeSelect &&
                            (Csw.string(cswPrivate.state.materialType.val) !== Csw.string(cswPrivate.materialTypeSelect.val()))) {

                            cswPrivate.state.materialType = {
                                name: cswPrivate.materialTypeSelect.find(':selected').text(),
                                val: cswPrivate.materialTypeSelect.val()
                            };
                            cswPrivate.state.physicalState = ''; //Case 29015
                            cswPrivate['step' + cswPrivate.makeSizesStep.stepNo + 'Complete'] = false;
                        }
                        // Note: When importing from ChemCatCentral, the materialTypeSelect is null because
                        // the Material Type is rendered as readOnly. Hence, we need to account for this when
                        // checking if we should add the Attach SDS step.
                        if (cswPrivate.materialTypeSelect) {
                            cswPrivate.state.canAddSDS =
                                cswPrivate.materialTypeSelect.find(':selected').data('objectclassid') === cswPrivate.state.chemicalObjClassId
                                    && false === cswPrivate.isConstituent();
                        } else {
                            cswPrivate.state.canAddSDS = (Csw.bool(cswPrivate.state.materialType.objclassid === cswPrivate.state.chemicalObjClassId) && false === cswPrivate.isConstituent());
                        }

                        cswPrivate.wizard.toggleStepVisibility(cswPrivate.makeAttachSDSStep.stepNo, cswPrivate.state.canAddSDS);
                        if (cswPrivate.containersModuleEnabled) {
                            cswPrivate.wizard.toggleStepVisibility(cswPrivate.makeSizesStep.stepNo, false == cswPrivate.isConstituent());
                        }
                        if (cswPrivate.supplierSelect) {
                            if (cswPrivate.isConstituent()) {
                                cswPrivate.supplierLabel.hide();
                                cswPrivate.supplierSelect.hide();
                                cswPrivate.state.supplier = {
                                    name: '',
                                    val: '',
                                    nodelink: '',
                                    corporate: cswPrivate.state.supplier.corporate
                                };
                            } else {
                                cswPrivate.supplierLabel.show();
                                cswPrivate.supplierSelect.show();
                                
                                //NOTE: the following should only run while doing a ChemCatCentral import. 
                                //If there is no longer a comment ~30 lines above this about materialTypeSelect 
                                //always being null during C3 imports, this block of logic is highly suspect.
                                if (undefined == cswPrivate.materialTypeSelect &&
                                    cswPrivate.supplierSelect.selectedText &&
                                    Csw.string(cswPrivate.state.supplier.val) !== Csw.string(cswPrivate.supplierSelect.val())) {
                                    
                                    if (cswPrivate.supplierSelect.selectedText() === cswPrivate.newSupplierName) {
                                        cswPrivate.makeNewC3SupplierInput(true, 1, 1);
                                    } else {
                                        cswPrivate.makeNewC3SupplierInput(false, 1, 1);
                                    }
                                }
                            }
                        }//if (cswPrivate.supplierSelect)
                        if (cswPrivate.tradeNameInput &&
                            cswPrivate.state.tradeName !== cswPrivate.tradeNameInput.val()) {
                            cswPrivate.state.tradeName = cswPrivate.tradeNameInput.val();
                        }
                        if (cswPrivate.partNoInput) {
                            if (cswPrivate.isConstituent()) {
                                cswPrivate.partNoLabel.hide();
                                cswPrivate.partNoInput.hide();
                                cswPrivate.state.partNo = '';
                            } else {
                                cswPrivate.partNoLabel.show();
                                cswPrivate.partNoInput.show();
                                if (cswPrivate.state.partNo !== cswPrivate.partNoInput.val()) {
                                    cswPrivate.state.partNo = cswPrivate.partNoInput.val();
                                }
                            }
                        }
                        if (cswPrivate.newC3SupplierInput) {
                            if (cswPrivate.isConstituent()) {
                                cswPrivate.newC3SupplierInput.hide();
                            } else {
                                if (cswPrivate.supplierSelect.selectedText && cswPrivate.supplierSelect.selectedText() === cswPrivate.newSupplierName) {
                                    cswPrivate.newC3SupplierInput.show();
                                    cswPrivate.state.supplier = {
                                        name: cswPrivate.newC3SupplierInput.val(),
                                        val: '',
                                        nodelink: cswPrivate.state.supplier.nodelink,
                                        corporate: cswPrivate.state.supplier.corporate
                                    };
                                    cswPrivate.state.c3SupplierName = cswPrivate.newC3SupplierInput.val();
                                } else {
                                    // 'Search' case
                                    cswPrivate.state.supplier = {
                                        name: cswPrivate.newC3SupplierInput.val(),
                                        val: '',
                                        nodelink: cswPrivate.state.supplier.nodelink,
                                        corporate: cswPrivate.state.supplier.corporate
                                    };
                                    cswPrivate.state.c3SupplierName = cswPrivate.newC3SupplierInput.val();
                                }
                            }
                        }
                        cswPrivate.reinitSteps(2);
                    }//changeMaterial()

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.identityDiv = cswPrivate.identityDiv || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                        cswPrivate.identityDiv.empty();

                        cswPrivate.identityDiv.span({
                            text: "This wizard will guide you through the process of creating a new material." +
                                "If the attributes below match an existing material, you will be given the option to view that material.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.identityDiv.br({ number: 4 });

                        var tbl = cswPrivate.identityDiv.table({
                            FirstCellRightAlign: true,
                        });
                        //#region Step 1 Properties
                        /* Material Type */
                        tbl.cell(1, 1).span().setLabelText('Select a Material Type: ', true, false);
                        var cell12 = tbl.cell(1, 2).empty();

                        if (cswPrivate.state.materialType.readOnly) {
                            cell12.span({
                                text: cswPrivate.state.materialType.name,
                            });
                        } else {
                            cswPrivate.materialTypeSelect = cell12.nodeTypeSelect({
                                name: 'nodeTypeSelect',
                                propertySetName: 'MaterialSet',
                                value: cswPrivate.state.materialType.val || cswPrivate.state.materialNodeTypeId,
                                selectedName: 'Chemical',
                                filterToPermission: 'Create',
                                onChange: changeMaterial,
                                onSuccess: changeMaterial,
                                isRequired: true
                            });
                        }
                        // need this for refresh
                        if (cswPrivate.containersModuleEnabled) {
                            cswPrivate.wizard.toggleStepVisibility(cswPrivate.makeSizesStep.stepNo, false == cswPrivate.isConstituent());
                        }

                        /* Tradename */
                        cswPrivate.tradeNameLabel = tbl.cell(2, 1).span();
                        cswPrivate.tradeNameLabel.setLabelText('Tradename: ', true, false);
                        cswPrivate.tradeNameInput = tbl.cell(2, 2).input({
                            name: 'tradename',
                            cssclass: 'required',
                            value: cswPrivate.state.tradeName,
                            onChange: changeMaterial,
                            isRequired: true
                        });

                        /* Supplier */
                        var SupplierCtrlTbl = tbl.cell(3, 2).table();

                        cswPrivate.makeNewC3SupplierInput = function (visible, row, column) {
                            if (!cswPrivate.newC3SupplierInput) {
                                cswPrivate.newC3SupplierInput = SupplierCtrlTbl.cell(row, column).input({
                                    value: cswPrivate.state.c3SupplierName,
                                    onChange: changeMaterial
                                });
                            }

                            if (visible) {
                                cswPrivate.newC3SupplierInput.show();
                            } else {
                                cswPrivate.newC3SupplierInput.hide();
                            }

                        };

                        cswPrivate.makeSupplierCtrl = function (NodeTypeId) {

                            SupplierCtrlTbl.cell(1, 1).empty();
                            SupplierCtrlTbl.cell(1, 2).empty();
                            SupplierCtrlTbl.cell(1, 3).empty();

                            cswPrivate.supplierLabel = tbl.cell(3, 1).span();
                            cswPrivate.supplierLabel.setLabelText('Supplier: ', true, false);

                            var extraOptions = [];

                            // If we are importing from C3 with a new supplier, always show the
                            // 'New Supplier Name >>' option instead of the 'New+' button.
                            if (cswPrivate.state.addNewC3Supplier) {
                                cswPrivate.AllowSupplierAdd = false;
                                extraOptions.push({ id: '', value: cswPrivate.newSupplierName });
                            }

                            var ajaxData = {};
                            if (cswPrivate.supplierViewId) {
                                ajaxData.ViewId = cswPrivate.supplierViewId;
                            } else {
                                ajaxData.ObjectClass = 'VendorClass';
                            }

                            cswPrivate.supplierSelect = SupplierCtrlTbl.cell(1, 2).nodeSelect({
                                name: 'Supplier',
                                cssclass: 'required',
                                width: '200px',
                                ajaxData: ajaxData,
                                showSelectOnLoad: true,
                                allowAdd: cswPrivate.AllowSupplierAdd,
                                onAfterAdd: changeMaterial,
                                addNodeDialogTitle: 'Vendor',
                                selectedNodeId: cswPrivate.state.supplierId || cswPrivate.state.supplier.val,
                                selectedName: cswPrivate.state.supplier.name || '',
                                selectedNodeLink: cswPrivate.state.supplier.nodelink || '',
                                onChange: changeMaterial,
                                onSelectNode: function (nodeObject) {
                                    if (nodeObject) {
                                        cswPrivate.state.supplier = {
                                            name: nodeObject.name || nodeObject.nodename,
                                            val: nodeObject.nodeid,
                                            nodelink: nodeObject.relatednodelink || nodeObject.nodelink
                                        };
                                        if (cswPrivate.useSearch) {
                                            cswPrivate.makeNewC3SupplierInput(false, 1, 1);
                                        }
                                    }
                                },
                                onRemoveSelectedNode: function () {
                                    cswPrivate.state.supplier = {
                                        name: cswPrivate.state.c3SupplierName,
                                        val: "",
                                        nodelink: "",
                                        corporate: cswPrivate.state.supplier.corporate
                                    };
                                    cswPrivate.makeNewC3SupplierInput(true, 1, 1);
                                },
                                isRequired: true,
                                extraOptions: extraOptions,
                                showRemoveIcon: cswPrivate.state.addNewC3Supplier,
                                onSuccess: function (options, useSearch) {
                                    if (cswPrivate.supplierSelect.selectedText) {
                                        if (cswPrivate.supplierSelect.selectedText() === cswPrivate.newSupplierName) {
                                            cswPrivate.state.c3SupplierName = cswPrivate.state.supplier.name;
                                            cswPrivate.makeNewC3SupplierInput(true, 1, 3);
                                        }
                                    }

                                    // If we are using search (relationshipoptionlimit < # of options) && we have a new supplier incoming from chemcat
                                    if (useSearch && cswPrivate.state.addNewC3Supplier) {
                                        cswPrivate.useSearch = useSearch;
                                        cswPrivate.state.c3SupplierName = cswPrivate.state.supplier.name;
                                        cswPrivate.makeNewC3SupplierInput(true, 1, 1);
                                    }
                                }
                            });
                        };
                        cswPrivate.makeSupplierCtrl();

                        /* Part Number */
                        cswPrivate.partNoLabel = tbl.cell(4, 1).span();
                        cswPrivate.partNoLabel.setLabelText('Part No:  ');
                        cswPrivate.partNoInput = tbl.cell(4, 2).input({
                            name: 'partno',
                            value: cswPrivate.state.partNo,
                            onChange: changeMaterial
                        });
                        //#endregion Step 1 Properties

                        cswPrivate.identityDiv.br({ number: 3 });
                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }()),
            onStepChange: function (StepNo, onStepChangeSuccess) {
                var nextStep = cswPrivate.makeIdentityStep.stepNo + 1;
                if (false === cswPrivate['step' + nextStep + 'Complete']) {
                    cswPrivate.saveMaterial(cswPrivate.stepFunc[nextStep](nextStep), nextStep);
                }
                onStepChangeSuccess();
            }
        };

        cswPrivate.saveMaterial = function (onSuccess, StepNo) {
            cswPrivate.toggleButton(cswPrivate.buttons.prev, false, false);
            if (cswPrivate.tabsAndProps) {
                cswPrivate.tabsAndProps.tearDown();
            }
            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'saveMaterial',
                data: {
                    NodeTypeId: cswPrivate.state.materialType.val,
                    Tradename: cswPrivate.state.tradeName,
                    SupplierId: cswPrivate.state.supplier.val,
                    Suppliername: cswPrivate.state.supplier.name,
                    PartNo: cswPrivate.state.partNo,
                    NodeId: cswPrivate.state.materialId,
                    CorporateSupplier: cswPrivate.state.supplier.corporate || false
                },
                success: function (data) {
                    cswPrivate.isDuplicateMaterial = Csw.bool(data.materialexists);
                    if (cswPrivate.isDuplicateMaterial) {
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                        cswPrivate['step' + StepNo + 'Complete'] = false;
                        Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, "A material with these properties already exists with a tradename of " + data.noderef));
                    } else {
                        cswPrivate.state.materialId = data.materialid;
                        cswPrivate.state.documentTypeId = data.documenttypeid;
                        cswPrivate.state.properties = data.properties;
                        cswPrivate.state.supplier.val = data.supplierid;
                        Csw.tryExec(onSuccess);
                        cswPrivate.renderProps(StepNo);
                    }
                },
                error: function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                }
            });
        };//cswPrivate.saveMaterial()
        //#endregion Step: Choose Type and Identity

        //#region Step: Additional Properties
        cswPrivate.makeAdditionalPropsStep = {
            stepName: 'Additional Properties',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Provide additional data for this material.');

                        if (false === cswPrivate.state.useExistingTempNode) {
                            cswPrivate.SaveMaterialSuccess = function () {
                                cswPrivate.renderProps(StepNo);
                            };
                        } else {
                            cswPrivate.renderProps(StepNo);
                        }

                        cswPrivate['step' + StepNo + 'Complete'] = true;

                    } else {
                        var isLastStep = cswPrivate.isLastStep();

                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);
                    }
                };
            }()),
            onStepChange: function (StepNo, onStepChangeSuccess) {
                if (cswPrivate.sizesGrid) {
                    cswPrivate.sizesGrid.thinGrid.$.hide();
                }
                var PropsDefinition = {
                    NodeId: cswPrivate.state.materialId,
                    NodeTypeId: cswPrivate.state.materialType.val,
                    Properties: ''
                };
                if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps)) {
                    PropsDefinition.Properties = cswPrivate.tabsAndProps.getProps();
                }
                Csw.ajaxWcf.post({
                    urlMethod: 'Materials/saveMaterialProps',
                    data: Csw.serialize(PropsDefinition),
                    success: function (data) {
                        if (cswPrivate.state.physicalState !== data.Properties.PhysicalState) {
                            cswPrivate.reinitSteps(3);
                            cswPrivate.state.physicalState = data.Properties.PhysicalState || '';
                        }
                        onStepChangeSuccess();
                        if (cswPrivate.sizesGrid) {
                            cswPrivate.sizesGrid.thinGrid.$.show();
                        }
                    },
                    error: function () {
                        //todo: add error catcher
                    }
                });
            }
        };

        cswPrivate.renderProps = function (StepNo) {
            if (cswPrivate.tabsAndProps) {
                cswPrivate.tabsAndProps.tearDown();
            }
            var propsTable = cswPrivate['divStep' + StepNo].table();
            cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(propsTable.cell(1, 1), {
                tabState: {
                    excludeOcProps: ['tradename', 'supplier', 'partno', 'save'],
                    propertyData: cswPrivate.state.properties,
                    nodeid: cswPrivate.state.materialId,
                    ShowAsReport: false,
                    nodetypeid: cswPrivate.state.materialType.val,
                    EditMode: Csw.enums.editMode.Temp //This is intentional. We don't want the node accidentally promoted to a permanent node.
                },
                ReloadTabOnSave: false,
                onInitFinish: function () {
                    var isLastStep = cswPrivate.isLastStep();

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);
                }
            });
        };
        //#endregion Step: Additional Properties

        //#region Step: Size(s)
        cswPrivate.makeSizesStep = {
            stepName: 'Size(s)',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    var div, selectDiv;

                    var isLastStep = Csw.bool(false === cswPrivate.state.canAddSDS || false === cswPrivate.SDSModuleEnabled);
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);

                    //toggle these once the Sizes grid is loaded
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Sizes are used to receive material inventory. You can create additional sizes for this material elsewhere.');
                        div = cswPrivate['divStep' + StepNo].div();
                        div.empty();

                        var makeSizeSelect = function () {
                            var makeSizeGrid = function () {
                                cswPrivate.sizesGrid = Csw.wizard.sizesGrid(div, {
                                    name: 'sizesGrid',
                                    sizeRowsToAdd: cswPrivate.state.sizes || [],
                                    physicalState: cswPrivate.state.physicalState,
                                    sizeNodeTypeId: cswPrivate.state.sizeNodeTypeId,
                                    showQuantityEditable: cswPrivate.showQuantityEditable,
                                    showDispensable: cswPrivate.showDispensable,
                                    showOriginalUoM: cswPrivate.state.showOriginalUoM,
                                    containerlimit: cswPrivate.state.containerlimit
                                });
                            };
                            div.br();

                            var sizeSelect = function (retObj, count) {
                                cswPrivate.state.sizeNodeTypeId = cswPrivate.sizeSelect.val();
                                if (count > 1) {
                                    selectDiv.show();
                                }
                            };

                            /* Size Select (hidden if only 1 NodeType present) - to get size node type */
                            selectDiv = div.div();
                            cswPrivate.sizeSelect = selectDiv.nodeTypeSelect({
                                name: 'nodeTypeSelect',
                                useWide: true,
                                value: cswPrivate.state.sizeNodeTypeId,
                                labelText: 'Select a Material Size: ',
                                objectClassName: 'SizeClass',
                                onSelect: sizeSelect,
                                onSuccess: function (retObj, count) {
                                    sizeSelect(retObj, count);
                                    Csw.ajax.deprecatedWsNbt({
                                        urlMethod: 'getSizeLogicalsVisibility',
                                        data: { SizeNodeTypeId: cswPrivate.state.sizeNodeTypeId },
                                        success: function (data) {
                                            cswPrivate.showDispensable = Csw.bool(data.showDispensable);
                                            cswPrivate.showQuantityEditable = Csw.bool(data.showQuantityEditable);
                                            makeSizeGrid();

                                            cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);
                                            cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);
                                        }
                                    });
                                },
                                relatedToNodeTypeId: cswPrivate.state.materialType.val,
                                relatedObjectClassPropName: 'Material'
                            });
                            selectDiv.hide();

                            /* Populate this with onSuccess of cswPrivate.sizeSelect */
                            cswPrivate.addSizeNode = {};

                            cswPrivate['step' + StepNo + 'Complete'] = true;
                        };

                        //Case 29015 - if the Physical State is null, go get it
                        if (Csw.isNullOrEmpty(cswPrivate.state.physicalState)) {
                            Csw.ajaxWcf.post({
                                urlMethod: 'Materials/getPhysicalState',
                                data: cswPrivate.state.materialId,
                                success: function (data) {
                                    cswPrivate.state.physicalState = data.PhysicalState;
                                    makeSizeSelect();
                                }
                            });
                        } else {
                            makeSizeSelect();
                        }
                    } else {
                        cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);
                    }
                };
            }()),
            onStepChange: function (StepNo, onStepChangeSuccess) {
                onStepChangeSuccess();
            },
            onStepCleanup: function (StepNo) {
                cswPrivate.sizesGrid = null;
            }
        };
        //#endregion Step: Size(s)

        //#region Step: Attach SDS
        cswPrivate.makeAttachSDSStep = {
            stepName: 'Attach SDS',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);

                    if (false === cswPrivate['step' + StepNo + 'Complete']) {
                        cswPrivate.setStepHeader(StepNo, 'Define a Safety Data Sheet to attach to this material.');

                        var attachSDSTable = cswPrivate['divStep' + StepNo].table();
                        attachSDSTable.cell(1, 1).a({
                            text: 'Add a new SDS Document',
                            onClick: function () {
                                attachSDSTable.cell(1, 1).hide();
                                attachSDSTable.cell(1, 2).show();
                            }
                        });

                        // If an SDS document already exists, hide the option to add
                        // a new one and send the Temp edit mode so a new one isn't created
                        var editMode;
                        if (Csw.isNullOrEmpty(cswPrivate.state.sdsDocId)) {
                            attachSDSTable.cell(1, 2).hide();
                            editMode = Csw.enums.editMode.Add;
                        } else {
                            attachSDSTable.cell(1, 1).hide();
                            editMode = Csw.enums.editMode.Temp;
                        }

                        cswPrivate.documentTabsAndProps = Csw.layouts.tabsAndProps(attachSDSTable.cell(1, 2), {
                            tabState: {
                                excludeOcProps: ['owner', 'save'],
                                ShowAsReport: false,
                                nodeid: cswPrivate.state.sdsDocId,
                                nodetypeid: cswPrivate.state.documentTypeId,
                                EditMode: editMode
                            },
                            ReloadTabOnSave: false,
                            onNodeIdSet: function (sdsDocId) {
                                cswPrivate.state.sdsDocId = sdsDocId;
                            }
                        });

                        cswPrivate['step' + StepNo + 'Complete'] = true;
                    }
                };
            }()),
            onStepChange: function (StepNo, onStepChangeSuccess) {
                onStepChangeSuccess();
            }
        };
        //#endregion Step: Attach SDS

        //#region Finish
        cswPrivate.finalize = function () {
            function getMaterialDefinition() {
                var createMaterialDef = {
                    useexistingmaterial: cswPrivate.state.useExistingTempNode,
                    sizeNodes: []
                };

                //From step 0: request, materialid
                createMaterialDef.request = cswPrivate.state.request;
                createMaterialDef.materialId = cswPrivate.state.materialId;

                //From step 1: materialtype, tradename, supplier, partno
                createMaterialDef.materialnodetypeid = cswPrivate.state.materialType.val;
                createMaterialDef.tradename = cswPrivate.state.tradeName;
                createMaterialDef.partno = cswPrivate.state.partNo;
                createMaterialDef.supplierid = cswPrivate.state.supplier.val;
                createMaterialDef.suppliername = cswPrivate.state.supplier.name;

                //From step 2: any properties on 'Add' layout
                if (false === Csw.isNullOrEmpty(cswPrivate.state.properties)) {
                    cswPrivate.state.properties = cswPrivate.tabsAndProps.getProps();
                    createMaterialDef.properties = cswPrivate.state.properties;
                }

                //From step 3: Sizes
                if (cswPrivate.wizard.isStepVisible(cswPrivate.makeSizesStep.stepNo)) {
                    if (false === Csw.isNullOrEmpty(cswPrivate.sizesGrid)) {
                        var sizes = cswPrivate.sizesGrid.sizes();
                        Csw.each(sizes, function (size) {
                            createMaterialDef.sizeNodes.push(size.sizeValues);
                        });
                    }
                } else {
                    Csw.tryExec(cswPrivate.makeSizesStep.onStepCleanup);
                }

                //From step 4: material document
                if (cswPrivate.wizard.isStepVisible(cswPrivate.makeAttachSDSStep.stepNo)) {
                    createMaterialDef.sdsDocId = cswPrivate.state.sdsDocId;
                    if (false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                        createMaterialDef.sdsDocProperties = cswPrivate.documentTabsAndProps.getProps();
                    }
                } else {
                    Csw.tryExec(cswPrivate.makeAttachSDSStep.onStepCleanup);
                }

                // Return the created object
                return JSON.stringify(createMaterialDef);
            }

            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'commitMaterial',
                data: {
                    MaterialDefinition: getMaterialDefinition()
                },
                success: function (data) {
                    cswPrivate.clearState();
                    Csw.tryExec(cswPrivate.onFinish, data.landingpagedata);
                }
            });
        };//cswPrivate.finalize
        //#endregion Finish

        //#region ctor
        (function () {
            Csw.extend(cswPrivate, options, true);

            cswPrivate.validateState();
            cswPrivate.currentStepNo = cswPrivate.startingStep;

            // Initialize the wizard:
            //  -Get the supplier view 
            //  -Get or create a temp node
            //  -Get documenttypeid
            Csw.ajaxWcf.post({
                urlMethod: 'Materials/initialize',
                data: cswPrivate.state.materialId,
                success: function (data) {
                    cswPrivate.supplierViewId = data.SuppliersView.ViewId;
                    cswPrivate.state.materialId = data.TempNode.NodeId;
                    cswPrivate.state.materialType.objclassid = data.TempNodeObjClassId;
                    cswPrivate.state.chemicalObjClassId = data.ChemicalObjClassId;
                    cswPrivate.state.constituentNtIds = data.ConstituentNodeTypeIds;
                    cswPrivate.state.containerlimit = data.ContainerLimit;

                    cswPrivate.containersModuleEnabled = data.ContainersModuleEnabled;
                    cswPrivate.SDSModuleEnabled = data.SDSModuleEnabled;
                    cswPrivate.AllowSupplierAdd = data.AllowSupplierAdd;

                    var stepCount = 0;
                    cswPrivate.wizardSteps = {};
                    Csw.each(data.Steps, function (Step) {
                        cswPrivate.wizardSteps[Step.StepNo] = Step.StepName;
                        stepCount++;
                    });

                    var wizardSteps = cswPrivate.setWizardSteps();

                    cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                        Title: 'Create Material',
                        StepCount: stepCount,
                        Steps: wizardSteps,
                        StartingStep: cswPrivate.startingStep,
                        FinishText: 'Finish',
                        onNext: cswPrivate.handleStep,
                        onPrevious: cswPrivate.handleStep,
                        onCancel: function () {
                            cswPrivate.clearState();
                            Csw.tryExec(cswPrivate.onCancel);
                        },
                        onFinish: cswPrivate.finalize,
                        doNextOnInit: false
                    });

                    // This checks the step visibility on refresh, C3 import, and copy.
                    cswPrivate.state.canAddSDS = Csw.bool(cswPrivate.state.materialType.objclassid === cswPrivate.state.chemicalObjClassId) && false === cswPrivate.isConstituent();
                    cswPrivate.wizard.toggleStepVisibility(cswPrivate.makeAttachSDSStep.stepNo, cswPrivate.state.canAddSDS);

                    //cswPrivate.makeStep1();
                    cswPrivate.stepFunc[1](1);
                }
            });
        }());
        //#endregion ctor

        return cswPublic;
    });
}());