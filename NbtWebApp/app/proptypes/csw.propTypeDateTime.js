/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.dateTime = Csw.properties.dateTime ||
        Csw.properties.register('dateTime',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                var render = function (o) {
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = o.propData.values;
                    cswPrivate.parent = o.propDiv;
                    cswPrivate.date = (false === o.Multi) ? Csw.string(cswPrivate.propVals.value.date).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.time = (false === o.Multi) ? Csw.string(cswPrivate.propVals.value.time).trim() : Csw.enums.multiEditDefaultValue;

                    cswPublic.control = cswPrivate.parent.div();
                    if (o.ReadOnly) {
                        cswPublic.control.append(o.propData.gestalt);
                    } else {
                        cswPrivate.dateTimePicker = cswPublic.control.dateTimePicker({
                            ID: o.ID,
                            Date: cswPrivate.date,
                            Time: cswPrivate.time,
                            DateFormat: Csw.serverDateFormatToJQuery(cswPrivate.propVals.value.dateformat),
                            TimeFormat: Csw.serverTimeFormatToJQuery(cswPrivate.propVals.value.timeformat),
                            DisplayMode: cswPrivate.propVals.displaymode,
                            ReadOnly: o.ReadOnly,
                            Required: o.Required,
                            onChange: function () {
                                var val = cswPrivate.dateTimePicker.val();
                                Csw.tryExec(o.onChange, val);
                                o.onPropChange({ value: val });
                            }
                        });

                        cswPublic.control.find('input').clickOnEnter(o.saveBtn);
                    }

                };

                propertyOption.render(render);
                return cswPublic;
            }));

}());

