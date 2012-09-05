/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.auditHistoryGrid = Csw.properties.auditHistoryGrid ||
        Csw.properties.register('auditHistoryGrid',
            Csw.method(function(cswParent, propertyOption) {
                var o = Csw.nbt.propertyOption(propertyOption);
                var ret = {
                    data: propertyOption
                };
                if (false === o.Multi) {
                    ret.control = Csw.actions.auditHistory(cswParent, {
                        ID: o.ID,
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
                return ret;
            }));
    
}());
