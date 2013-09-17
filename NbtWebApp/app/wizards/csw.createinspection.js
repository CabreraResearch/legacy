/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {


    Csw.nbt.register('createInspectionWizard', function (cswParent, options) {
        'use strict';

        //#region Variable Declaration
        var cswPrivate = {
            name: 'cswInspectionDesignWizard',
            onCancel: null, //function ($wizard) {},
            onFinish: null, //function ($wizard) {},
            startingStep: 1,
            wizard: '',
            wizardSteps: {
                1: Csw.enums.wizardSteps_InspectionDesign.step1.description,
                2: Csw.enums.wizardSteps_InspectionDesign.step2.description,
                3: Csw.enums.wizardSteps_InspectionDesign.step3.description,
                4: Csw.enums.wizardSteps_InspectionDesign.step4.description,
                5: Csw.enums.wizardSteps_InspectionDesign.step5.description
            },
            buttons: {
                next: 'next',
                prev: 'previous',
                finish: 'finish',
                cancel: 'cancel'
            },
            divStep1: '', divStep2: '', divStep3: '', divStep4: '', divStep5: '',
            selectedInspectionTarget: '',
            inspectionTargetSelect: '',
            addNewTarget: '',
            categoryNameInput: '',
            selectedInspectionDesign: { id: '[Create New]', name: '[Create New]' },
            categoryName: '',
            inspectionDesignSelect: '',
            gridIsPopulated: false,
            inspectionGrid: '',
            gridOptions: {}
        };
        if (options) Csw.extend(cswPrivate, options);

        var cswPublic = cswParent.div();
        cswPrivate.currentStepNo = cswPrivate.startingStep;

        cswPrivate.isNewInspectionDesign = function () {
            return ('[Create New]' === cswPrivate.selectedInspectionDesign.id);
        };

        cswPrivate.isNewTarget = (function () {
            var ret = false;
            return function (isNew) {
                if (arguments.length > 0) {
                    ret = Csw.bool(isNew);
                }
                return ret;
            };
        }());

        cswPrivate.createInspectionEvents = {
            targetNameChanged: 'targetNameChanged',
            designNameChanged: 'designNameChanged'
        };

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
            if (button !== cswPrivate.buttons.finish) {
                cswPrivate.toggleButton(cswPrivate.buttons.finish, (cswPrivate.currentStepNo === 5));
            }

            return false;
        };

        cswPrivate.validationFailed = function () {
            cswPrivate.toggleButton(cswPrivate.buttons.next, true);
            cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
        };

        cswPrivate.checkTargetIsClientSideUnique = function () {
            var ret = false;
            if (Csw.string(cswPrivate.selectedInspectionTarget).trim().toLowerCase() != Csw.string(cswPrivate.selectedInspectionDesign.name).trim().toLowerCase()) {
                ret = true;
            } else {
                cswPrivate.validationFailed();
                var err = Csw.error.makeErrorObj(Csw.enums.errorType.error,
                                                 'An Inspection Design and an Inspection Target cannot have the same name.',
                                                 'Attempted to create Inspection Target ' + cswPrivate.selectedInspectionTarget + ' against Inspection Design ' + cswPrivate.selectedInspectionDesign.name);
                Csw.error.showError(err);
            }
            return ret;
        };

        cswPrivate.checkIsNodeTypeNameUnique = function (name, success, error) {
            if (cswPrivate.checkTargetIsClientSideUnique()) {
                Csw.ajax.deprecatedWsNbt({
                    urlMethod: 'IsNodeTypeNameUnique',
                    data: { 'NodeTypeName': name },
                    success: function (data) {
                        Csw.tryExec(success, data);
                    },
                    error: function (data) {
                        cswPrivate.validationFailed();
                        Csw.tryExec(error, data);
                    }
                });
            }
        };

        //Step 1. Select an Inspection Target.
        cswPrivate.makeStepOne = (function () {
            var stepOneComplete = false,
                inspectionTable, addBtnCell, addBtn, rowOneTable;
            return function () {

                var onNodeTypeSelectSuccess = function (data) {

                    if (data.nodetypecount === 0) {//If the picklist is empty, we have to add a new Target
                        cswPrivate.wizard.next.disable();
                        cswPrivate.inspectionTargetSelect.hide();
                    } else {
                        cswPrivate.wizard.next.enable();
                    }

                    var selectedTarget = cswPrivate.inspectionTargetSelect.find(':selected');
                    if (false === Csw.isNullOrEmpty(selectedTarget)) {
                        cswPrivate.selectedInspectionTarget = selectedTarget.text();
                    }

                    addBtnCell = addBtnCell || rowOneTable.cell(2, 3);
                    addBtnCell.css({ 'padding': '1px', 'vertical-align': 'middle' });
                    addBtn = addBtnCell.div().buttonExt({
                        enabledText: 'Add New',
                        disableOnClick: false,
                        size: 'small',
                        tooltip: { title: 'Add new Target Type' },
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                        onClick: function () {
                            $.CswDialog('AddNodeTypeDialog', {
                                objectclassid: data.objectClassId,
                                nodetypename: '',
                                category: 'Checklist',
                                select: cswPrivate.inspectionTargetSelect,
                                nodeTypeDescriptor: 'Target',
                                maxlength: 40,
                                onSuccess: function (newData) {
                                    var proposedInspectionTarget = newData.nodetypename;
                                    if (cswPrivate.checkTargetIsClientSideUnique(proposedInspectionTarget)) {
                                        cswPrivate.selectedInspectionTarget = proposedInspectionTarget;
                                        cswPrivate.isNewTarget(true);
                                        cswPrivate.wizard.next.enable();
                                        cswPrivate.inspectionTargetSelect.show();
                                        Csw.publish(cswPrivate.createInspectionEvents.targetNameChanged);
                                    } else {
                                        cswPrivate.inspectionTargetSelect.find('option[value="' + proposedInspectionTarget + '"]').remove();
                                    }
                                },
                                title: 'Create a New Inspection Target Type.'
                            });
                            return false;
                        }
                    });
                };

                var makeTargetSelect = function () {
                    if (false === Csw.isNullOrEmpty(cswPrivate.inspectionTargetSelect, true)) {
                        cswPrivate.inspectionTargetSelect.remove();
                    }

                    cswPrivate.inspectionTargetSelect = rowOneTable.cell(2, 1)
                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                        .div()
                        .nodeTypeSelect({
                            name: 'nodeTypeSelect',
                            objectClassName: 'InspectionTargetClass',
                            onSelect: function () {
                                var selected = cswPrivate.inspectionTargetSelect.find(':selected');
                                cswPrivate.isNewTarget(selected.propNonDom('data-newNodeType'));
                                cswPrivate.selectedInspectionTarget = selected.text();
                                Csw.publish(cswPrivate.createInspectionEvents.targetNameChanged);
                            },
                            onSuccess: function (data) {
                                onNodeTypeSelectSuccess(data);
                            }
                        });
                };

                if (false === stepOneComplete) {
                    cswPrivate.divStep1 = cswPrivate.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step1.step);
                    cswPrivate.divStep1.br();

                    inspectionTable = cswPrivate.divStep1.table({
                        name: 'setInspectionTargetTable'
                    });

                    rowOneTable = inspectionTable.cell(1, 1).table({
                        FirstCellRightAlign: true
                    });

                    rowOneTable.cell(1, 1)
                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                        .span({ text: 'What do you want to inspect?' });

                    makeTargetSelect();
                    stepOneComplete = true;
                } // if (false === stepOneComplete)

                cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
                cswPrivate.toggleButton(cswPrivate.buttons.next, (false === Csw.isNullOrEmpty(cswPrivate.selectedInspectionTarget)));
            };
        }());

        //Step 2. Select an Inspection Design Design.
        cswPrivate.makeStepTwo = (function () {
            var stepTwoComplete = false;

            return function () {
                function makeTempInspectionDesignName(name) {
                    var ret = Csw.string(name).trim();
                    if (-1 === ret.indexOf('Checklist') && -1 === ret.indexOf('checklist')) {
                        ret += ' Checklist';
                    }
                    return ret;
                }

                var inspectionTable,
                    tempInspectionName = makeTempInspectionDesignName(cswPrivate.selectedInspectionTarget),
                    tempCategoryName = cswPrivate.selectedInspectionTarget;

                var toggleNewDesignName = function () {
                    if (cswPrivate.isNewInspectionDesign()) {
                        cswPrivate.newDesignName.show();
                        cswPrivate.newDesignNameLabel.show();
                    } else {
                        cswPrivate.newDesignName.hide();
                        cswPrivate.newDesignNameLabel.hide();
                    }
                };
                var nextBtnEnabled = function () {
                    return (false === Csw.isNullOrEmpty(cswPrivate.selectedInspectionDesign.name));
                };

                var targetChangedHandle = function () {
                    cswPrivate.newDesignName.val(cswPrivate.selectedInspectionTarget);
                    cswPrivate.categoryNameInput.val(cswPrivate.selectedInspectionTarget);
                    if (cswPrivate.isNewInspectionDesign()) {
                        cswPrivate.selectedInspectionDesign.name = cswPrivate.selectedInspectionTarget;
                    }
                    Csw.publish(cswPrivate.createInspectionEvents.designNameChanged);
                };

                cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());

                if (false === stepTwoComplete) {
                    cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step2.step);
                    cswPrivate.divStep2.empty();
                    cswPrivate.divStep2.br();

                    Csw.subscribe(cswPrivate.createInspectionEvents.targetNameChanged, targetChangedHandle);

                    inspectionTable = cswPrivate.divStep2.table({
                        name: 'inspectionTable',
                        FirstCellRightAlign: true
                    });

                    //1. Copy from Inspection Design
                    inspectionTable.cell(1, 1)
                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                        .span({ text: 'Select an Inspection Design&nbsp' });

                    cswPrivate.inspectionDesignSelect = inspectionTable.cell(1, 2)
                        .div()
                        .nodeTypeSelect({
                            name: 'nodeTypeSelect',
                            objectClassName: 'InspectionDesignClass',
                            blankOptionText: '[Create New]'
                        })
                        .change(function () {
                            var selected = cswPrivate.inspectionDesignSelect.find(':selected');
                            cswPrivate.selectedInspectionDesign.id = selected.val();
                            if (cswPrivate.isNewInspectionDesign() && cswPrivate.newDesignName && false === Csw.isNullOrEmpty(cswPrivate.newDesignName.val())) {
                                cswPrivate.selectedInspectionDesign.name = cswPrivate.newDesignName.val();
                            } else {
                                cswPrivate.selectedInspectionDesign.name = selected.text();
                            }
                            Csw.publish(cswPrivate.createInspectionEvents.designNameChanged);
                            toggleNewDesignName();
                        });
                    //Create New is selected by default
                    cswPrivate.selectedInspectionDesign.id = cswPrivate.inspectionDesignSelect.val();
                    cswPrivate.selectedInspectionDesign.name = tempInspectionName;

                    inspectionTable.cell(2, 1).br();

                    //2. New Inspection Design Name
                    cswPrivate.newDesignNameLabel = inspectionTable.cell(3, 1)
                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                        .span({ text: 'New Inspection Design Name' });

                    cswPrivate.newDesignName = inspectionTable.cell(3, 2)
                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                        .input({
                            name: cswPrivate.name + '_newDesignName',
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'required',
                            maxlength: 50,
                            width: (50 * 7) + 'px',
                            value: tempInspectionName
                        })
                        .bind('change keypress keydown keyup', function () {
                            setTimeout(function () {
                                var newInspectionDesignName = cswPrivate.newDesignName.val();
                                cswPrivate.selectedInspectionDesign.id = '[Create New]';
                                cswPrivate.selectedInspectionDesign.name = newInspectionDesignName;
                                cswPrivate.toggleButton(cswPrivate.buttons.next, nextBtnEnabled());
                                Csw.publish(cswPrivate.createInspectionEvents.designNameChanged);
                            }, 10);
                        });

                    inspectionTable.cell(5, 1).br();

                    //2. Category Name
                    inspectionTable.cell(6, 1)
                        .css({ 'padding': '1px', 'vertical-align': 'middle' })
                        .span({ text: 'Category Name&nbsp' });

                    cswPrivate.categoryNameInput = cswPrivate.categoryNameInput ||
                        inspectionTable.cell(6, 2)
                            .css({ 'padding': '1px', 'vertical-align': 'middle' })
                            .input({
                                name: cswPrivate.name + '_newDesignCategory',
                                type: Csw.enums.inputTypes.text,
                                value: tempCategoryName,
                                maxlength: 40,
                                width: (40 * 7) + 'px'
                            });

                    toggleNewDesignName();

                    inspectionTable.cell(6, 1).br();
                }
                stepTwoComplete = true;
            };
        }());

        //File upload onSuccess event to prep Step 4
        cswPrivate.makeInspectionDesignGrid = function (onSuccess) {
            Csw.tryExec(onSuccess);
            cswPrivate.gridIsPopulated = true;

            //This is ugly. Abstract the step div from this function.
            cswPrivate.divStep4 = cswPrivate.divStep4 || cswPrivate.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step4.step);
            cswPrivate.divStep4.empty();

            var helpText = cswPrivate.divStep4.div();
            helpText.p({ text: '<p>Review the <b>' + cswPrivate.selectedInspectionDesign.name + '</b> upload results.' });
            helpText.p({ text: Csw.string(cswPrivate.importMsg) });
            var designChangeHandle = function () {
                helpText.empty();
                helpText.p({ text: '<p>Review the <b>' + cswPrivate.selectedInspectionDesign.name + '</b> upload results.' });
                helpText.p({ text: Csw.string(cswPrivate.importMsg) });
                cswPrivate.toggleButton(cswPrivate.buttons.next, true);
            };
            Csw.subscribe(cswPrivate.createInspectionEvents.designNameChanged, designChangeHandle);

            cswPrivate.inspectionGridDiv = cswPrivate.divStep4.div();

            if (false === Csw.contains(cswPrivate.gridJson, 'data') || false === Csw.contains(cswPrivate.gridJson, 'columns') || cswPrivate.gridJson.columns.length === 0) {
                Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Inspection Design upload failed. Please check your design and try again.'));
                cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
            } else {
                var previewId = cswPrivate.name + '_previewGrid';
                cswPrivate.gridOptions = {
                    name: previewId,
                    storeId: previewId,
                    title: 'Preview Inspection Design',
                    stateId: previewId,
                    usePaging: false,
                    showActionColumn: false,
                    canSelectRow: false,
                    onLoad: null,   // function()
                    onEdit: null,   // function(row)
                    onDelete: null, // function(row)
                    onSelect: null, // function(row)
                    onDeselect: null, // function(row)
                    height: 200,
                    forcefit: true,
                    width: '100%',
                    fields: cswPrivate.gridJson.fields,
                    columns: cswPrivate.gridJson.columns,
                    data: cswPrivate.gridJson.data
                };
                cswPrivate.inspectionGrid = cswPrivate.inspectionGridDiv.grid(cswPrivate.gridOptions);
            }
        };

        //If this is a new Design, upload the template. Otherwise skip to step 5.
        cswPrivate.makeStepThree = (function () {
            var stepThreeComplete = false;

            return function (forward) {
                //this is somewhat dundant, but these calls are cheap and it improves readability until we have time to tighten our shot group
                var nextIsEnabled = function () {
                    return cswPrivate.gridIsPopulated || false === cswPrivate.isNewInspectionDesign();
                };
                var doNextClick = function () {
                    return false === cswPrivate.isNewInspectionDesign() && forward;
                };
                var doPrevClick = function () {
                    return false === cswPrivate.isNewInspectionDesign() && false === Csw.bool(forward);
                };

                var doStepThree = function () {
                    var step3List, templateLink, uploadP, helpText;
                    var designChangeHandle = function (help) {

                        helpText.empty();
                        helpText.span({ text: 'Create a new <b>' + cswPrivate.selectedInspectionDesign.name + '</b> Design using the Excel template.' })
                            .p()
                            .a({ href: 'etc/InspectionDesign.xls', text: 'Download Template' })
                            .$.button();
                    };
                    if (false === stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step3.step);
                        cswPrivate.divStep3.empty();

                        //Ordered instructions
                        step3List = cswPrivate.divStep3.ol({
                            name: 'uploadTemplateList'
                        });

                        //1. Download template

                        helpText = step3List.li();
                        designChangeHandle();
                        Csw.subscribe(cswPrivate.createInspectionEvents.designNameChanged, designChangeHandle);

                        //2. Edit the template.
                        step3List.li().span({ text: 'Edit the Inspection template.' });

                        //3. Upload the design
                        uploadP = step3List.li()
                                            .span({ text: 'Upload the completed Inspection Design.' })
                                            .p();


                        uploadP.fileUpload({
                            url: Csw.enums.ajaxUrlPrefix + '/previewInspectionFile',
                            onSuccess: function (data) {
                                cswPrivate.gridJson = {};
                                if (Csw.contains(data, 'grid')) {
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, true, true);
                                    cswPrivate.gridJson = data.grid;
                                    cswPrivate.importMsg = data.error;
                                    cswPrivate.makeInspectionDesignGrid();
                                } else {
                                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                                    Csw.debug.error('Could not upload the provided file');
                                }
                            }
                        });



                        //$fileUploadBtn.hide();
                        //stepThreeComplete = true;
                    }
                }; //doStepTwo

                if (cswPrivate.isNewInspectionDesign()) {
                    doStepThree();
                }

                cswPrivate.toggleButton(cswPrivate.buttons.next, nextIsEnabled(), doNextClick());
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, doPrevClick());
            };
        }());

        //Step 4. Review the Design grid.
        cswPrivate.makeStepFour = (function () {
            var stepFourComplete = false;
            //We populate this step as the result of the async design upload. Improve the readability of this code when you next visit.
            return function (forward) {
                var skipStepFour = false;
                var doNextClick = function () {
                    skipStepFour = (false === cswPrivate.isNewInspectionDesign() && forward);
                    return skipStepFour;
                };
                var doPrevClick = function () {
                    skipStepFour = (false === cswPrivate.isNewInspectionDesign() && false == Csw.bool(forward));
                    return skipStepFour;
                };

                cswPrivate.toggleButton(cswPrivate.buttons.next, true, doNextClick());
                cswPrivate.toggleButton(cswPrivate.buttons.prev, true, doPrevClick());

                if (false === stepFourComplete &&
                    false === skipStepFour) {
                    cswPrivate.divStep4 = cswPrivate.divStep4 || cswPrivate.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step4.step);
                    stepFourComplete = true;
                }
            };
        }());

        //Step 5. Preview and Finish.
        cswPrivate.makeStepFive = (function () {

            return function () {
                var confirmationList, confirmTypesList, confirmViewsList, confirmGridOptions = {};

                if (cswPrivate.checkTargetIsClientSideUnique()) {

                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.next, false);

                    cswPrivate.categoryName = cswPrivate.categoryNameInput.val();

                    cswPrivate.divStep5 = cswPrivate.divStep5 || cswPrivate.wizard.div(Csw.enums.wizardSteps_InspectionDesign.step5.step);
                    cswPrivate.divStep5.empty();

                    cswPrivate.divStep5.p({ text: 'You are about to create the following items. Click Finish to create the design.' });
                    confirmationList = cswPrivate.divStep5.ol({
                        name: 'confirmationList'
                    });

                    if (cswPrivate.isNewInspectionDesign()) {
                        if (cswPrivate.gridOptions) {
                            Csw.extend(confirmGridOptions, cswPrivate.gridOptions, true);
                        }

                        confirmGridOptions.name = 'confirmGrid';
                        confirmGridOptions.storeId = 'confirmGrid';
                        confirmGridOptions.stateId = 'confirmGrid';
                        confirmGridOptions.height = 150;
                        confirmGridOptions.onEdit = null;
                        confirmGridOptions.onDelete = null;

                        var confirmGridParent = confirmationList.li({
                            text: 'Creating a new Inspection Design <b>' + cswPrivate.selectedInspectionDesign.name + '</b>.'
                        });
                        confirmGridParent.div().grid(confirmGridOptions);
                    } else {
                        confirmationList.li({
                            text: 'Creating a new copy of Inspection Design <b>' + cswPrivate.selectedInspectionDesign.name + '</b> to Inspection Target <b> ' + cswPrivate.selectedInspectionTarget + '</b>.'
                        }).br();
                    }

                    if (cswPrivate.isNewInspectionDesign() || cswPrivate.isNewTarget()) {
                        confirmTypesList = confirmationList.li({
                            text: 'New Types'
                        })
                            .ul({
                                name: 'confirmationTypes'
                            });

                        if (cswPrivate.isNewInspectionDesign()) {
                            confirmTypesList.li({
                                text: 'New Inspection Design <b>' + cswPrivate.selectedInspectionDesign.name + '</b> on Inspection Target <b>' + cswPrivate.selectedInspectionTarget + '</b>'
                            });
                        }

                        if (cswPrivate.isNewTarget()) {
                            confirmTypesList.li({
                                text: 'New Inspection Target <b>' + cswPrivate.selectedInspectionTarget + '</b>'
                            });

                            confirmTypesList.li({
                                text: 'New Inspection Target Group <b>' + cswPrivate.selectedInspectionTarget + ' Group</b>'
                            });
                        }
                    }

                    confirmViewsList = confirmationList.li({
                        text: 'New Views'
                    })
                        .ul({
                            name: 'confirmationViews'
                        });
                    if (cswPrivate.isNewTarget()) {
                        confirmViewsList.li({
                            text: '<b>Scheduling: ' + cswPrivate.selectedInspectionTarget + '</b>'
                        });
                        confirmViewsList.li({
                            text: '<b>Groups: ' + cswPrivate.selectedInspectionTarget + '</b>'
                        });
                    }
                    confirmViewsList.li({
                        text: '<b>' + cswPrivate.selectedInspectionDesign.name + ' Inspections: ' + cswPrivate.selectedInspectionTarget + '</b>'
                    });
                } /*else {
                        cswPrivate.toggleButton(cswPrivate.buttons.prev, true, true);
                    }*/
            };
        }());

        cswPrivate.handleNext = function (newStepNo) {
            cswPrivate.currentStepNo = newStepNo;
            switch (newStepNo) {
                case Csw.enums.wizardSteps_InspectionDesign.step2.step:
                    cswPrivate.makeStepTwo();
                    break;
                case Csw.enums.wizardSteps_InspectionDesign.step3.step:
                    if (cswPrivate.isNewInspectionDesign()) {
                        cswPrivate.checkIsNodeTypeNameUnique(cswPrivate.selectedInspectionDesign.name, function () {
                            cswPrivate.makeStepThree(true);
                        });
                    }
                    break;
                case Csw.enums.wizardSteps_InspectionDesign.step4.step:
                    cswPrivate.makeStepFour(true); //we're moving forward
                    break;
                case Csw.enums.wizardSteps_InspectionDesign.step5.step:
                    cswPrivate.makeStepFive();
                    break;
            } // switch(newstepno)
        }; // handleNext()

        cswPrivate.handlePrevious = function (newStepNo) {
            cswPrivate.currentStepNo = newStepNo;
            switch (newStepNo) {
                case Csw.enums.wizardSteps_InspectionDesign.step1.step:
                    cswPrivate.makeStepOne();
                    break;
                case Csw.enums.wizardSteps_InspectionDesign.step2.step:
                    cswPrivate.makeStepTwo(); //we're moving backward
                    break;
                case Csw.enums.wizardSteps_InspectionDesign.step3.step:
                    cswPrivate.makeStepThree(false); //we're moving backward
                    break;
                case Csw.enums.wizardSteps_InspectionDesign.step4.step:
                    cswPrivate.makeStepFour(false);
                    break;
            }
        };

        cswPrivate.onConfirmFinish = function () {
            var designGrid = '';

            cswPrivate.toggleButton(cswPrivate.buttons.prev, false);
            cswPrivate.toggleButton(cswPrivate.buttons.next, false);
            cswPrivate.toggleButton(cswPrivate.buttons.cancel, false);
            cswPrivate.toggleButton(cswPrivate.buttons.finish, false);

            if (false === Csw.isNullOrEmpty(cswPrivate.inspectionGrid)) {
                designGrid = Csw.serialize(cswPrivate.gridJson.data.items);
            }

            var jsonData = {
                DesignGrid: designGrid,
                InspectionDesignName: Csw.string(cswPrivate.selectedInspectionDesign.name),
                InspectionTargetName: Csw.string(cswPrivate.selectedInspectionTarget),
                IsNewInspection: cswPrivate.isNewInspectionDesign(),
                IsNewTarget: cswPrivate.isNewTarget(),
                Category: Csw.string(cswPrivate.categoryName)
            };

            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'finalizeInspectionDesign',
                data: jsonData,
                success: function (data) {
                    //Come back and hammer this out
                    var views = data.views,
                            values = [];

                    Csw.each(views, function (thisView) {
                        if (Csw.contains(thisView, 'viewid') &&
                                Csw.contains(thisView, 'viewname')) {
                            values.push({
                                value: thisView.viewid,
                                display: thisView.viewname
                            });
                        }
                    });

                    $.CswDialog('NavigationSelectDialog', {
                        name: 'FinishDialog',
                        title: 'The Inspection Design Wizard Completed Successfully',
                        navigationText: 'Please select from the following views. Click OK to continue.',
                        values: values,
                        onOkClick: function (selectedView) {
                            var viewId = selectedView.val();
                            Csw.tryExec(cswPrivate.onFinish, viewId);
                        }
                    });

                },
                error: function () {
                    cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                    cswPrivate.toggleButton(cswPrivate.buttons.prev, true);
                }
            });
        };

        (function () {

            cswPrivate.wizard = Csw.layouts.wizard(cswPublic, {
                name: 'wizard',
                Title: 'Create New Inspection',
                StepCount: Csw.enums.wizardSteps_InspectionDesign.stepcount,
                Steps: cswPrivate.wizardSteps,
                StartingStep: cswPrivate.startingStep,
                FinishText: 'Finish',
                onNext: cswPrivate.handleNext,
                onPrevious: cswPrivate.handlePrevious,
                onCancel: cswPrivate.onCancel,
                onFinish: cswPrivate.onConfirmFinish,
                doNextOnInit: false
            });

            cswPrivate.makeStepOne();
        }());

        return cswPublic;
    });
}());

