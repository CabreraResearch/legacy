/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.buttonGroup = Csw.controls.buttonGroup ||
        Csw.controls.register('buttonGroup', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                buttons: {
                    previous: {
                        suffix: 'previous',
                        text: '< Previous',
                        enabled: true,
                        hidden: false,
                        onclick: null
                    },
                    next: {
                        suffix: 'next',
                        text: 'Next >',
                        enabled: true,
                        hidden: false,
                        onclick: null
                    },
                    finish: {
                        suffix: 'finish',
                        text: 'Finish',
                        hidden: false,
                        enabled: true,
                        onclick: null
                    }

                },
                cancel: {
                    suffix: 'cancel',
                    text: 'Cancel',
                    enabled: true,
                    hidden: false,
                    onclick: null
                }
            };
            var cswPublic = {};
            try {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Button Group without a valid Csw Parent object.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 14);
                }

                if (options) {
                    $.extend(true, cswPrivate, options);
                }

                cswPrivate.buttonTbl = cswParent.table({
                    suffix: 'btntbl',
                    width: '100%'
                });
                cswPrivate.buttonTbl.addClass('CswWizard_ButtonsCell');
                //cswPublic = Csw.dom({}, cswPrivate.buttonTbl);
                
                cswPrivate.bCell11 = cswPrivate.buttonTbl.cell(1, 1);
                cswPrivate.bCell11.propDom({
                    'align': 'right',
                    'width': '65%'
                });
                cswPrivate.bCell12 = cswPrivate.buttonTbl.cell(1, 2);
                cswPrivate.bCell12.propDom({
                    'align': 'right',
                    'width': '35%'
                });

                cswPrivate.makeButton = function (thisBtn, cell) {
                    cswPublic[thisBtn.suffix] = cell.button({
                        suffix: thisBtn.suffix,
                        enabledText: thisBtn.text,
                        disableOnClick: false,
                        onClick: function () {
                            Csw.tryExec(thisBtn.onclick);
                        }
                    });
                    if (false === thisBtn.enabled) {
                        cswPublic[thisBtn.suffix].disable();
                    }
                    if (thisBtn.hidden) {
                        cswPublic[thisBtn.suffix].hide();
                    }
                };

                Csw.each(cswPrivate.buttons, function(btn, name) {
                    cswPrivate.makeButton(btn, cswPrivate.bCell11);
                });

                cswPrivate.makeButton(cswPrivate.cancel, cswPrivate.bCell12);
                
            } catch (exception) {
                Csw.error.catchException(exception);
            }
            return cswPublic;
        });

} ());
