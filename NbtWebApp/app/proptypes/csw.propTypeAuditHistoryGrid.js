/* globals Csw:false, $:false  */
(function () {
    'use strict';
    Csw.properties.register('auditHistoryGrid',
        function(nodeProperty) {

            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                if (nodeProperty.isMulti()) {
                    nodeProperty.propDiv.append('[History display disabled]');
                } else {
                    Csw.actions.auditHistory(nodeProperty.propDiv, {
                        name: nodeProperty.name,
                        nodeid: nodeProperty.tabState.nodeid,
                        nodekey: nodeProperty.tabState.nodekey,
                        EditMode: nodeProperty.tabState.EditMode,
                        width: '100%',
                        allowEditRow: (nodeProperty.tabState.EditMode !== Csw.enums.editMode.PrintReport),
                        onEditRow: function(date) {
                            Csw.publish('initPropertyTearDown');
                            $.CswDialog('EditNodeDialog', {
                                currentNodeId: nodeProperty.tabState.nodeid,
                                currentNodeKey: nodeProperty.tabState.nodekey,
                                onEditNode: nodeProperty.onEditNode,
                                date: date,
                                ReadOnly: true,
                                editMode: Csw.enums.editMode.AuditHistoryInPopup
                            });
                        }
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
