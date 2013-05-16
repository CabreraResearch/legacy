/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _columnIIFE(){

    /**
     * Ext xtypes constant. Possible values: 'checkcolumn', 'actioncolumn', 'gridcolumn'
    */
    var xtypes = Object.create(null);
    xtypes.checkcolumn = 'checkcolumn';
    xtypes.gridcolumn = 'gridcolumn';
    xtypes.actioncolumn = 'actioncolumn';
    Csw2.constant(Csw2.grids, 'xtypes', xtypes);


    /**
     * Private column constructor class
     * @param xtyle {Csw2.constants.xtype} [xtype=Csw2.grids.constants.xtypes.gridcolumn] The type of column
     * @param sortable {Boolean} [sortable=true] Is Column Sortable
     * @param text {String} Column name
     * @param flex {Number} [flex=0.125] relative Column width
     * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu disabled?
     * @param dataIndex {String} [dataIndex=text] Unique Index Id for the column
     * @param editor {String} If the column is editable, type of editor
    */
    var Column = function (xtype, sortable, text, flex, menuDisabled, dataIndex, editor) {
        var that = this;

        if(false === Csw2.grids.constants.xtypes.has(xtype)) {
            xtype = Csw2.grids.constants.xtypes.gridcolumn;
        }
        if(!text) {
           // throw new Error('Text is required for column construction.');
        }

        Csw2.property(that, 'xtype', xtype);
            
        if (sortable === true || sortable === false) {
            Csw2.property(that, 'sortable', sortable);
        }
        if (text && text !== '' ) {
            Csw2.property(that, 'text', text);
        }
        if (flex && flex !== 0) {
            Csw2.property(that, 'flex', flex);
        }
        if (menuDisabled === true || menuDisabled === false) {
            Csw2.property(that, 'menuDisabled', menuDisabled);
        }
        Csw2.property(that, 'dataIndex', dataIndex || text);
        
        if(editor) {
            Csw2.property(that, 'editor', editor);
        }

        return that;
    };

    Csw2.instanceOf.lift('Column', Column);

    /**
     * Create a column definition.
     * @param def {Object} Possible property members: def.xtype, def.sortable, def.text, def.flex, def.menuDisabled, def.dataIndex, def.editor
    */
    Csw2.grids.columns.lift('column', function (def){
        if(!def) {
            throw new Error('Cannot create a column without parameters');
        }
        var ret = new Column(def.xtype, def.sortable, def.text, def.flex, def.menuDisabled, def.dataIndex, def.editor);
        return ret;
    });


    }());