/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.register('dateTimePicker', function (cswParent, options) {
        ///<summary>Generates a dateTimePicker</summary>
        ///<param name="cswParent" type="Csw.literals">Parent element to attach dateTimePicker to.</param>
        ///<param name="options" type="Object">Object defining paramaters for dateTimePicker construction.</param>
        ///<returns type="Csw.composites.dateTimePicker">Object representing a dateTimePicker</returns>
        'use strict';
        var cswPrivate = {
            name: '',
            Date: '',
            Time: '',
            DateFormat: 'mm/dd/yyyy',
            TimeFormat: '',
            DisplayMode: 'Date',    // Date, Time, DateTime
            ReadOnly: false,
            isRequired: false,
            onChange: null,
            showTodayButton: false,
            changeYear: true,
            minDate: null,
            maxDate: null
        };
        var cswPublic = {};

        cswPrivate.dateFormats = {
            'mm/dd/yyyy': /^(0[1-9]|1[012])\/(0[1-9]|[12][0-9]|3[01])\/(\d{4})$/,
            'M/d/yyyy': /^([1-9]|1[012])\/([1-9]|[12][0-9]|3[01])\/(\d{4})$/,
            'd-M-yyyy': /^([1-9]|[12][0-9]|3[01])-([1-9]|1[012])-(\d{4})$/,
            'dd MMM yyyy': /^(0[1-9]|[12][0-9]|3[01]) (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) (\d{4})$/i,
            'yyyy-MM-dd': /^(\d{4})-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])$/,
            'yyyy/M/d': /^(\d{4})\/([1-9]|1[012])\/([1-9]|[12][0-9]|3[01])$/,
        };

        cswPrivate.timeFormats = {
            'h:mm:ss tt': /^([1-9]|1[012]):([0-5][0-9]):([0-5][0-9]) (AM|PM)$/,
            'H:mm:ss': /^(1?[0-9]|2[0123]):([0-5][0-9]):([0-5][0-9])$/
        };

        cswPrivate.addValidators = function () {

            if (cswPrivate.DateFormat in cswPrivate.dateFormats && undefined != cswPublic.dateBox) {
                $.validator.addMethod('validateDate', function (value, element) {
                    return cswPublic.isDateValid(value);
                }, 'Please select a valid date');
                cswPublic.dateBox.addClass('validateDate');
            }//if preferredFormat in dateFormats && undefined != cswPublic.dateBox

            if (cswPrivate.TimeFormat in cswPrivate.timeFormats && undefined != cswPublic.timeBox) {
                $.validator.addMethod('validateTime', function (value, element) {
                    return cswPublic.isTimeValid(value);
                }, 'Please enter a valid time in the format ' + cswPrivate.TimeFormat);
                cswPublic.timeBox.addClass('validateTime');
            }//if cswPrivate.TimeFormat in cswPrivate.timeFormats && undefined != cswPublic.timeBox

        };//function addValidators()

        (function () {
            if (options) {
                Csw.extend(cswPrivate, options);
            }
            cswPrivate.dateTimeTbl = cswParent.table({
                isControl: cswPrivate.isControl,
                name: cswPrivate.id
            });
            cswPublic = Csw.dom({}, cswPrivate.dateTimeTbl);
            //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));

            if (cswPrivate.ReadOnly) {
                switch (cswPrivate.DisplayMode) {
                    case 'Date':
                        cswPrivate.dateTimeTbl.cell(1, 1).div({ name: cswPrivate.name + '_date', value: cswPrivate.Date });
                        break;
                    case 'Time':
                        cswPrivate.dateTimeTbl.cell(1, 1).div({ name: cswPrivate.name + '_time', value: cswPrivate.Time });
                        break;
                    case 'DateTime':
                        cswPrivate.dateTimeTbl.cell(1, 1).div({ name: cswPrivate.name + '_time', value: cswPrivate.Date + ' ' + cswPrivate.Time });
                        break;
                }
            } else {
                var changeEvent = function () {
                    Csw.tryExec(cswPrivate.onChange, cswPublic.val());
                };

                if (cswPrivate.DisplayMode === 'Date' || cswPrivate.DisplayMode === 'DateTime') {
                    cswPublic.dateBox = cswPrivate.dateTimeTbl.cell(1, 1).input({
                        name: cswPrivate.name + '_date',
                        type: Csw.enums.inputTypes.text,
                        value: cswPrivate.Date,
                        onChange: changeEvent,
                        width: '80px',
                        cssclass: 'textinput'
                    });
                    if (cswPrivate.Date.substr(0, 'today'.length) !== 'today') {
                        cswPublic.dateBox.$.datepicker({
                            'dateFormat': Csw.serverDateFormatToJQuery(cswPrivate.DateFormat),
                            'changeYear': cswPrivate.changeYear,
                            'minDate': cswPrivate.minDate,
                            'maxDate': cswPrivate.maxDate
                        });
                    }
                    cswPublic.dateBox.required(cswPrivate.isRequired);
                }

                if (cswPrivate.DisplayMode === 'Time' || cswPrivate.DisplayMode === 'DateTime') {
                    cswPublic.timeBox = cswPrivate.dateTimeTbl.cell(1, 3).input({
                        name: cswPrivate.name + '_time',
                        type: Csw.enums.inputTypes.text,
                        cssclass: 'textinput',
                        onChange: changeEvent,
                        value: cswPrivate.Time,
                        width: '80px'
                    });
                    cswPrivate.dateTimeTbl.cell(1, 4).button({
                        name: cswPrivate.name + '_now',
                        disableOnClick: false,
                        onClick: function () {
                            cswPublic.timeBox.val(Csw.getTimeString(new Date(), cswPrivate.TimeFormat));
                            changeEvent();
                        },
                        enabledText: 'Now'
                    });
                    cswPublic.timeBox.required(cswPrivate.isRequired);
                }

                if (Csw.bool(cswPrivate.showTodayButton)) {
                    cswPrivate.dateTimeTbl.cell(1, 2).button({
                        name: cswPrivate.name + '_today',
                        disableOnClick: false,
                        onClick: function () {
                            cswPublic.dateBox.$.datepicker('destroy');
                            cswPublic.dateBox.val('today');  // this doesn't trigger onchange
                            changeEvent();
                        },
                        enabledText: 'Today'
                    });
                }

                cswPrivate.addValidators();
            } // if-else(o.ReadOnly)
        }());

        cswPublic.setMaxDate = function (maxDate) {
            cswPublic.dateBox.$.datepicker('option', 'maxDate', maxDate);
        };

        cswPublic.val = function (readOnly, value) {
            var ret = Csw.object();
            ret.date = '';
            ret.time = '';

            if (cswPublic.dateBox && cswPublic.dateBox.length() > 0) {
                ret.date = (false === Csw.bool(readOnly)) ? cswPublic.dateBox.val() : cswPublic.dateBox.text();
            }
            if (cswPublic.timeBox && cswPublic.timeBox.length() > 0) {
                ret.time = (false === Csw.bool(readOnly)) ? cswPublic.timeBox.val() : cswPublic.timeBox.text();
            }
            if (value) {
                if (cswPublic.dateBox) {
                    cswPublic.dateBox.val(value.date);
                }
                if (cswPublic.timeBox) {
                    cswPublic.timeBox.val(value.time);
                }
            }
            return ret;
        };

        cswPublic.isDateValid = function (dateIn) {
            //incredibly irritating, but after spending 2 hours tracing down the jQuery stack I found a jQuery bug with the way our validator is called, so we have to be able to accept an optional parameter
            var date = dateIn || cswPublic.dateBox.val();
            var ret = true;
            if (cswPrivate.isRequired || false == Csw.isNullOrEmpty(date)) {
                var dateExpression = cswPrivate.dateFormats[cswPrivate.DateFormat];
                ret = dateExpression.test(date);
            }
            return ret;
        };

        cswPublic.isTimeValid = function (timeIn) {
            //incredibly irritating, but after spending 2 hours tracing down the jQuery stack I found a jQuery bug with the way our validator is called, so we have to be able to accept an optional parameter
            var time = timeIn || cswPublic.timeBox.val();
            var ret = true;
            if (cswPrivate.isRequired || false == Csw.isNullOrEmpty(time)) {
                var timeExpression = cswPrivate.timeFormats[cswPrivate.TimeFormat];
                ret = timeExpression.test(time);
            }
            return ret;
        };



        return cswPublic;
    });

}());
