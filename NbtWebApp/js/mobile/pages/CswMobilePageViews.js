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
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobilePageViews

function CswMobilePageViews(viewsDef,$parent,mobileStorage) {
	/// <summary>
	///   Online Page class. Responsible for generating a Mobile login page.
	/// </summary>
    /// <param name="viewsDef" type="Object">Login definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageViews">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
    var _viewsPage, _$content, _viewsHeader, _viewsFooter, _json, _divId, _headerText;
    
    //ctor
    (function() {
        if (isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }

        var p = {
            level: -1,
            ParentId: '',
            DivId: 'viewsdiv',       // required
            HeaderText: 'Views',
            theme: 'b',
            $content: '',
            onHelpClick: function() {},
            onOnlineClick: function() {},
            onRefreshClick: function() {},
            //force compatibility with soon to be legacy code
            HideBackButton: true,
            HideRefreshButton: true,
            HideSearchButton: true,
            PageType: "view",
            SessionId: mobileStorage.sessionid(),
            json: { },
            parentlevel: -1

        };
        if (viewsDef) $.extend(p, viewsDef);

        var pageDef = p;
        delete pageDef.onHelpClick;
        delete pageDef.onOnlineClick;
        delete pageDef.onRefreshClick;

        if (isNullOrEmpty(pageDef.footerDef)) {
            pageDef.footerDef = { };
            pageDef.footerDef.buttons = { };
            pageDef.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, p.DivId, p.onOnlineClick, mobileStorage);
            pageDef.footerDef.buttons.refresh = makeFooterButtonDef(CswMobileFooterButtons.refresh, p.DivId, p.onRefreshClick);
            pageDef.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, p.DivId);
            pageDef.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, p.DivId, p.onHelpClick);
        }

        if (isNullOrEmpty(pageDef.headerDef)) {
            pageDef.headerDef = { };
            pageDef.headerDef.buttons = { };
            pageDef.headerDef.buttons.back = makeHeaderButtonDef(CswMobileHeaderButtons.back, p.DivId);
        }

        _viewsPage = new CswMobilePageFactory(pageDef, $parent);
        _viewsHeader = _viewsPage.mobileHeader;
        _viewsFooter = _viewsPage.mobileFooter;
        _$content = _viewsPage.$content;
        _divId = p.DivId;
        _headerText = p.HeaderText;

        var now = new Date();
        if (!mobileStorage.amOnline() || 
            ( now - mobileStorage.lastSyncSuccess() < 300000 ) ) //it's been less than 5 minutes since the last sync
        {
            _json = mobileStorage.fetchCachedViewJson(p.DivId);
            //we don't want to wait here
            setTimeout(function() { refreshViewContent(); }, 500);
        } else {
            refreshViewJson();
        }
    })();
    
    function refreshViewJson() {
        ///<summary>Fetches the current views list from the web server and rebuilds the list.</summary>
		var getViewsUrl = '/NbtWebApp/wsNBT.asmx/GetViewsList';
		
		var jsonData = {
			SessionId: mobileStorage.sessionid(),
			ParentId: 'logindiv',
			ForMobile: true
		};

        CswAjaxJSON({
				//async: false,   // required so that the link will wait for the content before navigating
				formobile: true,
				url: getViewsUrl,
				data: jsonData,
				onloginfail: function(text) { onLoginFail(text, mobileStorage); },
				success: function(data) {
					setOnline(mobileStorage);
					_json = data;
					mobileStorage.storeViewJson(_divId, _headerText, _json, 0);

				    refreshViewContent(_json);
				},
				error: function() {
					onError();
				}
			});
    }
    
    function refreshViewContent(viewJson) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        $content.empty();
        
        var ulDef = {
            ID: _divId + '_views',
            cssclass: 'csw_listview',
            onClick: function () {}
        };
        var listView = new CswMobileListView(ulDef, _$content);
        	
		for(var key in viewJson)
		{
		    var id = key;
		    var text = viewJson[key];
		    function onClick() {
		        //we know the next level is going to be nodes. 
		        //onClick should trigger new CswMobilePageNodes
		    }
		    listView.addListItemLink(id, text, onClick);
		}
			
		if(!mobileStorage.stayOffline()) {
			toggleOnline(mobileStorage);
		}
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = _$content;
    this.mobileHeader = _viewsHeader;
    this.mobileFooter = _viewsFooter;
    this.$pageDiv = _viewsPage.$pageDiv;

    this.onPageOpen = function() {
        this.$pageDiv.CswChangePage({ transition: 'slideup' });
    };

    this.refreshViewContent = refreshViewContent;
    this.refreshViewJson = refreshViewJson;
    //#endregion public, priveleged
}

//#endregion CswMobilePageViews