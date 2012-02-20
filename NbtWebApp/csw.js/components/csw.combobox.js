/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function () {
    'use strict';

    function comboBox(options) {
        var internal = {
            $parent: '',
            ID: '',
            topContent: '',
            selectContent: 'This ComboBox Is Empty!',
            width: '180px',
            onClick: null,
            topTable: {}
        };
        var external = {};

        (function () {

            function handleClick() {
                Csw.tryExec(internal.onClick);
                external.toggle();
            }

            if (options) {
                $.extend(internal, options);
            }

            $.extend(external, Csw.controls.div(internal));

            internal.topDiv = external.div({
                ID: internal.ID + '_top',
                cssclass: 'CswComboBox_TopDiv',
                styles: { width: internal.width }
            });

            internal.topTable = internal.topDiv.table({
                ID: Csw.controls.dom.makeId(internal.ID, 'tbl'),
                width: '100%'
            });

            internal.topTable.cell(1, 1).text(internal.topContent)
                .propDom('width', '100%')
                .bind('click', handleClick);

            internal.topTable.cell(1, 2)
                .addClass('CswComboBox_ImageCell')
                .imageButton({
                    'ButtonType': Csw.enums.imageButton_ButtonType.Select,
                    ID: internal.ID + '_top_img',
                    AlternateText: '',
                    onClick: handleClick
                });

            external.pickList = external.div({
                ID: internal.ID + '_child',
                cssclass: 'CswComboBox_ChildDiv',
                text: internal.selectContent,
                styles: { width: internal.width }
            }).bind('click', handleClick);

            var hideTo;
            external.pickList.$.hover(function () {
                clearTimeout(hideTo);
            }, function () {
                hideTo = setTimeout(external.pickList.hide, 300);
            });

        } ());

        external.topContent = function (content, itemid) {
            var cell1 = internal.topTable.cell(1, 1);
            cell1.text('');
            cell1.empty();
            cell1.append(content);
            external.val(itemid);
        };
        external.toggle = function () {
            internal.topDiv.$.toggleClass('CswComboBox_TopDiv_click');
            external.pickList.$.toggle();
            setTimeout(external.close, 5000);
        };
        external.close = function () {
            internal.topDiv.$.removeClass('CswComboBox_TopDiv_click');
            external.pickList.hide();
        };

        external.val = function (value) {
            var ret;
            if (Csw.isNullOrEmpty(value)) {
                ret = internal.value;
            } else {
                ret = external;
                external.propNonDom('value', value);
                internal.value = value;
            }
            return ret;
        };

        return external;
    }

    Csw.controls.register('comboBox', comboBox);
    Csw.controls.comboBox = Csw.controls.comboBox || comboBox;

} ());