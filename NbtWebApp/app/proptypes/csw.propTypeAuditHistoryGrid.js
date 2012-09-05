/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeAuditHistoryGrid';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            var ret = '';
            if (false === o.Multi) {
                ret = Csw.actions.auditHistory(propDiv, {
                    ID: o.ID,
                    nodeid: o.nodeid,
                    cswnbtnodekey: o.cswnbtnodekey,
                    EditMode: o.EditMode,
                    width: '100%',
                    allowEditRow: (o.EditMode !== Csw.enums.editMode.PrintReport),
                    onEditRow: function (date) {
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
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeAuditHistoryGrid = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
