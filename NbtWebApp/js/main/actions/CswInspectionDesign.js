/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../controls/CswNodeTypeSelect.js" />
/// <reference path="../pagecmp/CswWizard.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../pagecmp/CswDialog.js" />
/// <reference path="../controls/CswTimeInterval.js" />
/// <reference path="../controls/CswTable.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    $.fn.CswInspectionDesign = function (options) {

        //#region Variable Declaration
        var o = {
            ImportFileUrl: '',
            viewid: '',
            viewname: '',
            viewmode: '',
            ID: 'cswInspectionDesignWizard',
            onCancel: null, //function($wizard) {},
            onFinish: null, //function($wizard) {},
            startingStep: 1
        };
        if (options) $.extend(o, options);

        var wizardSteps = {
            1: ChemSW.enums.CswInspectionDesign_WizardSteps.step1.description,
            2: ChemSW.enums.CswInspectionDesign_WizardSteps.step2.description,
            3: ChemSW.enums.CswInspectionDesign_WizardSteps.step3.description,
            4: ChemSW.enums.CswInspectionDesign_WizardSteps.step4.description,
            5: ChemSW.enums.CswInspectionDesign_WizardSteps.step5.description
        };

        //var currentStep = o.startingStep;

        var $parent = $(this),
            $div = $('<div></div>').appendTo($parent),
            $wizard,
            currentStepNo = 0,
            buttons = {
                next: 'next',
                prev: 'prev',
                finish: 'finish'
            },

        // Step 1 - Select or Create Inspection Design
            selectedInspectionDesign = { id: '[Create New]', name: '[Create New]' },
            $divStep1, newInspectionName, newCategoryName, $inspectionDesignSelect, $newDesignName, $newDesignCategory,
        // Step 2 - Upload Inspection Design
            $divStep2, gridIsPopulated = false,
        // Step 3 - Review and Revise Inspection Design
            $divStep3, inspectionGrid, gridOptions,
        // Step 4 - Select or Create Inspection Target
            $divStep4, selectedInspectionTarget, $inspectionTarget, $addNewTarget, isNewTarget = false,
        // Step 5 - Review and Finish
            $divStep5, //inspectionTargetGroups = { }, newSchedules = { }, $scheduleList,

            isNewInspectionDesign = function() {
                return ('[Create New]' === selectedInspectionDesign.name);
            },
            
            toggleButton = function (button, isEnabled, doClick) {
                var $btn;
                if(isTrue(isEnabled)) {
                    $btn = $wizard.CswWizard('button', button, 'enable');
                    if (isTrue(doClick)) {
                        $btn.click();
                    }
                } else {
                    $btn = $wizard.CswWizard('button', button, 'disable');
                }
                return false;
            },
            
            makeStepId = function (suffix, stepNo) {
                var step = stepNo || currentStepNo;
                return makeId({ prefix: 'step_' + step, ID: o.ID, suffix: suffix });
            },
            
            //Step 1. Name the Design.
            makeStepOne = (function() {
                var stepOneComplete = false;

                return function() {
                    var toggleNewDesignName = function() {
                        if (isNewInspectionDesign()) {
                            $newDesignName.hide();
                        } else {
                            $newDesignName.show();
                        }
                    };
                    var nextBtnEnabled = function() {
                        return ((isNewInspectionDesign() &&
                                    false === isNullOrEmpty(newInspectionName)) ||
                                (false === isNewInspectionDesign() && 
                                    false === isNullOrEmpty(selectedInspectionDesign.name)));
                    };
                    var $inspectionTable;

                    toggleButton(buttons.prev, false);
                    toggleButton(buttons.finish, false);
                    toggleButton(buttons.next, nextBtnEnabled());
                    
                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step1.step);
                        $divStep1.append('<br />');

                        $inspectionTable = $divStep1.CswTable('init', {
                            ID: makeSafeId('inspectionTable'),
                            FirstCellRightAlign: true
                        });

                        //1. Copy from Inspection Design
                        $inspectionTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>Copy from Inspection Design&nbsp</span>');

                        $inspectionDesignSelect = $inspectionTable.CswTable('cell', 1, 2)
                            .CswDiv('init')
                            .CswNodeTypeSelect('init', {
                                ID: makeSafeId('nodeTypeSelect'),
                                objectClassName: 'InspectionDesignClass',
                                addNewOption: true
                            })
                            .change(function() {
                                var $selected = $inspectionDesignSelect.find(':selected');
                                selectedInspectionDesign.id = $selected.val();
                                selectedInspectionDesign.name = $selected.text();
                                toggleNewDesignName();
                            });
                        selectedInspectionDesign.id = $inspectionDesignSelect.find(':selected').val();
                        selectedInspectionDesign.name = $inspectionDesignSelect.find(':selected').text();
                        
                        $inspectionTable.CswTable('cell', 2, 1).append('<br />');

                        //2. New Inspection Design Name
                        $inspectionTable.CswTable('cell', 3, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span class="required">New Inspection Design Name&nbsp</span>');

                        $newDesignName = $inspectionTable.CswTable('cell', 3, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .CswInput('init', {
                                ID: o.ID + '_newDesignName',
                                type: CswInput_Types.text,
                                cssclass: 'required'
                            })
                            .keypress(function() {
                                setTimeout(function() {
                                    newInspectionName = $newDesignName.val();
                                    toggleButton(buttons.next, nextBtnEnabled());
                                }, 100);
                            });

                        toggleNewDesignName();
                        
                        $inspectionTable.CswTable('cell', 4, 1).append('<br />');

                        //3. Category Name
                        $inspectionTable.CswTable('cell', 5, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>Category Name&nbsp</span>');

                        $newDesignCategory = $inspectionTable.CswTable('cell', 5, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .CswInput('init', {
                                ID: o.ID + '_newDesignCategory',
                                type: CswInput_Types.text
                            });
                    }
                    stepOneComplete = true;
                };
            }()),

            checkIsNodeTypeNameUnique = function(name, success, error) {
                CswAjaxJson({
                        url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
                        async: false,
                        data: { 'NodeTypeName': name },
                        success: function(data) {
                            if (isFunction(success)) {
                                success(data);
                            }
                        },
                        error: function(data) {
                            if (isFunction(error)) {
                                error(data);
                            }
                            enablePrev(true);
                            disableNext();
                        }
                    });
            },

            //File upload onSuccess event to prep Step 3
            makeInspectionDesignGrid = function(id, fileName, data, onSuccess) {
                if (isFunction(onSuccess)) {
                    onSuccess();
                }
                gridIsPopulated = true;
                $divStep3.empty();
                var previewGridId = makeStepId('previewGrid_outer', 3),
                    $previewGrid = $divStep2.find('#' + previewGridId);

                $divStep3.append('<p>Inspection Name: ' + newInspectionName + '</p>');
                $divStep3.append("<p>Review the upload results. Make any necessary edits.</p>");

                if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                    $previewGrid = $('<div id="' + previewGridId + '"></div>').appendTo($divStep3);
                } else {
                    $previewGrid.empty();
                }

                gridOptions = {
                    ID: makeStepId('previewGrid'),
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
                        editfunc: function(rowid) {
                            return inspectionGrid.$gridTable.jqGrid('editGridRow', rowid, { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterEdit: true });
                        },
                        addfunc: function() {
                            return inspectionGrid.$gridTable.jqGrid('editGridRow', 'new', { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterAdd: true });
                        },
                        delfunc: function(rowid) {
                            return inspectionGrid.$gridTable.jqGrid('delRowData', rowid);
                        }
                    }
                };

                $.extend(gridOptions.gridOpts, data.jqGridOpt);

                inspectionGrid = new CswGrid(gridOptions, $previewGrid);
            },

            //File upload button for Step 2
            makeInspectionDesignUpload = function($control) {
                var f = {
                    url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                    params: {
                        InspectionName: newInspectionName
                    },
                    onSuccess: function() {
                        $wizard.CswWizard('button', 'next', 'enable').click();
                    },
                    stepNo: ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step,
                    uploadName: 'design'
                };

                var uploadTemplate = '<div class="qq-uploader"><div class="qq-upload-drop-area"><span>Drop ' + f.uploadName + ' here to process</span></div><div class="qq-upload-button">Upload Design</div><ul class="qq-upload-list"></ul></div>';
                var uploader = new qq.FileUploader({
                        element: $control.get(0),
                        multiple: false,
                        action: f.url,
                        template: uploadTemplate,
                        params: f.params,
                        onSubmit: function() {
                            $('.qq-upload-list').empty();
                        },
                        onComplete: function(id, fileName, data) {
                            makeInspectionDesignGrid(id, fileName, data, f.onSuccess);
                        },
                        showMessage: function(error) {
                            $.CswDialog('ErrorDialog', error);
                        }
                    });
            },

            //If this is a new Design, upload the template. Otherwise skip to step 4.
            makeStepTwo = (function() {
                var stepTwoComplete = false;

                return function(forward) {
                    var nextIsEnabled = function() {
                        return gridIsPopulated || false === isNewInspectionDesign();
                    };
                    var doNextClick = function() {
                        return false === isNewInspectionDesign() && forward;
                    };
                    var doPrevClick = function() {
                        return false === isNewInspectionDesign() && false === isTrue(forward);
                    };
                   
                    var doStepTwo = function() {
                        var $fileUploadBtn, $step2List, $templateLink, $uploadP;

                        if (false === stepTwoComplete) {
                            $divStep2 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step2.step);

                            //Ordered instructions
                            $step2List = $divStep2.CswList('init', {
                                ID: makeStepId('uploadTemplateList'),
                                ordered: true
                            });

                            //1. Download template
                            $templateLink = $('<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download Template</a>').button();
                            $step2List.CswList('addItem', {
                                value: $('<span>Create a new <b>' + newInspectionName + '</b> Inspection Design using the Excel template.</span>')
                                    .append($('<p/>').append($templateLink))
                            });

                            //2. Edit the template.
                            $step2List.CswList('addItem', {
                                value: $('<span>Edit the Inspection template.</span>')
                            });

                            //3. Upload the design
                            $uploadP = $('<p/>');
                            $fileUploadBtn = $uploadP;
                            makeInspectionDesignUpload($fileUploadBtn);

                            $step2List.CswList('addItem', {
                                value: $('<span>Upload the completed InspectionDesign.</span>').append($uploadP)
                            });

                            //$fileUploadBtn.hide();
                            stepTwoComplete = true;
                        }
                    }; //doStepTwo

                    newCategoryName = $newDesignCategory.val();
                    if(isNewInspectionDesign()) {
                        newInspectionName = $newDesignName.val();
                        checkIsNodeTypeNameUnique(newInspectionName, doStepTwo);
                    }
                    toggleButton(buttons.next, nextIsEnabled(), doNextClick());
                    toggleButton(buttons.prev, true, doPrevClick());
                };
            }()),

            //Step 3. Review the Design grid.
            makeStepThree = (function() {
                var stepThreeComplete = false;
                return function(forward) {
                    var $prevBtn = $wizard.CswWizard('button', 'previous', 'enable'),
                        $nextBtn = $wizard.CswWizard('button', 'next', 'enable');

                    if (false === isNewInspectionDesign()) {
                        if (forward) {
                            $nextBtn.click();
                        } else {
                            $prevBtn.click();
                        }
                    }
                    else if (false === stepThreeComplete) {
                        $divStep3 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step);
                        stepThreeComplete = true;
                    }
                };
            }()),

            //Step 4. Select a Target.
            makeStepFour = (function() {
                var stepFourComplete = false,
                    lastSelectedInspectionName = selectedInspectionDesign.name;
                return function() {
                    var $inspectionTable;
                    var onNodeTypeSelectSuccess = function(data) {
                        if (data.nodetypecount === 0) { //Add a new Target
                            $inspectionTarget.hide();
                            isNewTarget = true;
                            $addNewTarget = $inspectionTable.CswTable('cell', 1, 3)
                                .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .CswInput('init', {
                                    ID: o.ID + '_newTargetName',
                                    value: ''
                                })
                                .keypress(function() {
                                    setTimeout(function() {
                                        var newTargetName = $addNewTarget.val();
                                        if (false === isNullOrEmpty(newTargetName)) {
                                            $wizard.CswWizard('button', 'next', 'enable');
                                        }
                                    }, 100);
                                });
                        } else { //Select an existing Target or add a new Target
                            selectedInspectionTarget = $inspectionTarget.find(':selected').text();
                            $wizard.CswWizard('button', 'next', 'enable');

                            $inspectionTable.CswTable('cell', 1, 4)
                                .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .CswDiv('init')
                                .CswButton('init', {
                                    ID: makeStepId('addNewInspectionTarget'),
                                    enabledText: 'Add New',
                                    disableOnClick: false,
                                    onclick: function() {
                                        $.CswDialog('AddNodeTypeDialog', {
                                            objectclassid: $inspectionTarget.find(':selected').data('objectClassId'),
                                            nodetypename: '',
                                            category: newCategoryName,
                                            $select: $inspectionTarget,
                                            nodeTypeDescriptor: 'Target',
                                            onSuccess: function(data) {
                                                selectedInspectionTarget = data.nodetypename;
                                                isNewTarget = true;
                                                newCategoryName = data.category;
                                                $wizard.CswWizard('button', 'next', 'enable');
                                            },
                                            title: 'Create a New Inspection Target Type.'
                                        });
                                        return false;
                                    }
                                });
                        } // else
                    };
                    
                    var makeTargetSelect = function () {
                        var excludeNodeTypeId = '';
                        if(false === isNullOrEmpty($inspectionTarget, true)) {
                            $inspectionTarget.remove();
                        }
                        if (isNewInspectionDesign()) {
                            excludeNodeTypeId = selectedInspectionDesign.id;
                        }
                        $inspectionTarget = $inspectionTable.CswTable('cell', 1, 2)
                                                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                                            .CswDiv('init')
                                                            .CswNodeTypeSelect('init', {
                                                                ID: makeStepId('nodeTypeSelect'),
                                                                objectClassName: 'InspectionTargetClass',
                                                                excludeNodeTypeIds: excludeNodeTypeId,
                                                                onSelect: function() {
                                                                    var $this = $inspectionTarget.find(':selected');
                                                                    isNewTarget = isTrue($this.CswAttrXml('data-newNodeType'));
                                                                    selectedInspectionTarget = $this.text();
                                                                },
                                                                onSuccess: function(data) {
                                                                    onNodeTypeSelectSuccess(data);
                                                                }
                        });
                    };
                    
                    $wizard.CswWizard('button', 'previous', 'enable');
                    
                    if (false === isNullOrEmpty(selectedInspectionTarget)) {
                        $wizard.CswWizard('button', 'next', 'enable');
                    } else {
                        $wizard.CswWizard('button', 'next', 'disable');
                    }
                    
                    if (false === stepFourComplete) {
                        $divStep4 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step4.step);
                        $divStep4.append('<br />');

                        $inspectionTable = $divStep4.CswTable('init', {
                            ID: makeStepId('setInspectionTargetTable'),
                            FirstCellRightAlign: true
                        });

                        $inspectionTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>Select an Inspection Target&nbsp</span>');

                        makeTargetSelect();

                        stepFourComplete = true;
                    } 
                    else if(lastSelectedInspectionName !== selectedInspectionDesign) {
                        makeTargetSelect();
                    }
                };
            }()),

