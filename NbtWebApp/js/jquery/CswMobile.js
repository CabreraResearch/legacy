/// <reference path="../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../thirdparty/jquery/core/jquery.mobile/jquery.mobile-1.0b1.js" />
/// <reference path="../thirdparty/jquery/plugins/jquery-validate-1.8/jquery.validate.js" />
/// <reference path="../thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />
/// <reference path="../CswClasses.js" />

//var profiler = $createProfiler();

CswAppMode.mode = 'mobile';

;(function($) {
    /// <param name="$" type="jQuery" />

    $.fn.cswUL = function(params) {
        var p = {
            'id': '',
            'data-filter': false,
            'data-role': 'listview',
            'data-inset': true,
            'cssclass': ''
        };
        if (params) $.extend(p, params);

        var $div = $(this);
        var $ret = undefined;
        if (!isNullOrEmpty($div)) {
            $ret = $('<ul class="' + p.cssclass + '" id="' + tryParseString(p.id, '') + '"></ul>')
                                                    .appendTo($div)
                                                    .bind('click', function () { $.mobile.showPageLoadingMsg; })
                                                    .CswAttrXml(p);
        }
        return $ret;
    };

    $.fn.cswChangePage = function(options) {
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
            if(debugOn()) {
                log('changePage from ' + $.mobile.path.get() + ' to ' + $div.CswAttrDom('id'), true);
            }
            ret = $.mobile.changePage($div, o);
        }
        return ret;
    };

    $.fn.cswPage = function() {
        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div)) {
            ret = $div.page(); 
        }
        return ret;
    };

    $.fn.unbindJqmEvents = function() {
        var $div = $(this);
        if (!isNullOrEmpty($div) && $div.length > 0) {
            $div.unbind('pageshow');
        }
        return $div;
    };

    $.fn.bindJqmEvents = function(params) {
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
                if (p.level === 1) storeLocalData('currentviewid', p.DivId);
                p.onPageShow(p);
                if($('#logindiv')) $('#logindiv').remove();
            });
        }
        return $ret;
    };

    $.fn.CswMobile = function(options) {
        /// <summary>
        ///   Generates the Nbt Mobile page
        /// </summary>
        var $body = this;

        var opts = {
            ViewsListUrl: '/NbtWebApp/wsNBT.asmx/GetViewsList',
            ViewUrl: '/NbtWebApp/wsNBT.asmx/GetView',
            ConnectTestUrl: '/NbtWebApp/wsNBT.asmx/ConnectTest',
            ConnectTestRandomFailUrl: '/NbtWebApp/wsNBT.asmx/ConnectTestRandomFail',
            UpdateViewUrl: '/NbtWebApp/wsNBT.asmx/UpdateProperties',
            MainPageUrl: '/NbtWebApp/Mobile.html',
            AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/Authenticate',
            SendLogUrl: '/NbtWebApp/wsNBT.asmx/collectClientLogInfo',
            Theme: 'a',
            PollingInterval: 30000, //30 seconds
            RandomConnectionFailure: false
        };

        if (options) {
            $.extend(opts, options);
        }

        debugOn(debug);
        
        var ForMobile = true;

        var mobileStorage = new CswMobileStorage();
        
        var SessionId = getStoredLocalString('sessionid');
        if(isNullOrEmpty(SessionId)) {
            Logout();
        }

        var storedViews = getStoredLocalJSON('storedViews');

        var $logindiv = _loadLoginDiv();
        var $viewsdiv = reloadViews();
        var $syncstatus = _makeSyncStatusDiv();
        var $helpdiv = _makeHelpDiv();
        var $sorrycharliediv = _loadSorryCharlieDiv();

        // case 20355 - error on browser refresh
        // there is a problem if you refresh with #viewsdiv where we'll generate a 404 error, but the app will continue to function
        if (!isNullOrEmpty(SessionId)) {
            $.mobile.path.set('#viewsdiv');
            storeLocalData('refreshPage', 'viewsdiv');
        } else {
            $.mobile.path.set('#logindiv');
            storeLocalData('refreshPage', 'logindiv' );
        }
              
        window.onload = function() {
            if (!isNullOrEmpty(SessionId)) {
                $.mobile.path.set('#viewsdiv');
                $viewsdiv = reloadViews();
                _waitForData();
            }
            else {
                $.mobile.path.set('#logindiv');
                // this will trigger _waitForData(), but we don't want to wait here
                _handleDataCheckTimer(
                    function() {
                        // online
                        if( !$logindiv || $logindiv.length === 0 ) {
                            $logindiv = _loadLoginDiv();
                        }
                        $logindiv.cswChangePage();
                    },
                    function() {
                        // offline
                        if( !$sorrycharliediv || $sorrycharliediv.length === 0 ) {
                            $sorrycharliediv = _loadSorryCharlieDiv();
                        }
                        $sorrycharliediv.cswChangePage();
                    }
                );
            }
        };
        
        function _loadLoginDiv() {

            var LoginContent = '<p style="text-align: center;">Login to Mobile Inspection Manager</p>';
            LoginContent += '<input type="textbox" id="login_customerid" placeholder="Customer Id"/><br>';
            LoginContent += '<input type="textbox" id="login_username" placeholder="User Name"/><br>';
            LoginContent += '<input type="password" id="login_password" placeholder="Password"/><br>';
            LoginContent += '<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>';
            
            var $retDiv = _addPageDivToBody({
                    DivId: 'logindiv',
                    HeaderText: 'ChemSW Live',
                    $content: $(LoginContent),
                    HideSearchButton: true,
                    HideOnlineButton: true,
                    HideRefreshButton: true,
                    HideHelpButton: false,
                    HideBackButton: true
                });

            //.prepend( $('<img src="Images/pagelayout/header_logo32.gif" /><br/>') );
            
            var loginFailure = getStoredLocalString('loginFailure');
            if( !isNullOrEmpty(loginFailure) ) {
                _addToDivHeaderText($retDiv, loginFailure)
                    .css('color','yellow');
            }
            
            $('#loginsubmit')
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg(function() { onLoginSubmit(); });
            });
            $('#login_customerid').clickOnEnter($('#loginsubmit'));
            $('#login_username').clickOnEnter($('#loginsubmit'));
            $('#login_password').clickOnEnter($('#loginsubmit'));

            return $retDiv;
        }

        function reloadViews() {
            /// <summary>
            ///   Refreshes the viewsdiv
            /// </summary>
            var params = {
                parentlevel: -1,
                level: 0,
                DivId: 'viewsdiv',
                HeaderText: 'Views',
                HideRefreshButton: true,
                HideSearchButton: true,
                HideBackButton: true
            };
            if (!$viewsdiv) {
                $viewsdiv = _addPageDivToBody(params);
            }
            params.onPageShow = function() { return _loadDivContents(params); };

            $viewsdiv.unbindJqmEvents();
            $viewsdiv.bindJqmEvents(params);
            return $viewsdiv;
        }
        
        function _loadSorryCharlieDiv(params) {
            var p = {
                DivId: 'sorrycharliediv',
                HeaderText: 'Sorry Charlie!',
                HideHelpButton: false,
                HideOnlineButton: false,
                HideBackButton: true,
                HideRefreshButton: true,
                HideSearchButton: true,
                $content: 'You must have internet connectivity to login.'
            };
            if (params) $.extend(p, params);
            var $retDiv = _addPageDivToBody(p);
            return $retDiv;
        }

       

        // ------------------------------------------------------------------------------------
        // Online indicator
        // ------------------------------------------------------------------------------------

        function setOffline() {
            /// <summary>
            ///   Sets 'Online' button style 'offline',
            /// </summary>

            amOnline(false);
            
            $('.onlineStatus').removeClass('online')
                              .addClass('offline')
                              .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                              .text('Offline')
                              .removeClass('online')
                              .addClass('offline')
                              .end();
            
            $('.refresh').css('visibility', 'hidden');

            $viewsdiv = reloadViews(); //no changePage
        }

        function setOnline(reloadViewsPage) {
            
            amOnline(true);
            removeStoredLocalData('loginFailure');
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
                if (reloadViewsPage) {
                    $viewsdiv = reloadViews(); //no changePage
                }
            }
        }

        function amOnline(amOnline,loginFailure) {
            if(arguments.length > 0 ) {
                storeLocalData('online', isTrue(amOnline) );
            }
            if(loginFailure) {
                storeLocalData('loginFailure',loginFailure );
            }
            var ret = ( isTrue(getStoredLocalString('online')) && !mobileStorage.stayOffline());
            return ret;
        }

        // ------------------------------------------------------------------------------------
        // Logging Button
        // ------------------------------------------------------------------------------------

        function _toggleLogging() {
            var logging = !doLogging();
            doLogging(logging);
            if (logging) {
                setStartLog();
            } else {
                setStopLog();
            }
        }

        function setStartLog() {
            if (doLogging()) {
                var logger = new profileMethod('setStartLog');
                cacheLogInfo(logger);
                $('.debug').removeClass('debug-off')
                            .addClass('debug-on')
                            .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                            .text('Sync Log')
                            .addClass('debug-on')
                            .removeClass('debug-off')
                            .end();
            }
        }

        function setStopLog() {
            if (!doLogging()) {
                $('.debug').removeClass('debug-on')
                            .addClass('debug-off')
                            .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                            .text('Sync Log')
                            .addClass('debug-off')
                            .removeClass('debug-on')
                            .end();
                var logger = new profileMethod('setStopLog');
                cacheLogInfo(logger);

                var dataJson = {
                    'Context': 'CswMobile',
                    'UserName': getStoredLocalString('username'),
                    'CustomerId': getStoredLocalString('customerid'),
                    'LogInfo': getStoredLocalString('debuglog')
                };

//                CswAjaxJSON({
//                        url: opts.SendLogUrl,
//                        data: dataJson,
//                        success: function() {
//                            $loggingBtn.removeClass('debug-on')
//                                            .addClass('debug-off')
//                                            .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
//                                            .text('Start Log')
//                                .end();
//                            purgeLogInfo();
//                        }
//                    });

                var params = {
                    DivId: 'loginfodiv',
                    HeaderText: 'Log Info',
                    HideHelpButton: false,
                    HideOnlineButton: false,
                    HideBackButton: false,
                    HideRefreshButton: true,
                    HideSearchButton: true,
                    $content: $( JSON.parse(sessionStorage['debuglog']))
                };
                var $logDiv = _addPageDivToBody(params);
                $logDiv.unbindJqmEvents();
                $logDiv.bindJqmEvents(params);
                $logDiv.cswChangePage();
            }
        }

        // ------------------------------------------------------------------------------------
        // List items fetching
        // ------------------------------------------------------------------------------------

        function _loadDivContents(params) {
            var logger = new profileMethod('loadDivContents');

            startLoadingMsg();
            kickStartAutoSync();
            
            var p = {
                ParentId: '',
                level: 1,
                DivId: '',
                HeaderText: '',
                HideRefreshButton: false,
                HideSearchButton: false,
                json: '',
                SessionId: SessionId,
                PageType: 'search'
            };
            if (params) $.extend(p, params);

            var viewId = (p.level < 2) ? currentViewId(p.DivId) : currentViewId();
       
            var $retDiv = $('#' + p.DivId);

            if (isNullOrEmpty($retDiv) || $retDiv.length === 0 || $retDiv.find('div:jqmData(role="content")').length === 1) {
                if (p.level === 0) {
                    p.PageType = 'view';
                    if (!amOnline()) {
                        p.json = _fetchCachedViewJson(p.DivId);
                        $retDiv = _loadDivContentsJson(p);
                    } else {
                        p.url = opts.ViewsListUrl;
                        $retDiv = _getDivJson(p);
                    }
                } else if (p.level === 1) {
                    // case 20354 - try cached first
                    var cachedJson = _fetchCachedViewJson(viewId);
                    p.PageType = 'node';
                    if (!isNullOrEmpty(cachedJson)) {
                        p.json = cachedJson;
                        $retDiv = _loadDivContentsJson(p);
                    } else if (amOnline()) {
                        p.url = opts.ViewUrl;
                        $retDiv = _getDivJson(p);
                    } else {
                        stopLoadingMsg();
                    }
                    
                } else { // Level 2 and up
                    var cachedJson = _fetchCachedNodeJson(p.DivId);
                    p.PageType = 'tab';
                    if( !isNullOrEmpty(cachedJson) ) {
                        p.json = cachedJson['subitems'];

                        if (!isNullOrEmpty(p.json)) {
                            $retDiv = _loadDivContentsJson(p);
                        }
                    } else {
                        stopLoadingMsg();
                    }
                }
            }
            cacheLogInfo(logger);
            return $retDiv;
        } // _loadDivContents()

        function _loadDivContentsJson(params) {
            params.parentlevel = params.level;
            var $retDiv = _processViewJson(params);
            return $retDiv;
        }

        function _getDivJson(params) {
            var logger = new profileMethod('getDivJson');
            var $retDiv = undefined;

            var p = {
                url: opts.ViewUrl
            };
            $.extend(p, params);

            var jsonData = {
                SessionId: p.SessionId,
                ParentId: p.DivId,
                ForMobile: ForMobile
            };
        
            CswAjaxJSON({
                    //async: false,   // required so that the link will wait for the content before navigating
                    formobile: ForMobile,
                    url: p.url,
                    data: jsonData,
                    onloginfail: function(text) { onLoginFail(text); },
                    success: function(data) {
                        setOnline(false);
                        logger.setAjaxSuccess();
                        
                        p.json = data;
                        var searchJson = { };
                        if( params.level === 1) {
                            searchJson = data['searches'];
                        }
                        if( params.level !== 0) {
                            p.json = data['nodes'];    
                        }
                        if( params.level < 2) {
                            _storeViewJson(p.DivId, p.HeaderText, p.json, params.level, searchJson);
                        }
                        $retDiv = _loadDivContentsJson(p);
                    },
                    error: function() {
                        onError();
                    }
                });
            cacheLogInfo(logger);
            return $retDiv;
        }

        var currenttab;

        function _processViewJson(params) {
            var logger = new profileMethod('processViewJson');
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                json: '',
                parentlevel: '',
                PageType: '',
                level: '',
                HideSearchButton: false,
                HideOnlineButton: false,
                HideRefreshButton: false,
                HideHelpButton: false,
                HideBackButton: false,
                nextTab: ''
            };
            if (params) $.extend(p, params);

            var $retDiv = _addPageDivToBody({
                    ParentId: p.ParentId,
                    level: p.parentlevel,
                    DivId: p.DivId,
                    HeaderText: p.HeaderText,
                    HideSearchButton: p.HideSearchButton,
                    HideOnlineButton: p.HideOnlineButton,
                    HideRefreshButton: p.HideRefreshButton,
                    HideHelpButton: p.HideHelpButton,
                    HideBackButton: p.HideBackButton
                });

            var $content = $retDiv.find('div:jqmData(role="content")').empty();

            var $list = $content.cswUL({cssclass: 'csw_listview'});
            currenttab = '';
            
            for(var key in p.json)
            {
                var item = { };
                $.extend(item, p);
                item.json = { id: key, value: p.json[key] };
                _makeListItemFromJson($list, item)
                    .CswAttrXml('data-icon', false) //hides the arrow
                    .appendTo($list);
            }

            if( !isNullOrEmpty(p.nextTab) ) {
                var item = { };
                $.extend(item, p);
                item.json = { id: p.nextTab };
                item.PageType = 'tab';
                item.DivId = item.ParentId;
                item.suppressProps = true;
                _makeListItemFromJson($list, item)
                    //.CswAttrXml('data-icon', true) //show the arrow
                    .appendTo($list);
            }
            
            logger.setAjaxSuccess();
            
            _resetPendingChanges();
            
            if(!mobileStorage.stayOffline()) {
                _toggleOffline(false);
            }
            cacheLogInfo(logger);

            stopLoadingMsg();
            
            return $retDiv;
        } // _processViewJson()

        function _makeListItemFromJson($list, params) {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                json: '',
                PageType: '',
                parentlevel: '',
                level: '',
                HideRefreshButton: false,
                HideSearchButton: false,
                nextTab: '',
                suppressProps: false
            };
            if (params) $.extend(p, params);

            var id = makeSafeId({ ID: p.json['id'] });
            var text = '';

            var IsDiv = (!isNullOrEmpty(id));

            var $retLI = $('');

            switch (p.PageType) {
                case 'search':
                        // ignore this
                    break;
                case 'node':
                    text = p.json['value']['node_name'];
                    $retLI = _makeObjectClassContent(p)
                                            .appendTo($list);
                    break;
                case 'tab':
                    {
                        text = id;
                        id = makeSafeId({prefix: id.replace(' ', ''), ID: p.DivId }); //we prefer nodeid_ to be at the end
                        $retLI = $('<li></li>')
                                    .appendTo($list);
                        $retLI.CswLink('init', { href: 'javascript:void(0);', value: text })
                                            .css('white-space', 'normal')
                                            .CswAttrXml({
                                            'data-identity': id,
                                            'data-url': id
                                        });
                        
                        var nextTab = (!isNullOrEmpty(p.json['value'])) ? p.json['value']['nexttab'] : '';
                        if( !isNullOrEmpty(nextTab)) {
                            //we're creating a tab link on a prop
                            delete p.json['value']['nexttab'];
                        }

                        if( !p.suppressProps) {
                            //we're creating a tab link which needs child props
                            setTimeout(function() {
                                _processViewJson({
                                        ParentId: p.DivId,
                                        DivId: id,
                                        HeaderText: text,
                                        json: p.json['value'],
                                        parentlevel: p.level,
                                        level: p.level + 1,
                                        PageType: 'prop',
                                        nextTab: nextTab,
                                        suppressProps: true
                                    });
                            }, 500);
                        }
                        break;   
                    } // case 'prop':
                case 'prop':
                    {
                        if( !isNullOrEmpty(p.json['value']) && !isNullOrEmpty(p.json['id']) ) {
                            _FieldTypeJsonToHtml(p.json['value'], p.DivId, p.json['id'])
                                .appendTo($list);
                        }
                        break;
                    }
                default:
                    {
                        text = p.json['value'];
                        $retLI = $('<li></li>').appendTo($list);
                        if (IsDiv) {
                            $retLI.CswLink('init', { href: 'javascript:void(0);', value: text })
                                              .css('white-space', 'normal')
                                              .CswAttrXml({
                                              'data-identity': id,
                                              'data-url': id
                                          });
                        } else {
                            $retLI.val(text);
                        }
                        break;
                    }// default:
            }

            $retLI.unbind('click');
            $retLI.bind('click', function() {
                return startLoadingMsg(function() {
                    var par = {ParentId: p.DivId,
                        parentlevel: p.parentlevel,
                        level: p.parentlevel + 1,
                        DivId: id,
                        persistBindEvent: true,
                        HeaderText: text  };
                    var $div = _addPageDivToBody(par);
                    par.onPageShow = function() { return _loadDivContents(par); };
                    $div.unbindJqmEvents();
                    $div.bindJqmEvents(par);
                    $div.cswChangePage({ reloadPage: true });
                });
            });
            
            return $retLI;
        }// _makeListItemFromJson()

        function _makeObjectClassContent(params) {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                json: '',
                parentlevel: '',
                HideRefreshButton: false,
                HideSearchButton: false
            };
            if (params) $.extend(p, params);

            var $retHtml;
            var Html = '';
            var id = makeSafeId({ ID: p.json['id'] });
            var nodeSpecies = p.json['value']['nodespecies'];
            var NodeName = p.json['value']['node_name'];
            var icon = '';
            if (!isNullOrEmpty(p.json['value']['iconfilename'])) {
                icon = 'images/icons/' + p.json['value']['iconfilename'];
            }
            var ObjectClass = p.json['value']['objectclass'];

            if( nodeSpecies !== 'More' )
            {
                switch (ObjectClass) {
                case "InspectionDesignClass":
                    var DueDate = tryParseString(p.json['value']['duedate'],'' );
                    var Location = tryParseString(p.json['value']['location'],'' );
                    var MountPoint = tryParseString(p.json['value']['target'],'' );
                    var Status = tryParseString(p.json['value']['status'],'' );
//Case 22579: just remove for now
//                var UnansweredCnt = 0;

//                p.$xmlitem.find('prop[fieldtype="Question"]').each(function() {
//                    var $question = $(this).clone();
//                    if (isNullOrEmpty($question.children('Answer').text())) {
//                        UnansweredCnt++;
//                    }
//                });

                    Html += '<li>';
                    Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">';
                    if (!isNullOrEmpty(icon))
                        Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<h2>' + NodeName + '</h2>';
                    Html += '<p>' + Location + '</p>';
                    Html += '<p>' + MountPoint + '</p>';
                    Html += '<p>';
                    if (!isNullOrEmpty(Status)) Html += Status + ', ';
                    Html += 'Due: ' + DueDate + '</p>';
//                Html += '<span id="' + makeSafeId({ prefix: id, ID: 'unansweredcnt' }) + '" class="ui-li-count">' + UnansweredCnt + '</span>';
                    Html += '</a>';
                    Html += '</li>';
                    break;
                default:
                    Html += '<li>';
                    if (!isNullOrEmpty(icon))
                        Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">' + NodeName + '</a>';
                    Html += '</li>';
                    break;
                }
            } //if( nodeSpecies !== 'More' )
            else {
                Html += '<li>';
                Html += '<h2 id="' + id + '">' + NodeName + '</h2>';
                Html += '</li>';
            }
            $retHtml = $(Html);
            
            return $retHtml;
        }

        function _FieldTypeJsonToHtml(json, ParentId, IdStr) {
            /// <summary>
            ///   Converts JSON into DOM content
            /// </summary>
            /// <param name="json" type="Object">A JSON Object</param>
            /// <param name="ParentId" type="String">The ElementID of the parent control (should be a prop)</param>
            /// <param name="IdStr" type="String">The ElementID of the child control</param>
            var $retLi = $('');
            if( !isNullOrEmpty(json)) {
                var FieldType = json['fieldtype'];
                var PropName = json['prop_name'];
                var ReadOnly = isTrue(json['isreadonly']);

                // Subfield values
                var sf_text = tryParseString(json['text'], '');
                var sf_value = tryParseString(json['value'], '');
                var sf_href = tryParseString(json['href'], '');
                var sf_checked = tryParseString(json['checked'], '');
                //var sf_relationship = tryParseString(json['value']['name'], '');
                var sf_required = tryParseString(json['required'], '');
                var sf_units = tryParseString(json['units'], '');
                var sf_answer = tryParseString(json['answer'], '');
                var sf_allowedanswers = tryParseString(json['allowedanswers'], '');
                var sf_correctiveaction = tryParseString(json['correctiveaction'], '');
                var sf_comments = tryParseString(json['comments'], '');
                var sf_compliantanswers = tryParseString(json['compliantanswers'], '');
                var sf_options = tryParseString(json['options'], '');

                $retLi = $('<li id="' + IdStr + '_li"></li>')
                    .CswAttrXml('data-icon', false);
                var $label = $('<h2 id="' + IdStr + '_label" style="white-space:normal;" class="csw_prop_label">' + PropName + '</h2>')
                    .appendTo(($retLi));

                var $fieldcontain = $('<div class="csw_fieldset" ></div>')
                    .appendTo($retLi);

                var $propDiv = $('<div></div>');

                if (FieldType === "Question" &&
                    !(sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) &&
                        isNullOrEmpty(sf_correctiveaction)) {
                    $label.addClass('OOC');
                } else {
                    $label.removeClass('OOC');
                }

                var $prop;
                var propId = IdStr + '_input';

                if (!ReadOnly) {
                    var addChangeHandler = true;

                    switch (FieldType) {
                    case "Date":
                        $prop = $propDiv.CswInput('init', { type: CswInput_Types.date, ID: propId, value: sf_value });
                        break;
                    case "Link":
                        $prop = $propDiv.CswLink('init', { ID: propId, href: sf_href, rel: 'external', value: sf_text });
                        break;
                    case "List":
                        $prop = $('<select class="csw_prop_select" name="' + propId + '" id="' + propId + '"></select>')
                            .appendTo($propDiv)
                            .selectmenu();
                        var selectedvalue = sf_value;
                        var optionsstr = sf_options;
                        var options = optionsstr.split(',');
                        for (var i = 0; i < options.length; i++) {
                            var $option = $('<option value="' + options[i] + '"></option>')
                                .appendTo($prop);
                            if (selectedvalue === options[i]) {
                                $option.CswAttrDom('selected', 'selected');
                            }

                            if (!isNullOrEmpty(options[i])) {
                                $option.val(options[i]);
                            } else {
                                $option.valueOf('[blank]');
                            }
                        }
                        $prop.selectmenu('refresh');
                        break;
                    case "Logical":
                        addChangeHandler = false; //_makeLogicalFieldSet() does this for us
                        _makeLogicalFieldSet(ParentId, IdStr, sf_checked, sf_required)
                            .appendTo($fieldcontain);
                        break;
                    case "Memo":
                        $prop = $('<textarea name="' + propId + '">' + sf_text + '</textarea>')
                            .appendTo($propDiv);
                        break;
                    case "Number":
                        sf_value = tryParseNumber(sf_value, '');
                        $prop = $propDiv.CswInput('init', { type: CswInput_Types.number, ID: propId, value: sf_value });
                        break;
                    case "Password":
                            //nada
                        break;
                    case "Quantity":
                        $prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value })
                            .append(sf_units);
                        break;
                    case "Question":
                        addChangeHandler = false; //_makeQuestionAnswerFieldSet() does this for us
                        _makeQuestionAnswerFieldSet(ParentId, IdStr, sf_allowedanswers, sf_answer, sf_compliantanswers)
                            .appendTo($fieldcontain);
                        var hideComments = true;
                        if (!isNullOrEmpty(sf_answer) &&
                            (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') < 0 &&
                                isNullOrEmpty(sf_correctiveaction)) {
                            $label.addClass('OOC');
                            hideComments = false;
                        }

                        $prop = $('<div data-role="collapsible" class="csw_collapsible" data-collapsed="' + hideComments + '"><h3>Comments</h3></div>')
                            .appendTo($retLi);

                        var $corAction = $('<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action">' + sf_correctiveaction + '</textarea>')
                            .appendTo($prop);

                        if (sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) {
                            $corAction.css('display', 'none');
                        }
                        $corAction.unbind('change');
                        $corAction.bind('change', function(eventObj) {
                            var $cor = $(this);
                            if ($cor.val() === '') {
                                $label.addClass('OOC');
                            } else {
                                $label.removeClass('OOC');
                            }
                            onPropertyChange(ParentId, eventObj, $cor.val(), IdStr + '_cor', IdStr);
                        });

                        $('<textarea name="' + IdStr + '_input" id="' + IdStr + '_input" placeholder="Comments">' + sf_comments + '</textarea>')
                            .appendTo($prop)
                            .unbind('change')
                            .bind('change', function(eventObj) {
                                var $com = $(this);
                                onPropertyChange(ParentId, eventObj, $com.val(), IdStr + '_com', IdStr);
                            });
                        break;
                    case "Static":
                        $propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + sf_text + '</p>'));
                        break;
                    case "Text":
                        $prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_text });
                        break;
                    case "Time":
                        $prop = $propDiv.CswInput('init', { type: CswInput_Types.time, ID: propId, value: sf_value });
                        break;
                    default:
                        $propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['gestalt'] + '</p>'));
                        break;
                    } // switch (FieldType)

                    if (addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0) {
                        $prop.unbind('change').bind('change', function(eventObj) {
                            var $this = $(this);
                            onPropertyChange(ParentId, eventObj, $this.val(), propId, Id);
                        });
                    }
                } else {
                    $propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['gestalt'] + '</p>'));
                }
                if ($propDiv.children().length > 0) {
                    $fieldcontain.append($propDiv);
                }
            }
            return $retLi;
        }

        function _FieldTypeHtmlToJson(json, elementId, propId, value) {
            /// <summary>
            ///   Converts DOM content back to JSON
            /// </summary>
            /// <param name="json" type="Object">A JSON Object</param>
            /// <param name="elementId" type="String">The id of the DOM element</param>
            /// <param name="propId" type="String">The id of the property</param>
            /// <param name="value" type="String">The stored value</param>
            
            var elementName = new CswString(elementId);
            var propName = tryParseString(makeSafeId({ ID: json['id'] }), propId);
            var fieldtype = json['fieldtype'];
            //var propname = json.name;

            // subfield nodes
            var sf_text = 'text';
            var sf_value = 'value';
            //var $sf_href = json.href;
            //var $sf_options = json.options;
            var sf_checked = 'checked';
            //var $sf_required = json.required;
            //var $sf_units = json.units;
            var sf_answer = 'answer';
            //var $sf_allowedanswers = json.allowedanswers;
            var sf_correctiveaction = 'correctiveaction';
            var sf_comments = 'comments';
            //var $sf_compliantanswers = json.compliantanswers;

            var propToUpdate = '';
            switch (fieldtype) {
                case "Date":
                    if (elementName.contains(propName)) {
                        propToUpdate = sf_value;
                    }
                    break;
                case "Link":
                    break;
                case "List":
                    if (elementName.contains(propName)) {
                        propToUpdate = sf_value;
                    }
                    break;
                case "Logical":
                    if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
                        propToUpdate = sf_checked;
                    }
                    break;
                case "Memo":
                    if (elementName.contains(propName)) {
                        propToUpdate = sf_text;
                    }
                    break;
                case "Number":
                    if (elementName.contains(propName)) propToUpdate = sf_value;
                    break;
                case "Password":
                    break;
                case "Quantity":
                    if (elementName.contains(propName)) {
                        propToUpdate = sf_value;
                    }
                    break;
                case "Question":
                    if (elementName.contains(makeSafeId({ ID: propName, suffix: 'com' }))) {
                        propToUpdate = sf_comments;
                    } 
                    else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
                        propToUpdate = sf_answer;
                    } 
                    else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'cor' }))) {
                        propToUpdate = sf_correctiveaction;
                    }
                    break;
                case "Static":
                    break;
                case "Text":
                    if (elementName.contains(propName)) {
                        propToUpdate = sf_text;
                    }
                    break;
                case "Time":
                    if (elementName.contains(propName)) {
                        propToUpdate = sf_value;
                    }
                    break;
                default:
                    break;
            }
            if (!isNullOrEmpty(propToUpdate)) {
                json[propToUpdate] = value;
                json['wasmodified'] = '1';
            }
        }// _FieldTypeHtmlToJson()

        function _makeLogicalFieldSet(ParentId, IdStr, Checked, Required) {
            var Suffix = 'ans';
            var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
                                         .CswAttrDom({
                                         'class': 'csw_fieldset',
                                         'id': IdStr + '_fieldset'
                                     })
                                         .CswAttrXml({
                                         'data-role': 'controlgroup',
                                         'data-type': 'horizontal'
                                     });
                                     
            var answers = ['True', 'False'];
            if ( !isTrue(Required)) {
                answers.push = 'Null';
            }
            var inputName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unique and shared

            for (var i = 0; i < answers.length; i++) {
                var answertext = '';
                switch (answers[i]) {
                case 'Null':
                    answertext = '?';
                    break;
                case 'True':
                    answertext = 'Yes';
                    break;
                case 'False':
                    answertext = 'No';
                    break;
                }

                var inputId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });

                $fieldset.append('<label for="' + inputId + '">' + answertext + '</label>');
                var $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] })

                // Checked is a Tristate, so isTrue() is not useful here
                if ((Checked === 'false' && answers[i] === 'False') ||
                    (Checked === 'true' && answers[i] === 'True') ||
                        (Checked === '' && answers[i] === 'Null')) {
                    $input.CswAttrXml({ 'checked': 'checked' });
                }
                
                $input.unbind('change');
                $input.bind('change', function(eventObj) {
                    var $this = $(this);
                    var thisInput = $this.val();
                    onPropertyChange(ParentId, eventObj, thisInput, inputId, IdStr);
                    return false;
                });
            } // for (var i = 0; i < answers.length; i++)
            return $fieldset;
        }// _makeLogicalFieldSet()

        function _makeQuestionAnswerFieldSet(ParentId, IdStr, Options, Answer, CompliantAnswers) {
            var Suffix = 'ans';
            var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
    								    .CswAttrDom({
								        'id': IdStr + '_fieldset'
								    })
    								    .CswAttrXml({
								        'data-role': 'controlgroup',
								        'data-type': 'horizontal',
    								    'data-theme': 'b'
								    });
            var answers = Options.split(',');
            var answerName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unqiue and shared

            for (var i = 0; i < answers.length; i++) {
                var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });

                $fieldset.append('<label for="' + answerid + '" id="' + answerid + '_lab">' + answers[i] + '</label');
                var $answer = $('<input type="radio" name="' + answerName + '" id="' + answerid + '" class="csw_answer" value="' + answers[i] + '" />')
                                .appendTo($fieldset);
                
                if (Answer === answers[i]) {
                     $answer.CswAttrDom('checked', 'checked');
                }
                $answer.unbind('change');
                $answer.bind('change', function(eventObj) {

                    var thisAnswer = $(this).val();

                    var correctiveActionId = makeSafeId({ prefix: IdStr, ID: 'cor' });
                    var liSuffixId = makeSafeId({ prefix: IdStr, ID: 'label' });

                    var $cor = $('#' + correctiveActionId);
                    var $li = $('#' + liSuffixId);

                    if ((',' + CompliantAnswers + ',').indexOf(',' + thisAnswer + ',') >= 0) {
                        $cor.css('display', 'none');
                        $li.removeClass('OOC');

                    } else {
                        $cor.css('display', '');

                        if (isNullOrEmpty($cor.val())) {
                            $li.addClass('OOC');
                        } else {
                            $li.removeClass('OOC');
                        }
                    }
                    onPropertyChange(ParentId, eventObj, thisAnswer, answerName, IdStr);

                    return false;
                });
            } // for (var i = 0; i < answers.length; i++)
            return $fieldset;
        } // _makeQuestionAnswerFieldSet()
        
        function _addPageDivToBody(params) {
            var p = {
                ParentId: undefined,
                level: 1,
                DivId: '',       // required
                HeaderText: '',
                $toolbar: $(''),
                $content: $(''),
                HideSearchButton: false,
                HideOnlineButton: false,
                HideRefreshButton: false,
                HideHelpButton: false,
                HideBackButton: false,
                HideHeaderOnlineButton: true
            };

            if (params) {
                $.extend(p, params);
            }

            p.DivId = makeSafeId({ ID: p.DivId });

            var $pageDiv = $('#' + p.DivId);

            var $searchBtn = $('#' + p.DivId + '_searchopen');
            var $syncstatusBtn = $('#' + p.DivId + '_gosyncstatus');
            var $refreshBtn = $('#' + p.DivId + '_refresh');
            var $helpBtn = $('#' + p.DivId + '_help');
            var $headerOnlineBtn = $('#' + p.DivId + '_headeronline');
            var $headerTitle = $('#' + p.DivId + '_header_title');
            var $backlink = $('#' + p.DivId + '_back');
            
            if (isNullOrEmpty($pageDiv) || $pageDiv.length === 0) {
                $pageDiv = $body.CswDiv('init', { ID: p.DivId })
                                        .CswAttrXml({
                                        'data-role': 'page',
                                        'data-url': p.DivId,
                                        'data-title': p.HeaderText,
                                        'data-rel': 'page'
                                    });

                var $header = $pageDiv.CswDiv('init', { ID: p.DivId + '_header' })
                                              .CswAttrXml({
                                              'data-role': 'header',
                                              'data-theme': opts.Theme,
                                              'data-position': 'fixed',
                                              'data-id': 'csw_header'
                                          });
                $backlink = $header.CswLink('init', {
                                                'href': 'javascript:void(0)',
                                                ID: p.DivId + '_back',
                                                cssclass: 'ui-btn-left',
                                                value: 'Back'
                                            })
                                                .CswAttrXml({
                                                'data-identity': p.DivId + '_back', 
                                                'data-rel': 'back',
                                                'data-direction': 'reverse',
                                                'data-icon': 'arrow-l'
                                            });
                
                $headerTitle = $('<h1 id="' + p.DivId + '_header_title"></h1>').appendTo($header);                

                $searchBtn = $header.CswLink('init', {
                                              'href': 'javascript:void(0)',
                                              ID: p.DivId + '_searchopen',
                                              cssclass: 'ui-btn-right',
                                              value: 'Search'
                                          })
                                              .CswAttrXml({
                                              'data-identity': p.DivId + '_searchopen',
                                              'data-url': p.DivId + '_searchopen',
                                              'data-transition': 'pop',
                                              'data-rel': 'dialog'
                                          });

                $headerOnlineBtn = $header.CswLink('init', {
                                                  ID: p.DivId + '_headeronline',
                                                  cssclass: 'ui-btn-right onlineStatus online',
                                                  value: 'Online'
                                                    })
                                                  .CswAttrDom({ 'disabled': 'disabled' })
                                                  .hide();

                $header.CswDiv('init', { cssclass: 'toolbar' })
                               .append(p.$toolbar)
                               .CswAttrXml({ 'data-role': 'header', 'data-type': 'horizontal', 'data-theme': opts.Theme });

                $pageDiv.CswDiv('init', { ID: p.DivId + '_content' })
                                               .CswAttrXml({ 'data-role': 'content', 'data-theme': opts.Theme })
                                               .append(p.$content);
                var $footer = $pageDiv.CswDiv('init', { ID: p.DivId + '_footer' })
                                              .CswAttrXml({
                                              'data-role': 'footer',
                                              'data-theme': opts.Theme,
                                              'data-position': 'fixed',
                                              'data-id': 'csw_footer'
                                          });

                var $footerNav = $('<div data-role="navbar">').appendTo($footer);
                var $footerCtn = $('<ul></ul>').appendTo($footerNav);
                var onlineValue = (!amOnline()) ? 'Offline' : 'Online';

                $syncstatusBtn = $('<li></li>').CswLink('init', {
                                                        'href': 'javascript:void(0)',
                                                        ID: p.DivId + '_gosyncstatus',
                                                        cssclass: 'ui-btn-active onlineStatus ' + onlineValue.toLowerCase(), 
                                                        value: onlineValue
                                                    })
                                                        .CswAttrXml({
                                                        'data-identity': p.DivId + '_gosyncstatus',
                                                        'data-url': p.DivId + '_gosyncstatus',
                                                        'data-transition': 'pop',
                                                        'data-rel': 'dialog',
                                                        'data-icon': 'gear'
                                                    })
                                                    .css('display', '')
                                                    .appendTo($footerCtn);

                $refreshBtn = $('<li></li>').CswLink('init', {
                                                           'href': 'javascript:void(0)',
                                                           ID: p.DivId + '_refresh',
                                                           value: 'Refresh',
                                                           cssclass: 'refresh'
                                                       }) 
                                                           .CswAttrXml({
                                                           'data-identity': p.DivId + '_refresh',
                                                           'data-url': p.DivId + '_refresh',
                                                           'data-icon': 'refresh'
                                                       })
                                                       .css('display', '')
                                                       .appendTo($footerCtn);

                $('<li></li>').CswLink('init', { href: 'Main.html', rel: 'external', ID: p.DivId + '_main', value: 'Full Site' })
                              .CswAttrXml({ 'data-transition': 'pop', 'data-icon': 'home' })
                              .appendTo($footerCtn);

                $helpBtn = $('<li></li>').CswLink('init', {
                                             'href': 'javascript:void(0)',
                                             ID: p.DivId + '_help',
                                             value: 'Help'
                                      })
                                      .CswAttrXml({
                                             'data-identity': p.DivId + '_help',
                                             'data-url': p.DivId + '_help',
                                             'data-transition': 'pop',
                                             'data-rel': 'dialog',
                                             'data-icon': 'info'
                                      })
                                      .css('display', '')
                                      .appendTo($footerCtn);
            }

            //case 22323
            $headerTitle.text(p.HeaderText);
            
            if (!p.HideBackButton) {
                $backlink.css('display', '').show();
            } else {
                $backlink.css('display', 'none').hide();
            }
            if (p.HideOnlineButton) {
                $syncstatusBtn.css('display', 'none').hide();
            } else {
                $syncstatusBtn.css('display', '').show();
            }
            if( _pendingChanges() ) {
                $syncstatusBtn.addClass('pendingchanges');
            }
            else {
                $syncstatusBtn.removeClass('pendingchanges');
            }
            if (p.HideHelpButton) {
                $helpBtn.css('display', 'none').hide();
            } else {
                $helpBtn.css('display', '').show();
            }
            if (p.HideRefreshButton || !amOnline() ) {
                $refreshBtn.css('display', 'none').hide();
            } else {
                $refreshBtn.css('display', '').show();
            }
            if (p.HideSearchButton) {
                $searchBtn.css('display', 'none').hide();
            } else {
                $searchBtn.css('display', '').show();
            }
            if (p.HideHeaderOnlineButton) {
                $headerOnlineBtn.css('display', 'none').hide();
            } else {
                $headerOnlineBtn.css('display', '').show();
            }

            _bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv);
            return $pageDiv;

        }// _addPageDivToBody()

        function _getDivHeaderText(DivId) {
            return $('#' + DivId).find('div:jqmData(role="header") h1').text();
        }

        function _addToDivHeaderText($div, text, style) {
            var $ret = $('<p white-space: normal;">' + text + '</p>');
            
            $div.find('div:jqmData(role="header") h1')
                .css('white-space','normal')
                .append($ret);
            
            return $ret;
        }
        
        function _bindPageEvents(DivId, ParentId, level, $div) {
            $div.find('#' + DivId + '_searchopen')
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg( function () { onSearchOpen(DivId); });
                })
                .end()
                .find('#' + DivId + '_gosyncstatus')
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg( function () { onSyncStatusOpen(DivId); });
                })
                .end()
                .find('#' + DivId + '_refresh')
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg( function () { onRefresh(); });
                })
                .end()
                .find('#' + DivId + '_help')
                .unbind('click')
                .bind('click', function() {
                    return startLoadingMsg( function () { onHelp(DivId, ParentId); });
                })
//                .end()
//                .find('textarea')
//                .unbind('change')
//                .bind('change', function(eventObj) {
//                    var $this = $(this);
//                    onPropertyChange(DivId, eventObj, $this.val(), $this.CswAttrDom('id'));
//                })
//                .end()
//                .find('.csw_prop_select')
//                .unbind('change')
//                .bind('change', function(eventObj) {
//                    var $this = $(this);
//                    onPropertyChange(DivId, eventObj, $this.val(), $this.CswAttrDom('id'));
//                })
                .end();
        }

        // ------------------------------------------------------------------------------------
        // Sync Status Div
        // ------------------------------------------------------------------------------------

        function _makeSyncStatusDiv() {
            var content = '';
            content += '<p>Pending Unsynced Changes: <span id="ss_pendingchangecnt">' + tryParseString(getStoredLocalString('unSyncedChanges'),'0') + '</span></p>';
            content += '<p>Last Sync Success: <span id="ss_lastsync_success">' + mobileStorage.lastSyncSuccess + '</span></p>';
            var hideFailure = isNullOrEmpty(mobileStorage.lastSyncAttempt) ? '' : 'none';
            content += '<p style="display:' + hideFailure + ' ;">Last Sync Failure: <span id="ss_lastsync_attempt">' + mobileStorage.lastSyncAttempt + '</span></p>';
            content += '<a id="ss_forcesync" data-identity="ss_forcesync" data-url="ss_forcesync" href="javascript:void(0)" data-role="button">Force Sync Now</a>';
            content += '<a id="ss_gooffline" data-identity="ss_gooffline" data-url="ss_gooffline" href="javascript:void(0)" data-role="button">Go Offline</a>';
            content += '<br/>';
            content += '<a id="ss_logout" data-identity="ss_logout" data-url="ss_logout" href="javascript:void(0)" data-role="button">Logout</a>';
            if( debugOn() ) {
                content += '<a id="ss_debuglog" class="debug" data-identity="ss_debuglog" data-url="ss_debuglog" href="javascript:void(0)" data-role="button">Start Logging</a>';
            }
            
            var $retDiv = _addPageDivToBody({
                    DivId: 'syncstatus',
                    HeaderText: 'Sync Status',
                    $content: $(content),
                    HideSearchButton: true,
                    HideOnlineButton: true,
                    HideRefreshButton: false,
                    HideHelpButton: false,
                    HideBackButton: false,
                    HideHeaderOnlineButton: false
                });
                   
            $retDiv.find('#ss_forcesync')
                    .bind('click', function() {
                        return startLoadingMsg( function () {
                             _processChanges(false);
                        });
                    })
                    .end()
                    .find('#ss_gooffline')
                    .bind('click', function() {
                        var stayOffline = !mobileStorage.stayOffline();
                        mobileStorage.stayOffline(stayOffline);
                        _toggleOffline(true);
                        return false;
                    })
                    .end()
                    .find('#ss_logout')
                    .bind('click', function() {
                        return startLoadingMsg( function () { onLogout(); });
                    })
                    .end()
                    .find('#ss_debuglog')
                    .bind('click', function() {
                        _toggleLogging();
                        return false;
                    })
                    .end();

            return $retDiv;
        }

        function _toggleOffline(doWaitForData) {

            var $onlineBtn = $('#ss_gooffline span').find('span.ui-btn-text');
            if (amOnline() || $onlineBtn.text() === 'Go Online') {
                setOnline(false);
                if (doWaitForData) {
                    _clearWaitForData();
                    _waitForData();
                }
                $onlineBtn.text('Go Offline');
                $('.refresh').each(function(){
                    var $this = $(this);
                    $this.css({'display': ''}).show();
                });
            }
            else {
                setOffline();
                if (doWaitForData) {
                    _clearWaitForData();
                }
                $onlineBtn.text('Go Online');
                $('.refresh').each(function(){
                    var $this = $(this);
                    $this.css({'display': 'none'}).hide();
                });
            }
        }

        function _resetPendingChanges(succeeded) {
            if ( _pendingChanges() ) {
                $('.onlineStatus').addClass('pendingchanges')
                                  .find('span.ui-btn-text')
                                  .addClass('pendingchanges');
            } else {
                $('.onlineStatus').removeClass('pendingchanges')
                                  .find('span.ui-btn-text')
                                  .removeClass('pendingchanges');
            }
            
            if(arguments.length === 1) {
                if (succeeded) {
                    mobileStorage.clearUnsyncedChanges();
                    $('#ss_lastsync_success').text(mobileStorage.lastSyncSuccess());
                }
                else {
                    $('#ss_lastsync_attempt').text(mobileStorage.lastSyncAttempt());
                }
            }
        }

        // returns true if no pending changes or user is willing to lose them
        function _checkNoPendingChanges() {
            var pendingChanges = (!_pendingChanges() ||
                confirm('You have pending unsaved changes.  These changes will be lost.  Continue?'));
            return pendingChanges;
        }

        function _pendingChanges() {
            var changes = new Number(tryParseString(getStoredLocalString('unSyncedChanges'),'0'))
            return (changes > 0);
        }

        // ------------------------------------------------------------------------------------
        // Help Div
        // ------------------------------------------------------------------------------------

        function _makeHelpDiv() {
            var $help = $('<p>Help</p>');

            if (debugOn()) //this is set onLoad based on the includes variable 'debug'
            {
                $help.append('</br></br></br>');
                var $logLevelDiv = $help.CswDiv('init')
                                        .CswAttrXml({ 'data-role': 'fieldcontain' });
                $('<label for="mobile_log_level">Logging</label>')
                                        .appendTo($logLevelDiv);

                $logLevelDiv.CswSelect('init', {
                                                ID: 'mobile_log_level',
                                                selected: debugOn() ? 'on' : 'off',
                                                values: [{ value: 'off', display: 'Logging Disabled' },
                                                    { value: 'on', display: 'Logging Enabled' }],
                                                onChange: function($select) {
                                                    if ($select.val() === 'on') {
                                                        debugOn(true);
                                                        $('.debug').css('display', '').show();
                                                    } else {
                                                        debugOn(false);
                                                        $('.debug').css('diplay', 'none').hide();
                                                    }
                                                }
                                            })
                                            .CswAttrXml({ 'data-role': 'slider' });

            }
            var $retDiv = _addPageDivToBody({
                    DivId: 'help',
                    HeaderText: 'Help',
                    $content: $help,
                    HideSearchButton: true,
                    HideOnlineButton: false,
                    HideRefreshButton: true,
                    HideHelpButton: true,
                    HideBackButton: true
                });

            return $retDiv;
        }

        //#region Events

        function onLoginFail(text) {
            Logout(false);
            _addToDivHeaderText($logindiv, text)
                .css('color','yellow');
            storeLocalData('loginFailure', text);
        }

        function onLogout() {
            Logout(true);
        }

        function onError() {
            stopLoadingMsg();
        }
        
        function Logout(reloadWindow) {
            if ( _checkNoPendingChanges() ) {
                
                var loginFailure = tryParseString(getStoredLocalString('loginFailure'), '');
                var onlineStatus = amOnline();
                
                clearStorage();
                
                amOnline(onlineStatus,loginFailure);
                // reloading browser window is the easiest way to reset
                if (reloadWindow) {
                    window.location.href = window.location.pathname;
                }
            }
        }

        //#endregion Events
        
        //#region Button Bindings
        
        function onLoginSubmit() {
            if (amOnline()) {
                var UserName = $('#login_username').val();
                var AccessId = $('#login_customerid').val();

                var ajaxData = {
                    'AccessId': AccessId, //We're displaying "Customer ID" but processing "AccessID"
                    'UserName': UserName,
                    'Password': $('#login_password').val(),
                    ForMobile: ForMobile
                };

                CswAjaxJSON({
                        formobile: ForMobile,
                        async: false,
                        url: opts.AuthenticateUrl,
                        data: ajaxData,
                        onloginfail: function(text) {
                            onLoginFail(text);
                        },
                        success: function(data) {
                            SessionId = $.CswCookie('get', CswCookieName.SessionId);
                            _cacheSession(SessionId, UserName, AccessId);
                            $viewsdiv = reloadViews();
                            $viewsdiv.cswChangePage();
                        },
                        error: function() {
                            onError();
                        }
                    });
            }

        } //onLoginSubmit() 
        
        function onRefresh() {
            var DivId = currentViewId();
            if(isNullOrEmpty(DivId)) {
                window.location.reload();
            }
            else if (amOnline() && 
                _checkNoPendingChanges() ) {
                
                if(DivId === 'viewsdiv') {
                    window.location.reload();
                }
                else {
                    var HeaderText = _getDivHeaderText(DivId);
                    var jsonData = {
                        SessionId: SessionId,
                        ParentId: DivId,
                        ForMobile: ForMobile
                    };

                    CswAjaxJSON({
                            formobile: ForMobile,
                            url: opts.ViewUrl,
                            data: jsonData,
                            stringify: false,
                            onloginfail: function(text) { onLoginFail(text); },
                            success: function(data) {
                                setOnline(false);
                                if( !isNullOrEmpty(data['nodes']) ) {
                                    var viewJSON = data['nodes'];
                                    
                                    var params = {
                                        ParentId: 'viewsdiv',
                                        DivId: DivId,
                                        HeaderText: HeaderText,
                                        json: _updateStoredViewJson(DivId, viewJSON),
                                        parentlevel: 0,
                                        level: 1,
                                        HideRefreshButton: false,
                                        HideSearchButton: false,
                                        HideBackButton: false
                                    };
                                    params.onPageShow = function() { return _loadDivContents(params); };
                                    _loadDivContents(params).cswChangePage();
                                }
                            }, // success
                            error: function() {
                                onError();
                            }
                        });
                }
            }
        }

        function onSyncStatusOpen() {
            $syncstatus.cswChangePage({ transition: 'slideup' });
        }

        function onHelp() {
            $help = _makeHelpDiv();
            $help.cswChangePage({ transition: 'slideup' });
        }

        //#endregion Button Bindings
        
        function onPropertyChange(DivId, eventObj, inputVal, inputId, inputPropId) {
            var logger = new profileMethod('onPropertyChange');
            var $elm = $(eventObj.target);

            var name = tryParseString(inputId, $elm.CswAttrDom('id'));
            var value = tryParseString(inputVal, eventObj.target.innerText);
       
            var nodeId = DivId.substr(DivId.indexOf('nodeid_nodes_'),DivId.length);
            var nodeJson = _fetchCachedNodeJson(nodeId);
            
            // update the xml and store it
            if (!isNullOrEmpty(nodeId) && !isNullOrEmpty(nodeJson)) {
                mobileStorage.addUnsyncedChange();
                _resetPendingChanges();
                
                if( !isNullOrEmpty(inputPropId) )
                {
                    for (var key in nodeJson['subitems'])
                    {
                        var tab = nodeJson['subitems'][key];
                        if (!isNullOrEmpty(tab[inputPropId])) {
                            var prop = tab[inputPropId];
                            _FieldTypeHtmlToJson(prop, name, inputPropId, value);
                            //we're only updating one prop--don't iterate them all.
                            break;
                        }
                    }
                }
                else { //remove else as soon as we can verify we never need to enter here
                    errorHandler('Could not find a prop to update');
                }
                _updateStoredNodeJson(nodeId, nodeJson, '1');
            }
            kickStartAutoSync();
            cacheLogInfo(logger);
        } // onPropertyChange()

        function onSearchOpen(DivId) {
            var viewId = currentViewId();
            var searchJson = _fetchCachedViewJson(viewId,'search');
            if (!isNullOrEmpty(searchJson)) {
                var $wrapper = $('<div></div>');
                var $fieldCtn = $('<div data-role="fieldcontain"></div>')
                                            .appendTo($wrapper);
                var values = [];
                var selected;
                
                for(var key in searchJson ) {
                    if( !selected ) selected = key;
                    values.push({ 'value': key, 'display': searchJson[key]});
                }
                
                var $select = $fieldCtn.CswSelect('init',  {
                                                ID: DivId + '_searchprop',
                                                selected: selected,
                                                cssclass: 'csw_search_select',
                                                values: values
                                            })
                                            .CswAttrXml({ 'data-native-menu': 'false' });

                var $searchCtn = $('<div data-role="fieldcontain"></div>')
                                            .appendTo($wrapper);
                $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: DivId + '_searchfor' })
                                                    .CswAttrXml({
                                                    'placeholder': 'Search',
                                                    'data-placeholder': 'Search'
                                                });
                $wrapper.CswLink('init', { type: 'button', ID: DivId + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
                                                .CswAttrXml({ 'data-role': 'button' })
                                                .unbind('click')
                                                .bind('click', function() {
                                                    return startLoadingMsg( function () { onSearchSubmit(DivId); });
                                                });
                $wrapper.CswDiv('init', { ID: DivId + '_searchresults' });

                var $searchDiv = _addPageDivToBody({
                        ParentId: DivId,
                        DivId: 'CswMobile_SearchDiv' + viewId,
                        HeaderText: 'Search',
                        $content: $wrapper,
                        HideSearchButton: true,
                        HideOnlineButton: true,
                        HideRefreshButton: true,
                        HideHelpButton: false,
                        HideBackButton: false
                    });
                $searchDiv.cswChangePage({ transition: 'slideup' });
            }
        }

        function onSearchSubmit(DivId) {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            var $resultsDiv = $('#' + DivId + '_searchresults')
                .empty();
            
            var viewId = currentViewId();
            var searchJson = _fetchCachedViewJson(viewId);
            
            if (!isNullOrEmpty(searchJson)) {
                var $content = $resultsDiv.cswUL({id: DivId + '_searchresultslist', 'data-filter': false })
                                          .append($('<li data-role="list divider">Results</li>'));

                var hitcount = 0;
                for(var key in searchJson)
                {
                    var node = searchJson[key];
                    if (!isNullOrEmpty(node[searchprop])) {
                        if (node[searchprop].toLowerCase().indexOf(searchfor.toLowerCase()) >= 0) {
                            hitcount++;
                            var nodeJson = { id: key, value: node};
                            $content.append(
                                _makeListItemFromJson($content, {
                                    ParentId: DivId + '_searchresults',
                                    DivId: DivId + '_searchresultslist',
                                    HeaderText: 'Results',
                                    PageType: 'node',
                                    json: nodeJson,
                                    parentlevel: 1 }
                                    )
                                );
                        }
                    }
                }
                if (hitcount === 0) {
                    $content.append($('<li>No Results</li>'));
                }
                $content.cswPage();
            }
            stopLoadingMsg();
        } // onSearchSubmit()

        // ------------------------------------------------------------------------------------
        // UI functions
        // ------------------------------------------------------------------------------------
        
        function startLoadingMsg(onSuccess) {
            $.mobile.showPageLoadingMsg();
            if( arguments.length === 1 && !isNullOrEmpty(onSuccess) ) {
                onSuccess();
            }
            return false;
        }
        
        function stopLoadingMsg(onSuccess) {
            if( arguments.length === 1 && !isNullOrEmpty(onSuccess) ) {
                onSuccess();
            } 
            $.mobile.hidePageLoadingMsg();
            var $currentDiv = $("div[data-role='page']:visible:visible");
            $currentDiv.find('.csw_listview').cswPage();
            return false;
        }
        
        // ------------------------------------------------------------------------------------
        // Persistance functions
        // ------------------------------------------------------------------------------------
        
        function _cacheSession(sessionid, username, customerid) {
            setOnline(false);
            storeLocalData('username',username);
            storeLocalData('customerid',customerid);
            storeLocalData('sessionid',sessionid);
        } //_cacheSession()

        function CswMobileStorage() {
            this.lastSyncSuccess = function() {
                var now = new Date();
                var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
                storeLocalData('lastSyncSuccess', ret);
                removeStoredLocalData('lastSyncAttempt'); //clear last failed on next success
                storeLocalData('lastSyncTime',now);
                return ret;
            };
            this.lastSyncSuccess.toString = function() {
                return getStoredLocalString('lastSyncSuccess');
            };
            this.lastSyncAttempt = function() {
                var now = new Date();
                var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
                storeLocalData('lastSyncAttempt', ret);
                storeLocalData('lastSyncTime', now);
                return ret;
            };
            this.lastSyncAttempt.toString = function() {
                return getStoredLocalString('lastSyncAttempt');
            };
            this.lastSyncTime = getStoredLocalString('lastSyncTime');
            this.addUnsyncedChange = function() {
                var unSyncedChanges = tryParseNumber(getStoredLocalString('unSyncedChanges'), '0');
                unSyncedChanges++;
                storeLocalData('unSyncedChanges', unSyncedChanges);
                _updatedUnsyncedChanges();
                return unSyncedChanges;
            };
            this.clearUnsyncedChanges = function() {
                storeLocalData('unSyncedChanges', '0');
                _updatedUnsyncedChanges();
            };
            this.stayOffline = function(value) {
                if(arguments.length === 1) {
                    storeLocalData('stayOffline', isTrue(value) );
                }
                var ret = isTrue(getStoredLocalString('stayOffline'));
                return ret;
            };
        }
        
        function _updatedUnsyncedChanges() {
            $('#ss_pendingchangecnt').text( tryParseString(getStoredLocalString('unSyncedChanges'),'0') );
        }
        
        function currentViewId(viewId)
        {
            /// <summary>
            ///   Persists the current NBT ViewId. 
            /// </summary>
            /// <param name="viewId" type="String">Optional. An NBT ViewId</param>
            var ret = '';
            if (arguments.length === 1 && viewId)
            {
                ret = viewId;
                storeLocalData('currentviewid', viewId );
            }
            if (isNullOrEmpty(ret)) {
                ret = getStoredLocalString('currentviewid');
            }
            return ret;
        }

        function _storeViewJson(viewId, viewName, viewJson, level, viewSearch, wasModified) {
            /// <summary>
            ///   Stores a view in localStorage
            /// </summary>
            /// <param name="viewId" type="String">An NBT ViewId</param>
            /// <param name="viewName" type="String">Human readable view name</param>
            /// <param name="viewJson" type="JSON">JSON representation of the nodes of the view</param>
            /// <param name="level" type="Number">Number indicating tree depth</param>
            /// <param name="viewSearch" type="JSON">JSON representation of the possible mobile searches on this view</param>
            var logger = new profileMethod('storeViewJson');
            if(level === 0 && !isNullOrEmpty(viewJson))
            {
                storedViews = {}; //the viewnames may have changed. clear to be sure.
                for(var view in viewJson)
                {
                    storedViews[view] = viewJson[view]; 
                }
                //no need to cache the viewsdiv, just store ViewNames
                storeLocalData("storedviews", storedViews);
            }
            else {
                var viewNodes = { };
                for(var nodeId in  viewJson) {
                    viewNodes[nodeId] = viewJson[nodeId];
                    if( wasModified ) {
                        viewNodes[nodeId]['wasmodified'] = true;
                    }
                    viewNodes[nodeId]['viewid'] = viewId;
                    storeLocalData(nodeId, viewNodes[nodeId]);
                    delete viewNodes[nodeId]['subitems'];
                }
                if( wasModified ) {
                    viewNodes['wasmodified'] = true;
                } 
                storeLocalData(viewId, { 'name': viewName, 'json': viewNodes, 'search': viewSearch });
            }
            cacheLogInfo(logger);
        }

        function _updateStoredViewJson(viewId, viewJson, wasModified) {
            /// <summary>
            ///   Updates a view in localStorage
            /// </summary>
            /// <param name="viewId" type="String">An NBT ViewId</param>
            /// <param name="viewName" type="String">Human readable view name</param>
            /// <param name="wasModified" type="Boolean">Indicates whether this update modifies the view</param>
            
            if (!isNullOrEmpty(viewId) && !isNullOrEmpty(viewJson)) {
                var currentView = getStoredLocalJSON(viewId);
                var viewName = currentView['name'];
                _storeViewJson(viewId, viewName, viewJson, 1, '', wasModified);
            }
            return viewJson;
        }

        function _updateStoredNodeJson(nodeId, nodeJson, wasModified) {
            /// <summary>
            ///   Updates a node in view in localStorage
            /// </summary>
            /// <param name="nodeId" type="String">An NBT NodeId</param>
            /// <param name="nodeJson" type="JSON">JSON representation of the node</param>
            /// <param name="wasModified" type="Boolean">Indicates whether this update modifies the view</param>
            
            if (!isNullOrEmpty(nodeId) && !isNullOrEmpty(nodeJson)) {
                if( wasModified ) {
                    nodeJson['wasmodified'] = true;
                } else {
                    delete nodeJson['wasmodified'];
                }
                storeLocalData(nodeId, nodeJson);
            }
            return nodeJson;
        }
        
        function _processModifiedNodes(onSuccess) {
            var modified = false;
            if (isNullOrEmpty(storedViews)) {
                storedViews = getStoredLocalJSON('storedviews');
            }
            if( !isNullOrEmpty(storedViews)) {
                for (var viewid in storedViews) {
                    var view = getStoredLocalJSON(viewid);
                    if (!isNullOrEmpty(view)) {
                        for(var nodeId in view['json']) {
                            var node = getStoredLocalJSON(nodeId);
                            if (!isNullOrEmpty(node) && node['wasmodified']) {
                                modified = true;
                                onSuccess(nodeId, node);
                            }                            
                        }
                    }
                }
                if (!modified) {
                    onSuccess();
                }
            }
        }

        function _deleteNode(nodeId,viewId,reloadView) {
            //remove the cached node JSON
            removeStoredLocalData(nodeId);
                                        
            //remove the Div
            $('#' + nodeId).remove();
                                        
            //remove the node from the View JSON
            if( !isNullOrEmpty(viewId) ) {
                var view = getStoredLocalJSON(viewId);
                if( !isNullOrEmpty(view['json']) ) {
                    delete view['json'][nodeId];
                    storeLocalData(viewId, view);
                }
                if( reloadView ) {
                    $('#' + viewId).cswChangePage();
                }
            }
        }
        
        function _fetchCachedViewJson(viewId,viewObj) {
            /// <summary>
            ///   Retrieve a view from localStorage
            /// </summary>
            /// <param name="viewId" type="String">An NBT ViewId</param>
            /// <param name="viewObj" type="String">Optional. The JSON property to retrieve. 'json' if omitted.</param>
            var ret = {};
            var rootObj = getStoredLocalJSON(viewId);
            if (!isNullOrEmpty(rootObj)) {
                var jProp = 'json';
                if(arguments.length === 2 && viewObj ) {
                    jProp = viewObj;
                }
                ret = rootObj[jProp];
            }
            return ret;
        }
        
        function _fetchCachedNodeJson(nodeId) {
            /// <summary>
            ///   Retrieve a node from the current view
            /// </summary>
            /// <param name="nodeId" type="String">An NBT NodeId</param>
            var ret = {};
            if ( !isNullOrEmpty(nodeId)) {
                ret = getStoredLocalJSON(nodeId);
            }
            return ret;
        }
        
        
        // ------------------------------------------------------------------------------------
        // Synchronization
        // ------------------------------------------------------------------------------------

        var _waitForData_TimeoutId;

        function _waitForData() {
            _waitForData_TimeoutId = setTimeout( function() {
                                        _handleDataCheckTimer();
                                     }, 
                                     opts.PollingInterval); //30 seconds
        }

        function _clearWaitForData() {
            clearTimeout(_waitForData_TimeoutId);
        }

        function kickStartAutoSync() {
            var now = new Date().getTime();
            var last = new Date(mobileStorage.lastSyncTime).getTime();
            var lastSync = now - last;
            
            if( lastSync > opts.PollingInterval * 3 ) {//90 seconds
                _clearWaitForData();
                _waitForData();
            }
        }
        
        function _handleDataCheckTimer(onSuccess, onFailure) {
            var url = opts.ConnectTestUrl;
            if (opts.RandomConnectionFailure) {
                url = opts.ConnectTestRandomFailUrl;
            }
            if( !mobileStorage.stayOffline() ) {
                CswAjaxJSON({
                        formobile: ForMobile,
                        url: url,
                        data: {  },
                        stringify: false,
                        onloginfail: function(text) { onLoginFail(text); },
                        success: function(data) {
                            setOnline(false);
                            _processChanges(true);
                            if (!isNullOrEmpty(onSuccess)) {
                                onSuccess( data );
                            }
                        },
                        error: function(data) {
                            setOffline();
                            if (!isNullOrEmpty(onFailure)) {
                                onFailure( data );
                            }
                            _waitForData();
                        }
                    });
            } // if(amOnline())
            else {
                _waitForData();
            }
        } //_handleDataCheckTimer()

        function _processChanges(perpetuateTimer) {
            var logger = new profileMethod('processChanges');
            if (!isNullOrEmpty(SessionId) && !mobileStorage.stayOffline() ) {
                _processModifiedNodes(function(objectId, objectJSON) {
                    if (!isNullOrEmpty(objectId) && !isNullOrEmpty(objectJSON)) {
                        
                        var dataJson = {
                            SessionId: SessionId,
                            ParentId: objectId,
                            UpdatedViewJson: JSON.stringify(objectJSON),
                            ForMobile: ForMobile
                        };

                        CswAjaxJSON({
                                formobile: ForMobile,
                                url: opts.UpdateViewUrl,
                                data: dataJson,
                                onloginfail: function(text) {
                                    setOnline(false);
                                    onLoginFail(text);
                                    processChangesLoop(perpetuateTimer);
                                },
                                success: function(data) {
                                    logger.setAjaxSuccess();
                                    setOnline(false);
                                    var completed = isTrue(data['completed']);
                                    var isView = !isNullOrEmpty(data['nodes']);
                                    if( isView )
                                    {
                                        var json = data['nodes'];
                                        _updateStoredViewJson(objectId, json, false);
                                    } else if( !completed ) {
                                        _updateStoredNodeJson(objectId, objectJSON, false);
                                    } 
                                    
                                    _resetPendingChanges(true);
                                    
                                    processChangesLoop(perpetuateTimer);
                                    var reloadView = !perpetuateTimer;
                                    if( completed && !isView ) {
                                        _deleteNode(objectId, objectJSON['viewid'], reloadView);
                                    }
                                },
                                error: function() {
                                    processChangesLoop(perpetuateTimer);
                                }
                            });
                    } else {
                        processChangesLoop(perpetuateTimer);
                    }
                }); // _getModifiedView();
            } else {
                _resetPendingChanges(true);
                
            } // if(SessionId != '') 
            _updatedUnsyncedChanges();
            cacheLogInfo(logger);
        } //_processChanges()

        function processChangesLoop(perpetuateTimer) {
            if (perpetuateTimer) {
                    _waitForData();
            } else { //we called this manually
                stopLoadingMsg();
            }
        }
        
        // For proper chaining support
        return this;
    };
})(jQuery);