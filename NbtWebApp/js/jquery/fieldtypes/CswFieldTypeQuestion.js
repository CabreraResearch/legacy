; (function ($) {
        
    var PluginName = 'CswFieldTypeQuestion';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

                var $Div = $(this);
                $Div.contents().remove();

                var Answer = o.$propxml.children('answer').text().trim();
                var AllowedAnswers = o.$propxml.children('allowedanswers').text().trim();
				var CompliantAnswers = o.$propxml.children('compliantanswers').text().trim();
				var Comments =  o.$propxml.children('comments').text().trim();
				var CorrectiveAction =  o.$propxml.children('correctiveaction').text().trim();
				var DateAnswered =  o.$propxml.children('dateanswered').text().trim();
				var DateCorrected =  o.$propxml.children('datecorrected').text().trim();
				var IsCompliant =  o.$propxml.children('iscompliant').text().trim();

                if(o.ReadOnly)
                {
                    $Div.append('Answer: ' + Answer);
                    if(DateAnswered !== '')
					{
						$Div.append(' ('+ DateAnswered +')');
					}
					$Div.append('<br/>');
                    $Div.append('Corrective Action: ' + CorrectiveAction);
                    if(DateCorrected !== '')
					{
						$Div.append(' ('+ DateCorrected +')');
					}
					$Div.append('<br/>');
                    $Div.append('Comments: ' + Comments + '<br/>');
                }
                else 
                {
					var $table = $Div.CswTable('init', { 
														'ID': o.ID + '_tbl', 
														'FirstCellRightAlign': true 
														});

					$table.CswTable('cell', 1, 1).append('Answer');
					var splitAnswers = AllowedAnswers.split(',');
					var $AnswerSel = $('<select id="'+ o.ID +'_ans" />')
										.appendTo($table.CswTable('cell', 1, 2))
										.change(function() { 
											checkCompliance(CompliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
											o.onchange();
										});
					var $thisOpt = $('<option value=""></option>').appendTo($AnswerSel);
					for(var i = 0; i < splitAnswers.length; i++)
					{
						var thisAnswer = splitAnswers[i];
						var $thisOpt = $('<option value="'+ thisAnswer +'">'+ thisAnswer + '</option>').appendTo($AnswerSel);
						if(thisAnswer === Answer)
							$thisOpt.CswAttrDom('selected', 'true');
					}

					var $CorrectiveActionLabel = $table.CswTable('cell', 2, 1).append('Corrective Action');
					var $CorrectiveActionTextBox = $('<textarea id="'+ o.ID +'_cor" />')
										.appendTo($table.CswTable('cell', 2, 2))
										.text(CorrectiveAction)
										.change(function() { 
											checkCompliance(CompliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
											o.onchange();
										});

					$table.CswTable('cell', 3, 1).append('Comments');
					var $CommentsTextBox = $('<textarea id="'+ o.ID +'_com" />')
										.appendTo($table.CswTable('cell', 3, 2))
										.text(Comments)
										.change(o.onchange);

					checkCompliance(CompliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox);
                }
            },
        save: function(o) {
                var Answer = o.$propdiv.find('#' + o.ID + '_ans').val();
                var CorrectiveAction = o.$propdiv.find('#' + o.ID + '_cor').val();
                var Comments = o.$propdiv.find('#' + o.ID + '_com').val();
                
				o.$propxml.children('answer').text(Answer);
				o.$propxml.children('correctiveaction').text(CorrectiveAction);
				o.$propxml.children('comments').text(Comments);
            }
    };
    
	function checkCompliance(CompliantAnswers, $AnswerSel, $CorrectiveActionLabel, $CorrectiveActionTextBox)
	{
		var splitCompliantAnswers = CompliantAnswers.split(',');
		var isCompliant = true;
		var SelectedAnswer = $AnswerSel.val();
		var CorrectiveAction = $CorrectiveActionTextBox.val();

		if(SelectedAnswer !== '' && CorrectiveAction === '')
		{
			isCompliant = false;
			for(var i = 0; i < splitCompliantAnswers.length; i++)
			{
				if(splitCompliantAnswers[i] === SelectedAnswer)
				{
					isCompliant = true;
				}
			}
		}
		if(isCompliant)
		{
		    $AnswerSel.removeClass('CswFieldTypeQuestion_OOC');
			if(CorrectiveAction === '')
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
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
