/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeStatic';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var text = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
            var columns = Csw.number(propVals.columns);
            var rows = Csw.number(propVals.rows);

            var overflow = 'auto';
            var width = '';
            var height = '';
            if (columns > 0 && rows > 0) {
                overflow = 'scroll';
                width = Math.round(columns + 2 - (columns / 2.25)) + 'em';
                height = Math.round(rows + 2.5 + (rows / 5)) + 'em';
            }
            else if (columns > 0) {
                width = Math.round(columns - (columns / 2.25)) + 'em';
            }
            else if (rows > 0) {
                height = Math.round(rows + 0.5 + (rows / 5)) + 'em';
            }
            
            propDiv.div({
                cssclass: 'staticvalue',
                text: Csw.string(text, '&nbsp;&nbsp;')
            }).css({
                overflow: overflow,
                width: width,
                height: height
            });
            
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeStatic = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
