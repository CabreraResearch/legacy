/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.list = Csw.properties.register('list',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();
                
                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.text = nodeProperty.propData.values.text;
                cswPrivate.options = nodeProperty.propData.values.options;

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.value !== val) {
                        cswPrivate.value = val;
                        if (select) {
                            select.val(val);
                        }
                        if (span) {
                            span.remove();
                            span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                        }
                    }
                });

                if (nodeProperty.isReadOnly()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.text });
                } else {
                    cswPrivate.values = cswPrivate.options.split(',');

                    //case 28020 - if a list has a value selected that's not in the list, add it to the options
                    if (false == Csw.contains(cswPrivate.values, cswPrivate.value)) {
                        cswPrivate.values.push(cswPrivate.value);
                    }

                    var select = nodeProperty.propDiv.select({
                        name: nodeProperty.name,
                        cssclass: 'selectinput',
                        onChange: function(val) {
                            cswPrivate.value = val;
                            nodeProperty.propData.values.value = val;
                            nodeProperty.broadcastPropChange(val);
                        },
                        values: cswPrivate.values,
                        selected: cswPrivate.value
                    });
                    select.required(nodeProperty.isRequired());
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());
