/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    Csw.nbt.futureSchedulingWizard = Csw.nbt.futureSchedulingWizard ||
        Csw.nbt.register('futureSchedulingWizard', function (cswParent, options) {
            'use strict';
            
            var cswPrivateVar = {
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
                $.extend(cswPrivateVar, options);
            }
            var cswPublicRet = {};

            cswPrivateVar.onBeforeNext = function () {
                if (cswPrivateVar.generatorTree.checkedNodes().length === 0) {
                    $.CswDialog('AlertDialog', 'You must select at least one Generator to continue.');
                    return false;
                } else {
                    return true;
                }
            };

            cswPrivateVar.handleNext = function () {

                cswPrivateVar.resultscell.empty();

                // Disable all buttons until Ajax finishes
                cswPrivateVar.wizard.previous.disable();
                cswPrivateVar.wizard.next.disable();
                cswPrivateVar.wizard.finish.disable();
                cswPrivateVar.wizard.cancel.disable();

                var checkedNodeKeys = '';

                Csw.each(cswPrivateVar.generatorTree.checkedNodes(), function (thisObj) {
                    if (checkedNodeKeys !== '') checkedNodeKeys += ',';
                    checkedNodeKeys += thisObj.cswnbtnodekey;
                });

                Csw.ajax.post({
                    url: cswPrivateVar.futureurl,
                    data: {
                        SelectedGeneratorNodeKeys: checkedNodeKeys,
                        EndDate: cswPrivateVar.endDatePicker.val().date
                    },
                    success: function (data) {

                        if (Csw.number(data.result) > 0) {
                            var resultstree = Csw.nbt.nodeTree({
                                ID: Csw.makeId(cswPrivateVar.ID, '', 'resultstree'),
                                height: '250px',
                                width: '500px',
                                parent: cswPrivateVar.resultscell
                            });
                            resultstree.makeTree(data.treedata);

                            cswPrivateVar.resultsviewid = data.sessionviewid;
                            cswPrivateVar.resultsviewmode = data.viewmode;

                            // Only Finish for step 2
                            cswPrivateVar.wizard.finish.enable();
                        } else {
                            cswPrivateVar.resultscell.append('<br>No results');
                            cswPrivateVar.wizard.cancel.enable();
                            cswPrivateVar.wizard.previous.enable();
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

                cswPrivateVar.wizard = Csw.layouts.wizard(div, {
                    ID: Csw.makeId(cswPrivateVar.ID, '', 'wizard'),
                    Title: 'Future Scheduling Wizard',
                    StepCount: Csw.enums.wizardSteps_FutureScheduling.stepcount,
                    Steps: wizardSteps,
                    StartingStep: cswPrivateVar.startingStep,
                    FinishText: 'Finish',
                    onBeforeNext: cswPrivateVar.onBeforeNext,
                    onNext: cswPrivateVar.handleNext,
                    onCancel: cswPrivateVar.onCancel,
                    onFinish: function () {
                        Csw.tryExec(cswPrivateVar.onFinish, cswPrivateVar.resultsviewid, cswPrivateVar.resultsviewmode);
                    },
                    doNextOnInit: false
                });

                // Only Next and Cancel for step 1
                cswPrivateVar.wizard.previous.disable();
                cswPrivateVar.wizard.next.enable();
                cswPrivateVar.wizard.finish.disable();
                cswPrivateVar.wizard.cancel.enable();

                cswPrivateVar.step1div = cswPrivateVar.wizard.div(Csw.enums.wizardSteps_FutureScheduling.step1.step);

                var step1table = cswPrivateVar.step1div.table({
                    ID: Csw.makeId(cswPrivateVar.ID, '', 'table1'),
                    FirstCellRightAlign: true,
                    width: '100%',
                    cellpadding: 2
                });

                step1table.cell(1, 1).span({ text: 'Select future date:' });

                cswPrivateVar.endDatePicker = step1table.cell(1, 2).dateTimePicker({
                    ID: Csw.makeId(cswPrivateVar.ID, '', 'date'),
                    DisplayMode: 'Date',
                    Date: Csw.date().addDays(90).toString()
                });

                step1table.cell(2, 1).append('&nbsp;');

                step1table.cell(3, 1).span({ text: 'Select Generators:' });

                var cell42 = step1table.cell(4, 2);

                Csw.ajax.post({
                    url: cswPrivateVar.treeurl,
                    data: {},
                    success: function (data) {

                        cswPrivateVar.generatorTree = Csw.nbt.nodeTree({
                            ID: Csw.makeId(cswPrivateVar.ID, '', 'gentree'),
                            height: '225px',
                            width: '500px',
                            parent: cell42,
                            ShowCheckboxes: true,
                            ValidateCheckboxes: false
                        });
                        cswPrivateVar.generatorTree.makeTree(data);

                    } // success
                }); // ajax

                cswPrivateVar.step2div = cswPrivateVar.wizard.div(Csw.enums.wizardSteps_FutureScheduling.step2.step);

                var step2table = cswPrivateVar.step2div.table({ ID: Csw.makeId(cswPrivateVar.ID, '', 'table2'), width: '100%' });
                step2table.cell(1, 1).span({ text: 'Results:' });
                cswPrivateVar.resultscell = step2table.cell(2, 1);

            })(); // init

            return cswPublicRet;
        });

})();
