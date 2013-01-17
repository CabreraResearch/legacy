/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    Csw.nbt.legacyMobileWizard = Csw.nbt.legacyMobileWizard ||
        Csw.nbt.register('legacyMobileWizard', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
                name: 'CswLegacyMobile',
                $parent: null,
                onCancel: null, //function ($wizard) {},
                onFinish: null, //function (viewid, viewmode) {},

                filedata: '',
                uploadurl: 'Services/LegacyMobile/parseDataFile',
                futureurl: 'LegacyMobile/performOperations',
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
            var uploadBtn;

            cswPrivate.onBeforeNext = function () {
                //Check to see if a file was uploaded
                return true;
            };

            cswPrivate.handleNext = function () {

                cswPrivate.resultscell.empty();

                // Disable all buttons until Ajax finishes
                cswPrivate.wizard.previous.disable();
                cswPrivate.wizard.next.disable();
                cswPrivate.wizard.finish.disable();
                cswPrivate.wizard.cancel.disable();

                Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.futureurl,
                    data: cswPrivate.filedata,
                    success: function (data) {
                        if (null == data.Error) {

                            var resultstree = Csw.nbt.nodeTree({
                                name: 'resultstree',
                                height: '250px',
                                width: '500px',
                                parent: cswPrivate.resultscell
                            });
                            // Need to convert to an object because I used
                            // pre-existing code that returns a string from 
                            // the old asmx web service
                            var treedata = JSON.parse(data.TreeData);
                            resultstree.makeTree(treedata.treedata);

                            cswPrivate.resultsviewid = treedata.sessionviewid;
                            cswPrivate.resultsviewmode = treedata.viewmode;

                            // Only Finish for step 2
                            cswPrivate.wizard.finish.enable();
                        }
                        else {
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
                    1: Csw.enums.wizardSteps_LegacyMobile.step1.description,
                    2: Csw.enums.wizardSteps_LegacyMobile.step2.description
                };

                cswPrivate.wizard = Csw.layouts.wizard(div, {
                    Title: 'Upload Legacy Mobile Data',
                    StepCount: Csw.enums.wizardSteps_LegacyMobile.stepcount,
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

                cswPrivate.step1div = cswPrivate.wizard.div(Csw.enums.wizardSteps_LegacyMobile.step1.step);

                var step1table = cswPrivate.step1div.table({
                    FirstCellLeftAlign: true,
                    width: '100%',
                    cellpadding: 2
                });

                step1table.cell(1, 1).span({ text: 'Choose a data file to upload. Videx and CISPro Mobile (Windows CE) data formats are both supported.' });

                step1table.cell(2, 1).div({
                });

                var filePreview = step1table.cell(4, 1).textArea({
                    name: 'legacyfilepreview',
                    rows: 12,
                    cols: 50,
                    readonly: true
                });

                var uploadBtn = step1table.cell(3, 1).input({
                    name: 'fileupload',
                    type: Csw.enums.inputTypes.file
                });
                uploadBtn.$.fileupload({
                    datatype: 'json',
                    url: cswPrivate.uploadurl,
                    paramName: 'filename',
                    uploadUrl: cswPrivate.uploadurl,
                    done: function (e, data) {
                        cswPrivate.filedata = data.result.Data.FileContents;
                        filePreview.val(data.result.Data.FileContents);
                    }
                });

                cswPrivate.step2div = cswPrivate.wizard.div(Csw.enums.wizardSteps_LegacyMobile.step2.step);

                var step2table = cswPrivate.step2div.table({ width: '100%' });
                step2table.cell(1, 1).span({ text: 'This operation will be completed by the following Batch Operations:' });
                cswPrivate.resultscell = step2table.cell(2, 1);

            })(); // init

            return cswPublic;
        });

})();
