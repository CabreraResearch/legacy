/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.dateTime = Csw.properties.register('dateTime',
        function(nodeProperty) {
            'use strict';

            var eventName = 'onChangeDateTime_' + nodeProperty.propid;
            //The render function to be executed as a callback
            var render = function() {

                var cswPrivate = Csw.object();
                cswPrivate.date = nodeProperty.propData.values.value.date;
                cswPrivate.time = nodeProperty.propData.values.value.time;

                var div = nodeProperty.propDiv.div({ cssclass: 'cswInline' });
                if (nodeProperty.isReadOnly()) {
                    div.append(nodeProperty.propData.gestalt);
                } else {

                    Csw.properties.subscribe(eventName, function (eventObj, val) {
                        if (cswPrivate.date !== val.date || cswPrivate.time !== val.time) {
                            cswPrivate.date = val.date;
                            cswPrivate.time = val.time;
                            
                            cswPrivate.dateTimePicker.val(false, val);
                        }
                    });

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
                            Csw.properties.publish(eventName, dateTime);
                            
                            //Csw.tryExec(nodeProperty.onChange, val);
                            //nodeProperty.onPropChange({ value: val });
                        }
                    });

                    div.find('input').clickOnEnter(function() {
                        cswPrivate.publish('CswSaveTabsAndProp_tab' + nodeProperty.tabState.tabid + '_' + nodeProperty.tabState.nodeid);
                    });
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            nodeProperty.unBindRender(function() {
                Csw.properties.unsubscribe(eventName);
            });

            return true;
        });

} ());

