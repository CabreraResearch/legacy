/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = "CswSelect";
    
    var methods = {
    
        init: function(options) {
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

            var values = makeOptions(o.values);
            setOptions(values, o.selected, $select);
            
            if (isFunction(o.onChange)) {
                 $select.bind('change', function () {
                    var $this = $(this);
                    o.onChange($this);
                 });
            }
            
            $parent.append($select);
            return $select;
        },
        setoptions: function (values, selected, doEmpty) {
            var $select = $(this);
            setOptions(values, selected, $select, doEmpty);
            return $select;
        },
        makeoptions: function (valueArray, selected, doEmpty) {
            var $select = $(this);
            var values = makeOptions(valueArray);
            setOptions(values, selected, $select, doEmpty);
            return $select;
        },
        addoption: function (value, isSelected) {
            var $select = $(this),
                val = makeOption(value);
            addOption(val, isSelected, $select);
        }
    };
    
    function makeOption(opt) {
        var ret, display;
        if (contains(opt, 'value') && contains(opt, 'display')) {
            ret = opt;
        }
        else if (contains(opt, 'value')) {
            display = tryParseString(opt.display, opt.value);
            ret = { value: opt.value, display: display };
        } else {
            ret = { value: opt, display: opt };
        }
        return ret;
    }
    
    function makeOptions(valueArray) {
        var values = [];
        crawlObject(valueArray, function(val) {
            var value = makeOption(val);
            if(false === isNullOrEmpty(value)) {
                values.push(value);
            }
        }, false);
        return values;
    }
    
    function addOption(thisOpt, isSelected, $select) {
        var value = tryParseString(thisOpt.value);
        var display = tryParseString(thisOpt.display);
        var $opt = $('<option value="' + value + '">' + display + '</option>')
                        .appendTo($select);
        if (isSelected) {
            $opt.CswAttrDom('selected', 'selected');
        }
        if (false === isNullOrEmpty(value.data)) {
            $opt.data(value.dataName, value.data);
        }
    }
    
    function setOptions(values, selected, $select, doEmpty) {
        var key, thisOpt;
        if (isArray(values) && values.length > 0) {
            if (doEmpty) {
                $select.empty();
            }
            for (key in values) {
                if (contains(values, key)) {
                    thisOpt = values[key];
                    addOption(thisOpt, (thisOpt.value === selected), $select);
                }
            }
        }
        return $select;
    }
    
    // Method calling logic
    $.fn.CswSelect = function (method) {
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
