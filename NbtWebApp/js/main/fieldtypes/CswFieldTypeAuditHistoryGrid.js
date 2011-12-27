/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeAuditHistoryGrid';

    var methods = {
        init: function(o) {
			
			var $Div = $(this);
            var ret = '';
            if (false === o.Multi) {
                ret = $Div.CswAuditHistoryGrid({
                        ID: o.ID,
                        nodeid: o.nodeid,
						EditMode: o.EditMode,
                        onEditRow: function(date) {
                            $.CswDialog('EditNodeDialog', {
                                nodeids: [ o.nodeid ],
                                onEditNode: o.onEditNode,
                                date: date
                            });
                        }
                    });
            }
            return ret;
        },
        save: function(o) {
            preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeAuditHistoryGrid = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
