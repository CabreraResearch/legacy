/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.literals.select = Csw.literals.select ||
        Csw.literals.register('select', function select(options) {

            var cswPrivate = {
                ID: '',
                name: '',
                selected: '',
                values: [],
                cssclass: '',
                multiple: false,
                width: '',
                onChange: null, //function () {}
                onComplete: null,
                isRequired: false
            };
            var cswPublic = {};


            cswPublic.change = function (func) {
                if (Csw.isFunction(func)) {
                    cswPublic.bind('change', func, cswPublic.selectedVal());
                } else {
                    cswPublic.trigger('change');
                }
                return cswPublic;
            };

            cswPublic.selectedData = function (propName) {
                var ret = '';
                if(cswPublic.$.find('option:selected') && cswPublic.$.find('option:selected')[0]) {
                    var dataset = cswPublic.$.find('option:selected')[0].dataset;
                    if(dataset) { ret = dataset[propName]; }
                }
                return ret;
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
                } else if (Csw.contains(opt, 'id') && Csw.contains(opt, 'value')) {
                    ret = { value: opt.id, display: opt.value };
                } else if (Csw.contains(opt, 'value')) {
                    value = Csw.string(opt.value);
                    ret = { value: value, display: value };
                } else if (Csw.contains(opt, 'display')) {
                    display = Csw.string(opt.display);
                    ret = { value: display, display: display };
                } else {
                    ret = { value: opt, display: opt };
                }
                ret.isSelected = opt.isSelected || opt.value === cswPrivate.selected;
                return ret;
            };

            cswPublic.makeOptions = function (valueArray) {
                var values = [];
                Csw.iterate(valueArray, function (val) {
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
                    Csw.iterate(values, function (thisOpt) {
                        var opt = cswPublic.makeOption(thisOpt);
                        cswPublic.addOption(opt, opt.isSelected);
                    });
                }
                Csw.tryExec(cswPrivate.onComplete, cswPublic.selectedVal());
                return cswPublic;
            };

            cswPublic.removeOption = function(valueToRemove) {
                cswPrivate.values.splice(cswPrivate.values.indexOf(valueToRemove), 1); //removes the item from the list
                var selectControl = cswPublic[0];
                for (var i = 0; i < selectControl.length; i++) {
                    if (selectControl.options[i].value === valueToRemove) {
                        selectControl.remove(i);
                    }
                }
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
                    Csw.extend(optInternal, optionOpts);

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

            (function () {
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();
                
                Csw.extend(cswPrivate, options);
                
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

                cswPublic.bind('change', function() {
                    Csw.tryExec(cswPrivate.onChange, cswPublic.selectedVal());
                });

                var values = cswPublic.makeOptions(cswPrivate.values);
                cswPublic.setOptions(values);

                if (false === Csw.isNullOrEmpty(cswPrivate.value)) {
                    cswPublic.text(cswPrivate.value);
                }

                if (Csw.bool(cswPrivate.multiple)) {
                    cswPublic.propDom('multiple', 'multiple');
                }

            } ());

            return cswPublic;
        });

} ());
