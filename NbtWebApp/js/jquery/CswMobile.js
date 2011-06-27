﻿/// <reference path="../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../thirdparty/jquery/core/jquery.mobile/jquery.mobile.2011.5.27.js" />
/// <reference path="../thirdparty/jquery/plugins/jquery-validate-1.8/jquery.validate.js" />
/// <reference path="../thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />
/// <reference path="../CswClasses.js" />
/// <reference path="../thirdparty/js/modernizr-2.0.3.js" />

//var profiler = $createProfiler();

;
(function($) {
    /// <param name="$" type="jQuery" />

    $.fn.makeUL = function(id, params) {
        var p = {
            'data-filter': false,
            'data-role': 'listview',
            'data-inset': false
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
                var dataurl = $(this).CswAttrXml('data-url');
                var $thisPage = $('#' + dataurl);
                $thisPage.doChangePage();
            });

        }
        return $ret;
    };

    $.fn.doChangePage = function(options) {
        var o = {
            //transition: $.mobile.defaultPageTransition,
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
            //$div.cachePage(); //not yet, but we'll want to update the cache with the latest version of content
            var $page = $.mobile.activePage;
            var id = (isNullOrEmpty($page)) ? 'no ID' : $page.CswAttrDom('id');
            //if(doLogging()) log('doChangePage from: ' + id + ' to: ' + $div.CswAttrDom('id'),true);

            if (id !== $div.CswAttrDom('id')) ret = $.mobile.changePage($div, o);
        }
        return ret;
    };

    $.fn.doPage = function() {
        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div)) {
            //if(doLogging()) log('doPage on ' + $div.CswAttrDom('id'),true);
            //ret = $.mobile.loadPage( $div.CswAttrXml('data-url'));
            ret = $div.page(); //cachePage() //not yet, but we'll want to update the cache with the latest version of content
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
                $xml: '',
                parentlevel: 0,
                level: 1,
                HideRefreshButton: false,
                HideSearchButton: false,
//                persistBindEvent: true,
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
//                if (!p.persistBindEvent) {
//                    // If the page is constructed entirely from cache, we only do this once.
//                    $(this).unbind('pageshow');
//                }
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
            //ViewUrl: '/NbtWebApp/wsNBT.asmx/RunView',
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

        var ForMobile = true;
        var rootid;

        var UserName = localStorage["username"];
        if (!localStorage["sessionid"]) {
            Logout();
        }
        var SessionId = localStorage["sessionid"];
        var $currentViewXml;

        var storedViews = '';
        if (localStorage.storedViews) storedViews = JSON.parse(localStorage['storedviews']); // {name: '', rootid: ''}

        //$body.restorePages(); //not yet. someday.

        var $logindiv = _loadLoginDiv();
        var $viewsdiv = _loadViewsDiv();
        var $syncstatus = _makeSyncStatusDiv();
        var $helpdiv = _makeHelpDiv();
        var $sorrycharliediv = _loadSorryCharlieDiv();

        // case 20355 - error on browser refresh
        // there is a problem if you refresh with #viewsdiv where we'll generate a 404 error, but the app will continue to function
        if (!isNullOrEmpty(SessionId)) {
            $.mobile.path.set('#viewsdiv'); // we can use restorePages() to eliminate this later.
        }

        if (!isNullOrEmpty(SessionId)) {
            $viewsdiv = reloadViews();
            $viewsdiv.page();
            //$viewsdiv.doChangePage(); //JQM will do this for us.
            _waitForData();
        } else {
            // this will trigger _waitForData(), but we don't want to wait here
            _handleDataCheckTimer(
                function() {
                    // online
                    $logindiv.doPage();
                    $logindiv.doChangePage();
                },
                function() {
                    // offline
                    $sorrycharliediv.doPage();
                    $sorrycharliediv.doChangePage();
                }
            ); // _handleDataCheckTimer();
        } // if-else (configvar_sessionid != '' && configvar_sessionid != undefined)

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
            $('#loginsubmit').bind('click', function() {
                $.mobile.showPageLoadingMsg();
                onLoginSubmit();
            });
            $('#login_customerid').clickOnEnter($('#loginsubmit'));
            $('#login_username').clickOnEnter($('#loginsubmit'));
            $('#login_password').clickOnEnter($('#loginsubmit'));

            return $retDiv;
        }

        function _loadViewsDiv() {
            var params = {
                ParentId: '',
                DivId: 'viewsdiv',
                HeaderText: 'Views',
                $xml: '',
                parentlevel: -1,
                level: 0,
                HideRefreshButton: true,
                HideSearchButton: true,
                HideBackButton: true,
                onPageShow: function(p) { return _loadDivContents(p); }
            };
            var $retDiv = _addPageDivToBody({
                    DivId: 'viewsdiv',
                    HeaderText: 'Views',
                    HideRefreshButton: false,
                    HideSearchButton: true,
                    HideOnlineButton: false,
                    HideLogoutButton: false,
                    HideHelpButton: false,
                    HideCloseButton: true,
                    HideBackButton: true,
                    onPageShow: function(p) { return _loadDivContents(p); }
                });
            $retDiv.bindJqmEvents(params);
            return $retDiv;
        }

        function _loadSorryCharlieDiv(params) {
            var p = {
                DivId: 'sorrycharliediv',
                HeaderText: 'Sorry Charlie!',
                HideHelpButton: false,
                HideCloseButton: true,
                HideOnlineButton: false,
                content: 'You must have internet connectivity to login.'
            };
            if (params) $.extend(p, params);

            $retDiv = _addPageDivToBody({
                    DivId: p.DivId,
                    HeaderText: p.HeaderText,
                    HideSearchButton: true,
                    HideOnlineButton: p.HideOnlineButton,
                    HideRefreshButton: true,
                    HideLogoutButton: true,
                    HideHelpButton: p.HideHelpButton,
                    HideCloseButton: p.HideCloseButton,
                    HideBackButton: true,
                    dataRel: 'dialog',
                    $content: $('<p>' + p.content + '</p>')
                });
            return $retDiv;
        }

        function removeDiv(DivId) {
            $('#' + DivId).find('div:jqmData(role="content")').empty();
        }

        function reloadViews() {
            if ($viewsdiv) $viewsdiv.find('div:jqmData(role="content")').empty();
            var params = {
                parentlevel: -1,
                level: 0,
                DivId: 'viewsdiv',
                HeaderText: 'Views',
                HideRefreshButton: true,
                HideSearchButton: true,
                HideBackButton: true,
                onPageShow: function(p) { return _loadDivContents(p); }
            };
            $viewsdiv.bindJqmEvents(params);
            return $viewsdiv;
        }

        // ------------------------------------------------------------------------------------
        // Online indicator
        // ------------------------------------------------------------------------------------

        function setOffline() {
            amOnline(false);
            var $onlineStatus = $('.onlineStatus');
            if ($onlineStatus.hasClass('online')) {
                $onlineStatus.removeClass('online')
                                     .addClass('offline')
                                     .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                                     .text('Offline')
                    .end();
                $('.refresh').css('visibility', 'hidden');

                $viewsdiv = reloadViews(); //no changePage
            }
            if ($.mobile.activePage === $logindiv) {
                $sorrycharliediv.doPage(); // doChangePage();
            }
            $.mobile.hidePageLoadingMsg();
        }

        function setOnline() {
            amOnline(true);
            var $onlineStatus = $('.onlineStatus');
            if ($onlineStatus.hasClass('offline')) {
                $onlineStatus.removeClass('offline')
                                     .addClass('online')
                                     .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                                     .text('Online')
                    .end();

                $('.refresh').css('visibility', '');
                $viewsdiv = reloadViews(); //no changePage
            }
            if ($.mobile.activePage === $sorrycharliediv) {
                $logindiv.doPage(); //doChangePage();;
            }
        }

        function amOnline(amOnline) {
            if(arguments.length === 1 ) {
                localStorage['online'] = isTrue(amOnline);
            }
            var ret = isTrue(localStorage['online']);
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
                var $loggingBtn = $('.debug');
                $loggingBtn.removeClass('debug-off')
                                   .addClass('debug-on')
                                   .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                                   .text('Sync Log')
                    .end();
            }
        }

        function setStopLog() {
            if (!doLogging()) {
                var $loggingBtn = $('.debug');
                var logger = new profileMethod('setStopLog');
                cacheLogInfo(logger);

                var dataJson = {
                    'Context': 'CswMobile',
                    'UserName': localStorage['username'],
                    'CustomerId': localStorage['customerid'],
                    'LogInfo': sessionStorage['debuglog']
                };

                CswAjaxJSON({
                        url: opts.SendLogUrl,
                        data: dataJson,
                        success: function() {
                            $loggingBtn.removeClass('debug-on')
                                            .addClass('debug-off')
                                            .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                                            .text('Start Log')
                                .end();
                            purgeLogInfo();
                        }
                    });
            }
        }

        // ------------------------------------------------------------------------------------
        // List items fetching
        // ------------------------------------------------------------------------------------

        function _loadDivContents(params) {
            var logger = new profileMethod('loadDivContents');
            var p = {
                ParentId: '',
                level: 1,
                DivId: '',
                HeaderText: '',
                HideRefreshButton: false,
                HideSearchButton: false,
                $xml: '',
                SessionId: SessionId
            };
            if (params) $.extend(p, params);

            if (p.level === 1) {
                rootid = p.DivId;
            }
            var $retDiv = $('#' + p.DivId);

            if (isNullOrEmpty($retDiv) || $retDiv.length === 0 || $retDiv.find('div:jqmData(role="content")').length === 1) {
                if (p.level === 0) {
                    if (!amOnline()) {
                        p.$xml = _fetchCachedRootXml();
                        $retDiv = _loadDivContentsXml(p);
                    } else {
                        p.url = opts.ViewsListUrl;
                        $retDiv = _getDivXml(p);
                    }
                } else if (p.level === 1) {
                    // case 20354 - try cached first
                    var $xmlstr = _fetchCachedViewXml(rootid);
                    if (!isNullOrEmpty($xmlstr)) {
                        $currentViewXml = $xmlstr;
                        p.$xml = $currentViewXml;
                        $retDiv = _loadDivContentsXml(p);
                    } else if (amOnline()) {
                        p.url = opts.ViewUrl;
                        $retDiv = _getDivXml(p);
                    }
                } else  // Level 2 and up
                {
                    if (!isNullOrEmpty($currentViewXml)) {
                        p.$xml = $currentViewXml.find('#' + p.DivId)
                                                        .children('subitems').first();
                        $retDiv = _loadDivContentsXml(p);
                    }
                }
            }
            logger.setEnded();
            cacheLogInfo(logger);
            return $retDiv;
        } // _loadDivContents()

        function _loadDivContentsXml(params) {
            params.parentlevel = params.level;
            var $retDiv = _processViewXml(params);
            return $retDiv;
        }

        function _getDivXml(params) {
            var $retDiv = undefined;

            var p = {
                url: opts.ViewUrl
            };
            $.extend(p, params);

            var dataXml = {
                SessionId: p.SessionId,
                ParentId: p.DivId,
                formobile: ForMobile
            };
            //clearPath();
            CswAjaxXml({
                    //async: false,   // required so that the link will wait for the content before navigating
                    url: p.url,
                    data: dataXml,
                    onloginfail: function(text) { onLoginFail(text); },
                    success: function($xml) {
                        $currentViewXml = $xml;
                        p.$xml = $currentViewXml;
                        if (params.level === 1) {
                            _storeViewXml(p.DivId, p.HeaderText, $currentViewXml);
                        }
                        $retDiv = _loadDivContentsXml(p);
                    },
                    error: function(xml) {
                    }
                });

            return $retDiv;
        }

        var currenttab;

        function _processViewXml(params) {
            var logger = new profileMethod('processViewXml');
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xml: '',
                $xmlitem: '',
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

            p.$xml.children().each(function() {
                p.$xmlitem = $(this);
                _makeListItemFromXml($list, p)
                    .CswAttrXml('data-icon', false)
                    .appendTo($list);
            });
            
            //$list.bindLI(); //listview('refresh')
            try {
                $('.csw_collapsible').page();
                $('.csw_fieldset').page();
                $('.csw_listview').page();
            }
            catch(e)
            {
                $('.csw_collapsible').page();
                $('.csw_fieldset').page();
                $('.csw_listview').page();
            }
            $content.page();

            $.mobile.hidePageLoadingMsg();
            
            logger.setEnded();
            cacheLogInfo(logger);
            return $retDiv;
        } // _processViewXml()

        function _makeListItemFromXml($list, params) {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xmlitem: '',
                parentlevel: '',
                level: '',
                HideRefreshButton: false,
                HideSearchButton: false
            };
            if (params) $.extend(p, params);

            var id = makeSafeId({ ID: p.$xmlitem.CswAttrXml('id') });
            var text = p.$xmlitem.CswAttrXml('name');

            var IsDiv = (!isNullOrEmpty(id));
            var PageType = tryParseString(p.$xmlitem.get(0).nodeName, '').toLowerCase();

            var $retLI = $('');

            switch (PageType) {
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
                    var tab = p.$xmlitem.CswAttrXml('tab');

                    if (currenttab !== tab) {
                        //should be separate ULs eventually
                        $tab = $('<li data-role="list-divider">' + tab + '</li>')
                                                .appendTo($list);
                        currenttab = tab;
                    }

                    var $prop = _FieldTypeXmlToHtml(p.$xmlitem, id)
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
                    if (p.parentlevel === 0) {
//                        var $newDiv = _preFormNextLevelPages({
//                                ParentId: p.DivId,
//                                parentlevel: p.parentlevel,
//                                level: p.parentlevel + 1,
//                                DivId: id,
//                                // Case 22211: IDC content is not cached. We need to reconstruct nodes on each page load.
//                                persistBindEvent: true,
//                                HeaderText: text
//                                //,$toolbar: $toolbar
//                            });
//                        $newDiv.addClass('CswNbtView')
//                                           .doPage($newDiv.CswAttrXml('data-url'));
                        $retLI.bind('vclick', function() {
                            var par = {ParentId: p.DivId,
                                parentlevel: p.parentlevel,
                                level: p.parentlevel + 1,
                                DivId: id,
                                persistBindEvent: true,
                                HeaderText: text  };
                            var $div = _addPageDivToBody(par);
                            par.onPageShow = function() { _loadDivContents(par); };
                            $div.bindJqmEvents(par);
                            //$div.doPage();
                            $div.doChangePage({reloadPage: true});
                        });
                    }
                    break;
                }// default:
            }
            return $retLI;
        }// _makeListItemFromXml()

        function _makeObjectClassContent(params) {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xmlitem: '',
                parentlevel: '',
                HideRefreshButton: false,
                HideSearchButton: false
            };
            if (params) $.extend(p, params);

            var $retHtml;
            var Html = '';
            var id = makeSafeId({ ID: p.$xmlitem.CswAttrXml('id') });
            var NodeName = p.$xmlitem.CswAttrXml('name');
            var icon = '';
            if (!isNullOrEmpty(p.$xmlitem.CswAttrXml('iconfilename'))) {
                icon = 'images/icons/' + p.$xmlitem.CswAttrXml('iconfilename');
            }
            var ObjectClass = p.$xmlitem.CswAttrXml('objectclass');

            switch (ObjectClass) {
            case "InspectionDesignClass":
                var DueDate = p.$xmlitem.find('prop[ocpname="Due Date"]').CswAttrXml('gestalt');
                var Location = p.$xmlitem.find('prop[ocpname="Location"]').CswAttrXml('gestalt');
                var MountPoint = p.$xmlitem.find('prop[ocpname="Target"]').CswAttrXml('gestalt');
                var Status = p.$xmlitem.find('prop[ocpname="Status"]').CswAttrXml('gestalt');
                var UnansweredCnt = 0;

                p.$xmlitem.find('prop[fieldtype="Question"]').each(function() {
                    var $question = $(this);
                    if (isNullOrEmpty($question.children('Answer').text())) {
                        UnansweredCnt++;
                    }
                });

                Html += '<li>';
                if (!isNullOrEmpty(icon))
                    Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">';
                Html += '<h2>' + NodeName + '</h2>';
                Html += '<p>' + Location + '</p>';
                Html += '<p>' + MountPoint + '</p>';
                Html += '<p>';
                if (!isNullOrEmpty(Status)) Html += Status + ', ';
                Html += 'Due: ' + DueDate + '</p>';
                Html += '<span id="' + makeSafeId({ prefix: id, ID: 'unansweredcnt' }) + '" class="ui-li-count">' + UnansweredCnt + '</span>';
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

//            var $newDiv = _preFormNextLevelPages({
//                                  ParentId: p.DivId,
//                                  parentlevel: p.parentlevel,
//                                  level: p.parentlevel + 1,
//                                  DivId: id,
//                                  HeaderText: NodeName
//                              })
//                                  .addClass('CswNbtNode');

            $retHtml = $(Html); //.listview('refresh');
            
            return $retHtml;
        }

        function _FieldTypeXmlToHtml($xmlitem, ParentId) {
            var IdStr = makeSafeId({ ID: $xmlitem.CswAttrXml('id') });
            var FieldType = $xmlitem.CswAttrXml('fieldtype');
            var PropName = $xmlitem.CswAttrXml('name');
            var ReadOnly = (isTrue($xmlitem.CswAttrXml('isreadonly')));

            // Subfield values
            var sf_text = tryParseString($xmlitem.children('text').text(), '');
            var sf_value = tryParseString($xmlitem.children('value').text(), '');
            var sf_href = tryParseString($xmlitem.children('href').text(), '');
            var sf_checked = tryParseString($xmlitem.children('checked').text(), '');
            var sf_required = tryParseString($xmlitem.children('required').text(), '');
            var sf_units = tryParseString($xmlitem.children('units').text(), '');
            var sf_answer = tryParseString($xmlitem.children('answer').text(), '');
            var sf_allowedanswers = tryParseString($xmlitem.children('allowedanswers').text(), '');
            var sf_correctiveaction = tryParseString($xmlitem.children('correctiveaction').text(), '');
            var sf_comments = tryParseString($xmlitem.children('comments').text(), '');
            var sf_compliantanswers = tryParseString($xmlitem.children('compliantanswers').text(), '');
            var sf_options = tryParseString($xmlitem.children('options').text(), '');

            var $retLi = $('<li id="' + IdStr + '_li"></li>')
                                .CswAttrXml('data-icon', false);
            var $label = $('<h2 id="' + IdStr + '_label" style="white-space:normal;" class="csw_prop_label">' + PropName + '</h2>')
                            .appendTo(($retLi));
            
            var $fieldcontain = $('<div class="csw_fieldset" data-role="fieldcontain"></div>')
                                    .appendTo($retLi);
            var $propDiv;
            
            if (FieldType === "Question" &&
                !(sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) &&
                    isNullOrEmpty(sf_correctiveaction)) {
                $label.addClass('OOC');
            } else {
                $label.removeClass('OOC');
            }
            
            if( FieldType !== 'Question' && FieldType !== 'Logical') {
                $retLi.css('height',(FieldType === 'Memo') ? '100px' : '75px');
                $fieldcontain.addClass('ui-grid-a');
                $propDiv = $('<div class="ui-block-b"></div>')
                                .appendTo($fieldcontain);
            }
            
            var $prop;
            var propId = IdStr + '_input';
            
            if (!ReadOnly) {
                var addChangeHandler = true;
                
                switch (FieldType) {
                case "Date":
                    $prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value });
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
                    $propDiv.append($('<p id="' + propId + '">' + sf_text + '</p>'));
                    break;
                case "Text":
                    $prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_text });
                    break;
                case "Time":
                    $prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value });
                    break;
                default:
                    $propDiv.append($('<p id="' + propId + '">' + $xmlitem.CswAttrXml('gestalt') + '</p>'));
                    break;
                } // switch (FieldType)

                if (addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0) {
                    $prop.bind('change', function(eventObj) {
                        var $this = $(this);
                        onPropertyChange(ParentId, eventObj, $this.val(), propId);
                    });
                }
            } else {
                $propDiv.append($('<p id="' + propId + '">' + $xmlitem.CswAttrXml('gestalt') + '</p>'));
            }
            return $retLi;
        }

        function _FieldTypeHtmlToXml($xmlitem, id, value) {
            var name = new CswString(id);
            var IdStr = makeSafeId({ ID: $xmlitem.CswAttrXml('id') });
            var fieldtype = $xmlitem.CswAttrXml('fieldtype');
            var propname = $xmlitem.CswAttrXml('name');

            // subfield nodes
            var $sf_text = $xmlitem.children('text');
            var $sf_value = $xmlitem.children('value');
            var $sf_href = $xmlitem.children('href');
            var $sf_options = $xmlitem.children('options');
            var $sf_checked = $xmlitem.children('checked');
            var $sf_required = $xmlitem.children('required');
            var $sf_units = $xmlitem.children('units');
            var $sf_answer = $xmlitem.children('answer');
            var $sf_allowedanswers = $xmlitem.children('allowedanswers');
            var $sf_correctiveaction = $xmlitem.children('correctiveaction');
            var $sf_comments = $xmlitem.children('comments');
            var $sf_compliantanswers = $xmlitem.children('compliantanswers');

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
                } else if (name.contains(makeSafeId({ ID: IdStr, suffix: 'ans' }))) {
                    $sftomodify = $sf_answer;
                } else if (name.contains(makeSafeId({ ID: IdStr, suffix: 'cor' }))) {
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
                $xmlitem.CswAttrXml('wasmodified', '1');
            }
        }// _FieldTypeHtmlToXml()

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
                var answertext;
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
                //var $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] });
                var $input = $('<button name="' + inputName + '" id="' + inputId + '" class="csw_logical">' + answertext + '</button>')
                                .appendTo($fieldset);
                

                // Checked is a Tristate, so isTrue() is not useful here
                if ((Checked === 'false' && answers[i] === 'False') ||
                    (Checked === 'true' && answers[i] === 'True') ||
                        (Checked === '' && answers[i] === 'Null')) {
                    $input.CswAttrXml({ 'data-theme': 'b' });
                }
                else {
                    $input.CswAttrXml({ 'data-theme': 'c' });
                }
                
                $input.unbind('vclick');
                $input.bind('vclick', function(eventObj) {
                    var $this = $(this);
                    var thisInput = eventObj.srcElement.innerText;
                    var realVal = '';
                    switch(thisInput) {
                        case '?':
                            realVal = 'null';
                            break;
                        case 'Yes':
                            realVal = 'true';
                            break;
                        case 'No':
                            realVal = 'false';
                            break;
                    }
                    
                    for (var i = 0; i < answers.length; i++) {
                        var inpId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });
                        var $inpBtn = $('#' + inpId);

                        if($inpBtn.text() === thisInput ) {
                            $inpBtn = toggleButton($inpBtn, true);
                        }
                        else {
                            $inpBtn = toggleButton($inpBtn, false);
                        }
                    }
                    onPropertyChange(ParentId, eventObj, realVal, inputId);
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
								        'data-type': 'horizontal'
								    });
            var answers = Options.split(',');
            var answerName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unqiue and shared

            for (var i = 0; i < answers.length; i++) {
                var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });
                
                var $answer = $('<button name="' + answerName + '" id="' + answerid + '" class="csw_answer">' + answers[i] + '</button>')
                                .appendTo($fieldset);
                
                if (Answer === answers[i]) {
                    $answer.CswAttrXml({ 'data-theme': 'b' });
                }
                else {
                    $answer.CswAttrXml({ 'data-theme': 'c' });
                }
                
                $answer.unbind('vclick');
                $answer.bind('vclick', function(eventObj) {

                    var $this = $(this);
                    var thisAnswer = eventObj.srcElement.innerText;

                    for (var i = 0; i < answers.length; i++) {
                        var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });
                        var $ansBtn = $('#' + answerid);

                        if($ansBtn.text() === thisAnswer ) {
                            $ansBtn = toggleButton($ansBtn, true);
                        }
                        else {
                            $ansBtn = toggleButton($ansBtn, false);
                        }
                    }

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
                    if (!isNullOrEmpty(Answer)) {
                        // update unanswered count when this question is answered
                        var $parentfieldset = $('#' + IdStr + '_fieldset');
                        if ($parentfieldset.CswAttrDom('answered')) {
                            var $cntspan = $('#' + ParentId + '_unansweredcnt');
                            $cntspan.text(parseInt($cntspan.text()) - 1);
                            $parentfieldset.CswAttrDom('answered', 'true');
                        }
                    }
                    onPropertyChange(ParentId, eventObj, $this.text(), answerName);
                    return false;
                });
                //$retHtml.data('thisI', i);
            } // for (var i = 0; i < answers.length; i++)
            
//            $retHtml.find('input[type="radio"]').checkboxradio();
            return $fieldset;
        } // _makeQuestionAnswerFieldSet()

        function toggleButton($button,on) {
            if(on) {
                $button.CswAttrXml({ 'data-theme': 'b' })
                       .removeClass('ui-btn-hover-c ui-btn-up-c')
                       .parent('div')
                       .removeClass('ui-btn-hover-c ui-btn-up-c')
                       .addClass('ui-btn-up-b')
                       .CswAttrXml({ 'data-theme': 'b' });
            }
            else {
                $button.CswAttrXml({ 'data-theme': 'c' })
                       .removeClass('ui-btn-hover-b ui-btn-up-b')
                       .parent('div')
                       .removeClass('ui-btn-hover-b ui-btn-up-b')
                       .addClass('ui-btn-up-c')
                       .CswAttrXml({ 'data-theme': 'c' });
            }
            
            return $button;
        }
        
        function _preFormNextLevelPages(params) {
            var $retDiv = undefined;
            var p = {
                ParentId: '',
                parentlevel: 0,
                level: 1,
                DivId: '',
                HeaderText: '',
                $toolbar: '',
                persistBindEvent: false,
                onPageShow: function(p) { return _loadDivContents(p); }
            };
            if (params) $.extend(p, params);

            $retDiv = _addPageDivToBody(p);
            $retDiv.bindJqmEvents(p);
            return $retDiv;
        }

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

            if (isNullOrEmpty($pageDiv) || $pageDiv.length === 0) {
                $pageDiv = $body.CswDiv('init', { ID: p.DivId })
                                        .CswAttrXml({
                                        'data-role': 'page',
                                        'data-url': p.DivId,
                                        'data-title': p.HeaderText,
                                        'data-rel': p.dataRel
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
                                                     //'data-url': p.DivId + '_back',
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
//                if ( isNullOrEmpty(p.ParentId) )
//                {
//                    $backlink.css('visibility','hidden');
//                }

                if (!isNullOrEmpty(p.backicon)) {
                    $backlink.CswAttrXml('data-icon', p.backicon);
                } else {
                    $backlink.CswAttrXml('data-icon', 'arrow-l');
                }

                $header.append($('<h1>' + p.HeaderText + '</h1>'));

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

                var $content = $pageDiv.CswDiv('init', { ID: p.DivId + '_content' })
                                               .CswAttrXml({ 'data-role': 'content', 'data-theme': opts.Theme })
                                               .append(p.$content);
                var $footer = $pageDiv.CswDiv('init', { ID: p.DivId + '_footer', cssclass: 'ui-bar' })
                                              .CswAttrXml({
                                              'data-role': 'footer',
                                              'data-theme': opts.Theme,
                                              'data-position': 'fixed',
                                              'data-id': 'csw_footer'
                                          });

                var $footerCtn = $('<div data-role="navbar">')
                                            .appendTo($footer);
                var onlineClass = (!amOnline()) ? 'onlineStatus offline' : 'onlineStatus online';
                var onlineValue = (!amOnline()) ? 'Offline' : 'Online';

                $syncstatusBtn = $footerCtn.CswLink('init', {
                                                        'href': 'javascript:void(0)',
                                                        ID: p.DivId + '_gosyncstatus',
                                                        cssclass: onlineClass, // + ' ui-btn-left',  
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
                                                       }) //, ui-btn-left'})
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
                    //, cssclass: 'ui-btn-left' })
                                                        .CswAttrXml({
                                                        'data-identity': p.DivId + '_logout',
                                                        'data-url': p.DivId + '_logout',
                                                        'data-transition': 'flip'
                                                    })
                                                        .css('display', '');


                var $mainBtn = $footerCtn.CswLink('init', { href: 'NewMain.html', rel: 'external', ID: p.DivId + '_newmain', value: 'Full Site' })
                                              .CswAttrXml({ 'data-transition': 'pop' });


                $helpBtn = $footerCtn.CswLink('init', {
                                             'href': 'javascript:void(0)',
                                             ID: p.DivId + '_help',
                                             value: 'Help'
                                         })
                    //, cssclass: 'ui-btn-left' })
                                             .CswAttrXml({
                                             'data-identity': p.DivId + '_help',
                                             'data-url': p.DivId + '_help',
                                             'data-transition': 'pop',
                                             'data-rel': 'dialog'
                                         })
                                             .css('display', '');

                $loggingBtn = $footerCtn.CswLink('init', {
                                                 'href': 'javascript:void(0)',
                                                 ID: p.DivId + '_debuglog',
                                                 value: doLogging() ? 'Start Log' : 'Sync Log',
                                                 cssclass: 'debug'
                                             })
                                                 .addClass(doLogging() ? 'debug-on' : 'debug-off');
            }

            if (p.HideOnlineButton) {
                $syncstatusBtn.css('display', 'none');
            } else {
                $syncstatusBtn.css('display', '');
            }
            if (p.HideHelpButton) {
                $helpBtn.css('display', 'none');
            } else {
                $helpBtn.css('display', '');
            }
            if (p.HideLogoutButton) {
                $logoutBtn.css('display', 'none');
            } else {
                $logoutBtn.css('display', '');
            }
            if (p.HideRefreshButton) {
                $refreshBtn.css('display', 'none');
            } else {
                $refreshBtn.css('display', '');
            }
            if (p.HideSearchButton) {
                $searchBtn.css('display', 'none');
            } else {
                $searchBtn.css('display', '');
            }
            if (p.HideHeaderOnlineButton) {
                $headerOnlineBtn.css('display', 'none');
            } else {
                $headerOnlineBtn.show()
                                        .css('display', '');
            }
            if (p.dataRel === 'dialog' && !p.HideCloseButton) {
                $closeBtn.css('display', '');
            } else {
                $closeBtn.css('display', 'none');
            }
            if (!p.HideBackButton) {
                $backlink.css('display', '');
            } else {
                $backlink.css('display', 'none');
            }
            if (debugOn()) {
                $loggingBtn.css({ 'display': '' });
            } else {
                $loggingBtn.css({ 'display': 'none' });
            }

            _bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv);

            //$pageDiv.cachePage(); //not yet

            return $pageDiv;

        }// _addPageDivToBody()

        function _getDivHeaderText(DivId) {
            return $('#' + DivId).find('div:jqmData(role="header") h1').text();
        }

        function _addToDivHeaderText($div, text) {
            $div.find('div:jqmData(role="header") h1').append($('<p style="color: yellow;">' + text + '</p>'));
            $.mobile.loadPage($div);
            return $div;
        }

        function _bindPageEvents(DivId, ParentId, level, $div) {
            $div.find('#' + DivId + '_searchopen')
                        .unbind('vclick')
                        .bind('vclick', function() {
                            onSearchOpen(DivId);
                            return false;
                        })
                        .end()
                        .find('#' + DivId + '_gosyncstatus')
                        .unbind('vclick')
                        .bind('vclick', function() {
                            onSyncStatusOpen(DivId);
                            return false;
                        })
                        .end()
                        .find('#' + DivId + '_refresh')
                        .unbind('vclick')
                        .bind('vclick', function() {
                            $.mobile.showPageLoadingMsg();
                            return onRefresh();
                        })
                        .end()
                        .find('#' + DivId + '_logout')
                        .unbind('vclick')
                        .bind('vclick', function(e) {
                            onLogout(DivId, e);
                            return false;
                        })
                        .end()
                        .find('#' + DivId + '_help')
                        .unbind('vclick')
                        .bind('vclick', function() {
                            onHelp(DivId, ParentId);
                            return false;
                        })
                        .end()
                        .find('#' + DivId + '_debuglog')
                        .die('vclick')
                        .live('vclick', function() {
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
            content += '<p>Pending Unsynced Changes: <span id="ss_pendingchangecnt">No</span></p>';
            content += '<p>Last sync: <span id="ss_lastsync"></span></p>';
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
                    HideCloseButton: false,
                    HideBackButton: true,
                    HideHeaderOnlineButton: false
                });

            $retDiv.find('#ss_forcesync')
                            .bind('vclick', function() {
                                _processChanges(false);
                                return false;
                            })
                            .end()
                            .find('#ss_gooffline')
                            .bind('vclick', function() {
                                _toggleOffline();
                                return false;
                            });

            return $retDiv;
        }

        function _toggleOffline() {
            //eventObj.preventDefault();
            if (!amOnline()) {
                _clearWaitForData();
                _waitForData();
                setOnline();
                $('#ss_gooffline span').text('Go Offline');
            } else {
                _clearWaitForData();
                setOffline();
                $('#ss_gooffline span').text('Go Online');
            }
        }

        function _resetPendingChanges(val, setlastsyncnow) {
            if (val) {
                $('#ss_pendingchangecnt').text('Yes');
                $('.onlineStatus').addClass('pendingchanges');
            } else {
                $('#ss_pendingchangecnt').text('No');
                $('.onlineStatus').removeClass('pendingchanges');
            }
            if (setlastsyncnow) {
                var d = new Date();
                $('#ss_lastsync').text(d.toLocaleDateString() + ' ' + d.toLocaleTimeString());
            }
        }

        // returns true if no pending changes or user is willing to lose them
        function _checkNoPendingChanges() {
            return (!_pendingChanges() ||
                confirm('You have pending unsaved changes.  These changes will be lost.  Continue?'));
        }

        function _pendingChanges() {
            return ($('#ss_pendingchangecnt').text() === 'Yes');
        }

        // ------------------------------------------------------------------------------------
        // Help Div
        // ------------------------------------------------------------------------------------

        function _makeHelpDiv() {
            var $help = $('<p>Help</p>')
                                    .append('</br></br></br>');
            var $logLevelDiv = $help.CswDiv('init')
                                            .CswAttrXml({ 'data-role': 'fieldcontain' });
            var $logLevelLabel = $('<label for="mobile_log_level">Logging</label>')
                                            .appendTo($logLevelDiv);

            var $logLevelSelect = $logLevelDiv.CswSelect('init', {
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

                //clearPath();
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
                            $viewsdiv.doChangePage({changeHash: false});
                            //restorePath();
                        },
                        error: function(XMLHttpRequest, textStatus, errorThrown) {
                            $.mobile.hidePageLoadingMsg();
                        }
                    });
            }

        }

