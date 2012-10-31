


(function () {
    'use strict';
    Csw.properties.auditHistoryGrid = Csw.properties.auditHistoryGrid ||
        Csw.properties.register('auditHistoryGrid',
            Csw.method(function(propertyOption) {
                var cswPublic = {
                    data: propertyOption
                };

                //The render function to be executed as a callback
                var render = function() {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    if (false === cswPublic.data.isMulti()) {
                        cswPublic.control = Csw.actions.auditHistory(cswPublic.data.propDiv, {
                            name: cswPublic.data.name,
                            nodeid: cswPublic.data.tabState.nodeid,
                            nodekey: cswPublic.data.tabState.nodekey,
                            EditMode: cswPublic.data.tabState.EditMode,
                            width: '100%',
                            allowEditRow: (cswPublic.data.tabState.EditMode !== Csw.enums.editMode.PrintReport),
                            onEditRow: function(date) {
                                Csw.publish('initPropertyTearDown');
                                $.CswDialog('EditNodeDialog', {
                                    currentNodeId: cswPublic.data.tabState.nodeid,
                                    currentNodeKey: cswPublic.data.tabState.nodekey,
                                    onEditNode: cswPublic.data.onEditNode,
                                    date: date
                                });
                            }
                        });
                    }
                };
                
                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));
    
}());
