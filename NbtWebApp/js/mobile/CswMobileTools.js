/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />
/// <reference path="clientdb/CswMobileClientDbResources.js" />

//#region plugins

; (function ($)
{ /// <param name="$" type="jQuery" />
    
    $.fn.cswUL = function(params) {
        //this will become CswMobileListView
        var p = {
            'id': '',
            'data-filter': false,
            'data-role': 'listview',
            'data-inset': true,
            'cssclass': '',
            'showLoading': true
        };
        if (params) $.extend(p, params);

        var $div = $(this);
        var $ret = undefined;
        if (!isNullOrEmpty($div)) {
            $ret = $('<ul class="' + p.cssclass + '" id="' + tryParseString(p.id, '') + '"></ul>')
                                                    .appendTo($div)
                                                    .CswAttrXml(p);
            if(params.showLoading) {
                $ret.bind('click', function() { $.mobile.showPageLoadingMsg(); });
            }
        }
        return $ret;
    };

    $.fn.CswChangePage = function(options) {
        /// <summary>
        ///   Initiates page transition between CswMobilePages
        /// </summary>
        /// <param name="options" type="JSON">JQM options for the $.mobile.changePage() method</param>
        /// <returns type="void"></returns>
        var o = {
            transition: 'fade'
            //reverse: false,
            //changeHash: true,
            //role: 'page',
            //pageContainer: $.mobile.pageContainer,
            //type: 'get',
            //data: undefined,
            //reloadPage: false,
            //showLoadMsg: true
        };
        if (options) $.extend(o, options);

        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div)) {
            ret = $.mobile.changePage($div, o);
        }
        return ret;
    };

    $.fn.CswPage = function() {
        /// <summary>
        ///   Calls page styling on a DOM element
        /// </summary>
        /// <returns type="void"></returns>
        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div)) {
            ret = $div.page(); 
        }
        return ret;
    };

    $.fn.CswUnbindJqmEvents = function() {
        /// <summary>
        ///   Unbinds the 'pageshow' event from a DOM element
        /// </summary>
        /// <returns type="jQuery">Returns self for chaining</returns>
        var $div = $(this);
        if (!isNullOrEmpty($div) && $div.length > 0) {
            $div.unbind('pageshow');
        }
        return $div;
    };

    $.fn.CswBindJqmEvents = function(params) {
        /// <summary>
        ///   Binds the 'pageshow' event from a DOM element
        /// </summary>
        /// <param name="params" type="JSON">Options for the event binding. Should include an onPageShow() method.</param>
        /// <returns type="jQuery">Returns self for chaining</returns>
        var $div = $(this); 
        var $ret = false;
        if (!isNullOrEmpty($div)) {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                json: '',
                parentlevel: 0,
                level: 1,
                HideRefreshButton: false,
                HideSearchButton: false,
                onPageShow: function() {}
            };

            if (params) $.extend(p, params);
            p.level = (p.parentlevel === p.level) ? p.parentlevel + 1 : p.level;

            $ret = $div.bind('pageshow', function() {
                p.onPageShow(p);
                fixGeometry();
            });
        }
        return $ret;
    };
    
    $.fn.CswSetPath = function() {
        /// <summary>
        ///   Updates the JQM path
        /// </summary>
        /// <returns type="jQuery">Returns self for chaining</returns>
        var $ret = $(this); 
        var id = $ret.CswAttrDom('id');
        if (!isNullOrEmpty(id)) {
            $.mobile.path.set('#' + id);
        }
        return $ret;
    };
    
})(jQuery);

//#endregion plugins

//#region functions

function startLoadingMsg(onSuccess) {
    /// <summary> Starts the JQM "loading.." message and executes a function.
	/// </summary>
	/// <param name="onSuccess" type="Function">Function to execute.</param>
	/// <returns type="Boolean">False (to support 'click' events)</returns>
    $.mobile.showPageLoadingMsg();
    if( arguments.length === 1 && !isNullOrEmpty(onSuccess) ) {
        onSuccess();
    }
    return false;
}
        
function stopLoadingMsg(onSuccess) {
    /// <summary> Stops the JQM "loading.." message and executes a function.
	/// </summary>
	/// <param name="onSuccess" type="Function">Function to execute.</param>
	/// <returns type="Boolean">False (to support 'click' events)</returns>
    if( arguments.length === 1 && !isNullOrEmpty(onSuccess) ) {
        onSuccess();
    } 
    $.mobile.hidePageLoadingMsg();
    var $currentDiv = $("div[data-role='page']:visible:visible");
    $currentDiv.find('.csw_listview').CswPage();
    return false;
}

function onError() {
	/// <summary> Stops the JQM "loading.." message on error. </summary> 
    stopLoadingMsg();
}

function onLoginFail(text,mobileStorage) {
	/// <summary> On login failure event </summary>
	/// <param name="text" type="String">Login failure text</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="void"></returns>
    Logout(mobileStorage,true);
	mobileStorage.setItem('loginFailure', text);
	stopLoadingMsg();
}

function onLogout(mobileStorage) {
	/// <summary> Calls Logout() </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="Boolean">false, for use in 'click' event.</returns>
    Logout(mobileStorage,true);
    return false;
}

	
function Logout(mobileStorage,reloadWindow) {
	/// <summary> On login failure event </summary>
	/// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="reloadWindow" type="Boolean">If true, reload the login page.</param>
    /// <returns type="void"></returns>
    
    if ( mobileStorage.checkNoPendingChanges() ) {
				
		var loginFailure = tryParseString(mobileStorage.getItem('loginFailure'), '');

		mobileStorage.clear();
				
		mobileStorage.amOnline(true,loginFailure);
		// reloading browser window is the easiest way to reset
		if (reloadWindow) {
			window.location.href = window.location.pathname;
		}
	}
}

function setOffline(mobileStorage,onComplete) {
	/// <summary>
	///   Sets 'Online' button style 'offline',
	/// </summary>
	mobileStorage.amOnline(false);
			
	$('.onlineStatus').removeClass('online')
						.addClass('offline')
						.find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
						.text('Offline')
						.removeClass('online')
						.addClass('offline')
						.end();
			
	$('.refresh').css('visibility', 'hidden');

    if(onComplete) {
        onComplete();
    }
	//$viewsdiv = reloadViews(); //no changePage
}

function setOnline(mobileStorage,onComplete) {
			
	mobileStorage.amOnline(true);
	mobileStorage.removeItem('loginFailure');
	if( !mobileStorage.stayOffline() )
	{
		$('.onlineStatus').removeClass('offline')
							.addClass('online')
							.find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
							.text('Online')
							.removeClass('offline')
							.addClass('online')
							.end();
		$('.refresh').css('visibility', '');
		if (onComplete) {
		    onComplete();
		}
	}
}

//#endregion functions