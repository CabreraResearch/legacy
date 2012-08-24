/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../main/tools/CswTools.js" />
/// <reference path="../../main/tools/CswAttr.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../objectclasses/CswMobileNodesFactory.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

//#region CswMobilePageNodes

function CswMobilePageNodes(nodesDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Nodes Page class. Responsible for generating a Mobile nodes page.
    /// </summary>
    /// <param name="nodesDef" type="Object">Nodes definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageNodes">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    var pageDef = { };
    var id, title, contentDivId, $content, viewId, level, forceRefresh,
        divSuffix = '_nodes',
        ulSuffix = '_list';
    
    //ctor
    (function () {
        pageDef = {
            level: 1,
            ParentId: '',
            DivId: '',
            buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help, CswMobileHeaderButtons.back, CswMobileHeaderButtons.search],
            title: '',
            viewId: '',
            nodeId: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (nodesDef) {
            $.extend(pageDef, nodesDef);
        }
        forceRefresh = mobileStorage.forceContentRefresh();
        mobileStorage.forceContentRefresh(false);

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.nodes.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.nodes.title);
        $content = ensureContent($contentRole, contentDivId);
        
        viewId = mobileStorage.currentViewId(pageDef.viewId);
        level = tryParseNumber(pageDef.level, 1);
    })();    //ctor

    function getContent(onSuccess) {
        var cachedJson,
            doServerRefresh = isTimeToRefresh(mobileStorage) || forceRefresh; 
        if(false === isNullOrEmpty(pageDef.nodeId)) {
            cachedJson = mobileStorage.fetchCachedNodeJson(pageDef.nodeId).nodes;
            doServerRefresh = false;
        } else {
            cachedJson = mobileStorage.fetchCachedViewJson(viewId);
        }
        
        if (doServerRefresh) {
            refreshNodeJson(onSuccess);
        } else if (false === isNullOrEmpty(cachedJson)) {
            refreshNodeContent(cachedJson, onSuccess);
        } else {
            makeEmptyListView(null, $content, 'No Results');
            stopLoadingMsg();
        }
    }
    
    function refreshNodeJson(onSuccess) {
        ///<summary>Fetches the nodes from the selected view from the web server and rebuilds the list.</summary>
        var getView = '/NbtWebApp/wsNBT.asmx/GetView',
            jsonData = {
                SessionId: mobileStorage.sessionid(),
                ParentId: viewId,
                ForMobile: true
            };

            CswAjaxJson({
                formobile: true,
                url: getView,
                data: jsonData,
                onloginfail: function (text) { onLoginFail(text, mobileStorage); },
                success: function (data) {
                    var searchJson = data.searches,
                        nodesJson = data.nodes;

                    setOnline(mobileStorage);
                    mobileStorage.storeViewJson(id, title, nodesJson, level, searchJson);
                    refreshNodeContent(nodesJson, onSuccess);
                },
                error: function () {
                    onError();
                }
            });
    }
    
    function refreshNodeContent(cachedJson, onSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name,
            showLoading: false
        };
        var listView = new CswMobileListView(ulDef, $content),
            nodeCount = 0,
            nodeId, nodeJson, ocDef, node, opts, onClick;

        if (isNullOrEmpty(cachedJson)) {
            getContent(onSuccess);
        } else {
            for (nodeId in cachedJson) {
                if (contains(cachedJson, nodeId)) {
                    nodeJson = cachedJson[nodeId];
                    if (Int32MinVal === nodeId.split('_')[1] || 'No Results' === nodeJson) {
                        makeEmptyListView(listView, null, 'No Results');
                    } else {
                        //delete nodeJson.subitems;
                        ocDef = { nodeKey: nodeId };
                        $.extend(ocDef, nodeJson);
                        node = new CswMobileNodesFactory(ocDef);

                        opts = {
                            ParentId: id,
                            DivId: nodeId,
                            viewId: viewId,
                            nodeId: mobileStorage.currentNodeId(nodeId),
                            level: pageDef.level + 1,
                            title: node.nodeName,
                            onHelpClick: pageDef.onHelpClick,
                            onOnlineClick: pageDef.onOnlineClick,
                            onRefreshClick: pageDef.onRefreshClick,
                            mobileStorage: mobileStorage
                        };

                        onClick = makeDelegate(pageDef.onListItemSelect, opts);

                        if (node.nodeSpecies.name !== CswNodeSpecies.More.name) {
                            listView.addListItemLinkHtml(nodeId, node.$content, onClick, { icon: node.icon });
                        } else {
                            listView.addListItem(nodeId, node.nodeName, null, { icon: node.icon });
                        }
                        nodeCount += 1;
                    }
                }
            }
            if (nodeCount === 0) {
                makeEmptyListView(listView, null, 'No Results');
            }
            if (false === mobileStorage.stayOffline()) {
                toggleOnline(mobileStorage);
            }
            $contentRole.append($content);
            doSuccess(onSuccess, $contentRole);
        }
    }

    //#endregion private
    
    //#region public, priveleged

    return {
        $parent: $parent,
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

//#endregion CswMobilePageNodes