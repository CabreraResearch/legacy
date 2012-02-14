/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeComposite';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
            propDiv.append(value);

        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeComposite = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
