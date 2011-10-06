/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../main/tools/CswClientDb.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../globals/CswMobileTools.js" />

//#region CswMobileClientDb

CswMobileClientDb.inheritsFrom(CswClientDb);

function CswMobileClientDb() {
    /// <summary>
    ///   Mobile client db class to encapsulate JSON fetch/store/update methods.
    ///   Inherits from CswClientDb.
    /// </summary>
    /// <returns type="CswMobileClientDb">Instance of itself. Must instance with 'new' keyword.</returns>
    CswClientDb.call(this);

    var storedViews = {};

    this.storedViews = storedViews;
    
    this.storeViewJson = function(viewId, viewName, viewJson, level, viewSearch, wasModified) {
        /// <summary>
        ///   Stores a view in localStorage
        /// </summary>
        /// <param name="viewId" type="String">An NBT ViewId</param>
        /// <param name="viewName" type="String">Human readable view name</param>
        /// <param name="viewJson" type="JSON">JSON representation of the nodes of the view</param>
        /// <param name="level" type="Number">Number indicating tree depth</param>
        /// <param name="viewSearch" type="JSON">JSON representation of the possible mobile searches on this view</param>
        if (level === 0 && !isNullOrEmpty(viewJson)) {
            storedViews = { }; //the viewnames may have changed. clear to be sure.
            for (var view in viewJson) {
                storedViews[view] = viewJson[view];
            }
            //no need to cache the viewsdiv, just store ViewNames
            this.setItem(CswMobileGlobal_Config.storedViews, storedViews);
        } else {
            var viewNodes = { };
            for (var nodeId in viewJson)
            {
                viewNodes[nodeId] = viewJson[nodeId];
                if (wasModified)
                {
                    viewNodes[nodeId]['wasmodified'] = true;
                }
                viewNodes[nodeId]['viewid'] = viewId;
                this.setItem(nodeId, viewNodes[nodeId]);
                delete viewNodes[nodeId]['subitems'];
            }
            if (wasModified)
            {
                viewNodes['wasmodified'] = true;
            }
            this.setItem(viewId, { 'name': viewName, 'json': viewNodes, 'search': viewSearch });
        }
    };

    this.updateStoredViewJson = function(viewId, viewJson, wasModified) {
        /// <summary>
        ///   Updates a view in localStorage
        /// </summary>
        /// <param name="viewId" type="String">An NBT ViewId</param>
        /// <param name="viewName" type="String">Human readable view name</param>
        /// <param name="wasModified" type="Boolean">Indicates whether this update modifies the view</param>

        if (!isNullOrEmpty(viewId) && !isNullOrEmpty(viewJson))
        {
            var currentView = this.getItem(viewId);
            var viewName = currentView['name'];
            this.storeViewJson(viewId, viewName, viewJson, 1, '', wasModified);
        }
        return viewJson;
    };

    this.updateStoredNodeJson = function(nodeId, nodeJson, wasModified) {
        /// <summary>
        ///   Updates a node in view in localStorage
        /// </summary>
        /// <param name="nodeId" type="String">An NBT NodeId</param>
        /// <param name="nodeJson" type="JSON">JSON representation of the node</param>
        /// <param name="wasModified" type="Boolean">Indicates whether this update modifies the view</param>

        if (!isNullOrEmpty(nodeId) && !isNullOrEmpty(nodeJson))
        {
            if (isTrue(wasModified))
            {
                nodeJson['wasmodified'] = true;
            } else
            {
                delete nodeJson['wasmodified'];
            }
            this.setItem(nodeId, nodeJson);
        }
        return nodeJson;
    };

    this.deleteNode = function(nodeId, viewId) {
        /// <summary>
        ///   Remove a node from localStorage and the DOM
        /// </summary>
        /// <param name="nodeId" type="String">An NBT ViewId</param>
        /// <param name="viewId" type="String">Optional. The JSON property to retrieve. 'json' if omitted.</param>
        
        //remove the cached node JSON
        this.removeItem(nodeId);

        //remove the Div
        $('#' + nodeId).remove();

        //remove the node from the View JSON
        if (!isNullOrEmpty(viewId))
        {
            var view = this.getItem(viewId);
            if (!isNullOrEmpty(view['json']))
            {
                delete view['json'][nodeId];
                this.setItem(viewId, view);
            }
        }
    };

    this.fetchStoredViews = function() {
        /// <summary> Retrieve the cached View names from LocalStorage </summary>
        /// <returns type="Object">Cached views</returns>
        var ret = { };
        var viewObj = this.getItem(CswMobileGlobal_Config.storedViews);
        if (false === isNullOrEmpty(viewObj)) {
            ret = viewObj;
        }
        return ret;
    };
    
    this.fetchCachedViewJson = function(viewId, viewObj) {
        /// <summary>
        ///   Retrieve a view from localStorage
        /// </summary>
        /// <param name="viewId" type="String">An NBT ViewId</param>
        /// <param name="viewObj" type="String">Optional. The JSON property to retrieve. 'json' if omitted.</param>
        var ret = { };
        var rootObj = this.getItem(viewId);
        if (!isNullOrEmpty(rootObj))
        {
            var jProp = 'json';
            if (arguments.length === 2 && viewObj)
            {
                jProp = viewObj;
            }
            ret = rootObj[jProp];
        }
        return ret;
    };

    this.fetchCachedNodeJson = function(nodeId) {
        /// <summary>
        ///   Retrieve a node from the current view
        /// </summary>
        /// <param name="nodeId" type="String">An NBT NodeId</param>
        var ret = {};
        if (!isNullOrEmpty(nodeId))
        {
            ret = this.getItem(nodeId);
        }
        return ret;
    };
    
}

//#endregion CswMobileClientDb