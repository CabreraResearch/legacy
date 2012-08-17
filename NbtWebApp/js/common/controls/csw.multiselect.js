/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    Csw.controls.multiSelect = Csw.controls.multiSelect ||
        Csw.controls.register('multiSelect', function (cswParent, options) {

            var cswPrivate = {
                $parent: '',
                ID: '',
                values: [],
                multiple: true,
                cssclass: '',
                isMultiEdit: false,
                onChange: null, //function () {}
                isControl: false
            };

            var cswPublic = {};

            (function () {

                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                var optionCount = 0,
                    isMultiEdit = Csw.bool(cswPrivate.isMultiEdit),
                    values = cswPrivate.values;
                delete cswPrivate.values;

                cswPrivate.select = cswParent.select(cswPrivate);
                cswPublic = Csw.dom({}, cswPrivate.select);
                //Csw.extend(cswPublic, Csw.literals.select(cswPrivate));

                if (Csw.isFunction(cswPrivate.onChange)) {
                    cswPrivate.select.bind('change', function () {
                        cswPrivate.onChange(cswPrivate.select);
                    });
                }

                Csw.each(values, function (opt) {
                    var value = Csw.string(opt.value, opt.text),
                        text = Csw.string(opt.text, value),
                        isSelected;
                    if (false === Csw.isNullOrEmpty(value)) {
                        isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                        cswPrivate.select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                        optionCount += 1;
                    }
                });

                var filterThreshold = 20;
                if (optionCount > filterThreshold) {
                    cswPrivate.select.$.multiselect().multiselectfilter();
                } else {
                    cswPrivate.select.$.multiselect({
                        selectedList: filterThreshold
                    });
                }

            } ());

            cswPublic.val = function () {
                //In IE an empty array is frequently !== [], rather === null
                var values = cswPrivate.select.$.val() || [],
                    valArray = values.sort();
                return valArray.join(',');
            };

            return cswPublic;
        });


} ());
