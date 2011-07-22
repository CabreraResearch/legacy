/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
    var PluginName = "CswInput";
    
    var methods = {
	
        'init': function(options) 
		{
            var o = {
                'ID': '',
				'name': '',
                'type': CswInput_Types.text,
                'placeholder': '',
                'cssclass': '',
                'value': '',
                'size': '',
                'autofocus': false,
                'autocomplete': 'on',
                'onChange': function() {}
            };
            if (options) $.extend(o, options);

			o.name = tryParseString(o.name,o.ID);
            o.ID = tryParseString(o.ID,o.name);

            var $parent = $(this);
            var $input = $('<input />');

            $input.CswAttrDom('id',o.ID);
            $input.CswAttrDom('name',o.name);
            
            if( !isNullOrEmpty( o.type ) ) 
            {
                $input.CswAttrDom('type', o.type.name);
                //cannot style placeholder across all browsers yet. Ignore for now.
                //if( o.type.placeholder === true && o.placeholder !== '')
                //{
                //    $input.CswAttrDom('placeholder',o.placeholder);
                //}
                if( o.type.autocomplete === true && o.autocomplete === 'on' )
                {
                    $input.CswAttrDom('autocomplete','on');
                }
                
                o.value = tryParseString(o.value, '');
                if( isTrue( o.type.value.required ) || ( !isNullOrEmpty( o.value ) ) )
                {
                    $input.val(o.value);
                }

                o.size = tryParseString( o.size, o.type.defaultwidth);
            }

            if( !isNullOrEmpty( o.cssclass ) ) $input.addClass(o.cssclass);
            if( !isNullOrEmpty( o.size ) ) $input.CswAttrDom('size', o.size);
            if( isTrue( o.autofocus ) ) $input.CswAttrDom('autofocus', o.autofocus);
            if( !isNullOrEmpty( o.onChange ) ) $input.change( function () { o.onChange() } );
                                
            $parent.append($input);
            return $input;
        }

    };
    	// Method calling logic
	$.fn.CswInput = function (method) { /// <param name="$" type="jQuery" />
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
    };


})(jQuery);
