/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="CswMobileClientDb.js" />
/// <reference path="../../CswClientDb.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />

//#region CswMobileClientDbResources

CswMobileClientDbResources.inheritsFrom(CswMobileClientDb);

function CswMobileClientDbResources()
{
    CswMobileClientDb.call(this);

    var dataStore = new CswMobileClientDb();
    
    this.currentViewId = function(viewId)
    {
        /// <summary>
        ///   Persists the current NBT ViewId. 
        /// </summary>
        /// <param name="viewId" type="String">Optional. An NBT ViewId</param>
        /// <returns type="String">Stored viewid</returns>
        var ret = '';
        if (arguments.length === 1 && viewId)
        {
            ret = viewId;
            dataStore.setItem('currentviewid', viewId);
        }
        if (isNullOrEmpty(ret))
        {
            ret = dataStore.getItem('currentviewid');
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
            dataStore.setItem('username',ret);
        } else {
            ret = dataStore.getItem('username');
        }
        return ret;
    };
    //this.username.toString = function () { return this.username(); };

    this.customerid = function (customerid)
    {
        /// <summary>
        ///   Persists the current NBT user's customerid. 
        /// </summary>
        /// <param name="customerid" type="String">Optional. An NBT customerid</param>
        /// <returns type="String">Stored userid</returns>
        var ret = customerid;
        if (!isNullOrEmpty(customerid)) {
            dataStore.setItem('customerid', ret);
        } else {
            ret = dataStore.getItem('customerid');
        }
        return ret;
    };
    //this.customerid.toString = function () { return this.customerid(); };

    this.sessionid = function (sessionid)
    {
        /// <summary>
        ///   Persists the current NBT user's sessionid. 
        /// </summary>
        /// <param name="sessionid" type="String">Optional. An NBT sessionid</param>
        /// <returns type="String">Stored sessionid</returns>
        var ret = sessionid;
        if (!isNullOrEmpty(sessionid)) {
            dataStore.setItem('sessionid', ret);
        } else {
            ret = dataStore.getItem('sessionid');
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
        dataStore.setItem('lastSyncSuccess', ret);
        this.removeItem('lastSyncAttempt'); //clear last failed on next success
        dataStore.setItem('lastSyncTime', now);
        return ret;
    };
    
    this.lastSyncAttempt = function () {
        /// <summary>
        ///   Stores the time of the last attempted (failed) sync as now. 
        /// </summary>
        /// <returns type="String">Now, as human-readable string</returns>
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        dataStore.setItem('lastSyncAttempt', ret);
        dataStore.setItem('lastSyncTime', now);
        return ret;
    };
    
    this.lastSyncTime = dataStore.getItem('lastSyncTime');
    
    this.addUnsyncedChange = function () {
        /// <summary>
        ///   Increments the number of pending, unsyced changes 
        /// </summary>
        /// <returns type="String">Number of unsynced changes</returns>
        var unSyncedChanges = tryParseNumber(dataStore.getItem('unSyncedChanges'), '0');
        unSyncedChanges++;
        dataStore.setItem('unSyncedChanges', unSyncedChanges);
        return unSyncedChanges;
    };
    
    this.clearUnsyncedChanges = function () {
        /// <summary>
        ///   Resets number of unsynced changes to 0. 
        /// </summary>
        /// <returns type="void"></returns>
        dataStore.setItem('unSyncedChanges', '0');
    };
    
    this.stayOffline = function (value) {
        /// <summary>
        ///   If the user chooses to 'Go Offline', set this preference in storage. 
        /// </summary>
        /// <returns type="Boolean">True if the user elected to go offline.</returns>
        if (arguments.length === 1)
        {
            dataStore.setItem('stayOffline', isTrue(value));
        }
        var ret = isTrue(dataStore.getItem('stayOffline'));
        return ret;
    };
    
}

//#endregion CswMobileClientDbResources