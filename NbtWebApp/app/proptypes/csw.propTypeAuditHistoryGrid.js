


(function () {
    'use strict';
    Csw.properties.auditHistoryGrid = Csw.properties.auditHistoryGrid ||
        Csw.properties.register('auditHistoryGrid',
            Csw.method(function(propertyOption) {
                var cswPublic = {
                    data: propertyOption
                };

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
                                    nodeids: [cswPublic.data.tabState.nodeid],
                                    nodekeys: [cswPublic.data.tabState.nodekey],
                                    onEditNode: cswPublic.data.onEditNode,
                                    date: date
                                });
                            }
                        });
                    }
                };
                cswPublic.data.bindRender(render);
                return cswPublic;
            }));
    
}());
