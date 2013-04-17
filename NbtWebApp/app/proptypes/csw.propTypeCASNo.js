/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.CASNo = Csw.properties.register('CASNo',
        function(nodeProperty) {
            'use strict';

            var eventName = 'onChangeCasNo_' + nodeProperty.propid;

            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                
                var cswPrivate = Csw.object();

                cswPrivate.value = Csw.string(nodeProperty.propData.values.text).trim();
                cswPrivate.size = Csw.number(nodeProperty.propData.values.size, 14);
                
                if (nodeProperty.isReadOnly()) {
                    nodeProperty.propDiv.append(cswPrivate.value);
                } else {

                    Csw.properties.subscribe(eventName, function (eventObj, casNo) {
                        if (casNo !== cswPrivate.value) {
                            cswPrivate.value = casNo;
                            
                            cswCasNo.val(casNo);
                        }
                    });

                    var cswCasNo = nodeProperty.propDiv.CASNoTextBox({
                        name: nodeProperty.name,
                        type: Csw.enums.inputTypes.text,
                        value: cswPrivate.value,
                        cssclass: 'textinput',
                        size: cswPrivate.size,
                        onChange: function(casNo) {
                            nodeProperty.propData.values.text = casNo;
                            Csw.properties.publish(eventName, casNo);
                            //Csw.tryExec(nodeProperty.onChange, val);
                            //nodeProperty.onPropChange({ text: val });
                        },
                        isRequired: nodeProperty.isRequired()
                    });

                    cswCasNo.required(nodeProperty.isRequired());
                    cswCasNo.clickOnEnter(function () {
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

}());