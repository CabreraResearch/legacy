///// <reference path="~/js/CswCommon-vsdoc.js" />
///// <reference path="~/js/CswNbt-vsdoc.js" />

//(function ($) {
//    "use strict";
//    $.fn.CswInspectionDesign = function (options) {

//        //#region Variable Declaration
//        var o = {
//            ID: 'cswInspectionDesignWizard',
//            onCancel: null, //function ($wizard) {},
//            onFinish: null, //function ($wizard) {},
//            startingStep: 1
//        };
//        if (options) $.extend(o, options);

//        var wizardSteps = {
//            1: Csw.enums.wizardSteps_InspectionDesign.step1.description,
//            2: Csw.enums.wizardSteps_InspectionDesign.step2.description,
//            3: Csw.enums.wizardSteps_InspectionDesign.step3.description,
//            4: Csw.enums.wizardSteps_InspectionDesign.step4.description,
//            5: Csw.enums.wizardSteps_InspectionDesign.step5.description
//        };

//        var $parent = $(this),
//            $div = $('<div></div>').appendTo($parent),
//            $wizard,
//            currentStepNo = o.startingStep,
//            buttons = {
//                next: 'next',
//                prev: 'previous',
//                finish: 'finish',
//                cancel: 'cancel'
//            },

//        // Step 1 - Select or Create Inspection Target
//            $divStep1, selectedInspectionTarget, inspectionTargetSelect, addNewTarget, categoryNameInput,
//        // Step 2 - Select or Create Inspection Design
//            selectedInspectionDesign = { id: '[Create New]', name: '[Create New]' },
//            $divStep2, categoryName, inspectionDesignSelect, newDesignName,
//        // Step 3 - Upload Inspection Design
//            $divStep3, gridIsPopulated = false,
//        // Step 4 - Review and Revise Inspection Design
//            $divStep4, inspectionGrid, gridOptions,

//        // Step 5 - Review and Finish
//            $divStep5, //inspectionTargetGroups = { }, newSchedules = { }, $scheduleList,

//            isNewInspectionDesign = function () {
//                return ('[Create New]' === selectedInspectionDesign.id);
//            },

//            isNewTarget = (function () {
//                var ret = false;
//                return function (isNew) {
//                    if (arguments.length > 0) {
//                        ret = Csw.bool(isNew);
//                    }
//                    return ret;
//                };
//            } ()),

//            createInspectionEvents = {
//                targetNameChanged: 'targetNameChanged',
//                designNameChanged: 'designNameChanged'
//            },

//            toggleButton = function (button, isEnabled, doClick) {
//                var $btn;
//                if (Csw.bool(isEnabled)) {
//                    $btn = $wizard.CswWizard('button', button, 'enable');
//                    if (Csw.bool(doClick)) {
//                        $btn.click();
//                    }
//                } else {
//                    $wizard.CswWizard('button', button, 'disable');
//                }
//                if (button !== buttons.finish) {
//                    toggleButton(buttons.finish, (currentStepNo === 5));
//                }

//                return false;
//            },

//            makeStepId = function (suffix, stepNo) {
//                var step = stepNo || currentStepNo;
//                return Csw.makeId({ prefix: 'step_' + step, ID: o.ID, suffix: suffix });
//            },

//        //Step 1. Select an Inspection Target.
//            makeStepOne = (function () {
//                var stepOneComplete = false,
//                    inspectionTable, addBtn, rowOneTable;
//                return function () {

