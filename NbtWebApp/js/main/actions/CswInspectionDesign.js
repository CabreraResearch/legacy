/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../controls/CswNodeTypeSelect.js" />
/// <reference path="../pagecmp/CswWizard.js" />
/// <reference path="../controls/CswGrid.js" />

(function ($) { /// <param name="$" type="jQuery" />

    $.fn.CswInspectionDesign = function (options) {
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

        var wizardStepArray = [CswImportInspectionQuestions_WizardSteps.step1, CswImportInspectionQuestions_WizardSteps.step2, CswImportInspectionQuestions_WizardSteps.step3, CswImportInspectionQuestions_WizardSteps.step4, CswImportInspectionQuestions_WizardSteps.step5],
            wizardSteps = {
                1: CswImportInspectionQuestions_WizardSteps.step1.description,
                2: CswImportInspectionQuestions_WizardSteps.step2.description,
                3: CswImportInspectionQuestions_WizardSteps.step3.description,
                4: CswImportInspectionQuestions_WizardSteps.step4.description,
                5: CswImportInspectionQuestions_WizardSteps.step5.description
            };

        //var currentStep = o.startingStep;

        var $parent = $(this);
        var $div = $('<div></div>')
                        .appendTo($parent);

        var $wizard = $div.CswWizard('init', {
            ID: o.ID + '_wizard',
            Title: 'Create New Inspection',
            StepCount: wizardStepArray.length,
            Steps: wizardSteps,
            StartingStep: o.startingStep,
            FinishText: 'Finish',
            onNext: handleNext,
            onPrevious: handlePrevious,
            onCancel: o.onCancel,
            onFinish: o.onFinish
        });

        // don't activate Save and Finish until step 2
        if (o.startingStep === 1) {
            $wizard.CswWizard('button', 'finish', 'disable');
        }

        // Step 1 - Download the Excel template
        var $divStep1 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step1.step),
            downloadButton = $("<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Excel template</a>").appendTo($divStep1),
        // Step 2 - Get new inspection name and target type
            $divStep2, $inspectionLabel, $inspectionName, $errorLabel, targetName, $table, $addNodeType, $inspectionTarget,
        // step 3 - Upload file
            $divStep3, instructions3, $fileUploadDiv,
        // step 4 - Preview results
            $divStep4, $textBoxTempFileName, grid,
        // step 5
            $divStep5;

        var makeStepTwo = (function () {
            var stepTwoComplete = false;
            return function () {
                $wizard.CswWizard('button', 'previous', 'enable');
                $wizard.CswWizard('button', 'next', 'disable');
                $wizard.CswWizard('button', 'finish', 'disable');
                if (false === stepTwoComplete) {
                    $divStep2 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step2.step);

                    $inspectionLabel = $("<br/><br/>Please enter the name of the new inspection:<br />").appendTo($divStep2);
                    $inspectionName = $divStep2.CswInput('init', {
                        ID: o.ID + '_inspectionName',
                        type: CswInput_Types.text
                    });
                    $inspectionName.keypress(function () {
                        setTimeout(function () {
                            if (false === isNullOrEmpty(tryParseString($inspectionName.val()).trim())) {
                                $wizard.CswWizard('button', 'next', 'enable');
                            } else {
                                $wizard.CswWizard('button', 'next', 'disable');
                            }
                        }, 100);
                    });
                    $errorLabel = $('<div ID="inspectionNameErrorLabel" style="visibility:hidden">ERROR: inspection name is NOT unique.</div>').appendTo($divStep2);
                    targetName = $("<br/><br/>Please enter the target of the new inspection:<br />").appendTo($divStep2);
                    $table = $divStep2.CswTable();
                    $addNodeType = $table.CswTable('cell', 1, 2).CswDiv('init');
                    $inspectionTarget = $table.CswTable('cell', 1, 1).CswDiv('init').CswNodeTypeSelect('init', { ID: 'step2_nodeTypeSelect', objectClassName: 'InspectionTargetClass' });

                    $addNodeType.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                        AlternateText: "Create New Inspection Target",
                        onClick: function ($ImageDiv) {
                            $.CswDialog('AddNodeTypeDialog', {
                                objectclassid: $inspectionTarget.find(':selected').data('objectClassId'),
                                $select: $inspectionTarget,
                                nodeTypeDescriptor: 'Inspection Target Type'
                            });
                            return CswImageButton_ButtonType.None;
                        }
                    });
                    stepTwoComplete = true;
                }
            };
        } ());

        var makeStepThree = (function () {
            var stepThreeComplete = false;
            return function () {
                $wizard.CswWizard('button', 'previous', 'enable');
                $wizard.CswWizard('button', 'next', 'disable');
                $wizard.CswWizard('button', 'finish', 'disable');

                if (false === stepThreeComplete) {
                    $divStep3 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step3.step);

                    instructions3 = $("<br/><br/>Please select your Excel file containing your inspection questions.<br/><br/>").appendTo($divStep3);
                    $fileUploadDiv = $('<div></div>');

                    var fileUpOpts = {
                        url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                        params: {
                            InspectionName: $inspectionName.val()
                        },
                        onSuccess: function (id, fileName, data) {
                            var onStepThreeComplete = function () {
                                $divStep4.empty();
                                $textBoxTempFileName = $divStep4.CswInput('init', {
                                    ID: 'step4_tempFileName',
                                    type: CswInput_Types.hidden,
                                    value: data.tempFileName
                                });
                                var $newInspectionName = $inspectionName.val(),
                                    $targetName = $inspectionTarget.find(':selected').text(),
                                    step4PreviewGridId = o.ID + '_step4_previewGrid_outer',
                                    $previewGrid = $div.find('#' + step4PreviewGridId),
                                    lastSelRow, g;

                                $divStep4.append("Inspection Name: ");
                                $divStep4.append($newInspectionName);
                                $divStep4.append("<br/><br/>Target: ");
                                $divStep4.append($targetName);
                                $divStep4.append("<br/><br/>Your data from the Excel spreadsheet is shown below.  If this all looks correct then click the 'Save and Finish' button to create the new inspection.<br/><br/>");

                                if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                                    $previewGrid = $('<div id="' + o.ID + '"></div>').appendTo($divStep4);
                                }
                                else {
                                    $previewGrid.empty();
                                }

                                g = {
                                    Id: o.ID,
                                    pagermode: 'default',
                                    gridOpts: {
                                        autowidth: true,
                                        rowNum: 20,
                                        onSelectRow: function (rowId) {
                                            if (rowId && rowId !== lastSelRow) {
                                                grid.$gridTable.jqGrid('saveRow', lastSelRow, false, 'clientArray');
                                                lastSelRow = rowId;
                                            }
                                            grid.$gridTable.jqGrid('editRow', rowId, true, '', '', 'clientArray');
                                        }
                                    },
                                    optNav: {
                                        add: true,
                                        del: true,
                                        edit: false,
                                        view: false
                                    },
                                    canDelete: true

                                };

                                $.extend(g.gridOpts, data.jqGridOpt);

                                grid = new CswGrid(g, $previewGrid);
                            };

                            $wizard.CswWizard('button', 'next', 'enable');
                            $wizard.CswWizard('button', 'next', 'click');
                            onStepThreeComplete();
                        }
                    };

                    var uploader = new qq.FileUploader({
                        element: $fileUploadDiv.get(0),
                        action: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                        params: fileUpOpts.params,
                        onSubmit: function () {
                            fileUpOpts.params['InspectionName'] = $inspectionName.val();
                        },
                        onComplete: function (id, fileName, data) {
                            fileUpOpts.onSuccess(id, fileName, data);
                        },
                        showMessage: function (error) {
                            $.CswDialog('ErrorDialog', error);
                        }
                    });
                    $divStep3.append($fileUploadDiv);
                    stepThreeComplete = true;
                }
                CswAjaxJson({
                    url: '/NbtWebApp/wsNBT.asmx/IsNewInspectionNameUnique',
                    data: { 'NewInspectionName': $inspectionName.val() },
                    success: function (data) {
                        
                    },
                    error: function (data) {
                        $wizard.CswWizard('button', 'previous', 'click');
                        $.CswDialog('ErrorDialog', data); 
                    }
                });
            };
        } ());

        function makeStepFour() {
            $wizard.CswWizard('button', 'previous', 'enable');
            $wizard.CswWizard('button', 'next', 'enable');
            $wizard.CswWizard('button', 'finish', 'disable');
            $divStep4 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step4.step);
        }

        var makeStepFive = (function () {
            var stepFiveComplete = false;
            return function () {
                log(grid.$gridTable.jqGrid('jqGridExport', 'jsonstring'));

                $wizard.CswWizard('button', 'previous', 'enable');
                $wizard.CswWizard('button', 'next', 'disable');
                $wizard.CswWizard('button', 'finish', 'enable');
                $divStep5 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step5.step);

                if (false === stepFiveComplete) {
                    var newInspectionName = $inspectionName.val(),
                        targetInspection = $inspectionTarget.find(':selected').text(),
                        tempFileName = $textBoxTempFileName.val();

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
        } ());

        function handleNext($wizardTable, newStepNo) {
            //currentStep = newStepNo;
            switch (newStepNo) {
                case CswImportInspectionQuestions_WizardSteps.step1.step:
                    // dont think well ever have a step one in next
                    $wizardTable.CswWizard('button', 'previous', 'disable');
                    $wizardTable.CswWizard('button', 'next', 'enable');
                    $wizardTable.CswWizard('button', 'finish', 'disable');
                    break;
                case CswImportInspectionQuestions_WizardSteps.step2.step:
                    makeStepTwo();
                    break;
                case CswImportInspectionQuestions_WizardSteps.step3.step:
                    makeStepThree();
                    break;
                case CswImportInspectionQuestions_WizardSteps.step4.step:
                    makeStepFour();
                    break;
                case CswImportInspectionQuestions_WizardSteps.step5.step:
                    makeStepFive();
                    break;
            } // switch(newstepno)
        } // handleNext()

        function handlePrevious($wizardTable, newStepNo) {
            //currentStep = newStepNo;
            switch (newStepNo) {
                case CswImportInspectionQuestions_WizardSteps.step1.step:
                    $wizard.CswWizard('button', 'previous', 'disable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    break;
                case CswImportInspectionQuestions_WizardSteps.step2.step:
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    break;
                case CswImportInspectionQuestions_WizardSteps.step3.step:
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    break;
                case CswImportInspectionQuestions_WizardSteps.step4.step:
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    break;
            }
        }

        function handleFinish(ignore) {
        } //_handleFinish

        return $div;
    }; // $.fn.CswInspectionDesign_WizardSteps
})(jQuery);

