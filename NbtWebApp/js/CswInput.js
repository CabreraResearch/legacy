/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

// for CswInput
var CswInput_Types = {
    button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' },
    checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true}, defaultwidth: '' },
    color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '' },
    date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '200px' },
    datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '200px' },
    'datetime-local': { value: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '200px' },
    email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '200px' },
    file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false}, defaultwidth: '' },
    hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' },
    image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' },
    month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' },
    number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '200px' },
    password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true}, defaultwidth: '200px' },
    radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true}, defaultwidth: '' },
    range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '' },
    reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' },
    search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '' },
    submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' },
    tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '' },
    text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '200px' },
    url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true}, defaultwidth: '200px' },
    week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true}, defaultwidth: '' }
};

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
                'width': '',
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

                if( o.type === CswInput_Types.text && !isNullOrEmpty( o.width ) && !isNullOrEmpty(o.type.defaultwidth) )
                {
                    o.width = o.type.defaultwidth;
                }
            }
            
            if( !isNullOrEmpty( o.cssclass ) ) $input.addClass(o.cssclass);
            if( !isNullOrEmpty( o.width ) ) $input.css('width', o.width);
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
