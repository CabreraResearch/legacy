/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.layouts.wizard = Csw.layouts.wizard ||
        Csw.layouts.register('wizard', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
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
                $.extend(cswPrivateVar, options);
            }

            var cswPublicRet = {};

            cswPrivateVar.getCurrentStepNo = function () {
                return cswPrivateVar.currentStepNo;
            };

            cswPrivateVar.selectStep = function (stepno) {
                if (stepno > 0 && stepno <= cswPrivateVar.StepCount) {
                    cswPrivateVar.currentStepNo = stepno;
                    Csw.each(cswPrivateVar.stepDivLinks, function (val, key) {
                        if (val) {
                            if (key !== stepno) {
                                val.removeClass('CswWizard_StepLinkDivSelected');
                            } else {
                                val.addClass('CswWizard_StepLinkDivSelected');
                            }
                        }
                    });

                    Csw.each(cswPrivateVar.stepDivs, function (val, key) {
                        if (val) {
                            if (key !== stepno) {
                                val.hide();
                            } else {
                                val.show();
                            }
                        }
                    });

                    if (stepno <= cswPrivateVar.StartingStep) {
                        cswPublicRet.previous.disable();
                    } else {
                        cswPublicRet.previous.enable();
                    }

                    if (stepno >= cswPrivateVar.StepCount) {
                        cswPublicRet.next.disable();
                    } else {
                        cswPublicRet.next.enable();
                    }
                } // if(stepno <= stepcount)
            }; // _selectStep()

            (function () {
                var indexCell, stepsCell, s, steptitle;

                if (cswPrivateVar.StartingStep > cswPrivateVar.SelectedStep) {
                    cswPrivateVar.SelectedStep = cswPrivateVar.StartingStep;
                }

                cswPublicRet.table = cswParent.table({
                    suffix: cswPrivateVar.ID,
                    TableCssClass: 'CswWizard_WizardTable'
                });

                /* Title Cell */
                cswPublicRet.table.cell(1, 1).text(cswPrivateVar.Title)
                    .propDom('colspan', 2)
                    .addClass('CswWizard_TitleCell');

                indexCell = cswPublicRet.table.cell(2, 1)
                    .propDom('rowspan', 2)
                    .addClass('CswWizard_IndexCell');

                stepsCell = cswPublicRet.table.cell(2, 2)
                    .addClass('CswWizard_StepsCell');

                for (s = 1; s <= cswPrivateVar.StepCount; s += 1) {
                    steptitle = cswPrivateVar.Steps[s];
                    cswPrivateVar.stepDivLinks[s] = indexCell.div({
                        cssclass: 'CswWizard_StepLinkDiv',
                        text: s + '.&nbsp;' + steptitle
                    }).propNonDom({ stepno: s });

                    cswPrivateVar.stepDivs[s] = stepsCell.div({ cssclass: 'CswWizard_StepDiv', suffix: s });

                    cswPrivateVar.stepDivs[s].propNonDom({ stepno: s })
                        .span({ cssclass: 'CswWizard_StepTitle', text: steptitle })
                        .br({number: 2})
                        .div({ suffix: s + '_content' });
                }

                var buttonTable = cswPublicRet.table.cell(3, 1).table({
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
                cswPublicRet.previous = bCell11.button({
                    suffix: 'prev',
                    enabledText: '< Previous',
                    disableOnClick: false,
                    onClick: function () {
                        var currentStepNo = cswPrivateVar.getCurrentStepNo();
                        if (false === cswPrivateVar.onBeforePrevious || Csw.tryExec(cswPrivateVar.onBeforePrevious, currentStepNo)) {
                            cswPrivateVar.selectStep(currentStepNo - 1);
                            Csw.tryExec(cswPrivateVar.onPrevious, currentStepNo - 1);
                        }
                    }
                });

                /* Next Button */
                cswPublicRet.next = bCell11.button({
                    suffix: 'next',
                    enabledText: 'Next >',
                    disableOnClick: false,
                    onClick: function () {
                        var currentStepNo = cswPrivateVar.getCurrentStepNo();
                        if (false === cswPrivateVar.onBeforeNext || Csw.tryExec(cswPrivateVar.onBeforeNext, currentStepNo)) {
                            cswPrivateVar.selectStep(currentStepNo + 1);
                            Csw.tryExec(cswPrivateVar.onNext, currentStepNo + 1);
                        }
                    }
                });

                /* Finish Button */
                cswPublicRet.finish = bCell11.button({
                    suffix: 'finish',
                    enabledText: cswPrivateVar.FinishText,
                    onClick: function () { Csw.tryExec(cswPrivateVar.onFinish); }
                });

                /* Cancel Button */
                cswPublicRet.cancel = bCell12.button({
                    suffix: 'cancel',
                    enabledText: 'Cancel',
                    onClick: function () { Csw.tryExec(cswPrivateVar.onCancel); }
                });

                cswPrivateVar.selectStep(cswPrivateVar.SelectedStep);
                if (cswPrivateVar.doNextOnInit) {
                    Csw.tryExec(cswPrivateVar.onNext, cswPrivateVar.SelectedStep);
                }

            } ());

            cswPublicRet.div = function (stepno) {
                var ret = null;
                if (Csw.contains(cswPrivateVar.stepDivs, stepno)) {
                    ret = cswPrivateVar.stepDivs[stepno];
                }
                if (ret === null) {
                    throw new Error('The requested wizard step [' + stepno + '] does not exist.');
                }
                return ret;
            };

            cswPublicRet.setStep = function (stepno) {
                cswPrivateVar.selectStep(stepno);
            };

            cswPublicRet.makeStepId = function (suffix, stepNo) {
                var step = stepNo || cswPrivateVar.currentStepNo;
                return Csw.makeId('step_' + step, cswPrivateVar.ID, suffix);
            };

            return cswPublicRet;
        });
} ());

