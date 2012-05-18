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
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                cswPrivateVar.dateTimeDiv = cswParent.div({
                    isControl: cswPrivateVar.isControl,
                    ID: cswPrivateVar.id
                });
                cswPublicRet = Csw.dom({ }, cswPrivateVar.dateTimeDiv);
                //$.extend(cswPublicRet, Csw.literals.div(cswPrivateVar));

                if (cswPrivateVar.ReadOnly) {
                    switch (cswPrivateVar.DisplayMode) {
                        case 'Date':
                            cswPrivateVar.dateTimeDiv.div({ ID: cswPrivateVar.ID + '_date', value: cswPrivateVar.Date });
                            break;
                        case 'Time':
                            cswPrivateVar.dateTimeDiv.div({ ID: cswPrivateVar.ID + '_time', value: cswPrivateVar.Time });
                            break;
                        case 'DateTime':
                            cswPrivateVar.dateTimeDiv.div({ ID: cswPrivateVar.ID + '_time', value: cswPrivateVar.Date + ' ' + cswPrivateVar.Time });
                            break;
                    }
                } else {
                    if (cswPrivateVar.DisplayMode === 'Date' || cswPrivateVar.DisplayMode === 'DateTime') {
                        cswPrivateVar.dateBox = cswPrivateVar.dateTimeDiv.input({
                            ID: cswPrivateVar.ID + '_date',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivateVar.Date,
                            onChange: cswPrivateVar.onChange,
                            width: '80px',
                            cssclass: 'textinput'
                        });
                        if(cswPrivateVar.Date.substr(0, 'today'.length) !== 'today')
                        {
                            cswPrivateVar.dateBox.$.datepicker({ 'dateFormat': Csw.serverDateFormatToJQuery(cswPrivateVar.DateFormat) });
                        }
                        if (cswPrivateVar.Required) {
                            cswPrivateVar.dateBox.addClass('required');
                        }
                    }

                    if (cswPrivateVar.DisplayMode === 'Time' || cswPrivateVar.DisplayMode === 'DateTime') {
                        cswPrivateVar.timeBox = cswPrivateVar.dateTimeDiv.input({
                            ID: cswPrivateVar.ID + '_time',
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: cswPrivateVar.onChange,
                            value: cswPrivateVar.Time,
                            width: '80px'
                        });
                        cswPrivateVar.dateTimeDiv.button({
                            ID: cswPrivateVar.ID + '_now',
                            disableOnClick: false,
                            onClick: function () {
                                cswPrivateVar.timeBox.val(Csw.getTimeString(new Date(), cswPrivateVar.TimeFormat));
                            },
                            enabledText: 'Now'
                        });

                        if (cswPrivateVar.Required) {
                            cswPrivateVar.timeBox.addClass('required');
                        }
                    }

                    if(Csw.bool(cswPrivateVar.showTodayButton)) {
                        cswPrivateVar.dateTimeDiv.button({
                            ID: cswPrivateVar.ID + '_today',
                            disableOnClick: false,
                            onClick: function () {
                                cswPrivateVar.dateBox.$.datepicker('destroy');
                                cswPrivateVar.dateBox.val('today');  // this doesn't trigger onchange
                                Csw.tryExec(cswPrivateVar.onChange);
                            },
                            enabledText: 'Today'
                        });
                    }
                } // if-else(o.ReadOnly)
            } ());

            cswPublicRet.val = function (readOnly) {
                var ret = {};
                if (cswPrivateVar.dateBox && cswPrivateVar.dateBox.length() > 0) {
                    ret.date = (false === Csw.bool(readOnly)) ? cswPrivateVar.dateBox.val() : cswPrivateVar.dateBox.text();
                }
                if (cswPrivateVar.timeBox && cswPrivateVar.timeBox.length() > 0) {
                    ret.time = (false === Csw.bool(readOnly)) ? cswPrivateVar.timeBox.val() : cswPrivateVar.timeBox.text();
                }
                return ret;
            };

            return cswPublicRet;
        });

} ());
