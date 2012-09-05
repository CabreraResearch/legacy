/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.controls.dateTimePicker = Csw.controls.dateTimePicker ||
        Csw.controls.register('dateTimePicker', function (cswParent, options) {
            ///<summary>Generates a dateTimePicker</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach dateTimePicker to.</param>
            ///<param name="options" type="Object">Object defining paramaters for dateTimePicker construction.</param>
            ///<returns type="Csw.controls.dateTimePicker">Object representing a dateTimePicker</returns>
            'use strict';
            var cswPrivate = {
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
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.dateTimeDiv = cswParent.div({
                    isControl: cswPrivate.isControl,
                    ID: cswPrivate.id
                });
                cswPublic = Csw.dom({ }, cswPrivate.dateTimeDiv);
                //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));

                if (cswPrivate.ReadOnly) {
                    switch (cswPrivate.DisplayMode) {
                        case 'Date':
                            cswPrivate.dateTimeDiv.div({ ID: cswPrivate.ID + '_date', value: cswPrivate.Date });
                            break;
                        case 'Time':
                            cswPrivate.dateTimeDiv.div({ ID: cswPrivate.ID + '_time', value: cswPrivate.Time });
                            break;
                        case 'DateTime':
                            cswPrivate.dateTimeDiv.div({ ID: cswPrivate.ID + '_time', value: cswPrivate.Date + ' ' + cswPrivate.Time });
                            break;
                    }
                } else {
                    if (cswPrivate.DisplayMode === 'Date' || cswPrivate.DisplayMode === 'DateTime') {
                        cswPrivate.dateBox = cswPrivate.dateTimeDiv.input({
                            ID: cswPrivate.ID + '_date',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.Date,
                            onChange: cswPrivate.onChange,
                            width: '80px',
                            cssclass: 'textinput'
                        });
                        if(cswPrivate.Date.substr(0, 'today'.length) !== 'today')
                        {
                            cswPrivate.dateBox.$.datepicker({ 'dateFormat': Csw.serverDateFormatToJQuery(cswPrivate.DateFormat) });
                        }
                        if (cswPrivate.Required) {
                            cswPrivate.dateBox.addClass('required');
                        }
                    }

                    if (cswPrivate.DisplayMode === 'Time' || cswPrivate.DisplayMode === 'DateTime') {
                        cswPrivate.timeBox = cswPrivate.dateTimeDiv.input({
                            ID: cswPrivate.ID + '_time',
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: cswPrivate.onChange,
                            value: cswPrivate.Time,
                            width: '80px'
                        });
                        cswPrivate.dateTimeDiv.button({
                            ID: cswPrivate.ID + '_now',
                            disableOnClick: false,
                            onClick: function () {
                                cswPrivate.timeBox.val(Csw.getTimeString(new Date(), cswPrivate.TimeFormat));
                            },
                            enabledText: 'Now'
                        });

                        if (cswPrivate.Required) {
                            cswPrivate.timeBox.addClass('required');
                        }
                    }

                    if(Csw.bool(cswPrivate.showTodayButton)) {
                        cswPrivate.dateTimeDiv.button({
                            ID: cswPrivate.ID + '_today',
                            disableOnClick: false,
                            onClick: function () {
                                cswPrivate.dateBox.$.datepicker('destroy');
                                cswPrivate.dateBox.val('today');  // this doesn't trigger onchange
                                Csw.tryExec(cswPrivate.onChange);
                            },
                            enabledText: 'Today'
                        });
                    }
                } // if-else(o.ReadOnly)
            } ());

            cswPublic.val = function (readOnly) {
                var ret = {};
                if (cswPrivate.dateBox && cswPrivate.dateBox.length() > 0) {
                    ret.date = (false === Csw.bool(readOnly)) ? cswPrivate.dateBox.val() : cswPrivate.dateBox.text();
                }
                if (cswPrivate.timeBox && cswPrivate.timeBox.length() > 0) {
                    ret.time = (false === Csw.bool(readOnly)) ? cswPrivate.timeBox.val() : cswPrivate.timeBox.text();
                }
                return ret;
            };

            return cswPublic;
        });

} ());