//            //Old Step 5. Add schedules.
//            makeStepFive = (function() {
//                var stepFiveComplete = false,
//                    lastSelectedTarget = selectedInspectionTarget;

//                return function() {
//                    var $newScheduleName, $newScheduleInterval, timeInterval, $addBtn, selectedGroup;
//                    
//                    $wizard.CswWizard('button', 'previous', 'enable');
//                    $wizard.CswWizard('button', 'next', 'enable');

//                    if (isNullOrEmpty(selectedInspectionTarget)) {
//                        if (false === isNullOrEmpty($addNewTarget, true)) {
//                            selectedInspectionTarget = tryParseString($addNewTarget.val());
//                            checkIsNodeTypeNameUnique(selectedInspectionTarget);
//                        } else {
//                            selectedInspectionTarget = $inspectionTarget.find(':selected').text();
//                        }
//                    }

//                    if (isNullOrEmpty(selectedInspectionTarget)) {
//                        $wizard.CswWizard('button', 'previous', 'enable').click();
//                        $wizard.CswWizard('button', 'next', 'disable');
//                        $.CswDialog('ErrorDialog', ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'An Inspection Design must have a Target.'));
//                    }

//                    //We can't validate this server-side, because the NodeTypes don't exist (yet)
//                    if (trim(selectedInspectionTarget).toLowerCase() === trim(newInspectionName).toLowerCase()) {
//                        $wizard.CswWizard('button', 'previous', 'enable').click();
//                        $wizard.CswWizard('button', 'next', 'disable');
//                        $.CswDialog('ErrorDialog', ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'An Inspection Target cannot have the same name as an Inspection Design.'));
//                    }

