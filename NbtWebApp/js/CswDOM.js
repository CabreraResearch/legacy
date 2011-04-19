; (function ($) {
	
    var PluginName = "CswDOM";
    
    var methods = {
	
		'div': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'suffix': '',
                'value': '',
                'cssclass': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            
            var $div = $('<div></div>');
            var elementId = makeId(o);
            if( elementId !== '' ) 
            {
                $div.attr('id',elementId);
                $div.attr('name',elementId);
            }
            if( o.cssclass !== '' ) $div.attr('class',o.cssclass);
            if( o.value !== '' ) $div.text( o.value );
                    
            $parent.append($div);
            return $div;
        },
        'span': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'suffix': '',
                'value': '',
                'cssclass': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $span = $('<span></span>');
            var elementId = makeId(o);
            if( elementId !== '' ) 
            {
                $span.attr('id',elementId);
                $span.attr('name',elementId);
            }
            if( o.cssclass !== '' ) $span.attr('class',o.cssclass);
            if( o.value !== '' ) $span.text( o.value );
                    
            $parent.append($span);
            return $span;
        },
        'link': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'suffix': '',
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
            var $link = $('<a></a>');
            
            var elementId = makeId(o);
            if( elementId !== '' ) $link.attr('id',elementId);
            if( o.href !== '' ) $link.attr('href', o.href);
            if( o.value !== '' ) $link.text(o.value);
            if( o.cssclass !== '' ) $link.attr('class',o.cssclass);
            if( o.type !== '' ) $link.attr('type',o.type);
            if( o.rel !== '' ) $link.attr('rel',o.rel);
            if( o.media !== '' ) $link.attr('media',o.media);
            if( o.target !== '' ) $link.attr('target',o.target);
            if( o.onClick !== undefined ) 
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
                prefix: '',
                suffix: ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $children;

            var elementId = makeId(o);
            if( elementId !== '' )
            {
                $children = $parent.children('#' + elementId);
            }
            return $children;
        },
        'findelement': function(options)
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
            if( elementId !== '' )
            {
                $children = $parent.find('#' + elementId);
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
