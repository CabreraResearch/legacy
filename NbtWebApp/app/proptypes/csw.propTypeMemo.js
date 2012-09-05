/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeMemo';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();

            //var Value = extractCDataValue($xml.children('text'));
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
            var rows = Csw.string(propVals.rows);
            var columns = Csw.string(propVals.columns);

            propDiv.textArea({
                onChange: o.onChange,
                ID: o.ID,
                rows: rows,
                cols: columns,
                value: value,
                disabled: o.ReadOnly,
                required: o.Required,
                readonly: o.ReadOnly
            });
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { text: null };
            var compare = {};
            var textArea = o.propDiv.find('textarea');
            if (false === Csw.isNullOrEmpty(textArea)) {
                attributes.text = textArea.val();
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeMemo = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
