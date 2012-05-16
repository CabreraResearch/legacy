/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {


    Csw.controls.dateTimePicker = Csw.controls.dateTimePicker ||
        Csw.controls.register('dateTimePicker', function (cswParent, options) {
            ///<summary>Generates a dateTimePicker</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach dateTimePicker to.</param>
            ///<param name="options" type="Object">Object defining paramaters for dateTimePicker construction.</param>
            ///<returns type="Csw.controls.dateTimePicker">Object representing a dateTimePicker</returns>
            'use strict';
            var internal = {
                ID: '',
                Date: '',
                Time: '',
                DateFormat: 'mm/dd/yyyy',
                TimeFormat: '',
                DisplayMode: 'Date',    // Date, Time, DateTime
                ReadOnly: false,
                Required: false,
                onChange: null,
                showTodayButton: false
            };
            var external = {};

            (function () {
                if (options) {
                    $.extend(internal, options);
                }
                internal.dateTimeDiv = cswParent.div({
                    isControl: internal.isControl,
                    ID: internal.id
                });
                external = Csw.dom({ }, internal.dateTimeDiv);
                //$.extend(external, Csw.literals.div(internal));

                if (internal.ReadOnly) {
                    switch (internal.DisplayMode) {
                        case 'Date':
                            internal.dateTimeDiv.div({ ID: internal.ID + '_date', value: internal.Date });
                            break;
                        case 'Time':
                            internal.dateTimeDiv.div({ ID: internal.ID + '_time', value: internal.Time });
                            break;
                        case 'DateTime':
                            internal.dateTimeDiv.div({ ID: internal.ID + '_time', value: internal.Date + ' ' + internal.Time });
                            break;
                    }
                } else {
                    if (internal.DisplayMode === 'Date' || internal.DisplayMode === 'DateTime') {
                        internal.dateBox = internal.dateTimeDiv.input({
                            ID: internal.ID + '_date',
                            type: Csw.enums.inputTypes.text,
                            value: internal.Date,
                            onChange: internal.onChange,
                            width: '80px',
                            cssclass: 'textinput'
                        });
                        if(internal.Date.substr(0, 'today'.length) !== 'today')
                        {
                            internal.dateBox.$.datepicker({ 'dateFormat': Csw.serverDateFormatToJQuery(internal.DateFormat) });
                        }
                        if (internal.Required) {
                            internal.dateBox.addClass('required');
                        }
                    }

                    if (internal.DisplayMode === 'Time' || internal.DisplayMode === 'DateTime') {
                        internal.timeBox = internal.dateTimeDiv.input({
                            ID: internal.ID + '_time',
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: internal.onChange,
                            value: internal.Time,
                            width: '80px'
                        });
                        internal.dateTimeDiv.button({
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

                    if(Csw.bool(internal.showTodayButton)) {
                        internal.dateTimeDiv.button({
                            ID: internal.ID + '_today',
                            disableOnClick: false,
                            onClick: function () {
                                internal.dateBox.$.datepicker('destroy');
                                internal.dateBox.val('today');  // this doesn't trigger onchange
                                Csw.tryExec(internal.onChange);
                            },
                            enabledText: 'Today'
                        });
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
        });

} ());
