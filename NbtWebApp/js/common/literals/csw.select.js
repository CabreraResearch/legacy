/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    Csw.literals.select = Csw.literals.select ||
        Csw.literals.register('select', function select(options) {

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

            external.selectedText = function () {
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

            external.option = function (optionOpts) {
                var optInternal = {
                    value: '',
                    display: '',
                    isSelected: false,
                    isDisabled: false
                };
                var optExternal = {

                };

                (function () {
                    $.extend(optInternal, optionOpts);

                    var html = '<option ',
                        $option,
                        attr = Csw.makeAttr(),
                        display;

                    display = Csw.string(optInternal.display, optInternal.value);
                    attr.add('value', optInternal.value);
                    attr.add('text', display);
                    if (optInternal.isSelected) {
                        attr.add('selected', 'selected');
                    }
                    if (optInternal.isDisabled) {
                        attr.add('disabled', 'disabled');
                    }
                    html += attr.get();
                    html += '>';
                    html += display;
                    html += '</option>';
                    $option = $(html);

                    Csw.literals.factory($option, optExternal);
                    external.append($option);
                } ());

                return optExternal;
            };

            (function() {
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

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

                Csw.literals.factory($(html), external);

                if (false === Csw.isNullOrEmpty(internal.$parent)) {
                    internal.$parent.append(external.$);
                }

                if (Csw.isFunction(internal.onChange)) {
                    external.change(internal.onChange);
                }

                var values = external.makeOptions(internal.values);
                external.setOptions(values);

                if (false === Csw.isNullOrEmpty(internal.value)) {
                    external.text(internal.value);
                }

                if (Csw.bool(internal.multiple)) {
                    external.propDom('multiple', 'multiple');
                }

            }());

            return external;
        });

} ());
