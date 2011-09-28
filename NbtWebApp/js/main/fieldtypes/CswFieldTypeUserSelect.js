/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeUserSelect';
    var nameCol = 'label';
    var keyCol = "key";
    var stringKeyCol = "UserIdString";
    var valueCol = "value";

    var methods = {
        'init': function(o) { 

            var $Div = $(this);
            
            var propVals = o.propData.values;
            var options = propVals.options;
                
            var $cbaDiv = $('<div />')
                    .CswCheckBoxArray('init', {
                        ID: o.ID + '_cba',
                        UseRadios: false,
                        Required: o.Required,
                        Multi: o.Multi,
                        ReadOnly: o.ReadOnly,
                        onchange: o.onchange,
                        dataAry: options,
                        nameCol: nameCol,
                        keyCol: keyCol,
                        valCol: valueCol,
                        valColName: 'Include'
                    });
            
            $Div.contents().remove();
            $Div.append($cbaDiv);
            return $Div;
        },
        'save': function(o) {
            var attributes = { options: null };
            var $CBADiv = o.$propdiv.children('div').first();
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                attributes.options = formdata.data;
            } 
            preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeUserSelect = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
