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
	///   Online Page class. Responsible for generating a Mobile login page.
	/// </summary>
    /// <param name="onlineDef" type="Object">Login definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageSearch">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    if(isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    var viewId = mobileStorage.currentViewId();
	var searchJson = mobileStorage.fetchCachedViewJson(viewId,'search');
	
	var $searchContent = $('<div></div>');
	var $fieldCtn = $('<div data-role="fieldcontain"></div>')
    	.appendTo($searchContent);
	var values = [];
	var selected;
    
    var p = {
	    level: -1,
	    ParentId: '',
	    DivId: 'CswMobile_SearchDiv' + viewId,       // required
	    HeaderText: 'Search',
	    theme: 'b',
	    $content: $searchContent,
        onHelpClick: function () {}
	};
	if (searchDef) $.extend(p, searchDef);

    var pageDef = p;
    delete pageDef.onHelpClick;
    
	if (isNullOrEmpty(pageDef.footerDef)) {
	    pageDef.footerDef = {
	        buttons: {
	            fullsite: { ID: p.DivId + '_main',
	                text: 'Full Site',
	                href: 'Main.html',
	                rel: 'external',
	                dataIcon: 'home'
	            },
	            help: { ID: p.DivId + '_help',
	                text: 'Help',
	                dataIcon: 'info',
	                onClick: p.onHelpClick
	            }
	        }
	    };
	}

	if (isNullOrEmpty(pageDef.headerDef)) {
	    pageDef.headerDef = {
	        buttons: {
	            back: { ID: p.DivId + '_back',
	                text: 'Back',
	                cssClass: 'ui-btn-left',
	                dataDir: 'reverse',
	                dataIcon: 'arrow-l' }
	        }
	    };
	}

	var onlinePage = new CswMobilePageFactory(pageDef, $parent);
	var onlineHeader = onlinePage.mobileHeader;
	var onlineFooter = onlinePage.mobileFooter;
	var $content = onlinePage.$content;
    
    if (!isNullOrEmpty(searchJson)) {
	    for (var key in searchJson) {
	        if (!selected) selected = key;
	        values.push({ 'value': key, 'display': searchJson[key] });
	    }

	    var $select = $fieldCtn.CswSelect('init', {
	        ID: p.DivId + '_searchprop',
	        selected: selected,
	        cssclass: 'csw_search_select',
	        values: values
	    })
    	    .CswAttrXml({ 'data-native-menu': 'false' });

	    var $searchCtn = $('<div data-role="fieldcontain"></div>')
    	    .appendTo($searchContent);
	    $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: p.DivId + '_searchfor' })
    	    .CswAttrXml({
    	            'placeholder': 'Search',
    	            'data-placeholder': 'Search'
    	        });
	    $searchContent.CswLink('init', { type: 'button', ID: p.DivId + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
    	    .CswAttrXml({ 'data-role': 'button' })
    	    .unbind('click')
    	    .bind('click', function() {
    	        return startLoadingMsg(function() { onSearchSubmit(); });
    	    });
	    $searchContent.CswDiv('init', { ID: p.DivId + '_searchresults' });
	}
    
	function onSearchSubmit() {
		var searchprop = $('#' + p.DivId + '_searchprop').val();
		var searchfor = $('#' + p.DivId + '_searchfor').val();
		var $resultsDiv = $('#' + p.DivId + '_searchresults')
			.empty();
			
		if (!isNullOrEmpty(searchJson)) {
			var $searchResults = $resultsDiv.cswUL({id: p.DivId + '_searchresultslist', 'data-filter': false })
										.append($('<li data-role="list p.DivIder">Results</li>'));

			var hitcount = 0;
			for(var nodeKey in searchJson)
			{
				var node = searchJson[nodeKey];
				if (!isNullOrEmpty(node[searchprop])) {
					if (node[searchprop].toLowerCase().indexOf(searchfor.toLowerCase()) >= 0) {
						hitcount++;
						var nodeJson = { id: nodeKey, value: node};
						$searchResults.append(
//!HEY YOU!. This becomes var x = new CswMobilePageNodes()
//							_makeListItemFromJson($content, {
//								ParentId: DivId + '_searchresults',
//								DivId: DivId + '_searchresultslist',
//								HeaderText: 'Results',
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
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.mobileHeader = onlineHeader;
    this.mobileFooter = onlineFooter;
    this.$pageDiv = onlinePage.$pageDiv;

    this.onPageOpen = function() {
        this.$pageDiv.CswChangePage({ transition: 'slideup' });
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageSearch