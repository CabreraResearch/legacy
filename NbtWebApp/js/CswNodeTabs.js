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
                            $div = $("#" + tabid);
                            $div.children().remove();
                            
                            var $table = makeTable('proptable').appendTo($div);
                            
                            var i = 0;

                            $xml.children().each(function() { 
                                var $this = $(this);

                                var $labelcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2 ) - 1);
                                $labelcell.addClass('propertylabel');
                                $labelcell.append($this.attr('name'));

                                var $propcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2));
                                $propcell.addClass('propertyvaluecell');
                                var $propdiv = $('<div/>').appendTo($propcell); 

                                var fieldtype = $this.attr('fieldtype');
                                makePropControl($propdiv, fieldtype, $this);
                                
                            });

                            $table.append('<tr><td><input type="button" id="SaveTab" name="SaveTab" value="Save"/></td></tr>')
                                  .find('#SaveTab')
                                  .click(function() { Save($table, $xml) });
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
                case "PropertyReference":
                    $propdiv.CswFieldTypePropertyReference('init', o.nodeid, $propxml);
                    break;
                case "Relationship":
                    $propdiv.CswFieldTypeRelationship('init', o.nodeid, $propxml);
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

        function Save($table, $propsxml)
        {
            $propsxml.children().each(function() { 
                var $propxml = $(this);
                var $propcell = getTableCell($table, $propxml.attr('displayrow'), ($propxml.attr('displaycol') * 2));
                var fieldtype = $propxml.attr('fieldtype');
                var $propdiv = $propcell.children('div');
                                
                switch(fieldtype)
                {
                    case "List":
                        $propdiv.CswFieldTypeList( 'save', $propdiv, $propxml );
                        break;
                    case "Logical":
                        $propdiv.CswFieldTypeLogical( 'save', $propdiv, $propxml );
                        break;
                    case "Memo":
                        $propdiv.CswFieldTypeMemo( 'save', $propdiv, $propxml );
                        break;
                    case "PropertyReference":
                        $propdiv.CswFieldTypePropertyReference( 'save', $propdiv, $propxml );
                        break;                    
                    case "Relationship":
                        $propdiv.CswFieldTypeRelationship( 'save', $propdiv, $propxml );
                        break;
                    case "Static":
                        $propdiv.CswFieldTypeStatic( 'save', $propdiv, $propxml );
                        break;
                    case "Text":
                        $propdiv.CswFieldTypeText( 'save', $propdiv, $propxml );
                        break;
                    default:
                        break;
                } // switch
            }); // each()

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

