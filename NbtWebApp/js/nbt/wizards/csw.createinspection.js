/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.nbt.createInspectionWizard = Csw.nbt.createInspectionWizard ||
        Csw.nbt.register('createInspectionWizard', function (cswParent, options) {
            'use strict';

            //#region Variable Declaration
            var cswPrivateVar = {
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
            if (options) $.extend(cswPrivateVar, options);

            var cswPublicRet = cswParent.div();
            cswPrivateVar.currentStepNo = cswPrivateVar.startingStep;

            cswPrivateVar.isNewInspectionDesign = function () {
                return ('[Create New]' === cswPrivateVar.selectedInspectionDesign.id);
            };

            cswPrivateVar.isNewTarget = (function () {
                var ret = false;
                return function (isNew) {
                    if (arguments.length > 0) {
                        ret = Csw.bool(isNew);
                    }
                    return ret;
                };
            } ());

            cswPrivateVar.createInspectionEvents = {
                targetNameChanged: 'targetNameChanged',
                designNameChanged: 'designNameChanged'
            };

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
                if (button !== cswPrivateVar.buttons.finish) {
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, (cswPrivateVar.currentStepNo === 5));
                }

                return false;
            };

            cswPrivateVar.makeStepId = function (suffix, stepNo) {
                var step = stepNo || cswPrivateVar.currentStepNo;
                return Csw.makeId({ prefix: 'step_' + step, ID: cswPrivateVar.ID, suffix: suffix });
            };

            cswPrivateVar.validationFailed = function () {
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true, true);
            };

            cswPrivateVar.checkTargetIsClientSideUnique = function () {
                var ret = false;
                if (Csw.string(cswPrivateVar.selectedInspectionTarget).trim().toLowerCase() != Csw.string(cswPrivateVar.selectedInspectionDesign.name).trim().toLowerCase()) {
                    ret = true;
                } else {
                    cswPrivateVar.validationFailed();
                    var err = Csw.error.makeErrorObj(Csw.enums.errorType.error,
                                                     'An Inspection Design and an Inspection Target cannot have the same name.',
                                                     'Attempted to create Inspection Target ' + cswPrivateVar.selectedInspectionTarget + ' against Inspection Design ' + cswPrivateVar.selectedInspectionDesign.name);
                    Csw.error.showError(err);
                }
                return ret;
            };

            cswPrivateVar.checkIsNodeTypeNameUnique = function (name, success, error) {
                if (cswPrivateVar.checkTargetIsClientSideUnique()) {
                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
                        async: false,
                        data: { 'NodeTypeName': name },
                        success: function (data) {
                            Csw.tryExec(success, data);
                        },
                        error: function (data) {
                            cswPrivateVar.validationFailed();
                            Csw.tryExec(error, data);
                        }
                    });
                }
            };

            //Step 1. Select an Inspection Target.
            cswPrivateVar.makeStepOne = (function () {
                var stepOneComplete = false,
                    inspectionTable, addBtn, rowOneTable;
                return function () {

                    var onNodeTypeSelectSuccess = function (data) {
                        //If the picklist is empty, we have to add a new Target
                        if (data.nodetypecount === 0) {
                            cswPrivateVar.inspectionTargetSelect.hide();
                            cswPrivateVar.isNewTarget(true);
                            cswPrivateVar.addNewTarget = rowOneTable.cell(2, 2);
                            cswPrivateVar.addNewTarget.css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .input({
                                    suffix: 'newTargetName',
                                    value: '',
                                    maxlength: 40
                                })
                                .propNonDom('maxlength', 40)
                                .$.keypress(function () {
                                    setTimeout(function () {
                                        var newTargetName = cswPrivateVar.addNewTarget.val();
                                        if (false === Csw.isNullOrEmpty(newTargetName)) {
                                            cswPrivateVar.wizard.next.enable();
                                        }
                                    }, 100);
                                });
                        } else { //Select an existing Target or add a new Target
                            cswPrivateVar.selectedInspectionTarget = cswPrivateVar.inspectionTargetSelect.find(':selected').text();
                            cswPrivateVar.wizard.next.enable();

                            addBtn = addBtn || rowOneTable.cell(2, 3);
                            addBtn.css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .div()
                                .button({
                                    ID: cswPrivateVar.makeStepId('addNewInspectionTarget'),
                                    enabledText: 'Add New',
                                    disableOnClick: false,
                                    onClick: function () {
                                        $.CswDialog('AddNodeTypeDialog', {
                                            objectclassid: cswPrivateVar.inspectionTargetSelect.find(':selected').data('objectClassId'),
                                            nodetypename: '',
                                            category: 'do not show',
                                            select: cswPrivateVar.inspectionTargetSelect,
                                            nodeTypeDescriptor: 'Target',
                                            maxlength: 40,
                                            onSuccess: function (newData) {
                                                var proposedInspectionTarget = newData.nodetypename;
                                                if (cswPrivateVar.checkTargetIsClientSideUnique(proposedInspectionTarget)) {
                                                    cswPrivateVar.selectedInspectionTarget = proposedInspectionTarget;
                                                    cswPrivateVar.isNewTarget(true);
                                                    cswPrivateVar.wizard.next.enable();
                                                    Csw.publish(cswPrivateVar.createInspectionEvents.targetNameChanged);
                                                } else {
                                                    cswPrivateVar.inspectionTargetSelect.find('option[value="' + proposedInspectionTarget + '"]').remove();
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
                        if (false === Csw.isNullOrEmpty(cswPrivateVar.inspectionTargetSelect, true)) {
                            cswPrivateVar.inspectionTargetSelect.remove();
                        }

                        cswPrivateVar.inspectionTargetSelect = rowOneTable.cell(2, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .div()
                            .nodeTypeSelect({
                                ID: cswPrivateVar.makeStepId('nodeTypeSelect'),
                                objectClassName: 'InspectionTargetClass',
                                onSelect: function () {
                                    var selected = cswPrivateVar.inspectionTargetSelect.find(':selected');
                                    cswPrivateVar.isNewTarget(selected.propNonDom('data-newNodeType'));
                                    cswPrivateVar.selectedInspectionTarget = selected.text();
                                    Csw.publish(cswPrivateVar.createInspectionEvents.targetNameChanged);
                                },
                                onSuccess: function (data) {
                                    onNodeTypeSelectSuccess(data);
                                    cswPrivateVar.selectedInspectionTarget = cswPrivateVar.inspectionTargetSelect.find(':selected').text();
                                }
                            });
                    };

                    if (false === stepOneComplete) {
                        cswPrivateVar.divStep1 = cswPrivateVar.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step1.step);
                        cswPrivateVar.divStep1.br();

                        inspectionTable = cswPrivateVar.divStep1.table({
                            ID: cswPrivateVar.makeStepId('setInspectionTargetTable')
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

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, (false === Csw.isNullOrEmpty(cswPrivateVar.selectedInspectionTarget)));
                };
            } ());

            //Step 2. Select an Inspection Design Design.
            cswPrivateVar.makeStepTwo = (function () {
                var stepTwoComplete = false;

                return function () {
                    function makeTempInspectionDesignName(name) {
                        var ret = Csw.string(name).trim();
                        if (-1 === ret.indexOf('Checklist') && -1 === ret.indexOf('checklist')) {
                            ret += ' Checklist';
                        }
                        return ret;
                    }

                    var inspectionTable,
                        tempInspectionName = makeTempInspectionDesignName(cswPrivateVar.selectedInspectionTarget),
                        tempCategoryName = cswPrivateVar.selectedInspectionTarget;

                    var toggleNewDesignName = function () {
                        if (cswPrivateVar.isNewInspectionDesign()) {
                            cswPrivateVar.newDesignName.show();
                        } else {
                            cswPrivateVar.newDesignName.hide();
                        }
                    };
                    var nextBtnEnabled = function () {
                        return (false === Csw.isNullOrEmpty(cswPrivateVar.selectedInspectionDesign.name));
                    };

                    var targetChangedHandle = function () {
                        cswPrivateVar.newDesignName.val(cswPrivateVar.selectedInspectionTarget);
                        cswPrivateVar.categoryNameInput.val(cswPrivateVar.selectedInspectionTarget);
                        if (cswPrivateVar.isNewInspectionDesign()) {
                            cswPrivateVar.selectedInspectionDesign.name = cswPrivateVar.selectedInspectionTarget;
                        }
                        Csw.publish(cswPrivateVar.createInspectionEvents.designNameChanged);
                    };

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());

                    if (false === stepTwoComplete) {
                        cswPrivateVar.divStep2 = cswPrivateVar.divStep2 || cswPrivateVar.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step2.step);
                        cswPrivateVar.divStep2.empty();
                        cswPrivateVar.divStep2.br();

                        Csw.subscribe(cswPrivateVar.createInspectionEvents.targetNameChanged, targetChangedHandle);

                        inspectionTable = cswPrivateVar.divStep2.table({
                            ID: cswPrivateVar.makeStepId('inspectionTable'),
                            FirstCellRightAlign: true
                        });

                        //1. Copy from Inspection Design
                        inspectionTable.cell(1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'Select an Inspection Design&nbsp' });

                        cswPrivateVar.inspectionDesignSelect = inspectionTable.cell(1, 2)
                            .div()
                            .nodeTypeSelect({
                                ID: Csw.makeSafeId('nodeTypeSelect'),
                                objectClassName: 'InspectionDesignClass',
                                blankOptionText: '[Create New]'
                            })
                            .change(function () {
                                var selected = cswPrivateVar.inspectionDesignSelect.find(':selected');
                                cswPrivateVar.selectedInspectionDesign.id = selected.val();
                                if (cswPrivateVar.isNewInspectionDesign() && cswPrivateVar.newDesignName && false === Csw.isNullOrEmpty(cswPrivateVar.newDesignName.val())) {
                                    cswPrivateVar.selectedInspectionDesign.name = cswPrivateVar.newDesignName.val();
                                } else {
                                    cswPrivateVar.selectedInspectionDesign.name = selected.text();
                                }
                                Csw.publish(cswPrivateVar.createInspectionEvents.designNameChanged);
                                toggleNewDesignName();
                            });
                        //Create New is selected by default
                        cswPrivateVar.selectedInspectionDesign.id = cswPrivateVar.inspectionDesignSelect.val();
                        cswPrivateVar.selectedInspectionDesign.name = tempInspectionName;

                        inspectionTable.cell(2, 1).br();

                        //2. New Inspection Design Name
                        cswPrivateVar.newDesignName = inspectionTable.cell(3, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .input({
                                ID: cswPrivateVar.ID + '_newDesignName',
                                type: Csw.enums.inputTypes.text,
                                cssclass: 'required',
                                maxlength: 50,
                                width: (50 * 7) + 'px',
                                value: tempInspectionName
                            })
                            .bind('change keypress keydown keyup', function () {
                                setTimeout(function () {
                                    var newInspectionDesignName = cswPrivateVar.newDesignName.val();
                                    cswPrivateVar.selectedInspectionDesign.id = '[Create New]';
                                    cswPrivateVar.selectedInspectionDesign.name = newInspectionDesignName;
                                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());
                                    Csw.publish(cswPrivateVar.createInspectionEvents.designNameChanged);
                                }, 10);
                            });
                            
                        inspectionTable.cell(5, 1).br();

                        //2. Category Name
                        inspectionTable.cell(6, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .span({ text: 'Category Name&nbsp' });

                        cswPrivateVar.categoryNameInput = cswPrivateVar.categoryNameInput ||
                            inspectionTable.cell(6, 2)
                                .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .input({
                                    ID: cswPrivateVar.ID + '_newDesignCategory',
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
            
            //File upload onSuccess event to prep Step 4
            cswPrivateVar.makeInspectionDesignGrid = function (jqGridOpts, onSuccess) {
                Csw.tryExec(onSuccess);
                cswPrivateVar.gridIsPopulated = true;

                //This is ugly. Abstract the step div from this function.
                cswPrivateVar.divStep4.empty();
                var previewGridId = cswPrivateVar.makeStepId('previewGrid_outer', 4);

                var helpText = cswPrivateVar.divStep4.p({ text: '<p>Review the <b>' + cswPrivateVar.selectedInspectionDesign.name + '</b> upload results. Make any necessary edits.' });

                var designChangeHandle = function () {
                    helpText.remove();
                    helpText = cswPrivateVar.divStep4.p({ text: '<p>Review the <b>' + cswPrivateVar.selectedInspectionDesign.name + '</b> upload results. Make any necessary edits.' });
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true);
                };
                Csw.subscribe(cswPrivateVar.createInspectionEvents.designNameChanged, designChangeHandle);

                cswPrivateVar.inspectionGridDiv = cswPrivateVar.divStep4.div({ ID: previewGridId });
                
                cswPrivateVar.gridOptions = {
                    ID: cswPrivateVar.makeStepId('previewGrid'),
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
                            return cswPrivateVar.inspectionGrid.gridTable.$.jqGrid('editGridRow', rowid, { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterEdit: true });
                        },
                        addfunc: function () {
                            return cswPrivateVar.inspectionGrid.gridTable.$.jqGrid('editGridRow', 'new', { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterAdd: true });
                        },
                        delfunc: function (rowid) {
                            return cswPrivateVar.inspectionGrid.gridTable.$.jqGrid('delRowData', rowid);
                        }
                    }
                };

                if (false === Csw.contains(jqGridOpts, 'data') ||
                    false === Csw.contains(jqGridOpts, 'colNames') ||
                        jqGridOpts.colNames.length === 0) {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Inspection Design upload failed. Please check your design and try again.'));
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true, true);
                } else {
                    $.extend(cswPrivateVar.gridOptions.gridOpts, jqGridOpts);
                }
                cswPrivateVar.inspectionGrid = cswPrivateVar.inspectionGridDiv.grid(cswPrivateVar.gridOptions);
            };

            //File upload button for Step 3
            cswPrivateVar.makeInspectionDesignUpload = function (control) {
                var f = {
                    url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                    onSuccess: function () {
                        cswPrivateVar.wizard.next.enable().click();
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
                            cswPrivateVar.makeInspectionDesignGrid(gridData, f.onSuccess);
                        }
                    }
                });
            };

            //If this is a new Design, upload the template. Otherwise skip to step 5.
            cswPrivateVar.makeStepThree = (function () {
                var stepThreeComplete = false;

                return function (forward) {
                    //this is somewhat dundant, but these calls are cheap and it improves readability until we have time to tighten our shot group
                    var nextIsEnabled = function () {
                        return cswPrivateVar.gridIsPopulated || false === cswPrivateVar.isNewInspectionDesign();
                    };
                    var doNextClick = function () {
                        return false === cswPrivateVar.isNewInspectionDesign() && forward;
                    };
                    var doPrevClick = function () {
                        return false === cswPrivateVar.isNewInspectionDesign() && false === Csw.bool(forward);
                    };

                    var doStepThree = function () {
                        var step3List, templateLink, uploadP, helpText;
                        var designChangeHandle = function (help) {

                            helpText.empty();
                            helpText.span({ text: 'Create a new <b>' + cswPrivateVar.selectedInspectionDesign.name + '</b> Design using the Excel template.' })
                                .p()
                                .a({ href: '/NbtWebApp/etc/InspectionDesign.xls', text: 'Download Template' })
                                .$.button();
                        };
                        if (false === stepThreeComplete) {
                            cswPrivateVar.divStep3 = cswPrivateVar.divStep3 || cswPrivateVar.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step3.step);
                            cswPrivateVar.divStep3.empty();

                            //Ordered instructions
                            step3List = cswPrivateVar.divStep3.ol({
                                ID: cswPrivateVar.makeStepId('uploadTemplateList')
                            });

                            //1. Download template

                            helpText = step3List.li();
                            designChangeHandle();
                            Csw.subscribe(cswPrivateVar.createInspectionEvents.designNameChanged, designChangeHandle);

                            //2. Edit the template.
                            step3List.li().span({ text: 'Edit the Inspection template.' });

                            //3. Upload the design
                            uploadP = step3List.li()
                                                .span({ text: 'Upload the completed Inspection Design.' })
                                                .p()
                                                .input({ ID: cswPrivateVar.makeStepId('fileUploadBtn'), type: Csw.enums.inputTypes.file, name: 'fileupload', value: 'Upload' });
                            cswPrivateVar.makeInspectionDesignUpload(uploadP);



                            //$fileUploadBtn.hide();
                            //stepThreeComplete = true;
                        }
                    }; //doStepTwo

                    if (cswPrivateVar.isNewInspectionDesign()) {
                        doStepThree();
                    }

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextIsEnabled(), doNextClick());
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true, doPrevClick());
                };
            } ());

            //Step 4. Review the Design grid.
            cswPrivateVar.makeStepFour = (function () {
                var stepFourComplete = false;
                //We populate this step as the result of the async design upload. Improve the readability of this code when you next visit.
                return function (forward) {
                    var skipStepFour = false;
                    var doNextClick = function () {
                        skipStepFour = (false === cswPrivateVar.isNewInspectionDesign() && forward);
                        return skipStepFour;
                    };
                    var doPrevClick = function () {
                        skipStepFour = (false === cswPrivateVar.isNewInspectionDesign() && false == Csw.bool(forward));
                        return skipStepFour;
                    };

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, true, doNextClick());
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true, doPrevClick());

                    if (false === stepFourComplete &&
                        false === skipStepFour) {
                        cswPrivateVar.divStep4 = cswPrivateVar.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step4.step);
                        stepFourComplete = true;
                    }
                };
            } ());

            //Step 5. Preview and Finish.
            cswPrivateVar.makeStepFive = (function () {

                return function () {
                    var confirmationList, confirmTypesList, confirmViewsList, confirmGridOptions = {};

                    if (cswPrivateVar.checkTargetIsClientSideUnique()) {

                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, false);

                        cswPrivateVar.categoryName = cswPrivateVar.categoryNameInput.val();

                        cswPrivateVar.divStep5 = cswPrivateVar.divStep5 || cswPrivateVar.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step5.step);
                        cswPrivateVar.divStep5.empty();

                        cswPrivateVar.divStep5.p({ text: 'You are about to create the following items. Click Finish to create the design.' });
                        confirmationList = cswPrivateVar.divStep5.ol({
                            ID: cswPrivateVar.makeStepId('confirmationList')
                        });

                        if (cswPrivateVar.isNewInspectionDesign()) {
                            if (cswPrivateVar.gridOptions) {
                                $.extend(true, confirmGridOptions, cswPrivateVar.gridOptions);
                            }

                            confirmGridOptions.ID = cswPrivateVar.makeStepId('confirmGrid');
                            confirmGridOptions.gridOpts.data = cswPrivateVar.inspectionGrid.gridTable.$.jqGrid('getRowData');
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
                                text: 'Creating a new Inspection Design <b>' + cswPrivateVar.selectedInspectionDesign.name + '</b>.'
                            });
                            confirmGridParent.div().grid(confirmGridOptions);
                        } else {
                            confirmationList.li({
                                text: 'Assigning Inspection Design <b>' + cswPrivateVar.selectedInspectionDesign.name + '</b> to Inspection Target <b> ' + cswPrivateVar.selectedInspectionTarget + '</b>.'
                            }).br();
                        }

                        if (cswPrivateVar.isNewInspectionDesign() || cswPrivateVar.isNewTarget()) {
                            confirmTypesList = confirmationList.li({
                                text: 'New Types'
                            })
                                .ul({
                                    ID: cswPrivateVar.makeStepId('confirmationTypes')
                                });

                            if (cswPrivateVar.isNewInspectionDesign()) {
                                confirmTypesList.li({
                                    text: 'New Inspection Design <b>' + cswPrivateVar.selectedInspectionDesign.name + '</b> on Inspection Target <b>' + cswPrivateVar.selectedInspectionTarget + '</b>'
                                });
                            }

                            if (cswPrivateVar.isNewTarget) {
                                confirmTypesList.li({
                                    text: 'New Inspection Target <b>' + cswPrivateVar.selectedInspectionTarget + '</b>'
                                });

                                confirmTypesList.li({
                                    text: 'New Inspection Target Group <b>' + cswPrivateVar.selectedInspectionTarget + ' Group</b>'
                                });
                            }
                        }

                        confirmViewsList = confirmationList.li({
                            text: 'New Views'
                        })
                            .ul({
                                ID: cswPrivateVar.makeStepId('confirmationViews')
                            });
                        confirmViewsList.li({
                            text: '<b>Scheduling, ' + cswPrivateVar.selectedInspectionDesign.name + ': ' + cswPrivateVar.selectedInspectionTarget + '</b>'
                        });
                        confirmViewsList.li({
                            text: '<b>Groups, ' + cswPrivateVar.selectedInspectionDesign.name + ': ' + cswPrivateVar.selectedInspectionTarget + '</b>'
                        });
                        confirmViewsList.li({
                            text: '<b>Inspections, ' + cswPrivateVar.selectedInspectionDesign.name + ': ' + cswPrivateVar.selectedInspectionTarget + '</b>'
                        });
                    } /*else {
                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true, true);
                    }*/
                };
            } ());

            cswPrivateVar.handleNext = function (newStepNo) {
                cswPrivateVar.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_InspectionDesign.step2.step:
                        cswPrivateVar.makeStepTwo();
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step3.step:
                        cswPrivateVar.checkIsNodeTypeNameUnique(cswPrivateVar.selectedInspectionDesign.name);
                        cswPrivateVar.makeStepThree(true); //we're moving forward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step4.step:
                        cswPrivateVar.makeStepFour(true); //we're moving forward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step5.step:
                        cswPrivateVar.makeStepFive();
                        break;
                } // switch(newstepno)
            }; // handleNext()

            cswPrivateVar.handlePrevious = function (newStepNo) {
                cswPrivateVar.currentStepNo = newStepNo;
                switch (newStepNo) {
                    case Csw.enums.wizardSteps_InspectionDesign.step1.step:
                        cswPrivateVar.makeStepOne();
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step2.step:
                        cswPrivateVar.makeStepTwo(); //we're moving backward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step3.step:
                        cswPrivateVar.makeStepThree(false); //we're moving backward
                        break;
                    case Csw.enums.wizardSteps_InspectionDesign.step4.step:
                        cswPrivateVar.makeStepFour(false);
                        break;
                }
            };

            cswPrivateVar.onConfirmFinish = function () {
                var designGrid = '';

                cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, false);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, false);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, false);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, false);                

                if (false === Csw.isNullOrEmpty(cswPrivateVar.inspectionGrid)) {
                    designGrid = JSON.stringify(cswPrivateVar.inspectionGrid.getAllGridRows());
                }

                var jsonData = {
                    DesignGrid: designGrid,
                    InspectionDesignName: Csw.string(cswPrivateVar.selectedInspectionDesign.name),
                    InspectionTargetName: Csw.string(cswPrivateVar.selectedInspectionTarget),
                    IsNewInspection: cswPrivateVar.isNewInspectionDesign(),
                    IsNewTarget: cswPrivateVar.isNewTarget(),
                    Category: Csw.string(cswPrivateVar.categoryName)
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
                                Csw.tryExec(cswPrivateVar.onFinish, viewId);
                            }
                        });

                    },
                    error: function () {
                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                        cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);
                    }
                });
            };

            (function () {

                cswPrivateVar.wizard = Csw.layouts.wizard(cswPublicRet, {
                    ID: Csw.makeId({ ID: cswPrivateVar.ID, suffix: 'wizard' }),
                    Title: 'Create New Inspection',
                    StepCount: Csw.enums.wizardSteps_InspectionDesign.stepcount,
                    Steps: cswPrivateVar.wizardSteps,
                    StartingStep: cswPrivateVar.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivateVar.handleNext,
                    onPrevious: cswPrivateVar.handlePrevious,
                    onCancel: cswPrivateVar.onCancel,
                    onFinish: cswPrivateVar.onConfirmFinish,
                    doNextOnInit: false
                });

                cswPrivateVar.makeStepOne();
            } ());

            return cswPublicRet;
        });
} ());

