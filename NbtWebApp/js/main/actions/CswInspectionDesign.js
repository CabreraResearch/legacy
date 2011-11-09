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
            3: CswInspectionDesign_WizardSteps.step3.description
        };

        //var currentStep = o.startingStep;

        var $parent = $(this),
            $div = $('<div></div>').appendTo($parent),
            $wizard,

        // Step 1 - Upload Inspection Design
            $divStep1, selectedInspectionName, selectedInspectionTarget, tempFileName, inspectionGrid, $inspectionName,
        // Step 2 - Preview Inspection Design Grid
            $divStep2,
        // Step 3 - 
            $divStep3,


            makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    $wizard.CswWizard('button', 'previous', 'disable');
                    $wizard.CswWizard('button', 'next', 'disable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    var $uploadCell, $inspectionTarget, $inspectionTable;

                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step1.step);
                        $divStep1.append('<br />');
                        $divStep1.append($('<p><a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Inspection Design template</a></p>'));
                        $divStep1.append('<br />');

                        $inspectionTable = $divStep1.CswTable('init');

                        $inspectionTable.CswTable('cell', 3, 1)
                                        .css({ 'padding': '5px', 'vertical-align': 'middle' })
                                        .CswDiv('init')
                                        .append('Inspection Name:');
                        $inspectionName = $inspectionTable.CswTable('cell', 3, 2)
                            .CswDiv('init')
                            .CswInput('init', {
                                ID: o.ID + '_inspectionName',
                                type: CswInput_Types.text
                            });

                        $inspectionTable.CswTable('cell', 4, 1).append('<br />');

                        $inspectionTable.CswTable('cell', 5, 1)
                                        .css({ 'padding': '5px', 'vertical-align': 'middle' })
                                        .append('Inspection Target:');
                        $inspectionTarget = $inspectionTable.CswTable('cell', 5, 2)
                                                            .CswDiv('init')
                                                            .CswNodeTypeSelect('init', {
                                                                ID: 'step2_nodeTypeSelect',
                                                                objectClassName: 'InspectionTargetClass',
                                                                width: $inspectionName.css('width')
                                                            });

                        $inspectionTable.CswTable('cell', 5, 3)
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

                        $inspectionTable.CswTable('cell', 6, 1)
                                        .append('<br />');

                        $uploadCell = $inspectionTable.CswTable('cell', 7, 1);
                        makeInspectionDesignUpload({
                            url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                            params: {
                                InspectionName: selectedInspectionName
                            },
                            $parent: $uploadCell,
                            onSuccess: function () {
                                selectedInspectionTarget = $inspectionTarget.text();
                                $wizard.CswWizard('button', 'next', 'enable');
                                $wizard.CswWizard('button', 'next', 'click');
                            },
                            stepNo: CswInspectionDesign_WizardSteps.step3.step,
                            uploadName: 'design'
                        });
                    }

                    stepOneComplete = true;
                };
            } ()),

            checkInspectionNameUnique = function (inspectionName) {
                var isUnique = true;
                CswAjaxJson({
                    url: '/NbtWebApp/wsNBT.asmx/IsNewInspectionNameUnique',
                    async: false,
                    data: { 'NewInspectionName': inspectionName },
                    error: function (error) {
                        isUnique = false;
                        $.CswDialog('ErrorDialog', error);
                    },
                    overrideError: true
                });
                return isUnique;
            },

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

                    $divStep2.empty();
                    var previewGridId = makeId({ prefix: o.ID, ID: +'step' + parseInt(f.stepNo), suffix: 'previewGrid_outer' }),
                        $previewGrid = $divStep2.find('#' + previewGridId),
                        lastSelRow, g, emptyRow = [], colNames = {};

                    $divStep2.append('<p>Inspection Name: ' + selectedInspectionName + '</p>');
                    $divStep2.append('<p>Inspection Target: ' + selectedInspectionTarget + '</p>');
                    $divStep2.append("<p>Verify the results of the upload. Make any necessary edits.</p>");

                    if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                        $previewGrid = $('<div id="' + o.ID + '"></div>').appendTo($divStep2);
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
                            },
                            height: 'auto'
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
                        }
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
                        $('.qq-upload-list').empty();
                        selectedInspectionName = $inspectionName.val().trim();
                        return checkInspectionNameUnique(selectedInspectionName);
                    },
                    onComplete: function (id, fileName, data) {
                        onComplete(id, fileName, data);
                    },
                    showMessage: function (error) {
                        $.CswDialog('ErrorDialog', error);
                    }
                });
            },

            makeStepTwo = (function () {
                var stepTwoComplete = false;
                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'enable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    if (false === stepTwoComplete) {
                        $divStep2 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step2.step);
                        stepTwoComplete = true;
                    }
                };
            } ()),

            makeStepThree = (function () {
                var stepThreeComplete = false;
                return function () {
                    $wizard.CswWizard('button', 'previous', 'enable');
                    $wizard.CswWizard('button', 'next', 'disable');
                    $wizard.CswWizard('button', 'finish', 'disable');
                    if (false === stepThreeComplete) {
                        $divStep3 = $wizard.CswWizard('div', CswInspectionDesign_WizardSteps.step3.step);
                        stepThreeComplete = true;
                    }
                };
            } ()),

            handleNext = function ($wizardTable, newStepNo) {
                switch (newStepNo) {
                    case CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo();
                        break;
                    case CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree();
                        break;
                } // switch(newstepno)
            }, // handleNext()

            handlePrevious = function (newStepNo) {
                switch (newStepNo) {
                    case CswInspectionDesign_WizardSteps.step1.step:
                        makeStepOne();
                        break;
                    case CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo();
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
            StepCount: CswInspectionDesign_WizardSteps.stepcount,
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

