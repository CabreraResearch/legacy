/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobileGenericClass

function CswMobileGenericClass(ocDef) {
	/// <summary>
	///   Generic. Responsible for generating nodes according to Object Class rules.
	/// </summary>
    /// <param name="ocDef" type="Object">Object Class definitional data.</param>
	/// <returns type="CswMobileGenericClass">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content, contentDivId;
    
    //ctor
    (function () {
        
        var p = { 
            nodekey: '',
            'node_name': ''
        };
        if (ocDef) $.extend(p, ocDef);

        $content = $('<h2 id="' + p.nodekey + '">' + p['node_name'] + '</h2>');
        contentDivId = p.nodekey;
    })(); //ctor

	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.contentDivId = contentDivId;
    //#endregion public, priveleged
}

//#endregion CswMobileGenericClass