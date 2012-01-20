/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../main/controls/CswSelect.js" />

//#region CswMobileFieldTypeList

function CswMobileFieldTypeList(ftDef) {
    /// <summary>
    ///   List field type. Responsible for generating prop according to Field Type rules.
    /// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
    /// <returns type="CswMobileFieldTypeList">Instance of itself. Must instance with 'new' keyword.</returns>

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
            options: ''
        };
        if (ftDef) $.extend(p, ftDef);
        var propVals = p.values,
            values = [],
            options = propVals.options.split(','),
            i, optval, optdis;

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        
        subfields = CswSubFields_Map.List.subfields;
        value = tryParseString(propVals[subfields.Value.name]);
        gestalt = tryParseString(p.gestalt, '');
        
        for (i = 0; i < options.length; i++) {
            if (contains(options, i)) {
                optval = options[i];
                optdis = tryParseString(optval, '[blank]');
                values.push({ value: optval, display: optdis });
            }
        }
        
        $content = ensureContent($content, contentDivId);
        $content.CswSelect('init', {
                                ID: elementId,
                                selected: value,
                                values: values,
                                cssclass: CswMobileCssClasses.select.name
                            })
                .CswAttrNonDom({ 'data-native-menu': 'false' });
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }

    function updatePropValue(json,id,newValue) {
        json = modifyPropJson(json, subfields.Value.name, newValue);
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
    this.fieldType = CswSubFields_Map.List;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeList