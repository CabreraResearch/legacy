/*global Csw:true, QUnit:false, module:false, test:false, asyncTest:false, expect:false*/
/*global start:false, stop:false ok:false, equal:false, notEqual:false, deepEqual:false*/
/*global notDeepEqual:false, strictEqual:false, notStrictEqual:false, raises:false*/

module("Namespace");
test("ChemSW namespace exists and has methods", function () {
    window.expect(2); //all 2 assertions must pass
    notDeepEqual(Csw, null, "ChemSW namespace is not null.");
    notDeepEqual(Csw, undefined, "ChemSW namespace is not undefined.");
});
    
