/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="CswMobileClientDb.js" />
/// <reference path="../../CswClientDb.js" />
/// <reference path="../../_Global.js" />

//#region CswMobileClientDbResources

CswMobileClientDbResources.prototype = new CswMobileClientDb;
CswMobileClientDbResources.prototype.constructor = CswMobileClientDbResources;


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
        var ret = sessionid;
        if (!isNullOrEmpty(sessionid)) {
            dataStore.setItem('sessionid', ret);
        } else {
            ret = dataStore.getItem('sessionid');
        }
        return ret;
    };
    //this.sessionid.toString = function() { return 'something'; }; //this.sessionid(); //function () { debugger; return ; };
    

    this.lastSyncSuccess = function () {
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        dataStore.setItem('lastSyncSuccess', ret);
        this.removeItem('lastSyncAttempt'); //clear last failed on next success
        dataStore.setItem('lastSyncTime', now);
        return ret;
    };
    this.lastSyncSuccess.toString = function () { return dataStore.getItem('lastSyncSuccess'); };
    
    this.lastSyncAttempt = function () {
        var now = new Date();
        var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
        dataStore.setItem('lastSyncAttempt', ret);
        dataStore.setItem('lastSyncTime', now);
        return ret;
    };
    //this.lastSyncAttempt.toString = function () { return dataStore.getItem('lastSyncAttempt'); };
    
    this.lastSyncTime = dataStore.getItem('lastSyncTime');
    
    this.addUnsyncedChange = function () {
        var unSyncedChanges = tryParseNumber(dataStore.getItem('unSyncedChanges'), '0');
        unSyncedChanges++;
        dataStore.setItem('unSyncedChanges', unSyncedChanges);
        return unSyncedChanges;
    };
    
    this.clearUnsyncedChanges = function () {
        dataStore.setItem('unSyncedChanges', '0');
    };
    
    this.stayOffline = function (value) {
        if (arguments.length === 1)
        {
            dataStore.setItem('stayOffline', isTrue(value));
        }
        var ret = isTrue(dataStore.getItem('stayOffline'));
        return ret;
    };
    
}

//#endregion CswMobileClientDbResources