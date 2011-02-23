; (function ($) {
    $.fn.CswNodeTabs = function (options) {

        var o = {
            ID: '',
            TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
            nodeid: '',
            nodetypeid: '',
            EditMode: 'Edit', // Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue
            onSave: function() {}
        };

        if (options) {
            $.extend(o, options);
        }

        var $outertabdiv = $('<div id="' + o.ID + '_tabdiv" />')
                        .appendTo($(this));

        getTabs(o.nodeid);


        function clearTabs()
        {
            $outertabdiv.children().remove();
        }

        function getTabs()
        {
            CswAjaxXml({
                url: o.TabsUrl,
                data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&NodeTypeId=' + o.nodetypeid,
                success: function ($xml) {
                            clearTabs();
                            var $tabdiv = $("<div><ul></ul></div>");
                            $outertabdiv.append($tabdiv);
                            //var firsttabid = null;
                            $xml.children().each(function() { 
                                $this = $(this);
                                $tabdiv.children('ul').append('<li><a href="#'+ $this.attr('id') +'">'+ $this.attr('name') +'</a></li>');
                                $tabdiv.append('<div id="'+ $this.attr('id') +'"><form id="'+ $this.attr('id') +'_form"></div>');
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
            CswAjaxXml({
                url: o.PropsUrl,
                data: 'EditMode='+ o.EditMode +'&NodePk=' + o.nodeid + '&TabId=' + tabid + '&NodeTypeId=' + o.nodetypeid,
                success: function ($xml) {
                            $div = $("#" + tabid);
                            $form = $div.children('form');
                            $form.children().remove();
                            
                            var $table = makeTable(o.ID + '_proptable').appendTo($form);
                            
                            var i = 0;

                            $xml.children().each(function() { 
                                var $this = $(this);
                                var fieldtype = $this.attr('fieldtype');

                                if( fieldtype != 'Image' && 
                                    fieldtype != 'Grid' )
                                {
                                    var $labelcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2 ) - 1);
                                    $labelcell.addClass('propertylabel');
                                    $labelcell.append($this.attr('name'));
                                }

                                var $propcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2));
                                $propcell.addClass('propertyvaluecell');
                                var $propdiv = $('<div/>').appendTo($propcell); 

                                $.CswFieldTypeFactory('make', o.nodeid, fieldtype, $propdiv, $this); 
                                
                            });

                            $table.append('<tr><td><input type="button" id="SaveTab" name="SaveTab" value="Save"/></td></tr>')
                                  .find('#SaveTab')
                                  .click(function() { Save($table, $xml) });

                            // Validation
                            $form.validate({ 
                                             highlight: function(element, errorClass) {
                                                 var $elm = $(element);
                                                 $elm.animate({ backgroundColor: '#ff6666'});
                                             },
                                             unhighlight: function(element, errorClass) {
                                                 var $elm = $(element);
                                                 $elm.css('background-color', '#66ff66');
                                                 setTimeout(function() { $elm.animate({ backgroundColor: 'transparent'}); }, 500);
                                             }
                                           });
                        } // success{}
            }); 
        } // getProps()

        function Save($table, $propsxml)
        {
            $propsxml.children().each(function() { 
                var $propxml = $(this);
                var $propcell = getTableCell($table, $propxml.attr('displayrow'), ($propxml.attr('displaycol') * 2));
                var fieldtype = $propxml.attr('fieldtype');
                var $propdiv = $propcell.children('div');
                  
                $.CswFieldTypeFactory('save', fieldtype, o.nodeid, $propdiv, $propxml);              

            }); // each()

            CswAjaxJSON({
                url: '/NbtWebApp/wsNBT.asmx/SaveProps',
                data: "{ EditMode: '"+ o.EditMode + "', NodePk: '" + o.nodeid + "', NodeTypeId: '"+ o.nodetypeid +"', NewPropsXml: '" + xmlToString($propsxml) + "' }",
                success: function(data) { 
                    o.onSave(data.nodeid); 
                }
            });

        } // Save()

        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

