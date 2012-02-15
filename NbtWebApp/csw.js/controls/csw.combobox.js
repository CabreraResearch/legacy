/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    'use strict';

    function comboBox(options) {
        var internal = {
            ID: '',
            topContent: '',
            selectContent: 'This ComboBox Is Empty!',
            width: '180px',
            onClick: null // function () { }
        };
        var external = {};

        external.topContent = function (content) {
            var table = internal.topDiv.children('table');
            var cell1 = table.cell(1, 1);
            cell1.text('');
            cell1.empty();
            cell1.append(content);
        };
        external.toggle = function () {
            internal.topDiv.$.toggleClass('CswComboBox_TopDiv_click');
            internal.childDiv.$.toggle();
        };
        external.close = function () {
            internal.topDiv.$.removeClass('CswComboBox_TopDiv_click');
            internal.childDiv.hide();
        };

        (function () {

            function handleClick() {
                Csw.tryExec(internal.onClick);
                external.toggle();
            }

            if (options) {
                $.extend(internal, options);
            }

            internal.topDiv = $.extend(external,
                Csw.controls.div(
                    $.extend(internal, {
                        ID: internal.ID + '_top',
                        cssclass: 'CswComboBox_TopDiv'
                    })
                )
            );
            internal.topDiv.css('width', internal.Width);

            var table = internal.topDiv.table({
                ID: Csw.controls.dom.makeId(internal.ID, 'tbl'),
                width: '100%'
            });

            table.add(1, 1, internal.topContent)
                .propDom('width', '100%')
                .bind('click', handleClick);

            table.cell(1, 2)
                .addClass('CswComboBox_ImageCell')
                .imageButton({
                    'ButtonType': Csw.enums.imageButton_ButtonType.Select,
                    ID: internal.ID + '_top_img',
                    AlternateText: '',
                    onClick: handleClick
                });

            var hideTo;
            internal.childDiv = $.extend(external,
                Csw.controls.div(
                    $.extend(internal, {
                        ID: internal.ID + '_child',
                        cssclass: 'CswComboBox_ChildDiv',
                        text: internal.selectContent
                    })
                )
            );
            internal.childDiv.css('width', internal.width);
            internal.childDiv.$.hover(function () { clearTimeout(hideTo); }, function () { hideTo = setTimeout(function () { internal.childDiv.hide(); }, 750); });

        } ());

        return external;
    }

    Csw.controls.register('comboBox', comboBox);
    Csw.controls.comboBox = Csw.controls.comboBox || comboBox;

} ());