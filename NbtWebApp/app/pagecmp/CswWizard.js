
/// <reference path="~app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = "CswWizard";

    var methods = {
        'init': function (options) {
            var o = {
                ID: '',
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
                ID: o.ID,
                TableCssClass: 'CswWizard_WizardTable'
            });
            table.propNonDom({
                stepcount: o.StepCount,
                startingstep: o.StartingStep
            });

            /* Title Cell */
            table.cell(1, 1).text(o.Title)
                .propDom('colspan', 2)
                .addClass('CswWizard_TitleCell');

            var indexCell = table.cell(2, 1)
                .propDom('rowspan', 2)
                .addClass('CswWizard_IndexCell');

            var stepsCell = table.cell(2, 2)
                .addClass('CswWizard_StepsCell');

            for (var s = 1; s <= o.StepCount; s += 1) {
                var steptitle = o.Steps[s];
                indexCell.append('<div class="CswWizard_StepLinkDiv" stepno="' + s + '">' + s + '.&nbsp;' + steptitle + '</div>');
                stepsCell.append('<div class="CswWizard_StepDiv" id="' + o.ID + '_' + s + '" stepno="' + s + '" ><span class="CswWizard_StepTitle">' + steptitle + '</span><br/></br><div id="' + o.ID + '_' + s + '_content"></div></div>');
            }

            var buttonTable = table.cell(3, 1).table({
                ID: o.ID + '_btntbl',
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
            bCell11.button({ 'ID': o.ID + '_prev',
                'enabledText': '< Previous',
                'disableOnClick': false,
                'onClick': function () {
                    var currentStepNo = _getCurrentStepNo(table);
                    if (Csw.tryExec(o.onBeforePrevious, table.$, currentStepNo)) {
                        _selectStep(table, currentStepNo - 1);
                        Csw.tryExec(o.onPrevious, table.$, currentStepNo - 1);
                    }
                }
            });
            /* Next Button */
            bCell11.button({ 'ID': o.ID + '_next',
                'enabledText': 'Next >',
                'disableOnClick': false,
                'onClick': function () {
                    var currentStepNo = _getCurrentStepNo(table);
                    if (Csw.tryExec(o.onBeforeNext, table.$, currentStepNo)) {
                        _selectStep(table, currentStepNo + 1);
                        Csw.tryExec(o.onNext, table.$, currentStepNo + 1);
                    }
                }
            });
            /* Finish Button */
            bCell11.button({ 'ID': o.ID + '_finish',
                'enabledText': o.FinishText,
                'onClick': function () { Csw.tryExec(o.onFinish, table); }
            });
            /* Cancel Button */
            bCell12.button({ 'ID': o.ID + '_cancel',
                'enabledText': 'Cancel',
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
                    ret = $('#' + $table.prop('id') + '_prev');
                    break;
                case 'next':
                    ret = $('#' + $table.prop('id') + '_next');
                    break;
                case 'finish':
                    ret = $('#' + $table.prop('id') + '_finish');
                    break;
                case 'cancel':
                    ret = $('#' + $table.prop('id') + '_cancel');
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
        return Csw.number(table.find('.CswWizard_StepLinkDivSelected').propNonDom('stepno'));
    }

    function _selectStep(table, stepno) {
        var stepcount = +table.propNonDom('stepcount');
        var startingstep = +table.propNonDom('startingstep');
        if (stepno > 0 && stepno <= stepcount) {
            table.find('.CswWizard_StepLinkDiv').removeClass('CswWizard_StepLinkDivSelected');
            table.find('.CswWizard_StepLinkDiv[stepno=' + stepno + ']').addClass('CswWizard_StepLinkDivSelected');

            table.find('.CswWizard_StepDiv').hide();
            table.find('.CswWizard_StepDiv[stepno=' + stepno + ']').show();

            var $prevbtn = $('#' + table.propDom('id') + '_prev');
            if (stepno <= startingstep) {
                disable($prevbtn);
            } else {
                enable($prevbtn);
            }

            var $nextbtn = $('#' + table.propDom('id') + '_next');
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

