/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.mtbf = Csw.properties.register('mtbf',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();

                cswPrivate.startDate = nodeProperty.propData.values.startdatetime.date;
                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.units = nodeProperty.propData.values.units;

                cswPrivate.dateFormat = Csw.serverDateFormatToJQuery(nodeProperty.propData.values.startdatetime.dateformat);

                var table = nodeProperty.propDiv.table();
                var cell1 = table.cell(1, 1);

                var makeStatic = function() {
                    cswPrivate.mtbfStatic = cswPrivate.value + '&nbsp;' + cswPrivate.units;
                    cell1.span({ text: cswPrivate.mtbfStatic });
                    //Case 29390: No sync for MTBF
                };
                makeStatic();
                
                cswPrivate.cell12 = table.cell(1, 2);

                if (false === nodeProperty.isReadOnly()) {
                    cswPrivate.cell12.icon({
                        name: nodeProperty.name,
                        iconType: Csw.enums.iconType.pencil,
                        hovertext: 'Edit',
                        size: 16,
                        isButton: true,
                        onClick: function() {
                            cswPrivate.editTable.show();
                        }
                    });

                    cswPrivate.editTable = table.cell(2, 2).table({ name: 'edittbl' });
                    cswPrivate.editTable.cell(1, 1).text('Start Date');

                    cswPrivate.datePicker = cswPrivate.editTable.cell(1, 2)
                        .dateTimePicker({
                            name: nodeProperty.name + '_sd',
                            Date: cswPrivate.startDate,
                            DateFormat: cswPrivate.dateFormat,
                            DisplayMode: 'Date',
                            ReadOnly: nodeProperty.isReadOnly(),
                            isRequired: nodeProperty.isRequired(),
                            onChange: function(val) {
                                nodeProperty.propData.values.startdatetime.date = val.date;
                            }
                        });

                    cswPrivate.editTable.cell(3, 1).text('Units');
                    cswPrivate.unitVals = ['hours', 'days'];

                    cswPrivate.unitSelect = cswPrivate.editTable.cell(3, 2).select({
                        name: nodeProperty.name + '_units',
                        onChange: function(val) {
                            nodeProperty.propData.values.units = val;
                        },
                        values: cswPrivate.unitVals,
                        selected: cswPrivate.units
                    });

                    cswPrivate.editTable.hide();
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());
