/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
    var PluginName = "CswSelect";
    
    var methods = {
	
		'init': function(options) 
		{
            var o = {
                'ID': '',
                'selected': '',
                'values': [{value: '', display: ''}],
                'cssclass': '',
                'onChange': function () {}
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            
            var $select = $('<select></select>');
            var elementId = tryParseString(o.ID,'');
            
            $select.CswAttrDom('id',elementId);
            $select.CswAttrDom('name',elementId);
            
            if (!isNullOrEmpty( o.cssclass )) $select.addClass(o.cssclass);
            if (!isNullOrEmpty( o.value )) $select.text( o.value );

            for(var key in o.values)
            {
                if (o.values.hasOwnProperty(key)) {
                    var value = o.values[key].value;
                    var display = o.values[key].display;
                    var $opt = $('<option value="' + value + '">' + display + '</option>')
                        .appendTo($select);
                    if (value === o.selected) {
                        $opt.CswAttrDom('selected', 'selected');
                    }
                }
            }
            
            if( !isNullOrEmpty( o.onChange ) ) {
                 $select.bind('change', function () {
                    var $this = $(this);
                    o.onChange($this);
                 });
            }
            
            $parent.append($select);
            return $select;
        }
    };
    	// Method calling logic
	$.fn.CswSelect = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};


})(jQuery);
