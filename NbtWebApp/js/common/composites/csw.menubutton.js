/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="http://cdn.sencha.io/ext-4.1.0-gpl/ext-all-debug.js" />

(function () {
    'use strict';

    Csw.composites.menuButton = Csw.composites.menuButton ||
        Csw.composites.register('menuButton', function (cswParent, options) {
            Ext.require('Ext.button.*');
            Ext.require('Ext.menu.*');

            var cswPrivate = {
                ID: '',
                menuOptions: ['Menu Item 1', 'Menu Item 2'],
                menu: [],
                selectedText: '',
                onClick: null,
                state: '',
                width: 240
            };
            var cswPublic = {};
            
            cswPrivate.handleMenuItemClick = function(menuItemName, menuItemJson) {
                if(false === Csw.isNullOrEmpty(menuItemJson)) {
                
                    
                } // if( false === Csw.isNullOrEmpty(menuItemJson))
            }; // handleMenuItemClick()


            //constructor
            (function () {
                if (options) {
                    $.extend(cswPrivate, options);
                }
                cswParent.empty();

                Csw.each(cswPrivate.menuOptions, function(val, key) {
                    cswPrivate.menu.push({ text: val, handler: function(par1, par2) { Csw.tryExec(cswPrivate.onClick, par1, par2); } });
                });

                Ext.create('Ext.button.Split', {
                    renderTo: cswParent.getId(),
                    text: cswPrivate.selectedText,
                    handler: cswPrivate.onClick,
                    scale: 'medium',
                    menu: new Ext.menu.Menu({items: cswPrivate.menu})
                });

            } ()); // constructor

            cswPublic.disable = function() {

            };

            cswPublic.enable = function () {

            };

            return cswPublic;
        });


    

} ());
