/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function () {
    "use strict";
    function select(options) {

        var internal = {
            ID: '',
            selected: '',
            values: [],
            cssclass: '',
            multiple: false,
            onChange: null //function () {}
        };
        var external = {};


        external.change = function (func) {
            /// <summary>Trigger or assign a button click event.</summary>
            /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
            /// <returns type="button">The button object.</returns>
            if (Csw.isFunction(func)) {
                external.bind('change', func);
            } else {
                external.trigger('change');
            }
            return external;
        };

        external.makeOption = function (opt) {
            var ret, display, value;
            if (Csw.contains(opt, 'value') && Csw.contains(opt, 'display')) {
                ret = opt;
            } else if (Csw.contains(opt, 'value')) {
                value = Csw.string(opt.value);
                ret = { value: value, display: value };
            } else if (Csw.contains(opt, 'display')) {
                display = Csw.string(opt.display);
                ret = { value: display, display: display };
            } else {
                ret = { value: opt, display: opt };
            }
            return ret;
        };

        external.makeOptions = function (valueArray) {
            var values = [];
            Csw.crawlObject(valueArray, function (val) {
                var value = external.makeOption(val);
                if (false === Csw.isNullOrEmpty(value)) {
                    values.push(value);
                }
            }, false);
            return values;
        };

        external.addOption = function (thisOpt, isSelected) {
            var value = Csw.string(thisOpt.value),
                display = Csw.string(thisOpt.display),
                opt = external.option({ value: value, display: display, isSelected: isSelected });

            if (false === Csw.isNullOrEmpty(value.data)) {
                opt.data(value.dataName, value.data);
            }
            return opt;
        };

        external.setOptions = function (values, selected, $select, doEmpty) {
            if (Csw.isArray(values) && values.length > 0) {
                if (doEmpty) {
                    $select.empty();
                }
                Csw.each(values, function (thisOpt) {
                    var opt = external.makeOption(thisOpt);
                    external.addOption(opt, (opt.value === selected), $select);
                });
            }
            return $select;
        };

        (function () {
            var html = '',
                attr = Csw.controls.dom.attributes();
            var $select;

            if (options) {
                $.extend(internal, options);
            }

            internal.ID = Csw.string(internal.ID, internal.name);

            attr.add('id', internal.ID);
            attr.add('class', internal.cssclass);
            attr.add('name', internal.name);

            html += '<select ';

            html += attr.get();

            html += '>';
            html += '</select>';
            $select = $(html);

            Csw.controls.factory($select, external);
            internal.$parent.append(external.$);

            if (Csw.isFunction(internal.onChange)) {
                external.change(internal.onChange);
            }

            var values = external.makeOptions(internal.values);
            external.setOptions(values, internal.selected, $select);

            if (false === Csw.isNullOrEmpty(internal.value)) {
                $select.text(internal.value);
            }

            if (Csw.bool(internal.multiple)) {
                external.propDom('multiple', 'multiple');
                external.$.multiselect();
            }

        } ());

        return external;
    }

    Csw.controls.register('select', select);
    Csw.controls.select = Csw.controls.select || select;

    function option(options) {
        var internal = {
            value: '',
            display: '',
            isSelected: false
        };
        var external = {

        };

        (function () {
            $.extend(internal, options);

            var html = '<option ',
                $option,
                attr = Csw.controls.dom.attributes();

            attr.add('value', internal.value);
            if (internal.isSelected) {
                attr.add('selected', 'selected');
            }
            html += attr.get();
            html += '>';
            html += internal.display;
            html += '</option>';
            $option = $(html);

            Csw.controls.factory($option, external);
            internal.$parent.append(external.$);
        } ());

        return external;
    }
    Csw.controls.register('option', option);
    Csw.controls.option = Csw.controls.option || option;

} ());
