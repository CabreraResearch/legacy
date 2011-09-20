/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="CswMobileBackgroundTask.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

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
        onError: onError,
        onLoginFailure: onLoginFail,
        onComplete: function () {},
        syncUrl: '/NbtWebApp/wsNBT.asmx/UpdateProperties',
        ForMobile: true
    };

    if(options) $().extend(o, options);

    if (isNullOrEmpty(mobileStorage)) { //this enables Intellisense
        mobileStorage = new CswMobileClientDbResources();
    }
    
    //#endregion private
    
    //#region public, priveleged

    this.initSync = function (afterComplete) {
    	/// <summary>
    	///   Initiates a sync event  
    	/// </summary>
    	/// <returns type="void"></returns>

    	var sessionId = mobileStorage.sessionid();
    	if (!isNullOrEmpty(o.onSync) &&
            !isNullOrEmpty(sessionId) &&
			!mobileStorage.stayOffline()) {

    		o.onSync(function (objectId, objectJSON) {
    			if (!isNullOrEmpty(objectId) && !isNullOrEmpty(objectJSON)) {

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
    						if (!isNullOrEmpty(o.onLoginFailure)) {
    							o.onLoginFailure(text);
    						}
    					},
    					success: function (data) {
    						if (!isNullOrEmpty(o.onSuccess)) {
    							o.onSuccess(data, objectId, objectJSON, false);
    						}
    						if (!isNullOrEmpty(o.onComplete)) {
    							o.onComplete();
    						}
    						if (!isNullOrEmpty(afterComplete)) {
    							afterComplete();
    						}
    					},
    					error: function () {
    						if (!isNullOrEmpty(o.onFailure)) {
    							o.onError();
    						}
    						if (!isNullOrEmpty(o.onComplete)) {
    							o.onComplete();
    						}
    						if (!isNullOrEmpty(afterComplete)) {
    							afterComplete();
    						}
    					}
    				});
    			}
    		}); // o.onSync();
    	} else {
    		if (!isNullOrEmpty(o.onSuccess)) {
    			o.onSuccess();
    		}
    		if (!isNullOrEmpty(o.onComplete)) {
    			o.onComplete();
    		}
    		if (!isNullOrEmpty(afterComplete)) {
    			afterComplete();
    		}
    	} // if-else(SessionId != '') 

    };  //initSync();
    
    //#endregion public, priveleged
}

//#endregion CswMobileSync