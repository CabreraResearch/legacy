/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { /// <param name="$" type="jQuery" />
    
    var PluginName = "CswSpan";
    
    var methods = {
    
        'init': function(options) 
        {
            var o = {
                'ID': '',
                'value': '',
                'cssclass': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $span = $('<span></span>');
            var elementId = tryParseString(o.ID,'');
            if( elementId !== '' ) 
            {
                $span.CswAttrDom('id',elementId);
                $span.CswAttrDom('name',elementId);
            }
            if( !isNullOrEmpty( o.cssclass ) ) $span.addClass(o.cssclass);
            if( !isNullOrEmpty( o.value ) ) $span.text( o.value );
            $parent.append($span);
            return $span;
        }
    };
        // Method calling logic
    $.fn.CswSpan = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };


})(jQuery);
