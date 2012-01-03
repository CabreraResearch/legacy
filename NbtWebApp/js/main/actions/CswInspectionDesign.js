/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
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

        var $parent = $(this),
            $div = $('<div></div>').appendTo($parent),
            $wizard,
            currentStepNo = o.startingStep,
            buttons = {
                next: 'next',
                prev: 'previous',
                finish: 'finish',
                cancel: 'cancel'
            },

        // Step 4 - Select or Create Inspection Target
            $divStep1, selectedInspectionTarget, $inspectionTarget, $addNewTarget, $categoryName,
        // Step 2 - Select or Create Inspection Design
            selectedInspectionDesign = { id: '[Create New]', name: '[Create New]' },
            $divStep2, categoryName, $inspectionDesignSelect, $newDesignName, 
        // Step 3 - Upload Inspection Design
            $divStep3, gridIsPopulated = false,
        // Step 4 - Review and Revise Inspection Design
            $divStep4, inspectionGrid, gridOptions,
        
        // Step 5 - Review and Finish
            $divStep5, //inspectionTargetGroups = { }, newSchedules = { }, $scheduleList,

            isNewInspectionDesign = function() {
                return ('[Create New]' === selectedInspectionDesign.id);
            },
            
            isNewTarget = (function () {
                var ret = false;
                return function (isNew) {
                    if (arguments.length > 0) {
                        ret = isTrue(isNew);
                    }
                    return ret;
                };    
            }()),
            
            toggleButton = function (button, isEnabled, doClick) {
                var $btn;
                if(isTrue(isEnabled)) {
                    $btn = $wizard.CswWizard('button', button, 'enable');
                    if (isTrue(doClick)) {
                        $btn.click();
                    }
                } else {
                    $wizard.CswWizard('button', button, 'disable');
                }
                return false;
            },
            
            makeStepId = function (suffix, stepNo) {
                var step = stepNo || currentStepNo;
                return makeId({ prefix: 'step_' + step, ID: o.ID, suffix: suffix });
            },
            
            //Step 1. Select an Inspection Target.
            makeStepOne = (function() {
                var stepOneComplete = false,
                    lastSelectedInspectionName = selectedInspectionDesign.name,
                    $inspectionTable, $addBtn, $rowOneTable;
                return function() {
                    //For tomorrow: remmeber that Target only affects View creation--we're using Object Classes as the target of relationships
                    //In the case of creating a new Target, we're maintaing Object Class loyalty even as we create new Node Types
                    var onNodeTypeSelectSuccess = function(data) {
                        if (data.nodetypecount === 0) { //Add a new Target
                            $inspectionTarget.hide();
                            isNewTarget(true);
                            $addNewTarget = $rowOneTable.CswTable('cell', 2, 2)
                                .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                .CswInput('init', {
                                    ID: o.ID + '_newTargetName',
                                    value: '',
                                    maxlength: 40
                                })
                                .CswAttrXml('maxlength', 40)
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

                            $addBtn = $addBtn || $rowOneTable.CswTable('cell', 2, 3)
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
                                                                            category: 'do not show',
                                                                            $select: $inspectionTarget,
                                                                            nodeTypeDescriptor: 'Target',
                                                                            maxlength: 40,
                                                                            onSuccess: function(newData) {
                                                                                var proposedInspectionTarget = newData.nodetypename;
                                                                                if( checkTargetIsClientSideUnique(proposedInspectionTarget) ) {
                                                                                    selectedInspectionTarget = proposedInspectionTarget;
                                                                                    isNewTarget(true);
                                                                                    lastSelectedInspectionName = selectedInspectionDesign.name;
                                                                                    $wizard.CswWizard('button', 'next', 'enable');
                                                                                } else {
                                                                                    $inspectionTarget.find('option[value="' + proposedInspectionTarget + '"]').remove();
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

                        if(false === isNullOrEmpty($inspectionTarget, true)) {
                            $inspectionTarget.remove();
                        }

                        $inspectionTarget = $rowOneTable.CswTable('cell', 2, 1)
                                                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                                            .CswDiv('init')
                                                            .CswNodeTypeSelect('init', {
                                                                ID: makeStepId('nodeTypeSelect'),
                                                                objectClassName: 'InspectionTargetClass',
                                                                onSelect: function() {
                                                                    var $this = $inspectionTarget.find(':selected');
                                                                    isNewTarget($this.CswAttrXml('data-newNodeType'));
                                                                    selectedInspectionTarget = $this.text();
                                                                    lastSelectedInspectionName = selectedInspectionDesign.name;
                                                                },
                                                                onSuccess: function(data) {
                                                                    onNodeTypeSelectSuccess(data);
                                                                    selectedInspectionTarget = $inspectionTarget.find(':selected').text();
                                                                }
                        });
                    };
                    
                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step1.step);
                        $divStep1.append('<br />');

                        $inspectionTable = $divStep1.CswTable('init', {
                            ID: makeStepId('setInspectionTargetTable')
                        });

                        $rowOneTable = $inspectionTable.CswTable('cell', 1, 1)
                                                        .CswTable({
                                                            FirstCellRightAlign: true
                                                        });
                        
                        $rowOneTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>What do you want to inspect?</span>');
        
                        makeTargetSelect();
                        lastSelectedInspectionName = selectedInspectionDesign.name;
                        stepOneComplete = true;
                    } // if (false === stepOneComplete)
                    else if(lastSelectedInspectionName !== selectedInspectionDesign.name) {
                        //In this case, we've navigated back, changed Step 1 options and moved forward. We must refresh.
                        makeTargetSelect();
                    }
                    
                    toggleButton(buttons.prev, false);
                    toggleButton(buttons.next, (false === isNullOrEmpty(selectedInspectionTarget)));
                };
            }()),

            //Step 2. Select an Inspection Design Design.
            makeStepTwo = (function() {
                var stepTwoComplete = false;

                return function() {
                    var $inspectionTable, $categoryLabel, $newDesignLabel, $newDesignNameDisplay,
                        tempInspectionName = selectedInspectionTarget.toUpperCaseEachWord() + ' Inspection',
                        tempCategoryName = tempInspectionName + ': ' + selectedInspectionTarget;

                    var makeInspectionDesignName = function(name) {
                        var ret = trim(tryParseString(name));
                        if(-1 === ret.indexOf('Inspection') && -1 === ret.indexOf('inspection')) {
                            ret += ' Inspection';
                        }
                        return ret;
                    };

                    var toggleNewDesignName = function() {
                        if (isNewInspectionDesign()) {
                            $newDesignName.show();
                            $newDesignLabel.show();
                            $newDesignNameDisplay.show();
                        } else {
                            $newDesignName.hide();
                            $newDesignLabel.hide();
                            $newDesignNameDisplay.hide();
                        }
                    };
                    var nextBtnEnabled = function() {
                        return (false === isNullOrEmpty(selectedInspectionDesign.name));
                    };
                    
                    toggleButton(buttons.prev, true);
                    toggleButton(buttons.finish, false);
                    toggleButton(buttons.next, nextBtnEnabled());
                    
                    if (false === stepTwoComplete) {
                        $divStep2 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step2.step);
                        $divStep2.append('<br />');

                        $inspectionTable = $divStep2.CswTable('init', {
                            ID: makeSafeId('inspectionTable'),
                            FirstCellRightAlign: true
                        });

                        //1. Copy from Inspection Design
                        $inspectionTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>Select an Inspection Design&nbsp</span>');

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
                                if(isNewInspectionDesign()) {
                                    tempCategoryName = (selectedInspectionTarget + ': ' + selectedInspectionTarget).toUpperCaseEachWord();
                                } else {
                                    tempCategoryName = (selectedInspectionDesign.name + ': ' + selectedInspectionTarget).toUpperCaseEachWord();
                                }
                                $categoryName.val(tempCategoryName);

                                toggleNewDesignName();
                            });
                        //Create New is selected by default
                        selectedInspectionDesign.id = $inspectionDesignSelect.find(':selected').val();
                        selectedInspectionDesign.name = makeInspectionDesignName(selectedInspectionTarget);
                        
                        $inspectionTable.CswTable('cell', 2, 1).append('<br />');

                        //2. New Inspection Design Name
                        $newDesignLabel = $('<span class="required">New Inspection Design Name&nbsp</span>')
                                                .appendTo($inspectionTable.CswTable('cell', 3, 1))
                                                .css({ 'padding': '1px', 'vertical-align': 'middle' });

                        $newDesignName = $inspectionTable.CswTable('cell', 3, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .CswInput('init', {
                                ID: o.ID + '_newDesignName',
                                type: CswInput_Types.text,
                                cssclass: 'required',
                                maxlength: 50,
                                value: tempInspectionName
                            })
                            .on('keypress keydown keyup', function() {
                                setTimeout(function() {
                                    var newInspectionDesignName = makeInspectionDesignName($newDesignName.val());
                                    selectedInspectionDesign.id = '[Create New]';
                                    selectedInspectionDesign.name = newInspectionDesignName;

                                    $newDesignNameDisplay.text(newInspectionDesignName);
                                    tempCategoryName = (newInspectionDesignName + ': ' + selectedInspectionTarget).toUpperCaseEachWord();
                                    $categoryName.val(tempCategoryName);
                                    
                                    toggleButton(buttons.next, nextBtnEnabled());
                                }, 10);
                            });
                        
                        $newDesignNameDisplay = $('<span>' + tempInspectionName + '</span>').appendTo( 
                                                                $inspectionTable.CswTable('cell', 4, 2)
                                                                                .css({ 'padding': '1px', 'vertical-align': 'middle' }));

                        $inspectionTable.CswTable('cell', 5, 1).append('<br />');

                         //2. Category Name
                        $categoryLabel = $categoryLabel || $inspectionTable.CswTable('cell', 6, 1)
                                                                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                                                            .append('<span>Category Name&nbsp</span>');

                        $categoryName = $categoryName || $inspectionTable.CswTable('cell', 6, 2)
                                                                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                                                                        .CswInput('init', {
                                                                            ID: o.ID + '_newDesignCategory',
                                                                            type: CswInput_Types.text,
                                                                            value: tempCategoryName
                                                                        });

                        toggleNewDesignName();
                        
                        $inspectionTable.CswTable('cell', 6, 1).append('<br />');
                    }
                    stepTwoComplete = true;
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
                            toggleButton(buttons.next);
                            toggleButton(buttons.prev, true);
                        }
                    });
            },

            //File upload onSuccess event to prep Step 4
            makeInspectionDesignGrid = function(data, onSuccess) {
                if (isFunction(onSuccess)) {
                    onSuccess();
                }
                gridIsPopulated = true;
                
                //This is ugly. Abstract the step div from this function.
                $divStep4.empty();
                var previewGridId = makeStepId('previewGrid_outer', 4),
                    $previewGrid = $divStep4.find('#' + previewGridId);

                $divStep4.append('<p>Review the upload results. Make any necessary edits.</p>');

                if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
                    $previewGrid = $('<div id="' + previewGridId + '"></div>').appendTo($divStep4);
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

                inspectionGrid = CswGrid(gridOptions, $previewGrid);
            },

            //File upload button for Step 3
            makeInspectionDesignUpload = function($control) {
                var f = {
                    url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
                    onSuccess: function() {
                        $wizard.CswWizard('button', 'next', 'enable').click();
                    },
                    stepNo: ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step,
                    uploadName: 'design'
                };
               
                $control.fileupload({
                    datatype: 'json',
                    url: f.url,
                    paramName: 'fileupload',
                    done: function (e, ret) {
                        var gridData, $resultDoc, $firstChild;
                        //See case 24511. This is ugly, but there's not much else to be done.
                        if(false === isNullOrEmpty(ret.result)) {
                            $resultDoc = $(ret.result);
                            $firstChild = $resultDoc.find(':first-child');
                            if(false === isNullOrEmpty($firstChild) &&
                                false === isNullOrEmpty($firstChild.text())) {
                                gridData = JSON.parse($firstChild.text());
                                makeInspectionDesignGrid(gridData, f.onSuccess);
                            }
                        }
                    }
                });
            },

            //If this is a new Design, upload the template. Otherwise skip to step 5.
            makeStepThree = (function() {
                var stepThreeComplete = false;

                return function(forward) {
                    //this is somewhat dundant, but these calls are cheap and it improves readability until we have time to tighten our shot group
                    var nextIsEnabled = function() {
                        return gridIsPopulated || false === isNewInspectionDesign();
                    };
                    var doNextClick = function() {
                        return false === isNewInspectionDesign() && forward;
                    };
                    var doPrevClick = function() {
                        return false === isNewInspectionDesign() && false === isTrue(forward);
                    };
                   
                    var doStepThree = function() {
                        var $step3List, $templateLink, $uploadP;

                        if (false === stepThreeComplete) {
                            $divStep3 = $divStep3 || $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step);
                            $divStep3.empty();
                            
                            //Ordered instructions
                            $step3List = $divStep3.CswList('init', {
                                ID: makeStepId('uploadTemplateList'),
                                ordered: true
                            });

                            //1. Download template
                            $templateLink = $('<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download Template</a>').button();
                            $step3List.CswList('addItem', {
                                value: $('<span>Create a new <b>' + selectedInspectionDesign.name + '</b> Design using the Excel template.</span>')
                                    .append($('<p/>').append($templateLink))
                            });

                            //2. Edit the template.
                            $step3List.CswList('addItem', {
                                value: $('<span>Edit the Inspection template.</span>')
                            });

                            //3. Upload the design
                            $uploadP = $('<input id="' + makeStepId('fileUploadBtn') + '" type="file" name="fileupload" />');
                            makeInspectionDesignUpload($uploadP);

                            $step3List.CswList('addItem', {
                                value: $('<span>Upload the completed InspectionDesign.<p/></span>').append($uploadP)
                            });

                            //$fileUploadBtn.hide();
                            //stepThreeComplete = true;
                        }
                    }; //doStepTwo

                    if(isNewInspectionDesign()) {
                        //selectedInspectionDesign.name = $newDesignName.val();
                        checkIsNodeTypeNameUnique(selectedInspectionDesign.name, doStepThree);
                    }
                    toggleButton(buttons.next, nextIsEnabled(), doNextClick());
                    toggleButton(buttons.prev, true, doPrevClick());
                };
            }()),

            //Step 4. Review the Design grid.
            makeStepFour = (function() {
                var stepFourComplete = false;
                //We populate this step as the result of the async design upload. Improve the readability of this code when you next visit.
                return function(forward) {
                    var skipStepFour = false;
                    var doNextClick = function() {
                        skipStepFour = (false === isNewInspectionDesign() && forward);
                        return skipStepFour;
                    };
                    var doPrevClick = function() {
                        skipStepFour = (false === isNewInspectionDesign() && false == isTrue(forward));
                        return skipStepFour;
                    };

                    toggleButton(buttons.next, true, doNextClick());
                    toggleButton(buttons.prev, true, doPrevClick());
                    
                    if (false === stepFourComplete &&
                            false === skipStepFour) {
                        $divStep4 = $wizard.CswWizard('div', ChemSW.enums.CswInspectionDesign_WizardSteps.step4.step);
                        stepFourComplete = true;
                    }
                };
            }()),

            checkTargetIsClientSideUnique = function (proposedTargetName) {
                var ret = false,
                    targetName = proposedTargetName || selectedInspectionTarget;
                if( trim(targetName).toLowerCase() != trim(selectedInspectionDesign.name).toLowerCase() ) {
                    ret = true;
                } else {
                    $.CswDialog('ErrorDialog', ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'An Inspection Design and an Inspection Target cannot have the same name.', 'Attempted to create Inspection Target ' + targetName + ' against Inspection Design ' + selectedInspectionDesign.name));
                }
                return ret;
            },

            //Step 5. Preview and Finish.
            makeStepFive = (function() {

                return function() {
                    var $confirmationList, $confirmTypesList, $confirmViewsList, $confirmationDesign, confirmGridOptions = { };

                    if(checkTargetIsClientSideUnique()) {

                        toggleButton(buttons.prev, true);
                        toggleButton(buttons.next, false);
                        toggleButton(buttons.finish, true);

                        categoryName = $categoryName.val();

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
                            confirmGridOptions.gridOpts.data = inspectionGrid.$gridTable.jqGrid('getRowData');
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
                                value: 'Creating a new Inspection Design <b>' + selectedInspectionDesign.name + '</b>.'
                            });

                            CswGrid(confirmGridOptions, $confirmationDesign);
                        } else {
                            $confirmationList.CswList('addItem', {
                                value: 'Assigning Inspection Design <b>' + selectedInspectionDesign.name + '</b> to Inspection Target <b> ' + selectedInspectionTarget + '</b>.'
                            });
                        }

                        if (isNewInspectionDesign() || isNewTarget()) {
                            $confirmTypesList = $confirmationList.CswList('addItem', {
                                value: 'New Types'
                            })
                                .CswList('init', {
                                    ID: makeStepId('confirmationTypes')
                                });

                            if (isNewInspectionDesign()) {
                                $confirmTypesList.CswList('addItem', {
                                    value: 'New Inspection Design <b>' + selectedInspectionDesign.name + '</b> on Inspection Target <b>' + selectedInspectionTarget + '</b>'
                                });
                            }

                            if (isNewTarget) {
                                $confirmTypesList.CswList('addItem', {
                                    value: 'New Inspection Target <b>' + selectedInspectionTarget + '</b>'
                                });

                                $confirmTypesList.CswList('addItem', {
                                    value: 'New Inspection Target Group <b>' + selectedInspectionTarget + ' Group</b>'
                                });
                            }
                        }

                        $confirmViewsList = $confirmationList.CswList('addItem', {
                            value: 'New Views'
                        })
                            .CswList('init', {
                                ID: makeStepId('confirmationViews')
                            });
                        $confirmViewsList.CswList('addItem', {
                            value: '<b>Scheduling, ' + selectedInspectionDesign.name + ': ' + selectedInspectionTarget + '</b>'
                        });
                        $confirmViewsList.CswList('addItem', {
                            value: '<b>Groups, ' + selectedInspectionDesign.name + ': ' + selectedInspectionTarget + '</b>'
                        });
                        $confirmViewsList.CswList('addItem', {
                            value: '<b>Inspections, ' + selectedInspectionDesign.name + ': ' + selectedInspectionTarget + '</b>'
                        });
                    } else {
                        toggleButton(buttons.prev, true, true);
                    }
                };
            }()),

            handleNext = function($wizardTable, newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo(); 
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree(true); //we're moving forward
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step4.step:
                        makeStepFour(true); //we're moving forward
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step5.step:
                        makeStepFive();
                        break;
                } // switch(newstepno)
            }, // handleNext()

            handlePrevious = function(newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step1.step:
                        makeStepOne();
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step2.step:
                        makeStepTwo(); //we're moving backward
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step3.step:
                        makeStepThree(false); //we're moving backward
                        break;
                    case ChemSW.enums.CswInspectionDesign_WizardSteps.step4.step:
                        makeStepFour(false); 
                        break;
                }
            },

            onFinish = function() {
                var designGrid = '';

                toggleButton(buttons.prev, false);
                toggleButton(buttons.next, false);
                toggleButton(buttons.finish, false);
                toggleButton(buttons.cancel, false);
                
                if (false === isNullOrEmpty(inspectionGrid)) {
                    designGrid = JSON.stringify(inspectionGrid.getAllGridRows());
                }

                var jsonData = {
                    DesignGrid: designGrid,
                    InspectionDesignName: tryParseString(selectedInspectionDesign.name),
                    InspectionTargetName: tryParseString(selectedInspectionTarget),
                    IsNewInspection: isNewInspectionDesign(),
                    IsNewTarget: isNewTarget(),
                    Category: tryParseString(categoryName)
                };

                CswAjaxJson({
                        url: '/NbtWebApp/wsNBT.asmx/finalizeInspectionDesign',
                        data: jsonData,
                        success: function(data) {
                            //Come back and hammer this out
                            var views = data.views,
                                values = [];

                            each(views, function(thisView) {
                                if(contains(thisView, 'viewid') && 
                                        contains(thisView,'viewname')) {
                                    values.push({
                                            value: thisView.viewid,
                                            display: thisView.viewname
                                        });
                                }
                            });
                            
                            $.CswDialog('NavigationSelectDialog', {
                                ID: makeSafeId('FinishDialog'),
                                title: 'The Inspection Design Wizard Completed Successfully',
                                navigationText: 'Please select from the following views. Click OK to continue.',
                                values: values,
                                onOkClick: function (selectedView) {
                                    var $selectedView = $(selectedView),
                                        viewId = $selectedView.val();
                                    if(isFunction(o.onFinish)) {
                                        o.onFinish(viewId);
                                    }
                                }
                            });
                            
                        },
                        error: function() {
                            toggleButton(buttons.cancel, true);
                            toggleButton(buttons.prev, true);
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


//#region Archive

//This was seriously non-trivial to write. Don't delete it until after the ship arrives at New Earth.            
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

//#endregion Archive