/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="CswMobileSync.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswPrototypeExtensions.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

//#region CswMobileBackgroundTask

function CswMobileBackgroundTask(mobileStorage, mobileSync, options) {
    "use strict";
    /// <summary>
    ///   Mobile background task class.  
    /// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">A Csw mobile client database resource object.</param>
    /// <param name="mobileSync" type="CswMobileSync">A Csw mobile synchronization object.</param>
    /// <param name="options" type="JSON">JSON options for configuring the class.</param>
    /// <returns type="CswMobileBackgroundTask">Returns an instance of itself.</returns>

    //#region private
    var o = {
        onSuccess: null, //function () { },
        onError: null, //function () { },
        onLoginFailure: null, //function () { },
        PollingInterval: '30000', //30 seconds
        taskUrl: '/NbtWebApp/wsNBT.asmx/ConnectTest'
    };

    if (options) $.extend(o, options);

    var backgroundTaskId;

    function handleDataCheckTimer(onSuccessOveride, onFailureOveride) {
        /// <summary>
        ///   Execute background task and perpetuate thread  
        /// </summary>
        /// <param name="onSuccessOveride" type="Object">Override the onSuccess event</param>
        /// <param name="onFailureOveride" type="Object">Override the onFailure event</param>
        /// <returns type="void"></returns>
        if (false === mobileStorage.stayOffline())
        {
            CswAjaxJson({
                formobile: o.ForMobile,
                url: o.taskUrl,
                data: {},
                onloginfail: function (data) { o.onLoginFailure(data); },
                success: function (data) {
                    if (false === isNullOrEmpty(mobileSync)) {
                        mobileSync.initSync();
                    }
                    if (isFunction(onSuccessOveride))
                    {
                        onSuccessOveride(data);
                    } else if (isFunction(o.onSuccess)) {
                        o.onSuccess(data);
                    }
                    //we don't want to start the next iteration until ajax completes
                    _startBackgroundTask();
                },
                error: function (data)
                {
                    if (isFunction(onFailureOveride)) {
                        onFailureOveride(data);
                    } else if (isFunction(o.onError)) {
                        o.onError(data);  
                    }
                    _startBackgroundTask();
                }
            });
        } else { // if(amOnline())
            _startBackgroundTask();
        }
    } //_handleDataCheckTimer()

    //keep this private to avoid repeated dot operations on this
    function _startBackgroundTask (onSuccessOveride, onFailureOveride) {
        /// <summary>
        ///   Queue the background task. 
        /// </summary>
        /// <param name="onSuccessOveride" type="Object">Override the onSuccess event</param>
        /// <param name="onFailureOveride" type="Object">Override the onFailure event</param>
        /// <returns type="void"></returns>
        
        backgroundTaskId = setTimeout(function ()
            {
                handleDataCheckTimer(onSuccessOveride, onFailureOveride);
            },
            o.PollingInterval);
    }
    //#endregion private
    
    //#region public, priveleged
    this.start = _startBackgroundTask;

    this.stop = function() {
        /// <summary>
        ///   Terminate background task.  
        /// </summary>
        /// <returns type="void"></returns>
        clearTimeout(backgroundTaskId);
    };
    
    this.reset = function ()
    {
        var lastSyncTime = mobileStorage.lastSyncTime;

        var now = new Date().getTime();
        var last = new Date(lastSyncTime).getTime();
        var lastSync = now - last;

        if (lastSync > o.PollingInterval * 3) { //90 seconds
            this.stop();
            _startBackgroundTask();
        }
    };
    //#endregion public, priveleged

    //start task immediately on init
    _startBackgroundTask();
    
}

//#endregion CswMobileBackgroundTask