//onLoginSubmit() 

        function onLoginFail(text) {
            Logout(false);
            $.mobile.hidePageLoadingMsg();
            _addToDivHeaderText($logindiv, text);
        }

        function onLogout() {
            Logout(true);
        }

        function Logout(reloadWindow) {
            if (_checkNoPendingChanges()) {
                _clearStorage();
                // reloading browser window is the easiest way to reset
                if (reloadWindow) {
                    window.location.href = window.location.pathname;
                }
            }
        }

        function _clearStorage() {
            sessionStorage.clear();
            localStorage.clear();
        }

        function onRefresh() {
            if (amOnline()) {
                if (_checkNoPendingChanges()) {
                    continueRefresh();
                }
            }
            return false;
        }

        function continueRefresh() {
            var DivId = localStorage['currentviewid'];
            if (!isNullOrEmpty(DivId)) {
                var HeaderText = _getDivHeaderText(DivId);
                var dataXml = {
                    SessionId: SessionId,
                    ParentId: DivId,
                    ForMobile: ForMobile
                };

                CswAjaxXml({
                        async: false,   // required so that the link will wait for the content before navigating
                        formobile: ForMobile,
                        url: opts.ViewUrl,
                        data: dataXml,
                        stringify: false,
                        onloginfail: function(text) { onLoginFail(text); },
                        success: function($xml) {
                            $currentViewXml = $xml;
                            _updateStoredViewXml(DivId, $currentViewXml, '0');

                            var params = {
                                ParentId: 'viewsdiv',
                                DivId: DivId,
                                HeaderText: HeaderText,
                                '$xml': $currentViewXml,
                                parentlevel: 0,
                                level: 1,
                                HideRefreshButton: false,
                                HideSearchButton: false,
                                HideBackButton: false,
                                onPageShow: function(p) { return _loadDivContents(p); }
                            };

                            var $thisDiv = _loadDivContents(params);
                            //$thisDiv.bindJqmEvents(params);
                            $thisDiv.doChangePage();

                            $.mobile.hidePageLoadingMsg();
                        }, // success
                        error: function(txt) {
                        }
                    });
            }
        }

        function onSyncStatusOpen(DivId) {
            $syncstatus.doChangePage({ transition: 'slideup' });
        }

        function onHelp(DivId) {
            $help = _makeHelpDiv();
            $help.doChangePage({ transition: 'slideup' });
        }

        function onPropertyChange(DivId, eventObj, inputVal, inputId) {
            var logger = new profileMethod('onPropertyChange');
            var $elm = $(eventObj.target);

            var name = tryParseString(inputId,$elm.CswAttrDom('id'))
            var value = tryParseString(inputVal, eventObj.target.innerText);
       
            // update the xml and store it
            if (!isNullOrEmpty($currentViewXml)) {
                var $divxml = $currentViewXml.find('#' + DivId);
                $divxml.andSelf().find('prop').each(function() {
                    var $fieldtype = $(this);
                    _FieldTypeHtmlToXml($fieldtype, name, value);
                });

                _updateStoredViewXml(rootid, $currentViewXml, '1');
                _resetPendingChanges(true, false);
            }
            logger.setEnded();
            cacheLogInfo(logger);
        } // onPropertyChange()

        function onSearchOpen(DivId) {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            var $xmlstr = _fetchCachedViewXml(rootid);
            if (!isNullOrEmpty($xmlstr)) {
                var $wrapper = $('<div></div>');
                var $fieldCtn = $('<div data-role="fieldcontain"></div>')
                                            .appendTo($wrapper);
                var $select = $('<select id="' + DivId + '_searchprop" name="' + DivId + '_searchprop" class="csw_prop_select">')
                                            .appendTo($fieldCtn)
                                            .CswAttrXml({ 'data-native-menu': 'false' });

                $xmlstr.children('search').each(function() {
                    var $search = $(this);
                    var $option = $('<option value="' + $search.CswAttrXml('id') + '">' + $search.CswAttrXml('name') + '</option>')
                                            .appendTo($select);
                });

                var $searchCtn = $('<div data-role="fieldcontain"></div>')
                                            .appendTo($wrapper);
                var $searchBox = $searchCtn.CswInput('init', { type: CswInput_Types.search, ID: DivId + '_searchfor' })
                                                    .CswAttrXml({
                                                    'placeholder': 'Search',
                                                    'data-placeholder': 'Search'
                                                });
                var $goBtn = $wrapper.CswLink('init', { type: 'button', ID: DivId + '_searchgo', value: 'Go', href: 'javascript:void(0)' })
                                                .CswAttrXml({ 'data-inline': 'true' })
                                                .bind('click', function() {
                                                    onSearchSubmit(DivId);
                                                });
                var $results = $wrapper.CswDiv('init', { ID: DivId + '_searchresults' });

                var $searchDiv = _addPageDivToBody({
                        ParentId: DivId,
                        DivId: 'CswMobile_SearchDiv',
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
                $searchDiv.doChangePage({ transition: 'slideup', changeHash: false });
            }
        }

        function onSearchSubmit(DivId) {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            var $resultsDiv = $('#' + DivId + '_searchresults')
                .empty();
            var $xmlstr = _fetchCachedViewXml(rootid);
            if (!isNullOrEmpty($xmlstr)) {
                var $content = $resultsDiv.makeUL(DivId + '_searchresultslist', { 'data-filter': false })
                                                    .append($('<li data-role="list divider">Results</li>'));

                var hitcount = 0;
                $xmlstr.find('node').each(function() {
                    var $node = $(this);
                    if (!isNullOrEmpty($node.CswAttrXml(searchprop))) {
                        if ($node.CswAttrXml(searchprop).toLowerCase().indexOf(searchfor.toLowerCase()) >= 0) {
                            hitcount++;
                            $content.append(_makeListItemFromXml($content, {
                                    ParentId: DivId + '_searchresults',
                                    DivId: DivId + '_searchresultslist',
                                    HeaderText: 'Results',
                                    $xmlitem: $node,
                                    parentlevel: 1
                                })
                                            );
                            $content.bindLI();
                        }
                    }
                });
                if (hitcount === 0) {
                    $content.append($('<li>No Results</li>'));
                }
                $content.listview('refresh');
            }
        }

// onSearchSubmit()

        // ------------------------------------------------------------------------------------
        // Persistance functions
        // ------------------------------------------------------------------------------------

        function _cacheSession(sessionid, username, customerid) {
            localStorage['online'] = true;
            localStorage['username'] = username;
            localStorage['customerid'] = customerid;
            localStorage['sessionid'] = sessionid;
        }

//_cacheSession()

        function _storeViewXml(rootid, rootname, $viewxml) {
            if (isNullOrEmpty(storedViews)) {
                storedViews = [{ rootid: rootid, name: rootname }];
            } else if (storedViews.indexOf(rootid) === -1) {
                storedViews.push({ rootid: rootid, name: rootname });
            }
            localStorage["storedviews"] = JSON.stringify(storedViews);
            localStorage[rootid] = JSON.stringify({ name: rootname, xml: xmlToString($viewxml), wasmodified: false });
        }

        function _updateStoredViewXml(rootid, $viewxml, wasmodified) {
            if (!isNullOrEmpty(localStorage[rootid])) {
                var view = JSON.parse(localStorage[rootid]);
                var update = { xml: xmlToString($viewxml), wasmodified: wasmodified };
                if (view) $.extend(view, update);
                localStorage[rootid] = JSON.stringify(view);
            }
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
                            var viewxml = view.xml;
                            if (!isNullOrEmpty(rootid) && !isNullOrEmpty(viewxml)) {
                                onSuccess(rootid, viewxml);
                            }
                        }
                    }
                }
                if (!modified) {
                    _resetPendingChanges(false, true);
                    onSuccess();
                }
            }
        }

        function _fetchCachedViewXml(rootid) {
            var $view;
            if (!isNullOrEmpty(localStorage[rootid])) {
                //View is JSON: {name: '', xml: '', wasmodified: ''}
                var rootObj = JSON.parse(localStorage[rootid]);
                var rootXml = rootObj.xml;
                $view = $(rootXml);
            }
            return $view;
        }

        function _fetchCachedRootXml() {
            var ret = '';
            for (var view in storedViews) {
                ret += "<view id=\"" + view.rootid + "\" name=\"" + view.name + "\" />";
            }
            return $(ret);
        }

        // ------------------------------------------------------------------------------------
        // Synchronization
        // ------------------------------------------------------------------------------------

        var _waitForData_TimeoutId;

        function _waitForData() {
            _waitForData_TimeoutId = setTimeout(_handleDataCheckTimer, opts.PollingInterval);
        }

        function _clearWaitForData() {
            clearTimeout(_waitForData_TimeoutId);
        }

        function _handleDataCheckTimer(onSuccess, onFailure) {
            var url = opts.ConnectTestUrl;
            if (opts.RandomConnectionFailure) {
                url = opts.ConnectTestRandomFailUrl;
            }
            //clearPath();
            CswAjaxXml({
                    formobile: ForMobile,
                    url: url,
                    data: {  },
                    stringify: false,
                    onloginfail: function(text) { onLoginFail(text); },
                    success: function($xml) {
                        setOnline();
                        _processChanges(true);
                        if (!isNullOrEmpty(onSuccess)) {
                            onSuccess($xml);
                        }
                        //restorePath();
                    },
                    error: function(xml) {
                        setOffline();
                        var $xml = $(xml);
                        if (!isNullOrEmpty(onFailure)) {
                            onFailure($xml);
                        }
                        _waitForData();
                        //restorePath();
                    }
                });
        }

