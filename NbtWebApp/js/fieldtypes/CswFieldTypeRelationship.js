; (function ($) {
        
    $.fn.CswFieldTypeRelationship = function (method) {

        var PluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function(nodepk, $xml) {
            
                    var $Div = $(this);
                    $Div.children().remove();

                    var ID = $xml.attr('id');
                    var Required = ($xml.attr('required') == "true");
                    var ReadOnly = ($xml.attr('readonly') == "true");

                    var SelectedNodeId = $xml.children('nodeid').text();
                    var SelectedName = $xml.children('name').text();
                    var $Options = $xml.children('options');

                    if(ReadOnly)
                    {
                        $Div.append(SelectedName);
                    }
                    else 
                    {
                        var $mytable = makeTable(ID + '_tbl').appendTo($Div);

                        var $selectcell = getTableCell($mytable, 1, 1);
                        var $SelectBox = $('<select id="'+ ID +'" name="'+ ID +'" class="selectinput" />"' )
                                           .appendTo($selectcell);

                        $Options.children().each(function() {
                            var $this = $(this);
                            $SelectBox.append('<option value="' + $this.attr('id') + '">' + $this.attr('value') + '</option>');
                        });

                        $SelectBox.val( SelectedNodeId );

                        var $addcell = getTableCell($mytable, 1, 2);
                        var $AddButton = $('<div />').appendTo($addcell);
                        $AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
                                                    AlternateText: "Add New",
                                                    onClick: onAdd 
                                                  });

                        if(Required)
                        {
                            $SelectBox.addClass("required");
                        }
                    }

                },
            save: function($propdiv, $xml) {
                    var $SelectBox = $propdiv.find('select');
                    $xml.children('nodeid').text($SelectBox.val());
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
