/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
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

function CswMobilePageTabs(tabsDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Nodes Page class. Responsible for generating a Mobile nodes page.
    /// </summary>
    /// <param name="nodesDef" type="Object">Nodes definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageTabs">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    var pageDef = { };
    var id, title, contentDivId, $content, viewId, level, nodeId,
        divSuffix = '_tabs',
        ulSuffix = '_list';
    
    //ctor
    (function () {
        pageDef = {
            level: 2,
            ParentId: '',
            DivId: '',
            buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help, CswMobileHeaderButtons.back, CswMobileHeaderButtons.search],
            viewId: mobileStorage.currentViewId(),
            nodeId: mobileStorage.currentNodeId(),
            title: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (tabsDef) {
            $.extend(pageDef, tabsDef);
        }

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.tabs.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.tabs.title);
        $content = ensureContent($contentRole, contentDivId);

        nodeId = pageDef.nodeId;
        viewId = pageDef.viewId;
        level = tryParseNumber(pageDef.level, 2);
    })();   //ctor
   
    function getContent(onSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function to execute after the list is built.</param>
        $content = ensureContent($contentRole, contentDivId);
        var cachedJson = mobileStorage.fetchCachedNodeJson(nodeId),
            nodeJson;

        if (false === isNullOrEmpty(cachedJson) && 
            contains(cachedJson, 'tabs') &&
            false === isNullOrEmpty(cachedJson.tabs)) {
            nodeJson = cachedJson.tabs;
            refreshTabContent(nodeJson, onSuccess);
        } else {
            makeEmptyListView(null, $content, 'No Tabs to Display');
            stopLoadingMsg();
        }
    }
    
    function refreshTabContent(nodeJson, onSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        var tabName, tabId, opts, onClick,
            listView = new CswMobileListView(ulDef, $content),
            tabCount = 0;

        if (false === isNullOrEmpty(nodeJson)) {
            for (tabName in nodeJson) {
                if (contains(nodeJson, tabName)) {
                    tabId = makeSafeId({prefix: tabName, ID: nodeId }); 
                    
                    opts = {
                        ParentId: id,
                        DivId: tabId,
                        viewId: viewId,
                        nodeId: nodeId,
                        tabId: tabId,
                        tabName: tabName,
                        level: level + 1,
                        title: tabName,
                        onHelpClick: pageDef.onHelpClick,
                        onOnlineClick: pageDef.onOnlineClick,
                        onRefreshClick: pageDef.onRefreshClick,
                        mobileStorage: mobileStorage
                    };

                    onClick = makeDelegate(pageDef.onListItemSelect, opts);
                    
                    listView.addListItemLink(tabId, tabName, onClick);
                    tabCount += 1;
                }
            }
        } 
        if (tabCount === 0) {
            listView.addListItemLink('no_results', 'No Tabs to Display');
        }
        if (false === mobileStorage.stayOffline()) {
            toggleOnline(mobileStorage);
        }
        $contentRole.append($content);
        doSuccess(onSuccess, $content);
    }
    
    //#endregion private
    
    //#region public, priveleged

    return {
        $pageDiv: $parent,
        $contentRole: $contentRole,
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        id: id,
        title: title,
        getContent: getContent
    };
    //#endregion public, priveleged
}

//#endregion CswMobilePageTabs