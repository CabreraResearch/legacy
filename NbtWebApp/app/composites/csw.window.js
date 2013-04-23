/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {

    Csw.composites.window = Csw.composites.window ||
        Csw.composites.register('window', function (cswParent, options) {
            'use strict';
            /// <summary> 
            /// Create and return a Ext.window.Window object
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.name:</para>
            /// <para>options.title:</para>
            /// <para>options.x: The X position of the left edge of the window on initial showing.</para>
            /// <para>options.y: The Y position of the top edge fo the window on initial showing.</para>
            /// <para>options.height: The window height.</para>
            /// <para>options.width: The window width.</para>
            /// <para>options.layout:</para>
            /// <para>options.items:</para>
            /// <para>options.buttons:</para>
            /// </param>
            /// <returns type="Ext.window.Window">A Ext.window.Window object</returns>
            var cswPrivate = {
                name: '',
                title: '',
                y: '',
                x: '',
                height: '',
                width: '',
                layout: 'fit',
                constrain: false,
                items: [{
                    xtype: 'component',
                    layout: 'fit'
                }],
                buttons: []
            };
            var cswPublic = {};

            (function _preCtor() {
                Csw.extend(cswPrivate, options, true);
                cswPrivate.div = cswParent.div({ cssclass: 'cswInline' });
            }());

            (function _postCtor() {
                
                if (Csw.isElementInDom(cswPrivate.div.getId())) {
                    cswPublic.window = window.Ext.create('Ext.window.Window', {
                        title: cswPrivate.title,
                        y: cswPrivate.y,
                        x: cswPrivate.x,
                        height: cswPrivate.height,
                        width: cswPrivate.width,
                        layout: cswPrivate.layout,
                        items: cswPrivate.items,
                        buttons: cswPrivate.buttons,
                        renderTo: cswPrivate.div.getId(),
                        constrain: cswPrivate.constrain
                    }).show();
                }
            }());

            cswPublic.attachToMe = function () {

                var componentToAttachTo;
                
                if(Csw.isElementInDom(cswPublic.window.getId()))
                {
                    componentToAttachTo = Csw.domNode({
                        el: cswPublic.window.items.items[0].getEl().dom,
                        ID: cswPublic.window.items.items[0].getEl().id
                    }).css({ overflow: 'auto' });
                }
                
                return componentToAttachTo;
            };

            cswPublic.close = function () {
                cswPublic.window.close();
            }; // close()

            return cswPublic;
        });
}());

