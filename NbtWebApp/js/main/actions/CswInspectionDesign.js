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
            $divStep5,
        // Step 6 - Add new Inspection Schedules
            $divStep6,
        // Step 7 - Preview Results
            $divStep7,

            makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'disable').hide();
                    $wizard.CswWizard('button', 'finish', 'disable').hide();

                    var $inspectionTable,
                        $nextBtn = $wizard.CswWizard('button', 'next', 'enable');

                    if (false === stepOneComplete) {
                        $nextBtn.bind('click', function () {
                            if (currentStepNo = CswInspectionDesign_WizardSteps.step1.step) {
                                selectedInspectionName = $inspectionDesign.val();
                                isNewInspectionDesign = isTrue($inspectionDesign.CswAttrXml('data-newNodeType'));
                                excludeInspectionTargetId = +($inspectionDesign.CswAttrXml('targetnodetypeid'));
                            }
                        });

                        $divStep1 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step1.step);
                        $divStep1.append('<br />');

                        $inspectionTable = $divStep1.CswTable('init');

                        $inspectionTable.CswTable('cell', 1, 1)
                                        .css({ 'padding': '5px', 'vertical-align': 'middle' })
                                        .append('Select Inspection Design:');

                        $inspectionDesign = $inspectionTable.CswTable('cell', 1, 2)
                                                .CswDiv('init')
                                                .CswNodeTypeSelect('init', {
                                                    ID: 'step2_nodeTypeSelect',
                                                    objectClassName: 'InspectionDesignClass'
                                                });

                        $inspectionTable.CswTable('cell', 1, 3)
                            .CswDiv('init')
                            .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                AlternateText: "Create New Inspection Design",
                                onClick: function () {
                                    $.CswDialog('AddNodeTypeDialog', {
                                        objectclassid: $inspectionDesign.find(':selected').data('objectClassId'),
                                        $select: $inspectionDesign,
                                        nodeTypeDescriptor: 'Inspection Design Type',
                                        onSucess: function () {

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

                    var $prevBtn = $wizard.CswWizard('button', 'previous', 'enable').show();
                    $wizard.CswWizard('button', 'next', (gridIsPopulated) ? 'enable' : 'disable');
                    var $fileUploadBtn;

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
                    var previewGridId = makeId({ prefix: o.ID, ID: +'step' + parseInt(f.stepNo), suffix: 'previewGrid_outer' }),
                        $previewGrid = $divStep2.find('#' + previewGridId),
                        g;

                    $divStep3.append('<p>Inspection Name: ' + selectedInspectionName + '</p>');
                    $divStep3.append("<p>Verify the results of the Inspection Design processing. Make any necessary edits.</p>");

                    if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                        $previewGrid = $('<div id="' + o.ID + '"></div>').appendTo($divStep3);
                    } else {
                        $previewGrid.empty();
                    }

                    g = {
                        Id: o.ID,
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

                    var $inspectionTable,
                        $nextBtn = $wizard.CswWizard('button', 'next', 'enable').show();


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
                                                                ID: 'step4_nodeTypeSelect',
                                                                objectClassName: 'InspectionTargetClass',
                                                                excludeNodeTypeIds: excludeInspectionTargetId
                                                            });

                        $inspectionTable.CswTable('cell', 1, 3)
                            .CswDiv('init')
                            .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                AlternateText: "Create New Inspection Target",
                                onClick: function () {
                                    $.CswDialog('AddNodeTypeDialog', {
                                        objectclassid: $inspectionTarget.find(':selected').data('objectClassId'),
                                        $select: $inspectionTarget,
                                        nodeTypeDescriptor: 'Inspection Target Type',
                                        onSucess: function () {

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
                    $wizard.CswWizard('button', 'next', 'enable').text('Next'); //In case we come back from step 6

                    if (false === stepFiveComplete) {
                        $divStep5 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step5.step);
                    }
                };
            } ()),

            makeStepSix = (function () {
                var stepSixComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable').text('Create Inspection Design');

                    if (false === stepSixComplete) {
                        $divStep6 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step6.step);
                    }
                };
            } ()),

            makeStepSeven = function () {

                $wizard.CswWizard('button', 'previous', 'disable').hide();
                $wizard.CswWizard('button', 'next', 'disable').hide();
                $wizard.CswWizard('button', 'cancel', 'disable').hide();
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

            handleFinish = function (ignore) {
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

            }; //_handleFinish

        //#endregion Variable Declaration

        //#region Execution
        $wizard = $div.CswWizard('init', {
            ID: o.ID + '_wizard',
            Title: 'Create New Inspection',
            StepCount: CswInspectionDesign_WizardSteps.stepcount,
            Steps: wizardSteps,
            StartingStep: o.startingStep,
            FinishText: 'Finish',
            onNext: handleNext,
            onPrevious: handlePrevious,
            onCancel: o.onCancel,
            onFinish: handleFinish,
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

