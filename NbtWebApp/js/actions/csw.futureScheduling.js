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
            onFinish: null, //function ($wizard) {},

            treeurl: '/NbtWebApp/wsNBT.asmx/getGeneratorsTree',
            futureurl: '/NbtWebApp/wsNBT.asmx/futureScheduling',
            $wizard: null,
            startingStep: 1,
            currentStepNo: 1,
            step1div: null,
            step2div: null,
            resultscell: null,
            generatorTree: null,
            endDatePicker: null
            //            buttons = {
            //                next: 'next',
            //                prev: 'previous',
            //                finish: 'finish',
            //                cancel: 'cancel'
            //            },
        };
        if (options) $.extend(internal, options);

        var external = {};

        
        internal.handleNext = function ($wizard, SelectedStep) {
            var checkedNodeKeys = '';
            
            Csw.each(internal.generatorTree.$.CswNodeTree('checkedNodes'), function(thisObj) {
                if(checkedNodeKeys !== '') checkedNodeKeys += ',';
                checkedNodeKeys += thisObj.cswnbtnodekey;
            });
        
            Csw.ajax.post({
                url: internal.futureurl,
                data: {
                    SelectedGeneratorNodeKeys: checkedNodeKeys,
                    EndDate: internal.endDatePicker.val().date
                },
                success: function (data) {

                    internal.resultscell.$.CswNodeTree('makeTree', data.treedata, {
                        ID: Csw.controls.dom.makeId(internal.ID, '', 'resultstree')
                    }); // makeTree

                } // success
            }); // ajax
        };

        internal.handlePrevious = function () {

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
                onNext: internal.handleNext,
                onPrevious: internal.handlePrevious,
                onCancel: internal.onCancel,
                onFinish: internal.onFinish,
                doNextOnInit: false
            });

            var $divstep1 = $(internal.$wizard.CswWizard('div', Csw.enums.wizardSteps_FutureScheduling.step1.step));
            internal.step1div = Csw.controls.factory($divstep1, {});

            var step1table = internal.step1div.table({ 
                ID: Csw.controls.dom.makeId(internal.ID, '', 'table1'), 
                FirstCellRightAlign: true,
                width: '100%' 
            });

            /*
            _NoGeneratorsWarning = new Label();
            _NoGeneratorsWarning.Text = "The currently loaded view contains no Generator nodes";
            _NoGeneratorsWarning.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            _NoGeneratorsWarning.Visible = false;
            _StepOneCswAutoTable.addControl( 0, 1, _NoGeneratorsWarning );
            */

            step1table.cell(1, 1).span({ text: 'Select future date:' });

            internal.endDatePicker = step1table.cell(1, 2).dateTimePicker({
                ID: Csw.controls.dom.makeId(internal.ID, '', 'date'),
                DisplayMode: 'Date',
                Date: Csw.date().addDays(90).toString()
            });

            step1table.cell(2, 1).append('&nbsp;');

            step1table.cell(3, 1).span({ text: 'Select Generators:' });
            //            step1table.cell(3, 3).link({ ID: Csw.controls.dom.makeId(internal.ID, '', 'selectall') });
            //            step1table.cell(3, 4).link({ ID: Csw.controls.dom.makeId(internal.ID, '', 'deselectall') });

            var cell42 = step1table.cell(4, 2);

            Csw.ajax.post({
                url: internal.treeurl,
                data: {},
                success: function (data) {

                    internal.generatorTree = cell42.$.CswNodeTree('makeTree', data, {
                        ID: Csw.controls.dom.makeId(internal.ID, '', 'gentree'),
                        ShowCheckboxes: true,
                        ValidateCheckboxes: false
                    }); // makeTree

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

    Csw.register('futureScheduling', futureScheduling);
    Csw.actions.futureScheduling = Csw.actions.futureScheduling || futureScheduling;

})();
