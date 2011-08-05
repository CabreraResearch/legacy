/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobilePageProps

function CswMobilePageProps(propsDef, $page, mobileStorage) {
	/// <summary>
	///   Props Page class. Responsible for generating a Mobile props page.
	/// </summary>
    /// <param name="propsDef" type="Object">Props definitional data.</param>
	/// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageProps">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    var pageDef = { };
    var id = CswMobilePage_Type.nodes.id;
    var title = CswMobilePage_Type.nodes.title;
    var viewId, level, nodeId;
    var divSuffix = '_props';
    var ulSuffix = '_list';
    var $contentPage = $page.find('#' + id).find('div:jqmData(role="content")');
    var $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + id + divSuffix);

    //ctor
    (function () {
        
        var p = {
	        level: 1,
	        ParentId: '',
            DivId: '', 
            viewId: mobileStorage.currentViewId(),
            nodeId: mobileStorage.currentNodeId(),
	        title: '',
	        theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
	        onHelpClick: null, //function () {},
            onOnlineClick: null, //function () {},
            onRefreshClick: null, //function () {},
            onSearchClick: null //function () {}
        };
        if (propsDef) $.extend(p, propsDef);

        if (!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }
        if (!isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        nodeId = p.nodeId;
        viewId = p.viewId;
        level = tryParseNumber(p.level, 2);
        
        var buttons = { };
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.refresh.name] = p.onRefreshClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        buttons[CswMobileHeaderButtons.back.name] = '';
        buttons[CswMobileHeaderButtons.search.name] = p.onSearchClick;

        pageDef = p = makeMenuButtonDef(p, id, buttons, mobileStorage);
        ensureContent();
    })(); //ctor
    
    function ensureContent() {
        if (isNullOrEmpty($content) || $content.length === 0) {
            $content = $('<div id="' + id + divSuffix + '"></div>');
        } else {
            $content.empty();
        }
    }
    
    function getContent(onSuccess, postSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function to execute after the list is built.</param>
        ensureContent();
        var cachedJson = mobileStorage.fetchCachedNodeJson(nodeId);
		if( !isNullOrEmpty(cachedJson) && 
    		cachedJson.hasOwnProperty('subitems') &&
		    !isNullOrEmpty(cachedJson['subitems'])) 
		{
			var propJson = cachedJson['subitems'];
			refreshPropContent(propJson, onSuccess, postSuccess);
		} else {
			makeEmptyListView(null, $content, 'No Properties to Display');
		    stopLoadingMsg();
		}
    }
    
    function refreshPropContent(propJson, onSuccess, postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        
        var listView = new CswMobileListView(ulDef, $content);
        var propCount = 0;
        if (!isNullOrEmpty(propJson)) {
            for (var tabName in propJson)
            {
                if(propJson.hasOwnProperty(tabName)) {
					var tabId = makeSafeId({prefix: tabName, ID: nodeId }); 
					listView.addListItemLink(tabId, tabName);
                    propCount++;
                }
            }
        } 
        if(propCount === 0) {
            makeEmptyListView(listView, , 'No Properties to Display');
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
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageProps