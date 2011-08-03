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
    /// <param name="pageType" type="CswMobilePage_Type">CswMobilePage_Type enum value.</param>
	/// <param name="pageDef" type="Object">JSON definition of content to display, including header/footerDef properties.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
	/// <returns type="CswMobilePageFactory">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
	var mobileHeader, mobileFooter, $content, $pageDiv, id, title, getContent;
    var cswMobilePage;
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
            p.DivId = pageType.id;
        }
        id = makeSafeId({ ID: p.DivId });

        if(isNullOrEmpty(p.title)) {
            title = p.title = pageType.title;
        }
        
        var $page = getPageDiv(p.title, p.theme);

        switch (pageType.name) {
            case CswMobilePage_Type.help.name:
                {
                    cswMobilePage = new CswMobilePageHelp(p, $page, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.login.name:
                {
                    cswMobilePage = new CswMobilePageLogin(p, $page, p.mobileStorage, p.onSuccess);
                    break;
                }
            case CswMobilePage_Type.nodes.name:
                {
                    cswMobilePage = new CswMobilePageNodes(p, $page, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.offline.name:
                {
                    cswMobilePage = new CswMobilePageOffline(p, $page, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.online.name:
                {
                    cswMobilePage = new CswMobilePageOnline(p, $parent, p.mobileStorage, p.mobileSync, p.mobileBgTask);
                    break;
                }
            case CswMobilePage_Type.props.name:
                {
                    cswMobilePage = new CswMobilePageProps(p, $page, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.search.name:
                {
                    cswMobilePage = new CswMobilePageSearch(p, $page, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.tabs.name:
                {
                    cswMobilePage = new CswMobilePageTabs(p, $page, p.mobileStorage);
                    break;
                }
            case CswMobilePage_Type.views.name:
                {
                    cswMobilePage = new CswMobilePageViews(p, $page, p.mobileStorage);
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
            $content = getContentDiv(p.theme);
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
    
    function getContentDiv(theme,forceRefresh) {
        /// <summary> Refreshes the content of a page.</summary>
        /// <param name="theme" type="String">JQM theme to style content.</param>
	    /// <param name="forceRefresh" type="Boolean">True to force a refresh from the page class, false to load from memory.</param>
	    /// <returns type="void"></returns>
        
        $content = $pageDiv.find('div:jqmData(role="content")');

        if (!isNullOrEmpty($content) && $content.length > 0) {
            $content.empty();
        } else {
            $content = $pageDiv.CswDiv('init', { ID: id + '_content' })
                .CswAttrXml({ 'data-role': 'content', 'data-theme': theme });
        }
        if (cswMobilePage.$content && !forceRefresh) {
            $content.append(cswMobilePage.$content);
            onPageComplete();
        } else {
            $content.append(cswMobilePage.getContent(refreshPageContent));
        }
        return $content;
    }
    
    function refreshPageContent($newContent) {
        onPageInit();
        $content.append($newContent);
        onPageComplete();
    }
    
    function onPageInit(onSuccess) {
        startLoadingMsg(onSuccess);
    }
    
    function onPageComplete(onSuccess) {
        $content.CswPage();
        stopLoadingMsg(onSuccess);
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
    this.CswSetPath = function() {
        $pageDiv.CswSetPath();
    };
	//#region public, priveleged
}

//#endregion CswMobilePageFactory