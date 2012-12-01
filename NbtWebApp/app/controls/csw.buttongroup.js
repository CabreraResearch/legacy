/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.buttonGroup = Csw.controls.buttonGroup ||
        Csw.controls.register('buttonGroup', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                buttons: {
                    previous: {
                        tooltip: {
                            title: 'Go Back a Step'
                        },
                        suffix: 'previous',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.back),
                        text: 'Previous',
                        enabled: true,
                        hidden: false,
                        onclick: null
                    },
                    next: {
                        tooltip: {
                            title: 'Go to Next Step' 
                        },
                        suffix: 'next',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.right),
                        text: 'Next',
                        enabled: true,
                        hidden: false,
                        onclick: null
                    },
                    finish: {
                        tooltip: {
                            title: 'Submit Action'
                        },
                        suffix: 'finish',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                        text: 'Finish',
                        hidden: false,
                        enabled: true,
                        onclick: null
                    }

                },
                cancel: {
                    tooltip: {
                        title: 'Cancel Action'
                    },
                    suffix: 'cancel',
                    icon: Csw.enums.getName( Csw.enums.iconType, Csw.enums.iconType.cancel),
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
                    Csw.extend(cswPrivate, options, true);
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
                cswPrivate.leftTbl = cswPrivate.bCell11.table();
                
                cswPrivate.bCell12 = cswPrivate.buttonTbl.cell(1, 2);
                cswPrivate.bCell12.propDom({
                    'align': 'right',
                    'width': '35%'
                });

                cswPrivate.makeButton = function (thisBtn, cell) {
                    cswPublic[thisBtn.suffix] = cell.buttonExt({
                        icon: thisBtn.icon,
                        tooltip: thisBtn.tooltip,
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

                var i = 0;
                Csw.each(cswPrivate.buttons, function(btn, name) {
                    i += 1;
                    cswPrivate.makeButton(btn, cswPrivate.leftTbl.cell(1, i));
                });

                cswPrivate.makeButton(cswPrivate.cancel, cswPrivate.bCell12);
                
            } catch (exception) {
                Csw.error.catchException(exception);
            }
            return cswPublic;
        });

} ());
