/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswSelect.js" />

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
                
        subfields = CswFieldTypes.List.subfields;
        value = tryParseString(p[subfields.Value.subfield.name]);
        var optionsstr = p.options;
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
        var $prop = $content.CswSelect('init', {
                                ID: elementId,
                                selected: value,
                                values: values,
                                cssclass: CswMobileCssClasses.select.name
                            });
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }

    function updatePropValue(json,id,newValue) {
        if (json.hasOwnProperty(subfields.Value.subfield.name)) {
            json[subfields.Value.subfield.name] = newValue;
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
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.List;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeList