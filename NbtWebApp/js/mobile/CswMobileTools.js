/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />
/// <reference path="clientdb/CswMobileClientDbResources.js" />
/// <reference path="../CswEnums.js" />

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
                title: '',
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
	///   Sets 'Online' button style 'offline'
	/// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="onComplete" type="Function">Event to fire on complete.</param>
	if(isNullOrEmpty(mobileStorage)) {
	    mobileStorage = new CswMobileClientDbResources();
	}
    mobileStorage.amOnline(false);
			
	$('.onlineStatus').removeClass('online')
						.addClass('offline')
						.find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
						.text('Offline')
						.removeClass('online')
						.addClass('offline')
						.end();
			
	$('.refresh').each(function(){
		var $this = $(this);
		try { //we'd prefer to simply disable it, but it might not be initialized yet.
		    $this.button('disable');
		} catch (e) {
		    $this.css({ display: 'none', visibility: 'hidden' }).hide();
		}
	});

    if(onComplete) {
        onComplete();
    }
	//$viewsdiv = reloadViews(); //no changePage
}

function setOnline(mobileStorage,onComplete) {
	/// <summary>
	///   Sets 'Online' button style 'online'
	/// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="onComplete" type="Function">Event to fire on complete.</param>
    if(isNullOrEmpty(mobileStorage)) {
	    mobileStorage = new CswMobileClientDbResources();
	}
    
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
		$('.refresh').each(function(){
			var $this = $(this);
		    try { //we may not be initialized
		        $this.removeAttr('display').removeAttr('visibility').show();
		        $this.button('enable');
		    } catch (e) {
		        //suppress error
		    }
		});
	    if (onComplete) {
		    onComplete();
		}
	}
}

function toggleOnline(mobileStorage,onComplete) {
    /// <summary>
	///   Toggles the online status displayed in UI according to actual status.
	/// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="onComplete" type="Function">Event to fire on complete.</param>
    if( isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    if(mobileStorage.amOnline()) {
        setOnline(mobileStorage,onComplete);
    } else {
        setOffline(mobileStorage,onComplete);
    }
}

function makeFooterButtonDef(name,id,onClick,mobileStorage) {
    /// <summary>Generate the JSON definition for a Mobile footer button</summary>
    /// <param name="name" type="CswMobileFooterButtons">CswMobileFooterButtons enum name for the button</param>
    /// <param name="id" type="String">Proposed Element ID</param>
    /// <param name="onClick" type="Fuction">Function to fire on button click</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="Object"> JSON for consumption by CswMobileMenuButton</returns>
    var ret = {};
    switch(name) {
        case CswMobileFooterButtons.online:
            {
                if( isNullOrEmpty(mobileStorage) ) {
                    mobileStorage = new CswMobileClientDbResources();
                }
                var onlineValue = mobileStorage.onlineStatus();
                var online = CswMobileFooterButtons.online;
                online.ID = id + '_gosyncstatus';
                online.text = onlineValue;
                online.cssClass += onlineValue.toLowerCase();
                
                ret = online;
                break;
            }
        case CswMobileFooterButtons.refresh:
            {
                var refresh = CswMobileFooterButtons.refresh;
                refresh.ID = id + '_refresh';

                ret = refresh;
                break;        
            }
        case CswMobileFooterButtons.fullsite:
            {
                var fullsite = CswMobileFooterButtons.fullsite;
                fullsite.ID = id + '_main';
                
                ret = fullsite;
                break;
            }
        case CswMobileFooterButtons.help:
            {
                var help = CswMobileFooterButtons.help;
                help.ID = id + '_help';

                ret = help;
                break;
            }
    }
    if(ret && !isNullOrEmpty(onClick)) {
        ret.onClick = onClick;
    }
    return ret;
}

function makeHeaderButtonDef(name,id,onClick) {
    /// <summary>Generate the JSON definition for a Mobile header button</summary>
    /// <param name="name" type="CswMobileHeaderButtons">CswMobileHeaderButtons enum name for the button</param>
    /// <param name="id" type="String">Proposed Element ID</param>
    /// <param name="onClick" type="Fuction">Function to fire on button click</param>
    /// <returns type="Object"> JSON for consumption by CswMobileMenuButton</returns>
    var ret = {};
    switch(name) {
        case CswMobileHeaderButtons.back:
            {
                var back = CswMobileHeaderButtons.back;
	            back.ID = id + '_back';

                ret = back;
                break;
            }
        case CswMobileHeaderButtons.search:
            {
                var search = CswMobileHeaderButtons.search;
	            search.ID = id + '_search';
                
                ret = search;
                break;
            }
    }
    if(ret && !isNullOrEmpty(onClick)) {
        ret.onClick = onClick;
    }
    return ret;
}

function makeMenuButtonDef(pageDef,id,buttonNames,mobileStorage) {
    /// <summary>Generate the JSON definition for a Mobile button header and footer</summary>
    /// <param name="pageDef" type="Object">JSON page defintion to extend</param>
    /// <param name="id" type="String">Page ID</param>
    /// <param name="buttonNames" type="Object">JSON collection of buttons: { name: onClick }</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="Object">JSON for consumption by CswMobilePageFactory</returns>
    var newPageDef = {
        headerDef: { buttons: { } },
        footerDef: { buttons: { } }
    };
    if(pageDef) {
        $.extend(newPageDef, pageDef);
    }
    if( isNullOrEmpty(newPageDef.footerDef)) {
        newPageDef.footerDef = { buttons: { } };
    }
    if( isNullOrEmpty(newPageDef.headerDef)) {
        newPageDef.headerDef = { buttons: { } };
    }
    var onClick;
    for (var name in buttonNames ) {
        onClick = buttonNames[name];
        switch(name) {
            case CswMobileHeaderButtons.back.name:
                {
                    newPageDef.headerDef.buttons.back = makeHeaderButtonDef(CswMobileHeaderButtons.back, id, onClick);
                    break;
                }
            case CswMobileHeaderButtons.search.name:
                {
                    newPageDef.headerDef.buttons.search = makeHeaderButtonDef(CswMobileHeaderButtons.search, id, onClick);
                    break;
                }
            case CswMobileFooterButtons.fullsite.name:
                {
                    newPageDef.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
                    break;
                }
            case CswMobileFooterButtons.help.name:
                {
                    newPageDef.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, onClick);
                    break;
                }
            case CswMobileFooterButtons.online.name:
                {
                    newPageDef.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, id, onClick, mobileStorage);
                    break;
                }
            case CswMobileFooterButtons.refresh.name:
                {
                    newPageDef.footerDef.buttons.refresh = makeFooterButtonDef(CswMobileFooterButtons.refresh, id, onClick);
                    break;
                }
        }
    }
    return newPageDef;
}
//#endregion functions