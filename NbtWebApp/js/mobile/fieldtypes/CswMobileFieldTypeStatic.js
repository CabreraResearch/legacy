/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeStatic

function CswMobileFieldTypeStatic(ftDef) {
	/// <summary>
	///   Static field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeStatic">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var divSuffix = '_propdiv';
    var propSuffix = '_input';
    var $content, contentDivId, elementId, propId, propName, subfields, value, gestalt;
    
    //ctor
    (function () {
        var p = { 
            propId: '',
            propName: '',
            gestalt: '',
            value: '',
            text: ''
        };
        if (ftDef) $.extend(p, ftDef);

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        
        var propVals = p.values;
        subfields = CswSubFields_Map.Static.subfields; //don't use this yet. Non-implemented Field Types default to Static.
        value = tryParseString(propVals.value,propVals.text);
        gestalt = tryParseString(p.gestalt, '');
        
        $content = ensureContent(contentDivId);
        $content.append($('<p style="white-space:normal;" id="' + elementId + '">' + tryParseString(value,gestalt) + '</p>'));
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }
    
    function updatePropValue(json,id,newValue) {
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
    this.fieldType = CswSubFields_Map.Static;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeStatic