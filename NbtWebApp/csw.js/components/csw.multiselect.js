/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    function multiSelect(options) {

        var internal = {
            $parent: '',
            ID: '',
            values: [], 
            multiple: true,
            cssclass: '',
            isMultiEdit: false,
            onChange: null //function () {}
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
            
            $.extend(external, Csw.controls.select(internal));

            if (Csw.isFunction(internal.onChange)) {
                select.bind('change', function () {
                    internal.onChange(select);
                });
            }

            Csw.each(values, function (opt) {
                var value = Csw.string(opt.value, opt.text),
                    text = Csw.string(opt.text, value),
                    isSelected;
                if (false === Csw.isNullOrEmpty(value)) {
                    isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                    external.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                    optionCount += 1;
                }
            });

            if (optionCount > 20) {
                external.$.multiselect().multiselectfilter();
            } else {
                external.$.multiselect();
            }
        } ());

        external.val = function () {
            //In IE an empty array is frequently !== [], rather === null
            var values = external.$.val() || [],
                valArray = values.sort();
            return valArray.join(',');
        };

        return external;
    }

    Csw.controls.register('multiSelect', multiSelect);
    Csw.controls.multiSelect = Csw.controls.multiSelect || multiSelect;

} ());
