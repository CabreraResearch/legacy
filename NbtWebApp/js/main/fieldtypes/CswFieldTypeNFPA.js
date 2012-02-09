/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeNFPA';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            var propVals = o.propData.values;
            var red = (false === o.Multi) ? propVals.flammability : Csw.enums.multiEditDefaultValue;
            var yellow = (false === o.Multi) ? propVals.reactivity : Csw.enums.multiEditDefaultValue;
            var blue = (false === o.Multi) ? propVals.health : Csw.enums.multiEditDefaultValue;
            var white = (false === o.Multi) ? propVals.special : Csw.enums.multiEditDefaultValue;

            var outerTable = Csw.controls.table({
                $parent: $Div,
                ID: Csw.controls.dom.makeId(o.ID, 'tbl')
            });

            var table = outerTable.cell(1, 1).table(Csw.controls.dom.makeId(o.ID, 'tbl1'))
                                   .addClass('CswFieldTypeNFPA_table');

            var $reddiv = $('<div class="CswFieldTypeNFPA_text"></div>');
            table.add(1, 1, $reddiv)
                 .addClass('CswFieldTypeNFPA_red CswFieldTypeNFPA_cell');
            
            var $yellowdiv = $('<div class="CswFieldTypeNFPA_text"></div>');
            table.add(1, 2, $yellowdiv)
                 .addClass('CswFieldTypeNFPA_yellow CswFieldTypeNFPA_cell');

            var $bluediv = $('<div class="CswFieldTypeNFPA_text"></div>');
            table.add(2, 1, $bluediv)
                 .addClass('CswFieldTypeNFPA_blue CswFieldTypeNFPA_cell');
            
            var $whitediv = $('<div class="CswFieldTypeNFPA_text"></div>')
            table.add(2, 2, $whitediv)
                 .addClass('CswFieldTypeNFPA_white CswFieldTypeNFPA_cell');

            function setValue($div, value) {
                $div.text(value);

                if (value === "W")
                    $div.addClass("strikethrough");
                else
                    $div.removeClass("strikethrough");
            }

            function makeSelect(cell, id, selected, $div) {
                var $sel = cell.$.CswSelect({
                    'ID': Csw.controls.dom.makeId(o.ID, id),
                    'selected': selected,
                    'values': selVals,
                    'cssclass': '',
                    'onChange': function () {
                        setValue($div, $sel.val());
                    }
                });
            } // makeSelect()

            setValue($reddiv, red);
            setValue($yellowdiv, yellow);
            setValue($bluediv, blue);
            setValue($whitediv, white);

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

                editTable.add(1, 1, 'Flammability');
                editTable.add(2, 1, 'Health');
                editTable.add(3, 1, 'Reactivity');
                editTable.add(4, 1, 'Special');

                makeSelect(editTable.cell(1, 2), 'red', red, $reddiv);
                makeSelect(editTable.cell(2, 2), 'yellow', yellow, $yellowdiv);
                makeSelect(editTable.cell(3, 2), 'blue', blue, $bluediv);

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
                var $whitesel = editTable.cell(4, 2)
                                          .$.CswSelect({
                                              'ID': Csw.controls.dom.makeId({ ID: o.ID, suffix: 'white' }),
                                              'selected': white,
                                              'values': whiteVals,
                                              'cssclass': '',
                                              'onChange': function () {
                                                  setValue($whitediv, $whitesel.val());
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
            var $red = o.$propdiv.find('#' + o.ID + '_red');
            if (false === Csw.isNullOrEmpty($red)) {
                attributes.flammability = $red.val();
            }
            var $yellow = o.$propdiv.find('#' + o.ID + '_yellow');
            if (false === Csw.isNullOrEmpty($yellow)) {
                attributes.reactivity = $yellow.val();
            }
            var $blue = o.$propdiv.find('#' + o.ID + '_blue');
            if (false === Csw.isNullOrEmpty($blue)) {
                attributes.health = $blue.val();
            }
            var $white = o.$propdiv.find('#' + o.ID + '_white');
            if (false === Csw.isNullOrEmpty($white)) {
                attributes.special = $white.val();
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
