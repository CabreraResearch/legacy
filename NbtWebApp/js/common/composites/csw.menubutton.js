/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/thirdparty/extjs-4.1.0/ext-all-debug.js" />

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
                size: 'medium',
                selectedText: '',
                onClick: null,
                state: '',
                width: 240
            };
            var cswPublic = {};
            
            cswPrivate.handleMenuItemClick = function(selectedOption) {
                if (false === Csw.isNullOrEmpty(selectedOption)) {
                    cswPrivate.onClick(selectedOption);
                    
                } // if( false === Csw.isNullOrEmpty(menuItemJson))
            }; // handleMenuItemClick()


            //constructor
            (function () {
                if (options) {
                    $.extend(cswPrivate, options);
                }
                cswParent.empty();

                Csw.each(cswPrivate.menuOptions, function(val, key) {
                    //http://docs.sencha.com/ext-js/4-1/#!/api/Ext.button.Button-event-click
                    cswPrivate.menu.push({ text: val, handler: function () { Csw.tryExec(cswPrivate.handleMenuItemClick, val); } });
                });
                
                Ext.create('Ext.button.Split', {
                    renderTo: cswParent.getId(),
                    text: cswPrivate.selectedText,
                    handler: cswPrivate.handleMenuItemClick,
                    scale: Csw.string(cswPrivate.size, 'medium'),
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
