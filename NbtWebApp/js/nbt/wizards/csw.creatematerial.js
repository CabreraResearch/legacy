/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    var cswCreateMaterialWizardStateName = 'cswCreateMaterialWizardStateName';

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
        Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
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
                divStep1: '', divStep2: '', divStep3: '',
                materialTypeSelect: null,
                sizeNodes: [],
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                stepFourComplete: false,
                config: {
                    quantityName: 'Initial Quantity *',
                    numberName: 'Catalog No.',
                    dispensibleName: 'Dispensible',
                    quantEditableName: 'Quantity Editable'
                },
                rows: [],
                relatedNodeId: null,
                state: {
                    sizeNodeTypeId: '',
                    relatedNodeId: null,
                    materialId: '',
                    materialType: { name: '', val: '' },
                    tradeName: '',
                    supplier: { name: '', val: '' },
                    partNo: '',
                    physicalState: '',
                    useExistingMaterial: false,
                    materialProperies: {}
                }
            };

            var cswPublic = {
                physicalStateCrl: null,
                catalogNoCtrl: null,
                quantityCtrl: null,
                untitsCrl: null,
                dispensibleCtrl: null,
                quantEditableCtrl: null,
                sizesForm: null,
                sizeGrid: null,
                sizes: []
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
                return Csw.clientDb.getItem(cswPrivate.ID + '_' + cswCreateMaterialWizardStateName);
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.ID + '_' + cswCreateMaterialWizardStateName, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.ID + '_' + cswCreateMaterialWizardStateName);
            };

            cswPrivate.reinitSteps = function (startWithStep) {
                cswPrivate.stepThreeComplete = false;

                if (startWithStep <= 2) {
                    cswPrivate.state.physicalState = '';
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
                    3: 'Size(s)'
                };

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    cswPrivate.setState();
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();

                        if (cswPrivate.currentStepNo === 2 &&
                            cswPrivate.isDuplicateMaterial) {
                            cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                        }
                    }
                };

                cswPrivate.finalize = function () {
                    function getMaterialDefinition() {
                        var createMaterialDef = {
                            useexistingmaterial: cswPrivate.state.useExistingMaterial,
                            sizes: cswPrivate.sizeNodes
                        };

                        if (false === cswPrivate.state.useExistingMaterial) {
                            createMaterialDef.materialId = cswPrivate.state.materialId;
                            createMaterialDef.materialnodetypeid = cswPrivate.state.materialType.val;
                            createMaterialDef.tradename = cswPrivate.state.tradeName;
                            createMaterialDef.partno = cswPrivate.state.partNo;
                            createMaterialDef.supplierid = cswPrivate.state.supplier.val;
                            createMaterialDef.suppliername = cswPrivate.state.supplier.name;
                            createMaterialDef.physicalState = cswPrivate.state.physicalState;
                            createMaterialDef.sizeNodes = cswPublic.sizes;
                            if (false === Csw.isNullOrEmpty(cswPrivate.tabsAndProps)) {
                                createMaterialDef.properties = cswPrivate.tabsAndProps.getPropJson();
                            }
                        } else {
                            createMaterialDef.materialnodeid = cswPrivate.materialNodeId;
                        }
                        return JSON.stringify(createMaterialDef);
                    }

                    Csw.ajax.post({
                        urlMethod: 'createMaterial',
                        data: {
                            MaterialDefinition: getMaterialDefinition()
                        },
                        success: function (data) {
                            cswPrivate.clearState();
                            var viewid = '';
                            if (Csw.contains(data, 'nextoptions')) {
                                viewid = data.nextoptions.nodeview;
                            }
                            Csw.tryExec(cswPrivate.onFinish, viewid);
                        }
                    });
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    ID: Csw.makeId(cswPrivate.ID, 'wizard'),
                    Title: 'Create Material',
                    StepCount: 3,
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
                    function supplierSelect() {
                        cswPrivate.state.supplier = { name: cswPrivate.supplierSelect.selectedText(), val: cswPrivate.supplierSelect.val() };
                        cswPrivate.reinitSteps(2);
                    }
                    function typeSelect() {
                        cswPrivate.state.materialType = { name: cswPrivate.materialTypeSelect.find(':selected').text(), val: cswPrivate.materialTypeSelect.val() };
                        cswPrivate.reinitSteps(2);
                    }

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.label({
                            text: "This wizard will guide you through the process of creating a new material. If the attributes below match an existing material, you will be given the option to view that material.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 4 });

                        cswPrivate.materialTypeSelect = cswPrivate.divStep1.nodeTypeSelect({
                            ID: cswPrivate.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            labelText: 'Select a Material Type*: ',
                            objectClassName: 'MaterialClass',
                            onSelect: typeSelect,
                            value: cswPrivate.state.materialType.val || cswPrivate.state.materialNodeTypeId,
                            onChange: function () {
                                typeSelect();
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.tradeName)) {
                                    checkIfMaterialExists();
                                    cswPrivate.reinitSteps(2);
                                }
                            },
                            onSuccess: typeSelect
                        });
                        cswPrivate.divStep1.br({ number: 1 });

                        /* TRADENAME */
                        cswPrivate.tradeNameInput = cswPrivate.divStep1.input({
                            ID: cswPrivate.wizard.makeStepId('tradename'),
                            useWide: true,
                            labelText: 'Tradename*: ',
                            cssclass: 'required',
                            value: cswPrivate.state.tradeName,
                            onChange: function () {
                                cswPrivate.state.tradeName = cswPrivate.tradeNameInput.val();
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.tradeName)) {
                                    checkIfMaterialExists();
                                } else {
                                    removeFoundMaterialLabel();
                                }
                            }
                        });
                        cswPrivate.divStep1.br({ number: 1 });

                        /* SUPPLIER */
                        cswPrivate.supplierSelect = cswPrivate.divStep1.nodeSelect({
                            ID: cswPrivate.wizard.makeStepId('supplier'),
                            cssclass: 'required',
                            objectClassName: 'VendorClass',
                            useWide: true,
                            selectedNodeId: cswPrivate.state.supplierId,
                            labelText: 'Supplier*: ',
                            onChange: function () {
                                supplierSelect();
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.tradeName)) {
                                    checkIfMaterialExists();
                                }
                            },
                            onSuccess: supplierSelect
                        });
                        //cswPrivate.divStep1.br({ number: 1 });

                        /* PARTNO */
                        cswPrivate.partNoInput = cswPrivate.divStep1.input({
                            ID: cswPrivate.wizard.makeStepId('partno'),
                            useWide: true,
                            value: cswPrivate.state.partNo,
                            labelText: 'Part No: ',
                            onChange: function () {
                                cswPrivate.state.partNo = cswPrivate.partNoInput.val();
                                if (false === Csw.isNullOrEmpty(cswPrivate.state.tradeName)) {
                                    checkIfMaterialExists();
                                }
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
                        var checkIfMaterialExists = function () {
                            Csw.ajax.post({
                                urlMethod: 'getMaterial',
                                async: false,
                                data: {
                                    NodeTypeId: cswPrivate.state.materialType.val,
                                    Tradename: cswPrivate.state.tradeName,
                                    Supplier: cswPrivate.state.supplier.name,
                                    PartNo: cswPrivate.state.partNo
                                },
                                success: function (data) {
                                    removeFoundMaterialLabel();
                                    cswPrivate.isDuplicateMaterial = materialExists(data);
                                    if (cswPrivate.isDuplicateMaterial) {
                                        foundMaterialLabel = cswPrivate.divStep1.nodeLink({
                                            text: "A material with these properties already exists with a tradename of " + data.noderef,
                                            ID: "materialExistsLabel"
                                        });
                                    }
                                }
                            });
                        };

                        var materialExists = function (data) {
                            var ret = false;
                            if (data["tradename"] == cswPrivate.state.tradeName &&
                                data["supplier"] == cswPrivate.state.supplier.name &&
                                data["partno"] == cswPrivate.state.partNo &&
                                data["nodetypeid"] == cswPrivate.state.materialType.val) {
                                ret = true;
                            }
                            return ret;
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
                        var hiddenDiv = cswPrivate.divStep2.div();
                        cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(hiddenDiv, {
                            nodetypeid: cswPrivate.state.materialType.val,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['tradename', 'supplier', 'partno'],
                            onNodeIdSet: function (materialId) {
                                cswPrivate.state.materialId = materialId;
                                Csw.ajax.post({
                                    urlMethod: 'getProps',
                                    data: {
                                        EditMode: Csw.enums.editMode.Add,
                                        NodeId: cswPrivate.state.materialId,
                                        NodeTypeId: cswPrivate.state.materialType.val,
                                        SafeNodeKey: '',
                                        TabId: '',
                                        Date: '',
                                        Multi: '',
                                        filterToPropId: '',
                                        ConfigMode: '',
                                        RelatedNodeId: '',
                                        RelatedNodeTypeId: '',
                                        RelatedObjectClassId: ''
                                    },
                                    success: function (data) {

                                        //splits the comma delimited string into an array for the selector
                                        var getOpts = function (str) {
                                            var ret = [];
                                            var splitStr = str.split(",");
                                            for (var i = 0; i < splitStr.length; i++) {
                                                ret.push({ display: splitStr[i], value: splitStr[i] });
                                            }
                                            return ret;
                                        };

                                        for (var propKey in data) {
                                            var propertyData = data[propKey];
                                            switch (propertyData["fieldtype"]) {
                                                case "List":
                                                    div.span({ text: propertyData["name"] + "*:", cssclass: "PhysicalStateSpan" });
                                                    cswPublic.physicalStateCrl = div.select({
                                                        ID: Csw.tryExec(Csw.makeId, 'physState'),
                                                        selected: cswPrivate.state.physicalState,
                                                        values: getOpts(propertyData["values"]["options"]),
                                                        onChange: function () {
                                                            cswPrivate.state.physicalState = cswPublic.physicalStateCrl.val();
                                                            cswPrivate.reinitSteps(3);
                                                        }
                                                    });
                                                    if (false === Csw.isNullOrEmpty(cswPublic.physicalStateCrl)) {
                                                        cswPrivate.state.physicalState = cswPublic.physicalStateCrl.val();
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                });
                            }
                        });
                        
                        hiddenDiv.hide();

                        cswPrivate.stepTwoComplete = true;
                    }
                };

            } ());

            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;

                return function () {
                    var div, selectDiv;

                    function isSizeNew(size) {
                        var ret = true;
                        for (var i = 0; i < cswPublic.sizes.length; i++) {
                            if (cswPublic.sizes[i]["quantity"] === size["quantity"] &&
                                cswPublic.sizes[i]["unit"] === size["unit"]
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
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();
                        div = cswPrivate.divStep3.div();

                        div.label({
                            text: "Sizes are used to receive material inventory. This step is optional - you may create sizes for this material elsewhere.",
                            cssclass: "wizardHelpDesc"
                        });
                        div.br({ number: 1 });

                        var makeGrid = function () {

                            //get Units of Measure for this Material
                            var unitsOfMeasure = [];
                            Csw.ajax.post({
                                urlMethod: 'getMaterialUnitsOfMeasure',
                                data: {
                                    PhysicalState: cswPrivate.state.physicalState || 'n/a' //if we couldn't choose a state, assume it's a supply
                                },
                                async: false, //wait for this request to finish
                                success: function (data) {
                                    unitsOfMeasure = data;
                                }
                            });

                            var newSize = {
                                rowid: 1,
                                catalogNo: '',
                                quantity: '',
                                unit: '',
                                unitid: '',
                                quantEditableChecked: 'false', //default?
                                dispensibleChecked: 'false', //default?
                                nodetypeid: cswPrivate.state.sizeNodeTypeId
                            };

                            var extendNewAmount = function (object) {
                                //To mitigate the risk of unknowingly passing the outer scope thisAmount, we're explicitly mapping the values down
                                Csw.extend(newSize, object);
                            };

                            var extractNewAmount = function (object) {
                                var ret = Csw.extend({}, object, true);
                                return ret;
                            };

                            cswPrivate.header = [cswPrivate.config.quantityName, cswPrivate.config.numberName, cswPrivate.config.quantEditableName, cswPrivate.config.dispensibleName];
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
                                    var thisSize = {
                                        rowid: rowid,
                                        catalogNo: '',
                                        quantity: '',
                                        unit: '',
                                        unitid: '',
                                        quantEditableChecked: 'false', //default?
                                        dispensibleChecked: 'false' //default?
                                    };

                                    switch (columnName) {
                                        case cswPrivate.config.quantityName:
                                            cswPublic.quantityCtrl = cswCell.numberTextBox({
                                                ID: Csw.tryExec(Csw.makeId, 'quantityNumberBox'),
                                                width: '60px'
                                            });
                                            cswPublic.unitsCtrl = cswCell.select({
                                                ID: Csw.tryExec(Csw.makeId, 'unitsOfMeasureSelect'),
                                                values: unitsOfMeasure
                                            });
                                            break;
                                        case cswPrivate.config.numberName:
                                            cswPublic.catalogNoCtrl = cswCell.input({
                                                ID: Csw.tryExec(Csw.makeId, 'sizeCatalogNo'),
                                                width: '80px',
                                                onChange: function (value) {
                                                    thisSize.catalogNo = value;
                                                    extendNewAmount(thisSize);
                                                }
                                            });
                                            break;
                                        case cswPrivate.config.quantEditableName:
                                            cswPublic.quantEditableCtrl = cswCell.checkBox({
                                                ID: Csw.tryExec(Csw.makeId, 'sizeQuantEditable'),
                                                Checked: true,
                                                onChange: function (value) {
                                                    thisSize.quantEditableChecked = cswPublic.quantEditableCtrl.val();
                                                    extendNewAmount(thisSize);
                                                }
                                            });
                                            break;
                                        case cswPrivate.config.dispensibleName:
                                            cswPublic.dispensibleCtrl = cswCell.checkBox({
                                                ID: Csw.tryExec(Csw.makeId, 'sizeDispensible'),
                                                Checked: true,
                                                onChange: function (value) {
                                                    thisSize.dispensibleChecked = cswPublic.dispensibleCtrl.val();
                                                    extendNewAmount(thisSize);
                                                }
                                            });
                                            break;
                                    }
                                    extendNewAmount(thisSize);
                                },
                                onAdd: function () {

                                    var getID = function (unitType) {
                                        var ret = '';
                                        for (var key in unitsOfMeasure) {
                                            if (unitsOfMeasure[key] === unitType) {
                                                ret = key;
                                            }
                                        }
                                        return ret;
                                    };

                                    if (cswPublic.sizesForm.isFormValid()) {
                                        newSize.quantity = cswPublic.quantityCtrl.val();
                                        newSize.unit = cswPublic.unitsCtrl.val();
                                        newSize.catalogNo = cswPublic.catalogNoCtrl.val();
                                        newSize.dispensibleChecked = cswPublic.dispensibleCtrl.val();
                                        newSize.quantEditableChecked = cswPublic.quantEditableCtrl.val();
                                        newSize.unitid = getID(newSize.unit);
                                        if (false === Csw.isNullOrEmpty(newSize.quantity)) {
                                            if (isSizeNew(newSize)) {
                                                cswPublic.sizes.push(extractNewAmount(newSize));
                                                cswPublic.sizeGrid.addRows([newSize.quantity + ' ' + newSize.unit, newSize.catalogNo, newSize.quantEditableChecked, newSize.dispensibleChecked]);
                                            } else {
                                                $.CswDialog('AlertDialog', 'This size is already defined. Please define a new, unique size.');
                                            }
                                        } else {
                                            $.CswDialog('AlertDialog', 'A quantity must be specified when creating a size. Please specify the quantity.');
                                        }
                                    }
                                },
                                onDelete: function (rowid) {
                                    var reducedSizes = cswPublic.sizes.filter(function (size) {
                                        return size.rowid != rowid;
                                    });
                                    cswPublic.sizes = reducedSizes;
                                }
                            });
                        };
                        div.br();

                        /* Size Select (hidden if only 1 NodeType present) - to get size node type */
                        selectDiv = div.div();
                        cswPrivate.sizeSelect = selectDiv.nodeTypeSelect({
                            ID: cswPrivate.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            value: cswPrivate.state.sizeNodeTypeId,
                            labelText: 'Select a Material Size: ',
                            objectClassName: 'SizeClass',
                            onSelect: sizeSelect,
                            onSuccess: function (retObj, count) {
                                sizeSelect(retObj, count);
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

            cswPrivate.makeStep1();

            return cswPublic;
        });
} ());