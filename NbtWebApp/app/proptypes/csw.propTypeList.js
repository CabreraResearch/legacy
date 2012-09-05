/// <reference path="~app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeList';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
            var options = Csw.string(propVals.options).trim();

            if (o.ReadOnly) {
                propDiv.append(value);
            } else {
                var values = options.split(',');
                if (o.Multi) {
                    values.push(Csw.enums.multiEditDefaultValue);
                }
                var selectBox = propDiv.select({
                    ID: o.ID,
                    cssclass: 'selectinput',
                    onChange: o.onChange,
                    values: values,
                    selected: value
                });

                if (o.Required) {
                    selectBox.addClass("required");
                }
            }

        },
        save: function (o) { //$propdiv, $xml
            var attributes = { value: null };
            var compare = {};
            var selectBox = o.propDiv.find('select');
            if (false === Csw.isNullOrEmpty(selectBox)) {
                attributes.value = selectBox.val();
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeList = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
