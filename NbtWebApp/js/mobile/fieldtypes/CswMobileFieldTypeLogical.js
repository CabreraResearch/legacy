/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeLogical

function CswMobileFieldTypeLogical(ftDef) {
	/// <summary>
	///   Logical field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeLogical">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var divSuffix = '_propdiv';
    var propSuffix = '_input';
    var $content, contentDivId, elementId, propId, propName, subfields, value, gestalt;
    
    //ctor
    (function () {
        var p = { 
            propid: '',
            propname: '',
            gestalt: '',
            value: '',
            checked: 'false',
            required: false
        };
        if (ftDef) $.extend(p, ftDef);

        contentDivId = p.nodekey + divSuffix;
        elementId = p.propId + propSuffix;
        value = tryParseString(p.checked);
        gestalt = tryParseString(p.gestalt, '');
        propId = p.propid;
        propName = p.propname;
        subfields = '';
        
        $content = ensureContent(contentDivId);
        contentDivId = p.nodekey;
        
        var suffix = 'ans';
		var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
							.CswAttrDom({
								'class': CswMobileCssClasses.fieldset.name,
								'id': id + divSuffix
							})
							.CswAttrXml({
								'data-role': 'controlgroup',
								'data-type': 'horizontal'
							})
		                    .appendTo($content);
									 
		var answers = ['True', 'False'];
		if (!isTrue(p.required)) {
			answers.push = 'Null';
		}
		var inputName = makeSafeId({ prefix: propId, ID: suffix }); //Name needs to be non-unique and shared

		for (var i = 0; i < answers.length; i++) {
			if (answers.hasOwnProperty(i)) {
			    var answertext = '';
			    switch (answers[i]) {
			        case 'Null':
			            answertext = '?';
			            break;
			        case 'True':
			            answertext = 'Yes';
			            break;
			        case 'False':
			            answertext = 'No';
			            break;
			    }

			    var inputId = makeSafeId({ prefix: id, ID: suffix, suffix: answers[i] });

			    $fieldset.append('<label for="' + inputId + '">' + answertext + '</label>');
			    var $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] });

			    // Checked is a Tristate, so isTrue() is not useful here
			    if ((value === 'false' && answers[i] === 'False') ||
    			    (value === 'true' && answers[i] === 'True') ||
        			    (value === '' && answers[i] === 'Null')) {
			        $input.CswAttrXml({ 'checked': 'checked' });
			    }
			} // if (answers.hasOwnProperty(i))
		} // for (var i = 0; i < answers.length; i++)
        
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.value = value;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Logical;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeLogical