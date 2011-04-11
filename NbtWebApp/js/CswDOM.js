; (function ($) {
	
    var PluginName = "CswDOM";
    var Delimiter = '_';
    var methods = {
	
		'div': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'value': '',
                'class': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $div = $('<div></div>').attr('id',elementId)
                                       .attr('name',elementId)
                                       .attr('class',o.class)
                                       .val( o.value );
                    
            $parent.append($div);
            return $div;
        },
        'span': function(options) 
		{
            var o = {
                'ID': '',
                'prefix': '',
                'value': '',
                'class': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $span = $('<span></span>').attr('id',elementId)
                                        .attr('name',elementId)
                                        .attr('class',o.class)
                                        .val( o.value );
                    
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
                'class': ''
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $input = $('<input />').attr('id',elementId)
                                       .attr('name',elementId)
                                       .attr('type', o.type)
                                       .attr('class',o.class)
                                       .attr('placeholder',o.placeholder);
                    
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
                'class': '',
                'onClick': function() {}
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var elementId = o.prefix + Delimiter + o.ID;
            var $link = $('<a></a>').attr('id',elementId)
                                    .attr('href', o.href)
                                    .attr('class',o.class)
                                    .attr('type',o.type)
                                    .attr('rel',o.rel)
                                    .attr('media',o.media)
                                    .attr('target',o.target)
                                    .val(o.value)
                                    .click( function() {
                                        o.onClick();
                                    });
                    
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
