/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

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
        if(o.type.toLowerCase() === "warning")
        {
            $errordiv.addClass('CswErrorMessage_Warning');
        } else {
            $errordiv.addClass('CswErrorMessage_Error');
        }

        var $tbl = $errordiv.CswTable('init', {
                                                'id': Csw.makeId({ 
                                                        'prefix': id, 
                                                        'id': 'tbl' 
                                                    }), 
                                                'width': '100%' 
                                            });
        var $cell11 = $tbl.CswTable('cell', 1, 1)
                            .CswAttrDom('width', '100%');
        var $cell12 = $tbl.CswTable('cell', 1, 2);
        var $cell21 = $tbl.CswTable('cell', 2, 1);

        /* Image Link */
        $('<a href="#">' + o.message + '</a>')
                        .appendTo($cell11)
                        .click(function () { 
                            $cell21.toggle();
                        });
        $cell21.append(o.detail);
        $cell21.hide();

        $cell12.CswImageButton({
                ButtonType: CswImageButton_ButtonType.Delete,
                AlternateText: 'Hide',
                ID: Csw.makeId({ 'prefix': id, 'id': 'hidebtn' }),
                onClick: function () {
                    $errordiv.remove();
                    if ($parentdiv.children().length === 0)
                        $parentdiv.hide();
                    return CswImageButton_ButtonType.None;
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
