/// <reference path="~app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.radiobutton = Csw.controls.radiobutton ||
        Csw.controls.register('radiobutton', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                prefix: '',
                ReadOnly: false,
                Multi: false,
                names: [],
                onChange: function () {
                },
                value: '',
                cssClass: '',
                radio: [],
                checkedIndex: ''
            };

            var cswPublic = {};

            Csw.tryExec(function () {

                (function preInit() {
                    if (options) {
                        Csw.extend(cswPrivate, options);
                    }

                    cswPublic = cswParent.div();
                    var radioTable = cswPublic.table({
                        ID: cswPrivate.ID,
                        cellvalign: 'middle'
                    });
                    cswPrivate.names.forEach(function (name, index, array) {
                        cswPrivate.radio[index] =
                        radioTable.cell(index+1, 1).input({
                            ID: cswPrivate.ID + index,
                            name: cswPrivate.ID,
                            type: Csw.enums.inputTypes.radio,
                            value: name,
                            cssclass: cswPrivate.cssClass,
                            onChange: function () {
                                cswPrivate.value = cswPrivate.radio[index].val();
                                cswPrivate.onChange();
                            },
                            checked: cswPrivate.checkedIndex === index
                        });
                        radioTable.cell(index+1, 2).span({ text: name });
                    });
                } ());

                cswPublic.val = function (value) {
                    var ret;
                    if (Csw.isNullOrEmpty(value)) {
                        ret = cswPrivate.value;
                    } else {
                        ret = cswPublic;
                        cswPublic.propNonDom('value', value);
                        cswPublic.propDom('title', value);
                        cswPrivate.value = value;
                    }
                    return ret;
                };

                (function postInit() {

                    if (cswPrivate.ReadOnly) {
                        cswPrivate.radio = cswParent.div(cswPrivate);
                    }

                    //                    cswPrivate.radio.bind('change', function () {
                    //                        if (!Csw.bool(cswPrivate.ReadOnly)) {
                    //                            cswPublic.val(cswPrivate.value);
                    //                            cswPublic.propNonDom('value', cswPrivate.value);
                    //                            cswPublic.propDom('title', cswPrivate.value);
                    //                            cswPrivate.onChange();
                    //                        }
                    //                    });

                    cswPublic.val(cswPrivate.value);
                } ());

            });

            return cswPublic;
        });

} ());
