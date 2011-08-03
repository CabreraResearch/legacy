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

//#region CswMobilePageProps

function CswMobilePageProps(propsDef,$parent,mobileStorage) {
	/// <summary>
	///   Props Page class. Responsible for generating a Mobile props page.
	/// </summary>
    /// <param name="propsDef" type="Object">Props definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageProps">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content = '';
    var pageDef = { };
    var id = CswMobilePage_Type.props.id;
    var title = CswMobilePage_Type.props.title;
    
    //ctor
    (function(){
    
        if(isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }
        
        var p = {
	        level: 3,
	        DivId: '', 
	        title: '',
	        theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
	        onHelpClick: null, //function () {},
            onOnlineClick: null, //function () {},
            onRefreshClick: null, //function () {},
            onSearchClick: null //function () {}
        };
        if(propsDef) $.extend(p, propsDef);

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
            p.footerDef.buttons.refresh = makeFooterButtonDef(CswMobileFooterButtons.refresh, id, p.onRefreshClick);
            p.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
            p.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, p.onHelpClick);
        }

        if( isNullOrEmpty(p.headerDef.buttons)) {
            p.headerDef.buttons.back = makeHeaderButtonDef(CswMobileHeaderButtons.back, id);
            p.headerDef.buttons.search = makeHeaderButtonDef(CswMobileHeaderButtons.search, id, p.onSearchClick);
        }
        
        pageDef = p;
        $content = getContent();
    })(); //ctor
    
    function getContent() {
        var $props;
        return $props;
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

//#endregion CswMobilePageProps