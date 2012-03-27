/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
        var parent = Csw.literals.factory($parentdiv);
        parent.show();

        var date = new Date();
        var id = "error_" + date.getTime();

        var errorDiv = parent.div({
            ID: id,
            cssclass: 'CswErrorMessage_Message'
        });
        
        if (o.type.toLowerCase() === "warning") {
            errorDiv.addClass('CswErrorMessage_Warning');
        } else {
            errorDiv.addClass('CswErrorMessage_Error');
        }

        var table = errorDiv.table({
            ID: Csw.makeId(id, 'tbl'),
            width: '100%'
        });

        var cell21 = table.cell(2, 1);
        cell21.append(o.detail);         // using append() on error messages can send the browser into an infinite loop!
        cell21.hide();

        table.cell(1, 1).link({
            ID: Csw.makeId({ ID: id, suffix: 'cell' }),
            text: o.message,
            onClick: function () { cell21.$.toggle(); }
        });
        var cell12 = table.cell(1, 2);


        cell12.imageButton({
            ButtonType: Csw.enums.imageButton_ButtonType.Delete,
            AlternateText: 'Hide',
            ID: Csw.makeId(id, 'hidebtn'),
            onClick: function () {
                errorDiv.remove();
                if ($parentdiv.children().length === 0)
                    $parentdiv.hide();
                return Csw.enums.imageButton_ButtonType.None;
            }
        });
        $('html, body').animate({ scrollTop: 0 }, 0);
        //case 23675
        var dialog = parent.parent();
        if (dialog.$.hasClass('ui-dialog-content')) {
            dialog.$.animate({ scrollTop: 0 }, 0);
        }
        return errorDiv;

    }; // function (options) {
})(jQuery);
