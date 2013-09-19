(function () {
    'use strict';
    Csw.properties.formula = Csw.properties.register('formula',
        function (nodeProperty) {
            'use strict';
            var cswPrivate = {};

            //The render function to be executed as a callback
            var render = function () {
                'use strict';

                cswPrivate.formula = nodeProperty.propDiv.formula({
                    nodeid: nodeProperty.propData.values.nodeid,
                    viewid: nodeProperty.propData.values.viewid,
                    nodeKey: '',
                    selectednodelink: nodeProperty.propData.values.selectednodelink,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    EditMode: nodeProperty.tabState.EditMode,
                    onChange: function (value) {
                        nodeProperty.propData.values.text = value;
                        nodeProperty.broadcastPropChange(value);
                    },
                    value: nodeProperty.propData.values.text,
                    formattedText: nodeProperty.propData.values.formattedText
                });
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());