/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _columnsIIFE() {

    //Csw2.dependsOn(['Csw2.models.field'], function () {

        /**
         * Defines a collection of columns
         */
        var Columns = function() {
            var that = this;
            Object.defineProperties(that, {
                value: {
                    value: [],
                    writable: true,
                    configurable: true,
                    enumerable: true
                },
                add: {
                    value: function (column) {
                        if (!(column instanceof Csw2.instanceOf.Column)) {
                            throw new Error('Only columns can be added to the Columns collection');
                        }
                        that.value.push(column);
                        return that;
                    }
                }
            });
            return that;
        };

        Csw2.instanceOf.lift('Columns', Columns);

        /**
         * A mechanism for generating columns
         */
        Csw2.grids.columns.lift('columns', function() {
            var ret = new Columns();
            return ret;
        });

    //});

}());