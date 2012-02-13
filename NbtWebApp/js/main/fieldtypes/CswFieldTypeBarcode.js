/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeBarcode';

    var methods = {
        init: function (o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly  == nodeid,propxml,onchange

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.barcode).trim() : Csw.enums.multiEditDefaultValue;

            if (o.ReadOnly) {
                $Div.append(value);
            }
            else {
                var table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                });

                var cell1 = table.cell(1, 1);
                var $TextBox = cell1.$.CswInput('init', { ID: o.ID,
                    type: Csw.enums.inputTypes.text,
                    cssclass: 'textinput',
                    onChange: o.onchange,
                    value: value
                });
                if (false === o.Multi) {
                    var cell2 = table.add(1, 2, '<div />');
                    cell2.children('div')
                         .CswImageButton({ ButtonType: Csw.enums.imageButton_ButtonType.Print,
                            AlternateText: '',
                            ID: Csw.controls.dom.makeId(o.ID, 'print'),
                            onClick: function () {
                                $.CswDialog('PrintLabelDialog', { 'nodeid': o.nodeid, 'propid': o.ID });
                                return Csw.enums.imageButton_ButtonType.None;
                            }
                        });
                }
                if (o.Required) {
                    $TextBox.addClass("required");
                }

                $TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function (o) {
            var attributes = { barcode: null };
            var $TextBox = o.$propdiv.find('input');
            if (false === Csw.isNullOrEmpty($TextBox)) {
                attributes.barcode = $TextBox.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeBarcode = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
