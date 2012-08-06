/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    
    Csw.controls.comboBox = Csw.controls.comboBox ||
        Csw.controls.register('comboBox', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                $parent: '',
                ID: '',
                topContent: '',
                selectContent: 'This ComboBox Is Empty!',
                width: '180px',
                onClick: function () {
                    return true;
                },
                topTable: {},
            hidedelay: 500,

                hideTo: null
            };
            var cswPublic = {};

            cswPrivate.hoverIn = function () {
                clearTimeout(cswPrivate.hideTo);
            };

            cswPrivate.hoverOut = function () {
            cswPrivate.hideTo = setTimeout(cswPublic.pickList.hide, cswPrivate.hidedelay);
            };
            
            (function () {

                function handleClick() {
                    if (Csw.tryExec(cswPrivate.onClick)) {
                        cswPublic.toggle();
                    }
                }

                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.comboDiv = cswParent.div({
                    ID: cswPrivate.ID
                });
                cswPublic = Csw.dom({ }, cswPrivate.comboDiv);
                //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));

                cswPrivate.topDiv = cswPrivate.comboDiv.div({
                    ID: cswPrivate.ID + '_top',
                    cssclass: 'CswComboBox_TopDiv',
                    width: cswPrivate.width
                });

                cswPrivate.topTable = cswPrivate.topDiv.table({
                    ID: Csw.makeId(cswPrivate.ID, 'tbl'),
                    width: cswPrivate.width
                });

                cswPrivate.topTable.cell(1, 1).append(cswPrivate.topContent)
                    .propDom('width', cswPrivate.width)
                    .bind('click', handleClick);

                cswPrivate.topTable.cell(1, 2)
                    .addClass('CswComboBox_ImageCell')
                    .imageButton({
                        'ButtonType': Csw.enums.imageButton_ButtonType.Select,
                        ID: cswPrivate.ID + '_top_img',
                        AlternateText: '',
                        onClick: handleClick
                    });

                cswPublic.pickList = cswPrivate.comboDiv.div({
                    ID: cswPrivate.ID + '_child',
                    cssclass: 'CswComboBox_ChildDiv',
                    width: cswPrivate.width
                })
                .bind('click', handleClick)
                .append(cswPrivate.selectContent);

                cswPublic.pickList.$.hover(cswPrivate.hoverIn, cswPrivate.hoverOut);
            } ());

            cswPublic.topContent = function (content, itemid) {
                var cell1 = cswPrivate.topTable.cell(1, 1);
                cell1.text('');
                cell1.empty();
                cell1.append(content);
                cswPublic.val(itemid);
            };
            cswPublic.toggle = function () {
                cswPrivate.topDiv.$.toggleClass('CswComboBox_TopDiv_click');
                cswPublic.pickList.$.toggle();
            };
            cswPublic.close = function () {
                cswPrivate.topDiv.$.removeClass('CswComboBox_TopDiv_click');
                cswPublic.pickList.hide();
            };
            cswPublic.open = function () {
                cswPrivate.topDiv.$.addClass('CswComboBox_TopDiv_click');
                cswPublic.pickList.show();
            };

            cswPublic.val = function (value) {
                var ret;
                if (Csw.isNullOrEmpty(value)) {
                    ret = cswPrivate.value;
                } else {
                    ret = cswPublic;
                    cswPublic.propNonDom('value', value);
                    cswPrivate.value = value;
                }
                return ret;
            };

            return cswPublic;
        });

} ());