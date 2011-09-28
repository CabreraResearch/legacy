/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

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
    var id = CswMobilePage_Type.views.id,
        title = CswMobilePage_Type.views.title,
        divSuffix = '_views',
        ulSuffix = '_list',
        $contentPage, $content, contentDivId;
    
    //ctor
    (function() {
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
        $contentPage = $page.find('div:jqmData(role="content")');
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);
        
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

        pageDef = makeMenuButtonDef(p, id, buttons, mobileStorage);
        
        $content = ensureContent($content, contentDivId);
    })(); //ctor
    
    function getContent(onSuccess,postSuccess) {
        if (isTimeToRefresh(mobileStorage)) { 
            refreshViewJson(onSuccess,postSuccess);
        } else { //it's been less than 5 minutes since the last sync or we're offline
             refreshViewContent('', onSuccess,postSuccess); 
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
            viewJson = mobileStorage.fetchStoredViews();
        }
        
        $content = ensureContent($content, contentDivId);
        
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        var listView = new CswMobileListView(ulDef, $content);

        var viewCount = 0;
        for(var viewId in viewJson) {
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