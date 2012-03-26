/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    Csw.components.multiSelect = Csw.components.multiSelect ||
        Csw.components.register('multiSelect', function (cswParent, options) {

            var internal = {
                $parent: '',
                ID: '',
                values: [],
                multiple: true,
                cssclass: '',
                isMultiEdit: false,
                onChange: null, //function () {}
                isControl: false
            };

            var external = {};

            (function () {

                if (options) {
                    $.extend(internal, options);
                }

                var optionCount = 0,
                    isMultiEdit = Csw.bool(internal.isMultiEdit),
                    values = internal.values;
                delete internal.values;

                internal.select = cswParent.select(internal);
                external = Csw.dom({ }, internal.select);
                //$.extend(external, Csw.literals.select(internal));

                if (Csw.isFunction(internal.onChange)) {
                    internal.select.bind('change', function () {
                        internal.onChange(internal.select);
                    });
                }

                Csw.each(values, function (opt) {
                    var value = Csw.string(opt.value, opt.text),
                        text = Csw.string(opt.text, value),
                        isSelected;
                    if (false === Csw.isNullOrEmpty(value)) {
                        isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                        internal.select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                        optionCount += 1;
                    }
                });

                if (optionCount > 20) {
                    internal.select.$.multiselect().multiselectfilter();
                } else {
                    internal.select.$.multiselect();
                }
            } ());

            external.val = function () {
                //In IE an empty array is frequently !== [], rather === null
                var values = internal.select.$.val() || [],
                    valArray = values.sort();
                return valArray.join(',');
            };

            return external;
        });


} ());
