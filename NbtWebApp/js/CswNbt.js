; (function ($) {
    $.fn.CswNbt = function (options) {

        var o = {
            ViewSelectDivId: 'LeftDiv',
            TreeDivId: 'LeftDiv',
            TabDivId: 'RightDiv',
            TimerDiv: 'CenterDiv',
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

        var $viewsdiv = $('<div id="viewsdiv" />')
                        .appendTo($("#" + o.ViewSelectDivId));
        var $treediv = $('<div id="treediv" class="treediv" />')
                        .appendTo($("#" + o.TreeDivId));
        var $outertabdiv = $('<div id="tabdiv" />')
                        .appendTo($("#" + o.TabDivId));
        var $timerdiv = $('<div id="timerdiv" />')
                        .appendTo($("#" + o.TimerDiv));

        getViewSelect();


        function getViewSelect()
        {
            starttime = new Date();
            CswAjax({
                url: o.ViewUrl,
                data: '{ SessionId: "'+ o.SessionId +'" }',
                success: function ($xml)
                {
                    $viewsdiv.children().remove();
                    $select = $('<select name="viewselect" id="viewselect"><option value="">Select A View</option></select>')
                              .appendTo($viewsdiv);
                    $xml.children().each(function() {
                        $this = $(this);
                        $select.append('<option value="'+$this.attr('id')+'">'+$this.attr('name')+'</option>');
                    });
                    $select.bind('change', function(e, data) { 
                        getTree(e.target.value);
                    });
                    updateTimer("getViewSelect", starttime, new Date());
                } // success{}
            });
        } // getViewSelect()

        function getTree(viewid)
        {
            starttime = new Date();
            CswAjax({
                url: o.TreeUrl,
                data: '{ SessionId: "'+ o.SessionId +'", ViewId: "'+ viewid +'" }',
                success: function ($xml, xml) {
                    $treediv.jstree({
                            "xml_data": {
                                "data": xml,
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
                    clearTabs();
                    updateTimer("getTree", starttime, new Date());
                } // success{}
            });
        } // getTree()

        function clearTabs()
        {
            $outertabdiv.children().remove();
        }

        function getTabs(nodepk)
        {
            starttime = new Date();
            CswAjax({
                url: o.TabsUrl,
                data: '{ SessionId: "' + o.SessionId +'", NodePk: "' + nodepk + '" }',
                success: function ($xml) {
                            clearTabs();
                            var $tabdiv = $("<div><ul></ul></div>");
                            $outertabdiv.append($tabdiv);

                            //var firsttabid = null;
                            $xml.children().each(function() { 
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
                        } // success{}
            });
        } // getTabs()
            
        function getProps(nodepk, tabid)
        {
            starttime = new Date();
            CswAjax({
                url: o.PropsUrl,
                data: '{ SessionId: "' + o.SessionId +'", NodePk: "' + nodepk + '", TabId: "' + tabid + '" }',
                success: function ($xml) {
                            $div = $("#" + tabid);
                            $div.children().remove();
                            var $table = $('<table></table>').appendTo($div);
                            $xml.children().each(function() { 
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
                        } // success{}
            }); 
        } // getProps()


        function updateTimer(label, starttime, endtime)
        {
            $timerdiv.append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] "+ label + " time: " + (endtime - starttime) + "ms<br>");
        }

        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

