/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function _columnsIIFE(nameSpace) {

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
                        if (!(column instanceof nameSpace.instanceOf.Column)) {
                            throw new Error('Only columns can be added to the Columns collection');
                        }
                        that.value.push(column);
                        return that;
                    }
                }
            });
            return that;
        };

        nameSpace.instanceOf.lift('Columns', Columns);

        /**
         * A mechanism for generating columns
         */
        nameSpace.grids.columns.lift('columns', function () {
            var ret = new Columns();
            return ret;
        });

    //});

}(window.$om$));