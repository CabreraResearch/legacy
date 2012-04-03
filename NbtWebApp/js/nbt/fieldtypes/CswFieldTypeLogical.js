/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeLogical';

    var methods = {
        init: function (o) { //nodepk = o.nodeid, $xml = o.propData, onChange = o.onChange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var checkOpt = {
                Checked: (false === o.Multi) ? Csw.string(propVals.checked).trim() : null,
                Required: Csw.bool(o.Required),
                ReadOnly: Csw.bool(o.ReadOnly),
                Multi: o.Multi,
                onChange: o.onChange
            };

            propDiv.triStateCheckBox(checkOpt);
        },
        save: function (o) {
            var propDiv = o.propDiv.find('.CswTristateCheckBox');
            var attributes = { checked: Csw.string(propDiv.propNonDom('value')) };
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeLogical = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);





