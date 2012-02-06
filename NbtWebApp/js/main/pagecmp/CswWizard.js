/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

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
                onNext: function () { },
                onPrevious: function () { },
                onBeforeNext: function () { return true; },
                onBeforePrevious: function () { return true; },
                onFinish: function () { },
                onCancel: function () {},
                doNextOnInit: true
            };
            if (options) $.extend(o, options);
            if (o.StartingStep > o.SelectedStep) o.SelectedStep = o.StartingStep;

            var $parent = $(this);

            var $table = $parent.CswTable({ ID: o.ID, TableCssClass: 'CswWizard_WizardTable' });
            $table.CswAttrDom("stepcount", o.StepCount);
            $table.CswAttrDom("startingstep", o.StartingStep);

            /* Title Cell */
            $table.CswTable('cell', 1, 1)
                .addClass('CswWizard_TitleCell')
                .CswAttrDom('colspan', 2)
                .append(o.Title);

            var $indexcell = $table.CswTable('cell', 2, 1)
                .CswAttrDom('rowspan', 2)
                .addClass('CswWizard_IndexCell');

            var $stepscell = $table.CswTable('cell', 2, 2)
                .addClass('CswWizard_StepsCell');

            var $buttonscell = $table.CswTable('cell', 3, 1)
                .addClass('CswWizard_ButtonsCell');

            for (var s = 1; s <= o.StepCount; s += 1) {
                var steptitle = o.Steps[s];

//					$('<div class="CswWizard_StepLinkDiv" stepno="' + s + '"><a href="#">' + s + '.&nbsp;' + steptitle + '</a></div>')
//										.appendTo($indexcell)
//										.children('a')
//										.click( function (stepno) { return function () { _selectStep($table, stepno); return false; }; }(s) );
                $('<div class="CswWizard_StepLinkDiv" stepno="' + s + '">' + s + '.&nbsp;' + steptitle + '</div>')
                    .appendTo($indexcell);

                $('<div class="CswWizard_StepDiv" id="' + o.ID + '_' + s + '" stepno="' + s + '" ><span class="CswWizard_StepTitle">' + steptitle + '</span><br/></br><div id="' + o.ID + '_' + s + '_content"></div></div>')
                    .appendTo($stepscell);
            }

            var $buttontbl = $buttonscell.CswTable({ ID: o.ID + '_btntbl', width: '100%' });
            var $bcell11 = $buttontbl.CswTable('cell', 1, 1)
                .CswAttrDom('align', 'right')
                .CswAttrDom('width', '65%');
            var $bcell12 = $buttontbl.CswTable('cell', 1, 2)
                .CswAttrDom('align', 'right')
                .CswAttrDom('width', '35%');

            /* Previous Button */
            $bcell11.CswButton('init', { 'ID': o.ID + '_prev',
                'enabledText': '< Previous',
                'disableOnClick': false,
                'onclick': function () {
                    var currentStepNo = _getCurrentStepNo($table);
                    if (o.onBeforePrevious($table, currentStepNo))
                    {
                        _selectStep($table, currentStepNo - 1);
                        o.onPrevious(currentStepNo - 1);
                    }
                }
            });
            /* Next Button */
            $bcell11.CswButton('init', { 'ID': o.ID + '_next',
                'enabledText': 'Next >',
                'disableOnClick': false,
                'onclick': function () {
                    var currentStepNo = _getCurrentStepNo($table);
                    if (o.onBeforeNext(currentStepNo))
                    {
                        _selectStep($table, currentStepNo + 1);
                        o.onNext($table, currentStepNo + 1);
                    }
                }
            });
            /* Finish Button */
            $bcell11.CswButton('init', { 'ID': o.ID + '_finish',
                'enabledText': o.FinishText,
                'onclick': function () { if (Csw.isFunction(o.onFinish)) o.onFinish($table); }
            });
            /* Cancel Button */
            $bcell12.CswButton('init', { 'ID': o.ID + '_cancel',
                'enabledText': 'Cancel',
                'onclick': function () { if (Csw.isFunction(o.onCancel)) o.onCancel($table); }
            });

            _selectStep($table, o.SelectedStep);
            if (o.doNextOnInit && Csw.isFunction(o.onNext)) {
                o.onNext($table, o.SelectedStep);
            }

            return $table;
        }, // init()

        div: function (stepno) {
            var $table = $(this);
            return $table.find('.CswWizard_StepDiv[stepno=' + stepno + '] div');
        },

        // e.g. $wizard.CswWizard('button', 'next', 'disable');
        button: function (button, action) {
            var $table = $(this);
            var ret;
            switch (button) {
                case 'previous':
                    ret = $('#' + $table.CswAttrDom('id') + '_prev');
                    break;
                case 'next':
                    ret = $('#' + $table.CswAttrDom('id') + '_next');
                    break;
                case 'finish':
                    ret = $('#' + $table.CswAttrDom('id') + '_finish');
                    break;
                case 'cancel':
                    ret = $('#' + $table.CswAttrDom('id') + '_cancel');
                    break;
            }
            if (false === Csw.isNullOrEmpty(ret, true)) {
                ret.CswButton(action);
            }
            return ret;
        }
    };


    function _getCurrentStepNo($table) {
        return Csw.number($table.find('.CswWizard_StepLinkDivSelected').CswAttrDom('stepno'));
    }
    
    function _selectStep($table, stepno) {
        var stepcount = $table.CswAttrDom("stepcount");
        var startingstep = $table.CswAttrDom("startingstep");
        if(stepno > 0 && stepno <= stepcount)
        {
            $table.find('.CswWizard_StepLinkDiv').removeClass('CswWizard_StepLinkDivSelected');
            $table.find('.CswWizard_StepLinkDiv[stepno='+ stepno + ']').addClass('CswWizard_StepLinkDivSelected');

            $table.find('.CswWizard_StepDiv').hide();
            $table.find('.CswWizard_StepDiv[stepno=' + stepno + ']').show();

            var $prevbtn = $('#' + $table.CswAttrDom('id') + '_prev');
            if(stepno <= startingstep) 
                $prevbtn.CswButton('disable');
            else
                $prevbtn.CswButton('enable');

            var $nextbtn = $('#' + $table.CswAttrDom('id') + '_next');
            if(stepno >= stepcount)
                $nextbtn.CswButton('disable');
            else
                $nextbtn.CswButton('enable');
        } // if(stepno <= stepcount)
    } // _selectStep()
    
    // Method calling logic
    $.fn.CswWizard = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };

}) (jQuery);

