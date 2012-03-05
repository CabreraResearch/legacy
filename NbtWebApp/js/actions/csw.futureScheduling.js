/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    "use strict";
    var futureScheduling = function (options) {

        //#region Variable Declaration
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
        if (options) $.extend(internal, options);

        var external = {};

        internal.onBeforeNext = function ($wizard, stepno) {
            if (internal.generatorTree.checkedNodes().length === 0) {
                alert('You must select at least one Generator to continue.');
                return false;
            } else {
                return true;
            }
        };

        internal.handleNext = function ($wizard, SelectedStep) {

            // Disable all buttons until Ajax finishes
            internal.$wizard.CswWizard('button', 'previous', 'disable');
            internal.$wizard.CswWizard('button', 'next', 'disable');
            internal.$wizard.CswWizard('button', 'finish', 'disable');
            internal.$wizard.CswWizard('button', 'cancel', 'disable');

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

                    var resultstree = Csw.nbt.nodeTree({
                        ID: Csw.controls.dom.makeId(internal.ID, '', 'resultstree'),
                        height: '250px',
                        width: '500px',
                        parent: internal.resultscell
                    });
                    resultstree.makeTree(data.treedata);

                    internal.resultsviewid = data.sessionviewid;
                    internal.resultsviewmode = data.viewmode;

                    // Only Finish for step 2
                    internal.$wizard.CswWizard('button', 'finish', 'enable');
                } // success
            }); // ajax 
        };


        // Init
        (function () {
            var div = Csw.controls.div({
                ID: Csw.controls.dom.makeId(internal.ID, '', 'div'),
                $parent: internal.$parent
            });
            var wizardSteps = {
                1: Csw.enums.wizardSteps_FutureScheduling.step1.description,
                2: Csw.enums.wizardSteps_FutureScheduling.step2.description
            };

            internal.$wizard = div.$.CswWizard('init', {
                ID: Csw.controls.dom.makeId(internal.ID, '', 'wizard'),
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
            internal.$wizard.CswWizard('button', 'previous', 'disable');
            internal.$wizard.CswWizard('button', 'next', 'enable');
            internal.$wizard.CswWizard('button', 'finish', 'disable');
            internal.$wizard.CswWizard('button', 'cancel', 'enable');


            var $divstep1 = $(internal.$wizard.CswWizard('div', Csw.enums.wizardSteps_FutureScheduling.step1.step));
            internal.step1div = Csw.controls.factory($divstep1, {});

            var step1table = internal.step1div.table({
                ID: Csw.controls.dom.makeId(internal.ID, '', 'table1'),
                FirstCellRightAlign: true,
                width: '100%',
                cellpadding: 2
            });

            step1table.cell(1, 1).span({ text: 'Select future date:' });

            internal.endDatePicker = step1table.cell(1, 2).dateTimePicker({
                ID: Csw.controls.dom.makeId(internal.ID, '', 'date'),
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
                        ID: Csw.controls.dom.makeId(internal.ID, '', 'gentree'),
                        height: '225px',
                        width: '500px',
                        parent: cell42,
                        ShowCheckboxes: true,
                        ValidateCheckboxes: false
                    });
                    internal.generatorTree.makeTree(data);

                } // success
            }); // ajax

            var $divstep2 = $(internal.$wizard.CswWizard('div', Csw.enums.wizardSteps_FutureScheduling.step2.step));
            internal.step2div = Csw.controls.factory($divstep2, {});

            var step2table = internal.step2div.table({ ID: Csw.controls.dom.makeId(internal.ID, '', 'table2'), width: '100%' });
            step2table.cell(1, 1).span({ text: 'Results:' });
            internal.resultscell = step2table.cell(2, 1);

        })(); // init

        return external;
    };

    Csw.actions.register('futureScheduling', futureScheduling);
    Csw.actions.futureScheduling = Csw.actions.futureScheduling || futureScheduling;

})();
