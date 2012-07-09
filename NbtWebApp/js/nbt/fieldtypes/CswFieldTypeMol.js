/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeMol';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var width = 200; //Csw.string(propVals.width);
            var mol = Csw.string(propVals.mol).trim();

            var table = propDiv.table({
                ID: Csw.makeId(o.ID, 'tbl')
            });
            var cell11 = table.cell(1, 1).propDom('colspan', '3');
            var cell21 = table.cell(2, 1).css('width', width - 36);
            var cell22 = table.cell(2, 2).css('textAlign', 'right');
            var cell23 = table.cell(2, 3).css('textAlign', 'right');

            //JMOL stuff
            if (false === Csw.isNullOrEmpty(mol)) {
                window.jmolInitialize('./js/thirdparty/jmol/', 'JmolApplet.jar');
                window.jmolSetDocument(false);
                var myApplet = window.jmolAppletInline('300px', mol);
                cell11.append(myApplet);
                var myCheck = window.jmolCheckbox("spin on", "spin off", "Rotate");
                cell21.append(myCheck);
            }

            if (false === Csw.bool(o.ReadOnly) && o.EditMode !== Csw.enums.editMode.Add) {
                /* Edit Button */
                cell22.div()
                    .imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: o.ID + '_edit',
                        onClick: function () {
                            $.CswDialog('EditMolDialog', {
                                TextUrl: '/NbtWebApp/wsNBT.asmx/saveMolProp',
                                FileUrl: '/NbtWebApp/wsNBT.asmx/saveMolPropFile',
                                PropId: o.propData.id,
                                molData: mol,
                                onSuccess: function () {
                                    o.onReload();
                                }
                            });
                        }
                    });

                /* Clear Button */
                cell23.div()
                    .imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Clear,
                        AlternateText: 'Clear',
                        ID: o.ID + '_clr',
                        onClick: function () {
                            /* remember: confirm is globally blocking call */
                            if (confirm("Are you sure you want to clear this structure?")) {
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
                    });
            }
        },
        save: function (o) { //$propdiv, o.propData
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeMol = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
