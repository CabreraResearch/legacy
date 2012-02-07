/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeLogicalSet';
    var nameCol = 'name';
    var keyCol = 'key';
  
    var methods = {
        init: function (o) { 

            var $Div = $(this),
                propVals = o.propData.values,
                logicalSetJson = propVals.logicalsetjson;

            var $cbaDiv = $('<div />')
                    .CswCheckBoxArray('init', {
                        ID: o.ID + '_cba',
                        onchange: o.onchange,
                        ReadOnly: o.ReadOnly,
                        dataAry: logicalSetJson.data,
                        cols: logicalSetJson.columns,
                        nameCol: nameCol,
                        keyCol: keyCol,
                        Multi: o.Multi
                    });
            
            $Div.contents().remove();
            $Div.append($cbaDiv);
            return $Div;
        },
        save: function (o) { //$propdiv, $xml
            var $CBADiv = o.$propdiv.children('div').first();
            var attributes = { logicalsetjson: null };
            var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            
            if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                attributes.logicalsetjson = formdata;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);





