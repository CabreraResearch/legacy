/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _actionColumnIIFE(nameSpace) {

    /**
     * Internal action column class
     * @param text {String} Name of the column
    */
    var ActionColumn = function (text) {
        'use strict';
        var that = nameSpace.grids.columns.column({
            xtype: nameSpace.grids.constants.xtypes.actioncolumn,
            width: 60,
            text: text
        });
        nameSpace.property(that, 'items', []);
        nameSpace.property(that, 'addItem', function (columnItem) {
            if (!(columnItem instanceof nameSpace.instanceOf.ColumnItem)) {
                throw new Error('Invalid column item specified for collection.');
            }
            that.items.push(columnItem);
            return that;
        }, false, false, false);


        return that;
    };

    nameSpace.instanceOf.lift('ActionColumn', ActionColumn);

    nameSpace.grids.columns.lift('actionColumn',
        /**
         * Create an action column
         * @param sortable {Boolean} [sortable=true] Is Column Sortable
         * @param text {String} Column Name
         * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu Disabled
        */
        function actionColumn(sortable, text, menuDisabled) {
            'use strict';
            if (arguments.length === 0) {
                throw new Error('Cannot create a column without parameters');
            }

            var ret = new ActionColumn(text);
            ret.menuDisabled = menuDisabled;
            ret.sortable = sortable;

            return ret;
        });


}(window.$om$));