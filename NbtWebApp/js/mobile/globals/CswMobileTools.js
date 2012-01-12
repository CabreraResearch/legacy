/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="CswMobileEnums.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../controls/CswMobileListView.js" />

//#region plugins

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    $.fn.CswChangePage = function (options) {
        /// <summary>
        ///   Initiates page transition between CswMobilePages
        /// </summary>
        /// <param name="options" type="JSON">JQM options for the $.mobile.changePage() method</param>
        /// <returns type="void"></returns>
        var ret = false,
            $div = $(this),
            $currentPage = $.mobile.activePage;

        var o = {
            transition: 'fade',
            removeActivePage: false
        };
        if (options) {
            $.extend(o, options);
        }

        if (false === isNullOrEmpty($div)) {
            try {
                ret = $.mobile.changePage($div, o);
                if (o.removeActivePage) {
                    setTimeout(function () {
                        $currentPage.remove();
                    }, 1000);
                }
            } catch (e) {
                if (debugOn()) {
                    log('changePage() failed.', true);
                }
            }
        }
        return ret;
    };

    $.fn.CswSetPath = function () {
        /// <summary>
        ///   Updates the JQM path
        /// </summary>
        /// <returns type="jQuery">Returns self for chaining</returns>
        var $ret = $(this);
        var id = $ret.CswAttrDom('id');
        if (false === isNullOrEmpty(id)) {
            $.mobile.path.set('#' + id);
        }
        return $ret;
    };

})(jQuery);

//#endregion plugins

//#region functions

function startLoadingMsg(onSuccess) {
    "use strict";
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
    "use strict";
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
    "use strict";
    /// <summary> Stops the JQM "loading.." message on error. </summary> 
    stopLoadingMsg();
}

function onLoginFail(text,mobileStorage) {
    "use strict";
    /// <summary> On login failure event </summary>
    /// <param name="text" type="String">Login failure text</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="void"></returns>
    Logout(mobileStorage,true);
    mobileStorage.setItem('loginFailure', text);
    stopLoadingMsg();
}

function onLogout(mobileStorage) {
    "use strict";
    /// <summary> Calls Logout() </summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <returns type="Boolean">false, for use in 'click' event.</returns>
    Logout(mobileStorage,true);
    return false;
}

    
function Logout(mobileStorage, reloadWindow) {
    "use strict";
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
    "use strict";
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
            $this.hide().css({ display: 'none', visibility: 'hidden' });
        }
    });

    if (onComplete) {
        onComplete();
    }
    //$viewsdiv = reloadViews(); //no changePage
}

function setOnline(mobileStorage, onComplete) {
    "use strict";
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
                $this.css('display', '').css('visibility', '').show();
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
    "use strict";
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
    "use strict";
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
    "use strict";
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
    "use strict";
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
    var onClick, name;
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
    for (name in buttonNames ) {
        if (contains(buttonNames, name)) {
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
    "use strict";
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

function ensureContent($contentRole, contentDivId) {
    "use strict";
    /// <summary>
    ///    Ensures every page has a valid content Div to insert new HTML.
    ///    if content is populated, empty it.
    ///    This will become a child element of the JQM 'content' page.
    /// </summary>
    /// <param name="$contentRole" type="jQuery">Some contentRole element.</param>
    /// <param name="contentDivId" type="String">DivId</param>
    /// <returns type="jQuery">An empty content div.</returns>
    var $content = $('<div id="' + tryParseString(contentDivId) + '"></div>'); ;
    if (false === isNullOrEmpty($contentRole)) {
        $contentRole.empty();
    }
    return $content;
}    

function modifyPropJson(json,key,value) {
    "use strict";
    /// <summary> Sets the value of a key on a JSON object, and adds a 'wasmodified' = true property. </summary>
    /// <param name="json" type="Object">Some JSON object.</param>
    /// <param name="key" type="String">A JSON property name (key).</param>
    /// <param name="value" type="Object">A value to set.</param>
    /// <returns type="Object">The modified JSON</returns>
    var oldValue;
    if (false === isNullOrEmpty(key)) {
        if (contains(json, 'values')) {
            if (contains(json.values, key)) {
                oldValue = json.values[key];
                json.values[key] = value;
                if (oldValue !== value) {
                    json.wasmodified = true;
                }
            }
            else if (contains(json.values, 'value') && contains(json.values.value, key)) {
                oldValue = json.values.value[key];
                json.values.value[key] = value;
                if (oldValue !== value) {
                    json.wasmodified = true;
                }
            }
        } else if (contains(json, key)) {
            oldValue = json[key];
            json[key] = value;
            if (oldValue !== value) {
                json.wasmodified = true;
            }
        }
    }
    return json;
}

function isTimeToRefresh(mobileStorage,refreshInterval) {
    "use strict";
    /// <summary> Determines whether to use cached data or request new data from server.</summary>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="refreshInterval" type="Number">A refresh interval. Default is 5 minutes (300000).</param>
    /// <returns type="Boolean">True if online and refreshInterval has passed (lastSyncTime - interval).</returns>
    var ret = true,
        now = new Date(),
        lastSync = new Date(Date(mobileStorage.lastSyncTime)),
        interval = tryParseNumber(refreshInterval, 300000);
        
    if ((false === mobileStorage.amOnline()) || 
        (false === isNullOrEmpty(mobileStorage.lastSyncTime) && 
        ( now.getTime() - lastSync.getTime() < interval ))) {
        ret = false;
    }
    return ret;
}

function recalculateFooter($page, startingHeight) {
    "use strict";
    /// <summary> JQM's footer position calculation is based on document height, which for some as-of-yet-unknown reason is WAY off (too high). So let's recalibrate using the window height.</summary>
    /// <param name="$page" type="JQuery">A page to fix</param>
    /// <returns type="void" />
    var documentHeight = $(document).height(),
        windowHeight = $(window).height(),
        adjustedHeight = windowHeight - 333, // documentHeight - 542,,
        winDocHeightDif = documentHeight - windowHeight,
        footer = $page.find('div:jqmData(role="footer")'),
        top = footer.css('top'),
        heightChange = documentHeight - startingHeight;
    
//    if( isNumeric(heightChange) && heightChange !== 0) {
//        top += heightChange;
//        footer.css('top', top);
//    }
//    else if (winDocHeightDif > 0) {
        footer.css('top', 0);
//    }
}

function doSuccess(onSuccess) {
    "use strict";
    var args = Array.prototype.slice.call(arguments);
    if (args.length > 0) {
        args.shift();
    } else {
        args = [];
    }
    if (isFunction(onSuccess)) {
        onSuccess(args);
    } else {
    onSuccess.forEach(function (func) {
        if (isFunction(func)) {
            func.apply(null, args);
        }
    });

    }
}
//#endregion functions