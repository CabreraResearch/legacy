/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswCheckBoxArray.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeLogicalSet';
    var nameCol = 'name';
    var keyCol = 'key';
  
    var methods = {
        init: function(o) { 

            var $Div = $(this),
                propVals = o.propData.values,
                logicalSetJson = propVals.logicalsetjson;

            var $cbaDiv = $('<div />')
                    .CswCheckBoxArray('init', {
                        ID: o.ID + '_cba',
                        onchange: o.onchange,
					    ReadOnly: o.ReadOnly,
                        dataAry: logicalSetJson,
			            nameCol: nameCol,
			            keyCol: keyCol,
                        Multi: o.Multi
                    });
            
            $Div.contents().remove();
            $Div.append($cbaDiv);
            return $Div;
        },
        save: function(o) { //$propdiv, $xml
            var $CBADiv = o.$propdiv.children('div').first();
            var attributes = { 
					logicalsetjson: {
						data: null,
						columns: null
					}
                };
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            
            if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                attributes.logicalsetjson = formdata;
            }
            preparePropJsonForSave(o.Multi, o.propData, attributes);
            return $(this);
        } // save()
    };
    

    // Method calling logic
    $.fn.CswFieldTypeLogicalSet = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);