//                    if (false === stepFiveComplete || 
//                            lastSelectedTarget !== selectedInspectionTarget) {
//                        
//                        lastSelectedTarget = selectedInspectionTarget;
//                        $divStep5 = $divStep5 || $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step5.step);
//                        
//                        //Clear anything saved before
//                        $divStep5.empty();
//                        inspectionTargetGroups = { };
//                        newSchedules = { };
//                        
//                        CswAjaxJson({
//                                url: '/NbtWebApp/wsNBT.asmx/getScheduleNodesForInspection',
//                                data: { InspectionTargetName: selectedInspectionTarget,
//                                    CopyInspectionDesignName: copyFromInspectionDesign.name },
//                                success: function(data) {
//                                    var makeTimeInterval = function() {
//                                        $newScheduleInterval = $newScheduleInterval || $addTable.CswTable('cell', 5, 2)
//                                                                                                .CswDiv('init');
//                                        $newScheduleInterval.empty();
//                                        
//                                        timeInterval = CswTimeInterval({
//                                                $parent: $newScheduleInterval,
//                                                propVals: {
//                                                    Interval: {
//                                                        rateintervalvalue: {
//                                                            dateformat: 'M/d/yyyy',
//                                                            ratetype: CswRateIntervalTypes.WeeklyByDay,
//                                                            startingdate: {
//                                                                date: '',
//                                                                dateformat: 'M/d/yyyy'
//                                                            },
//                                                            weeklyday: ''
//                                                        }
//                                                    }
//                                                }
//                                            });
//                                    };
//                                    
//                                    var groupCount = +(data.groupcount),
//                                        groupNodes = data.groupnodenames,
//                                        $addTable, $list, $targetGroupSelect, $groupTable, $scheduleTable, $left, $right;

