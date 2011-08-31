/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeQuestion';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var answer = tryParseString(propVals.answer).trim();
            var allowedAnswers = tryParseString(propVals.allowedanswers).trim();
			var compliantAnswers = tryParseString(propVals.compliantanswers).trim();
			var comments =  tryParseString(propVals.comments).trim();
			var correctiveAction =  tryParseString(propVals.correctiveaction).trim();
			var isCompliant =  isTrue(propVals.iscompliant);

			var dateAnswered =  tryParseString(propVals.dateanswered.date).trim();
			var dateCorrected =  tryParseString(propVals.datecorrected.date).trim();
			var dateAnsweredFormat =  ServerDateFormatToJQuery(propVals.dateanswered.dateformat);
			var dateCorrectedFormat =  ServerDateFormatToJQuery(propVals.datecorrected.dateformat);

            if(o.ReadOnly)
            {
                $Div.append('Answer: ' + answer);
                if(dateAnswered !== '')
				{
					$Div.append(' ('+ dateAnswered +')');
				}
				$Div.append('<br/>');
                $Div.append('Corrective Action: ' + correctiveAction);
                if(dateCorrected !== '')
				{
					$Div.append(' ('+ dateCorrected +')');
				}
				$Div.append('<br/>');
                $Div.append('Comments: ' + comments + '<br/>');
            }
            else 
            {
				var $table = $Div.CswTable('init', { 
													'ID': o.ID + '_tbl', 
													'FirstCellRightAlign': true 
													});

				$table.CswTable('cell', 1, 1).append('Answer');
				var splitAnswers = allowedAnswers.split(',');
				var $AnswerSel = $('<select id="'+ o.ID +'_ans" />')
									.appendTo($table.CswTable('cell', 1, 2))
									.change(function() { 
										checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
										o.onchange();
									});
				$('<option value=""></option>').appendTo($AnswerSel);
                var $thisOpt;
				for(var i = 0; i < splitAnswers.length; i++)
				{
					var thisAnswer = splitAnswers[i];
					$thisOpt = $('<option value="'+ thisAnswer +'">'+ thisAnswer + '</option>').appendTo($AnswerSel);
					if (thisAnswer === answer) {
						$thisOpt.CswAttrDom('selected', 'true');
					}
				}

				var $CorrectiveActionLabel = $table.CswTable('cell', 2, 1).append('Corrective Action');
				var $CorrectiveActionTextBox = $('<textarea id="'+ o.ID +'_cor" />')
									.appendTo($table.CswTable('cell', 2, 2))
									.text(correctiveAction)
									.change(function() { 
										checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
										o.onchange();
									});

				$table.CswTable('cell', 3, 1).append('Comments');
				var $CommentsTextBox = $('<textarea id="'+ o.ID +'_com" />')
									.appendTo($table.CswTable('cell', 3, 2))
									.text(comments)
									.change(o.onchange);

				checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
            }
        },
        save: function(o) {
            var answer = o.$propdiv.find('#' + o.ID + '_ans').val();
            var correctiveAction = o.$propdiv.find('#' + o.ID + '_cor').val();
            var comments = o.$propdiv.find('#' + o.ID + '_com').val();
            var propVals = o.propData.values;    
			propVals.answer = answer;
			propVals.correctiveaction = correctiveAction;
			propVals.comments = comments;
        }
    };
    
	function checkCompliance(compliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox)
	{
		var splitCompliantAnswers = compliantAnswers.split(',');
		var isCompliant = true;
		var selectedAnswer = $AnswerSel.val();
		var correctiveAction = $CorrectiveActionTextBox.val();

		if(selectedAnswer !== '' && correctiveAction === '')
		{
			isCompliant = false;
			for(var i = 0; i < splitCompliantAnswers.length; i++)
			{
				if(splitCompliantAnswers[i] === selectedAnswer)
				{
					isCompliant = true;
				}
			}
		}
		if(isCompliant)
		{
		    $AnswerSel.removeClass('CswFieldTypeQuestion_OOC');
			if(correctiveAction === '')
			{
				$CorrectiveActionLabel.hide();
				$CorrectiveActionTextBox.hide();
			}
		} else {
		    $AnswerSel.addClass('CswFieldTypeQuestion_OOC');
			$CorrectiveActionLabel.show();
			$CorrectiveActionTextBox.show();
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
