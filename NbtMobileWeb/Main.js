
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
            MainPageUrl: '/NbtMobileWeb/Main.html'
        };

        if (options)
        {
            $.extend(opts, options);
        }

        //    var $Div = this;
        var rootid;
        var db;

        _initDB(true);
        _loadDivContents(0, this);


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

        function _loadDivContents(level, $Div)
        {
            var ParentId = $Div.attr('id');
            if (level == 1)
                rootid = ParentId;
            var $Ul = $Div.children('div[data-role="content"]').children('ul').first();
            if ($Ul.children('li').length == 0)
            {
                if (level <= 1)
                {
                    if (amOffline())
                    {
                        if (level == 0)
                        {
                            _fetchCachedRootXml(function (xml)
                            {
                                _processSubLevelXml($Div, $(xml).children(), $Ul, level, true);
                                $Ul.listview();
                            });
                        } else
                        {
                            _fetchCachedSubLevelXml(ParentId, function (xmlstr)
                            {
                                var $thisxmlstr = $(xmlstr).find('#' + ParentId);
                                _processSubLevelXml($Div, $thisxmlstr.children('subitems').first().children(), $Ul, level);
                                $Ul.listview();
                            });
                        }
                    } else
                    {
                        $.ajax({
                            type: 'POST',
                            url: opts.WebServiceUrl,
                            dataType: "json",
                            contentType: 'application/json; charset=utf-8',
                            data: "{ ParentId: '" + ParentId + "' }",
                            success: function (data, textStatus, XMLHttpRequest)
                            {
                                if (level == 1)
                                {
                                    _storeSubLevelXml(ParentId, $Div.children('div[data-role="header"]').children('h1').text(), '', data.d);
                                }
                                _processSubLevelXml($Div, $(data.d).children(), $Ul, level, true);
                                $Ul.listview();
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
                        var $thisxmlstr = $(xmlstr).find('#' + ParentId);
                        _processSubLevelXml($Div, $thisxmlstr.children('subitems').first().children(), $Ul, level);
                        $Ul.listview();
                    });
                }
            }
        }

        function _processSubLevelXml($Div, $xml, $ParentUL, parentlevel)
        {
            $xml.each(function ()
            {
                var $xmlitem = $(this);
                var id = $xmlitem.attr('id');
                var text = $xmlitem.children('text').first().contents().first().text();
                var texthtml = $xmlitem.children('text').html();
                var IsDiv = (id != undefined && id != '');
                var PageType = $xmlitem.attr('pagetype');

                var nextid = $xmlitem.next().attr('id');
                var previd = $xmlitem.prev().attr('id');
                var parentid = $Div.attr('id');
                
                var currentcnt = $xmlitem.prevAll().andSelf().length;
                var siblingcnt = $xmlitem.siblings().andSelf().length;

                var lihtml = '<li';
                // if ($xmlitem.attr('arrow') == 'true')
                //     lihtml += ' class="arrow"';
                lihtml += '>';
                if (IsDiv)
                    lihtml += '<a href="#' + id + '">' + texthtml + '</a>';
                else
                    lihtml += texthtml;
                lihtml += '</li>';

                $(lihtml).appendTo($ParentUL)
                    .find('input')
                    .change(onPropertyChange)
                    .end()
                    .find('textarea')
                    .change(onPropertyChange)
                    .end()
                    .find('select')
                    .change(onPropertyChange);

                if (IsDiv)
                {
                    var divhtml = '<div id="' + id + '" data-role="page">' +
                                  '  <div data-role="header">' +
                    //            '    <a href="#" class="back">Back</a>' +
                                  '    <h1>' + text + ' (' + PageType + ')</h1>' +
                                  '    <a href="#" class="offlineIndicator ' + getCurrentOfflineIndicatorCssClass() + '" onclick="toggleOffline();">Online</a>' +
                                  '    <div class="toolbar">' +
                                  '      <a href="' + opts.MainPageUrl + '" data-transition="flip" rel="external">Top</a>' +
                                  '      <a href="#' + parentid + '" data-back="true">Back</a>' +
                                  '    </div>' +
                                  '  </div>' +
                                  '  <div data-role="content">' +
                                  '    <ul></ul>' +
                                  '  </div>' +
                                  '  <div data-role="footer">' +
                                  '  </div>' +
                                  '</div>';

                    $(divhtml).appendTo($('body'))
                        .bind('pageshow', function () { _loadDivContents(parentlevel + 1, $(this)); })
                        .page();


                    var $cg = $('div#' + id + ' div[data-role="header"] div.toolbar');

                    if (PageType == "Property")
                    {
                        if (previd != undefined)
                        {
                            var $prevbutton = $('<a href="#' + previd + '" data-transition="slideup" data-back="true">Previous</a>');
                            $prevbutton.buttonMarkup({
                                'icon': 'arrow-u',
                                'iconpos': 'left',
                                'inline': true,
                                'shadow': false,
                                'theme': 'a'
                            })
                                .appendTo($cg);
                            //  if (previd == undefined)
                            //  {
                            //      $prevbutton.attr('disabled', 'true');
                            //  }
                        }

                        if (nextid != undefined)
                        {
                            var $nextbutton = $('<a href="#' + nextid + '" data-transition="slideup">Next</a>');
                            $nextbutton.buttonMarkup({
                                'icon': 'arrow-d',
                                'iconpos': 'left',
                                'inline': true,
                                'shadow': false,
                                'theme': 'a'
                            })
                                .appendTo($cg);
                            //  if (nextid == undefined)
                            //  {
                            //      $nextbutton.attr('disabled', 'true');
                            //  }
                        }
                        
                        $cg.append('&nbsp;'+currentcnt+'&nbsp;of&nbsp;'+ siblingcnt);

                    } // if (PageType == "Property")
                    $cg.controlgroup({
                        'direction': 'horizontal',
                    });
                } // if (IsDiv)

            }); // $xml.each(function () {
        } // _processSubLevelXml()

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
            db = openDatabase(opts.DBShortName, opts.DBVersion, opts.DisplayName, opts.MaxSize);
            if (doreset)
            {
                _DoSql('DROP TABLE IF EXISTS sublevels; ');
                _DoSql('DROP TABLE IF EXISTS changes; ');
            }
            _createDB();
        }

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
    popup.document.write(str);
}
