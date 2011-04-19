; (function ($) {
	
    var PluginName = "CswInput";
    
    var inputTypes = {
            button: {value: 0, name: 'button'},
            checkbox: {value: 1, name: 'checkbox'},
            color: {value: 2, name: 'color'},
            date: {value: 3, name: 'date'}, 
            datetime: {value: 4, name: 'datetime'}, 
            'datetime-local': {value: 5, name: 'datetime-local'}, 
            email: {value: 6, name: 'email'}, 
            file: {value: 7, name: 'file'},
            hidden: {value: 8, name: 'hidden'},
            image: {value: 9, name: 'image'},
            month: {value: 10, name: 'month'}, 
            number: {value: 11, name: 'number'}, 
            password: {value: 12, name: 'password'},
            radio: {value: 13, name: 'radio'},
            range: {value: 14, name: 'range'}, 
            reset: {value: 15, name: 'reset'},
            search: {value: 16, name: 'search'},
            submit: {value: 17, name: 'submit'},
            tel: {value: 18, name: 'button'},
            text: {value: 19, name: 'text'},
            time: {value: 20, name: 'time'}, 
            url: {value: 21, name: 'url'},
            week: {value: 22, name: 'week'}
    };
    
    var methods = {
	
        'init': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'suffix': '',
                'type': '',
                'placeholder': '',
                'cssclass': '',
                'text': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $input = $('<input />');
            var elementId = makeId(o);
            if( elementId !== '' ) 
            {
                $input.attr('id',elementId);
                $input.attr('name',elementId);
            }
            if( o.type !== '' ) $input.attr('type', o.type);
            if( o.cssclass !== '' ) $input.attr('class',o.cssclass)
            if( o.placeholder !== '' ) $input.attr('placeholder',o.placeholder);
                    
            $parent.append($input);
            return $input;
       },
        'get': function(options)
        {
            var o = {
                ID: '',
                prefix: '',
                suffix: ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $element;
            
            var elementId = makeId(o);
            if( elementId !== '' && elementId !== undefined)
            {
                $element = $parent.find('#' + elementId);
            }
            return $element;
        }
    };
    	// Method calling logic
	$.fn.CswDOM = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};


})(jQuery);
