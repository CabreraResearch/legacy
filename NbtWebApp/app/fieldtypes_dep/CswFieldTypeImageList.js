///// <reference path="~app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeImageList';

//    var methods = {
//        init: function (o) {

//            var propDiv = o.propDiv;
//            propDiv.empty();
//            var propVals = o.propData.values;

//            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
//            var options = propVals.options;
//            var width = Csw.string(propVals.width);
//            var height = Csw.string(propVals.height);
//            var allowMultiple = Csw.bool(propVals.allowmultiple);
//            var imageListTable = propDiv.table({
//                ID: Csw.makeId(o.ID, 'tbl'),
//                cellvalign: 'top'
//            });
//            var imageTable = imageListTable.cell(1, 1).table({
//                ID: Csw.makeId(o.ID, 'tbl')
//            });
//            var imgTblCol = 1;
//            var selectedValues = [];

//            if (false === o.ReadOnly) {
//                var imageSelectList = imageListTable.cell(1, 2).select({ id: o.ID });
//                var selectOption = imageSelectList.option({ value: '', display: 'Select...' });
//                if (o.Multi) {
//                    imageSelectList.option({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue, isSelected: true });
//                }
//                var hiddenValue = propDiv.textArea({
//                    ID: o.ID + '_value',
//                    value: value
//                }).hide();

//                imageSelectList.bind('change', function () {
//                    var selected = imageSelectList.children(':selected');
//                    changeImage(selected.text(), selected.val(), true, selected);
//                    if (o.Required && false === allowMultiple) {
//                        selectOption.remove();
//                    }
//                    o.onChange();
//                });

//                Csw.crawlObject(options,
//                    function (thisOpt) {
//                        if (Csw.bool(thisOpt.selected)) {
//                            selectedValues.push(thisOpt.value);
//                            addImage(thisOpt.text, thisOpt.value, false);
//                        } else {
//                            if (false === o.ReadOnly) {
//                                imageSelectList.option({ value: thisOpt.value, display: thisOpt.text });
//                            }
//                        }
//                    },
//                    false
//                );

//                if (o.Required) {
//                    $.validator.addMethod('imageRequired', function (value, element) {
//                        return (selectedValues.length > 0);
//                    }, 'An image is required.');
//                    imageSelectList.addClass('imageRequired');
//                }
//            }

//            function changeImage(name, href, doAnimation, selected) {
//                if (false === allowMultiple) {
//                    imageTable.empty();
//                    selectedValues = [];
//                }
//                addValue(selected.val());
//                if (allowMultiple) {
//                    selected.remove();
//                }
//                if (name != 'Select...') {
//                    addImage(name, href, doAnimation);
//                }
//            }

//            function addImage(name, href, doAnimation) {
//                var imageCell = imageTable.cell(1, imgTblCol)
//                    .css({ 'text-align': 'center',
//                        'padding-left': '10px'
//                    });
//                imageCell.a({
//                    href: href,
//                    target: '_blank'
//                })
//                    .img({
//                        src: href,
//                        alt: name,
//                        width: width,
//                        height: height
//                    });
//                var nameCell = imageTable.cell(2, imgTblCol)
//                    .css({ 'text-align': 'center',
//                        'padding-left': '10px'
//                    });
//                imgTblCol += 1;

//                if (doAnimation) {
//                    imageCell.hide();
//                    nameCell.hide();
//                }

//                if (name !== href) {
//                    nameCell.a({ href: href, target: '_blank', text: name });
//                }
//                if (false === o.ReadOnly && (false === o.Required || allowMultiple)) {
//                    nameCell.icon({
//                        ID: Csw.makeId('image', imgTblCol, 'rembtn'),
//                        iconType: Csw.enums.iconType.trash,
//                        hovertext: 'Remove',
//                        size: 16,
//                        isButton: true,
//                        onClick: function () {
//                            nameCell.$.fadeOut('fast');
//                            imageCell.$.fadeOut('fast');
//                            removeValue(href);
//                            if (allowMultiple) {
//                                imageSelectList.option({ value: href, display: name });
//                            }
//                            Csw.tryExec(o.onChange);
//                        } // onClick
//                    }); //
//                } // if(!o.ReadOnly)

//                if (doAnimation) {
//                    imageCell.$.fadeIn('fast');
//                    nameCell.$.fadeIn('fast');
//                }
//            } // addImage()

//            function addValue(valueToAdd) {
//                if (false === Csw.contains(selectedValues, valueToAdd) &&
//                    false === Csw.isNullOrEmpty(valueToAdd)) {
//                    selectedValues.push(valueToAdd);
//                }
//                hiddenValue.text(selectedValues.join('\n'));
//            }

//            function removeValue(valueToRemove) {
//                var idx = selectedValues.indexOf(valueToRemove);
//                if (idx !== -1) {
//                    selectedValues.splice(idx, 1);
//                }
//                hiddenValue.text(selectedValues.join('\n'));
//            }
//        },
//        save: function (o) {
//            var attributes = { value: null };
//            var compare = {};
//            var hiddenValue = o.propDiv.find('#' + o.ID + '_value');
//            if (false === Csw.isNullOrEmpty(hiddenValue)) {
//                attributes.value = hiddenValue.text();
//                compare = attributes;
//            }
//            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//        }
//    };

//    // Method calling logic
//    $.fn.CswFieldTypeImageList = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
