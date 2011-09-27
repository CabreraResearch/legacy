/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeQuestion

function CswMobileFieldTypeQuestion(ftDef) {
    /// <summary>
    ///   Question field type. Responsible for generating prop according to Field Type rules.
    /// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
    /// <returns type="CswMobileFieldTypeQuestion">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var divSuffix = '_propdiv',
        propSuffix = '_input',
        $content, contentDivId, elementId, propId, propName, subfields, value, gestalt, compliantAnswers;
    
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
        subfields = CswSubFields_Map.Question.subfields;
        var propVals = p.values,
            answer = tryParseString(propVals.answer),
            allowedAnswers = tryParseString(propVals.allowedanswers).split(','),
            comments = tryParseString(propVals[subfields.Comments.name]),
            correctiveAction = tryParseString(propVals[subfields.CorrectiveAction.name]),
            suffix = 'ans',
            $fieldset, answerName, i, answerid, $answer, isCompliant, $prop;
        
        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        
        
        
        value = tryParseString(propVals[subfields.Answer.name]);
        gestalt = tryParseString(p.gestalt, '');

        compliantAnswers = tryParseString(propVals.compliantanswers).split(',');

        $content = ensureContent($content, contentDivId);

        $fieldset = $('<fieldset class="' + CswMobileCssClasses.fieldset.name + '"></fieldset>')
                                .CswAttrDom({
                                    'id': propId + '_fieldset'
                                })
                                .CswAttrXml({
                                    'data-role': 'controlgroup',
                                    'data-type': 'horizontal',
                                    'data-theme': 'b'
                                })
                                .appendTo($content);

        answerName = makeSafeId({ prefix: propId, ID: suffix }); //Name needs to be non-unqiue and shared
        subfields.answer = answerName;
        
        for (i = 0; i < allowedAnswers.length; i++) {
            if (allowedAnswers.hasOwnProperty(i)) {
                answerid = makeSafeId({ prefix: propId, ID: suffix, suffix: allowedAnswers[i] });

                $fieldset.append('<label for="' + answerid + '" id="' + answerid + '_lab">' + allowedAnswers[i] + '</label');
                $answer = $('<input type="radio" name="' + answerName + '" id="' + answerid + '" class="' + CswMobileCssClasses.answer.name + '" value="' + allowedAnswers[i] + '" />')
                    .appendTo($fieldset);

                if (answer === allowedAnswers[i]) {
                    $answer.CswAttrXml('checked', 'checked');
                }
            }
        }
        isCompliant = inCompliance(answer,correctiveAction);
        $prop = $('<div data-role="collapsible" class="' + CswMobileCssClasses.collapsible.name + '" data-collapsed="' + isCompliant + '"><h3>Comments</h3></div>')
            .appendTo($content);

        $('<textarea id="' + propId + '_cor" name="' + propId + '_cor" placeholder="Corrective Action">' + correctiveAction + '</textarea>')
            .appendTo($prop);
        subfields.correctiveaction = propId + '_cor';

        $('<textarea name="' + propId + '_com" id="' + propId + '_com" placeholder="Comments">' + comments + '</textarea>')
            .appendTo($prop);
        subfields.comments = propId + '_com';
    })(); //ctor
    
    function inCompliance(currentAnswer,correctiveAction) {
        var ret = true;
        if (isNullOrEmpty(currentAnswer)) {
            currentAnswer = $content.find('input:checked').val();
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
        if (!isNullOrEmpty($control)) {
            if (inCompliance()) {
                $control.find('#' + propId + '_cor').css('display','none').hide();
                $control.find('h2').removeClass(CswMobileCssClasses.OOC.name);
            } else {
                $control.find('#' + propId + '_cor').css('display','').show();
                $control.find('h2').addClass(CswMobileCssClasses.OOC.name);
            }
        }
        //fixGeometry();
    }
    
    function updatePropValue(json,id,newValue) {
        var subFieldToUpdate = '';
        if (id.contains(makeSafeId({ ID: propId, suffix: 'com' }))) {
            subFieldToUpdate = subfields.Comments.name;
        } 
        else if (id.contains(makeSafeId({ ID: propId, suffix: 'ans' }))) {
            subFieldToUpdate = subfields.Answer.name;
        } 
        else if (id.contains(makeSafeId({ ID: propId, suffix: 'cor' }))) {
            subFieldToUpdate = subfields.CorrectiveAction.name;
        }

        json = modifyPropJson(json, subFieldToUpdate, newValue);
        return json;
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.updatePropValue = updatePropValue;
    this.value = value;
    this.gestalt = gestalt;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswSubFields_Map.Question;

    this.inCompliance = inCompliance;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeQuestion