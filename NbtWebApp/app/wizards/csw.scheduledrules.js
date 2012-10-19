/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.nbt.scheduledRulesWizard = Csw.nbt.scheduledRulesWizard ||
        Csw.nbt.register('scheduledRulesWizard', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                name: 'cswScheduledRulesGrid',
                exitFunc: null, //function ($wizard) {},
                startingStep: 1,
                wizard: '',
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '',
                divStep2: '',
                selectedCustomerId: '', // Step 1 - Select a Customer ID
                scheduledRulesGrid: '', // Step 2 - Review Scheduled Rules
                gridOptions: ''
            };

            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var cswPublic = cswParent.div();

            cswPrivate.wizardSteps = {
                1: Csw.enums.wizardSteps_ScheduleRulesGrid.step1.description,
                2: Csw.enums.wizardSteps_ScheduleRulesGrid.step2.description
            };
                        

            cswPrivate.currentStepNo = cswPrivate.startingStep;

            cswPrivate.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = cswPrivate.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    cswPrivate.wizard[button].disable();
                }
                return false;
            };
            
            cswPrivate.makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    var nextBtnEnabled = function () {
                        return (false === Csw.isNullOrEmpty(cswPrivate.selectedCustomerId));
                    };
                    var customerIdTable, customerIdSelect;

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                    if (false === stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.wizard.div(Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step);
                        cswPrivate.divStep1.br();

                        customerIdTable = cswPrivate.divStep1.table({
                            name: 'inspectionTable',
                            FirstCellRightAlign: true
                        });

                        customerIdTable.cell(1, 1).span({ text: 'Select a Customer ID&nbsp' })
                            .css({ 'padding': '1px', 'vertical-align': 'middle' });

                        customerIdSelect = customerIdTable.cell(1, 2)
                            .select({
                                name: 'customerIdSelect',
                                selected: '',
                                values: [{ value: '[ None ]', display: '[ None ]'}],
                                onChange: function () {
                                    var selected = customerIdSelect.find(':selected');
                                    cswPrivate.selectedCustomerId = selected.val();
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, (false === Csw.isNullOrEmpty(cswPrivate.selectedCustomerId) && cswPrivate.selectedCustomerId !== '[ None ]'));
                                }
                            });

                        Csw.ajax.post({
                            urlMethod: 'getActiveAccessIds',
                            success: function (data) {
                                var values = data.customerids;
                                customerIdSelect.setOptions(values);
                                cswPrivate.selectedCustomerId = customerIdSelect.find(':selected').val();
                            }
                        });

                        cswPrivate.selectedCustomerId = customerIdSelect.find(':selected').val();
                    }
                    stepOneComplete = true;
                };
            } ());

            //Step 2: Review Scheduled Rules
            cswPrivate.makeStepTwo = function() {
                var rulesGridId = 'previewGrid_outer',
                    rulesGridDiv, headerTable;

                var makeRulesGrid = function() {
                    rulesGridDiv = rulesGridDiv || cswPrivate.divStep2.div({name: rulesGridId });
                    rulesGridDiv.empty();

                    var gridId = 'rulesGrid';

                    cswPrivate.scheduledRulesGrid = rulesGridDiv.grid({
                        name: gridId,
                        storeId: gridId,
                        title: 'Scheduled Rules',
                        stateId: gridId,
                        usePaging: false,
                        showActionColumn: false,
                        canSelectRow: false,
                        ajax: {
                            urlMethod: 'getScheduledRulesGrid',
                            data: { AccessId: cswPrivate.selectedCustomerId }
                        }
                    });
                }; // makeRulesGrid()

                cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step);
                cswPrivate.divStep2.empty();

                cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
                cswPrivate.toggleButton(cswPrivate.buttons.finish, true);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true);

                headerTable = cswPrivate.divStep2.table({
                    name: 'headerTable'
                });
                headerTable.cell(1, 1)
                    .span({ text: 'Review Customer ID <b>' + cswPrivate.selectedCustomerId + '\'s</b> Scheduled Rules. Make any necessary edits.' });

                headerTable.cell(1, 2)
                    .button({
                        name: 'clearAll',
                        enabledText: 'Clear All Reprobates',
                        disabledText: 'Clearing...',
                        onClick: function() {
                            Csw.ajax.post({
                                urlMethod: 'updateAllScheduledRules',
                                data: { AccessId: cswPrivate.selectedCustomerId, Action: 'ClearAllReprobates' },
                                success: cswPrivate.makeStepTwo
                            });
                        }
                    });
                cswPrivate.divStep2.br();

                makeRulesGrid();

            };

            cswPrivate.handleNext = function(newStepNo) {
                cswPrivate.currentStepNo = newStepNo;
                switch (newStepNo) {
                case Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step:
                    cswPrivate.makeStepTwo();
                    break;
                } // switch(newstepno)
            }; // handleNext()

            cswPrivate.handlePrevious = function (newStepNo) {
                    cswPrivate.currentStepNo = newStepNo;
                    switch (newStepNo) {
                        case Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step:
                            cswPrivate.makeStepOne();
                            break;
                    }
                },

            cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                Title: 'View Nbt Scheduler Rules by Schema',
                StepCount: Csw.enums.wizardSteps_ScheduleRulesGrid.stepcount,
                Steps: cswPrivate.wizardSteps,
                StartingStep: cswPrivate.startingStep,
                FinishText: 'Finish',
                onNext: cswPrivate.handleNext,
                onPrevious: cswPrivate.handlePrevious,
                onCancel: cswPrivate.exitFunc, //There is nothing to finish or cancel, just exixt the wizard
                onFinish: cswPrivate.exitFunc,
                doNextOnInit: false
            });

            cswPrivate.makeStepOne();

            return cswPublic;
        });
} ());