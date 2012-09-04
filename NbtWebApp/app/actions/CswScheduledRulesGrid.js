///// <reference path="~/js/CswNbt-vsdoc.js" />
///// <reference path="~/js/CswCommon-vsdoc.js" />

//(function ($) {
//    "use strict";

//    $.fn.CswScheduledRulesGrid = function (options) {

//        //#region Variable Declaration
//        var o = {
//            ID: 'cswScheduledRulesGrid',
//            exitFunc: null, //function ($wizard) {},
//            startingStep: 1
//        };
//        if (options) $.extend(o, options);

//        var wizardSteps = {
//            1: Csw.enums.wizardSteps_ScheduleRulesGrid.step1.description,
//            2: Csw.enums.wizardSteps_ScheduleRulesGrid.step2.description
//        };

//        var $parent = $(this),
//            $div = $('<div></div>').appendTo($parent),
//            $wizard,
//            currentStepNo = o.startingStep,
//            buttons = {
//                next: 'next',
//                prev: 'previous',
//                finish: 'finish',
//                cancel: 'cancel'
//            },

//        // Step 1 - Select a Customer ID
//            $divStep1, selectedCustomerId = '',
//        // Step 2 - Review Scheduled Rules
//            $divStep2, scheduledRulesGrid, gridOptions,

//            toggleButton = function (button, isEnabled, doClick) {
//                var $btn;
//                if (Csw.bool(isEnabled)) {
//                    $btn = $wizard.CswWizard('button', button, 'enable');
//                    if (Csw.bool(doClick)) {
//                        $btn.click();
//                    }
//                } else {
//                    $wizard.CswWizard('button', button, 'disable');
//                }
//                return false;
//            },

//            makeStepId = function (suffix, stepNo) {
//                var step = stepNo || currentStepNo;
//                return Csw.makeId({ prefix: 'step_' + step, ID: o.ID, suffix: suffix });
//            },

//        //Step 1. Select a Customer ID
//            makeStepOne = (function () {
//                var stepOneComplete = false;

//                return function () {
//                    var nextBtnEnabled = function () {
//                        return (false === Csw.isNullOrEmpty(selectedCustomerId));
//                    };
//                    var customerIdTable, customerIdSelect;

//                    toggleButton(buttons.prev, false);
//                    toggleButton(buttons.cancel, true);
//                    toggleButton(buttons.finish, false);
//                    toggleButton(buttons.next, nextBtnEnabled());

//                    if (false === stepOneComplete) {
//                        $divStep1 = $wizard.CswWizard('div', Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step);
//                        $divStep1.append('<br />');

//                        customerIdTable = Csw.literals.table({
//                            $parent: $divStep1,
//                            ID: makeStepId('inspectionTable'),
//                            FirstCellRightAlign: true
//                        });

//                        customerIdTable.cell(1, 1).span({ text: 'Select a Customer ID&nbsp' })
//                                        .css({ 'padding': '1px', 'vertical-align': 'middle' });

//                        customerIdSelect = customerIdTable.cell(1, 2)
//                            .select({
//                                ID: Csw.makeSafeId('customerIdSelect'),
//                                selected: '',
//                                values: [{ value: '[ None ]', display: '[ None ]'}],
//                                onChange: function () {
//                                    var $selected = customerIdSelect.find(':selected');
//                                    selectedCustomerId = $selected.val();
//                                    toggleButton(buttons.next, (false === Csw.isNullOrEmpty(selectedCustomerId) && selectedCustomerId !== '[ None ]'));
//                                }
//                            });

//                        Csw.ajax.post({
//                            url: '/NbtWebApp/wsNBT.asmx/getActiveAccessIds',
//                            success: function (data) {
//                                var values = data.customerids;
//                                customerIdSelect.setOptions(values);
//                                selectedCustomerId = customerIdSelect.find(':selected').val();
//                            }
//                        });

//                        selectedCustomerId = customerIdSelect.find(':selected').val();
//                    }
//                    stepOneComplete = true;
//                };
//            } ()),

//        //Step 2: Review Scheduled Rules
//            makeStepTwo = function () {
//                var rulesGridId = makeStepId('previewGrid_outer', 3),
//                    $rulesGrid, headerTable;

