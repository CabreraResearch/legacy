/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";

    $.fn.CswErrorMessage = function (options) {

        var o = {
            type: '',   // Warning, Error 
            message: '',
            detail: ''
        };
        if (options) Csw.extend(o, options);

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
        } else if (o.type.toLowerCase() === 'js') {
            errorDiv.addClass('CswErrorMessage_JS');
        } else {
            errorDiv.addClass('CswErrorMessage_Error');
        }

        var table = errorDiv.table({
            ID: Csw.makeId(id, 'tbl'),
            width: '100%'
        });

        var cell11 = table.cell(1, 1);
        var cell12 = table.cell(1, 2);
        var cell13 = table.cell(1, 3);
        var cell22 = table.cell(2, 2);
        
        // Look for node references in the error message
        cell12.nodeLink({ text: o.message });

        cell13.css({ width: '18px' });
        cell13.imageButton({
            ButtonType: Csw.enums.imageButton_ButtonType.Delete,
            AlternateText: 'Hide',
            ID: Csw.makeId(id, 'hidebtn'),
            onClick: function () {
                errorDiv.remove();
                if ($parentdiv.children().length === 0)
                    $parentdiv.hide();
            }
        });

        if (false === Csw.isNullOrEmpty(o.detail)) {
            cell11.css({ width: '18px' });
            var togglebtn = cell11.imageButton({
                ButtonType: Csw.enums.imageButton_ButtonType.ToggleInactive,
                AlternateText: 'Expand',
                ID: Csw.makeId(id, 'expandbtn'),
                onClick: function () {
                    cell22.$.toggle();
                    if (togglebtn.getButtonType() == Csw.enums.imageButton_ButtonType.ToggleActive) {
                        togglebtn.setButtonType(Csw.enums.imageButton_ButtonType.ToggleInactive);
                    } else {
                        togglebtn.setButtonType(Csw.enums.imageButton_ButtonType.ToggleActive);
                    }
                }
            });

            cell22.append(o.detail);
            cell22.hide();
        }

        $('html, body').animate({ scrollTop: 0 }, 0);

        //case 23675
        var dialog = parent.parent();
        if (dialog.$.hasClass('ui-dialog-content')) {
            dialog.$.animate({ scrollTop: 0 }, 0);
        }
        return errorDiv;

    }; // function (options) {
})(jQuery);
