/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
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

function CswMobilePageNodes(nodesDef, $page, mobileStorage) {
    /// <summary>
    ///   Nodes Page class. Responsible for generating a Mobile nodes page.
    /// </summary>
    /// <param name="nodesDef" type="Object">Nodes definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageNodes">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }

    var pageDef = { };
    var id, title, contentDivId, $contentPage, $content, viewId, level, forceRefresh,
        divSuffix = '_nodes',
        ulSuffix = '_list';
    
    //ctor
    (function () {

        var p = {
            level: 1,
            ParentId: '',
            DivId: '',
            title: '',
            viewId: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (nodesDef) {
            $.extend(p, nodesDef);
        }
        forceRefresh = mobileStorage.forceContentRefresh();
        mobileStorage.forceContentRefresh(false);

        id = tryParseString(p.DivId, CswMobilePage_Type.nodes.id);
        contentDivId = id + divSuffix;
        title = tryParseString(p.title, CswMobilePage_Type.nodes.title);
        $contentPage = $page.find('div:jqmData(role="content")');
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);

        viewId = mobileStorage.currentViewId(p.viewId);
        level = tryParseNumber(p.level, 1);

        $content = ensureContent($content, contentDivId);
    })();    //ctor

    function getContent(onSuccess) {
        var cachedJson = mobileStorage.fetchCachedViewJson(viewId);

        if (isTimeToRefresh(mobileStorage) || forceRefresh) {
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
                onloginfail: function(text) { onLoginFail(text, mobileStorage); },
                success: function(data) {
                    var searchJson = data.searches,
                        nodesJson = data.nodes;
                    
                    setOnline(mobileStorage);
                    mobileStorage.storeViewJson(id, title, nodesJson, level, searchJson);
                    refreshNodeContent(nodesJson,onSuccess);
                },
                error: function() {
                    onError();
                }
            });
    }
    
    function refreshNodeContent(viewJson, onSuccess) {
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

        if (isNullOrEmpty(viewJson)) {
            viewJson = mobileStorage.fetchCachedViewJson(id);
        }

        if (isNullOrEmpty(viewJson)) {
            refreshNodeJson(onSuccess);
        } else {
            for (nodeId in viewJson) {
                if (viewJson.hasOwnProperty(nodeId)) {
                    nodeJson = viewJson[nodeId];
                    if (Int32MinVal === nodeId.split('_')[1] || 'No Results' === nodeJson) {
                        makeEmptyListView(listView, null, 'No Results');
                    } else {
                        delete nodeJson.subitems;
                        ocDef = { nodeKey: nodeId };
                        $.extend(ocDef, nodeJson);
                        node = new CswMobileNodesFactory(ocDef);

                        opts = {
                            ParentId: id,
                            DivId: nodeId,
                            viewId: viewId,
                            nodeId: mobileStorage.currentNodeId(nodeId),
                            level: 2,
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
                    }
                    nodeCount += 1;
                }
            }
            if (nodeCount === 0) {
                makeEmptyListView(listView, null, 'No Results');
            }
            if (false === mobileStorage.stayOffline()) {
                toggleOnline(mobileStorage);
            }
            doSuccess(onSuccess, $content);
        }
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

//#endregion CswMobilePageNodes