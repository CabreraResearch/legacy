/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _checkColumnIIFE(nameSpace) {

    /**
     * Internal check column class
     * @param text {String} Name of the column
    */
    var CheckColumn = function (text) {
        'use strict';
        var that = nameSpace.grids.columns.column({
            xtype: nameSpace.grids.constants.xtypes.checkcolumn,
            flex: 0.075,
            text: text
        });
        that.align = 'center';

        return that;
    };

    nameSpace.instanceOf.lift('CheckColumn', CheckColumn);

    nameSpace.grids.columns.lift('checkColumn',
            /**
         * Create a check column
         * @param sortable {Boolean} [sortable=true] Is Column Sortable
         * @param text {String} Column Name
         * @param menuDisabled {Boolean} [menuDisabled=false] Is Menu Disabled
        */
        function checkColumn(sortable, text, menuDisabled) {
            'use strict';
            if (arguments.length === 0) {
                throw new Error('Cannot create a column without parameters');
            }

            var ret = new CheckColumn(text);
            ret.menuDisabled = menuDisabled;
            ret.sortable = sortable;

            return ret;
        });


}(window.$om$));