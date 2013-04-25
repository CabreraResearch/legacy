/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.sequence = Csw.properties.register('sequence',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();
                
                cswPrivate.value = nodeProperty.propData.values.sequence;

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.value !== val) {
                        cswPrivate.value = val;
                        updateProp(val);
                    }
                });

                var updateProp = function (val) {
                    nodeProperty.propData.values.sequence = val;
                    if (sequence) {
                        sequence.val(val);
                    }
                    if (span) {
                        span.remove();
                        span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                    }
                };

                if (nodeProperty.isReadOnly() || nodeProperty.isMulti()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                } else {
                    var sequence = nodeProperty.propDiv.input({
                        name: nodeProperty.name,
                        type: Csw.enums.inputTypes.text,
                        cssclass: 'textinput',
                        onChange: function(val) {
                            cswPrivate.value = val;
                            nodeProperty.propData.values.sequence = val;
                            nodeProperty.broadcastPropChange(val);
                        },
                        value: cswPrivate.value,
                        isRequired: nodeProperty.isRequired()
                    });

                    sequence.required(nodeProperty.isRequired());
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());

