/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function () {
    'use strict';
    function select(options) {

        var internal = {
            ID: '',
            selected: '',
            values: [],
            cssclass: '',
            multiple: false,
            width: '',
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

        external.selectedText = function() {
            return external.$.find('option:selected').text();
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

        external.setOptions = function (values, doEmpty) {
            if (Csw.isArray(values) && values.length > 0) {
                if (doEmpty) {
                    external.empty();
                }
                Csw.each(values, function (thisOpt) {
                    var opt = external.makeOption(thisOpt);
                    external.addOption(opt, (opt.value === internal.selected));
                });
            }
            return external;
        };

        (function () {
            var html = '',
                attr = Csw.controls.dom.attributes(),
                style = Csw.controls.dom.style();

            if (options) {
                $.extend(internal, options);
            }

            internal.ID = Csw.string(internal.ID, internal.name);

            attr.add('id', internal.ID);
            attr.add('class', internal.cssclass);
            attr.add('name', internal.name);
            style.add('width', internal.width);
            
            html += '<select ';
            html += attr.get();
            html += style.get();
            html += '>';
            html += '</select>';

            Csw.controls.factory($(html), external);

            if (false === Csw.isNullOrEmpty(internal.$parent)) {
                internal.$parent.append(external.$);
            }

            if (Csw.isFunction(internal.onChange)) {
                external.change(internal.onChange);
            }

            var values = external.makeOptions(internal.values);
            external.setOptions(values);

            if (false === Csw.isNullOrEmpty(internal.value)) {
                external.$.text(internal.value);
            }

            if (Csw.bool(internal.multiple)) {
                external.propDom('multiple', 'multiple');
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
            isSelected: false,
            isDisabled: false
        };
        var external = {

        };

        (function () {
            $.extend(internal, options);

            var html = '<option ',
                $option,
                attr = Csw.controls.dom.attributes(),
                display;

            display = Csw.string(internal.display, internal.value);
            attr.add('value', internal.value);
            attr.add('text', display);
            if (internal.isSelected) {
                attr.add('selected', 'selected');
            }
            if (internal.isDisabled) {
                attr.add('disabled', 'disabled');
            }
            html += attr.get();
            html += '>';
            html += display;
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