//                                    $scheduleTable = $divStep5.CswTable('init', { ID: makeStepId('scheduleTable') });

//                                    //Controls go to the left
//                                    $left = $scheduleTable.CswTable('cell', 1, 1);
//                                    
//                                    //New schedules go to the right
//                                    $right = $scheduleTable.CswTable('cell', 1, 2);

//                                    $scheduleList = $('<p>New <b>' + selectedInspectionTarget + ' Group</b> Inspection Schedules: </p>')
//                                        .appendTo($right)
//                                        .hide();
//                                    $list = $scheduleList.CswList('init', {
//                                        ID: makeStepId('targetGroupList')
//                                    });

//                                    //Build the list of existing groups and schedule names
//                                    each(groupNodes, function(name) {
//                                        inspectionTargetGroups[name] = { name: name };
//                                        inspectionTargetGroups[name].sched = { };
//                                        if (contains(data, name) && false === isNullOrEmpty(data[name])) {
//                                            each(data[name], function(sched) {
//                                                inspectionTargetGroups[name].sched[sched] = sched;
//                                            });
//                                        }
//                                    });

//                                    $left.append('<br />');
//                                    $addTable = $left.CswTable('init', {
//                                        ID: makeStepId('schedulesTable'),
//                                        FirstCellRightAlign: true
//                                    });
//                                    
//                                    //1. Select or add an Inspection Target Group
//                                    //#region Inspection Target Group select
//                                    $addTable.CswTable('cell', 1, 1)
//                                        .CswDiv('init', { value: 'Inspection Target Group  ' })
//                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
//                                    
//                                    $groupTable = $addTable.CswTable('cell', 1, 2)
//                                        .CswTable();

