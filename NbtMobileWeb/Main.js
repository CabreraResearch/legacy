
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
        _loadDivContents(0, 'viewsdiv', 'Views');


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

        function _loadDivContents(level, DivId, HeaderText)
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
                                _processSubLevelXml(DivId, HeaderText, $(xml).children(), level, true);
                                lock = false;
                            });
                        } else
                        {
                            _fetchCachedSubLevelXml(DivId, function (xmlstr)
                            {
                                var $thisxmlstr = $(xmlstr).find('#' + DivId);
                                _processSubLevelXml(DivId, HeaderText, $thisxmlstr.children('subitems').first().children(), level);
                                lock = false;
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
                                if (level == 1)
                                {
                                    _storeSubLevelXml(DivId, HeaderText, '', data.d);
                                }
                                _processSubLevelXml(DivId, HeaderText, $(data.d).children(), level, true);
                                lock = false;
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown)
                            {
                                alert("An Error Occurred: " + errorThrown);
                                lock = false;
                            }
                        });
                    }
                } else
                {
                    _fetchCachedSubLevelXml(rootid, function (xmlstr)
                    {
                        var $thisxmlstr = $(xmlstr).find('#' + DivId);
                        _processSubLevelXml(DivId, HeaderText, $thisxmlstr.children('subitems').first().children(), level);
                        lock = false;
                    });
                }
            }
        }



        function _processSubLevelXml(DivId, HeaderText, $xml, parentlevel)
        {
            var currenttab;
            var content = '<ul data-role="listview">';

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
                            lihtml += '</ul><ul data-role="listview">';
                        lihtml += '<li data-role="list-divider">' + tab + '</li>'
                        currenttab = tab;
                    }

                    lihtml += '<li>';
                    lihtml += '<a href="#' + id + '">' + text + '</a>';
                    lihtml += '</li>';
                    if (fieldtype == 'Question')
                    {
                        lihtml += '<li>';
                        lihtml += '    <fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
                        lihtml += '        <legend>Answer:</legend>';
                        lihtml += '            <input type="radio" name="' + id + '_ans" id="' + id + '_ans_Yes" value="Yes" />';
                        lihtml += '            <label for="' + id + '_ans_Yes">Yes</label>';
                        lihtml += '            <input type="radio" name="' + id + '_ans" id="' + id + '_ans_No" value="No" />';
                        lihtml += '            <label for="' + id + '_ans_No">No</label>';
                        lihtml += '    </fieldset>';
                        lihtml += '</li>';
                    }

                    // add a div for editing the property directly
                    var toolbar = '';
                    if (previd != undefined)
                        toolbar += '<a href="#' + previd + '" data-role="button" data-icon="arrow-u" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup" data-back="true">Previous</a>';

                    if (nextid != undefined)
                        toolbar += '<a href="#' + nextid + '" data-role="button" data-icon="arrow-d" data-inline="true" data-theme="' + opts.Theme + '" data-transition="slideup">Next</a>';

                    toolbar += '&nbsp;' + currentcnt + '&nbsp;of&nbsp;' + siblingcnt;

                    _addPageDivToBody(parentlevel, id, text, toolbar, _makeFieldTypeContent($xmlitem));

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
                    _processSubLevelXml(id, text, $xmlchildren, parentlevel + 1)
                }

            }); // $xml.each(function () {


            _addPageDivToBody(parentlevel, DivId, HeaderText, '', content);

        } // _processSubLevelXml()

        function _makeFieldTypeContent($xmlitem)
        {
            var IdStr = $xmlitem.attr('id');
            var FieldType = $xmlitem.attr('fieldtype');

            //var Html = 'Fieldtype == ' + FieldType + '<br>';
            switch (FieldType)
            {
                case "Date":
                    Html += '<input type="date" name="' + IdStr + '" value="' + $xmlitem.children('Value').text() + '" />';
                    break;

                case "Link":
                    Html += '<a href="' + $xmlitem.children('Href').text() + '">' + $xmlitem.children('Text').text() + '</a>';
                    break;

                case "List":
                    Html += '<select name="' + IdStr + '">';
                    var selectedvalue = $xmlitem.children('Value').text();
                    var optionsstr = $xmlitem.children('Options').text();
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
                    Html += '    <fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
                    Html += '        <legend></legend>';
                    Html += '            <input type="radio" name="' + IdStr + '_ans" id="' + IdStr + '_ans_Blank" value="?" ';
                    if ($xmlitem.children('Checked').text() == '')
                        Html += 'checked';
                    Html += '/>';
                    Html += '            <label for="' + IdStr + '_ans_Blank">?</label>';
                    Html += '            <input type="radio" name="' + IdStr + '_ans" id="' + IdStr + '_ans_Yes" value="Yes" ';
                    if ($xmlitem.children('Checked').text() == 'Yes')
                        Html += 'checked';
                    Html += '/>';
                    Html += '            <label for="' + IdStr + '_ans_Yes">Yes</label>';
                    Html += '            <input type="radio" name="' + IdStr + '_ans" id="' + IdStr + '_ans_No" value="No" ';
                    if ($xmlitem.children('Checked').text() == 'No')
                        Html += 'checked';
                    Html += '/>';
                    Html += '            <label for="' + IdStr + '_ans_No">No</label>';
                    Html += '    </fieldset>';
                    break;

                case "Memo":
                    Html += '<textarea name="' + IdStr + '">' + $xmlitem.children('Text').text() + '</textarea>';
                    break;

                case "Number":
                    Html += '<input type="number" name="' + IdStr + '" value="' + $xmlitem.children('Value').text() + '"';
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
                    Html += '<input type="text" name="' + IdStr + '_qty" value="' + $xmlitem.children('Value').text() + '" />';
                    Html += $xmlitem.children('Units').text()
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
                    var answer = $xmlitem.attr('answer');
                    var compliantanswer = $xmlitem.attr('compliantanswer');
                    // Html += '    <fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">';
                    // Html += '        <legend></legend>';
                    // Html += '            <input type="radio" name="' + IdStr + '_ans" id="' + IdStr + '_ans_Yes" value="Yes" ';
                    // if (compliantanswer == 'Yes')
                    //     Html += 'onclick="$(\'#' + IdStr + '_cor\').hide();"';
                    // else
                    //     Html += 'onclick="$(\'#' + IdStr + '_cor\').show();"';
                    // if (answer == 'Yes')
                    //     Html += 'checked';
                    // Html += '/>';
                    // Html += '            <label for="' + IdStr + '_ans_Yes">Yes</label>';
                    // Html += '            <input type="radio" name="' + IdStr + '_ans" id="' + IdStr + '_ans_No" value="No" ';
                    // if (compliantanswer == 'No')
                    //     Html += 'onclick="$(\'#' + IdStr + '_cor\').hide();"';
                    // else
                    //     Html += 'onclick="$(\'#' + IdStr + '_cor\').show();"';
                    // Html += '/>';
                    // Html += '            <label for="' + IdStr + '_ans_No">No</label>';
                    // Html += '    </fieldset>';

                    Html += '<textarea name="' + IdStr + '_com" placeholder="Comments">';
                    Html += $xmlitem.attr('comments');
                    Html += '</textarea>';

                    Html += '<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action"';
                    if (answer == '' || answer == compliantanswer)
                        Html += 'style="display: none"';
                    Html += '>';
                    Html += $xmlitem.attr('correctiveaction');
                    Html += '</textarea>';
                    break;

                case "Static":
                    Html += $xmlitem.children('Text').text();
                    break;

                case "Text":
                    Html += '<input type="text" name="' + IdStr + '" value="' + $xmlitem.children('Text').text() + '" />';
                    break;

                case "Time":
                    Html += '<input type="time" name="' + IdStr + '" value="' + $xmlitem.children('Value').text() + '" />';
                    break;

                default:
                    Html += $xmlitem.attr('gestalt');
                    break;
            }
            return Html;
        }

        function _addPageDivToBody(level, DivId, HeaderText, toolbar, content)
        {
            var divhtml = '<div id="' + DivId + '" data-role="page">' +
                          '  <div data-role="header" data-theme="' + opts.Theme + '">' +
            //            '    <a href="#" class="back">Back</a>' +
                          '    <h1>' + HeaderText + '</h1>' +
                          '    <a href="#" class="offlineIndicator ' + getCurrentOfflineIndicatorCssClass() + '" onclick="toggleOffline();">Online</a>' +
                          '    <div class="toolbar" data-role="controlgroup" data-type="horizontal">' +
                          '      <a href="' + opts.MainPageUrl + '" data-transition="flip" rel="external">Top</a>' +
            //            '      <a href="#' + ParentId + '" data-back="true">Back</a>' +
                                 toolbar +
                          '    </div>' +
                          '  </div>' +
                          '  <div data-role="content" data-theme="' + opts.Theme + '">' +
                               content +
                          '  </div>' +
                          '  <div data-role="footer" data-theme="' + opts.Theme + '">' +
                          '  </div>' +
                          '</div>';

            $(divhtml)
                .appendTo($('body'))
                .page()
                .find('a')
                .click(function (e) { _loadDivContents((level + 1), $(this).attr('href').substr(1), $(this).text()); })
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
