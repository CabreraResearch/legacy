/// <reference path="../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../thirdparty/jquery/core/jquery.mobile/jquery.mobile.2011.5.27.js" />
/// <reference path="../thirdparty/jquery/plugins/jquery-validate-1.8/jquery.validate.js" />
/// <reference path="../thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />
/// <reference path="../CswClasses.js" />
/// <reference path="../thirdparty/js/modernizr-2.0.3.js" />

//var profiler = $createProfiler();

; (function ($) { /// <param name="$" type="jQuery" />

    $.fn.makeUL = function(id, params)
    {
        var p = {
            'data-filter': true,
            'data-role': 'listview',
            'data-inset': true
        };
        if (params) $.extend(p, params);

        var $div = $(this);
        var $ret = undefined;
        if (!isNullOrEmpty($div))
        {
            $ret = $('<ul id="' + tryParseString(id, '') + '"></ul>')
                                            .appendTo($div)
                                            .CswAttrXml(p);
            $ret.listview();
        }
        return $ret;
    };

    $.fn.bindLI = function()
    {
        var $li = $(this);
        var $ret = undefined;
        if (!isNullOrEmpty($li))
        {
            $li.unbind('click');
            $ret = $li.find('li a').bind('click', function()
                {
                var dataurl = $(this).CswAttrXml('data-url');
                var $thisPage = $('#' + dataurl);
                $thisPage.doChangePage();
            });

        }
        return $ret;
    };

    $.fn.doChangePage = function(options)
    {
        var o = {
            transition: $.mobile.defaultPageTransition,
            reverse: false,
            changeHash: true,
            role: 'page',
            pageContainer: $.mobile.pageContainer,
            type: 'get',
            data: undefined,
            reloadPage: false,
            showLoadMsg: true
        };
        if (options) $.extend(o, options);

        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div))
        {
            //$div.cachePage(); //not yet, but we'll want to update the cache with the latest version of content
            var $page = $.mobile.activePage;
            var id = (isNullOrEmpty($page)) ? 'no ID' : $page.CswAttrDom('id');
            //if(doLogging()) log('doChangePage from: ' + id + ' to: ' + $div.CswAttrDom('id'),true);

            if (id !== $div.CswAttrDom('id')) ret = $.mobile.changePage($div.CswAttrXml('data-url'), o);
        }
        return ret;
    };

    $.fn.doPage = function()
    {
        var $div = $(this);
        var ret = false;
        if (!isNullOrEmpty($div))
        {
            //if(doLogging()) log('doPage on ' + $div.CswAttrDom('id'),true);
            //ret = $.mobile.loadPage( $div.CswAttrXml('data-url'));
            ret = $div.page(); //cachePage() //not yet, but we'll want to update the cache with the latest version of content
        }
        return ret;
    };

    $.fn.cachePage = function()
    {
        // we have the technology, we can persist the DOM
        var $div = $(this);
        var divid = $div.CswAttrDom('id');
        var storedPages = [];
        if (!isNullOrEmpty(sessionStorage.storedPages))
        {
            storedPages = sessionStorage.storedPages.split(',');
        }
        if (storedPages.indexOf(divid) === -1)
        {
            storedPages.push(divid);
        }
        sessionStorage.storedPages = storedPages.toString();
        sessionStorage[divid] = xmlToString($div);
        return $div;
    };

    $.fn.restorePages = function(params)
    {
        //this also needs to bindJqmEvents, but let's not inject this now.
        var $parent = $(this);
        if (!isNullOrEmpty(sessionStorage.storedPages))
        {
            var storedPages = sessionStorage.storedPages.split(',');
            for (var i = 0; i < storedPages.length; i++)
            {
                var divid = storedPages[i];
                if (!isNullOrEmpty(sessionStorage[divid]))
                {
                    var $page = $(sessionStorage[divid])
                                                    .bindJqmEvents(params)
                                                    .appendTo($parent)
                        .page();
                }

            }
        }
        return $parent;
    };

    $.fn.bindJqmEvents = function(params)
    {
        var $div = $(this);
        var $ret = false;
        if (!isNullOrEmpty($div))
        {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xml: '',
                parentlevel: 0,
                level: 1,
                HideRefreshButton: false,
                HideSearchButton: false,
                persistBindEvent: false,
                onPageShow: function(p) {},
                onSuccess: function() { $.mobile.pageLoading(true); }
            };

            if (params) $.extend(p, params);
            p.level = (p.parentlevel === p.level) ? p.parentlevel + 1 : p.level;

            $div.unbind('pageshow');
            $ret = $div.bind('pageshow', function()
                {
                $.mobile.pageLoading();

                if (p.level === 1) localStorage['currentviewid'] = p.DivId;
                p.onPageShow(p);
                if (!p.persistBindEvent) {
                    // If the page is constructed entirely from cache, we only do this once.
                    $(this).unbind('pageshow');
                }
            });

//            $div.unbind('pagebeforecreate');
//            $div.bind('pagebeforecreate', function()
//            {
//                //$div.find('input[type="radio"]').checkboxradio();
//                //$div.find('input[type="checkbox"]').checkboxradio();
//            });

//            $div.unbind('pagecreate');
//            $div.bind('pagecreate', function()
//            {
//                //$div.find('input[type="radio"]').checkboxradio('refresh',true);
//                //$div.find('input[type="checkbox"]').checkboxradio('refresh',true);
//            });
        }
        return $ret;
    };

    $.fn.CswMobile = function (options) {
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
        
        if (options)
        {
            $.extend(opts, options);
        }
        
        var ForMobile = true;
        var rootid;

        var UserName = localStorage["username"];
        if( !localStorage["sessionid"] ) {
            Logout();
        }
        var SessionId = localStorage["sessionid"];
        var $currentViewXml;

        var storedViews = '';
        if(localStorage.storedViews) storedViews = JSON.parse( localStorage['storedviews'] );  // {name: '', rootid: ''}

        //$body.restorePages(); //not yet. someday.

        var $logindiv = _loadLoginDiv();
        var $viewsdiv = _loadViewsDiv();
        var $syncstatus = _makeSyncStatusDiv();
        var $helpdiv = _makeHelpDiv();
        var $sorrycharliediv = _loadSorryCharlieDiv();

		// case 20355 - error on browser refresh
        // there is a problem if you refresh with #viewsdiv where we'll generate a 404 error, but the app will continue to function
        if ( !isNullOrEmpty(SessionId) ) {
            $.mobile.path.set('#viewsdiv'); // we can use restorePages() to eliminate this later.
        }        

        if ( !isNullOrEmpty(SessionId) )
        {
            $viewsdiv = reloadViews();
            $viewsdiv.page();
            //$viewsdiv.doChangePage(); //JQM will do this for us.
            _waitForData();
        }
        else
        {
            // this will trigger _waitForData(), but we don't want to wait here
            _handleDataCheckTimer(
                function ()
                {
                    // online
                    $logindiv.doPage();
					$logindiv.doChangePage();
                },
                function ()
                {
                    // offline
                    $sorrycharliediv.doPage();
					$sorrycharliediv.doChangePage();
                }
            ); // _handleDataCheckTimer();
        } // if-else (configvar_sessionid != '' && configvar_sessionid != undefined)

        function _loadLoginDiv()
        {
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
                $.mobile.pageLoading();
                onLoginSubmit(); 
            });
            $('#login_customerid').clickOnEnter($('#loginsubmit'));
            $('#login_username').clickOnEnter($('#loginsubmit'));
            $('#login_password').clickOnEnter($('#loginsubmit'));
           
            return $retDiv;
		}

        function _loadViewsDiv()
        {
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

		function _loadSorryCharlieDiv(params)
        {
            var p = {
                DivId: 'sorrycharliediv',
                HeaderText: 'Sorry Charlie!',
                HideHelpButton: false,
                HideCloseButton: true,
                HideOnlineButton: false,
                content: 'You must have internet connectivity to login.'
            };
            if(params) $.extend(p,params);

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

        function removeDiv(DivId)
        {
            $('#' + DivId).find('div:jqmData(role="content")').empty();
        }

        function reloadViews()
        {
            if($viewsdiv) $viewsdiv.find('div:jqmData(role="content")').empty();
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

        function setOffline()
        {
            localStorage['online'] = false;
            var $onlineStatus = $('.onlineStatus');
            if ($onlineStatus.hasClass('online'))
            {
                $onlineStatus.removeClass('online')
                             .addClass('offline')
                             .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                             .text('Offline')
                             .end();
                $('.refresh').css('visibility', 'hidden');

                $viewsdiv = reloadViews(); //no changePage
            }
            if ( $.mobile.activePage === $logindiv)
            {
                $sorrycharliediv.doPage(); // doChangePage();
            }
        }
        function setOnline()
        {
            localStorage['online'] = true;
            var $onlineStatus = $('.onlineStatus');
            if ($onlineStatus.hasClass('offline'))
            {
                $onlineStatus.removeClass('offline')
                             .addClass('online')
                             .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
                             .text('Online')
                             .end();

                $('.refresh').css('visibility', '');
                $viewsdiv =  reloadViews(); //no changePage
            }
            if ( $.mobile.activePage === $sorrycharliediv )
            {
                $logindiv.doPage(); //doChangePage();;
            }
        }
        function amOffline()
        {
            var isOffline = !isTrue( localStorage['online'] );
            return isOffline;
        }

        // ------------------------------------------------------------------------------------
        // Logging Button
        // ------------------------------------------------------------------------------------

        function _toggleLogging()
        {
            var logging = !doLogging();            
            doLogging(logging);
            if(logging) {
                setStartLog();
            } 
            else {
                setStopLog();
            }

        }

        function setStartLog()
        {
            if( doLogging() )
            {
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

        function setStopLog()
        {
            if( !doLogging() )
            {
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
                    success: function ()
                    {
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

        function _loadDivContents(params)
        {
            var logger = new profileMethod('loadDivContents');
            var p = {
                ParentId: '',
                level: 1,
                DivId: '',
                HeaderText: '',
                HideRefreshButton: false,
                HideSearchButton: false,
                $xml: '',
                SessionId: SessionId,
                onSuccess: function() {}
            };
            if (params) $.extend(p, params);
            
            if (p.level === 1)
            {
                rootid = p.DivId;
            }
            var $retDiv = $('#' + p.DivId);
            
            if ( isNullOrEmpty($retDiv) || $retDiv.length === 0 || $retDiv.find('div:jqmData(role="content")').length === 1 )
            {
                if (p.level === 0)
                {
                    if (amOffline())
                    {
                        p.$xml = _fetchCachedRootXml();
                        $retDiv = _loadDivContentsXml(p);
                    } 
                    else
                    {
                        p.url = opts.ViewsListUrl;
                        $retDiv = _getDivXml(p);
                    }
                } 
                else if (p.level === 1)
                {
                    // case 20354 - try cached first
                    var $xmlstr = _fetchCachedViewXml(rootid);
                    if ( !isNullOrEmpty($xmlstr) )
                    {
                        $currentViewXml = $xmlstr;
                        p.$xml = $currentViewXml;
                        $retDiv = _loadDivContentsXml(p);
                    }
                    else if (!amOffline())
                    {
                        p.url = opts.ViewUrl;
                        $retDiv = _getDivXml(p);
                    }
                } 
                else  // Level 2 and up
                {
                    if( !isNullOrEmpty($currentViewXml) )
                    {
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

        function _loadDivContentsXml(params)
        {
            params.parentlevel = params.level;
            var $retDiv = _processViewXml(params);
            return $retDiv;
        }

        function _getDivXml(params)
        {
            var $retDiv = undefined;
            
            var p = {
                url: opts.ViewUrl
            }
            $.extend(p,params);

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
                success: function ($xml)
                {
                    $currentViewXml = $xml;
                    p.$xml = $currentViewXml;
                    if (params.level === 1)
                    {
                        _storeViewXml(p.DivId, p.HeaderText, $currentViewXml);
                    }
                    $retDiv = _loadDivContentsXml(p);    
                },
                error: function(xml)
                {
                }
            });

            return $retDiv;
        }

        var currenttab;
        var onAfterAddDiv;
        function _processViewXml(params)
        {
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
                HideBackButton: false,
                onSuccess: function() {}
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

            onAfterAddDiv = function ($retDiv) { };

            p.$xml.children().each(function ()
            {
                p.$xmlitem = $(this);
                var $li = _makeListItemFromXml($list, p)
                            .CswAttrXml('data-icon', false)                            
                            .appendTo($list);
            });
            $list.listview('refresh')
                 .bindLI();
                 //.find('input[type="radio"]').checkboxradio('refresh',true);
            onAfterAddDiv($retDiv);
            
            p.onSuccess();
            logger.setEnded();
            cacheLogInfo(logger);
            return $retDiv;
        } // _processViewXml()

        function _makeListItemFromXml ($list, params)
        {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xmlitem: '',
                parentlevel: '',
                level: '',
                HideRefreshButton: false,
                HideSearchButton: false
            }
            
            if(params) $.extend(p,params);

            var id = makeSafeId({ID: p.$xmlitem.CswAttrXml('id') });
            var text = p.$xmlitem.CswAttrXml('name');

            var IsDiv = ( !isNullOrEmpty(id) );
            var PageType = tryParseString(p.$xmlitem.get(0).nodeName,'').toLowerCase();
            
            var nextid = p.$xmlitem.next().CswAttrXml('id');
            var previd = p.$xmlitem.prev().CswAttrXml('id');
            
            // add a div for editing the property directly
            var $toolbar = $('<div data-role="controlgroup" data-type="horizontal" class="ui-bar"></div>');
            if ( !isNullOrEmpty(previd) )
            {
                $toolbar.CswLink('init',{href:'javascript:void(0);', value: 'Previous'})
                        .CswAttrXml({'data-identity': previd,
                                     'data-url': previd,
                                     'data-icon': 'arrow-u',
                                     'data-inline': true
                        })
                        .bind('click', function() { 
                            var $prev = $('#' + previd);
                            $prev.doChangePage({transition:'slideup', reverse: true, changeHash: false});
                        });
            }
            if ( !isNullOrEmpty(nextid) )
            {
                $toolbar.CswLink('init',{href:'javascript:void(0);', value: 'Next'})
                        .CswAttrXml({'data-identity': nextid,
                                     'data-url': nextid,
                                     'data-icon': 'arrow-d',
                                     'data-inline': true
                        })
                        .bind('click', function() { 
                            var $next = $('#' + nextid);
                            $next.doChangePage({transition:'slidedown', changeHash: false});
                        });

            }
            
            var $retLI = $('');

            switch (PageType)
            {
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
                        var fieldtype = tryParseString(p.$xmlitem.CswAttrXml('fieldtype'),'');
                        var gestalt = p.$xmlitem.CswAttrXml('gestalt');
                        var ReadOnly = ( isTrue(p.$xmlitem.CswAttrXml('isreadonly')) );
                        if (gestalt === 'NaN') gestalt = '';
                        
                        var currentNo =  p.$xmlitem.prevAll('[fieldtype="'+fieldtype+'"]').andSelf().length;
                        var totalCnt = p.$xmlitem.siblings('[fieldtype="'+fieldtype+'"]').andSelf().length;

                        if (currenttab !== tab)
                        {
//                            if ( !isNullOrEmpty(currenttab) )
//                            {    
//                                lihtml += _endUL() + _makeUL();
//                            }
                            $tab = $('<li data-role="list-divider">' + tab + '</li>')
                                        .appendTo($list);
                            currenttab = tab;
                        }
                        
                        var $lItem = $('<li id="' + id + '_li"></li>')
                                        .CswAttrXml('data-icon', false)
                                        .appendTo($list);
                        var $link = $lItem.CswLink('init',{ID: id + '_href', href:'javascript:void(0)', value: text})
                                          .css('white-space','normal');
                        if( !ReadOnly ) {
                            $link.CswAttrXml({'data-identity': id, 'data-url': id });
                        }
                        var $div;
                        switch (fieldtype.toLowerCase())
                        {
                            case 'logical':
                                var sf_checked = tryParseString( p.$xmlitem.children('checked').text(), '');
                                var sf_required = tryParseString( p.$xmlitem.children('required').text(), '');

                                $div = $('<div class="lisubstitute ui-li ui-btn-up-c"></div>')
                                                .appendTo($list);
                                var $logical = _makeLogicalFieldSet(p.DivId, id, 'ans', 'ans2', sf_checked, sf_required)
                                                .appendTo($div);
                                break;

                            case 'question':
                                var sf_answer = tryParseString( p.$xmlitem.children('answer').text() , '');
                                var sf_allowedanswers = tryParseString( p.$xmlitem.children('allowedanswers').text(), '');
                                var sf_compliantanswers = tryParseString( p.$xmlitem.children('compliantanswers').text(), '');
                                var sf_correctiveaction = tryParseString( p.$xmlitem.children('correctiveaction').text(), '');

                                $div = $('<div class="lisubstitute ui-li ui-btn-up-c"><div>')
                                                .appendTo($list);
                                var $question = _makeQuestionAnswerFieldSet(p.DivId, id, 'ans', 'ans2', 'cor', 'li', 'label', sf_allowedanswers, sf_answer, sf_compliantanswers)
                                                .appendTo($div);

                                if ( !isNullOrEmpty(sf_answer) && (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') < 0 && isNullOrEmpty(sf_correctiveaction) )
                                {
                                    // mark the li div OOC after it is created
                                    var old_onAfterAddDiv = onAfterAddDiv;
                                    onAfterAddDiv = function ($divhtml)
                                    {
                                        $divhtml.find('#' + id + '_li div')
                                                .addClass('OOC');
                                        old_onAfterAddDiv($divhtml);
                                    }
                                }
                                break;

                            default:
                                var $gestalt = $('<div><p>' + gestalt + '</p></div>')
                                                    .appendTo($link);
                                break;
                        }

                        if( fieldtype.toLowerCase() === "question")
                        {
                            var $count = $('<span>' + currentNo + '&nbsp;of&nbsp;' + totalCnt +'</span>')
                                         .addClass('ui-btn-right');
                            $toolbar.append($count);
                        }

                        _addPageDivToBody({
                                            ParentId: p.DivId,
                                            level: p.parentlevel,
                                            DivId: id,
                                            HeaderText: text,
                                            $toolbar: $toolbar,
                                            $content: _FieldTypeXmlToHtml(p.$xmlitem, p.DivId, id + '_href')
                                        })
                                        .addClass('CswNbtNodeProp');
                        break;
                    } // case 'prop':
                default:
                    {
                        $retLI = $('<li></li>');
                        if (IsDiv)
                        {
                            $retLI.CswLink('init',{href: 'javascript:void(0);', value: text})
                                  .css('white-space','normal')
                                  .CswAttrXml({'data-identity': id, 
                                               'data-url': id });
                        }
                        else
                        {
                            $retLI.val(text);
                        }
                        if(p.parentlevel === 0) 
                        {
                            var $newDiv = _preFormNextLevelPages({
                                                ParentId: p.DivId,
                                                parentlevel: p.parentlevel,
                                                level: p.parentlevel+1,
                                                DivId: id,
                                                // Case 22211: IDC content is not cached. We need to reconstruct nodes on each page load.
                                                persistBindEvent: true,
                                                HeaderText: text
                                                //,$toolbar: $toolbar
                            });
                            $newDiv.addClass('CswNbtView')
                                   .doPage( $newDiv.CswAttrXml('data-url') );
                        }
                        break;
                    } // default:
            }
            return $retLI;
        } // _makeListItemFromXml()

        function _makeObjectClassContent(params)
        {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xmlitem: '',
                parentlevel: '',
                HideRefreshButton: false,
                HideSearchButton: false
            };
            if(params) $.extend(p,params);

            var $retHtml;
            var Html = '';
            var id = makeSafeId({ID: p.$xmlitem.CswAttrXml('id') });
            var NodeName = p.$xmlitem.CswAttrXml('name');
            var icon = '';
            if ( !isNullOrEmpty(p.$xmlitem.CswAttrXml('iconfilename')))
            {
				icon = 'images/icons/' + p.$xmlitem.CswAttrXml('iconfilename');
            }
			var ObjectClass = p.$xmlitem.CswAttrXml('objectclass');
            
            switch (ObjectClass)
            {
                case "InspectionDesignClass":
                    var DueDate = p.$xmlitem.find('prop[ocpname="Due Date"]').CswAttrXml('gestalt');
                    var Location = p.$xmlitem.find('prop[ocpname="Location"]').CswAttrXml('gestalt');
                    var MountPoint = p.$xmlitem.find('prop[ocpname="Target"]').CswAttrXml('gestalt');
                    var Status = p.$xmlitem.find('prop[ocpname="Status"]').CswAttrXml('gestalt');
                    var UnansweredCnt = 0;

                    p.$xmlitem.find('prop[fieldtype="Question"]').each(function ()
                    {
                        var $question = $(this);
                        if ( isNullOrEmpty($question.children('Answer').text() ) )
                        {
                            UnansweredCnt++;
                        }
                    });

                    Html += '<li>';
                    if ( !isNullOrEmpty(icon) )
                        Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">';
                    Html += '<h2>' + NodeName + '</h2>';
                    Html += '<p>' + Location + '</p>';
                    Html += '<p>' + MountPoint + '</p>';
                    Html += '<p>';
                    if(!isNullOrEmpty(Status)) Html +=  Status + ', ';
                    Html += 'Due: ' + DueDate + '</p>';
                    Html += '<span id="' + makeSafeId({prefix: id, ID: 'unansweredcnt'}) + '" class="ui-li-count">' + UnansweredCnt + '</span>';
                    Html += '</a>';
                    Html += '</li>';
                    break;

                default:
                    Html += '<li>';
                    if ( !isNullOrEmpty(icon) )
                        Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">' + NodeName + '</a>';
                    Html += '</li>';
                    break;
            }

            var $newDiv = _preFormNextLevelPages({
                                ParentId: p.DivId,
                                parentlevel: p.parentlevel,
                                level: p.parentlevel+1,
                                DivId: id,
                                HeaderText: NodeName
                          })
                          .addClass('CswNbtNode');

            $retHtml = $(Html); //.listview('refresh');
            return $retHtml;
        }

        function _FieldTypeXmlToHtml($xmlitem, ParentId, SiblingId)
        {
            var IdStr = makeSafeId({ID: $xmlitem.CswAttrXml('id') });
            var FieldType = $xmlitem.CswAttrXml('fieldtype');
            var PropName = $xmlitem.CswAttrXml('name');
            var ReadOnly = ( isTrue($xmlitem.CswAttrXml('isreadonly')) );

            // Subfield values
            var sf_text = tryParseString( $xmlitem.children('text').text(), '');
            var sf_value = tryParseString( $xmlitem.children('value').text(), '');
            var sf_href = tryParseString( $xmlitem.children('href').text(), '');
            var sf_checked = tryParseString( $xmlitem.children('checked').text(), '');
            var sf_required = tryParseString( $xmlitem.children('required').text(), '');
            var sf_units = tryParseString( $xmlitem.children('units').text(), '');
            var sf_answer = tryParseString( $xmlitem.children('answer').text(), '');
            var sf_allowedanswers = tryParseString( $xmlitem.children('allowedanswers').text(), '');
            var sf_correctiveaction = tryParseString( $xmlitem.children('correctiveaction').text(), '');
            var sf_comments = tryParseString( $xmlitem.children('comments').text(), '');
            var sf_compliantanswers = tryParseString( $xmlitem.children('compliantanswers').text(), '');
            var sf_options = tryParseString( $xmlitem.children('options').text(), '');
            
            var $retHtml = $('<div data-role="fieldcontain" id="' + IdStr + '_fieldcontain"></div>');
            var $propNameDiv = $('<label for="' + IdStr + '_input" id="' + IdStr + '_label">' + PropName + '</label>')
                                .appendTo($retHtml);

            //var Html = '<div id="' + IdStr + '_propname"';
            if (FieldType === "Question" && 
                !(sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) && 
                isNullOrEmpty(sf_correctiveaction) )
            {
                $propNameDiv.addClass('OOC');
            }
            else {
                $propNameDiv.removeClass('OOC');
            }
            //$retHtml.append('<br/>');

            if( !ReadOnly )
            {
                var addChangeHandler = true;
                var $prop;
                var propId = IdStr + '_input';

                switch (FieldType)
                {
                    case "Date":
                        $prop = $retHtml.CswInput('init', {type: CswInput_Types.text, ID: propId, value: sf_value });
                        break;

                    case "Link":
                        $prop = $retHtml.CswLink('init', {ID: propId, href: sf_href, rel: 'external',value: sf_text});
                        break;

                    case "List":
                        $prop = $('<select class="csw_prop_select" name="' + propId + '" id="' + propId + '"></select>')
                                        .appendTo($retHtml)
                                        .selectmenu();
                        var selectedvalue = sf_value;
                        var optionsstr = sf_options;
                        var options = optionsstr.split(',');
                        for (var i = 0; i < options.length; i++)
                        {
                            var $option = $('<option value="' + options[i] + '"></option>')
                                            .appendTo($prop);
                            if (selectedvalue === options[i])
                            {
                                $option.CswAttrDom('selected','selected');
                            }
                        
                            if( !isNullOrEmpty(options[i]) )
                            {
                                $option.val( options[i] );
                            }
                            else
                            {
                                $option.valueOf('[blank]');
                            }
                        }
                        $prop.selectmenu('refresh');
                        break;

                    case "Logical":
                        addChangeHandler = false; //_makeLogicalFieldSet() does this for us
                        $prop = _makeLogicalFieldSet(ParentId, IdStr, 'ans2', 'ans', sf_checked, sf_required)
                                            .appendTo($retHtml);
                        break;

                    case "Memo":
                        $prop = $('<textarea name="' + propId + '">' + sf_text + '</textarea>')
                                            .appendTo($retHtml);
                        break;

                    case "Number":
                        sf_value = tryParseNumber(sf_value, '');
                        $prop = $retHtml.CswInput('init', {type: CswInput_Types.number, ID: propId, value: sf_value});
                        
                        // if (Prop.MinValue != Int32.MinValue)
                        //     Html += "min = \"" + Prop.MinValue + "\"";
                        // if (Prop.MaxValue != Int32.MinValue)
                        //     Html += "max = \"" + Prop.MaxValue + "\"";
                        break;

                    case "Password":
                        //nada
                        break;

                    case "Quantity":
                        $prop = $retHtml.CswInput('init', {type: CswInput_Types.text, ID: propId, value: sf_value})
                                        .append( sf_units );
                        // Html += "<select name=\"" + IdStr + "_units\">";
                        // string SelectedUnit = PropWrapper.AsQuantity.Units;
                        // foreach( CswNbtNode UnitNode in PropWrapper.AsQuantity.UnitNodes )
                        // {
                        //     string ThisUnitText = UnitNode.Properties[CswNbtObjClassUnitOfMeasure.NamePropertyName].AsText.Text;
                        //     Html += "<option value=\"" + UnitNode.Properties[CswNbtObjClassUnitOfMeasure.NamePropertyName].AsText.Text + "\"";
                        //     if( ThisUnitText == SelectedUnit )
                        //         Html += " selected";
                        //     Html += ">" + ThisUnitText + "</option>";
                        // }
                        // Html += "</select>";

                        break;

                    case "Question":
                        addChangeHandler = false; //_makeQuestionAnswerFieldSet() does this for us
                        $prop = _makeQuestionAnswerFieldSet(ParentId, IdStr, 'ans2', 'ans', 'cor', 'li', 'label', sf_allowedanswers, sf_answer, sf_compliantanswers)
                                            .appendTo($retHtml);

                        var $corAction = $('<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action">' + sf_correctiveaction + '</textarea>')
                                            .appendTo($prop);
                     
                        if (sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0)
                        {
                            $corAction.css('display','none');
                        }
                        $corAction.bind('change', function()
                        {
                            var $cor = $(this);
                            if($cor.val() === '') 
                            { 
                                  $('#' + IdStr + '_li div').addClass('OOC'); 
                                  $('#' + IdStr + '_label').addClass('OOC');
                            } 
                            else 
                            {
                                  $('#' + IdStr + '_li div').removeClass('OOC'); 
                                  $('#' + IdStr + '_label').removeClass('OOC'); 
                            }
                        });

                        var $comments = $('<textarea name="' + propId + '" id="' + propId + '" placeholder="Comments">' + sf_comments + '</textarea>')
                                            .appendTo($prop);
                        break;

                    case "Static":
                        $retHtml.append( $('<p id="' + propId + '">' + sf_text + '</p>') );
                        break;

                    case "Text":
                        $prop = $retHtml.CswInput('init', {type: CswInput_Types.text, ID: propId, value: sf_text});
                        break;

                    case "Time":
                        $prop = $retHtml.CswInput('init', {type: CswInput_Types.text, ID: propId, value: sf_value})
                        break;

                    default:
                        $retHtml.append( $('<p id="' + propId + '">' + $xmlitem.CswAttrXml('gestalt') + '</p>') );
                        break;
                } // switch (FieldType)

                if( addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0 )
                {
                    $prop.bind('change', function(eventObj) {
                        var $this = $(this);
                        var $sibling = $('#' + SiblingId);
                        if( !isNullOrEmpty($sibling) && $sibling.length !== 0 )
                        {
                            $sibling.children('div').children('p').text( $this.val() );
                        }
                        onPropertyChange(ParentId,eventObj);
                    });
                }
            }
            else 
            {
                $retHtml.append( $('<p id="' + propId + '">' + $xmlitem.CswAttrXml('gestalt') + '</p>') );
            }
            return $retHtml;
        }

        function _FieldTypeHtmlToXml($xmlitem, id, value)
        {
            var name = new CswString(id);
            var IdStr = makeSafeId({ID: $xmlitem.CswAttrXml('id') });
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
            switch (fieldtype)
            {
                case "Date": if (name.contains( IdStr )) $sftomodify = $sf_value; break;
                case "Link": break;
                case "List": if (name.contains( IdStr )) $sftomodify = $sf_value; break;
                case "Logical":
                    if (name.contains( makeSafeId({ID: IdStr, suffix: 'ans'}) ) )
                    {
                        $sftomodify = $sf_checked;
                    }
                    break;
                case "Memo": if (name.contains( IdStr )) $sftomodify = $sf_text; break;
                case "Number": if (name.contains( IdStr )) $sftomodify = $sf_value; break;
                case "Password": break;
                case "Quantity": if (name.contains( IdStr )) $sftomodify = $sf_value; break;
                case "Question":
                    if (name.contains( makeSafeId({ID: IdStr, suffix: 'input'}) ))
                    {
                        $sftomodify = $sf_comments;
                    }
                    else if (name.contains( makeSafeId({ID: IdStr, suffix: 'ans'}) ) )
                    {
                        $sftomodify = $sf_answer;
                    }
                    else if (name.contains( makeSafeId({ID: IdStr, suffix: 'cor'}) ) )
                    {
                        $sftomodify = $sf_correctiveaction;
                    }
                    break;
                case "Static": break;
                case "Text": if (name.contains( IdStr )) $sftomodify = $sf_text; break;
                case "Time": if (name.contains( IdStr )) $sftomodify = $sf_value; break;
                default: break;
            }
            if ( !isNullOrEmpty($sftomodify) )
            {
                $sftomodify.text(value);
                $xmlitem.CswAttrXml('wasmodified', '1');
            }
        } // _FieldTypeHtmlToXml()

        function _makeLogicalFieldSet(ParentId, IdStr, Suffix, OtherSuffix, Checked, Required)
        {
            var $retHtml = $('<div class="csw_fieldset ui-field-contain ui-body ui-br" data-role="fieldcontain"></div>');
            var $fieldset = $('<fieldset></fieldset>')
                                .appendTo($retHtml)
                                .CswAttrDom({
                                    'class': 'csw_fieldset',
                                    'id': IdStr + '_fieldset'})
                                .CswAttrXml({
                                    'data-role': 'controlgroup',
                                    'data-type': 'horizontal'                                     
                                 })
                                 .addClass('csw_fieldset toolbar ui-corner-all ui-controlgroup ui-controlgroup-horizontal');
            var answers = ['Null', 'True', 'False'];
            if ( isTrue(Required) )
            {
                answers = ['True', 'False'];
            }
            var inputName = makeSafeId({ prefix: IdStr, ID: Suffix}); //Name needs to be non-unique and shared

            for (var i = 0; i < answers.length; i++)
            {
                var answertext;
                switch (answers[i])
                {
                    case 'Null': answertext = '?'; break;
                    case 'True': answertext = 'Yes'; break;
                    case 'False': answertext = 'No'; break;
                }
                
                var inputId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i]});
                var $input = $fieldset.CswInput('init',{type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i]});
                                
                var $label = $('<label for="' + inputId + '">' + answertext + '</label>')
                                .appendTo($fieldset);

                // Checked is a Tristate, so isTrue() is not useful here
				if ((Checked === 'false' && answers[i] === 'False') ||
					(Checked === 'true' && answers[i] === 'True') ||
					(Checked === '' && answers[i] === 'Null'))
				{
                    $input.CswAttrDom('checked','checked');
                }
                $input.data('thisI',i);
                $input.bind('change', function (eventObj)
                {
                    var i = $(this).data('thisI');
                    for (var k = 0; k < answers.length; k++)
                    {
                        var input1Id = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[k]});
                        var $input1 = $('#' + input1Id);

                        var input2Id = makeSafeId({ prefix: IdStr, ID: OtherSuffix, suffix: answers[k]});
                        var $input2 = $('#' + input2Id);
            
                        if (answers[k] === answers[i])
                        {
                            $input1.CswAttrDom('checked', 'checked');
                            $input2.CswAttrDom('checked', 'checked');
                        }
                        else
                        {
                            $input1.removeAttr('checked');
                            $input2.removeAttr('checked');
                        }

                        $input1.checkboxradio('refresh');
                        $input2.checkboxradio('refresh');

                    } // for (var k = 0; k < answers.length; k++)
                    onPropertyChange(ParentId,eventObj);
                });
            } // for (var i = 0; i < answers.length; i++)
            $retHtml.find('input[type="radio"]').checkboxradio();
            return $retHtml;
        } // _makeLogicalFieldSet()

        function _makeQuestionAnswerFieldSet(ParentId, IdStr, Suffix, OtherSuffix, CorrectiveActionSuffix, LiSuffix, PropNameSuffix, Options, Answer, CompliantAnswers)
        {
            var $retHtml = $('<div class="csw_fieldset ui-field-contain ui-body ui-br" data-role="fieldcontain"></div>');
            var $fieldset = $('<fieldset></fieldset>')
                                .appendTo($retHtml)
                                .CswAttrDom({
                                    'class': 'csw_fieldset',
                                    'id': IdStr + '_fieldset'})
                                .CswAttrXml({
                                    'data-role': 'controlgroup',
                                    'data-type': 'horizontal'
								})
								.addClass('csw_fieldset toolbar ui-corner-all ui-controlgroup ui-controlgroup-horizontal');
            var answers = Options.split(',');
            var answerName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unqiue and shared

            for (var i = 0; i < answers.length; i++)
            {
				var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });
                var $answer = $fieldset.CswInput('init',{type: CswInput_Types.radio, name: answerName, ID: answerid, value: answers[i] });
				var $label = $('<label for="' + answerid + '">' + answers[i] + '</label>')
								.appendTo($fieldset);
                if (Answer === answers[i])
                {
                    $answer.CswAttrDom('checked','checked');
                } 
                $answer.data('thisI',i);

				$answer.bind('change', function(eventObj) 
				{
                    var thisI = $(this).data('thisI');

					for (var k = 0; k < answers.length; k++)
					{
                        var answer1Id = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[k] });
                        var $answer1 = $('#' + answer1Id);

                        var answer2Id = makeSafeId({ prefix: IdStr, ID: OtherSuffix, suffix: answers[k]});
						var $answer2 = $('#' + answer2Id);

                        if (answers[k] === answers[thisI])
						{
                            $answer1.CswAttrDom('checked', 'checked');
                            $answer2.CswAttrDom('checked', 'checked');
                            
						} 
                        else
						{
                            $answer1.removeAttr('checked');
                            $answer2.removeAttr('checked');
						}
                        $answer2.checkboxradio('refresh');
                        $answer1.checkboxradio('refresh');
                        
					} // for (var k = 0; k < answers.length; k++)
					
					var correctiveActionId = makeSafeId({ prefix: IdStr, ID: CorrectiveActionSuffix});
					var liSuffixId = makeSafeId({ prefix: IdStr, ID: LiSuffix});
					var propNameSuffixId = makeSafeId({ prefix: IdStr, ID: PropNameSuffix});
					
					var $cor = $('#' + correctiveActionId);
					var $li = $('#' + liSuffixId);
					var $prop = $('#' + propNameSuffixId);

					if ((',' + CompliantAnswers + ',').indexOf(',' + answers[thisI] + ',') >= 0)
					{
						$cor.css('display','none');
						$li.children('div').removeClass('OOC').children('div').removeClass('OOC');
						$prop.removeClass('OOC');
					}
					else
					{
						$cor.css('display','');

						if( isNullOrEmpty($cor.val()) )
						{
                            $li.children('div').addClass('OOC');
							$prop.addClass('OOC');
						}
						else
						{
							$li.children('div').removeClass('OOC');
							$prop.removeClass('OOC');
						}
					}
					if ( !isNullOrEmpty(Answer) )
					{
						// update unanswered count when this question is answered
						var $fieldset = $('#' + IdStr + '_fieldset');
						if( $fieldset.CswAttrDom('answered') )
						{
							var $cntspan = $('#' + ParentId + '_unansweredcnt');
							$cntspan.text(parseInt($cntspan.text()) - 1);
							$fieldset.CswAttrDom('answered','true');
						}
					}
                    onPropertyChange(ParentId,eventObj);
				}); //click()
            } // for (var i = 0; i < answers.length; i++)
            $retHtml.find('input[type="radio"]').checkboxradio();
            return $retHtml;
        } // _makeQuestionAnswerFieldSet()

        function _preFormNextLevelPages(params)
        {
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
            if(params) $.extend(p,params);

            $retDiv = _addPageDivToBody(p);
            $retDiv.bindJqmEvents(p);
            return $retDiv;
        }
                
        function _addPageDivToBody(params)
        {
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

            if (params)
            {
                $.extend(p, params);
            }

            p.DivId = makeSafeId({ID: p.DivId});

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

            if( isNullOrEmpty($pageDiv) || $pageDiv.length === 0 )
            {
                $pageDiv = $body.CswDiv('init',{ID: p.DivId})
                                .CswAttrXml({'data-role': 'page', 
                                             'data-url': p.DivId, 
                                             'data-title': p.HeaderText,
                                             'data-rel': p.dataRel
                                 }); 

			    var $header = $pageDiv.CswDiv('init',{ID: p.DivId + '_header'})
                                      .CswAttrXml({'data-role': 'header',
                                                   'data-theme': opts.Theme, 
                                                   'data-position':'fixed',
                                                   'data-id': 'csw_header'});
                $backlink = $header.CswLink('init',{'href': 'javascript:void(0)', 
                                                        ID: p.DivId + '_back',
                                                        cssclass: 'ui-btn-left',
                                                        value: 'Back'})
                                        .CswAttrXml({'data-identity': p.DivId + '_back', 
                                                     //'data-url': p.DivId + '_back',
													 'data-rel': 'back',
                                                     'data-direction': 'reverse' });

                $closeBtn = $header.CswLink('init',{href:'javascript:void(0)',
                                                    ID: p.DivId + '_close',
                                                    cssclass: 'ui-btn-left'})
                                       .CswAttrXml({
                                            'data-identity': p.DivId + '_close',
                                            'data-icon': 'delete',
                                            'data-rel': 'back',
                                            'data-iconpos': 'notext',
                                            'data-direction': 'reverse',
                                            'title': 'Close'
                                       });
            
                if ( !isNullOrEmpty(p.backtransition) )
                {
                    $backlink.CswAttrXml('data-transition', p.backtransition);
                }
//                if ( isNullOrEmpty(p.ParentId) )
//                {
//                    $backlink.css('visibility','hidden');
//                }
            
                if ( !isNullOrEmpty(p.backicon) )
                {
                    $backlink.CswAttrXml('data-icon',p.backicon);
                }
                else
                {
                    $backlink.CswAttrXml('data-icon','arrow-l');
                }

                $header.append($('<h1>' + p.HeaderText + '</h1>'));

                $searchBtn = $header.CswLink('init',{'href': 'javascript:void(0)', 
                                            ID: p.DivId + '_searchopen',
                                            cssclass: 'ui-btn-right',
                                            value: 'Search' })
                                      .CswAttrXml({'data-identity': p.DivId + '_searchopen', 
                                                   'data-url': p.DivId + '_searchopen', 
                                                   'data-transition': 'pop',
                                                   'data-rel': 'dialog' });

                $headerOnlineBtn = $header.CswLink('init',{ ID: p.DivId + '_headeronline',
                                                            cssclass: 'ui-btn-right onlineStatus online',
                                                            value: 'Online' })
                                          .CswAttrDom({'disabled':'disabled'})
                                          .hide();

                $header.CswDiv('init',{cssclass: 'toolbar'})
                       .append(p.$toolbar)
                       .CswAttrXml({'data-role':'header','data-type':'horizontal', 'data-theme': opts.Theme});

                var $content = $pageDiv.CswDiv('init',{ID: p.DivId + '_content'})
                                       .CswAttrXml({'data-role':'content','data-theme': opts.Theme})
                                       .append(p.$content);
                var $footer = $pageDiv.CswDiv('init',{ID: p.DivId + '_footer', cssclass: 'ui-bar'})
                                      .CswAttrXml({'data-role':'footer', 
                                                   'data-theme': opts.Theme, 
                                                   'data-position':'fixed',
                                                   'data-id': 'csw_footer' });

                var $footerCtn = $('<div data-role="navbar">')
                                    .appendTo($footer);
                var onlineClass = (amOffline()) ? 'onlineStatus offline' : 'onlineStatus online';
                var onlineValue = (amOffline()) ? 'Offline' : 'Online';

                $syncstatusBtn = $footerCtn.CswLink('init',{'href': 'javascript:void(0)', 
                                                    ID: p.DivId + '_gosyncstatus', 
                                                    cssclass: onlineClass, // + ' ui-btn-left',  
                                                    value: onlineValue })
                                    .CswAttrXml({'data-identity': p.DivId + '_gosyncstatus', 
                                                'data-url': p.DivId + '_gosyncstatus', 
                                                'data-transition': 'pop',
                                                'data-rel': 'dialog' 
                                                })
                                                .css('display','');

                $refreshBtn = $footerCtn.CswLink('init',{'href': 'javascript:void(0)', 
                                                       ID: p.DivId + '_refresh', 
                                                       value:'Refresh', 
                                                       cssclass: 'refresh'}) //, ui-btn-left'})
                                      .CswAttrXml({'data-identity': p.DivId + '_refresh', 
                                                   'data-url': p.DivId + '_refresh'
                                                   })
                                                   .css('display','');

                $logoutBtn = $footerCtn.CswLink('init',{'href': 'javascript:void(0)', 
                                                    ID: p.DivId + '_logout', 
                                                    value: 'Logout' })
                                                    //, cssclass: 'ui-btn-left' })
                                   .CswAttrXml({'data-identity': p.DivId + '_logout', 
                                                'data-url': p.DivId + '_logout', 
                                                'data-transition': 'flip' })
                                                .css('display','');
                
            
                var $mainBtn = $footerCtn.CswLink('init',{href: 'NewMain.html', rel: 'external', ID: p.DivId + '_newmain', value: 'Full Site'})
                                      .CswAttrXml({'data-transition':'pop'});


                $helpBtn = $footerCtn.CswLink('init',{'href': 'javascript:void(0)', 
                                                   ID: p.DivId + '_help', 
                                                   value: 'Help' })
                                                   //, cssclass: 'ui-btn-left' })
                                     .CswAttrXml({'data-identity': p.DivId + '_help', 
                                                'data-url': p.DivId + '_help', 
                                                'data-transition': 'pop',
                                                'data-rel': 'dialog'})
                                     .css('display','');
                
                $loggingBtn = $footerCtn.CswLink('init',{'href': 'javascript:void(0)', 
                                                   ID: p.DivId + '_debuglog', 
                                                   value: doLogging() ?  'Start Log' : 'Sync Log',
                                                   cssclass: 'debug' })
                                         .addClass( doLogging() ? 'debug-on' : 'debug-off');
            }

            if ( p.HideOnlineButton ) { 
                $syncstatusBtn.css('display','none'); 
            }
            else {
                $syncstatusBtn.css('display',''); 
            }
            if ( p.HideHelpButton ) {
                $helpBtn.css('display','none');
            }
            else {
                $helpBtn.css('display',''); 
            }
            if ( p.HideLogoutButton ) {
                $logoutBtn.css('display','none');
            }
            else {
                $logoutBtn.css('display',''); 
            }
            if ( p.HideRefreshButton ) {
                $refreshBtn.css('display','none');
            }
            else {
                $refreshBtn.css('display',''); 
            }
            if ( p.HideSearchButton ) {
                $searchBtn.css('display','none');
            }
            else {
                $searchBtn.css('display',''); 
            }
            if( p.HideHeaderOnlineButton ) {
                $headerOnlineBtn.css('display','none');
            }
            else {
                $headerOnlineBtn.show()
                                .css('display','');
            }
            if ( p.dataRel === 'dialog' && !p.HideCloseButton ) {
                $closeBtn.css('display',''); 
            }
            else {
                $closeBtn.css('display','none');
            }
            if( !p.HideBackButton ) {
                $backlink.css('display',''); 
            } 
            else {
                $backlink.css('display','none');
            }
            if( debugOn() ) {
                $loggingBtn.css({'display':''});
            }
            else {
                $loggingBtn.css({'display':'none'});
            }
            
            _bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv);
            
            //$pageDiv.cachePage(); //not yet
            
            return $pageDiv;

        } // _addPageDivToBody()

        function _getDivHeaderText(DivId)
        {
            return $('#' + DivId).find('div:jqmData(role="header") h1').text();
        }

        function _addToDivHeaderText($div,text)
        {
            $div.find('div:jqmData(role="header") h1').append( $('<p style="color: yellow;">' + text + '</p>'));
            $.mobile.loadPage($div);
            return $div;
        }

        function _getDivParentId(DivId)
        {
            var ret = '';
            var $back = $('#' + DivId).find('div:jqmData(role="header") #' + DivId + '_back');
            if ($back.length > 0)
                ret = $back.CswAttrXml('data-identity');
            return ret;
        }

        function _bindPageEvents(DivId, ParentId, level, $div)
        {
            $div.find('#' + DivId + '_searchopen')
                .unbind('tap')
                .bind('tap',function () { 
						onSearchOpen(DivId); 
						return false;
					})
                .end()
                .find('#' + DivId + '_gosyncstatus')
                .unbind('tap')
                .bind('tap', function () { 
						onSyncStatusOpen(DivId); 
						return false;
					})
                .end()
                .find('#' + DivId + '_refresh')
                .unbind('tap')
                .bind('tap', function () { 
						onRefresh($(this).CswAttrDom('id'));
						return false;
					})
                .end()
                .find('#' + DivId + '_logout')
                .unbind('tap')
                .bind('tap', function (e) { 
						onLogout(DivId, e); 
						return false;
					})
                .end()
                .find('#' + DivId + '_help')
                .unbind('tap')
                .bind('tap', function () { 
						onHelp(DivId, ParentId); 
						return false;
					})
                .end()
                .find('#' + DivId + '_debuglog')
                .die('tap')
                .live('tap', function () { 
						_toggleLogging();
						return false;
					})
                .end()
                .find('textarea')
                .unbind('change')
                .bind('change', function (eventObj) { 
						onPropertyChange(DivId, eventObj); 
					})
                .end()
                .find('.csw_prop_select')
                .unbind('change')
                .bind('change', function (eventObj) { 
						onPropertyChange(DivId, eventObj); 
					})
                .end();
        }

        // ------------------------------------------------------------------------------------
        // Sync Status Div
        // ------------------------------------------------------------------------------------

        function _makeSyncStatusDiv()
        {
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
                    .bind('tap', function () { 
                            _processChanges(false); 
                            return false;
                        }) 
                    .end()
                    .find('#ss_gooffline')
                    .bind('tap', function () { 
							_toggleOffline(); 
                            return false;
                        });

            return $retDiv;
        }

        function _toggleOffline()
        {
            //eventObj.preventDefault();
            if (amOffline())
            {
                _clearWaitForData();
                _waitForData();
                setOnline();
                $('#ss_gooffline span').text('Go Offline');
            } else
            {
                _clearWaitForData();
                setOffline();
                $('#ss_gooffline span').text('Go Online');
            }
        }

        function _resetPendingChanges(val, setlastsyncnow)
        {
            if (val)
            {
                $('#ss_pendingchangecnt').text('Yes');
                $('.onlineStatus').addClass('pendingchanges');
            }
            else
            {
                $('#ss_pendingchangecnt').text('No');
                $('.onlineStatus').removeClass('pendingchanges');
            }
            if (setlastsyncnow)
            {
                var d = new Date();
                $('#ss_lastsync').text(d.toLocaleDateString() + ' ' + d.toLocaleTimeString());
            }
        }

        // returns true if no pending changes or user is willing to lose them
        function _checkNoPendingChanges()
        {
            return ( !_pendingChanges() ||
                     confirm('You have pending unsaved changes.  These changes will be lost.  Continue?'));
        }

        function _pendingChanges()
        {
            return ($('#ss_pendingchangecnt').text() === 'Yes');
        }

        // ------------------------------------------------------------------------------------
        // Help Div
        // ------------------------------------------------------------------------------------
        
        function _makeHelpDiv()
        {
            var $help = $('<p>Help</p>')
                            .append('</br></br></br>');
            var $logLevelDiv = $help.CswDiv('init')
                                    .CswAttrXml({'data-role':'fieldcontain'});
            var $logLevelLabel = $('<label for="mobile_log_level">Logging</label>')
                                    .appendTo($logLevelDiv);

            var $logLevelSelect = $logLevelDiv.CswSelect('init',{ID: 'mobile_log_level',
                                                                 selected: debugOn() ? 'on' : 'off',
                                                                 values: [{value: 'off', display: 'Logging Disabled'},
                                                                          {value: 'on', display: 'Logging Enabled'}],
                                                                 onChange: function ($select) {
                                                                     if( $select.val() === 'on' ) {
                                                                        debugOn(true);
                                                                        $('.debug').css('display','').show();
                                                                    }
                                                                    else {
                                                                        debugOn(false);
                                                                        $('.debug').css('diplay', 'none').hide();
                                                                    }
                                                                 }
                                                })
                                                .CswAttrXml({'data-role': 'slider'});

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

        function onLoginSubmit()
        {
            if (!amOffline())
            {
                var UserName = $('#login_username').val();
                var AccessId = $('#login_customerid').val();
                
                var ajaxData = {
                    'AccessId': AccessId, //We're displaying "Customer ID" but processing "AccessID"
                    'UserName': UserName, 
                    'Password': $('#login_password').val()
                };

                //clearPath();
                CswAjaxJSON({
                    formobile: ForMobile,
                    async: false,
                    url: opts.AuthenticateUrl,
                    data: ajaxData,
                    onloginfail: function (text) { 
                        onLoginFail(text);
                    },
                    success: function (data)
                    {
                        SessionId = $.CswCookie('get', CswCookieName.SessionId);
						_cacheSession(SessionId, UserName, AccessId);
                        $viewsdiv = reloadViews();
                        $viewsdiv.doChangePage();
                        //restorePath();
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown)
                    {
                        $.mobile.pageLoading(true);
                    }
                });
            }

        } //onLoginSubmit() 

        function onLoginFail(text)
        {
            Logout(false); 
            $.mobile.pageLoading(true);
            _addToDivHeaderText( $logindiv, text);
        }

        function onLogout()
        {
            Logout(true);
        }

        function Logout(reloadWindow)
        {
            if (_checkNoPendingChanges())
            {
                _clearStorage();
                // reloading browser window is the easiest way to reset
                if( reloadWindow ) {
                    window.location.href = window.location.pathname;
                }
            }
        }

        function _clearStorage()
        {
            sessionStorage.clear();
            localStorage.clear();
        }

        function onRefresh(refreshBtnId)
        {
            if (!amOffline())
            {
                if (_checkNoPendingChanges())
                {
                    $.mobile.pageLoading();
                    continueRefresh(refreshBtnId); 
                }
            }
            return false;
        }

        function continueRefresh(refreshBtnId)
        {
            var DivId = localStorage['currentviewid'];
            if( !isNullOrEmpty(DivId) )
            {
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
                    success: function ($xml)
                    {
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
                        $.mobile.loadPage(DivId);

                        $.mobile.pageLoading(true);
                    }, // success
                    error: function(txt) {
                    }
                });
            }
        }

        function onSyncStatusOpen(DivId)
        {
            $syncstatus.doChangePage({transition:'slideup'});
        }

        function onHelp(DivId)
        {
            $help = _makeHelpDiv();
            $help.doChangePage({transition:'slideup'});
        }

        function onPropertyChange(DivId, eventObj)
        {
            var logger = new profileMethod('onPropertyChange');
            var $elm = $(eventObj.target);
            var name = $elm.CswAttrDom('name');
            var value = $elm.val();
            // update the xml and store it
            if( !isNullOrEmpty($currentViewXml) )
            {
                var $divxml = $currentViewXml.find('#' + DivId);
                $divxml.andSelf().find('prop').each(function ()
                {
                    var $fieldtype = $(this);
                    _FieldTypeHtmlToXml($fieldtype, name, value);
                });

                _updateStoredViewXml(rootid, $currentViewXml, '1');
                _resetPendingChanges(true, false);
            }
            logger.setEnded();
            cacheLogInfo(logger);
        } // onPropertyChange()

        function onSearchOpen(DivId)
        {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            var $xmlstr = _fetchCachedViewXml(rootid);
            if ( !isNullOrEmpty($xmlstr) )
            {
                var $wrapper = $('<div></div>');
                var $fieldCtn = $('<div data-role="fieldcontain"></div>')
                                    .appendTo($wrapper);
                var $select =  $('<select id="' + DivId + '_searchprop" name="' + DivId + '_searchprop" class="csw_prop_select">')
                                    .appendTo($fieldCtn)
                                    .CswAttrXml({'data-native-menu': 'false'});

                $xmlstr.children('search').each(function ()
                {
                    var $search = $(this);
                    var $option = $('<option value="' + $search.CswAttrXml('id') + '">' + $search.CswAttrXml('name') + '</option>')
                                    .appendTo($select);
                });

                var $searchCtn = $('<div data-role="fieldcontain"></div>')
                                    .appendTo($wrapper);
                var $searchBox = $searchCtn.CswInput('init',{type: CswInput_Types.search, ID: DivId + '_searchfor'})
                                            .CswAttrXml({'placeholder':'Search',
                                                'data-placeholder': 'Search'
                                            });
                var $goBtn = $wrapper.CswLink('init',{type:'button', ID: DivId + '_searchgo', value:'Go', href: 'javascript:void(0)'})
                                        .CswAttrXml({'data-inline': 'true'})
                                        .bind('click', function () { 
                                            onSearchSubmit(DivId); 
                                        });
                var $results = $wrapper.CswDiv('init',{ID: DivId + '_searchresults'});

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
                $searchDiv.doChangePage({transition:'slideup', changeHash: false});
            }
        }

        function onSearchSubmit(DivId)
        {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            var $resultsDiv = $('#' + DivId + '_searchresults')
                                    .empty();
            var $xmlstr = _fetchCachedViewXml(rootid);
            if ( !isNullOrEmpty($xmlstr) )
            {
                var $content = $resultsDiv.makeUL(DivId + '_searchresultslist', {'data-filter': false})
                                            .append( $('<li data-role="list divider">Results</li>') );
                    
                var hitcount = 0;
                $xmlstr.find('node').each(function ()
                {
                    var $node = $(this);
                    if ( !isNullOrEmpty($node.CswAttrXml(searchprop)) )
                    {
                        if ($node.CswAttrXml(searchprop).toLowerCase().indexOf(searchfor.toLowerCase()) >= 0)
                        {
                            hitcount++;
                            $content.append( _makeListItemFromXml($content, {ParentId: DivId + '_searchresults',
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
                if (hitcount === 0)
                {
                    $content.append( $('<li>No Results</li>'));
                }
                $content.listview('refresh');
            }
        } // onSearchSubmit()

        // ------------------------------------------------------------------------------------
        // Persistance functions
        // ------------------------------------------------------------------------------------
        function _cacheSession(sessionid, username, customerid)
        {
            localStorage['online'] = true;
            localStorage['username'] = username;
            localStorage['customerid'] = customerid;
            localStorage['sessionid'] = sessionid;
        } //_cacheSession()

        function _storeViewXml(rootid, rootname, $viewxml)
        {
            if( isNullOrEmpty(storedViews) ) {
                storedViews = [{rootid: rootid, name: rootname}];
            }
            else if( storedViews.indexOf(rootid) === -1 )  {
                storedViews.push({rootid: rootid, name: rootname});
            }
            localStorage["storedviews"] = JSON.stringify( storedViews );
            localStorage[rootid] = JSON.stringify( {name: rootname, xml: xmlToString( $viewxml ), wasmodified: false } );
        }

        function _updateStoredViewXml(rootid, $viewxml, wasmodified)
        {
            if( !isNullOrEmpty(localStorage[rootid]) )
            {
                var view = JSON.parse( localStorage[rootid] );
                var update = { xml: xmlToString( $viewxml ), wasmodified: wasmodified };
                if( view ) $.extend(view,update);
                localStorage[rootid] = JSON.stringify( view );   
            }
        }

        function _getModifiedView(onSuccess)
        {
            var modified = false;
            if( !isNullOrEmpty(localStorage['storedviews']) )
            {
                var storedViews = JSON.parse( localStorage['storedviews'] );   
         
                for( var i=0; i < storedViews.length; i++ )
                {
                    stored = storedViews[i];
                    if( !isNullOrEmpty(localStorage[stored.rootid]) )
                    {
                        var view = JSON.parse( localStorage[stored.rootid] );
                        if( view.wasmodified )
                        {
                            modified = true;
                            var rootid = stored.rootid;
                            var viewxml = view.xml;
                            if( !isNullOrEmpty(rootid) && !isNullOrEmpty(viewxml) )
                            {
                                onSuccess(rootid, viewxml);
                            }
                        }
                    }
                }
                if( !modified ) {
                    _resetPendingChanges(false, true);
                    onSuccess();
                }
            }
        }

        function _fetchCachedViewXml(rootid)
        {
            var $view;
            if( !isNullOrEmpty(localStorage[rootid]) ) {
                //View is JSON: {name: '', xml: '', wasmodified: ''}
                var rootObj = JSON.parse( localStorage[rootid] );
                var rootXml = rootObj.xml;
                $view = $( rootXml );
            }
            return $view;
        }

        function _fetchCachedRootXml()
        {
            var ret = '';
            for( var view in storedViews )
            {
                ret += "<view id=\"" + view.rootid + "\" name=\"" + view.name + "\" />";
            }
            return $(ret);
        }
        
        // ------------------------------------------------------------------------------------
        // Synchronization
        // ------------------------------------------------------------------------------------

        var _waitForData_TimeoutId;
        function _waitForData()
        {
            _waitForData_TimeoutId = setTimeout(_handleDataCheckTimer, opts.PollingInterval);
        }
        function _clearWaitForData()
        {
            clearTimeout(_waitForData_TimeoutId);
        }

        function _handleDataCheckTimer(onSuccess, onFailure)
        {
            var url = opts.ConnectTestUrl;
            if (opts.RandomConnectionFailure)
            {
                url = opts.ConnectTestRandomFailUrl;
            }
            //clearPath();
            CswAjaxXml({
                formobile: ForMobile,
                url: url,
                data: {},
                stringify: false,
                onloginfail: function(text) { onLoginFail(text); },
                success: function ($xml)
                {
                    setOnline();
                    _processChanges(true);
                    if ( !isNullOrEmpty(onSuccess) )
                    {
                        onSuccess($xml);
                    }
                    //restorePath();
                },
                error: function (xml)
                {
                    var $xml = $(xml);
                    if ( !isNullOrEmpty(onFailure) )
                    {
                        onFailure($xml);
                    }
                    _waitForData();
                    //restorePath();
                }
            });
        } //_handleDataCheckTimer()

        function _processChanges(perpetuateTimer)
        {
            var logger = new profileMethod('processChanges');
            if ( !isNullOrEmpty(SessionId) )
            {
                _getModifiedView(function (rootid, viewxml)
                {
                    if ( !isNullOrEmpty(rootid) && !isNullOrEmpty(viewxml) )
                    {
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
                            onloginfail: function(text) 
                            { 
                                if (perpetuateTimer)
                                {
                                    _waitForData();
                                }
                                onLoginFail(text);
                            },
                            success: function (data)
                            {
                                logger.setAjaxSuccess();
                                var $xml = data.xml;
                                _updateStoredViewXml(rootid, $xml, '0');
                                _resetPendingChanges(false, true);
                                if (perpetuateTimer)
                                {
                                    _waitForData();
                                }
                            },
                            error: function (data)
                            {
                                if (perpetuateTimer)
                                {
                                    _waitForData();
                                }
                                //restorePath();
                            }
                        });
                    }
                    else
                    {
                        if (perpetuateTimer)
                        {
                            _waitForData();
                        }
                    }
                }); // _getModifiedView();
            } else
            {
                if (perpetuateTimer)
                    _waitForData();
            } // if(SessionId != '') 
            logger.setEnded();
            cacheLogInfo(logger);
        } //_processChanges()

        // For proper chaining support
        return this;
    };
}) ( jQuery );

