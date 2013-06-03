/*global Csw2:true, QUnit:false, module:false, test:false, asyncTest:false, expect:false*/
/*global start:false, stop:false ok:false, equal:false, notEqual:false, deepEqual:false*/
/*global notDeepEqual:false, strictEqual:false, notStrictEqual:false, raises:false*/

// It's desirable that these tests not be optimized for pure functional programming,
// because they need to quickly communicate their intention to the reader.
// Unless testing abstraction and encapsulation, it is better to be dundant and verbose,
// as this will make troubleshooting failures easier for anyone who might come behind

// String conversion checks
(function _toString() {

	//#region Csw2.is.string
	module("Csw2.to.string");
	test( "Csw2.to.string(null)", function() {
        deepEqual( Csw2.to.string(null) === '', true, "Csw2.to.string converts null to string empty.");
	});

    test( "Csw2.to.string(null, 'a')", function() {
        deepEqual( Csw2.to.string(null, 'a') === 'a', true, "Csw2.to.string converts null to 'a'.");
	});

	test( "Csw2.to.string(undefined)", function() {
        deepEqual( Csw2.to.string(undefined) === '', true, "Csw2.to.string converts undefined to string empty.");
	});

	test( "Csw2.to.string(undefined, 'a')", function() {
        deepEqual( Csw2.to.string(undefined, 'a') === 'a', true, "Csw2.to.string converts undefined to 'a'.");
	});

	test( "Csw2.to.string(NaN)", function() {
        deepEqual( Csw2.to.string(NaN) === '', true, "Csw2.to.string converts NaN to ''.");
	});

	test( "Csw2.to.string(Infinity)", function() {
        deepEqual( Csw2.to.string(Infinity) === '', true, "Csw2.to.string converts Infinity to ''.");
	});

	test( "Csw2.to.string({})", function() {
        deepEqual( Csw2.to.string({}) === '', true, "Csw2.to.string converts {} to string empty.");
	});

	test( "Csw2.to.string({a: 'a', 1: '1', x: false, y: []})", function() {
        deepEqual( Csw2.to.string({a: 'a', 1: '1', x: false, y: []}) === '', true, "Csw2.to.string converts {a: 'a', 1: '1', x: false, y: []} to string empty.");
	});

	test( "Csw2.to.string({}, 'a')", function() {
        deepEqual( Csw2.to.string({}, 'a') === 'a', true, "Csw2.to.string converts {} to 'a'.");
	});

	test( "Csw2.to.string([])", function() {
        deepEqual( Csw2.to.string([]) === '', true, "Csw2.to.string converts [] to string empty.");
	});

	test( "Csw2.to.string([1, '1', false, {}, []])", function() {
        deepEqual( Csw2.to.string([1, '1', false, {}, []]) === '', true, "Csw2.to.string converts [1, '1', false, {}, []] to string empty.");
	});

	test( "Csw2.to.string([], 'a')", function() {
        deepEqual( Csw2.to.string([], 'a') === 'a', true, "Csw2.to.string converts [] to 'a'.");
	});

	test( "Csw2.to.string(1, 'a')", function() {
        deepEqual( Csw2.to.string(1, 'a') === '1', true, "Csw2.to.string converts 1 to '1'.");
	});

	test( "Csw2.to.string(0, 'a')", function() {
        deepEqual( Csw2.to.string(0, 'a') === '0', true, "Csw2.to.string converts 0 to '0'.");
	});

	test( "Csw2.to.string(true, 'a')", function() {
        deepEqual( Csw2.to.string(true, 'a') === 'true', true, "Csw2.to.string converts true to 'true'.");
	});

	test( "Csw2.to.string(false, 'a')", function() {
        deepEqual( Csw2.to.string(false, 'a') === 'false', true, "Csw2.to.string converts false to 'false'.");
	});


	//#endregion Csw2.is.string

}());

