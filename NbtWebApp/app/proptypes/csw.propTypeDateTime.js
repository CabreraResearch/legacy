/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.dateTime = Csw.properties.dateTime ||
        Csw.properties.register('dateTime',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPublic = {
                    data: propertyOption
                };
                var render = function (o) {
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    var propVals = o.propData.values;
                    var parent = o.propDiv;
                    var date = (false === o.Multi) ? Csw.string(propVals.value.date).trim() : Csw.enums.multiEditDefaultValue;
                    var time = (false === o.Multi) ? Csw.string(propVals.value.time).trim() : Csw.enums.multiEditDefaultValue;

                    if (o.ReadOnly) {
                        parent.append(o.propData.gestalt);
                    } else {
                        cswPublic.control = parent.dateTimePicker({
                            ID: o.ID,
                            Date: date,
                            Time: time,
                            DateFormat: Csw.serverDateFormatToJQuery(propVals.value.dateformat),
                            TimeFormat: Csw.serverTimeFormatToJQuery(propVals.value.timeformat),
                            DisplayMode: propVals.displaymode,
                            ReadOnly: o.ReadOnly,
                            Required: o.Required,
                            onChange: function () {
                                var val = cswPublic.control.val();
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

