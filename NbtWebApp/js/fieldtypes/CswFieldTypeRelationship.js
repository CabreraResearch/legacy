; (function ($) {
        
    $.fn.CswFieldTypeRelationship = function (method) {

        var PluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function(o) { //nodepk = o.nodeid, o.$propxml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
            
                var $Div = $(this);
                $Div.contents().remove();

                var SelectedNodeId = o.$propxml.children('nodeid').text().trim();
                var SelectedName = o.$propxml.children('name').text().trim();
                var $Options = o.$propxml.children('options');

                if(o.ReadOnly)
                {
                    $Div.append(SelectedName);
                }
                else 
                {
                    var $table = $.CswTable({ ID: o.ID + '_tbl' })
                                    .appendTo($Div);

                    var $selectcell = $table.CswTable('cell', 1, 1);
                    var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                        .appendTo($selectcell)
                                        .change(o.onchange);

                    $Options.children().each(function() {
                        var $this = $(this);
                        $SelectBox.append('<option value="' + $this.attr('id') + '">' + $this.attr('value') + '</option>');
                    });

                    $SelectBox.val( SelectedNodeId );

                    var $addcell = $table.CswTable('cell', 1, 2);
                    var $AddButton = $('<div />').appendTo($addcell);
                    $AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
                                                AlternateText: "Add New",
                                                onClick: onAdd 
                                                });

                    if(o.Required)
                    {
                        $SelectBox.addClass("required");
                    }
                }

            },
            save: function(o) {
                    var $SelectBox = o.$propdiv.find('select');
                    o.$propxml.children('nodeid').text($SelectBox.val());
                }
        };
    

        function onAdd($ImageDiv)
        {
            alert('This function has not been implemented yet.');
        }

        // Method calling logic
        if ( methods[method] ) {
            return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
