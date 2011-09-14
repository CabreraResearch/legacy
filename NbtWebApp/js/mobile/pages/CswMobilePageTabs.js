/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../main/tools/CswAttr.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

//#region CswMobilePageTabs

function CswMobilePageTabs(tabsDef, $page, mobileStorage) {
    /// <summary>
    ///   Nodes Page class. Responsible for generating a Mobile nodes page.
    /// </summary>
    /// <param name="nodesDef" type="Object">Nodes definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageTabs">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    var pageDef = { };
    var id = CswMobilePage_Type.tabs.id;
    var title = CswMobilePage_Type.tabs.title;
    var viewId, level, nodeId;
    var divSuffix = '_tabs';
    var ulSuffix = '_list';
    var $contentPage = $page.find('div:jqmData(role="content")');
    var $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + id + divSuffix);
    var contentDivId;
    
    //ctor
    (function () {
        
        var p = {
            level: 2,
            ParentId: '',
            DivId: '', 
            viewId: mobileStorage.currentViewId(),
            nodeId: mobileStorage.currentNodeId(),
            title: '',
            theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            onHelpClick: null, //function () {},
            onOnlineClick: null, //function () {},
            onRefreshClick: null, //function () {},
            onSearchClick: null //function () {}
        };
        if (tabsDef) $.extend(p, tabsDef);

        if (!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }

        contentDivId = id + divSuffix;
        
        if (!isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        nodeId = p.nodeId;
        viewId = p.viewId;
        level = tryParseNumber(p.level, 2);
        
        var buttons = { };
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.refresh.name] = p.onRefreshClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        buttons[CswMobileHeaderButtons.back.name] = '';
        buttons[CswMobileHeaderButtons.search.name] = p.onSearchClick;

        pageDef = makeMenuButtonDef(p, id, buttons, mobileStorage);
        $content = ensureContent($content, contentDivId);
    })(); //ctor
   
    function getContent(onSuccess, postSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function to execute after the list is built.</param>
        var cachedJson = mobileStorage.fetchCachedNodeJson(nodeId);
        if( !isNullOrEmpty(cachedJson) && 
            cachedJson.hasOwnProperty('subitems') &&
            !isNullOrEmpty(cachedJson['subitems'])) 
        {
            var nodeJson = cachedJson['subitems'];
            refreshTabContent(nodeJson, onSuccess, postSuccess);
        } else {
            makeEmptyListView(null, $content, 'No Tabs to Display');
            stopLoadingMsg();
        }
    }
    
    function refreshTabContent(nodeJson, onSuccess, postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        $content = ensureContent($content, contentDivId);
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        
        var listView = new CswMobileListView(ulDef, $content);
        var tabCount = 0;
        if (false === isNullOrEmpty(nodeJson)) {
            for (var tabName in nodeJson) {
                if (nodeJson.hasOwnProperty(tabName)) {
                    var tabId = makeSafeId({prefix: tabName, ID: nodeId }); 
                    
                    var opts = {
                        ParentId: id,
                        DivId: tabId,
                        viewId: viewId,
                        nodeId: nodeId,
                        tabId: tabId,
                        tabName: tabName,
                        tabJson: nodeJson[tabName],
                        level: 3,
                        title: tabName,
                        onHelpClick: pageDef.onHelpClick,
                        onOnlineClick: pageDef.onOnlineClick,
                        onRefreshClick: pageDef.onRefreshClick,
                        mobileStorage: mobileStorage
                    };

                    var onClick = makeDelegate(pageDef.onListItemSelect, opts);
                    
                    listView.addListItemLink(tabId, tabName, onClick);
                    tabCount++;
                }
            }
        } 
        if (tabCount === 0) {
            listView.addListItemLink('no_results', 'No Tabs to Display');
        }
        if (!mobileStorage.stayOffline()) {
            toggleOnline(mobileStorage);
        }
        if (isFunction(onSuccess)) {
            onSuccess($content);
        }
        if (isFunction(postSuccess)) {
            postSuccess();
        }
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

//#endregion CswMobilePageTabs