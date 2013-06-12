/* global nameSpace:true, window:true, Ext:true */

(function _columnItemIIFE(nameSpace) {

    /**
     * Private column item class constructor
     * @param icon {String} Relative path to icon file
     * @param tooltip {String} Tooltip to display on hover
     * @param onGetClass {Function} [onGetClass] Method to call when getClass is called
     * @param onHandler {Function} Method to fire in response to action column click
    */
    var ColumnItem = function (icon, tooltip, onGetClass, onHandler) {
        'use strict';
        var that = this;
        nameSpace.property(that, 'icon', icon);
        nameSpace.property(that, 'tooltip', tooltip);
        nameSpace.property(that, 'getClass',
            /**
             * Get the CSS class for the supplied values
            */
            function getClass(value, metadata, record) {
                var store, index, ret = 'x-grid-center-icon';
                store = record.store;
                index = store.indexOf(record);
                if (onGetClass && onGetClass(index, store)) {
                    ret = 'x-action-icon-disabled';
                }
                return ret;
            });
        nameSpace.property(that, 'handler',
            /**
             * Generic handler
            */
            function handler(grid, rowIndex, colIndex) {
                if (onHandler) {
                    var args = nameSpace.getArguments(arguments);
                    onHandler.apply(this, args);
                }
            });

        return that;
    };

    nameSpace.instanceOf.lift('ColumnItem', ColumnItem);

    nameSpace.grids.columns.lift('columnItem',
        /**
         * Create a column item, usually for inclusion in an ActionColumn
         * @param icon {String} Relative path to icon file
         * @param tooltip {String} Tooltip to display on hover
         * @param onGetClass {Function} [onGetClass] Method to call when getClass is called
         * @param onHandler {Function} Method to fire in response to action column click
        */
        function columnItem(icon, tooltip, onGetClass, onHandler) {
            'use strict';
            if (arguments.length === 0 || !onHandler) {
                throw new Error('Cannot create a column item without parameters');
            }
            icon = icon || '';
            tooltip = tooltip || '';
            onGetClass = onGetClass || function () { };
            var ret = new ColumnItem(icon, tooltip, onGetClass, onHandler)
            return ret;
        });


}(window.$om$));