//                    var onNodeTypeSelectSuccess = function (data) {
//                        //If the picklist is empty, we have to add a new Target
//                        if (data.nodetypecount === 0) {
//                            inspectionTargetSelect.hide();
//                            isNewTarget(true);
//                            addNewTarget = rowOneTable.cell(2, 2);
//                            addNewTarget.css({ 'padding': '1px', 'vertical-align': 'middle' })
//                                .input({
//                                    ID: o.ID + '_newTargetName',
//                                    value: '',
//                                    maxlength: 40
//                                })
//                                .propNonDom('maxlength', 40)
//                                .$.keypress(function () {
//                                    setTimeout(function () {
//                                        var newTargetName = addNewTarget.val();
//                                        if (false === Csw.isNullOrEmpty(newTargetName)) {
//                                            $wizard.CswWizard('button', 'next', 'enable');
//                                        }
//                                    }, 100);
//                                });
//                        } else { //Select an existing Target or add a new Target
//                            selectedInspectionTarget = inspectionTargetSelect.find(':selected').text();
//                            $wizard.CswWizard('button', 'next', 'enable');

//                            addBtn = addBtn || rowOneTable.cell(2, 3);
//                            addBtn.css({ 'padding': '1px', 'vertical-align': 'middle' })
//                                    .div()
//                                    .button({
//                                        ID: makeStepId('addNewInspectionTarget'),
//                                        enabledText: 'Add New',
//                                        disableOnClick: false,
//                                        onClick: function () {
//                                            $.CswDialog('AddNodeTypeDialog', {
//                                                objectclassid: inspectionTargetSelect.find(':selected').data('objectClassId'),
//                                                nodetypename: '',
//                                                category: 'do not show',
//                                                select: inspectionTargetSelect,
//                                                nodeTypeDescriptor: 'Target',
//                                                maxlength: 40,
//                                                onSuccess: function (newData) {
//                                                    var proposedInspectionTarget = newData.nodetypename;
//                                                    if (checkTargetIsClientSideUnique(proposedInspectionTarget)) {
//                                                        selectedInspectionTarget = proposedInspectionTarget;
//                                                        isNewTarget(true);
//                                                        $wizard.CswWizard('button', 'next', 'enable');
//                                                        Csw.publish(createInspectionEvents.targetNameChanged);
//                                                    } else {
//                                                        inspectionTargetSelect.find('option[value="' + proposedInspectionTarget + '"]').remove();
//                                                    }
//                                                },
//                                                title: 'Create a New Inspection Target Type.'
//                                            });
//                                            return false;
//                                        }
//                                    });
//                        } // else
//                    };

//                    var makeTargetSelect = function () {
//                        //Normally this would be written as $inspectionTarget = $inspectionTarget || ...
//                        //However, the variable assignment is sufficiently complex that this deviation is justified.
//                        if (false === Csw.isNullOrEmpty(inspectionTargetSelect, true)) {
//                            inspectionTargetSelect.remove();
//                        }

//                        inspectionTargetSelect = rowOneTable.cell(2, 1)
//                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                            .div()
//                            .nodeTypeSelect({
//                                ID: makeStepId('nodeTypeSelect'),
//                                objectClassName: 'InspectionTargetClass',
//                                onSelect: function () {
//                                    var selected = inspectionTargetSelect.find(':selected');
//                                    isNewTarget(selected.propNonDom('data-newNodeType'));
//                                    selectedInspectionTarget = selected.text();
//                                    Csw.publish(createInspectionEvents.targetNameChanged);
//                                },
//                                onSuccess: function (data) {
//                                    onNodeTypeSelectSuccess(data);
//                                    selectedInspectionTarget = inspectionTargetSelect.find(':selected').text();
//                                }
//                            });
//                    };

//                    if (false === stepOneComplete) {
//                        $divStep1 = $wizard.CswWizard('div', Csw.enums.wizardSteps_InspectionDesign.step1.step);
//                        $divStep1.append('<br />');

//                        inspectionTable = Csw.literals.table({
//                            $parent: $divStep1,
//                            ID: makeStepId('setInspectionTargetTable')
//                        });

//                        rowOneTable = Csw.literals.table({
//                            $parent: inspectionTable.cell(1, 1).$,
//                            FirstCellRightAlign: true
//                        });

//                        rowOneTable.cell(1, 1)
//                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                            .span({ text: 'What do you want to inspect?' });

//                        makeTargetSelect();
//                        stepOneComplete = true;
//                    } // if (false === stepOneComplete)

//                    toggleButton(buttons.prev, false);
//                    toggleButton(buttons.next, (false === Csw.isNullOrEmpty(selectedInspectionTarget)));
//                };
//            } ()),

//        //Step 2. Select an Inspection Design Design.
//            makeStepTwo = (function () {
//                var stepTwoComplete = false;

//                return function () {
//                    var inspectionTable, $newDesignLabel, newDesignNameDisplay,
//                        tempInspectionName = selectedInspectionTarget + ' Inspection',
//                        tempCategoryName = selectedInspectionTarget;

//                    var makeInspectionDesignName = function (name) {
//                        var ret = Csw.string(name).trim();
//                        if (-1 === ret.indexOf('Inspection') && -1 === ret.indexOf('inspection')) {
//                            ret += ' Inspection';
//                        }
//                        return ret;
//                    };

//                    var toggleNewDesignName = function () {
//                        if (isNewInspectionDesign()) {
//                            newDesignName.$.show();
//                            $newDesignLabel.show();
//                            newDesignNameDisplay.show();
//                        } else {
//                            newDesignName.$.hide();
//                            $newDesignLabel.hide();
//                            newDesignNameDisplay.hide();
//                        }
//                    };
//                    var nextBtnEnabled = function () {
//                        return (false === Csw.isNullOrEmpty(selectedInspectionDesign.name));
//                    };

//                    var targetChangedHandle = function () {
//                        newDesignName.val(selectedInspectionTarget + ' Inspection');
//                        newDesignNameDisplay.text(selectedInspectionTarget + ' Inspection');
//                        categoryNameInput.val(selectedInspectionTarget);
//                        if (isNewInspectionDesign()) {
//                            selectedInspectionDesign.name = selectedInspectionTarget + ' Inspection';
//                        }
//                        Csw.publish(createInspectionEvents.designNameChanged);
//                    };

//                    toggleButton(buttons.prev, true);
//                    toggleButton(buttons.next, nextBtnEnabled());

//                    if (false === stepTwoComplete) {
//                        $divStep2 = $divStep2 || $wizard.CswWizard('div', Csw.enums.wizardSteps_InspectionDesign.step2.step);
//                        $divStep2.empty();
//                        $divStep2.append('<br />');

//                        Csw.subscribe(createInspectionEvents.targetNameChanged, targetChangedHandle);

//                        inspectionTable = Csw.literals.table({
//                            $parent: $divStep2,
//                            ID: makeStepId('inspectionTable'),
//                            FirstCellRightAlign: true
//                        });

//                        //1. Copy from Inspection Design
//                        inspectionTable.cell(1, 1)
//                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                            .span({ text: 'Select an Inspection Design&nbsp' });

//                        inspectionDesignSelect = inspectionTable.cell(1, 2);
//                        inspectionDesignSelect.div()
//                            .nodeTypeSelect({
//                                ID: Csw.makeSafeId('nodeTypeSelect'),
//                                objectClassName: 'InspectionDesignClass',
//                                addNewOption: true
//                            })
//                            .change(function () {
//                                var $selected = inspectionDesignSelect.find(':selected');
//                                selectedInspectionDesign.id = $selected.val();
//                                if (isNewInspectionDesign() && newDesignName && false === Csw.isNullOrEmpty(newDesignName.val())) {
//                                    selectedInspectionDesign.name = newDesignName.val();
//                                } else {
//                                    selectedInspectionDesign.name = $selected.text();
//                                }
//                                tempCategoryName = selectedInspectionTarget;
//                                categoryNameInput.val(tempCategoryName);
//                                Csw.publish(createInspectionEvents.designNameChanged);
//                                toggleNewDesignName();
//                            });
//                        //Create New is selected by default
//                        selectedInspectionDesign.id = inspectionDesignSelect.find(':selected').val();
//                        selectedInspectionDesign.name = makeInspectionDesignName(selectedInspectionTarget);

