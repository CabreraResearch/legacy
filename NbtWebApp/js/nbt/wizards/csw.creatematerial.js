/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
        Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
            'use strict';

            var cswPrivateVar = {
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

            var cswPublicRet = {};

            cswPrivateVar.reinitSteps = function (startWithStep) {
                cswPrivateVar.stepFiveComplete = false;
                if (startWithStep <= 4) {
                    cswPrivateVar.stepFourComplete = false;

                    if (startWithStep <= 3) {
                        cswPrivateVar.stepThreeComplete = false;

                        if (startWithStep <= 2) {
                            cswPrivateVar.stepTwoComplete = false;

                            if (startWithStep <= 1) {
                                /* This is mostly for debugging, you probably never need to reset step 1 in practice */
                                cswPrivateVar.stepOneComplete = false;
                            }
                        }
                    }
                }
            };

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.wizardSteps = {
                    1: 'Choose Type',
                    2: 'Identity',
                    3: 'Validate',
                    4: 'Properties',
                    5: 'Size(s)'
                };

                cswPrivateVar.currentStepNo = cswPrivateVar.startingStep;

                cswPrivateVar.handleStep = function (newStepNo) {
                    if (Csw.contains(cswPrivateVar, 'makeStep' + newStepNo)) {
                        cswPrivateVar.lastStepNo = cswPrivateVar.currentStepNo;
                        cswPrivateVar.currentStepNo = newStepNo;
                        cswPrivateVar['makeStep' + newStepNo]();

                        if (cswPrivateVar.currentStepNo === 4 &&
                            cswPrivateVar.useExistingMaterial) {
                            if (cswPrivateVar.currentStepNo > cswPrivateVar.lastStepNo) {
                                cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true, true);
                            }
                            else if (cswPrivateVar.currentStepNo < cswPrivateVar.lastStepNo) {
                                cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true, true);
                            }
                        }
                    }
                };

                cswPrivateVar.finalize = function () {
                    function getMaterialDefinition() {
                        var createMaterialDef = {
                            useexistingmaterial: cswPrivateVar.useExistingMaterial,
                            sizes: cswPrivateVar.sizeNodes
                        };

                        if (false === cswPrivateVar.useExistingMaterial) {
                            createMaterialDef.materialnodetypeid = cswPrivateVar.materialType.val;
                            createMaterialDef.tradename = cswPrivateVar.tradeName;
                            createMaterialDef.partno = cswPrivateVar.partNo;
                            createMaterialDef.supplierid = cswPrivateVar.supplier.val;
                            createMaterialDef.suppliername = cswPrivateVar.supplier.name;
                            if (false === Csw.isNullOrEmpty(cswPrivateVar.tabsAndProps)) {
                                createMaterialDef.properties = cswPrivateVar.tabsAndProps.getPropJson();
                            }
                        } else {
                            createMaterialDef.materialnodeid = cswPrivateVar.materialNodeId;
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
                            Csw.tryExec(cswPrivateVar.onFinish, viewid);
                        }
                    });
                };

                cswPrivateVar.wizard = Csw.layouts.wizard(cswParent.div(), {
                    ID: Csw.makeId(cswPrivateVar.ID, 'wizard'),
                    Title: 'Create Material',
                    StepCount: 5,
                    Steps: cswPrivateVar.wizardSteps,
                    StartingStep: cswPrivateVar.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivateVar.handleStep,
                    onPrevious: cswPrivateVar.handleStep,
                    onCancel: cswPrivateVar.onCancel,
                    onFinish: cswPrivateVar.finalize,
                    doNextOnInit: false
                });

            } ());


            cswPrivateVar.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = cswPrivateVar.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    cswPrivateVar.wizard[button].disable();
                }
                return false;
            };

            cswPrivateVar.makeStep1 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(cswPrivateVar.materialType);
                    };
                    function typeSelect() {
                        cswPrivateVar.materialType = { name: cswPrivateVar.materialTypeSelect.find(':selected').text(), val: cswPrivateVar.materialTypeSelect.val() };
                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true);
                        cswPrivateVar.reinitSteps(2);
                    }
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());

                    if (false === cswPrivateVar.stepOneComplete) {
                        cswPrivateVar.divStep1 = cswPrivateVar.divStep1 || cswPrivateVar.wizard.div(1);
                        cswPrivateVar.divStep1.empty();

                        cswPrivateVar.divStep1.br({ number: 2 });

                        cswPrivateVar.materialTypeSelect = cswPrivateVar.divStep1.nodeTypeSelect({
                            ID: cswPrivateVar.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            labelText: 'Select a Material Type: ',
                            objectClassName: 'MaterialClass',
                            onSelect: typeSelect,
                            onSuccess: typeSelect
                        });

                        cswPrivateVar.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: 
            cswPrivateVar.makeStep2 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(cswPrivateVar.tradeName) && false === Csw.isNullOrEmpty(cswPrivateVar.supplier.val);
                    };
                    function supplierSelect() {
                        cswPrivateVar.supplier = { name: cswPrivateVar.supplierSelect.find(':selected').text(), val: cswPrivateVar.supplierSelect.val() };
                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());
                        cswPrivateVar.reinitSteps(3);
                    }

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());

                    if (false === cswPrivateVar.stepTwoComplete) {
                        cswPrivateVar.divStep2 = cswPrivateVar.divStep2 || cswPrivateVar.wizard.div(2);
                        cswPrivateVar.divStep2.empty();

                        cswPrivateVar.divStep2.br({ number: 2 });

                        /* TRADENAME */
                        cswPrivateVar.tradeNameInput = cswPrivateVar.divStep2.input({
                            ID: cswPrivateVar.wizard.makeStepId('tradename'),
                            useWide: true,
                            labelText: 'Tradename: ',
                            cssclass: 'required',
                            onChange: function () {
                                cswPrivateVar.tradeName = cswPrivateVar.tradeNameInput.val();
                                cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());
                                cswPrivateVar.reinitSteps(3);
                            }
                        });
                        cswPrivateVar.divStep2.br({ number: 1 });

                        /* SUPPLIER */
                        cswPrivateVar.supplierSelect = cswPrivateVar.divStep2.nodeSelect({
                            ID: cswPrivateVar.wizard.makeStepId('supplier'),
                            cssclass: 'required',
                            objectClassName: 'VendorClass',
                            useWide: true,
                            labelText: 'Supplier: ',
                            onChange: supplierSelect,
                            onSuccess: supplierSelect
                        });
                        cswPrivateVar.divStep2.br({ number: 1 });

                        /* PARTNO */
                        cswPrivateVar.partNoInput = cswPrivateVar.divStep2.input({
                            ID: cswPrivateVar.wizard.makeStepId('partno'),
                            useWide: true,
                            labelText: 'Part No: ',
                            onChange: function () {
                                cswPrivateVar.partNo = cswPrivateVar.partNoInput.val();
                                cswPrivateVar.reinitSteps(3);
                            }
                        });

                        cswPrivateVar.stepTwoComplete = true;
                    }
                };
            } ());

            cswPrivateVar.makeStep3 = (function () {

                return function () {
                    var div;
                    function makeConfirmation() {
                        div.p({ useWide: true, labelText: 'Tradename:', text: cswPrivateVar.tradeName });
                        div.p({ useWide: true, labelText: 'Supplier: ', text: cswPrivateVar.supplier.name });
                        if (false === Csw.isNullOrEmpty(cswPrivateVar.partNo)) {
                            div.p({ useWide: true, labelText: 'Part No: ', text: cswPrivateVar.partNo });
                        }
                    }

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true);

                    if (false === cswPrivateVar.stepThreeComplete) {
                        cswPrivateVar.divStep3 = cswPrivateVar.divStep3 || cswPrivateVar.wizard.div(3);
                        cswPrivateVar.divStep3.empty();

                        cswPrivateVar.divStep3.br({ number: 2 });

                        div = cswPrivateVar.divStep3.div();


                        Csw.ajax.post({
                            urlMethod: 'getMaterial',
                            data: {
                                NodeTypeId: cswPrivateVar.materialType.val,
                                Tradename: cswPrivateVar.tradeName,
                                Supplier: cswPrivateVar.supplier.name,
                                PartNo: cswPrivateVar.partNo
                            },
                            success: function (data) {
                                var topText = '', bottomText = '';
                                cswPrivateVar.useExistingMaterial = (false === Csw.isNullOrEmpty(data.tradename));
                                if (cswPrivateVar.useExistingMaterial) {
                                    topText = 'A material named ' + data.tradename + ' already exists as: ';
                                    bottomText = 'Click next to use this existing material.';
                                    cswPrivateVar.tradeName = data.tradename;
                                    cswPrivateVar.supplier.name = data.supplier;
                                    cswPrivateVar.partNo = data.partno;
                                    cswPrivateVar.materialNodeId = data.nodeid;
                                } else {
                                    topText = 'Creating a new ' + cswPrivateVar.tradeName + ' material: ';
                                }
                                div.p({ text: topText });
                                makeConfirmation();
                                div.p({ text: bottomText });
                            }
                        });

                        cswPrivateVar.stepThreeComplete = true;
                    }
                };

            } ());

            cswPrivateVar.makeStep4 = (function () {
                cswPrivateVar.stepFourComplete = false;

                return function () {
                    var div;

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true);

                    if (false === cswPrivateVar.stepFourComplete &&
                        false === cswPrivateVar.useExistingMaterial) {
                        cswPrivateVar.divStep4 = cswPrivateVar.divStep4 || cswPrivateVar.wizard.div(4);
                        cswPrivateVar.divStep4.empty();

                        cswPrivateVar.divStep4.br({ number: 2 });

                        div = cswPrivateVar.divStep4.div();
                        cswPrivateVar.tabsAndProps = Csw.layouts.tabsAndProps(div, {
                            nodetypeid: cswPrivateVar.materialType.val,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['tradename', 'supplier', 'partno']
                        });

                        cswPrivateVar.stepFourComplete = true;
                    }
                };

            } ());

            cswPrivateVar.makeStep5 = (function () {
                cswPrivateVar.stepFiveComplete = false;

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
                        cswPrivateVar.sizeNodeTypeId = cswPrivateVar.sizeSelect.val();
                        if (count > 1) {
                            selectDiv.show();
                        }
                        addDiv = addDiv || div.div();
                        addDiv.empty();
                        cswPrivateVar.addSizeNode = Csw.layouts.tabsAndProps(addDiv, {
                            nodetypeid: cswPrivateVar.sizeNodeTypeId,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['material']
                        });
                        cswPrivateVar.addSizeBtn = addDiv.button({
                            enabledText: 'Add',
                            onClick: function () {
                                var sizeData = cswPrivateVar.addSizeNode.getPropJson();

                                Csw.ajax.post({
                                    urlMethod: 'getSizeNodeProps',
                                    data: {
                                        SizeDefinition: JSON.stringify(sizeData),
                                        SizeNodeTypeId: cswPrivateVar.sizeNodeTypeId
                                    },
                                    success: function (data) {
                                        var size = data.row;
                                        if (isSizeNew(size)) {
                                            cswPrivateVar.sizeGrid.addRows(size);
                                            cswPrivateVar.sizeNodes.push({
                                                nodetypeid: cswPrivateVar.sizeNodeTypeId,
                                                sizedef: Csw.clone(sizeData) 
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

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, false);

                    if (false === cswPrivateVar.stepFiveComplete) {
                        cswPrivateVar.divStep5 = cswPrivateVar.divStep5 || cswPrivateVar.wizard.div(5);
                        cswPrivateVar.divStep5.empty();

                        cswPrivateVar.divStep5.br({ number: 2 });

                        div = cswPrivateVar.divStep5.div();

                        /* Thin Grid of sizes */
                        cswPrivateVar.sizeGrid = div.thinGrid({ linkText: '', hasHeader: true });


                        if (cswPrivateVar.useExistingMaterial) {

                            Csw.ajax.post({
                                urlMethod: 'getMaterialSizes',
                                data: { MaterialId: cswPrivateVar.materialNodeId },
                                success: function (data) {
                                    sizes = data.rows || [];

                                    cswPrivateVar.sizeGrid.addRows(sizes);
                                }
                            });
                        } else {
                            cswPrivateVar.sizeGrid.addRows(['', 'Capacity', 'Quantity Editable', 'Dispensable']);
                        }

                        div.br();

                        /* Size Select (hidden if only 1 NodeType present) */
                        selectDiv = div.div();
                        cswPrivateVar.sizeSelect = selectDiv.nodeTypeSelect({
                            ID: cswPrivateVar.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            labelText: 'Select a Material Size: ',
                            objectClassName: 'SizeClass',
                            onSelect: sizeSelect,
                            onSuccess: sizeSelect,
                            relatedToNodeTypeId: cswPrivateVar.materialType.val,
                            relatedObjectClassPropName: 'Material'
                        });
                        selectDiv.hide();

                        /* Populate this with onSuccess of cswPrivateVar.sizeSelect */
                        cswPrivateVar.addSizeNode = {};

                        cswPrivateVar.stepFiveComplete = true;
                    }
                };

            } ());

            cswPrivateVar.makeStep1();

            return cswPublicRet;
        });
} ());