


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
                    if (false === cswPublic.data.Multi) {
                        cswPublic.control = Csw.actions.auditHistory(cswPublic.data.propDiv, {
                            ID: Csw.makeId(cswPublic.data.ID, window.Ext.id()),
                            nodeid: cswPublic.data.nodeid,
                            cswnbtnodekey: cswPublic.data.cswnbtnodekey,
                            EditMode: cswPublic.data.EditMode,
                            width: '100%',
                            allowEditRow: (cswPublic.data.EditMode !== Csw.enums.editMode.PrintReport),
                            onEditRow: function(date) {
                                $.CswDialog('EditNodeDialog', {
                                    nodeids: [cswPublic.data.nodeid],
                                    nodekeys: [cswPublic.data.cswnbtnodekey],
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
