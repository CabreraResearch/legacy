/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    Csw.controls.multiSelect = Csw.controls.multiSelect ||
        Csw.controls.register('multiSelect', function (cswParent, options) {

            var cswPrivateVar = {
                $parent: '',
                ID: '',
                values: [],
                multiple: true,
                cssclass: '',
                isMultiEdit: false,
                onChange: null, //function () {}
                isControl: false
            };

            var cswPublicRet = {};

            (function () {

                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                var optionCount = 0,
                    isMultiEdit = Csw.bool(cswPrivateVar.isMultiEdit),
                    values = cswPrivateVar.values;
                delete cswPrivateVar.values;

                cswPrivateVar.select = cswParent.select(cswPrivateVar);
                cswPublicRet = Csw.dom({ }, cswPrivateVar.select);
                //$.extend(cswPublicRet, Csw.literals.select(cswPrivateVar));

                if (Csw.isFunction(cswPrivateVar.onChange)) {
                    cswPrivateVar.select.bind('change', function () {
                        cswPrivateVar.onChange(cswPrivateVar.select);
                    });
                }

                Csw.each(values, function (opt) {
                    var value = Csw.string(opt.value, opt.text),
                        text = Csw.string(opt.text, value),
                        isSelected;
                    if (false === Csw.isNullOrEmpty(value)) {
                        isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                        cswPrivateVar.select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                        optionCount += 1;
                    }
                });

                if (optionCount > 20) {
                    cswPrivateVar.select.$.multiselect().multiselectfilter();
                } else {
                    cswPrivateVar.select.$.multiselect();
                }
            } ());

            cswPublicRet.val = function () {
                //In IE an empty array is frequently !== [], rather === null
                var values = cswPrivateVar.select.$.val() || [],
                    valArray = values.sort();
                return valArray.join(',');
            };

            return cswPublicRet;
        });


} ());