//                var makeRulesGrid = function () {
//                    $rulesGrid = $rulesGrid || $('<div id="' + rulesGridId + '"></div>').appendTo($divStep2);
//                    $rulesGrid.empty();

//                    gridOptions = {
//                        $parent: $rulesGrid,
//                        ID: makeStepId('rulesGrid'),
//                        pagermode: 'default',
//                        gridOpts: {
//                            autowidth: true,
//                            height: '200'
//                        },
//                        optNav: {
//                            add: false,
//                            del: false,
//                            edit: true,
//                            view: false,
//                            editfunc: function (rowid) {
//                                var onEdit = {
//                                    url: '/NbtWebApp/wsNBT.asmx/updateScheduledRule',
//                                    editData: { AccessId: selectedCustomerId },
//                                    reloadAfterSubmit: false,
//                                    checkOnSubmit: true,
//                                    closeAfterEdit: true,
//                                    afterComplete: makeRulesGrid
//                                };
//                                return scheduledRulesGrid.gridTable.$.jqGrid('editGridRow', rowid, onEdit);
//                            }
//                        }
//                    };

//                    Csw.ajax.post({
//                        url: '/NbtWebApp/wsNBT.asmx/getScheduledRulesGrid',
//                        data: { AccessId: selectedCustomerId },
//                        success: function (data) {
//                            $.extend(gridOptions.gridOpts, data);
//                            //gridOptions.$parnet = $rulesGrid;
//                            var parent = Csw.literals.factory($rulesGrid);
//                            scheduledRulesGrid = parent.grid(gridOptions);
//                        }
//                    });
//                };

//                $divStep2 = $divStep2 || $wizard.CswWizard('div', Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step);
//                $divStep2.empty();

//                toggleButton(buttons.next, false);
//                toggleButton(buttons.cancel, false);
//                toggleButton(buttons.finish, true);
//                toggleButton(buttons.prev, true);

//                headerTable = Csw.literals.table({
//                    $parent: $divStep2,
//                    ID: makeStepId('headerTable')
//                });
//                headerTable.cell(1, 1)
//                    .span({ text: 'Review Customer ID <b>' + selectedCustomerId + '\'s</b> Scheduled Rules. Make any necessary edits.' });

//                headerTable.cell(1, 2)
//                           .button({
//                               ID: Csw.makeSafeId('clearAll'),
//                               enabledText: 'Clear All Reprobates',
//                               disabledText: 'Clearing...',
//                               onClick: function () {
//                                   Csw.ajax.post({
//                                       url: '/NbtWebApp/wsNBT.asmx/updateAllScheduledRules',
//                                       data: { AccessId: selectedCustomerId, Action: 'ClearAllReprobates' },
//                                       success: makeStepTwo
//                                   });
//                               }
//                           });
//                $divStep2.append('<br/>');

//                makeRulesGrid();

//            },

//            handleNext = function ($wizardTable, newStepNo) {
//                currentStepNo = newStepNo;
//                switch (newStepNo) {
//                    case Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step:
//                        makeStepTwo();
//                        break;
//                } // switch(newstepno)
//            }, // handleNext()

//            handlePrevious = function (newStepNo) {
//                currentStepNo = newStepNo;
//                switch (newStepNo) {
//                    case Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step:
//                        makeStepOne();
//                        break;
//                }
//            },

//        //#endregion Variable Declaration

//        //#region Execution
//        $wizard = $div.CswWizard('init', {
//            ID: Csw.makeId({ ID: o.ID, suffix: 'wizard' }),
//            Title: 'View Nbt Scheduler Rules by Schema',
//            StepCount: Csw.enums.wizardSteps_ScheduleRulesGrid.stepcount,
//            Steps: wizardSteps,
//            StartingStep: o.startingStep,
//            FinishText: 'Finish',
//            onNext: handleNext,
//            onPrevious: handlePrevious,
//            onCancel: o.exitFunc, //There is nothing to finish or cancel, just exixt the wizard
//            onFinish: o.exitFunc,
//            doNextOnInit: false
//        });

//        makeStepOne();
//        //#endregion Execution

//        return $div;
//    }; // $.fn.Csw.enums.wizardSteps_ScheduleRulesGrid
//})(jQuery);