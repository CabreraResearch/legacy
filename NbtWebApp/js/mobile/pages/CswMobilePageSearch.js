/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
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

function CswMobilePageSearch(searchDef,$page,mobileStorage) {
    /// <summary>
    ///   Search Page class. Responsible for generating a Mobile search page.
    /// </summary>
    /// <param name="searchDef" type="Object">Search definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageSearch">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var pageDef = { };
    var id = CswMobilePage_Type.search.id,
        title = CswMobilePage_Type.search.title,
        divSuffix = '_search',
        ulSuffix = '_ul',
        contentDivId, $contentPage, $content, viewId;
    
    
    //ctor
    (function() {

        if (isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }

        viewId = mobileStorage.currentViewId();
        
        var p = {
            level: -1,
            ParentId: '',
            DivId: CswMobilePage_Type.search.id + viewId,
            title: '',
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            theme: CswMobileGlobal_Config.theme,
            onHelpClick: function() {}
        };
        if (searchDef) $.extend(p, searchDef);
        
        if (false === isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }

        contentDivId = id + divSuffix;
        $contentPage = $page.find('div:jqmData(role="content")');
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);
        
        if (false === isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
       
        var buttons = { };
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        buttons[CswMobileHeaderButtons.back.name] = '';

        pageDef = makeMenuButtonDef(p, id, buttons, mobileStorage);

        getContent();
    })(); //ctor
        
    function getContent() {
        $content = ensureContent($content, contentDivId);
        
        var searchJson = mobileStorage.fetchCachedViewJson(viewId, 'search'),
            $fieldCtn = $('<div data-role="fieldcontain"></div>')
                            .appendTo($content),
            values = [],
            selected = '', key, $searchCtn;

        if (false === isNullOrEmpty(searchJson)) {
            for (key in searchJson) {
                if (false === selected) selected = key;
                values.push({ 'value': key, 'display': searchJson[key] });
            }

            $fieldCtn.CswSelect('init', {
                          ID: id + '_searchprop',
                          selected: selected,
                          cssclass: 'csw_search_select',
                          values: values
                      })  
                      .CswAttrXml({ 'data-native-menu': 'false' });

            $searchCtn = $('<div data-role="fieldcontain"></div>')
                            .appendTo($content);
            $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: id + '_searchfor' })
                       .CswAttrXml({
                            'placeholder': 'Search',
                            'data-placeholder': 'Search'
                        });
            $content.CswLink('init', { type: 'button', ID: id + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
                .CswAttrXml({ 'data-role': 'button' })
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg(function() { onSearchSubmit(viewId); });
                });
            $content.CswDiv('init', { ID: id + '_searchresults' });
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
                                nodeCount++;
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

    this.$content = $content;
    this.contentDivId = contentDivId;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageSearch