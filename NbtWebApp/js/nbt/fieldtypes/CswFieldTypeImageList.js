/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeImageList';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;

            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
            var options = propVals.options;
            var width = Csw.string(propVals.width);
            var height = Csw.string(propVals.height);
            var allowMultiple = Csw.bool(propVals.allowmultiple);
            var table = propDiv.table({
                ID: Csw.makeId(o.ID, 'tbl')
            });
            var currCol = 1;
            var selectedValues = [];

            if (false === o.ReadOnly) {
                var select = propDiv.select({ id: o.ID });
                select.option({ value: '', display: 'Select...' });
                if (o.Multi) {
                    select.option({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue, isSelected: true });
                }
                var hiddenValue = propDiv.textArea({
                    ID: o.ID + '_value',
                    value: value
                }).hide();

                select.bind('change', function () {
                    var selected = select.children(':selected');
                    changeImage(selected.text(), selected.val(), true, selected);
                    o.onChange();
                });

                Csw.crawlObject(options,
                    function (thisOpt) {
                        if (Csw.bool(thisOpt.selected)) {
                            selectedValues.push(thisOpt.value);
                            addImage(thisOpt.text, thisOpt.value, false);
                        } else {
                            if (false === o.ReadOnly) {
                                select.option({ value: thisOpt.value, display: thisOpt.text });
                            }
                        }
                    },
                    false);
            }

            function changeImage(name, href, doAnimation, selected) {
                if (false === allowMultiple) {
                    table.empty();
                    select.children(':not(:selected)').show();
                    selected.hide();
                }
                changeValue(selected.val());
                if (allowMultiple && false === Csw.isNullOrEmpty(selected)) {
                    selected.remove();
                }
                addImage(name, href, doAnimation);
            }

            function addImage(name, href, doAnimation) {
                var imageCell = table.cell(1, currCol)
                                    .css({ 'text-align': 'center',
                                        'padding-left': '10px'
                                    });
                imageCell.a({
                    href: href,
                    target: '_blank'
                })
                    .img({
                        src: href,
                        alt: name,
                        width: width,
                        height: height
                    });
                var nameCell = table.cell(2, currCol)
                                    .css({ 'text-align': 'center',
                                        'padding-left': '10px'
                                    });
                currCol += 1;

                if (doAnimation) {
                    imageCell.hide();
                    nameCell.hide();
                }


                if (name !== href) {
                    nameCell.a({ href: href, target: '_blank', text: name });
                }
                if (false === o.ReadOnly) {
                    nameCell.imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                        AlternateText: 'Remove',
                        ID: Csw.makeId('image', currCol, 'rembtn'),
                        onClick: function () {
                            nameCell.$.fadeOut('fast');
                            imageCell.$.fadeOut('fast');

                            removeValue(href);
                            select.option({ value: href, display: name });

                            Csw.tryExec(o.onChange);
                        } // onClick
                    }); //
                } // if(!o.ReadOnly)

                if (doAnimation) {
                    imageCell.$.fadeIn('fast');
                    nameCell.$.fadeIn('fast');
                }
            } // addImage()

            function changeValue(valueToChange) {
                if (false === allowMultiple) {
                    hiddenValue.text(valueToChange);
                } else {
                    addValue(valueToChange);
                }
            }

            function addValue(valueToAdd) {
                if (false === Csw.contains(selectedValues, valueToAdd) &&
                    false === Csw.isNullOrEmpty(valueToAdd)) {
                    selectedValues.push(valueToAdd);
                }
                hiddenValue.text(selectedValues.join('\n'));
            }

            function removeValue(valueToRemove) {
                var idx = selectedValues.indexOf(valueToRemove);
                if (idx !== -1) {
                    selectedValues.splice(idx, 1);
                }
                hiddenValue.text(selectedValues.join('\n'));
            }

        },
        save: function (o) {
            var attributes = { value: null };
            var compare = {};
            var hiddenValue = o.propDiv.find('#' + o.ID + '_value');
            if (false === Csw.isNullOrEmpty(hiddenValue)) {
                attributes.value = hiddenValue.text();
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeImageList = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
