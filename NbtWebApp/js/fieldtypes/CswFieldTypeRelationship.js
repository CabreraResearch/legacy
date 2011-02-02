; (function ($) {
        
    $.fn.CswFieldTypeRelationship = function (method) {

        var PluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function(nodepk, $xml) {
            
                    var $Div = $(this);
                    $Div.children().remove();

                    var ID = $xml.attr('id');
                    var Required = $xml.attr('required');
                    var ReadOnly = $xml.attr('readonly');

                    var SelectedNodeId = $xml.children('nodeid').text();
                    var SelectedName = $xml.children('name').text();
                    var $Options = $xml.children('options');

                    if(ReadOnly)
                    {
                        $Div.append(SelectedName);
                    }
                    else 
                    {
                        var $SelectBox = $('<select id="'+ ID +'" name="'+ ID +'" class="selectinput" />"' )
                                           .appendTo($Div);

                        $Options.children().each(function() {
                            var $this = $(this);
                            $SelectBox.append('<option value="' + $this.attr('id') + '">' + $this.attr('value') + '</option>');
                        });

                        $SelectBox.val( SelectedNodeId );

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
