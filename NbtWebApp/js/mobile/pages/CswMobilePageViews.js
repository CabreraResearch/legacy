/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
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

function CswMobilePageViews(viewsDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Views Page class. Responsible for generating a Mobile views page.
    /// </summary>
    /// <param name="viewsDef" type="Object">Views definitional data.</param>
    /// <param name="$page" type="jQuery">Mobile page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageViews">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //#region private
    var pageDef = { };
    var id, title, contentDivId, $content, forceRefresh,
        divSuffix = '_views',
        ulSuffix = '_list';
    
    //ctor
    (function () {
        pageDef = {
            parentlevel: -1,
            level: -1,
            ParentId: '',
            buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help],
            DivId: '',
            title: '',
            theme: 'b',
            PageType: CswMobilePage_Type.views
        };
        if (viewsDef) {
            $.extend(pageDef, viewsDef);
        }
        forceRefresh = mobileStorage.forceContentRefresh();
        mobileStorage.forceContentRefresh(false);

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.views.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.views.title);
        $content = ensureContent($contentRole, contentDivId);
    })();   //ctor
    
    function getContent(onSuccess) {
        $content = ensureContent($contentRole, contentDivId);
        if (isTimeToRefresh(mobileStorage) || forceRefresh) { 
            refreshViewJson(onSuccess);
        } else { //it's been less than 5 minutes since the last sync or we're offline
            refreshViewContent('', onSuccess); 
        } 
    }
    
    function refreshViewJson(onSuccess, runOnce) {
        ///<summary>Fetches the current views list from the web server and rebuilds the list.</summary>
        var getViewsUrl = '/NbtWebApp/wsNBT.asmx/GetViewsList',
            ret = {};
        
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
                onloginfail: function(text) {
                    onLoginFail(text, mobileStorage);
                },
                success: function(data) {
                    setOnline(mobileStorage);

                    mobileStorage.storeViewJson(id, title, data, 0);

                    refreshViewContent(data,onSuccess,runOnce);
                },
                error: function() {
                    onError();
                }
            });
    }
    
    function refreshViewContent(viewJson,onSuccess,runOnce) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef, listView, viewId, viewName, opts, onClick,
            viewCount = 0;
        if (isNullOrEmpty(viewJson)) {
            viewJson = mobileStorage.fetchStoredViews();
        }
        if (isNullOrEmpty(viewJson)) {
            if (true === runOnce) {
                viewJson = {};
            } else {
                 return refreshViewJson(onSuccess, true);
            }
        } 
        ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        listView = new CswMobileListView(ulDef, $content);

        for (viewId in viewJson) {
            if (viewId !== -1 && 
                contains(viewJson, viewId)) {
                viewName = viewJson[viewId];
                opts = {
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

                onClick = makeDelegate(pageDef.onListItemSelect, opts);
                listView.addListItemLink(viewId, viewName, onClick);
                viewCount += 1;
            }
        }
        if (viewCount === 0) {
            listView.addListItemHtml('no_results', 'No Mobile Views to Display');
        }

        if (false === mobileStorage.stayOffline()) {
            toggleOnline(mobileStorage);
        }
        $contentRole.append($content);
        doSuccess(onSuccess, $contentRole, { 1: 1 }, 1, $content);
        
    }
    
    //#endregion private
    
    //#region public, priveleged
    return {
        $pageDiv: $parent,
        $contentRole: $contentRole,
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        title: title,
        getContent: getContent
    };
    //#endregion public, priveleged
}

//#endregion CswMobilePageViews