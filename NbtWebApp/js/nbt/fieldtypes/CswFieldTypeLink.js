/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeLink';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();

            var propVals = o.propData.values;
            var text = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
            var href = (false === o.Multi) ? Csw.string(propVals.href).trim() : Csw.enums.multiEditDefaultValue;

            if (o.ReadOnly) {
                propDiv.link({
                    href: href,
                    text: text
                });
            } else {
                var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });

                table.cell(1, 1).link({
                    href: href,
                    text: text
                });
                var cell12 = table.cell(1, 2).div();

                cell12.imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: o.ID + '_edit',
                        Required: o.Required,
                        onClick: function () {
                            editTable.show();
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    });

                var editTable = propDiv.table({
                    ID: Csw.makeId(o.ID, 'edittbl')
                }).hide();

                editTable.cell(1, 1).span({text: 'Text'});

                var editTextCell = editTable.cell(1, 2);
                var editText = editTextCell.input({ 
                    ID: o.ID + '_text',
                    type: Csw.enums.inputTypes.text,
                    value: text,
                    onChange: o.onChange
                });

                editTable.cell(2, 1).span({text: 'URL'});

                var editHrefCell = editTable.cell(2, 2);
                var editHref = editHrefCell.input({ 
                    ID: o.ID + '_href',
                    type: Csw.enums.inputTypes.text,
                    value: href,
                    onChange: o.onChange
                });

                if (o.Required && href === '') {
                    editTable.show();
                    editText.addClass("required");
                    editHref.addClass("required");
                }
                editText.clickOnEnter(o.saveBtn);
                editHref.clickOnEnter(o.saveBtn);
            }
        },
        save: function (o) {
            var attributes = {
                text: null,
                href: null
            };
            var editText = o.propDiv.find('#' + o.ID + '_text');
            if (false === Csw.isNullOrEmpty(editText)) {
                attributes.text = editText.val();
            }
            var editHref = o.propDiv.find('#' + o.ID + '_href');
            if (false === Csw.isNullOrEmpty(editHref)) {
                attributes.href = editHref.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeLink = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
