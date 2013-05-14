/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _panelIIFE() {

    /**
     * Private class representing the construnction of a panel. It returns a Csw2.panel.panel instance with collections for adding columns and listeners.
     * @param panelName {String} The ClassName of the panel to associate with ExtJS
     * @param requires {Array} An array of ExtJS dependencies
     * @param extend {String} [extend='Ext.panel.Panel'] An ExtJs class name to extend, usually the panel panel
     * @param alias {Array} [alias] An array of aliases to reference the panel
     * @param id {String} An id to uniquely identify the panel
     * @param store {Csw2.panels.stores.store} A store to provide data to the panel
     * @param plugins {Array} An array of plugins to load with the panel
    */
    var Panel = function(panelName, requires, extend, alias, id, store, plugins) {
        var that = this;

        var classDef = window.Csw2.classDefinition({
            requires: requires,
            extend: extend || 'Ext.panel.Panel',
            alias: alias,
            id: id,
            store: store,
            plugins: plugins
        });

        Csw2.property(classDef, 'initComponent', function() {
            this.callParent(arguments);
        });

        Csw2.property(that, 'listeners', Csw2.panels.listeners.listeners());
        
        Csw2.property(that, 'init', function () {
            Csw2.property(classDef, 'listeners', that.listeners);
            
            return Csw2.define(panelName, classDef);
        });

        return that;
    };

    Csw2.instanceof.lift('Panel', Panel);

    /**
     * Create a panel object.
     * @returns {Csw.panels.panel} A panel object. Exposese listeners and columns collections. Call init when ready to construct the panel. 
    */
    Csw2.panels.lift('panel', function(panelName, panelDef) {
        if(!(panelDef)) {
            throw new Error('Cannot instance a panel without properties');
        }
        if (!(panelName)) {
            throw new Error('Cannot instance a panel without a classname');
        }
        var panel = new Panel(panelName, panelDef.requires, panelDef.extend, panelDef.alias, panelDef.id, panelDef.store, panelDef.plugins);
        return panel;
    });


}());