/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.mtbf = Csw.properties.mtbf ||
        Csw.properties.register('mtbf',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };
                
                //The render function to be executed as a callback
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.startDate = Csw.string(cswPrivate.propVals.startdatetime.date);
                    cswPrivate.dateFormat = Csw.serverDateFormatToJQuery(cswPrivate.propVals.startdatetime.dateformat);

                    cswPrivate.value = Csw.string(cswPrivate.propVals.value).trim();
                    cswPrivate.units = Csw.string(cswPrivate.propVals.units).trim();

                    cswPublic.control = cswPrivate.parent.table();

                    cswPrivate.mtbfStatic = cswPrivate.value + '&nbsp;' + cswPrivate.units;
                    cswPublic.control.cell(1, 1).append(cswPrivate.mtbfStatic);

                    cswPrivate.cell12 = cswPublic.control.cell(1, 2);

                    if (false === cswPublic.data.isReadOnly()) {
                        cswPrivate.cell12.icon({
                            name: cswPublic.data.name,
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function() {
                                cswPrivate.editTable.show();
                            }
                        });

                        cswPrivate.editTable = cswPublic.control.cell(2, 2).table({ name: 'edittbl' });
                        cswPrivate.editTable.cell(1, 1).text('Start Date');

                        cswPrivate.datePicker = cswPrivate.editTable.cell(1, 2)
                            .dateTimePicker({
                                name: cswPublic.data.name + '_sd',
                                Date: cswPrivate.startDate,
                                DateFormat: cswPrivate.dateFormat,
                                DisplayMode: 'Date',
                                ReadOnly: cswPublic.data.isReadOnly(),
                                Required: cswPublic.data.isRequired(),
                                onChange: function() {
                                    var val = cswPrivate.datePicker.val();
                                    Csw.tryExec(cswPublic.data.onChange, val);
                                    cswPublic.data.onPropChange({ startdatetime: val });
                                }
                            });

                        cswPrivate.editTable.cell(3, 1).text('Units');
                        cswPrivate.unitVals = ['hours', 'days'];

                        cswPrivate.unitSelect = cswPrivate.editTable.cell(3, 2).select({
                            name: cswPublic.data.name + '_units',
                            onChange: function () {
                                var val = cswPrivate.unitSelect.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ unit: val });
                            },
                            values: cswPrivate.unitVals,
                            selected: cswPrivate.units
                        });

                        cswPrivate.editTable.hide();
                    }
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());
