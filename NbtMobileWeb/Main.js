(function ($)
{
    $.fn.CswMobile = function (options)
    {

        var opts = {
            DBShortName: 'main.html',
            DBVersion: '1.0',
            DBDisplayName: 'main.html',
            DBMaxSize: 65536,
            ViewUrl: '/NbtMobileWeb/wsNBT.asmx/RunView',
            ConnectTestUrl: '/NbtMobileWeb/wsNBT.asmx/ConnectTest',
            UpdateUrl: '/NbtMobileWeb/wsNBT.asmx/UpdateProperties',
            MainPageUrl: '/NbtMobileWeb/Main.html',
            Theme: 'a',
            PollingInterval: 5000,
            DivRemovalTime: 2000
        };

        if (options)
        {
            $.extend(opts, options);
        }

        var rootid;
        var db;

        _initDB(true, _waitForData);
        reloadViews();
        _makeSynchStatusDiv();


        function reloadViews()
        {
            $('#viewsdiv').remove();
            _loadDivContents('', 0, 'viewsdiv', 'Views', true);
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

                reloadViews();
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

                reloadViews();
            }
        }
        function amOffline()
        {
            return $('.onlineStatus').hasClass('offline');
        }


        // ------------------------------------------------------------------------------------
        // List items fetching
        // ------------------------------------------------------------------------------------

        function _loadDivContents(ParentId, level, DivId, HeaderText, IsFirst)
        {
            var ret = true;

            if (level == 1)
                rootid = DivId;

            if ($('#' + DivId).length == 0)
            {
                if (level == 0)
                {
                    if (amOffline())
                    {
                        _fetchCachedRootXml(function (xml)
                        {
                            _processViewXml(ParentId, DivId, HeaderText, $(xml).children(), level, IsFirst);
                        });
                    } else
                    {
                        _ajaxViewXml(DivId, function (xml)
                        {
                            if (level == 1)
                            {
                                _storeViewXml(DivId, HeaderText, xml);
                            }
                            _processViewXml(ParentId, DivId, HeaderText, $(xml).children(), level, IsFirst);
                        });
                    }
                } else if (level == 1)
                {
                    // case 20354 - try cached first
                    _fetchCachedViewXml(rootid, function (xmlstr)
                    {
                        if (xmlstr != '')
                        {
                            _processViewXml(ParentId, DivId, HeaderText, $(xmlstr).children(), level, IsFirst);
                        } else if (!amOffline())
                        {
                            _ajaxViewXml(DivId, function (xml)
                            {
                                if (level == 1)
                                {
                                    _storeViewXml(DivId, HeaderText, xml);
                                }
                                _processViewXml(ParentId, DivId, HeaderText, $(xml).children(), level, IsFirst);
                            });
                        }
                    });
                } else  // Level 2 and up
                {
                    _fetchCachedViewXml(rootid, function (xmlstr)
                    {
                        if (xmlstr != '')
                        {
                            var $thisxmlstr = $(xmlstr).find('#' + DivId);
                            _processViewXml(ParentId, DivId, HeaderText, $thisxmlstr.children('subitems').first().children(), level, IsFirst);
                        }
                    });
                }

                // prevent link navigation!
                // _processViewXml() above will do the page transition for us when the div is ready
                ret = false;
            }
            return ret;
        }

        function _ajaxViewXml(DivId, onsuccess)
        {
            $.ajax({
                async: false,   // required so that the link will wait for the content before navigating
                type: 'POST',
                url: opts.ViewUrl,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: "{ ParentId: '" + DivId + "' }",
                success: function (data, textStatus, XMLHttpRequest)
                {
                    var $xml = $(data.d);
                    if ($xml.get(0).nodeName == "ERROR")
                    {
                        _handleAjaxError(XMLHttpRequest, $xml.text(), '');
                    } else
                    {
                        onsuccess(data.d);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                }
            });
        }

        var currenttab;
        var onAfterAddDiv;
        function _processViewXml(ParentId, DivId, HeaderText, $xml, parentlevel, IsFirst)
        {
            var content = _makeUL();
            currenttab = '';

            onAfterAddDiv = function ($divhtml) { };

            $xml.each(function ()
            {
                content += _makeListItemFromXml($xml, this, DivId, parentlevel, IsFirst);
            });
            content += _endUL();

            $divhtml = _addPageDivToBody(ParentId, parentlevel, DivId, HeaderText, '', content, IsFirst);

            onAfterAddDiv($divhtml);

            // this replaces the link navigation
            if (!IsFirst)
                $.mobile.changePage($('#' + DivId), "slide", false, true);

        } // _processViewXml()

        function _makeListItemFromXml($xml, xmlitem, DivId, parentlevel, IsFirst)
        {
            var $xmlitem = $(xmlitem);
            var id = $xmlitem.attr('id');
            var text = $xmlitem.attr('name');
            var IsDiv = (id != undefined && id != '');
            var PageType = xmlitem.nodeName;

            var nextid = $xmlitem.next().attr('id');
            var previd = $xmlitem.prev().attr('id');

            var currentcnt = $xmlitem.prevAll().andSelf().length;
            var siblingcnt = $xmlitem.siblings().andSelf().length;

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
                    if (currenttab != tab)
                    {
                        if (currenttab != undefined)
                            lihtml += _endUL() + _makeUL();
                        lihtml += '<li data-role="list-divider">' + tab + '</li>'
                        currenttab = tab;
                    }

                    lihtml += '<li id="' + id + '_li">';
                    lihtml += '<a href="#' + id + '">' + text + '</a>';
                    lihtml += '</li>';

                    switch (fieldtype)
                    {
                        case 'Logical':
                            var sf_checked = $xmlitem.children('checked').text();
                            var sf_required = $xmlitem.children('required').text();
                            if (sf_checked == undefined) sf_checked = '';
                            if (sf_required == undefined) sf_required = '';

                            lihtml += _makeLogicalFieldSet(id, '_ans', '_ans2', sf_checked, sf_required);
                            break;

                        case 'Question':
                            var sf_answer = $xmlitem.children('answer').text();
                            var sf_options = $xmlitem.children('allowedanswers').text();
                            var sf_compliantanswers = $xmlitem.children('compliantanswers').text();
                            var sf_correctiveaction = $xmlitem.children('correctiveaction').text();
                            if (sf_answer == undefined) sf_answer = '';
                            if (sf_options == undefined) sf_options = '';
                            if (sf_compliantanswers == undefined) sf_compliantanswers = '';
                            if (sf_correctiveaction == undefined) sf_correctiveaction = '';

                            lihtml += _makeQuestionAnswerFieldSet(id, '_ans', '_ans2', '_cor', '_li', sf_options, sf_answer, sf_compliantanswers);

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
                            break;
                    }


                    // add a div for editing the property directly
                    var toolbar = '';
                    if (previd != undefined)
                        toolbar += '<a href="#' + previd + '" data-role="button" data-icon="arrow-u" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup" data-back="true">Previous</a>';
                    if (nextid != undefined)
                        toolbar += '<a href="#' + nextid + '" data-role="button" data-icon="arrow-d" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup">Next</a>';
                    toolbar += '&nbsp;' + currentcnt + '&nbsp;of&nbsp;' + siblingcnt;
                    _addPageDivToBody(DivId, parentlevel, id, text, toolbar, _FieldTypeXmlToHtml($xmlitem), IsFirst);

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
            var ret = '<ul data-role="listview" data-inset="true" ';
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
            var icon = 'images/icons/' + $xmlitem.attr('iconfilename');
            var ObjectClass = $xmlitem.attr('objectclass');

            switch (ObjectClass)
            {
                case "InspectionDesignClass":
                    var DueDate = $xmlitem.find('prop[ocpname="Due Date"]').attr('gestalt');
                    var Target = $xmlitem.find('prop[ocpname="Target"]').attr('gestalt');
                    var UnansweredCnt = 0;
                    $xmlitem.find('prop[fieldtype="Question"]').each(function ()
                    {
                        if ($(this).children('Answer').text() == '')
                        {
                            UnansweredCnt++;
                        }
                    });

                    Html += '<li>';
                    Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<h3><a href="#' + id + '">' + NodeName + '</a></h3>';
                    Html += '<p>' + Target + '</p>';
                    Html += '<p>Due: ' + DueDate + '</p>';
                    Html += '<span class="ui-li-count">' + UnansweredCnt + '</span>';
                    Html += '</li>';
                    break;

                default:
                    Html += '<li>';
                    Html += '<img src="' + icon + '" class="ui-li-icon"/>';
                    Html += '<a href="#' + id + '">' + NodeName + '</a>';
                    Html += '</li>';
                    break;
            }
            return Html;
        }

        function _FieldTypeXmlToHtml($xmlitem)
        {
            var IdStr = $xmlitem.attr('id');
            var FieldType = $xmlitem.attr('fieldtype');
            var PropName = $xmlitem.attr('name');

            var Html = PropName + '<br/>';

            // Subfield values
            var sf_text = $xmlitem.children('text').text();
            var sf_value = $xmlitem.children('value').text();
            var sf_href = $xmlitem.children('href').text();
            var sf_options = $xmlitem.children('options').text();
            var sf_checked = $xmlitem.children('checked').text();
            var sf_required = $xmlitem.children('required').text();
            var sf_units = $xmlitem.children('units').text();
            var sf_answer = $xmlitem.children('answer').text();
            var sf_options = $xmlitem.children('allowedanswers').text();
            var sf_correctiveaction = $xmlitem.children('correctiveaction').text();
            var sf_comments = $xmlitem.children('comments').text();
            var sf_compliantanswers = $xmlitem.children('compliantanswers').text();

            if (sf_text == undefined) sf_text = '';
            if (sf_value == undefined) sf_value = '';
            if (sf_href == undefined) sf_href = '';
            if (sf_options == undefined) sf_options = '';
            if (sf_checked == undefined) sf_checked = '';
            if (sf_required == undefined) sf_required = '';
            if (sf_units == undefined) sf_units = '';
            if (sf_answer == undefined) sf_answer = '';
            if (sf_options == undefined) sf_options = '';
            if (sf_correctiveaction == undefined) sf_correctiveaction = '';
            if (sf_comments == undefined) sf_comments = '';
            if (sf_compliantanswers == undefined) sf_compliantanswers = '';

            switch (FieldType)
            {
                case "Date":
                    Html += '<input type="date" name="' + IdStr + '" value="' + sf_value + '" />';
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
                        Html += '>' + options[i] + "</option>";
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
                    Html += _makeQuestionAnswerFieldSet(IdStr, '_ans2', '_ans', '_cor', '_li', sf_options, sf_answer, sf_compliantanswers);

                    Html += '<textarea name="' + IdStr + '_com" placeholder="Comments">';
                    Html += sf_comments
                    Html += '</textarea>';

                    Html += '<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action"';
                    if (sf_answer == '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0)
                        Html += 'style="display: none"';
                    Html += 'onchange="';
                    Html += 'var $cor = $(this); ';
                    Html += 'if($cor.attr(\'value\') == \'\') { ';
                    Html += '  $(\'#' + IdStr + '_li div\').addClass(\'OOC\'); '
                    Html += '} else {';
                    Html += '  $(\'#' + IdStr + '_li div\').removeClass(\'OOC\'); '
                    Html += '}';
                    Html += '">';
                    Html += sf_correctiveaction;
                    Html += '</textarea>';
                    break;

                case "Static":
                    Html += sf_text;
                    break;

                case "Text":
                    Html += '<input type="text" name="' + IdStr + '" value="' + sf_text + '" />';
                    break;

                case "Time":
                    Html += '<input type="time" name="' + IdStr + '" value="' + sf_value + '" />';
                    break;

                default:
                    Html += $xmlitem.attr('gestalt');
                    break;
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
                case "Quantity": if (name == IdStr) $sftomodify = $sf_value; break;
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
            var Html = '<fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';

            var answers = ['Blank', 'Yes', 'No'];
            if (Required == "true")
                answers = ['Yes', 'No'];

            for (var i = 0; i < answers.length; i++)
            {
                var answertext = answers[i];
                if (answertext == 'Blank') answertext = '?';

                Html += '<input type="radio" name="' + IdStr + Suffix + '" id="' + IdStr + Suffix + '_' + answers[i] + '" value="' + answertext + '" ';
                if ((Checked == 'false' && answers[i] == 'No') ||
                    (Checked == 'true' && answers[i] == 'Yes') ||
                    (Checked == '' && answers[i] == 'Blank'))
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

        function _makeQuestionAnswerFieldSet(IdStr, Suffix, OtherSuffix, CorrectiveActionSuffix, LiSuffix, Options, Answer, CompliantAnswers)
        {
            var Html = '<fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
            var answers = Options.split(',');
            for (var i = 0; i < answers.length; i++)
            {
                Html += '<input type="radio" name="' + IdStr + Suffix + '" id="' + IdStr + Suffix + '_' + answers[i] + '" value="' + answers[i] + '" ';
                if (Answer == answers[i])
                    Html += ' checked';
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
                }
                else
                {
                    Html += 'var $cor = $(\'#' + IdStr + CorrectiveActionSuffix + '\'); ';
                    Html += '$cor.css(\'display\', \'\'); ';
                    Html += 'if($cor.attr(\'value\') == \'\') { ';
                    Html += '  $(\'#' + IdStr + LiSuffix + ' div\').addClass(\'OOC\'); ';
                    Html += '} else {';
                    Html += '  $(\'#' + IdStr + LiSuffix + ' div\').removeClass(\'OOC\'); ';
                    Html += '}';
                }
                Html += ' " />';
                Html += '            <label for="' + IdStr + Suffix + '_' + answers[i] + '">' + answers[i] + '</label>';
            } // for (var i = 0; i < answers.length; i++)
            Html += '</fieldset>';
            return Html;
        } // _makeQuestionAnswerFieldSet()

        function _addPageDivToBody(ParentId, level, DivId, HeaderText, toolbar, content, IsFirst, HideSearchButton, backicon, backtransition)
        {
            var divhtml = '<div id="' + DivId + '" data-role="page">' +
                          '<div data-role="header" data-theme="' + opts.Theme + '">';
            divhtml += '<a href="#' + ParentId + '" id="' + DivId + '_back" data-back="true" ';
            if (backtransition != undefined)
                divhtml += ' data-transition="' + backtransition + '" ';
            if (ParentId == '' || ParentId == undefined)
                divhtml += ' style="visibility: hidden;" ';
            divhtml += ' data-icon="';
            if (backicon != undefined)
                divhtml += backicon;
            else
                divhtml += 'arrow-l';
            divhtml += '">Back</a>';

            divhtml += '<h1>' + HeaderText + '</h1>';
            if (!IsFirst && !HideSearchButton)
            {
                divhtml += '    <a href="#" id="' + DivId + '_searchopen">Search</a>';
            }
            divhtml += '    <div class="toolbar" data-role="controlgroup" data-type="horizontal">' +
                              toolbar +
                       '    </div>' +
                       '  </div>' +
                       '  <div data-role="content" data-theme="' + opts.Theme + '">' +
                            content +
                       '  </div>' +
                       '  <div data-role="footer" data-theme="' + opts.Theme + '"><a href="#" id="' + DivId + '_gosynchstatus" data-transition="slideup">';
            if (amOffline())
                divhtml += '    <div class="onlineStatus offline">Offline</div>';
            else
                divhtml += '    <div class="onlineStatus online">Online</div>';
            divhtml += '  </a></div>' +
                       '</div>';

            var $divhtml = $(divhtml);
            if (IsFirst)
                $('body').prepend($divhtml);
            else
                $('body').append($divhtml);

            $divhtml.page();

            _bindEvents(DivId, ParentId, level, $divhtml);

            return $divhtml;

        } // _addPageDivToBody()

        function _bindEvents(DivId, ParentId, level, $div)
        {
            $div.find('#' + DivId + '_searchopen')
                .click(function (eventObj) { onSearchOpen(DivId, eventObj); })
                .end()
                .find('#' + DivId + '_gosynchstatus')
                .click(function (eventObj) { onSynchStatusOpen(DivId, eventObj); })
                .end()
                .find('#' + DivId + '_back')
                .click(function (eventObj) { return onBack(DivId, ParentId, eventObj); })
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
                .click(function (e) { return _loadDivContents(DivId, (level + 1), $(this).attr('href').substr(1), $(this).text(), false); })
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
            $divhtml = _addPageDivToBody('', 1, 'synchstatus', 'Synch Status', '', content, false, true);
            $divhtml.find('#ss_forcesynch')
                    .click(function (eventObj) { _processChanges(false); return false; })
                    .end()
                    .find('#ss_gooffline')
                    .click(function (eventObj) { _toggleOffline(); return false; });
        }

        function _toggleOffline()
        {
            if (amOffline())
            {
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
            $('#ss_pendingchangecnt').text(val);
            if (setlastsynchnow)
            {
                var d = new Date();
                $('#ss_lastsynch').text(d.toLocaleDateString() + ' ' + d.toLocaleTimeString());
            }
        }


        // ------------------------------------------------------------------------------------
        // Events
        // ------------------------------------------------------------------------------------

        function onBack(DivId, DestinationId, eventObj)
        {
            if (DivId != 'synchstatus' && DivId.indexOf('prop_') != 0)
            {
                // case 20367 - remove DivId.  Doing it immediately causes bugs.
                setTimeout('$(\'div[id*="' + DivId + '"]\').remove();', opts.DivRemovalTime);
            }
            return true;
        }


        function onSynchStatusOpen(DivId, eventObj)
        {
            $('#synchstatus_back').attr('href', '#' + DivId);
            $('#synchstatus_back').css('visibility', '');
            $.mobile.changePage($('#synchstatus'), 'slideup');
        }

        function onPropertyChange(DivId, eventObj)
        {
            var $elm = $(eventObj.srcElement);
            var name = $elm.attr('name');
            var value = $elm.attr('value');

            // update the xml and store it
            _fetchCachedViewXml(rootid, function (xmlstr)
            {
                if (xmlstr != '')
                {
                    var $xmlstr = $(xmlstr);
                    var $divxml = $xmlstr.find('#' + DivId);
                    $divxml.andSelf().find('prop').each(function ()
                    {
                        _FieldTypeHtmlToXml($(this), name, value);
                    });

                    // Strictly speaking, this is not a valid use of html() since we're operating on xml.  
                    // However, it appears to work, for now.
                    _updateStoredViewXml(rootid, $xmlstr.wrap('<wrapper />').parent().html(), '1');

                    _resetPendingChanges('Yes', false);
                }
            });

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


                    _addPageDivToBody(DivId, 1, DivId + '_searchdiv', 'Search', '', Html, false, true, 'arrow-u');

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
                            if ($node.attr(searchprop).toLowerCase().indexOf(searchfor.toLowerCase()) > 0)
                            {
                                hitcount++;
                                content += _makeListItemFromXml($xmlstr, this, DivId, 1, false);
                            }
                        }
                    });
                    if (hitcount.length == 0)
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
                _DoSql('DROP TABLE IF EXISTS views; ', null, function () { _createDb(OnSuccess); });
            } else
            {
                _createDb(OnSuccess);
            }
            //console.log("tables created");
        } //_initDb()

        function _createDb(OnSuccess)
        {
            _DoSql('CREATE TABLE IF NOT EXISTS views ' +
                    '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
                    '   rootid TEXT NOT NULL, ' +
                    '   rootname TEXT NOT NULL, ' +
                    '   viewxml TEXT, ' +
                    '   wasmodified INTEGER );',
                    null,
                    OnSuccess
                    );

        } //_createDb() 

        function _errorHandler(transaction, error)
        {
            console.log('Database Error: ' + error.message + ' (Code ' + error.code + ')');
            return true;
        }



        // ------------------------------------------------------------------------------------
        // Persistance functions
        // ------------------------------------------------------------------------------------


        function _storeViewXml(rootid, rootname, viewxml)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('INSERT INTO views (rootid, rootname, viewxml, wasmodified) VALUES (?, ?, ?, 0);',
                       [rootid, rootname, viewxml],
                       function () { }
                       );
            }
        }

        function _updateStoredViewXml(rootid, viewxml, wasmodified)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('UPDATE views SET wasmodified = ?, viewxml = ? WHERE rootid = ?;',
                       [wasmodified, viewxml, rootid],
                       function () { }
                       );
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
                           _resetPendingChanges('Yes', true);
                           var row = result.rows.item(0);
                           onSuccess(row.rootid, row.viewxml);
                       } else
                       {
                           _resetPendingChanges('No', true);
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

        function _handleDataCheckTimer()
        {
            $.ajax({
                type: 'POST',
                url: opts.ConnectTestUrl,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: "{}",
                success: function (data, textStatus, XMLHttpRequest)
                {
                    var $xml = $(data.d);
                    if ($xml.get(0).nodeName == "ERROR")
                    {
                        _handleAjaxError(XMLHttpRequest, $xml.text(), '');
                        _waitForData();
                    } else
                    {
                        setOnline();
                        _processChanges(true);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                    _waitForData();
                }
            });

        } //_handleDataCheckTimer()

        function _processChanges(perpetuateTimer)
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
                        data: "{ParentId: '" + rootid + "', UpdatedViewXml: '" + viewxml + "'}",
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
                                _updateStoredViewXml(rootid, data.d, '0');
                                //console.log("UpdateProperties succeeded");
                                if (perpetuateTimer)
                                    _waitForData();
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
            });

        } //_processChanges()



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
})(jQuery);



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
