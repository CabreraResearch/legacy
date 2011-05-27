/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="http://code.jquery.com/mobile/latest/jquery.mobile.js" />
/// <reference path="../jquery/jquery-validate-1.8/jquery.validate.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

//var profiler = $createProfiler();
//if (!debug) profiler.disable();

; (function ($) { /// <param name="$" type="jQuery" />
    
    $.fn.makeUL = function(id)
    {
        var $div = $(this);
        var $ret = undefined;
        if( !isNullOrEmpty($div) )
        {
            $ret = $('<ul id="' + tryParseString(id,'') + '"></ul>')
                            .appendTo($div)
                            .CswAttrXml({'data-filter': true,
                                         'data-role': 'listview',
                                         'data-inset': true});
            $ret.listview();
        }
        return $ret;
    }

    $.fn.bindLI = function()
    {
        var $li = $(this);
        var $ret = undefined;
        if( !isNullOrEmpty($li) )
        {
            $li.unbind('click');
            $ret = $li.find('li a').bind('click', function ()
            {
                var dataurl = $(this).CswAttrXml('data-url');
                var $thisPage = $('#' + dataurl);
                $.mobile.path.set(dataurl);
                $thisPage.doChangePage();
            });
            
        }
        return $ret;
    }

    $.fn.doChangePage = function (transition, reverse, changeHash)
	{
        var $div = $(this);
        var ret = false;
        if( !isNullOrEmpty($div) )
        {
            if(debug) log('doChangePage from: ' + $.mobile.activePage.CswAttrDom('id') + ' to: ' + $div.CswAttrDom('id'),true);
            ret = $.mobile.changePage($div, transition, reverse, changeHash);
        }
        return ret;
	}

    $.fn.doPage = function ()
    {
        var $div = $(this);
        var ret = false;
        if( !isNullOrEmpty($div) )
        {
            if(debug) log('doPage on ' + $div.CswAttrDom('id'),true);
            ret = $div.page();
        }
        return ret;
    }
    
    $.fn.bindJqmEvents = function(params)
    {
        var $div = $(this);
        var $ret = false;
        if( !isNullOrEmpty($div) )
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
                onPageShow: function(p) {}
            }
            
            if(params) $.extend(p,params);
            p.level = (p.parentlevel === p.level ) ? p.parentlevel+1 : p.level;
            $div.unbind('pageshow');
            $ret = $div.bind('pageshow', function() 
            {
                $.mobile.pageLoading();
                p.onPageShow(p);
                $.mobile.pageLoading(true);
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
    }

    $.fn.CswMobile = function (options) {
        /// <summary>
        ///   Generates the Nbt Mobile page
        /// </summary>
        /// <param name="options" type="Object">
        ///     A JSON Object
        ///     &#10;1 - options.Theme: 'a'
        ///     &#10;2 - options.PollingInterval: 30000
        ///     &#10;3 - options.DivRemovalDelay: 1000
        /// </param>

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
            Theme: 'a',
            PollingInterval: 30000,
            DivRemovalDelay: 1000,
            RandomConnectionFailure: false
        };
        
        if (options)
        {
            $.extend(opts, options);
        }
        
        var ForMobile = true;
        var rootid;
        var db;
        var UserName;
        var SessionId;
        var $currentViewXml;
        var currentMobilePath = '';


		// case 20355 - error on browser refresh
        // there is a problem if you refresh with #viewsdiv where we'll generate a 404 error, but the app will continue to function
        var tempdivid = 'initialloadingdiv';
        if (window.location.hash.length > 0)
        {
            var potentialtempdivid = window.location.hash.substr(1);
            if ($('#' + potentialtempdivid).length === 0 && potentialtempdivid !== 'viewsdiv' && potentialtempdivid !== 'logindiv')
            {
                tempdivid = potentialtempdivid;
            }
        }

        // Make loading div first
        _addPageDivToBody({
            DivId: tempdivid,
            HeaderText: 'Please wait',
            content: 'Loading...',
            HideSearchButton: true,
            HideOnlineButton: true,
            HideRefreshButton: true,
            HideLogoutButton: true,
            HideHelpButton: true
        });

        var $logindiv = _loadLoginDiv();
        
        var $viewsdiv = _loadViewsDiv();
        var $syncstatus = _makeSynchStatusDiv();
        var $helpdiv = _makeHelpDiv();
        var $sorrycharliediv = _loadSorryCharlieDiv(false);

        //$logindiv.doChangePage();

        _initDB(false, function ()
        {
            //_waitForData();

            readConfigVar('sessionid', function (configvar_sessionid)
            {
                if ( !isNullOrEmpty(configvar_sessionid) )
                {
                    SessionId = configvar_sessionid;
                    $viewsdiv = reloadViews();
                    $viewsdiv.doChangePage();
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
            }); // readConfigVar();
        }); // _initDB();

        function _loadLoginDiv()
        {
            var LoginContent = '<input type="textbox" id="login_accessid" placeholder="Access Id"/><br>';
            LoginContent += '<input type="textbox" id="login_username" placeholder="User Name"/><br>';
            LoginContent += '<input type="password" id="login_password" placeholder="Password"/><br>';
            LoginContent += '<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>';
            var $retDiv = _addDialogDivToBody({
                    DivId: 'logindiv',
                    HeaderText: 'Login to ChemSW Fire Inspection',
                    $content: $(LoginContent),
                    HideHelpButton: false
            });
            $('#loginsubmit').click(onLoginSubmit);
            $('#login_accessid').clickOnEnter($('#loginsubmit'));
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
                onPageShow: function(p) { return _loadDivContents(p); }
            };
            var $retDiv = _addDialogDivToBody({
                        DivId: 'viewsdiv',
                        HeaderText: 'Views',
                        HideRefreshButton: true,
                        HideSearchButton: true,
                        onPageShow: function(p) { return _loadDivContents(p); }
                });
            $retDiv.bindJqmEvents(params);
            return $retDiv;
		}

		function _loadSorryCharlieDiv()
        {
            $retDiv = _addDialogDivToBody({
                DivId: 'sorrycharliediv',
                HeaderText: 'Sorry Charlie!',
                $content: $('<p>You must have internet connectivity to login.</p>')                
            });
            return $retDiv;
        }

        function removeDiv(DivId)
        {
            setTimeout('$(\'#' + DivId + '\').find(' + 'div:jqmData(role="content")' + ').empty();', opts.DivRemovalDelay);
        }

        function reloadViews()
        {
            if ( $.mobile.activePage === $viewsdiv)
            {
                setTimeout(function () { 
                        $viewsdiv = continueReloadViews();
                    }, 
                    opts.DivRemovalDelay);
            } else
            {
                $viewsdiv = continueReloadViews();
            }
            return $viewsdiv;
        }

        function continueReloadViews()
        {
            if($viewsdiv) $viewsdiv.find('div:jqmData(role="content")').empty();
            var params = {
                parentlevel: -1,
                level: 0,
                DivId: 'viewsdiv',
                HeaderText: 'Views',
                HideRefreshButton: true,
                HideSearchButton: true,
                onPageShow: function(p) { return _loadDivContents(p); }
            };
            $viewsdiv.bindJqmEvents(params);
            return $viewsdiv;
        }

//        function clearPath()
//        {
//            currentMobilePath = tryParseString( $.mobile.path.get(), '');
//            if(debug) log('pre set path = ' + currentMobilePath, true);
//            if( currentMobilePath !== '') $.mobile.path.set('');
//            if(debug) log('post set path = ' + $.mobile.path.get());
//        }

//        function restorePath()
//        {
//            currentMobilePath = tryParseString( currentMobilePath, '');
//            $.mobile.path.set(currentMobilePath);
//        }

        // ------------------------------------------------------------------------------------
        // Online indicator
        // ------------------------------------------------------------------------------------

        function setOffline()
        {
            var $onlineStatus = $('.onlineStatus');
            if ($onlineStatus.hasClass('online'))
            {
                $onlineStatus.removeClass('online')
                             .addClass('offline')
                             .text('Offline');
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
            var $onlineStatus = $('.onlineStatus');
            if ($onlineStatus.hasClass('offline'))
            {
                $onlineStatus.removeClass('offline')
                             .addClass('online')
                             .text('Online');

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
            return $('.onlineStatus').hasClass('offline');
        }


        // ------------------------------------------------------------------------------------
        // List items fetching
        // ------------------------------------------------------------------------------------

        function _loadDivContents(params)
        {
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
                        _fetchCachedRootXml(function ($xml)
                        {
                            p.$xml = $xml;
                            $retDiv = _loadDivContentsXml(p);
                        });
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
                    _fetchCachedViewXml(rootid, function ($xmlstr)
                    {
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
                    });
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
                onloginfail: function() { Logout(); },
                success: function ($xml)
                {
                    if (debug) log('On Success ' + opts.ViewUrl, true);
                    $currentViewXml = $xml;
                    p.$xml = $currentViewXml;
                    if (params.level === 1)
                    {
                        _storeViewXml(p.DivId, p.HeaderText, $currentViewXml);
                    }
                    $retDiv = _loadDivContentsXml(p);    
                    //restorePath();
                },
                error: function(xml)
                {
                    if(debug) log(xml, true);
                    //restorePath();
                }
            });

            return $retDiv;
        }

        var currenttab;
        var onAfterAddDiv;
        function _processViewXml(params)
        {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xml: '',
                $xmlitem: '',
                parentlevel: '',
                level: '',
                HideRefreshButton: false,
                HideSearchButton: false
            };
            if (params)
            {
                $.extend(p, params);
            }

            var $retDiv = _addPageDivToBody({
                    ParentId: p.ParentId,
                    level: p.parentlevel,
                    DivId: p.DivId,
                    HeaderText: p.HeaderText,
                    //$content: $content,
                    HideRefreshButton: p.HideRefreshButton,
                    HideSearchButton: p.HideSearchButton
            });

            var $content = $retDiv.find('div:jqmData(role="content")').empty();
            var $list = $content.makeUL();            
            currenttab = '';

            onAfterAddDiv = function ($retDiv) { };

            p.$xml.children().each(function ()
            {
                p.$xmlitem = $(this);
                var $li = _makeListItemFromXml($list, p)
                            .appendTo($list);
            });
            $list.listview('refresh')
                 .bindLI()
                 .find('input[type="radio"]').checkboxradio('refresh',true);
            onAfterAddDiv($retDiv);
            
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
            var $toolbar = $('<div data-role="controlgroup data-type="horizontal"></div>');
            if ( !isNullOrEmpty(previd) )
            {
                $toolbar.CswLink('init',{href:'javascript:void(0);', value: 'Previous'})
                        .CswAttrXml({'data-identity': previd,
                                     'data-url': previd,
                                     'data-role': 'button',
                                     'data-icon': 'arrow-u',
                                     'data-inline': true,
                                     'data-theme': opts.Theme,
                                     'data-transition': 'slideup',
                                     'data-direction': 'reverse'
                        });
            }
            if ( !isNullOrEmpty(nextid) )
            {
                $toolbar.CswLink('init',{href:'javascript:void(0);', value: 'Next'})
                        .CswAttrXml({'data-identity': nextid,
                                     'data-url': nextid,
                                     'data-role': 'button',
                                     'data-icon': 'arrow-d',
                                     'data-inline': true,
                                     'data-theme': opts.Theme,
                                     'data-transition': 'slideup',
                                     'data-direction': 'reverse'
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
                        var fieldtype = tryParseString(p.$xmlitem.CswAttrXml('fieldtype'),'').toLowerCase();
                        var gestalt = p.$xmlitem.CswAttrXml('gestalt');
                        if (gestalt === 'NaN') gestalt = '';

                        var currentcnt = p.$xmlitem.prevAll('[fieldtype="'+fieldtype+'"]').andSelf().length;
                        var siblingcnt = p.$xmlitem.siblings('[fieldtype="'+fieldtype+'"]').andSelf().length;

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

                        var $link = $('<li id="' + id + '_li"><a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">' + text + '</a></li>')
                                        .appendTo($list);

                        switch (fieldtype)
                        {
                            case 'logical':
                                var sf_checked = tryParseString( p.$xmlitem.children('checked').text(), '');
                                var sf_required = tryParseString( p.$xmlitem.children('required').text(), '');

                                var $div = $('<div class="lisubstitute ui-li ui-btn-up-c"></div>')
                                                .appendTo($list);
                                var $logical = _makeLogicalFieldSet(id, 'ans', 'ans2', sf_checked, sf_required)
                                                .appendTo($div);
                                break;

                            case 'question':
                                var sf_answer = tryParseString( p.$xmlitem.children('answer').text() , '');
                                var sf_allowedanswers = tryParseString( p.$xmlitem.children('allowedanswers').text(), '');
                                var sf_compliantanswers = tryParseString( p.$xmlitem.children('compliantanswers').text(), '');
                                var sf_correctiveaction = tryParseString( p.$xmlitem.children('correctiveaction').text(), '');

                                var $div = $('<div class="lisubstitute ui-li ui-btn-up-c"><div>')
                                                .appendTo($list);
                                var $question = _makeQuestionAnswerFieldSet(p.DivId, id, 'ans', 'ans2', 'cor', 'li', 'propname', sf_allowedanswers, sf_answer, sf_compliantanswers)
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
                                var $gestalt = $('<p class="ui-li-aside">' + gestalt + '</p>')
                                                    .appendTo($link);
                                break;
                        }

                        if( fieldtype === "question")
                        {
                            $toolbar.append('&nbsp;' + currentcnt + '&nbsp;of&nbsp;' + siblingcnt);
                        }

                        _addPageDivToBody({
                            ParentId: p.DivId,
                            level: p.parentlevel,
                            DivId: id,
                            HeaderText: text,
                            $toolbar: $toolbar,
                            $content: _FieldTypeXmlToHtml(p.$xmlitem, p.DivId)
                        });
                        break;
                    } // case 'prop':
                default:
                    {
                        $retLI = $('<li></li>');
                        if (IsDiv)
                        {
                            $retLI.CswLink('init',{href: 'javascript:void(0);', value: text})
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
                                                HeaderText: text
                                                //,$toolbar: $toolbar
                            });
                        }
                        break;
                    } // default:
            }
            return $retLI;
        } // _makeListItemFromXml()

        function _makeUL(id)
        {
            var retUL = '<ul data-role="listview" id="' + tryParseString(id,'') + '">';
            return retUL;
        }

        function _endUL()
        {
            return '</ul>';
        }

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
                    Html += '<p>' + NodeName + '</p>';
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
                          });

            $retHtml = $(Html); //.listview('refresh');
            return $retHtml;
        }

        function _FieldTypeXmlToHtml($xmlitem, ParentId)
        {
            var IdStr = makeSafeId({ID: $xmlitem.CswAttrXml('id') });
            var FieldType = $xmlitem.CswAttrXml('fieldtype');
            var PropName = $xmlitem.CswAttrXml('name');
            var ReadOnly = ( isTrue($xmlitem.CswAttrXml('readonly')) );

            // Subfield values
            var sf_text = tryParseString( $xmlitem.children('text'), '');
            var sf_value = tryParseNumber( $xmlitem.children('value').text(), '');
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
            
            var $retHtml = $('<div id="' + IdStr + '_propwrapper"></div>');
            var $propNameDiv = $('<div id="' + IdStr + '_propname">' + PropName + '</div>')
                                .appendTo($retHtml);
            var $propContDiv = $('<div id="' + IdStr + '_propcontent"></div>')
                                .appendTo($retHtml);
            //var Html = '<div id="' + IdStr + '_propname"';
            if (FieldType === "Question" && !(sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0))
            {
                $propNameDiv.addClass('OOC');
            }
            $retHtml.append('<br/>');

            if( !ReadOnly )
            {
                switch (FieldType)
                {
                    case "Date":
                        $propContDiv.append('<input type="text" name="' + IdStr + '" value="' + sf_value + '" />');
                        break;

                    case "Link":
                        $propContDiv.append('<a href="' + sf_href + '" rel="external">' + sf_text + '</a>');
                        break;

                    case "List":
                        var $select = $('<select name="' + IdStr + '"></select>')
                                        .appendTo($propContDiv)
                                        .selectmenu();
                        var selectedvalue = sf_value;
                        var optionsstr = sf_options;
                        var options = optionsstr.split(',');
                        for (var i = 0; i < options.length; i++)
                        {
                            var $option = $('<option value="' + options[i] + '"></option>')
                                            .appendTo($select);
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
                        $select.selectmenu('refresh');
                        break;

                    case "Logical":
                        var $logical = _makeLogicalFieldSet(IdStr, 'ans2', 'ans', sf_checked, sf_required)
                                            .appendTo($propContDiv);
                        break;

                    case "Memo":
                        var $memo = $('<textarea name="' + IdStr + '">' + sf_text + '</textarea>')
                                            .appendTo($propContDiv);
                        break;

                    case "Number":
                        var $number = $('<input type="number" name="' + IdStr + '" value="' + sf_value + '" />')
                                            .appendTo($propContDiv);
                        // if (Prop.MinValue != Int32.MinValue)
                        //     Html += "min = \"" + Prop.MinValue + "\"";
                        // if (Prop.MaxValue != Int32.MinValue)
                        //     Html += "max = \"" + Prop.MaxValue + "\"";
                        break;

                    case "Password":
                        //nada
                        break;

                    case "Quantity":
                        var $quantity = $('<input type="text" name="' + IdStr + '_qty" value="' + sf_value + '" />')
                                            .appendTo($propContDiv);
                        $quantity.append( sf_units );
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
                        var $question = _makeQuestionAnswerFieldSet(ParentId, IdStr, 'ans2', 'ans', 'cor', 'li', 'propname', sf_allowedanswers, sf_answer, sf_compliantanswers)
                                            .appendTo($propContDiv);

                        var $corAction = $('<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action">' + sf_correctiveaction + '</textarea>')
                                            .appendTo($question);
                        if (sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0)
                        {
                            $corAction.css('display','none');
                        }
                        $corAction.change( function()
                        {
                            var $cor = $(this);
                            if($cor.val() === '') 
                            { 
                                  $('#' + IdStr + '_li div').addClass('OOC'); 
                                  $('#' + IdStr + '_propname').addClass('OOC');
                            } 
                            else 
                            {
                              $('#' + IdStr + '_li div').removeClass('OOC'); 
                              $('#' + IdStr + '_propname').removeClass('OOC'); 
                            }
                        });

                        var $comments = $('<textarea name="' + IdStr + '_com" placeholder="Comments">' + sf_comments + '</textarea>')
                                            .appendTo($question);
                        break;

                    case "Static":
                        $propContDiv.append( sf_text );
                        break;

                    case "Text":
                        var $text = $('<input type="text" name="' + IdStr + '" value="' + sf_text + '" />')
                                        .appendTo($propContDiv);
                        break;

                    case "Time":
                        var $time = $('<input type="text" name="' + IdStr + '" value="' + sf_value + '" />')
                                        .appendTo($propContDiv);
                        break;

                    default:
                        $propContDiv.append( $xmlitem.CswAttrXml('gestalt') );
                        break;
                }
            }
            else 
            {
                $propContDiv.append( $xmlitem.CswAttrXml('gestalt') );
            }
            return $retHtml;
        }

        function _FieldTypeHtmlToXml($xmlitem, name, value)
        {
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
                case "Date": if (name === IdStr) $sftomodify = $sf_value; break;
                case "Link": break;
                case "List": if (name === IdStr) $sftomodify = $sf_value; break;
                case "Logical":
                    if (name === makeSafeId({ID: IdStr, suffix: 'ans'}) || name === makeSafeId({ID: IdStr, suffix: 'ans2'}))
                    {
                        $sftomodify = $sf_checked;
                    }
                    break;
                case "Memo": if (name === IdStr) $sftomodify = $sf_text; break;
                case "Number": if (name === IdStr) $sftomodify = $sf_value; break;
                case "Password": break;
                case "Quantity": if (name === makeSafeId({ID: IdStr, suffix: 'qty'}) ) $sftomodify = $sf_value; break;
                case "Question":
                    if (name === makeSafeId({ID: IdStr, suffix: 'com'}) )
                    {
                        $sftomodify = $sf_comments;
                    }
                    else if (name === makeSafeId({ID: IdStr, suffix: 'ans'}) || name === makeSafeId({ID: IdStr, suffix: 'ans2'}))
                    {
                        $sftomodify = $sf_answer;
                    }
                    else if (name === makeSafeId({ID: IdStr, suffix: 'cor'}) )
                    {
                        $sftomodify = $sf_correctiveaction;
                    }
                    break;
                case "Static": break;
                case "Text": if (name === IdStr) $sftomodify = $sf_text; break;
                case "Time": if (name === IdStr) $sftomodify = $sf_value; break;
                default: break;
            }
            if ( !isNullOrEmpty($sftomodify) )
            {
                $sftomodify.text(value);
                $xmlitem.CswAttrXml('wasmodified', '1');
            }
        } // _FieldTypeHtmlToXml()

        function _makeLogicalFieldSet(IdStr, Suffix, OtherSuffix, Checked, Required)
        {
            var $retHtml = $('<div class="csw_fieldset" data-role="fieldcontain"></div>');
            var $fieldset = $('<fieldset></fieldset>')
                                .appendTo($retHtml)
                                .CswAttrDom({
                                    class: 'csw_fieldset',
                                    id: IdStr + '_fieldset'})
                                .CswAttrXml({
                                    'data-role': 'controlgroup',
                                    'data-type': 'horizontal'                                     
                                 });
            var answers = ['Null', 'True', 'False'];
            if ( isTrue(Required) )
            {
                answers = ['True', 'False'];
            }

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
                var $input = $fieldset.CswInput('init',{type: CswInput_Types.radio, ID: inputId, value: answers[i]})
                                .CswAttrXml('data-role','button');
                var $label = $('<label for="' + inputId + '">' + answertext + '</label>')
                                .appendTo($fieldset);
				
                // Checked is a Tristate, so isTrue() is not useful here
				if ((Checked === 'false' && answers[i] === 'False') ||
					(Checked === 'true' && answers[i] === 'True') ||
					(Checked === '' && answers[i] === 'Null'))
				{
                    $input.CswAttrDom('checked','checked');
                }
                
                $input.click( function ()
                {
                    for (var k = 0; k < answers.length; k++)
                    {
                        var thisAnswerId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[k]});
                        var $answer = $('#' + thisAnswerId);

                        var radioId = makeSafeId({ prefix: IdStr, ID: OtherSuffix, suffix: answers[k]});
                        var $otherradio = $('#' + radioId);
                        
                        if (answers[k] === answers[i])
                        {
                            //$answer.siblings('label').addClass('ui-btn-active'); // case 20307: bug is still here
                            $otherradio.CswAttrDom('checked', 'checked');
                            $otherradio.checkboxradio('refresh');
                            //$otherradio.siblings('label').addClass('ui-btn-active');
                        }
                        else
                        {
                            //$answer.siblings('label').removeClass('ui-btn-active'); // case 20307: bug is still here
                            $otherradio.removeProp('checked');
                            //$otherradio.siblings('label').removeClass('ui-btn-active');
                        }
                    } // for (var k = 0; k < answers.length; k++)
                });
            } // for (var i = 0; i < answers.length; i++)
            $retHtml.checkboxradio();
            return $retHtml;
        }

        function _makeQuestionAnswerFieldSet(ParentId, IdStr, Suffix, OtherSuffix, CorrectiveActionSuffix, LiSuffix, PropNameSuffix, Options, Answer, CompliantAnswers)
        {
            var $retHtml = $('<div class="csw_fieldset" data-role="fieldcontain"></div>');
            var $fieldset = $('<fieldset></fieldset>')
                                .appendTo($retHtml)
                                .CswAttrDom({
                                    class: 'csw_fieldset',
                                    id: IdStr + '_fieldset'})
                                .CswAttrXml({
                                    'data-role': 'controlgroup',
                                    'data-type': 'horizontal'  
                                 });
            var answers = Options.split(',');
            for (var i = 0; i < answers.length; i++)
            {
				var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });
                var $answer = $fieldset.CswInput('init',{type: CswInput_Types.radio, ID: answerid, value: answers[i] })
								.CswAttrXml('data-role','button');
				var $label = $('<label for="' + answerid + '">' + answers[i] + '</label>')
								.appendTo($fieldset);
                
                if (Answer === answers[i])
                {
                    $answer.CswAttrDom('checked','checked');
                } 
				$answer.bind('click', function(eventObj) 
				{
					for (var k = 0; k < answers.length; k++)
					{
                        var suffixAnswerId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[k] });
                        var $suffixAnswer = $('#' + suffixAnswerId);
						
                        var oSuffixAnswerId = makeSafeId({ prefix: IdStr, ID: OtherSuffix, suffix: answers[k]});
						var $oSuffixAnswer = $('#' + oSuffixAnswerId);
						
                        if (answers[k] === answers[i])
						{
                            //$suffixAnswer.siblings('label').addClass('ui-btn-active'); // case 20307: bug is still here
							$oSuffixAnswer.CswAttrDom('checked', 'checked');
						    $oSuffixAnswer.checkboxradio('refresh');
                        	//$oSuffixAnswer.siblings('label').addClass('ui-btn-active');
                            //$oSuffixAnswer.siblings('label').addClass('ui-btn-active');
						} 
                        else
						{
							//$suffixAnswer.siblings('label').removeClass('ui-btn-active'); // case 20307: bug is still here
                            $oSuffixAnswer.removeProp('checked');
							//$oSuffixAnswer.siblings('label').removeClass('ui-btn-active');
                            //$oSuffixAnswer.siblings('label').removeClass('ui-btn-active');
						}
                        
					} // for (var k = 0; k < answers.length; k++)
					
					var correctiveActionId = makeSafeId({ prefix: IdStr, ID: CorrectiveActionSuffix});
					var liSuffixId = makeSafeId({ prefix: IdStr, ID: LiSuffix});
					var propNameSuffixId = makeSafeId({ prefix: IdStr, ID: PropNameSuffix});
					
					var $cor = $('#' + correctiveActionId);
					var $li = $('#' + liSuffixId);
					var $prop = $('#' + propNameSuffixId);
					
					if ((',' + CompliantAnswers + ',').indexOf(',' + answers[i] + ',') >= 0)
					{
						$cor.css('display','none');
						$li.children('div').removeClass('OOC');
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
					if ( isNullOrEmpty(Answer) )
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
            $retHtml.checkboxradio();
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
            var $synchstatusBtn = $('#' + p.DivId + '_gosynchstatus');
            var $refreshBtn = $('#' + p.DivId + '_refresh');
            var $logoutBtn = $('#' + p.DivId + '_logout');
            var $helpBtn = $('#' + p.DivId + '_help');

            if( isNullOrEmpty($pageDiv) || $pageDiv.length === 0 )
            {
                $pageDiv = $body.CswDiv('init',{ID: p.DivId})
                                .CswAttrXml({'data-role':'page', 'data-url': p.DivId, 'data-title': p.HeaderText}); 
            
			    var $header = $pageDiv.CswDiv('init',{ID: p.DivId + '_header'})
                                      .CswAttrXml({'data-role': 'header','data-theme': opts.Theme, 'data-position':'fixed'});
                var $backlink = $header.CswLink('init',{'href': 'javascript:void(0)', 
                                                        ID: p.DivId + '_back',
                                                        value: 'Back'})
                                        .CswAttrXml({'data-identity': p.DivId + '_back', 
                                                     //'data-url': p.DivId + '_back',
													 'data-rel': 'back',
                                                     'data-direction': 'reverse' });
            
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
                                            value: 'Search' })
                                      .CswAttrXml({'data-identity': p.DivId + '_searchopen', 
                                                   'data-url': p.DivId + '_searchopen', 
                                                   'data-transition': 'slidedown',
                                                   'data-role': 'button' });

                $header.CswDiv('init',{class: 'toolbar'})
                       .append(p.$toolbar)
                       .CswAttrXml({'data-role':'controlgroup','data-type':'horizontal'});
                var $content = $pageDiv.CswDiv('init',{ID: p.DivId + '_content'})
                                       .CswAttrXml({'data-role':'content','data-theme': opts.Theme})
                                       .append(p.$content);
                var $footer = $pageDiv.CswDiv('init',{ID: p.DivId + '_footer'})
                                      .CswAttrXml({'data-role':'footer', 'data-theme': opts.Theme, 'data-position':'fixed'});

                
                var onlineClass = (amOffline()) ? 'onlineStatus offline' : 'onlineStatus online';
                var onlineValue = (amOffline()) ? 'Offline' : 'Online';

                $synchstatusBtn = $footer.CswLink('init',{'href': 'javascript:void(0)', 
                                                    ID: p.DivId + '_gosynchstatus', 
                                                    class: onlineClass,  
                                                    value: onlineValue })
                                    .CswAttrXml({'data-identity': p.DivId + '_gosynchstatus', 
                                                'data-url': p.DivId + '_gosynchstatus', 
                                                'data-transition': 'slideup',
                                                'data-role': 'button' });

                $refreshBtn = $footer.CswLink('init',{'href': 'javascript:void(0)', 
                                                       ID: p.DivId + '_refresh', 
                                                       value:'Refresh', 
                                                       class: 'refresh'})
                                      .CswAttrXml({'data-identity': p.DivId + '_refresh', 
                                                   'data-url': p.DivId + '_refresh',
                                                   'data-role': 'button' });

                $logoutBtn = $footer.CswLink('init',{'href': 'javascript:void(0)', 
                                                    ID: p.DivId + '_logout', 
                                                    value: 'Logout' })
                                   .CswAttrXml({'data-identity': p.DivId + '_logout', 
                                                'data-url': p.DivId + '_logout', 
                                                'data-transition': 'flip' });
                
            
                var $mainBtn = $footer.CswLink('init',{href: 'NewMain.html', rel: 'external', ID: p.DivId + '_newmain', value: 'Full Site'})
                                      .CswAttrXml('data-transition', 'pop');


                $helpBtn = $footer.CswLink('init',{'href': 'javascript:void(0)', 
                                                   ID: p.DivId + '_help', 
                                                   value: 'Help' })
                                   .CswAttrXml({'data-identity': p.DivId + '_help', 
                                                'data-url': p.DivId + '_help', 
                                                'data-transition': 'slideup' });
            }

            if ( p.HideOnlineButton ) { 
                $synchstatusBtn.hide(); 
            }
            else {
                $synchstatusBtn.show(); 
            }
            if ( p.HideHelpButton ) {
                $helpBtn.hide();
            }
            else {
                $helpBtn.show();
            }
            if ( p.HideLogoutButton ) {
                $logoutBtn.hide();
            }
            else {
                $logoutBtn.show();
            }
            if ( p.HideRefreshButton ) {
                $refreshBtn.hide();
            }
            else {
                $refreshBtn.show();
            }
            if ( p.HideSearchButton ) {
                $searchBtn.hide();
            }
            else {
                $searchBtn.show();
            }

            //if(p.level === 0) $pageDiv.doPage();
            _bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv);

            return $pageDiv;

        } // _addPageDivToBody()
        
        function _addDialogDivToBody(params)
        {
            var p = {
                DivId: '',       // required
                HeaderText: '',
                $toolbar: '',
                $content: '',
                HideHelpButton: false
            };

            if (params)
            {
                $.extend(p, params);
            }

            p.DivId = makeSafeId({ID: p.DivId});

            var $pageDiv = $('#' + p.DivId);

            if( isNullOrEmpty($pageDiv) || $pageDiv.length === 0 )
            {
                $pageDiv = $body.CswDiv('init',{ID: p.DivId})
                                .CswAttrXml({'data-role':'page', 'data-url': p.DivId, 'data-title': p.HeaderText, 'data-rel': 'dialog'}); 
            
		        var $header = $pageDiv.CswDiv('init',{ID: p.DivId + '_header'})
                                        .CswAttrXml({'data-role': 'header','data-theme': opts.Theme, 'data-position':'inline'});
                $header.append($('<h1>' + p.HeaderText + '</h1>'));
                $header.CswDiv('init',{class: 'toolbar'})
                        .append(p.$toolbar)
                        .CswAttrXml({'data-role':'controlgroup','data-type':'horizontal'});
                var $content = $pageDiv.CswDiv('init',{ID: p.DivId + '_content'})
                                        .CswAttrXml({'data-role':'content','data-theme': opts.Theme})
                                        .append(p.$content);
                var $footer = $pageDiv.CswDiv('init',{ID: p.DivId + '_footer'})
                                        .CswAttrXml({'data-role':'footer', 'data-theme': opts.Theme, 'data-position':'fixed'});
            
                $footer.CswLink('init',{href: 'NewMain.html', rel: 'external', ID: p.DivId + '_newmain', value: 'Full Site'});

                if (!p.HideHelpButton)
                {
                    $footer.CswLink('init',{'href': 'javascript:void(0)', ID: p.DivId + '_help', value: 'Help'})
                            .CswAttrXml({'data-identity': p.DivId, 'data-url': p.DivId });
                }
                $pageDiv.doPage();
            }
            _bindDialogEvents(p.DivId, p.ParentId, p.level, $pageDiv);

            return $pageDiv;

        } // _addPageDivToBody()

        function _getDivHeaderText(DivId)
        {
            return $('#' + DivId).find('div:jqmData(role="header") h1').text();
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
                .click(function (eventObj) { onSearchOpen(DivId, eventObj); })
                .end()
                .find('#' + DivId + '_gosynchstatus')
                .click(function (eventObj) { onSynchStatusOpen(DivId, eventObj); })
                .end()
                .find('#' + DivId + '_refresh')
                .click(function (e) { /*e.stopPropagation(); e.preventDefault();*/ return onRefresh(DivId, e); })
                .end()
                .find('#' + DivId + '_logout')
                .click(function (e) { /*e.stopPropagation(); e.preventDefault();*/ return onLogout(DivId, e); })
                .end()
                .find('#' + DivId + '_back')
                .click(function (eventObj) { return onBack(DivId, ParentId, eventObj); })
                .end()
                .find('#' + DivId + '_help')
                .click(function (eventObj) { return onHelp(DivId, ParentId, eventObj); })
                .end()
//                .find('input')
//                .change(function (eventObj) {  onPropertyChange(DivId, eventObj); })
//                .end()
                .find('textarea')
                .change(function (eventObj) { onPropertyChange(DivId, eventObj); })
                .end()
                .find('select')
                .change(function (eventObj) { onPropertyChange(DivId, eventObj); })
                .end();
        }
        
        function _bindDialogEvents(DivId, ParentId, level, $div)
        {
            $div.find('#' + DivId + '_help')
                .click(function (eventObj) { return onHelp(DivId, ParentId, eventObj); })
                .end()
                .find('input')
                .change(function (eventObj) { onPropertyChange(DivId, eventObj); })
                .end()
                .find('textarea')
                .change(function (eventObj) { onPropertyChange(DivId, eventObj); })
                .end()
                .find('select')
                .change(function (eventObj) { onPropertyChange(DivId, eventObj); })
                .end();
        }
        // ------------------------------------------------------------------------------------
        // Synch Status Div
        // ------------------------------------------------------------------------------------

        function _makeSynchStatusDiv()
        {
            var content = '';
            content += '<p>Pending Unsynched Changes: <span id="ss_pendingchangecnt">No</span></p>';
            content += '<p>Last synch: <span id="ss_lastsynch"></span></p>';
            content += '<a id="ss_forcesynch" data-identity="ss_forcesynch" data-url="ss_forcesynch" href="javascript:void(0)" data-role="button">Force Synch Now</a>';
            content += '<a id="ss_gooffline" data-identity="ss_gooffline" data-url="ss_gooffline" href="javascript:void(0)" data-role="button">Go Offline</a>';
            

            var $retDiv = _addPageDivToBody({
                    DivId: 'synchstatus',
                    HeaderText: 'Synch Status',
                    $content: $(content),
                    HideSearchButton: true,
                    HideRefreshButton: true
            });

            $retDiv.find('#ss_forcesynch')
                    .click(function (eventObj) { _processChanges(false); } ) //eventObj.preventDefault(); })
                    .end()
                    .find('#ss_gooffline')
                    .click(function (eventObj) { _toggleOffline(eventObj); });

            return $retDiv;
        }

        function _toggleOffline(eventObj)
        {
            eventObj.preventDefault();
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

        function _resetPendingChanges(val, setlastsynchnow)
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
            if (setlastsynchnow)
            {
                var d = new Date();
                $('#ss_lastsynch').text(d.toLocaleDateString() + ' ' + d.toLocaleTimeString());
            }
        }

        // returns true if no pending changes or user is willing to lose them
        function _checkNoPendingChanges()
        {
            return (!_pendingChanges() ||
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
            var $help = $('<p>Help</p>');
            
            var $retDiv = _addPageDivToBody({
                    DivId: 'help',
                    HeaderText: 'Help',
                    $content: $help,
                    HideSearchButton: true,
                    HideRefreshButton: true
            });
            
            return $retDiv;
        }

        // ------------------------------------------------------------------------------------
        // Events
        // ------------------------------------------------------------------------------------

        function onLoginSubmit(eventObj)
        {
            // authenticate here
            UserName = $('#login_username').val();
            var Password = $('#login_password').val();
            var AccessId = $('#login_accessid').val();

            if (!amOffline())
            {
                var ajaxData = {
                    'AccessId': AccessId, 
                    'UserName': UserName, 
                    'Password': Password
                };

                //clearPath();
                CswAjaxJSON({
                    formobile: ForMobile,
                    async: false,
                    url: opts.AuthenticateUrl,
                    data: ajaxData,
                    onloginfail: function () { Logout(); },
                    success: function (data)
                    {
                        SessionId = $.CswCookie('get', CswCookieName.SessionId);
						_cacheSession(SessionId, UserName);
                        $viewsdiv = reloadViews();
                        $viewsdiv.doChangePage();
                        //restorePath();
                    },
                    error: function()
                    {
                        //restorePath();
                    }
                });
            }

        } //onLoginSubmit() 

        function onLogout(DivId, eventObj)
        {
            Logout();
        }

        function Logout()
        {
            if (_checkNoPendingChanges())
            {
                _dropDb(function ()
                {
                    // reloading browser window is the easiest way to reset
                    window.location.href = window.location.pathname;
                });
            }
        }

        function onRefresh(DivId, eventObj)
        {
            if (!amOffline())
            {
                if (_checkNoPendingChanges())
                {
                    setTimeout(function () { 
                            continueRefresh(DivId); 
                        }, 
                        opts.DivRemovalDelay);
                }
            }
            return false;
        }

        function continueRefresh(DivId)
        {
            // remove existing divs
            var NextParentId = '';
            var ThisParentId = DivId;
//            while ( !isNullOrEmpty(ThisParentId) && ThisParentId.substr(0, 'viewid_'.length) !== 'viewid_')
//            {
//                NextParentId = _getDivParentId(ThisParentId);
//                $('div[id*="' + ThisParentId + '"]').find('div:jqmData(role="content")').empty();
//                ThisParentId = NextParentId;
//            }

            if ( !isNullOrEmpty(ThisParentId) )
            {
                var RealDivId = ThisParentId;
                var HeaderText = _getDivHeaderText(RealDivId);

               // $('div[id*="' + RealDivId + '"]').find('div:jqmData(role="content")').empty();

                var dataXml = {
                    SessionId: SessionId,
                    ParentId: RealDivId,
                    ForMobile: ForMobile
                };
                //clearPath();
                // fetch new content
                CswAjaxXml({
                    async: false,   // required so that the link will wait for the content before navigating
                    formobile: ForMobile,
                    url: opts.ViewsListUrl,
                    data: dataXml,
                    stringify: false,
                    onloginfail: function() { Logout(); },
                    success: function ($xml)
                    {
                        if (debug) log('On Success ' + opts.ViewsListUrl, true);

                        $currentViewXml = $xml;
                        _updateStoredViewXml(RealDivId, $currentViewXml, '0');

                        var params = {
                            ParentId: 'viewsdiv',
                            DivId: RealDivId,
                            HeaderText: HeaderText,
                            '$xml': $currentViewXml,
                            parentlevel: -1,
                            level: 0,
                            HideRefreshButton: false,
                            HideSearchButton: true,
                            onPageShow: function(p) { return _loadDivContents(p); }
                        };

                        $viewsdiv.bindJqmEvents(params);
                        $viewsdiv.doChangePage();
                        //restorePath();
                    }, // success
                    error: function()
                    {
                        //restorePath();
                    } 
                });
            }
        }

        function onBack(DivId, DestinationId, eventObj)
        {
            if (DivId !== 'synchstatus' && DivId.indexOf('prop_') !== 0)
            {
                // case 20367 - remove all matching DivId.  Doing it immediately causes bugs.
                //setTimeout('$(\'div[id*="' + DivId + '"]\').remove();', opts.DivRemovalDelay);
            }
            return true;
        }


        function onSynchStatusOpen(DivId, eventObj)
        {
            $('#synchstatus_back').CswAttrDom({'href': 'javascript:void(0)' })
                                  .CswAttrXml({'data-identity': DivId, 
                                               'data-url': DivId });
            
            $('#synchstatus_back').css('visibility', '');
        }

        function onHelp(DivId, eventObj)
        {
            //
        }

        function onPropertyChange(DivId, eventObj)
        {
            var $elm = $(eventObj.target);
            var name = $elm.CswAttrDom('name');
            var value = $elm.val();

            // update the xml and store it
            if( !isNullOrEmpty($currentViewXml) )
            {
//            _fetchCachedViewXml(rootid, function (xmlstr)
//            {
//                if (xmlstr != '')
//                {

                    var $divxml = $currentViewXml.find('#' + DivId);
                    $divxml.andSelf().find('prop').each(function ()
                    {
                        var $fieldtype = $(this);
                        _FieldTypeHtmlToXml($fieldtype, name, value);
                    });

                    // Strictly speaking, this is not a valid use of html() since we're operating on xml.  
                    // However, it appears to work, for now.
                    _updateStoredViewXml(rootid, $currentViewXml, '1');

                    _resetPendingChanges(true, false);
                }
            //});

        } // onPropertyChange()

        function onSearchOpen(DivId, eventObj)
        {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            _fetchCachedViewXml(rootid, function ($xmlstr)
            {
                if ( !isNullOrEmpty($xmlstr) )
                {
                    var Html = '<select id="' + DivId + '_searchprop" name="' + DivId + '_searchprop">';

                    $xmlstr.closest('result')
                    .find('searches')
                    .children()
                    .each(function ()
                    {
                        var $search = $(this);
                        Html += '<option value="' + $search.CswAttrXml('id') + '">' + $search.CswAttrXml('name') + '</option>';
                    });

                    Html += '</select>' +
                        '<input type="search" name="' + DivId + '_searchfor" id="' + DivId + '_searchfor" value="" placeholder="Search" />' +
                        '<input type="button" id="' + DivId + '_searchgo" data-inline="true" value="Go" /> ' +
                        '<div id="' + DivId + '_searchresults"></div>';


                    _addPageDivToBody({
                        ParentId: DivId,
                        DivId: DivId + '_searchdiv',
                        HeaderText: 'Search',
                        $content: $(Html),
                        HideSearchButton: true,
                        HideRefreshButton: true,
                        backicon: 'arrow-u'
                    });

                    $('#' + DivId + '_searchgo').click(function (eventObj) { onSearchSubmit(DivId, eventObj); });

                    $('#' + DivId + '_searchdiv').doChangePage("slideup", false, true);
                }
            });
        }

        function onSearchSubmit(DivId, eventObj)
        {
            var searchprop = $('#' + DivId + '_searchprop').val();
            var searchfor = $('#' + DivId + '_searchfor').val();
            _fetchCachedViewXml(rootid, function ($xmlstr)
            {
                if ( !isNullOrEmpty($xmlstr) )
                {
                    var $content = _makeUL(DivId + '_searchresultslist');

                    var hitcount = 0;
                    $xmlstr.find('node').each(function ()
                    {
                        var $node = $(this);
                        if ( !isNullOrEmpty($node.CswAttrXml(searchprop)) )
                        {
                            if ($node.CswAttrXml(searchprop).toLowerCase().indexOf(searchfor.toLowerCase()) >= 0)
                            {
                                hitcount++;
                                $content.append( makeListItemFromXml($node, DivId, 1) );
                            }
                        }
                    });
                    if (hitcount === 0)
                    {
                        $content.append( $('<li>No Results</li>'));
                    }
                    $content.listview('refresh');
                    var $srdiv = $('#' + DivId + '_searchresults');
                    $srdiv.children().remove();
                    $srdiv.append($content).doPage();
                    $('#' + DivId + '_searchresultslist').listview();

                    _bindPageEvents(DivId + '_searchdiv', DivId, 1, $srdiv);
                }
            });
        }

        // ------------------------------------------------------------------------------------
        // Client-side Database Interaction
        // ------------------------------------------------------------------------------------


        function _DoSql(sql, params, onSuccess)
        {
            if (window.openDatabase)
            {
                db.transaction(function (transaction)
                {
                    transaction.executeSql(sql, params, onSuccess, _errorHandler);
                });
            } else
            {
                log("database is not opened", true);
            }
        } //_DoSql

        function _initDB(doreset, OnSuccess)
        {
            db = openDatabase(opts.DBShortName, opts.DBVersion, opts.DBDisplayName, opts.DBMaxSize);
            if (doreset)
            {
                _dropDb(function () { _createDb(OnSuccess); });
            } else
            {
                _createDb(OnSuccess);
            }
        } //_initDb()

        function _dropDb(OnSuccess)
        {
            _DoSql('DROP TABLE IF EXISTS configvars; ', null, function ()
            {
                _DoSql('DROP TABLE IF EXISTS views; ', null, function ()
                {
                    if (!isNullOrEmpty(OnSuccess) )
                    {
                        OnSuccess();
                    }
                });
            });
        } // _dropDB()

        function _createDb(OnSuccess)
        {
            _DoSql( 'CREATE TABLE IF NOT EXISTS configvars ' +
                    '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
                    '   varname TEXT NOT NULL, ' +
                    '   varval TEXT);',
                    null,
                    function() {
                        _DoSql( 'CREATE TABLE IF NOT EXISTS views ' +
                                '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
                                '   rootid TEXT NOT NULL, ' +
                                '   rootname TEXT NOT NULL, ' +
                                '   viewxml TEXT, ' +
                                '   wasmodified INTEGER );',
                                null,
                                OnSuccess
                                );
                    } );

        } //_createDb() 

        function _errorHandler(transaction, error)
        {
            log('Database Error: ' + error.message + ' (Code ' + error.code + ')', true);
            return true;
        }

        function writeConfigVar(varname, varval, onsuccess)
        {
            _DoSql("select varval from configvars where varname=?;",
                   [varname],
                   function (transaction, result)
                   {
                       if (0 === result.rows.length)
                       {
                           _DoSql("insert into configvars (varname, varval) values ( ?, ? );",
                                  [varname, varval],
                                  function ()
                                  {
                                      if ( !isNullOrEmpty(onsuccess) )
                                      {
                                          onsuccess();
                                      }
                                  });
                       } else
                       {
                           _DoSql("update configvars set varval = ? where varname = ?",
                                  [varval, varname],
                                  function ()
                                  {
                                      if ( !isNullOrEmpty(onsuccess) )
                                      {
                                          onsuccess();
                                      }
                                  });
                       } //if-else the configvar row already exists
                   });
        } //writeConfigVar() 

        function readConfigVar(varname, onSuccess)
        {
            _DoSql("select varval from configvars where varname=?;",
                   [varname],
                   function (transaction, result)
                   {
                       if (result.rows.length > 0)
                       {
                           var row = result.rows.item(0);
                           onSuccess(row.varval);
                       } else
                       {
                           onSuccess('');
                       }
                   });

        } //readConfigVar()

        // ------------------------------------------------------------------------------------
        // Persistance functions
        // ------------------------------------------------------------------------------------


        function _cacheSession(sessionid, username, onsuccess)
        {
            writeConfigVar('username', username, function ()
            {
                writeConfigVar('sessionid', sessionid, function ()
                {
                    if ( !isNullOrEmpty(onsuccess) )
                    {
                        onsuccess();
                    }
                });
            });
        } //_cacheSession()

        //        function _clearSession(onsuccess)
        //        {
        //            writeConfigVar('username', '', function ()
        //            {
        //                writeConfigVar('sessionid', '', function ()
        //                {
        //                    if (onsuccess != undefined)
        //                        onsuccess();
        //                });
        //            });
        //        } //_clearSession()


        function _storeViewXml(rootid, rootname, $viewxml)
        {
            if ( !isNullOrEmpty(rootid) )
            {
                _DoSql('INSERT INTO views (rootid, rootname, viewxml, wasmodified) VALUES (?, ?, ?, 0);',
                       [rootid, rootname, xmlToString($viewxml)]);
            }
        }

        function _updateStoredViewXml(rootid, $viewxml, wasmodified)
        {
            if ( !isNullOrEmpty(rootid) )
            {
                _DoSql('UPDATE views SET wasmodified = ?, viewxml = ? WHERE rootid = ?;',
                       [wasmodified, xmlToString($viewxml), rootid]);
            }
        }

        function _getModifiedView(onSuccess)
        {
            _DoSql('SELECT rootid, viewxml FROM views WHERE wasmodified = 1 ORDER BY id DESC;',
                   [],
                   function (transaction, result)
                   {
                       if (result.rows.length > 0)
                       {
                            _resetPendingChanges(true, true);
                            var row = result.rows.item(0);
                            var rootid = row.rootid;
                            var viewxml = row.viewxml;
                            if( !isNullOrEmpty(rootid) && !isNullOrEmpty(viewxml) )
                            {
                                onSuccess(rootid, viewxml);
                            }
                       } else
                       {
                           _resetPendingChanges(false, true);
                           onSuccess();
                       }
                   });
        }

        function _fetchCachedViewXml(rootid, onsuccess)
        {
            if ( !isNullOrEmpty(rootid) )
            {
                _DoSql('SELECT viewxml FROM views WHERE rootid = ? ORDER BY id DESC;',
                       [rootid],
                       function (transaction, result)
                       {
                           if (result.rows.length > 0)
                           {
                               var row = result.rows.item(0);
							   var $viewxml = $(row.viewxml);
                               onsuccess($viewxml);
                           } else
                           {
                               onsuccess();
                           }
                       });
            }
        }
        function _fetchCachedRootXml(onsuccess)
        {
            _DoSql('SELECT rootid, rootname FROM views ORDER BY rootname;',
                   [],
                   function (transaction, result)
                   {
                       var ret = '';
                       for (var i = 0; i < result.rows.length; i++)
                       {
                           var row = result.rows.item(i);
                           ret += "<view id=\"" + row.rootid + "\"" +
                                  " name=\"" + row.rootname + "\" />";
                       }
                       var $xml = $("<result>" + ret + "</result>");
                       onsuccess($xml);
                   });
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
                onloginfail: function() { Logout(); },
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
                            onloginfail: function() 
                            { 
                                if (perpetuateTimer)
                                {
                                    _waitForData();
                                }
                                //restorePath(); 
                            },
                            success: function (data)
                            {
                                if (debug) log('On Success ' + opts.UpdateUrl, true);
                                var $xml = data.xml;
                                _updateStoredViewXml(rootid, $xml, '0');
                                if (perpetuateTimer)
                                {
                                    _waitForData();
                                }
                                //restorePath();
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
        } //_processChanges()

        // For proper chaining support
        return this;
    };
}) ( jQuery );

