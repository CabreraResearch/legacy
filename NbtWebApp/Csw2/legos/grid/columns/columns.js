/* jshint undef: true, unused: true */
/* global nameSpace:true, window:true, Ext:true, $: true */

(function _columnsIIFE(nameSpace) {

    //nameSpace.dependsOn(['nameSpace.models.field'], function () {

    /**
     * Defines a collection of columns
     */
    var Columns = function () {
        'use strict';

        var that = this;
        nameSpace.property(that, 'value', []);
        nameSpace.property(that, 'add',
            /**
             * Add a column to the collection
            */
            function add(column) {
                if (!(column instanceof nameSpace.instanceOf.Column)) {
                    throw new Error('Only columns can be added to the Columns collection');
                }
                that.value.push(column);
                return that;
            });
        return that;
    };

    nameSpace.instanceOf.lift('Columns', Columns);

    nameSpace.grids.columns.lift('columns',
        /**
         * A mechanism for generating columns
         */
        function columns() {
            'use strict';
            var ret = new Columns();
            return ret;
        });

    //});

}(window.$om$));