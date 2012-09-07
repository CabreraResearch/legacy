/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeMemo

function CswMobileFieldTypeMemo(ftDef) {
    /// <summary>
    ///   Memo field type. Responsible for generating prop according to Field Type rules.
    /// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
    /// <returns type="CswMobileFieldTypeMemo">Instance of itself. Must instance with 'new' keyword.</returns>

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
                text: ''
            };
        if (ftDef) $.extend(p, ftDef);
        var propVals = p.values;

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;

        subfields = CswSubFields_Map.Memo.subfields;        
        value = tryParseString(propVals[subfields.Text.name]);
        gestalt = tryParseString(p.gestalt, '');
        
        $content = ensureContent($content, contentDivId);
        $content.append($('<textarea name="' + elementId + '">' + value + '</textarea>'));
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }

    function updatePropValue(json,id,newValue) {
        json = modifyPropJson(json, subfields.Text.name, newValue);
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
    this.fieldType = CswSubFields_Map.Memo;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeMemo