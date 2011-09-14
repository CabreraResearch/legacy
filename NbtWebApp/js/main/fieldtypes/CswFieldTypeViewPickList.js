/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeViewPickList';
	var nameCol = 'label';
    var keyCol = 'key';
    var valueCol = 'value';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            var propVals = o.propData.values;
			var optionData = propVals.options;
			var selectMode = propVals.selectmode;
			var $cbaDiv = $('<div />')
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
                                valueCol: valueCol,
			                    valueColName: 'View Name'
			                });
            $Div.contents().remove();
            $Div.append($cbaDiv);
            return $Div;    
        },
        'save': function(o) {
            var attributes = { options: null };
            var $cbaDiv = o.$propdiv.children('div').first();
			var formdata = $cbaDiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                attributes.options = formdata.data;
            } 
            preparePropJsonForSave(o.Multi, o.propData, attributes);
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
