/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";
    var pluginName = 'CswFieldTypeMol';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var width = 100; //Csw.string(propVals.width);
            var mol = Csw.string(propVals.mol).trim();

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1).CswAttrDom('colspan', '3');
            var $cell21 = $table.CswTable('cell', 2, 1).css('width', width - 36);
            var $cell22 = $table.CswTable('cell', 2, 2).css('textAlign', 'right');
            var $cell23 = $table.CswTable('cell', 2, 3).css('textAlign', 'right');

            if (false === Csw.isNullOrEmpty(mol)) {
                jmolInitialize('./js/thirdparty/js/jmol/', 'JmolApplet.jar');
                jmolSetDocument(false);
                var myApplet = jmolAppletInline('300px', mol);
                $cell11.append(myApplet); 
                var myCheck = jmolCheckbox("spin on", "spin off", "Rotate");
                $cell21.append(myCheck);
                //$Div.css('z-index', '0'); //this doesn't prevent jmol overlapping dialog
            }

            if (false === Csw.bool(o.ReadOnly) && o.EditMode !== Csw.enums.EditMode.Add) {
                /* Edit Button */
                $('<div/>')
                    .appendTo($cell22)
                    .CswImageButton({
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
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    });
                /* Clear Button */
                $('<div/>')
                    .appendTo($cell23)
                    .CswImageButton({
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
                            return Csw.enums.imageButton_ButtonType.None;
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
