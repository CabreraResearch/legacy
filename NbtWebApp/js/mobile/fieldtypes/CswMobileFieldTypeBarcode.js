/// <reference path="../../_Global.js" />
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

    var $content, contentDivId, propId, propName, subfields;
    
    //ctor
    (function () {
        var p = { 
            propid: '',
            propname: ''
        };
        if (ftDef) $.extend(p, ftDef);

        $content = $('');
        contentDivId = p.nodekey;
    })(); //ctor

	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Barcode;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeBarcode