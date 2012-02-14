/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";    
    var pluginName = 'CswFieldTypeMemo';

    var methods = {
        init: function (o) {
        
            var $Div = $(this);
            $Div.contents().remove();

            //var Value = extractCDataValue($xml.children('text'));
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
            var rows = Csw.string(propVals.rows);
            var columns = Csw.string(propVals.columns);

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
                var $TextArea = $('<textarea id="'+ o.ID +'" name="' + o.ID + '" rows="'+rows+'" cols="'+columns+'">'+ value +'</textarea>' )
                                    .appendTo($Div)
                                    .change(o.onChange); 
                if(o.Required)
                {
                    $TextArea.addClass("required");
                }
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { text: null };
            var $TextArea = o.$propdiv.find('textarea');
            if (false === Csw.isNullOrEmpty($TextArea)) {
                attributes.text = $TextArea.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMemo = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
