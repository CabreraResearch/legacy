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

        var wizardStepArray = [CswImportInspectionQuestions_WizardSteps.step1,
                               CswImportInspectionQuestions_WizardSteps.step2,
                               CswImportInspectionQuestions_WizardSteps.step3,
                               CswImportInspectionQuestions_WizardSteps.step4,
                               CswImportInspectionQuestions_WizardSteps.step5,
                               CswImportInspectionQuestions_WizardSteps.step6],
            wizardSteps = {
                1: CswImportInspectionQuestions_WizardSteps.step1.description,
                2: CswImportInspectionQuestions_WizardSteps.step2.description,
                3: CswImportInspectionQuestions_WizardSteps.step3.description,
                4: CswImportInspectionQuestions_WizardSteps.step4.description,
                5: CswImportInspectionQuestions_WizardSteps.step5.description,
                6: CswImportInspectionQuestions_WizardSteps.step6.description
            };

        //var currentStep = o.startingStep;

        var $parent = $(this),
            $div = $('<div></div>').appendTo($parent),
            $wizard,

        // Step 1 - Create or Select Inspection Design
            $divStep1, selectedInspectionName,
            isNewInspection = true,
        // Step 2 - Upload Inspection Design
            $divStep2, selectedInspectionTarget, tempFileName, inspectionGrid,
        // Step 3 - Preview Inspection Design Grid
            $divStep3,
        // step 4 - Import Inspection Points
            $divStep4, $inspectPointUpload,
        // step 5 - Preview results
            $divStep5, importGrid,
        // step 6
            $divStep6,

            makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'disable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    var $createNewInspect, $inspectionLabel, $inspectionName, $selectLabel, $selectName, $radioTable;

                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step1.step);
                        $radioTable = $divStep1.CswTable('init');

                        $createNewInspect = $radioTable.CswTable('cell', 1, 1).CswInput('init', {
                            type: CswInput_Types.radio,
                            name: o.ID + '_InspectDesignRadio',
                            ID: o.ID + '_newInspectDesignRadio',
                            onChange: function () {
                                var $this = $(this);
                                if ($this.is(':checked')) {
                                    isNewInspection = true;
                                    $inspectionLabel.show();
                                    $inspectionName.show();
                                    if (false === isNullOrEmpty($selectName)) {
                                        $selectLabel.hide();
                                        $selectName.hide();
                                    }
                                }
                            }
                        });
                        $radioTable.CswTable('cell', 1, 2).append('Create a New Inspection Design');
                        $createNewInspect.CswAttrDom('checked', true);
                        $radioTable.CswTable('cell', 2, 1).append('<br />');

                        $inspectionLabel = $radioTable.CswTable('cell', 3, 2).CswDiv('init').append('Inspection Name: ');

                        $inspectionName = $radioTable.CswTable('cell', 3, 3)
                            .CswDiv('init')
                            .CswInput('init', {
                                ID: o.ID + '_inspectionName',
                                type: CswInput_Types.text
                            });


                        $inspectionName.keypress(function () {
                            setTimeout(function () {
                                selectedInspectionName = $inspectionName.val().trim();
                                if (false === isNullOrEmpty(selectedInspectionName)) {
                                    $wizard.CswWizard('button', 'next', 'enable');
                                } else {
                                    $wizard.CswWizard('button', 'next', 'disable');
                                }
                            }, 100);
                        });

                        $radioTable.CswTable('cell', 4, 1).append('<br />');
                        $radioTable.CswTable('cell', 5, 1).CswInput('init', {
                            type: CswInput_Types.radio,
                            name: o.ID + '_InspectDesignRadio',
                            ID: o.ID + '_newInspectDesignRadio',
                            onChange: function () {
                                var $this = $(this);
                                if ($this.is(':checked')) {
                                    $inspectionName.hide();
                                    $inspectionLabel.hide();
                                    isNewInspection = false;
                                    if (false === isNullOrEmpty($selectName)) {
                                        $selectLabel.show();
                                        $selectName.show();
                                        selectedInspectionName = $selectName.val();
                                    } else {
                                        $radioTable.CswTable('cell', 6, 1).append('<br />');
                                        $selectLabel = $radioTable.CswTable('cell', 7, 2).CswDiv('init').append('Inspection Name: ');
                                        $selectName = $radioTable.CswTable('cell', 7, 3)
                                            .CswDiv('init')
                                            .CswNodeTypeSelect('init', {
                                                ID: o.ID + '_step1_nodeTypeSelect',
                                                objectClassName: 'InspectionDesignClass',
                                                onChange: function () {
                                                    selectedInspectionName = $selectName.val();
                                                }
                                            });
                                    }
                                }
                            }
                        });
                        $radioTable.CswTable('cell', 5, 2).append('Choose an Inspection');
                        $selectName = '';
                    }
                    stepOneComplete = true;
                };
            } ()),

            makeStepTwo = (function () {
                var stepTwoComplete = false;
                return function (forward) {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'disable');
                    $wizard.CswWizard('button', 'finish', 'disable');

                    var $step2Table, $inspectionTarget, $templateUpload, $uploadCell;

                    if (false === isNewInspection) {
                        if (isTrue(forward)) {
                            $wizard.CswWizard('button', 'next', 'enable');
                            $wizard.CswWizard('button', 'next', 'click');
                        } else {
                            $wizard.CswWizard('button', 'previous', 'click');
                        }
                    } else {

                        CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/IsNewInspectionNameUnique',
                            data: { 'NewInspectionName': selectedInspectionName },
                            success: function (data) {
                                if (false === stepTwoComplete) {
                                    $divStep2 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step2.step);
                                    $divStep2.append('<br />');
                                    $divStep2.append($('<p><a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Inspection Design template</a></p>'));

                                    $step2Table = $divStep2.CswTable();
                                    $step2Table.CswTable('cell', 1, 1).append('Set Inspection Target: ');
                                    $inspectionTarget = $step2Table.CswTable('cell', 1, 2)
                                            .CswDiv('init')
                                            .CswNodeTypeSelect('init', {
                                                ID: 'step2_nodeTypeSelect',
                                                objectClassName: 'InspectionTargetClass'
                                            });

                                    $step2Table.CswTable('cell', 1, 3)
                                            .CswDiv('init')
                                            .CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                                                AlternateText: "Create New Inspection Target",
                                                onClick: function () {
                                                    $.CswDialog('AddNodeTypeDialog', {
                                                        objectclassid: $inspectionTarget.find(':selected').data('objectClassId'),
                                                        $select: $inspectionTarget,
                                                        nodeTypeDescriptor: 'Inspection Target Type'
                                                    });
                                                    return CswImageButton_ButtonType.None;
                                                }
                                            });

                                    $step2Table.CswTable('cell', 2, 1).append('<br />');
                                    $uploadCell = $step2Table.CswTable('cell', 3, 1);

                                    $templateUpload = makeInspectionDesignUpload({
                                        url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                                        params: {
                                            InspectionName: selectedInspectionName
                                        },
                                        $parent: $uploadCell,
                                        onSuccess: function () {
                                            selectedInspectionTarget = $inspectionTarget.val();
                                            $wizard.CswWizard('button', 'next', 'enable');
                                            $wizard.CswWizard('button', 'next', 'click');
                                        },
                                        stepNo: CswImportInspectionQuestions_WizardSteps.step3.step,
                                        uploadName: 'design'
                                    },
                                    inspectionGrid, $divStep3);

                                    stepTwoComplete = true;
                                }
                            },
                            error: function (data) {
                                $wizard.CswWizard('button', 'previous', 'click');
                                $.CswDialog('ErrorDialog', data);
                            }
                        });
                    }
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
                    tempFileName = data.tempFileName;

                    $divStep3.empty();
                    var previewGridId = makeId({ prefix: o.ID, ID: +'step' + parseInt(f.stepNo), suffix: 'previewGrid_outer' }),
                        $previewGrid = $divStep3.find('#' + previewGridId),
                        lastSelRow, g, emptyRow = [], colNames = {};

                    $divStep3.append('<p>Inspection Name: ' + selectedInspectionName + '</p>');
                    $divStep3.append('<p>Inspection Target: ' + selectedInspectionTarget + '</p>');
                    $divStep3.append("<p>Verify the results of the upload. Make any necessary edits.</p>");

                    if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                        $previewGrid = $('<div id="' + o.ID + '"></div>').appendTo($divStep3);
                    } else {
                        $previewGrid.empty();
                    }

                    each(data.jqGridOpt.colNames, function (name) {
                        colNames[name] = '';
                    });
                    emptyRow.push(colNames);

                    g = {
                        Id: o.ID,
                        pagermode: 'default',
                        gridOpts: {
                            autowidth: true,
                            rowNum: 20,
                            onSelectRow: function (rowId) {
                                if (rowId && rowId !== lastSelRow) {
                                    inspectionGrid.$gridTable.jqGrid('saveRow', lastSelRow, false, 'clientArray');
                                    lastSelRow = rowId;
                                }
                                inspectionGrid.$gridTable.jqGrid('editRow', rowId, true, '', '', 'clientArray');
                            }
                        },
                        optNav: {
                            add: true,
                            del: true,
                            edit: false,
                            view: false,
                            addfunc: function (rowid) {
                                return inspectionGrid.$gridTable.jqGrid('addRowData', 'new', colNames, 'first');
                            },
                            delfunc: function (rowid) {
                                return inspectionGrid.$gridTable.jqGrid('delRowData', rowid);
                            }
                        },
                        canDelete: true

                    };

                    $.extend(g.gridOpts, data.jqGridOpt);

                    inspectionGrid = new CswGrid(g, $previewGrid);
                };
                uploadTemplate = '<div class="qq-uploader"><div class="qq-upload-drop-area"><span>Drop ' + f.uploadName + ' here to upload</span></div><div class="qq-upload-button">Upload ' + f.uploadName + '</div><ul class="qq-upload-list"></ul></div>';
                var uploader = new qq.FileUploader({
                    element: f.$parent.get(0),
                    multiple: false,
                    action: f.url,
                    template: uploadTemplate,
                    params: f.params,
                    onSubmit: function () {
                        //f.params['InspectionName'] = selectedInspectionName;
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
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'disable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    if (false === isNewInspection) {
                        if (isTrue(forward)) {
                            $wizard.CswWizard('button', 'next', 'enable');
                            $wizard.CswWizard('button', 'next', 'click');
                        } else {
                            $wizard.CswWizard('button', 'previous', 'click');
                        }
                    }
                    else if (false === stepThreeComplete) {
                        $divStep3 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step3.step);
                        stepThreeComplete = true;
                    }
                };
            } ()),

            makeStepFour = (function () {
                var stepFourComplete = false;
                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    if (false === stepFourComplete) {
                        $divStep4 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step4.step);
                        stepFourComplete = true;
                    }
                };
            } ()),

            makeStepFive = (function () {
                var stepFiveComplete = false;
                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'disable');
                    $wizard.CswWizard('button', 'finish', 'enable');
                    $divStep5 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step5.step);

                    if (false === stepFiveComplete) {
                        var newInspectionName = selectedInspectionName,
                            targetInspection = selectedInspectionTarget;

                        CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/uploadInspectionFile',
                            data: {
                                NewInspectionName: newInspectionName,
                                TargetName: targetInspection,
                                TempFileName: tempFileName
                            },
                            success: function (response) {
                                if (response.success == 'true') {
                                    $wizard.CswWizard('button', 'previous', 'disable');
                                    $wizard.CswWizard('button', 'cancel', 'disable');
                                    $divStep5.append("Your design was created successfully");
                                }
                                else {
                                    $divStep5.append("Error: " + response.error.message);
                                }
                            }
                        });
                        stepFiveComplete = true;
                    }
                };
            } ()),

            makeStepSix = (function () {
                var stepSixComplete = false;

                return function () {

                };
            } ()),

            handleNext = function ($wizardTable, newStepNo) {
                switch (newStepNo) {
                    case CswImportInspectionQuestions_WizardSteps.step1.step:
                        // dont think well ever have a step one in next
                        makeStepOne();
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step2.step:
                        makeStepTwo(true);
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step3.step:
                        makeStepThree(true);
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step4.step:
                        makeStepFour();
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step5.step:
                        makeStepFive();
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step6.step:
                        makeStepSix();
                        break;
                } // switch(newstepno)
            }, // handleNext()

            handlePrevious = function (newStepNo) {
                switch (newStepNo) {
                    case CswImportInspectionQuestions_WizardSteps.step1.step:
                        makeStepOne();
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step2.step:
                        makeStepTwo(false);
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step3.step:
                        makeStepThree(false);
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step4.step:
                        makeStepFour();
                        break;
                    case CswImportInspectionQuestions_WizardSteps.step5.step:
                        makeStepFive();
                        break;
                }
            },

            handleFinish = function (ignore) {
            }; //_handleFinish

        //#endregion Variable Declaration

        //#region Execution
        $wizard = $div.CswWizard('init', {
            ID: o.ID + '_wizard',
            Title: 'Create New Inspection',
            StepCount: wizardStepArray.length,
            Steps: wizardSteps,
            StartingStep: o.startingStep,
            FinishText: 'Finish',
            onNext: handleNext,
            onPrevious: handlePrevious,
            onCancel: o.onCancel,
            onFinish: o.onFinish,
            doNextOnInit: false
        });

        // don't activate Save and Finish until step 2
        if (o.startingStep === 1) {
            $wizard.CswWizard('button', 'finish', 'disable');
        }

        makeStepOne();
        //#endregion Execution

        return $div;
    }; // $.fn.CswInspectionDesign_WizardSteps
})(jQuery);

