; (function ($) {
    $.fn.CswNbt = function (options) {

        var o = {
            ViewSelectDivId: 'viewdiv',
            TreeDivId: 'treediv',
            TabDivId: 'tabdiv',
            TimerDiv: 'timerdiv',
            ViewUrl: '/NbtWebApp/wsNBT.asmx/JQueryGetViews',
            TreeUrl: '/NbtWebApp/wsNBT.asmx/JQueryGetTree',
            TabsUrl: '/NbtWebApp/wsNBT.asmx/JQueryGetTabs',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/JQueryGetProps',
            SessionId: ''
        };

        if (options) {
            $.extend(o, options);
        }

        var DefaultViewId = "176";
        var SelectedNodePk;


        getViewSelect();


        function getViewSelect()
        {
            starttime = new Date();
            $.ajax({
                type: 'POST',
                url: o.ViewUrl,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: '{ SessionId: "'+ o.SessionId +'" }',
                success: function (data, textStatus, XMLHttpRequest)
                {
                    var $viewsdiv = $("#" + o.ViewSelectDivId);
                    $viewsdiv.children().remove();
                    $select = $('<select name="viewselect" id="viewselect"><option value="">Select A View</option></select>').appendTo($viewsdiv);
                    $(data.d).children().each(function() {
                        $this = $(this);
                        $select.append('<option value="'+$this.attr('id')+'">'+$this.attr('name')+'</option>');
                    });
                    $select.bind('change', function(e, data) { 
                        getTree(e.target.value);
                    });
                    updateTimer("getViewSelect", starttime, new Date());
                }, // success{}
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                }
            }); // $.ajax({
        } // getViewSelect()

        function getTree(viewid)
        {
            starttime = new Date();
            $.ajax({
                type: 'POST',
                url: o.TreeUrl,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: '{ SessionId: "'+ o.SessionId +'", ViewId: "'+ viewid +'" }',
                success: function (data, textStatus, XMLHttpRequest)
                {
                    $("#" + o.TreeDivId)
                        .addClass('treediv')
                        .jstree({
                            "xml_data": {
                                "data": data.d,
                                "xsl": "nest"
                            },
                            "ui": {
                                "select_limit": 1
                            },
                            "plugins": ["themes", "xml_data", "ui"]
                        })  // .jstree({
                        .bind('select_node.jstree', 
                                function(e, data) {
                                    SelectedNodePk = data.args[0].parentNode.id;
                                    getTabs(SelectedNodePk);
                                });

                    updateTimer("getTree", starttime, new Date());
                }, // success{}
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                }
            }); // $.ajax({
        } // getTree()


        function getTabs(nodepk)
        {
            starttime = new Date();
            $.ajax({
                type: 'POST',
                url: o.TabsUrl,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: '{ SessionId: "' + o.SessionId +'", NodePk: "' + nodepk + '" }',
                success: function (data, textStatus, XMLHttpRequest)
                        {
                            var $outertabdiv = $("#" + o.TabDivId);
                            $outertabdiv.children().remove()
                            var $tabdiv = $("<div><ul></ul></div>");
                            $outertabdiv.append($tabdiv);

                            //var firsttabid = null;
                            $(data.d).children().each(function() { 
                                $this = $(this);
                                $tabdiv.children('ul').append('<li><a href="#'+ $this.attr('id') +'">'+ $this.attr('name') +'</a></li>');
                                $tabdiv.append('<div id="'+ $this.attr('id') +'"></div>');
                                //if(null == firsttabid) 
                                //    firsttabid = $this.attr('id');
                            });
                            $tabdiv.tabs({
                                select: function(event, ui) {
                                            getProps(nodepk, $($tabdiv.children('div')[ui.index]).attr('id'));
                                        }
                            });
                            updateTimer("getTabs", starttime, new Date());
                            getProps(nodepk, $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]).attr('id'));
                        }, // success{}
                error: function (XMLHttpRequest, textStatus, errorThrown)
                        {
                            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                        }
            });
        } // getTabs()
            
        function getProps(nodepk, tabid)
        {
            starttime = new Date();
            $.ajax({
                type: 'POST',
                url: o.PropsUrl,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: '{ SessionId: "' + o.SessionId +'", NodePk: "' + nodepk + '", TabId: "' + tabid + '" }',
                success: function (data, textStatus, XMLHttpRequest)
                        {
                            $div = $("#" + tabid);
                            $div.children().remove();
                            var $table = $('<table></table>').appendTo($div);
                            var $data = $(data.d);
                            $data.children().each(function() { 
                                $this = $(this);
                                while($this.attr('displayrow') >= $table.find('tr').length)
                                {
                                    $table.append('<tr></tr>');
                                }
                                var $row = $($table.find('tr')[$this.attr('displayrow')]);
                                while($this.attr('displaycol') >= $row.find('td').length)
                                {
                                    $row.append('<td></td>');
                                }
                                var $cell = $($row.find('td')[$this.attr('displaycol')]);
                                $cell.append($this.attr('name') + ' = ' + $this.attr('gestalt'));
                            });
                            updateTimer("getProps", starttime, new Date());
                        }, // success{}
                error: function (XMLHttpRequest, textStatus, errorThrown)
                        {
                            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
                        }
            }); 
        } // getProps()

        function _handleAjaxError(XMLHttpRequest, textStatus, errorThrown)
        {
            ErrorMessage = "Error: " + textStatus;
            if (null != errorThrown)
            {
                ErrorMessage += "; Exception: " + errorThrown.toString()
            }
            console.log(ErrorMessage);
        } // _handleAjaxError()

        function updateTimer(label, starttime, endtime)
        {
            $('#'+o.TimerDiv).append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] "+ label + " time: " + (endtime - starttime) + "ms<br>");
        }

        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

