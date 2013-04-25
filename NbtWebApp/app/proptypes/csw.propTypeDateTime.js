/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.dateTime = Csw.properties.register('dateTime',
        function(nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function() {

                var cswPrivate = Csw.object();
                cswPrivate.date = nodeProperty.propData.values.value.date;
                cswPrivate.time = nodeProperty.propData.values.value.time;

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.date !== val.date || cswPrivate.time !== val.time) {
                        cswPrivate.date = val.date;
                        cswPrivate.time = val.time;

                        if (cswPrivate.dateTimePicker) {
                            cswPrivate.dateTimePicker.val(false, val);
                        }
                        if (span) {
                            span.remove();
                            span = div.span({ text: cswPrivate.date + ' ' + cswPrivate.time });
                        }
                    }
                });

                var div = nodeProperty.propDiv.div({ cssclass: 'cswInline' });
                if (nodeProperty.isReadOnly()) {
                    var span = div.span({ text: nodeProperty.propData.gestalt });
                } else {
                    
                    cswPrivate.dateTimePicker = div.dateTimePicker({
                        name: nodeProperty.name,
                        Date: cswPrivate.date,
                        Time: cswPrivate.time,
                        DateFormat: nodeProperty.propData.values.value.dateformat, //dateTimePicker does the format conversion for us
                        TimeFormat: Csw.serverTimeFormatToJQuery(nodeProperty.propData.values.value.timeformat),
                        DisplayMode: nodeProperty.propData.values.displaymode,
                        ReadOnly: nodeProperty.isReadOnly(),
                        isRequired: nodeProperty.isRequired(),
                        onChange: function(dateTime) {
                            nodeProperty.propData.values.value = dateTime;
                            nodeProperty.broadcastPropChange(dateTime);
                        }
                    });
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender(function() {

            return true;
        });

} ());

