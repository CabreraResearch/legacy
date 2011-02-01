; (function ($) {
        
    var PluginName = 'CswFieldTypeStatic';
    var $propxml;

    var methods = {
        init: function(nodepk, $xml) {
                $propxml = $xml;

                var $Div = $(this);
                $Div.children().remove();
                 
                var ID = $propxml.attr('id');
                //var Required = $propxml.attr('required');
                //var ReadOnly = $propxml.attr('readonly');

                var Text = $propxml.children('text').text();
                var Columns = parseInt( $propxml.children('text').attr('columns') );
                var Rows = parseInt( $propxml.children('text').attr('rows') );

                var overflow = 'auto';
                var width = '';
                var height = '';
                if(Columns > 0 && Rows > 0)
                {
                    overflow = 'scroll';
                    width = Math.round( Columns + 2 - ( Columns / 2.25)) + 'em';
                    height = Math.round( Rows + 2.5 + ( Rows / 5)) + 'em';
                }
                else if(Columns > 0)
                {
                    width = Math.round( Columns - ( Columns / 2.25)) + 'em';
                }
                else if(Rows > 0)
                {
                    height = Math.round( Rows + 0.5 + ( Rows / 5)) + 'em';
                }
            
                var $StaticDiv = $('<div style="overflow: '+ overflow +'; width: '+ width +'; height: '+ height +';">' + Text + '</div>' )
                                    .appendTo($Div); 
            },
        save: function() {
                var $Div = $(this);
                var o = $Div.data(PluginName);
                //var $newPropXml = $propxml;
                //$newPropXml.children('text').text($TextBox.val());
                //SaveProp(nodepk, $newPropXml, function () { });
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeStatic = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
