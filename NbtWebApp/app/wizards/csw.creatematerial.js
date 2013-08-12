/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    var cswCreateMaterialWizardStateName = 'cswCreateMaterialWizardStateName';

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
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

                identityDiv: '', additionalPropsDiv: '', sizesDiv: '', AttachSDSDiv: '',
                materialTypeSelect: null,
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                stepFourComplete: false,
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
                    supplier: { name: '', val: '' },
                    c3supplierName: '',
                    addNewC3Supplier: false,
                    partNo: '',
                    properties: {},
                    useExistingTempNode: false,
                    physicalState: 'liquid',
                    sizes: [],
                    canAddSDS: true,
                    showOriginalUoM: false,
                    chemicalObjClassId: '',
                    constituentNtIds: ''
                },
                physicalStateModified: false,
                containersModuleEnabled: true,
                SDSModuleEnabled: true,
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
                if (cswPrivate.materialTypeSelect) {
                    ret = cswPrivate.state.constituentNtIds.contains(cswPrivate.materialTypeSelect.selectedVal());
                }
                return ret;
            } // isConstituent()

            //#region Wizard Functions

            cswPrivate.reinitSteps = function (startWithStep) {
                if (startWithStep === 2) {
                    cswPrivate.stepThreeComplete = false;
                }
                if (startWithStep === 1) {
                    cswPrivate.stepTwoComplete = false;
                }
            };

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

            cswPrivate.handleStep = function (newStepNo) {
                cswPrivate.toggleButton(cswPrivate.buttons.next, false); //let each step activate this button if needed

                cswPrivate.setState();

                if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                    cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                    cswPrivate.currentStepNo = newStepNo;

                    if (cswPrivate.currentStepNo === 2 && cswPrivate.lastStepNo === 1) {
                        if (false === cswPrivate.stepTwoComplete) {
                            cswPrivate.saveMaterial();
                        }
                        cswPrivate['makeStep' + newStepNo]();
                    } else if (cswPrivate.currentStepNo === 3 && cswPrivate.lastStepNo === 2) {
                        if (false === cswPrivate.containersModuleEnabled || cswPrivate.isConstituent()) {
                            newStepNo = 4;
                        }
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
                                    cswPrivate.reinitSteps(2);
                                    cswPrivate.state.physicalState = data.Properties.PhysicalState || '';
                                }
                                cswPrivate['makeStep' + newStepNo]();
                                if (cswPrivate.sizesGrid) {
                                    cswPrivate.sizesGrid.thinGrid.$.show();
                                }
                            },
                            error: function () {
                                //todo: add error catcher
                            }
                        });
                    } else {
                        cswPrivate['makeStep' + newStepNo]();
                    }
                }
            };

            //#endregion Wizard Functions

            //#region Step 1: Choose Type and Identity
            cswPrivate.makeStep1 = (function () {

                return function () {
                    cswPrivate.makeIdentityStep();
                };
            }());

            cswPrivate.makeIdentityStep = function () {
                function changeMaterial() {
                    if (cswPrivate.materialTypeSelect &&
                        (Csw.string(cswPrivate.state.materialType.val) !== Csw.string(cswPrivate.materialTypeSelect.val()))) {

                        cswPrivate.state.materialType = {
                            name: cswPrivate.materialTypeSelect.find(':selected').text(),
                            val: cswPrivate.materialTypeSelect.val()
                        };
                        cswPrivate.state.physicalState = ''; //Case 29015
                        cswPrivate.stepThreeComplete = false;
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

                    cswPrivate.wizard.toggleStepVisibility(cswPrivate.containersModuleEnabled ? 4 : 3, cswPrivate.state.canAddSDS);
                    if (cswPrivate.containersModuleEnabled) {
                        cswPrivate.wizard.toggleStepVisibility(3, false == cswPrivate.isConstituent());
                    }
                    if (cswPrivate.supplierSelect) {
                        if (cswPrivate.isConstituent()) {
                            cswPrivate.supplierLabel.hide();
                            cswPrivate.supplierSelect.hide();
                            cswPrivate.state.supplier = {
                                name: '',
                                val: ''
                            };
                        } else {
                            cswPrivate.supplierLabel.show();
                            cswPrivate.supplierSelect.show();
                            if (cswPrivate.supplierSelect.selectedText &&
                                Csw.string(cswPrivate.state.supplier.val) !== Csw.string(cswPrivate.supplierSelect.val())) {
                                cswPrivate.state.supplier = {
                                    name: cswPrivate.supplierSelect.selectedText(),
                                    val: cswPrivate.supplierSelect.val()
                                };
                                if (cswPrivate.supplierSelect.selectedText() === cswPrivate.newSupplierName) {
                                    cswPrivate.makeNewC3SupplierInput(true);
                                } else {
                                    cswPrivate.makeNewC3SupplierInput(false);
                                }
                            }
                        }
                    }
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
                    if (cswPrivate.newC3SupplierInput && cswPrivate.supplierSelect.selectedText) {
                        if (cswPrivate.isConstituent()) {
                            cswPrivate.newC3SupplierInput.hide();
                        } else {
                            if (cswPrivate.supplierSelect.selectedText() === cswPrivate.newSupplierName) {
                                cswPrivate.newC3SupplierInput.show();
                                cswPrivate.state.supplier = { name: cswPrivate.newC3SupplierInput.val(), val: '' };
                                cswPrivate.state.c3SupplierName = cswPrivate.newC3SupplierInput.val();
                            }
                        }
                    }
                    cswPrivate.stepTwoComplete = false;
                }//changeMaterial()

                cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                if (false === cswPrivate.stepOneComplete) {
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
                            onChange: changeMaterial,
                            onSuccess: changeMaterial,
                            isRequired: true
                        });
                    }
                    // need this for refresh
                    if (cswPrivate.containersModuleEnabled) {
                        cswPrivate.wizard.toggleStepVisibility(3, false == cswPrivate.isConstituent());
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

                    cswPrivate.makeNewC3SupplierInput = function (visible) {
                        if (!cswPrivate.newC3SupplierInput) {
                            cswPrivate.newC3SupplierInput = tbl.cell(3, 3).input({
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

                    /* Supplier */
                    cswPrivate.makeSupplierCtrl = function (NodeTypeId) {
                        tbl.cell(3, 1).empty();
                        tbl.cell(3, 2).empty();
                        tbl.cell(3, 3).empty();

                        cswPrivate.supplierLabel = tbl.cell(3, 1).span();
                        cswPrivate.supplierLabel.setLabelText('Supplier: ', true, false);

                        var allowAddButton = true;
                        var extraOptions = [];

                        // If we are importing from C3 with a new supplier, always show the
                        // 'New Supplier Name >>' option instead of the 'New+' button.
                        if (cswPrivate.state.addNewC3Supplier) {
                            allowAddButton = false;
                            extraOptions.push({ id: '', value: cswPrivate.newSupplierName });
                        }

                        var ajaxData = {};
                        if (cswPrivate.supplierViewId) {
                            ajaxData.ViewId = cswPrivate.supplierViewId;
                        } else {
                            ajaxData.ObjectClass = 'VendorClass';
                        }

                        cswPrivate.supplierSelect = tbl.cell(3, 2).nodeSelect({
                            name: 'Supplier',
                            cssclass: 'required',
                            width: '200px',
                            ajaxData: ajaxData,
                            showSelectOnLoad: true,
                            allowAdd: allowAddButton,
                            onAfterAdd: changeMaterial,
                            addNodeDialogTitle: 'Vendor',
                            selectedNodeId: cswPrivate.state.supplierId || cswPrivate.state.supplier.val,
                            onChange: changeMaterial,
                            isRequired: true,
                            extraOptions: extraOptions,
                            onSuccess: function () {
                                if (cswPrivate.supplierSelect.selectedText) {
                                    if (cswPrivate.supplierSelect.selectedText() === cswPrivate.newSupplierName) {
                                        cswPrivate.state.c3SupplierName = cswPrivate.state.supplier.name;
                                        cswPrivate.makeNewC3SupplierInput(true);
                                    }
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

                    var foundMaterialLabel = null;
                    var removeFoundMaterialLabel = function () {
                        if (false === Csw.isNullOrEmpty(foundMaterialLabel)) {
                            foundMaterialLabel.remove();
                            foundMaterialLabel = null;
                        }
                    };

                    cswPrivate.saveMaterial = function () {
                        Csw.ajax.post({
                            urlMethod: 'saveMaterial',
                            data: {
                                NodeTypeId: cswPrivate.state.materialType.val,
                                Tradename: cswPrivate.state.tradeName,
                                SupplierId: cswPrivate.state.supplier.val,
                                Suppliername: cswPrivate.state.supplier.name,
                                PartNo: cswPrivate.state.partNo,
                                NodeId: cswPrivate.state.materialId
                            },
                            success: function (data) {
                                removeFoundMaterialLabel();
                                cswPrivate.isDuplicateMaterial = Csw.bool(data.materialexists);
                                if (cswPrivate.isDuplicateMaterial) {
                                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                                    foundMaterialLabel = cswPrivate.identityDiv.nodeLink({
                                        text: "A material with these properties already exists with a tradename of " + data.noderef,
                                        name: "materialExistsLabel"
                                    });
                                } else {
                                    cswPrivate.state.materialId = data.materialid;
                                    cswPrivate.state.documentTypeId = data.documenttypeid;
                                    cswPrivate.state.properties = data.properties;
                                    cswPrivate.state.supplier.val = data.supplierid;
                                    cswPrivate.renderProps();
                                }
                            },
                            error: function () {
                                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                            }
                        });
                    };//cswPrivate.saveMaterial()

                    cswPrivate.stepOneComplete = true;

                }//if (false === cswPrivate.stepOneComplete)
            };
            //#endregion Step 1: Choose Type and Identity

            //#region Step 2: Additional Properties
            cswPrivate.makeStep2 = (function () {
                return function () {
                    cswPrivate.makeAdditionalPropsStep();
                };
            }());

            cswPrivate.isLastStep = function() {
                return ((false === cswPrivate.state.canAddSDS || false === cswPrivate.SDSModuleEnabled) &&
                            (false === cswPrivate.containersModuleEnabled || cswPrivate.isConstituent()));
            };

            cswPrivate.renderProps = function () {
                if (cswPrivate.tabsAndProps) {
                    cswPrivate.tabsAndProps.tearDown();
                }
                var propsTable = cswPrivate.additionalPropsDiv.table();
                cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(propsTable.cell(1, 1), {
                    tabState: {
                        excludeOcProps: ['tradename', 'supplier', 'partno', 'save'],
                        propertyData: cswPrivate.state.properties,
                        nodeid: cswPrivate.state.materialId,
                        ShowAsReport: false,
                        nodetypeid: cswPrivate.state.materialType.val,
                        EditMode: Csw.enums.editMode.Temp //This is intentional. We don't want the node accidental upversioned to a real node.,
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

            cswPrivate.makeAdditionalPropsStep = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                if (false === cswPrivate.stepTwoComplete) {
                    cswPrivate.additionalPropsDiv = cswPrivate.additionalPropsDiv || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.additionalPropsDiv.empty();

                    cswPrivate.additionalPropsDiv.label({
                        text: "Provide additional data for this material.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.additionalPropsDiv.br({ number: 2 }); //Changed from 4 to 2: See Case 28655

                    if (false === cswPrivate.state.useExistingTempNode) {
                        cswPrivate.SaveMaterialSuccess = function() {
                            cswPrivate.renderProps();
                        };
                    } else {
                        cswPrivate.renderProps();
                    }

                    cswPrivate.stepTwoComplete = true;

                } else {
                    var isLastStep = cswPrivate.isLastStep();

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);
                } // if (false === cswPrivate.stepTwoComplete)
            };

            //#endregion Step 2: Additional Properties

            //#region Step 3: Size(s)
            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;

                return function () {
                    cswPrivate.makeSizesStep();
                };
            }());

            cswPrivate.makeSizesStep = function () {
                var div, selectDiv;

                var isLastStep = Csw.bool(false === cswPrivate.state.canAddSDS || false === cswPrivate.SDSModuleEnabled);
                cswPrivate.toggleButton(cswPrivate.buttons.next, false === isLastStep);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, isLastStep);

                if (false === cswPrivate.stepThreeComplete) {
                    cswPrivate.sizesDiv = cswPrivate.sizesDiv || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.sizesDiv.empty();
                    div = cswPrivate.sizesDiv.div();

                    div.label({
                        text: "Sizes are used to receive material inventory. You can create additional sizes for this material elsewhere.",
                        cssclass: "wizardHelpDesc"
                    });
                    div.br({ number: 1 });

                    var makeSizeSelect = function () {

                        var makeSizeGrid = function () {
                            cswPrivate.sizesGrid = Csw.wizard.sizesGrid(div, {
                                name: 'sizesGrid',
                                sizeRowsToAdd: cswPrivate.state.sizes,
                                physicalState: cswPrivate.state.physicalState,
                                sizeNodeTypeId: cswPrivate.state.sizeNodeTypeId,
                                showQuantityEditable: cswPrivate.showQuantityEditable,
                                showDispensable: cswPrivate.showDispensable,
                                showOriginalUoM: cswPrivate.state.showOriginalUoM
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
                                Csw.ajax.post({
                                    urlMethod: 'getSizeLogicalsVisibility',
                                    data: { SizeNodeTypeId: cswPrivate.state.sizeNodeTypeId },
                                    async: false,
                                    success: function (data) {
                                        cswPrivate.showDispensable = Csw.bool(data.showDispensable);
                                        cswPrivate.showQuantityEditable = Csw.bool(data.showQuantityEditable);
                                    }
                                });
                                makeSizeGrid();
                            },
                            relatedToNodeTypeId: cswPrivate.state.materialType.val,
                            relatedObjectClassPropName: 'Material'
                        });
                        selectDiv.hide();

                        /* Populate this with onSuccess of cswPrivate.sizeSelect */
                        cswPrivate.addSizeNode = {};

                        cswPrivate.stepThreeComplete = true;
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
                }
            };
            //#endregion Step 3: Size(s)

            //#region Step 4: Attach SDS
            cswPrivate.makeStep4 = (function () {
                cswPrivate.stepFourComplete = false;

                return function () {
                    cswPrivate.makeAttachSDSStep();
                };
            }());

            cswPrivate.makeAttachSDSStep = function () {
                var attachSDSTable;

                cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, true);

                if (false === cswPrivate.stepFourComplete) {
                    cswPrivate.AttachSDSDiv = cswPrivate.AttachSDSDiv || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.AttachSDSDiv.empty();



                    cswPrivate.AttachSDSDiv.label({
                        text: "Define a Safety Data Sheet to attach to this material.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.AttachSDSDiv.br({ number: 2 });

                    attachSDSTable = cswPrivate.AttachSDSDiv.table();
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

                    cswPrivate.stepFourComplete = true;
                }
            };
            //#endregion Step 4: Attach SDS

            //#region ctor

            (function () {
                Csw.extend(cswPrivate, options, true);
                cswPrivate.validateState();
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.finalize = function () {

                    function getMaterialDefinition() {
                        var createMaterialDef = {
                            useexistingmaterial: cswPrivate.state.useExistingTempNode,
                            sizeNodes: []
                        };

                        //From step 0: request, materialid
                        createMaterialDef.request = cswPrivate.state.request || cswPrivate.request;
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
                        if (false === Csw.isNullOrEmpty(cswPrivate.sizesGrid)) {
                            var sizes = cswPrivate.sizesGrid.sizes();
                            Csw.each(sizes, function (size) {
                                createMaterialDef.sizeNodes.push(size.sizeValues);
                            });
                            createMaterialDef.deletedSizes = cswPrivate.sizesGrid.deletedSizes();
                        }

                        //From step 4: material document
                        createMaterialDef.sdsDocId = cswPrivate.state.sdsDocId;
                        if (false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                            createMaterialDef.sdsDocProperties = cswPrivate.documentTabsAndProps.getProps();
                        }

                        // Return the created object
                        return JSON.stringify(createMaterialDef);
                    }

                    Csw.ajax.post({
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

                        cswPrivate.containersModuleEnabled = data.ContainersModuleEnabled;
                        cswPrivate.SDSModuleEnabled = data.SDSModuleEnabled;

                        var stepCount = 0;
                        cswPrivate.wizardSteps = {};
                        Csw.each(data.Steps, function (Step) {
                            cswPrivate.wizardSteps[Step.StepNo] = Step.StepName;
                            stepCount++;
                        });

                        cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                            Title: 'Create Material',
                            StepCount: stepCount,
                            Steps: cswPrivate.wizardSteps,
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

                        // This checks the step visibility on refresh and on C3 import.
                        cswPrivate.state.canAddSDS = Csw.bool(cswPrivate.state.materialType.objclassid === cswPrivate.state.chemicalObjClassId);
                        cswPrivate.wizard.toggleStepVisibility(cswPrivate.containersModuleEnabled ? 4 : 3, cswPrivate.state.canAddSDS);

                        cswPrivate.makeStep1();
                    }
                });

            }());

            //#endregion ctor

            return cswPublic;
        });
}());