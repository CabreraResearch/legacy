/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.number = Csw.properties.register('number',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = {};

                cswPrivate.precision = nodeProperty.propData.values.precision;
                cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);

                var number = nodeProperty.propDiv.numberTextBox({
                    name: nodeProperty.name + '_num',
                    value: nodeProperty.propData.values.value,
                    MinValue: Csw.number(nodeProperty.propData.values.minvalue),
                    MaxValue: Csw.number(nodeProperty.propData.values.maxvalue),
                    excludeRangeLimits: nodeProperty.propData.values.excludeRangeLimits,
                    ceilingVal: cswPrivate.ceilingVal,
                    Precision: cswPrivate.precision,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    onChange: function(val) {
                        nodeProperty.propData.values.value = val;
                        nodeProperty.broadcastPropChange();
                    },
                    isValid: true
                });
                number.required(nodeProperty.propData.required);
                if (false === Csw.isNullOrEmpty(number) && number.length > 0) {
                    number.clickOnEnter(function() {
                        cswPrivate.publish('CswSaveTabsAndProp_tab' + nodeProperty.tabState.tabid + '_' + nodeProperty.tabState.nodeid);
                    });
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());
