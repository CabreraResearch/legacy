/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobileInspectionDesignClass

function CswMobileInspectionDesignClass(ocDef, $div, mobileStorage) {
	/// <summary>
	///   Inspection Design. Responsible for generating nodes according to Object Class rules.
	/// </summary>
    /// <param name="ocDef" type="Object">Object Class definitional data.</param>
	/// <param name="$div" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobileInspectionDesignClass">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content;
    
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    //ctor
    (function () {
        
        var p = {
        };
        if (ocDef) $.extend(p, ocDef);

        $content = '';
    })(); //ctor

    function getContent() {
        
    }
   
	//#endregion private
    
    //#region public, priveleged

    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobileInspectionDesignClass