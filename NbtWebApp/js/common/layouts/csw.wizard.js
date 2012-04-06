/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.layouts.wizard = Csw.layouts.wizard ||
        Csw.layouts.register('wizard', function (cswParent, options) {
            'use strict';
            var internal = {
                ID: 'wizard',
                Title: 'A Wizard',
                StepCount: 1,
                Steps: { 1: 'Default' },
                StartingStep: 1,
                SelectedStep: 1,
                FinishText: 'Finish',
                onNext: null,
                onPrevious: null,
                onBeforeNext: false, //return true;
                onBeforePrevious: false, //return true;
                onFinish: null,
                onCancel: null,
                doNextOnInit: true,
                stepDivs: [],
                stepDivLinks: [],
                currentStepNo: 1
            };
            if (options) {
                $.extend(internal, options);
            }

            var external = {};

            internal.getCurrentStepNo = function () {
                return internal.currentStepNo;
            };

            internal.selectStep = function (stepno) {
                if (stepno > 0 && stepno <= internal.StepCount) {
                    internal.currentStepNo = stepno;
                    Csw.each(internal.stepDivLinks, function (val, key) {
                        if (val) {
                            if (key !== stepno) {
                                val.removeClass('CswWizard_StepLinkDivSelected');
                            } else {
                                val.addClass('CswWizard_StepLinkDivSelected');
                            }
                        }
                    });

                    Csw.each(internal.stepDivs, function (val, key) {
                        if (val) {
                            if (key !== stepno) {
                                val.hide();
                            } else {
                                val.show();
                            }
                        }
                    });

                    if (stepno <= internal.StartingStep) {
                        external.previous.disable();
                    } else {
                        external.previous.enable();
                    }

                    if (stepno >= internal.StepCount) {
                        external.next.disable();
                    } else {
                        external.next.enable();
                    }
                } // if(stepno <= stepcount)
            }; // _selectStep()

            (function () {
                var indexCell, stepsCell, s, steptitle;

                if (internal.StartingStep > internal.SelectedStep) {
                    internal.SelectedStep = internal.StartingStep;
                }

                external.table = cswParent.table({
                    suffix: internal.ID,
                    TableCssClass: 'CswWizard_WizardTable'
                });

                /* Title Cell */
                external.table.cell(1, 1).text(internal.Title)
                    .propDom('colspan', 2)
                    .addClass('CswWizard_TitleCell');

                indexCell = external.table.cell(2, 1)
                    .propDom('rowspan', 2)
                    .addClass('CswWizard_IndexCell');

                stepsCell = external.table.cell(2, 2)
                    .addClass('CswWizard_StepsCell');

                for (s = 1; s <= internal.StepCount; s += 1) {
                    steptitle = internal.Steps[s];
                    internal.stepDivLinks[s] = indexCell.div({
                        cssclass: 'CswWizard_StepLinkDiv',
                        text: s + '.&nbsp;' + steptitle
                    }).propNonDom({ stepno: s });

                    internal.stepDivs[s] = stepsCell.div({ cssclass: 'CswWizard_StepDiv', suffix: s });

                    internal.stepDivs[s].propNonDom({ stepno: s })
                        .span({ cssclass: 'CswWizard_StepTitle', text: steptitle })
                        .br().br()
                        .div({ suffix: s + '_content' });
                }

                var buttonTable = external.table.cell(3, 1).table({
                    suffix: 'btntbl',
                    width: '100%'
                });
                buttonTable.addClass('CswWizard_ButtonsCell');

                var bCell11 = buttonTable.cell(1, 1);
                bCell11.propDom({
                    'align': 'right',
                    'width': '65%'
                });
                var bCell12 = buttonTable.cell(1, 2);
                bCell12.propDom({
                    'align': 'right',
                    'width': '35%'
                });

                /* Previous Button */
                external.previous = bCell11.button({
                    suffix: 'prev',
                    enabledText: '< Previous',
                    disableOnClick: false,
                    onClick: function () {
                        var currentStepNo = internal.getCurrentStepNo();
                        if (false === internal.onBeforePrevious || Csw.tryExec(internal.onBeforePrevious, currentStepNo)) {
                            internal.selectStep(currentStepNo - 1);
                            Csw.tryExec(internal.onPrevious, currentStepNo - 1);
                        }
                    }
                });

                /* Next Button */
                external.next = bCell11.button({
                    suffix: 'next',
                    enabledText: 'Next >',
                    disableOnClick: false,
                    onClick: function () {
                        var currentStepNo = internal.getCurrentStepNo();
                        if (false === internal.onBeforeNext || Csw.tryExec(internal.onBeforeNext, currentStepNo)) {
                            internal.selectStep(currentStepNo + 1);
                            Csw.tryExec(internal.onNext, currentStepNo + 1);
                        }
                    }
                });

                /* Finish Button */
                external.finish = bCell11.button({
                    suffix: 'finish',
                    enabledText: internal.FinishText,
                    onClick: function () { Csw.tryExec(internal.onFinish); }
                });

                /* Cancel Button */
                external.cancel = bCell12.button({
                    suffix: 'cancel',
                    enabledText: 'Cancel',
                    onClick: function () { Csw.tryExec(internal.onCancel); }
                });

                internal.selectStep(internal.SelectedStep);
                if (internal.doNextOnInit) {
                    Csw.tryExec(internal.onNext, internal.SelectedStep);
                }

            } ());

            external.div = function (stepno) {
                var ret = null;
                if (Csw.contains(internal.stepDivs, stepno)) {
                    ret = internal.stepDivs[stepno];
                }
                if (ret === null) {
                    throw new Error('The requested wizard step [' + stepno + '] does not exist.');
                }
                return ret;
            };

            external.setStep = function (stepno) {
                internal.selectStep(stepno);
            };

            external.makeStepId = function (suffix, stepNo) {
                var step = stepNo || internal.currentStepNo;
                return Csw.makeId('step_' + step, internal.ID, suffix);
            };

            return external;
        });
} ());

