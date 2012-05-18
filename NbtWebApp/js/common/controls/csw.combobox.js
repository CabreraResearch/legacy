/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    
    Csw.controls.comboBox = Csw.controls.comboBox ||
        Csw.controls.register('comboBox', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            cswPrivateVar.hoverIn = function () {
                clearTimeout(cswPrivateVar.hideTo);
            };

            cswPrivateVar.hoverOut = function () {
            cswPrivateVar.hideTo = setTimeout(cswPublicRet.pickList.hide, cswPrivateVar.hidedelay);
            };
            
            (function () {

                function handleClick() {
                    if (Csw.tryExec(cswPrivateVar.onClick)) {
                        cswPublicRet.toggle();
                    }
                }

                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                cswPrivateVar.comboDiv = cswParent.div({
                    ID: cswPrivateVar.ID
                });
                cswPublicRet = Csw.dom({ }, cswPrivateVar.comboDiv);
                //$.extend(cswPublicRet, Csw.literals.div(cswPrivateVar));

                cswPrivateVar.topDiv = cswPrivateVar.comboDiv.div({
                    ID: cswPrivateVar.ID + '_top',
                    cssclass: 'CswComboBox_TopDiv',
                    width: cswPrivateVar.width
                });

                cswPrivateVar.topTable = cswPrivateVar.topDiv.table({
                    ID: Csw.makeId(cswPrivateVar.ID, 'tbl'),
                    width: cswPrivateVar.width
                });

                cswPrivateVar.topTable.cell(1, 1).append(cswPrivateVar.topContent)
                    .propDom('width', cswPrivateVar.width)
                    .bind('click', handleClick);

                cswPrivateVar.topTable.cell(1, 2)
                    .addClass('CswComboBox_ImageCell')
                    .imageButton({
                        'ButtonType': Csw.enums.imageButton_ButtonType.Select,
                        ID: cswPrivateVar.ID + '_top_img',
                        AlternateText: '',
                        onClick: handleClick
                    });

                cswPublicRet.pickList = cswPrivateVar.comboDiv.div({
                    ID: cswPrivateVar.ID + '_child',
                    cssclass: 'CswComboBox_ChildDiv',
                    width: cswPrivateVar.width
                })
                .bind('click', handleClick)
                .append(cswPrivateVar.selectContent);

                cswPublicRet.pickList.$.hover(cswPrivateVar.hoverIn, cswPrivateVar.hoverOut);
            } ());

            cswPublicRet.topContent = function (content, itemid) {
                var cell1 = cswPrivateVar.topTable.cell(1, 1);
                cell1.text('');
                cell1.empty();
                cell1.append(content);
                cswPublicRet.val(itemid);
            };
            cswPublicRet.toggle = function () {
                cswPrivateVar.topDiv.$.toggleClass('CswComboBox_TopDiv_click');
                cswPublicRet.pickList.$.toggle();
            };
            cswPublicRet.close = function () {
                cswPrivateVar.topDiv.$.removeClass('CswComboBox_TopDiv_click');
                cswPublicRet.pickList.hide();
            };
            cswPublicRet.open = function () {
                cswPrivateVar.topDiv.$.addClass('CswComboBox_TopDiv_click');
                cswPublicRet.pickList.show();
            };

            cswPublicRet.val = function (value) {
                var ret;
                if (Csw.isNullOrEmpty(value)) {
                    ret = cswPrivateVar.value;
                } else {
                    ret = cswPublicRet;
                    cswPublicRet.propNonDom('value', value);
                    cswPrivateVar.value = value;
                }
                return ret;
            };

            return cswPublicRet;
        });

} ());