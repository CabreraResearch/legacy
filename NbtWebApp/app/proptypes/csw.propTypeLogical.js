/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.logical = Csw.properties.register('logical',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();

                var checked = nodeProperty.propData.values.checked;
                nodeProperty.onSyncProps(function (val) {
                    if (checked !== val) {
                        checked = val;
                        tri.val(val);
                    }
                });

                var tri = nodeProperty.propDiv.triStateCheckBox({
                    checked: nodeProperty.propData.values.checked,
                    isRequired: nodeProperty.isRequired(),
                    ReadOnly: nodeProperty.isReadOnly(),
                    Multi: nodeProperty.isMulti(),
                    onChange: function(val) {
                        nodeProperty.propData.values.checked = val;
                        checked = val;
                        nodeProperty.doSyncProps(val);
                        
                        //Csw.tryExec(nodeProperty.onChange, val);
                        //nodeProperty.onPropChange({ checked: val });
                    }
                });
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());






