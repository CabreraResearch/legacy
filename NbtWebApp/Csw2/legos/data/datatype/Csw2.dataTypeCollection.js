/* jshint undef: true, unused: true */
/* global window:true, Ext:true, $: true */

(function _dataTypeCollectionIIFE(nameSpace) {

    /**
     * Defines a collection of data types
     * @constructor
     * @internal
     */
    var DataTypeCollection = function () {
        var that = this;
        /**
        * Get the value of the data type collection
        */
        nameSpace.property(that, 'value', []);

        nameSpace.property(that, 'add',
            /**
             * Add a validated data type to the collection
            */
            function (dataType) {
                if (!(dataType instanceof nameSpace.instanceOf.DataType)) {
                    throw new Error('Only fields can be added to the Fields collection');
                }
                that.value.push(dataType);
                return that;
            });
        return that;
    };

    nameSpace.instanceOf.lift('DataTypeCollection', DataTypeCollection);

    /**
     * A mechanism for generating data type
     */
    nameSpace.dataTypes.lift('collection', function () {
        var ret = new DataTypeCollection();
        return ret;
    });

}(window.$om$));