; (function ($) {
	
    var PluginName = "CswDOM";
    var Delimiter = '_';
    
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
	
		'div': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'value': '',
                'cssclass': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $div = $('<div></div>');
            if( elementId != Delimiter ) 
            {
                $div.attr('id',elementId);
                $div.attr('name',elementId);
            }
            if( o.cssclass != '' ) $div.attr('class',o.cssclass);
            if( o.value != '' ) $div.text( o.value );
                    
            $parent.append($div);
            return $div;
        },
        'span': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'value': '',
                'cssclass': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $span = $('<span></span>');
            if( elementId != Delimiter ) 
            {
                $span.attr('id',elementId);
                $span.attr('name',elementId);
            }
            if( o.cssclass != '' ) $span.attr('class',o.cssclass);
            if( o.value != '' ) $span.text( o.value );
                    
            $parent.append($span);
            return $span;
        },
        'input': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'type': '',
                'placeholder': '',
                'cssclass': '',
                'value': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $input = $('<input />');
            if( elementId != Delimiter ) 
            {
                $input.attr('id',elementId);
                $input.attr('name',elementId);
            }
            if( o.type != '' ) $input.attr('type', o.type);
            if( o.cssclass != '' ) $input.attr('class',o.cssclass)
            if( o.placeholder != '' ) $input.attr('placeholder',o.placeholder);
                    
            $parent.append($input);
            return $input;
        },
        'link': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'value': '',
                'type': '', //MIME type
                'media': 'all',
                'target': '',
                'rel': '',
                'cssclass': '',
                'onClick': function() {}
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $link = $('<a></a>');
            
            if( elementId != Delimiter ) $link.attr('id',elementId);
            if( o.href != '' ) $link.attr('href', o.href);
            if( o.value != '' ) $link.text(o.value);
            if( o.cssclass != '' ) $link.attr('class',o.cssclass);
            if( o.type != '' ) $link.attr('type',o.type);
            if( o.rel != '' ) $link.attr('rel',o.rel);
            if( o.media != '' ) $link.attr('media',o.media);
            if( o.target != '' ) $link.attr('target',o.target);
            if( o.onClick != undefined ) 
            {
                $link.click( function() {
                             o.onClick();
                });
            }
                    
            $parent.append($link);
            return $link;
        },
        'break': function(options) 
		{
            var o = {
                count: 1
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var i = 0;

            while(i < o.count)  
            {
                $parent.append( $('<br/>') );
                i++;
            }
        },
        'getchildren': function(options)
        {
            var o = {
                ID: '',
                prefix: ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = '#' + o.prefix + Delimiter + o.ID;
            var $children;
            if( elementId != '#' + Delimiter )
            {
                $children = $parent.children(elementId);
            }
            return $children;
        },
        'findelement': function(options)
        {
            var o = {
                ID: '',
                prefix: ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = '#' + o.prefix + Delimiter + o.ID;
            var $element;
            if( elementId != '#' + Delimiter )
            {
                $element = $parent.find(elementId);
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
