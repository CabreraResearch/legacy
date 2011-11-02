/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../thirdparty/js/jmol/Jmol.js" />

; (function ($) { /// <param name="$" type="jQuery" />

    var pluginName = 'CswFieldTypeMol';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var width = 100; //tryParseString(propVals.width);
            var mol = tryParseString(propVals.mol).trim();

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
            var $cell11 = $table.CswTable('cell', 1, 1).CswAttrDom('colspan', '3');
            var $cell21 = $table.CswTable('cell', 2, 1).css('width', width - 36);
            var $cell22 = $table.CswTable('cell', 2, 2).css('textAlign', 'right');
            var $cell23 = $table.CswTable('cell', 2, 3).css('textAlign', 'right');

            if (false === isNullOrEmpty(mol)) {
                jmolInitialize('./js/thirdparty/js/jmol/', 'JmolApplet.jar');
                jmolSetDocument(false);
                var myApplet = jmolAppletInline('300px', mol);
                $cell11.append(myApplet); 
                var myCheck = jmolCheckbox("spin on", "spin off", "Rotate");
                $cell21.append(myCheck);
                //$Div.css('z-index', '0'); //this doesn't prevent jmol overlapping dialog
            }

            if (false === isTrue(o.ReadOnly) && o.EditMode !== EditMode.AddInPopup.name) {
                var $editButton = $('<div/>')
                    .appendTo($cell22)
                    .CswImageButton({
                        ButtonType: CswImageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: o.ID + '_edit',
                        onClick: function ($ImageDiv) {
                            $.CswDialog('EditMolDialog', {
                                TextUrl: '/NbtWebApp/wsNBT.asmx/saveMolProp',
                                FileUrl: '/NbtWebApp/wsNBT.asmx/saveMolPropFile',
                                PropId: o.propData.id,
                                molData: mol,
                                onSuccess: function () {
                                    o.onReload();
                                }
                            });
                            return CswImageButton_ButtonType.None;
                        }
                    });
                var $clearButton = $('<div/>')
                    .appendTo($cell23)
                    .CswImageButton({
                        ButtonType: CswImageButton_ButtonType.Clear,
                        AlternateText: 'Clear',
                        ID: o.ID + '_clr',
                        onClick: function ($ImageDiv) {

                            if (confirm("Are you sure you want to clear this structure?")) {
                                var dataJson = {
                                    PropId: o.propData.id,
                                    IncludeBlob: true
                                };

                                CswAjaxJson({
                                    url: '/NbtWebApp/wsNBT.asmx/clearProp',
                                    data: dataJson,
                                    success: function () { o.onReload(); }
                                });
                            }
                            return CswImageButton_ButtonType.None;
                        }
                    });
            }

        },
        save: function (o) { //$propdiv, o.propData
            preparePropJsonForSave(o.propData);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeMol = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);
