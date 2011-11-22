/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../controls/CswNodeTypeSelect.js" />
/// <reference path="../pagecmp/CswWizard.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../pagecmp/CswDialog.js" />
/// <reference path="../controls/CswTimeInterval.js" />

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
            1: CswInspectionDesign_WizardSteps.step1.description,
            2: CswInspectionDesign_WizardSteps.step2.description,
            3: CswInspectionDesign_WizardSteps.step3.description,
            4: CswInspectionDesign_WizardSteps.step4.description,
            5: CswInspectionDesign_WizardSteps.step5.description,
            6: CswInspectionDesign_WizardSteps.step6.description
        };

        //var currentStep = o.startingStep;

        var $parent = $(this),
            $div = $('<div></div>').appendTo($parent),
            $wizard,
            currentStepNo = 0,

        // Step 1 - Select or Create Inspection Design
            copyFromInspectionDesign = '[Create New]',
            $divStep1, newInspectionName, newCategoryName, $inspectionDesignSelect, excludeInspectionTargetId = '', $newDesignName, $newDesignCategory,
        // Step 2 - Upload Inspection Design
            $divStep2, gridIsPopulated = false,
        // Step 3 - Review and Revise Inspection Design
            $divStep3, inspectionGrid,
        // Step 4 - Select or Create Inspection Target
            $divStep4, selectedInspectionTarget, $inspectionTarget,
        // Step 5 - Add new Inspection Target Groups
            $divStep5, inspectionTargetGroups = { }, newSchedules = { },
        // Step 6 - Add new Inspection Schedules
            $divStep6, 

            isNewInspectionDesign = function() {
                return ('[Create New]' === copyFromInspectionDesign);
            },

            makeStepOne = (function() {
                var stepOneComplete = false;

                return function() {
                    $wizard.CswWizard('button', 'previous', 'disable').hide();
                    $wizard.CswWizard('button', 'finish', 'disable').hide();
                    $wizard.CswWizard('button', 'next', 'disable').show();
                    if(false === isNullOrEmpty(newInspectionName)) {
                        $wizard.CswWizard('button', 'next', 'enable');
                    }
                    
                    var $inspectionTable;

                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step1.step);
                        $divStep1.append('<br />');

                        $inspectionTable = $divStep1.CswTable('init');

                        $inspectionTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('Copy from Inspection Design: ');

                        $inspectionDesignSelect = $inspectionTable.CswTable('cell', 1, 2)
                            .CswDiv('init')
                            .CswNodeTypeSelect('init', {
                                ID: makeSafeId('nodeTypeSelect'),
                                objectClassName: 'InspectionDesignClass',
                                addNewOption: true
                            })
                            .change(function () {
                                copyFromInspectionDesign = $inspectionDesignSelect.find(':selected').text();
                            });
                        copyFromInspectionDesign = $inspectionDesignSelect.find(':selected').val();

                        $inspectionTable.CswTable('cell', 2, 1).append('<br />');

                        $inspectionTable.CswTable('cell', 3, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span class="required">New Inspection Design Name: </span>');

                        $newDesignName = $inspectionTable.CswTable('cell', 3, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .CswInput('init', {
                                ID: o.ID + '_newDesignName',
                                type: CswInput_Types.text,
                                cssclass: 'required'
                            })
                            .keypress(function() {
                                setTimeout(function() {
                                    var newDesignName = $newDesignName.val();
                                    if (false === isNullOrEmpty(newDesignName)) {
                                        $wizard.CswWizard('button', 'next', 'enable');
                                    }
                                }, 100);
                            });

                        $inspectionTable.CswTable('cell', 4, 1).append('<br />');

                        $inspectionTable.CswTable('cell', 5, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>New Category Name: </span>');

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
                            if(isFunction(success)) {
                                success(data);
                            }
                        },
                        error: function (data) {
                            if(isFunction(error)) {
                                error(data);
                            }
                        }
                });
            },
            
            //If this is a new Design, upload the template. Otherwise skip to step 4.
            makeStepTwo = (function () {
                var stepTwoComplete = false;

                return function(forward) {
                    var success = function() {

                        if (gridIsPopulated) {
                            $wizard.CswWizard('button', 'next', 'enable').show();
                        } else {
                            $wizard.CswWizard('button', 'next', 'disable').hide();
                        }
                        var $prevBtn = $wizard.CswWizard('button', 'previous', 'enable').show();
                        var $fileUploadBtn,
                            $selected = $inspectionDesignSelect.find(':selected');

                        if (false === isNewInspectionDesign()) {
                            excludeInspectionTargetId = +($selected.CswAttrXml('targetnodetypeid'));
                            if (forward) {
                                $wizard.CswWizard('button', 'next', 'enable').click();
                            } else {
                                $prevBtn.click();
                            }
                        }
                        else if (false === stepTwoComplete) {
                            $divStep2 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step2.step);
                            $divStep2.CswList('init', { values: ['Create a new <b>' + newInspectionName + '</b> Inspection Design using the Excel template.', 'Upload the finished design to proceed.'] });
                            $divStep2.append($('<p><a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download Template</a></p>'));
                            $divStep2.append('<br />');

                            $fileUploadBtn = $divStep2.CswDiv();
                            makeInspectionDesignUpload({
                                    url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                                    params: {
                                        InspectionName: newInspectionName
                                    },
                                    $parent: $fileUploadBtn,
                                    onSuccess: function() {
                                        $wizard.CswWizard('button', 'next', 'enable').click();
                                    },
                                    stepNo: CswInspectionDesign_WizardSteps.step3.step,
                                    uploadName: 'design'
                                });
                            stepTwoComplete = true;
                        }
                    };
                    var error = function() {
                        $wizard.CswWizard('button', 'previous', 'enable').click();
                        $wizard.CswWizard('button', 'next', 'disable');
                    };
                    
                    newInspectionName = $newDesignName.val();
                    newCategoryName = $newDesignCategory.val();
                    
                    checkIsNodeTypeNameUnique(newInspectionName, success, error);
                };
            } ()),

            makeInspectionDesignUpload = function (opts) {
                var f = {
                    url: '',
                    params: {},
                    $parent: '',
                    onSuccess: null,
                    stepNo: '',
                    uploadName: ''
                };
                if (opts) {
                    $.extend(f, opts);
                }

                var uploadTemplate;

                var onComplete = function (id, fileName, data) {
                    if (isFunction(f.onSuccess)) {
                        f.onSuccess();
                    }

                    $divStep3.empty();
                    var previewGridId = makeStepId('previewGrid_outer', 3),
                        $previewGrid = $divStep2.find('#' + previewGridId),
                        g;

                    $divStep3.append('<p>Inspection Name: ' + newInspectionName + '</p>');
                    $divStep3.append("<p>Review the upload results. Make any necessary edits.</p>");

                    if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                        $previewGrid = $('<div id="' + previewGridId + '"></div>').appendTo($divStep3);
                    } else {
                        $previewGrid.empty();
                    }

                    g = {
                        Id: makeStepId(),
                        pagermode: 'default',
                        gridOpts: {
                            autowidth: true,
                            rowNum: 20,
                            height: 'auto'
                        },
                        optNav: {
                            add: true,
                            del: true,
                            edit: true,
                            view: false,
                            editfunc: function (rowid) {
                                return inspectionGrid.$gridTable.jqGrid('editGridRow', rowid, { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterEdit: true });
                            },
                            addfunc: function () {
                                return inspectionGrid.$gridTable.jqGrid('editGridRow', 'new', { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterAdd: true });
                            },
                            delfunc: function (rowid) {
                                return inspectionGrid.$gridTable.jqGrid('delRowData', rowid);
                            }
                        }
                    };

                    $.extend(g.gridOpts, data.jqGridOpt);

                    inspectionGrid = new CswGrid(g, $previewGrid);
                };
                uploadTemplate = '<div class="qq-uploader"><div class="qq-upload-drop-area"><span>Drop ' + f.uploadName + ' here to process</span></div><div class="qq-upload-button">Upload Design</div><ul class="qq-upload-list"></ul></div>';
                var uploader = new qq.FileUploader({
                    element: f.$parent.get(0),
                    multiple: false,
                    action: f.url,
                    template: uploadTemplate,
                    params: f.params,
                    onSubmit: function () {
                        $('.qq-upload-list').empty();
                    },
                    onComplete: function (id, fileName, data) {
                        onComplete(id, fileName, data);
                    },
                    showMessage: function (error) {
                        $.CswDialog('ErrorDialog', error);
                    }
                });
            },

            makeStepThree = (function () {
                var stepThreeComplete = false;
                return function (forward) {
                    var $prevBtn = $wizard.CswWizard('button', 'previous', 'enable').show(),
                        $nextBtn = $wizard.CswWizard('button', 'next', 'enable').show();

                    if (false === isNewInspectionDesign()) {
                        if (forward) {
                            $nextBtn.click();
                        } else {
                            $prevBtn.click();
                        }
                    }
                    else if (false === stepThreeComplete) {
                        $divStep3 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step3.step);
                        stepThreeComplete = true;
                    }
                };
            } ()),

            makeStepFour = (function () {
                var stepFourComplete = false;
                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable').show();

                    var $inspectionTable, $addNewTarget,
                        $nextBtn = $wizard.CswWizard('button', 'next', 'disable').show();


                    if (false === stepFourComplete) {
                        $nextBtn.bind('click', function () {
                            if (currentStepNo = CswInspectionDesign_WizardSteps.step4.step) {
                                selectedInspectionTarget = $inspectionTarget.val();
                            }
                        });

                        $divStep4 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step4.step);
                        $divStep4.append('<br />');

                        $inspectionTable = $divStep4.CswTable('init');

                        $inspectionTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('Select an Inspection Target: ');

                        $inspectionTarget = $inspectionTable.CswTable('cell', 1, 2)
                            .CswDiv('init')
                            .CswNodeTypeSelect('init', {
                                ID: makeStepId('nodeTypeSelect'),
                                objectClassName: 'InspectionTargetClass',
                                excludeNodeTypeIds: excludeInspectionTargetId,
                                onChange: function () {
                                    var $this = $(this).find(':selected');
                                    selectedInspectionTarget = $this.text();
                                }
                            });
                        
                        if ($inspectionTarget.children().length === 0) {
                            $inspectionTarget.hide();
                            $addNewTarget = $inspectionTable.CswTable('cell', 1, 3)
                                .CswInput('init', {
                                    ID: o.ID + '_newTargetName', 
                                    value: '[New Inspection Target]'
                                })
                                .css({ 'padding': '1px', 'vertical-align': 'middle' });
                        } else {
                            selectedInspectionTarget = $inspectionTarget.find(':selected').text();
                            $nextBtn.CswButton('enable');
                        }
                        
                        $inspectionTable.CswTable('cell', 1, 4)
                            .CswDiv('init')
                            .CswButton('init', { 
                                ID: makeStepId('addNewInspectionTarget'),
                                enabledText: 'Add New',
                                disableOnClick: false,
                                onclick: function () {
                                    selectedInspectionTarget = $addNewTarget.val();
                                    
                                    $.CswDialog('AddNodeTypeDialog', {
                                        objectclassid: $inspectionTarget.find(':selected').data('objectClassId'),
                                        nodetypename: selectedInspectionTarget,
                                        category: newCategoryName,
                                        $select: $inspectionTarget,
                                        nodeTypeDescriptor: 'Target',
                                        onSuccess: function (data) {
                                            selectedInspectionTarget = data.nodetypename;
                                            newCategoryName = data.category;
                                            
                                            $inspectionTarget.show();
                                            if (false === isNullOrEmpty($addNewTarget)) {
                                                $addNewTarget.remove();
                                            }
                                            $nextBtn.CswButton('enable');
                                        },
                                        title: 'Create a New Inspection Target Type.'
                                    });
                                    return false;
                                }
                            });
                        stepFourComplete = true;
                    }
                };
            } ()),

            makeStepFive = (function () {
                var stepFiveComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable').val('Create Inspection Design');

                    if(isNullOrEmpty(selectedInspectionTarget)) {
                        selectedInspectionTarget = $inspectionTarget.find(':selected').text();
                    }
                    
                    var $newScheduleName, $newScheduleInterval, timeInterval, $addBtn, selectedGroup;
                    
                    if (false === stepFiveComplete) {
                        $divStep5 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step5.step);

                        CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/getScheduleNodesForInspection',
                            data: { InspectionTargetName: selectedInspectionTarget, 
                                    CopyInspectionDesignName: copyFromInspectionDesign },
                            success: function (data) {
                                var groupCount = +(data.groupcount),
                                        groupNodes = data.groupnodenames,
                                        $addTable, $list, $targetGroupSelect, $scheduleList, $groupTable;

                                $divStep5.append('<p>Add schedules for the new Inspection.<br />Create new Inspection Target groups if needed.</p>');
                                
                                $scheduleList = $('<p>New Inspection Schedules: </p>')
                                                    .appendTo($divStep5)
                                                    .hide();
                                $list = $divStep5.CswList('init', {
                                    ID: makeStepId('targetGroupList')
                                });

                                each(groupNodes, function (name) {
                                    inspectionTargetGroups[name] = { name: name };
                                    inspectionTargetGroups[name].sched = {};
                                    if (contains(data, name) && false === isNullOrEmpty(data[name])) {
                                        each(data[name], function (sched) {
                                            inspectionTargetGroups[name].sched[sched] = sched;
                                        });
                                    }
                                });

                                $divStep5.append('<br />');
                                $addTable = $divStep5.CswTable();
                                $addTable.CswTable('cell', 1, 1)
                                        .CswDiv('init', { value: 'Select an Inspection Target Group: ' })
                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });

                                $groupTable = $addTable.CswTable('cell', 1, 2)
                                                       .CswTable();
                                
                                $targetGroupSelect = $groupTable.CswTable('cell', 1, 1)
                                        .CswSelect('init', {
                                            ID: makeStepId('selectInspectionGroupName'),
                                            values: groupNodes,
                                            onChange: function () {
                                                selectedGroup = $targetGroupSelect.find(':selected').val();
                                            }
                                        })
                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
                                selectedGroup = $targetGroupSelect.find(':selected').val(); 
                                
                                $groupTable.CswTable('cell', 1, 2)
                                        .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                            AlternateText: 'Add New Inspection Target Group',
                                            onClick: function () {
                                                $.CswDialog('AddNodeClientSideDialog', {
                                                    ID: makeStepId('newITG'),
                                                    title: 'Create New Inspection Target Group',
                                                    onSuccess: function (newGroup) {
                                                        if (false === isNullOrEmpty(newGroup) &&
                                                                false === contains(inspectionTargetGroups, newGroup) &&
                                                                    false === contains(newSchedules, newGroup)) {

                                                            groupCount += 1;
                                                            groupNodes.push(newGroup);
                                                            newSchedules[newGroup] = { name: newGroup, sched: {} };
                                                            inspectionTargetGroups[newGroup] = { name: newGroup, sched: {} };
                                                            $targetGroupSelect.CswSelect('addoption', newGroup, true);
                                                            $addBtn.CswButton('enable');
                                                            selectedGroup = newGroup;
                                                        } else {
                                                            //$.CswDialog('ErrorDialog', error);
                                                        }
                                                        return CswImageButton_ButtonType.None;        
                                                    }
                                                });
                                            }
                                        })
                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });

                                $addTable.CswTable('cell', 2, 1)
                                         .append('<br />');
                                $addTable.CswTable('cell', 3, 1)
                                         .append('New Schedule Name')
                                         .css({ 'padding': '1px', 'vertical-align': 'middle' });
                                $newScheduleName = $addTable.CswTable('cell', 3, 2)
                                                            .CswInput('init', {
                                                                ID: makeStepId('newScheduleName'),
                                                                type: CswInput_Types.text,
                                                                cssclass: "required"
                                                            })
                                                            .css({ 'padding': '1px', 'vertical-align': 'middle' });
                                
                                $addTable.CswTable('cell', 4, 1)
                                         .append('<br />');
                                $addTable.CswTable('cell', 5, 1)
                                         .append('New Schedule Interval')
                                         .css({ 'padding': '1px', 'vertical-align': 'middle' });
                                $newScheduleInterval = $addTable.CswTable('cell', 5, 2)
                                                                .CswDiv('init');
                                timeInterval = CswTimeInterval({
                                        $parent: $newScheduleInterval,
                                        propVals: {
                                            Interval: {
                                                rateintervalvalue: {
                                                    dateformat: 'M/d/yyyy',
                                                    ratetype: CswRateIntervalTypes.WeeklyByDay,
                                                    startingdate: {
                                                        date: '',
                                                        dateformat: 'M/d/yyyy'
                                                    },
                                                    weeklyday: ''
                                                }
                                            }
                                        }
                                    });

                                $addBtn = $addTable.CswTable('cell', 6, 1)
                                                    .CswButton('init', {
                                                        ID: makeStepId('createInspectionSchedBtn'),
                                                        enabledText: 'Add Schedule',
                                                        disableOnClick: false,
                                                        onclick: function() {
                                                            var selectedName = $newScheduleName.val(),
                                                                selectedInterval = timeInterval.rateInterval();
                                                            
                                                            if(false === contains(inspectionTargetGroups[selectedGroup].sched, selectedName)) {
                                                                $scheduleList.show();
                                                                
                                                                inspectionTargetGroups[selectedGroup].sched[selectedName] = {
                                                                    name: selectedName
                                                                };
                                                                
                                                                if(false === contains(newSchedules, selectedGroup)) {
                                                                    newSchedules[selectedGroup] = { name: selectedGroup, sched: { } };
                                                                }
                                                                newSchedules[selectedGroup].sched[selectedName] = {
                                                                    name: selectedName,
                                                                    interval: selectedInterval
                                                                };
                                                                $list.CswList('addItem', { value: selectedGroup + ': ' + selectedName + ', interval: ' + selectedInterval.ratetype });
                                                            } else {
                                                                //$.CswDialog('ErrorDialog', error);
                                                            }
                                                        }
                                                    });
                                if (groupCount > 0) {
                                    $addBtn.CswButton('enable');
                                } else {
                                    $addBtn.CswButton('disable');
                                }
                            },
                            error: function (error) {
                                //$.CswDialog('ErrorDialog', error);
                            }
                        });
                        stepFiveComplete = true;
                    }
                };
            } ()),

            makeStepSix = (function () {
                
                return function () {
                    $wizard.CswWizard('button', 'previous', 'disable').hide().remove();
                    $wizard.CswWizard('button', 'next', 'disable').hide().remove();
                    $wizard.CswWizard('button', 'cancel', 'disable').hide().remove();
                    $wizard.CswWizard('button', 'finish', 'enable').show();
                    
                    var $addTable, $list, $addName, $groupSelect, $interval, groupValues = [];
                    $divStep6 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step6.step);

                    var jsonData = {
                        DesignGrid: inspectionGrid.$gridTable.jqGrid('getRowData'),
                        InspectionDesignName: newInspectionName,
                        InspectionTargetName: selectedInspectionTarget,
                        Schedules: newSchedules,
                        CopyFromInspectionDesign: copyFromInspectionDesign
                    };
                    
                    CswAjaxJson({
                        url: '/NbtWebApp/wsNBT.aspx/finalizeInspectionDesign',
                        data: jsonData,
                        success: function (data) {
                            alert("You've won! .... a chance to implement this feature.");
                            //load the relevant Inspection Points by Location view
                        },
                        error: function (error) {
                            //$.CswDialog('ErrorDialog', error);
                        }
                    });    
                    
                    
                };
            } ()),

            handleNext = function ($wizardTable, newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo(true);
                        break;
                    case CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree(true);
                        break;
                    case CswInspectionDesign_WizardSteps.step4.step:
                        makeStepFour();
                        break;
                    case CswInspectionDesign_WizardSteps.step5.step:
                        makeStepFive();
                        break;
                    case CswInspectionDesign_WizardSteps.step6.step:
                        makeStepSix();
                        break;
                } // switch(newstepno)
            }, // handleNext()

            handlePrevious = function (newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case CswInspectionDesign_WizardSteps.step1.step:
                        makeStepOne();
                        break;
                    case CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo(false);
                        break;
                    case CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree(false);
                        break;
                    case CswInspectionDesign_WizardSteps.step4.step:
                        makeStepFour();
                        break;
                }
            },

            onFinish = function () {
                //load some view
            },
            
            makeStepId = function (suffix, stepNo) {
                var step = stepNo || currentStepNo;
                return makeId({ prefix: 'step_' + step, ID: o.ID, suffix: suffix });
            };


        //#endregion Variable Declaration

        //#region Execution
        $wizard = $div.CswWizard('init', {
            ID: makeId({ ID: o.ID, suffix: 'wizard' }),
            Title: 'Create New Inspection',
            StepCount: CswInspectionDesign_WizardSteps.stepcount,
            Steps: wizardSteps,
            StartingStep: o.startingStep,
            FinishText: 'Finish',
            onNext: handleNext,
            onPrevious: handlePrevious,
            onCancel: o.onCancel,
            onFinish: onFinish,
            doNextOnInit: false
        });

        // don't activate Finish until step 7
        if (o.startingStep !== 7) {
            $wizard.CswWizard('button', 'finish', 'disable').hide();
        }

        makeStepOne();
        //#endregion Execution

        return $div;
    }; // $.fn.CswInspectionDesign_WizardSteps
})(jQuery);

