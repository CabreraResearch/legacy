/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeImage';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();

            if (o.Multi) {
                propDiv.append(Csw.enums.multiEditDefaultValue);
            } else {

                var propVals = o.propData.values,
                    width,
                    href = '/NbtWebApp/' + Csw.string(propVals.href);

                if (false === Csw.isNullOrEmpty(propVals.width) &&
                   Csw.isNumeric(propVals.width)) {
                    width = Math.abs(Csw.number(propVals.width, 100) - 36);
                } else {
                    width = '';
                }

                var fileName = Csw.string(propVals.name).trim();

                var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });
                var cell11 = table.cell(1, 1).propDom('colspan', '3');
                var cell21 = table.cell(2, 1).propDom('width', width);
                var cell22 = table.cell(2, 2).propDom({ align: 'right', width: '20px' }).div();
                var cell23 = table.cell(2, 3).propDom({ align: 'right', width: '20px' }).div();

                if (false === Csw.isNullOrEmpty(fileName)) {
                    //Case 24389: IE interprets height and width absolutely, better not to use them at all.
                    cell11.a({
                        href: href,
                        target: '_blank'
                    })
                        .img({
                            src: href, 
                            alt: fileName
                        });
                    cell21.a({ 
                        href: href,
                        target: '_blank',
                        text: fileName
                    });
                } else {
                    cell21.append('(no image selected)');
                }


                if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                    //Edit button
                    cell22.icon({
                        ID: Csw.makeId(o.ID, 'edit'),
                        iconType: Csw.enums.iconType.pencil,
                        hovertext: 'Edit',
                        size: 16,
                        isButton: true,
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
                        }
                    }); // icon
                    if (false === Csw.isNullOrEmpty(fileName)) {
                        //Clear button
                        cell23.icon({
                            ID: Csw.makeId(o.ID, 'clr'),
                            iconType: Csw.enums.iconType.trash,
                            hovertext: 'Clear Image',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                /* remember: confirm is globally blocking call */
                                if (confirm("Are you sure you want to clear this image?")) {
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
                            }
                        }); // icon
                    } // if (false === Csw.isNullOrEmpty(fileName)) {
                } // if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
            } // if-else (o.Multi)
        }, // init
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeImage = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
