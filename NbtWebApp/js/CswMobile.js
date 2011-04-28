/// <reference path="../jquery/jquery-1.5.2-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    
    $.fn.CswMobile = function (options)
    {
        var opts = {
            DBShortName: 'Mobile.html',
            DBVersion: '1.0',
            DBDisplayName: 'Mobile.html',
            DBMaxSize: 65536,
            ViewUrl: '/NbtWebApp/wsMobile.asmx/RunView',
            ConnectTestUrl: '/NbtWebApp/wsMobile.asmx/ConnectTest',
            ConnectTestRandomFailUrl: '/NbtWebApp/wsMobile.asmx/ConnectTestRandomFail',
            UpdateUrl: '/NbtWebApp/wsMobile.asmx/UpdateProperties',
            MainPageUrl: '/NbtWebApp/Mobile.html',
            AuthenticateUrl: '/NbtWebApp/wsMobile.asmx/Authenticate',
            Theme: 'a',
            PollingInterval: 30000,
            DivRemovalDelay: 1000,
            RandomConnectionFailure: false
        };
        
        if (options)
        {
            $.extend(opts, options);
        }
        
        var rootid;
        var db;
        var UserName;
        var SessionId;
        var $currentViewXml;

        // case 20355 - error on browser refresh
        // there is a problem if you refresh with #viewsdiv where we'll generate a 404 error, but the app will continue to function
        var tempdivid = 'initialloadingdiv';
        if (window.location.hash.length > 0)
        {
            var potentialtempdivid = window.location.hash.substr(1);
            if ($('#' + potentialtempdivid).length == 0 && potentialtempdivid != 'viewsdiv' && potentialtempdivid != 'logindiv')
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

        _makeSynchStatusDiv();
        _makeHelpDiv();

        _initDB(false, function ()
        {
            //_waitForData();

            readConfigVar('sessionid', function (configvar_sessionid)
            {
                if (configvar_sessionid != '' && configvar_sessionid != undefined)
                {
                    SessionId = configvar_sessionid;
                    reloadViews(true);
                    removeDiv(tempdivid);
                    _waitForData();
                }
                else
                {
                    // this will trigger _waitForData(), but we don't want to wait here
                    _handleDataCheckTimer(
                        function ()
                        {
                            // online
                            _loadLoginDiv(true);
                            removeDiv(tempdivid);
                        },
                        function ()
                        {
                            // offline
                            _loadSorryCharlieDiv(true);
                            removeDiv(tempdivid);
                        }
                    ); // _handleDataCheckTimer();
                } // if-else (configvar_sessionid != '' && configvar_sessionid != undefined)
            }); // readConfigVar();
        }); // _initDB();

        function _loadLoginDiv(ChangePage)
        {
            var LoginContent = '<input type="textbox" id="login_accessid" placeholder="Access Id"/><br>';
            LoginContent += '<input type="textbox" id="login_username" placeholder="User Name"/><br>';
            LoginContent += '<input type="password" id="login_password" placeholder="Password"/><br>';
            LoginContent += '<a id="loginsubmit" data-role="button" href="#">Continue</a>';
            _addPageDivToBody({
                DivId: 'logindiv',
                HeaderText: 'Login to ChemSW Fire Inspection',
                content: LoginContent,
                HideSearchButton: true,
                HideRefreshButton: true,
                HideLogoutButton: true
            });
            $('#loginsubmit').click(onLoginSubmit);
            if (ChangePage)
                $.mobile.changePage($('#logindiv'), 'fade', false, true);
        }

        function _loadSorryCharlieDiv(ChangePage)
        {
            _addPageDivToBody({
                DivId: 'sorrycharliediv',
                HeaderText: 'Sorry Charlie!',
                content: 'You must have internet connectivity to login.',
                HideSearchButton: true,
                HideRefreshButton: true,
                HideLogoutButton: true
            });
            if (ChangePage)
                $.mobile.changePage($('#sorrycharliediv'), 'fade', false, true);
        }

        function removeDiv(DivId)
        {
            setTimeout('$(\'#' + DivId + '\').remove();', opts.DivRemovalDelay);
        }

        function reloadViews(ChangePage)
        {
            if ($('#viewsdiv').hasClass('ui-page-active'))
            {
                _addPageDivToBody({
                    DivId: 'loadingdiv',
                    HeaderText: 'Please wait',
                    content: 'Loading...',
                    HideSearchButton: true,
                    HideOnlineButton: true,
                    HideRefreshButton: true,
                    HideLogoutButton: true,
                    HideHelpButton: true
                });
                $.mobile.changePage($('#loadingdiv'), "fade", false, true);
                setTimeout(function () { continueReloadViews(true); removeDiv('loadingdiv') }, opts.DivRemovalDelay);
            } else
            {
                continueReloadViews(ChangePage)
            }
        }

        function continueReloadViews(ChangePage)
        {
            $('#viewsdiv').remove();
            _loadDivContents({
                level: 0,
                DivId: 'viewsdiv',
                HeaderText: 'Views',
                HideRefreshButton: true,
                HideSearchButton: true,
                ChangePage: ChangePage
            });

        }

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

                reloadViews(false);
            }
            if ($('#logindiv').length > 0)
            {
                _loadSorryCharlieDiv(true);
                removeDiv('logindiv');
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
                reloadViews(false);
            }
            if ($('#sorrycharliediv').length > 0)
            {
                _loadLoginDiv(true);
                removeDiv('sorrycharliediv');
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
                ChangePage: false
            };
            
            if (params)
            {
                $.extend(p, params);
            }


            var ret = true;

            if (p.level == 1)
                rootid = p.DivId;

            if ($('#' + p.DivId).length == 0)
            {
                if (p.level == 0)
                {
                    if (amOffline())
                    {
                        _fetchCachedRootXml(function (xml)
                        {
                            _processViewXml({
                                ParentId: p.ParentId,
                                DivId: p.DivId,
                                HeaderText: p.HeaderText,
                                $xml: $(xml),
                                parentlevel: p.level,
                                HideRefreshButton: p.HideRefreshButton,
                                HideSearchButton: p.HideSearchButton,
                                ChangePage: p.ChangePage
                            });
                        });
                    } else
                    {
                        _ajaxViewXml(p.DivId, function (xml)
                        {
                            if (p.level == 1)
                            {
                                _storeViewXml(p.DivId, p.HeaderText, xml);
                            }
                            _processViewXml({
                                ParentId: p.ParentId,
                                DivId: p.DivId,
                                HeaderText: p.HeaderText,
                                $xml: $(xml),
                                parentlevel: p.level,
                                HideRefreshButton: p.HideRefreshButton,
                                HideSearchButton: p.HideSearchButton,
                                ChangePage: p.ChangePage
                            });
                        });
                    }
                } else if (p.level == 1)
                {
                    // case 20354 - try cached first
                    _fetchCachedViewXml(rootid, function (xmlstr)
                    {
                        if (xmlstr != '')
                        {
                            $currentViewXml = $(xmlstr);
                            
                            _processViewXml({
                                ParentId: p.ParentId,
                                DivId: p.DivId,
                                HeaderText: p.HeaderText,
                                $xml: $(xmlstr),
                                parentlevel: p.level,
                                HideRefreshButton: p.HideRefreshButton,
                                HideSearchButton: p.HideSearchButton,
                                ChangePage: p.ChangePage
                            });
                        } else if (!amOffline())
                        {
                            _ajaxViewXml(p.DivId, function (xml)
                            {
                                if (p.level == 1)
                                {
                                    _storeViewXml(p.DivId, p.HeaderText, xml);
                                }
                                $currentViewXml = $(xml);
                                
                                _processViewXml({
                                    ParentId: p.ParentId,
                                    DivId: p.DivId,
                                    HeaderText: p.HeaderText,
                                    $xml: $(xml),
                                    parentlevel: p.level,
                                    HideRefreshButton: p.HideRefreshButton,
                                    HideSearchButton: p.HideSearchButton,
                                    ChangePage: p.ChangePage
                                });
                            });
                        }
                    });
                } else  // Level 2 and up
                {
//                    _fetchCachedViewXml(rootid, function (xmlstr)
//                    {
//                        if (xmlstr != '')
//                        {
                        if($currentViewXml != '')
                        {
                            var $thisxmlstr = $currentViewXml.find('#' + p.DivId);
                            _processViewXml({
                                ParentId: p.ParentId,
                                DivId: p.DivId,
                                HeaderText: p.HeaderText,
                                $xml: $thisxmlstr.children('subitems').first(),
                                parentlevel: p.level,
                                HideRefreshButton: p.HideRefreshButton,
                                HideSearchButton: p.HideSearchButton,
                                ChangePage: p.ChangePage
                            });
                        }
                    //});
                }
            } else
            {
                ret = false;
            }
            return ret;
        } // _loadDivContents()

        function _ajaxViewXml(DivId, onsuccess)
        {
            var o = {
                async: false,   // required so that the link will wait for the content before navigating
                url: opts.ViewUrl,
                data: "{ SessionId: '" + SessionId + "', ParentId: '" + DivId + "', ForMobile: '" + true + "'}",
                success: function (data, textStatus, XMLHttpRequest)
                {
                    
                    $auth = $xml.find('AuthenticationStatus');
                    if ($auth.length > 0)
                    {
                        _handleAuthenticationStatus($auth.text());
                    } else
                    {
                        onsuccess(data.d);
                    }
                }
            };
            
            CswAjaxJSON(o);
            
        } // _ajaxViewXml()

        var currenttab;
        var onAfterAddDiv;
        function _processViewXml(params)
        {
            var p = {
                ParentId: '',
                DivId: '',
                HeaderText: '',
                $xml: '',
                parentlevel: '',
                HideRefreshButton: false,
                HideSearchButton: false,
                ChangePage: false
            };
            if (params)
            {
                $.extend(p, params);
            }

            var content = _makeUL();
            currenttab = '';

            onAfterAddDiv = function ($divhtml) { };

            p.$xml.children().each(function ()
            {
                content += _makeListItemFromXml(this, p.DivId, p.parentlevel);
            });
            content += _endUL();

            $divhtml = _addPageDivToBody({
                ParentId: p.ParentId,
                level: p.parentlevel,
                DivId: p.DivId,
                HeaderText: p.HeaderText,
                content: content,
                HideRefreshButton: p.HideRefreshButton,
                HideSearchButton: p.HideSearchButton
            });
            onAfterAddDiv($divhtml);

            // this replaces the link navigation
            if (p.ChangePage)
                $.mobile.changePage($('#' + p.DivId), "slide", false, true);

        } // _processViewXml()

        function _makeListItemFromXml(xmlitem, DivId, parentlevel)
        {
            var $xmlitem = $(xmlitem);
            var id = $xmlitem.attr('id');
            var text = $xmlitem.attr('name');
            var IsDiv = (id != undefined && id != '');
            var PageType = xmlitem.nodeName;

            var nextid = $xmlitem.next().attr('id');
            var previd = $xmlitem.prev().attr('id');

            var lihtml = '';
            switch (PageType)
            {
                case "SEARCHES":
                    // ignore this
                    break;

                case "NODE":
                    lihtml += _makeObjectClassContent($xmlitem);
                    break;

                case "PROP":
                    var tab = $xmlitem.attr('tab');
                    var fieldtype = $xmlitem.attr('fieldtype');
                    var gestalt = $xmlitem.attr('gestalt');
                    if (gestalt == 'NaN') gestalt = '';

                    var currentcnt = $xmlitem.prevAll('[fieldtype="'+fieldtype+'"]').andSelf().length;
                    var siblingcnt = $xmlitem.siblings('[fieldtype="'+fieldtype+'"]').andSelf().length;

                    if (currenttab != tab)
                    {
                        if (currenttab != '')
                            lihtml += _endUL() + _makeUL();
                        lihtml += '<li data-role="list-divider">' + tab + '</li>'
                        currenttab = tab;
                    }

                    switch (fieldtype)
                    {
                        case 'Logical':
                            lihtml += '<li id="' + id + '_li"><a href="#' + id + '">' + text + '</a></li>';

                            var sf_checked = $xmlitem.children('checked').text();
                            var sf_required = $xmlitem.children('required').text();
                            if (sf_checked == undefined) sf_checked = '';
                            if (sf_required == undefined) sf_required = '';

                            lihtml += '<div class="lisubstitute ui-li ui-btn-up-c">';
                            lihtml += _makeLogicalFieldSet(id, '_ans', '_ans2', sf_checked, sf_required);
                            lihtml += '</div>';
                            break;

                        case 'Question':
                            lihtml += '<li id="' + id + '_li"><a href="#' + id + '">' + text + '</a></li>';

                            var sf_answer = $xmlitem.children('answer').text();
                            var sf_allowedanswers = $xmlitem.children('allowedanswers').text();
                            var sf_compliantanswers = $xmlitem.children('compliantanswers').text();
                            var sf_correctiveaction = $xmlitem.children('correctiveaction').text();
                            if (sf_answer == undefined) sf_answer = '';
                            if (sf_allowedanswers == undefined) sf_allowedanswers = '';
                            if (sf_compliantanswers == undefined) sf_compliantanswers = '';
                            if (sf_correctiveaction == undefined) sf_correctiveaction = '';

                            lihtml += '<div class="lisubstitute ui-li ui-btn-up-c">';
                            lihtml += _makeQuestionAnswerFieldSet(DivId, id, '_ans', '_ans2', '_cor', '_li', '_propname', sf_allowedanswers, sf_answer, sf_compliantanswers);
                            lihtml += '</div>';

                            if (sf_answer != '' && (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') < 0 && sf_correctiveaction == '')
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
                            lihtml += '<li id="' + id + '_li">';
                            lihtml += ' <a href="#' + id + '">' + text + '</a>';
                            lihtml += ' <p class="ui-li-aside">' + gestalt + '</p>';
                            lihtml += '</li>';
                            break;
                    }


                    // add a div for editing the property directly
                    var toolbar = '';
                    if (previd != undefined)
                        toolbar += '<a href="#' + previd + '" data-role="button" data-icon="arrow-u" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup" data-back="true">Previous</a>';
                    if (nextid != undefined)
                        toolbar += '<a href="#' + nextid + '" data-role="button" data-icon="arrow-d" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup">Next</a>';
                    if(fieldtype == "Question")
                        toolbar += '&nbsp;' + currentcnt + '&nbsp;of&nbsp;' + siblingcnt;

                    _addPageDivToBody({
                        ParentId: DivId,
                        level: parentlevel,
                        DivId: id,
                        HeaderText: text,
                        toolbar: toolbar,
                        content: _FieldTypeXmlToHtml($xmlitem, DivId)
                    });

                    break;

                default:
                    lihtml += '<li>';
                    if (IsDiv)
                        lihtml += '<a href="#' + id + '">' + text + '</a>';
                    else
                        lihtml += text;
                    lihtml += '</li>';
                    break;
            }
            return lihtml;
        } // _makeListItemFromXml()

        function _makeUL(id)
        {
            var ret = '<ul data-role="listview" ';
            if (id != undefined)
                ret += 'id="' + id + '"';
            ret += '>';
            return ret;
        }

        function _endUL()
        {
            return '</ul>';
        }

        function _makeObjectClassContent($xmlitem)
        {
            var Html = '';

            var id = $xmlitem.attr('id');
            var NodeName = $xmlitem.attr('name');
            var icon = '';
            if ($xmlitem.attr('iconfilename') != '' && $xmlitem.attr('iconfilename') != undefined)
                icon = 'images/icons/' + $xmlitem.attr('iconfilename');
            var ObjectClass = $xmlitem.attr('objectclass');

            switch (ObjectClass)
            {
                case "InspectionDesignClass":
                    var DueDate = $xmlitem.find('prop[ocpname="Due Date"]').attr('gestalt');
                    var Location = $xmlitem.find('prop[ocpname="Location"]').attr('gestalt');
                    var MountPoint = $xmlitem.find('prop[ocpname="Target"]').attr('gestalt');
                    var Status = $xmlitem.find('prop[ocpname="Status"]').attr('gestalt');
                    var UnansweredCnt = 0;
                    $xmlitem.find('prop[fieldtype="Question"]').each(function ()
                    {
                        if ($(this).children('Answer').text() == '')
                        {
                            UnansweredCnt++;
                        }
                    });

                    Html += '<li>';
                    if (icon != '')
                        Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<h3><a href="#' + id + '">' + NodeName + '</a></h3>';
                    Html += '<p>' + Location + '</p>';
                    Html += '<p>' + MountPoint + '</p>';
                    Html += '<p>'+ Status + ', Due: ' + DueDate + '</p>';
                    Html += '<span id="' + id + '_unansweredcnt" class="ui-li-count">' + UnansweredCnt + '</span>';
                    Html += '</li>';
                    break;

                default:
                    Html += '<li>';
                    if (icon != '')
                        Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<a href="#' + id + '">' + NodeName + '</a>';
                    Html += '</li>';
                    break;
            }
            return Html;
        }

        function _extractCDataValue($node)
        {
            // default
            ret = $node.text();

            // for some reason, CDATA fields come through from the webservice like this:
            // <node><!--[CDATA[some text]]--></node>
            var cdataval = $node.html();
            if (cdataval != undefined && cdataval != '')
            {
                var prefix = '<!--[CDATA[';
                var suffix = ']]-->';

                if (cdataval.substr(0, prefix.length) == prefix)
                {
                    ret = cdataval.substr(prefix.length, cdataval.length - prefix.length - suffix.length);
                }
            }
            return ret;
        }

        function _FieldTypeXmlToHtml($xmlitem, ParentId)
        {
            var IdStr = $xmlitem.attr('id');
            var FieldType = $xmlitem.attr('fieldtype');
            var PropName = $xmlitem.attr('name');
            var ReadOnly = ( "true" == $xmlitem.attr('readonly') );

            // Subfield values
            var sf_text = _extractCDataValue($xmlitem.children('text'));
            var sf_value = $xmlitem.children('value').text();
            var sf_href = $xmlitem.children('href').text();
            var sf_checked = $xmlitem.children('checked').text();
            var sf_required = $xmlitem.children('required').text();
            var sf_units = $xmlitem.children('units').text();
            var sf_answer = $xmlitem.children('answer').text();
            var sf_allowedanswers = $xmlitem.children('allowedanswers').text();
            var sf_correctiveaction = $xmlitem.children('correctiveaction').text();
            var sf_comments = $xmlitem.children('comments').text();
            var sf_compliantanswers = $xmlitem.children('compliantanswers').text();
            var sf_options = $xmlitem.children('options').text();

            if (sf_text == undefined) sf_text = '';
            if (sf_value == undefined) sf_value = '';
            if (sf_href == undefined) sf_href = '';
            if (sf_checked == undefined) sf_checked = '';
            if (sf_required == undefined) sf_required = '';
            if (sf_units == undefined) sf_units = '';
            if (sf_answer == undefined) sf_answer = '';
            if (sf_options == undefined) sf_options = '';
            if (sf_allowedanswers == undefined) sf_allowedanswers = '';
            if (sf_correctiveaction == undefined) sf_correctiveaction = '';
            if (sf_comments == undefined) sf_comments = '';
            if (sf_compliantanswers == undefined) sf_compliantanswers = '';

            if (sf_value == 'NaN') sf_value = '';

            var Html = '<div id="' + IdStr + '_propname"';
            if (FieldType == "Question" && !(sf_answer == '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0))
                Html += ' class="OOC"'
            Html += '>' + PropName + '</div><br/>';

            if( !ReadOnly )
            {
                switch (FieldType)
                {
                    case "Date":
                        Html += '<input type="text" name="' + IdStr + '" value="' + sf_value + '" />';
                        break;

                    case "Link":
                        Html += '<a href="' + sf_href + '">' + sf_text + '</a>';
                        break;

                    case "List":
                        Html += '<select name="' + IdStr + '">';
                        var selectedvalue = sf_value;
                        var optionsstr = sf_options;
                        var options = optionsstr.split(',');
                        for (var i = 0; i < options.length; i++)
                        {
                            Html += '<option value="' + options[i] + '"';
                            if (selectedvalue == options[i])
                                Html += ' selected';
                        
                            if(options[i] != '')
                                Html += '>' + options[i] + '</option>';
                            else
                                Html += '>[blank]</option>';
                        }
                        Html += '</select>';
                        break;

                    case "Logical":
                        Html += _makeLogicalFieldSet(IdStr, '_ans2', '_ans', sf_checked, sf_required);
                        break;

                    case "Memo":
                        Html += '<textarea name="' + IdStr + '">' + sf_text + '</textarea>';
                        break;

                    case "Number":
                        Html += '<input type="number" name="' + IdStr + '" value="' + sf_value + '"';
                        // if (Prop.MinValue != Int32.MinValue)
                        //     Html += "min = \"" + Prop.MinValue + "\"";
                        // if (Prop.MaxValue != Int32.MinValue)
                        //     Html += "max = \"" + Prop.MaxValue + "\"";
                        Html += '/>';
                        break;

                    case "Password":
                        Html += string.Empty;
                        break;

                    case "Quantity":
                        Html += '<input type="text" name="' + IdStr + '_qty" value="' + sf_value + '" />';
                        Html += sf_units;
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
                        Html += _makeQuestionAnswerFieldSet(ParentId, IdStr, '_ans2', '_ans', '_cor', '_li', '_propname', sf_allowedanswers, sf_answer, sf_compliantanswers);

                        Html += '<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action"';
                        if (sf_answer == '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0)
                            Html += 'style="display: none"';
                        Html += 'onchange="';
                        Html += 'var $cor = $(this); ';
                        Html += 'if($cor.attr(\'value\') == \'\') { ';
                        Html += '  $(\'#' + IdStr + '_li div\').addClass(\'OOC\'); '
                        Html += '  $(\'#' + IdStr + '_propname\').addClass(\'OOC\'); '
                        Html += '} else {';
                        Html += '  $(\'#' + IdStr + '_li div\').removeClass(\'OOC\'); '
                        Html += '  $(\'#' + IdStr + '_propname\').removeClass(\'OOC\'); '
                        Html += '}';
                        Html += '">';
                        Html += sf_correctiveaction;
                        Html += '</textarea>';

                        Html += '<textarea name="' + IdStr + '_com" placeholder="Comments">';
                        Html += sf_comments
                        Html += '</textarea>';

                        break;

                    case "Static":
                        Html += sf_text;
                        break;

                    case "Text":
                        Html += '<input type="text" name="' + IdStr + '" value="' + sf_text + '" />';
                        break;

                    case "Time":
                        Html += '<input type="text" name="' + IdStr + '" value="' + sf_value + '" />';
                        break;

                    default:
                        Html += $xmlitem.attr('gestalt');
                        break;
                }
            }
            else {
                Html += $xmlitem.attr('gestalt');
            }
            return Html;
        }

        function _FieldTypeHtmlToXml($xmlitem, name, value)
        {
            var IdStr = $xmlitem.attr('id');
            var fieldtype = $xmlitem.attr('fieldtype');
            var propname = $xmlitem.attr('name');

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
                case "Date": if (name == IdStr) $sftomodify = $sf_value; break;
                case "Link": break;
                case "List": if (name == IdStr) $sftomodify = $sf_value; break;
                case "Logical":
                    if (name == IdStr + '_ans' || name == IdStr + '_ans2')
                        $sftomodify = $sf_checked;
                    break;
                case "Memo": if (name == IdStr) $sftomodify = $sf_text; break;
                case "Number": if (name == IdStr) $sftomodify = $sf_value; break;
                case "Password": break;
                case "Quantity": if (name == IdStr + '_qty') $sftomodify = $sf_value; break;
                case "Question":
                    if (name == IdStr + '_com')
                        $sftomodify = $sf_comments;
                    else if (name == IdStr + '_ans' || name == IdStr + '_ans2')
                        $sftomodify = $sf_answer;
                    else if (name == IdStr + '_cor')
                        $sftomodify = $sf_correctiveaction;
                    break;
                case "Static": break;
                case "Text": if (name == IdStr) $sftomodify = $sf_text; break;
                case "Time": if (name == IdStr) $sftomodify = $sf_value; break;
                default: break;
            }
            if ($sftomodify != null)
            {
                $sftomodify.text(value);
                $xmlitem.attr('wasmodified', '1');
            }
        } // _FieldTypeHtmlToXml()

        function _makeLogicalFieldSet(IdStr, Suffix, OtherSuffix, Checked, Required)
        {
            var Html = '<fieldset class="csw_fieldset" data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';

            var answers = ['Null', 'True', 'False'];
            if (Required == "true")
                answers = ['True', 'False'];

            for (var i = 0; i < answers.length; i++)
            {
                var answertext;
                switch (answers[i])
                {
                    case 'Null': answertext = '?'; break;
                    case 'True': answertext = 'Yes'; break;
                    case 'False': answertext = 'No'; break;
                }

                Html += '<input type="radio" name="' + IdStr + Suffix + '" id="' + IdStr + Suffix + '_' + answers[i] + '" value="' + answers[i] + '" ';
                if ((Checked == 'false' && answers[i] == 'False') ||
                    (Checked == 'true' && answers[i] == 'True') ||
                    (Checked == '' && answers[i] == 'Null'))
                    Html += 'checked';
                Html += ' onclick="';

                // case 20307: workaround for a bug with JQuery Mobile Alpha2
                for (var j = 0; j < answers.length; j++)
                {
                    if (answers[j] == answers[i])
                        Html += ' $(\'#' + IdStr + Suffix + '_' + answers[j] + '\').siblings(\'label\').addClass(\'ui-btn-active\');';
                    else
                        Html += ' $(\'#' + IdStr + Suffix + '_' + answers[j] + '\').siblings(\'label\').removeClass(\'ui-btn-active\');';
                }

                Html += ' var $otherradio; ';
                for (var k = 0; k < answers.length; k++)
                {
                    Html += ' $otherradio = $(\'#' + IdStr + OtherSuffix + '_' + answers[k] + '\'); ';
                    if (answers[k] == answers[i])
                    {
                        Html += ' $otherradio.attr(\'checked\', true); ';
                        Html += ' $otherradio.siblings(\'label\').addClass(\'ui-btn-active\'); ';
                    }
                    else
                    {
                        Html += ' $otherradio.attr(\'checked\', false); ';
                        Html += ' $otherradio.siblings(\'label\').removeClass(\'ui-btn-active\'); ';
                    }
                } // for (var k = 0; k < answers.length; k++)
                Html += '" />';
                Html += '<label for="' + IdStr + Suffix + '_' + answers[i] + '">' + answertext + '</label>';
            } // for (var i = 0; i < answers.length; i++)

            Html += '</fieldset>';
            return Html;
        }

        function _makeSafeId(val)
        {
            return val.replace(/'/gi, '');
        }


        function _makeQuestionAnswerFieldSet(ParentId, IdStr, Suffix, OtherSuffix, CorrectiveActionSuffix, LiSuffix, PropNameSuffix, Options, Answer, CompliantAnswers)
        {
            var Html = '<fieldset class="csw_fieldset" id="' + IdStr + '_fieldset" data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
            var answers = Options.split(',');
            for (var i = 0; i < answers.length; i++)
            {
                var answerid = _makeSafeId(answers[i]);
                Html += '<input type="radio" name="' + IdStr + Suffix + '" id="' + IdStr + Suffix + '_' + answerid + '" value="' + answers[i] + '" ';
                if (Answer == answers[i])
                    Html += ' checked';
                Html += ' onclick="';

                // case 20307: workaround for a bug with JQuery Mobile Alpha2
                for (var j = 0; j < answers.length; j++)
                {
                    var otheranswerid = _makeSafeId(answers[j]);
                    if (answers[j] == answers[i])
                        Html += ' $(\'#' + IdStr + Suffix + '_' + otheranswerid + '\').siblings(\'label\').addClass(\'ui-btn-active\');';
                    else
                        Html += ' $(\'#' + IdStr + Suffix + '_' + otheranswerid + '\').siblings(\'label\').removeClass(\'ui-btn-active\');';
                }

                Html += ' var $otherradio; ';
                for (var k = 0; k < answers.length; k++)
                {
                    var yetanotheranswerid = _makeSafeId(answers[k]);
                    Html += ' $otherradio = $(\'#' + IdStr + OtherSuffix + '_' + yetanotheranswerid + '\'); ';
                    if (answers[k] == answers[i])
                    {
                        Html += ' $otherradio.attr(\'checked\', true); ';
                        Html += ' $otherradio.siblings(\'label\').addClass(\'ui-btn-active\'); ';
                    } else
                    {
                        Html += ' $otherradio.attr(\'checked\', false); ';
                        Html += ' $otherradio.siblings(\'label\').removeClass(\'ui-btn-active\'); ';
                    }
                } // for (var k = 0; k < answers.length; k++)

                if ((',' + CompliantAnswers + ',').indexOf(',' + answers[i] + ',') >= 0)
                {
                    Html += ' $(\'#' + IdStr + CorrectiveActionSuffix + '\').css(\'display\', \'none\'); ';
                    Html += ' $(\'#' + IdStr + LiSuffix + ' div\').removeClass(\'OOC\'); ';
                    Html += ' $(\'#' + IdStr + PropNameSuffix + '\').removeClass(\'OOC\'); ';
                }
                else
                {
                    Html += ' var $cor = $(\'#' + IdStr + CorrectiveActionSuffix + '\'); ';
                    Html += ' $cor.css(\'display\', \'\'); ';
                    Html += ' if($cor.attr(\'value\') == \'\') { ';
                    Html += '   $(\'#' + IdStr + LiSuffix + ' div\').addClass(\'OOC\'); ';
                    Html += '   $(\'#' + IdStr + PropNameSuffix + '\').addClass(\'OOC\'); ';
                    Html += ' } else {';
                    Html += '   $(\'#' + IdStr + LiSuffix + ' div\').removeClass(\'OOC\'); ';
                    Html += '   $(\'#' + IdStr + PropNameSuffix + '\').removeClass(\'OOC\'); ';
                    Html += ' } ';
                }
                if (Answer == '')
                {
                    // update unanswered count when this question is answered
                    Html += ' if(! $(\'#' + IdStr + '_fieldset\').attr(\'answered\')) { ';
                    Html += '   var $cntspan = $(\'#' + ParentId + '_unansweredcnt\'); ';
                    Html += '   $cntspan.text(parseInt($cntspan.text()) - 1); ';
                    Html += '   $(\'#' + IdStr + '_fieldset\').attr(\'answered\', \'true\'); ';
                    Html += ' }';
                }
                Html += ' " />';
                Html += '            <label for="' + IdStr + Suffix + '_' + answerid + '">' + answers[i] + '</label>';
            } // for (var i = 0; i < answers.length; i++)
            Html += '</fieldset>';
            return Html;
        } // _makeQuestionAnswerFieldSet()


        function _addPageDivToBody(params)
        {
            var p = {
                ParentId: undefined,
                level: 1,
                DivId: '',       // required
                HeaderText: '',
                toolbar: '',
                content: '',
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


            var divhtml = '<div id="' + p.DivId + '" data-role="page">' +
                          '<div data-role="header" data-theme="' + opts.Theme + '" data-position="fixed">';
            divhtml += '<a href="#' + p.ParentId + '" id="' + p.DivId + '_back" data-back="true" ';
            if (p.backtransition != undefined)
                divhtml += ' data-transition="' + p.backtransition + '" ';
            if (p.ParentId == '' || p.ParentId == undefined)
                divhtml += ' style="visibility: hidden;" ';
            divhtml += ' data-icon="';
            if (p.backicon != undefined)
                divhtml += p.backicon;
            else
                divhtml += 'arrow-l';
            divhtml += '">Back</a>';

            divhtml += '<h1>' + p.HeaderText + '</h1>';
            if (!p.HideSearchButton)
                divhtml += '    <a href="#" id="' + p.DivId + '_searchopen">Search</a>';
            divhtml += '    <div class="toolbar" data-role="controlgroup" data-type="horizontal">' +
                              p.toolbar +
                       '    </div>' +
                       '  </div>' +
                       '  <div data-role="content" data-theme="' + opts.Theme + '">' +
                            p.content +
                       '  </div>' +
                       '  <div data-role="footer" data-theme="' + opts.Theme + '" data-position="fixed">';
            if (!p.HideOnlineButton)
            {
                divhtml += '    <a href="#" id="' + p.DivId + '_gosynchstatus" data-transition="slideup">';
                if (amOffline())
                    divhtml += '    <div class="onlineStatus offline">Offline</div>';
                else
                    divhtml += '    <div class="onlineStatus online">Online</div>';
                divhtml += '    </a>';
            }
            if (!p.HideRefreshButton)
                divhtml += '    <a href="#" id="' + p.DivId + '_refresh" class="refresh">Refresh</a>';
            if (!p.HideLogoutButton)
                divhtml += '    <a href="#" id="' + p.DivId + '_logout">Logout</a>';

            divhtml += '     <a rel="external" href="Login.aspx?redir=n">Full Site</a>';
            if (!p.HideHelpButton)
                divhtml += '    <a href="#" id="' + p.DivId + '_help">Help</a>';
            divhtml += '  </div>' +
                       '</div>';

            var $divhtml = $(divhtml);
            $('body').append($divhtml);

            $divhtml.page();

            _bindEvents(p.DivId, p.ParentId, p.level, $divhtml);

            return $divhtml;

        } // _addPageDivToBody()


        function _getDivHeaderText(DivId)
        {
            return $('#' + DivId).find('div[data-role="header"] h1').text();
        }

        function _getDivParentId(DivId)
        {
            var ret = '';
            var $back = $('#' + DivId).find('div[data-role="header"] #' + DivId + '_back');
            if ($back.length > 0)
                ret = $back.attr('href').substr(1);
            return ret;
        }


        function _bindEvents(DivId, ParentId, level, $div)
        {
            $div.find('#' + DivId + '_searchopen')
                .click(function (eventObj) { onSearchOpen(DivId, eventObj); })
                .end()
                .find('#' + DivId + '_gosynchstatus')
                .click(function (eventObj) { onSynchStatusOpen(DivId, eventObj); })
                .end()
                .find('#' + DivId + '_refresh')
                .click(function (e) { e.stopPropagation(); e.preventDefault(); return onRefresh(DivId, e); })
                .end()
                .find('#' + DivId + '_logout')
                .click(function (e) { e.stopPropagation(); e.preventDefault(); return onLogout(DivId, e); })
                .end()
                .find('#' + DivId + '_back')
                .click(function (eventObj) { return onBack(DivId, ParentId, eventObj); })
                .end()
                .find('#' + DivId + '_help')
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
                .end()
                .find('li a')
                .click(function (e)
                {
                    if (_loadDivContents({
                        ParentId: DivId,
                        level: (level + 1),
                        DivId: $(this).attr('href').substr(1),
                        HeaderText: $(this).text(),
                        ChangePage: true
                    })) { e.stopPropagation(); e.preventDefault(); }
                })
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
            content += '<a id="ss_forcesynch" href="#" data-role="button">Force Synch Now</a>';
            content += '<a id="ss_gooffline" href="#" data-role="button">Go Offline</a>';

            $divhtml = _addPageDivToBody({
                DivId: 'synchstatus',
                HeaderText: 'Synch Status',
                content: content,
                HideSearchButton: true,
                HideRefreshButton: true
            });

            $divhtml.find('#ss_forcesynch')
                    .click(function (eventObj) { _processChanges(false); eventObj.preventDefault(); })
                    .end()
                    .find('#ss_gooffline')
                    .click(function (eventObj) { _toggleOffline(eventObj); });
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
            return ($('#ss_pendingchangecnt').text() == 'Yes');
        }

        // ------------------------------------------------------------------------------------
        // Help Div
        // ------------------------------------------------------------------------------------
        
        function _makeHelpDiv()
        {
            var content = '';
            content += '<p>Help</p>';
            
            $divhtml = _addPageDivToBody({
                DivId: 'help',
                HeaderText: 'Help',
                content: content,
                HideSearchButton: true,
                HideRefreshButton: true
            });

        }

        // ------------------------------------------------------------------------------------
        // Events
        // ------------------------------------------------------------------------------------

        function onLoginSubmit(eventObj)
        {
            // authenticate here
            UserName = $('#login_username').attr('value');
            var Password = $('#login_password').attr('value');
            var AccessId = $('#login_accessid').attr('value');

            if (!amOffline())
            {
                $.ajax({
                    type: 'POST',
                    url: opts.AuthenticateUrl,
                    dataType: "json",
                    contentType: 'application/json; charset=utf-8',
                    data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
                    success: function (data, textStatus, XMLHttpRequest)
                    {
                        var $xml = $(data.d);
                        if ($xml.get(0).nodeName == "ERROR")
                        {
                            _handleAjaxError(XMLHttpRequest, $xml.text(), '');
                        } else
                        {
                            SessionId = $xml.find('SessionId').text();
                            if (SessionId != "")
                            {
                                _cacheSession(SessionId, UserName);
                                reloadViews(true);
                                removeDiv('logindiv');
                            } else
                            {
                                _handleAuthenticationStatus($xml.find('AuthenticationStatus').text());
                            }
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
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
                    _addPageDivToBody({
                        DivId: 'loadingdiv',
                        HeaderText: 'Please wait',
                        content: 'Loading...',
                        HideSearchButton: true,
                        HideOnlineButton: true,
                        HideRefreshButton: true,
                        HideLogoutButton: true,
                        HideHelpButton: true
                    });
                    $.mobile.changePage($('#loadingdiv'), "fade", false, true);
                    setTimeout(function () { continueRefresh(DivId); }, opts.DivRemovalDelay);
                }
            }
            return false;
        }

        function continueRefresh(DivId)
        {
            // remove existing divs
            var NextParentId = '';
            var ThisParentId = DivId;
            while (ThisParentId != '' && ThisParentId.substr(0, 'viewid_'.length) != 'viewid_')
            {
                NextParentId = _getDivParentId(ThisParentId);
                $('div[id*="' + ThisParentId + '"]').remove();
                ThisParentId = NextParentId;
            }

            if (ThisParentId != '')
            {
                var RealDivId = ThisParentId;
                var HeaderText = _getDivHeaderText(RealDivId);

                $('div[id*="' + RealDivId + '"]').remove();

                // fetch new content
                _ajaxViewXml(RealDivId, function (xml)
                {
                    $currentViewXml = $(xml);
                    _updateStoredViewXml(RealDivId, xml, '0');

                    _processViewXml({
                        ParentId: 'viewsdiv',
                        DivId: RealDivId,
                        HeaderText: HeaderText,
                        '$xml': $(xml),
                        parentlevel: 1,
                        HideRefreshButton: false,
                        HideSearchButton: true,
                        ChangePage: true
                    });
                    removeDiv('loadingdiv');
                });
            }
        }

        function onBack(DivId, DestinationId, eventObj)
        {
            if (DivId != 'synchstatus' && DivId.indexOf('prop_') != 0)
            {
                // case 20367 - remove all matching DivId.  Doing it immediately causes bugs.
                setTimeout('$(\'div[id*="' + DivId + '"]\').remove();', opts.DivRemovalDelay);
            }
            return true;
        }


        function onSynchStatusOpen(DivId, eventObj)
        {
            $('#synchstatus_back').attr('href', '#' + DivId);
            $('#synchstatus_back').css('visibility', '');
            $.mobile.changePage($('#synchstatus'), 'slideup');
        }

        function onHelp(DivId, eventObj)
        {
            $.mobile.changePage($('#help'), 'slideup');
        }

        function onPropertyChange(DivId, eventObj)
        {
            var $elm = $(eventObj.target);
            var name = $elm.attr('name');
            var value = $elm.attr('value');

            // update the xml and store it
            if($currentViewXml != '')
            {
//            _fetchCachedViewXml(rootid, function (xmlstr)
//            {
//                if (xmlstr != '')
//                {
                    var $divxml = $currentViewXml.find('#' + DivId);
                    $divxml.andSelf().find('prop').each(function ()
                    {
                        _FieldTypeHtmlToXml($(this), name, value);
                    });

                    // Strictly speaking, this is not a valid use of html() since we're operating on xml.  
                    // However, it appears to work, for now.
                    _updateStoredViewXml(rootid, $currentViewXml.wrap('<wrapper />').parent().html(), '1');

                    _resetPendingChanges(true, false);
                }
            //});

        } // onPropertyChange()

        function onSearchOpen(DivId, eventObj)
        {
            var searchprop = $('#' + DivId + '_searchprop').attr('value');
            var searchfor = $('#' + DivId + '_searchfor').attr('value');
            _fetchCachedViewXml(rootid, function (xmlstr)
            {
                if (xmlstr != '')
                {
                    var $xmlstr = $(xmlstr);
                    var Html = '<select id="' + DivId + '_searchprop" name="' + DivId + '_searchprop">';

                    $xmlstr.closest('result')
                    .find('searches')
                    .children()
                    .each(function ()
                    {
                        var $search = $(this);
                        Html += '<option value="' + $search.attr('id') + '">' + $search.attr('name') + '</option>';
                    });

                    Html += '</select>' +
                        '<input type="search" name="' + DivId + '_searchfor" id="' + DivId + '_searchfor" value="" placeholder="Search" />' +
                        '<input type="button" id="' + DivId + '_searchgo" data-inline="true" value="Go" /> ' +
                        '<div id="' + DivId + '_searchresults"></div>';


                    _addPageDivToBody({
                        ParentId: DivId,
                        DivId: DivId + '_searchdiv',
                        HeaderText: 'Search',
                        content: Html,
                        HideSearchButton: true,
                        HideRefreshButton: true,
                        backicon: 'arrow-u'
                    });

                    $('#' + DivId + '_searchgo').click(function (eventObj) { onSearchSubmit(DivId, eventObj); });

                    $.mobile.changePage($('#' + DivId + '_searchdiv'), "slideup", false, true);
                }
            });
        }

        function onSearchSubmit(DivId, eventObj)
        {
            var searchprop = $('#' + DivId + '_searchprop').attr('value');
            var searchfor = $('#' + DivId + '_searchfor').attr('value');
            _fetchCachedViewXml(rootid, function (xmlstr)
            {
                if (xmlstr != '')
                {
                    var $xmlstr = $(xmlstr);
                    var content = _makeUL(DivId + '_searchresultslist');

                    var hitcount = 0;
                    $xmlstr.find('node').each(function ()
                    {
                        var $node = $(this);
                        if ($node.attr(searchprop) != undefined)
                        {
                            if ($node.attr(searchprop).toLowerCase().indexOf(searchfor.toLowerCase()) >= 0)
                            {
                                hitcount++;
                                content += _makeListItemFromXml(this, DivId, 1, false);
                            }
                        }
                    });
                    if (hitcount == 0)
                    {
                        content += "<li>No Results</li>";
                    }

                    content += _endUL();

                    var $srdiv = $('#' + DivId + '_searchresults');
                    $srdiv.children().remove();
                    $srdiv.append(content);
                    $('#' + DivId + '_searchresultslist').listview();

                    _bindEvents(DivId + '_searchdiv', DivId, 1, $srdiv);
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
                console.log("database is not opened");
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
                    if (OnSuccess != null)
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
            console.log('Database Error: ' + error.message + ' (Code ' + error.code + ')');
            return true;
        }

        function writeConfigVar(varname, varval, onsuccess)
        {
            _DoSql("select varval from configvars where varname=?;",
                   [varname],
                   function (transaction, result)
                   {
                       if (0 == result.rows.length)
                       {
                           _DoSql("insert into configvars (varname, varval) values ( ?, ? );",
                                  [varname, varval],
                                  function ()
                                  {
                                      if (onsuccess != undefined)
                                          onsuccess();
                                  });
                       } else
                       {
                           _DoSql("update configvars set varval = ? where varname = ?",
                                  [varval, varname],
                                  function ()
                                  {
                                      if (onsuccess != undefined)
                                          onsuccess();
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
                    if (onsuccess != undefined)
                        onsuccess();
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


        function _storeViewXml(rootid, rootname, viewxml)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('INSERT INTO views (rootid, rootname, viewxml, wasmodified) VALUES (?, ?, ?, 0);',
                       [rootid, rootname, viewxml]);
            }
        }

        function _updateStoredViewXml(rootid, viewxml, wasmodified)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('UPDATE views SET wasmodified = ?, viewxml = ? WHERE rootid = ?;',
                       [wasmodified, viewxml, rootid]);
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
                           onSuccess(row.rootid, row.viewxml);
                       } else
                       {
                           _resetPendingChanges(false, true);
                           onSuccess('', '');
                       }
                   });
        }

        function _fetchCachedViewXml(rootid, onsuccess)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('SELECT viewxml FROM views WHERE rootid = ? ORDER BY id DESC;',
                       [rootid],
                       function (transaction, result)
                       {
                           if (result.rows.length > 0)
                           {
                               var row = result.rows.item(0);
                               onsuccess(row.viewxml);
                           } else
                           {
                               onsuccess('');
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
                       onsuccess("<result>" + ret + "</result>");
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
                url = opts.ConnectTestRandomFailUrl;

            $.ajax({
                type: 'POST',
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: "{}",
                success: function (data, textStatus, XMLHttpRequest)
                {
                    var $xml = $(data.d);
                    if ($xml.get(0).nodeName == "ERROR")
                    {
                        _handleAjaxError(XMLHttpRequest, $xml.text(), '');
                        if (onFailure != undefined)
                            onFailure();
                        _waitForData();
                    } else
                    {
                        setOnline();
                        _processChanges(true);
                        if (onSuccess != undefined)
                            onSuccess();
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                    if (onFailure != undefined)
                        onFailure();
                    _waitForData();
                }
            });

        } //_handleDataCheckTimer()

        function _processChanges(perpetuateTimer)
        {
            if (SessionId != '' && SessionId != undefined)
            {
                _getModifiedView(function (rootid, viewxml)
                {
                    if (rootid != '' && viewxml != '')
                    {
                        $.ajax({
                            type: 'POST',
                            url: opts.UpdateUrl,
                            dataType: "json",
                            contentType: 'application/json; charset=utf-8',
                            //data: "{ParentId: '" + rootid + "', UpdatedViewXml: '" + viewxml + "'}",
                            data: "{ SessionId: '" + SessionId + "', ParentId: '" + rootid + "', UpdatedViewXml: '" + viewxml.replace(/'/gi, '\\\'') + "'}",
                            success: function (data, textStatus, XMLHttpRequest)
                            {
                                var $xml = $(data.d);
                                if ($xml.get(0).nodeName == "ERROR")
                                {
                                    _handleAjaxError(XMLHttpRequest, $xml.text(), '');
                                    if (perpetuateTimer)
                                        _waitForData();
                                } else
                                {
                                    $auth = $xml.find('AuthenticationStatus');
                                    if ($auth.length > 0)
                                    {
                                        _handleAuthenticationStatus($auth.text());
                                        if (perpetuateTimer)
                                            _waitForData();
                                    } else
                                    {
                                        _updateStoredViewXml(rootid, data.d, '0');
                                        if (perpetuateTimer)
                                            _waitForData();
                                    }
                                }
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown)
                            {
                                _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                                if (perpetuateTimer)
                                    _waitForData();
                            }
                        });
                    }
                    else
                    {
                        if (perpetuateTimer)
                            _waitForData();
                    }
                }); // _getModifiedView();
            } else
            {
                if (perpetuateTimer)
                    _waitForData();
            } // if(SessionId != '') 
        } //_processChanges()


        function _handleAuthenticationStatus(status)
        {
            alert(status);
            Logout();
        }

        function _handleAjaxError(XMLHttpRequest, textStatus, errorThrown)
        {
            ErrorMessage = "Error: " + textStatus;
            if (null != errorThrown)
            {
                ErrorMessage += "; Exception: " + errorThrown.toString()
            }
            console.log(ErrorMessage);
            setOffline();
        }

        // For proper chaining support
        return this;
    };
   
}) ( jQuery );



// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj)
{
    var str;
    for (var x in obj)
    {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    if (popup != null)
        popup.document.write(str);
    else
        console.log("iterate() error: No popup!");
}
