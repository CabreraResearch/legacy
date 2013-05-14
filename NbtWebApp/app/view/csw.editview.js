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
                        'width': '320px'
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
                                cswPrivate.propsTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 3,
                                    cellpadding: 3
                                });

                                var row = 1;
                                Csw.each(response.Step2.Relationships, function (ViewRel) {
                                    var thisChckBox = cswPrivate.propsTbl.cell(row, 1).input({
                                        name: ViewRel.Relationship.ArbitraryId + '_chkbox',
                                        type: Csw.enums.inputTypes.checkbox,
                                        canCheck: true,
                                        checked: ViewRel.Checked,
                                        onClick: function () {
                                            if (thisChckBox.checked()) {
                                                cswPrivate.View.Root.ChildRelationships.push(ViewRel.Relationship);
                                            } else {
                                                var cleansedRelationships = [];
                                                Csw.iterate(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                                                    if (childRel.ArbitraryId !== ViewRel.Relationship.ArbitraryId) {
                                                        cleansedRelationships.push(childRel);
                                                    }
                                                });
                                                cswPrivate.View.Root.ChildRelationships = cleansedRelationships;
                                            }
                                            cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                        }
                                    });
                                    cswPrivate.propsTbl.cell(row, 2).text(ViewRel.Relationship.TextLabel);
                                    row++;
                                });

                                cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

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
                                cswPrivate.propsTbl = cswPrivate.propsDiv.table({
                                    cellspacing: 3,
                                    cellpadding: 3
                                });

                                var row = 1;
                                Csw.each(response.Step3.Properties, function (ViewProp) {
                                    var thisChckBox = cswPrivate.propsTbl.cell(row, 1).input({
                                        name: ViewProp.Property.ArbitraryId + '_chkbox',
                                        type: Csw.enums.inputTypes.checkbox,
                                        canCheck: true,
                                        checked: ViewProp.Checked,
                                        onClick: function () {
                                            Csw.iterate(cswPrivate.View.Root.ChildRelationships, function (childRel) {
                                                if (ViewProp.Property.ParentArbitraryId === childRel.ArbitraryId) {
                                                    if (thisChckBox.checked()) {
                                                        childRel.Properties.push(ViewProp.Property);
                                                    }
                                                    else {
                                                        var newProps = [];
                                                        Csw.iterate(childRel.Properties, function (prop) {
                                                            if (prop.ArbitraryId !== ViewProp.Property.ArbitraryId) {
                                                                newProps.push(prop);
                                                            }
                                                        });
                                                        childRel.Properties = newProps;
                                                    }
                                                }
                                            });
                                            cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                        }
                                    });
                                    cswPrivate.propsTbl.cell(row, 2).text(ViewProp.Property.TextLabel);
                                    var thisOrderInput = cswPrivate.propsTbl.cell(row, 3).input({
                                        name: ViewProp.Property.ArbitraryId + '_orderinput',
                                        value: Csw.int32MinVal !== ViewProp.Property.Order ? ViewProp.Property.Order : '',
                                        size: 4,
                                        onChange: function () {
                                            ViewProp.Property.Order = thisOrderInput.val(); //TODO: this must be a number
                                            cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);
                                        }
                                    });

                                    row++;
                                });

                                cswPrivate.buildPreview(cswPrivate.previewDiv, cswPrivate.View);

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