//                                    $targetGroupSelect = $groupTable.CswTable('cell', 1, 1)
//                                        .CswSelect('init', {
//                                            ID: makeStepId('selectInspectionGroupName'),
//                                            values: groupNodes,
//                                            onChange: function() {
//                                                selectedGroup = $targetGroupSelect.find(':selected').val();
//                                            }
//                                        })
//                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
//                                    selectedGroup = $targetGroupSelect.find(':selected').val();

//                                    $groupTable.CswTable('cell', 1, 2)
//                                        .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
//                                                AlternateText: 'Add New Inspection Target Group',
//                                                onClick: function() {
//                                                    $.CswDialog('AddNodeClientSideDialog', {
//                                                        ID: makeStepId('newITG'),
//                                                        title: 'Create New Inspection Target Group',
//                                                        onSuccess: function(newGroup) {
//                                                            if (false === isNullOrEmpty(newGroup) &&
//                                                                false === contains(inspectionTargetGroups, newGroup) &&
//                                                                    false === contains(newSchedules, newGroup)) {

//                                                                groupCount += 1;
//                                                                groupNodes.push(newGroup);
//                                                                newSchedules[newGroup] = { name: newGroup, sched: { } };
//                                                                inspectionTargetGroups[newGroup] = { name: newGroup, sched: { } };
//                                                                $targetGroupSelect.CswSelect('addoption', newGroup, true);
//                                                                $addBtn.CswButton('enable');
//                                                                selectedGroup = newGroup;
//                                                            } else {
//                                                                $.CswDialog('ErrorDialog', ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'Inspection Target Group names must be unique.', 'Attempted to add Inspection Target Group name ' + newGroup + ', which already exists.'));
//                                                            }
//                                                            return CswImageButton_ButtonType.None;
//                                                        }
//                                                    });
//                                                }
//                                            })
//                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
//                                    //#endregion Inspection Target Group select
//                                    
//                                    //2. Name the new Schedule
//                                    //#region Inspection Schedule Name
//                                    $addTable.CswTable('cell', 2, 1)
//                                        .append('<br />');
//                                    $addTable.CswTable('cell', 3, 1)
//                                        .append('<span>New Schedule Name&nbsp</span>')
//                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
//                                    $newScheduleName = $addTable.CswTable('cell', 3, 2)
//                                        .CswInput('init', {
//                                            ID: makeStepId('newScheduleName'),
//                                            type: CswInput_Types.text,
//                                            cssclass: "required"
//                                        })
//                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
//                                    //#endregion Inspection Schedule Name
//                                    
//                                    $addTable.CswTable('cell', 4, 1)
//                                        .append('<br />');
//                                    
//                                    //3. Define the Schedule interval
//                                    //#region Schedule Interval Control
//                                    $addTable.CswTable('cell', 5, 1)
//                                        .append('<span>New Schedule Interval&nbsp</span>')
//                                        .css({ 'padding': '1px' });

//                                    makeTimeInterval();

//                                    $addBtn = $addTable.CswTable('cell', 6, 1)
//                                        .CswButton('init', {
//                                            ID: makeStepId('createInspectionSchedBtn'),
//                                            enabledText: 'Add Schedule',
//                                            disableOnClick: false,
//                                            onclick: function() {
//                                                var selectedName = $newScheduleName.val(),
//                                                    intervalError = timeInterval.validateRateInterval(),
//                                                    selectedInterval = timeInterval.rateInterval();
//                                                if (false !== intervalError) {
//                                                    $.CswDialog('ErrorDialog', intervalError);
//                                                }
//                                                else if (false === contains(inspectionTargetGroups[selectedGroup].sched, selectedName)) {
//                                                    $scheduleList.show();

//                                                    inspectionTargetGroups[selectedGroup].sched[selectedName] = {
//                                                        name: selectedName
//                                                    };

//                                                    if (false === contains(newSchedules, selectedGroup)) {
//                                                        newSchedules[selectedGroup] = { name: selectedGroup, sched: { } };
//                                                    }
//                                                    if (isNullOrEmpty(newSchedules.count)) {
//                                                        newSchedules.count = 0;
//                                                    }
//                                                    newSchedules.count += 1;
//                                                    newSchedules[selectedGroup].sched[selectedName] = {
//                                                        name: selectedName,
//                                                        interval: selectedInterval
//                                                    };
//                                                    $list.CswList('addItem', { value: selectedGroup + ': ' + selectedName + ', interval: ' + selectedInterval.ratetype });
//                                                    $newScheduleName.val('');
//                                                    makeTimeInterval();
//                                                } else {
//                                                    $.CswDialog('ErrorDialog', ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'A schedule with the name ' + selectedName + ' already exists on this Inspection Target Group.'));
//                                                }
//                                            }
//                                        });
//                                    if (groupCount > 0) {
//                                        $addBtn.CswButton('enable');
//                                    } else {
//                                        $addBtn.CswButton('disable');
//                                    }
//                                    //#endregion Schedule Interval Control
//                                    
//                                } // success: function()
//                            });
//                        stepFiveComplete = true;
//                    }
//                };
//            }()),

            //Step 5. Preview and Finish.
            makeStepFive = (function() {

                return function() {
                    var $confirmationList, $confirmTypesList, $confirmViewsList, $confirmationDesign, confirmGridOptions = { }, confirmGrid;

                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'disable');
                    $wizard.CswWizard('button', 'cancel', 'enable');
                    $wizard.CswWizard('button', 'finish', 'enable');

                    $divStep5 = $divStep5 || $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step5.step);
                    $divStep5.empty();

                    $divStep5.append('<p>You are about to create the following items. Click Finish to create the design.</p>');
                    $confirmationList = $divStep5.CswList('init', {
                        ID: makeStepId('confirmationList'),
                        ordered: true
                    });

                    if (isNewInspectionDesign()) {
                        if (gridOptions) {
                            $.extend(true, confirmGridOptions, gridOptions);
                        }
                        confirmGridOptions.ID = makeStepId('confirmGrid');
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
                        each(confirmGridOptions.gridOpts.colModel, function(col) {
                            if (contains(col, 'editable')) {
                                delete col.editable;
                            }
                            if (contains(col, 'edittype')) {
                                delete col.edittype;
                            }
                        });
                        $confirmationDesign = $confirmationList.CswList('addItem', {
                            value: 'Creating a new Inspection Design <b>' + newInspectionName + '</b>.'
                        });

                        confirmGrid = new CswGrid(confirmGridOptions, $confirmationDesign);
                    } else {
                        $confirmationList.CswList('addItem', {
                            value: 'Copying Inspection Design <b>' + selectedInspectionDesign.id + '</b>.'
                        });
                    }

                    $confirmTypesList = $confirmationList.CswList('addItem', {
                        value: 'New Types'
                    })
                        .CswList('init', {
                            ID: makeStepId('confirmationTypes')
                        });

                    $confirmTypesList.CswList('addItem', {
                        value: 'New Inspection Design <b>' + newInspectionName + '</b> on Inspection Target <b>' + selectedInspectionTarget + '</b>'
                    });

                    if (isNewTarget) {
                        $confirmTypesList.CswList('addItem', {
                            value: 'New Inspection Target <b>' + selectedInspectionTarget + '</b>'
                        });

                        $confirmTypesList.CswList('addItem', {
                            value: 'New Inspection Target Group <b>' + selectedInspectionTarget + ' Group</b>'
                        });
                    }

//                    if (newSchedules.count > 0) {
//                        $confirmationList.CswList('addItem', {
//                            value: $($scheduleList.html())
//                        });
//                    }

                    $confirmViewsList = $confirmationList.CswList('addItem', {
                        value: 'New Views'
                    })
                        .CswList('init', {
                            ID: makeStepId('confirmationViews')
                        });
                    $confirmViewsList.CswList('addItem', {
                        value: '<b>' + selectedInspectionTarget + ' Inspections Due Today</b>'
                    });
                    $confirmViewsList.CswList('addItem', {
                        value: '<b>All ' + selectedInspectionTarget + ' Inspections</b>'
                    });
                    if (isNewTarget) {
                        $confirmViewsList.CswList('addItem', {
                            value: '<b>All ' + selectedInspectionTarget + '</b>'
                        });
                    }
                };
            }()),

            handleNext = function($wizardTable, newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo(true);
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree(true);
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step4.step:
                        makeStepFour();
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step5.step:
                        makeStepFive();
                        break;
//                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step6.step:
//                        makeStepFive();
//                        break;
                } // switch(newstepno)
            }, // handleNext()

            handlePrevious = function(newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step1.step:
                        makeStepOne();
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo(false);
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree(false);
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step4.step:
                        makeStepFour();
                        break;
//                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step5.step:
//                        makeStepFive();
//                        break;
                }
            },

            onFinish = function() {
                var designGrid = '';

                $wizard.CswWizard('button', 'previous', 'disable');
                $wizard.CswWizard('button', 'next', 'disable');
                $wizard.CswWizard('button', 'cancel', 'disable');

                if (false === isNullOrEmpty(inspectionGrid)) {
                    designGrid = JSON.stringify(inspectionGrid.$gridTable.jqGrid('getRowData'));
                }

                var jsonData = {
                    DesignGrid: designGrid,
                    InspectionDesignName: tryParseString(newInspectionName),
                    InspectionTargetName: tryParseString(selectedInspectionTarget),
                    //Schedules: JSON.stringify(newSchedules),
                    CopyFromInspectionDesign: tryParseString(selectedInspectionDesign.name),
                    Category: tryParseString(newCategoryName)
                };

                CswAjaxJson({
                        url: '/NbtWebApp/wsNBT.asmx/finalizeInspectionDesign',
                        data: jsonData,
                        success: function(data) {
                            alert("You've won! .... a chance to implement this feature.");
                            //load the relevant Inspection Points by Location view
                        },
                        error: function(error) {
                            //$.CswDialog('ErrorDialog', error);
                        }
                    });
            };

        //#endregion Variable Declaration

        //#region Execution
        $wizard = $div.CswWizard('init', {
            ID: makeId({ ID: o.ID, suffix: 'wizard' }),
            Title: 'Create New Inspection',
            StepCount: ChemSW.enums.CswInspectionDesign_WizardSteps.stepcount,
            Steps: wizardSteps,
            StartingStep: o.startingStep,
            FinishText: 'Finish',
            onNext: handleNext,
            onPrevious: handlePrevious,
            onCancel: o.onCancel,
            onFinish: onFinish,
            doNextOnInit: false
        });

        makeStepOne();
        //#endregion Execution

        return $div;
    }; // $.fn.ChemSW.enums.CswInspectionDesign_WizardSteps
})(jQuery);

