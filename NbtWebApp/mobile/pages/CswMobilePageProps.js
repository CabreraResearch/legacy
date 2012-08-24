/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
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

function CswMobilePageProps(propsDef, $parent, mobileStorage, $contentRole) {
    /// <summary>
    ///   Props Page class. Responsible for generating a Mobile props page.
    /// </summary>
    /// <param name="propsDef" type="Object">Props definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="CswMobilePageProps">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private
    var pageDef = { };
    var id, title, contentDivId, $content, viewId, level, nodeId, tabId, tabName, tabJson,
        divSuffix = '_props',
        ulSuffix = '_list';
    
    //ctor
    (function () {
        pageDef = {
            level: 1,
            ParentId: '',
            DivId: '',
            buttons: [CswMobileFooterButtons.online, CswMobileFooterButtons.fullsite, CswMobileFooterButtons.refresh, CswMobileFooterButtons.help, CswMobileHeaderButtons.back, CswMobileHeaderButtons.search],
            viewId: mobileStorage.currentViewId(),
            tabId: mobileStorage.currentTabId(),
            tabName: '',
            nodeId: mobileStorage.currentNodeId(),
            title: '',
            theme: CswMobileGlobal_Config.theme,
            onListItemSelect: null, //function () {}
            onPropChange: null //function () {}
        };
        if (propsDef) {
            $.extend(pageDef, propsDef);
        }

        var cachedJson, nodeJson;

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.props.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.props.title);
        $content = ensureContent($contentRole, contentDivId);

        nodeId = pageDef.nodeId;
        tabId = mobileStorage.currentTabId(pageDef.tabId);
        tabName = pageDef.tabName;

        cachedJson = mobileStorage.fetchCachedNodeJson(nodeId);
        if (false === isNullOrEmpty(cachedJson) &&
            contains(cachedJson, 'tabs') &&
            false === isNullOrEmpty(cachedJson.tabs)) {
            nodeJson = cachedJson.tabs;
            tabJson = nodeJson[tabName];

            viewId = pageDef.viewId;
            level = tryParseNumber(pageDef.level, 2);
        } else {
            throw new Error('Cannot create a property pages without Tab content', tabId);
        }
    })();    //ctor
   
    function getContent(onSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function or Array of functions to execute after the list is built.</param>
        $content = ensureContent($contentRole, contentDivId);
        if (false === isNullOrEmpty(tabJson)) {
            refreshPropContent(onSuccess);
        } else {
            makeEmptyListView(null, $content, 'No Properties to Display');
            stopLoadingMsg();
        }
    }
    
    function refreshPropContent(onSuccess) {
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
            propId, propJson, propName, ftDef, prop, $li, onChange, cachedJson, newTabJson, opts, nextTabId;
        
        for (propId in tabJson) {
            if (contains(tabJson, propId)) {
                propJson = tabJson[propId];
                if (false === isNullOrEmpty(propJson) && propId !== 'nexttab' && propId !== 'currenttab') {
                    propName = propJson.prop_name;
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
                        onSuccess: [ prop.applyFieldTypeLogicToContent, pageDef.onPropChange ]
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
            if (contains(cachedJson,'tabs') &&
                contains(cachedJson.tabs, nextTab)) {
                
                newTabJson = cachedJson.tabs[nextTab];
                nextTabId = mobileStorage.currentTabId(makeSafeId({ID: nextTab, suffix: nodeId}));
                opts = {
                    ParentId: id,
                    DivId: nextTabId,
                    viewId: viewId,
                    nodeId: nodeId,
                    tabId: nextTabId,
                    tabName: nextTab,
                    tabJson: newTabJson,
                    level: level,
                    title: nextTab,
                    onHelpClick: pageDef.onHelpClick,
                    onOnlineClick: pageDef.onOnlineClick,
                    onRefreshClick: pageDef.onRefreshClick,
                    mobileStorage: mobileStorage
                };
                onClick = makeDelegate(pageDef.onListItemSelect, opts); 
            }
            listView.addListItemLink(makeSafeId({ prefix: id, ID: nextTab }), nextTab, { event: onClick, name: CswDomElementEvent.click.name });    
        }
        if (propCount === 0) {
            makeEmptyListView(listView, $content, 'No Properties to Display');
        }
        if (false === mobileStorage.stayOffline()) {
            toggleOnline(mobileStorage);
        }
        $contentRole.append($content);
        doSuccess(onSuccess, $content);
    }
    
    function onPropertyChange(eventObj,options) {
        var o = {
            elementId: '',
            prop: '',
            control: '',
            onSuccess: []
        };
        if(options) {
            $.extend(o, options);
        }
        var nodeJson = mobileStorage.fetchCachedNodeJson(nodeId),
            propId = o.prop.propId,
            elementId = new CswString(eventObj.target.id),
            value = eventObj.target.value,
            propJson = null;
        
        if (false === isNullOrEmpty(nodeJson)) {
            mobileStorage.addUnsyncedChange();

            if (contains(nodeJson, 'tabs') &&
                contains(nodeJson.tabs, tabName) &&
                contains(nodeJson.tabs[tabName], propId)) {
                propJson = nodeJson.tabs[tabName][propId];
            }
            if (false === isNullOrEmpty(propJson)) {
                propJson = o.prop.updatePropValue(propJson, elementId, value);
                nodeJson.tabs[tabName][propId] = propJson;
                mobileStorage.updateStoredNodeJson(nodeId, nodeJson, '1');
            } else { 
                errorHandler('Could not find a prop to update');
            }
        }
        recalculateFooter($parent);
        doSuccess(o.onSuccess, o.control);
    } // onPropertyChange()
    
    //#endregion private
    
    //#region public, priveleged

    return {
        $pageDiv: $parent,
        $contentRole: $contentRole,
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        id: id,
        title: title,
        getContent: getContent
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageProps