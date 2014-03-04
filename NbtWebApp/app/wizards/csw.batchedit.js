/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
    Csw.nbt.register('batchEditWizard', function (cswParent, options) {
        'use strict';

        //#region Properties
        var cswPrivate = {
            name: 'mergeWizard',
            startingStep: 1,
            stepCount: 0,
            wizard: null,
            buttons: {
                next: 'next',
                prev: 'previous',
                finish: 'finish',
                cancel: 'cancel'
            },
            onFinish: function() {},
            onCancel: function() {},
            wizardStepSelectNodeType_init: false,
            wizardStepSelectProperties_init: false,
            nodeTypeSel: null,
            propSel: null
        };

        var cswPublic = {};
        //#endregion Properties

        //#region Wizard Functions
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

        cswPrivate.toggleStepButtons = function (StepNo) {
            cswPrivate.toggleButton(cswPrivate.buttons.prev, StepNo > 1);
            cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
            cswPrivate.toggleButton(cswPrivate.buttons.finish, StepNo === cswPrivate.stepCount);
            cswPrivate.toggleButton(cswPrivate.buttons.next, StepNo !== cswPrivate.stepCount);
        };

        cswPrivate.reinitSteps = function (startWithStep) {
            for (var i = startWithStep; i <= cswPrivate.stepCount; i++) {
                cswPrivate['step' + i + 'Complete'] = false;
            }
        };

        cswPrivate.handleStep = function (newStepNo) {
            //cswPrivate.setState();
            if (false === Csw.isNullOrEmpty(cswPrivate.onStepChange[cswPrivate.currentStepNo])) {
                cswPrivate.onStepChange[cswPrivate.currentStepNo](cswPrivate.currentStepNo);
            }
            cswPrivate.lastStepNo = cswPrivate.currentStepNo;
            cswPrivate.currentStepNo = newStepNo;
            if (false === Csw.isNullOrEmpty(cswPrivate.stepFunc[newStepNo])) {
                cswPrivate.stepFunc[newStepNo](newStepNo);
            }
        };

        cswPrivate.setWizardSteps = function () {
            var wizardSteps = {};
            cswPrivate.stepFunc = {};
            cswPrivate.onStepChange = {};
            cswPrivate.stepCount = 0;
            var setWizardStep = function (wizardStep) {
                cswPrivate.stepCount++;
                cswPrivate.stepFunc[cswPrivate.stepCount] = wizardStep.makeStep;
                cswPrivate.onStepChange[cswPrivate.stepCount] = wizardStep.onStepChange;
                wizardStep.stepNo = cswPrivate.stepCount;
                wizardSteps[cswPrivate.stepCount] = wizardStep.stepName;
            };
            //Add steps here:
            setWizardStep(cswPrivate.wizardStepSelectNodeType);
            setWizardStep(cswPrivate.wizardStepSelectProperties);
            setWizardStep(cswPrivate.wizardStepPerformEdits);
            cswPrivate.reinitSteps(1);
            return wizardSteps;
        };

        cswPrivate.setStepHeader = function (StepNo, Header) {
            cswPrivate['divStep' + StepNo] = cswPrivate['divStep' + StepNo] || cswPrivate.wizard.div(StepNo);
            cswPrivate['divStep' + StepNo].empty();
            cswPrivate['divStep' + StepNo].span({
                text: Header,
                cssclass: 'wizardHelpDesc'
            });
            cswPrivate['divStep' + StepNo].br({ number: 2 });
        };
        //#endregion Wizard Functions

        //#region ctor preInit
        (function _pre() {
            if (options) {
                Csw.extend(cswPrivate, options);
            }
            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException(Csw.error.exception('Cannot create a Material Receiving wizard without a parent.', '', 'csw.receivematerial.js', 57));
            }
            //cswPrivate.validateState();
        }());
        //#endregion ctor preInit

        //#region Steps
        cswPrivate.wizardStepSelectNodeType = {
            stepName: 'Select Type',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);
                    if (false === cswPrivate.wizardStepSelectNodeType_init) {
                        cswPrivate.wizardStepSelectNodeType_init = true;
                        cswPrivate.setStepHeader(StepNo, 'Select which type you would like to edit.');
                        var div = cswPrivate['divStep' + StepNo];

                        cswPrivate.nodeTypeSel = div.nodeTypeSelect({
                            onSelect: function() {
                                cswPrivate.wizardStepSelectProperties_init = false;
                            }
                        });
                    }
                };
            }()),
            onStepChange: function () {}
        }; // wizardStepSelectNodeType
        
        cswPrivate.wizardStepSelectProperties = {
            stepName: 'Select Properties',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);
                    if (false === cswPrivate.wizardStepSelectProperties_init) {
                        cswPrivate.wizardStepSelectProperties_init = true;
                        cswPrivate.setStepHeader(StepNo, 'Select which properties you would like to edit.');
                        var div = cswPrivate['divStep' + StepNo];

                        Csw.ajax.deprecatedWsNbt({
                            urlMethod: 'getNodeTypeProps',
                            data: {
                                NodeTypeName: '',
                                NodeTypeId: Csw.string(cswPrivate.nodeTypeSel.val()),
                                EditablePropsOnly: true
                            },
                            success: function (data) {
                                var opts = [];
                                Csw.iterate(data, function(prop) {
                                    opts[opts.length] = { value: prop.id, selected: false, text: prop.name };
                                });
                                cswPrivate.propSel = Csw.dialogs.multiselectedit({
                                    parent: div,
                                    inDialog: false,
                                    opts: opts
                                });
                            } // success()
                        }); // ajax
                    }
                };
            }()),
            onStepChange: function () {}
        }; // wizardStepSelectProperties
        

        cswPrivate.wizardStepPerformEdits = {
            stepName: 'Perform Edits',
            stepNo: '',
            makeStep: (function () {
                return function (StepNo) {
                    cswPrivate.toggleStepButtons(StepNo);
                    //cswPrivate.setStepHeader(StepNo, 'Download the excel spreadsheet, make changes, and upload the edited spreadsheet.');
                    var div = cswPrivate['divStep' + StepNo];
                    var tbl = div.table();
                    var row = 1;
                    
                    tbl.cell(row,1).text("1. Download the excel spreadsheet (.csv format): ");
                    row++;
                    
                    cswPrivate.downloadButton = tbl.cell(row, 1).buttonExt({
                        name: 'downloadDataBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docexport),
                        enabledText: 'Download',
                        disabledText: 'Download',
                        disableOnClick: false,
                        onClick: function() {

                            Csw.ajaxWcf.post({
                                urlMethod: 'Nodes/downloadBatchEditData',
                                data: {
                                    CAFSchema: fields['CAF Database Name'].val(),
                                    CAFPassword: fields['CAF Database Password'].val(),
                                    CAFDatabase: fields['CAF Server'].val(),
                                },
                                success: function(data) {
                                    // show success or show progress
                                    inputDialog.close();
                                }
                            });
                        }
                    }); // downloadButton buttonExt()
                    row++;
                    
                    tbl.cell(row,1).append("&nbsp;");
                    row++;

                    tbl.cell(row,1).text("2. Make desired changes to the spreadsheet, and save it as an .xlsx.");
                    row++;

                    tbl.cell(row,1).append("&nbsp;");
                    row++;

                    tbl.cell(row,1).text("3. Upload the edited excel spreadsheet (.xlsx format): ");
                    row++;

                    cswPrivate.uploadButton = tbl.cell(row,1).buttonExt({
                        name: 'uploadDataBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docimport),
                        enabledText: 'Upload',
                        disabledText: 'Upload',
                        disableOnClick: false,
                        onClick: function () {
                            Csw.dialogs.fileUpload({
                                urlMethod: 'Services/Nodes/uploadBatchEditData',
//                                params: {
//                                    defname: cswPrivate.selDefName.val(),
//                                    overwrite: cswPrivate.cbOverwrite.checked
//                                },
                                forceIframeTransport: true,
                                dataType: 'iframe',
                                onSuccess: function (response) {
                                    var batchid = Csw.number(Csw.getPropFromIFrame(response, 'batchid', false), Csw.int32MinVal);
                                    //cswPrivate.makeStatusTable();
                                }
                            }).open();
                        }
                    }); // uploadButton buttonExt()
                    row++;
                    
                };
            }()),
            onStepChange: function () {}
        }; // wizardStepPerformEdits

        
        //#endregion Steps


        //#region Finish
        cswPrivate.finalize = function () {
            cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
            
            Csw.ajaxWcf.post({
                urlMethod: 'Nodes/finishBatchEdit',
                data: {
                    //Choices: cswPrivate.mergeData
                },
                success: function(data) {
                    Csw.tryExec(cswPrivate.onFinish);
                } // success()
            }); // ajax
        };
        //#endregion Finish

        //#region ctor _post
        (function _post() {
            var wizardSteps = cswPrivate.setWizardSteps();
            cswPrivate.currentStepNo = cswPrivate.startingStep;

            cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                Title: 'Batch Edit',
                StepCount: cswPrivate.stepCount,
                Steps: wizardSteps,
                StartingStep: cswPrivate.startingStep,
                FinishText: 'Finish',
                onNext: cswPrivate.handleStep,
                onPrevious: cswPrivate.handleStep,
                onCancel: cswPrivate.onCancel,
                onFinish: cswPrivate.finalize,
                doNextOnInit: false
            });
            cswPrivate.stepFunc[1](1);
        }());
        //#endregion ctor _post

        return cswPublic;
    });
}());