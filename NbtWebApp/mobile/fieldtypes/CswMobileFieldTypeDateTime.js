/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeDate

function CswMobileFieldTypeDate(ftDef) {
    "use strict";
    /// <summary>
    ///   Date field type. Responsible for generating prop according to Field Type rules.
    /// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
    /// <returns type="CswMobileFieldTypeDate">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var divSuffix = '_propdiv',
        propSuffix = '_input',
        value = '',
        $content, contentDivId, elementId, propId, propName, subfields, gestalt;
    
    //ctor
    (function () {
        var p = { 
                propId: '',
                propName: '',
                gestalt: '',
                value: ''
            };
        if (ftDef) $.extend(p, ftDef);
        var propVals = p.values,
            date = tryParseString(propVals.value.date).trim(),
            time = tryParseString(propVals.value.time).trim(),
            dateFormat = ServerDateFormatToJQuery(propVals.value.dateformat),
            timeFormat = ServerTimeFormatToJQuery(propVals.value.timeformat),
            displayMode = propVals.displaymode;

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        
        subfields = CswSubFields_Map.DateTime.subfields;
        
        $content = ensureContent($content, contentDivId);
        gestalt = tryParseString(p.gestalt, '');

        switch (displayMode.toLowerCase()) {
            case subfields.DisplayMode.DateTime.name.toLowerCase():
                value = date + ' ' + time;
                $content.CswInput('init', { type: CswInput_Types.date, ID: elementId, value: date })
                        .data('type', 'Date');
                $content.CswInput('init', { type: CswInput_Types.time, ID: elementId, value: time })
                        .data('type', 'Time'); 
                break;
            case subfields.DisplayMode.Date.name.toLowerCase():
                value = date;
                $content.CswInput('init', { type: CswInput_Types.date, ID: elementId, value: value })
                        .data('type', 'Date'); 
                break;
            case subfields.DisplayMode.Time.name.toLowerCase():
                value = time;
                $content.CswInput('init', { type: CswInput_Types.time, ID: elementId, value: value })
                        .data('type', 'Time');                 
                break;
            default :
                $content.append($('<p style="white-space:normal;" id="' + elementId + '">' + gestalt + '</p>'));
                break;
        }
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }
    
    function updatePropValue(json,id,newValue) {
        var $elem = $('#' + id.toString());
        var type = $elem.data('type');
        json = modifyPropJson(json, subfields.Value[type].name, newValue);
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
    this.fieldType = CswSubFields_Map.Date;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeDate