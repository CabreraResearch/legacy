/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.memo = Csw.properties.register('memo',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();

                var text = nodeProperty.propData.values.text;

                nodeProperty.onSyncProps(function (val) {
                    if (text !== val) {
                        text = val;
                        textArea.val(val);
                    }
                });

                var textArea = nodeProperty.propDiv.textArea({
                    onChange: function(val) {
                        text = val;
                        nodeProperty.propData.values.text = val;

                        nodeProperty.doSyncProps(val);
                        //Csw.tryExec(nodeProperty.onChange, val);
                        //nodeProperty.onPropChange({ text: val });
                    },
                    name: nodeProperty.name,
                    rows: nodeProperty.propData.values.rows,
                    cols: nodeProperty.propData.values.columns,
                    value: text,
                    disabled: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    readonly: nodeProperty.isReadOnly()
                });

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());
