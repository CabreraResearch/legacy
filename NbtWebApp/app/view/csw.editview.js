/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.nbt.vieweditor = Csw.nbt.vieweditor ||
        Csw.nbt.register('vieweditor', function (cswParent, options) {
            'use strict';

            //#region Properties

            var cswPrivate = {
                name: 'cswViewEditor',
                exitFunc: null,
                startingStep: 1,
                wizard: null,
                wizardSteps: {
                    1: 'Choose a View',
                    2: 'Build a View',
                    3: 'Add to View',
                    4: 'Set Filters',
                    5: 'View Attributes',
                    6: 'Fine Tuning (Advanced)'
                },
                stepCount: 6,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                stepFourComplete: false,
                stepFiveComplete: false,
                stepSixComplete: false,
                selectedViewId: ''
            };

            var cswPublic = {};

            //#endregion Properties

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
                if (1 === newStepNo) {
                    cswPrivate.makeStep1();
                } else if (2 === newStepNo) {
                    cswPrivate.makeStep2();
                } else if (3 === newStepNo) {
                    cswPrivate.makeStep3();
                }
            };

            //#endregion Wizard Functions

            cswPrivate.makeStep1 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 1;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step1Div = cswPrivate.step1Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step1Div.empty();

                    cswPrivate.step1Div.span({
                        text: "A View controls the arrangement of information you see in a tree or grid. " +
                        "Views are useful for defining a user's workflow or for creating elaborate search criteria. " +
                            "This wizard will take you step by step through the process of creating a new View or " +
                                "editing an existing View.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step1Div.br({ number: 2 });

                    cswPrivate.viewsDiv = cswPrivate.step1Div.div();
                    cswPrivate.buttonsTbl = cswPrivate.step1Div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });
                    cswPrivate.copyViewBtn = cswPrivate.buttonsTbl.cell(1, 1).buttonExt({
                        name: 'vieweditor_step1_copyviewbtn',
                        enabledText: 'Copy View',
                        onClick: function () {
                            //TODO: copy view
                        }
                    });
                    cswPrivate.deleteViewBtn = cswPrivate.buttonsTbl.cell(1, 2).buttonExt({
                        name: 'vieweditor_step1_deleteviewbtn',
                        enabledText: 'Delete View',
                        onClick: function () {
                            //TODO: delete view
                        }
                    });
                    cswPrivate.buttonsTbl.cell(1, 3).buttonExt({
                        name: 'vieweditor_step1_createviewbtn',
                        enabledText: 'Create New View',
                        onClick: function () {
                            $.CswDialog('AddViewDialog', {
                                onAddView: function (newViewId, viewMode) {
                                    cswPrivate.selectedViewId = newViewId;
                                    makeViewsGrid(cswPrivate.showAllChkBox.checked());
                                }
                            });
                        }
                    });
                    cswPrivate.showAllDiv = cswPrivate.step1Div.div().css({
                        'float': 'right'
                    });
                    cswPrivate.showAllChkBox = cswPrivate.showAllDiv.input({
                        name: 'vieweditor_step1_showallchkbox',
                        type: Csw.enums.inputTypes.checkbox,
                        labelText: 'Show All Roles/Users',
                        canCheck: true,
                        onClick: function () {
                            makeViewsGrid(cswPrivate.showAllChkBox.checked());
                        }
                    });

                    var makeViewsGrid = function (all) {
                        Csw.ajax.post({
                            urlMethod: 'getViewGrid',
                            data: {
                                All: all,
                                SelectedViewId: cswPrivate.selectedViewId
                            },
                            success: function (gridData) {
                                cswPrivate.viewsDiv.empty();
                                cswPrivate.viewsDiv.grid({
                                    name: 'vieweditor_grid',
                                    storeId: 'vieweditor_store',
                                    title: '',
                                    stateId: 'vieweditor_gridstate',
                                    usePaging: false,
                                    showActionColumn: false,
                                    height: 225,
                                    fields: gridData.grid.fields,
                                    columns: gridData.grid.columns,
                                    data: gridData.grid.data,
                                    pageSize: gridData.grid.pageSize,
                                    canSelectRow: true,
                                    onSelect: function (row) {
                                        cswPrivate.selectedViewId = row.viewid;
                                        cswPrivate.deleteViewBtn.enable();
                                        cswPrivate.copyViewBtn.enable();
                                    },
                                    onDeselect: function (row) {
                                        cswPrivate.selectedViewId = '';
                                        cswPrivate.deleteViewBtn.disable();
                                        cswPrivate.copyViewBtn.disable();
                                    },
                                    onLoad: function (grid) {
                                        if (false === Csw.isNullOrEmpty(cswPrivate.selectedViewId)) {
                                            var rowid = grid.getRowIdForVal('viewid', cswPrivate.selectedViewId);
                                            grid.setSelection(rowid);
                                            grid.scrollToRow(rowid);
                                        } else {
                                            cswPrivate.deleteViewBtn.disable();
                                            cswPrivate.copyViewBtn.disable();
                                        }
                                    }
                                });
                            }
                        });
                    };
                    makeViewsGrid(false);
                };
            }());

            cswPrivate.makeStep2 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 2;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step2Div = cswPrivate.step2Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step2Div.empty();

                    cswPrivate.step2Div.span({
                        text: "What do you want in your Grid?",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step2Div.br({ number: 3 });

                    cswPrivate.step2Tbl = cswPrivate.step2Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    cswPrivate.propsScrollable = cswPrivate.step2Tbl.cell(1, 1).div().css({
                        'overflow': 'auto',
                        'width': '390px'
                    });
                    cswPrivate.propsDiv = cswPrivate.propsScrollable.div().css({
                        height: '270px'
                    });
                    cswPrivate.previewDiv = cswPrivate.step2Tbl.cell(1, 2).div().css({
                        'padding-left': '50px'
                    });

                    var getStep2Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleStep',
                            data: {
                                ViewId: cswPrivate.selectedViewId,
                                StepNo: cswPrivate.currentStepNo
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;

                                cswPrivate.propsDiv.empty();

                                cswPrivate.selectTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 5
                                });
                                cswPrivate.relSelect = cswPrivate.selectTbl.cell(1, 1).select({
                                    name: 'vieweditor_step2_relationshipselect'
                                });
                                cswPrivate.selectTbl.cell(1, 2).buttonExt({
                                    enabledText: 'Add Relationship',
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.plus),
                                    disableOnClick: false,
                                    onClick: function () {
                                        var selected = cswPrivate.relSelect.selectedVal();
                                        cswPrivate.relationships[selected].Checked = true;
                                        cswPrivate.View.Root.ChildRelationships.push(cswPrivate.relationships[selected].Relationship);
                                        cswPrivate.makeCells();
                                        cswPrivate.relSelect.removeOption(cswPrivate.relationships[selected].Relationship.ArbitraryId);
                                        cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                    }
                                });
                                cswPrivate.propsTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 5,
                                    cellpadding: 5
                                });
                                cswPrivate.relationships = {};
                                var selectOpts = [];
                                Csw.each(response.Step2.Relationships, function (ViewRel) {
                                    var newOpt = {
                                        display: ViewRel.Relationship.TextLabel,
                                        value: ViewRel.Relationship.ArbitraryId
                                    };
                                    selectOpts.push(newOpt);
                                    cswPrivate.relationships[ViewRel.Relationship.ArbitraryId] = ViewRel;
                                });
                                cswPrivate.relSelect.setOptions(selectOpts, true);
                                cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

                                cswPrivate.makeCells = function () {
                                    var row = 1;
                                    cswPrivate.propsTbl.empty();
                                    Csw.iterate(cswPrivate.relationships, function (thisRel) {
                                        if (thisRel.Checked) {
                                            cswPrivate.propsTbl.cell(row, 1).icon({
                                                hovertext: 'Remove this from view',
                                                isButton: true,
                                                iconType: Csw.enums.iconType.x,
                                                onClick: function () {
                                                    cswPrivate.relationships[thisRel.Relationship.ArbitraryId].Checked = false;
                                                    var cleansedRelationships = [];
                                                    Csw.iterate(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                                                        if (childRel.ArbitraryId !== thisRel.Relationship.ArbitraryId) {
                                                            cleansedRelationships.push(childRel);
                                                        }
                                                    });
                                                    cswPrivate.View.Root.ChildRelationships = cleansedRelationships;
                                                    cswPrivate.makeCells();
                                                    cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                                    var newOpt = {
                                                        display: thisRel.Relationship.TextLabel,
                                                        value: thisRel.Relationship.ArbitraryId
                                                    };
                                                    cswPrivate.relSelect.addOption(newOpt, false);
                                                }
                                            });
                                            cswPrivate.propsTbl.cell(row, 2).text(thisRel.Relationship.TextLabel);
                                            row++;
                                        }
                                    });
                                };
                                cswPrivate.makeCells();

                            }
                        });
                    };
                    getStep2Data();
                };
            }());

            cswPrivate.makeStep3 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 3;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step3Div = cswPrivate.step3Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step3Div.empty();

                    cswPrivate.step3Div.span({
                        text: "What columns do you want in your Grid?",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step3Div.br({ number: 3 });

                    cswPrivate.step3Tbl = cswPrivate.step3Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    cswPrivate.propsScrollable = cswPrivate.step3Tbl.cell(1, 1).div().css({
                        'overflow': 'auto',
                        'width': '320px'
                    });
                    cswPrivate.propsDiv = cswPrivate.propsScrollable.div().css({
                        height: '270px'
                    });
                    cswPrivate.previewDiv = cswPrivate.step3Tbl.cell(1, 2).div().css({
                        'padding-left': '50px'
                    });

                    var getStep3Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleStep',
                            data: {
                                CurrentView: cswPrivate.View,
                                StepNo: cswPrivate.currentStepNo
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;

                                cswPrivate.propsDiv.empty();
                                cswPrivate.propSelect = cswPrivate.propsDiv.select({
                                    name: 'vieweditor_step3_propselect',
                                    onChange: function () {
                                        var selectedProp = cswPrivate.properties[cswPrivate.propSelect.selectedVal()];
                                        selectedProp.Checked = true;
                                        cswPrivate.makePropsTbl();
                                        Csw.iterate(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                                            if (selectedProp.Property.ParentArbitraryId === childRel.ArbitraryId) {
                                                childRel.Properties.push(selectedProp.Property);
                                            }
                                        });
                                        cswPrivate.propSelect.removeOption(selectedProp.Property.ArbitraryId);
                                        cswPrivate.propSelect.removeOption('');
                                        cswPrivate.propSelect.addOption({ value: '', display: '' }, true);
                                        cswPrivate.makePropsTbl();
                                        cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                    }
                                });
                                cswPrivate.propsDiv.br({ number: 2 });
                                cswPrivate.propsTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 3,
                                    cellpadding: 3
                                });

                                cswPrivate.properties = {};
                                cswPrivate.selectOpts = [];
                                Csw.each(response.Step3.Properties, function (ViewProp) {
                                    cswPrivate.properties[ViewProp.Property.ArbitraryId] = ViewProp;
                                    var newOpt = {
                                        value: ViewProp.Property.ArbitraryId,
                                        display: ViewProp.Property.TextLabel
                                    };
                                    cswPrivate.selectOpts.push(newOpt);
                                });
                                cswPrivate.selectOpts.push({ value: '', display: '', isSelected: true });
                                cswPrivate.propSelect.setOptions(cswPrivate.selectOpts, true);

                                cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

                                cswPrivate.makePropsTbl = function () {
                                    var row = 1;
                                    cswPrivate.propsTbl.empty();
                                    Csw.iterate(cswPrivate.properties, function (prop) {
                                        if (prop.Checked) {
                                            cswPrivate.propsTbl.cell(row, 1).icon({
                                                hovertext: 'Remove this from view',
                                                isButton: true,
                                                iconType: Csw.enums.iconType.x,
                                                onClick: function () {
                                                    var newProps = [];
                                                    Csw.iterate(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                                                        if (childRel.ArbitraryId === prop.Property.ParentArbitraryId) {
                                                            Csw.iterate(childRel.Properties, function (childProp) {
                                                                if (childProp.ArbitraryId !== prop.Property.ArbitraryId) {
                                                                    newProps.push(childProp);
                                                                }
                                                            });
                                                            childRel.Properties = newProps;
                                                        }
                                                    });
                                                    var newOpt = {
                                                        value: prop.Property.ArbitraryId,
                                                        display: prop.Property.TextLabel
                                                    };
                                                    cswPrivate.propSelect.addOption(newOpt, false);
                                                    prop.Checked = false;
                                                    cswPrivate.makePropsTbl();
                                                    cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                                }
                                            });
                                            cswPrivate.propsTbl.cell(row, 2).text(prop.Property.TextLabel);
                                            var thisOrderInput = cswPrivate.propsTbl.cell(row, 3).input({ //TODO: this MUST be a number or blank
                                                name: 'vieweditor_step3_orderinput_' + prop.Property.ArbitraryId,
                                                size: 3,
                                                onChange: function () {
                                                    //TODO: update props order, update preview
                                                }
                                            });
                                            row++;
                                        }
                                    });
                                };
                                cswPrivate.makePropsTbl();
                            }
                        });
                    };
                    getStep3Data();
                };
            }());

            cswPrivate.buildPreview = function (previewDiv, view) {
                Csw.ajaxWcf.post({
                    urlMethod: 'ViewEditor/GetPreview',
                    data: {
                        CurrentView: view
                    },
                    success: function (response) {
                        previewDiv.empty();
                        var gridData = JSON.parse(response.Preview);
                        previewDiv.grid({
                            name: 'vieweditor_previewgrid',
                            storeId: 'vieweditor_store',
                            title: '',
                            stateId: 'vieweditor_gridstate',
                            usePaging: false,
                            showActionColumn: false,
                            height: 230,
                            width: 700,
                            fields: gridData.grid.fields,
                            columns: gridData.grid.columns,
                            data: gridData.grid.data,
                            pageSize: gridData.grid.pageSize,
                            canSelectRow: false,
                        });
                    }
                });
            };

            //#region ctor

            (function () {
                Csw.extend(cswPrivate, options, true);
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.finalize = function () {
                    //TODO: save view
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'View Editor',
                    StepCount: cswPrivate.stepCount,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                    },
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

                cswPrivate.makeStep1();

            }());

            //#endregion ctor



            return cswPublic;
        });
}());