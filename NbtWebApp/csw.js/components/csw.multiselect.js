/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    function multiSelect(options) {

        var internal = {
            cswParent: {},
            ID: '',
            values: [], /* [{ value: '', text: '', selected: '', disabled: ''}], */
            cssclass: '',
            isMultiEdit: false,
            onChange: null //function () {}
        };

        var external = {};

        (function () {

            if (options) {
                $.extend(internal, options);
            }
            
            var elementId = Csw.string(internal.ID),
                select = internal.cswParent.select({ multiple: true, ID: elementId }),
                optionCount = 0,
                isMultiEdit = Csw.bool(internal.isMultiEdit);

            if (false === Csw.isNullOrEmpty(internal.cssclass)) {
                select.addClass(internal.cssclass);
            }

            if (Csw.isFunction(internal.onChange)) {
                select.bind('change', function () {
                    internal.onChange(select);
                });
            }

            Csw.each(internal.values, function (opt) {
                var value = Csw.string(opt.value),
                    text = Csw.string(opt.text, value),
                    isSelected;
                if (false === Csw.isNullOrEmpty(value)) {
                    isSelected = (Csw.bool(opt.selected) &&
                        false === isMultiEdit);
                    select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                    optionCount += 1;
                }
            });

            if (optionCount > 20) {
                select.$.multiselect().multiselectfilter();
            } else {
                select.$.multiselect();
            }
        } ());

        external.val = function () {
            //In IE an empty array is frequently !== [], rather === null
                values = $select.val() || [],
                valArray = values.sort();
            return valArray.join(',');
        };

        return external;
    }

    Csw.controls.register('multiSelect', multiSelect);
    Csw.controls.multiSelect = Csw.controls.multiSelect || multiSelect;

} ());
