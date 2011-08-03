/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />

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
        
        $content = getContent();
        
    })(); //ctor
        
    function getContent(viewId) {
        if(isNullOrEmpty(viewId)) {
            viewId = mobileStorage.currentViewId();
        }
        var searchJson = mobileStorage.fetchCachedViewJson(viewId, 'search');

        var $searchContent = $('<div></div>');
        var $fieldCtn = $('<div data-role="fieldcontain"></div>')
            .appendTo($searchContent);
        var values = [];
        var selected;

        if (!isNullOrEmpty(searchJson)) {
            for (var key in searchJson) {
                if (!selected) selected = key;
                values.push({ 'value': key, 'display': searchJson[key] });
            }

            var $select = $fieldCtn.CswSelect('init', {
                ID: id + '_searchprop',
                selected: selected,
                cssclass: 'csw_search_select',
                values: values
            })
                .CswAttrXml({ 'data-native-menu': 'false' });

            var $searchCtn = $('<div data-role="fieldcontain"></div>')
                .appendTo($searchContent);
            $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: id + '_searchfor' })
                .CswAttrXml({
                        'placeholder': 'Search',
                        'data-placeholder': 'Search'
                    });
            $searchContent.CswLink('init', { type: 'button', ID: id + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
                .CswAttrXml({ 'data-role': 'button' })
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg(function() { onSearchSubmit(); });
                });
            $searchContent.CswDiv('init', { ID: id + '_searchresults' });
        }

        function onSearchSubmit() {
            var searchprop = $('#' + id + '_searchprop').val();
            var searchfor = $('#' + id + '_searchfor').val();
            var $resultsDiv = $('#' + id + '_searchresults')
                .empty();

            if (!isNullOrEmpty(searchJson)) {
                var $searchResults = $resultsDiv.cswUL({ id: id + '_searchresultslist', 'data-filter': false })
                    .append($('<li data-role="list p.DivIder">Results</li>'));

                var hitcount = 0;
                for (var nodeKey in searchJson)
                {
                    var node = searchJson[nodeKey];
                    if (!isNullOrEmpty(node[searchprop])) {
                        if (node[searchprop].toLowerCase().indexOf(searchfor.toLowerCase()) >= 0) {
                            hitcount++;
                            var nodeJson = { id: nodeKey, value: node };
                            $searchResults.append(
    //!HEY YOU!. This becomes var x = new CswMobilePageNodes()
    //							_makeListItemFromJson($content, {
    //								ParentId: DivId + '_searchresults',
    //								DivId: DivId + '_searchresultslist',
    //								title: 'Results',
    //								PageType: 'node',
    //								json: nodeJson,
    //								parentlevel: 1 }
    //								)
							    );
                        }
                    }
                }
                if (hitcount === 0) {
                    $searchResults.append($('<li>No Results</li>'));
                }
                $searchResults.CswPage();
            }
            stopLoadingMsg();
        } // onSearchSubmit()
        
        return $searchContent;
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageSearch