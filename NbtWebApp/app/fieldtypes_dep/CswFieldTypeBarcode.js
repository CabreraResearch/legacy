///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeBarcode';

//    var methods = {
//        init: function (o) {

//            var propDiv = o.propDiv;
//            propDiv.empty();
//            var propVals = o.propData.values;
//            var value = (false === o.Multi) ? Csw.string(propVals.barcode).trim() : Csw.enums.multiEditDefaultValue;

//            var table = propDiv.table({
//                ID: Csw.makeId(o.ID, 'tbl')
//            });

//            var cell1 = table.cell(1, 1);

//            if (o.ReadOnly) {
//                cell1.text(value);
//            }
//            else {

//                var textBox = cell1.input({ ID: o.ID,
//                    type: Csw.enums.inputTypes.text,
//                    cssclass: 'textinput',
//                    onChange: o.onChange,
//                    value: value
//                });

//                if (o.Required) {
//                    textBox.addClass('required');
//                }

//                textBox.clickOnEnter(o.saveBtn);
//            }
//            if (false === o.Multi) {
//                table.cell(1, 2).div()
//                    .buttonExt({
//                        ID: Csw.makeId(o.ID, 'print'),
//                        enabledText: 'Print',
//                        size: 'small',
//                        tooltip: { title: 'Print Barcode Label' },
//                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.barcode),
//                        onClick: function () {
//                            $.CswDialog('PrintLabelDialog', { 'nodeid': o.nodeid, 'propid': o.propid });
//                        },
//                        editMode: o.EditMode
//                    });
//            }
//        },
//        save: function (o) {
//            var attributes = { barcode: null };
//            var compare = {};
//            var textBox = o.propDiv.find('input');
//            if (false === Csw.isNullOrEmpty(textBox)) {
//                attributes.barcode = textBox.val();
//                compare = attributes;
//            }
//            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//        }
//    };

//    // Method calling logic
//    $.fn.CswFieldTypeBarcode = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