// Boolean conversion checks
(function _isBool() {

	//#region Csw2.to.bool

	module("Csw2.to.bool");

	test( "Csw2.to.bool(null)", function() {
        deepEqual( Csw2.to.bool(null), false, "Csw2.to.bool converts null to false.");
	});

	test( "Csw2.to.bool(undefined)", function() {
        deepEqual( Csw2.to.bool(undefined), false, "Csw2.to.bool converts undefined to false.");
	});

	test( "Csw2.to.bool(NaN)", function() {
        deepEqual( Csw2.to.bool(NaN), false, "Csw2.to.bool converts NaN to false.");
	});

	test( "Csw2.to.bool(Infinity)", function() {
        deepEqual( Csw2.to.bool(Infinity), false, "Csw2.to.bool converts Infinity to false.");
	});

	test( "Csw2.to.bool(-Infinity)", function() {
        deepEqual( Csw2.to.bool(-Infinity), false, "Csw2.to.bool converts -Infinity to false.");
	});

	test( "Csw2.to.bool({})", function() {
        deepEqual( Csw2.to.bool({}), false, "Csw2.to.bool converts {} to false.");
	});

	test( "Csw2.to.bool([])", function() {
        deepEqual( Csw2.to.bool([]), false, "Csw2.to.bool converts [] to false.");
	});

	test( "Csw2.to.bool(new Date())", function() {
        deepEqual( Csw2.to.bool(new Date()), false, "Csw2.to.bool converts new Date() to false.");
	});

	test( "Csw2.to.bool(5)", function() {
        deepEqual( Csw2.to.bool(5), false, "Csw2.to.bool converts 5 to false.");
	});

	test( "Csw2.to.bool(0)", function() {
        deepEqual( Csw2.to.bool(0), false, "Csw2.to.bool converts 0 to false.");
	});

	test( "Csw2.to.bool('0')", function() {
        deepEqual( Csw2.to.bool('0'), false, "Csw2.to.bool converts '0' to false.");
	});

	test( "Csw2.to.bool('false')", function() {
        deepEqual( Csw2.to.bool('false'), false, "Csw2.to.bool converts 'false' to false.");
	});

	test( "Csw2.to.bool(false)", function() {
        deepEqual( Csw2.to.bool(false), false, "Csw2.to.bool converts false to false.");
	});

	test( "Csw2.to.bool(1)", function() {
        deepEqual( Csw2.to.bool(1), true, "Csw2.to.bool converts 1 to true.");
	});

	test( "Csw2.to.bool('1')", function() {
        deepEqual( Csw2.to.bool('1'), true, "Csw2.to.bool converts '1' to true.");
	});

	test( "Csw2.to.bool('true')", function() {
        deepEqual( Csw2.to.bool('true'), true, "Csw2.to.bool converts 'true' to true.");
	});

	test( "Csw2.to.bool(true)", function() {
        deepEqual( Csw2.to.bool(true), true, "Csw2.to.bool converts true to true.");
	});

	//#endregion Csw2.to.bool

}());

// Number conversion checks
(function _isNumber() {

	//#region Csw2.to.number

	module("Csw2.to.number");

	test( "Csw2.to.number(null)", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number(null)), true, "Csw2.to.number converts null to NaN.");
	});

	test( "Csw2.to.number(undefined)", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number(undefined)), true, "Csw2.to.number converts undefined to NaN.");
	});

	test( "Csw2.to.number(NaN)", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number(NaN)), true, "Csw2.to.number converts NaN to NaN.");
	});

	test( "Csw2.to.number(Infinity)", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number(Infinity)), true, "Csw2.to.number converts Infinity to NaN.");
	});

	test( "Csw2.to.number(-Infinity)", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number(-Infinity)), true, "Csw2.to.number converts -Infinity to NaN.");
	});

	test( "Csw2.to.number({})", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number({})), true, "Csw2.to.number converts {} to NaN.");
	});

	test( "Csw2.to.number([])", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number([])), true, "Csw2.to.number converts [] to NaN.");
	});

	test( "Csw2.to.number(new Date())", function() {
        deepEqual( Csw2.number.isNaN(Csw2.to.number(new Date())), true, "Csw2.to.number converts new Date() to NaN.");
	});

	test( "Csw2.to.number(0)", function() {
        deepEqual( Csw2.to.number(0) === 0, true, "Csw2.to.number converts 0 to 0.");
	});

	test( "Csw2.to.number('0')", function() {
        deepEqual( Csw2.to.number('0') === 0, true, "Csw2.to.number converts '0' to 0.");
	});

	test( "Csw2.to.number('false')", function() {
        deepEqual( Csw2.to.number('false') === 0, true, "Csw2.to.number converts 'false' to 0.");
	});

	test( "Csw2.to.number(false)", function() {
        deepEqual( Csw2.to.number(false) === 0, true, "Csw2.to.number converts false to 0.");
	});

	test( "Csw2.to.number(1)", function() {
        deepEqual( Csw2.to.number(1) === 1, true, "Csw2.to.number converts 1 to 1.");
	});

	test( "Csw2.to.number('1')", function() {
        deepEqual( Csw2.to.number('1') === 1, true, "Csw2.to.number converts '1' to 1.");
	});

	test( "Csw2.to.number('true')", function() {
        deepEqual( Csw2.to.number('true') === 1, true, "Csw2.to.number converts 'true' to 1.");
	});

	test( "Csw2.to.number(true)", function() {
        deepEqual( Csw2.to.number(true) === 1, true, "Csw2.to.bool converts true to 1.");
	});

	test( "Csw2.to.number('42')", function() {
        deepEqual( Csw2.to.number('42') === 42, true, "Csw2.to.number converts '42' to 42.");
	});

	test( "Csw2.to.number('-42')", function() {
        deepEqual( Csw2.to.number('-42') === -42, true, "Csw2.to.number converts '-42' to -42.");
	});



	//#endregion Csw2.is.number

}());

