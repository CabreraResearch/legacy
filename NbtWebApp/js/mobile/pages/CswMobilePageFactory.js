/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="CswMobilePageViews.js" />
/// <reference path="CswMobilePageHelp.js" />
/// <reference path="CswMobilePageLogin.js" />
/// <reference path="CswMobilePageOffline.js" />
/// <reference path="CswMobilePageOnline.js" />
/// <reference path="CswMobilePageSearch.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="CswMobilePageTabs.js" />
/// <reference path="CswMobilePageProps.js" />
/// <reference path="CswMobilePageNodes.js" />

//#region CswMobilePageFactory

function CswMobilePageFactory(pageType, pageDef, $parent ) {
	/// <summary>
	///   Page factory class. Responsible for generating a Mobile page.
	/// </summary>
	/// <param name="pageDef" type="Object">JSON definition of content to display, including header/footerDef properties.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
	/// <returns type="CswMobilePageFactory">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
	var mobileHeader, mobileFooter, $content, $pageDiv, id, title, getContent;

    //ctor
    (function() {
        var p = {
            ParentId: undefined,
            level: 1,
            DivId: '',       // required
            title: '',
            $content: $(''),
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            theme: CswMobileGlobal_Config.theme,
            onHelpClick: null, //function () {}
            onOnlineClick: null, //function () {}
            onRefreshClick: null, //function () {}
            onSearchClick: null, //function () {}
            mobileStorage: null,
            mobileSync: null,
            mobileBgTask: null,
            onSuccess: null //function () {}
        };

        if (pageDef) {
            $.extend(p, pageDef);
        }

        if( isNullOrEmpty(p.mobileStorage)) {
            p.mobileStorage = new CswMobileClientDbResources();
        }
        
        if(isNullOrEmpty(p.DivId)) {
            p.DivId = pageType.name + 'div';
        }
        id = makeSafeId({ ID: p.DivId });
        
        var cswMobilePage;
        switch (pageType.name) {
            case CswMobilePage_Type.help.name:
                {
                    cswMobilePage = new CswMobilePageHelp(p, $parent, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.login.name:
                {
                    cswMobilePage = new CswMobilePageLogin(p, $parent, p.mobileStorage, p.onSuccess);
                    break;
                }
            case CswMobilePage_Type.nodes.name:
                {
                    cswMobilePage = new CswMobilePageNodes(p, $parent, mobileStorage);
                    break;
                }
            case CswMobilePage_Type.offline.name:
                {
                    cswMobilePage = new CswMobilePageOffline(p, $parent, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.online.name:
                {
                    cswMobilePage = new CswMobilePageOnline(p, $parent, p.mobileStorage, p.mobileSync, p.mobileBgTask);
                    break;
                }
            case CswMobilePage_Type.props.name:
                {
                    cswMobilePage = new CswMobilePageProps(p, $parent, mobileStorage);
                    break;
                }
            case CswMobilePage_Type.search.name:
                {
                    cswMobilePage = new CswMobilePageSearch(p, $parent, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.tabs.name:
                {
                    cswMobilePage = new CswMobilePageTabs(, p, $parent, mobileStorage);
                    break;
                }
            case CswMobilePage_Type.views.name:
                {
                    cswMobilePage = new CswMobilePageViews(p, $parent, p.mobileStorage);
                    break;
                }
            default:
                {
                    throw ('CswMobilePageFactory initialized without CswMobilePage_Type');
                }
        }
        if (cswMobilePage) {

            title = cswMobilePage.title;
            $.extend(p, cswMobilePage.pageDef);
        
	        $pageDiv = getPageDiv(title, p.theme);
            mobileHeader = getMenuHeader(p.headerDef);
            $content = getContentDiv(cswMobilePage);
            mobileFooter = getMenuFooter(p.footerDef);

            getContent = cswMobilePage.getContent;
            
            $pageDiv.page();
        }
    })(); //ctor
    
    function getPageDiv(headerText, theme) {
        var $ret = $('#' + id);

        var firstInit = (isNullOrEmpty($ret) || $ret.length === 0);
			
	    if (firstInit) {
		    $ret = $parent.CswDiv('init', { ID: id })
			                .CswAttrXml({
					                'data-role': 'page',
					                'data-url': id,
					                'data-title': headerText,
					                'data-rel': 'page',
			                        'data-theme': theme
				                });
	    }
        return $ret;
    }
    
    function getMenuHeader(options) {
        var ret;
        var headerDef = {
            dataId: 'csw_header',
            text: title
        };
        if (options) {
            $.extend(headerDef, options);
        }
        ret = new CswMobilePageHeader(headerDef, $pageDiv);
        return ret;
    }
    
    function getMenuFooter(options) {
        var ret;
        var footerDef = {
            dataId: 'csw_footer'
        };
        if (options) {
            $.extend(footerDef, options);
        }
        ret = new CswMobilePageFooter(footerDef, $pageDiv);
        return ret;
    }
    
    function getContentDiv(cswMobilePage) {
        var $ret = $pageDiv.find('div:jqmData(role="content")');

        if (!isNullOrEmpty($ret) && $ret.length > 0) {
            $ret.empty();
        } else {
            $ret = $pageDiv.CswDiv('init', { ID: id + '_content' })
                .CswAttrXml({ 'data-role': 'content' });
        }
        if (cswMobilePage.$content) {
            $ret.append(cswMobilePage.$content);
        } else {
            $ret.append(cswMobilePage.getContent());
        }
        return $ret;
    }
    
    //#endregion private	
	
	//#region public, priveleged

	this.mobileHeader = mobileHeader;
	this.mobileFooter = mobileFooter;
	this.$content = $content;
    this.$pageDiv = $pageDiv;
    this.getContent = getContent;
    this.CswChangePage = function(options) {
        $pageDiv.CswChangePage(options);
    };
    this.CswPage = function() {
        $pageDiv.CswPage();
    };
	//#region public, priveleged
}

//#endregion CswMobilePageFactory