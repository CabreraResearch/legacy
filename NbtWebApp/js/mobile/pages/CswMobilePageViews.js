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

function CswMobilePageViews(viewsDef,$page,mobileStorage) {
	/// <summary>
	///   Views Page class. Responsible for generating a Mobile views page.
	/// </summary>
    /// <param name="viewsDef" type="Object">Views definitional data.</param>
	/// <param name="$page" type="jQuery">Mobile page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageViews">Instance of itself. Must instance with 'new' keyword.</returns>
    
	//#region private
    var pageDef = { };
    var id = CswMobilePage_Type.views.id;
    var title = CswMobilePage_Type.views.title;
    var divSuffix = '_views';
    var ulSuffix = '_list';
    function $contentPage() { return $page.find('div:jqmData(role="content")'); };
    var $content = (isNullOrEmpty($contentPage()) || $contentPage().length === 0) ? null : $contentPage().find('#' + id + divSuffix);
    var contentDivId;
    
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
            onHelpClick: null, // function () {}
            onOnlineClick: null, // function () {}
            onRefreshClick: null, // function () {}
            onListItemSelect: null, // function (opts) {}
            PageType: CswMobilePage_Type.views
        };
        if (viewsDef) $.extend(p, viewsDef);

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
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.refresh.name] = p.onRefreshClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;

        pageDef = p = makeMenuButtonDef(p, id, buttons, mobileStorage);
        
        $content = ensureContent($content, contentDivId);
    })(); //ctor
    
    function getContent(onSuccess,postSuccess) {
        var now = new Date();
        var lastSync = new Date(mobileStorage.lastSyncTime);
        if (!mobileStorage.amOnline() || 
            ( now.getTime() - lastSync.getTime() < 300000 ) ) //it's been less than 5 minutes since the last sync
        {
            refreshViewContent('', onSuccess,postSuccess); 
        } else {
            refreshViewJson(onSuccess,postSuccess);
        }
    }
    
    function refreshViewJson(onSuccess,postSuccess) {
        ///<summary>Fetches the current views list from the web server and rebuilds the list.</summary>
		var getViewsUrl = '/NbtWebApp/wsNBT.asmx/GetViewsList';
		
		var jsonData = {
			SessionId: mobileStorage.sessionid(),
			ParentId: 'logindiv',
			ForMobile: true
		};

        CswAjaxJson({
				//async: false,   // required so that the link will wait for the content before navigating
				formobile: true,
				url: getViewsUrl,
				data: jsonData,
				onloginfail: function(text) { onLoginFail(text, mobileStorage); },
				success: function(data) {
					setOnline(mobileStorage);

					mobileStorage.storeViewJson(id, title, data, 0);

				    refreshViewContent(data,onSuccess,postSuccess);
				},
				error: function() {
					onError();
				}
			});
    }
    
    function refreshViewContent(viewJson,onSuccess,postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        if (isNullOrEmpty(viewJson)) {
            viewJson = mobileStorage.fetchCachedViewJson(id);
        }
        
        $content = ensureContent($content, contentDivId);
        
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        var listView = new CswMobileListView(ulDef, $content);

        var viewCount = 0;
		for(var viewId in viewJson)
		{
		    if(viewJson.hasOwnProperty(viewId)) {
		        var viewName = viewJson[viewId];
		        var opts = {
		            ParentId: id,
		            DivId: viewId,
		            viewId: viewId,
		            level: 1,
		            title: viewName,
		            onHelpClick: pageDef.onHelpClick,
		            onOnlineClick: pageDef.onOnlineClick,
		            onRefreshClick: pageDef.onRefreshClick,
		            mobileStorage: mobileStorage
		        };

		        var onClick = makeDelegate(pageDef.onListItemSelect,opts);
                listView.addListItemLink(viewId, viewName, onClick);
		        viewCount++;
		    }
		}
        if (viewCount === 0) {
            listView.addListItemLink('no_results', 'No Mobile Views to Display');
        }
        
		if (!mobileStorage.stayOffline()) {
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
    this.title = title;
    this.getContent = getContent;
    //#endregion public, priveleged
}

//#endregion CswMobilePageViews