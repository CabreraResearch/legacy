/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../controls/CswNodeTypeSelect.js" />
/// <reference path="../pagecmp/CswWizard.js" />
/// <reference path="../controls/CswGrid.js" />

(function ($) { /// <param name="$" type="jQuery" />

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
            6: CswInspectionDesign_WizardSteps.step6.description,
            7: CswInspectionDesign_WizardSteps.step7.description
        };

        //var currentStep = o.startingStep;

        var $parent = $(this),
            $div = $('<div></div>').appendTo($parent),
            $wizard, isNewInspectionDesign = false,
            currentStepNo = 0,

        // Step 1 - Select or Create Inspection Design
            $divStep1, selectedInspectionName, $inspectionDesign, excludeInspectionTargetId,
        // Step 2 - Upload Inspection Design
            $divStep2, gridIsPopulated = false,
        // Step 3 - Review and Revise Inspection Design
            $divStep3, inspectionGrid,
        // Step 4 - Select or Create Inspection Target
            $divStep4, selectedInspectionTarget, $inspectionTarget,
        // Step 5 - Add new Inspection Target Groups
            $divStep5, inspectionTargetGroups = {}, newInspectionTargetGroups = {},
        // Step 6 - Add new Inspection Schedules
            $divStep6, inspectionSchedules = {}, newInspectionSchedules = {},
        // Step 7 - Preview Results
            $divStep7,

            makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'disable').hide();
                    $wizard.CswWizard('button', 'finish', 'disable').hide();

                    var $inspectionTable, $selected,
                        $nextBtn = $wizard.CswWizard('button', 'next', 'enable');

                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step1.step);
                        $divStep1.append('<br />');

                        $inspectionTable = $divStep1.CswTable('init');

                        $inspectionTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '5px', 'vertical-align': 'middle' })
                            .append('Select Inspection Design:');

                        $inspectionDesign = $inspectionTable.CswTable('cell', 1, 2)
                            .CswDiv('init')
                            .CswNodeTypeSelect('init', {
                                ID: makeSafeId('nodeTypeSelect'),
                                objectClassName: 'InspectionDesignClass',
                                onChange: function () {
                                    var $this = $(this);
                                    $selected = $this.find(':selected');
                                    isNewInspectionDesign = isTrue($selected.CswAttrXml('data-newNodeType'));
                                }
                            });

                        $inspectionTable.CswTable('cell', 1, 3)
                            .CswDiv('init')
                            .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                AlternateText: "Create New Inspection Design",
                                onClick: function () {
                                    $selected = $inspectionDesign.find(':selected');
                                    $.CswDialog('AddNodeTypeDialog', {
                                        objectclassid: +($selected.CswAttrXml('objectclassid')),
                                        $select: $inspectionDesign,
                                        nodeTypeDescriptor: 'Inspection Design Type',
                                        onSucess: function () {
                                            isNewInspectionDesign = true;
                                        },
                                        title: 'Create a New Inspection Design.'
                                    });
                                    return CswImageButton_ButtonType.None;
                                }
                            });
                    }

                    stepOneComplete = true;
                };
            } ()),

            makeStepTwo = (function () {
                var stepTwoComplete = false;

                return function (forward) {

                    $wizard.CswWizard('button', 'next', (gridIsPopulated) ? 'enable' : 'disable');
                    var $prevBtn = $wizard.CswWizard('button', 'previous', 'enable').show();
                    var $fileUploadBtn,
                        $selected = $inspectionDesign.find(':selected');

                    selectedInspectionName = $selected.val();
                    isNewInspectionDesign = isTrue($selected.CswAttrXml('data-newNodeType'));
                    excludeInspectionTargetId = +($selected.CswAttrXml('targetnodetypeid'));

                    if (false === isNewInspectionDesign) {
                        if (forward) {
                            $wizard.CswWizard('button', 'next', 'enable').click();
                        } else {
                            $prevBtn.click();
                        }
                    }
                    else if (false === stepTwoComplete) {
                        $divStep2 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step2.step);
                        $divStep2.append('<br />');
                        $divStep2.append($('<p><a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Inspection Design template for ' + selectedInspectionName + ' Inspection.</a></p>'));
                        $divStep2.append('<br />');

                        $fileUploadBtn = $divStep2.CswDiv();
                        makeInspectionDesignUpload({
                            url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                            params: {
                                InspectionName: selectedInspectionName
                            },
                            $parent: $fileUploadBtn,
                            onSuccess: function () {
                                $wizard.CswWizard('button', 'next', 'enable').click();
                            },
                            stepNo: CswInspectionDesign_WizardSteps.step3.step,
                            uploadName: 'design'
                        });
                    }

                    stepTwoComplete = true;
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

                    $divStep3.append('<p>Inspection Name: ' + selectedInspectionName + '</p>');
                    $divStep3.append("<p>Verify the results of the Inspection Design processing. Make any necessary edits.</p>");

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
                uploadTemplate = '<div class="qq-uploader"><div class="qq-upload-drop-area"><span>Drop ' + f.uploadName + ' here to process</span></div><div class="qq-upload-button">Process ' + f.uploadName + '</div><ul class="qq-upload-list"></ul></div>';
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
                    var $prevBtn = $wizard.CswWizard('button', 'previous', 'enable'),
                        $nextBtn = $wizard.CswWizard('button', 'next', 'enable');

                    if (false === isNewInspectionDesign) {
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
                            .css({ 'padding': '5px', 'vertical-align': 'middle' })
                            .append('Select Inspection Target:');

                        $inspectionTarget = $inspectionTable.CswTable('cell', 1, 2)
                            .CswDiv('init')
                            .CswNodeTypeSelect('init', {
                                ID: makeStepId('nodeTypeSelect'),
                                objectClassName: 'InspectionTargetClass',
                                excludeNodeTypeIds: excludeInspectionTargetId
                            });
                        if ($inspectionTarget.children().length === 0) {
                            $inspectionTarget.hide();
                            $addNewTarget = $inspectionTable.CswTable('cell', 1, 3)
                                .CswDiv('init', { value: '[Add a new Inspection Target]' })
                                .css({ 'padding': '5px', 'vertical-align': 'middle' });
                        } else {
                            $nextBtn.CswButton('enable');
                        }
                        $inspectionTable.CswTable('cell', 1, 4)
                            .CswDiv('init')
                            .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                AlternateText: "Create New Inspection Target",
                                onClick: function () {
                                    $.CswDialog('AddNodeTypeDialog', {
                                        objectclassid: $inspectionTarget.find(':selected').data('objectClassId'),
                                        $select: $inspectionTarget,
                                        nodeTypeDescriptor: 'Inspection Target Type',
                                        onSuccess: function () {
                                            $inspectionTarget.show();
                                            if (false === isNullOrEmpty($addNewTarget)) {
                                                $addNewTarget.remove();
                                            }
                                            $nextBtn.CswButton('enable');
                                        },
                                        title: 'Create a New Inspection Target Type.'
                                    });
                                    return CswImageButton_ButtonType.None;
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
                    var $nextBtn = $wizard.CswWizard('button', 'next', 'disable').text('Next'); //In case we come back from step 6

                    selectedInspectionTarget = $inspectionTarget.find(':selected').val();

                    if (false === stepFiveComplete) {
                        $divStep5 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step5.step);

                        CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/getNodesForInspectionTarget',
                            data: { InspectionTargetName: selectedInspectionTarget, IncludeSchedules: true },
                            success: function (data) {
                                var groupCount = +(data.groupcount),
                                        groupNodes = data.groupnodenames,
                                        $addTable, $list, $addName;

                                if (groupCount > 0) {
                                    $nextBtn.CswButton('enable');
                                }

                                $divStep5.append('<p>Inspection Target Groups: </p>');
                                $list = $divStep5.CswList('init', {
                                    ID: makeStepId('targetGroupList')
                                });

                                each(groupNodes, function (name) {
                                    inspectionTargetGroups[name] = { name: name };
                                    $list.CswList('addItem', { value: name });
                                    inspectionTargetGroups[name].sched = {};
                                    if (contains(data, name) && false === isNullOrEmpty(data[name])) {
                                        each(data[name], function (sched) {
                                            inspectionTargetGroups[name].sched[sched] = sched;
                                            inspectionSchedules[sched] = sched;
                                        });
                                    }
                                });

                                $divStep5.append('<br />');
                                $addTable = $divStep5.CswTable();
                                $addTable.CswTable('cell', 1, 1)
                                        .CswDiv('init', { value: 'Add a new Inspection Target Group:' })
                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
                                $addName = $addTable.CswTable('cell', 1, 2)
                                        .CswInput('init', {
                                            ID: makeStepId('newInspectionGroupName'),
                                            type: CswInput_Types.text
                                        })
                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
                                $addTable.CswTable('cell', 1, 3)
                                        .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                            AlternateText: 'Add New Inspection Target Group',
                                            onClick: function () {
                                                var newGroup = $addName.val();
                                                if (false === isNullOrEmpty(newGroup) &&
                                                        false === contains(inspectionTargetGroups, newGroup) &&
                                                            false === contains(newInspectionTargetGroups, newGroup)) {

                                                    newInspectionTargetGroups[newGroup] = { name: newGroup };
                                                    inspectionTargetGroups[newGroup] = { name: newGroup };
                                                    $list.CswList('addItem', {
                                                        value: newGroup + ' (NEW)'
                                                    });
                                                    $addName.val('');
                                                    $nextBtn.CswButton('enable');
                                                }
                                                return CswImageButton_ButtonType.None;
                                            }
                                        })
                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });
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
                var stepSixComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable').text('Create Inspection Design');

                    var $addTable, $list, $addName, $groupSelect, $interval, groupValues = [];

                    if (false === stepSixComplete) {
                        $divStep6 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step6.step);

                        $divStep6.append('<p>Inspection Schedules: </p>');
                        $list = $divStep6.CswList('init', {
                            ID: makeStepId('targetGroupList')
                        });

                        each(inspectionSchedules, function (name) {
                            $list.CswList('addItem', { value: name });
                        });

                        $divStep6.append('<br />');
                        $addTable = $divStep6.CswTable();
                        $addTable.CswTable('cell', 1, 1)
                                .CswDiv('init', { value: 'Add a new Inspection Schedule:' })
                                .css({ 'padding': '5px', 'vertical-align': 'middle' });

                        each(inspectionTargetGroups, function (prop, key) {
                            groupValues.push(key);
                        });
                        $groupSelect = $addTable.CswTable('cell', 1, 2)
                                                .CswSelect('init', {
                                                    ID: makeStepId('inspectionGroupSelect'),
                                                    values: groupValues
                                                })
                                                .css({ 'padding': '5px', 'vertical-align': 'middle' });

                        $addName = $addTable.CswTable('cell', 1, 3)
                                            .CswInput('init', {
                                                ID: makeStepId('newInspectionScheduleName'),
                                                type: CswInput_Types.text
                                            })
                                            .css({ 'padding': '5px', 'vertical-align': 'middle' });

                        //abstracted interval control goes here
                        $interval = $addTable.CswTable('cell', 1, 4);

                        $addTable.CswTable('cell', 1, 5)
                                .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                    AlternateText: 'Add New Inspection Schedule',
                                    onClick: function () {
                                        var newSched = $addName.val(),
                                            group = $groupSelect.find(':selected').val(),
                                            interval = $interval.val();

                                        if (false === isNullOrEmpty(newSched) &&
                                                false === isNullOrEmpty(group) &&
                                                    false === isNullOrEmpty(interval) &&
                                                        false === contains(inspectionSchedules, newSched) &&
                                                            false === contains(newInspectionSchedules, newSched)) {

                                            newInspectionSchedules[newSched] = {
                                                name: newSched,
                                                interval: interval,
                                                group: group
                                            };
                                            $list.CswList('addItem', {
                                                value: newSched + ' (NEW)'
                                            });
                                            $addName.val('');
                                        }
                                        return CswImageButton_ButtonType.None;
                                    }
                                })
                                .css({ 'padding': '5px', 'vertical-align': 'middle' });

                        stepSixComplete = true;
                    }
                };
            } ()),

            makeStepSeven = function () {

                $wizard.CswWizard('button', 'previous', 'disable').hide().remove();
                $wizard.CswWizard('button', 'next', 'disable').hide().remove();
                $wizard.CswWizard('button', 'cancel', 'disable').hide().remove();
                $wizard.CswWizard('button', 'finish', 'enable').show();

                $divStep7 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step7.step);
            },

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
                    case CswInspectionDesign_WizardSteps.step7.step:
                        finalize();
                        makeStepSeven();
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
                    case CswInspectionDesign_WizardSteps.step5.step:
                        makeStepFive();
                        break;
                }
            },

            finalize = function (ignore) {
                var jsonData = {
                    DesignGrid: inspectionGrid.$gridTable.jqGrid('getRowData'),
                    InspectionName: selectedInspectionName,
                    InspectionTarget: selectedInspectionTarget
                };
                CswAjaxJson({
                    url: '/NbtWebApp/wsNBT.aspx/finalizeInspectionDesign',
                    data: jsonData,
                    success: function (data) {
                        //show success dialog
                        //load the relevant Inspection Points by Location view
                    },
                    error: function (error) {
                        //$.CswDialog('ErrorDialog', error);
                    }
                });

            }, //finalize

            onFinish = function () {
                //load some view
            },
            
            makeStepId = function (suffix, stepNo) {
                var step = stepNo || currentStepNo;
                return makeId({ prefix: 'step' + currentStepNo, ID: o.ID, suffix: suffix });
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

