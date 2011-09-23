/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
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
    
    var pageDef = { },
        id = CswMobilePage_Type.nodes.id,
        title = CswMobilePage_Type.nodes.title,
        viewId, 
        level,
        divSuffix = '_nodes',
        ulSuffix = '_list',
        $contentPage = $page.find('div:jqmData(role="content")'),
        contentDivId = id + divSuffix,
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);
    
    //ctor
    (function () {
        
        var p = {
                level: 1,
                ParentId: '',
                DivId: '', 
                title: '',
                viewId: mobileStorage.currentViewId(),
                theme: CswMobileGlobal_Config.theme,
                headerDef: { buttons: {} },
                footerDef: { buttons: {} },
                onHelpClick: null, //function () {},
                onOnlineClick: null, //function () {},
                onRefreshClick: null, //function () {},
                onSearchClick: null //function () {}
            },
            buttons = { };
        
        if (nodesDef) $.extend(p, nodesDef);

        if (false === isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }
        
        contentDivId = id + divSuffix;
        
        if (false === isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        
        viewId = p.viewId;
        level = tryParseNumber(p.level, 1);
        
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
        var cachedJson = mobileStorage.fetchCachedViewJson(viewId);

        if (isTimeToRefresh(mobileStorage)) {
            refreshNodeJson(onSuccess, postSuccess);
        } else if (false === isNullOrEmpty(cachedJson)) {
            refreshNodeContent(cachedJson, onSuccess, postSuccess);
        } else {
            makeEmptyListView(null, $content, 'No Results');
            stopLoadingMsg();
        }
    }
    
    function refreshNodeJson(onSuccess, postSuccess) {
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
                    var searchJson = data['searches'],
                        nodesJson = data['nodes'];
                    
                    setOnline(mobileStorage);
                    mobileStorage.storeViewJson(id, title, nodesJson, level, searchJson);
                    refreshNodeContent(nodesJson,onSuccess,postSuccess);
                },
                error: function() {
                    onError();
                }
            });
    }
    
    function refreshNodeContent(viewJson, onSuccess, postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef = {
                ID: id + ulSuffix,
                cssclass: CswMobileCssClasses.listview.name
            },
            listView = new CswMobileListView(ulDef, $content),
            nodeCount = 0,
            nodeKey, nodeJson, ocDef, node, opts, onClick; 
        
        if (isNullOrEmpty(viewJson)) {
            viewJson = mobileStorage.fetchCachedViewJson(id);
        }
        
        if (false === isNullOrEmpty(viewJson)) {
            for (nodeKey in viewJson) {
                if(viewJson.hasOwnProperty(nodeKey)) {
                    nodeJson = viewJson[nodeKey];
                    if (Int32MinVal === nodeKey.split('_')[1] || 'No Results' === nodeJson) {
                        makeEmptyListView(listView, null, 'No Results');
                    } else {
                        delete nodeJson.subitems;
                        ocDef = { nodeKey: nodeKey };
                        $.extend(ocDef, nodeJson);
                        node = new CswMobileNodesFactory(ocDef);
                        
                        opts = {
                            ParentId: id,
                            DivId: nodeKey,
                            viewId: viewId,
                            nodeId: mobileStorage.currentNodeId(nodeKey),
                            level: 2,
                            title: node.nodeName,
                            onHelpClick: pageDef.onHelpClick,
                            onOnlineClick: pageDef.onOnlineClick,
                            onRefreshClick: pageDef.onRefreshClick,
                            mobileStorage: mobileStorage
                        };

                        onClick = makeDelegate(pageDef.onListItemSelect, opts);

                        if (node.nodeSpecies !== CswNodeSpecies.More) {
                            listView.addListItemLinkHtml(nodeKey, node.$content, onClick, { icon: node.icon });
                        } else {
                            listView.addListItemHtml(nodeKey, node.$content, null, { icon: node.icon });
                        }
                    }
                    nodeCount++;
                }
            }
        } 
        if (nodeCount === 0) {
            makeEmptyListView(listView, null, 'No Results');
        }
        if (false === mobileStorage.stayOffline()) {
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

//#endregion CswMobilePageNodes