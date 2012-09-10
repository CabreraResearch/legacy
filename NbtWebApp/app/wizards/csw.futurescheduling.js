/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    Csw.nbt.futureSchedulingWizard = Csw.nbt.futureSchedulingWizard ||
        Csw.nbt.register('futureSchedulingWizard', function (cswParent, options) {
            'use strict';
            
            var cswPrivate = {
                ID: 'CswFutureScheduling',
                $parent: null,
                onCancel: null, //function ($wizard) {},
                onFinish: null, //function (viewid, viewmode) {},

                treeurl: 'getGeneratorsTree',
                futureurl: 'futureScheduling',
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
                Csw.extend(cswPrivate, options);
            }
            var cswPublic = {};

            cswPrivate.onBeforeNext = function () {
                if (cswPrivate.generatorTree.checkedNodes().length === 0) {
                    $.CswDialog('AlertDialog', 'You must select at least one Generator to continue.');
                    return false;
                } else {
                    return true;
                }
            };

            cswPrivate.handleNext = function () {

                cswPrivate.resultscell.empty();

                // Disable all buttons until Ajax finishes
                cswPrivate.wizard.previous.disable();
                cswPrivate.wizard.next.disable();
                cswPrivate.wizard.finish.disable();
                cswPrivate.wizard.cancel.disable();

                var checkedNodeKeys = '';

                Csw.each(cswPrivate.generatorTree.checkedNodes(), function (thisObj) {
                    if (checkedNodeKeys !== '') checkedNodeKeys += ',';
                    checkedNodeKeys += thisObj.cswnbtnodekey;
                });

                Csw.ajax.post({
                    urlMethod: cswPrivate.futureurl,
                    data: {
                        SelectedGeneratorNodeKeys: checkedNodeKeys,
                        EndDate: cswPrivate.endDatePicker.val().date
                    },
                    success: function (data) {

                        if (Csw.number(data.result) > 0) {
                            var resultstree = Csw.nbt.nodeTree({
                                ID: Csw.makeId(cswPrivate.ID, '', 'resultstree'),
                                height: '250px',
                                width: '500px',
                                parent: cswPrivate.resultscell
                            });
                            resultstree.makeTree(data.treedata);

                            cswPrivate.resultsviewid = data.sessionviewid;
                            cswPrivate.resultsviewmode = data.viewmode;

                            // Only Finish for step 2
                            cswPrivate.wizard.finish.enable();
                        } else {
                            cswPrivate.resultscell.append('<br>No results');
                            cswPrivate.wizard.cancel.enable();
                            cswPrivate.wizard.previous.enable();
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

                cswPrivate.wizard = Csw.layouts.wizard(div, {
                    ID: Csw.makeId(cswPrivate.ID, '', 'wizard'),
                    Title: 'Future Scheduling Wizard',
                    StepCount: Csw.enums.wizardSteps_FutureScheduling.stepcount,
                    Steps: wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onBeforeNext: cswPrivate.onBeforeNext,
                    onNext: cswPrivate.handleNext,
                    onCancel: cswPrivate.onCancel,
                    onFinish: function () {
                        Csw.tryExec(cswPrivate.onFinish, cswPrivate.resultsviewid, cswPrivate.resultsviewmode);
                    },
                    doNextOnInit: false
                });

                // Only Next and Cancel for step 1
                cswPrivate.wizard.previous.disable();
                cswPrivate.wizard.next.enable();
                cswPrivate.wizard.finish.disable();
                cswPrivate.wizard.cancel.enable();

                cswPrivate.step1div = cswPrivate.wizard.div(Csw.enums.wizardSteps_FutureScheduling.step1.step);

                var step1table = cswPrivate.step1div.table({
                    ID: Csw.makeId(cswPrivate.ID, '', 'table1'),
                    FirstCellRightAlign: true,
                    width: '100%',
                    cellpadding: 2
                });

                step1table.cell(1, 1).span({ text: 'Select future date:' });

                cswPrivate.endDatePicker = step1table.cell(1, 2).dateTimePicker({
                    ID: Csw.makeId(cswPrivate.ID, '', 'date'),
                    DisplayMode: 'Date',
                    Date: Csw.date().addDays(90).toString()
                });

                step1table.cell(2, 1).append('&nbsp;');

                step1table.cell(3, 1).span({ text: 'Select Generators:' });

                var cell42 = step1table.cell(4, 2);

                Csw.ajax.post({
                    urlMethod: cswPrivate.treeurl,
                    data: {},
                    success: function (data) {

                        cswPrivate.generatorTree = Csw.nbt.nodeTree({
                            ID: Csw.makeId(cswPrivate.ID, '', 'gentree'),
                            height: '225px',
                            width: '500px',
                            parent: cell42,
                            ShowCheckboxes: true,
                            ValidateCheckboxes: false
                        });
                        cswPrivate.generatorTree.makeTree(data);

                    } // success
                }); // ajax

                cswPrivate.step2div = cswPrivate.wizard.div(Csw.enums.wizardSteps_FutureScheduling.step2.step);

                var step2table = cswPrivate.step2div.table({ ID: Csw.makeId(cswPrivate.ID, '', 'table2'), width: '100%' });
                step2table.cell(1, 1).span({ text: 'This operation will be completed by the following Batch Operations:' });
                cswPrivate.resultscell = step2table.cell(2, 1);

            })(); // init

            return cswPublic;
        });

})();
