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
	///   Views Page class. Responsible for generating a Mobile views page.
	/// </summary>
    /// <param name="viewsDef" type="Object">Views definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageViews">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
    var $content = '';
    var pageDef = { };
    var pageJson = { };
    var id = CswMobilePage_Type.views.id;
    var title = CswMobilePage_Type.views.title;
    
    //ctor
    (function() {
        if (isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }

        var p = {
            parentlevel: -1,
            level: -1,
            ParentId: '',
            DivId: '', 
            title: '',
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            theme: 'b',
            $content: '',
            onHelpClick: function() {},
            onOnlineClick: function() {},
            onRefreshClick: function() {},
            PageType: CswMobilePage_Type.views
        };
        if (viewsDef) $.extend(p, viewsDef);

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

        if (isNullOrEmpty(pageDef.footerDef)) {
            pageDef.footerDef = { buttons: { } };
        }
        if( isNullOrEmpty(pageDef.footerDef.buttons)) {
            pageDef.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, id, p.onOnlineClick, mobileStorage);
            pageDef.footerDef.buttons.refresh = makeFooterButtonDef(CswMobileFooterButtons.refresh, id, p.onRefreshClick);
            pageDef.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
            pageDef.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, p.onHelpClick);
        }

        if (isNullOrEmpty(pageDef.headerDef)) {
            pageDef.headerDef = { buttons: { } };
        }
        if( isNullOrEmpty(pageDef.headerDef.buttons)) {
            pageDef.headerDef.buttons.back = makeHeaderButtonDef(CswMobileHeaderButtons.back, id);
        }
        
        pageDef = p;
        getContent();
    })(); //ctor
    
    function getContent() {
        var now = new Date();
        if (!mobileStorage.amOnline() || 
            ( now - mobileStorage.lastSyncSuccess() < 300000 ) ) //it's been less than 5 minutes since the last sync
        {
            pageJson = mobileStorage.fetchCachedViewJson(id);
            refreshViewContent(); 
        } else {
            refreshViewJson();
        }
        return $content;
    }
    
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
					pageJson = data;
					mobileStorage.storeViewJson(_divId, _headerText, pageJson, 0);

				    refreshViewContent(pageJson);
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
            ID: id + '_views',
            cssclass: 'csw_listview',
            onClick: function () {}
        };
        var listView = new CswMobileListView(ulDef, $content);
        	
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

    this.$content = $content;
    this.pageDef = pageDef;

    this.refreshViewContent = refreshViewContent;
    this.refreshViewJson = refreshViewJson;
    this.getContent = getContent;
    //#endregion public, priveleged
}

//#endregion CswMobilePageViews