//                        inspectionTable.cell(2, 1).br();

//                        //2. New Inspection Design Name
//                        $newDesignLabel = inspectionTable.cell(3, 1)
//                                                         .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                                                         .span({ cssclass: 'required', text: 'New Inspection Design Name&nbsp' });

//                        newDesignName = inspectionTable.cell(3, 2)
//                                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                                            .input({
//                                                ID: o.ID + '_newDesignName',
//                                                type: Csw.enums.inputTypes.text,
//                                                cssclass: 'required',
//                                                maxlength: 50,
//                                                width: (50 * 7) + 'px',
//                                                value: tempInspectionName
//                                            })
//                            .bind('change keypress keydown keyup', function () {
//                                setTimeout(function () {
//                                    var newInspectionDesignName = makeInspectionDesignName(newDesignName.val());
//                                    selectedInspectionDesign.id = '[Create New]';
//                                    selectedInspectionDesign.name = newInspectionDesignName;
//                                    newDesignNameDisplay.text(newInspectionDesignName);
//                                    toggleButton(buttons.next, nextBtnEnabled());
//                                    Csw.publish(createInspectionEvents.designNameChanged);
//                                }, 10);
//                            });

//                        newDesignNameDisplay = inspectionTable.cell(4, 2)
//                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                            .span({ text: tempInspectionName });

//                        inspectionTable.cell(5, 1).br();

//                        //2. Category Name
//                        inspectionTable.cell(6, 1)
//                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                            .span({ text: 'Category Name&nbsp' });

//                        categoryNameInput = categoryNameInput ||
//                            inspectionTable.cell(6, 2)
//                                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
//                                            .input({
//                                                ID: o.ID + '_newDesignCategory',
//                                                type: Csw.enums.inputTypes.text,
//                                                value: tempCategoryName,
//                                                maxlength: 40,
//                                                width: (40 * 7) + 'px'
//                                            });

//                        toggleNewDesignName();

//                        inspectionTable.cell(6, 1).br();
//                    }
//                    stepTwoComplete = true;
//                };
//            } ()),

//            checkIsNodeTypeNameUnique = function (name, success, error) {
//                Csw.ajax.post({
//                    url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
//                    async: false,
//                    data: { 'NodeTypeName': name },
//                    success: function (data) {
//                        Csw.tryExec(success, data);
//                    },
//                    error: function (data) {
//                        Csw.tryExec(error, data);
//                        toggleButton(buttons.next);
//                        toggleButton(buttons.prev, true);
//                    }
//                });
//            },

//        //File upload onSuccess event to prep Step 4
//            makeInspectionDesignGrid = function (jqGridOpts, onSuccess) {
//                Csw.tryExec(onSuccess);
//                gridIsPopulated = true;

//                //This is ugly. Abstract the step div from this function.
//                $divStep4.empty();
//                var previewGridId = makeStepId('previewGrid_outer', 4),
//                    $previewGrid = $divStep4.find('#' + previewGridId);
//                var $helpText = $('<p>Review the <b>' + selectedInspectionDesign.name + '</b> upload results. Make any necessary edits.</p>')
//                                    .appendTo($divStep4);

//                var designChangeHandle = function () {
//                    $helpText.html('<p>Review the <b>' + selectedInspectionDesign.name + '</b> upload results. Make any necessary edits.</p>');
//                };
//                Csw.subscribe(createInspectionEvents.designNameChanged, designChangeHandle);

//                if (Csw.isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
//                    $previewGrid = $('<div id="' + previewGridId + '"></div>').appendTo($divStep4);
//                } else {
//                    $previewGrid.empty();
//                }
//                var preview = Csw.literals.factory($previewGrid);

//                gridOptions = {
//                    $parent: $previewGrid,
//                    ID: makeStepId('previewGrid'),
//                    pagermode: 'default',
//                    gridOpts: {
//                        autowidth: true,
//                        height: '200'
//                    },
//                    optNav: {
//                        add: true,
//                        del: true,
//                        edit: true,
//                        view: false,
//                        editfunc: function (rowid) {
//                            return inspectionGrid.gridTable.$.jqGrid('editGridRow', rowid, { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterEdit: true });
//                        },
//                        addfunc: function () {
//                            return inspectionGrid.gridTable.$.jqGrid('editGridRow', 'new', { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterAdd: true });
//                        },
//                        delfunc: function (rowid) {
//                            return inspectionGrid.gridTable.$.jqGrid('delRowData', rowid);
//                        }
//                    }
//                };

//                if (false === Csw.contains(jqGridOpts, 'data') ||
//                   false === Csw.contains(jqGridOpts, 'colNames') ||
//                   jqGridOpts.colNames.length === 0) {
//                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Inspection Design upload failed. Please check your design and try again.'));
//                    toggleButton(buttons.next, false);
//                    toggleButton(buttons.prev, true, true);
//                } else {
//                    $.extend(gridOptions.gridOpts, jqGridOpts);
//                }
//                inspectionGrid = preview.grid(gridOptions);
//            },

//        //File upload button for Step 3
//            makeInspectionDesignUpload = function ($control) {
//                var f = {
//                    url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
//                    onSuccess: function () {
//                        $wizard.CswWizard('button', 'next', 'enable').click();
//                    },
//                    stepNo: Csw.enums.wizardSteps_InspectionDesign.step3.step,
//                    uploadName: 'design'
//                };

//                $control.fileupload({
//                    datatype: 'json',
//                    dataType: 'json',
//                    url: f.url,
//                    paramName: 'fileupload',
//                    done: function (e, ret) {
//                        var gridData = {};
//                        if (Csw.contains(ret, 'result') && Csw.contains(ret.result, 'jqGridOpt')) {
//                            gridData = ret.result.jqGridOpt;
//                            makeInspectionDesignGrid(gridData, f.onSuccess);
//                        }
//                    }
//                });
//            },

//        //If this is a new Design, upload the template. Otherwise skip to step 5.
//            makeStepThree = (function () {
//                var stepThreeComplete = false;

//                return function (forward) {
//                    //this is somewhat dundant, but these calls are cheap and it improves readability until we have time to tighten our shot group
//                    var nextIsEnabled = function () {
//                        return gridIsPopulated || false === isNewInspectionDesign();
//                    };
//                    var doNextClick = function () {
//                        return false === isNewInspectionDesign() && forward;
//                    };
//                    var doPrevClick = function () {
//                        return false === isNewInspectionDesign() && false === Csw.bool(forward);
//                    };

//                    var doStepThree = function () {
//                        var $step3List, $templateLink, $uploadP, $helpText;
//                        var designChangeHandle = function () {
//                            $helpText.empty();
//                            $helpText.html($('<span>Create a new <b>' + selectedInspectionDesign.name + '</b> Design using the Excel template.</span>')
//                                    .append($('<p/>')
//                                    .append($templateLink)));
//                        };
//                        if (false === stepThreeComplete) {
//                            $divStep3 = $divStep3 || $wizard.CswWizard('div', Csw.enums.wizardSteps_InspectionDesign.step3.step);
//                            $divStep3.empty();

//                            //Ordered instructions
//                            $step3List = $divStep3.CswList('init', {
//                                ID: makeStepId('uploadTemplateList'),
//                                ordered: true
//                            });

//                            //1. Download template
//                            $templateLink = $('<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download Template</a>').button();
//                            $helpText = $('<span>Create a new <b>' + selectedInspectionDesign.name + '</b> Design using the Excel template.</span>')
//                                            .append($('<p/>')
//                                            .append($templateLink));
//                            Csw.subscribe(createInspectionEvents.designNameChanged, designChangeHandle);
//                            $step3List.CswList('addItem', {
//                                value: $helpText
//                            });

//                            //2. Edit the template.
//                            $step3List.CswList('addItem', {
//                                value: $('<span>Edit the Inspection template.</span>')
//                            });

//                            //3. Upload the design
//                            $uploadP = $('<input id="' + makeStepId('fileUploadBtn') + '" type="file" name="fileupload" />');
//                            makeInspectionDesignUpload($uploadP);

//                            $step3List.CswList('addItem', {
//                                value: $('<span>Upload the completed Inspection Design.<p/></span>').append($uploadP)
//                            });

//                            //$fileUploadBtn.hide();
//                            //stepThreeComplete = true;
//                        }
//                    }; //doStepTwo

//                    if (isNewInspectionDesign()) {
//                        //selectedInspectionDesign.name = $newDesignName.val();
//                        checkIsNodeTypeNameUnique(selectedInspectionDesign.name, doStepThree);
//                    }
//                    toggleButton(buttons.next, nextIsEnabled(), doNextClick());
//                    toggleButton(buttons.prev, true, doPrevClick());
//                };
//            } ()),

//        //Step 4. Review the Design grid.
//            makeStepFour = (function () {
//                var stepFourComplete = false;
//                //We populate this step as the result of the async design upload. Improve the readability of this code when you next visit.
//                return function (forward) {
//                    var skipStepFour = false;
//                    var doNextClick = function () {
//                        skipStepFour = (false === isNewInspectionDesign() && forward);
//                        return skipStepFour;
//                    };
//                    var doPrevClick = function () {
//                        skipStepFour = (false === isNewInspectionDesign() && false == Csw.bool(forward));
//                        return skipStepFour;
//                    };

//                    toggleButton(buttons.next, true, doNextClick());
//                    toggleButton(buttons.prev, true, doPrevClick());

//                    if (false === stepFourComplete &&
//                            false === skipStepFour) {
//                        $divStep4 = $wizard.CswWizard('div', Csw.enums.wizardSteps_InspectionDesign.step4.step);
//                        stepFourComplete = true;
//                    }
//                };
//            } ()),

//            checkTargetIsClientSideUnique = function (proposedTargetName) {
//                var ret = false,
//                    targetName = proposedTargetName || selectedInspectionTarget;
//                if (Csw.string(targetName).trim().toLowerCase() != Csw.string(selectedInspectionDesign.name).trim().toLowerCase()) {
//                    ret = true;
//                } else {
//                    $.CswDialog('ErrorDialog', Csw.error.makeErrorObj(Csw.enums.errorType.warning.name,
//                        'An Inspection Design and an Inspection Target cannot have the same name.',
//                        'Attempted to create Inspection Target ' + targetName + ' against Inspection Design ' + selectedInspectionDesign.name));
//                }
//                return ret;
//            },

//        //Step 5. Preview and Finish.
//            makeStepFive = (function () {

//                return function () {
//                    var $confirmationList, $confirmTypesList, $confirmViewsList, confirmGridOptions = {};

//                    if (checkTargetIsClientSideUnique()) {

//                        toggleButton(buttons.prev, true);
//                        toggleButton(buttons.next, false);

//                        categoryName = categoryNameInput.val();

//                        $divStep5 = $divStep5 || $wizard.CswWizard('div', Csw.enums.wizardSteps_InspectionDesign.step5.step);
//                        $divStep5.empty();

//                        $divStep5.append('<p>You are about to create the following items. Click Finish to create the design.</p>');
//                        $confirmationList = $divStep5.CswList('init', {
//                            ID: makeStepId('confirmationList'),
//                            ordered: true
//                        });

//                        if (isNewInspectionDesign()) {
//                            if (gridOptions) {
//                                $.extend(true, confirmGridOptions, gridOptions);
//                            }

