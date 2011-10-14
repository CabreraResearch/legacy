/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="CswMobileClientDb.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileClientDbResources

CswMobileClientDbResources.inheritsFrom(CswMobileClientDb);

function CswMobileClientDbResources() {
    /// <summary>
    ///   Resources class to encapsulate common Mobile persistence methods.
    ///   Inherits from CswMobileClientDb.
    /// </summary>
    /// <returns type="CswMobileClientDbResources">Instance of itself. Must instance with 'new' keyword.</returns>
    CswMobileClientDb.call(this);
   
    function handleStorageRequest(key, value, mobileStorage) {
        var ret = '';
        if (arguments.length === 3 && value) {
            ret = value;
            mobileStorage.setItem(key, value);
        }
        if (isNullOrEmpty(ret)) {
            ret = mobileStorage.getItem(key);
        }
        return ret;
    }
    
    this.currentViewId = function (viewId) {
        /// <summary>
        ///   Persists the current NBT ViewId. 
        /// </summary>
        /// <param name="viewId" type="String">Optional. An NBT ViewId</param>
        /// <returns type="String">Stored viewid</returns>
        return handleStorageRequest('currentviewid', viewId, this);
    };
    
    this.currentNodeId = function (nodeId) {
        /// <summary>
        ///   Persists the current NBT NodeId. 
        /// </summary>
        /// <param name="nodeId" type="String">Optional. An NBT NodeId</param>
        /// <returns type="String">Stored nodeid</returns>
        return handleStorageRequest('currentnodeid', nodeId, this);
    };
    
    this.currentTabId = function (tabId) {
        /// <summary>
        ///   Persists the current NBT TabId. 
        /// </summary>
        /// <param name="tabId" type="String">Optional. An NBT NodeId</param>
        /// <returns type="String">Stored nodeid</returns>
        return handleStorageRequest('currenttabid', tabId, this);
    };
    
    this.username = function (username) {
        /// <summary>
        ///   Persists the current NBT user's username. 
        /// </summary>
        /// <param name="username" type="String">Optional. An NBT username</param>
        /// <returns type="String">Stored username</returns>
        return handleStorageRequest('username', username, this);
    };

    this.customerid = function (customerid)
    {
        /// <summary>
        ///   Persists the current NBT user's customerid. 
        /// </summary>
        /// <param name="customerid" type="String">Optional. An NBT customerid</param>
        /// <returns type="String">Stored userid</returns>
        return handleStorageRequest('customerid', customerid, this);
    };

    this.sessionid = function (sessionid)
    {
        /// <summary>
        ///   Persists the current NBT user's sessionid. 
        /// </summary>
        /// <param name="sessionid" type="String">Optional. An NBT sessionid</param>
        /// <returns type="String">Stored sessionid</returns>
        var sessionId = $.CswCookie('get', CswCookieName.SessionId);
        if (false === isNullOrEmpty(sessionId)) {
            sessionId = handleStorageRequest('sessionid', sessionid, this);
        }
        return sessionId;
    };

    this.lastSyncSuccess = function () {
        /// <summary>
        ///   Stores the time of the last successful sync as now. 
        /// </summary>
        /// <returns type="String">Now, as human-readable string</returns>
        var mobileStorage = this;
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        mobileStorage.setItem('lastSyncSuccess', ret);
        mobileStorage.removeItem('lastSyncAttempt'); //clear last failed on next success
        mobileStorage.setItem('lastSyncTime', now);
        return ret;
    };
    
    this.lastSyncAttempt = function () {
        /// <summary>
        ///   Stores the time of the last attempted (failed) sync as now. 
        /// </summary>
        /// <returns type="String">Now, as human-readable string</returns>
        var mobileStorage = this;
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        mobileStorage.setItem('lastSyncAttempt', ret);
        mobileStorage.setItem('lastSyncTime', now);
        return ret;
    };
    
    this.lastSyncTime = this.getItem('lastSyncTime');
    
    this.addUnsyncedChange = function () {
        /// <summary>
        ///   Increments the number of pending, unsyced changes 
        /// </summary>
        /// <returns type="String">Number of unsynced changes</returns>
        var mobileStorage = this;
        var unSyncedChanges = tryParseNumber(mobileStorage.getItem('unSyncedChanges'), '0');
        unSyncedChanges++;
        this.setItem('unSyncedChanges', unSyncedChanges);
        return unSyncedChanges;
    };
    
    this.clearUnsyncedChanges = function () {
        /// <summary>
        ///   Resets number of unsynced changes to 0. 
        /// </summary>
        /// <returns type="void"></returns>
        var mobileStorage = this;
        mobileStorage.setItem('unSyncedChanges', '0');
    };
    
    this.stayOffline = function (value) {
        /// <summary>
        ///   If the user chooses to 'Go Offline', set this preference in storage. 
        /// </summary>
        /// <returns type="Boolean">True if the user elected to go offline.</returns>
        var mobileStorage = this;
        if (arguments.length === 1)
        {
            mobileStorage.setItem('stayOffline', isTrue(value));
        }
        var ret = isTrue(mobileStorage.getItem('stayOffline'));
        return ret;
    };

    this.amOnline = function (isOnline, loginFailure) {
        /// <summary>Evaluates or sets the user's online status.</summary>
        /// <param name="isOnline" type="Boolean">True if online.</param>
        /// <param name="loginFailure" type="String">Text of login failure, if any.</param>
        /// <returns type="Boolean">True if online. False otherwise.</returns>
        var mobileStorage = this;
        if (arguments.length > 0) {
            mobileStorage.setItem('online', isTrue(isOnline) );
        } 
        if (loginFailure) {
            mobileStorage.setItem('loginFailure', loginFailure );
        }
        var ret = (isTrue(mobileStorage.getItem('online')) && !mobileStorage.stayOffline());
        return ret;
    };

    this.onlineStatus = function () {
        /// <summary>Evaluates the user's online status for display.</summary>
        /// <returns type="String">'Online' or 'Offline'</returns>
        var mobileStorage = this;
        var ret = (!mobileStorage.amOnline() || mobileStorage.stayOffline()) ? 'Offline' : 'Online';
        return ret;
    };
    
    this.checkNoPendingChanges = function () {
        var mobileStorage = this;
        var pendingChanges = (!mobileStorage.pendingChanges() ||
            confirm('You have pending unsaved changes.  These changes will be lost.  Continue?'));
        return pendingChanges;
    };

    this.pendingChanges = function () {
        var mobileStorage = this;
        var changes = new Number(tryParseString(mobileStorage.getItem('unSyncedChanges'), '0'));
        return (changes > 0);
    };

    this.forceContentRefresh = function(value) {
        /// <summary>
        ///   On 'Refresh' force a call to the relevant webservice. 
        /// </summary>
        /// <returns type="Boolean">True if the very next request should specify a server refresh.</returns>
        var mobileStorage = this,
            ret;
        if (arguments.length === 1) {
            mobileStorage.setItem('forceContentRefresh', isTrue(value));
        }
        ret = isTrue(mobileStorage.getItem('forceContentRefresh'));
        return ret;
    };

}

//#endregion CswMobileClientDbResourcesz