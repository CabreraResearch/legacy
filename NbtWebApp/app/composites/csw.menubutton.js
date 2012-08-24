
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/js/thirdparty/extjs-4.1.0/ext-all-debug.js" />

(function () {
    'use strict';

    Csw.composites.menuButton = Csw.composites.menuButton ||
        Csw.composites.register('menuButton', function (cswParent, options) {
            window.Ext.require('Ext.button.*');
            window.Ext.require('Ext.menu.*');

            var cswPrivate = {
                ID: '',
                menuOptions: ['Menu Item 1', 'Menu Item 2'],
                menu: [],
                size: 'medium',
                selectedText: '',
                onClick: null,
                state: '',
                width: 240
            };
            var cswPublic = {};
            
            cswPrivate.handleMenuItemClick = function(selectedOption) {
                if (false === Csw.isString(selectedOption)) {
                    selectedOption = cswPrivate.selectedText;
                }
                cswPublic.selectedOption = selectedOption;
                Csw.tryExec(cswPrivate.onClick);
            }; // handleMenuItemClick()
            
            //constructor
            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswParent.empty();

                Csw.each(cswPrivate.menuOptions, function(val, key) {
                    //http://docs.sencha.com/ext-js/4-1/#!/api/Ext.button.Button-event-click
                    cswPrivate.menu.push({ text: val, handler: function () { Csw.tryExec(cswPrivate.handleMenuItemClick, val); } });
                });
                
                cswPublic.menu = window.Ext.create('Ext.button.Split', {
                    renderTo: cswParent.getId(),
                    text: cswPrivate.selectedText,
                    handler: cswPrivate.handleMenuItemClick,
                    scale: Csw.string(cswPrivate.size, 'medium'),
                    menu: new window.Ext.menu.Menu({items: cswPrivate.menu})
                });

            } ()); // constructor

            cswPublic.disable = function() {

            };

            cswPublic.enable = function () {

            };

            return cswPublic;
        });


    

} ());
