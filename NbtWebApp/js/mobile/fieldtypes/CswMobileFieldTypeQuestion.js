/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeQuestion

function CswMobileFieldTypeQuestion(ftDef) {
	/// <summary>
	///   Question field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeQuestion">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var divSuffix = '_propdiv';
    var propSuffix = '_input';
    var $content, contentDivId, elementId, propId, propName, subfields, value, gestalt, compliantAnswers;
    
    //ctor
    (function () {
        var p = { 
            propId: '',
            propName: '',
            gestalt: '',
            answer: '',
            allowedanswers: '',
            compliantanswers: '',
            correctiveaction: '',
            comments: ''
        };
        if (ftDef) $.extend(p, ftDef);

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        value = tryParseString(p.value);
        gestalt = tryParseString(p.gestalt, '');
        subfields = {};

        var answer = tryParseString(p.answer);
        var allowedAnswers = tryParseString(p.allowedanswers).split(',');
        compliantAnswers = tryParseString(p.compliantanswers).split(',');
        var comments = tryParseString(p.comments);
        var correctiveAction = tryParseString(p.correctiveaction);

        $content = ensureContent($content, contentDivId);

        var suffix = 'ans';
		var $fieldset = $('<fieldset class="' + CswMobileCssClasses.fieldset.name + '"></fieldset>')
								.CswAttrDom({
									'id': propId + '_fieldset'
								})
								.CswAttrXml({
									'data-role': 'controlgroup',
									'data-type': 'horizontal',
									'data-theme': 'b'
								})
		                        .appendTo($content);

		var answerName = makeSafeId({ prefix: propId, ID: suffix }); //Name needs to be non-unqiue and shared
        subfields.answer = answerName;
        
		for (var i = 0; i < allowedAnswers.length; i++) {
		    if (allowedAnswers.hasOwnProperty(i)) {
		        var answerid = makeSafeId({ prefix: propId, ID: suffix, suffix: allowedAnswers[i] });

		        $fieldset.append('<label for="' + answerid + '" id="' + answerid + '_lab">' + allowedAnswers[i] + '</label');
		        var $answer = $('<input type="radio" name="' + answerName + '" id="' + answerid + '" class="' + CswMobileCssClasses.answer.name + '" value="' + allowedAnswers[i] + '" />')
    		        .appendTo($fieldset);

		        if (answer === allowedAnswers[i]) {
		            $answer.CswAttrDom('checked', 'checked');
		        }
		    }
		}
        var isCompliant = inCompliance(answer,correctiveAction);
		var $prop = $('<div data-role="collapsible" class="' + CswMobileCssClasses.collapsible.name + '" data-collapsed="' + !isCompliant + '"><h3>Comments</h3></div>')
			.appendTo($content);

		var $corAction = $('<textarea id="' + propId + '_cor" name="' + propId + '_cor" placeholder="Corrective Action">' + correctiveAction + '</textarea>')
			.appendTo($prop);
        subfields.correctiveaction = propId + '_cor';
        
		if (isCompliant) {
			$corAction.css('display', 'none');
		}
//		$corAction.unbind('change');
//		$corAction.bind('change', function(eventObj) {
//			var $cor = $(this);
//			if ($cor.val() === '') {
//				$label.addClass('OOC');
//			} else {
//				$label.removeClass('OOC');
//			}
//		});

        $('<textarea name="' + propId + '_com" id="' + propId + '_com" placeholder="Comments">' + comments + '</textarea>')
            .appendTo($prop);
        subfields.comments = propId + '_com';
    })(); //ctor

    function makeQuestionAnswerFieldSet(answers, answer, compliantAnswers) {
//			    $answer.bind('change', function(eventObj) {

//			        var thisAnswer = $(this).val();

//			        var correctiveActionId = makeSafeId({ prefix: id, ID: 'cor' });
//			        var liSuffixId = makeSafeId({ prefix: id, ID: 'label' });

//			        var $cor = $('#' + correctiveActionId);
//			        var $li = $('#' + liSuffixId);

//			        if (compliantAnswers.indexOf(thisAnswer) >= 0) {
//			            $cor.css('display', 'none');
//			            $li.removeClass('OOC');

//			        } else {
//			            $cor.css('display', '');

//			            if (isNullOrEmpty($cor.val())) {
//			                $li.addClass('OOC');
//			            } else {
//			                $li.removeClass('OOC');
//			            }
//			        }
//			        fixGeometry();

//			        return false;
//			    });
	} // _makeQuestionAnswerFieldSet()
    
    function inCompliance(currentAnswer,correctiveAction) {
        var ret = true;
        if (isNullOrEmpty(currentAnswer)) {
            currentAnswer = $content.find('.' + CswMobileCssClasses.answer.name + ' :checked').val();
        }
        if (isNullOrEmpty(correctiveAction)) {
            correctiveAction = $content.find('#' + propId + '_cor').val();
        }
        if (!isNullOrEmpty(currentAnswer) &&
			compliantAnswers.indexOf(currentAnswer) === -1 &&
				isNullOrEmpty(correctiveAction)) {
			ret = false;
		}
        return ret;
    }
    
    function applyFieldTypeLogicToContent($control) {
        
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.value = value;
    this.gestalt = gestalt;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Question;

    this.inCompliance = inCompliance;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeQuestion