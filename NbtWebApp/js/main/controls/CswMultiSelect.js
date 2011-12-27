/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = "CswMultiSelect";
    
    var methods = {
    
        init: function(options) {
            var o = {
                ID: '',
                values: [{value: '', text: '', selected: '', disabled: ''}],
                cssclass: '',
                isMultiEdit: false,
                onChange: null //function () {}
            };
            if (options) $.extend(o, options);
            
            
            var elementId = tryParseString(o.ID),
                $select = $('<select multiple="multiple" id="' + elementId + '" name="' + elementId + '"></select>'),
                optionCount = 0,
                isMultiEdit = isTrue(o.isMultiEdit);
            
            if(false === isNullOrEmpty($(this), true)) {
                $(this).append($select);
            }
            
            if (false === isNullOrEmpty(o.cssclass)) {
                $select.addClass(o.cssclass);
            }
            
            if (isFunction(o.onChange)) {
                 $select.bind('change', function () {
                    var $this = $(this);
                    o.onChange($this);
                 });
            }

            each(o.values, function(option) {
                var $option,
                    value = tryParseString(option.value),
                    text = tryParseString(option.text, value);
                if(false === isNullOrEmpty(value)) {
                    $option = $('<option value="' + value + '" text="' + text + '">' + text + '</option>');
                    if(isTrue(option.disabled)) {
                        $option.CswAttrDom('disabled', 'disabled');
                    }
                    if(isTrue(option.selected) && 
                       false === isMultiEdit) {
                        $option.CswAttrDom('selected', 'selected');
                    }
                    $select.append($option);
                    optionCount += 1;
                }
            });
            
            if(optionCount > 20) {
                $select.multiselect().multiselectfilter();
            } else {
                $select.multiselect();    
            }
            
            return $select;
        },
        val: function ($element) {
            var $select = $element || $(this),
                valArray = $select.val().sort();
            return valArray.join(',');
        }
        
    };
   
    // Method calling logic
    $.fn.CswMultiSelect = function (method) {
        ///<summary>Generates and manipulates a well-formed pick list</summary>
        ///<param name="method">Options: 'init', 'setoptions', 'makeoptions'</param>
        ///<returns type="JQuery">A JQuery select element</returns>
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };


})(jQuery);
