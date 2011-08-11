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
	///   Offline Page class. Responsible for generating a Mobile offline page.
	/// </summary>
    /// <param name="offlineDef" type="Object">Offline definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageOffline">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content = '';
    var pageDef = { };
    var id = CswMobilePage_Type.offline.id;
    var title = CswMobilePage_Type.offline.title;
    var divSuffix = '_offline';
    var contentDivId;
    
    //ctor
    (function(){
    
        if(isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }
        
        var p = {
	        level: -1,
	        DivId: '', 
	        title: '',
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

        contentDivId = id + divSuffix;
        
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }

        var buttons = { };
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;

        pageDef = p = makeMenuButtonDef(p, id, buttons, mobileStorage);

        getContent();
    })();
    
    function getContent() {
        $content = ensureContent($content, contentDivId);
        $content.append($('<p>You must have internet connectivity to login.</p>'));
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.contentDivId = contentDivId;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageOffline