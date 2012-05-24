/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.layouts.wizard = Csw.layouts.wizard ||
        Csw.layouts.register('wizard', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
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
                $.extend(cswPrivate, options);
            }

            var cswPublic = {};

            cswPrivate.getCurrentStepNo = function () {
                return cswPrivate.currentStepNo;
            };

            cswPrivate.selectStep = function (stepno) {
                if (stepno > 0 && stepno <= cswPrivate.StepCount) {
                    cswPrivate.currentStepNo = stepno;
                    Csw.each(cswPrivate.stepDivLinks, function (val, key) {
                        if (val) {
                            if (key !== stepno) {
                                val.removeClass('CswWizard_StepLinkDivSelected');
                            } else {
                                val.addClass('CswWizard_StepLinkDivSelected');
                            }
                        }
                    });

                    Csw.each(cswPrivate.stepDivs, function (val, key) {
                        if (val) {
                            if (key !== stepno) {
                                val.hide();
                            } else {
                                val.show();
                            }
                        }
                    });

                    if (stepno <= cswPrivate.StartingStep) {
                        cswPublic.previous.disable();
                    } else {
                        cswPublic.previous.enable();
                    }

                    if (stepno >= cswPrivate.StepCount) {
                        cswPublic.next.disable();
                    } else {
                        cswPublic.next.enable();
                    }
                } // if(stepno <= stepcount)
            }; // _selectStep()

            (function () {
                var indexCell, stepsCell, s, steptitle;

                if (cswPrivate.StartingStep > cswPrivate.SelectedStep) {
                    cswPrivate.SelectedStep = cswPrivate.StartingStep;
                }

                cswPublic.table = cswParent.table({
                    suffix: cswPrivate.ID,
                    TableCssClass: 'CswWizard_WizardTable'
                });

                /* Title Cell */
                cswPublic.table.cell(1, 1).text(cswPrivate.Title)
                    .propDom('colspan', 2)
                    .addClass('CswWizard_TitleCell');

                indexCell = cswPublic.table.cell(2, 1)
                    .propDom('rowspan', 2)
                    .addClass('CswWizard_IndexCell');

                stepsCell = cswPublic.table.cell(2, 2)
                    .addClass('CswWizard_StepsCell');

                for (s = 1; s <= cswPrivate.StepCount; s += 1) {
                    steptitle = cswPrivate.Steps[s];
                    cswPrivate.stepDivLinks[s] = indexCell.div({
                        cssclass: 'CswWizard_StepLinkDiv',
                        text: s + '.&nbsp;' + steptitle
                    }).propNonDom({ stepno: s });

                    cswPrivate.stepDivs[s] = stepsCell.div({ cssclass: 'CswWizard_StepDiv', suffix: s });

                    cswPrivate.stepDivs[s].propNonDom({ stepno: s })
                        .span({ cssclass: 'CswWizard_StepTitle', text: steptitle })
                        .br({number: 2})
                        .div({ suffix: s + '_content' });
                }

                var buttonTable = cswPublic.table.cell(3, 1).table({
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
                cswPublic.previous = bCell11.button({
                    suffix: 'prev',
                    enabledText: '< Previous',
                    disableOnClick: false,
                    onClick: function () {
                        var currentStepNo = cswPrivate.getCurrentStepNo();
                        if (false === cswPrivate.onBeforePrevious || Csw.tryExec(cswPrivate.onBeforePrevious, currentStepNo)) {
                            cswPrivate.selectStep(currentStepNo - 1);
                            Csw.tryExec(cswPrivate.onPrevious, currentStepNo - 1);
                        }
                    }
                });

                /* Next Button */
                cswPublic.next = bCell11.button({
                    suffix: 'next',
                    enabledText: 'Next >',
                    disableOnClick: false,
                    onClick: function () {
                        var currentStepNo = cswPrivate.getCurrentStepNo();
                        if (false === cswPrivate.onBeforeNext || Csw.tryExec(cswPrivate.onBeforeNext, currentStepNo)) {
                            cswPrivate.selectStep(currentStepNo + 1);
                            Csw.tryExec(cswPrivate.onNext, currentStepNo + 1);
                        }
                    }
                });

                /* Finish Button */
                cswPublic.finish = bCell11.button({
                    suffix: 'finish',
                    enabledText: cswPrivate.FinishText,
                    onClick: function () { Csw.tryExec(cswPrivate.onFinish); }
                });

                /* Cancel Button */
                cswPublic.cancel = bCell12.button({
                    suffix: 'cancel',
                    enabledText: 'Cancel',
                    onClick: function () { Csw.tryExec(cswPrivate.onCancel); }
                });

                cswPrivate.selectStep(cswPrivate.SelectedStep);
                if (cswPrivate.doNextOnInit) {
                    Csw.tryExec(cswPrivate.onNext, cswPrivate.SelectedStep);
                }

            } ());

            cswPublic.div = function (stepno) {
                var ret = null;
                if (Csw.contains(cswPrivate.stepDivs, stepno)) {
                    ret = cswPrivate.stepDivs[stepno];
                }
                if (ret === null) {
                    throw new Error('The requested wizard step [' + stepno + '] does not exist.');
                }
                return ret;
            };

            cswPublic.setStep = function (stepno) {
                cswPrivate.selectStep(stepno);
            };

            cswPublic.makeStepId = function (suffix, stepNo) {
                var step = stepNo || cswPrivate.currentStepNo;
                return Csw.makeId('step_' + step, cswPrivate.ID, suffix);
            };

            return cswPublic;
        });
} ());

