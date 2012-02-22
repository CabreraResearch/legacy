/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = "CswInput";
    
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
                'width': '',
                'maxlength': '',
                'autofocus': false,
                'autocomplete': 'on',
                'onChange': null //function() {}
            };
            if (options) $.extend(o, options);

            o.name = tryParseString(o.name,o.ID);
            o.ID = tryParseString(o.ID,o.name);

            var $parent = $(this);
            var $input = $('<input />');

            $input.CswAttrDom('id',o.ID);
            $input.CswAttrDom('name',o.name);
            
            if (false === isNullOrEmpty(o.type)) 
            {
                $input.CswAttrDom('type', o.type.name);
                //cannot style placeholder across all browsers yet. Ignore for now.
                //if( o.type.placeholder === true && o.placeholder !== '')
                //{
                //    $input.CswAttrDom('placeholder',o.placeholder);
                //}
                if (o.type.autocomplete === true && o.autocomplete === 'on')
                {
                    $input.CswAttrDom('autocomplete','on');
                }
                
                o.value = tryParseString(o.value, '');
                if (isTrue(o.type.value.required) || ( !isNullOrEmpty( o.value )))
                {
                    $input.val(o.value);
                }

                o.width = tryParseString(o.width, o.type.defaultwidth);
            }

            if (!isNullOrEmpty(o.cssclass)) $input.addClass(o.cssclass);
            if (!isNullOrEmpty(o.width)) $input.css('width', o.width);
            if (isTrue(o.autofocus)) $input.CswAttrDom('autofocus', o.autofocus);
            if (false === isNullOrEmpty(o.maxlength)) $input.CswAttrDom('maxlength', +o.maxlength);
            if (isFunction(o.onChange)) $input.change( o.onChange );
                                
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
    };


})(jQuery);
