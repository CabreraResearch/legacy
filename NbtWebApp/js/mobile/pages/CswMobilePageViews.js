/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
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
    var id, title, contentDivId, $contentPage, $content, forceRefresh,
        divSuffix = '_views',
        ulSuffix = '_list';
    
    //ctor
    (function () {
        var p = {
            parentlevel: -1,
            level: -1,
            ParentId: '',
            DivId: '',
            title: '',
            theme: 'b',
            PageType: CswMobilePage_Type.views
        };
        if (viewsDef) {
            $.extend(p, viewsDef);
        }
        forceRefresh = mobileStorage.forceContentRefresh();
        mobileStorage.forceContentRefresh(false);

        id = tryParseString(p.DivId, CswMobilePage_Type.views.id);
        contentDivId = id + divSuffix;
        title = tryParseString(p.title, CswMobilePage_Type.views.title);
        $contentPage = $page.find('div:jqmData(role="content")');
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);

        $content = ensureContent($content, contentDivId);
    })();   //ctor
    
    function getContent(onSuccess) {
        if (isTimeToRefresh(mobileStorage) || forceRefresh) { 
            refreshViewJson(onSuccess);
        } else { //it's been less than 5 minutes since the last sync or we're offline
            refreshViewContent('', onSuccess); 
        } 
    }
    
    function refreshViewJson(onSuccess) {
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

                    refreshViewContent(data,onSuccess);
                },
                error: function() {
                    onError();
                }
            });
    }
    
    function refreshViewContent(viewJson,onSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef, listView, viewId, viewName, opts, onClick,
            viewCount = 0;
        if (isNullOrEmpty(viewJson)) {
            viewJson = mobileStorage.fetchStoredViews();
        }
        if (isNullOrEmpty(viewJson)) {
            refreshViewJson(onSuccess);
        } else {
            $content = ensureContent($content, contentDivId);

            ulDef = {
                ID: id + ulSuffix,
                cssclass: CswMobileCssClasses.listview.name
            };
            listView = new CswMobileListView(ulDef, $content);

            for (viewId in viewJson) {
                if (viewJson.hasOwnProperty(viewId)) {
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
                    viewCount++;
                }
            }
            if (viewCount === 0) {
                listView.addListItemHtml('no_results', 'No Mobile Views to Display');
            }

            if (false === mobileStorage.stayOffline()) {
                toggleOnline(mobileStorage);
            }
            doSuccess(onSuccess, $content);
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