/// <reference path="~/app/CswApp-vsdoc.js" />


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
                var fileCell = table.cell(1, 1);
                fileCell.a({ href: href, target: '_blank', text: fileName });
                var cell12 = table.cell(1, 2).div();
                var cell13 = table.cell(1, 3).div();

                if (false === o.ReadOnly) {
                    //Edit button
                    cell12.icon({
                            ID: o.ID + '_edit',
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                $.CswDialog('FileUploadDialog', {
                                    urlMethod: 'fileForProp',
                                    params: {
                                        PropId: o.propData.id
                                    },
                                    onSuccess: function (data) {
                                        if (data.success) {
                                            o.propData.values.href = data.href;
                                            o.propData.values.name = data.filename;
                                            o.propData.values.contenttype = data.contenttype;
                                            fileCell.empty();
                                            fileCell.a({ href: data.href, target: '_blank', text: data.filename });
                                            //o.onReload(); if we reload, we'll lose our reference
                                        }
                                    }
                                });
                            }
                        });
                    //Clear button
                    cell13.icon({
                            ID: o.ID + '_clr',
                            iconType: Csw.enums.iconType.trash,
                            hovertext: 'Clear File',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                /* remember: confirm is globally blocking call */
                                if (confirm("Are you sure you want to clear this file?")) {
                                    var dataJson = {
                                        PropId: o.propData.id,
                                        IncludeBlob: true
                                    };

                                    Csw.ajax.post({
                                        urlMethod: 'clearProp',
                                        data: dataJson,
                                        success: function () { o.onReload(); }
                                    });
                                }
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
