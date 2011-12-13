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
                                    if(false === isNullOrEmpty(selectedCustomerId)) {
                                        toggleButton(buttons.next, true);
                                    }
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
                $divStep2 = $divStep2 || $wizard.CswWizard('div', ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step2.step);
                $divStep2.empty();
                
                toggleButton(buttons.next);
                toggleButton(buttons.finish, true);
                toggleButton(buttons.prev, true);
                
                var rulesGridId = makeStepId('previewGrid_outer', 3),
                    $rulesGrid = $('<div id="' + rulesGridId + '"></div>').appendTo($divStep2);

                $divStep2.append('<p>Review the <b>' + selectedCustomerId + '</b> Scheduled Rules. Make any necessary edits.</p>');
                
                gridOptions = {
                    ID: makeStepId('rulesGrid'),
                    pagermode: 'default',
                    gridOpts: {
                        //autowidth: true,
                        //height: '200'
                    },
                    optNav: {
                        add: false,
                        del: false,
                        edit: true,
                        view: false,
                        editfunc: function(rowid) {
                            //add our onEdit event here
                            return scheduledRulesGrid.$gridTable.jqGrid('editGridRow', rowid, { url: '/NbtWebApp/wsNBT.asmx/ReturnTrue', reloadAfterSubmit: false, closeAfterEdit: true });
                        }
                    }
                };

                CswAjaxJson({
                    url: '/NbtWebApp/wsNBT.asmx/getScheduledRulesGrid',
                    data: {AccessId: selectedCustomerId},
                    success: function(data) {
                        $.extend(gridOptions.gridOpts, data);        
                        scheduledRulesGrid = new CswGrid(gridOptions, $rulesGrid);
                    }
                });
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
//                        $divStep5 = $divStep5 || $wizard.CswWizard('div', ChemSW.enums.CswScheduledRulesGrid_WizardSteps.step5.step);
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