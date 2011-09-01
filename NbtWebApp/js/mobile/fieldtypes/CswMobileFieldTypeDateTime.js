/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileFieldTypeDate

function CswMobileFieldTypeDate(ftDef) {
	/// <summary>
	///   Date field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeDate">Instance of itself. Must instance with 'new' keyword.</returns>

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
        
        var propVals = p.values;
        subfields = CswSubFields_Map.Date.subfields;
        
        var date = tryParseString(propVals.value.date).trim();
        var time = tryParseString(propVals.value.time).trim();
        var dateFormat = ServerDateFormatToJQuery(propVals.value.dateformat);
        var timeFormat = ServerTimeFormatToJQuery(propVals.value.timeformat);
        var displayMode = propVals.displaymode;

        $content = ensureContent(contentDivId);
        gestalt = tryParseString(p.gestalt, '');
        
        value = '';
        switch (displayMode.toLowerCase()) {
            case subfields.DisplayMode.DateTime.name.toLowerCase():
                value = date + ' ' + time;
                $content.CswInput('init', { type: CswInput_Types.date, ID: elementId, value: date });
                $content.CswInput('init', { type: CswInput_Types.time, ID: elementId, value: time });
                break;
            case subfields.DisplayMode.Date.name.toLowerCase():
                value = date;
                $content.CswInput('init', { type: CswInput_Types.date, ID: elementId, value: value });
                break;
            case subfields.DisplayMode.Time.name.toLowerCase():
                value = time;
                $content.CswInput('init', { type: CswInput_Types.date, ID: elementId, value: value });                
                break;
            default :
                $content.append($('<p style="white-space:normal;" id="' + elementId + '">' + gestalt + '</p>'));
                break;
        }
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
    this.gestalt = gestalt;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswSubFields_Map.Date;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeDate