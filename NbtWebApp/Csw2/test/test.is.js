/*global Csw2:true, QUnit:false, module:false, test:false, asyncTest:false, expect:false*/
/*global start:false, stop:false ok:false, equal:false, notEqual:false, deepEqual:false*/
/*global notDeepEqual:false, strictEqual:false, notStrictEqual:false, raises:false*/

// It's desirable that these tests not be optimized for pure functional programming,
// because they need to quickly communicate their intention to the reader.
// Unless testing abstraction and encapsulation, it is better to be dundant and verbose,
// as this will make troubleshooting failures easier for anyone who might come behind

// Test the truthiness of the null Type
(function _nullChecks() {
	module("null");
	test( "null is not of any Csw2 supported type", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(null), false, "null is not a String");
		deepEqual( Csw2.is.bool(null), false, "null is not a Boolean");
		deepEqual( Csw2.is.number(null), false, "null is not a Number");
		deepEqual( Csw2.is.numeric(null), false, "null is not numeric");
		deepEqual( Csw2.is.date(null), false, "null is not a Date");
		deepEqual( Csw2.is.func(null), false, "null is not a Function");
		deepEqual( Csw2.is.array(null), false, "null is not an Array");
		deepEqual( Csw2.is.plainObject(null), false, "null is not an Object");
	});
//}());
//
//// Test the truthiness of the undefined value of the undefined type, when undefined is explicitly undefined (Only required in < ES5 envs.)
//(function _undefinedChecks(undefined) {
	module("undefined");
	test( "undefined is not of any Csw2 supported type", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(undefined), false, "undefined is not a String");
		deepEqual( Csw2.is.bool(undefined), false, "undefined is not a Boolean");
		deepEqual( Csw2.is.number(undefined), false, "undefined is not a Number");
		deepEqual( Csw2.is.numeric(undefined), false, "undefined is not numeric");
		deepEqual( Csw2.is.date(undefined), false, "undefined is not a Date");
		deepEqual( Csw2.is.func(undefined), false, "undefined is not a Function");
		deepEqual( Csw2.is.array(undefined), false, "undefined is not an Array");
		deepEqual( Csw2.is.plainObject(undefined), false, "undefined is not an Object");
	});
//}());
//
//// Test the truthiness of implicitly pass undefined, by passing no arguments
//(function _emptyArgumentsChecks() {
	module("empty arguments");
	test( "Empty arguments are not of any Csw2 supported type", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(), false, "(An empty argument) is not a String");
		deepEqual( Csw2.is.bool(), false, "(An empty argument) is not a Boolean");
		deepEqual( Csw2.is.number(), false, "(An empty argument) is not a Number");
		deepEqual( Csw2.is.numeric(), false, "(An empty argument) is not numeric");
		deepEqual( Csw2.is.date(), false, "(An empty argument) is not a Date");
		deepEqual( Csw2.is.func(), false, "(An empty argument) is not a Function");
		deepEqual( Csw2.is.array(), false, "(An empty argument) is not an Array");
		deepEqual( Csw2.is.plainObject(), false, "(An empty argument) is not an Object");
	});
//}());
//
//// Test the truthiness of NaN
//(function _NaNChecks() {
	module("NaN");
	test( "NaN not of any Csw2 supported type, except Number", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(NaN), false, "NaN is not a String");
		deepEqual( Csw2.is.bool(NaN), false, "NaN is not a Boolean");
		deepEqual( Csw2.is.number(NaN), false, "NaN is not a Number");
		deepEqual( Csw2.is.numeric(NaN), false, "NaN is not numeric");
		deepEqual( Csw2.is.date(NaN), false, "NaN is not a Date");
		deepEqual( Csw2.is.func(NaN), false, "NaN is not a Function");
		deepEqual( Csw2.is.array(NaN), false, "NaN is not an Array");
		deepEqual( Csw2.is.plainObject(NaN), false, "NaN is not an Object");
	});
//}());
//
//// Test the truthiness of -Infinity
//(function _InfinityChecks() {
	module("-Infinity");
	test( "-Infinity not of any Csw2 supported type, except Number", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(-Infinity), false, "-Infinity is not a String");
		deepEqual( Csw2.is.bool(-Infinity), false, "-Infinity is not a Boolean");
		deepEqual( Csw2.is.number(-Infinity), false, "-Infinity is not a Number");
		deepEqual( Csw2.is.numeric(-Infinity), false, "-Infinity is not numeric");
		deepEqual( Csw2.is.date(-Infinity), false, "-Infinity is not a Date");
		deepEqual( Csw2.is.func(-Infinity), false, "-Infinity is not a Function");
		deepEqual( Csw2.is.array(-Infinity), false, "-Infinity is not an Array");
		deepEqual( Csw2.is.plainObject(-Infinity), false, "-Infinity is not an Object");
	});
//}());
//
//// Test the truthiness of ''
//(function _emptyStringChecks() {
	module("empty string");
	test( "'' is not of any Csw2 supported type, except String", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(''), true, "'' is (actually) a String!");
		deepEqual( Csw2.is.bool(''), false, "'' is not a Boolean");
		deepEqual( Csw2.is.number(''), false, "'' is not a Number");
		deepEqual( Csw2.is.numeric(''), true, "'' is binary 0 and is numeric");
		deepEqual( Csw2.is.date(''), false, "'' is not a Date");
		deepEqual( Csw2.is.func(''), false, "'' is not a Function");
		deepEqual( Csw2.is.array(''), false, "'' is not an Array");
		deepEqual( Csw2.is.plainObject(''), false, "'' is not an Object");
	});
//}());
//
//// Test the truthiness of String 'false'
//(function _stringFalseChecks() {
	module("string 'false'");
	test( "'false' is a String and only a String", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string('false'), true, "'false' is (actually) a String!");
		deepEqual( Csw2.is.bool('false'), false, "'false' is not a Boolean");
		deepEqual( Csw2.is.number('false'), false, "'false' is not a Number");
		deepEqual( Csw2.is.numeric('false'), true, "'false' is binary 0 is numeric");
		deepEqual( Csw2.is.date('false'), false, "'false' is not a Date");
		deepEqual( Csw2.is.func('false'), false, "'false' is not a Function");
		deepEqual( Csw2.is.array('false'), false, "'false' is not an Array");
		deepEqual( Csw2.is.plainObject('false'), false, "'false' is not an Object");
	});
//}());
//
//// Test the truthiness of String 'true'
//(function _stringTrueChecks() {
	module("string 'true'");
	test( "'true' is a String and only a String", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string('true'), true, "'true' is (actually) a String!");
		deepEqual( Csw2.is.bool('true'), false, "'true' is not a Boolean");
		deepEqual( Csw2.is.number('true'), false, "'true' is not a Number");
		deepEqual( Csw2.is.numeric('true'), true, "'true' is binary 1 and is numeric");
		deepEqual( Csw2.is.date('true'), false, "'true' is not a Date");
		deepEqual( Csw2.is.func('true'), false, "'true' is not a Function");
		deepEqual( Csw2.is.array('true'), false, "'true' is not an Array");
		deepEqual( Csw2.is.plainObject('true'), false, "'true' is not an Object");
	});
//}());
//
//// Test the truthiness of Boolean false
//(function _falseChecks() {
	module("boolean false");
	test( "false is a Boolean and only a Boolean", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(false), false, "false is not a String");
		deepEqual( Csw2.is.bool(false), true, "false is (actuall) a Boolean!");
		deepEqual( Csw2.is.number(false), false, "false is not a Number");
		deepEqual( Csw2.is.numeric(false), true, "false converts to 0 is numeric");
		deepEqual( Csw2.is.date(false), false, "false is not a Date");
		deepEqual( Csw2.is.func(false), false, "false is not a Function");
		deepEqual( Csw2.is.array(false), false, "false is not an Array");
		deepEqual( Csw2.is.plainObject(false), false, "false is not an Object");
	});
//}());
//
//// Test the truthiness of Boolean true
//(function _trueChecks() {
	module("boolean true");
	test( "true is a Boolean and only a Boolean", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(true), false, "true is not a String");
		deepEqual( Csw2.is.bool(true), true, "true is (actuall) a Boolean!");
		deepEqual( Csw2.is.number(true), false, "true is not a Number");
		deepEqual( Csw2.is.numeric(true), true, "true converts to 1 is numeric");
		deepEqual( Csw2.is.date(true), false, "true is not a Date");
		deepEqual( Csw2.is.func(true), false, "true is not a Function");
		deepEqual( Csw2.is.array(true), false, "true is not an Array");
		deepEqual( Csw2.is.plainObject(true), false, "true is not an Object");
	});
//}());
//
//// Test the truthiness of empty Array []
//(function _arrayChecks() {
	module("Array []");
	test( "[] is an Array", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string([]), false, "[] is not a String");
		deepEqual( Csw2.is.bool([]), false, "[] is not a Boolean");
		deepEqual( Csw2.is.number([]), false, "[] is not a Number");
		deepEqual( Csw2.is.numeric([]), false, "[] is not numeric");
		deepEqual( Csw2.is.date([]), false, "[] is not a Date");
		deepEqual( Csw2.is.func([]), false, "[] is not a Function");
		deepEqual( Csw2.is.array([]), true, "[] is (actually) an Array!");
		deepEqual( Csw2.is.plainObject([]), false, "[] is not an Object");
	});
//}());
//
//// Test the truthiness of empty Object  {}
//(function _objectChecks() {
	module("Object {}");
	test( "{} is an Object", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string({}), false, "{} is not a String");
		deepEqual( Csw2.is.bool({}), false, "{} is not a Boolean");
		deepEqual( Csw2.is.number({}), false, "{} is not a Number");
		deepEqual( Csw2.is.numeric({}), false, "{} is not numeric");
		deepEqual( Csw2.is.date({}), false, "{} is not a Date");
		deepEqual( Csw2.is.func({}), false, "{} is not a Function");
		deepEqual( Csw2.is.array({}), false, "{} is not an Array");
		deepEqual( Csw2.is.plainObject({}), true, "{} is (actually) an Object!");
	});
//}());
//
//// Test the truthiness of 'empty' Function function() {}
//(function _functionChecks() {
	module("Function() {}");
	test( "function() {} is a Function", function() {
		expect(8); //all 8 assertions must pass
		deepEqual( Csw2.is.string(function() {}), false, "function() {} is not a String");
		deepEqual( Csw2.is.bool(function() {}), false, "function() {} is not a Boolean");
		deepEqual( Csw2.is.number(function() {}), false, "function() {} is not a Number");
		deepEqual( Csw2.is.numeric(function() {}), false, "function() {} is not numeric");
		deepEqual( Csw2.is.date(function() {}), false, "function() {} is not a Date");
		deepEqual( Csw2.is.func(function() {}), true, "function() {} is (actually) a Function!");
		deepEqual( Csw2.is.array(function() {}), false, "function() {} is not an Array");
		deepEqual( Csw2.is.plainObject(function() {}), false, "function() {} is not an Object");
	});
//}());
//
//// String truthy checks
//(function _isString() {

	//#region Csw2.is.string

	module("Csw2.is.string");
	test( "Csw2.is.string('a')", function() {
		deepEqual( Csw2.is.string('a'), true, "'a' is _a_ string");
	});

	test( "Csw2.is.string('A')", function() {
		deepEqual( Csw2.is.string('A'), true, "'A' is _a_ string");
	});

	test( "Csw2.is.string('[]')", function() {
		deepEqual( Csw2.is.string('[]'), true, "'[]' is a string");
	});

	test( "Csw2.is.string('{}')", function() {
		deepEqual( Csw2.is.string('{}'), true, "'{}' is a string");
	});

	test( "Csw2.is.string('NaN')", function() {
		deepEqual( Csw2.is.string('NaN'), true, "'NaN' is a string");
	});

	test( "Csw2.is.string('0')", function() {
		deepEqual( Csw2.is.string('0'), true, "'0' is a string");
	});

	test( "Csw2.is.string('null')", function() {
		deepEqual( Csw2.is.string('null'), true, "'null' is a string");
	});

	test( "Csw2.is.string('undefined')", function() {
		deepEqual( Csw2.is.string('undefined'), true, "'undefined' is a string");
	});

	test( "Csw2.is.string(' ')", function() {
		deepEqual( Csw2.is.string(' '), true, "' ' is a string");
	});

	test( "Csw2.is.string(0)", function() {
		deepEqual( Csw2.is.string(0), false, "0 is not a string");
	});

	test( "Csw2.is.string(new Date())", function() {
		deepEqual( Csw2.is.string(new Date()), false, "new Date() is not a string");
	});

	test( "Csw2.is.string(String)", function() {
		deepEqual( Csw2.is.string(String), false, "String is not a string");
	});

	//#endregion Csw2.is.string

//}());
//
//// Boolean truthy checks
//(function _isBool() {

	//#region Csw2.is.bool

	module("Csw2.is.bool");

	test( "Csw2.is.bool(0)", function() {
		deepEqual( Csw2.is.bool(0), false, "0 is not a boolean value");
	});

	test( "Csw2.is.bool('0')", function() {
		deepEqual( Csw2.is.bool('0'), false, "'0' is not a boolean value");
	});

	test( "Csw2.is.bool(1)", function() {
		deepEqual( Csw2.is.bool(1), false, "1 is not a boolean value");
	});

	test( "Csw2.is.bool('1')", function() {
		deepEqual( Csw2.is.bool('1'), false, "'1' is not a boolean value");
	});

	//#endregion Csw2.is.bool

//}());
//
//// Number truthy checks
//(function _isNumber() {

	//#region Csw2.is.number

	module("Csw2.is.number");

	test( "Csw2.is.number(1)", function() {
		deepEqual( Csw2.is.number(1), true, "1 is a number");
	});

	test( "Csw2.is.number(-1)", function() {
		deepEqual( Csw2.is.number(-1), true, "-1 is a number");
	});

	test( "Csw2.is.number(0)", function() {
		deepEqual( Csw2.is.number(0), true, "0 is a number");
	});

	test( "Csw2.is.number(-0)", function() {
		deepEqual( Csw2.is.number(-0), true, "-0 is a number");
	});

	test( "Csw2.is.number(Infinity)", function() {
		deepEqual( Csw2.is.number(Infinity), false, "Infinity is not a number");
	});

	test( "Csw2.is.number(-Infinity)", function() {
		deepEqual( Csw2.is.number(-Infinity), false, "-Infinity is not a number");
	});

	test( "Csw2.is.number(0.000000000000000000000000001)", function() {
		deepEqual( Csw2.is.number(0.000000000000000000000000001), true, "0.000000000000000000000000001 is a number");
	});

	test( "Csw2.is.number(-0.000000000000000000000000001)", function() {
		deepEqual( Csw2.is.number(-0.000000000000000000000000001), true, "-0.000000000000000000000000001 is a number");
	});

	test( "Csw2.is.number(1e+20)", function() {
		deepEqual( Csw2.is.number(1e+20), true, "1e+20 is a number");
	});

	test( "Csw2.is.number(-1e+20)", function() {
		deepEqual( Csw2.is.number(1e+20), true, "1e+20 is a number");
	});

	test( "Csw2.is.number(0xA)", function() {
		deepEqual( Csw2.is.number(0xA), true, "0xA a number");
	});

	test( "Csw2.is.number(-0xA)", function() {
		deepEqual( Csw2.is.number(0xA), true, "0xA a number");
	});

	test( "Csw2.is.number('-')", function() {
		deepEqual( Csw2.is.number('-'), false, "'-' is not a number");
	});


	//#endregion Csw2.is.number

}());

