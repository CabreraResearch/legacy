/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.createInspectionWizard = Csw.nbt.createInspectionWizard ||
        Csw.nbt.register('createInspectionWizard', function (cswParent, options) {
            'use strict';

            //#region Variable Declaration
            var internal = {
                ID: 'cswInspectionDesignWizard',
                onCancel: null, //function ($wizard) {},
                onFinish: null, //function ($wizard) {},
                startingStep: 1,
                wizard: '',
                wizardSteps: {
                    1: Csw.enums.wizardSteps_InspectionDesign.step1.description,
                    2: Csw.enums.wizardSteps_InspectionDesign.step2.description,
                    3: Csw.enums.wizardSteps_InspectionDesign.step3.description,
                    4: Csw.enums.wizardSteps_InspectionDesign.step4.description,
                    5: Csw.enums.wizardSteps_InspectionDesign.step5.description
                },
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '', divStep2: '', divStep3: '', divStep4: '', divStep5: '',
                selectedInspectionTarget: '',
                inspectionTargetSelect: '',
                addNewTarget: '',
                categoryNameInput: '',
                selectedInspectionDesign: { id: '[Create New]', name: '[Create New]' },
                categoryName: '',
                inspectionDesignSelect: '',
                gridIsPopulated: false,
                inspectionGrid: '',
                gridOptions: {}
            };
            if (options) $.extend(internal, options);

            var external = cswParent.div();
            internal.currentStepNo = internal.startingStep;

            internal.isNewInspectionDesign = function () {
                return ('[Create New]' === internal.selectedInspectionDesign.id);
            };

            internal.isNewTarget = (function () {
                var ret = false;
                return function (isNew) {
                    if (arguments.length > 0) {
                        ret = Csw.bool(isNew);
                    }
                    return ret;
                };
            } ());

            internal.createInspectionEvents = {
                targetNameChanged: 'targetNameChanged',
                designNameChanged: 'designNameChanged'
            };

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
                if (button !== internal.buttons.finish) {
                    internal.toggleButton(internal.buttons.finish, (internal.currentStepNo === 5));
                }

                return false;
            };

            internal.makeStepId = function (suffix, stepNo) {
                var step = stepNo || internal.currentStepNo;
                return Csw.makeId({ prefix: 'step_' + step, ID: internal.ID, suffix: suffix });
            };

            //Step 1. Select an Inspection Target.
            internal.makeStepOne = (function () {
                var stepOneComplete = false,
                    inspectionTable, addBtn, rowOneTable;
                return function () {

                    var onNodeTypeSelectSuccess = function (data) {
                        //If the picklist is empty, we have to add a new Target
                        if (data.nodetypecount === 0) {
                            internal.inspectionTargetSelect.hide();
                            internal.isNewTarget(true);
                            internal.addNewTarget = rowOneTable.cell(2, 2);
                            internal.addNewTarget.css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .input({
                                    suffix: 'newTargetName',
                                    value: '',
                                    maxlength: 40
                                })
                                .propNonDom('maxlength', 40)
                                .$.keypress(function () {
                                    setTimeout(function () {
                                        var newTargetName = internal.addNewTarget.val();
                                        if (false === Csw.isNullOrEmpty(newTargetName)) {
                                            internal.wizard.next.enable();
                                        }
                                    }, 100);
                                });
                        } else { //Select an existing Target or add a new Target
                            internal.selectedInspectionTarget = internal.inspectionTargetSelect.find(':selected').text();
                            internal.wizard.next.enable();

                            addBtn = addBtn || rowOneTable.cell(2, 3);
                            addBtn.css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .div()
                                .button({
                                    ID: internal.makeStepId('addNewInspectionTarget'),
                                    enabledText: 'Add New',
                                    disableOnClick: false,
                                    onClick: function () {
                                        $.CswDialog('AddNodeTypeDialog', {
                                            objectclassid: internal.inspectionTargetSelect.find(':selected').data('objectClassId'),
                                            nodetypename: '',
                                            category: 'do not show',
                                            select: internal.inspectionTargetSelect,
                                            nodeTypeDescriptor: 'Target',
                                            maxlength: 40,
                                            onSuccess: function (newData) {
                                                var proposedInspectionTarget = newData.nodetypename;
                                                if (internal.checkTargetIsClientSideUnique(proposedInspectionTarget)) {
                                                    internal.selectedInspectionTarget = proposedInspectionTarget;
                                                    internal.isNewTarget(true);
                                                    internal.wizard.next.enable();
                                                    Csw.publish(internal.createInspectionEvents.targetNameChanged);
                                                } else {
                                                    internal.inspectionTargetSelect.find('option[value="' + proposedInspectionTarget + '"]').remove();
                                                }
                                            },
                                            title: 'Create a New Inspection Target Type.'
                                        });
                                        return false;
                                    }
                                });
                        } // else
                    };

                    var makeTargetSelect = function () {
                        //Normally this would be written as $inspectionTarget = $inspectionTarget || ...
                        //However, the variable assignment is sufficiently complex that this deviation is justified.
                        if (false === Csw.isNullOrEmpty(internal.inspectionTargetSelect, true)) {
                            internal.inspectionTargetSelect.remove();
                        }

                        internal.inspectionTargetSelect = rowOneTable.cell(2, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .div()
                            .nodeTypeSelect({
                                ID: internal.makeStepId('nodeTypeSelect'),
                                objectClassName: 'InspectionTargetClass',
                                onSelect: function () {
                                    var selected = internal.inspectionTargetSelect.find(':selected');
                                    internal.isNewTarget(selected.propNonDom('data-newNodeType'));
                                    internal.selectedInspectionTarget = selected.text();
                                    Csw.publish(internal.createInspectionEvents.targetNameChanged);
                                },
                                onSuccess: function (data) {
                                    onNodeTypeSelectSuccess(data);
                                    internal.selectedInspectionTarget = internal.inspectionTargetSelect.find(':selected').text();
                                }
                            });
                    };

                    if (false === stepOneComplete) {
                        internal.divStep1 = internal.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step1.step);
                        internal.divStep1.br();

                        inspectionTable = internal.divStep1.table({
                            ID: internal.makeStepId('setInspectionTargetTable')
                        });

                        rowOneTable = inspectionTable.cell(1, 1).table({
                            FirstCellRightAlign: true
                        });

                        rowOneTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'What do you want to inspect?' });

                        makeTargetSelect();
                        stepOneComplete = true;
                    } // if (false === stepOneComplete)

                    internal.toggleButton(internal.buttons.prev, false);
                    internal.toggleButton(internal.buttons.next, (false === Csw.isNullOrEmpty(internal.selectedInspectionTarget)));
                };
            } ());

            //Step 2. Select an Inspection Design Design.
            internal.makeStepTwo = (function () {
                var stepTwoComplete = false;

                return function () {
                    var inspectionTable, $newDesignLabel, newDesignNameDisplay,
                        tempInspectionName = internal.selectedInspectionTarget + ' Inspection',
                        tempCategoryName = internal.selectedInspectionTarget;

                    var makeInspectionDesignName = function (name) {
                        var ret = Csw.string(name).trim();
                        if (-1 === ret.indexOf('Inspection') && -1 === ret.indexOf('inspection')) {
                            ret += ' Inspection';
                        }
                        return ret;
                    };

                    var toggleNewDesignName = function () {
                        if (internal.isNewInspectionDesign()) {
                            internal.newDesignName.$.show();
                            $newDesignLabel.show();
                            newDesignNameDisplay.show();
                        } else {
                            internal.newDesignName.$.hide();
                            $newDesignLabel.hide();
                            newDesignNameDisplay.hide();
                        }
                    };
                    var nextBtnEnabled = function () {
                        return (false === Csw.isNullOrEmpty(internal.selectedInspectionDesign.name));
                    };

                    var targetChangedHandle = function () {
                        internal.newDesignName.val(internal.selectedInspectionTarget + ' Inspection');
                        newDesignNameDisplay.text(internal.selectedInspectionTarget + ' Inspection');
                        internal.categoryNameInput.val(internal.selectedInspectionTarget);
                        if (internal.isNewInspectionDesign()) {
                            internal.selectedInspectionDesign.name = internal.selectedInspectionTarget + ' Inspection';
                        }
                        Csw.publish(internal.createInspectionEvents.designNameChanged);
                    };

                    internal.toggleButton(internal.buttons.prev, true);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === stepTwoComplete) {
                        internal.divStep2 = internal.divStep2 || internal.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step2.step);
                        internal.divStep2.empty();
                        internal.divStep2.br();

                        Csw.subscribe(internal.createInspectionEvents.targetNameChanged, targetChangedHandle);

                        inspectionTable = internal.divStep2.table({
                            ID: internal.makeStepId('inspectionTable'),
                            FirstCellRightAlign: true
                        });

                        //1. Copy from Inspection Design
                        inspectionTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'Select an Inspection Design&nbsp' });

                        internal.inspectionDesignSelect = inspectionTable.cell(1, 2);
                        internal.inspectionDesignSelect.div()
                            .nodeTypeSelect({
                                ID: Csw.makeSafeId('nodeTypeSelect'),
                                objectClassName: 'InspectionDesignClass',
                                addNewOption: true
                            })
                            .change(function () {
                                var selected = internal.inspectionDesignSelect.find(':selected');
                                internal.selectedInspectionDesign.id = selected.val();
                                if (internal.isNewInspectionDesign() && internal.newDesignName && false === Csw.isNullOrEmpty(internal.newDesignName.val())) {
                                    internal.selectedInspectionDesign.name = internal.newDesignName.val();
                                } else {
                                    internal.selectedInspectionDesign.name = selected.text();
                                }
                                tempCategoryName = internal.selectedInspectionTarget;
                                internal.categoryNameInput.val(tempCategoryName);
                                Csw.publish(internal.createInspectionEvents.designNameChanged);
                                toggleNewDesignName();
                            });
                        //Create New is selected by default
                        internal.selectedInspectionDesign.id = internal.inspectionDesignSelect.find(':selected').val();
                        internal.selectedInspectionDesign.name = makeInspectionDesignName(internal.selectedInspectionTarget);

                        inspectionTable.cell(2, 1).br();

                        //2. New Inspection Design Name
                        $newDesignLabel = inspectionTable.cell(3, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ cssclass: 'required', text: 'New Inspection Design Name&nbsp' });

                        internal.newDesignName = inspectionTable.cell(3, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .input({
                                ID: internal.ID + '_newDesignName',
                                type: Csw.enums.inputTypes.text,
                                cssclass: 'required',
                                maxlength: 50,
                                width: (50 * 7) + 'px',
                                value: tempInspectionName
                            })
                            .bind('change keypress keydown keyup', function () {
                                setTimeout(function () {
                                    var newInspectionDesignName = makeInspectionDesignName(internal.newDesignName.val());
                                    internal.selectedInspectionDesign.id = '[Create New]';
                                    internal.selectedInspectionDesign.name = newInspectionDesignName;
                                    newDesignNameDisplay.text(newInspectionDesignName);
                                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());
                                    Csw.publish(internal.createInspectionEvents.designNameChanged);
                                }, 10);
                            });

                        newDesignNameDisplay = inspectionTable.cell(4, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: tempInspectionName });

                        inspectionTable.cell(5, 1).br();

                        //2. Category Name
                        inspectionTable.cell(6, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'Category Name&nbsp' });

                        internal.categoryNameInput = internal.categoryNameInput ||
                            inspectionTable.cell(6, 2)
                                .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .input({
                                    ID: internal.ID + '_newDesignCategory',
                                    type: Csw.enums.inputTypes.text,
                                    value: tempCategoryName,
                                    maxlength: 40,
                                    width: (40 * 7) + 'px'
                                });

                        toggleNewDesignName();

                        inspectionTable.cell(6, 1).br();
                    }
                    stepTwoComplete = true;
                };
            } ());

            internal.checkIsNodeTypeNameUnique = function (name, success, error) {
                Csw.ajax.post({
                    url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
                    async: false,
                    data: { 'NodeTypeName': name },
                    success: function (data) {
                        Csw.tryExec(success, data);
                    },
                    error: function (data) {
                        Csw.tryExec(error, data);
                        internal.toggleButton(internal.buttons.next);
                        internal.toggleButton(internal.buttons.prev, true);
                    }
                });
            };

            //File upload onSuccess event to prep Step 4
            internal.makeInspectionDesignGrid = function (jqGridOpts, onSuccess) {
                Csw.tryExec(onSuccess);
                internal.gridIsPopulated = true;

                //This is ugly. Abstract the step div from this function.
                internal.divStep4.empty();
                var previewGridId = internal.makeStepId('previewGrid_outer', 4);

                var helpText = internal.divStep4.p({ text: '<p>Review the <b>' + internal.selectedInspectionDesign.name + '</b> upload results. Make any necessary edits.' });

                var designChangeHandle = function () {
                    helpText.remove();
                    helpText = internal.divStep4.p({ text: '<p>Review the <b>' + internal.selectedInspectionDesign.name + '</b> upload results. Make any necessary edits.' });
                };
                Csw.subscribe(internal.createInspectionEvents.designNameChanged, designChangeHandle);

                if (Csw.isNullOrEmpty(internal.inspectionGridDiv) || internal.inspectionGridDiv.length() === 0) {
                    internal.inspectionGridDiv = internal.divStep4.div({ ID: previewGridId });
                } else {
                    internal.inspectionGridDiv.empty();
                }

                internal.gridOptions = {
                    ID: internal.makeStepId('previewGrid'),
                    pagermode: 'default',
                    gridOpts: {
                        autowidth: true,
                        height: '200'
                    },
                    optNav: {
                        add: true,
                        del: true,
                        edit: true,
                        view: false,
                        editfunc: function (rowid) {
                            return internal.inspectionGrid.gridTable.$.jqGrid('editGridRow', rowid, { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterEdit: true });
                        },
                        addfunc: function () {
                            return internal.inspectionGrid.gridTable.$.jqGrid('editGridRow', 'new', { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterAdd: true });
                        },
                        delfunc: function (rowid) {
                            return internal.inspectionGrid.gridTable.$.jqGrid('delRowData', rowid);
                        }
                    }
                };

                if (false === Csw.contains(jqGridOpts, 'data') ||
                    false === Csw.contains(jqGridOpts, 'colNames') ||
                        jqGridOpts.colNames.length === 0) {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Inspection Design upload failed. Please check your design and try again.'));
                    internal.toggleButton(internal.buttons.next, false);
                    internal.toggleButton(internal.buttons.prev, true, true);
                } else {
                    $.extend(internal.gridOptions.gridOpts, jqGridOpts);
                }
                internal.inspectionGrid = internal.inspectionGridDiv.grid(internal.gridOptions);
            };

            //File upload button for Step 3
            internal.makeInspectionDesignUpload = function (control) {
                var f = {
                    url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                    onSuccess: function () {
                        internal.wizard.next.enable().click();
                    },
                    stepNo: Csw.enums.wizardSteps_InspectionDesign.step3.step,
                    uploadName: 'design'
                };

                control.$.fileupload({
                    datatype: 'json',
                    dataType: 'json',
                    url: f.url,
                    paramName: 'fileupload',
                    done: function (e, ret) {
                        var gridData = {};
                        if (Csw.contains(ret, 'result') && Csw.contains(ret.result, 'jqGridOpt')) {
                            gridData = ret.result.jqGridOpt;
                            internal.makeInspectionDesignGrid(gridData, f.onSuccess);
                        }
                    }
                });
            };

            //If this is a new Design, upload the template. Otherwise skip to step 5.
            internal.makeStepThree = (function () {
                var stepThreeComplete = false;

                return function (forward) {
                    //this is somewhat dundant, but these calls are cheap and it improves readability until we have time to tighten our shot group
                    var nextIsEnabled = function () {
                        return internal.gridIsPopulated || false === internal.isNewInspectionDesign();
                    };
                    var doNextClick = function () {
                        return false === internal.isNewInspectionDesign() && forward;
                    };
                    var doPrevClick = function () {
                        return false === internal.isNewInspectionDesign() && false === Csw.bool(forward);
                    };

                    var doStepThree = function () {
                        var step3List, templateLink, uploadP, helpText;
                        var designChangeHandle = function (help) {

                            helpText.empty();
                            helpText.span({ text: 'Create a new <b>' + internal.selectedInspectionDesign.name + '</b> Design using the Excel template.' })
                                    .p()
                                    .link({ href: '\"/NbtWebApp/etc/InspectionDesign.xls\"', text: 'Download Template' })
                                    .$.button();
                        };
                        if (false === stepThreeComplete) {
                            internal.divStep3 = internal.divStep3 || internal.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step3.step);
                            internal.divStep3.empty();

                            //Ordered instructions
                            step3List = internal.divStep3.ol({
                                ID: internal.makeStepId('uploadTemplateList')
                            });

                            //1. Download template

                            helpText = step3List.li();
                            designChangeHandle();
                            Csw.subscribe(internal.createInspectionEvents.designNameChanged, designChangeHandle);

                            //2. Edit the template.
                            step3List.li().span({ text: 'Edit the Inspection template.' });

                            //3. Upload the design
                            uploadP = step3List.li()
                                                .span({ text: 'Upload the completed Inspection Design.' })
                                                .p()
                                                .input({ ID: internal.makeStepId('fileUploadBtn'), type: Csw.enums.inputTypes.file, name: 'fileupload', value: 'Upload' });
                            internal.makeInspectionDesignUpload(uploadP);



                            //$fileUploadBtn.hide();
                            //stepThreeComplete = true;
                        }
                    }; //doStepTwo

                    if (internal.isNewInspectionDesign()) {
                        //selectedInspectionDesign.name = $newDesignName.val();
                        internal.checkIsNodeTypeNameUnique(internal.selectedInspectionDesign.name, doStepThree);
                    }
                    internal.toggleButton(internal.buttons.next, nextIsEnabled(), doNextClick());
                    internal.toggleButton(internal.buttons.prev, true, doPrevClick());
                };
            } ());

            //Step 4. Review the Design grid.
            internal.makeStepFour = (function () {
                var stepFourComplete = false;
                //We populate this step as the result of the async design upload. Improve the readability of this code when you next visit.
                return function (forward) {
                    var skipStepFour = false;
                    var doNextClick = function () {
                        skipStepFour = (false === internal.isNewInspectionDesign() && forward);
                        return skipStepFour;
                    };
                    var doPrevClick = function () {
                        skipStepFour = (false === internal.isNewInspectionDesign() && false == Csw.bool(forward));
                        return skipStepFour;
                    };

                    internal.toggleButton(internal.buttons.next, true, doNextClick());
                    internal.toggleButton(internal.buttons.prev, true, doPrevClick());

                    if (false === stepFourComplete &&
                        false === skipStepFour) {
                        internal.divStep4 = internal.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step4.step);
                        stepFourComplete = true;
                    }
                };
            } ());

            internal.checkTargetIsClientSideUnique = function (proposedTargetName) {
                var ret = false,
                    targetName = proposedTargetName || internal.selectedInspectionTarget;
                if (Csw.string(targetName).trim().toLowerCase() != Csw.string(internal.selectedInspectionDesign.name).trim().toLowerCase()) {
                    ret = true;
                } else {
                    $.CswDialog('ErrorDialog', Csw.error.makeErrorObj(Csw.enums.errorType.warning.name,
                        'An Inspection Design and an Inspection Target cannot have the same name.',
                        'Attempted to create Inspection Target ' + targetName + ' against Inspection Design ' + internal.selectedInspectionDesign.name));
                }
                return ret;
            };

            //Step 5. Preview and Finish.
            internal.makeStepFive = (function () {

                return function () {
                    var confirmationList, confirmTypesList, confirmViewsList, confirmGridOptions = {};

                    if (internal.checkTargetIsClientSideUnique()) {

                        internal.toggleButton(internal.buttons.prev, true);
                        internal.toggleButton(internal.buttons.next, false);

                        internal.categoryName = internal.categoryNameInput.val();

                        internal.divStep5 = internal.divStep5 || internal.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step5.step);
                        internal.divStep5.empty();

                        internal.divStep5.p({ text: 'You are about to create the following items. Click Finish to create the design.' });
                        confirmationList = internal.divStep5.ol({
                            ID: internal.makeStepId('confirmationList')
                        });

                        if (internal.isNewInspectionDesign()) {
                            if (internal.gridOptions) {
                                $.extend(true, confirmGridOptions, internal.gridOptions);
                            }

                            confirmGridOptions.ID = internal.makeStepId('confirmGrid');
                            confirmGridOptions.gridOpts.data = internal.inspectionGrid.gridTable.$.jqGrid('getRowData');
                            confirmGridOptions.gridOpts.autowidth = false;
                            confirmGridOptions.gridOpts.shrinkToFit = true;
                            confirmGridOptions.gridOpts.height = 150;
                            confirmGridOptions.optNav.add = false;
                            confirmGridOptions.optNav.del = false;
                            confirmGridOptions.optNav.edit = false;
                            confirmGridOptions.optNav.view = false;
                            confirmGridOptions.optNav.editfunc = null;
                            confirmGridOptions.optNav.addfunc = null;
                            confirmGridOptions.optNav.delfunc = null;
                            Csw.each(confirmGridOptions.gridOpts.colModel, function (col) {
                                if (Csw.contains(col, 'editable')) {
                                    delete col.editable;
                                }
                                if (Csw.contains(col, 'edittype')) {
                                    delete col.edittype;
                                }
                            });

                            var confirmGridParent = confirmationList.li({
                                text: 'Creating a new Inspection Design <b>' + internal.selectedInspectionDesign.name + '</b>.'
                            });
                            confirmGridParent.div().grid(confirmGridOptions);
                        } else {
                            confirmationList.li({
                                text: 'Assigning Inspection Design <b>' + internal.selectedInspectionDesign.name + '</b> to Inspection Target <b> ' + internal.selectedInspectionTarget + '</b>.'
                            }).br();
                        }

                        if (internal.isNewInspectionDesign() || internal.isNewTarget()) {
                            confirmTypesList = confirmationList.li({
                                text: 'New Types'
                            })
                                .ul({
                                    ID: internal.makeStepId('confirmationTypes')
                                });

                            if (internal.isNewInspectionDesign()) {
                                confirmTypesList.li({
                                    text: 'New Inspection Design <b>' + internal.selectedInspectionDesign.name + '</b> on Inspection Target <b>' + internal.selectedInspectionTarget + '</b>'
                                });
                            }

                            if (internal.isNewTarget) {
                                confirmTypesList.li({
                                    text: 'New Inspection Target <b>' + internal.selectedInspectionTarget + '</b>'
                                });

                                confirmTypesList.li({
                                    text: 'New Inspection Target Group <b>' + internal.selectedInspectionTarget + ' Group</b>'
                                });
                            }
                        }

                        confirmViewsList = confirmationList.li({
                            text: 'New Views'
                        })
                            .ul({
                                ID: internal.makeStepId('confirmationViews')
                            });
                        confirmViewsList.li({
                            text: '<b>Scheduling, ' + internal.selectedInspectionDesign.name + ': ' + internal.selectedInspectionTarget + '</b>'
                        });
                        confirmViewsList.li({
                            text: '<b>Groups, ' + internal.selectedInspectionDesign.name + ': ' + internal.selectedInspectionTarget + '</b>'
                        });
                        confirmViewsList.li({
                            text: '<b>Inspections, ' + internal.selectedInspectionDesign.name + ': ' + internal.selectedInspectionTarget + '</b>'
                        });
                    } else {
                        internal.toggleButton(internal.buttons.prev, true, true);
                    }
                };
            } ());

            internal.handleNext = function (newStepNo) {
                internal.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_InspectionDesign.step2.step:
                        internal.makeStepTwo();
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step3.step:
                        internal.makeStepThree(true); //we're moving forward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step4.step:
                        internal.makeStepFour(true); //we're moving forward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step5.step:
                        internal.makeStepFive();
                        break;
                } // switch(newstepno)
            }; // handleNext()

            internal.handlePrevious = function (newStepNo) {
                internal.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_InspectionDesign.step1.step:
                        internal.makeStepOne();
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step2.step:
                        internal.makeStepTwo(); //we're moving backward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step3.step:
                        internal.makeStepThree(false); //we're moving backward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step4.step:
                        internal.makeStepFour(false);
                        break;
                }
            };

            internal.onFinish = function () {
                var designGrid = '';

                internal.toggleButton(internal.buttons.prev, false);
                internal.toggleButton(internal.buttons.next, false);
                internal.toggleButton(internal.buttons.finish, false);
                internal.toggleButton(internal.buttons.cancel, false);

                if (false === Csw.isNullOrEmpty(internal.inspectionGrid)) {
                    designGrid = JSON.stringify(internal.inspectionGrid.getAllGridRows());
                }

                var jsonData = {
                    DesignGrid: designGrid,
                    InspectionDesignName: Csw.string(internal.selectedInspectionDesign.name),
                    InspectionTargetName: Csw.string(internal.selectedInspectionTarget),
                    IsNewInspection: internal.isNewInspectionDesign(),
                    IsNewTarget: internal.isNewTarget(),
                    Category: Csw.string(internal.categoryName)
                };

                Csw.ajax.post({
                    url: '/NbtWebApp/wsNBT.asmx/finalizeInspectionDesign',
                    data: jsonData,
                    success: function (data) {
                        //Come back and hammer this out
                        var views = data.views,
                                values = [];

                        Csw.each(views, function (thisView) {
                            if (Csw.contains(thisView, 'viewid') &&
                                    Csw.contains(thisView, 'viewname')) {
                                values.push({
                                    value: thisView.viewid,
                                    display: thisView.viewname
                                });
                            }
                        });

                        $.CswDialog('NavigationSelectDialog', {
                            ID: Csw.makeSafeId('FinishDialog'),
                            title: 'The Inspection Design Wizard Completed Successfully',
                            navigationText: 'Please select from the following views. Click OK to continue.',
                            values: values,
                            onOkClick: function (selectedView) {
                                var viewId = selectedView.val();
                                Csw.tryExec(internal.onFinish, viewId);
                            }
                        });

                    },
                    error: function () {
                        internal.toggleButton(internal.buttons.cancel, true);
                        internal.toggleButton(internal.buttons.prev, true);
                    }
                });
            };

            (function () {

                internal.wizard = Csw.layouts.wizard(external, {
                    ID: Csw.makeId({ ID: internal.ID, suffix: 'wizard' }),
                    Title: 'Create New Inspection',
                    StepCount: Csw.enums.wizardSteps_InspectionDesign.stepcount,
                    Steps: internal.wizardSteps,
                    StartingStep: internal.startingStep,
                    FinishText: 'Finish',
                    onNext: internal.handleNext,
                    onPrevious: internal.handlePrevious,
                    onCancel: internal.onCancel,
                    onFinish: internal.onFinish,
                    doNextOnInit: false
                });

                internal.makeStepOne();

            } ());

            return external;
        });
} ());

