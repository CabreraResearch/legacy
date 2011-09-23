/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
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
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

//#region CswMobilePageFactory

function CswMobilePageFactory(pageType, pageDef, $parent) {
    /// <summary>
    ///   Page factory class. Responsible for generating a Mobile page.
    /// </summary>
    /// <param name="pageType" type="CswMobilePage_Type">CswMobilePage_Type enum value.</param>
    /// <param name="pageDef" type="Object">JSON definition of content to display, including header/footerDef properties.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobilePageFactory">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    var mobileHeader, mobileFooter, $contentRole, $pageDiv, id, title, getContent;
    var cswMobilePage;
    //ctor
    (function () {
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

        if (isNullOrEmpty(p.mobileStorage)) {
            p.mobileStorage = new CswMobileClientDbResources();
        }
        
        if (isNullOrEmpty(p.DivId)) {
            p.DivId = pageType.id;
        }
        id = makeSafeId({ ID: p.DivId });

        if (isNullOrEmpty(p.title)) {
            title = p.title = pageType.title;
        }
        
        $pageDiv = getPageDiv(title, p.theme, p.doChangePage);

        switch (pageType.name) {
            case CswMobilePage_Type.help.name:
                cswMobilePage = new CswMobilePageHelp(p, $pageDiv, p.mobileStorage);
                break;
            case CswMobilePage_Type.login.name:
                cswMobilePage = new CswMobilePageLogin(p, $pageDiv, p.mobileStorage, p.onSuccess);
                break;
            case CswMobilePage_Type.nodes.name:
                cswMobilePage = new CswMobilePageNodes(p, $pageDiv, p.mobileStorage);
                break;
            case CswMobilePage_Type.offline.name:
                cswMobilePage = new CswMobilePageOffline(p, $pageDiv, p.mobileStorage);
                break;
            case CswMobilePage_Type.online.name:
                cswMobilePage = new CswMobilePageOnline(p, $pageDiv, p.mobileStorage, p.mobileSync, p.mobileBgTask);
                break;
            case CswMobilePage_Type.props.name:
                cswMobilePage = new CswMobilePageProps(p, $pageDiv, p.mobileStorage);
                break;
            case CswMobilePage_Type.search.name:
                cswMobilePage = new CswMobilePageSearch(p, $pageDiv, p.mobileStorage);
                break;
            case CswMobilePage_Type.tabs.name:
                cswMobilePage = new CswMobilePageTabs(p, $pageDiv, p.mobileStorage);
                break;
            case CswMobilePage_Type.views.name:
                cswMobilePage = new CswMobilePageViews(p, $pageDiv, p.mobileStorage);
                break;
            default:
                throw ('CswMobilePageFactory initialized without CswMobilePage_Type');
        }
        if (cswMobilePage) {
            title = cswMobilePage.title;
            $.extend(p, cswMobilePage.pageDef);
        
            mobileHeader = getMenuHeader(p.headerDef);
            $contentRole = getContentDiv(p.theme);
            mobileFooter = getMenuFooter(p.footerDef);
            getContent = cswMobilePage.getContent;
            bindPageEvents();
        }
    })(); //ctor
    
    function getPageDiv(headerText, theme) {
        var $ret = $('#' + id);
            
        if (isNullOrEmpty($ret) || $ret.length === 0) {
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
    
    function getContentDiv(theme) {
        /// <summary> Refreshes the content of a page.</summary>
        /// <param name="theme" type="String">JQM theme to style content.</param>
        /// <param name="forceRefresh" type="Boolean">True to force a refresh from the page class, false to load from memory.</param>
        /// <returns type="void"></returns>
        
        $contentRole = $pageDiv.find('div:jqmData(role="content")');

        if (!isNullOrEmpty($contentRole) && $contentRole.length > 0) {
            $contentRole.empty();
        } else {
            $contentRole = $pageDiv.CswDiv('init', { ID: id + '_content' })
                .CswAttrXml({ 'data-role': 'content', 'data-theme': theme });
        }
        return $contentRole;
    }
    
    function bindPageEvents() {
        
        $pageDiv.bind('pageshow', function() {
            var $this = $(this);
            startLoadingMsg();
            setTimeout(function() {
                fillContent(false, function() {
                    stopLoadingMsg();
                    
                    var documentHeight = $(document).height(),
                        windowHeight = $(window).height(),
                        adjustedHeight = windowHeight - 333, // documentHeight - 542,
                        winDocHeightDif = documentHeight - windowHeight;
                    
                    if(winDocHeightDif > 0) {
                        $this.find('div:jqmData(role="footer")').css('top', adjustedHeight);
                    }
                });
            }, 500);
        });
    }
    
    function fillContent(forceRefresh,onSuccess) {
        if (contentIsFullyPopulated() && !forceRefresh) {
            $contentRole.append(cswMobilePage.$content);
            onPageComplete(onSuccess);
        } else {
            $contentRole.append(cswMobilePage.getContent(refreshPageContent,onSuccess));
        }
        return $contentRole;        
    }
    
    function contentIsFullyPopulated() {
        var ret = false;
        var $div = cswMobilePage.$content;
        if (!isNullOrEmpty($div) &&
            $div.length > 0 &&
            $div.children().length > 0) {
            ret = true;
        }
        return ret;
    }
    
    function refreshPageContent($newContent) {
        $contentRole.append($newContent);
        onPageComplete();
    }
    
    function onPageComplete(onSuccess) {
        $contentRole.trigger('create');
        if (isFunction(onSuccess)) {
            onSuccess();
        }
    }
    //#endregion private	
    
    //#region public, priveleged

    this.mobileHeader = mobileHeader;
    this.mobileFooter = mobileFooter;
    this.page = cswMobilePage;
    this.$content = $contentRole;
    this.$pageDiv = $pageDiv;
    this.fillContent = fillContent;
    this.remove = function() {
        $pageDiv.remove();
        return null;
    };
    this.CswChangePage = function(options) {
        $pageDiv.CswChangePage(options);
    };
    this.CswSetPath = function() {
        $pageDiv.CswSetPath();
    };

    //#region public, priveleged
}

//#endregion CswMobilePageFactory