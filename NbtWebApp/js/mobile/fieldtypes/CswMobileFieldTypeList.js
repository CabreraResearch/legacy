/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
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
            options: ''
        };
        if (ftDef) $.extend(p, ftDef);

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        
        var propVals = p.values;
        subfields = CswSubFields_Map.List.subfields;
        value = tryParseString(propVals[subfields.Value.name]);
        var optionsstr = propVals.options;
        gestalt = tryParseString(p.gestalt, '');
        
        var values = [];
        var options = optionsstr.split(',');
		for (var i = 0; i < options.length; i++) {
			if (options.hasOwnProperty(i)) {
			    var optval = options[i];
			    var optdis = tryParseString(optval, '[blank]');
			    values.push({ value: optval, display: optdis });
			}
		}
        
        $content = ensureContent(contentDivId);
        $content.CswSelect('init', {
                                ID: elementId,
                                selected: value,
                                values: values,
                                cssclass: CswMobileCssClasses.select.name
                            });
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