//_handleDataCheckTimer()

        function _processChanges(perpetuateTimer) {
            var logger = new profileMethod('processChanges');
            if (!isNullOrEmpty(SessionId)) {
                _getModifiedView(function(rootid, viewxml) {
                    if (!isNullOrEmpty(rootid) && !isNullOrEmpty(viewxml)) {
                        var dataJson = {
                            SessionId: SessionId,
                            ParentId: rootid,
                            UpdatedViewXml: viewxml,
                            ForMobile: ForMobile
                        };

                        //clearPath();
                        CswAjaxJSON({
                                formobile: ForMobile,
                                url: opts.UpdateUrl,
                                data: dataJson,
                                stringify: true,
                                onloginfail: function(text) {
                                    if (perpetuateTimer) {
                                        _waitForData();
                                    }
                                    onLoginFail(text);
                                },
                                success: function(data) {
                                    logger.setAjaxSuccess();
                                    var $xml = data.xml;
                                    _updateStoredViewXml(rootid, $xml, '0');
                                    _resetPendingChanges(false, true);
                                    if (perpetuateTimer) {
                                        _waitForData();
                                    }
                                },
                                error: function(data) {
                                    if (perpetuateTimer) {
                                        _waitForData();
                                    }
                                    //restorePath();
                                }
                            });
                    } else {
                        if (perpetuateTimer) {
                            _waitForData();
                        }
                    }
                }); // _getModifiedView();
            } else {
                if (perpetuateTimer)
                    _waitForData();
            } // if(SessionId != '') 
            logger.setEnded();
            cacheLogInfo(logger);
        }

//_processChanges()

        // For proper chaining support
        return this;
    };
})(jQuery);