/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.nfpa = Csw.properties.nfpa ||
        Csw.properties.register('nfpa',
            Csw.method(function (propertyOption) {
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
                var cswPublic = {
                    data: propertyOption
                };

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.red = cswPrivate.propVals.flammability;
                    cswPrivate.yellow = cswPrivate.propVals.reactivity;
                    cswPrivate.blue = cswPrivate.propVals.health;
                    cswPrivate.white = cswPrivate.propVals.special;
                    cswPrivate.displayMode = cswPrivate.propVals.displaymode;
                    cswPrivate.hideSpecial = cswPrivate.propVals.hidespecial;

                    cswPublic.control = cswPrivate.parent.table();

                    cswPrivate.table = cswPublic.control
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
                        cswPrivate.redDiv = cswPrivate.table.cell(1, 1)
                            .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_red' + cswPrivate.displayMode })
                            .div({ cssclass: 'CswFieldTypeNFPA_text' });

                        cswPrivate.blueDiv = cswPrivate.table.cell(1, 2)
                            .div({ cssclass: 'CswFieldTypeNFPA_cell CswFieldTypeNFPA_blue' + cswPrivate.displayMode })
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
                            onChange: function () {
                                var val = select.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPrivate.setValue(div, select.val());
                                var attr = {};
                                switch (id) {
                                    case 'red':
                                        attr.flammability = val;
                                        break;
                                    case 'yellow':
                                        attr.reactivity = val;
                                        break;
                                    case 'blue':
                                        attr.health = val;
                                        break;
                                    case 'white':
                                        attr.special = val;
                                        break;
                                }
                                cswPublic.data.onPropChange(attr);
                            }
                        });
                    }; // makeSelect()

                    cswPrivate.setValue(cswPrivate.redDiv, cswPrivate.red);
                    cswPrivate.setValue(cswPrivate.yellowDiv, cswPrivate.yellow);
                    cswPrivate.setValue(cswPrivate.blueDiv, cswPrivate.blue);
                    cswPrivate.setValue(cswPrivate.whiteDiv, cswPrivate.white);

                    if (false === cswPublic.data.isReadOnly()) {
                        cswPrivate.editTable = cswPublic.control.cell(1, 2).table({
                            FirstCellRightAlign: true
                        });

                        cswPrivate.editTable.cell(1, 1).text('Flammability');
                        cswPrivate.editTable.cell(2, 1).text('Reactivity');
                        if (cswPrivate.displayMode === Csw.enums.NFPADisplayMode.Diamond) {
                            cswPrivate.editTable.cell(2, 1).text('Reactivity');
                            cswPrivate.editTable.cell(3, 1).text('Health');
                        } else {
                            cswPrivate.editTable.cell(2, 1).text('Health');
                            cswPrivate.editTable.cell(3, 1).text('Reactivity');
                        }
                        if (false === cswPrivate.hideSpecial) {
                            cswPrivate.editTable.cell(4, 1).text('Special');
                        }
                        cswPrivate.makeSelect(cswPrivate.editTable.cell(1, 2), 'red', cswPrivate.red, cswPrivate.redDiv, null);
                        if (cswPrivate.displayMode === Csw.enums.NFPADisplayMode.Diamond) {
                            cswPrivate.makeSelect(cswPrivate.editTable.cell(2, 2), 'yellow', cswPrivate.yellow, cswPrivate.yellowDiv, null);
                            cswPrivate.makeSelect(cswPrivate.editTable.cell(3, 2), 'blue', cswPrivate.blue, cswPrivate.blueDiv, null);
                        } else {
                            cswPrivate.makeSelect(cswPrivate.editTable.cell(2, 2), 'blue', cswPrivate.blue, cswPrivate.blueDiv, null);
                            cswPrivate.makeSelect(cswPrivate.editTable.cell(3, 2), 'yellow', cswPrivate.yellow, cswPrivate.yellowDiv, null);
                        }
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
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());

