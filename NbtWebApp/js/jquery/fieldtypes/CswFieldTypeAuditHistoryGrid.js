/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypeAuditHistoryGrid';

    var methods = {
        init: function(o) {
			
			var $Div = $(this);

			return $Div.CswAuditHistoryGrid({ 
				'ID': o.ID,
				'nodeid': o.nodeid,
				'onEditRow': function(date) {
								$.CswDialog('EditNodeDialog', {
									'nodeid': o.nodeid,
									'onEditNode': o.onEditNode,
									'date': date
								});
							}
			});
        },
        save: function(o) {
                
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeAuditHistoryGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
