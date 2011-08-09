/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../fieldtypes/CswMobilePropFactory.js" />
/// <reference path="../../CswString.js" />

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
    var id = CswMobilePage_Type.props.id;
    var title = CswMobilePage_Type.tabs.title;
    var viewId, level, nodeId, tabId, tabName, tabJson;
    var divSuffix = '_props';
    var ulSuffix = '_list';
    var $contentPage = $page.find('#' + id).find('div:jqmData(role="content")');
    var $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + id + divSuffix);
    var contentDivId;
    
    //ctor
    (function () {
        
        var p = {
	        level: 1,
	        ParentId: '',
            DivId: '', 
            viewId: mobileStorage.currentViewId(),
            tabId: mobileStorage.currentTabId(),
            tabName: '',
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

        contentDivId = id + divSuffix;
        
        if (!isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        nodeId = p.nodeId;
        tabId = p.tabId;
        tabName = p.tabName;
        tabJson = p.tabJson;
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
        $content = ensureContent($content, contentDivId);
    })(); //ctor
   
    function getContent(onSuccess, postSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function to execute after the list is built.</param>
        $content = ensureContent($content, contentDivId);
		if (!isNullOrEmpty(tabJson)) {
			refreshPropContent(onSuccess, postSuccess);
		} else {
			makeEmptyListView(null, $content, 'No Properties to Display');
		    stopLoadingMsg();
		}
    }
    
    function refreshPropContent(onSuccess, postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name,
            showLoading: false
        };
        
        var listView = new CswMobileListView(ulDef, $content, CswDomElementEvent.change);
        var propCount = 0;
        var nextTab;
        for (var propId in tabJson)
        {
            if(tabJson.hasOwnProperty(propId)) {
                var propJson = tabJson[propId];
                if (!isNullOrEmpty(propJson) && propId !== 'nexttab') {
                    var propName = propJson['prop_name'];
                    var ftDef = {
                        propId: propId,
                        propName: propName,
                        nodeId: nodeId,
                        tabId: tabId,
                        viewId: viewId
                    };
                    $.extend(ftDef, tabJson[propId]);
                    var prop = new CswMobilePropsFactory(ftDef);
                    
                    var $li = listView.addListItemHtml(propId, prop.$label);
                    $li.append(prop.$content);
                    prop.applyFieldTypeLogicToContent($li);
                    
                    var onChange = makeDelegate(onPropertyChange, {
                        prop: prop,
                        control: $li,
                        onSuccess: prop.applyFieldTypeLogicToContent
                    });
                    
                    $li.bind('change', onChange);

                } else {
                    nextTab = propJson;
                }
                propCount++;
            }
        }
        if (!isNullOrEmpty(nextTab)) {
            //remember to wire up click event
            listView.addListItemLink(makeSafeId({ ID: nextTab }), nextTab);    
        }
        if (propCount === 0) {
            makeEmptyListView(listView, $content, 'No Properties to Display');
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
    
	function onPropertyChange(options) {
	    var o = {
            elementId: '',
	        prop: '',
	        control: '',
	        onSuccess: ''
	    };
		if(options) {
		    $.extend(o, options);
		}
		var nodeJson = mobileStorage.fetchCachedNodeJson(nodeId);
	    var propId = o.prop.propId;
	    var elementId = new CswString(o.control.CswAttrDom('id'));
	    
		if (!isNullOrEmpty(nodeJson)) {
			mobileStorage.addUnsyncedChange();

		    var propJson;
		    if (nodeJson.hasOwnProperty('subitems') &&
		        nodeJson.subitems.hasOwnProperty(tabName) &&
    		    nodeJson.subitems[tabName].hasOwnProperty(propId)) {
		        propJson = nodeJson.subitems[tabName][propId];
		    }
		    
			if (!isNullOrEmpty(propJson)) {
			    propJson = o.prop.updatePropValue(propJson, elementId, 'new Value here');
			    nodeJson.subitems[tabName][propId] = propJson;
                mobileStorage.updateStoredNodeJson(nodeId, nodeJson, '1');
			} else { 
				errorHandler('Could not find a prop to update');
			}
		}
	    if (isFunction(o.onSuccess)) {
	        o.onSuccess(o.control);
	    }
	} // onPropertyChange()
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.contentDivId = contentDivId;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageProps