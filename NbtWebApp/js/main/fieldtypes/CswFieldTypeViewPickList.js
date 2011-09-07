/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeViewPickList';
	var nameCol = "View Name";
    var keyCol = "nodeviewid";
    var valueCol = "Include";

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
			var optionData = propVals.options;
			var selectMode = propVals.selectmode;
			var $cbaDiv = $('<div />')
							.appendTo($Div)
                            .CswCheckBoxArray('init', {
				                ID: o.ID + '_cba',
				                UseRadios: (selectMode === 'Single'),
				                Required: o.Required,
				                ReadOnly: o.ReadOnly,
                                Multi: o.Multi,
				                onchange: o.onchange,
                                dataAry: optionData,
			                    nameCol: nameCol,
			                    keyCol: keyCol,
                                valueCol: valueCol
			                });
            return $Div;    
        },
        'save': function(o) {
			var $cbaDiv = o.$propdiv.children('div').first();
			var formdata = $cbaDiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            o.propData.values.options = formdata;
            return $(this);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeViewPickList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
