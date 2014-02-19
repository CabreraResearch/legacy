/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('memo', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            var text = nodeProperty.propData.values.text;

            if (nodeProperty.isReadOnly()) {
                nodeProperty.propDiv.div({
                    name: nodeProperty.name,
                    height: (nodeProperty.propData.values.rows * 24) + 'px',
                    width: (nodeProperty.propData.values.columns * 9) + 'px',
                    text: text.replace(/\n/g,'<br>')
                }).css({ overflow: 'auto' });
            } else {
                nodeProperty.onPropChangeBroadcast(function(val) {
                    if (text !== val) {
                        text = val;
                        textArea.val(val);
                    }
                });

                var textArea = nodeProperty.propDiv.textArea({
                    onChange: function(val) {
                        text = val;
                        nodeProperty.propData.values.text = val;
                        nodeProperty.broadcastPropChange(val);
                    },
                    name: nodeProperty.name,
                    rows: nodeProperty.propData.values.rows,
                    cols: nodeProperty.propData.values.columns,
                    value: text,
                    isRequired: nodeProperty.isRequired(),
                    readonly: nodeProperty.isReadOnly()
                });
            } // if-else(nodeProperty.isReadOnly()) {
        }; // render()

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
