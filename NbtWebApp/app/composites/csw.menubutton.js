
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
                size: 'medium',
                selectedText: '',
                onClick: null,
                state: '',
                width: 240,
                disabled: false
            };
            var cswPublic = {};

            cswPrivate.handleMenuItemClick = function (selectedOption) {
                if (false === Csw.isString(selectedOption)) {
                    selectedOption = cswPrivate.selectedText;
                }
                cswPublic.selectedOption = selectedOption;
                Csw.tryExec(cswPrivate.onClick, selectedOption);
            }; // handleMenuItemClick()

            //constructor
            (function () {
                Csw.extend(cswPrivate, options);
                
                cswParent.empty();
                cswPublic = cswParent.div();

                Csw.each(cswPrivate.menuOptions, function (val, key) {
                    //http://docs.sencha.com/ext-js/4-1/#!/api/Ext.button.Button-event-click
                    cswPrivate.menu.push({ text: val, handler: function () { Csw.tryExec(cswPrivate.handleMenuItemClick, val); } });
                });

                if (Csw.isElementInDom(cswPublic.getId())) {
                    try {
                        cswPublic.menu = window.Ext.create('Ext.button.Split', {
                            id: cswPublic.getId() + 'splitmenu',
                            renderTo: cswPublic.getId(),
                            text: cswPrivate.selectedText,
                            handler: cswPrivate.handleMenuItemClick,
                            scale: Csw.string(cswPrivate.size, 'medium'),
                            width: cswPrivate.width,
                            menu: new window.Ext.menu.Menu({ items: cswPrivate.menu }),
                            disabled: cswPrivate.disabled
                        });
                    } catch (e) {
                        Csw.debug.error('Failed to create Ext.button.Split in csw.menuButton');
                        Csw.debug.error(e);
                    }
                } else {
                    cswPublic.menu = window.Ext.create('Ext.button.Split');
                }

            } ()); // constructor

            cswPublic.disable = function () {

            };

            cswPublic.enable = function () {

            };

            return cswPublic;
        });




} ());
