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

            var cswPublic = {};

            cswPrivate.reinitSteps = function (startWithStep) {
                cswPrivate.stepFiveComplete = false;
                if (startWithStep <= 4) {
                    cswPrivate.stepFourComplete = false;

                    if (startWithStep <= 3) {
                        cswPrivate.stepThreeComplete = false;

                        if (startWithStep <= 2) {
                            cswPrivate.stepTwoComplete = false;

                            if (startWithStep <= 1) {
                                /* This is mostly for debugging, you probably never need to reset step 1 in practice */
                                cswPrivate.stepOneComplete = false;
                            }
                        }
                    }
                }
            };

            (function () {
                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswPrivate.wizardSteps = {
                    1: 'Choose Type',
                    2: 'Identity',
                    3: 'Validate',
                    4: 'Properties',
                    5: 'Size(s)'
                };

                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.handleStep = function (newStepNo) {
                    if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                        cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                        cswPrivate.currentStepNo = newStepNo;
                        cswPrivate['makeStep' + newStepNo]();

                        if (cswPrivate.currentStepNo === 4 &&
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
                    StepCount: 5,
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

            cswPrivate.makeStep1 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(cswPrivate.materialType);
                    };
                    function typeSelect() {
                        cswPrivate.materialType = { name: cswPrivate.materialTypeSelect.find(':selected').text(), val: cswPrivate.materialTypeSelect.val() };
                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                        cswPrivate.reinitSteps(2);
                    }
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.br({ number: 2 });

                        cswPrivate.materialTypeSelect = cswPrivate.divStep1.nodeTypeSelect({
                            ID: cswPrivate.wizard.makeStepId('nodeTypeSelect'),
                            useWide: true,
                            labelText: 'Select a Material Type: ',
                            objectClassName: 'MaterialClass',
                            onSelect: typeSelect,
                            onSuccess: typeSelect
                        });

                        cswPrivate.stepOneComplete = true;
                    }
                };
            } ());

            //Step 2: 
            cswPrivate.makeStep2 = (function () {

                return function () {
                    var nextBtnEnabled = function () {
                        return false === Csw.isNullOrEmpty(cswPrivate.tradeName) && false === Csw.isNullOrEmpty(cswPrivate.supplier.val);
                    };
                    function supplierSelect() {
                        cswPrivate.supplier = { name: cswPrivate.supplierSelect.find(':selected').text(), val: cswPrivate.supplierSelect.val() };
                        cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                        cswPrivate.reinitSteps(3);
                    }

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();

                        cswPrivate.divStep2.br({ number: 2 });

                        /* TRADENAME */
                        cswPrivate.tradeNameInput = cswPrivate.divStep2.input({
                            ID: cswPrivate.wizard.makeStepId('tradename'),
                            useWide: true,
                            labelText: 'Tradename: ',
                            cssclass: 'required',
                            onChange: function () {
                                cswPrivate.tradeName = cswPrivate.tradeNameInput.val();
                                cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                                cswPrivate.reinitSteps(3);
                            }
                        });
                        cswPrivate.divStep2.br({ number: 1 });

                        /* SUPPLIER */
                        cswPrivate.supplierSelect = cswPrivate.divStep2.nodeSelect({
                            ID: cswPrivate.wizard.makeStepId('supplier'),
                            cssclass: 'required',
                            objectClassName: 'VendorClass',
                            useWide: true,
                            labelText: 'Supplier: ',
                            onChange: supplierSelect,
                            onSuccess: supplierSelect
                        });
                        cswPrivate.divStep2.br({ number: 1 });

                        /* PARTNO */
                        cswPrivate.partNoInput = cswPrivate.divStep2.input({
                            ID: cswPrivate.wizard.makeStepId('partno'),
                            useWide: true,
                            labelText: 'Part No: ',
                            onChange: function () {
                                cswPrivate.partNo = cswPrivate.partNoInput.val();
                                cswPrivate.reinitSteps(3);
                            }
                        });

                        cswPrivate.stepTwoComplete = true;
                    }
                };
            } ());

            cswPrivate.makeStep3 = (function () {

                return function () {
                    var div;
                    function makeConfirmation() {
                        div.p({ useWide: true, labelText: 'Tradename:', text: cswPrivate.tradeName });
                        div.p({ useWide: true, labelText: 'Supplier: ', text: cswPrivate.supplier.name });
                        if (false === Csw.isNullOrEmpty(cswPrivate.partNo)) {
                            div.p({ useWide: true, labelText: 'Part No: ', text: cswPrivate.partNo });
                        }
                    }

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();

                        cswPrivate.divStep3.br({ number: 2 });

                        div = cswPrivate.divStep3.div();


                        Csw.ajax.post({
                            urlMethod: 'getMaterial',
                            data: {
                                NodeTypeId: cswPrivate.materialType.val,
                                Tradename: cswPrivate.tradeName,
                                Supplier: cswPrivate.supplier.name,
                                PartNo: cswPrivate.partNo
                            },
                            success: function (data) {
                                var topText = '', bottomText = '';
                                cswPrivate.useExistingMaterial = (false === Csw.isNullOrEmpty(data.tradename));
                                if (cswPrivate.useExistingMaterial) {
                                    topText = 'A material named ' + data.tradename + ' already exists as: ';
                                    bottomText = 'Click next to use this existing material.';
                                    cswPrivate.tradeName = data.tradename;
                                    cswPrivate.supplier.name = data.supplier;
                                    cswPrivate.partNo = data.partno;
                                    cswPrivate.materialNodeId = data.nodeid;
                                } else {
                                    topText = 'Creating a new ' + cswPrivate.tradeName + ' material: ';
                                }
                                div.p({ text: topText });
                                makeConfirmation();
                                div.p({ text: bottomText });
                            }
                        });

                        cswPrivate.stepThreeComplete = true;
                    }
                };

            } ());

            cswPrivate.makeStep4 = (function () {
                cswPrivate.stepFourComplete = false;

                return function () {
                    var div;

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    if (false === cswPrivate.stepFourComplete &&
                        false === cswPrivate.useExistingMaterial) {
                        cswPrivate.divStep4 = cswPrivate.divStep4 || cswPrivate.wizard.div(4);
                        cswPrivate.divStep4.empty();

                        cswPrivate.divStep4.br({ number: 2 });

                        div = cswPrivate.divStep4.div();
                        cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(div, {
                            nodetypeid: cswPrivate.materialType.val,
                            showSaveButton: false,
                            EditMode: Csw.enums.editMode.Add,
                            ReloadTabOnSave: false,
                            ShowAsReport: false,
                            excludeOcProps: ['tradename', 'supplier', 'partno']
                        });

                        cswPrivate.stepFourComplete = true;
                    }
                };

            } ());

            cswPrivate.makeStep5 = (function () {
                cswPrivate.stepFiveComplete = false;

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
                            enabledText: 'Add',
                            onClick: function () {
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
                        });
                    }

                    if (false === cswPrivate.stepFiveComplete) {
                        cswPrivate.divStep5 = cswPrivate.divStep5 || cswPrivate.wizard.div(5);
                        cswPrivate.divStep5.empty();

                        cswPrivate.divStep5.br({ number: 2 });

                        div = cswPrivate.divStep5.div();
                        div.label({ text: 'Now creating sizes (not material attributes)', cssclass: 'CswLabelCreateMaterial' });

                        var makeGrid = function () {
                            /* Thin Grid of sizes */
                            cswPrivate.sizeGrid = div.thinGrid({ linkText: '', hasHeader: true, TableCssClass: 'CswThinGridTableWizard' });


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
                                cswPrivate.sizeGrid.addRows(['', 'Capacity', 'Quantity Editable', 'Dispensable']);
                                cswPrivate.sizeGrid.hide();
                            }
                            cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                            cswPrivate.toggleButton(cswPrivate.buttons.next, false);
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

                        cswPrivate.stepFiveComplete = true;
                    }
                };

            } ());

            cswPrivate.makeStep1();

            return cswPublic;
        });
} ());