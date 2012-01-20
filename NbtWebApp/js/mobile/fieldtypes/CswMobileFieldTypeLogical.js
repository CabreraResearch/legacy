/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeLogical

function CswMobileFieldTypeLogical(ftDef) {
    /// <summary>
    ///   Logical field type. Responsible for generating prop according to Field Type rules.
    /// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
    /// <returns type="CswMobileFieldTypeLogical">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var divSuffix = '_propdiv',
        propSuffix = '_input',
        $content, contentDivId, elementId, propId, propName, subfields, value, gestalt;
    
    //ctor
    (function () {
        var p = { 
                propId: '',
                propName: '',
                gestalt: '',
                value: '',
                checked: 'false',
                required: false
            };
        if (ftDef) $.extend(p, ftDef);
        var propVals = p.values,
            suffix = 'ans',
            answers = ['True', 'False'],
            answertext = '',
            $fieldset, i, inputName, inputId, $input;

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;

        subfields = CswSubFields_Map.Logical.subfields;
        value = tryParseString(propVals[subfields.Checked.name]);
        gestalt = tryParseString(p.gestalt, '');
        
        $content = ensureContent($content, contentDivId);
        contentDivId = p.nodekey;
        
        $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
                            .CswAttrDom({
                                'class': CswMobileCssClasses.fieldset.name,
                                'id': propId + divSuffix
                            })
                            .CswAttrNonDom({
                                'data-role': 'controlgroup',
                                'data-type': 'horizontal'
                            })
                            .appendTo($content);
                                     
        if (false === isTrue(p.required)) {
            answers.push = 'Null';
        }
        inputName = makeSafeId({ prefix: propId, ID: suffix }); //Name needs to be non-unique and shared

        for (i = 0; i < answers.length; i++) {
            if (contains(answers, i)) {
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

                inputId = makeSafeId({ prefix: propId, ID: suffix, suffix: answers[i] });

                $fieldset.append('<label for="' + inputId + '">' + answertext + '</label>');
                $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] });

                // Checked is a Tristate, so isTrue() is not useful here
                if ((value === 'false' && answers[i] === 'False') ||
                    (value === 'true' && answers[i] === 'True') ||
                        (value === '' && answers[i] === 'Null')) {
                    $input.CswAttrDom({ 'checked': 'checked' });
                }
            } // if (answers.hasOwnProperty(i))
        } // for (var i = 0; i < answers.length; i++)
        
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }

    function updatePropValue(json,id,newValue) {
        json = modifyPropJson(json, subfields.Checked.name, newValue);
        return json;
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.updatePropValue = updatePropValue;
    this.value = value;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswSubFields_Map.Logical;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeLogical