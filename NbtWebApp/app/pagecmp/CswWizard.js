
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = "CswWizard";

    var methods = {
        'init': function (options) {
            var o = {
                name: '',
                Title: 'A Wizard',
                StepCount: 1,
                Steps: { 1: 'Default' },
                StartingStep: 1,
                SelectedStep: 1,
                FinishText: 'Finish',
                onNext: function () {
                },
                onPrevious: function () {
                },
                onBeforeNext: function () {
                    return true;
                },
                onBeforePrevious: function () {
                    return true;
                },
                onFinish: function () {
                },
                onCancel: function () {
                },
                doNextOnInit: true
            };
            if (options) {
                Csw.extend(o, options);
            }

            if (o.StartingStep > o.SelectedStep) {
                o.SelectedStep = o.StartingStep;
            }

            var $parent = $(this);
            var table = Csw.literals.table({
                $parent: $parent,
                name: o.name,
                TableCssClass: 'CswWizard_WizardTable'
            });
            table.$.attr({
                stepcount: o.StepCount,
                startingstep: o.StartingStep
            });

            /* Title Cell */
            table.cell(1, 1).text(o.Title)
                .$.prop('colspan', 2)
                .addClass('CswWizard_TitleCell');

            var indexCell = table.cell(2, 1)
                .$.prop('rowspan', 2)
                .addClass('CswWizard_IndexCell');

            var stepsCell = table.cell(2, 2)
                .addClass('CswWizard_StepsCell');

            for (var s = 1; s <= o.StepCount; s += 1) {
                var steptitle = o.Steps[s];
                indexCell.append('<div class="CswWizard_StepLinkDiv" stepno="' + s + '">' + s + '.&nbsp;' + steptitle + '</div>');
                stepsCell.append('<div class="CswWizard_StepDiv" id="' + o.name + '_' + s + '" stepno="' + s + '" ><span class="CswWizard_StepTitle">' + steptitle + '</span><br/></br><div id="' + o.name + '_' + s + '_content"></div></div>');
            }

            var buttonTable = table.cell(3, 1).table({
                name: o.name + '_btntbl',
                width: '100%'
            });
            buttonTable.addClass('CswWizard_ButtonsCell');

            var bCell11 = buttonTable.cell(1, 1);
            bCell11.$.prop({
                'align': 'right',
                'width': '65%'
            });
            var bCell12 = buttonTable.cell(1, 2);
            bCell12.$.prop({
                'align': 'right',
                'width': '35%'
            });

            /* Previous Button */
            var prev = bCell11.button({ "name": o.name + '_prev',
                'enabledText': '< Previous',
                'disableOnClick': false,
                'cssclass': o.name + '_prev',
                'isEnabled': o.SelectedStep > o.StartingStep,
                'onClick': function () {
                    var currentStepNo = _getCurrentStepNo(table);
                    if (Csw.tryExec(o.onBeforePrevious, table.$, currentStepNo)) {
                        _selectStep(table, currentStepNo - 1);
                        Csw.tryExec(o.onPrevious, table.$, currentStepNo - 1);
                        
                        // redundant with _selectStep, but works with new buttons
                        if(currentStepNo-1 <= o.StartingStep) {
                            prev.disable();
                        }
                        if(currentStepNo-1 < o.StepCount) {
                            next.enable();
                        }
                    }
                }
            });
            /* Next Button */
            var next = bCell11.button({ "name": o.name + '_next',
                'enabledText': 'Next >',
                'disableOnClick': false,
                'cssclass': o.name + '_next',
                'onClick': function () {
                    var currentStepNo = _getCurrentStepNo(table);
                    if (Csw.tryExec(o.onBeforeNext, table.$, currentStepNo)) {
                        _selectStep(table, currentStepNo + 1);
                        Csw.tryExec(o.onNext, table.$, currentStepNo + 1);

                        // redundant with _selectStep, but works with new buttons
                        if(currentStepNo+1 > o.StartingStep) {
                            prev.enable();
                        }
                        if(currentStepNo+1 >= o.StepCount) {
                            next.disable();
                        }
                    }
                }
            });
            /* Finish Button */
            bCell11.button({ "name": o.name + '_finish',
                'enabledText': o.FinishText,
                'cssclass': o.name + '_finish',
                'onClick': function () { Csw.tryExec(o.onFinish, table); }
            });
            /* Cancel Button */
            bCell12.button({ "name": o.name + '_cancel',
                'enabledText': 'Cancel',
                'cssclass': o.name + '_cancel',
                'onClick': function () { Csw.tryExec(o.onCancel, table); }
            });

            _selectStep(table, o.SelectedStep);
            if (o.doNextOnInit) {
                Csw.tryExec(o.onNext, table.$, o.SelectedStep);
            }

            return table.$;
        }, // init()

        div: function (stepno) {
            var $table = $(this);
            return $table.find('.CswWizard_StepDiv[stepno=' + stepno + '] div');
        },

        // e.g. $wizard.CswWizard('button', 'next', 'disable');
        button: function (button, action) {
            var $table = $(this);
            var ret = null;
            switch (button) {
                case 'previous':
                    ret = $('.' + $table.attr('name') + '_prev');
                    break;
                case 'next':
                    ret = $('.' + $table.attr('name') + '_next');
                    break;
                case 'finish':
                    ret = $('.' + $table.attr('name') + '_finish');
                    break;
                case 'cancel':
                    ret = $('.' + $table.attr('name') + '_cancel');
                    break;
            }
            switch (action) {
                case 'enable':
                    enable(ret);
                    break;
                case 'disable':
                    disable(ret);
                    break;
            }
            return ret;
        },

        setStep: function (stepno) {
            var $table = $(this);
            var table = Csw.literals.factory($table, {});
            _selectStep(table, stepno);
        }
    };

    function enable($button) {
        if ($button && $button.length > 0) {
            $button.button({ label: $button.attr('enabledText'), disabled: false });
        }
    }
    function disable($button) {
        if ($button && $button.length > 0) {
            $button.button({ label: $button.attr('disabledText'), disabled: true });
        }
    }

    function _getCurrentStepNo(table) {
        return Csw.number(table.find('.CswWizard_StepLinkDivSelected').$.attr('stepno'));
    }

    function _selectStep(table, stepno) {
        var stepcount = +table.$.attr('stepcount');
        var startingstep = +table.$.attr('startingstep');
        if (stepno > 0 && stepno <= stepcount) {
            table.find('.CswWizard_StepLinkDiv').removeClass('CswWizard_StepLinkDivSelected');
            table.find('.CswWizard_StepLinkDiv[stepno=' + stepno + ']').addClass('CswWizard_StepLinkDivSelected');

            table.find('.CswWizard_StepDiv').hide();
            table.find('.CswWizard_StepDiv[stepno=' + stepno + ']').show();

            var $prevbtn = $('#' + table.$.prop('id') + '_prev');
            if (stepno <= startingstep) {
                disable($prevbtn);
            } else {
                enable($prevbtn);
            }

            var $nextbtn = $('#' + table.$.prop('id') + '_next');
            if (stepno >= stepcount) {
                disable($nextbtn);
            } else {
                enable($nextbtn);
            }
        } // if(stepno <= stepcount)
    } // _selectStep()

    // Method calling logic
    $.fn.CswWizard = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };

})(jQuery);

