/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function () {
    'use strict';

    Csw.controls.comboBox = Csw.controls.comboBox ||
        Csw.controls.register('comboBox', function (options) {
            var internal = {
                $parent: '',
                ID: '',
                topContent: '',
                selectContent: 'This ComboBox Is Empty!',
                width: '180px',
                onClick: function () {
                    return true;
                },
                topTable: {},

                hideTo: null
            };
            var external = {};

            internal.hoverIn = function () {
                clearTimeout(internal.hideTo);
            };

            internal.hoverOut = function () {
                internal.hideTo = setTimeout(external.pickList.hide, 300);
            };


            (function () {

                function handleClick() {
                    if (Csw.tryExec(internal.onClick)) {
                        external.toggle();
                    }
                }

                if (options) {
                    $.extend(internal, options);
                }

                $.extend(external, Csw.controls.div(internal));

                internal.topDiv = external.div({
                    ID: internal.ID + '_top',
                    cssclass: 'CswComboBox_TopDiv',
                    width: internal.width
                });

                internal.topTable = internal.topDiv.table({
                    ID: Csw.controls.dom.makeId(internal.ID, 'tbl'),
                    width: internal.width
                });

            internal.topTable.cell(1, 1).append(internal.topContent)
                    .propDom('width', internal.width)
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
                width: internal.width
            })
                .bind('click', handleClick)
                .append(internal.selectContent);

                external.pickList.$.hover(internal.hoverIn, internal.hoverOut);
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
            };
            external.close = function () {
                internal.topDiv.$.removeClass('CswComboBox_TopDiv_click');
                external.pickList.hide();
            };
            external.open = function () {
                internal.topDiv.$.addClass('CswComboBox_TopDiv_click');
                external.pickList.show();
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
        });

} ());