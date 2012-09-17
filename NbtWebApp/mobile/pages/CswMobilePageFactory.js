/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
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

function CswMobilePageFactory(theme, mobileStorage, $parent) {
    /// <summary> Page factory class. Responsible for generating a Mobile page.  </summary>
    /// <param name="$" type="jQuery"></param>
    /// <returns type="CswMobilePageFactory">Instance of itself.</returns>

    //#region private
    
    function getPageRoleDiv(headerText, id) {
        if (false === isNullOrEmpty($parent, true)) {
            var $ret = $('#' + id);
            if (false === isNullOrEmpty($ret) || $ret.length > 0) {
                $ret.remove();
            }

            $ret = $parent.CswDiv('init', { ID: id })
                            .CswAttrNonDom({
                                    'data-role': 'page',
                                    'data-url': id,
                                    'data-title': headerText,
                                    'data-rel': 'page',
                                    'data-theme': theme
                                });
            return $ret;
        }
    }
    
    function getMenuHeader(options, $pageDiv, title) {
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
    
    function getMenuFooter(options, $pageDiv) {
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

    function getContentDiv($pageDiv, id) {
        /// <summary> Refreshes the content of a page.</summary>
        /// <param name="theme" type="String">JQM theme to style content.</param>
        /// <param name="forceRefresh" type="Boolean">True to force a refresh from the page class, false to load from memory.</param>
        /// <returns type="void"></returns>
        var $contentRole = $pageDiv.find('div:jqmData(role="content")');
        if (false === isNullOrEmpty($contentRole) && $contentRole.length > 0) {
            $contentRole.remove();
        }
        $contentRole = $('<div id="' + id + '_content" data-role="content" data-theme="' + theme + '"></div>');
        return $contentRole;
    }
    
    function bindPageEvents($pageDiv, cswMobilePage) {
        $pageDiv.unbind('pageshow');
        $pageDiv.bind('pageshow', function() {
            startLoadingMsg();
            setTimeout(function() {
                fillContent(false, function() {
                        stopLoadingMsg();
                    },
                    $pageDiv, cswMobilePage);
            }, 500);
        });
    }

    function fillContent(forceRefresh, onSuccess, $pageDiv, cswMobilePage) {
        var $contentRole = cswMobilePage.$contentRole,
            $content = cswMobilePage.$content;
        if (contentIsFullyPopulated($content) && false === forceRefresh) {
            $contentRole.append($content);
            onPageComplete(onSuccess, $contentRole);
        } else {
            cswMobilePage.getContent([refreshPageContent, onSuccess ]);
            //$contentRole.append(cswMobilePage.getContent([refreshPageContent, onSuccess]));
            //onPageComplete(onSuccess);
        }
        if ($contentRole.height() < 300) {
            $contentRole.css('min-height', $(document).height() - 125);
            recalculateFooter($pageDiv);
        }
        
        
        return $contentRole;        
    }
    
    function contentIsFullyPopulated($content) {
        var ret = false;
        var $div = $content;
        if (false === isNullOrEmpty($div) &&
            $div.length > 0 &&
            $div.children().length > 0) {
            ret = true;
        }
        return ret;
    }
    
    function refreshPageContent($contentRole) {
        ///<summary>Append content to page</summary>
        onPageComplete([], $contentRole);
    }
    
    function onPageComplete(onSuccess, $contentRole) {
        $contentRole.parent().trigger('create');
        //$contentRole.trigger('create');
        //$contentRole.find(':jqmData(role=listview)').listview();
        doSuccess(onSuccess, $contentRole);
    }

    function makeButtonDef(options, id) {
        var ret,
            buttons = {},
            p = {
                buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help, CswMobileHeaderButtons.back, CswMobileHeaderButtons.search],
                onOnlineClick: null,
                onRefreshClick: null,
                onHelpClick: null,
                onSearchClick: null
            };
        if (options) {
            $.extend(p, options);
        }

        if (contains(p.buttons, CswMobileFooterButtons.online)) {
            buttons[CswMobileFooterButtons.online.name] = isFunction(p.onOnlineClick) ? p.onOnlineClick : '';
        }
        if (contains(p.buttons, CswMobileFooterButtons.refresh)) {
            buttons[CswMobileFooterButtons.refresh.name] = isFunction(p.onRefreshClick) ? p.onRefreshClick : '';
        }
        if (contains(p.buttons, CswMobileFooterButtons.fullsite)) {
            buttons[CswMobileFooterButtons.fullsite.name] = '';
        }
        if (contains(p.buttons, CswMobileFooterButtons.help)) {
            buttons[CswMobileFooterButtons.help.name] = isFunction(p.onHelpClick) ? p.onHelpClick : '';
        }
        if (contains(p.buttons, CswMobileHeaderButtons.back)) {
            buttons[CswMobileHeaderButtons.back.name] = '';
        }
        if (contains(p.buttons, CswMobileHeaderButtons.search)) {
            buttons[CswMobileHeaderButtons.search.name] = isFunction(p.onSearchClick) ? p.onSearchClick : '';
        }

        ret = makeMenuButtonDef(p, id, buttons, mobileStorage);
        return ret;
    }
    
    function makePage(pageType, pageDef) {
        /// <summary></summary>
        /// <param name="pageType" type="CswMobilePage_Type">CswMobilePage_Type enum value.</param>
        /// <param name="pageDef" type="Object">JSON definition of content to display, including header/footerDef properties.</param>
        /// <param name="$parent" type="jQuery">Parent element to attach to.</param>

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
        var ret = null,
            $pageDiv, id, $contentRole,
            cswMobilePage = null;
        
        if (isNullOrEmpty(p.DivId)) {
            p.DivId = pageType.id;
        }
        id = makeSafeId({ ID: p.DivId });

        if (isNullOrEmpty(p.title)) {
            title = pageType.title;
            p.title = title;
        }
        $pageDiv = getPageRoleDiv(title, id);
        $contentRole = getContentDiv($pageDiv, id);
        
        switch (pageType.name) {
            case CswMobilePage_Type.help.name:
                cswMobilePage = CswMobilePageHelp(p, $pageDiv, mobileStorage, $contentRole);
                break;
            case CswMobilePage_Type.login.name:
                cswMobilePage = CswMobilePageLogin(p, $pageDiv, mobileStorage, p.onSuccess, $contentRole);
                break;
            case CswMobilePage_Type.nodes.name:
                cswMobilePage = CswMobilePageNodes(p, $pageDiv, mobileStorage, $contentRole);
                break;
            case CswMobilePage_Type.offline.name:
                cswMobilePage = CswMobilePageOffline(p, $pageDiv, mobileStorage, $contentRole);
                break;
            case CswMobilePage_Type.online.name:
                cswMobilePage = CswMobilePageOnline(p, $pageDiv, mobileStorage, p.mobileSync, p.mobileBgTask, $contentRole);
                break;
            case CswMobilePage_Type.props.name:
                cswMobilePage = CswMobilePageProps(p, $pageDiv, mobileStorage, $contentRole);
                break;
            case CswMobilePage_Type.search.name:
                cswMobilePage = CswMobilePageSearch(p, $pageDiv, mobileStorage, $contentRole);
                break;
            case CswMobilePage_Type.tabs.name:
                cswMobilePage = CswMobilePageTabs(p, $pageDiv, mobileStorage, $contentRole);
                break;
            case CswMobilePage_Type.views.name:
                cswMobilePage = CswMobilePageViews(p, $pageDiv, mobileStorage, $contentRole);
                break;
            default:
                throw ('makePage() called without CswMobilePage_Type');
        }
        if (cswMobilePage) {
            ret = cswMobilePage;
            title = cswMobilePage.title;
            $.extend(p, cswMobilePage.pageDef);

            var buttonDef = makeButtonDef(cswMobilePage.pageDef, id);
            
            ret.mobileHeader = getMenuHeader(buttonDef.headerDef, $pageDiv, title);
            $pageDiv.append($contentRole);
            ret.mobileFooter = getMenuFooter(buttonDef.footerDef, $pageDiv);
            ret.remove = function () {
                $pageDiv.remove();
                return null;
            };
            ret.CswChangePage = function(options) {
                $pageDiv.CswChangePage(options);
            };
            ret.CswSetPath = function() {
                $pageDiv.CswSetPath();
            };
            bindPageEvents($pageDiv, ret);
        }
        return ret;
    } //makePage()
    
    //#endregion private	
    
    //#region public, priveleged

    return {
        makePage: function (pageType, pageDef) {
            /// <summary></summary>
            /// <param name="pageType" type="CswMobilePage_Type">CswMobilePage_Type enum value.</param>
            /// <param name="pageDef" type="Object">JSON definition of content to display, including header/footerDef properties.</param>
            /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
            var page = makePage(pageType, pageDef);
            return page;
        }
    };
    //#endregion public, priveleged
}

//#endregion CswMobilePageFactory