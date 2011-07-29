/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="CswMobileClientDb.js" />
/// <reference path="../../CswClientDb.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />

//#region CswMobileClientDbResources

CswMobileClientDbResources.inheritsFrom(CswMobileClientDb);

function CswMobileClientDbResources() {
    /// <summary>
    ///   Resources class to encapsulate common Mobile persistence methods.
    ///   Inherits from CswMobileClientDb.
    /// </summary>
    /// <returns type="CswMobileClientDbResources">Instance of itself. Must instance with 'new' keyword.</returns>
    CswMobileClientDb.call(this);
    
    //var this = new CswMobileClientDb();
    
    this.currentViewId = function(viewId) {
        /// <summary>
        ///   Persists the current NBT ViewId. 
        /// </summary>
        /// <param name="viewId" type="String">Optional. An NBT ViewId</param>
        /// <returns type="String">Stored viewid</returns>
        var ret = '';
        if (arguments.length === 1 && viewId) {
            ret = viewId;
            this.setItem('currentviewid', viewId);
        }
        if (isNullOrEmpty(ret)) {
            ret = this.getItem('currentviewid');
        }
        return ret;
    };
    //this.currentViewId.toString = function () { return this.currentViewId(); };

    this.username = function (username) {
        /// <summary>
        ///   Persists the current NBT user's username. 
        /// </summary>
        /// <param name="username" type="String">Optional. An NBT username</param>
        /// <returns type="String">Stored username</returns>
        var ret = username;
        if(!isNullOrEmpty(username)) {
            this.setItem('username',ret);
        } else {
            ret = this.getItem('username');
        }
        return ret;
    };

    this.customerid = function (customerid)
    {
        /// <summary>
        ///   Persists the current NBT user's customerid. 
        /// </summary>
        /// <param name="customerid" type="String">Optional. An NBT customerid</param>
        /// <returns type="String">Stored userid</returns>
        var ret = customerid;
        if (!isNullOrEmpty(customerid)) {
            this.setItem('customerid', ret);
        } else {
            ret = this.getItem('customerid');
        }
        return ret;
    };

    this.sessionid = function (sessionid)
    {
        /// <summary>
        ///   Persists the current NBT user's sessionid. 
        /// </summary>
        /// <param name="sessionid" type="String">Optional. An NBT sessionid</param>
        /// <returns type="String">Stored sessionid</returns>
        var ret = sessionid;
        if (!isNullOrEmpty(sessionid)) {
            this.setItem('sessionid', ret);
        } else {
            ret = this.getItem('sessionid');
        }
        return ret;
    };

    this.lastSyncSuccess = function () {
        /// <summary>
        ///   Stores the time of the last successful sync as now. 
        /// </summary>
        /// <returns type="String">Now, as human-readable string</returns>
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        this.setItem('lastSyncSuccess', ret);
        this.removeItem('lastSyncAttempt'); //clear last failed on next success
        this.setItem('lastSyncTime', now);
        return ret;
    };
    
    this.lastSyncAttempt = function () {
        /// <summary>
        ///   Stores the time of the last attempted (failed) sync as now. 
        /// </summary>
        /// <returns type="String">Now, as human-readable string</returns>
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        this.setItem('lastSyncAttempt', ret);
        this.setItem('lastSyncTime', now);
        return ret;
    };
    
    this.lastSyncTime = this.getItem('lastSyncTime');
    
    this.addUnsyncedChange = function () {
        /// <summary>
        ///   Increments the number of pending, unsyced changes 
        /// </summary>
        /// <returns type="String">Number of unsynced changes</returns>
        var unSyncedChanges = tryParseNumber(this.getItem('unSyncedChanges'), '0');
        unSyncedChanges++;
        this.setItem('unSyncedChanges', unSyncedChanges);
        return unSyncedChanges;
    };
    
    this.clearUnsyncedChanges = function () {
        /// <summary>
        ///   Resets number of unsynced changes to 0. 
        /// </summary>
        /// <returns type="void"></returns>
        this.setItem('unSyncedChanges', '0');
    };
    
    this.stayOffline = function (value) {
        /// <summary>
        ///   If the user chooses to 'Go Offline', set this preference in storage. 
        /// </summary>
        /// <returns type="Boolean">True if the user elected to go offline.</returns>
        if (arguments.length === 1)
        {
            this.setItem('stayOffline', isTrue(value));
        }
        var ret = isTrue(this.getItem('stayOffline'));
        return ret;
    };
}

//#endregion CswMobileClientDbResources