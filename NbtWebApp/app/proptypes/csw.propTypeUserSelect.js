/// <reference path="~app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeUserSelect';
    var nameCol = 'label';
    var keyCol = "key";
    /*  var stringKeyCol = "UserIdString"; */
    var valueCol = "value";

    var methods = {
        'init': function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();

            var propVals = o.propData.values;
            var options = propVals.options;

            propDiv.div()
                .checkBoxArray({
                    ID: o.ID + '_cba',
                    UseRadios: false,
                    Required: o.Required,
                    Multi: o.Multi,
                    ReadOnly: o.ReadOnly,
                    onChange: o.onChange,
                    dataAry: options,
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
    $.fn.CswFieldTypeUserSelect = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
