/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region plugins

; (function ($)
{ /// <param name="$" type="jQuery" />
    
    $.fn.CswChangePage = function(options) {
        /// <summary>
        ///   Initiates page transition between CswMobilePages
        /// </summary>
        /// <param name="options" type="JSON">JQM options for the $.mobile.changePage() method</param>
        /// <returns type="void"></returns>
        var ret = false;
        
        var o = {
            transition: 'fade'
        };
        if (options) $.extend(o, options);
        var $div = $(this);
        if (!isNullOrEmpty($div)) {
            try {
                ret = $.mobile.changePage($div, o);
            } catch (e) {
                if (debugOn()) {
                    log('changePage() failed.',true);
                }
            }
        }
        return ret;
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
    if (arguments.length === 1 && isFunction(onSuccess)) {
        onSuccess();
    }
    return false;
}
        
function stopLoadingMsg(onSuccess,mobilePage) {
    /// <summary> Stops the JQM "loading.." message and executes a function.
	/// </summary>
	/// <param name="onSuccess" type="Function">Function to execute.</param>
    /// <param name="mobilePage" type="CswMobilePageFactory">A Csw Mobile Page</param>
	/// <returns type="Boolean">False (to support 'click' events)</returns>
    if (isFunction(onSuccess)) {
        onSuccess();
    } 
    if (isNullOrEmpty(mobilePage)) {
        $.mobile.hidePageLoadingMsg();    
    } else {
        $.mobile.hidePageLoadingMsg();
        setTimeout($.mobile.hidePageLoadingMsg, 1500);
    }
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

	
function Logout(mobileStorage, reloadWindow) {
	/// <summary> On login failure event </summary>
	/// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="reloadWindow" type="Boolean">If true, reload the login page.</param>
    /// <returns type="void"></returns>
    
    if (mobileStorage.checkNoPendingChanges()) {
				
		var loginFailure = tryParseString(mobileStorage.getItem('loginFailure'), '');

		mobileStorage.clear();
				
		mobileStorage.amOnline(true, loginFailure);
		// reloading browser window is the easiest way to reset
		if (reloadWindow) {
			window.location.href = window.location.pathname;
		}
	}
}

function setOffline(mobileStorage, onComplete) {
    /// <summary>
	///   Sets 'Online' button style 'offline'
	/// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="onComplete" type="Function">Event to fire on complete.</param>
	if (isNullOrEmpty(mobileStorage)) {
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
			
	$('.refresh').each(function () {
		var $this = $(this);
		try { //we'd prefer to simply disable it, but it might not be initialized yet.
		    $this.button('disable');
		} catch (e) {
		    $this.css({ display: 'none', visibility: 'hidden' }).hide();
		}
	});

    if (onComplete) {
        onComplete();
    }
	//$viewsdiv = reloadViews(); //no changePage
}

function setOnline(mobileStorage, onComplete) {
	/// <summary>
	///   Sets 'Online' button style 'online'
	/// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="onComplete" type="Function">Event to fire on complete.</param>
    if (isNullOrEmpty(mobileStorage)) {
	    mobileStorage = new CswMobileClientDbResources();
	}
    
	mobileStorage.amOnline(true);
	mobileStorage.removeItem('loginFailure');
	if (!mobileStorage.stayOffline())
	{
		$('.onlineStatus').removeClass('offline')
							.addClass('online')
							.find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
							.text('Online')
							.removeClass('offline')
							.addClass('online')
							.end();
		$('.refresh').each(function () {
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

function toggleOnline(mobileStorage, onComplete) {
    /// <summary>
	///   Toggles the online status displayed in UI according to actual status.
	/// </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="onComplete" type="Function">Event to fire on complete.</param>
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    if (mobileStorage.amOnline()) {
        setOnline(mobileStorage, onComplete);
    } else {
        setOffline(mobileStorage, onComplete);
    }
}

function makeFooterButtonDef(name, id, onClick, mobileStorage) {
    /// <summary>Generate the JSON definition for a Mobile footer button</summary>
    /// <param name="name" type="CswMobileFooterButtons">CswMobileFooterButtons enum name for the button</param>
    /// <param name="id" type="String">Proposed Element ID</param>
    /// <param name="onClick" type="Fuction">Function to fire on button click</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="Object"> JSON for consumption by CswMobileMenuButton</returns>
    var ret = {};
    switch (name) {
        case CswMobileFooterButtons.online:
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
        case CswMobileFooterButtons.refresh:
            var refresh = CswMobileFooterButtons.refresh;
            refresh.ID = id + '_refresh';

            ret = refresh;
            break;        
        case CswMobileFooterButtons.fullsite:
            var fullsite = CswMobileFooterButtons.fullsite;
            fullsite.ID = id + '_main';
                
            ret = fullsite;
            break;
        case CswMobileFooterButtons.help:
            var help = CswMobileFooterButtons.help;
            help.ID = id + '_help';

            ret = help;
            break;
    }
    if (ret && !isNullOrEmpty(onClick)) {
        ret.onClick = onClick;
    }
    return ret;
}

function makeHeaderButtonDef(name, id, onClick) {
    /// <summary>Generate the JSON definition for a Mobile header button</summary>
    /// <param name="name" type="CswMobileHeaderButtons">CswMobileHeaderButtons enum name for the button</param>
    /// <param name="id" type="String">Proposed Element ID</param>
    /// <param name="onClick" type="Fuction">Function to fire on button click</param>
    /// <returns type="Object"> JSON for consumption by CswMobileMenuButton</returns>
    var ret = {};
    switch (name) {
        case CswMobileHeaderButtons.back:
            var back = CswMobileHeaderButtons.back;
	        back.ID = id + '_back';

            ret = back;
            break;
        case CswMobileHeaderButtons.search:
            var search = CswMobileHeaderButtons.search;
	        search.ID = id + '_search';
                
            ret = search;
            break;
    }
    if (ret && !isNullOrEmpty(onClick)) {
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
    if (pageDef) {
        $.extend(newPageDef, pageDef);
    }
    if (isNullOrEmpty(newPageDef.footerDef)) {
        newPageDef.footerDef = { buttons: { } };
    }
    if (isNullOrEmpty(newPageDef.headerDef)) {
        newPageDef.headerDef = { buttons: { } };
    }
    if (isNullOrEmpty(newPageDef.headerDef.ID)) {
        newPageDef.headerDef.ID = id;
    }
    if (isNullOrEmpty(newPageDef.footerDef.ID)) {
        newPageDef.footerDef.ID = id;
    }
    var onClick;
    for (var name in buttonNames ) {
        if (buttonNames.hasOwnProperty(name)) {
            onClick = buttonNames[name];
            switch (name) {
                case CswMobileHeaderButtons.back.name:
                    newPageDef.headerDef.buttons.back = makeHeaderButtonDef(CswMobileHeaderButtons.back, id, onClick);
                    break;
                case CswMobileHeaderButtons.search.name:
                    newPageDef.headerDef.buttons.search = makeHeaderButtonDef(CswMobileHeaderButtons.search, id, onClick);
                    break;
                case CswMobileFooterButtons.fullsite.name:
                    newPageDef.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
                    break;
                case CswMobileFooterButtons.help.name:
                    newPageDef.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, onClick);
                    break;
                case CswMobileFooterButtons.online.name:
                    newPageDef.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, id, onClick, mobileStorage);
                    break;
                case CswMobileFooterButtons.refresh.name:
                    newPageDef.footerDef.buttons.refresh = makeFooterButtonDef(CswMobileFooterButtons.refresh, id, onClick);
                    break;
            }
        }
    }
    return newPageDef;
}

function makeEmptyListView(listView, $parent, noResultsText) {
    /// <summary>Generate a 'No Results' list item.</summary>
    /// <param name="listView" type="CswMobileListView">Csw Mobile List View.</param>
    /// <param name="$parent" type="jQuery">Some parent element to attach to, if listView is null.</param>
    /// <param name="noResultsText" type="String">Text to display on empty.</param>
    /// <returns type="void">Simply appends. No return.</returns>
    if (isNullOrEmpty(listView) && !isNullOrEmpty($parent)) {
        var ulDef = {
            ID: $parent.CswAttrDom('id') + '_noresult',
            cssclass: CswMobileCssClasses.listview.name
        };

        listView = new CswMobileListView(ulDef, $parent);
    }
    
    if (!isNullOrEmpty(listView)) {
        var text = tryParseString(noResultsText, 'No Results');
        var id = listView.$control.CswAttrDom('id') + '_noresult';
        listView.addListItem(id, text);
    }
}

function ensureContent($content, contentDivId) {
    /// <summary>
    ///    Ensures every page has a valid content Div to insert new HTML.
    ///    if content is populated, empty it.
    ///    This is a child element of the JQM 'content' page.
    /// </summary>
    /// <param name="$content" type="jQuery">Some content element.</param>
    /// <param name="contentDivId" type="String">DivId</param>
    /// <returns type="jQuery">An empty content div.</returns>
    if (isNullOrEmpty($content)) {
        $content = $('<div id="' + contentDivId + '"></div>');
    } else {
        $content.empty();   
    }
    return $content;
}    

function modifyPropJson(json,key,value) {
    /// <summary> Sets the value of a key on a JSON object, and adds a 'wasmodified' = true property. </summary>
    /// <param name="json" type="Object">Some JSON object.</param>
    /// <param name="key" type="String">A JSON property name (key).</param>
    /// <param name="value" type="Object">A value to set.</param>
    /// <returns type="Object">The modified JSON</returns>
    if (!isNullOrEmpty(key)) {
        if (json.hasOwnProperty('values') && json['values'].hasOwnProperty(key)) {
            var oldValue = json['values'][key];
            json['values'][key] = value;
            if (oldValue !== value) {
                json.wasmodified = true;
            }
        } else if (json.hasOwnProperty(key)) {
            var oldValue = json[key];
            json[key] = value;
            if (oldValue !== value) {
                json.wasmodified = true;
            }
        }
    }
    return json;
}

//#endregion functions