/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

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
                materialType: { name: '', val: '' },
                tradeName: '',
                supplier: { name: '', val: '' },
                partNo: '',
                useExistingMaterial: false,
                materialProperies: {},
                sizeNodes: [],
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false
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

            (function () {
                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswPrivate.wizardSteps = {
                    1: 'Choose Type and Identity',
                    2: 'Additional Properties',
                    3: 'Size(s)'
                };

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();

                        if (cswPrivate.currentStepNo === 3 &&
                            cswPrivate.useExistingMaterial) {
                            if (cswPrivate.currentStepNo > cswPrivate.lastStepNo) {
                                cswPrivate.toggleButton(cswPrivate.buttons.next, true, true);
                            }
                            else if (cswPrivate.currentStepNo < cswPrivate.lastStepNo) {
                                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                            }
                        }
                    }
                };

                cswPrivate.finalize = function () {
                    function getMaterialDefinition() {
                        var createMaterialDef = {
                            useexistingmaterial: cswPrivate.useExistingMaterial,
                            sizes: cswPrivate.sizeNodes
                        };

                        if (false === cswPrivate.useExistingMaterial) {
                            createMaterialDef.materialnodetypeid = cswPrivate.materialType.val;
                            createMaterialDef.tradename = cswPrivate.tradeName;
                            createMaterialDef.partno = cswPrivate.partNo;
                            createMaterialDef.supplierid = cswPrivate.supplier.val;
                            createMaterialDef.suppliername = cswPrivate.supplier.name;
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
                    onCancel: cswPrivate.onCancel,
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
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(cswPrivate.tradeName) && false === Csw.isNullOrEmpty(cswPrivate.supplier.val);
                    };
                    function supplierSelect() {
                        cswPrivate.supplier = { name: cswPrivate.supplierSelect.selectedText(), val: cswPrivate.supplierSelect.val() };
                        cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                        cswPrivate.reinitSteps(2);
                    }
                    function typeSelect() {
                        cswPrivate.materialType = { name: cswPrivate.materialTypeSelect.find(':selected').text(), val: cswPrivate.materialTypeSelect.val() };
                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                        cswPrivate.reinitSteps(2);
                    }

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.br({ number: 1 });

                        cswPrivate.divStep1.label({
                            text: "Choose Type and Identity",
                            cssclass: "wizardHelpTitle"
                        });
                        cswPrivate.divStep1.br({ number: 2 });
                        cswPrivate.divStep1.label({
                            text: "This wizard will guide you through the process of creating a new material. Fill out the following identification properties. If the attributes below match an existing material, you will be given the option to edit that material.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 4 });

                        cswPrivate.materialTypeSelect = cswPrivate.divStep1.nodeTypeSelect({
                            ID: cswPrivate.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            labelText: 'Select a Material Type*: ',
                            objectClassName: 'MaterialClass',
                            onSelect: typeSelect,
                            onChange: function () {
                                typeSelect();
                                if (false === Csw.isNullOrEmpty(cswPrivate.tradeName)) {
                                    checkIfMaterialExists();
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
                            onChange: function () {
                                cswPrivate.tradeName = cswPrivate.tradeNameInput.val();
                                cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                                if (false === Csw.isNullOrEmpty(cswPrivate.tradeName)) {
                                    checkIfMaterialExists();
                                } else {
                                    removeFoundMaterialLabel();
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
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
                            labelText: 'Supplier*: ',
                            onChange: function () {
                                supplierSelect();
                                if (false === Csw.isNullOrEmpty(cswPrivate.tradeName)) {
                                    checkIfMaterialExists();
                                }
                            },
                            onSuccess: supplierSelect
                        });
                        cswPrivate.divStep1.br({ number: 1 });

                        /* PARTNO */
                        cswPrivate.partNoInput = cswPrivate.divStep1.input({
                            ID: cswPrivate.wizard.makeStepId('partno'),
                            useWide: true,
                            labelText: 'Part No: ',
                            onChange: function () {
                                cswPrivate.partNo = cswPrivate.partNoInput.val();
                                if (false === Csw.isNullOrEmpty(cswPrivate.tradeName)) {
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
                        }
                        var checkIfMaterialExists = function () {
                            Csw.ajax.post({
                                urlMethod: 'getMaterial',
                                data: {
                                    NodeTypeId: cswPrivate.materialType.val,
                                    Tradename: cswPrivate.tradeName,
                                    Supplier: cswPrivate.supplier.name,
                                    PartNo: cswPrivate.partNo
                                },
                                success: function (data) {
                                    removeFoundMaterialLabel();
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                                    if (materialExists(data)) {
                                        foundMaterialLabel = cswPrivate.divStep1.nodeLink({
                                            text: "A material with these properties already exists with a tradename of " + data.noderef,
                                            ID: "materialExistsLabel"
                                        });
                                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    }
                                }
                            });
                        }

                        var materialExists = function (data) {
                            if (Csw.isNullOrEmpty(data)) {
                                return false;
                            }
                            if (data["tradename"] == cswPrivate.tradeName &&
                                data["supplier"] == cswPrivate.supplier.name &&
                                    data["partno"] == cswPrivate.partNo &&
                                        data["nodetypeid"] == cswPrivate.materialType.val) {
                                return true;
                            }
                            return false;
                        }

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
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepTwoComplete &&
                        false === cswPrivate.useExistingMaterial) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.br({ number: 1 });

                        cswPrivate.divStep2.label({
                            text: "Physical Properties",
                            cssclass: "wizardHelpTitle"
                        });
                        cswPrivate.divStep2.br({ number: 2 });
                        cswPrivate.divStep2.label({
                            text: "Fill out the physical properties of this material.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 4 });

                        div = cswPrivate.divStep2.div();
                        cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(div, {
                            nodetypeid: cswPrivate.materialType.val,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['tradename', 'supplier', 'partno']
                        });

                        cswPrivate.stepTwoComplete = true;
                    }
                };

            } ());

            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;

                return function () {
                    var div, addDiv, selectDiv, sizes = [];

                    function isSizeNew(size) {
                        var ret = true;
                        Csw.each(sizes, function (sizeVal, sizeKey) {
                            if (Csw.string(sizeVal[1]).toLowerCase() === Csw.string(size[1]).toLowerCase() &&
                               Csw.string(sizeVal[2]).toLowerCase() === Csw.string(size[2]).toLowerCase() &&
                                   Csw.string(sizeVal[3]).toLowerCase() === Csw.string(size[3]).toLowerCase()) {
                                ret = false;
                            }
                        });
                        return ret;
                    }

                    function sizeSelect(retObj, count) {
                        cswPrivate.sizeNodeTypeId = cswPrivate.sizeSelect.val();
                        if (count > 1) {
                            selectDiv.show();
                        }
                        addDiv = addDiv || div.div();
                        addDiv.empty();

                        cswPrivate.addSizeNode = Csw.layouts.tabsAndProps(addDiv, {
                            nodetypeid: cswPrivate.sizeNodeTypeId,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['material']
                        });
                        cswPrivate.addSizeBtn = addDiv.button({
                            disableOnClick: false,
                            enabledText: 'Add',
                            onClick: function () {
                                if (cswPrivate.addSizeNode.isFormValid()) {
                                    var sizeData = cswPrivate.addSizeNode.getPropJson();

                                    Csw.ajax.post({
                                        urlMethod: 'getSizeNodeProps',
                                        data: {
                                            SizeDefinition: JSON.stringify(sizeData),
                                            SizeNodeTypeId: cswPrivate.sizeNodeTypeId
                                        },
                                        success: function (data) {
                                            var size = data.row;
                                            if (isSizeNew(size)) {
                                                cswPrivate.sizeGrid.addRows(size);
                                                cswPrivate.sizeNodes.push({
                                                    nodetypeid: cswPrivate.sizeNodeTypeId,
                                                    sizedef: Csw.clone(sizeData)
                                                });
                                                sizes.push(size);
                                                cswPrivate.sizeGrid.show();
                                            } else {
                                                $.CswDialog('AlertDialog', 'This size is already defined. Please define a new, unique size.');
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();

                        div = cswPrivate.divStep3.div();

                        div.br({ number: 1 });
                        div.label({
                            text: "Create sizes of the material",
                            cssclass: "wizardHelpTitle"
                        });
                        div.br({ number: 2 });
                        div.label({
                            text: "Sizes are used in receiving materials. This part is optional and sizes can be created elsewhere.",
                            cssclass: "wizardHelpDesc"
                        });
                        div.br({ number: 4 });

                        var makeGrid = function () {
                            /* Thin Grid of sizes */
                            cswPrivate.sizeGrid = div.thinGrid({ linkText: '', hasHeader: true, TableCssClass: 'CswThinGridTableWizard' });

                            cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                            if (cswPrivate.useExistingMaterial) {

                                Csw.ajax.post({
                                    urlMethod: 'getMaterialSizes',
                                    data: { MaterialId: cswPrivate.materialNodeId },
                                    success: function (data) {
                                        sizes = data.rows || [];

                                        cswPrivate.sizeGrid.addRows(sizes);
                                    }
                                });
                            } else {
                                cswPrivate.sizeGrid.addRows(['', 'Initial Quantity', 'Quantity Editable', 'Dispensable', 'Catalog No']);
                                cswPrivate.sizeGrid.hide();
                            }
                        }

                        div.br();

                        /* Size Select (hidden if only 1 NodeType present) */
                        selectDiv = div.div();
                        cswPrivate.sizeSelect = selectDiv.nodeTypeSelect({
                            ID: cswPrivate.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            labelText: 'Select a Material Size: ',
                            objectClassName: 'SizeClass',
                            onSelect: sizeSelect,
                            onSuccess: function (retObj, count) {
                                sizeSelect(retObj, count);
                                makeGrid();
                            },
                            relatedToNodeTypeId: cswPrivate.materialType.val,
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