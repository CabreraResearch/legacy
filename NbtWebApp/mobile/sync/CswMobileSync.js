/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="CswMobileBackgroundTask.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../globals/CswMobileTools.js" />

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
        onSync: null, 
        onSuccess: [], 
        onError: [],
        onLoginFailure: null,
        syncUrl: '/NbtWebApp/wsNBT.asmx/UpdateProperties',
        ForMobile: true
    };
    if (options) $().extend(o, options);
    var onSuccess = o.onSuccess,
        onError = o.onError;
    //#endregion private
    
    //#region public, priveleged

    return {
        initSync: function () {
            /// <summary>
            ///   Initiates a sync event  
            /// </summary>
            /// <returns type="void"></returns>

            var sessionId = mobileStorage.sessionid();
            if (false === isNullOrEmpty(o.onSync) &&
                false === isNullOrEmpty(sessionId) &&
                    false === mobileStorage.stayOffline()) {

                o.onSync(function (objectId, objectJSON) {
                    if (false === isNullOrEmpty(objectId) && false === isNullOrEmpty(objectJSON)) {

                        var dataJson = {
                            SessionId: sessionId,
                            ParentId: objectId,
                            UpdatedViewJson: JSON.stringify(objectJSON),
                            ForMobile: o.ForMobile
                        };

                        CswAjaxJson({
                            formobile: o.ForMobile,
                            url: o.syncUrl,
                            data: dataJson,
                            onloginfail: function (text) {
                                if (isFunction(o.onLoginFailure)) {
                                    o.onLoginFailure(text);
                                }
                            },
                            success: function (data) {
                                doSuccess(onSuccess, data, objectId, objectJSON, false);
                            },
                            error: function () {
                                doSuccess(onError);
                            }
                        });
                    }
                }); // o.onSync();
            } else {
                doSuccess(onSuccess);
            } // if-else(SessionId != '') 

        }, //initSync();
        queueOnSuccess: function (func) {
            if (isFunction(func)) {
                onSuccess.push(func);
            }
            return onSuccess;
        },
        queueOnError: function (func) {
            if (isFunction(func)) {
                onError.push(func);
            }
            return onError;
        }
    };
    //#endregion public, priveleged
}

//#endregion CswMobileSync