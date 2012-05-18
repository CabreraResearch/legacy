/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.scheduledRulesWizard = Csw.nbt.scheduledRulesWizard ||
        Csw.nbt.register('scheduledRulesWizard', function (cswParent, options) {
            'use strict';

            var cswPrivateVar = {
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
                $.extend(cswPrivateVar, options);
            }

            var cswPublicRet = cswParent.div();

            cswPrivateVar.wizardSteps = {
                1: Csw.enums.wizardSteps_ScheduleRulesGrid.step1.description,
                2: Csw.enums.wizardSteps_ScheduleRulesGrid.step2.description
            };
                        

            cswPrivateVar.currentStepNo = cswPrivateVar.startingStep;

            cswPrivateVar.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = cswPrivateVar.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    cswPrivateVar.wizard[button].disable();
                }
                return false;
            };

            cswPrivateVar.makeStepId = function (suffix, stepNo) {
                var step = stepNo || cswPrivateVar.currentStepNo;
                return Csw.makeId({ prefix: 'step_' + step, ID: cswPrivateVar.ID, suffix: suffix });
            };

            cswPrivateVar.makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    var nextBtnEnabled = function () {
                        return (false === Csw.isNullOrEmpty(cswPrivateVar.selectedCustomerId));
                    };
                    var customerIdTable, customerIdSelect;

                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, true);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, false);
                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, nextBtnEnabled());

                    if (false === stepOneComplete) {
                        cswPrivateVar.divStep1 = cswPrivateVar.wizard.div(Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step);
                        cswPrivateVar.divStep1.br();

                        customerIdTable = cswPrivateVar.divStep1.table({
                            ID: cswPrivateVar.makeStepId('inspectionTable'),
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
                                    cswPrivateVar.selectedCustomerId = selected.val();
                                    cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, (false === Csw.isNullOrEmpty(cswPrivateVar.selectedCustomerId) && cswPrivateVar.selectedCustomerId !== '[ None ]'));
                                }
                            });

                        Csw.ajax.post({
                            url: '/NbtWebApp/wsNBT.asmx/getActiveAccessIds',
                            success: function (data) {
                                var values = data.customerids;
                                customerIdSelect.setOptions(values);
                                cswPrivateVar.selectedCustomerId = customerIdSelect.find(':selected').val();
                            }
                        });

                        cswPrivateVar.selectedCustomerId = customerIdSelect.find(':selected').val();
                    }
                    stepOneComplete = true;
                };
            } ());

            //Step 2: Review Scheduled Rules
            cswPrivateVar.makeStepTwo = function() {
                var rulesGridId = cswPrivateVar.makeStepId('previewGrid_outer', 3),
                    rulesGridDiv, headerTable;

                var makeRulesGrid = function() {
                    rulesGridDiv = rulesGridDiv || cswPrivateVar.divStep2.div({ID: rulesGridId });
                    rulesGridDiv.empty();

                    cswPrivateVar.gridOptions = {
                        ID: cswPrivateVar.makeStepId('rulesGrid'),
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
                                    editData: { AccessId: cswPrivateVar.selectedCustomerId },
                                    reloadAfterSubmit: false,
                                    checkOnSubmit: true,
                                    closeAfterEdit: true,
                                    afterComplete: makeRulesGrid
                                };
                                return cswPrivateVar.scheduledRulesGrid.gridTable.$.jqGrid('editGridRow', rowid, onEdit);
                            }
                        }
                    };

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/getScheduledRulesGrid',
                        data: { AccessId: cswPrivateVar.selectedCustomerId },
                        success: function(data) {
                            $.extend(cswPrivateVar.gridOptions.gridOpts, data);
                            cswPrivateVar.scheduledRulesGrid = rulesGridDiv.grid(cswPrivateVar.gridOptions);
                        }
                    });
                };

                cswPrivateVar.divStep2 = cswPrivateVar.divStep2 || cswPrivateVar.wizard.div(Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step);
                cswPrivateVar.divStep2.empty();

                cswPrivateVar.toggleButton(cswPrivateVar.buttons.next, false);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.cancel, false);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.finish, true);
                cswPrivateVar.toggleButton(cswPrivateVar.buttons.prev, true);

                headerTable = cswPrivateVar.divStep2.table({
                    ID: cswPrivateVar.makeStepId('headerTable')
                });
                headerTable.cell(1, 1)
                    .span({ text: 'Review Customer ID <b>' + cswPrivateVar.selectedCustomerId + '\'s</b> Scheduled Rules. Make any necessary edits.' });

                headerTable.cell(1, 2)
                    .button({
                        ID: Csw.makeSafeId('clearAll'),
                        enabledText: 'Clear All Reprobates',
                        disabledText: 'Clearing...',
                        onClick: function() {
                            Csw.ajax.post({
                                url: '/NbtWebApp/wsNBT.asmx/updateAllScheduledRules',
                                data: { AccessId: cswPrivateVar.selectedCustomerId, Action: 'ClearAllReprobates' },
                                success: cswPrivateVar.makeStepTwo
                            });
                        }
                    });
                cswPrivateVar.divStep2.br();

                makeRulesGrid();

            };

            cswPrivateVar.handleNext = function(newStepNo) {
                cswPrivateVar.currentStepNo = newStepNo;
                switch (newStepNo) {
                case Csw.enums.wizardSteps_ScheduleRulesGrid.step2.step:
                    cswPrivateVar.makeStepTwo();
                    break;
                } // switch(newstepno)
            }; // handleNext()

            cswPrivateVar.handlePrevious = function (newStepNo) {
                    cswPrivateVar.currentStepNo = newStepNo;
                    switch (newStepNo) {
                        case Csw.enums.wizardSteps_ScheduleRulesGrid.step1.step:
                            cswPrivateVar.makeStepOne();
                            break;
                    }
                },

            cswPrivateVar.wizard = Csw.layouts.wizard(cswPublicRet, {
                ID: Csw.makeId({ ID: cswPrivateVar.ID, suffix: 'wizard' }),
                Title: 'View Nbt Scheduler Rules by Schema',
                StepCount: Csw.enums.wizardSteps_ScheduleRulesGrid.stepcount,
                Steps: cswPrivateVar.wizardSteps,
                StartingStep: cswPrivateVar.startingStep,
                FinishText: 'Finish',
                onNext: cswPrivateVar.handleNext,
                onPrevious: cswPrivateVar.handlePrevious,
                onCancel: cswPrivateVar.exitFunc, //There is nothing to finish or cancel, just exixt the wizard
                onFinish: cswPrivateVar.exitFunc,
                doNextOnInit: false
            });

            cswPrivateVar.makeStepOne();

            return cswPublicRet;
        });
} ());