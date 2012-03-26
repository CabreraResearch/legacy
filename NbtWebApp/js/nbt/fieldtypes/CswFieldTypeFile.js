/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeFile';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();

            if (o.Multi) {
                propDiv.append(Csw.enums.multiEditDefaultValue);
            } else {

                var propVals = o.propData.values;

                var href = Csw.string(propVals.href).trim();
                var fileName = Csw.string(propVals.name).trim();

                var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });
                table.cell(1, 1).link({ href: href, target: '_blank', text: fileName });
                var cell12 = table.cell(1, 2).div();
                var cell13 = table.cell(1, 3).div();

                if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                    //Edit button
                    cell12.imageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                            AlternateText: 'Edit',
                            ID: o.ID + '_edit',
                            onClick: function () {
                                $.CswDialog('FileUploadDialog', {
                                    url: '/NbtWebApp/wsNBT.asmx/fileForProp',
                                    params: {
                                        PropId: o.propData.id
                                    },
                                    onSuccess: function () {
                                        o.onReload();
                                    }
                                });
                                return Csw.enums.imageButton_ButtonType.None;
                            }
                        });
                    //Clear button
                    cell13.imageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Clear,
                            AlternateText: 'Clear',
                            ID: o.ID + '_clr',
                            onClick: function () {
                                /* remember: confirm is globally blocking call */
                                if (confirm("Are you sure you want to clear this file?")) {
                                    var dataJson = {
                                        PropId: o.propData.id,
                                        IncludeBlob: true
                                    };

                                    Csw.ajax.post({
                                        url: '/NbtWebApp/wsNBT.asmx/clearProp',
                                        data: dataJson,
                                        success: function () { o.onReload(); }
                                    });
                                }
                                return Csw.enums.imageButton_ButtonType.None;
                            }
                        });
                }
            }
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeFile = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
