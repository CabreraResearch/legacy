﻿/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeBarcode

function CswMobileFieldTypeBarcode(ftDef) {
	/// <summary>
	///   Barcode field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeBarcode">Instance of itself. Must instance with 'new' keyword.</returns>

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
            value: ''
        };
        if (ftDef) $.extend(p, ftDef);

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;

        subfields = CswFieldTypes.Barcode.subfields;
        
        value = tryParseString(p[subfields.Barcode.subfield.name]);
        gestalt = tryParseString(p.gestalt);
        
        $content = ensureContent(contentDivId);
        $content.CswInput('init', { type: CswInput_Types.text, ID: elementId, value: value });
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }
    
    function updatePropValue(json,id,newValue) {
        if (json.hasOwnProperty(subfields.Barcode.subfield.name)) {
            json[subfields.Barcode.subfield.name] = newValue;
            json.wasmodified = true;
        }
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
    this.fieldType = CswFieldTypes.Barcode;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeBarcode