; (function ($) {
        
    var PluginName = 'CswFieldTypeStatic';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
                
            var $Div = $(this);
            $Div.contents().remove();
                 
            var Text = o.propData.children('text').text().trim();
            var Columns = parseInt( o.propData.children('text').CswAttrXml('columns') );
            var Rows = parseInt( o.propData.children('text').CswAttrXml('rows') );

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
            
            var $StaticDiv = $('<div class="staticvalue" style="overflow: '+ overflow +'; width: '+ width +'; height: '+ height +';">' + Text + '</div>' )
                            .appendTo($Div); 
        },
        save: function(o) {
                // no changes to save
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
