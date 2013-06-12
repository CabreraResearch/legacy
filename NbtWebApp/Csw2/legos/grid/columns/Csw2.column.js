/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _columnIIFE(nameSpace) {

    /**
     * Ext xtypes constant. Possible values: 'checkcolumn', 'actioncolumn', 'gridcolumn'
    */
    var xtypes = nameSpace.object();
    xtypes.checkcolumn = 'checkcolumn';
    xtypes.gridcolumn = 'gridcolumn';
    xtypes.actioncolumn = 'actioncolumn';
    nameSpace.constant(nameSpace.grids, 'xtypes', xtypes);


    /**
     * Private column constructor class
     * @param xtyle {nameSpace.constants.xtype} [xtype=nameSpace.grids.constants.xtypes.gridcolumn] The type of column
     * @param sortable {Boolean} [sortable=true] Is Column Sortable
     * @param text {String} Column name
     * @param flex {Number} [flex=0.125] relative Column width
     * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu disabled?
     * @param dataIndex {String} [dataIndex=text] Unique Index Id for the column
     * @param editor {String} If the column is editable, type of editor
    */
    var Column = function (xtype, sortable, text, flex, menuDisabled, dataIndex, editor) {
        'use strict';
        var that = this;

        if(false === nameSpace.grids.constants.xtypes.has(xtype)) {
            xtype = nameSpace.grids.constants.xtypes.gridcolumn;
        }
        if(!text) {
           // throw new Error('Text is required for column construction.');
        }

        nameSpace.property(that, 'xtype', xtype);
            
        if (sortable === true || sortable === false) {
            nameSpace.property(that, 'sortable', sortable);
        }
        if (text && text !== '' ) {
            nameSpace.property(that, 'text', text);
        }
        if (flex && flex !== 0) {
            nameSpace.property(that, 'flex', flex);
        }
        if (menuDisabled === true || menuDisabled === false) {
            nameSpace.property(that, 'menuDisabled', menuDisabled);
        }
        var idx = (dataIndex || text).toLowerCase();
        nameSpace.property(that, 'dataIndex', idx);
        
        if(editor) {
            nameSpace.property(that, 'editor', editor);
        }

        return that;
    };

    nameSpace.instanceOf.lift('Column', Column);

    nameSpace.grids.columns.lift('column',
        /**
         * Create a column definition.
         * @param def {Object} Possible property members: def.xtype, def.sortable, def.text, def.flex, def.menuDisabled, def.dataIndex, def.editor
        */
        function (def) {
        'use strict';
        if (!def) {
            throw new Error('Cannot create a column without parameters');
        }
        var ret = new Column(def.xtype, def.sortable, def.text, def.flex, def.menuDisabled, def.dataIndex, def.editor);
        return ret;
    });


}(window.$om$));