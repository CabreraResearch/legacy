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
                previousStep: -1,
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
                selectedViewId: '',
                viewStack: [],

                onFinish: function () { },
                onCancel: function () { },
                onDeleteView: function () { },
            };

            var stepNames = Csw.object();
            Object.defineProperties(stepNames, {
                BuildView: { value: 'Build a View' },
                AddToView: { value: 'Add to View' },
                SetFilters: { value: 'Set Filters' },
                ViewAttributes: { value: 'View Attributes' },
                FineTuning: { value: 'Fine Tuning (Advanced)' }
            });
            Object.freeze(stepNames);
            Object.seal(stepNames);

            var renderedStep3 = false;
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
                } else if (4 === newStepNo) {
                    cswPrivate.makeStep4();
                } else if (5 === newStepNo) {
                    cswPrivate.makeStep5();
                } else if (6 === newStepNo) {
                    cswPrivate.makeStep6();
                }
            };

            //#endregion Wizard Functions

            cswPrivate.makeStep1 = (function () {
                return function () {
                    cswPrivate.View = null;
                    cswPrivate.currentStepNo = 1;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    if (false == Csw.isNullOrEmpty(cswPrivate.selectedViewId)) {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                    } else {
                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                    }

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

                    var viewsDiv = cswPrivate.step1Div.div();
                    var buttonsTbl = cswPrivate.step1Div.table({
                        cellspacing: 2,
                        cellpadding: 2
                    });
                    var copyViewBtn = buttonsTbl.cell(1, 1).buttonExt({
                        name: 'vieweditor_step1_copyviewbtn',
                        enabledText: 'Copy View',
                        onClick: function () {
                            Csw.ajax.post({
                                urlMethod: 'copyView',
                                data: {
                                    ViewId: cswPrivate.selectedViewId,
                                    CopyToViewId: ''
                                },
                                success: function (gridJson) {
                                    cswPrivate.selectedViewId = gridJson.copyviewid;
                                    makeViewsGrid(showAllChkBox.checked());
                                },
                                error: function () {
                                    copyViewBtn.enable();
                                }
                            });
                        }
                    });
                    var deleteViewBtn = buttonsTbl.cell(1, 2).buttonExt({
                        name: 'vieweditor_step1_deleteviewbtn',
                        enabledText: 'Delete View',
                        onClick: function () {
                            Csw.ajax.post({
                                urlMethod: 'deleteView',
                                data: {
                                    ViewId: cswPrivate.selectedViewId
                                },
                                success: function () {
                                    makeViewsGrid(showAllChkBox.checked());
                                    copyViewBtn.disable();
                                    deleteViewBtn.disable();
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    Csw.tryExec(cswPrivate.onDeleteView, cswPrivate.selectedViewId);
                                },
                                error: function () {
                                    deleteViewBtn.enable();
                                    copyViewBtn.enable();
                                }
                            });
                        }
                    });
                    buttonsTbl.cell(1, 3).buttonExt({
                        name: 'vieweditor_step1_createviewbtn',
                        enabledText: 'Create New View',
                        onClick: function () {
                            $.CswDialog('AddViewDialog', {
                                onAddView: function (newViewId, viewMode) {
                                    cswPrivate.selectedViewId = newViewId;
                                    makeViewsGrid(showAllChkBox.checked());
                                }
                            });
                        }
                    });
                    var showAllDiv = cswPrivate.step1Div.div().css({
                        'float': 'right'
                    });
                    var showAllChkBox = showAllDiv.input({
                        name: 'vieweditor_step1_showallchkbox',
                        type: Csw.enums.inputTypes.checkbox,
                        labelText: 'Show All Roles/Users',
                        canCheck: true,
                        onClick: function () {
                            makeViewsGrid(showAllChkBox.checked());
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
                                viewsDiv.empty();
                                viewsDiv.grid({
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
                                        deleteViewBtn.enable();
                                        copyViewBtn.enable();
                                        cswPrivate.toggleButton(cswPrivate.buttons.next, true);
                                    },
                                    onDeselect: function (row) {
                                        cswPrivate.selectedViewId = '';
                                        deleteViewBtn.disable();
                                        copyViewBtn.disable();
                                        cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    },
                                    onLoad: function (grid) {
                                        if (false === Csw.isNullOrEmpty(cswPrivate.selectedViewId)) {
                                            var rowid = grid.getRowIdForVal('viewid', cswPrivate.selectedViewId);
                                            grid.setSelection(rowid);
                                            grid.scrollToRow(rowid);
                                        } else {
                                            deleteViewBtn.disable();
                                            copyViewBtn.disable();
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
                    cswPrivate.step2Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step2Div.empty();

                    var step2Desc = cswPrivate.step2Div.span({
                        text: "",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step2Div.br({ number: 3 });

                    var step2Tbl = cswPrivate.step2Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var propsCell = step2Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    var propsScrollable = propsCell.div().css({
                        'overflow': 'auto'
                    });
                    var propsDiv = propsScrollable.div().css({
                        height: '270px'
                    });
                    var previewCell = step2Tbl.cell(1, 2).css({
                        'padding-left': '50px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    var previewDiv = previewCell.div();

                    var getStep2Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/GetStepData',
                            data: {
                                ViewId: cswPrivate.selectedViewId,
                                StepName: stepNames.BuildView,
                                CurrentView: cswPrivate.View
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;
                                step2Desc.text("What do you want in your " + cswPrivate.View.ViewMode + "?");

                                propsDiv.empty();
                                var propsTbl = propsDiv.table({
                                    cellspacing: 5,
                                    cellpadding: 5
                                });

                                var relSelect = propsDiv.select({
                                    name: 'vieweditor_step2_relationshipselect',
                                    onChange: function () {
                                        if (relSelect.selectedText() !== 'Select...') {
                                            var selected = relSelect.selectedVal();

                                            Csw.ajaxWcf.post({
                                                urlMethod: 'ViewEditor/HandleAction',
                                                data: {
                                                    CurrentView: cswPrivate.View,
                                                    StepName: stepNames.BuildView,
                                                    Action: "AddRelationship",
                                                    Relationship: cswPrivate.relationships[selected]
                                                },
                                                success: function (addRelResponse) {
                                                    cswPrivate.View = addRelResponse.CurrentView;
                                                    cswPrivate.makeStep2();
                                                }
                                            });
                                        }
                                    }
                                });

                                cswPrivate.relationships = {};
                                var selectOpts = [];
                                selectOpts.push({ value: 'Select...', display: 'Select...', selected: true });
                                Csw.each(response.Step2.Relationships, function (ViewRel) {
                                    var newOpt = {
                                        display: ViewRel.TextLabel,
                                        value: ViewRel.ArbitraryId
                                    };
                                    selectOpts.push(newOpt);
                                    cswPrivate.relationships[ViewRel.ArbitraryId] = ViewRel;
                                });
                                relSelect.setOptions(selectOpts, true);
                                cswPrivate.buildPreview(previewDiv, cswPrivate.View);

                                cswPrivate.makeCells = function () {
                                    var row = 1;
                                    propsTbl.empty();
                                    Csw.iterate(cswPrivate.getRootLevelRelationships(), function (thisRel) {
                                        propsTbl.cell(row, 1).icon({
                                            hovertext: 'Remove this from view',
                                            isButton: true,
                                            iconType: Csw.enums.iconType.x,
                                            onClick: function () {
                                                var cleansedRelationships = [];
                                                Csw.iterate(cswPrivate.getRootLevelRelationships(), function (childRel) {
                                                    if (childRel.ArbitraryId !== thisRel.ArbitraryId) {
                                                        cleansedRelationships.push(childRel);
                                                    }
                                                });
                                                var root = cswPrivate.getRootRelationship();
                                                root.ChildRelationships = cleansedRelationships;
                                                cswPrivate.makeStep2();
                                            }
                                        });
                                        propsTbl.cell(row, 2).text(thisRel.TextLabel);
                                        row++;
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
                    cswPrivate.step3Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step3Div.empty();

                    var txt = '';
                    if (cswPrivate.View.ViewMode === 'Grid') {
                        txt = 'What columns do you want in your Grid? Drag columns in the grid preview to set the display order.';
                    } else if (cswPrivate.View.ViewMode === 'Tree') {
                        txt = "What else do you want in your Tree?";
                    } else if (cswPrivate.View.ViewMode === 'Table') {
                        txt = "What properties do you want in your Table?";
                    } else {
                        txt = 'Click "Next" to continue editing your view';
                    }
                    cswPrivate.step3Div.span({
                        text: txt,
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step3Div.br({ number: 3 });

                    var step3Tbl = cswPrivate.step3Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var propsCell = step3Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    var propsScrollable = propsCell.css({
                        'overflow': 'auto'
                    });
                    var propsDiv = propsScrollable.div().css({
                        'height': '270px'
                    });
                    var previewCell = step3Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    var previewDiv = previewCell.div();

                    cswPrivate.getStep3Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/GetStepData',
                            data: {
                                CurrentView: cswPrivate.View,
                                StepName: stepNames.AddToView
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;
                                propsDiv.br({ number: 2 });

                                if ('Grid' === cswPrivate.View.ViewMode || 'Table' === cswPrivate.View.ViewMode) {
                                    propsDiv.empty();

                                    var propsTbl = propsDiv.table({
                                        cellspacing: 3,
                                        cellpadding: 3
                                    });

                                    var propSelect = propsDiv.select({
                                        name: 'vieweditor_step3_propselect',
                                        onChange: function () {
                                            if (propSelect.selectedText() !== 'Select...') {
                                                var selectedProp = cswPrivate.properties[propSelect.selectedVal()];

                                                Csw.ajaxWcf.post({
                                                    urlMethod: 'ViewEditor/HandleAction',
                                                    data: {
                                                        CurrentView: cswPrivate.View,
                                                        StepName: stepNames.AddToView,
                                                        Action: "AddProp",
                                                        Relationship: cswPrivate.secondRelationships[selectedProp.ParentArbitraryId],
                                                        Property: selectedProp
                                                    },
                                                    success: function (addPropResponse) {
                                                        cswPrivate.View = addPropResponse.CurrentView;
                                                        cswPrivate.makeStep3();
                                                    }
                                                });
                                            }
                                        }
                                    });

                                    cswPrivate.properties = {};
                                    cswPrivate.selectOpts = [];
                                    cswPrivate.selectOpts.push({ value: 'Select...', display: 'Select...', isSelected: true });
                                    Csw.each(response.Step3.Properties, function (ViewProp) {
                                        cswPrivate.properties[ViewProp.ArbitraryId] = ViewProp;
                                        var newOpt = {
                                            value: ViewProp.ArbitraryId,
                                            display: ViewProp.TextLabel
                                        };
                                        cswPrivate.selectOpts.push(newOpt);
                                    });
                                    propSelect.setOptions(cswPrivate.selectOpts, true);

                                    cswPrivate.secondRelationships = {};
                                    Csw.each(response.Step3.SecondRelationships, function (secondRel) {
                                        cswPrivate.secondRelationships[secondRel.ArbitraryId] = secondRel;
                                    });

                                    cswPrivate.buildPreview(previewDiv, cswPrivate.View, cswPrivate.onColumnReorder); //this will update the order of the props in the view);

                                    cswPrivate.makePropsTbl = function () {
                                        var row = 2;
                                        propsTbl.empty();
                                        Csw.iterate(cswPrivate.getProperties(), function (prop) {
                                            propsTbl.cell(row, 1).icon({
                                                hovertext: 'Remove this from view',
                                                isButton: true,
                                                iconType: Csw.enums.iconType.x,
                                                onClick: function () {
                                                    Csw.ajaxWcf.post({
                                                        urlMethod: 'ViewEditor/HandleAction',
                                                        data: {
                                                            CurrentView: cswPrivate.View,
                                                            StepName: stepNames.AddToView,
                                                            Action: "RemoveProp",
                                                            Relationship: cswPrivate.secondRelationships[prop.ParentArbitraryId],
                                                            Property: prop
                                                        },
                                                        success: function (removePropResponse) {
                                                            cswPrivate.View = removePropResponse.CurrentView;
                                                            cswPrivate.makeStep3();
                                                        }
                                                    });
                                                }
                                            });
                                            propsTbl.cell(row, 2).text(prop.TextLabel);
                                            row++;
                                        });
                                    };
                                    cswPrivate.makePropsTbl();
                                    renderedStep3 = true;
                                } else if ('Tree' === cswPrivate.View.ViewMode) {
                                    var makeRelsTbl = function (thisRelTbl, innerRow, selectedRel, thisSel) {
                                        var thisRow = thisRelTbl.cell(innerRow, 1).icon({
                                            hovertext: 'Remove this from view',
                                            isButton: true,
                                            iconType: Csw.enums.iconType.x,
                                            onClick: function () {
                                                var relToRemoveFrom = cswPrivate.findRelationshipByArbitraryId(selectedRel.ParentArbitraryId);
                                                var newRels = [];
                                                Csw.each(relToRemoveFrom.ChildRelationships, function (childRel) {
                                                    if (childRel.ArbitraryId !== selectedRel.ArbitraryId) {
                                                        newRels.push(childRel);
                                                    }
                                                });
                                                relToRemoveFrom.ChildRelationships = newRels;
                                                thisRow.remove();
                                                thisRowTxt.remove();
                                                thisSel.addOption({ value: selectedRel.ArbitraryId, display: selectedRel.TextLabel });
                                                innerRow--;
                                                cswPrivate.makeStep3();
                                            }
                                        });
                                        var thisRowTxt = thisRelTbl.cell(innerRow, 2).text(selectedRel.TextLabel);
                                    };

                                    propsDiv.empty();

                                    var opts = {};
                                    var rels = {};
                                    Csw.each(response.Step3.SecondRelationships, function (rel) {
                                        rels[rel.ArbitraryId] = rel;
                                        var viewContains = cswPrivate.findRelationshipByArbitraryId(rel.ArbitraryId);
                                        if (opts[rel.ParentArbitraryId]) {
                                            if (!viewContains) { //we don't care that this coerces type
                                                opts[rel.ParentArbitraryId].push({ display: rel.TextLabel, value: rel.ArbitraryId });
                                            }
                                        } else {
                                            opts[rel.ParentArbitraryId] = [{ display: 'Select...', value: 'Select...' }];
                                            if (!viewContains) { //we don't care this this coerces type
                                                opts[rel.ParentArbitraryId].push({ display: rel.TextLabel, value: rel.ArbitraryId });
                                            }
                                        }
                                    });

                                    Csw.each(cswPrivate.getAllRelationships(), function (relProp) {
                                        var relDiv = propsDiv.div();
                                        relDiv.setLabelText('Under ' + relProp.TextLabel + '&nbsp;', false, false);
                                        relDiv.br({ number: 1 });

                                        var thisRelTbl = propsDiv.div().table({
                                            cellpadding: 3,
                                            cellspacing: 2
                                        });

                                        var viewRel = cswPrivate.findRelationshipByArbitraryId(relProp.ArbitraryId);
                                        var thisSel = propsDiv.div().select({
                                            name: 'vieweditor_relselect_' + relProp.ArbitraryId,
                                            values: opts[relProp.ArbitraryId],
                                            onChange: function () {
                                                if (thisSel.selectedVal() !== 'Select...') {
                                                    var selectedRel = rels[thisSel.selectedVal()];
                                                    var relToAddTo = cswPrivate.findRelationshipByArbitraryId(relProp.ArbitraryId);
                                                    relToAddTo.ChildRelationships.push(selectedRel);
                                                    thisSel.removeOption(selectedRel.ArbitraryId);

                                                    thisSel.removeOption('Select...');
                                                    thisSel.addOption({ display: 'Select...', value: 'Select...' }, true);

                                                    cswPrivate.makeStep3();
                                                }
                                            }
                                        });

                                        var row = 1;
                                        Csw.each(viewRel.ChildRelationships, function (childRel) {
                                            makeRelsTbl(thisRelTbl, row, childRel, thisSel);
                                            row++;
                                        });
                                        propsDiv.br({ number: 2 });
                                    });

                                    cswPrivate.buildPreview(previewDiv, cswPrivate.View);
                                } else if ('List' === cswPrivate.View.ViewMode) {
                                    propsDiv.text('Create a Tree view to add relationships below the root level.').css({
                                        'font-style': 'italic',
                                        'margin-right': '15px'
                                    });
                                    cswPrivate.buildPreview(previewDiv, cswPrivate.View);
                                }
                            }
                        });
                    };
                    cswPrivate.getStep3Data();
                };
            }());

            cswPrivate.makeStep4 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 4;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.step4Div = cswPrivate.step4Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step4Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step4Div.empty();

                    cswPrivate.step4Div.span({
                        text: "How do you want to filter your results?",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step4Div.br({ number: 3 });

                    var step4Tbl = cswPrivate.step4Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var propsCell = step4Tbl.cell(1, 1).css({
                        'width': '40%'
                    });

                    propsCell.br({ number: 2 });
                    var propsScrollable = propsCell.div().css({
                        'overflow': 'auto'
                    });
                    var filtersDiv = propsScrollable.div().css({
                        height: '230px'
                    });
                    var filtersTbl = filtersDiv.table({
                        cellpadding: 4,
                        cellspacing: 4
                    });
                    var previewCell = step4Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    var previewDiv = previewCell.div();

                    cswPrivate.relationships = {};
                    var getStep4Data = function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/GetStepData',
                            data: {
                                CurrentView: cswPrivate.View,
                                StepName: stepNames.SetFilters
                            },
                            success: function (response) {
                                handleStep4Data(response);
                            }
                        });
                    };

                    var handleStep4Data = function (response) {
                        if (cswPrivate.filterSelectDiv) {
                            cswPrivate.filterSelectDiv.remove();
                        }
                        var selectOpts = [];
                        cswPrivate.View = response.CurrentView;
                        cswPrivate.ViewJson = response.Step4.ViewJson;

                        var row = 1;
                        filtersTbl.empty();
                        Csw.iterate(response.Step4.Filters, function (filter) {
                            filtersTbl.cell(row, 1).icon({
                                hovertext: 'Remove filter',
                                isButton: true,
                                iconType: Csw.enums.iconType.x,
                                onClick: function () {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/HandleAction',
                                        data: {
                                            FilterToRemove: filter,
                                            CurrentView: cswPrivate.View,
                                            StepName: stepNames.SetFilters,
                                            Action: "RemoveFilter"
                                        },
                                        success: function (removeFilterResponse) {
                                            handleStep4Data(removeFilterResponse);
                                        }
                                    });
                                }
                            });
                            Csw.nbt.viewPropFilter({
                                name: 'vieweditor_filter_' + filter.ArbitraryId,
                                parent: filtersTbl,
                                viewId: cswPrivate.View.ViewId,
                                viewJson: response.Step4.ViewJson,
                                proparbitraryid: filter.ParentArbitraryId,
                                propname: filter.PropName,
                                selectedConjunction: filter.Conjunction,
                                selectedSubFieldName: filter.SubfieldName,
                                selectedFilterMode: filter.FilterMode,
                                selectedValue: filter.Value,
                                doStringify: false,
                                readOnly: true,
                                propRow: row,
                                firstColumn: 2
                            });
                            row++;
                        });

                        cswPrivate.filterSelectDiv = filtersDiv.div();
                        var filterSelect = cswPrivate.filterSelectDiv.select({
                            name: 'vieweditor_filter_relSelect',
                            onChange: function () {
                                if (cswPrivate.propSelect) {
                                    cswPrivate.propSelect.remove();
                                    if (cswPrivate.propFilterTbl) {
                                        cswPrivate.propFilterTbl.remove();
                                        cswPrivate.addFilterBtn.remove();
                                    }
                                }
                                if (filterSelect.selectedText() !== 'Add Filter On...') {
                                    cswPrivate.propSelect = cswPrivate.filterSelectDiv.select({
                                        name: 'vieweditor_propfilter_select',
                                        onChange: function () {
                                            if (cswPrivate.propFilterTbl) {
                                                cswPrivate.propFilterTbl.remove();
                                                cswPrivate.addFilterBtn.remove();
                                            }
                                            if (cswPrivate.propSelect.selectedText() !== 'Select...') {
                                                cswPrivate.propFilterTbl = cswPrivate.filterSelectDiv.table();
                                                var selectedProp = properties[cswPrivate.propSelect.selectedVal()];

                                                var currentFilter = Csw.nbt.viewPropFilter({
                                                    name: 'vieweditor_filter_' + selectedProp.ArbitraryId,
                                                    parent: cswPrivate.propFilterTbl,
                                                    viewJson: cswPrivate.viewJson,
                                                    proparbitraryid: selectedProp.ArbitraryId,
                                                    propname: selectedProp.PropName,
                                                    showPropertyName: false,
                                                    showOwnerName: false,
                                                    doStringify: false
                                                });

                                                cswPrivate.addFilterBtn = cswPrivate.filterSelectDiv.buttonExt({
                                                    name: 'vieweditor_applyfilter_btn',
                                                    enabledText: 'Apply Filter',
                                                    onClick: function () {
                                                        var filterData = currentFilter.getFilterJson();
                                                        var ajaxData = {
                                                            CurrentView: cswPrivate.View,
                                                            Property: selectedProp,
                                                            PropArbId: filterData.proparbitraryid,
                                                            FilterSubfield: filterData.subfieldname,
                                                            FilterValue: filterData.filtervalue,
                                                            FilterMode: filterData.filter,
                                                            FilterConjunction: filterData.conjunction,
                                                            StepName: stepNames.SetFilters,
                                                            Action: "AddFilter"
                                                        };
                                                        Csw.ajaxWcf.post({
                                                            urlMethod: 'ViewEditor/HandleAction',
                                                            data: ajaxData,
                                                            success: function (addFilterResponse) {
                                                                handleStep4Data(addFilterResponse);
                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        }
                                    });

                                    var propOpts = [];
                                    var properties = {};
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/HandleAction',
                                        data: {
                                            Relationship: cswPrivate.relationships[filterSelect.selectedVal()],
                                            CurrentView: cswPrivate.View,
                                            StepName: stepNames.SetFilters,
                                            Action: "GetFilterProps"
                                        },
                                        success: function (filterPropsresponse) {
                                            cswPrivate.viewJson = filterPropsresponse.Step4.ViewJson;
                                            Csw.iterate(filterPropsresponse.Step4.Properties, function (prop) {
                                                properties[prop.ArbitraryId] = prop;
                                                var newOpt = {
                                                    value: prop.ArbitraryId,
                                                    display: prop.TextLabel
                                                };
                                                propOpts.push(newOpt);
                                            });
                                            cswPrivate.propSelect.setOptions(propOpts, true);
                                            cswPrivate.propSelect.addOption({ display: 'Select...', value: 'Select...' }, true);
                                        }
                                    });
                                }
                            }
                        });

                        selectOpts.push({ display: 'Add Filter On...', value: 'Add Filter On...', isSelected: true });
                        Csw.iterate(response.Step4.Relationships, function (relationship) {
                            cswPrivate.relationships[relationship.ArbitraryId] = relationship;
                            var newOpt = {
                                value: relationship.ArbitraryId,
                                display: relationship.SecondName
                            };
                            selectOpts.push(newOpt);
                        });
                        filterSelect.setOptions(selectOpts, false);

                        cswPrivate.buildPreview(previewDiv, cswPrivate.View);
                    };

                    getStep4Data();

                };
            }());

            cswPrivate.makeStep5 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 5;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, true);

                    cswPrivate.viewStack = []; //clear the view stack from step 6
                    cswPrivate.step5Div = cswPrivate.step5Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step5Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step5Div.empty();

                    cswPrivate.step5Div.span({
                        text: "Set who can access this view and where it can be found in the View Selector.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step5Div.br({ number: 3 });

                    var step5Tbl = cswPrivate.step5Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var attrCell = step5Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    var attributesTbl = attrCell.div().table({
                        cellpadding: 5,
                        cellspacing: 5
                    });

                    var nameCell = attributesTbl.cell(1, 1).setLabelText('View Name', false, false);
                    var viewNameInput = attributesTbl.cell(1, 2).input({
                        name: 'vieweditor_viewname_input',
                        value: cswPrivate.View.ViewName,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    var catCell = attributesTbl.cell(2, 1).setLabelText('Category', false, false);
                    var categoryInput = attributesTbl.cell(2, 2).input({
                        name: 'vieweditor_category_input',
                        value: cswPrivate.View.Category,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    var visibilityTbl = attributesTbl.cell(3, 2).div().table();
                    var visCell = attributesTbl.cell(3, 1).setLabelText('View Visibility', false, false);
                    var visibilitySelect = Csw.composites.makeViewVisibilitySelect(visibilityTbl, 1, '', {
                        visibility: cswPrivate.View.Visibility,
                        roleid: cswPrivate.View.VisibilityRoleId,
                        userid: cswPrivate.View.VisibilityUserId,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    attributesTbl.cell(4, 1).setLabelText('Display Mode', false, false);
                    attributesTbl.cell(4, 2).text(cswPrivate.View.ViewMode);

                    var widthCell = attributesTbl.cell(5, 1).setLabelText('Width', false, false);
                    var widthInput = attributesTbl.cell(5, 2).numberTextBox({
                        name: 'vieweditor_width_input',
                        value: cswPrivate.View.Width,
                        MaxValue: 1000,
                        MinValue: 100,
                        onChange: function () {
                            handleAttributeChange();
                        }
                    });

                    if ("Property" == cswPrivate.View.Visibility) {
                        nameCell.hide();
                        viewNameInput.hide();
                        catCell.hide();
                        categoryInput.hide();
                        visCell.hide();
                        visibilityTbl.hide();
                    } else {
                        widthCell.hide();
                        widthInput.hide();
                    }


                    var handleAttributeChange = function () {
                        //It's better to send this to the server to modify - in some cases (ex: ViewName) we need DB resources which are not available during the "blackbox" deserialization events
                        var visibilityData = visibilitySelect.getSelected();
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleAction',
                            data: {
                                NewViewName: viewNameInput.val(),
                                NewViewCategory: categoryInput.val(),
                                NewViewWidth: widthInput.val(),
                                NewViewVisibility: visibilityData.visibility,
                                NewVisibilityRoleId: visibilityData.roleid,
                                NewVisbilityUserId: visibilityData.userid,
                                CurrentView: cswPrivate.View,
                                StepName: stepNames.ViewAttributes
                            },
                            success: function (response) {
                                cswPrivate.View = response.CurrentView;
                            }
                        });
                    };

                    var previewCell = step5Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    var previewDiv = previewCell.div();
                    cswPrivate.buildPreview(previewDiv, cswPrivate.View);

                };
            }());

            cswPrivate.makeStep6 = (function () {
                return function () {
                    cswPrivate.currentStepNo = 6;
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    cswPrivate.step6Div = cswPrivate.step6Div || cswPrivate.wizard.div(cswPrivate.currentStepNo);
                    cswPrivate.step6Div.css({
                        'width': '100%'
                    });
                    cswPrivate.step6Div.empty();

                    cswPrivate.step6Div.span({
                        text: "Edit or add any property or relationship attributes in your view.",
                        cssclass: "wizardHelpDesc"
                    });
                    cswPrivate.step6Div.br({ number: 3 });

                    cswPrivate.step6Tbl = cswPrivate.step6Div.table({
                        cellpadding: 1,
                        cellspacing: 1
                    });

                    var contentCell = cswPrivate.step6Tbl.cell(1, 1).css({
                        'width': '40%'
                    });
                    var viewContentDiv = contentCell.div();
                    cswPrivate.step6Div.buttonExt({
                        enabledText: 'Undo',
                        disabled: (cswPrivate.viewStack.length === 0),
                        onClick: function () {
                            cswPrivate.View = JSON.parse(cswPrivate.viewStack.splice(cswPrivate.viewStack.length - 1, 1)[0]);
                            cswPrivate.makeStep6();
                        }
                    });
                    var previewCell = cswPrivate.step6Tbl.cell(1, 2).css({
                        'padding-left': '40px',
                        'border-left': '1px solid #A7D3FF'
                    });
                    var previewDiv = previewCell.div();

                    Csw.ajaxWcf.post({
                        urlMethod: 'ViewEditor/GetStepData',
                        data: {
                            StepName: stepNames.FineTuning,
                            CurrentView: cswPrivate.View
                        },
                        success: function (response) {
                            viewContentDiv.$.CswViewContentTree({
                                viewstr: response.Step4.ViewJson,
                                onClick: function (node, ref_node) {
                                    onNodeClick(ref_node.rslt.obj[0].id);
                                },
                                onDeleteClick: function (arbid) {
                                    cswPrivate.viewStack.push(JSON.stringify(cswPrivate.View)); //preserve the change we make the change
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'ViewEditor/HandleAction',
                                        data: {
                                            Action: 'RemoveNode',
                                            StepName: stepNames.FineTuning,
                                            ArbitraryId: arbid,
                                            CurrentView: cswPrivate.View
                                        },
                                        success: function (removeNodeResponse) {
                                            cswPrivate.View = removeNodeResponse.CurrentView;
                                            cswPrivate.makeStep6();
                                        }
                                    });
                                }
                            });
                        }
                    });

                    cswPrivate.buildPreview(previewDiv, cswPrivate.View);

                    var onNodeClick = function (arbitraryId) {
                        Csw.ajaxWcf.post({
                            urlMethod: 'ViewEditor/HandleAction',
                            data: {
                                Action: 'Click',
                                ArbitraryId: arbitraryId,
                                CurrentView: cswPrivate.View,
                                StepName: stepNames.FineTuning
                            },
                            success: function (response) {
                                if (false === Csw.isNullOrEmpty(response.Step6.FilterNode)) {
                                    $.CswDialog('ViewEditorFilterEdit', {
                                        filterNode: response.Step6.FilterNode,
                                        view: cswPrivate.View,
                                        onBeforeFilterEdit: function () {
                                            cswPrivate.viewStack.push(JSON.stringify(cswPrivate.View));
                                        },
                                        onFilterEdit: function (updatedView) {
                                            cswPrivate.View = updatedView;
                                            cswPrivate.makeStep6();
                                        }
                                    });
                                } else if (false === Csw.isNullOrEmpty(response.Step6.RelationshipNode)) {
                                    $.CswDialog('ViewEditorRelationshipEdit', {
                                        relationshipNode: response.Step6.RelationshipNode,
                                        view: cswPrivate.View,
                                        findRelationshipByArbitraryId: cswPrivate.findRelationshipByArbitraryId,
                                        properties: response.Step6.Properties,
                                        relationships: response.Step6.Relationships,
                                        stepName: stepNames.FineTuning,
                                        onBeforeRelationshipEdit: function () {
                                            cswPrivate.viewStack.push(JSON.stringify(cswPrivate.View));
                                        },
                                        onRelationshipEdit: function (updatedView) {
                                            cswPrivate.View = updatedView;
                                            cswPrivate.makeStep6();
                                        }
                                    });
                                } else if (false === Csw.isNullOrEmpty(response.Step6.PropertyNode)) {
                                    $.CswDialog('ViewEditorPropertyEdit', {
                                        propertyNode: response.Step6.PropertyNode,
                                        view: cswPrivate.View,
                                        viewJson: response.Step4.ViewJson,
                                        stepName: stepNames.FineTuning,
                                        onBeforeFilterAdd: function () {
                                            cswPrivate.viewStack.push(JSON.stringify(cswPrivate.View));
                                        },
                                        onFilterAdd: function (updatedView) {
                                            cswPrivate.View = updatedView;
                                            cswPrivate.makeStep6();
                                        }
                                    });
                                } else if (false == Csw.isNullOrEmpty(response.Step6.RootNode)) {
                                    $.CswDialog('ViewEditorRootEdit', {
                                        relationships: response.Step6.Relationships,
                                        view: cswPrivate.View,
                                        onBeforeRelationshipAdd: function () {
                                            cswPrivate.viewStack.push(JSON.stringify(cswPrivate.View));
                                        },
                                        onAddRelationship: function (updatedView) {
                                            cswPrivate.View = updatedView;
                                            cswPrivate.makeStep6();
                                        }
                                    });
                                }
                            }
                        });
                    };

                };
            }());

            cswPrivate.buildPreview = function (previewDiv, view, afterRender) {
                Csw.ajaxWcf.post({
                    urlMethod: 'ViewEditor/GetPreview',
                    data: {
                        CurrentView: view,
                        CurrentNodeId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId)
                    },
                    success: function (response) {
                        previewDiv.empty();
                        var txtDiv = previewDiv.div();
                        txtDiv.setLabelText('Preview: ', false, false);
                        previewDiv.br({ number: 2 });
                        var previewData = JSON.parse(response.Preview);
                        if (cswPrivate.View.ViewMode === 'Grid' || cswPrivate.View.ViewMode === 'Table') {
                            if (cswPrivate.previewGrid) {
                                cswPrivate.previewGrid.remove(); //if we don't remove, we got wacky column behavior
                            }
                            var gridWidth = 700;
                            //if ("Property" === cswPrivate.View.Visibility) {
                            //    gridWidth = cswPrivate.View.Width;
                            //}
                            cswPrivate.previewGrid = previewDiv.grid({
                                name: 'vieweditor_previewgrid',
                                storeId: 'vieweditor_store',
                                title: '',
                                stateId: 'vieweditor_gridstate',
                                usePaging: false,
                                showActionColumn: false,
                                height: 210,
                                width: gridWidth,
                                fields: previewData.grid.fields,
                                columns: previewData.grid.columns,
                                data: previewData.grid.data,
                                pageSize: previewData.grid.pageSize,
                                canSelectRow: false,
                                onColumnReorder: cswPrivate.onColumnReorder
                            });
                            Csw.tryExec(afterRender);
                        } else if (cswPrivate.View.ViewMode === 'Tree' || cswPrivate.View.ViewMode === 'List') {
                            if (cswPrivate.previewTree) {
                                cswPrivate.previewTree.menuDiv.remove();
                            }
                            cswPrivate.previewTree = Csw.nbt.nodeTree({
                                name: 'vieweditor_treepreview',
                                height: '175px',
                                width: '700px',
                                parent: previewDiv
                            });
                            cswPrivate.previewTree.makeTree(previewData);
                            cswPrivate.previewTree.expandAll();
                        }
                    }
                });
            };

            cswPrivate.findRelationshipByArbitraryId = function (arbitraryId) {
                var ret = null;
                var recurse = function (relationship) {
                    var innerRet = null;
                    if (relationship.ArbitraryId === arbitraryId) {
                        innerRet = relationship;
                    } else {
                        Csw.each(relationship.ChildRelationships, function (childRel) {
                            var found = recurse(childRel);
                            if (found) {
                                innerRet = found;
                            }
                        });
                    }
                    return innerRet;
                };
                if (cswPrivate.View.Root.ArbitraryId == arbitraryId) {
                    ret = cswPrivate.View.Root;
                } else {
                    ret = recurse(cswPrivate.View.Root);
                }
                return ret;
            };

            cswPrivate.findPropertiesByName = function (name) {
                var ret = [];

                var recurse = function (relationship) {
                    Csw.each(relationship.Properties, function (prop) {
                        if (prop.TextLabel === name) {
                            ret.push(prop);
                        }
                    });
                    Csw.each(relationship.ChildRelationships, function (childRel) {
                        recurse(childRel);
                    });
                };
                recurse(cswPrivate.View.Root);

                return ret;
            };

            cswPrivate.getRootLevelRelationships = function () {
                var ret = [];

                Csw.each(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                    if ('Property' !== cswPrivate.View.Visibility) {
                        ret.push(childRel);
                    } else {
                        Csw.each(childRel.ChildRelationships, function (childChildRel) {
                            ret.push(childChildRel);
                        });
                    }
                });
                return ret;
            };

            cswPrivate.getAllRelationships = function () {
                var ret = [];

                var recurse = function (relationship) {
                    Csw.each(relationship.ChildRelationships, function (childRel) {
                        ret.push(childRel);
                        recurse(childRel);
                    });
                };

                recurse(cswPrivate.View.Root);
                return ret;
            };

            cswPrivate.getRootRelationship = function () {
                //for property views this isn't the view root, it's the 2nd level
                var ret = cswPrivate.View.Root;
                if ('Property' === cswPrivate.View.Visibility) {
                    ret = cswPrivate.View.Root.ChildRelationships[0];
                }
                return ret;
            };

            cswPrivate.getProperties = function (includeDuplicates) {
                var props = [];
                var seen = [];

                var recurse = function (relationship) {
                    Csw.each(relationship.Properties, function (prop) {
                        if ((seen.indexOf(prop.TextLabel) == -1 || includeDuplicates) && prop.ShowInGrid) {
                            props.push(prop);
                            seen.push(prop.TextLabel);
                        }
                    });
                    Csw.each(relationship.ChildRelationships, function (childRel) {
                        recurse(childRel);
                    });
                };
                recurse(cswPrivate.View.Root);

                var sortByOrder = function (prop1, prop2) {
                    var ret = 0;
                    if (prop1.Order < prop2.Order) ret = -1;
                    if (prop1.Order > prop2.Order) ret = 1;
                    return ret;
                };

                return props.sort(sortByOrder);
            };

            cswPrivate.onColumnReorder = function () {
                if (cswPrivate.previewGrid && 3 === cswPrivate.currentStepNo && 'Grid' === cswPrivate.View.ViewMode) {
                    var colIdx = 1;
                    var orderMap = {};
                    Csw.each(cswPrivate.previewGrid.extGrid.getView().getGridColumns(), function (col) {
                        if (false === col.hidden) { //ignore internal columns
                            var foundProps = cswPrivate.findPropertiesByName(col.text);
                            Csw.each(foundProps, function (prop) {
                                prop.Order = colIdx;
                            });
                            colIdx++;
                        }
                    });
                    if (renderedStep3) {
                        cswPrivate.makePropsTbl();
                    }
                }
            };

            //#region ctor

            (function () {
                Csw.extend(cswPrivate, options, true);
                cswPrivate.startingStep = Csw.number(cswPrivate.startingStep, 1);
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.finalize = function () {
                    Csw.ajaxWcf.post({
                        urlMethod: 'ViewEditor/Finalize',
                        data: {
                            CurrentView: cswPrivate.View
                        },
                        success: function (response) {
                            cswPrivate.View = response.CurrentView;
                            cswPrivate.onFinish(cswPrivate.View.ViewId, cswPrivate.View.ViewMode);
                        }
                    });
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'View Editor',
                    StepCount: cswPrivate.stepCount,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: Csw.number(cswPrivate.startingStep, 1),
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                    },
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false,
                    onBeforePrevious: function (currentStep) {
                        var ret = true;
                        if (1 === (currentStep - 1)) {
                            if (confirm("You will lose any changes made to the current view if you continue. Are you sure?")) {
                                ret = true;
                            } else {
                                ret = false;
                            }
                        }
                        return ret;
                    }
                });

                cswPrivate.handleStep(cswPrivate.startingStep);

            }());

            //#endregion ctor



            return cswPublic;
        });
}());