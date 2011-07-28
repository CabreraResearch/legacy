/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />
/// <reference path="CswMobileBackgroundTask.js" />

//#region CswMobileSync

function CswMobileSync(options,mobileStorage) {
    /// <summary>
    ///   Mobile synchronization class.  
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - UpdateViewUrl: WebService URL
    ///     &#10;2 - onSync: function to wrap sync webservice call
    ///     &#10;3 - onSuccess: function to exec on webservice call success
    ///     &#10;4 - onError: function to exec on webservice call failure
    ///     &#10;5 - onComplete: function to exec on sync method completion
    ///     &#10;6 - onLoginFailure: function to exec on login failure   
    /// </param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">A Csw mobile client database resources object.</param>
    /// <returns type="CswMobileSync">Returns an instance of itself.</returns>

    //#region private
    
    var o = {
        onSync: function () {},
        onSuccess: function () {},
        onError: function () {},
        onLoginFailure: function () {},
        onComplete: function () {},
        syncUrl: '',
        ForMobile: true
    };

    if(options) $().extend(o, options);

    if (false) { //this enables Intellisense
        mobileStorage = new CswMobileClientDbResources();
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.initSync = function(mobileBgTask) {
        /// <summary>
        ///   Initiates a sync event  
        /// </summary>
        /// <param name="mobileBgTask" type="CswMobileBackgroundTask">Optional. A Csw Mobile background task object.</param>
        /// <returns type="void"></returns>
        
        if(false) { //this enables Intellisense
            mobileBgTask = new CswMobileBackgroundTask();
        }
        var isBackgroundTask = !isNullOrEmpty(mobileBgTask);
        var sessionId = mobileStorage.sessionid();
        if (!isNullOrEmpty(o.onSync) &&
            !isNullOrEmpty(sessionId) &&
                !mobileStorage.stayOffline()) {
            //_processModifiedNodes(function (objectId, objectJSON)
            o.onSync(function(objectId, objectJSON) {
                if (!isNullOrEmpty(objectId) && !isNullOrEmpty(objectJSON))
                {

                    var dataJson = {
                        SessionId: sessionId,
                        ParentId: objectId,
                        UpdatedViewJson: JSON.stringify(objectJSON),
                        ForMobile: o.ForMobile
                    };

                    CswAjaxJSON({
                            formobile: o.ForMobile,
                            url: o.syncUrl,
                            data: dataJson,
                            onloginfail: function(text)
                            {
                                //HEY YOU!: add setOffline(); to onLoginFailure if you know what's good for you!
                                if (!isNullOrEmpty(o.onLoginFailure)) {
                                    o.onLoginFailure(text);
                                }
                            },
                            success: function(data)
                            {
                                if (!isNullOrEmpty(o.onSuccess)) {
                                    o.onSuccess(data,objectId,objectJSON,isBackgroundTask);
                                }
                            },
                            error: function()
                            {
                                if (!isNullOrEmpty(o.onFailure)) {
                                    o.onError();
                                }
                            }
                        });
                }
            }); // o.onSync();
        } else
        {
            if (!isNullOrEmpty(o.onSuccess)) {
                o.onSuccess();
            }

        } // if(SessionId != '') 

        if (isBackgroundTask) {
            mobileBgTask.start();
        }
        if (!isNullOrEmpty(o.onComplete)) {
            o.onComplete();
        }

    }; //initSync();
    
    //#endregion public, priveleged
}

//#endregion CswMobileSync