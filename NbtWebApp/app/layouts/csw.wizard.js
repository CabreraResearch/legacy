/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.layouts.register('wizard', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            name: 'wizard',
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
            stepRootSpans: [],
            stepDivLinks: [],
            stepVisibility: [],
            currentStepNo: 1,
            state: {

            }
        };
        if (options) {
            Csw.extend(cswPrivate, options);
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

                Csw.each(cswPrivate.stepRootSpans, function (val, key) {
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

            cswPublic.form = cswParent.form();

            cswPublic.table = cswPublic.form.table({
                suffix: cswPrivate.name,
                TableCssClass: 'CswWizard_WizardTable'
            });

            /* Title Cell */
            cswPublic.table.cell(1, 1).text(cswPrivate.Title)
                .propDom('colspan', 3)
                .addClass('CswWizard_TitleCell');

            indexCell = cswPublic.table.cell(2, 1)
                .propDom('rowspan', 2)
                .addClass('CswWizard_IndexCell');

            stepsCell = cswPublic.table.cell(2, 2)
                .addClass('CswWizard_StepsCell');

            cswPrivate.staticCell = cswPublic.table.cell(2, 3);
            cswPrivate.staticCell.css({ 'background-color': '#ddeeff' });
            cswPrivate.staticDiv = cswPrivate.staticCell.div();

            for (s = 1; s <= cswPrivate.StepCount; s += 1) {
                steptitle = cswPrivate.Steps[s];
                cswPrivate.stepDivLinks[s] = indexCell.div({
                    cssclass: 'CswWizard_StepLinkDiv',
                    text: s + '.&nbsp;' + steptitle
                }).propNonDom({ stepno: s });
                cswPrivate.stepVisibility[s] = true;

                cswPrivate.stepRootSpans[s] = stepsCell.div({ cssclass: 'CswWizard_StepDiv', suffix: s });

                cswPrivate.stepRootSpans[s].propNonDom({ stepno: s })
                    .span({ cssclass: 'CswWizard_StepTitle', text: steptitle });
                cswPrivate.stepRootSpans[s].br({ number: 2 });
                cswPrivate.stepDivs[s] = cswPrivate.stepRootSpans[s].div({ suffix: s + '_content' });
            }

            cswPrivate.btnGroupCell = cswPublic.table.cell(3, 1).propDom('colspan', 3);
            cswPrivate.btnGroup = cswPrivate.btnGroupCell.buttonGroup({
                buttons: {
                    previous: {
                        onclick: function () {
                            var currentStepNo = cswPrivate.getCurrentStepNo();
                            var priorStepNo = currentStepNo - 1;
                            if (false === cswPrivate.onBeforePrevious || Csw.tryExec(cswPrivate.onBeforePrevious, currentStepNo)) {
                                while (cswPrivate.stepDivLinks[priorStepNo].$.is(':hidden') && priorStepNo > cswPrivate.StartingStep) {
                                    priorStepNo--;
                                }
                                cswPrivate.selectStep(priorStepNo);
                                Csw.tryExec(cswPrivate.onPrevious, priorStepNo);
                            }
                        }
                    },
                    next: {
                        onclick: function () {
                            if (cswPublic.form.isFormValid()) {
                                var currentStepNo = cswPrivate.getCurrentStepNo();
                                var nextStepNo = currentStepNo + 1;
                                if (false === cswPrivate.onBeforeNext || Csw.tryExec(cswPrivate.onBeforeNext, currentStepNo)) {
                                    while (cswPrivate.stepDivLinks[nextStepNo].$.is(':hidden') && nextStepNo < cswPrivate.StepCount) {
                                        nextStepNo++;
                                    }
                                    cswPrivate.selectStep(nextStepNo);
                                    Csw.tryExec(cswPrivate.onNext, nextStepNo);
                                }
                            }
                        }
                    },
                    finish: {
                        onclick: function () {
                            if (cswPublic.form.isFormValid()) {
                                Csw.tryExec(cswPrivate.onFinish);
                            }
                        }
                    }
                },
                cancel: {
                    onclick: function () { Csw.tryExec(cswPrivate.onCancel); }
                }
            });
            cswPublic.previous = cswPrivate.btnGroup.previous;
            cswPublic.next = cswPrivate.btnGroup.next;
            cswPublic.finish = cswPrivate.btnGroup.finish;
            cswPublic.cancel = cswPrivate.btnGroup.cancel;

            cswPrivate.selectStep(cswPrivate.SelectedStep);
            if (cswPrivate.doNextOnInit) {
                Csw.tryExec(cswPrivate.onNext, cswPrivate.SelectedStep);
            }

        }());

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

        cswPublic.toggleStepVisibility = function (stepno, show) {
            if (false === Csw.isNullOrEmpty(cswPrivate.stepDivLinks[stepno])) {
                if (show) {
                    cswPrivate.stepDivLinks[stepno].show();
                    cswPrivate.stepVisibility[stepno] = true;
                } else {
                    cswPrivate.stepDivLinks[stepno].hide();
                    cswPrivate.stepVisibility[stepno] = false;
                }
            }
        };
        
        cswPublic.isStepVisible = function (stepno) {
            return cswPrivate.stepVisibility[stepno];
        };

        cswPublic.staticDiv = function () {
            cswPrivate.staticDiv.css({
                'border': '1px solid #99ccff',
                'padding': '10px',
                'height': '360px',
                'margin': '10px 10px 0px 0px',
                'background': '#ffffff',
                'overflow': 'auto'
            });
            return cswPrivate.staticDiv;
        };

        cswPublic.hideStaticDiv = function () {
            cswPrivate.staticDiv.css({
                'border': 'transparent',
                'padding': '0px',
                'margin': '0px 0px 0px 0px',
                'background': '#ffffff',
            });
        };

        return cswPublic;
    });
}());

