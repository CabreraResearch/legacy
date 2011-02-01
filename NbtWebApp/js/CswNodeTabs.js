; (function ($) {
    $.fn.CswNodeTabs = function (options) {

        var o = {
            TabsUrl: '/NbtWebApp/wsNBT.asmx/GetTabs',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/GetProps',
            nodeid: ''
        };

        if (options) {
            $.extend(o, options);
        }

        var $outertabdiv = $('<div id="tabdiv" />')
                        .appendTo($(this));

        getTabs(o.nodeid);


        function clearTabs()
        {
            $outertabdiv.children().remove();
        }

        function getTabs(nodepk)
        {
            CswAjax({
                url: o.TabsUrl,
                data: '{ NodePk: "' + nodepk + '" }',
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
                            getProps(nodepk, $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]).attr('id'));
                        } // success{}
            });
        } // getTabs()
            
        function getProps(nodepk, tabid)
        {
            CswAjax({
                url: o.PropsUrl,
                data: '{ NodePk: "' + nodepk + '", TabId: "' + tabid + '" }',
                success: function ($xml) {
                            $div = $("#" + tabid);
                            $div.children().remove();
                            var $table = $('<table></table>')
                                           .appendTo($div);
                            $xml.children().each(function() { 
                                var $this = $(this);
                                var $labelcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2 ) - 1);
                                var $propcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2));
                                var fieldtype = $this.attr('fieldtype');
                                var $propdiv = $('<div/>').appendTo($propcell); 

                                $labelcell.append($this.attr('name'));

                                switch(fieldtype)
                                {
                                    case "List":
                                        $propdiv.CswFieldTypeList($this);
                                        break;
                                    case "Logical":
                                        $propdiv.CswFieldTypeLogical($this);
                                        break;
                                    case "Memo":
                                        $propdiv.CswFieldTypeMemo($this);
                                        break;
                                    case "Text":
                                        $propdiv.CswFieldTypeText($this);
                                        break;
                                    default:
                                        $propdiv.append($this.attr('gestalt'));
                                        break;
                                }
                            });
                        } // success{}
            }); 
        } // getProps()

        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

