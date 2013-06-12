/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _gridColumnIIFE(nameSpace) {

    /**
     * Private grid column class constructor
     * @param text {String} Column Name
     * @param editor {String} If column is editable, the type of editor
     * @param flex {Number} [flex=0.125] Relative width of the column
    */
    var GridColumn = function (text, editor, flex) {
        'use strict';
        var that = nameSpace.grids.columns.column({
            xtype: nameSpace.grids.constants.xtypes.gridcolumn,
            flex: flex || 0.125,
            editor: editor,
            text: text
        });

        return that;
    };

    nameSpace.instanceOf.lift('GridColumn', GridColumn);

    nameSpace.grids.columns.lift('gridColumn',
        /**
         * Create a grid column
         * @param sortable {Boolean} [sortable=true] Is Column Sortable
         * @param text {String} Column Name
         * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu Disabled
         * @param flex {Number} [flex=0.125] Relative width of the column
         * @param editor {String} If column is editable, the type of editor
        */
        function gridColumn(sortable, text, menuDisabled, flex, editor) {
            'use strict';
            if (arguments.length === 0) {
                throw new Error('Cannot create a column without parameters');
            }

            var ret = new GridColumn(text, editor, flex);
            ret.menuDisabled = menuDisabled;
            ret.sortable = sortable;
            return ret;
        });


}(window.$om$));