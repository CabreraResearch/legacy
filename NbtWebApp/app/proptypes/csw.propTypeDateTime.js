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
                var render = function () {
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.date = Csw.string(cswPrivate.propVals.value.date).trim();
                    cswPrivate.time = Csw.string(cswPrivate.propVals.value.time).trim();

                    cswPublic.control = cswPrivate.parent.div();
                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control.append(cswPublic.data.propData.gestalt);
                    } else {
                        cswPrivate.dateTimePicker = cswPublic.control.dateTimePicker({
                            name: cswPublic.data.name,
                            Date: cswPrivate.date,
                            Time: cswPrivate.time,
                            DateFormat: Csw.serverDateFormatToJQuery(cswPrivate.propVals.value.dateformat),
                            TimeFormat: Csw.serverTimeFormatToJQuery(cswPrivate.propVals.value.timeformat),
                            DisplayMode: cswPrivate.propVals.displaymode,
                            ReadOnly: cswPublic.data.isReadOnly(),
                            isRequired: cswPublic.data.isRequired(),
                            onChange: function () {
                                var val = cswPrivate.dateTimePicker.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ value: val });
                            }
                        });

                        cswPublic.control.find('input').clickOnEnter(cswPublic.data.saveBtn);
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

