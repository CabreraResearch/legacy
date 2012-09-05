/// <reference path="~app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeMTBF';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var startDate = (false === o.Multi) ? Csw.string(propVals.startdatetime.date) : Csw.enums.multiEditDefaultValue;
            var dateFormat = Csw.serverDateFormatToJQuery(propVals.startdatetime.dateformat);

            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
            var units = (false === o.Multi) ? Csw.string(propVals.units).trim() : Csw.enums.multiEditDefaultValue;

            var table = propDiv.table({
                ID: Csw.makeId(o.ID, 'tbl')
            });

            var mtbfStatic = (units !== Csw.enums.multiEditDefaultValue) ? value + '&nbsp;' + units : value;
            table.cell(1, 1).append(mtbfStatic);

            var cell12 = table.cell(1, 2);

            if (false === o.ReadOnly) {
                cell12.icon({
                    ID: o.ID,
                    iconType: Csw.enums.iconType.pencil,
                    hovertext: 'Edit',
                    size: 16,
                    isButton: true,
                    onClick: function () {
                        editTable.show();
                    }
                });

                var editTable = table.cell(2, 2).table({ ID: Csw.makeId(o.ID, 'edittbl') });
                editTable.cell(1, 1).text('Start Date');

                editTable.cell(1, 2)
                    .dateTimePicker({
                        ID: o.ID + '_sd',
                        Date: startDate,
                        DateFormat: dateFormat,
                        DisplayMode: 'Date',
                        ReadOnly: o.ReadOnly,
                        Required: o.Required,
                        onChange: o.onChange
                    });

                editTable.cell(3, 1).text('Units');
                var unitVals = ['hours', 'days'];
                if (o.Multi) {
                    unitVals.push(Csw.enums.multiEditDefaultValue);
                }
                editTable.cell(3, 2).select({
                    ID: o.ID + '_units',
                    onChange: o.onChange,
                    values: unitVals,
                    selected: units
                });

                editTable.hide();
            }
        },
        save: function (o) { //$propdiv, $xml

            var attributes = {
                startdatetime: {
                    date: null,
                    time: null
                },
                units: null
            };
            var compare = {};
            var startDate = o.propDiv.find('#' + o.ID + '_sd_date'),
                dateVal;

            if (false === Csw.isNullOrEmpty(startDate)) {
                dateVal = startDate.val();
                attributes.startdatetime.date = dateVal;
                compare = attributes;
                //attributes.startdatetime.time = dateVal.time;
            }

            var units = o.propDiv.find('#' + o.ID + '_units');
            if (false === Csw.isNullOrEmpty(units)) {
                attributes.units = units.val();
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeMTBF = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
