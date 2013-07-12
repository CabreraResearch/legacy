
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {
    'use strict';

    Csw.composites.menuButton = Csw.composites.menuButton ||
        Csw.composites.register('menuButton', function (cswParent, options) {

            var cswPrivate = {
                name: '',
                menuOptions: [], // ['Menu Item 1', 'Menu Item 2'],
                menu: [],
                size: 'small',
                selectedText: '',
                onClick: null,
                state: '',
                width: '100px',
                disabled: false,
                icon: '',
                propId: ''
            };
            var cswPublic = {};
            var _menuLoaded = false;

            cswPrivate.handleMenuItemClick = Csw.method(function (selectedOption) {
                if (false === Csw.isString(selectedOption)) {
                    selectedOption = cswPrivate.selectedText;
                }
                cswPublic.selectedOption = selectedOption;
                Csw.tryExec(cswPrivate.onClick, selectedOption);
            }); // handleMenuItemClick()

            //constructor
            (function () {
                Csw.extend(cswPrivate, options);

                cswParent.empty();
                cswPublic = cswParent.div();

                if (cswPrivate.menuOptions.length === 0 && cswPrivate.menu.length === 0) {
                    cswPrivate.menu.push({ /*dummy menu, the menu will be populated via a webservice call */ });
                } else {
                    Csw.each(cswPrivate.menuOptions, function (val, key) {
                        //http://docs.sencha.com/ext-js/4-1/#!/api/Ext.button.Button-event-click
                        cswPrivate.menu.push({ text: val, handler: function () { Csw.tryExec(cswPrivate.handleMenuItemClick, val); } });
                    });
                }
                var btnMenu = new window.Ext.menu.Menu({ items: cswPrivate.menu });

                if (Csw.isElementInDom(cswPublic.getId())) {
                    try {
                        cswPublic.menu = window.Ext.create('Ext.button.Split', {
                            id: cswPublic.getId() + 'splitmenu',
                            renderTo: cswPublic.getId(),
                            icon: cswPrivate.icon,
                            text: cswPrivate.selectedText,
                            handler: cswPrivate.handleMenuItemClick,
                            scale: Csw.string(cswPrivate.size, 'medium'),
                            width: cswPrivate.width,
                            menu: btnMenu,
                            disabled: cswPrivate.disabled,
                            arrowHandler: function () {
                                if (false === _menuLoaded) {
                                    btnMenu.remove(0); //remove the dummy item

                                    Csw.ajaxWcf.post({
                                        urlMethod: 'Properties/GetButtonOpts',
                                        data: cswPrivate.propId,
                                        success: function (response) {
                                            Csw.each(response.Opts, function (opt) {
                                                btnMenu.add({ text: opt, handler: function () { Csw.tryExec(cswPrivate.handleMenuItemClick, opt); } });
                                            });
                                            _menuLoaded = true;
                                        }
                                    });

                                }
                            }
                        });
                    } catch (e) {
                        Csw.debug.error('Failed to create Ext.button.Split in csw.menuButton');
                        Csw.debug.error(e);
                    }
                } else {
                    cswPublic.menu = window.Ext.create('Ext.button.Split');
                }

            }()); // constructor

            cswPublic.disable = function () {
                cswPublic.menu.disable();
            };

            cswPublic.enable = function () {
                cswPublic.menu.enable();
            };

            return cswPublic;
        });




}());
