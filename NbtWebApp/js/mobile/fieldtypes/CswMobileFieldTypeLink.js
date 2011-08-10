/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeLink

function CswMobileFieldTypeLink(ftDef) {
	/// <summary>
	///   Link field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeLink">Instance of itself. Must instance with 'new' keyword.</returns>

 	//#region private

    var divSuffix = '_propdiv';
    var propSuffix = '_input';
    var $content, contentDivId, elementId, propId, propName, subfields, value, href, gestalt;
    
    //ctor
    (function () {
        var p = { 
            propId: '',
            propName: '',
            gestalt: '',
            text: '',
            href: ''
        };
        if (ftDef) $.extend(p, ftDef);

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;

        subfields = CswFieldTypes.Link.subfields;
        href = tryParseString(subfields.Href.subfield.name);
        value = tryParseString(subfields.Text.subfield.name);
        gestalt = tryParseString(p.gestalt, '');
        
        $content = ensureContent(contentDivId);
        $content.CswLink('init', { ID: elementId, href: p.href, rel: 'external', value: value });
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
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Link;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeLink