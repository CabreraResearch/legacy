/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeQuestion';
    var multi = false;
    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var answer = (false === o.Multi) ? tryParseString(propVals.answer).trim() : CswMultiEditDefaultValue;
            var allowedAnswers = tryParseString(propVals.allowedanswers).trim();
			var compliantAnswers = tryParseString(propVals.compliantanswers).trim();
			var comments =  (false === o.Multi) ? tryParseString(propVals.comments).trim() : CswMultiEditDefaultValue;
			var correctiveAction = (false === o.Multi) ? tryParseString(propVals.correctiveaction).trim() : CswMultiEditDefaultValue;
            multi = o.Multi;
            
			var dateAnswered =  (false === o.Multi) ? tryParseString(propVals.dateanswered.date).trim() : ''; 
			var dateCorrected =  (false === o.Multi) ? tryParseString(propVals.datecorrected.date).trim() : '';

            if(o.ReadOnly) {
                $Div.append('Answer: ' + answer);
                if(dateAnswered !== '') {
					$Div.append(' ('+ dateAnswered +')');
				}
				$Div.append('<br/>');
                $Div.append('Corrective Action: ' + correctiveAction);
                if(dateCorrected !== '') {
					$Div.append(' ('+ dateCorrected +')');
				}
				$Div.append('<br/>');
                $Div.append('Comments: ' + comments + '<br/>');
            } else {
				var $table = $Div.CswTable('init', { 
													'ID': o.ID + '_tbl', 
													'FirstCellRightAlign': true 
													});

				$table.CswTable('cell', 1, 1).append('Answer');
				var splitAnswers = allowedAnswers.split(',');
                if (o.Multi) {
                    splitAnswers.push(CswMultiEditDefaultValue);
                } else {
                    splitAnswers.push('');
                }
                var $AnswerSel = $table.CswTable('cell', 1, 2)
                                       .CswSelect('init', {
                                           ID: o.ID,
                                           onChange: function() {
                                               checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
                                               o.onchange();
                                           },
                                           values: splitAnswers,
                                           selected: answer
                                       });

                var $CorrectiveActionLabel = $table.CswTable('cell', 2, 1).append('Corrective Action');
				var $CorrectiveActionTextBox = $('<textarea id="'+ o.ID +'_cor" />')
									.appendTo($table.CswTable('cell', 2, 2))
									.text(correctiveAction)
									.change(function() { 
										checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
										o.onchange();
									});

				$table.CswTable('cell', 3, 1).append('Comments');
				$('<textarea id="'+ o.ID +'_com" />')
									.appendTo($table.CswTable('cell', 3, 2))
									.text(comments)
									.change(o.onchange);

				checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
            }
        },
        save: function(o) {
            var attributes = {
                answer: null,
                correctiveaction: null,
                comments: null
            };
            var $answer = o.$propdiv.find('#' + o.ID + '_ans');
            if (false === isNullOrEmpty($answer)) {
                attributes.answer = $answer.val();
            }
            var $correct = o.$propdiv.find('#' + o.ID + '_cor');
            if (false === isNullOrEmpty($correct)) {
                attributes.correctiveaction = $correct.val();
            }
            var $comments = o.$propdiv.find('#' + o.ID + '_com');
            if (false === isNullOrEmpty($comments)) {
                attributes.comments = $comments.val();
            }
            preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
	function checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox)
	{
		if (false === multi) {
		    var splitCompliantAnswers = compliantAnswers.split(',');
		    var isCompliant = true;
		    var selectedAnswer = $AnswerSel.val();
		    var correctiveAction = $CorrectiveActionTextBox.val();

		    if (selectedAnswer !== '' && correctiveAction === '') {
		        isCompliant = false;
		        for (var i = 0; i < splitCompliantAnswers.length; i++) {
		            if (splitCompliantAnswers[i] === selectedAnswer) {
		                isCompliant = true;
		            }
		        }
		    }
		    if (isCompliant) {
		        $AnswerSel.removeClass('CswFieldTypeQuestion_OOC');
		        if (correctiveAction === '') {
		            $CorrectiveActionLabel.hide();
		            $CorrectiveActionTextBox.hide();
		        }
		    } else {
		        $AnswerSel.addClass('CswFieldTypeQuestion_OOC');
		        $CorrectiveActionLabel.show();
		        $CorrectiveActionTextBox.show();
		    }
		}
	} // checkCompliance()

    // Method calling logic
    $.fn.CswFieldTypeQuestion = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
