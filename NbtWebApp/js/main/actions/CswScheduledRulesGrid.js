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
/// <reference path="../controls/CswSelect.js" />

(function ($) { /// <param name="$" type="jQuery" />
    if(false === abandonHope) {
        "use strict";
    }
    $.fn.CswScheduledRulesGrid = function (options) {

        //#region Variable Declaration
        var o = {
            ID: 'cswScheduledRulesGrid',
            onCancel: null, //function($wizard) {},
            onFinish: null, //function($wizard) {},
            startingStep: 1
        };
        if (options) $.extend(o, options);

        var wizardSteps = {
            1: ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step1.description,
            2: ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step2.description
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

        // Step 1 - Select a Customer ID
            $divStep1, selectedCustomerId = '',
        // Step 2 - Review Scheduled Rules
            $divStep2, scheduledRulesGrid, gridOptions,
            
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
            
            //Step 1. Select a Customer ID
            makeStepOne = (function() {
                var stepOneComplete = false;

                return function() {
                    var nextBtnEnabled = function() {
                        return (false === isNullOrEmpty(selectedCustomerId));
                    };
                    var $customerIdTable, $customerIdSelect;

                    toggleButton(buttons.prev, false);
                    toggleButton(buttons.finish, false);
                    toggleButton(buttons.next, nextBtnEnabled());
                    
                    if (false === stepOneComplete) {
                        $divStep1 = $wizard.CswWizard('div', ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step1.step);
                        $divStep1.append('<br />');

                        $customerIdTable = $divStep1.CswTable('init', {
                            ID: makeStepId('inspectionTable'),
                            FirstCellRightAlign: true
                        });

                        $customerIdTable.CswTable('cell', 1, 1)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .append('<span>Select a Customer ID&nbsp</span>');
                        
                        $customerIdSelect = $customerIdTable.CswTable('cell', 1, 2)
                            .CswDiv('init')
                            .CswSelect('init', {
                                ID: makeSafeId('customerIdSelect'),
                                selected: '',
                                values: [{value: '', display: ''}],
                                onChange: function() {
                                    var $selected = $customerIdSelect.find(':selected');
                                    selectedCustomerId = $selected.val();
                                    toggleButton(buttons.next, (false === isNullOrEmpty(selectedCustomerId)));
                                }
                            });
                        
                        CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/getActiveAccessIds',
                            success: function(data) {
                                var values = data.customerids;
                                $customerIdSelect.CswSelect('setoptions', values);
                                selectedCustomerId = $customerIdSelect.find(':selected').val();
                            }
                        });
                        
                        selectedCustomerId = $customerIdSelect.find(':selected').val();
                    }
                    stepOneComplete = true;
                };
            }()),

            //Step 2: Review Scheduled Rules
            makeStepTwo = function() {
                var rulesGridId = makeStepId('previewGrid_outer', 3), 
                    $rulesGrid;

                var makeRulesGrid = function() {
                    $rulesGrid = $rulesGrid || $('<div id="' + rulesGridId + '"></div>').appendTo($divStep2);
                    $rulesGrid.empty();
                    
                    gridOptions = {
                        ID: makeStepId('rulesGrid'),
                        pagermode: 'default',
                        gridOpts: {
                            autowidth: true,
                            height: '200'
                        },
                        optNav: {
                            add: false,
                            del: false,
                            edit: true,
                            view: false,
                            editfunc: function(rowid) {
                                var onEdit = {
                                    url: '/NbtWebApp/wsNBT.asmx/updateScheduledRule', 
                                    editData: { AccessId: selectedCustomerId },
                                    reloadAfterSubmit: false,
                                    checkOnSubmit: true,
                                    closeAfterEdit: true,
                                    afterComplete: makeRulesGrid
                                };
                                return scheduledRulesGrid.$gridTable.jqGrid('editGridRow', rowid, onEdit);
                            }
                        }
                    };
                    
                    CswAjaxJson({
                            url: '/NbtWebApp/wsNBT.asmx/getScheduledRulesGrid',
                            data: { AccessId: selectedCustomerId },
                            success: function(data) {
                                $.extend(gridOptions.gridOpts, data);
                                scheduledRulesGrid = new CswGrid(gridOptions, $rulesGrid);
                            }
                        });
                };
                
                $divStep2 = $divStep2 || $wizard.CswWizard('div', ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step2.step);
                $divStep2.empty();
                
                toggleButton(buttons.next);
                toggleButton(buttons.finish, true);
                toggleButton(buttons.prev, true);
                
                $divStep2.append('<p>Review Customer ID <b>' + selectedCustomerId + '\'s</b> Scheduled Rules. Make any necessary edits.</p>');

                makeRulesGrid();
                
            },

            handleNext = function($wizardTable, newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step2.step:
                        makeStepTwo(); 
                        break;
                } // switch(newstepno)
            }, // handleNext()

            handlePrevious = function(newStepNo) {
                currentStepNo = newStepNo;
                switch (newStepNo) {
                    case ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step1.step:
                        makeStepOne();
                        break;
                }
            },

            onFinish = function() {

                toggleButton(buttons.prev, false);
                toggleButton(buttons.next, false);
                toggleButton(buttons.finish, false);
                toggleButton(buttons.cancel, false);
                
                //Probably load Welcome
            };

        //#endregion Variable Declaration

        //#region Execution
        $wizard = $div.CswWizard('init', {
            ID: makeId({ ID: o.ID, suffix: 'wizard' }),
            Title: 'View Nbt Scheduler Rules by Schema',
            StepCount: ChemSW.enums.CswScheduledRulesGrid_WizardSteps.stepcount,
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
    }; // $.fn.ChemSW.enums.CswScheduledRulesGrid_WizardSteps
})(jQuery);