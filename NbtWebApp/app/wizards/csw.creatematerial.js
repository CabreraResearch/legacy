/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    var cswCreateMaterialWizardStateName = 'cswCreateMaterialWizardStateName';

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
        Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
            'use strict';

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
                divStep1: '', divStep2: '', divStep3: '', divStep4: '',
                materialTypeSelect: null,
                sizeNodes: [],
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                stepFourComplete: false,
                config: {
                    quantityName: 'Initial Quantity',
                    numberName: 'Catalog No.',
                    dispensibleName: 'Dispensible',
                    quantityEditableName: 'Quantity Editable',
                    unitCountName: 'Unit Count'
                },
                rows: [],
                tabsAndProps: null,
                documentTabsAndProps: null,
                showQuantityEditable: false,
                showDispensable: false,
                state: {
                    sizeNodeTypeId: '',
                    relatedNodeId: null,
                    materialId: '',
                    documentTypeId: '',
                    documentId: '',
                    materialType: { name: '', val: '' },
                    tradeName: '',
                    supplier: { name: '', val: '' },
                    partNo: '',
                    properties: {},
                    documentProperties: {},
                    useExistingMaterial: false,
                    materialProperies: {}
                }
            };

            var cswPublic = {
                rows: {
                    rowid: {
                        catalogNoCtrl: {},
                        quantityCtrl: {},
                        unitsCtrl: {},
                        dispensibleCtrl: {},
                        quantEditableCtrl: {},
                        unitCountCtrl: {},
                        sizeValues: {}
                    }
                },
                sizes: [],
                sizesForm: null,
                sizeGrid: null
            };

            cswPrivate.validateState = function () {
                var state;
                if (Csw.isNullOrEmpty(cswPrivate.state.tradeName)) {
                    state = cswPrivate.getState();
                    Csw.extend(cswPrivate.state, state);
                }
                cswPrivate.setState();
            };

            cswPrivate.getState = function () {
                return Csw.clientDb.getItem(cswPrivate.name + '_' + cswCreateMaterialWizardStateName);
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_' + cswCreateMaterialWizardStateName, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_' + cswCreateMaterialWizardStateName);
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

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.validateState();
                cswPrivate.wizardSteps = {
                    1: 'Choose Type and Identity',
                    2: 'Additional Properties',
                    3: 'Size(s)',
                    4: 'Attach MSDS'
                };

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    cswPrivate.setState();
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();

                        if (cswPrivate.currentStepNo === 2) {
                            if (cswPrivate.lastStepNo === 3) {
                                cswPrivate.state.materialId = '';
                                cswPrivate.state.documentId = '';
                                cswPrivate.state.properties = {};
                                cswPrivate.state.useExistingMaterial = false;
                                cswPrivate.reinitSteps(2);
                            }
                            cswPrivate.createMaterial();
                            if (cswPrivate.isDuplicateMaterial) {

                                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                            }
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps) &&
                            cswPrivate.currentStepNo > 2) {

                            cswPrivate.state.properties = cswPrivate.tabsAndProps.getPropJson();
                            if (cswPrivate.lastStepNo === 2) {
                                cswPrivate.tabsAndProps.save({}, '', null, false);
                            }
                        }
                        if (false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                            cswPrivate.state.documentProperties = cswPrivate.documentTabsAndProps.getPropJson();
                        }
                    }
                };

                cswPrivate.finalize = function () {
                    function getMaterialDefinition() {
                        var createMaterialDef = {
                            useexistingmaterial: cswPrivate.state.useExistingMaterial,
                            sizes: cswPrivate.sizeNodes,
                            sizeNodes: []
                        };

                        if (false === cswPrivate.state.useExistingMaterial) {
                            createMaterialDef.materialId = cswPrivate.state.materialId;
                            createMaterialDef.materialnodetypeid = cswPrivate.state.materialType.val;
                            createMaterialDef.tradename = cswPrivate.state.tradeName;
                            createMaterialDef.partno = cswPrivate.state.partNo;
                            createMaterialDef.documentid = cswPrivate.state.documentId;
                            createMaterialDef.supplierid = cswPrivate.state.supplier.val;
                            createMaterialDef.suppliername = cswPrivate.state.supplier.name;
                            Csw.each(cswPublic.rows, function (row) {
                                createMaterialDef.sizeNodes.push(row.sizeValues);
                            });
                            createMaterialDef.properties = cswPrivate.state.properties;
                            if (false === Csw.isNullOrEmpty(cswPrivate.documentTabsAndProps)) {
                                createMaterialDef.documentProperties = cswPrivate.documentTabsAndProps.getPropJson();
                            }
                        } else {
                            createMaterialDef.materialnodeid = cswPrivate.materialNodeId;
                        }
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
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'Create Material',
                    StepCount: 4,
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

            } ());


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

            //Step 1: 
            cswPrivate.makeStep1 = (function () {

                return function () {
                    function changeMaterial() {
                        var hasChanged = false;
                        if (cswPrivate.supplierSelect &&
                            cswPrivate.state.supplier.val !== cswPrivate.supplierSelect.val()) {

                            hasChanged = true;
                            cswPrivate.state.supplier = { name: cswPrivate.supplierSelect.selectedText(), val: cswPrivate.supplierSelect.val() };
                        }
                        if (cswPrivate.materialTypeSelect &&
                            cswPrivate.state.materialType.val !== cswPrivate.materialTypeSelect.val()) {

                            hasChanged = true;
                            cswPrivate.state.materialType = { name: cswPrivate.materialTypeSelect.find(':selected').text(), val: cswPrivate.materialTypeSelect.val() };
                        }
                        if (cswPrivate.tradeNameInput &&
                            cswPrivate.state.tradeName !== cswPrivate.tradeNameInput.val()) {

                            hasChanged = true;
                            cswPrivate.state.tradeName = cswPrivate.tradeNameInput.val();
                        }
                        if (cswPrivate.partNoInput &&
                            cswPrivate.state.partNo !== cswPrivate.partNoInput.val()) {

                            hasChanged = true;
                            cswPrivate.state.partNo = cswPrivate.partNoInput.val();
                        }

                        if (hasChanged) {
                            cswPrivate.state.materialId = '';
                            cswPrivate.state.documentId = '';
                            cswPrivate.state.properties = {};
                            cswPrivate.state.useExistingMaterial = false;
                            cswPrivate.reinitSteps(2);
                        }
                    }

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.label({
                            text: "This wizard will guide you through the process of creating a new material. If the attributes below match an existing material, you will be given the option to view that material.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 4 });

                        cswPrivate.materialTypeSelect = cswPrivate.divStep1.nodeTypeSelect({
                            name: 'nodeTypeSelect',
                            useWide: true,
                            labelText: 'Select a Material Type:',
                            objectClassName: 'MaterialClass',
                            value: cswPrivate.state.materialType.val || cswPrivate.state.materialNodeTypeId,
                            selectedName: 'Chemical',
                            onSelect: changeMaterial,
                            onChange: changeMaterial,
                            onSuccess: changeMaterial,
                            isRequired: true
                        });
                        cswPrivate.divStep1.br({ number: 1 });

                        /* TRADENAME */
                        cswPrivate.tradeNameInput = cswPrivate.divStep1.input({
                            name: 'tradename',
                            useWide: true,
                            labelText: 'Tradename:',
                            cssclass: 'required',
                            value: cswPrivate.state.tradeName,
                            onChange: function () {
                                changeMaterial();
                            },
                            isRequired: true
                        });
                        cswPrivate.divStep1.br({ number: 1 });

                        /* SUPPLIER */
                        cswPrivate.supplierSelect = cswPrivate.divStep1.nodeSelect({
                            name: 'supplier',
                            cssclass: 'required',
                            objectClassName: 'VendorClass',
                            addNodeDialogTitle: 'Vendor',
                            useWide: true,
                            selectedNodeId: cswPrivate.state.supplierId || cswPrivate.state.supplier.val,
                            labelText: 'Supplier: ',
                            onChange: changeMaterial,
                            onSuccess: changeMaterial,
                            isRequired: true
                        });
                        //cswPrivate.divStep1.br({ number: 1 });

                        /* PARTNO */
                        cswPrivate.partNoInput = cswPrivate.divStep1.input({
                            name: 'partno',
                            useWide: true,
                            value: cswPrivate.state.partNo,
                            labelText: 'Part No:',
                            onChange: function () {
                                changeMaterial();
                            }
                        });

                        cswPrivate.divStep1.br({ number: 3 });

                        var foundMaterialLabel = null;
                        var removeFoundMaterialLabel = function () {
                            if (false === Csw.isNullOrEmpty(foundMaterialLabel)) {
                                foundMaterialLabel.remove();
                                foundMaterialLabel = null;
                            }
                        };
                        cswPrivate.createMaterial = function () {
                            Csw.ajax.post({
                                urlMethod: 'createMaterial',
                                async: false,
                                data: {
                                    NodeTypeId: cswPrivate.state.materialType.val,
                                    Tradename: cswPrivate.state.tradeName,
                                    Supplier: cswPrivate.state.supplier.val,
                                    PartNo: cswPrivate.state.partNo
                                },
                                success: function (data) {
                                    removeFoundMaterialLabel();
                                    cswPrivate.isDuplicateMaterial = Csw.bool(data.materialexists);
                                    if (cswPrivate.isDuplicateMaterial) {
                                        cswPrivate.toggleButton(cswPrivate.buttons.prev, false, true);
                                        foundMaterialLabel = cswPrivate.divStep1.nodeLink({
                                            text: "A material with these properties already exists with a tradename of " + data.noderef,
                                            name: "materialExistsLabel"
                                        });
                                    } else {
                                        cswPrivate.state.materialId = data.materialid;
                                        cswPrivate.state.documentTypeId = data.documenttypeid;
                                        cswPrivate.state.properties = data.properties;
                                        Csw.publish('CreateMaterialSuccess');
                                    }
                                },
                                error: function () {
                                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false, true);
                                }
                            });
                        };

                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ());

            cswPrivate.makeStep2 = (function () {
                cswPrivate.stepTwoComplete = false;

                return function () {
                    var div;

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

                    if (false === cswPrivate.stepTwoComplete &&
                        false === cswPrivate.state.useExistingMaterial) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.label({
                            text: "Provide additional data for this material.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 4 });

                        div = cswPrivate.divStep2.div();
                        Csw.subscribe('CreateMaterialSuccess', function () {
                            cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(div, {
                                globalState: {
                                    excludeOcProps: ['tradename', 'supplier', 'partno'],
                                    currentNodeId: cswPrivate.state.materialId,
                                    propertyData: cswPrivate.state.properties,
                                    ShowAsReport: false
                                },
                                tabState: {
                                    showSaveButton: false,
                                    nodetypeid: cswPrivate.state.materialType.val,
                                    EditMode: Csw.enums.editMode.Temp //This is intentional. We don't want the node accidental upversioned to a real node.
                                },
                                ReloadTabOnSave: false,
                                async: false
                            });

                            //cswPrivate.stepTwoComplete = true;
                        });
                    }
                };

            } ());

            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;

                return function () {
                    var div, selectDiv;

                    function isSizeNew(size) {
                        var ret = true;
                        for (var i = 0; i < cswPublic.rows.length; i++) {
                            if (cswPublic.rows[i].sizeValues["quantity"] === size["quantity"] &&
                                cswPublic.rows[i].sizeValues["unit"] === size["unit"] &&
                                cswPublic.rows[i].sizeValues["catalogNo"] === size["catalogNo"]
                            ) {
                                ret = false;
                            }
                        }
                        return ret;
                    }

                    function sizeSelect(retObj, count) {
                        cswPrivate.state.sizeNodeTypeId = cswPrivate.sizeSelect.val();
                        if (count > 1) {
                            selectDiv.show();
                        }
                    }

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();
                        div = cswPrivate.divStep3.div();



                        div.label({
                            text: "Sizes are used to receive material inventory.  You can create additional sizes for this material elsewhere.",
                            cssclass: "wizardHelpDesc"
                        });
                        div.br({ number: 1 });

                        var makeGrid = function () {

                            //get Units of Measure for this Material
                            var unitsOfMeasure = [];
                            Csw.ajax.post({
                                urlMethod: 'getMaterialUnitsOfMeasure',
                                data: {
                                    MaterialId: cswPrivate.state.materialId
                                },
                                async: false, //wait for this request to finish
                                success: function (data) {
                                    unitsOfMeasure = data;
                                }
                            });

                            var getID = function (unitType) {
                                var ret = '';
                                Csw.each(unitsOfMeasure, function (obj, key) {
                                    if (unitsOfMeasure[key] === unitType) {
                                        ret = key;
                                    }
                                });
                                return ret;
                            };

                            cswPrivate.header = [{ "value": cswPrivate.config.unitCountName, "isRequired": false },
                                { "value": cswPrivate.config.quantityName, "isRequired": true },
                                { "value": cswPrivate.config.numberName, "isRequired": false}];
                            if (cswPrivate.showQuantityEditable) {
                                cswPrivate.header = cswPrivate.header.concat([{ "value": cswPrivate.config.quantityEditableName, "isRequired": false}]);
                            }
                            if (cswPrivate.showDispensable) {
                                cswPrivate.header = cswPrivate.header.concat([{ "value": cswPrivate.config.dispensibleName, "isRequired": false}]);
                            }
                            if (cswPrivate.rows.length === 0) {
                                cswPrivate.rows.push(cswPrivate.header);
                            }
                            cswPublic.sizesForm = cswPrivate.divStep3.form();
                            cswPublic.sizeGrid = cswPublic.sizesForm.thinGrid({
                                linkText: '',
                                hasHeader: true,
                                rows: cswPrivate.rows,
                                allowDelete: true,
                                allowAdd: true,
                                makeAddRow: function (cswCell, columnName, rowid) {
                                    'use strict';

                                    switch (columnName) {
                                        case cswPrivate.config.unitCountName:
                                            cswPublic.rows[rowid].unitCountCtrl = cswCell.numberTextBox({
                                                name: 'sizeUnitCount',
                                                value: 1,
                                                MinValue: 1,
                                                Precision: 0,
                                                onChange: function (value) {
                                                    cswPublic.rows[rowid].sizeValues.unitCount = cswPublic.rows[rowid].unitCountCtrl.val();
                                                }
                                            });
                                            cswCell.span({ text: ' x' });
                                            break;
                                        case cswPrivate.config.quantityName:
                                            cswPublic.rows[rowid].quantityCtrl = cswCell.numberTextBox({
                                                name: 'quantityNumberBox',
                                                MinValue: 0,
                                                excludeRangeLimits: true,
                                                width: '60px',
                                                onChange: function (value) {
                                                    cswPublic.rows[rowid].sizeValues.quantity = cswPublic.rows[rowid].quantityCtrl.val();
                                                }
                                            });
                                            cswPublic.rows[rowid].unitsCtrl = cswCell.select({
                                                name: 'unitsOfMeasureSelect',
                                                values: unitsOfMeasure,
                                                onChange: function (value) {
                                                    cswPublic.rows[rowid].sizeValues.unit = cswPublic.rows[rowid].unitsCtrl.val();
                                                    cswPublic.rows[rowid].sizeValues.unitid = getID(cswPublic.rows[rowid].sizeValues.unit);
                                                }
                                            });
                                            cswPublic.rows[rowid].sizeValues.unit = cswPublic.rows[rowid].unitsCtrl.val();
                                            cswPublic.rows[rowid].sizeValues.unitid = getID(cswPublic.rows[rowid].sizeValues.unit);
                                            break;
                                        case cswPrivate.config.numberName:
                                            cswPublic.rows[rowid].catalogNoCtrl = cswCell.input({
                                                name: 'sizeCatalogNo',
                                                width: '80px',
                                                onChange: function (value) {
                                                    cswPublic.rows[rowid].sizeValues.catalogNo = value;
                                                }
                                            });
                                            break;
                                        case cswPrivate.config.quantityEditableName:
                                            cswPublic.rows[rowid].quantEditableCtrl = cswCell.checkBox({
                                                name: 'sizeQuantEditable',
                                                checked: true,
                                                onChange: function (value) {
                                                    cswPublic.rows[rowid].sizeValues.quantEditableChecked = cswPublic.rows[rowid].quantEditableCtrl.val();
                                                }
                                            });
                                            break;
                                        case cswPrivate.config.dispensibleName:
                                            cswPublic.rows[rowid].dispensibleCtrl = cswCell.checkBox({
                                                name: 'sizeDispensible',
                                                checked: true,
                                                onChange: function (value) {
                                                    cswPublic.rows[rowid].sizeValues.dispensibleChecked = cswPublic.rows[rowid].dispensibleCtrl.val();
                                                }
                                            });
                                            break;
                                    }
                                },
                                onAdd: function (newRowid) {
                                    var newSize = {};
                                    //This while loop serves as a buffer to remove the +1/-1 issues when comparing the data with the table cell rows in the thingrid.
                                    //This puts the burden on the user of thingrid to ensure their data lines up with the table cells.
                                    //Also, undefined size values break the serverside foreach loop, so an empty one is inserted in each element (including deleted elements).
                                    while (cswPublic.rows.length < newRowid) {
                                        cswPublic.rows[newRowid] = { sizeValues: {} };
                                    }
                                    newSize = {
                                        catalogNo: '',
                                        quantity: '',
                                        unit: '',
                                        unitid: '',
                                        unitCount: 1,
                                        quantEditableChecked: 'true',
                                        dispensibleChecked: 'true',
                                        nodetypeid: cswPrivate.state.sizeNodeTypeId
                                    };
                                    var extractNewAmount = function (object) {
                                        var ret = Csw.extend({}, object, true);
                                        return ret;
                                    };
                                    cswPublic.rows[newRowid] = { sizeValues: extractNewAmount(newSize) };
                                },
                                onDelete: function (rowid) {
                                    delete cswPublic.rows[rowid];
                                    cswPublic.rows[rowid] = { sizeValues: {} };
                                }
                            });
                        };

                        div.br();

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
                                makeGrid();
                            },
                            relatedToNodeTypeId: cswPrivate.state.materialType.val,
                            relatedObjectClassPropName: 'Material'
                        });
                        selectDiv.hide();

                        /* Populate this with onSuccess of cswPrivate.sizeSelect */
                        cswPrivate.addSizeNode = {};

                        cswPrivate.stepThreeComplete = true;
                    }
                };

            } ());

            //MSDS upload
            cswPrivate.makeStep4 = (function () {
                cswPrivate.stepFourComplete = false;

                return function () {
                    var div;

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);

                    if (false === cswPrivate.stepFourComplete) {
                        cswPrivate.divStep4 = cswPrivate.divStep4 || cswPrivate.wizard.div(4);
                        cswPrivate.divStep4.empty();

                        if (Csw.isNullOrEmpty(cswPrivate.state.documentTypeId)) {
                            cswPrivate.divStep4.label({
                                text: "No Material Documents have been defined. Click Finish to complete the wizard.",
                                cssclass: "wizardHelpDesc"
                            });
                        } else {

                            cswPrivate.divStep4.label({
                                text: "Define a Material Safety Data Sheet to attach to this material.",
                                cssclass: "wizardHelpDesc"
                            });
                            cswPrivate.divStep4.br({ number: 4 });

                            div = cswPrivate.divStep4.div();

                            cswPrivate.documentTabsAndProps = Csw.layouts.tabsAndProps(div, {
                                globalState: {
                                    ShowAsReport: false,
                                    excludeOcProps: ['owner']
                                },
                                tabState: {
                                    nodetypeid: cswPrivate.state.documentTypeId,
                                    showSaveButton: false,
                                    EditMode: Csw.enums.editMode.Add
                                },
                                ReloadTabOnSave: false,
                                onNodeIdSet: function (documentId) {
                                    cswPrivate.state.documentId = documentId;
                                }
                            });

                        }

                        cswPrivate.stepFourComplete = true;
                    }
                };

            } ());

            cswPrivate.makeStep1();

            return cswPublic;
        });
} ());