/// <reference path="~app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeViewPickList';
    var nameCol = 'label';
    var keyCol = 'key';
    var valueCol = 'value';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var optionData = propVals.options;
            var selectMode = propVals.selectmode;
            propDiv.div()
                .checkBoxArray({
                    ID: o.ID + '_cba',
                    UseRadios: (selectMode === 'Single'),
                    Required: o.Required,
                    ReadOnly: o.ReadOnly,
                    Multi: o.Multi,
                    onChange: o.onChange,
                    dataAry: optionData,
                    nameCol: nameCol,
                    keyCol: keyCol,
                    valCol: valueCol,
                    valColName: 'Include'
                });
            return propDiv;
        },
        'save': function (o) {
            var attributes = { options: null };
            var compare = {};
            var formdata = Csw.clientDb.getItem(o.ID + '_cba' + '_cswCbaArrayDataStore'); 
            if (false === o.Multi || false === formdata.MultiIsUnchanged) {
                attributes.options = formdata.data;
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeViewPickList = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
