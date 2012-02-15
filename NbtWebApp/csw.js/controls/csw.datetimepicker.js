/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function () {
    'use strict';

    function dateTimePicker(options) {
        var internal = {
            ID: '',
            Date: '',
            Time: '',
            DateFormat: '',
            TimeFormat: '',
            DisplayMode: 'Date',    // Date, Time, DateTime
            ReadOnly: false,
            Required: false,
            onChange: null
        };
        var external = {};

        (function () {
            if (options) {
                $.extend(internal, options);
            }

            $.extend(external, Csw.controls.div(internal));

            if (internal.ReadOnly) {
                switch (internal.DisplayMode) {
                    case 'Date':
                        external.div({ ID: internal.ID + '_date', value: internal.Date });
                        break;
                    case 'Time':
                        external.div({ ID: internal.ID + '_time', value: internal.Time });
                        break;
                    case 'DateTime':
                        external.div({ ID: internal.ID + '_time', value: internal.Date + ' ' + internal.Time });
                        break;
                }
            } else {
                if (internal.DisplayMode === 'Date' || internal.DisplayMode === 'DateTime') {
                    internal.dateBox = external.input({
                        ID: internal.ID + '_date',
                        type: Csw.enums.inputTypes.text,
                        value: internal.Date,
                        onChange: internal.onChange,
                        width: '80px',
                        cssclass: 'textinput'
                    });
                    internal.dateBox.$.datepicker({ 'dateFormat': Csw.serverDateFormatToJQuery(internal.DateFormat) });
                    if (internal.Required) {
                        internal.dateBox.addClass('required');
                    }
                }

                if (internal.DisplayMode === 'Time' || internal.DisplayMode === 'DateTime') {
                    internal.timeBox = external.input({
                        ID: internal.ID + '_time',
                        type: Csw.enums.inputTypes.text,
                        cssclass: 'textinput',
                        onChange: internal.onChange,
                        value: internal.Time,
                        width: '80px'
                    });
                    external.button({
                        ID: internal.ID + '_now',
                        disableOnClick: false,
                        onClick: function () {
                            internal.timeBox.val(Csw.getTimeString(new Date(), internal.TimeFormat));
                        },
                        enabledText: 'Now'
                    });

                    if (internal.Required) {
                        internal.timeBox.addClass('required');
                    }
                }
            } // if-else(o.ReadOnly)
        } ());
        
        external.val = function (readOnly) {
            var ret = {};
            if (internal.dateBox && internal.dateBox.length() > 0) {
                ret.date = (false === Csw.bool(readOnly)) ? internal.dateBox.val() : internal.dateBox.text();
            }
            if (internal.timeBox && internal.timeBox.length() > 0) {
                ret.time = (false === Csw.bool(readOnly)) ? internal.timeBox.val() : internal.timeBox.text();
            }
            return ret;
        };

        return external;
    }

    Csw.controls.register('dateTimePicker', dateTimePicker);
    Csw.controls.dateTimePicker = Csw.controls.dateTimePicker || dateTimePicker;

} ());
