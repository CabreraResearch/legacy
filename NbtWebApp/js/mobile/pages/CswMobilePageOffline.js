/// <reference path="../../_Global.js" />
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
	///   Help Page class. Responsible for generating a Mobile help page.
	/// </summary>
    /// <param name="helpDef" type="Object">Help definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageOffline">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    if(isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }

    var $offline = $('<p>You must have internet connectivity to login.</p>');

    var p = {
	    level: -1,
	    DivId: 'sorrycharliediv',       // required
	    HeaderText: 'Sorry Charlie!',
	    theme: 'b',
	    $content: $offline,
	    onHelpClick: function () {},
        onOnlineClick: function () {}
    };
    if(offlineDef) $.extend(p, offlineDef);

    var pageDef = p;
    delete pageDef.onRefreshClick;
    delete pageDef.onHelpClick;

    if( isNullOrEmpty(pageDef.footerDef)) {
        pageDef.footerDef = { };
        pageDef.footerDef.buttons = { };
        pageDef.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, p.DivId, null, mobileStorage);
        pageDef.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, p.DivId);
        pageDef.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, p.DivId, p.onHelpClick);
    }

	var offlinePage = new CswMobilePageFactory(pageDef, $parent);
	var offlineHeader = offlinePage.mobileHeader;
	var offlineFooter = offlinePage.mobileFooter;
	var $content = offlinePage.$content;
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.mobileHeader = offlineHeader;
    this.mobileFooter = offlineFooter;
    this.$pageDiv = offlinePage.$pageDiv;

    this.onPageOpen = function() {
        this.$pageDiv.CswChangePage({ transition: 'slideup' });
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageOffline