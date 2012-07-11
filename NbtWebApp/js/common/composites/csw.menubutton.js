/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="http://cdn.sencha.io/ext-4.1.0-gpl/ext-all-debug.js" />

(function () {
    'use strict';

    Csw.composites.menu = Csw.composites.menu ||
        Csw.composites.register('menu', function (cswParent, options) {

            var cswPrivate = {
                ID: '',
                menu: {
                    items: [{
                        text:'Menu Item 1'
                    }]
                },
                onClick: null,
                text: '',
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
                
                Ext.create('Ext.button.Split', {
                    renderTo: cswPrivate.ID,
                    text: cswPrivate.text,
                    handler: cswPrivate.onClick,
                    scale: 'small',
                    menu: new Ext.menu.Menu(cswPrivate.menu)
                });

            } ()); // constructor

            return cswPublic;
        });


    

} ());
