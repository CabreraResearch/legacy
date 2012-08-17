/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    Csw.literals.select = Csw.literals.select ||
        Csw.literals.register('select', function select(options) {

            var cswPrivate = {
                ID: '',
                selected: '',
                values: [],
                cssclass: '',
                multiple: false,
                width: '',
                onChange: null, //function () {}
                onComplete: null
            };
            var cswPublic = {};


            cswPublic.change = function (func) {
                /// <summary>Trigger or assign a button click event.</summary>
                /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
                /// <returns type="button">The button object.</returns>
                if (Csw.isFunction(func)) {
                    cswPublic.bind('change', func);
                } else {
                    cswPublic.trigger('change');
                }
                return cswPublic;
            };

            cswPublic.selectedText = function () {
                return cswPublic.$.find('option:selected').text();
            };

            cswPublic.selectedVal = function () {
                return cswPublic.val();
            };

            cswPublic.makeOption = function (opt) {
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

            cswPublic.makeOptions = function (valueArray) {
                var values = [];
                Csw.crawlObject(valueArray, function (val) {
                    var value = cswPublic.makeOption(val);
                    if (false === Csw.isNullOrEmpty(value)) {
                        values.push(value);
                    }
                }, false);
                return values;
            };

            cswPublic.addOption = function (thisOpt, isSelected) {
                var value = Csw.string(thisOpt.value),
                    display = Csw.string(thisOpt.display),
                    opt = cswPublic.option({ value: value, display: display, isSelected: isSelected });

                if (false === Csw.isNullOrEmpty(value.data)) {
                    opt.data(value.dataName, value.data);
                }
                return opt;
            };

            cswPublic.setOptions = function (values, doEmpty) {
                if (Csw.isArray(values) && values.length > 0) {
                    if (doEmpty) {
                        cswPublic.empty();
                    }
                    Csw.each(values, function (thisOpt) {
                        var opt = cswPublic.makeOption(thisOpt);
                        cswPublic.addOption(opt, (opt.value === cswPrivate.selected));
                    });
                }
                Csw.tryExec(cswPrivate.onComplete, cswPublic.selectedVal());
                return cswPublic;
            };

            cswPublic.option = function (optionOpts) {
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
                    cswPublic.append($option);
                } ());

                return optExternal;
            };

            (function() {
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswPrivate.ID = Csw.string(cswPrivate.ID, cswPrivate.name);

                attr.add('id', cswPrivate.ID);
                attr.add('class', cswPrivate.cssclass);
                attr.add('name', cswPrivate.name);
                style.add('width', cswPrivate.width);

                html += '<select ';
                html += attr.get();
                html += style.get();
                html += '>';
                html += '</select>';

                Csw.literals.factory($(html), cswPublic);

                if (false === Csw.isNullOrEmpty(cswPrivate.$parent)) {
                    cswPrivate.$parent.append(cswPublic.$);
                }

                if (Csw.isFunction(cswPrivate.onChange)) {
                    cswPublic.change(cswPrivate.onChange);
                }

                var values = cswPublic.makeOptions(cswPrivate.values);
                cswPublic.setOptions(values);

                if (false === Csw.isNullOrEmpty(cswPrivate.value)) {
                    cswPublic.text(cswPrivate.value);
                }

                if (Csw.bool(cswPrivate.multiple)) {
                    cswPublic.propDom('multiple', 'multiple');
                }

            }());

            return cswPublic;
        });

} ());
