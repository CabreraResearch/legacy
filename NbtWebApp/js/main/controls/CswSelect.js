/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = "CswSelect";
    
    var methods = {
    
        init: function (options) {
            var o = {
                ID: '',
                selected: '',
                values: [{value: '', display: '', data: {}}],
                cssclass: '',
                multiple: false,
                onChange: null //function () {}
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            
            var $select = $('<select></select>');
            var elementId = Csw.string(o.ID);
            
            $select.CswAttrDom('id',elementId);
            $select.CswAttrDom('name',elementId);
            
            if (false === Csw.isNullOrEmpty(o.cssclass)) {
                $select.addClass(o.cssclass);
            }
            
            if (false === Csw.isNullOrEmpty(o.value)) {
                $select.text(o.value);
            }
            
            var values = makeOptions(o.values);
            setOptions(values, o.selected, $select);
            
            if (Csw.isFunction(o.onChange)) {
                 $select.bind('change', function () {
                    var $this = $(this);
                    o.onChange($this);
                 });
            }
            
            $parent.append($select);
            
            if(Csw.bool(o.multiple)) {
                $select.CswAttrDom('multiple', 'multiple').multiselect();    
            }
            
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
        var ret, display, value;
        if (Csw.contains(opt, 'value') && Csw.contains(opt, 'display')) {
            ret = opt;
        }
        else if (Csw.contains(opt, 'value')) {
            value = Csw.string(opt.value);
            ret = { value: value, display: value };
        }
        else if (Csw.contains(opt, 'display')) {
            display = Csw.string(opt.display);
            ret = { value: display, display: display };
        } else {
            ret = { value: opt, display: opt };
        }
        return ret;
    }
    
    function makeOptions(valueArray) {
        var values = [];
        Csw.crawlObject(valueArray, function (val) {
            var value = makeOption(val);
            if(false === Csw.isNullOrEmpty(value)) {
                values.push(value);
            }
        }, false);
        return values;
    }
    
    function addOption(thisOpt, isSelected, $select) {
        var value = Csw.string(thisOpt.value);
        var display = Csw.string(thisOpt.display);
        var $opt = $('<option value="' + value + '">' + display + '</option>')
                        .appendTo($select);
        if (isSelected) {
            $opt.CswAttrDom('selected', 'selected');
        }
        if (false === Csw.isNullOrEmpty(value.data)) {
            $opt.data(value.dataName, value.data);
        }
    }
    
    function setOptions(values, selected, $select, doEmpty) {
        if (Csw.isArray(values) && values.length > 0) {
            if (doEmpty) {
                $select.empty();
            }
            Csw.each(values, function (thisOpt) {
                var opt = makeOption(thisOpt);       
                addOption(opt, (opt.value === selected), $select);
            });
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };


})(jQuery);
