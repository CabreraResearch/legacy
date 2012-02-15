/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";

    $.fn.CswErrorMessage = function (options) {

        var o = {
            type: '',   // Warning, Error 
            message: '',
            detail: ''
        };
        if (options) $.extend(o, options);

        var $parentdiv = $(this);
        $parentdiv.show();

        var date = new Date();
        var id = "error_" + date.getTime();

        var $errordiv = $('<div />')
                        .appendTo($parentdiv)
                        .CswAttrDom('id', id)
                        .addClass('CswErrorMessage_Message');
        if (o.type.toLowerCase() === "warning") {
            $errordiv.addClass('CswErrorMessage_Warning');
        } else {
            $errordiv.addClass('CswErrorMessage_Error');
        }

        var table = Csw.controls.table({
            $parent: $errordiv,
            ID: Csw.controls.dom.makeId(id, 'tbl'),
            width: '100%'
        });
        table.propDom('width', '100%');

        var cell21 = table.add(2, 1, o.detail);
        cell21.hide();
        table.cell(1,1).link({
            ID: Csw.controls.dom.makeId({ ID: id, suffix: 'cell' }),
            text: o.message,
            onClick: function () { cell21.$.toggle(); }
        });
        var cell12 = table.cell(1, 2);


        cell12.$.CswImageButton({
            ButtonType: Csw.enums.imageButton_ButtonType.Delete,
            AlternateText: 'Hide',
            ID: Csw.controls.dom.makeId({ 'prefix': id, 'id': 'hidebtn' }),
            onClick: function () {
                $errordiv.remove();
                if ($parentdiv.children().length === 0)
                    $parentdiv.hide();
                return Csw.enums.imageButton_ButtonType.None;
            }
        });
        $('html, body').animate({ scrollTop: 0 }, 0);
        //case 23675
        var $dialog = $(this).parent();
        if ($dialog.hasClass('ui-dialog-content')) {
            $dialog.animate({ scrollTop: 0 }, 0);
        }
        return $errordiv;

    }; // function (options) {
})(jQuery);
