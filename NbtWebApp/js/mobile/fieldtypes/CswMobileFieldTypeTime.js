/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeTime

function CswMobileFieldTypeTime(ftDef) {
	/// <summary>
	///   Time field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeTime">Instance of itself. Must instance with 'new' keyword.</returns>

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
            units: ''
        };
        if (ftDef) $.extend(p, ftDef);

        contentDivId = p.nodekey + divSuffix;
        elementId = p.propId + propSuffix;
        value = tryParseString(p.value);
        var units = tryParseString(p.units);
        if (!isNullOrEmpty(units)) {
            value += ' ' + units;
        }
        gestalt = tryParseString(p.gestalt, '');
        propId = p.propid;
        propName = p.propname;
        subfields = '';
        
        $content = ensureContent(contentDivId);
        $content.CswInput('init', { type: CswInput_Types.time, ID: elementId, value: value });
    })(); //ctor

	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.value = value;
    this.gestalt = gestalt;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Time;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeTime