


(function () {
    'use strict';
    Csw.properties.auditHistoryGrid = Csw.properties.auditHistoryGrid ||
        Csw.properties.register('auditHistoryGrid',
            Csw.method(function(propertyOption) {
                var cswPublic = {
                    data: propertyOption
                };

                var render = function(o) {
                    'use strict';
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    if (false === o.Multi) {
                        cswPublic.control = Csw.actions.auditHistory(o.propDiv, {
                            ID: Csw.makeId(o.ID, window.Ext.id()),
                            nodeid: o.nodeid,
                            cswnbtnodekey: o.cswnbtnodekey,
                            EditMode: o.EditMode,
                            width: '100%',
                            allowEditRow: (o.EditMode !== Csw.enums.editMode.PrintReport),
                            onEditRow: function(date) {
                                $.CswDialog('EditNodeDialog', {
                                    nodeids: [o.nodeid],
                                    nodekeys: [o.cswnbtnodekey],
                                    onEditNode: o.onEditNode,
                                    date: date
                                });
                            }
                        });
                    }
                };
                propertyOption.render(render);
                return cswPublic;
            }));
    
}());
