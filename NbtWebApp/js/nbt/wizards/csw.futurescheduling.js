/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    Csw.nbt.futureSchedulingWizard = Csw.nbt.futureSchedulingWizard ||
        Csw.nbt.register('futureSchedulingWizard', function (cswParent, options) {
            'use strict';
            
            var internal = {
                ID: 'CswFutureScheduling',
                $parent: null,
                onCancel: null, //function ($wizard) {},
                onFinish: null, //function (viewid, viewmode) {},

                treeurl: '/NbtWebApp/wsNBT.asmx/getGeneratorsTree',
                futureurl: '/NbtWebApp/wsNBT.asmx/futureScheduling',
                $wizard: null,
                startingStep: 1,
                currentStepNo: 1,
                step1div: null,
                step2div: null,
                resultscell: null,
                generatorTree: null,
                endDatePicker: null,
                resultsviewid: '',
                resultsviewmode: ''
            };
            if (options) {
                $.extend(internal, options);
            }
            var external = {};

            internal.onBeforeNext = function () {
                if (internal.generatorTree.checkedNodes().length === 0) {
                    $.CswDialog('AlertDialog', 'You must select at least one Generator to continue.');
                    return false;
                } else {
                    return true;
                }
            };

            internal.handleNext = function () {

                internal.resultscell.empty();

                // Disable all buttons until Ajax finishes
                internal.wizard.previous.disable();
                internal.wizard.next.disable();
                internal.wizard.finish.disable();
                internal.wizard.cancel.disable();

                var checkedNodeKeys = '';

                Csw.each(internal.generatorTree.checkedNodes(), function (thisObj) {
                    if (checkedNodeKeys !== '') checkedNodeKeys += ',';
                    checkedNodeKeys += thisObj.cswnbtnodekey;
                });

                Csw.ajax.post({
                    url: internal.futureurl,
                    data: {
                        SelectedGeneratorNodeKeys: checkedNodeKeys,
                        EndDate: internal.endDatePicker.val().date
                    },
                    success: function (data) {

                        if (Csw.number(data.result) > 0) {
                            var resultstree = Csw.nbt.nodeTree({
                                ID: Csw.makeId(internal.ID, '', 'resultstree'),
                                height: '250px',
                                width: '500px',
                                parent: internal.resultscell
                            });
                            resultstree.makeTree(data.treedata);

                            internal.resultsviewid = data.sessionviewid;
                            internal.resultsviewmode = data.viewmode;

                            // Only Finish for step 2
                            internal.wizard.finish.enable();
                        } else {
                            internal.resultscell.append('<br>No results');
                            internal.wizard.cancel.enable();
                            internal.wizard.previous.enable();
                        }
                    } // success
                }); // ajax 
            };


            // Init
            (function () {
                var div = cswParent.div({
                    suffix: 'div'
                });
                var wizardSteps = {
                    1: Csw.enums.wizardSteps_FutureScheduling.step1.description,
                    2: Csw.enums.wizardSteps_FutureScheduling.step2.description
                };

                internal.wizard = Csw.layouts.wizard(div, {
                    ID: Csw.makeId(internal.ID, '', 'wizard'),
                    Title: 'Future Scheduling Wizard',
                    StepCount: Csw.enums.wizardSteps_FutureScheduling.stepcount,
                    Steps: wizardSteps,
                    StartingStep: internal.startingStep,
                    FinishText: 'Finish',
                    onBeforeNext: internal.onBeforeNext,
                    onNext: internal.handleNext,
                    onCancel: internal.onCancel,
                    onFinish: function () {
                        Csw.tryExec(internal.onFinish, internal.resultsviewid, internal.resultsviewmode);
                    },
                    doNextOnInit: false
                });

                // Only Next and Cancel for step 1
                internal.wizard.previous.disable();
                internal.wizard.next.enable();
                internal.wizard.finish.disable();
                internal.wizard.cancel.enable();

                internal.step1div = internal.wizard.div(Csw.enums.wizardSteps_FutureScheduling.step1.step);

                var step1table = internal.step1div.table({
                    ID: Csw.makeId(internal.ID, '', 'table1'),
                    FirstCellRightAlign: true,
                    width: '100%',
                    cellpadding: 2
                });

                step1table.cell(1, 1).span({ text: 'Select future date:' });

                internal.endDatePicker = step1table.cell(1, 2).dateTimePicker({
                    ID: Csw.makeId(internal.ID, '', 'date'),
                    DisplayMode: 'Date',
                    Date: Csw.date().addDays(90).toString()
                });

                step1table.cell(2, 1).append('&nbsp;');

                step1table.cell(3, 1).span({ text: 'Select Generators:' });

                var cell42 = step1table.cell(4, 2);

                Csw.ajax.post({
                    url: internal.treeurl,
                    data: {},
                    success: function (data) {

                        internal.generatorTree = Csw.nbt.nodeTree({
                            ID: Csw.makeId(internal.ID, '', 'gentree'),
                            height: '225px',
                            width: '500px',
                            parent: cell42,
                            ShowCheckboxes: true,
                            ValidateCheckboxes: false
                        });
                        internal.generatorTree.makeTree(data);

                    } // success
                }); // ajax

                internal.step2div = internal.wizard.div(Csw.enums.wizardSteps_FutureScheduling.step2.step);

                var step2table = internal.step2div.table({ ID: Csw.makeId(internal.ID, '', 'table2'), width: '100%' });
                step2table.cell(1, 1).span({ text: 'Results:' });
                internal.resultscell = step2table.cell(2, 1);

            })(); // init

            return external;
        });

})();
