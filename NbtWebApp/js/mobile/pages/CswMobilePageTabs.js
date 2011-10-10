/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
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
    var pageDef = { };
    var id, title, contentDivId, $contentPage, $content, viewId, level, nodeId,
        divSuffix = '_tabs',
        ulSuffix = '_list';
    
    //ctor
    (function () {

        var p = {
            level: 2,
            ParentId: '',
            DivId: '',
            viewId: mobileStorage.currentViewId(),
            nodeId: mobileStorage.currentNodeId(),
            title: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (tabsDef) {
            $.extend(p, tabsDef);
        }

        id = tryParseString(p.DivId, CswMobilePage_Type.tabs.id);
        contentDivId = id + divSuffix;
        title = tryParseString(p.title, CswMobilePage_Type.tabs.title);
        $contentPage = $page.find('div:jqmData(role="content")');
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);

        nodeId = p.nodeId;
        viewId = p.viewId;
        level = tryParseNumber(p.level, 2);

        $content = ensureContent($content, contentDivId);
    })();   //ctor
   
    function getContent(onSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function to execute after the list is built.</param>
        var cachedJson = mobileStorage.fetchCachedNodeJson(nodeId),
            nodeJson;
        if (false === isNullOrEmpty(cachedJson) && 
            contains(cachedJson, 'subitems') &&
            false === isNullOrEmpty(cachedJson.subitems)) 
        {
            nodeJson = cachedJson.subitems;
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
        
        $content = ensureContent($content, contentDivId);

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
                        level: 3,
                        title: tabName,
                        onHelpClick: pageDef.onHelpClick,
                        onOnlineClick: pageDef.onOnlineClick,
                        onRefreshClick: pageDef.onRefreshClick,
                        mobileStorage: mobileStorage
                    };

                    onClick = makeDelegate(pageDef.onListItemSelect, opts);
                    
                    listView.addListItemLink(tabId, tabName, onClick);
                    tabCount++;
                }
            }
        } 
        if (tabCount === 0) {
            listView.addListItemLink('no_results', 'No Tabs to Display');
        }
        if (false === mobileStorage.stayOffline()) {
            toggleOnline(mobileStorage);
        }
        doSuccess(onSuccess, $content);
    }
    
    //#endregion private
    
    //#region public, priveleged

    return {
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