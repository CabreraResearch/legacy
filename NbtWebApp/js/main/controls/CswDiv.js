/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = "CswDiv";
    
    var methods = {
    
        'init': function (options) {
            var o = {
                'ID': '',
                'value': '',
                'cssclass': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            
            var $div = $('<div></div>');
            var elementId = Csw.string(o.ID,'');
            
            $div.CswAttrDom('id',elementId);
            $div.CswAttrDom('name',elementId);
            
            if( !Csw.isNullOrEmpty( o.cssclass ) ) $div.addClass(o.cssclass);
            if( !Csw.isNullOrEmpty( o.value ) ) $div.text( o.value );
                    
            $parent.append($div);
            return $div;
        }
    };
        // Method calling logic
    $.fn.CswDiv = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };


})(jQuery);
