/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
        Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
            'use strict';

            var internal = {
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
                divStep1: '', divStep2: '', divStep3: '', divStep4: '', divStep5: '',
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
                stepThreeComplete: false,
                stepFourComplete: false,
                stepFiveComplete: false
            };

            var external = {};

            internal.reinitSteps = function (startWithStep) {
                internal.stepFiveComplete = false;
                if (startWithStep <= 4) {
                    internal.stepFourComplete = false;
                }
                else if (startWithStep <= 3) {
                    internal.stepThreeComplete = false;
                }
                else if (startWithStep <= 2) {
                    internal.stepTwoComplete = false;
                }
                else if (startWithStep <= 1) {
                    /* This is mostly for debugging, you probably never need to reset step 1 in practice */
                    internal.stepOneComplete = false;
                }
            };

            (function () {
                if (options) {
                    $.extend(internal, options);
                }

                internal.wizardSteps = {
                    1: 'Choose Type',
                    2: 'Identity',
                    3: 'Validate',
                    4: 'Properties',
                    5: 'Size(s)'
                };

                internal.currentStepNo = internal.startingStep;

                internal.handleStep = function (newStepNo) {
                    if (Csw.contains(internal, 'makeStep' + newStepNo)) {
                        internal.lastStepNo = internal.currentStepNo;
                        internal.currentStepNo = newStepNo;
                        internal['makeStep' + newStepNo]();

                        if (internal.currentStepNo === 4 &&
                            internal.useExistingMaterial) {
                            if (internal.currentStepNo > internal.lastStepNo) {
                                internal.toggleButton(internal.buttons.next, true, true);
                            }
                            else if (internal.currentStepNo < internal.lastStepNo) {
                                internal.toggleButton(internal.buttons.prev, true, true);
                            }
                        }
                    }
                };

                internal.finalize = function () {
                    function getMaterialDefinition() {
                        var createMaterialDef = {
                            useexistingmaterial: internal.useExistingMaterial,
                            sizes: internal.sizeNodes
                        };

                        if (false === internal.useExistingMaterial) {
                            createMaterialDef.materialnodetypeid = internal.materialType.val;
                            createMaterialDef.tradename = internal.tradeName;
                            createMaterialDef.partno = internal.partNo;
                            createMaterialDef.supplierid = internal.supplier.val;
                            createMaterialDef.suppliername = internal.supplier.name;
                            if (false === Csw.isNullOrEmpty(internal.tabsAndProps)) {
                                createMaterialDef.properties = internal.tabsAndProps.getPropJson;
                            }
                        } else {
                            createMaterialDef.materialnodeid = internal.materialNodeId;
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
                            Csw.tryExec(internal.onFinish, viewid);
                        }
                    });
                };

                internal.wizard = Csw.layouts.wizard(cswParent.div(), {
                    ID: Csw.makeId(internal.ID, 'wizard'),
                    Title: 'Create Material',
                    StepCount: 5,
                    Steps: internal.wizardSteps,
                    StartingStep: internal.startingStep,
                    FinishText: 'Finish',
                    onNext: internal.handleStep,
                    onPrevious: internal.handleStep,
                    onCancel: internal.onCancel,
                    onFinish: internal.finalize,
                    doNextOnInit: false
                });

            } ());


            internal.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = internal.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    internal.wizard[button].disable();
                }
                return false;
            };

            internal.makeStep1 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(internal.materialType);
                    };
                    function typeSelect() {
                        internal.materialType = { name: internal.materialTypeSelect.find(':selected').text(), val: internal.materialTypeSelect.val() };
                        internal.toggleButton(internal.buttons.next, true);
                        internal.reinitSteps(2);
                    }
                    internal.toggleButton(internal.buttons.prev, false);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === internal.stepOneComplete) {
                        internal.divStep1 = internal.divStep1 || internal.wizard.div(1);
                        internal.divStep1.empty();
                        
                        internal.divStep1.br({ number: 2 });

                        internal.materialTypeSelect = internal.divStep1.nodeTypeSelect({
                            ID: internal.wizard.makeStepId('nodeTypeSelect'),
                            labelText: 'Select a Material Type: ',
                            objectClassName: 'MaterialClass',
                            onSelect: typeSelect,
                            onSuccess: typeSelect
                        });

                        internal.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: 
            internal.makeStep2 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(internal.tradeName) && false === Csw.isNullOrEmpty(internal.supplier);
                    };
                    function supplierSelect() {
                        internal.supplier = { name: internal.supplierSelect.find(':selected').text(), val: internal.supplierSelect.val() };
                        internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                        internal.reinitSteps(3);
                    }

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === internal.stepTwoComplete) {
                        internal.divStep2 = internal.divStep2 || internal.wizard.div(2);
                        internal.divStep2.empty();

                        internal.divStep2.br({ number: 2 });

                        /* TRADENAME */
                        internal.tradeNameInput = internal.divStep2.input({
                            ID: internal.wizard.makeStepId('tradename'),
                            labelText: 'Tradename: ',
                            onChange: function () {
                                internal.tradeName = internal.tradeNameInput.val();
                                internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                                internal.reinitSteps(3);
                            }
                        });
                        internal.divStep2.br({ number: 1 });

                        /* SUPPLIER */
                        internal.supplierSelect = internal.divStep2.nodeSelect({
                            ID: internal.wizard.makeStepId('supplier'),
                            objectClassName: 'VendorClass',
                            labelText: 'Supplier: ',
                            onChange: supplierSelect,
                            onSuccess: supplierSelect
                        });
                        internal.divStep2.br({ number: 1 });

                        /* PARTNO */
                        internal.partNoInput = internal.divStep2.input({
                            ID: internal.wizard.makeStepId('partno'),
                            labelText: 'Part No: ',
                            onChange: function () {
                                internal.partNo = internal.partNoInput.val();
                                internal.reinitSteps(3);
                            }
                        });

                        internal.stepTwoComplete = true;
                    }
                };
            } ());

            internal.makeStep3 = (function () {

                return function () {
                    var div;
                    function makeConfirmation() {
                        div.p({ labelText: 'Tradename:', text: internal.tradeName });
                        div.p({ labelText: 'Supplier: ', text: internal.supplier.name });
                        if (false === Csw.isNullOrEmpty(internal.partNo)) {
                            div.p({ labelText: 'Part No: ', text: internal.partNo });
                        }
                    }

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, true);

                    if (false === internal.stepThreeComplete) {
                        internal.divStep3 = internal.divStep3 || internal.wizard.div(3);
                        internal.divStep3.empty();

                        internal.divStep3.br({ number: 2 });

                        div = internal.divStep3.div();


                        Csw.ajax.post({
                            urlMethod: 'getMaterial',
                            data: {
                                NodeTypeId: internal.materialType.val,
                                Tradename: internal.tradeName,
                                Supplier: internal.supplier.name,
                                PartNo: internal.partNo
                            },
                            success: function (data) {
                                var topText = '', bottomText = '';
                                if (false === Csw.isNullOrEmpty(data.tradename)) {
                                    internal.useExistingMaterial = true;
                                    topText = 'A material named ' + data.tradename + ' already exists as: ';
                                    bottomText = 'Click next to use this existing material.';
                                    internal.tradeName = data.tradename;
                                    internal.supplier.name = data.supplier;
                                    internal.partNo = data.partno;
                                    internal.materialNodeId = data.nodeid;
                                } else {
                                    topText = 'Creating a new ' + internal.tradeName + ' material: ';
                                }
                                div.p({ text: topText });
                                makeConfirmation();
                                div.p({ text: bottomText });
                            }
                        });

                        internal.stepThreeComplete = true;
                    }
                };

            } ());

            internal.makeStep4 = (function () {
                internal.stepFourComplete = false;

                return function () {
                    var div;

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, true);

                    if (false === internal.stepFourComplete &&
                        false === internal.useExistingMaterial) {
                        internal.divStep4 = internal.divStep4 || internal.wizard.div(4);
                        internal.divStep4.empty();

                        internal.divStep4.br({ number: 2 });

                        div = internal.divStep4.div();
                        internal.tabsAndProps = Csw.layouts.tabsAndProps(div, {
                            nodetypeid: internal.materialType.val,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['tradename', 'supplier', 'partno']
                        });

                        internal.stepFourComplete = true;
                    }
                };

            } ());

            internal.makeStep5 = (function () {
                internal.stepFiveComplete = false;

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
                        internal.sizeNodeTypeId = internal.sizeSelect.val();
                        if (count > 1) {
                            selectDiv.show();
                        }
                        addDiv = addDiv || div.div();
                        addDiv.empty();
                        internal.addSizeNode = Csw.layouts.tabsAndProps(addDiv, {
                            nodetypeid: internal.sizeNodeTypeId,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['material']
                        });
                        internal.addSizeBtn = addDiv.button({
                            enabledText: 'Add',
                            onClick: function () {
                                var sizeData = internal.addSizeNode.getPropJson();

                                Csw.ajax.post({
                                    urlMethod: 'getSizeNodeProps',
                                    data: {
                                        SizeDefinition: JSON.stringify(sizeData),
                                        SizeNodeTypeId: internal.sizeNodeTypeId
                                    },
                                    success: function (data) {
                                        var size = data.row;
                                        if (isSizeNew(size)) {
                                            internal.sizeGrid.addRows(size);
                                            internal.sizeNodes.push({
                                                nodetypeid: internal.sizeNodeTypeId,
                                                sizedef: sizeData
                                            });
                                            sizes.push(size);
                                        } else {
                                            $.CswDialog('AlertDialog', 'This size is already defined. Please define a new, unique size.');
                                        }
                                    }
                                });
                            }
                        });
                    }

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, true);
                    internal.toggleButton(internal.buttons.next, false);

                    if (false === internal.stepFiveComplete) {
                        internal.divStep5 = internal.divStep5 || internal.wizard.div(5);
                        internal.divStep5.empty();

                        internal.divStep5.br({ number: 2 });

                        div = internal.divStep5.div();

                        /* Thin Grid of sizes */
                        internal.sizeGrid = div.thinGrid({ linkText: '', hasHeader: true });

                        /* We need the header regardless of whether the material exists
                           if (internal.useExistingMaterial) {
                        */
                            Csw.ajax.post({
                                urlMethod: 'getMaterialSizes',
                                data: { MaterialId: internal.materialNodeId },
                                success: function (data) {
                                    sizes = data.rows || [];
                                    Csw.log(sizes);
                                    internal.sizeGrid.addRows(sizes);
                                }
                            });
                        

                        div.br();

                        /* Size Select (hidden if only 1 NodeType present) */
                        selectDiv = div.div();
                        internal.sizeSelect = selectDiv.nodeTypeSelect({
                            ID: internal.wizard.makeStepId('nodeTypeSelect'),
                            labelText: 'Select a Material Size: ',
                            objectClassName: 'SizeClass',
                            onSelect: sizeSelect,
                            onSuccess: sizeSelect,
                            relatedToNodeTypeId: internal.materialType.val
                        });
                        selectDiv.hide();

                        /* Populate this with onSuccess of internal.sizeSelect */
                        internal.addSizeNode = {};

                        internal.stepFiveComplete = true;
                    }
                };

            } ());

            internal.makeStep1();

            return external;
        });
} ());