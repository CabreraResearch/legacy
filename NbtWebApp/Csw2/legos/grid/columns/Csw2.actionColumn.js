/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _actionColumnIIFE(){

    /**
     * Internal action column class
     * @param text {String} Name of the column
    */
    var ActionColumn = function (text) {
        var that = Csw2.grids.columns.column({
                xtype: Csw2.grids.constants.xtypes.actioncolumn,
                width: 60,
                text: text
            });
        Csw2.property(that, 'items', []);
        Csw2.property(that, 'addItem', function(columnItem) {
            if (!(columnItem instanceof Csw2.instanceOf.ColumnItem)) {
                throw new Error('Invalid column item specified for collection.')
            }
            that.items.push(columnItem);
            return that;
        },false, false, false);
                    

        return that;
    };

    Csw2.instanceOf.lift('ActionColumn', ActionColumn);

    /**
     * Create an action column
     * @param sortable {Boolean} [sortable=true] Is Column Sortable
     * @param text {String} Column Name
     * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu Disabled
    */
    Csw2.grids.columns.lift('actionColumn', function (sortable, text, menuDisabled){
        if(arguments.length === 0) {
            throw new Error('Cannot create a column without parameters');
        }

        var ret = new ActionColumn(text);
        ret.menuDisabled = menuDisabled;
        ret.sortable = sortable;

        return ret;
    });


    }());