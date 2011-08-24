/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
    var pluginName = "CswSelect";
    
    var methods = {
	
		'init': function(options) {
            var o = {
                ID: '',
                selected: '',
                values: [{value: '', display: '', data: {}}],
                cssclass: '',
                onChange: null //function () {}
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            
            var $select = $('<select></select>');
            var elementId = tryParseString(o.ID,'');
            
            $select.CswAttrDom('id',elementId);
            $select.CswAttrDom('name',elementId);
            
            if (!isNullOrEmpty( o.cssclass )) $select.addClass(o.cssclass);
            if (!isNullOrEmpty( o.value )) $select.text( o.value );

		    
		    setOptions(o.values, o.selected, $select);
            
            if (isFunction(o.onChange)) {
                 $select.bind('change', function () {
                    var $this = $(this);
                    o.onChange($this);
                 });
            }
            
            $parent.append($select);
            return $select;
        },
        'setoptions': function (values, selected, doEmpty) {
            var $select = $(this);
            setOptions(values, selected, $select, doEmpty);
            return $select;
        }
        
    };
    
    function setOptions(values, selected, $select, doEmpty) {
        if (isArray(values) && values.length > 0) {
            if (doEmpty) {
                $select.empty();
            }
            for (var key in values) {
                if (values.hasOwnProperty(key)) {
                    var thisOpt = values[key];
                    var value = tryParseString(thisOpt.value);
                    var display = tryParseString(thisOpt.display);
                    var $opt = $('<option value="' + value + '">' + display + '</option>')
                        .appendTo($select);
                    if (value === selected) {
                        $opt.CswAttrDom('selected', 'selected');
                    }
                    if (false === isNullOrEmpty(thisOpt.data)) {
                        $opt.data(thisOpt.dataName, thisOpt.data);
                    }
                }
            }
        }
        return $select;
    }
    
    // Method calling logic
	$.fn.CswSelect = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
		}    
  
	};


})(jQuery);
