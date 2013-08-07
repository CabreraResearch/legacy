/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {

    Csw.composites.buttonSplit = Csw.composites.buttonSplit ||
        Csw.composites.register('buttonSplit', function (cswParent, options) {
            'use strict';
            /// <summary> 
            /// Create a new Ext.splitButton and return a Csw.buttonSplit object
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.ID: An ID for the button.</para>
            /// <para>options.buttonText: Text to display on the button</para>
            /// <para>options.width: the width of the button</para>
            /// <para>options.menu: the default list of menu items</para>
            /// <para>options.buttonClickHandler: Event to execute when the button is clicked</para>
            /// <para>options.arrowHandler: Event raised when splitButton's arrow is clicked</para>
            /// <para>options.menuClickHandler: Event raised when a menu option is clicked</para>
            /// </param>
            /// <returns type="Ext.SplitButton">An Ext.SplitButton object</returns>
            var cswPrivate = {
                name: '',
                buttonText: 'SplitButton',
                width: '100px',
                menu: { items: [] },
            };
            var cswPublic = {};

            (function _preCtor() {
                Csw.extend(cswPrivate, options, true);
                cswPublic = cswParent.div({ cssclass: 'cswInline' });
            }());

            //expose the ability to dynamically set the button's text
            cswPublic.setText = Csw.method(function (text) {
                cswPublic.buttonSplit.setText(text);
                return cswPublic;
            });

            //expose the ability to dynamically set the button click handler
            cswPublic.setHandler = Csw.method(function (func) {
                cswPublic.buttonSplit.setHandler(func);
                return cswPublic;
            });

            //expose the ability to dynamically set the arrow click handler
            cswPublic.setArrowHandler = Csw.method(function (func) {
                cswPublic.buttonSplit.setArrowHandler(func);
                return cswPublic;
            });

            (function _postCtor() {



                if (Csw.isElementInDom(cswPublic.getId())) {
                    try {

                        cswPublic.buttonSplit = window.Ext.create('Ext.SplitButton', {
                            id: cswPrivate.ID + 'splitbutton',
                            text: Csw.string(cswPrivate.buttonText),
                            width: cswPrivate.width,
                            renderTo: cswPublic.getId(),
                            menu: cswPrivate.menu,
                            handler: cswPrivate.menuClickHandler,
                            arrowHandler: cswPrivate.arrowHandler
                        });

                    } catch (e) {
                        cswPublic.button = window.Ext.create('Ext.SplitButton');
                        Csw.debug.error('Failed to create Ext.SplitButton in csw.buttonSplit');
                        Csw.debug.error(e);
                    }
                } else {
                    cswPublic.button = window.Ext.create('Ext.SplitButton');
                }

            }());

            return cswPublic;

        });

}());

