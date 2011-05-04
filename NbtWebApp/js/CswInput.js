/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

// for CswInput
var CswInput_Types = {
    button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true} },
    color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true} },
    date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true} },
    datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    'datetime-local': { value: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true} },
    email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true} },
    file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false} },
    hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true} },
    radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true} },
    range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true} },
    reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true} },
    submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true} },
    tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true} },
    text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true} },
    time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true} },
    url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true} },
    week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true} }
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
                'width': "200px",
                'autofocus': false,
                'autocomplete': 'on',
                'onChange': function() {}
            };
            if (options) $.extend(o, options);

			if(isNullOrEmpty(o.name)) o.name = o.ID;

            var $parent = $(this);
            var $input = $('<input />');
            if( o.ID !== '' ) 
            {
                $input.attr('id',o.ID);
                $input.attr('name',o.name);
            }
            
            if( o.type !== '' && o.type !== undefined ) 
            {
                $input.attr('type', o.type.name);
                //cannot style placeholder across all browsers yet. Ignore for now.
                //if( o.type.placeholder === true && o.placeholder !== '')
                //{
                //    $input.attr('placeholder',o.placeholder);
                //}
                if( o.type.autocomplete === true && o.autocomplete === 'on' )
                {
                    $input.attr('autocomplete','on');
                }
                if( o.type.value.required === true || ( o.value !== '' && o.value !== undefined ) )
                {
                    if( o.value === undefined ) o.value = '';
                    $input.val(o.value);
                }
            }
            
            if( o.cssclass !== '' ) $input.attr('class',o.cssclass);
            if( o.width !== '' ) $input.attr({width: o.width});
            if( o.autofocus === true ) $input.attr('autofocus');
            if( o.onChange !== undefined ) $input.change( function () { o.onChange() } );
                                
            $parent.append($input);
            return $input;
        },
        'findandget': function(options)
        {
            var o = {
                ID: ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $input;

            if( o.ID !== '' && o.ID !== undefined)
            {
                $input = $parent.find('#' + o.ID);
            }
            return $input;
        },
        'findandset': function(options)
        {
            var succeeded = false;
            var o = {
                ID: '',
                value: ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $input;

            if( o.ID !== '' && o.ID !== undefined &&
                o.value !== undefined /*empty is OK*/ )
            {
                $input = $parent.find('#' + o.ID);
                if( $input.size() > 0 )
                {
                    $input.val(o.value);
                    succeeded = true;
                }
            }
            return succeeded;
        },
        'set': function(options)
        {
            var succeeded = false;
            var o = {
                value: ''
            };
            if (options) $.extend(o, options);

            var $input = $(this);

            if( $input.size() > 0 && o.value !== undefined /*empty is OK*/ )
            {
                $input.val(o.value);
                succeeded = true;
            }
            return succeeded;
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
