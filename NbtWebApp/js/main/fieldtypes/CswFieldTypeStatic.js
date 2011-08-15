﻿/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeStatic';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
                
            var $Div = $(this);
            $Div.contents().remove();
                 
            var text = tryParseString(o.propData.text).trim();
            var columns = parseInt( o.propData.columns);
            var rows = parseInt( o.propData.rows);

            var overflow = 'auto';
            var width = '';
            var height = '';
            if(columns > 0 && rows > 0)
            {
                overflow = 'scroll';
                width = Math.round( columns + 2 - ( columns / 2.25)) + 'em';
                height = Math.round( rows + 2.5 + ( rows / 5)) + 'em';
            }
            else if(columns > 0)
            {
                width = Math.round( columns - ( columns / 2.25)) + 'em';
            }
            else if(rows > 0)
            {
                height = Math.round( rows + 0.5 + ( rows / 5)) + 'em';
            }
            
            var $StaticDiv = $('<div class="staticvalue" style="overflow: '+ overflow +'; width: '+ width +'; height: '+ height +';">' + text + '</div>' )
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
