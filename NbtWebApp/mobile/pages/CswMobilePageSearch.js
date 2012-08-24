/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../objectclasses/CswMobileNodesFactory.js" />

//#region CswMobilePageSearch

function CswMobilePageSearch(searchDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Search Page class. Responsible for generating a Mobile search page.
    /// </summary>
    /// <param name="searchDef" type="Object">Search definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageSearch">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var pageDef = { };
    var id, title, contentDivId, $content, viewId,
        divSuffix = '_search',
        ulSuffix = '_ul';
    
    //ctor
    (function () {
        viewId = mobileStorage.currentViewId();

        pageDef = {
            level: -1,
            ParentId: '',
            DivId: CswMobilePage_Type.search.id + viewId,
            buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help, CswMobileHeaderButtons.back],
            title: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (searchDef) {
            $.extend(pageDef, searchDef);
        }

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.search.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.search.title);
        $content = ensureContent($contentRole, contentDivId);

        getContent();
    })();   //ctor
        
    function getContent() {
        $content = ensureContent($contentRole, contentDivId);
        
        var searchJson = mobileStorage.fetchCachedViewJson(viewId, 'search'),
            $fieldCtn = $('<div data-role="fieldcontain"></div>')
                            .appendTo($content),
            values = [],
            selected = '', key, $searchCtn;

        if (false === isNullOrEmpty(searchJson)) {
            for (key in searchJson) {
                if (false === selected) {
                    selected = key;
                }
                values.push({ 'value': key, 'display': searchJson[key] });
            }

            $fieldCtn.CswSelect('init', {
                          ID: id + '_searchprop',
                          selected: selected,
                          cssclass: 'csw_search_select',
                          values: values
                      })  
                      .CswAttrNonDom({ 'data-native-menu': 'false' });

            $searchCtn = $('<div data-role="fieldcontain"></div>')
                            .appendTo($content);
            $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: id + '_searchfor' })
                       .CswAttrNonDom({
                            'placeholder': 'Search',
                            'data-placeholder': 'Search'
                        });
            $content.CswLink('init', { type: 'button', ID: id + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
                .CswAttrNonDom({ 'data-role': 'button' })
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg(function() { onSearchSubmit(viewId); });
                });
            $content.CswDiv('init', { ID: id + '_searchresults' });
            $contentRole.append($content);
        }

        function onSearchSubmit() {
            var ulDef = {
                ID: id + ulSuffix,
                cssclass: CswMobileCssClasses.listview.name
            };

            var searchprop = $('#' + id + '_searchprop').val(),
                searchfor = $('#' + id + '_searchfor').val(),
                $resultsDiv = $('#' + id + '_searchresults')
                                .empty(),
                listView = new CswMobileListView(ulDef, $resultsDiv),
                nodeCount = 0,
                viewNodes = mobileStorage.fetchCachedViewJson(viewId);
                
            if (false === isNullOrEmpty(viewNodes)) {
                listView.addListItem(id + '_results', 'Results', null, { 'data-role': 'list-divider' });
                each(viewNodes, function (nodeObj, nodeKey) { //, childKey, thisObj, value) {
                    var ret = false,
                        node, nodePk, nodeId = '', nodeProp, onClick, opts;
                    
                    if (contains(nodeObj, searchprop)) {
                        nodeProp = nodeObj[searchprop];
                        if (nodeProp.toLowerCase().indexOf(searchfor.toLowerCase()) !== -1) {
                            nodePk = nodeKey.split('_');
                            if (contains(nodePk, 2)) {
                                nodeId = nodePk[2];
                            }
                            if (Int32MinVal === nodeId) { // || 'No Results' === searchJson[nodeKey]) {
                                makeEmptyListView(listView, null, 'No Results');
                                ret = true; //to break loop
                            } else {
                                //nodeJson = { ID: nodeKey, value: searchJson[nodeKey] };
                                
                                node = new CswMobileNodesFactory(nodeObj);

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

                                if (node.nodeSpecies.name !== CswNodeSpecies.More.name) {
                                    listView.addListItemLinkHtml(nodeKey, node.$content, onClick, { icon: node.icon });
                                } else {
                                    listView.addListItem(nodeKey, node.nodeName, null, { icon: node.icon });
                                }
                                nodeCount += 1;
                            }
                        }
                    }
                    return ret;
                });
            }
            if (nodeCount === 0) {
                makeEmptyListView(listView, null, 'No Results');
            }
            $content.trigger('create');
            stopLoadingMsg();
        } // onSearchSubmit()
        
        return $content;
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

//#endregion CswMobilePageSearch