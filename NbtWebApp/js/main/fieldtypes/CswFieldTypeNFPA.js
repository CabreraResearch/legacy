/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeNFPA';

    var methods = {
        init: function (o) {

            var propDiv = $(this);
            var propVals = o.propData.values;
            var red = (false === o.Multi) ? propVals.flammability : Csw.enums.multiEditDefaultValue;
            var yellow = (false === o.Multi) ? propVals.reactivity : Csw.enums.multiEditDefaultValue;
            var blue = (false === o.Multi) ? propVals.health : Csw.enums.multiEditDefaultValue;
            var white = (false === o.Multi) ? propVals.special : Csw.enums.multiEditDefaultValue;

            var outerTable = propDiv.table({
                ID: Csw.controls.dom.makeId(o.ID, 'tbl')
            });

            var table = outerTable.cell(1, 1).table(Csw.controls.dom.makeId(o.ID, 'tbl1'))
                                   .addClass('CswFieldTypeNFPA_table');

            var redDiv = table.cell(1, 1)
                               .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_cell CswFieldTypeNFPA_red' });

            var yellowDiv = table.cell(1, 2)
                               .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_cell CswFieldTypeNFPA_yellow' });

            var blueDiv = table.cell(2, 1)
                               .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_cell CswFieldTypeNFPA_blue' });

            var whiteDiv = table.cell(2, 2)
                               .div({ cssclass: 'CswFieldTypeNFPA_text CswFieldTypeNFPA_cell CswFieldTypeNFPA_white' });

            function setValue(div, value) {
                div.text(value);

                if (value === 'W') {
                    div.addClass("strikethrough");
                } else {
                    div.removeClass("strikethrough");
                }
            }

            function makeSelect(cell, id, selected, div) {
                var select = cell.select({
                    ID: Csw.controls.dom.makeId(o.ID, id),
                    selected: selected,
                    values: selVals,
                    cssclass: '',
                    onChange: function () {
                        setValue(div, select.val());
                    }
                });
            } // makeSelect()

            setValue(redDiv, red);
            setValue(yellowDiv, yellow);
            setValue(blueDiv, blue);
            setValue(whiteDiv, white);

            if (false === o.ReadOnly) {
                var editTable = outerTable.cell(1, 2).table({
                    ID: Csw.controls.dom.makeId(o.ID, 'edittbl'),
                    FirstCellRightAlign: true
                });
                var selVals = [
                    { value: '0', display: '0' },
                    { value: '1', display: '1' },
                    { value: '2', display: '2' },
                    { value: '3', display: '3' },
                    { value: '4', display: '4' }
                ];
                if (o.Multi) {
                    selVals.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                }

                editTable.cell(1, 1).text('Flammability');
                editTable.cell(2, 1).text('Health');
                editTable.cell(3, 1).text('Reactivity');
                editTable.cell(4, 1).text('Special');

                makeSelect(editTable.cell(1, 2), 'red', red, redDiv);
                makeSelect(editTable.cell(2, 2), 'yellow', yellow, yellowDiv);
                makeSelect(editTable.cell(3, 2), 'blue', blue, blueDiv);

                var whiteVals = [
                    { value: 'ACID', display: 'ACID' },
                    { value: 'ALK', display: 'ALK' },
                    { value: 'BIO', display: 'BIO' },
                    { value: 'COR', display: 'COR' },
                    { value: 'CRYO', display: 'CRYO' },
                    { value: 'CYL', display: 'CYL' },
                    { value: 'OX', display: 'OX' },
                    { value: 'POI', display: 'POI' },
                    { value: 'RAD', display: 'RAD' },
                    { value: 'W', display: 'W'}];
                if (o.Multi) {
                    whiteVals.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                }
                var whiteSelect = editTable.cell(4, 2)
                                          .select({
                                              ID: Csw.controls.dom.makeId({ ID: o.ID, suffix: 'white' }),
                                              selected: white,
                                              values: whiteVals,
                                              cssclass: '',
                                              onChange: function () {
                                                  setValue(whiteDiv, whiteSelect.val());
                                              }
                                          });

            } // if(!o.ReadOnly)
        },
        save: function (o) {
            var attributes = {
                flammability: null,
                reactivity: null,
                health: null,
                special: null
            };
            var redDiv = o.propDiv.find('#' + o.ID + '_red');
            if (false === Csw.isNullOrEmpty(redDiv)) {
                attributes.flammability = redDiv.val();
            }
            var yellowDiv = o.propDiv.find('#' + o.ID + '_yellow');
            if (false === Csw.isNullOrEmpty(yellowDiv)) {
                attributes.reactivity = yellowDiv.val();
            }
            var blueDiv = o.propDiv.find('#' + o.ID + '_blue');
            if (false === Csw.isNullOrEmpty(blueDiv)) {
                attributes.health = blueDiv.val();
            }
            var whiteDiv = o.propDiv.find('#' + o.ID + '_white');
            if (false === Csw.isNullOrEmpty(whiteDiv)) {
                attributes.special = whiteDiv.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeNFPA = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