//                            confirmGridOptions.ID = makeStepId('confirmGrid');
//                            confirmGridOptions.gridOpts.data = inspectionGrid.gridTable.$.jqGrid('getRowData');
//                            confirmGridOptions.gridOpts.autowidth = false;
//                            confirmGridOptions.gridOpts.shrinkToFit = true;
//                            confirmGridOptions.gridOpts.height = 150;
//                            confirmGridOptions.optNav.add = false;
//                            confirmGridOptions.optNav.del = false;
//                            confirmGridOptions.optNav.edit = false;
//                            confirmGridOptions.optNav.view = false;
//                            confirmGridOptions.optNav.editfunc = null;
//                            confirmGridOptions.optNav.addfunc = null;
//                            confirmGridOptions.optNav.delfunc = null;
//                            Csw.each(confirmGridOptions.gridOpts.colModel, function (col) {
//                                if (Csw.contains(col, 'editable')) {
//                                    delete col.editable;
//                                }
//                                if (Csw.contains(col, 'edittype')) {
//                                    delete col.edittype;
//                                }
//                            });

//                            var $confirmGridParent = $confirmationList.CswList('addItem', {
//                                value: 'Creating a new Inspection Design <b>' + selectedInspectionDesign.name + '</b>.'
//                            });
//                            var gridParent = Csw.literals.factory($confirmGridParent);
//                            gridParent.grid(confirmGridOptions);
//                        } else {
//                            $confirmationList.CswList('addItem', {
//                                value: 'Assigning Inspection Design <b>' + selectedInspectionDesign.name + '</b> to Inspection Target <b> ' + selectedInspectionTarget + '</b>.'
//                            });
//                        }

//                        if (isNewInspectionDesign() || isNewTarget()) {
//                            $confirmTypesList = $confirmationList.CswList('addItem', {
//                                value: 'New Types'
//                            })
//                                .CswList('init', {
//                                    ID: makeStepId('confirmationTypes')
//                                });

//                            if (isNewInspectionDesign()) {
//                                $confirmTypesList.CswList('addItem', {
//                                    value: 'New Inspection Design <b>' + selectedInspectionDesign.name + '</b> on Inspection Target <b>' + selectedInspectionTarget + '</b>'
//                                });
//                            }

//                            if (isNewTarget) {
//                                $confirmTypesList.CswList('addItem', {
//                                    value: 'New Inspection Target <b>' + selectedInspectionTarget + '</b>'
//                                });

//                                $confirmTypesList.CswList('addItem', {
//                                    value: 'New Inspection Target Group <b>' + selectedInspectionTarget + ' Group</b>'
//                                });
//                            }
//                        }

//                        $confirmViewsList = $confirmationList.CswList('addItem', {
//                            value: 'New Views'
//                        })
//                            .CswList('init', {
//                                ID: makeStepId('confirmationViews')
//                            });
//                        $confirmViewsList.CswList('addItem', {
//                            value: '<b>Scheduling, ' + selectedInspectionDesign.name + ': ' + selectedInspectionTarget + '</b>'
//                        });
//                        $confirmViewsList.CswList('addItem', {
//                            value: '<b>Groups, ' + selectedInspectionDesign.name + ': ' + selectedInspectionTarget + '</b>'
//                        });
//                        $confirmViewsList.CswList('addItem', {
//                            value: '<b>Inspections, ' + selectedInspectionDesign.name + ': ' + selectedInspectionTarget + '</b>'
//                        });
//                    } else {
//                        toggleButton(buttons.prev, true, true);
//                    }
//                };
//            } ()),

