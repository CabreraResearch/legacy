/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../../thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
    var PluginName = "CswMobileQuestion";
    
    var methods = {
	
        'init': function(options) 
		{
            var o = {
                'ID': '',
				'name': '',
                'placeholder': '',
                'cssclass': '',
                ParentId: '', 
                Options: '', 
                Answer: '', 
                CompliantAnswers: '',
                'Suffix': 'ans',
                'onChange': function() {}
            };
            if (options) $.extend(o, options);

			o.name = tryParseString(o.name,o.ID);
            o.ID = tryParseString(o.ID,o.name);

            var $parent = $(this);
            var $fieldcontain = $('<div class="csw_fieldset" data-role="fieldcontain"></div>');
            var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
    								    .appendTo($fieldcontain)
    								    .CswAttrDom({
								        'id': o.ID + '_fieldset'
								    })
    								.CswAttrXml({
								        'data-role': 'controlgroup',
								        'data-type': 'horizontal'
								    });
            var answers = o.Options.split(',');
            var answerName = makeSafeId({ prefix: o.ID, ID: o.Sufix }); //Name needs to be non-unqiue and shared

            for (var i = 0; i < answers.length; i++) {
                var answerid = makeSafeId({ prefix: o.ID, ID: o.Suffix, suffix: answers[i] });
                var $answer = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: answerName, ID: answerid, value: answers[i] });
                var $label = $('<label for="' + answerid + '">' + answers[i] + '</label>')
    								    .appendTo($fieldset);
                if (o.Answer === answers[i]) {
                    $answer.CswAttrDom('checked', 'checked');
                }
                //$retHtml.data('thisI', i);
            } // for (var i = 0; i < answers.length; i++)
            
            $parent.unbind('click');
            $parent.bind('click', function(eventObj) {
                var thisAnswer = eventObj.srcElement.innerText;
                var correctiveActionId = makeSafeId({ prefix: o.ID, ID: 'cor' });
                var liSuffixId = makeSafeId({ prefix: o.ID, ID: 'label' });

                var $cor = $('#' + correctiveActionId);
                var $li = $('#' + liSuffixId);
  
                if ((',' + p.CompliantAnswers + ',').indexOf(',' + thisAnswer + ',') >= 0) {
                    $cor.css('display', 'none');
                    $li.removeClass('OOC');

                } else {
                    $cor.css('display', '');

                    if (isNullOrEmpty($cor.val())) {
                        $li.addClass('OOC');
                    } else {
                        $li.removeClass('OOC');
                    }
                }
                if (!isNullOrEmpty(o.Answer)) {
                    // update unanswered count when this question is answered
                    var $fieldset = $('#' + IdStr + '_fieldset');
                    if ($fieldset.CswAttrDom('answered')) {
                        var $cntspan = $('#' + ParentId + '_unansweredcnt');
                        $cntspan.text(parseInt($cntspan.text()) - 1);
                        $fieldset.CswAttrDom('answered', 'true');
                    }
                }
                o.onChange(ParentId, eventObj);
            }); //click()

            return $fieldcontain;
        }

    };
    	// Method calling logic
	$.fn.CswMobileQuestion = function (method) { /// <param name="$" type="jQuery" />
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
    };


})(jQuery);
