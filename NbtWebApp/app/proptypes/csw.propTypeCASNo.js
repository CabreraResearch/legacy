/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.CASNo = Csw.properties.register('CASNo',
        function(nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                
                var cswPrivate = Csw.object();

                cswPrivate.value = Csw.string(nodeProperty.propData.values.text).trim();
                cswPrivate.size = Csw.number(nodeProperty.propData.values.size, 14);
                
                nodeProperty.onPropChangeBroadcast(function (casNo) {
                    if (casNo !== cswPrivate.value) {
                        cswPrivate.value = casNo;

                        if (cswCasNo) {
                            cswCasNo.val(casNo);
                        }
                        if (span) {
                            span.remove();
                            span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                        }
                    }
                });

                if (nodeProperty.isReadOnly()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                } else {
                    
                    var cswCasNo = nodeProperty.propDiv.CASNoTextBox({
                        name: nodeProperty.name,
                        type: Csw.enums.inputTypes.text,
                        value: cswPrivate.value,
                        cssclass: 'textinput',
                        size: cswPrivate.size,
                        onChange: function(casNo) {
                            cswPrivate.value = casNo;
                            nodeProperty.propData.values.text = casNo;
                            nodeProperty.broadcastPropChange(casNo);
                        },
                        isRequired: nodeProperty.isRequired()
                    });

                    cswCasNo.required(nodeProperty.isRequired());
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender(function() {
              
            return true;
        });

}());