//            handleNext = function ($wizardTable, newStepNo) {
//                currentStepNo = newStepNo;
//                switch (newStepNo) {
//                    case Csw.enums.wizardSteps_InspectionDesign.step2.step:
//                        makeStepTwo();
//                        break;
//                    case Csw.enums.wizardSteps_InspectionDesign.step3.step:
//                        makeStepThree(true); //we're moving forward
//                        break;
//                    case Csw.enums.wizardSteps_InspectionDesign.step4.step:
//                        makeStepFour(true); //we're moving forward
//                        break;
//                    case Csw.enums.wizardSteps_InspectionDesign.step5.step:
//                        makeStepFive();
//                        break;
//                } // switch(newstepno)
//            }, // handleNext()

//            handlePrevious = function (newStepNo) {
//                currentStepNo = newStepNo;
//                switch (newStepNo) {
//                    case Csw.enums.wizardSteps_InspectionDesign.step1.step:
//                        makeStepOne();
//                        break;
//                    case Csw.enums.wizardSteps_InspectionDesign.step2.step:
//                        makeStepTwo(); //we're moving backward
//                        break;
//                    case Csw.enums.wizardSteps_InspectionDesign.step3.step:
//                        makeStepThree(false); //we're moving backward
//                        break;
//                    case Csw.enums.wizardSteps_InspectionDesign.step4.step:
//                        makeStepFour(false);
//                        break;
//                }
//            },

//            onFinish = function () {
//                var designGrid = '';

//                toggleButton(buttons.prev, false);
//                toggleButton(buttons.next, false);
//                toggleButton(buttons.finish, false);
//                toggleButton(buttons.cancel, false);

//                if (false === Csw.isNullOrEmpty(inspectionGrid)) {
//                    designGrid = JSON.stringify(inspectionGrid.getAllGridRows());
//                }

//                var jsonData = {
//                    DesignGrid: designGrid,
//                    InspectionDesignName: Csw.string(selectedInspectionDesign.name),
//                    InspectionTargetName: Csw.string(selectedInspectionTarget),
//                    IsNewInspection: isNewInspectionDesign(),
//                    IsNewTarget: isNewTarget(),
//                    Category: Csw.string(categoryName)
//                };

//                Csw.ajax.post({
//                    url: '/NbtWebApp/wsNBT.asmx/finalizeInspectionDesign',
//                    data: jsonData,
//                    success: function (data) {
//                        //Come back and hammer this out
//                        var views = data.views,
//                                values = [];

//                        Csw.each(views, function (thisView) {
//                            if (Csw.contains(thisView, 'viewid') &&
//                                        Csw.contains(thisView, 'viewname')) {
//                                values.push({
//                                    value: thisView.viewid,
//                                    display: thisView.viewname
//                                });
//                            }
//                        });

//                        $.CswDialog('NavigationSelectDialog', {
//                            ID: Csw.makeSafeId('FinishDialog'),
//                            title: 'The Inspection Design Wizard Completed Successfully',
//                            navigationText: 'Please select from the following views. Click OK to continue.',
//                            values: values,
//                            onOkClick: function (selectedView) {
//                                var viewId = selectedView.val();
//                                Csw.tryExec(o.onFinish, viewId);
//                            }
//                        });

//                    },
//                    error: function () {
//                        toggleButton(buttons.cancel, true);
//                        toggleButton(buttons.prev, true);
//                    }
//                });
//            };

//        //#endregion Variable Declaration

//        //#region Execution
//        $wizard = $div.CswWizard('init', {
//            ID: Csw.makeId({ ID: o.ID, suffix: 'wizard' }),
//            Title: 'Create New Inspection',
//            StepCount: Csw.enums.wizardSteps_InspectionDesign.stepcount,
//            Steps: wizardSteps,
//            StartingStep: o.startingStep,
//            FinishText: 'Finish',
//            onNext: handleNext,
//            onPrevious: handlePrevious,
//            onCancel: o.onCancel,
//            onFinish: onFinish,
//            doNextOnInit: false
//        });

//        makeStepOne();
//        //#endregion Execution

//        return $div;
//    }; // $.fn.Csw.enums.wizardSteps_InspectionDesign
//})(jQuery);

////#endregion Archive