
(function ($)
{

    $.fn.CswMobile = function (options)
    {

        var opts = {
            DBShortName: 'main.html',
            DBVersion: '1.0',
            DBDisplayName: 'main.html',
            DBMaxSize: 65536,
            WebServiceUrl: '/NbtMobileWeb/wsView.asmx/Run',
            MainPageUrl: '/NbtMobileWeb/Main.html',
            Theme: 'a'
        };

        if (options)
        {
            $.extend(opts, options);
        }

        var rootid;
        var db;

        _initDB(true);
        _loadDivContents('', 0, 'viewsdiv', 'Views', true);
        _makeSearchDiv();

        // ------------------------------------------------------------------------------------
        // Offline indicator
        // ------------------------------------------------------------------------------------

        function toggleOffline()
        {
            // Reset all indicators
            $('.offlineIndicator').toggleClass('online')
                .toggleClass('offline');
            // Clear non-cached root contents
            //        $('#TopDiv').children('div[data-role="content"]').children('ul').children().remove();
            //        _loadDivContents(0, $('#TopDiv'));
        }

        function getCurrentOfflineIndicatorCssClass()
        {
            if ($('.offlineIndicator').hasClass('offline'))
                return 'offline';
            else
                return 'online';

        }
        function amOffline()
        {
            return $('.offlineIndicator').hasClass('offline');
        }


        // ------------------------------------------------------------------------------------
        // List items fetching
        // ------------------------------------------------------------------------------------

        function _loadDivContents(ParentId, level, DivId, HeaderText, IsFirst)
        {
            if (level == 1)
                rootid = DivId;

            if ($('#' + DivId).length == 0)
            {
                if (level <= 1)
                {
                    if (amOffline())
                    {
                        if (level == 0)
                        {
                            _fetchCachedRootXml(function (xml)
                            {
                                _processSubLevelXml(ParentId, DivId, HeaderText, $(xml).children(), level, IsFirst);
                            });
                        } else
                        {
                            _fetchCachedSubLevelXml(DivId, function (xmlstr)
                            {
                                var $thisxmlstr = $(xmlstr).find('#' + DivId);
                                _processSubLevelXml(ParentId, DivId, HeaderText, $thisxmlstr.children('subitems').first().children(), level, IsFirst);
                            });
                        }
                    } else
                    {
                        $.ajax({
                            async: false,   // required so that the link will wait for the content before navigating
                            type: 'POST',
                            url: opts.WebServiceUrl,
                            dataType: "json",
                            contentType: 'application/json; charset=utf-8',
                            data: "{ ParentId: '" + DivId + "' }",
                            success: function (data, textStatus, XMLHttpRequest)
                            {
                                var $xml = $(data.d);
                                var $firstchild = $xml.children().first();
                                if ($firstchild.get(0).nodeName == "ERROR")
                                {
                                    alert("An Error Occurred: " + $firstchild.text());
                                } else
                                {
                                    if (level == 1)
                                    {
                                        _storeSubLevelXml(DivId, HeaderText, '', data.d);
                                    }
                                    _processSubLevelXml(ParentId, DivId, HeaderText, $xml.children(), level, IsFirst);
                                }
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown)
                            {
                                alert("An Error Occurred: " + errorThrown);
                            }
                        });
                    }
                } else
                {
                    _fetchCachedSubLevelXml(rootid, function (xmlstr)
                    {
                        var $thisxmlstr = $(xmlstr).find('#' + DivId);
                        _processSubLevelXml(ParentId, DivId, HeaderText, $thisxmlstr.children('subitems').first().children(), level, IsFirst);
                    });
                }
            }
        }



        function _processSubLevelXml(ParentId, DivId, HeaderText, $xml, parentlevel, IsFirst)
        {
            var currenttab;
            var content = _makeUL();

            $xml.each(function ()
            {
                var $xmlitem = $(this);
                var id = $xmlitem.attr('id');
                var text = $xmlitem.attr('name');
                var IsDiv = (id != undefined && id != '');
                var PageType = this.nodeName;

                var nextid = $xmlitem.next().attr('id');
                var previd = $xmlitem.prev().attr('id');

                var currentcnt = $xmlitem.prevAll().andSelf().length;
                var siblingcnt = $xmlitem.siblings().andSelf().length;

                var lihtml = '';
                if (PageType == "PROP")
                {
                    var tab = $xmlitem.attr('tab');
                    var fieldtype = $xmlitem.attr('fieldtype');
                    if (currenttab != tab)
                    {
                        if (currenttab != undefined)
                            lihtml += '</ul>' + _makeUL();
                        lihtml += '<li data-role="list-divider">' + tab + '</li>'
                        currenttab = tab;
                    }

                    lihtml += '<li id="' + id + '_li">';
                    lihtml += '<a href="#' + id + '">' + text + '</a>';
                    lihtml += '</li>';

                    if (fieldtype == 'Logical')
                    {
                        var sf_checked = $xmlitem.children('Checked').text();
                        var sf_required = $xmlitem.children('Required').text();
                        if (sf_checked == undefined) sf_checked = '';
                        if (sf_required == undefined) sf_required = '';

                        lihtml += '<li>';
                        lihtml += _makeLogicalFieldSet(id, 'ans', 'ans2', sf_checked, sf_required);
                        lihtml += '</li>';
                    }

                    if (fieldtype == 'Question')
                    {
                        var sf_answer = $xmlitem.children('Answer').text();
                        var sf_compliantanswers = $xmlitem.children('CompliantAnswers').text();
                        if (sf_answer == undefined) sf_answer = '';
                        if (sf_compliantanswers == undefined) sf_compliantanswers = '';

                        lihtml += '<li>';
                        lihtml += _makeQuestionAnswerFieldSet(id, 'ans', 'ans2', 'cor', 'li', sf_answer, sf_compliantanswers);
                        lihtml += '</li>';
                    }


                    // add a div for editing the property directly
                    var toolbar = '';
                    if (previd != undefined)
                        toolbar += '<a href="#' + previd + '" data-role="button" data-icon="arrow-u" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup" data-back="true">Previous</a>';

                    if (nextid != undefined)
                        toolbar += '<a href="#' + nextid + '" data-role="button" data-icon="arrow-d" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup">Next</a>';

                    toolbar += '&nbsp;' + currentcnt + '&nbsp;of&nbsp;' + siblingcnt;

                    _addPageDivToBody(DivId, parentlevel, id, text, toolbar, _makeFieldTypeContent($xmlitem), IsFirst, true);

                }
                else
                {
                    lihtml += '<li>';
                    if (IsDiv)
                        lihtml += '<a href="#' + id + '">' + text + '</a>';
                    else
                        lihtml += text;
                    lihtml += '</li>';

                }
                content += lihtml;

                // to avoid race condition between link execution and callback from Sqlite,
                // create content for subitems now too
                $xmlchildren = $xmlitem.children('subitems').first().children();
                if ($xmlchildren.length > 0)
                {
                    _processSubLevelXml(DivId, id, text, $xmlchildren, parentlevel + 1)
                }

            }); // $xml.each(function () {


            _addPageDivToBody(ParentId, parentlevel, DivId, HeaderText, '', content, IsFirst, true);

        } // _processSubLevelXml()

        function _makeUL()
        {
            return '<ul data-role="listview" data-inset="true">';
        }

        function _makeFieldTypeContent($xmlitem)
        {
            var IdStr = $xmlitem.attr('id');
            var FieldType = $xmlitem.attr('fieldtype');
            var PropName = $xmlitem.attr('name');

            var Html = PropName + '<br/>';

            // Subfield values
            var sf_text = $xmlitem.children('Text').text();
            var sf_value = $xmlitem.children('Value').text();
            var sf_href = $xmlitem.children('Href').text();
            var sf_options = $xmlitem.children('Options').text();
            var sf_checked = $xmlitem.children('Checked').text();
            var sf_required = $xmlitem.children('Required').text();
            var sf_units = $xmlitem.children('Units').text();
            var sf_answer = $xmlitem.children('Answer').text();
            var sf_correctiveaction = $xmlitem.children('CorrectiveAction').text();
            var sf_comments = $xmlitem.children('Comments').text();
            var sf_compliantanswers = $xmlitem.children('CompliantAnswers').text();

            if (sf_text == undefined) sf_text = '';
            if (sf_value == undefined) sf_value = '';
            if (sf_href == undefined) sf_href = '';
            if (sf_options == undefined) sf_options = '';
            if (sf_checked == undefined) sf_checked = '';
            if (sf_required == undefined) sf_required = '';
            if (sf_units == undefined) sf_units = '';
            if (sf_answer == undefined) sf_answer = '';
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
                    Html += _makeLogicalFieldSet(IdStr, 'ans2', 'ans', sf_checked, sf_required);
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
                    Html += _makeQuestionAnswerFieldSet(IdStr, 'ans2', 'ans', 'cor', 'li', sf_answer, sf_compliantanswers);

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

        function _makeLogicalFieldSet(IdStr, Suffix, OtherSuffix, Checked, Required)
        {
            var Html = '<fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
            //Html += '        <legend></legend>';

            var answers = ['Blank', 'Yes', 'No'];
            if (Required == "true")
                answers = ['Yes', 'No'];

            for (var i = 0; i < answers.length; i++)
            {
                var answertext = answers[i];
                if (answertext == 'Blank') answertext = '?';

                Html += '<input type="radio" name="' + IdStr + '_' + Suffix + '" id="' + IdStr + '_' + Suffix + '_' + answers[i] + '" value="' + answertext + '" ';
                if ((Checked == "false" && answers[i] == "No") ||
                    (Checked == "true" && answers[i] == "Yes") ||
                    (Checked == '' && answers[i] == 'Blank'))
                    Html += 'checked';
                Html += ' onclick="';
                Html += ' var $otherradio; ';
                for (var j = 0; j < answers.length; j++)
                {
                    Html += ' $otherradio = $(\'#' + IdStr + '_' + OtherSuffix + '_' + answers[j] + '\'); ';
                    if (answers[j] == answers[i])
                    {
                        Html += ' $otherradio.attr(\'checked\', true); ';
                        Html += ' $otherradio.siblings(\'label\').addClass(\'ui-btn-active\'); ';
                    }
                    else
                    {
                        Html += ' $otherradio.attr(\'checked\', false); ';
                        Html += ' $otherradio.siblings(\'label\').removeClass(\'ui-btn-active\'); ';
                    }
                } // for (var j = 0; j < answers.length; j++)
                Html += '" />';
                Html += '<label for="' + IdStr + '_' + Suffix + '_' + answers[i] + '">' + answertext + '</label>';
            } // for (var i = 0; i < answers.length; i++)

            Html += '</fieldset>';
            return Html;
        }

        function _makeQuestionAnswerFieldSet(IdStr, Suffix, OtherSuffix, CorrectiveActionSuffix, LiSuffix, Answer, CompliantAnswers)
        {
            var Html = '<fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
            //Html += '<legend>Answer:</legend>';

            var answers = ['Yes', 'No'];

            for (var i = 0; i < answers.length; i++)
            {
                Html += '<input type="radio" name="' + IdStr + '_' + Suffix + '" id="' + IdStr + '_' + Suffix + '_' + answers[i] + '" value="' + answers[i] + '" ';
                if (Answer == answers[i])
                    Html += ' checked';
                Html += ' onclick="';
                Html += ' var $otherradio; ';
                for (var j = 0; j < answers.length; j++)
                {
                    Html += ' $otherradio = $(\'#' + IdStr + '_' + OtherSuffix + '_' + answers[j] + '\'); ';
                    if (answers[j] == answers[i])
                    {
                        Html += ' $otherradio.attr(\'checked\', true); ';
                        Html += ' $otherradio.siblings(\'label\').addClass(\'ui-btn-active\'); ';
                    } else
                    {
                        Html += ' $otherradio.attr(\'checked\', false); ';
                        Html += ' $otherradio.siblings(\'label\').removeClass(\'ui-btn-active\'); ';
                    }
                } // for (var j = 0; i < answers.length; j++)

                if ((',' + CompliantAnswers + ',').indexOf(',' + answers[i] + ',') >= 0)
                {
                    Html += ' $(\'#' + IdStr + '_' + CorrectiveActionSuffix + '\').css(\'display\', \'none\'); ';
                    Html += ' $(\'#' + IdStr + '_' + LiSuffix + ' div\').removeClass(\'OOC\'); ';
                }
                else
                {
                    Html += 'var $cor = $(\'#' + IdStr + '_' + CorrectiveActionSuffix + '\'); ';
                    Html += '$cor.css(\'display\', \'\'); ';
                    Html += 'if($cor.attr(\'value\') == \'\') { ';
                    Html += '  $(\'#' + IdStr + '_' + LiSuffix + ' div\').addClass(\'OOC\'); ';
                    Html += '} else {';
                    Html += '  $(\'#' + IdStr + '_' + LiSuffix + ' div\').removeClass(\'OOC\'); ';
                    Html += '}';
                }
                Html += ' " />';
                Html += '            <label for="' + IdStr + '_' + Suffix + '_' + answers[i] + '">' + answers[i] + '</label>';
            } // for (var i = 0; i < answers.length; i++)
            Html += '</fieldset>';
            return Html;
        }

        function _addPageDivToBody(ParentId, level, DivId, HeaderText, toolbar, content, IsFirst, BindEvents, backtransition)
        {
            var divhtml = '<div id="' + DivId + '" data-role="page">' +
                          '  <div data-role="header" data-theme="' + opts.Theme + '">';
            divhtml += '       <a href="#' + ParentId + '" data-back="true" ';
            if (backtransition != undefined)
                divhtml += '       data-transition="' + backtransition + '" ';
            if (ParentId == '' || ParentId == undefined)
                divhtml += '    style="visibility: hidden"';
            divhtml += '        >Back</a>';
            divhtml += '       <h1>' + HeaderText + '</h1>' +
            //            '    <a href="#" class="offlineIndicator ' + getCurrentOfflineIndicatorCssClass() + '" onclick="toggleOffline();">Online</a>' +
                          '    <a href="#Search" data-transition="slideup">Search</a>' +
                          '    <div class="toolbar" data-role="controlgroup" data-type="horizontal">' +
            //            '      <a href="' + opts.MainPageUrl + '" data-transition="flip" rel="external">Top</a>' +
                                 toolbar +
                          '    </div>' +
                          '  </div>' +
                          '  <div data-role="content" data-theme="' + opts.Theme + '">' +
                               content +
                          '  </div>' +
                          '  <div data-role="footer" data-theme="' + opts.Theme + '">' +
                          '  </div>' +
                          '</div>';

            var $divhtml = $(divhtml);
            if (IsFirst)
                $('body').prepend($divhtml);
            else
                $('body').append($divhtml);

            $divhtml.page();
            if (BindEvents)
            {
                $divhtml.find('a')
                    .click(function (e) { _loadDivContents(DivId, (level + 1), $(this).attr('href').substr(1), $(this).text(), false); })
                    .end()
                    .find('input')
                    .change(onPropertyChange)
                    .end()
                    .find('textarea')
                    .change(onPropertyChange)
                    .end()
                    .find('select')
                    .change(onPropertyChange);
            }
        }


        function _makeSearchDiv()
        {
            var toolbar = '';
            var content = 'Search page placeholder...'
            _addPageDivToBody('viewsdiv', 0, 'Search', 'Search', toolbar, content, false, false, 'slideup');
        }



        // ------------------------------------------------------------------------------------
        // Events
        // ------------------------------------------------------------------------------------

        function onPropertyChange(eventObj)
        {
            var $elm = $(eventObj.srcElement);
            var name = $elm.attr('name');
            var value = $elm.attr('value');

            // update the short summary value on the list item
            $('a[href="#' + name + '"]')
                .children('small')
                .text(value);

            // store the property value change in the database
            _storeChange(name, value)
        }


        // ------------------------------------------------------------------------------------
        // Client-side Database Interaction
        // ------------------------------------------------------------------------------------


        function _DoSql(sql, params, onSuccess)
        {
            db.transaction(function (transaction)
            {
                transaction.executeSql(sql, params, onSuccess, _errorHandler);
            });
        }

        function _initDB(doreset)
        {
           if (window.openDatabase) {

//                console.log("DbShortName: " + opts.DBShortName + "; DBVersion: " + opts.DBVersion + "; DisplayName: " + opts.DisplayName + "; MaxSize: " + opts.MaxSize ); 
//                db = openDatabase(opts.DBShortName, opts.DBVersion, opts.DisplayName, opts.MaxSize);

                console.log("DbShortName: " + opts.DBShortName + "; DBVersion: " + opts.DBVersion + "; DisplayName: " + opts.DBDisplayName + "; MaxSize: " + opts.DBMaxSize);
                db = openDatabase(opts.DBShortName, opts.DBVersion, opts.DBDisplayName, opts.DBMaxSize);

                console.log("got here");

                if (null == db) {
                    console.log("db is null");
                }

                if (doreset) {
                    _DoSql('DROP TABLE IF EXISTS sublevels; ');
                    _DoSql('DROP TABLE IF EXISTS changes; ');
                }

                _createDB();
            } else 
            {
                console.log("database is not opened"); 
            }
            
        }//_initDb()
 



        function _createDB()
        {
            _DoSql('CREATE TABLE IF NOT EXISTS sublevels ' +
                   '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
                   '   rootid TEXT NOT NULL, ' +
                   '   rootname TEXT NOT NULL, ' +
                   '   rootxml TEXT, ' +
                   '   sublevelxml TEXT );');

            _DoSql('CREATE TABLE IF NOT EXISTS changes ' +
                   '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
                   '   propid TEXT NOT NULL, ' +
                   '   newvalue TEXT, ' +
                   '   applied CHAR ); ');
        }

        function _storeSubLevelXml(rootid, rootname, rootxml, sublevelxml)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('INSERT INTO sublevels (rootid, rootname, rootxml, sublevelxml) VALUES (?, ?, ?, ?);',
                       [rootid, rootname, rootxml, sublevelxml],
                       function () { }
                       );
            }
        }
        function _storeChange(propid, newvalue)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('INSERT INTO changes (propid, newvalue) VALUES (?, ?);',
                       [propid, newvalue],
                       function () { }
                       );
            }
        }
        function _fetchCachedSubLevelXml(rootid, onsuccess)
        {
            if (rootid != undefined && rootid != '')
            {
                _DoSql('SELECT sublevelxml FROM sublevels WHERE rootid = ? ORDER BY id DESC;',
                       [rootid],
                       function (transaction, result)
                       {
                           if (result.rows.length > 0)
                           {
                               var row = result.rows.item(0);
                               onsuccess(row.sublevelxml);
                           }
                       });
            }
        }
        function _fetchCachedRootXml(onsuccess)
        {
            _DoSql('SELECT rootid, rootname, rootxml FROM sublevels ORDER BY rootname;',
                   [],
                   function (transaction, result)
                   {
                       var xml = '';
                       for (var i = 0; i < result.rows.length; i++)
                       {
                           var row = result.rows.item(i);
                           xml += "<item id=\"" + row.rootid + "\" arrow=\"true\">" +
                                  "  <text>" + row.rootname + "</text>" +
                                  "</item>";
                       }
                       onsuccess("<root>" + xml + "</root>");
                   });
        }


        function _errorHandler(transaction, error)
        {
            alert('Database Error: ' + error.message + ' (Code ' + error.code + ')');
            return true;
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
        alert("iterate() error: No popup!");
}
