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

    $.fn.makeUL = function(id, params) {
        var p = {
            'data-filter': false,
            'data-role': 'listview',
            'data-inset': true
        };
        if (params) $.extend(p, params);

        var $div = $(this);
        var $ret = undefined;
        if (!isNullOrEmpty($div)) {
            $ret = $('<ul class="csw_listview" id="' + tryParseString(id, '') + '"></ul>')
                                                    .appendTo($div)
                                                    .CswAttrXml(p);
        }
        return $ret;
    };

    $.fn.bindLI = function() {
        var $li = $(this);
        var $ret = undefined;
        if (!isNullOrEmpty($li)) {
            $li.unbind('click');
            $ret = $li.find('li a').bind('click', function() {
                $.mobile.hidePageLoadingMsg();
                var dataurl = $(this).CswAttrXml('data-url');
                var $thisPage = $('#' + dataurl);
                $thisPage.doChangePage();
            });
        }
        return $ret;
    };

    $.fn.doChangePage = function(options) {
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
            var $page = $.mobile.activePage;
            var id = (isNullOrEmpty($page)) ? 'no ID' : $page.CswAttrDom('id');
            if(debugOn()) log('changePage from ' + $page.CswAttrDom('id') + ' to ' + $div.CswAttrDom('id'), true);
            if (id !== $div.CswAttrDom('id')) ret = $.mobile.changePage($div, o);
        }
        return ret;
    };

    $.fn.doPage = function() {
        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div)) {
            ret = $div.page(); 
        }
        return ret;
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
                onPageShow: function(p) {
                }
            };

            if (params) $.extend(p, params);
            p.level = (p.parentlevel === p.level) ? p.parentlevel + 1 : p.level;

            $div.unbind('pageshow');
            $ret = $div.bind('pageshow', function() {
                $.mobile.showPageLoadingMsg();
                if (p.level === 1) localStorage['currentviewid'] = p.DivId;
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
            DBShortName: 'Mobile.html',
            DBVersion: '1.0',
            DBDisplayName: 'Mobile.html',
            DBMaxSize: 65536,
            ViewsListUrl: '/NbtWebApp/wsNBT.asmx/GetViewsList',
            ViewUrl: '/NbtWebApp/wsNBT.asmx/GetView',
            ConnectTestUrl: '/NbtWebApp/wsNBT.asmx/ConnectTest',
            ConnectTestRandomFailUrl: '/NbtWebApp/wsNBT.asmx/ConnectTestRandomFail',
            UpdateUrl: '/NbtWebApp/wsNBT.asmx/UpdateProperties',
            MainPageUrl: '/NbtWebApp/Mobile.html',
            AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/Authenticate',
            SendLogUrl: '/NbtWebApp/wsNBT.asmx/collectClientLogInfo',
            Theme: 'a',
            PollingInterval: 30000,
            DivRemovalDelay: 1,
            RandomConnectionFailure: false
        };

        if (options) {
            $.extend(opts, options);
        }

        debugOn(debug);
        
        var ForMobile = true;
        var rootid;

        var mobileStorage = new CswMobileStorage();
        
        var UserName = localStorage["username"];
        if (!localStorage["sessionid"]) {
            Logout();
        }
        var SessionId = localStorage["sessionid"];
        function currentViewJson(json,level) {
            var ret = { };
            if(json && arguments.length >= 1) {
                if( !isNullOrEmpty(level) ) {
                    switch(level) {
                        case 0:
                            {
                                ret = json.views.view;
                                break;
                            }
                        default:
                            {
                                ret = json.searches.node;
                                break;
                            }
                    }
                }
                else {
                    ret = json;
                }
                if(ret) {
                    localStorage.currentViewJson = JSON.stringify(ret);
                }
            }
            if( ( !ret || ret.length === 0) && 
                localStorage.currentViewJson && 
                'undefined' !== localStorage.currentViewJson ) {
                ret = JSON.parse(localStorage.currentViewJson);
            }
            return ret;
        }
        

        var storedViews = [];
        if (localStorage.storedViews) storedViews = JSON.parse(localStorage['storedviews']); // {name: '', rootid: ''}

        var $logindiv = _loadLoginDiv();
        var $viewsdiv = reloadViews();
        var $syncstatus = _makeSyncStatusDiv();
        var $helpdiv = _makeHelpDiv();
        var $sorrycharliediv = _loadSorryCharlieDiv();

        // case 20355 - error on browser refresh
        // there is a problem if you refresh with #viewsdiv where we'll generate a 404 error, but the app will continue to function
        if (!isNullOrEmpty(SessionId)) {
            $.mobile.path.set('#viewsdiv');
            localStorage['refreshPage'] = 'viewsdiv';
        } else {
            $.mobile.path.set('#logindiv');
            localStorage['refreshPage'] = 'logindiv';
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
                        $logindiv.doPage();
                        $logindiv.doChangePage();
                    },
                    function() {
                        // offline
                        if( !$sorrycharliediv || $sorrycharliediv.length === 0 ) {
                            $sorrycharliediv = _loadSorryCharlieDiv();
                        }
                        $sorrycharliediv.doPage();
                        $sorrycharliediv.doChangePage();
                    }
                );
            }
        };
        
        function _loadLoginDiv() {
            var LoginContent = '<input type="textbox" id="login_customerid" placeholder="Customer Id"/><br>';
            LoginContent += '<input type="textbox" id="login_username" placeholder="User Name"/><br>';
            LoginContent += '<input type="password" id="login_password" placeholder="Password"/><br>';
            LoginContent += '<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>';
            var $retDiv = _addPageDivToBody({
                    DivId: 'logindiv',
                    HeaderText: 'Login to ChemSW Fire Inspection',
                    $content: $(LoginContent),
                    HideSearchButton: true,
                    HideOnlineButton: true,
                    HideRefreshButton: true,
                    HideLogoutButton: true,
                    HideHelpButton: false,
                    HideCloseButton: true,
                    HideBackButton: true,
                    dataRel: 'dialog'
                });
            
            if( !isNullOrEmpty(localStorage['loginFailure']) ) {
                _addToDivHeaderText($retDiv, localStorage['loginFailure']);
            }
            
            $('#loginsubmit').bind('click', function() {
                $.mobile.showPageLoadingMsg();
                onLoginSubmit();
            });
            $('#login_customerid').clickOnEnter($('#loginsubmit'));
            $('#login_username').clickOnEnter($('#loginsubmit'));
            $('#login_password').clickOnEnter($('#loginsubmit'));

            return $retDiv;
        }

        function reloadViews() {
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
            $viewsdiv.doPage();
            $viewsdiv.bindJqmEvents(params);
            return $viewsdiv;
        }
        
        function _loadSorryCharlieDiv(params) {
            var p = {
                DivId: 'sorrycharliediv',
                HeaderText: 'Sorry Charlie!',
                HideHelpButton: false,
                HideCloseButton: true,
                HideOnlineButton: false,
                HideBackButton: true,
                HideRefreshButton: true,
                HideLogoutButton: true,
                HideSearchButton: true,
                dataRel: 'dialog',
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
            
            if ($.mobile.activePage === $logindiv) {
                $sorrycharliediv.doPage(); // doChangePage();
            }
            $.mobile.hidePageLoadingMsg();
        }

        function setOnline(reloadViewsPage) {
            
            amOnline(true);
            localStorage.removeItem('loginFailure');
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
            if ($.mobile.activePage === $sorrycharliediv) {
                $logindiv.doPage(); //doChangePage();
            }
        }

        function amOnline(amOnline,loginFailure) {
            if(arguments.length > 0 ) {
                localStorage['online'] = isTrue(amOnline);
            }
            if(loginFailure) {
                localStorage['loginFailure'] = loginFailure;
            }
            var ret = ( isTrue(localStorage['online']) && !mobileStorage.stayOffline());
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
                var $loggingBtn = $('.debug')
                                       .removeClass('debug-off')
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
                var $loggingBtn = $('.debug')
                                       .removeClass('debug-on')
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
                    'UserName': localStorage['username'],
                    'CustomerId': localStorage['customerid'],
                    'LogInfo': sessionStorage['debuglog']
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
                    HideCloseButton: true,
                    HideOnlineButton: false,
                    HideBackButton: false,
                    HideRefreshButton: true,
                    HideLogoutButton: true,
                    HideSearchButton: true,
                    dataRel: 'dialog',
                    $content: $( JSON.parse(sessionStorage['debuglog']))
                };
                var $logDiv = _addPageDivToBody(params);
                $logDiv.bindJqmEvents(params);
                $logDiv.doChangePage();
                $.mobile.hidePageLoadingMsg();
            }
        }

        // ------------------------------------------------------------------------------------
        // List items fetching
        // ------------------------------------------------------------------------------------

        function _loadDivContents(params) {
            var logger = new profileMethod('loadDivContents');
            $.mobile.showPageLoadingMsg();

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

            if (p.level === 1) {
                rootid = p.DivId;
            }
            var $retDiv = $('#' + p.DivId);

            if (isNullOrEmpty($retDiv) || $retDiv.length === 0 || $retDiv.find('div:jqmData(role="content")').length === 1) {
                if (p.level === 0) {
                    p.PageType = 'view';
                    if (!amOnline()) {
                        p.json = currentViewJson( _fetchCachedViewJson(p.DivId), p.level );
                        $retDiv = _loadDivContentsJson(p);
                    } else {
                        p.url = opts.ViewsListUrl;
                        $retDiv = _getDivJson(p);
                    }
                } else if (p.level === 1) {
                    // case 20354 - try cached first
                    var cachedJson = _fetchCachedViewJson(rootid);
                    p.PageType = 'node';
                    if (!isNullOrEmpty(cachedJson)) {
                        p.json = currentViewJson(cachedJson);
                        $retDiv = _loadDivContentsJson(p);
                    } else if (amOnline()) {
                        p.url = opts.ViewUrl;
                        $retDiv = _getDivJson(p);
                    }
                } else  // Level 2 and up
                {
                    var cachedJson = _fetchCachedNodeJson(p.ParentId, p.DivId);
                    p.PageType = 'prop';
                    if( isNullOrEmpty(cachedJson) || cachedJson.length === 0 ) {
                        cachedJson = currentViewJson();
                    }
                    if( !isNullOrEmpty(cachedJson) ) {
                        p.json = cachedJson.subitems.prop;

                        if (!isNullOrEmpty(p.json)) {
                            $retDiv = _loadDivContentsJson(p);
                        }
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
                    url: p.url,
                    data: jsonData,
                    onloginfail: function(text) { onLoginFail(text); },
                    success: function(data) {
                        setOnline(false);
                        logger.setAjaxSuccess();

                        p.json = currentViewJson( data, params.level );    
                        
                        if( params.level < 2) {
                            _storeViewJson(p.DivId, p.HeaderText, p.json, params.level);
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
                level: '',
                HideSearchButton: false,
                HideOnlineButton: false,
                HideRefreshButton: false,
                HideLogoutButton: false,
                HideHelpButton: false,
                HideCloseButton: true,
                HideBackButton: false
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
                    HideLogoutButton: p.HideLogoutButton,
                    HideHelpButton: p.HideHelpButton,
                    HideCloseButton: p.HideCloseButton,
                    HideBackButton: p.HideBackButton
                });

            var $content = $retDiv.find('div:jqmData(role="content")').empty();

            var $list = $content.makeUL();
            currenttab = '';

            for(var i=0; i< p.json.length; i++)
            {
                var item = { };
                $.extend(item, p);
                item.json = p.json[i];

                _makeListItemFromJson($list, item)
                    //.CswAttrXml('data-icon', false)
                    .appendTo($list);
            }

            logger.setAjaxSuccess();
            try {
                $('.csw_collapsible').page();
                $('.csw_fieldset').page();
//                $('.csw_answer').page();
                $('.csw_listview').page();
            }
            catch(e) //this is hackadelic, but it works. 
            {
                $('.csw_collapsible').page();
                $('.csw_fieldset').page();
//                $('.csw_answer').page();
                $('.csw_listview').page();
            }
            $content.page();

            _resetPendingChanges();
            
            $.mobile.hidePageLoadingMsg();
            if(!mobileStorage.stayOffline()) {
                _toggleOffline(false);
            }
            cacheLogInfo(logger);
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
                HideSearchButton: false
            };
            if (params) $.extend(p, params);

            var id = makeSafeId({ ID: p.json['@id'] });
            var text = p.json['@name'];

            var IsDiv = (!isNullOrEmpty(id));

            var $retLI = $('');

            switch (p.PageType) {
            case "search":
                    // ignore this
                break;
            case "node":
                $retLI = _makeObjectClassContent(p)
                                        .appendTo($list);
                break;
            case "prop":
                {
                    var $tab;
                    var tab = p.json['@tab'];

                    if (currenttab !== tab) {
                        //should be separate ULs eventually
                        $tab = $('</ul><li data-role="list-divider">' + tab + '</li><ul>')
                                                .appendTo($list);
                        currenttab = tab;
                    }

                    var $prop = _FieldTypeJsonToHtml(p.json, id)
                                    .appendTo($list);
                    break;   
                } // case 'prop':
            default:
                {
                    $retLI = $('<li></li>');
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
            
            $retLI.bind('click', function() {
                $.mobile.showPageLoadingMsg();
                var par = {ParentId: p.DivId,
                    parentlevel: p.parentlevel,
                    level: p.parentlevel + 1,
                    DivId: id,
                    persistBindEvent: true,
                    HeaderText: text  };
                var $div = _addPageDivToBody(par);
                par.onPageShow = function() { _loadDivContents(par); };
                $div.bindJqmEvents(par);
                $div.doChangePage({reloadPage: true});
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
            var id = makeSafeId({ ID: p.json['@id'] });
            var nodeSpecies = p.json['@nodespecies'];
            var NodeName = p.json['@name'];
            var icon = '';
            if (!isNullOrEmpty(p.json['@iconfilename'])) {
                icon = 'images/icons/' + p.json['@iconfilename'];
            }
            var ObjectClass = p.json['@objectclass'];

            if( nodeSpecies !== 'More' )
            {
                switch (ObjectClass) {
                case "InspectionDesignClass":
                    var DueDate = '';
                    var Location = '';
                    var MountPoint = '';
                    var Status = '';
                    for(var i=0; i<p.json.subitems.prop.length; i++) {
                        var prop = p.json.subitems.prop[i];
                        switch(prop['@ocpname']) {
                            case 'Location':
                                Location = prop['@gestalt'];
                                break;
                            case 'Target':
                                MountPoint = prop['@gestalt'];
                                break;
                            case 'Due Date':
                                DueDate = prop['@gestalt'];
                                break;
                            case 'Status':
                                Status = prop['@gestalt'];
                                break;
                        }
                    }
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

        function _FieldTypeJsonToHtml(json, ParentId) {
log(json);            
            var IdStr = makeSafeId({ ID: json['@id'] });
            var FieldType = json['@fieldtype'];
            var PropName = json['@name'];
            var ReadOnly = isTrue(json['@isreadonly']);

            // Subfield values
            var sf_text = tryParseString(json['text'], '');
            var sf_value = tryParseString(json['value'], '');
            var sf_href = tryParseString(json['href'], '');
            var sf_checked = tryParseString(json['checked'], '');
            var sf_required = tryParseString(json['required'], '');
            var sf_units = tryParseString(json['units'], '');
            var sf_answer = tryParseString(json['answer'], '');
            var sf_allowedanswers = tryParseString(json['allowedanswers'], '');
            var sf_correctiveaction = tryParseString(json['correctiveaction'], '');
            var sf_comments = tryParseString(json['comments'], '');
            var sf_compliantanswers = tryParseString(json['compliantanswers'], '');
            var sf_options = tryParseString(json['options'], '');

            var $retLi = $('<li id="' + IdStr + '_li"></li>')
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
                    $prop = $propDiv.CswLink('init', { ID: propId, href: sf_href, rel: 'external', value: sf_text});
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
                    $prop = _makeLogicalFieldSet(ParentId, IdStr, sf_checked, sf_required)
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
                    var $question = _makeQuestionAnswerFieldSet(ParentId, IdStr, sf_allowedanswers, sf_answer, sf_compliantanswers)
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
                    $corAction.bind('change', function(eventObj) {
                        var $cor = $(this);
                        if ($cor.val() === '') {
                            $label.addClass('OOC');
                        } else {
                            $label.removeClass('OOC');
                        }
                        onPropertyChange(ParentId, eventObj, $cor.val(), IdStr + '_cor');
                    });

                    var $comments = $('<textarea name="' + IdStr + '_input" id="' + IdStr + '_input" placeholder="Comments">' + sf_comments + '</textarea>')
                                                .appendTo($prop)
                                                .bind('change',function (eventObj) {
                                                    var $com = $(this);
                                                    onPropertyChange(ParentId, eventObj, $com.val(), IdStr + '_com');
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
                    $propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['@gestalt'] + '</p>'));
                    break;
                } // switch (FieldType)

                if (addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0) {
                    $prop.bind('change', function(eventObj) {
                        var $this = $(this);
                        onPropertyChange(ParentId, eventObj, $this.val(), propId);
                    });
                }
            } else {
                $propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['@gestalt'] + '</p>'));
            }
            if($propDiv.children().length > 0) {
                $fieldcontain.append($propDiv);
            }
            
            return $retLi;
        }

        function _FieldTypeHtmlToJson(json, id, value) {
            var name = new CswString(id);
            var IdStr = makeSafeId({ ID: json['@id'] });
            var fieldtype = json['@fieldtype'];
            //var propname = json.name;

            // subfield nodes
            var $sf_text = json['@text'];
            var $sf_value = json['@value'];
            //var $sf_href = json.href;
            //var $sf_options = json.options;
            var $sf_checked = json['@checked'];
            //var $sf_required = json.required;
            //var $sf_units = json.units;
            var $sf_answer = json['@answer'];
            //var $sf_allowedanswers = json.allowedanswers;
            var $sf_correctiveaction = json['@correctiveaction'];
            var $sf_comments = json['@comments'];
            //var $sf_compliantanswers = json.compliantanswers;

            var $sftomodify = null;
            switch (fieldtype) {
            case "Date":
                if (name.contains(IdStr)) $sftomodify = $sf_value;
                break;
            case "Link":
                break;
            case "List":
                if (name.contains(IdStr)) $sftomodify = $sf_value;
                break;
            case "Logical":
                if (name.contains(makeSafeId({ ID: IdStr, suffix: 'ans' }))) {
                    $sftomodify = $sf_checked;
                }
                break;
            case "Memo":
                if (name.contains(IdStr)) $sftomodify = $sf_text;
                break;
            case "Number":
                if (name.contains(IdStr)) $sftomodify = $sf_value;
                break;
            case "Password":
                break;
            case "Quantity":
                if (name.contains(IdStr)) $sftomodify = $sf_value;
                break;
            case "Question":
                if (name.contains(makeSafeId({ ID: IdStr, suffix: 'com' }))) {
                    $sftomodify = $sf_comments;
                } 
                else if (name.contains(makeSafeId({ ID: IdStr, suffix: 'ans' }))) {
                    $sftomodify = $sf_answer;
                } 
                else if (name.contains(makeSafeId({ ID: IdStr, suffix: 'cor' }))) {
                    $sftomodify = $sf_correctiveaction;
                }
                break;
            case "Static":
                break;
            case "Text":
                if (name.contains(IdStr)) $sftomodify = $sf_text;
                break;
            case "Time":
                if (name.contains(IdStr)) $sftomodify = $sf_value;
                break;
            default:
                break;
            }
            if (!isNullOrEmpty($sftomodify)) {
                $sftomodify.text(value);
                json['@wasmodified'] = '1';
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
                                        
            var answers = ['Null', 'True', 'False'];
            if (isTrue(Required)) {
                answers = ['True', 'False'];
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
                    onPropertyChange(ParentId, eventObj, thisInput, inputId);
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

                    setTimeout( function () { onPropertyChange(ParentId, eventObj, thisAnswer, answerName); }, 1);

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
                HideLogoutButton: false,
                HideHelpButton: false,
                HideCloseButton: true,
                HideBackButton: false,
                HideHeaderOnlineButton: true,
                dataRel: 'page',
                backicon: undefined,
                backtransition: undefined
            };

            if (params) {
                $.extend(p, params);
            }

            p.DivId = makeSafeId({ ID: p.DivId });

            var $pageDiv = $('#' + p.DivId);

            var $searchBtn = $('#' + p.DivId + '_searchopen');
            var $syncstatusBtn = $('#' + p.DivId + '_gosyncstatus');
            var $refreshBtn = $('#' + p.DivId + '_refresh');
            var $logoutBtn = $('#' + p.DivId + '_logout');
            var $helpBtn = $('#' + p.DivId + '_help');
            var $closeBtn = $('#' + p.DivId + '_close');
            var $backlink = $('#' + p.DivId + '_back');
            var $headerOnlineBtn = $('#' + p.DivId + '_headeronline');
            var $loggingBtn = $('#' + p.DivId + '_debuglog');
            var $headerTitle = $('#' + p.DivId + '_header_title');
            if (isNullOrEmpty($pageDiv) || $pageDiv.length === 0) {
                $pageDiv = $body.CswDiv('init', { ID: p.DivId })
                                        .CswAttrXml({
                                        'data-role': 'page',
                                        'data-url': p.DivId,
                                        'data-title': p.HeaderText,
                                        'data-rel': p.dataRel,
                                        'data-add-back-btn': !isTrue(p.HideBackButton)              
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
                                                'data-direction': 'reverse'
                                            });

                $closeBtn = $header.CswLink('init', {
                                               href: 'javascript:void(0)',
                                               ID: p.DivId + '_close',
                                               cssclass: 'ui-btn-left'
                                           })
                                               .CswAttrXml({
                                               'data-identity': p.DivId + '_close',
                                               'data-icon': 'delete',
                                               'data-rel': 'back',
                                               'data-iconpos': 'notext',
                                               'data-direction': 'reverse',
                                               'title': 'Close'
                                           });

                if (!isNullOrEmpty(p.backtransition)) {
                    $backlink.CswAttrXml('data-transition', p.backtransition);
                }

                if (!isNullOrEmpty(p.backicon)) {
                    $backlink.CswAttrXml('data-icon', p.backicon);
                } else {
                    $backlink.CswAttrXml('data-icon', 'arrow-l');
                }

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

                var $footerCtn = $('<div data-role="navbar">')
                                            .appendTo($footer);
                var onlineValue = (!amOnline()) ? 'Offline' : 'Online';

                $syncstatusBtn = $footerCtn.CswLink('init', {
                                                        'href': 'javascript:void(0)',
                                                        ID: p.DivId + '_gosyncstatus',
                                                        cssclass: 'onlineStatus ' + onlineValue.toLowerCase(), 
                                                        value: onlineValue
                                                    })
                                                        .CswAttrXml({
                                                        'data-identity': p.DivId + '_gosyncstatus',
                                                        'data-url': p.DivId + '_gosyncstatus',
                                                        'data-transition': 'pop',
                                                        'data-rel': 'dialog'
                                                    })
                                                    .css('display', '');

                $refreshBtn = $footerCtn.CswLink('init', {
                                                           'href': 'javascript:void(0)',
                                                           ID: p.DivId + '_refresh',
                                                           value: 'Refresh',
                                                           cssclass: 'refresh'
                                                       }) 
                                                           .CswAttrXml({
                                                           'data-identity': p.DivId + '_refresh',
                                                           'data-url': p.DivId + '_refresh'
                                                       })
                                                       .css('display', '');

                $logoutBtn = $footerCtn.CswLink('init', {
                                                        'href': 'javascript:void(0)',
                                                        ID: p.DivId + '_logout',
                                                        value: 'Logout'
                                                    })
                                                        .CswAttrXml({
                                                        'data-identity': p.DivId + '_logout',
                                                        'data-url': p.DivId + '_logout',
                                                        'data-transition': 'flip'
                                                    })
                                                        .css('display', '');


                $footerCtn.CswLink('init', { href: 'Main.html', rel: 'external', ID: p.DivId + '_main', value: 'Full Site' })
                          .CswAttrXml({ 'data-transition': 'pop' });


                $helpBtn = $footerCtn.CswLink('init', {
                                             'href': 'javascript:void(0)',
                                             ID: p.DivId + '_help',
                                             value: 'Help'
                                      })
                                      .CswAttrXml({
                                             'data-identity': p.DivId + '_help',
                                             'data-url': p.DivId + '_help',
                                             'data-transition': 'pop',
                                             'data-rel': 'dialog'
                                      })
                                      .css('display', '');

                if( debugOn() )
                {
                    $loggingBtn = $footerCtn.CswLink('init', {
                                                         'href': 'javascript:void(0)',
                                                         ID: p.DivId + '_debuglog',
                                                         value: doLogging() ? 'Sync Log' : 'Start Log',
                                                         cssclass: 'debug'
                                                     })
                                                     .addClass(doLogging() ? 'debug-on' : 'debug-off');
                }
            }

            //case 22323
            $headerTitle.text(p.HeaderText);
            
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
            if (p.HideLogoutButton) {
                $logoutBtn.css('display', 'none').hide();
            } else {
                $logoutBtn.css('display', '').show();
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
            if (p.dataRel === 'dialog' && !p.HideCloseButton) {
                $closeBtn.css('display', '').show();
            } else {
                $closeBtn.css('display', 'none').hide();
            }
            if (!p.HideBackButton) {
                $backlink.css('display', '').show();
            } else {
                $backlink.css('display', 'none').hide();
            }
            if (debugOn()) {
                $loggingBtn.css({ 'display': '' }).show();
            }

            _bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv);
            return $pageDiv;

        }// _addPageDivToBody()

        function _getDivHeaderText(DivId) {
            return $('#' + DivId).find('div:jqmData(role="header") h1').text();
        }

        function _addToDivHeaderText($div, text) {
            $div.find('div:jqmData(role="header") h1').append($('<p style="color: yellow; white-space: normal;">' + text + '</p>'));
            $.mobile.loadPage($div);
            return $div;
        }
        
        function _bindPageEvents(DivId, ParentId, level, $div) {
            $div.find('#' + DivId + '_searchopen')
                .unbind('click')
                .bind('click', function() {
                    $.mobile.showPageLoadingMsg();
                    onSearchOpen(DivId);
                    return false;
                })
                .end()
                .find('#' + DivId + '_gosyncstatus')
                .unbind('click')
                .bind('click', function() {
                    $.mobile.showPageLoadingMsg();
                    onSyncStatusOpen(DivId);
                    return false;
                })
                .end()
                .find('#' + DivId + '_refresh')
                .unbind('click')
                .bind('click', function() {
                    $.mobile.showPageLoadingMsg();
                    onRefresh();
                    return false;
                })
                .end()
                .find('#' + DivId + '_logout')
                .unbind('click')
                .bind('click', function(e) {
                    $.mobile.showPageLoadingMsg();
                    onLogout(DivId, e);
                    return false;
                })
                .end()
                .find('#' + DivId + '_help')
                .unbind('click')
                .bind('click', function() {
                    $.mobile.showPageLoadingMsg();
                    onHelp(DivId, ParentId);
                    return false;
                })
                .end()
                .find('#' + DivId + '_debuglog')
                .die('click')
                .live('click', function() {
                    _toggleLogging();
                    return false;
                })
                .end()
                .find('textarea')
                .unbind('change')
                .bind('change', function(eventObj) {
                    var $this = $(this);
                    onPropertyChange(DivId, eventObj, $this.val(), $this.CswAttrDom('id'));
                })
                .end()
                .find('.csw_prop_select')
                .unbind('change')
                .bind('change', function(eventObj) {
                    var $this = $(this);
                    onPropertyChange(DivId, eventObj, $this.val(), $this.CswAttrDom('id'));
                })
                .end();
        }

        // ------------------------------------------------------------------------------------
        // Sync Status Div
        // ------------------------------------------------------------------------------------

        function _makeSyncStatusDiv() {
            var content = '';
            content += '<p>Pending Unsynced Changes: <span id="ss_pendingchangecnt">' + tryParseString(localStorage.unSyncedChanges,'0') + '</span></p>';
            content += '<p>Last Sync Success: <span id="ss_lastsync_success">' + mobileStorage.lastSyncSuccess + '</span></p>';
            content += '<p>Last Sync Failure: <span id="ss_lastsync_attempt">' + mobileStorage.lastSyncAttempt + '</span></p>';
            content += '<a id="ss_forcesync" data-identity="ss_forcesync" data-url="ss_forcesync" href="javascript:void(0)" data-role="button">Force Sync Now</a>';
            content += '<a id="ss_gooffline" data-identity="ss_gooffline" data-url="ss_gooffline" href="javascript:void(0)" data-role="button">Go Offline</a>';

            var $retDiv = _addPageDivToBody({
                    DivId: 'syncstatus',
                    HeaderText: 'Sync Status',
                    $content: $(content),
                    dataRel: 'dialog',
                    HideSearchButton: true,
                    HideOnlineButton: true,
                    HideRefreshButton: false,
                    HideLogoutButton: false,
                    HideHelpButton: false,
                    HideCloseButton: true,
                    HideBackButton: false,
                    HideHeaderOnlineButton: false
                });
                   
            $retDiv.find('#ss_forcesync')
                            .bind('click', function() {
                                $.mobile.showPageLoadingMsg();
                                _processChanges(false);
                                return false;
                            })
                            .end()
                            .find('#ss_gooffline')
                            .bind('click', function() {
                                var stayOffline = !mobileStorage.stayOffline();
                                mobileStorage.stayOffline(stayOffline);
                                _toggleOffline(true);
                                return false;
                            });

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

        function _resetPendingChanges(setlastsyncnow) {

            $('#ss_pendingchangecnt').text( tryParseString(localStorage.unSyncedChanges,'0') );
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
                if (setlastsyncnow) {
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
            $.mobile.hidePageLoadingMsg();
            return pendingChanges;
        }

        function _pendingChanges() {
            var changes = new Number(tryParseString(localStorage.unSyncedChanges,'0'))
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
                    dataRel: 'dialog',
                    HideSearchButton: true,
                    HideOnlineButton: false,
                    HideRefreshButton: true,
                    HideLogoutButton: false,
                    HideHelpButton: true,
                    HideCloseButton: false,
                    HideBackButton: true
                });

            return $retDiv;
        }

        // ------------------------------------------------------------------------------------
        // Events
        // ------------------------------------------------------------------------------------

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
                            $viewsdiv.doChangePage();
                        },
                        error: function() {
                            onError();
                        }
                    });
            }

        } //onLoginSubmit() 

        function onLoginFail(text) {
            Logout(false);
            $.mobile.hidePageLoadingMsg();
            _addToDivHeaderText($logindiv, text);
            localStorage['loginFailure'] = text;
        }

        function onLogout() {
            Logout(true);
        }

        function onError() {
            $.mobile.hidePageLoadingMsg();
        }
        
        function Logout(reloadWindow) {
            if ( _checkNoPendingChanges() ) {
                
                var loginFailure = tryParseString(localStorage['loginFailure'], '');
                var onlineStatus = amOnline();
                
                _clearStorage();
                
                amOnline(onlineStatus,loginFailure);
                // reloading browser window is the easiest way to reset
                if (reloadWindow) {
                    window.location.href = window.location.pathname;
                }
            }
            $.mobile.hidePageLoadingMsg();
        }

        function _clearStorage() {
            sessionStorage.clear();
            localStorage.clear();
        }

        function onRefresh() {
            $.mobile.showPageLoadingMsg();
            var DivId = localStorage['currentviewid'];
            if (amOnline() && 
                _checkNoPendingChanges() &&
                !isNullOrEmpty(DivId) ) {
                
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
log(data);                            
                            var params = {
                                ParentId: 'viewsdiv',
                                DivId: DivId,
                                HeaderText: HeaderText,
                                json: _updateStoredViewJson(DivId, currentViewJson(data, 0)),
                                parentlevel: 0,
                                level: 1,
                                HideRefreshButton: false,
                                HideSearchButton: false,
                                HideBackButton: false
                            };
                            params.onPageShow = function() { return _loadDivContents(params); };
                            $.mobile.changePage( _loadDivContents(params) );
                        }, // success
                        error: function () {
                            onError(); //setOffline();
                        }
                    });
            }
            $.mobile.hidePageLoadingMsg();
        }

        function onSyncStatusOpen() {
            $syncstatus.doChangePage({ transition: 'slideup' });
        }

        function onHelp() {
            $help = _makeHelpDiv();
            $help.doChangePage({ transition: 'slideup' });
        }

        function onPropertyChange(DivId, eventObj, inputVal, inputId) {
            var logger = new profileMethod('onPropertyChange');
            var $elm = $(eventObj.target);

            var name = tryParseString(inputId,$elm.CswAttrDom('id'))
            var value = tryParseString(inputVal, eventObj.target.innerText);
       
            // update the xml and store it
            if (!isNullOrEmpty(currentViewJson())) {
                mobileStorage.addUnsyncedChange();
                _resetPendingChanges(false);
                
                var nodeId = DivId.substr(DivId.indexOf('nodeid_nodes_'),DivId.length);
                var nodeJson = _fetchCachedNodeJson(DivId, nodeId);
                
                for(var i=0; i<nodeJson.subitems.prop.length; i++)
                {
                    var prop = nodeJson.subitems.prop[i];
                    _FieldTypeHtmlToJson(prop, name, value);
                }
                _updateStoredViewJson(rootid, currentViewJson(), '1');
            }
            kickStartAutoSync();
            cacheLogInfo(logger);
        } // onPropertyChange()

        function onSearchOpen(DivId) {
            var searchJson = _fetchCachedViewJson(rootid);
            if (!isNullOrEmpty(searchJson)) {
                var $wrapper = $('<div></div>');
                var $fieldCtn = $('<div data-role="fieldcontain"></div>')
                                            .appendTo($wrapper);
                var $select = $('<select id="' + DivId + '_searchprop" name="' + DivId + '_searchprop" class="csw_prop_select">')
                                            .appendTo($fieldCtn)
                                            .CswAttrXml({ 'data-native-menu': 'false' });

                for(var i=0; i < searchJson.length; i++ ) {
                    var search = searchJson[i];
                    $('<option value="' + search.id + '">' + search.name + '</option>')
                        .appendTo($select);
                }
                var $searchCtn = $('<div data-role="fieldcontain"></div>')
                                            .appendTo($wrapper);
                $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: DivId + '_searchfor' })
                                                    .CswAttrXml({
                                                    'placeholder': 'Search',
                                                    'data-placeholder': 'Search'
                                                });
                $wrapper.CswLink('init', { type: 'button', ID: DivId + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
                                                .CswAttrXml({ 'data-role': 'button' })
                                                .bind('click', function() {
                                                    onSearchSubmit(DivId);
                                                    return false;
                                                });
                $wrapper.CswDiv('init', { ID: DivId + '_searchresults' });

                var $searchDiv = _addPageDivToBody({
                        ParentId: DivId,
                        DivId: 'CswMobile_SearchDiv' + rootid,
                        HeaderText: 'Search',
                        $content: $wrapper,
                        HideSearchButton: true,
                        HideOnlineButton: true,
                        HideRefreshButton: true,
                        HideLogoutButton: false,
                        HideHelpButton: false,
                        HideCloseButton: true,
                        HideBackButton: false
                    });
                $searchDiv.doChangePage({ transition: 'slideup' });
            }
        }

        function onSearchSubmit(DivId) {
            $.mobile.showPageLoadingMsg();
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            var $resultsDiv = $('#' + DivId + '_searchresults')
                .empty();
            var search = _fetchCachedViewJson(rootid);
            if (!isNullOrEmpty(search)) {
                var $content = $resultsDiv.makeUL(DivId + '_searchresultslist', { 'data-filter': false })
                                                    .append($('<li data-role="list divider">Results</li>'));

                var hitcount = 0;
                for(var i=0; i<search.length; i++ )
                {
                    var node = search[i].node;
                    if (!isNullOrEmpty(node[searchprop])) {
                        if (node[searchprop].toLowerCase().indexOf(searchfor.toLowerCase()) >= 0) {
                            hitcount++;
                            $content.append(
                                _makeListItemFromJson($content, {
                                    ParentId: DivId + '_searchresults',
                                    DivId: DivId + '_searchresultslist',
                                    HeaderText: 'Results',
                                    json: node,
                                    parentlevel: 1 }
                                    )
                                );
                        }
                    }
                }
                if (hitcount === 0) {
                    $content.append($('<li>No Results</li>'));
                }
                $content.page();
            }
            $.mobile.hidePageLoadingMsg();
        } // onSearchSubmit()

        // ------------------------------------------------------------------------------------
        // Persistance functions
        // ------------------------------------------------------------------------------------

        function _cacheSession(sessionid, username, customerid) {
            setOnline(false);
            localStorage['username'] = username;
            localStorage['customerid'] = customerid;
            localStorage['sessionid'] = sessionid;
        } //_cacheSession()

        function CswMobileStorage() {
            this.lastSyncSuccess = function() {
                var now = new Date();
                var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
                localStorage['lastSyncSuccess'] = ret;
                localStorage['lastSyncAttempt'] = ''; //clear last failed on next success
                localStorage['lastSyncTime'] = now;
                return ret;
            };
            this.lastSyncSuccess.toString = function() {
                return tryParseString(localStorage['lastSyncSuccess'],'');
            };
            this.lastSyncAttempt = function() {
                var now = new Date();
                var ret = now.toLocaleDateString() + ' ' + now.toLocaleTimeString();
                localStorage['lastSyncAttempt'] = ret;
                localStorage['lastSyncTime'] = now;
                return ret;
            };
            this.lastSyncAttempt.toString = function() {
                return tryParseString(localStorage['lastSyncAttempt'], '');
            };
            this.lastSyncTime = tryParseString(localStorage['lastSyncTime'], '');
            this.addUnsyncedChange = function() {
                var unSyncedChanges = tryParseNumber(localStorage['unSyncedChanges'], '0');
                unSyncedChanges++;
                localStorage['unSyncedChanges'] = unSyncedChanges;
                return unSyncedChanges;
            };
            this.clearUnsyncedChanges = function() {
                localStorage['unSyncedChanges'] = '0';
            };
            this.stayOffline = function(value) {
                if(arguments.length === 1) {
                    localStorage['stayOffline'] = isTrue(value);
                }
                var ret = isTrue(localStorage['stayOffline']);
                return ret;
            };
        }
        
        function _storeViewJson(rootid, rootname, viewJson, level) {
            var logger = new profileMethod('storeViewJson');
            if(level === 0)
            {
                var viewsCache = [tryParseString(localStorage["storedviews"], '')];

                for(var i=0; i< viewJson.length; i++)
                {
                    var thisView = viewJson[i];
                    var viewid = thisView['@id'];
                    var viewname = thisView['@name'];
                    if (viewsCache.indexOf('"rootid":"' + viewid + '"') === -1) {
                        storedViews.push({ rootid: viewid, name: viewname });
                    }
                }
                localStorage["storedviews"] = JSON.stringify(storedViews);
            }
            localStorage[rootid] = JSON.stringify({ name: rootname, json: viewJson, wasmodified: false });
            
//            $viewxml.andSelf().find('node').each(function() {
//                var $nodeXml = $(this).clone();
//                var nodeId = $nodeXml.CswAttrXml('id');
//                var nodeStr = xmlToString($nodeXml);
//                localStorage[nodeId] = nodeStr;
//            });
            
            cacheLogInfo(logger);
        }

        function _updateStoredViewJson(rootid, viewJson, wasmodified) {
            if (!isNullOrEmpty(localStorage[rootid]) && !isNullOrEmpty(viewJson)) {
                var view = JSON.parse(localStorage[rootid]);
                var update = { json: viewJson, wasmodified: wasmodified };
                if (view) $.extend(view, update);
                localStorage[rootid] = JSON.stringify(view);
                
//                $viewxml.andSelf().find('node').each(function() {
//                    var $nodeXml = $(this).clone();
//                    var nodeId = $nodeXml.CswAttrXml('id');
//                    var nodeStr = xmlToString($nodeXml);
//                    localStorage[nodeId] = nodeStr;
//                });
            }
            return viewJson;
        }

        function _getModifiedView(onSuccess) {
            var modified = false;
            if (!isNullOrEmpty(localStorage['storedviews'])) {
                var storedViews = JSON.parse(localStorage['storedviews']);

                for (var i = 0; i < storedViews.length; i++) {
                    stored = storedViews[i];
                    if (!isNullOrEmpty(localStorage[stored.rootid])) {
                        var view = JSON.parse(localStorage[stored.rootid]);
                        if (view.wasmodified) {
                            modified = true;
                            var rootid = stored.rootid;
                            var viewJson = view.json;
                            if (!isNullOrEmpty(rootid) && !isNullOrEmpty(viewJson)) {
                                onSuccess(rootid, viewJson);
                            }
                        }
                    }
                }
                if (!modified) {
                    _resetPendingChanges(true);
                    onSuccess();
                }
            }
        }

        function _fetchCachedViewJson(rootid) {
            var ret = {};
            if (!isNullOrEmpty(localStorage[rootid])) {
                //View is JSON: {name: '', json: '', wasmodified: ''}
                var rootObj = JSON.parse(localStorage[rootid]);
                ret = rootObj.json;
            }
            return ret;
        }
        
        function _fetchCachedNodeJson(rootid,nodeid) {
            var ret = {};
            if (!isNullOrEmpty(localStorage[rootid])) {
                //View is JSON: {name: '', json: '', wasmodified: ''}
                var rootObj = JSON.parse(localStorage[rootid]);
                var viewJson = currentViewJson( rootObj.json );
                var i = 0;
                
                while( isNullOrEmpty(ret) && i < viewJson.length ) {
                    var node = viewJson[i];
                    if(node['@id'] === nodeid) {
                        ret = node;
                    }
                    i++;
                }
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
                                onSuccess(currentViewJson( data ));
                            }
                        },
                        error: function(data) {
                            //setOffline();
                            
                            if (!isNullOrEmpty(onFailure)) {
                                onFailure(currentViewJson( data ));
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
                _getModifiedView(function(rootid, viewJson) {
                    if (!isNullOrEmpty(rootid) && !isNullOrEmpty(viewJson)) {
                        var dataJson = {
                            SessionId: SessionId,
                            ParentId: rootid,
                            UpdatedViewJson: viewJson,
                            ForMobile: ForMobile
                        };

                        CswAjaxJSON({
                                formobile: ForMobile,
                                url: opts.UpdateUrl,
                                data: dataJson,
                                onloginfail: function(text) {
                                    setOnline(false);
                                    if (perpetuateTimer) {
                                        _waitForData();
                                    }
                                    onLoginFail(text);
                                    $.mobile.hidePageLoadingMsg();
                                },
                                success: function(data) {
                                    logger.setAjaxSuccess();
                                    setOnline(false);
                                    var json = data;
                                    _updateStoredViewJson(rootid, json, '0');
                                    mobileStorage.clearUnsyncedChanges();
                                    _resetPendingChanges(true);
                                    if (perpetuateTimer) {
                                        _waitForData();
                                    }
                                    $.mobile.hidePageLoadingMsg();
                                },
                                error: function(data) {
                                    //setOffline();
                                    if (perpetuateTimer) {
                                        _waitForData();
                                    }
                                    $.mobile.hidePageLoadingMsg();
                                }
                            });
                    } else {
                        if (perpetuateTimer) {
                            _waitForData();
                        }
                        $.mobile.hidePageLoadingMsg();
                    }
                }); // _getModifiedView();
            } else {
                if (perpetuateTimer) {
                    _waitForData();
                }
                $.mobile.hidePageLoadingMsg();
            } // if(SessionId != '') 
            cacheLogInfo(logger);
        } //_processChanges()

        // For proper chaining support
        return this;
    };
})(jQuery);