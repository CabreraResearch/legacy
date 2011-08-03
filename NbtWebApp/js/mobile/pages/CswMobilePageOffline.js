﻿/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />

//#region CswMobilePageOffline

function CswMobilePageOffline(offlineDef,$parent,mobileStorage) {
	/// <summary>
	///   Offline Page class. Responsible for generating a Mobile offline page.
	/// </summary>
    /// <param name="offlineDef" type="Object">Offline definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageOffline">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content = '';
    var pageDef = { };
    var id = 'offlinediv';
    var title = 'Sorry Charlie!';
    
    //ctor
    (function(){
    
        if(isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }
        
        var p = {
	        level: -1,
	        DivId: 'sorrycharliediv',       // required
	        title: 'Sorry Charlie!',
	        theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
	        onHelpClick: function () {},
            onOnlineClick: function () {}
        };
        if(offlineDef) $.extend(p, offlineDef);

        if(!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }

        if( isNullOrEmpty(p.footerDef.buttons)) {
            p.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, id, null, mobileStorage);
            p.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
            p.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, p.onHelpClick);
        }
        
        pageDef = p;
        
        $content = getContent();
    })();
    
    function getContent() {
        var $offline = $('<p>You must have internet connectivity to login.</p>');
        return $offline;
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageOffline