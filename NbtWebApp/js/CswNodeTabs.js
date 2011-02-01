; (function ($) {
    $.fn.CswNodeTabs = function (options) {

        var o = {
            TabsUrl: '/NbtWebApp/wsNBT.asmx/GetTabs',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/GetProps',
            nodeid: '',
            onSave: function() {}
        };

        if (options) {
            $.extend(o, options);
        }

        var $propsxml;
        var controls;

        var $outertabdiv = $('<div id="tabdiv" />')
                        .appendTo($(this));

        getTabs(o.nodeid);


        function clearTabs()
        {
            $outertabdiv.children().remove();
        }

        function getTabs()
        {
            CswAjax({
                url: o.TabsUrl,
                data: '{ NodePk: "' + o.nodeid + '" }',
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
                                            getProps($($tabdiv.children('div')[ui.index]).attr('id'));
                                        }
                            });
                            getProps($($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]).attr('id'));
                        } // success{}
            });
        } // getTabs()
            
        function getProps(tabid)
        {
            CswAjax({
                url: o.PropsUrl,
                data: '{ NodePk: "' + o.nodeid + '", TabId: "' + tabid + '" }',
                success: function ($xml) {
                            // Store this for Save() later
                            $propsxml = $xml;

                            $div = $("#" + tabid);
                            $div.children().remove();
                            
                            var $table = $('<table></table>')
                                           .appendTo($div);

                            controls = new Array($xml.children().length);
                            var i = 0;

                            $propsxml.children().each(function() { 
                                var $this = $(this);
                                var $labelcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2 ) - 1);
                                var $propcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2));
                                var fieldtype = $this.attr('fieldtype');
                                var $propdiv = $('<div/>').appendTo($propcell); 

                                $labelcell.append($this.attr('name'));

                                makePropControl($propdiv, fieldtype, $this);
                                
                                controls[i] = { 'div': $propdiv, 'fieldtype': fieldtype };
                                i++;
                            });

                            $table.append('<tr><td><input type="button" id="SaveTab" name="SaveTab" value="Save"/></td></tr>')
                                  .find('#SaveTab')
                                  .click(Save);
                        } // success{}
            }); 
        } // getProps()

        function makePropControl($propdiv, fieldtype, $propxml)
        {
            switch(fieldtype)
            {
                case "List":
                    $propdiv.CswFieldTypeList( 'init', o.nodeid, $propxml );
                    break;
                case "Logical":
                    $propdiv.CswFieldTypeLogical( 'init', o.nodeid, $propxml );
                    break;
                case "Memo":
                    $propdiv.CswFieldTypeMemo( 'init', o.nodeid, $propxml );
                    break;
                case "Static":
                    $propdiv.CswFieldTypeStatic( 'init', o.nodeid, $propxml );
                    break;
                case "Text":
                    $propdiv.CswFieldTypeText( 'init', o.nodeid, $propxml );
                    break;
                default:
                    $propdiv.append($propxml.attr('gestalt'));
                    break;
            }
        } // makePropControl()

        function Save()
        {
            for(var i = 0; i < controls.length; i++)
            {
                switch(controls[i].fieldtype)
                {
                    case "List":
                        controls[i].div.CswFieldTypeList( 'save' );
                        break;
                    case "Logical":
                        controls[i].div.CswFieldTypeLogical( 'save' );
                        break;
                    case "Memo":
                        controls[i].div.CswFieldTypeMemo( 'save' );
                        break;
                    case "Static":
                        controls[i].div.CswFieldTypeStatic( 'save' );
                        break;
                    case "Text":
                        controls[i].div.CswFieldTypeText( 'save' );
                        break;
                    default:
                        break;
                }
            }

            CswAjax({
                url: '/NbtWebApp/wsNBT.asmx/SaveProps',
                data: "{ NodePk: '" + o.nodeid + "', NewPropsXml: '" + $propsxml.get(0).outerHTML + "' }",
                success: o.onSave 
            });

        } // Save()

        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

