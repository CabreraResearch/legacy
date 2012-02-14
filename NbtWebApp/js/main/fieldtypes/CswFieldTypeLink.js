/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeLink';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();

            var propVals = o.propData.values;
            var text = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
            var href = (false === o.Multi) ? Csw.string(propVals.href).trim() : Csw.enums.multiEditDefaultValue;

            var link = '<a href="' + href + '" target="_blank">' + text + '</a>&nbsp;&nbsp;';

            if (o.ReadOnly) {
                $Div.append(link);
            } else {
                var table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                });

                table.add(1, 1, link);
                var cell12 = table.add(1, 2, '<div />');

                cell12.children('div')
                    .$.CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: o.ID + '_edit',
                        Required: o.Required,
                        onClick: function () {
                            editTable.show();
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    });

                var editTable = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'edittbl')
                }).$.hide();

                editTable.add(1, 1, '<span>Text</span>');

                var editTextCell = editTable.cell(1, 2);
                var $edittext = editTextCell.$.CswInput('init', { ID: o.ID + '_text',
                    type: Csw.enums.inputTypes.text,
                    value: text,
                    onChange: o.onChange
                });

                editTable.add(2, 1, '<span>URL</span>');

                var editHrefCell = editTable.cell(2, 2);
                var $edithref = editHrefCell.$.CswInput('init', { ID: o.ID + '_href',
                    type: Csw.enums.inputTypes.text,
                    value: href,
                    onChange: o.onChange
                });

                if (o.Required && href === '') {
                    editTable.$.show();
                    $edittext.addClass("required");
                    $edithref.addClass("required");
                }
                $edittext.clickOnEnter(o.$savebtn);
                $edithref.clickOnEnter(o.$savebtn);
            }
        },
        save: function (o) {
            var attributes = {
                text: null,
                href: null
            };
            var $edittext = o.$propdiv.find('#' + o.ID + '_text');
            if (false === Csw.isNullOrEmpty($edittext)) {
                attributes.text = $edittext.val();
            }
            var $edithref = o.$propdiv.find('#' + o.ID + '_href');
            if (false === Csw.isNullOrEmpty($edithref)) {
                attributes.href = $edithref.val();
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
