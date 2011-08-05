/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region CswMobilePageNodes

function CswMobilePageNodes(nodesDef, $page, mobileStorage) {
	/// <summary>
	///   Nodes Page class. Responsible for generating a Mobile nodes page.
	/// </summary>
    /// <param name="nodesDef" type="Object">Nodes definitional data.</param>
	/// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageNodes">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    var pageDef = { };
    var id = CswMobilePage_Type.nodes.id;
    var title = CswMobilePage_Type.nodes.title;
    var viewId, level;
    var divSuffix = '_nodes';
    var ulSuffix = '_list';
    var $contentPage = $page.find('#' + id).find('div:jqmData(role="content")');
    var $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + id + divSuffix);

    //ctor
    (function () {
        
        var p = {
	        level: 1,
	        ParentId: '',
            DivId: '', 
	        title: '',
            viewId: mobileStorage.currentViewId(),
	        theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
	        onHelpClick: null, //function () {},
            onOnlineClick: null, //function () {},
            onRefreshClick: null, //function () {},
            onSearchClick: null //function () {}
        };
        if (nodesDef) $.extend(p, nodesDef);

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
        
        viewId = p.viewId;
        level = tryParseNumber(p.level, 1);
        
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
        //var now = new Date();
        //var lastSync = new Date(mobileStorage.lastSyncTime);
        //( now.getTime() - lastSync.getTime() < 300000 ) ) //it's been less than 5 minutes since the last sync
        ensureContent();
        var cachedJson = mobileStorage.fetchCachedViewJson(viewId);

		if (!isNullOrEmpty(cachedJson)) {
			refreshNodeContent(cachedJson, onSuccess, postSuccess);
		} else if (mobileStorage.amOnline()) {
			refreshNodeJson(onSuccess, postSuccess);
		} else {
		    makeEmptyListView(null, $content, 'No Results');
			stopLoadingMsg();
		}
    }
    
    function refreshNodeJson(onSuccess, postSuccess) {
        ///<summary>Fetches the nodes from the selected view from the web server and rebuilds the list.</summary>
		var getView = '/NbtWebApp/wsNBT.asmx/GetView';
		
		var jsonData = {
			SessionId: mobileStorage.sessionid(),
			ParentId: viewId,
			ForMobile: true
		};

        CswAjaxJSON({
				formobile: true,
				url: getView,
				data: jsonData,
				onloginfail: function(text) { onLoginFail(text, mobileStorage); },
				success: function(data) {
					setOnline(mobileStorage);

					var searchJson = data['searches'];
				    var nodesJson = data['nodes'];
					mobileStorage.storeViewJson(id, title, nodesJson, level, searchJson);
			    
				    refreshNodeContent(nodesJson,onSuccess,postSuccess);
				},
				error: function() {
					onError();
				}
			});
    }
    
    function refreshNodeContent(viewJson, onSuccess, postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        if (isNullOrEmpty(viewJson)) {
            viewJson = mobileStorage.fetchCachedViewJson(id);
        }
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name
        };
        
        var listView = new CswMobileListView(ulDef, $content);
        var nodeCount = 0;
        if (!isNullOrEmpty(viewJson)) {
            for (var nodeKey in viewJson)
            {
                if(viewJson.hasOwnProperty(nodeKey)) {
                    var nodePk = nodeKey.split('_');
                    if(nodePk.hasOwnProperty(1)) {
                        var nodeId = nodeKey[1];
                    }
                    if(Int32MinVal === nodeId || 'No Results' === viewJson[nodeKey]) {
                        makeEmptyListView(listView, null, 'No Results');
                    } else {

                        var nodeJson = { ID: nodeKey, value: viewJson[nodeKey] };
                        var nodeOc = makeOcContent(nodeJson);

                        var opts = {
		                    ParentId: id,
		                    DivId: nodeKey,
		                    viewId: viewId,
                            nodeId: mobileStorage.currentNodeId(nodeKey),
		                    level: 2,
		                    title: viewJson[nodeKey]['node_name'],
		                    onHelpClick: pageDef.onHelpClick,
		                    onOnlineClick: pageDef.onOnlineClick,
		                    onRefreshClick: pageDef.onRefreshClick,
		                    mobileStorage: mobileStorage
		                };

		                var onClick = makeDelegate(pageDef.onListItemSelect,opts);

                        if (nodeOc.isLink) {
                            listView.addListItemLinkHtml(nodeKey, nodeOc.$html, onClick);
                        } else {
                            listView.addListItemHtml(nodeKey, nodeOc.$html, onClick);
                        }
                        nodeCount++;
                    }
                }
            }
        } 
        if (nodeCount === 0) {
            makeEmptyListView(listView, null, 'No Results');
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
    
    function makeOcContent(json) {
		var ret = {
		    isLink: true,
		    $html: ''
		};
		var html = '';
		var nodeId = makeSafeId({ ID: json['id'] });
		var nodeSpecies = json['value']['nodespecies'];
		var nodeName = json['value']['node_name'];
		var icon = '';
		if (!isNullOrEmpty(json['value']['iconfilename'])) {
			icon = 'images/icons/' + json['value']['iconfilename'];
		}
		var objectClass = json['value']['objectclass'];

		if (nodeSpecies !== 'More')
		{
			if (!isNullOrEmpty(icon)) {
			    html += '<img src="' + icon + '" class="ui-li-icon"/>';
			}
		    
		    switch (objectClass) {
			case "InspectionDesignClass":
				var dueDate = tryParseString(json['value']['duedate'],'' );
				var location = tryParseString(json['value']['location'],'' );
				var mountPoint = tryParseString(json['value']['target'],'' );
				var status = tryParseString(json['value']['status'],'' );

				html += '<h2>' + nodeName + '</h2>';
				html += '<p>' + location + '</p>';
				html += '<p>' + mountPoint + '</p>';
				html += '<p>';
				if (!isNullOrEmpty(status)) html += status + ', ';
				html += 'Due: ' + dueDate + '</p>';
				break;
			}
		} //if( nodeSpecies !== 'More' )
		else {
			html += '<h2 id="' + nodeId + '">' + nodeName + '</h2>';
		    ret.isLink = false;
		}
        ret.$html = $(html);
			
		return ret;
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

//#endregion CswMobilePageNodes