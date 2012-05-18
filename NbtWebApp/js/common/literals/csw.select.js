/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    Csw.literals.select = Csw.literals.select ||
        Csw.literals.register('select', function select(options) {

            var cswPrivateVar = {
                ID: '',
                selected: '',
                values: [],
                cssclass: '',
                multiple: false,
                width: '',
                onChange: null //function () {}
            };
            var cswPublicRet = {};


            cswPublicRet.change = function (func) {
                /// <summary>Trigger or assign a button click event.</summary>
                /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
                /// <returns type="button">The button object.</returns>
                if (Csw.isFunction(func)) {
                    cswPublicRet.bind('change', func);
                } else {
                    cswPublicRet.trigger('change');
                }
                return cswPublicRet;
            };

            cswPublicRet.selectedText = function () {
                return cswPublicRet.$.find('option:selected').text();
            };

            cswPublicRet.makeOption = function (opt) {
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

            cswPublicRet.makeOptions = function (valueArray) {
                var values = [];
                Csw.crawlObject(valueArray, function (val) {
                    var value = cswPublicRet.makeOption(val);
                    if (false === Csw.isNullOrEmpty(value)) {
                        values.push(value);
                    }
                }, false);
                return values;
            };

            cswPublicRet.addOption = function (thisOpt, isSelected) {
                var value = Csw.string(thisOpt.value),
                    display = Csw.string(thisOpt.display),
                    opt = cswPublicRet.option({ value: value, display: display, isSelected: isSelected });

                if (false === Csw.isNullOrEmpty(value.data)) {
                    opt.data(value.dataName, value.data);
                }
                return opt;
            };

            cswPublicRet.setOptions = function (values, doEmpty) {
                if (Csw.isArray(values) && values.length > 0) {
                    if (doEmpty) {
                        cswPublicRet.empty();
                    }
                    Csw.each(values, function (thisOpt) {
                        var opt = cswPublicRet.makeOption(thisOpt);
                        cswPublicRet.addOption(opt, (opt.value === cswPrivateVar.selected));
                    });
                }
                return cswPublicRet;
            };

            cswPublicRet.option = function (optionOpts) {
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
                    cswPublicRet.append($option);
                } ());

                return optExternal;
            };

            (function() {
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.ID = Csw.string(cswPrivateVar.ID, cswPrivateVar.name);

                attr.add('id', cswPrivateVar.ID);
                attr.add('class', cswPrivateVar.cssclass);
                attr.add('name', cswPrivateVar.name);
                style.add('width', cswPrivateVar.width);

                html += '<select ';
                html += attr.get();
                html += style.get();
                html += '>';
                html += '</select>';

                Csw.literals.factory($(html), cswPublicRet);

                if (false === Csw.isNullOrEmpty(cswPrivateVar.$parent)) {
                    cswPrivateVar.$parent.append(cswPublicRet.$);
                }

                if (Csw.isFunction(cswPrivateVar.onChange)) {
                    cswPublicRet.change(cswPrivateVar.onChange);
                }

                var values = cswPublicRet.makeOptions(cswPrivateVar.values);
                cswPublicRet.setOptions(values);

                if (false === Csw.isNullOrEmpty(cswPrivateVar.value)) {
                    cswPublicRet.text(cswPrivateVar.value);
                }

                if (Csw.bool(cswPrivateVar.multiple)) {
                    cswPublicRet.propDom('multiple', 'multiple');
                }

            }());

            return cswPublicRet;
        });

} ());
