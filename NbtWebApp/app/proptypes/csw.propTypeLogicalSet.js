/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeLogicalSet';
    var nameCol = 'name';
    var keyCol = 'key';
  
    var methods = {
        init: function (o) { 

            var propDiv =  o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values,
                logicalSetJson = propVals.logicalsetjson;

            propDiv.div()
                   .checkBoxArray({
                        ID: o.ID + '_cba',
                        onChange: o.onChange,
                        ReadOnly: o.ReadOnly,
                        dataAry: logicalSetJson.data,
                        cols: logicalSetJson.columns,
                        nameCol: nameCol,
                        keyCol: keyCol,
                        Multi: o.Multi
                    });

            return propDiv;
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { logicalsetjson: null };
            var compare = {};
            var formdata = Csw.clientDb.getItem(o.ID + '_cba' + '_cswCbaArrayDataStore'); 
            
            if(false === Csw.isNullOrEmpty(formdata) && (
               false === o.Multi || 
                    false === formdata.MultiIsUnchanged)) {
                attributes.logicalsetjson = formdata;
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
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





