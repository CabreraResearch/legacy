/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    Csw.controls.imageSelect = Csw.controls.imageSelect ||
        Csw.controls.register('imageSelect', Csw.method(function (cswParent, params) {
            ///<summary>Generates a picklist of images and names</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach imageSelect to.</param>
            ///<param name="options" type="Object">Object defining paramaters for imageSelect construction.</param>
            ///<returns type="Csw.controls.imageSelect">Object representing an imageSelect</returns>
            'use strict';
            var cswPublic;
            var cswPrivate = {
                name: '',
                options: [], // [{ name: '', href: '', id: '' }],
                width: '180px',
                height: '150px',
                defaultText: 'Select...',
                comboImgWidth: '32px',
                comboImgHeight: '',
                onSelect: function(name, href, id, imageCell, nameCell) { return true; }  // true to send the selected content to the top of the comboBox
            };
            Csw.extend(cswPrivate, params);

            cswPrivate.optsTblMaxRow = 1;
            cswPrivate.optionsTable = cswParent.table({
                cellvalign: 'top',
                width: '100%',
                height: cswPrivate.height
            });
            
            cswPrivate.imageSelectList = cswParent.comboBox({
                name: cswPrivate.name + '_imgsel',
                topContent: cswPrivate.defaultText,
                selectContent: cswPrivate.optionsTable,
                width: cswPrivate.width,
                height: cswPrivate.height,
//                onClick: function () {
//                    return true;
//                },
                topTable: {},
                hidedelay: 500,
                hideTo: null
            });
            
            cswPublic = Csw.dom({ }, cswPrivate.imageSelectList);

            cswPublic.addOption = function (name, href, id) {
                var imageCell = cswPrivate.optionsTable.cell(cswPrivate.optsTblMaxRow, 1);
                if (false === Csw.isNullOrEmpty(href)) {
                    imageCell.img({
                        src: href,
                        alt: name,
                        width: cswPrivate.comboImgWidth,
                        height: cswPrivate.comboImgHeight,
                    });
                }
                var nameCell = cswPrivate.optionsTable.cell(cswPrivate.optsTblMaxRow, 2);
                nameCell.text(name);

                var hoverIn = function() {
                    imageCell.addClass('viewselectitemhover');
                    nameCell.addClass('viewselectitemhover');
                };
                var hoverOut = function() {
                    imageCell.removeClass('viewselectitemhover');
                    nameCell.removeClass('viewselectitemhover');
                };
                imageCell.$.hover(hoverIn, hoverOut);
                nameCell.$.hover(hoverIn, hoverOut);

                var handleSelect = function() {
                    if (Csw.tryExec(cswPrivate.onSelect, name, href, id, imageCell, nameCell)) {
                        cswPrivate.imageSelectList.topContent(name, id);
                    }
                };
                imageCell.bind('click', handleSelect);
                nameCell.bind('click', handleSelect);

                cswPrivate.optsTblMaxRow += 1;
            }; // addOption()

            Csw.each(cswPrivate.options, function(opt) {
                cswPublic.addOption(opt.name, opt.href, opt.id);
            });

            return cswPublic;
        }));

} ());

