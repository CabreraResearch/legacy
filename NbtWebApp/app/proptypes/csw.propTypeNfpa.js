/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('nfpa', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = {
                whiteVals: [
                    { value: '', display: '' },
                    { value: 'ACID', display: 'ACID' },
                    { value: 'ALK', display: 'ALK' },
                    { value: 'BIO', display: 'BIO' },
                    { value: 'COR', display: 'COR' },
                    { value: 'CRYO', display: 'CRYO' },
                    { value: 'CYL', display: 'CYL' },
                    { value: 'OX', display: 'OX' },
                    { value: 'POI', display: 'POI' },
                    { value: 'RAD', display: 'RAD' },
                    { value: 'W', display: 'W' }
                ]
            };

            cswPrivate.red = nodeProperty.propData.values.flammability;
            cswPrivate.yellow = nodeProperty.propData.values.reactivity;
            cswPrivate.blue = nodeProperty.propData.values.health;
            cswPrivate.white = nodeProperty.propData.values.special;
            cswPrivate.displayMode = nodeProperty.propData.values.displaymode;
            cswPrivate.hideSpecial = nodeProperty.propData.values.hidespecial;

            var table = nodeProperty.propDiv.table();

            cswPrivate.table = table
                .cell(1, 1)
                .table()
                .addClass('CswFieldTypeNFPA_table');

            if (cswPrivate.displayMode === Csw.enums.NFPADisplayMode.Diamond) {
                cswPrivate.table.addClass('CswFieldTypeNFPA_rotation');
                cswPrivate.redDiv = cswPrivate.table.cell(1, 1)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_red' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_textRotated' });

                cswPrivate.yellowDiv = cswPrivate.table.cell(1, 2)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_yellow' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_textRotated' });

                cswPrivate.blueDiv = cswPrivate.table.cell(2, 1)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_blue' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_textRotated' });

                cswPrivate.whiteDiv = cswPrivate.table.cell(2, 2)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_white' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_whitetext CswFieldTypeNFPA_textRotated' });
            } else {
                cswPrivate.blueDiv = cswPrivate.table.cell(1, 1)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_blue' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text' });

                cswPrivate.redDiv = cswPrivate.table.cell(1, 2)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_red' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text' });

                cswPrivate.yellowDiv = cswPrivate.table.cell(1, 3)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_yellow' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text' });

                cswPrivate.whiteDiv = cswPrivate.table.cell(1, 4)
                    .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_white' + cswPrivate.displayMode })
                    .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_whitetextLinear' });
            }
            cswPrivate.setValue = function (div, value) {
                div.text(value);

                if (value === 'W') {
                    div.addClass("strikethrough");
                } else {
                    div.removeClass("strikethrough");
                }
            };

            cswPrivate.makeSelect = function (cell, id, selected, div, vals) {
                if (Csw.isNullOrEmpty(vals)) {
                    vals = [
                        { value: '', display: '' },
                        { value: '0', display: '0' },
                        { value: '1', display: '1' },
                        { value: '2', display: '2' },
                        { value: '3', display: '3' },
                        { value: '4', display: '4' }];
                }

                var select = cell.select({
                    selected: selected,
                    values: vals,
                    cssclass: '',
                    onChange: function (val) {
                        //Case 29390: no sync for NFPA

                        cswPrivate.setValue(div, select.val());
                        switch (id) {
                            case 'red':
                                nodeProperty.propData.values.flammability = val;
                                break;
                            case 'yellow':
                                nodeProperty.propData.values.reactivity = val;
                                break;
                            case 'blue':
                                nodeProperty.propData.values.health = val;
                                break;
                            case 'white':
                                nodeProperty.propData.values.special = val;
                                break;
                        }
                        nodeProperty.broadcastPropChange(val);
                    }
                });
            }; // makeSelect()

            cswPrivate.setValue(cswPrivate.redDiv, cswPrivate.red);
            cswPrivate.setValue(cswPrivate.yellowDiv, cswPrivate.yellow);
            cswPrivate.setValue(cswPrivate.blueDiv, cswPrivate.blue);
            cswPrivate.setValue(cswPrivate.whiteDiv, cswPrivate.white);

            if (false === nodeProperty.isReadOnly()) {
                cswPrivate.editTable = table.cell(1, 2).table({
                    FirstCellRightAlign: true
                });
                cswPrivate.editTable.cell(1, 1).text('Health');
                cswPrivate.editTable.cell(2, 1).text('Flammability');
                cswPrivate.editTable.cell(3, 1).text('Reactivity');
                if (false === cswPrivate.hideSpecial) {
                    cswPrivate.editTable.cell(4, 1).text('Special');
                }
                cswPrivate.makeSelect(cswPrivate.editTable.cell(1, 2), 'blue', cswPrivate.blue, cswPrivate.blueDiv, null);
                cswPrivate.makeSelect(cswPrivate.editTable.cell(2, 2), 'red', cswPrivate.red, cswPrivate.redDiv, null);
                cswPrivate.makeSelect(cswPrivate.editTable.cell(3, 2), 'yellow', cswPrivate.yellow, cswPrivate.yellowDiv, null);
                if (false === cswPrivate.hideSpecial) {
                    cswPrivate.makeSelect(cswPrivate.editTable.cell(4, 2), 'white', cswPrivate.white, cswPrivate.whiteDiv, cswPrivate.whiteVals);
                }

            } // if(!o.ReadOnly)
            if (cswPrivate.hideSpecial) {
                if (cswPrivate.displayMode === Csw.enums.NFPADisplayMode.Diamond) {
                    cswPrivate.table.cell(2, 2).hide(); //white cell for diamond display
                } else {
                    cswPrivate.table.cell(1, 4).hide(); //white cell for linear display
                }
                cswPrivate.whiteDiv.hide();
            }
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());

