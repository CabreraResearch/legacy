/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../objectclasses/CswMobileObjectClassFactory.js" />

//#region CswMobilePageSearch

function CswMobilePageSearch(searchDef,$parent,mobileStorage) {
	/// <summary>
	///   Search Page class. Responsible for generating a Mobile search page.
	/// </summary>
    /// <param name="searchDef" type="Object">Search definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageSearch">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content = '';
    var pageDef = { };
    var id = CswMobilePage_Type.search.id;
    var title = CswMobilePage_Type.search.title;
    var divSuffix = '_search';
    var ulSuffix = '_ul';
    var contentDivId;
    
    //ctor
    (function() {

        if (isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }

        var viewId = mobileStorage.currentViewId();
        
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
        
        if(!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }

        contentDivId = id + divSuffix;
        
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
       
        var buttons = { };
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        buttons[CswMobileHeaderButtons.back.name] = '';

        pageDef = p = makeMenuButtonDef(p, id, buttons, mobileStorage);

        getContent();
    })(); //ctor
        
    function getContent(viewId) {
        $content = ensureContent($content, contentDivId);
        
        if(isNullOrEmpty(viewId)) {
            viewId = mobileStorage.currentViewId();
        }
        var searchJson = mobileStorage.fetchCachedViewJson(viewId, 'search');

        var $fieldCtn = $('<div data-role="fieldcontain"></div>')
            .appendTo($content);
        var values = [];
        var selected;

        if (!isNullOrEmpty(searchJson)) {
            for (var key in searchJson) {
                if (!selected) selected = key;
                values.push({ 'value': key, 'display': searchJson[key] });
            }

            $fieldCtn.CswSelect('init', {
                    ID: id + '_searchprop',
                    selected: selected,
                    cssclass: 'csw_search_select',
                    values: values
                })
                .CswAttrXml({ 'data-native-menu': 'false' });

            var $searchCtn = $('<div data-role="fieldcontain"></div>')
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
                    return startLoadingMsg(function() { onSearchSubmit(); });
                });
            $content.CswDiv('init', { ID: id + '_searchresults' });
        }

        function onSearchSubmit() {
            var searchprop = $('#' + id + '_searchprop').val();
            var searchfor = $('#' + id + '_searchfor').val();
            var $resultsDiv = $('#' + id + '_searchresults')
                .empty();
            
            var ulDef = {
                ID: id + ulSuffix,
                cssclass: CswMobileCssClasses.listview.name
            };
        
            var listView = new CswMobileListView(ulDef, $content);
            var nodeCount = 0;
            if (!isNullOrEmpty(searchJson)) {
                listView.addListItem(id + '_results', 'Results', null, { 'data-role': 'list-divider' });
                for (var nodeKey in searchJson)
                {
                    if (searchJson.hasOwnProperty(nodeKey) &&
                        searchJson[nodeKey].hasOwnProperty(searchprop)) {

                        var node = searchJson[nodeKey];
                        if (node[searchprop].toLowerCase().indexOf(searchfor.toLowerCase()) >= 0) {
                            var nodePk = nodeKey.split('_');
                            if (nodePk.hasOwnProperty(1)) {
                                var nodeId = nodeKey[1];
                            }
                            if (Int32MinVal === nodeId || 'No Results' === searchJson[nodeKey]) {
                                makeEmptyListView(listView, null, 'No Results');
                            } else {

                                var nodeJson = { ID: nodeKey, value: searchJson[nodeKey] };
                                //we need CswMobileObjectClassFactory to finish here.
                                //var nodeOc = makeOcContent(nodeJson);

                                var opts = {
                                    ParentId: id,
                                    DivId: nodeKey,
                                    viewId: viewId,
                                    nodeId: mobileStorage.currentNodeId(nodeKey),
                                    level: 2,
                                    title: searchJson[nodeKey]['node_name'],
                                    onHelpClick: pageDef.onHelpClick,
                                    onOnlineClick: pageDef.onOnlineClick,
                                    onRefreshClick: pageDef.onRefreshClick,
                                    mobileStorage: mobileStorage
                                };

                                var onClick = makeDelegate(pageDef.onListItemSelect, opts);

//                                if (nodeOc.isLink) {
//                                    listView.addListItemLinkHtml(nodeKey, nodeOc.$html, onClick);
//                                } else {
//                                    listView.addListItemHtml(nodeKey, nodeOc.$html, onClick);
//                                }
                                nodeCount++;
                            }
                        }

                    }
                }
            }
            if (nodeCount === 0) {
                makeEmptyListView(listView, null, 'No Results');
            }
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