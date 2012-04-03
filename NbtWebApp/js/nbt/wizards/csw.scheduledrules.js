/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.scheduledRulesWizard = Csw.nbt.scheduledRulesWizard ||
        Csw.nbt.register('scheduledRulesWizard', function (cswParent, options) {
            'use strict';

            var internal = {
                ID: 'cswScheduledRulesGrid',
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
                $.extend(internal, options);
            }

            var external = cswParent.div();

            internal.wizardSteps = {
                1: Csw.enums.wizardSteps_ScheduleRulesGrid.step1.description,
                2: Csw.enums.wizardSteps_ScheduleRulesGrid.step2.description
            };
                        

            internal.currentStepNo = internal.startingStep;

            internal.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = internal.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    internal.wizard[button].disable();
                }
                return false;
            };

            internal.makeStepId = function (suffix, stepNo) {
                var step = stepNo || internal.currentStepNo;
                return Csw.makeId({ prefix: 'step_' + step, ID: internal.ID, suffix: suffix });
            };

            internal.makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    var nextBtnEnabled = function () {
                        return (false === Csw.isNullOrEmpty(internal.selectedCustomerId));
                    };
                    var customerIdTable, customerIdSelect;

                    internal.toggleButton(internal.buttons.prev, false);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === stepOneComplete) {
                        internal.divStep1 = internal.wizard.div(Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step);
                        internal.divStep1.br();

                        customerIdTable = internal.divStep1.table({
                            ID: internal.makeStepId('inspectionTable'),
                            FirstCellRightAlign: true
                        });

                        customerIdTable.cell(1, 1).span({ text: 'Select a Customer ID&nbsp' })
                            .css({ 'padding': '1px', 'vertical-align': 'middle' });

                        customerIdSelect = customerIdTable.cell(1, 2)
                            .select({
                                ID: Csw.makeSafeId('customerIdSelect'),
                                selected: '',
                                values: [{ value: '[ None ]', display: '[ None ]'}],
                                onChange: function () {
                                    var selected = customerIdSelect.find(':selected');
                                    internal.selectedCustomerId = selected.val();
                                    internal.toggleButton(internal.buttons.next, (false === Csw.isNullOrEmpty(internal.selectedCustomerId) && internal.selectedCustomerId !== '[ None ]'));
                                }
                            });

                        Csw.ajax.post({
                            url: '/NbtWebApp/wsNBT.asmx/getActiveAccessIds',
                            success: function (data) {
                                var values = data.customerids;
                                customerIdSelect.setOptions(values);
                                internal.selectedCustomerId = customerIdSelect.find(':selected').val();
                            }
                        });

                        internal.selectedCustomerId = customerIdSelect.find(':selected').val();
                    }
                    stepOneComplete = true;
                };
            } ());

            //Step 2: Review Scheduled Rules
            internal.makeStepTwo = function() {
                var rulesGridId = internal.makeStepId('previewGrid_outer', 3),
                    rulesGridDiv, headerTable;

                var makeRulesGrid = function() {
                    rulesGridDiv = rulesGridDiv || internal.divStep2.div({ID: rulesGridId });
                    rulesGridDiv.empty();

                    internal.gridOptions = {
                        ID: internal.makeStepId('rulesGrid'),
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
                                    editData: { AccessId: internal.selectedCustomerId },
                                    reloadAfterSubmit: false,
                                    checkOnSubmit: true,
                                    closeAfterEdit: true,
                                    afterComplete: makeRulesGrid
                                };
                                return internal.scheduledRulesGrid.gridTable.$.jqGrid('editGridRow', rowid, onEdit);
                            }
                        }
                    };

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/getScheduledRulesGrid',
                        data: { AccessId: internal.selectedCustomerId },
                        success: function(data) {
                            $.extend(internal.gridOptions.gridOpts, data);
                            internal.scheduledRulesGrid = rulesGridDiv.grid(internal.gridOptions);
                        }
                    });
                };

                internal.divStep2 = internal.divStep2 || internal.wizard.div(Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step);
                internal.divStep2.empty();

                internal.toggleButton(internal.buttons.next, false);
                internal.toggleButton(internal.buttons.cancel, false);
                internal.toggleButton(internal.buttons.finish, true);
                internal.toggleButton(internal.buttons.prev, true);

                headerTable = internal.divStep2.table({
                    ID: internal.makeStepId('headerTable')
                });
                headerTable.cell(1, 1)
                    .span({ text: 'Review Customer ID <b>' + internal.selectedCustomerId + '\'s</b> Scheduled Rules. Make any necessary edits.' });

                headerTable.cell(1, 2)
                    .button({
                        ID: Csw.makeSafeId('clearAll'),
                        enabledText: 'Clear All Reprobates',
                        disabledText: 'Clearing...',
                        onClick: function() {
                            Csw.ajax.post({
                                url: '/NbtWebApp/wsNBT.asmx/updateAllScheduledRules',
                                data: { AccessId: internal.selectedCustomerId, Action: 'ClearAllReprobates' },
                                success: internal.makeStepTwo
                            });
                        }
                    });
                internal.divStep2.br();

                makeRulesGrid();

            };

            internal.handleNext = function(newStepNo) {
                internal.currentStepNo = newStepNo;
                switch (newStepNo) {
                case Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step:
                    internal.makeStepTwo();
                    break;
                } // switch(newstepno)
            }; // handleNext()

            internal.handlePrevious = function (newStepNo) {
                    internal.currentStepNo = newStepNo;
                    switch (newStepNo) {
                        case Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step:
                            internal.makeStepOne();
                            break;
                    }
                },

            internal.wizard = Csw.layouts.wizard(external, {
                ID: Csw.makeId({ ID: internal.ID, suffix: 'wizard' }),
                Title: 'View Nbt Scheduler Rules by Schema',
                StepCount: Csw.enums.wizardSteps_ScheduleRulesGrid.stepcount,
                Steps: internal.wizardSteps,
                StartingStep: internal.startingStep,
                FinishText: 'Finish',
                onNext: internal.handleNext,
                onPrevious: internal.handlePrevious,
                onCancel: internal.exitFunc, //There is nothing to finish or cancel, just exixt the wizard
                onFinish: internal.exitFunc,
                doNextOnInit: false
            });

            internal.makeStepOne();

            return external;
        });
} ());