/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../fieldtypes/CswMobilePropFactory.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../main/tools/CswTools.js" />
/// <reference path="../../main/tools/CswString.js" />
/// <reference path="../../main/tools/CswAttr.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />

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
    
    var pageDef = { },
        id = CswMobilePage_Type.props.id,
        title = CswMobilePage_Type.tabs.title,
        divSuffix = '_props',
        ulSuffix = '_list',
        $contentPage = $page.find('#' + id).find('div:jqmData(role="content")'),
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + id + divSuffix),
        contentDivId, viewId, level, nodeId, tabId, tabName, tabJson;
    
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
                onSearchClick: null, //function () {}
                onListItemSelect: null, //function () {}
                onPropChange: null //function () {}s
            },
            buttons = { };
        
        if (propsDef) $.extend(p, propsDef);
        if (false === isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }

        contentDivId = id + divSuffix;
        
        if (false === isNullOrEmpty(p.title)) {
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
        
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.refresh.name] = p.onRefreshClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        buttons[CswMobileHeaderButtons.back.name] = '';
        buttons[CswMobileHeaderButtons.search.name] = p.onSearchClick;

        pageDef = makeMenuButtonDef(p, id, buttons, mobileStorage);
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
            },
            listView = new CswMobileListView(ulDef, $content, CswDomElementEvent.change),
            propCount = 0,
            nextTab = '', 
            onClick = null,
            propId, propJson, propName, ftDef, prop, $li, onChange, cachedJson, newTabJson, opts;
        
        for (propId in tabJson) {
            if (contains(tabJson, propId)) {
                propJson = tabJson[propId];
                if (false == isNullOrEmpty(propJson) && propId !== 'nexttab') {
                    propName = propJson['prop_name'];
                    ftDef = {
                        propId: propId,
                        propName: propName,
                        nodeId: nodeId,
                        tabId: tabId,
                        viewId: viewId
                    };
                    $.extend(ftDef, tabJson[propId]);
                    prop = new CswMobilePropsFactory(ftDef);
                    
                    $li = listView.addListItemHtml(propId, prop.$label);
                    $li.append(prop.$content);
                    prop.applyFieldTypeLogicToContent($li);
                    
                    onChange = makeEventDelegate(onPropertyChange, {
                        prop: prop,
                        control: $li,
                        onSuccess: prop.applyFieldTypeLogicToContent,
                        onUpdate: pageDef.onPropChange
                    });
                    
                    $li.bind('change', onChange);

                } else {
                    nextTab = propJson;
                }
                propCount++;
            }
        }
        if (false === isNullOrEmpty(nextTab)) {
            cachedJson = mobileStorage.fetchCachedNodeJson(nodeId);
            if (contains(cachedJson,'subitems') &&
                contains(cachedJson.subitems, nextTab)) {
                
                newTabJson = cachedJson.subitems[nextTab];
                opts = {
                    ParentId: id,
                    DivId: tabId,
                    viewId: viewId,
                    nodeId: nodeId,
                    tabId: tabId,
                    tabName: nextTab,
                    tabJson: newTabJson,
                    level: 3,
                    title: tabName,
                    onHelpClick: pageDef.onHelpClick,
                    onOnlineClick: pageDef.onOnlineClick,
                    onRefreshClick: pageDef.onRefreshClick,
                    mobileStorage: mobileStorage
                };
                onClick = function() { pageDef.onListItemSelect(opts); }; //makeDelegate(pageDef.onListItemSelect, opts);    
            }
            listView.addListItemLink(makeSafeId({ prefix: id, ID: nextTab }), nextTab, onClick);    
        }
        if (propCount === 0) {
            makeEmptyListView(listView, $content, 'No Properties to Display');
        }
        if (false === mobileStorage.stayOffline()) {
            toggleOnline(mobileStorage);
        }
        if (isFunction(onSuccess)) {
            onSuccess($content);
        }
        if (isFunction(postSuccess)) {
            postSuccess();
        }
    }
    
    function onPropertyChange(eventObj,options) {
        var o = {
                elementId: '',
                prop: '',
                control: '',
                onSuccess: '',
                onUpdate: ''
            },
            nodeJson = mobileStorage.fetchCachedNodeJson(nodeId),
            propId = o.prop.propId,
            elementId = new CswString(eventObj.target.id),
            value = eventObj.target.value,
            propJson = null;
        
        if(options) {
            $.extend(o, options);
        }
        
        if (false === isNullOrEmpty(nodeJson)) {
            mobileStorage.addUnsyncedChange();

            if (nodeJson.hasOwnProperty('subitems') &&
                nodeJson.subitems.hasOwnProperty(tabName) &&
                nodeJson.subitems[tabName].hasOwnProperty(propId)) {
                propJson = nodeJson.subitems[tabName][propId];
            }
            if (false === isNullOrEmpty(propJson)) {
                propJson = o.prop.updatePropValue(propJson, elementId, value);
                nodeJson.subitems[tabName][propId] = propJson;
                mobileStorage.updateStoredNodeJson(nodeId, nodeJson, '1');
            } else { 
                errorHandler('Could not find a prop to update');
            }
        }
        if (isFunction(o.onSuccess)) {
            o.onSuccess(o.control);
        }
        if (isFunction(o.onUpdate)) {
            o.onUpdate();
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