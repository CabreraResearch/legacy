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
            onFinish: o.onFinish,
            doNextOnInit: false
        });

        // don't activate Save and Finish until step 2
        if (o.startingStep === 1) {
            $wizard.CswWizard('button', 'finish', 'disable');
        }

        // Step 1 - Create or Select Inspection Design
        var $divStep1, $createNewInspect, $chooseExistingInspect, selectedInspectionName,
            isNewInspection = true,
        // Step 2 - Upload Inspection Design
            $divStep2, targetName, $step2Table, $addNodeType, $inspectionTarget, $inspectDesignUpload,
        // Step 3 - Preview Inspection Design Grid
            $divStep3, instructions3, $textBoxTempFileName, inspectDesignGrid,
        // step 4 - Import Inspection Points
            $divStep4, $inspectPointUpload,
        // step 5 - Preview results
            $divStep5, inspectPointGrid,
        // step 6
            $divStep6;

        var makeStepOne = (function () {
            var stepOneComplete = false;

            return function () {
                $wizard.CswWizard('button', 'previous', 'disable');
                $wizard.CswWizard('button', 'next', 'enable');
                $wizard.CswWizard('button', 'finish', 'disable');
                var $newInspectDiv = '',
                    $inspectionLabel, $inspectionName, $selectLabel, $selectName, $radioTable;

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
                    $chooseExistingInspect = $radioTable.CswTable('cell', 5, 1).CswInput('init', {
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
                    $radioTable.CswTable('cell', 5, 2).append('Select an Existing Inspection');
                    $selectName = '';
                }
                stepOneComplete = true;
            };
        } ());

        makeStepOne();

        var makeStepTwo = (function () {
            var stepTwoComplete = false;
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
                else if (false === stepTwoComplete) {
                    $divStep2 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step2.step);

                    targetName = $("<br/><br/>Please enter the target of the new inspection:<br />").appendTo($divStep2);
                    $step2Table = $divStep2.CswTable();
                    $addNodeType = $step2Table.CswTable('cell', 1, 2).CswDiv('init');
                    $inspectionTarget = $step2Table.CswTable('cell', 1, 1).CswDiv('init').CswNodeTypeSelect('init', { ID: 'step2_nodeTypeSelect', objectClassName: 'InspectionTargetClass' });

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

                    instructions3 = $("<br/><br/>Please select your Excel file containing your inspection questions.<br/><br/>").appendTo($divStep3);
                    $inspectDesignUpload = $('<div></div>');

                    var fileUpOpts = {
                        url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                        params: {
                            InspectionName: selectedInspectionName
                        },
                        onSuccess: function (id, fileName, data) {
                            var onStepThreeComplete = function () {
                                $divStep4.empty();
                                $textBoxTempFileName = $divStep4.CswInput('init', {
                                    ID: 'step4_tempFileName',
                                    type: CswInput_Types.hidden,
                                    value: data.tempFileName
                                });
                                var $newInspectionName = selectedInspectionName,
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
                                                inspectDesignGrid.$gridTable.jqGrid('saveRow', lastSelRow, false, 'clientArray');
                                                lastSelRow = rowId;
                                            }
                                            inspectDesignGrid.$gridTable.jqGrid('editRow', rowId, true, '', '', 'clientArray');
                                        }
                                    },
                                    optNav: {
                                        add: true,
                                        del: true,
                                        edit: false,
                                        view: false,
                                        addfunc: function (rowid) {
                                            return inspectDesignGrid.$gridTable.jqGrid('addRowData', 'new', [{ 'SECTION': '', 'QUESTION': '', 'ALLOWED ANSWERS': '', 'COMPLIANT ANSWERS': '', 'HELP TEXT': ''}], 'first');
                                        }
                                    },
                                    canDelete: true

                                };

                                $.extend(g.gridOpts, data.jqGridOpt);

                                inspectDesignGrid = new CswGrid(g, $previewGrid);
                            };

                            $wizard.CswWizard('button', 'next', 'enable');
                            $wizard.CswWizard('button', 'next', 'click');
                            onStepThreeComplete();
                        }
                    };

                    var uploader = new qq.FileUploader({
                        element: $inspectDesignUpload.get(0),
                        action: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                        params: fileUpOpts.params,
                        onSubmit: function () {
                            fileUpOpts.params['InspectionName'] = selectedInspectionName;
                        },
                        onComplete: function (id, fileName, data) {
                            fileUpOpts.onSuccess(id, fileName, data);
                        },
                        showMessage: function (error) {
                            $.CswDialog('ErrorDialog', error);
                        }
                    });
                    $divStep3.append($inspectDesignUpload);
                    stepThreeComplete = true;
                }

                if (isNewInspection) {
                    CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/IsNewInspectionNameUnique',
                            data: { 'NewInspectionName': selectedInspectionName },
                            success: function(data) {

                            },
                            error: function(data) {
                                $wizard.CswWizard('button', 'previous', 'click');
                                $.CswDialog('ErrorDialog', data);
                            }
                        });
                }
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
                log(inspectDesignGrid.$gridTable.jqGrid('jqGridExport', 'jsonstring'));

                $wizard.CswWizard('button', 'previous', 'enable');
                $wizard.CswWizard('button', 'next', 'disable');
                $wizard.CswWizard('button', 'finish', 'enable');
                $divStep5 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step5.step);

                if (false === stepFiveComplete) {
                    var newInspectionName = selectedInspectionName,
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

        var makeStepSix = (function () {
            var stepSixComplete = false;

            return function () {

            };

        } ());

        function handleNext($wizardTable, newStepNo) {
            //currentStep = newStepNo;
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
        } // handleNext()

        function handlePrevious(newStepNo) {
            //currentStep = newStepNo;
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
        }

        function handleFinish(ignore) {
        } //_handleFinish

        return $div;
    }; // $.fn.CswInspectionDesign_WizardSteps
})(jQuery);

